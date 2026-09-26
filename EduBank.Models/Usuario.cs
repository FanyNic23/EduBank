using EduBank.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduBank.Models
{
    public partial class Usuario
    {
        public int UsuarioId { get; set; }

        public string Nombre { get; set; } = null!;

        public string Apellidos { get; set; } = null!;

        public string CorreoElectronico { get; set; } = null!;

        public string Contrasena { get; set; } = null!;

        public bool Activo { get; set; }

        public DateTime FechaRegistro { get; set; }

        public virtual ICollection<Categoria> Categoria { get; set; } = new List<Categoria>();

        public virtual ICollection<Cuenta> Cuenta { get; set; } = new List<Cuenta>();

        public virtual ICollection<PagosHabituales> PagosHabituales { get; set; } = new List<PagosHabituales>();

        public virtual ICollection<RecordatoriosGenerale> RecordatoriosGenerales { get; set; } = new List<RecordatoriosGenerale>();
    }
}
