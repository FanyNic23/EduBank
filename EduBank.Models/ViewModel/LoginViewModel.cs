using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduBank.Models.ViewModel
{
    public class LoginViewModel
    {




        public string CorreoElectronico { get; set; } = null!;

        [DataType(DataType.Password)]
        public string Contrasena { get; set; } = null!;


    }
}
