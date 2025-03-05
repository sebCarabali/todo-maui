using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApplication.Dtos
{
    public class LoginResponseDTO
    {
        [JsonProperty("id_cliente")]
        public required long IdCliente { get; set; }

        [JsonProperty("id_tipo_identificacion")]
        public long? IdTipoIdentificacion { get; set; }

        [JsonProperty("primer_nombre")]
        public required string PrimerNombre { get; set; }

        [JsonProperty("segundo_nombre")]
        public required string SegundoNombre { get; set; }

        [JsonProperty("primer_apellido")]
        public required string PrimerApellido { get; set; }

        [JsonProperty("razon_social")]
        public string? RazonSocial { get; set; }

        [JsonProperty("Token")]
        public required string Token { get; set; }
    }
}
