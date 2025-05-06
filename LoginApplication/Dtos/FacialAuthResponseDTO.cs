using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApplication.Dtos
{
    public class FacialAuthResponseDTO
    {
        public bool IsAuthenticated { get; set; }
        public string Message { get; set; }
        public double Confidence { get; set; }
        public Cliente Cliente { get; set; }
    }
}
