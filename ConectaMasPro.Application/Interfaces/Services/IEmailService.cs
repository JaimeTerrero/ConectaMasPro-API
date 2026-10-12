namespace ConectaMasPro.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task EnviarCodigoDosFactoresAsync(
            string destinatario,
            string codigo,
            CancellationToken cancellationToken = default);

        Task EnviarEnlaceRestablecimientoPasswordAsync(
            string destinatario,
            string enlace,
            CancellationToken cancellationToken = default);

        Task EnviarBienvenidaAsync(
            string destinatario,
            string nombre,
            Domain.Enums.TipoUsuario tipoUsuario,
            CancellationToken cancellationToken = default);
    }
}
