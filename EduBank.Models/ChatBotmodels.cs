using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduBank.Models
{
    // En Capa de Modelos - EduBank.Models
    public class ChatbotMessage
    {
        public int ChatbotMessageId { get; set; }
        public int UsuarioId { get; set; }
        public string Mensaje { get; set; }
        public string Respuesta { get; set; }
        public DateTime Fecha { get; set; }
        public string TipoConsulta { get; set; } // "financiera", "general", "recomendacion"

        public Usuario Usuario { get; set; }
    }

    public class ResumenFinanciero
    {
        public decimal IngresosTotales { get; set; }
        public decimal GastosTotales { get; set; }
        public decimal SaldoTotal { get; set; }
        public List<CategoriaGasto> GastosPorCategoria { get; set; }
        public List<MovimientoReciente> MovimientosRecientes { get; set; }
    }

    public class CategoriaGasto
    {
        public string Categoria { get; set; }
        public decimal Monto { get; set; }
        public decimal Porcentaje { get; set; }
    }

    public class MovimientoReciente
    {
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }
        public decimal Monto { get; set; }
        public string Tipo { get; set; }
    }
}
