namespace ConectaMasPro.Infrastructure.Email
{
    public class EmailSettings
    {
        public string Host { get; set; } = null!;

        public int Puerto { get; set; }

        public bool UsarSsl { get; set; }

        public string Usuario { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string CorreoRemitente { get; set; } = null!;

        public string NombreRemitente { get; set; } = null!;

        public string UrlRestablecimientoPassword { get; set; } = null!;
    }
}
