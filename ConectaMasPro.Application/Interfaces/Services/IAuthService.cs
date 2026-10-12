using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.DTOs.Auth;
using ConectaMasPro.Application.DTOs.Usuarios;
using ConectaMasPro.Application.Features.Auth.Requests;

namespace ConectaMasPro.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<Result> RegisterAsync(
            RegisterRequest request,
            string? direccionIp = null,
            string? agenteUsuario = null,
            CancellationToken cancellationToken = default);
        Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
        Task<Result<CurrentUserResponse>> GetCurrentUserAsync(Guid usuarioId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TipoUsuarioDto>> ObtenerTiposUsuarioAsync();
        Task<Result<AuthResponse>> VerificarLogin2FaAsync(VerificarLogin2FaRequest request, CancellationToken cancellationToken = default);
        Task<Result<bool>> SolicitarActivacion2FaCorreoAsync(Guid identityUserId, CancellationToken cancellationToken = default);
        Task<Result<bool>> ConfirmarActivacion2FaCorreoAsync(Guid identityUserId, ConfirmarActivacion2FaRequest request,
        CancellationToken cancellationToken = default);
        Task<Result<bool>> Desactivar2FaCorreoAsync(Guid identityUserId, Desactivar2FaRequest request, CancellationToken cancellationToken = default);
        Task<Result> SolicitarRestablecimientoPasswordAsync(OlvidePasswordRequest request, CancellationToken cancellationToken = default);
        Task<Result> RestablecerPasswordAsync(RestablecerPasswordRequest request, CancellationToken cancellationToken = default);
    }
}
