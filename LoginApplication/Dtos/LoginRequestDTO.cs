using Newtonsoft.Json;

namespace LoginApplication.Dtos
{
    public class LoginRequestDTO
    {
        [JsonProperty("id_tipo_identificacion")]
        public long? IdTipoIdentificacion { get; set; }
        [JsonProperty("identificacion")]
        public required string Identificacion { get; set; }
        [JsonProperty("contrasenia")]
        public required string Contrasenia { get; set; }
    }
}
