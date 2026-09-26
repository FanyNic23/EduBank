using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduBank.Models.ViewModel
{

    public class VMCategoria
    {
        public int CategoriaId { get; set; }
        public int UsuarioId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string Tipo { get; set; } = string.Empty; // ← Asegúrate del valor por defecto
        public string? Icono { get; set; }
        public string? Color { get; set; }
        public bool Activo { get; set; } = true;
    }
}
