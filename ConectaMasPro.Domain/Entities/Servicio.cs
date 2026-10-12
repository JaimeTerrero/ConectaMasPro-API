using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConectaMasPro.Domain.Common;

namespace ConectaMasPro.Domain.Entities
{
    public class Servicio : BaseEntity
    {
        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public Guid CategoriaServicioId { get; set; }
        public CategoriaServicio CategoriaServicio { get; set; } = null!;

        public string Titulo { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public decimal? PrecioBase { get; set; }
        public bool PrecioNegociable { get; set; } = true;

        public ICollection<SolicitudServicio> Solicitudes { get; set; } = new List<SolicitudServicio>();
    }
}
