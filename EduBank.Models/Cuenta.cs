using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduBank.Models
{
    public partial class Cuenta
    {
        public int CuentaId { get; set; }

        public int UsuarioId { get; set; }

        public string Nombre { get; set; } = null!;

        public string Tipo { get; set; } = null!;

        public decimal Saldo { get; set; }

        public string Moneda { get; set; } = null!;

        public bool Activo { get; set; }

        public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();

        public virtual ICollection<PagosHabituales> PagosHabituales { get; set; } = new List<PagosHabituales>();

        public virtual ICollection<Transferencia> TransferenciaCuentaDestinos { get; set; } = new List<Transferencia>();

        public virtual ICollection<Transferencia> TransferenciaCuentaOrigens { get; set; } = new List<Transferencia>();

        public virtual Usuario Usuario { get; set; } = null!;
    }
}
