using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApplication.Dtos.Acceso
{
    public class AccesoResponseDTO
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; }
        public string NombreCompleto { get; set; }
        public DateTime FechaAcceso { get; set; }
        public String TipoAcceso { get; set; }
        public String Resultado { get; set; }

        public String Observaciones { get; set; }
    }
}
