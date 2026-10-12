using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.Common.Pagination;
using ConectaMasPro.Application.DTOs.ActividadesSociales;
using ConectaMasPro.Application.Interfaces.Repositories;
using ConectaMasPro.Application.Interfaces.Services;
using ConectaMasPro.Domain.Entities;
using ConectaMasPro.Domain.Enums;
using System.Linq.Expressions;

namespace ConectaMasPro.Application.Services;

public class ActividadSocialService : IActividadSocialService
{
    private readonly IUnitOfWork _unitOfWork;

    public ActividadSocialService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<Guid>> CrearAsync(Guid usuarioId, CrearActividadSocialRequest request, CancellationToken ct = default)
    {
        var error = ValidarCreacion(request);
        if (error is not null) return Result<Guid>.Fail(error);
        if (!await EsUsuarioSocialValidoAsync(usuarioId, ct))
            return Result<Guid>.Fail("Solo un cliente, proveedor o perfil mixto activo puede crear actividades.");
        var categoria = await _unitOfWork.Repository<CategoriaActividadSocial>()
            .GetByIdAsync(request.CategoriaActividadSocialId, ct);
        if (categoria is null || !categoria.Activo)
            return Result<Guid>.Fail("La categoría de actividad social no existe o está inactiva.");

        var actividad = new ActividadSocial
        {
            CreadorId = usuarioId,
            Titulo = request.Titulo.Trim(),
            Descripcion = request.Descripcion.Trim(),
            CategoriaActividadSocialId = request.CategoriaActividadSocialId,
            FechaActividad = request.FechaActividad.ToUniversalTime(),
            Ubicacion = request.Ubicacion.Trim(),
            CantidadPersonasBuscadas = request.CantidadPersonasBuscadas,
            Modalidad = request.Modalidad,
            MontoOfrecido = request.Modalidad == ModalidadActividadSocial.Remunerada ? request.MontoOfrecido : null,
            Requisitos = LimpiarOpcional(request.Requisitos)
        };

        await _unitOfWork.Repository<ActividadSocial>().AddAsync(actividad, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<Guid>.Ok(actividad.Id, "Actividad social publicada correctamente.");
    }

    public async Task<Result<PagedResponse<IReadOnlyList<ActividadSocialDto>>>> ExplorarAsync(
        Guid usuarioId, ActividadSocialFiltroRequest filtro, CancellationToken ct = default)
    {
        var busqueda = LimpiarOpcional(filtro.Busqueda);
        Expression<Func<ActividadSocial, bool>> predicate = x =>
                 x.Activo && x.FechaActividad >= DateTime.UtcNow &&
                 (x.Estado == EstadoActividadSocial.Abierta || x.Estado == EstadoActividadSocial.Completa) &&
                 (busqueda == null || x.Titulo.Contains(busqueda) || x.Descripcion.Contains(busqueda)) &&
                 (!filtro.CategoriaActividadSocialId.HasValue ||
                    x.CategoriaActividadSocialId == filtro.CategoriaActividadSocialId.Value) &&
                 (!filtro.Modalidad.HasValue || x.Modalidad == filtro.Modalidad.Value);
        var total = await _unitOfWork.Repository<ActividadSocial>().CountAsync(predicate, ct);
        var actividades = await _unitOfWork.Repository<ActividadSocial>().GetWithQueryAsync(
            predicate, q => q.OrderBy(x => x.FechaActividad),
            q => q.Skip((filtro.Page - 1) * filtro.PageSize).Take(filtro.PageSize), ct,
            x => x.Creador, x => x.CategoriaActividadSocial, x => x.Postulaciones);

        var data = actividades.Select(x => MapActividad(x, usuarioId)).ToList();
        return Result<PagedResponse<IReadOnlyList<ActividadSocialDto>>>.Ok(
            PagedResponse<IReadOnlyList<ActividadSocialDto>>.Create(data, filtro.Page, filtro.PageSize, total),
            "Actividades sociales obtenidas correctamente.");
    }

    public async Task<Result<ActividadSocialDto>> ObtenerDetalleAsync(Guid actividadId, Guid usuarioId, CancellationToken ct = default)
    {
        var actividad = (await _unitOfWork.Repository<ActividadSocial>().GetAsync(
            x => x.Id == actividadId && x.Activo, null, ct,
            x => x.Creador, x => x.CategoriaActividadSocial, x => x.Postulaciones)).FirstOrDefault();
        return actividad is null
            ? Result<ActividadSocialDto>.Fail("La actividad social no existe.")
            : Result<ActividadSocialDto>.Ok(MapActividad(actividad, usuarioId), "Actividad social obtenida correctamente.");
    }

    public async Task<Result<IReadOnlyList<ActividadSocialDto>>> ObtenerMisActividadesAsync(Guid usuarioId, CancellationToken ct = default)
    {
        var actividades = await _unitOfWork.Repository<ActividadSocial>().GetAsync(
            x => x.CreadorId == usuarioId && x.Activo, q => q.OrderByDescending(x => x.FechaCreacion), ct,
            x => x.Creador, x => x.CategoriaActividadSocial, x => x.Postulaciones);
        return Result<IReadOnlyList<ActividadSocialDto>>.Ok(actividades.Select(x => MapActividad(x, usuarioId)).ToList());
    }

    public async Task<Result<Guid>> PostularseAsync(Guid actividadId, Guid usuarioId, CrearPostulacionActividadRequest request, CancellationToken ct = default)
    {
        if (request.Mensaje?.Trim().Length > 1000)
            return Result<Guid>.Fail("El mensaje no puede exceder 1000 caracteres.");
        if (!await EsUsuarioSocialValidoAsync(usuarioId, ct))
            return Result<Guid>.Fail("Solo un cliente, proveedor o perfil mixto activo puede postularse.");

        var actividad = await _unitOfWork.Repository<ActividadSocial>().GetByIdAsync(actividadId, ct);
        if (actividad is null || !actividad.Activo) return Result<Guid>.Fail("La actividad social no existe.");
        if (actividad.CreadorId == usuarioId) return Result<Guid>.Fail("No puedes postularte a tu propia actividad.");
        if (actividad.Estado != EstadoActividadSocial.Abierta) return Result<Guid>.Fail("La actividad no está abierta a postulaciones.");
        if (actividad.FechaActividad <= DateTime.UtcNow) return Result<Guid>.Fail("La fecha de la actividad ya pasó.");
        if ((await _unitOfWork.Repository<PostulacionActividadSocial>().FindAsync(
            x => x.ActividadSocialId == actividadId && x.PostulanteId == usuarioId, ct)).Count > 0)
            return Result<Guid>.Fail("Ya te postulaste a esta actividad.");

        var postulacion = new PostulacionActividadSocial
        {
            ActividadSocialId = actividadId,
            PostulanteId = usuarioId,
            Mensaje = LimpiarOpcional(request.Mensaje)
        };
        await _unitOfWork.Repository<PostulacionActividadSocial>().AddAsync(postulacion, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<Guid>.Ok(postulacion.Id, "Postulación enviada correctamente.");
    }

    public async Task<Result<IReadOnlyList<PostulacionActividadDto>>> ObtenerPostulacionesAsync(Guid actividadId, Guid usuarioId, CancellationToken ct = default)
    {
        var actividad = await _unitOfWork.Repository<ActividadSocial>().GetByIdAsync(actividadId, ct);
        if (actividad is null || !actividad.Activo) return Result<IReadOnlyList<PostulacionActividadDto>>.Fail("La actividad social no existe.");
        if (actividad.CreadorId != usuarioId) return Result<IReadOnlyList<PostulacionActividadDto>>.Fail("Solo el creador puede ver las postulaciones.");
        var postulaciones = await _unitOfWork.Repository<PostulacionActividadSocial>().GetAsync(
            x => x.ActividadSocialId == actividadId && x.Activo, q => q.OrderByDescending(x => x.FechaCreacion), ct,
            x => x.Postulante, x => x.ActividadSocial);
        return Result<IReadOnlyList<PostulacionActividadDto>>.Ok(postulaciones.Select(MapPostulacion).ToList());
    }

    public async Task<Result<IReadOnlyList<PostulacionActividadDto>>> ObtenerMisPostulacionesAsync(Guid usuarioId, CancellationToken ct = default)
    {
        var postulaciones = await _unitOfWork.Repository<PostulacionActividadSocial>().GetAsync(
            x => x.PostulanteId == usuarioId && x.Activo, q => q.OrderByDescending(x => x.FechaCreacion), ct,
            x => x.Postulante, x => x.ActividadSocial);
        return Result<IReadOnlyList<PostulacionActividadDto>>.Ok(postulaciones.Select(MapPostulacion).ToList());
    }

    public async Task<Result<DecisionPostulacionDto>> DecidirPostulacionAsync(
        Guid postulacionId, Guid usuarioId, DecidirPostulacionRequest request, CancellationToken ct = default)
    {
        if (request.Estado is not (EstadoPostulacionActividad.Aceptada or EstadoPostulacionActividad.Rechazada))
            return Result<DecisionPostulacionDto>.Fail("La decisión debe ser Aceptada o Rechazada.");
        var postulacion = (await _unitOfWork.Repository<PostulacionActividadSocial>().GetTrackedAsync(
            x => x.Id == postulacionId && x.Activo, null, ct,
            x => x.Postulante, x => x.ActividadSocial, x => x.ActividadSocial.Postulaciones)).FirstOrDefault();
        if (postulacion is null) return Result<DecisionPostulacionDto>.Fail("La postulación no existe.");
        var actividad = postulacion.ActividadSocial;
        if (actividad.CreadorId != usuarioId) return Result<DecisionPostulacionDto>.Fail("Solo el creador puede decidir esta postulación.");
        if (postulacion.Estado != EstadoPostulacionActividad.Pendiente) return Result<DecisionPostulacionDto>.Fail("Esta postulación ya fue decidida.");
        if (actividad.Estado is not (EstadoActividadSocial.Abierta or EstadoActividadSocial.Completa))
            return Result<DecisionPostulacionDto>.Fail("La actividad ya no admite decisiones.");

        Guid? conversacionId = null;
        if (request.Estado == EstadoPostulacionActividad.Aceptada)
        {
            var aceptadas = actividad.Postulaciones.Count(x => x.Activo && x.Estado == EstadoPostulacionActividad.Aceptada);
            if (aceptadas >= actividad.CantidadPersonasBuscadas)
                return Result<DecisionPostulacionDto>.Fail("La actividad ya alcanzó el cupo de personas aceptadas.");
            var conversacion = new Conversacion
            {
                ActividadSocialId = actividad.Id,
                UsuarioUnoId = actividad.CreadorId,
                UsuarioDosId = postulacion.PostulanteId
            };
            await _unitOfWork.Repository<Conversacion>().AddAsync(conversacion, ct);
            await _unitOfWork.Repository<Mensaje>().AddAsync(new Mensaje
            {
                ConversacionId = conversacion.Id,
                EmisorId = actividad.CreadorId,
                Contenido = $"Tu postulación a «{actividad.Titulo}» fue aceptada. Ya pueden coordinar los detalles por aquí."
            }, ct);
            conversacionId = conversacion.Id;
            if (aceptadas + 1 >= actividad.CantidadPersonasBuscadas)
                actividad.Estado = EstadoActividadSocial.Completa;
        }

        postulacion.Estado = request.Estado;
        postulacion.FechaDecision = DateTime.UtcNow;
        postulacion.FechaModificacion = DateTime.UtcNow;
        actividad.FechaModificacion = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<DecisionPostulacionDto>.Ok(new DecisionPostulacionDto
        {
            PostulacionId = postulacion.Id,
            Estado = postulacion.Estado,
            PostulanteId = postulacion.PostulanteId,
            ConversacionId = conversacionId
        }, request.Estado == EstadoPostulacionActividad.Aceptada ? "Postulación aceptada." : "Postulación rechazada.");
    }

    public async Task<Result> RetirarPostulacionAsync(Guid postulacionId, Guid usuarioId, CancellationToken ct = default)
    {
        var postulacion = (await _unitOfWork.Repository<PostulacionActividadSocial>().GetTrackedAsync(
            x => x.Id == postulacionId && x.Activo, null, ct)).FirstOrDefault();
        if (postulacion is null) return Result.Fail("La postulación no existe.");
        if (postulacion.PostulanteId != usuarioId) return Result.Fail("Solo el postulante puede retirarla.");
        if (postulacion.Estado != EstadoPostulacionActividad.Pendiente)
            return Result.Fail("Solo se puede retirar una postulación pendiente.");
        postulacion.Estado = EstadoPostulacionActividad.Retirada;
        postulacion.FechaModificacion = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(ct);
        return Result.Ok("Postulación retirada correctamente.");
    }

    public async Task<Result> CambiarEstadoAsync(Guid actividadId, Guid usuarioId, CambiarEstadoActividadRequest request, CancellationToken ct = default)
    {
        var actividad = await _unitOfWork.Repository<ActividadSocial>().GetByIdAsync(actividadId, ct);
        if (actividad is null || !actividad.Activo) return Result.Fail("La actividad social no existe.");
        if (actividad.CreadorId != usuarioId) return Result.Fail("Solo el creador puede cambiar el estado.");
        var valido = request.Estado == EstadoActividadSocial.Cancelada && actividad.Estado is EstadoActividadSocial.Abierta or EstadoActividadSocial.Completa
            || request.Estado == EstadoActividadSocial.EnCurso && actividad.Estado is EstadoActividadSocial.Abierta or EstadoActividadSocial.Completa
            || request.Estado == EstadoActividadSocial.Finalizada && actividad.Estado == EstadoActividadSocial.EnCurso;
        if (!valido) return Result.Fail("La transición de estado solicitada no es válida.");
        actividad.Estado = request.Estado;
        actividad.FechaModificacion = DateTime.UtcNow;
        _unitOfWork.Repository<ActividadSocial>().Update(actividad);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result.Ok("Estado de la actividad actualizado correctamente.");
    }

    private async Task<bool> EsUsuarioSocialValidoAsync(Guid usuarioId, CancellationToken ct)
    {
        var usuarios = await _unitOfWork.Repository<Usuario>().FindAsync(x => x.Id == usuarioId && x.Activo, ct);
        return usuarios.FirstOrDefault()?.TipoUsuario is TipoUsuario.Cliente or TipoUsuario.Proveedor or TipoUsuario.Ambos;
    }

    private static string? ValidarCreacion(CrearActividadSocialRequest x)
    {
        if (string.IsNullOrWhiteSpace(x.Titulo) || x.Titulo.Trim().Length > 150) return "El título es obligatorio y no puede exceder 150 caracteres.";
        if (string.IsNullOrWhiteSpace(x.Descripcion) || x.Descripcion.Trim().Length > 2000) return "La descripción es obligatoria y no puede exceder 2000 caracteres.";
        if (x.CategoriaActividadSocialId == Guid.Empty) return "La categoría de actividad social es obligatoria.";
        if (string.IsNullOrWhiteSpace(x.Ubicacion) || x.Ubicacion.Trim().Length > 300) return "La ubicación es obligatoria y no puede exceder 300 caracteres.";
        if (x.Requisitos?.Trim().Length > 1000) return "Los requisitos no pueden exceder 1000 caracteres.";
        if (x.FechaActividad.ToUniversalTime() <= DateTime.UtcNow) return "La fecha de la actividad debe ser futura.";
        if (x.CantidadPersonasBuscadas <= 0) return "Debes indicar al menos una persona.";
        if (!Enum.IsDefined(x.Modalidad)) return "La modalidad no es válida.";
        if (x.Modalidad == ModalidadActividadSocial.Remunerada && (!x.MontoOfrecido.HasValue || x.MontoOfrecido <= 0)) return "Una actividad remunerada requiere un monto mayor que cero.";
        if (x.Modalidad == ModalidadActividadSocial.Gratuita && x.MontoOfrecido.HasValue) return "Una actividad gratuita no debe incluir un monto.";
        return null;
    }

    private static ActividadSocialDto MapActividad(ActividadSocial x, Guid usuarioId)
    {
        var aceptadas = x.Postulaciones.Count(p => p.Activo && p.Estado == EstadoPostulacionActividad.Aceptada);
        return new ActividadSocialDto
        {
            Id = x.Id, CreadorId = x.CreadorId, CreadorNombre = $"{x.Creador.Nombre} {x.Creador.Apellido}".Trim(),
            Titulo = x.Titulo, Descripcion = x.Descripcion,
            CategoriaActividadSocialId = x.CategoriaActividadSocialId,
            Categoria = x.CategoriaActividadSocial.Nombre,
            FechaActividad = x.FechaActividad,
            Ubicacion = x.Ubicacion, CantidadPersonasBuscadas = x.CantidadPersonasBuscadas, PersonasAceptadas = aceptadas,
            CuposDisponibles = Math.Max(0, x.CantidadPersonasBuscadas - aceptadas), Modalidad = x.Modalidad,
            MontoOfrecido = x.MontoOfrecido, Requisitos = x.Requisitos, Estado = x.Estado,
            MiPostulacion = x.Postulaciones.FirstOrDefault(p => p.PostulanteId == usuarioId && p.Activo)?.Estado,
            FechaCreacion = x.FechaCreacion
        };
    }

    private static PostulacionActividadDto MapPostulacion(PostulacionActividadSocial x) => new()
    {
        Id = x.Id, ActividadSocialId = x.ActividadSocialId, ActividadTitulo = x.ActividadSocial.Titulo,
        PostulanteId = x.PostulanteId, PostulanteNombre = $"{x.Postulante.Nombre} {x.Postulante.Apellido}".Trim(),
        Mensaje = x.Mensaje, Estado = x.Estado, FechaCreacion = x.FechaCreacion, FechaDecision = x.FechaDecision
    };

    private static string? LimpiarOpcional(string? valor) => string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
}
