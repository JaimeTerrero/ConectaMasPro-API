using ConectaMasPro.Application.Interfaces.Repositories;
using ConectaMasPro.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace ConectaMasPro.Infrastructure.Seed
{
    public static class DocumentosLegalesSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var repository = unitOfWork.Repository<DocumentoLegal>();

            var documentosIniciales = new[]
            {
                DocumentosLegalesIniciales.CrearTerminosCondiciones(),
                DocumentosLegalesIniciales.CrearPoliticaPrivacidad()
            };

            var documentosExistentes = await repository.FindAsync(x =>
                x.Id == DocumentosLegalesIniciales.TerminosCondicionesId ||
                x.Id == DocumentosLegalesIniciales.PoliticaPrivacidadId);

            var idsExistentes = documentosExistentes.Select(x => x.Id).ToHashSet();
            var agregoDocumentos = false;

            foreach (var documento in documentosIniciales.Where(x => !idsExistentes.Contains(x.Id)))
            {
                await repository.AddAsync(documento);
                agregoDocumentos = true;
            }

            if (agregoDocumentos)
                await unitOfWork.SaveChangesAsync();
        }
    }
}
