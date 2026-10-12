using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ConectaMasPro.Application.Settings;
using ConectaMasPro.Infrastructure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ConectaMasPro.Infrastructure.Authentication
{
    public class JwtService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtService(IOptions<JwtSettings> options)
        {
            _jwtSettings = options.Value;
        }

        public string GenerateToken(
            ApplicationUser user,
            Guid usuarioId,
            string tipoUsuario)
        {
            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            new("identityUserId", user.Id.ToString()),
            new("usuarioId", usuarioId.ToString()),
            new("nombre", user.Nombre),
            new("apellido", user.Apellido),
            new("tipoUsuario", tipoUsuario)
        };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Key));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public DateTime GetExpirationDate()
        {
            return DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes);
        }

        public string GenerateTwoFactorPendingToken(ApplicationUser user)
        {
            var claims = new List<Claim>
        {
            // Identifica al usuario de Identity
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),

            // Indica que este token solo sirve para completar el 2FA
            new("purpose", "2fa_pending"),

            // Identificador único del token
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Key));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                // Este token debe vivir poco tiempo
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Guid? ValidateTwoFactorPendingToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),

                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,

                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,

                ValidateLifetime = true,

                // Evita aceptar el token después de su expiración.
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(
                    token,
                    validationParameters,
                    out var validatedToken);

                // Confirmamos que sea un JWT firmado con el algoritmo esperado.
                if (validatedToken is not JwtSecurityToken jwtToken ||
                    !jwtToken.Header.Alg.Equals(
                        SecurityAlgorithms.HmacSha256,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                // Confirmamos que el token fue generado específicamente para el paso 2FA.
                var purpose = principal.FindFirst("purpose")?.Value;

                if (purpose != "2fa_pending")
                    return null;

                // Recuperamos el ID del usuario de Identity.
                var identityUserIdClaim =
                    principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                    ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!Guid.TryParse(identityUserIdClaim, out var identityUserId))
                    return null;

                return identityUserId;
            }
            catch
            {
                // Firma incorrecta, expirado, issuer/audience inválidos, etc.
                return null;
            }
        }
    }
}
