using Newtonsoft.Json;

namespace LoginApplication.Dtos
{
    public class LoginRequestDTO
    {
        [JsonProperty("id_tipo_identificacion")]
        public int IdTipoIdentificacion { get; set; }
        [JsonProperty("identificacion")]
        public string Identificacion { get; set; }
        [JsonProperty("contrasenia")]
        public string Contrasenia { get; set; }
    }
}
