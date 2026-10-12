using System.Text;
using ConectaMasPro.Application.Interfaces.Repositories;
using ConectaMasPro.Application.Interfaces.Services;
using ConectaMasPro.Infrastructure.Authentication;
using ConectaMasPro.Infrastructure.Email;
using ConectaMasPro.Infrastructure.Identity;
using ConectaMasPro.Infrastructure.Persistence;
using ConectaMasPro.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ConectaMasPro.Infrastructure.Payments;

namespace ConectaMasPro.Infrastructure
{
    public static class IoC
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(
                typeof(IRepository<>),
                typeof(Repository<>));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services
                .AddIdentity<ApplicationUser, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<EmailSettings>(configuration.GetSection("Email"));
            services.Configure<StripeSettings>(configuration.GetSection("Stripe"));

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IPasarelaPagoService, StripePaymentService>();

            services.AddScoped<JwtService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IIdentityService, IdentityService>();

            var jwtKey = configuration["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "La configuración Jwt:Key no fue encontrada.");

            if (string.IsNullOrWhiteSpace(jwtKey) || Encoding.UTF8.GetByteCount(jwtKey) < 32)
                throw new InvalidOperationException(
                    "Configura Jwt:Key fuera del repositorio con una clave aleatoria de al menos 32 bytes.");

            var jwtIssuer = configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException(
                    "La configuración Jwt:Issuer no fue encontrada.");

            var jwtAudience = configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException(
                    "La configuración Jwt:Audience no fue encontrada.");

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme =
                        JwtBearerDefaults.AuthenticationScheme;

                    options.DefaultChallengeScheme =
                        JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = jwtIssuer,

                            ValidateAudience = true,
                            ValidAudience = jwtAudience,

                            ValidateLifetime = true,

                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(jwtKey)),

                            ClockSkew = TimeSpan.Zero
                        };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            if (!string.IsNullOrEmpty(accessToken) &&
                                context.HttpContext.Request.Path.StartsWithSegments("/hubs/mensajeria"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            var purpose = context.Principal?
                                .FindFirst("purpose")?
                                .Value;

                            /*
                             * El token temporal puede tener una firma válida,
                             * pero no debe autenticar al usuario en la API.
                             */
                            if (purpose == "2fa_pending")
                            {
                                context.Fail(
                                    "El token temporal de 2FA no puede utilizarse como token de acceso.");
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization();

            return services;
        }
    }
}
