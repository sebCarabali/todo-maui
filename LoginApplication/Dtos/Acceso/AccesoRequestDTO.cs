using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApplication.Dtos.Acceso
{
    public class AccesoRequestDTO
    {
        public string? Identificacion { get; set; }
        public string? Correo { get; set; }
        public char Resultado { get; set; }
    }
}
