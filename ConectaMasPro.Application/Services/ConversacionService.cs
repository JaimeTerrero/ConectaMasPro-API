using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.DTOs.Conversaciones;
using ConectaMasPro.Application.Interfaces.Repositories;
using ConectaMasPro.Application.Interfaces.Services;
using ConectaMasPro.Domain.Entities;

namespace ConectaMasPro.Application.Services;

public class ConversacionService : IConversacionService
{
    private readonly IUnitOfWork _unitOfWork;

    public ConversacionService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<Guid>> CrearAsync(
        CrearConversacionRequest request,
        Guid usuarioId,
        CancellationToken cancellationToken = default)
    {
        var solicitudes = await _unitOfWork.Repository<SolicitudServicio>().GetAsync(
            x => x.Id == request.SolicitudServicioId && x.Activo,
            null,
            cancellationToken,
            x => x.Servicio);
        var solicitud = solicitudes.FirstOrDefault();

        if (solicitud is null)
            return Result<Guid>.Fail("La solicitud de servicio no existe.");

        var proveedorId = solicitud.Servicio.UsuarioId;
        if (usuarioId != solicitud.ClienteId && usuarioId != proveedorId)
            return Result<Guid>.Fail("No tienes acceso a esta solicitud de servicio.");

        var existentes = await _unitOfWork.Repository<Conversacion>().FindAsync(
            x => x.SolicitudServicioId == request.SolicitudServicioId && x.Activo,
            cancellationToken);
        var existente = existentes.FirstOrDefault();
        if (existente is not null)
            return Result<Guid>.Ok(existente.Id, "La conversación ya existe.");

        var conversacion = new Conversacion
        {
            SolicitudServicioId = solicitud.Id,
            UsuarioUnoId = solicitud.ClienteId,
            UsuarioDosId = proveedorId
        };

        await _unitOfWork.Repository<Conversacion>().AddAsync(conversacion, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Ok(conversacion.Id, "Conversación creada correctamente.");
    }

    public async Task<Result<MensajeDto>> EnviarMensajeAsync(
        EnviarMensajeRequest request,
        Guid emisorId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Contenido))
            return Result<MensajeDto>.Fail("El mensaje no puede estar vacío.");
        if (request.Contenido.Trim().Length > 3000)
            return Result<MensajeDto>.Fail("El mensaje no puede exceder 3000 caracteres.");

        var conversaciones = await _unitOfWork.Repository<Conversacion>().GetAsync(
            x => x.Id == request.ConversacionId && x.Activo,
            null,
            cancellationToken,
            x => x.UsuarioUno,
            x => x.UsuarioDos);
        var conversacion = conversaciones.FirstOrDefault();

        if (conversacion is null)
            return Result<MensajeDto>.Fail("La conversación no existe.");
        if (conversacion.UsuarioUnoId != emisorId && conversacion.UsuarioDosId != emisorId)
            return Result<MensajeDto>.Fail("No perteneces a esta conversación.");

        var emisor = conversacion.UsuarioUnoId == emisorId
            ? conversacion.UsuarioUno
            : conversacion.UsuarioDos;
        var mensaje = new Mensaje
        {
            ConversacionId = conversacion.Id,
            EmisorId = emisorId,
            Contenido = request.Contenido.Trim(),
            Leido = false
        };

        await _unitOfWork.Repository<Mensaje>().AddAsync(mensaje, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<MensajeDto>.Ok(MapMensaje(mensaje, emisor), "Mensaje enviado correctamente.");
    }

