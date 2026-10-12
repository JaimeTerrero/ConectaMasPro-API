using ConectaMasPro.Application.Interfaces.Services;
using ConectaMasPro.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ConectaMasPro.Application
{
    public static class IoC
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ICategoriaService, CategoriaService>();
            services.AddScoped<IServicioService, ServicioService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<ISolicitudServicioService, SolicitudServicioService>();
            services.AddScoped<IPagoService, PagoService>();
            services.AddScoped<IResenaService, ResenaService>();
            services.AddScoped<IConversacionService, ConversacionService>();
            services.AddScoped<INotificacionService, NotificacionService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IPerfilService, PerfilService>();
            services.AddScoped<IUbicacionService, UbicacionService>();
            services.AddScoped<ICatalogoService, CatalogoService>();
            services.AddScoped<IRecompensaService, RecompensaService>();
            services.AddScoped<IProgresoProveedorService, ProgresoProveedorService>();
            services.AddScoped<IReporteInconvenienteService, ReporteInconvenienteService>();
            services.AddScoped<IDocumentoLegalService, DocumentoLegalService>();
            services.AddScoped<IActividadSocialService, ActividadSocialService>();

            return services;
        }
    }
}
