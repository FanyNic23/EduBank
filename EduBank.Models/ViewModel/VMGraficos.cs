
using System.Collections.Generic;

namespace EduBank.Models.ViewModel
{
    public class VMGraficos
    {
        public List<ResumenGastos> ResumenGastos { get; set; }
        public List<ResumenIngresos> ResumenIngresos { get; set; }
    }

    public class ResumenGastos
    {
        public string Mes { get; set; }
        public decimal Total { get; set; }
    }

    public class ResumenIngresos
    {
        public string Categoria { get; set; }
        public decimal Total { get; set; }
    }
}