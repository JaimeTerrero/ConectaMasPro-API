using System.ComponentModel.DataAnnotations;

namespace ConectaMasPro.Application.Features.Auth.Requests
{
    public class ConfirmarActivacion2FaRequest
    {
        [Required(ErrorMessage = "El código de verificación es obligatorio.")]
        public string Codigo { get; set; } = null!;
    }
}
