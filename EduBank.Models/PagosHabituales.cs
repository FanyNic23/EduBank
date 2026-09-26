using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduBank.Models
{
    public partial class PagosHabituales
    {
        public int PagoHabitualId { get; set; }
        public int UsuarioId { get; set; }
        public string Nombre { get; set; } = null!;
        public int Frecuencia { get; set; }
        public string UnidadFrecuencia { get; set; } = null!;
        public DateTime FechaInicio { get; set; }
        public TimeOnly? Hora { get; set; }
        public DateTime? FechaFin { get; set; }
        public int CuentaId { get; set; }
        public int CategoriaId { get; set; }
        public decimal Monto { get; set; }
        public string? Comentario { get; set; }
        public bool EsActivo { get; set; }
        public bool AgregarAutomaticamente { get; set; }
        public virtual Categoria Categoria { get; set; } = null!;
        public virtual Cuenta Cuenta { get; set; } = null!;
        public virtual Usuario Usuario { get; set; } = null!;
    }
}
