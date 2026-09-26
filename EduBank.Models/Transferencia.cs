using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduBank.Models
{
    public partial class Transferencia
    {
        public int TransferenciaId { get; set; }

        public int CuentaOrigenId { get; set; }

        public int CuentaDestinoId { get; set; }

        public decimal Monto { get; set; }

        public DateTime FechaTransferencia { get; set; }

        public string? Comentario { get; set; }

        public virtual Cuenta CuentaDestino { get; set; } = null!;

        public virtual Cuenta CuentaOrigen { get; set; } = null!;
    }

}
