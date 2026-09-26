using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduBank.Models
{
    public partial class RecordatoriosGenerale
    {
        public int RecordatorioId { get; set; }

        public int UsuarioId { get; set; }

        public string Nombre { get; set; } = null!;

        public int Frecuencia { get; set; }

        public string UnidadFrecuencia { get; set; } = null!;

        public DateTime FechaInicio { get; set; }

        public TimeOnly? Hora { get; set; }

        public DateTime? FechaFin { get; set; }

        public string? Comentario { get; set; }

        public bool EsActivo { get; set; }

        public virtual Usuario Usuario { get; set; } = null!;
    }
}
