using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.DTOs.DocumentosLegales;

namespace ConectaMasPro.Application.Interfaces.Services
{
    public interface IDocumentoLegalService
    {
        Task<Result<IReadOnlyList<DocumentoLegalDto>>> ObtenerVigentesAsync(
            CancellationToken cancellationToken = default);

        Task<Result<IReadOnlyList<AceptacionDocumentoLegalDto>>> ObtenerMisAceptacionesAsync(
            Guid usuarioId,
            CancellationToken cancellationToken = default);

    }
}
