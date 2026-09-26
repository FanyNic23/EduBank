using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduBank.Models.ViewModel
{
    public class VMCuenta
    {
        public int CuentaId { get; set; }

        public string Nombre { get; set; } = null!;

        public string Tipo { get; set; } = null!;

        public string Moneda { get; set; } = null!;

        public decimal Saldo { get; set; }
        public bool Activo { get; set; } = true;
    }
}