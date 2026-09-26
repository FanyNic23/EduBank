using EduBank.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduBank.Models
{
    public partial class Categoria
    {
        public int CategoriaId { get; set; }

        public int UsuarioId { get; set; }

        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }

        public string Tipo { get; set; } = null!;

        public string? Icono { get; set; }

        public string? Color { get; set; }

        public bool Activo { get; set; }

        public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();

        public virtual ICollection<PagosHabituales> PagosHabituales { get; set; } = new List<PagosHabituales>();

        public virtual Usuario Usuario { get; set; } = null!;
    }
}
