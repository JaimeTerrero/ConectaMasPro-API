using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.DTOs.Auth;
using ConectaMasPro.Application.DTOs.Usuarios;
using ConectaMasPro.Application.Features.Auth.Requests;
using ConectaMasPro.Application.Interfaces.Repositories;
using ConectaMasPro.Application.Interfaces.Services;
using ConectaMasPro.Domain.Entities;
using ConectaMasPro.Domain.Enums;
using ConectaMasPro.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using ConectaMasPro.Application.Constants;
using Microsoft.Extensions.Options;
using ConectaMasPro.Infrastructure.Email;
using Microsoft.Extensions.Logging;

namespace ConectaMasPro.Infrastructure.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtService _jwtService;
        private const string Issuer = "ConectaMas Pro";
        private readonly IEmailService _emailService;
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<AuthService> _logger;

        public AuthService(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IUnitOfWork unitOfWork,
        JwtService jwtService,
        IEmailService emailService,
        IOptions<EmailSettings> emailOptions,
        ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _emailService = emailService;
            _emailSettings = emailOptions.Value;
            _logger = logger;
        }

        public async Task<Result> RegisterAsync(
        RegisterRequest request,
        string? direccionIp = null,
        string? agenteUsuario = null,
        CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Nombre))
                return Result<AuthResponse>.Fail("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.Apellido))
                return Result<AuthResponse>.Fail("El apellido es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.Email))
                return Result<AuthResponse>.Fail("El correo es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.Password))
                return Result<AuthResponse>.Fail("La contraseña es obligatoria.");

            if (!request.AceptaTerminosCondiciones)
                return Result.Fail("Debes leer y aceptar los Términos y Condiciones para crear tu cuenta.");

            if (!request.AceptaPoliticaPrivacidad)
                return Result.Fail("Debes leer y aceptar la Política de Privacidad para crear tu cuenta.");

            if (!request.ConfirmaMayoriaEdad)
                return Result.Fail("Debes confirmar que tienes al menos 18 años para crear tu cuenta.");

            var documentos = await _unitOfWork.Repository<DocumentoLegal>()
                .FindAsync(
                    x => (x.Id == request.TerminosCondicionesId ||
                          x.Id == request.PoliticaPrivacidadId) &&
                         x.EstaVigente &&
                         x.Activo,
                    cancellationToken);

            var terminos = documentos.FirstOrDefault(x =>
                x.Id == request.TerminosCondicionesId &&
                x.Tipo == TipoDocumentoLegal.TerminosCondiciones);

            var politicaPrivacidad = documentos.FirstOrDefault(x =>
                x.Id == request.PoliticaPrivacidadId &&
                x.Tipo == TipoDocumentoLegal.PoliticaPrivacidad);

            if (terminos is null)
                return Result.Fail("Los Términos y Condiciones seleccionados no están vigentes. Actualiza el documento e inténtalo nuevamente.");

            if (politicaPrivacidad is null)
                return Result.Fail("La Política de Privacidad seleccionada no está vigente. Actualiza el documento e inténtalo nuevamente.");

            var email = request.Email.Trim().ToLower();

            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser is not null)
                return Result<AuthResponse>.Fail("Ya existe un usuario con ese correo.");

            var identityUser = new ApplicationUser
            {
                UserName = email,
                Email = email,
                PhoneNumber = request.Telefono,
                Nombre = request.Nombre.Trim(),
                Apellido = request.Apellido.Trim(),
                EmailConfirmed = true
            };

            var identityResult = await _userManager.CreateAsync(
                identityUser,
                request.Password);

            if (!identityResult.Succeeded)
            {
                var errors = string.Join(" | ", identityResult.Errors.Select(x => x.Description));
                return Result<AuthResponse>.Fail(errors);
            }

            var usuario = new Usuario
            {
                IdentityUserId = identityUser.Id,
                Nombre = request.Nombre.Trim(),
                Apellido = request.Apellido.Trim(),
                Email = email,
                Telefono = request.Telefono?.Trim(),
                TipoUsuario = (TipoUsuario)request.TipoUsuarioId
            };

            await _unitOfWork.Repository<Usuario>().AddAsync(usuario, cancellationToken);

            var fechaAceptacion = DateTime.UtcNow;
            var ipNormalizada = LimitarLongitud(direccionIp, 45);
            var agenteNormalizado = LimitarLongitud(agenteUsuario, 500);

            await _unitOfWork.Repository<AceptacionDocumentoLegal>().AddAsync(
                new AceptacionDocumentoLegal
                {
                    UsuarioId = usuario.Id,
                    DocumentoLegalId = terminos.Id,
                    FechaAceptacion = fechaAceptacion,
                    DireccionIp = ipNormalizada,
                    AgenteUsuario = agenteNormalizado
                },
                cancellationToken);

            await _unitOfWork.Repository<AceptacionDocumentoLegal>().AddAsync(
                new AceptacionDocumentoLegal
                {
                    UsuarioId = usuario.Id,
                    DocumentoLegalId = politicaPrivacidad.Id,
                    FechaAceptacion = fechaAceptacion,
                    DireccionIp = ipNormalizada,
                    AgenteUsuario = agenteNormalizado
                },
                cancellationToken);

            try
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                await _userManager.DeleteAsync(identityUser);
                throw;
            }

            try
            {
                await _emailService.EnviarBienvenidaAsync(
                    usuario.Email,
                    usuario.Nombre,
                    usuario.TipoUsuario,
                    cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(
                    exception,
                    "No se pudo enviar el correo de bienvenida al usuario {UsuarioId}.",
                    usuario.Id);
            }

            return Result.Ok("El usuario fue registrado exitosamente");
        }

        private static string? LimitarLongitud(string? valor, int longitudMaxima)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return null;

            var valorNormalizado = valor.Trim();
            return valorNormalizado.Length <= longitudMaxima
                ? valorNormalizado
                : valorNormalizado[..longitudMaxima];
        }

        public async Task<Result<AuthResponse>> LoginAsync(
    LoginRequest request,
    CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return Result<AuthResponse>.Fail(
                    "El correo es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return Result<AuthResponse>.Fail(
                    "La contraseña es obligatoria.");
            }

            var email = request.Email.Trim().ToLowerInvariant();

            var identityUser = await _userManager.FindByEmailAsync(email);

            if (identityUser is null)
            {
                return Result<AuthResponse>.Fail(
                    "Credenciales inválidas.");
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(
                identityUser,
                request.Password,
                lockoutOnFailure: false);

            if (!signInResult.Succeeded)
            {
                return Result<AuthResponse>.Fail(
                    "Credenciales inválidas.");
            }


            /*
             * Este bloque solamente se ejecuta cuando el usuario
             * no tiene 2FA habilitado.
             */
            var usuarios = await _unitOfWork
                .Repository<Usuario>()
                .FindAsync(
                    x => x.IdentityUserId == identityUser.Id &&
                         x.Activo,
                    cancellationToken);

            var usuario = usuarios.FirstOrDefault();

            if (identityUser.TwoFactorEnabled)
            {
                if (string.IsNullOrWhiteSpace(identityUser.Email))
                {
                    return Result<AuthResponse>.Fail(
                        "El usuario no tiene un correo electrónico configurado.");
                }

                var codigo = await _userManager.GenerateTwoFactorTokenAsync(
                    identityUser,
                    TokenOptions.DefaultEmailProvider);

                try
                {
                    await _emailService.EnviarCodigoDosFactoresAsync(
                        identityUser.Email,
                        codigo,
                        cancellationToken);
                }
                catch
                {
                    return Result<AuthResponse>.Fail(
                        "No fue posible enviar el código de verificación.");
                }

                var twoFactorToken =
                    _jwtService.GenerateTwoFactorPendingToken(identityUser);

                return Result<AuthResponse>.Ok(
                    new AuthResponse
                    {
                        RequiereDosFactores = true,
                        TwoFactorToken = twoFactorToken,

                        IdentityUserId = identityUser.Id,
                        UsuarioId = usuario.Id,
                        Nombre = usuario.Nombre,
                        Email = OcultarCorreo(identityUser.Email),

                        Token = null,
                        ExpiraEn = null
                    },
                    "Se envió un código de verificación a su correo electrónico.");
            }

            if (usuario is null)
            {
                return Result<AuthResponse>.Fail(
                    "El perfil del usuario no existe.");
            }

            var token = _jwtService.GenerateToken(
                identityUser,
                usuario.Id,
                usuario.TipoUsuario.ToString());

            var response = new AuthResponse
            {
                RequiereDosFactores = false,
                TwoFactorToken = null,

                IdentityUserId = identityUser.Id,
                UsuarioId = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Token = token,
                ExpiraEn = _jwtService.GetExpirationDate()
            };

            return Result<AuthResponse>.Ok(
                response,
                "Inicio de sesión exitoso.");
        }

        public async Task<Result<CurrentUserResponse>> GetCurrentUserAsync(Guid usuarioId, CancellationToken cancellationToken = default)
        {
            if (usuarioId == Guid.Empty)
                return Result<CurrentUserResponse>.Fail("El usuario autenticado no es válido.");

            var usuario = await _unitOfWork
                .Repository<Usuario>()
                .GetByIdAsync(usuarioId, cancellationToken);

            if (usuario is null || !usuario.Activo)
                return Result<CurrentUserResponse>.Fail("Usuario no encontrado.");

            var response = new CurrentUserResponse
            {
                IdentityUserId = usuario.IdentityUserId,
                UsuarioId = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                TipoUsuario = usuario.TipoUsuario.ToString()
            };

            return Result<CurrentUserResponse>.Ok(response);
        }

        public Task<IEnumerable<TipoUsuarioDto>> ObtenerTiposUsuarioAsync()
        {
            var tipos = Enum
            .GetValues<TipoUsuario>()
            .Select(x => new TipoUsuarioDto
            {
                Id = (int)x,
                Nombre = x.ToString()
            });

            return Task.FromResult(tipos);
        }

        public async Task<Result<AuthResponse>> VerificarLogin2FaAsync(
    VerificarLogin2FaRequest request,
    CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.TwoFactorToken))
            {
                return Result<AuthResponse>.Fail(
                    "El token temporal de autenticación es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(request.Codigo))
            {
                return Result<AuthResponse>.Fail(
                    "El código de autenticación es obligatorio.");
            }

            var identityUserId = _jwtService
                .ValidateTwoFactorPendingToken(request.TwoFactorToken);

            if (identityUserId is null)
            {
                return Result<AuthResponse>.Fail(
                    "La solicitud de autenticación de dos factores no es válida o expiró.");
            }

            var identityUser = await _userManager.FindByIdAsync(
                identityUserId.Value.ToString());

            if (identityUser is null)
            {
                return Result<AuthResponse>.Fail(
                    "El usuario no existe.");
            }

            if (!identityUser.TwoFactorEnabled)
            {
                return Result<AuthResponse>.Fail(
                    "La autenticación de dos factores no está habilitada para este usuario.");
            }

            var codigoNormalizado = request.Codigo
                .Replace(" ", string.Empty)
                .Replace("-", string.Empty);

            var codigoValido = await _userManager.VerifyTwoFactorTokenAsync(
                identityUser,
                TokenOptions.DefaultEmailProvider,
                codigoNormalizado);

            if (!codigoValido)
            {
                return Result<AuthResponse>.Fail(
                    "El código de autenticación no es válido.");
            }

            var usuarios = await _unitOfWork
                .Repository<Usuario>()
                .FindAsync(
                    x => x.IdentityUserId == identityUser.Id &&
                         x.Activo,
                    cancellationToken);

            var usuario = usuarios.FirstOrDefault();

            if (usuario is null)
            {
                return Result<AuthResponse>.Fail(
                    "El perfil del usuario no existe.");
            }

            var token = _jwtService.GenerateToken(
                identityUser,
                usuario.Id,
                usuario.TipoUsuario.ToString());

            var response = new AuthResponse
            {
                RequiereDosFactores = false,
                TwoFactorToken = null,

                IdentityUserId = identityUser.Id,
                UsuarioId = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,

                Token = token,
                ExpiraEn = _jwtService.GetExpirationDate()
            };

            return Result<AuthResponse>.Ok(
                response,
                "Inicio de sesión exitoso.");
        }

        private static string OcultarCorreo(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return string.Empty;

            var partes = email.Split('@');

            if (partes.Length != 2)
                return email;

            var usuario = partes[0];
            var dominio = partes[1];

            if (usuario.Length == 1)
                return $"*@{dominio}";

            var cantidadAsteriscos = Math.Min(
                usuario.Length - 1,
                5);

            return
                $"{usuario[0]}{new string('*', cantidadAsteriscos)}@{dominio}";
        }

        public async Task<Result<bool>> SolicitarActivacion2FaCorreoAsync(Guid identityUserId, CancellationToken cancellationToken = default)
        {
            var identityUser = await _userManager.FindByIdAsync(
                identityUserId.ToString());

            if (identityUser is null)
            {
                return Result<bool>.Fail(
                    "No fue posible encontrar el usuario.");
            }

            if (identityUser.TwoFactorEnabled)
            {
                return Result<bool>.Fail(
                    "La autenticación en dos pasos ya está habilitada.");
            }

            if (string.IsNullOrWhiteSpace(identityUser.Email))
            {
                return Result<bool>.Fail(
                    "El usuario no tiene un correo electrónico configurado.");
            }

            if (!identityUser.EmailConfirmed)
            {
                return Result<bool>.Fail(
                    "Debe confirmar su correo electrónico antes de activar la autenticación en dos pasos.");
            }

            var codigo = await _userManager.GenerateUserTokenAsync(
                identityUser,
                TokenOptions.DefaultEmailProvider,
                TokenPurposes.HabilitarDosFactoresCorreo);

            try
            {
                await _emailService.EnviarCodigoDosFactoresAsync(
                    identityUser.Email,
                    codigo,
                    cancellationToken);
            }
            catch
            {
                return Result<bool>.Fail(
                    "No fue posible enviar el código de verificación.");
            }

            return Result<bool>.Ok(
                true,
                $"Se envió un código de verificación a {OcultarCorreo(identityUser.Email)}.");
        }

        public async Task<Result<bool>> ConfirmarActivacion2FaCorreoAsync(Guid identityUserId, ConfirmarActivacion2FaRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Codigo))
            {
                return Result<bool>.Fail(
                    "El código de verificación es obligatorio.");
            }

            var identityUser = await _userManager.FindByIdAsync(
                identityUserId.ToString());

            if (identityUser is null)
            {
                return Result<bool>.Fail(
                    "No fue posible encontrar el usuario.");
            }

            if (identityUser.TwoFactorEnabled)
            {
                return Result<bool>.Fail(
                    "La autenticación en dos pasos ya está habilitada.");
            }

            if (string.IsNullOrWhiteSpace(identityUser.Email))
            {
                return Result<bool>.Fail(
                    "El usuario no tiene un correo electrónico configurado.");
            }

            if (!identityUser.EmailConfirmed)
            {
                return Result<bool>.Fail(
                    "Debe confirmar su correo electrónico antes de activar la autenticación en dos pasos.");
            }

            var codigoNormalizado = request.Codigo
                .Replace(" ", string.Empty)
                .Replace("-", string.Empty)
                .Trim();

            var codigoValido = await _userManager.VerifyUserTokenAsync(
                identityUser,
                TokenOptions.DefaultEmailProvider,
                TokenPurposes.HabilitarDosFactoresCorreo,
                codigoNormalizado);

            if (!codigoValido)
            {
                return Result<bool>.Fail(
                    "El código de verificación no es válido o expiró.");
            }

            var resultado = await _userManager.SetTwoFactorEnabledAsync(
                identityUser,
                true);

            if (!resultado.Succeeded)
            {
                var errores = string.Join(
                    " ",
                    resultado.Errors.Select(x => x.Description));

                return Result<bool>.Fail(
                    $"No fue posible activar la autenticación en dos pasos. {errores}");
            }

            return Result<bool>.Ok(
                true,
                "La autenticación en dos pasos fue activada correctamente.");
        }

        public async Task<Result<bool>> Desactivar2FaCorreoAsync(Guid identityUserId, Desactivar2FaRequest request, CancellationToken cancellationToken = default)
        {
            var identityUser = await _userManager.FindByIdAsync(
                identityUserId.ToString());

            if (identityUser is null)
            {
                return Result<bool>.Fail(
                    "No fue posible encontrar el usuario.");
            }

            if (!identityUser.TwoFactorEnabled)
            {
                return Result<bool>.Fail(
                    "La autenticación en dos pasos ya está deshabilitada.");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return Result<bool>.Fail(
                    "La contraseña es obligatoria.");
            }

            var passwordValido = await _userManager.CheckPasswordAsync(
                identityUser,
                request.Password);

            if (!passwordValido)
            {
                return Result<bool>.Fail(
                    "La contraseña ingresada no es correcta.");
            }

            var resultado = await _userManager.SetTwoFactorEnabledAsync(
                identityUser,
                false);

            if (!resultado.Succeeded)
            {
                var errores = string.Join(
                    " ",
                    resultado.Errors.Select(x => x.Description));

                return Result<bool>.Fail(
                    $"No fue posible desactivar la autenticación en dos pasos. {errores}");
            }

            return Result<bool>.Ok(
                true,
                "La autenticación en dos pasos fue desactivada correctamente.");
        }

        public async Task<Result> SolicitarRestablecimientoPasswordAsync(
            OlvidePasswordRequest request,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return Result.Fail("El correo es obligatorio.");

            const string mensajeRespuesta =
                "Si el correo está registrado, recibirás un enlace para restablecer tu contraseña.";

            var email = request.Email.Trim().ToLowerInvariant();
            var identityUser = await _userManager.FindByEmailAsync(email);

            if (identityUser is null || string.IsNullOrWhiteSpace(identityUser.Email))
                return Result.Ok(mensajeRespuesta);

            if (string.IsNullOrWhiteSpace(_emailSettings.UrlRestablecimientoPassword) ||
                !Uri.TryCreate(_emailSettings.UrlRestablecimientoPassword, UriKind.Absolute, out _))
            {
                return Result.Fail("La URL de restablecimiento de contraseña no está configurada correctamente.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(identityUser);
            var separador = _emailSettings.UrlRestablecimientoPassword.Contains('?') ? "&" : "?";
            var enlace = $"{_emailSettings.UrlRestablecimientoPassword}{separador}email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";

            try
            {
                await _emailService.EnviarEnlaceRestablecimientoPasswordAsync(
                    identityUser.Email,
                    enlace,
                    cancellationToken);
            }
            catch
            {
                return Result.Fail("No fue posible enviar el correo de recuperación.");
            }

            return Result.Ok(mensajeRespuesta);
        }

        public async Task<Result> RestablecerPasswordAsync(
            RestablecerPasswordRequest request,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Token) ||
                string.IsNullOrWhiteSpace(request.NuevaPassword))
            {
                return Result.Fail("El correo, el token y la nueva contraseña son obligatorios.");
            }

            var identityUser = await _userManager.FindByEmailAsync(request.Email.Trim().ToLowerInvariant());
            if (identityUser is null)
                return Result.Fail("La solicitud de restablecimiento no es válida o expiró.");

            var resultado = await _userManager.ResetPasswordAsync(
                identityUser,
                request.Token,
                request.NuevaPassword);

            if (!resultado.Succeeded)
            {
                var errores = string.Join(" | ", resultado.Errors.Select(x => x.Description));
                return Result.Fail($"No fue posible restablecer la contraseña. {errores}");
            }

            await _userManager.UpdateSecurityStampAsync(identityUser);
            return Result.Ok("La contraseña fue restablecida exitosamente.");
        }
    }
}
