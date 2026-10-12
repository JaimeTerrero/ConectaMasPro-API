using ConectaMasPro.Application.Interfaces.Repositories;
using ConectaMasPro.Domain.Entities;
using ConectaMasPro.Domain.Enums;
using ConectaMasPro.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace ConectaMasPro.Infrastructure.Seed
{
    public static class IdentitySeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var email = configuration["AdminSeed:Email"];
            var password = configuration["AdminSeed:Password"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return;

            var identityUser = await userManager.FindByEmailAsync(email);

            if (identityUser is null)
            {
                identityUser = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    Nombre = "Admin",
                    Apellido = "ConectaMas"
                };

                var result = await userManager.CreateAsync(identityUser, password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(" | ", result.Errors.Select(x => x.Description));
                    throw new Exception($"Error creando usuario Admin: {errors}");
                }
            }

            var perfilExistente = await unitOfWork
                .Repository<Usuario>()
                .FindAsync(x => x.IdentityUserId == identityUser.Id && x.Activo);

            if (!perfilExistente.Any())
            {
                var usuario = new Usuario
                {
                    IdentityUserId = identityUser.Id,
                    Nombre = identityUser.Nombre,
                    Apellido = identityUser.Apellido,
                    Email = email,
                    Telefono = identityUser.PhoneNumber,
                    TipoUsuario = TipoUsuario.Administrador
                };

                await unitOfWork.Repository<Usuario>().AddAsync(usuario);
                await unitOfWork.SaveChangesAsync();
            }
        }
    }
}
