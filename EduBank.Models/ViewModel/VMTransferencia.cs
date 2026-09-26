using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduBank.Models.ViewModel
{
    public class VMTransferencia
    {
        public int TransferenciaId { get; set; }

        [Required(ErrorMessage = "La cuenta origen es requerida")]
        public int CuentaOrigenId { get; set; }

        [Required(ErrorMessage = "La cuenta destino es requerida")]
        public int CuentaDestinoId { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        public DateTime FechaTransferencia { get; set; }

        [StringLength(200)]
        public string? Comentario { get; set; }

        // Propiedades para la UI
        public string? CuentaOrigenNombre { get; set; }
        public string? CuentaDestinoNombre { get; set; }
        public List<Cuenta>? CuentasUsuario { get; set; }
    }
}
