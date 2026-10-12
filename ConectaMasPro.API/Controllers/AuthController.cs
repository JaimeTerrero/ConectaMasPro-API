using ConectaMasPro.Application.DTOs.Auth;
using ConectaMasPro.Application.Features.Auth.Requests;
using ConectaMasPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConectaMasPro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;

        public AuthController(IAuthService authService, IEmailService emailService)
        {
            _authService = authService;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
        {
            var direccionIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            var agenteUsuario = Request.Headers.UserAgent.ToString();

            var result = await _authService.RegisterAsync(
                request,
                direccionIp,
                agenteUsuario,
                cancellationToken);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.LoginAsync(request, cancellationToken);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("password/forgot")]
        public async Task<IActionResult> ForgotPassword(
            [FromBody] OlvidePasswordRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _authService.SolicitarRestablecimientoPasswordAsync(request, cancellationToken);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [AllowAnonymous]
        [HttpPost("password/reset")]
        public async Task<IActionResult> RestablecerPassword(
            [FromBody] RestablecerPasswordRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _authService.RestablecerPasswordAsync(request, cancellationToken);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me(CancellationToken cancellationToken)
        {
            var usuarioIdClaim = User.FindFirst("usuarioId")?.Value;

            if (string.IsNullOrWhiteSpace(usuarioIdClaim))
                return Unauthorized("Token inválido. No contiene usuarioId.");

            if (!Guid.TryParse(usuarioIdClaim, out var usuarioId))
                return Unauthorized("Token inválido. usuarioId no tiene formato válido.");

            var result = await _authService.GetCurrentUserAsync(usuarioId, cancellationToken);

            if (!result.Success)
                return Unauthorized(result);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("2fa/verify-login")]
        public async Task<IActionResult> VerificarLogin2Fa([FromBody] VerificarLogin2FaRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.VerificarLogin2FaAsync(
                request,
                cancellationToken);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("2fa/email/request-enable")]
        public async Task<IActionResult> SolicitarActivacion2FaCorreo(CancellationToken cancellationToken)
        {
            var identityUserId = ObtenerIdentityUserId();

            if (identityUserId is null)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "No fue posible identificar al usuario autenticado."
                });
            }

            var result = await _authService.SolicitarActivacion2FaCorreoAsync(
                    identityUserId.Value,
                    cancellationToken);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost("2fa/email/confirm-enable")]
        public async Task<IActionResult> ConfirmarActivacion2FaCorreo([FromBody] ConfirmarActivacion2FaRequest request, CancellationToken cancellationToken)
        {
            var identityUserId = ObtenerIdentityUserId();

            if (identityUserId is null)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "No fue posible identificar al usuario autenticado."
                });
            }

            var result =
                await _authService.ConfirmarActivacion2FaCorreoAsync(
                    identityUserId.Value,
                    request,
                    cancellationToken);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost("2fa/email/disable")]
        public async Task<IActionResult> Desactivar2FaCorreo([FromBody] Desactivar2FaRequest request, CancellationToken cancellationToken)
        {
            var identityUserId = ObtenerIdentityUserId();

            if (identityUserId is null)
                return Unauthorized("Token inválido.");

            var result = await _authService.Desactivar2FaCorreoAsync(
                identityUserId.Value,
                request,
                cancellationToken);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpGet("tipos-usuario")]
        public async Task<IActionResult> ObtenerTipoUsuarios()
        {
            var result = await _authService.ObtenerTiposUsuarioAsync();

            return Ok(result);
        }

        private Guid? ObtenerIdentityUserId()
        {
            var claim = User.FindFirst("identityUserId")?.Value;

            return Guid.TryParse(claim, out var identityUserId)
                ? identityUserId
                : null;
        }
    }
}
