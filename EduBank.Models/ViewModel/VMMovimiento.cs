using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduBank.Models.ViewModel
{
    public class VMMovimiento
    {
        public long MovimientoId { get; set; }
        public int CategoriaId { get; set; }
        public int CuentaId { get; set; }
        public string Tipo { get; set; } = "G"; // 'I' o 'G'
        public string FechaOperacion { get; set; } = DateTime.Now.ToString("yyyy-MM-dd"); // yyyy-MM-dd desde el input date
        public decimal Monto { get; set; }
        public string? Comentario { get; set; }
        public string? CategoriaNombre { get; set; } // para retornos
        public string? CategoriaIcono { get; set; }
    }

}
