using Newtonsoft.Json;

namespace LoginApplication.Dtos
{
    public class LoginRequestDTO
    {
        [JsonProperty("correo")]
        public required string Correo { get; set; }
        [JsonProperty("contrasenia")]
        public required string Contrasenia { get; set; }
    }
}
