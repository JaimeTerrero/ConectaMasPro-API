using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.DTOs.DocumentosLegales;
using ConectaMasPro.Application.Interfaces.Repositories;
using ConectaMasPro.Application.Interfaces.Services;
using ConectaMasPro.Domain.Entities;

namespace ConectaMasPro.Application.Services
{
    public class DocumentoLegalService : IDocumentoLegalService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DocumentoLegalService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IReadOnlyList<DocumentoLegalDto>>> ObtenerVigentesAsync(
            CancellationToken cancellationToken = default)
        {
            var documentos = await _unitOfWork.Repository<DocumentoLegal>()
                .GetAsync(
                    x => x.EstaVigente && x.Activo,
                    x => x.OrderBy(d => d.Tipo),
                    cancellationToken);

            return Result<IReadOnlyList<DocumentoLegalDto>>.Ok(
                documentos.Select(CrearDocumentoDto).ToList());
        }

        public async Task<Result<IReadOnlyList<AceptacionDocumentoLegalDto>>> ObtenerMisAceptacionesAsync(
            Guid usuarioId,
            CancellationToken cancellationToken = default)
        {
            if (usuarioId == Guid.Empty)
                return Result<IReadOnlyList<AceptacionDocumentoLegalDto>>.Fail(
                    "El usuario autenticado no es válido.");

            var aceptaciones = await _unitOfWork.Repository<AceptacionDocumentoLegal>()
                .GetAsync(
                    x => x.UsuarioId == usuarioId && x.Activo,
                    x => x.OrderByDescending(a => a.FechaAceptacion),
                    cancellationToken,
                    x => x.DocumentoLegal);

            var resultado = aceptaciones.Select(x => new AceptacionDocumentoLegalDto
            {
                DocumentoLegalId = x.DocumentoLegalId,
                TipoId = (int)x.DocumentoLegal.Tipo,
                Tipo = x.DocumentoLegal.Tipo.ToString(),
                Version = x.DocumentoLegal.Version,
                Titulo = x.DocumentoLegal.Titulo,
                FechaAceptacion = x.FechaAceptacion
            }).ToList();

            return Result<IReadOnlyList<AceptacionDocumentoLegalDto>>.Ok(resultado);
        }

        private static DocumentoLegalDto CrearDocumentoDto(DocumentoLegal documento) => new()
        {
            Id = documento.Id,
            TipoId = (int)documento.Tipo,
            Tipo = documento.Tipo.ToString(),
            Version = documento.Version,
            Titulo = documento.Titulo,
            Contenido = documento.Contenido,
            FechaPublicacion = documento.FechaPublicacion,
            RequiereAceptacion = documento.RequiereAceptacion
        };

    }
}
