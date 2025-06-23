using System.Text;
using LoginApplication.Config;
using LoginApplication.Dtos;
using LoginApplication.Exceptions;
using LoginApplication.Services.Interfaces;
using LoginApplication.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;

namespace LoginApplication.Services
{
    public class AuthService(
        ISecureStorage secureStorage, 
        IOptions<AppSettings> appSettings,
        ILogger<AuthService> logger)
        : IAuthService
    {
        private readonly HttpClient _httpClient = new();
        private readonly ISecureStorage _secureStorage = secureStorage;
        private readonly AppSettings _appSettings = appSettings.Value;
        private readonly ILogger<AuthService> _logger = logger;

        public async Task<bool> LoginAsync(LoginRequestDTO request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "La solicitud de inicio de sesión no puede ser nula");

            if (string.IsNullOrWhiteSpace(request.Correo) || string.IsNullOrWhiteSpace(request.Contrasenia))
                throw new AppValidationException(new Dictionary<string, string[]> 
                { 
                    [nameof(request.Correo)] = new[] { "El correo electrónico es requerido" },
                    [nameof(request.Contrasenia)] = new[] { "La contraseña es requerida" }
                });

            string url = $"{_appSettings.BaseUrl}api/Login";
            string json = JsonConvert.SerializeObject(request);
            StringContent content = new(json, Encoding.UTF8, "application/json");

            try
            {
                _logger.LogInformation("Iniciando proceso de autenticación para el usuario: {Email}", request.Correo);
                
                string apiCredentials = Convert.ToBase64String(
                    Encoding.ASCII.GetBytes($"{_appSettings.PublicUser}:{_appSettings.PublicPass}")
                );
                
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", apiCredentials);
                
                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error en la autenticación. Código: {StatusCode}, Respuesta: {Response}", 
                        response.StatusCode, errorContent);

                    return response.StatusCode switch
                    {
                        HttpStatusCode.Unauthorized => 
                            throw new AppUnauthorizedException("Credenciales inválidas. Por favor, verifica tu usuario y contraseña."),
                        HttpStatusCode.BadRequest => 
                            throw new AppValidationException(await ParseValidationErrors(response)),
                        _ => throw new HttpRequestException(
                            $"Error en la solicitud: {response.StatusCode}", null, response.StatusCode)
                    };
                }

                // Decrypt the response
                string encryptedResponse = await response.Content.ReadAsStringAsync();
                LoginResponseDTO loginResponse = EncryptionUtils.Decrypt<LoginResponseDTO>(
                    encryptedResponse,
                    _appSettings.Key
                );

                if (string.IsNullOrEmpty(loginResponse?.Token))
                {
                    _logger.LogError("Token de autenticación vacío o nulo en la respuesta");
                    throw new AppException("Error de autenticación", "No se pudo obtener el token de autenticación");
                }


                // Save token in secure storage
                await _secureStorage.SetAsync("JwtToken", loginResponse.Token);
                _logger.LogInformation("Autenticación exitosa para el usuario: {Email}", request.Correo);

                return true;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                _logger.LogWarning(ex, "Error de autenticación para el usuario: {Email}", request.Correo);
                throw new AppUnauthorizedException("No se pudo autenticar. Verifica tus credenciales.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de conexión al intentar autenticar");
                throw new AppException("Error de conexión", 
                    "No se pudo conectar con el servidor. Por favor, verifica tu conexión a internet.");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error al procesar la respuesta del servidor");
                throw new AppException(
                    "Error en el servidor: Ocurrió un error al procesar la respuesta del servidor. Por favor, inténtalo de nuevo más tarde.",
                    ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado durante el inicio de sesión");
                throw new AppException(
                    "Error inesperado: Ocurrió un error inesperado durante el inicio de sesión. Por favor, inténtalo de nuevo.",
                    ex);
            }
        }

        private async Task<Dictionary<string, string[]>> ParseValidationErrors(HttpResponseMessage response)
        {
            try
            {
                var content = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(content);
                return errorResponse ?? new Dictionary<string, string[]> 
                { 
                    ["Error"] = new[] { "Error de validación no especificado" } 
                };
            }
            catch
            {
                return new Dictionary<string, string[]> 
                { 
                    ["Error"] = new[] { "Error al procesar los errores de validación" } 
                };
            }
        }
    }
}
