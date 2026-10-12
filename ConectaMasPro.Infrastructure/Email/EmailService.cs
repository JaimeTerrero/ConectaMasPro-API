using Microsoft.Extensions.Options;
using ConectaMasPro.Application.Interfaces.Services;
using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;
using ConectaMasPro.Domain.Enums;
using System.Net;

namespace ConectaMasPro.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task EnviarCodigoDosFactoresAsync(
            string destinatario,
            string codigo,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(destinatario))
            {
                throw new ArgumentException(
                    "El correo del destinatario es obligatorio.",
                    nameof(destinatario));
            }

            if (string.IsNullOrWhiteSpace(codigo))
            {
                throw new ArgumentException(
                    "El código de verificación es obligatorio.",
                    nameof(codigo));
            }

            var mensaje = new MimeMessage();

            mensaje.From.Add(
                new MailboxAddress(
                    _settings.NombreRemitente,
                    _settings.CorreoRemitente));

            mensaje.To.Add(MailboxAddress.Parse(destinatario));

            mensaje.Subject =
                "Código de verificación - ConectaMas Pro";

            mensaje.Body = new TextPart("html")
            {
                Text = $"""
                    <!DOCTYPE html>
                    <html lang="es">
                    <head>
                        <meta charset="UTF-8">
                    </head>
                    <body style="font-family: Arial, sans-serif;">
                        <h2>Verificación de inicio de sesión</h2>

                        <p>
                            Recibimos una solicitud para iniciar sesión
                            en tu cuenta de ConectaMas Pro.
                        </p>

                        <p>Tu código de verificación es:</p>

                        <div style="
                            font-size: 32px;
                            font-weight: bold;
                            letter-spacing: 8px;
                            margin: 24px 0;">
                            {codigo}
                        </div>

                        <p>
                            Introduce este código en la pantalla de
                            verificación para completar el inicio de sesión.
                        </p>

                        <p>
                            Si no intentaste iniciar sesión, puedes ignorar
                            este mensaje.
                        </p>
                    </body>
                    </html>
                    """
            };

            using var smtpClient = new SmtpClient();

            var seguridad = _settings.UsarSsl
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTls;

            await smtpClient.ConnectAsync(
                _settings.Host,
                _settings.Puerto,
                seguridad,
                cancellationToken);

            await smtpClient.AuthenticateAsync(
                _settings.Usuario,
                _settings.Password,
                cancellationToken);

            await smtpClient.SendAsync(
                mensaje,
                cancellationToken);

            await smtpClient.DisconnectAsync(
                true,
                cancellationToken);
        }

        public async Task EnviarEnlaceRestablecimientoPasswordAsync(
            string destinatario,
            string enlace,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(destinatario))
                throw new ArgumentException("El correo del destinatario es obligatorio.", nameof(destinatario));

            if (string.IsNullOrWhiteSpace(enlace))
                throw new ArgumentException("El enlace de recuperación es obligatorio.", nameof(enlace));

            var mensaje = new MimeMessage();
            mensaje.From.Add(new MailboxAddress(_settings.NombreRemitente, _settings.CorreoRemitente));
            mensaje.To.Add(MailboxAddress.Parse(destinatario));
            mensaje.Subject = "Restablece tu contraseña - ConectaMas Pro";
            mensaje.Body = new TextPart("html")
            {
                Text = $"""
                    <!DOCTYPE html>
                    <html lang="es">
                    <head><meta charset="UTF-8"></head>
                    <body style="font-family: Arial, sans-serif;">
                        <h2>Restablecimiento de contraseña</h2>
                        <p>Recibimos una solicitud para restablecer la contraseña de tu cuenta.</p>
                        <p style="margin: 28px 0;">
                            <a href="{System.Net.WebUtility.HtmlEncode(enlace)}"
                               style="background: #2563eb; color: white; padding: 12px 20px; text-decoration: none; border-radius: 6px;">
                                Restablecer contraseña
                            </a>
                        </p>
                        <p>Si no solicitaste este cambio, puedes ignorar este mensaje.</p>
                    </body>
                    </html>
                    """
            };

            await EnviarAsync(mensaje, cancellationToken);
        }

        public async Task EnviarBienvenidaAsync(
            string destinatario,
            string nombre,
            TipoUsuario tipoUsuario,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(destinatario))
                throw new ArgumentException("El correo del destinatario es obligatorio.", nameof(destinatario));

            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del destinatario es obligatorio.", nameof(nombre));

            var (titulo, descripcion, siguientePaso) = tipoUsuario switch
            {
                TipoUsuario.Cliente => (
                    "Encuentra al profesional que necesitas",
                    "En ConectaMas Pro puedes descubrir proveedores, comparar sus servicios y solicitar la ayuda que necesitas.",
                    "Explora los servicios disponibles y conecta con el proveedor ideal para ti."),
                TipoUsuario.Proveedor => (
                    "Haz crecer tus oportunidades",
                    "En ConectaMas Pro puedes mostrar tus servicios y conectar con clientes que necesitan tu experiencia.",
                    "Completa tu perfil y publica tus servicios para comenzar a recibir solicitudes."),
                _ => (
                    "Conecta con nuevas oportunidades",
                    "En ConectaMas Pro, clientes y proveedores se encuentran para convertir necesidades en soluciones.",
                    "Completa tu perfil y descubre todo lo que la comunidad tiene para ofrecerte.")
            };

            var nombreSeguro = WebUtility.HtmlEncode(nombre.Trim());
            var mensaje = new MimeMessage();
            mensaje.From.Add(new MailboxAddress(_settings.NombreRemitente, _settings.CorreoRemitente));
            mensaje.To.Add(MailboxAddress.Parse(destinatario));
            mensaje.Subject = "¡Bienvenido a ConectaMas Pro!";
            mensaje.Body = new TextPart("html")
            {
                Text = $"""
                    <!DOCTYPE html>
                    <html lang="es">
                    <head><meta charset="UTF-8"></head>
                    <body style="margin: 0; background: #f3f4f6; font-family: Arial, sans-serif; color: #1f2937;">
                        <div style="max-width: 600px; margin: 0 auto; padding: 32px 16px;">
                            <div style="background: #ffffff; border-radius: 12px; padding: 32px; border-top: 5px solid #2563eb;">
                                <h1 style="margin-top: 0; color: #2563eb;">¡Hola, {nombreSeguro}!</h1>
                                <h2 style="font-size: 20px;">{titulo}</h2>
                                <p style="line-height: 1.6;">{descripcion}</p>
                                <p style="line-height: 1.6;">{siguientePaso}</p>
                                <p style="margin-top: 28px;">Gracias por formar parte de ConectaMas Pro.</p>
                            </div>
                        </div>
                    </body>
                    </html>
                    """
            };

            await EnviarAsync(mensaje, cancellationToken);
        }

        private async Task EnviarAsync(MimeMessage mensaje, CancellationToken cancellationToken)
        {
            using var smtpClient = new SmtpClient();
            var seguridad = _settings.UsarSsl
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTls;

            await smtpClient.ConnectAsync(_settings.Host, _settings.Puerto, seguridad, cancellationToken);
            await smtpClient.AuthenticateAsync(_settings.Usuario, _settings.Password, cancellationToken);
            await smtpClient.SendAsync(mensaje, cancellationToken);
            await smtpClient.DisconnectAsync(true, cancellationToken);
        }
    }

}

