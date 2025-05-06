using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApplication.Dtos
{
    public class Cliente
    {
        public string identificacion { get; set; }
        public string primer_nombre { get; set; }
        public string segundo_nombre { get; set; }
        public string primer_apellido { get; set; }
        public string segundo_apellido { get; set; }
        public string correo { get; set; }
        public string celular { get; set; }
    }
}
