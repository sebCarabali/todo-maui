using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApplication.Dtos
{
    public class SetEncodingRequest
    {
        public required string Identificador { get; set; }
        public required string Encoding { get; set; }
    }

    public class SetEncodingResponse
    {
        public bool? Success { get; set; }
        public string? Message { get; set; }
    }
}
