using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.DTOs.Usuarios;

namespace ConectaMasPro.Application.Interfaces.Services
{
    public interface IUsuarioService
    {
        Task<Result<Guid>> CrearAsync(CrearUsuarioRequest request, CancellationToken cancellationToken = default);
        Task<Result<UsuarioDto>> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