    public async Task<Result<IReadOnlyList<ConversacionDto>>> ObtenerConversacionesAsync(
        Guid usuarioId,
        CancellationToken cancellationToken = default)
    {
        var conversaciones = await _unitOfWork.Repository<Conversacion>().GetWithQueryAsync(
            x => x.Activo && (x.SolicitudServicioId.HasValue || x.ActividadSocialId.HasValue) &&
                 (x.UsuarioUnoId == usuarioId || x.UsuarioDosId == usuarioId),
            q => q.OrderByDescending(x => x.Mensajes.Max(m => (DateTime?)m.FechaCreacion) ?? x.FechaCreacion),
            null,
            cancellationToken,
            x => x.UsuarioUno,
            x => x.UsuarioDos,
            x => x.SolicitudServicio!.Servicio,
            x => x.ActividadSocial!,
            x => x.Mensajes);

        var data = conversaciones.Select(x =>
        {
            var otro = x.UsuarioUnoId == usuarioId ? x.UsuarioDos : x.UsuarioUno;
            var ultimo = x.Mensajes.OrderByDescending(m => m.FechaCreacion).FirstOrDefault();
            return new ConversacionDto
            {
                Id = x.Id,
                SolicitudServicioId = x.SolicitudServicioId,
                ActividadSocialId = x.ActividadSocialId,
                Tipo = x.ActividadSocialId.HasValue ? "ActividadSocial" : "Servicio",
                OtroUsuarioId = otro.Id,
                OtroUsuarioNombre = $"{otro.Nombre} {otro.Apellido}".Trim(),
                ServicioTitulo = x.SolicitudServicio?.Servicio.Titulo,
                ActividadSocialTitulo = x.ActividadSocial?.Titulo,
                UltimoMensaje = ultimo is null ? null : MapMensaje(
                    ultimo,
                    ultimo.EmisorId == x.UsuarioUnoId ? x.UsuarioUno : x.UsuarioDos),
                MensajesNoLeidos = x.Mensajes.Count(m => !m.Leido && m.EmisorId != usuarioId)
            };
        }).ToList();

        return Result<IReadOnlyList<ConversacionDto>>.Ok(
            data,
            "Conversaciones obtenidas correctamente.");
    }

    public async Task<Result<IReadOnlyList<MensajeDto>>> ObtenerMensajesAsync(
         Guid conversacionId,
         Guid usuarioId,
         CancellationToken cancellationToken = default)
    {
        if (!await EsParticipanteAsync(
            conversacionId,
            usuarioId,
            cancellationToken))
        {
            return Result<IReadOnlyList<MensajeDto>>.Fail(
                "No perteneces a esta conversación.");
        }

        var mensajes = await _unitOfWork
            .Repository<Mensaje>()
            .GetTrackedAsync(
                x =>
                    x.ConversacionId == conversacionId &&
                    x.Activo,
                q => q.OrderBy(x => x.FechaCreacion),
                cancellationToken,
                x => x.Emisor);

        var noLeidos = mensajes
            .Where(x =>
                !x.Leido &&
                x.EmisorId != usuarioId)
            .ToList();

        foreach (var mensaje in noLeidos)
        {
            mensaje.Leido = true;
            mensaje.FechaModificacion = DateTime.UtcNow;
        }

        if (noLeidos.Count > 0)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<IReadOnlyList<MensajeDto>>.Ok(
            mensajes
                .Select(x => MapMensaje(x, x.Emisor))
                .ToList(),
            "Mensajes obtenidos correctamente.");
    }

    public async Task<bool> EsParticipanteAsync(
        Guid conversacionId,
        Guid usuarioId,
        CancellationToken cancellationToken = default)
    {
        var conversaciones = await _unitOfWork.Repository<Conversacion>().FindAsync(
            x => x.Id == conversacionId && x.Activo &&
                 (x.UsuarioUnoId == usuarioId || x.UsuarioDosId == usuarioId),
            cancellationToken);
        return conversaciones.Count > 0;
    }

    private static MensajeDto MapMensaje(Mensaje mensaje, Usuario emisor) => new()
    {
        Id = mensaje.Id,
        ConversacionId = mensaje.ConversacionId,
        EmisorId = mensaje.EmisorId,
        EmisorNombre = $"{emisor.Nombre} {emisor.Apellido}".Trim(),
        Contenido = mensaje.Contenido,
        Leido = mensaje.Leido,
        FechaCreacion = mensaje.FechaCreacion
    };
}
