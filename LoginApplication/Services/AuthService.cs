using System.Text;
using LoginApplication.Config;
using LoginApplication.Dtos;
using LoginApplication.Services.Interfaces;
using LoginApplication.Utils;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace LoginApplication.Services
{
    public class AuthService(ISecureStorage secureStorage, IOptions<AppSettings> appSettings)
        : IAuthService
    {
        private readonly HttpClient _httpClient = new();
        private readonly ISecureStorage _secureStorage = secureStorage;
        private readonly AppSettings _appSettings = appSettings.Value;

        public async Task<bool> LoginAsync(LoginRequestDTO request)
        {
            string url = $"{_appSettings.BaseUrl}api/Login";
            string json = JsonConvert.SerializeObject(request);
            StringContent content = new(json, Encoding.UTF8, "application/json");

            try
            {
                string apiCredentials = Convert.ToBase64String(
                    Encoding.ASCII.GetBytes($"{_appSettings.PublicUser}:{_appSettings.PublicPass}")
                );
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", apiCredentials);
                HttpResponseMessage response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) // 401
                    {
                        throw new UnauthorizedAccessException(
                            $"Credenciales inválidas. Por favor, verifica tu usuario y contraseña. {response}"
                        );
                    }
                    else // Other else
                    {
                        throw new HttpRequestException(
                            $"Error en la solicitud: {response.StatusCode}"
                        );
                    }
                }

                // Decrypt the response
                string encryptedResponse = await response.Content.ReadAsStringAsync();
                LoginResponseDTO loginResponse = EncryptionUtils.Decrypt<LoginResponseDTO>(
                    encryptedResponse,
                    _appSettings.Key
                );

                // Save token in secure storage
                await _secureStorage.SetAsync("JwtToken", loginResponse.Token);

                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Error de autenticación: {ex.Message}");
                throw;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error en la solicitud HTTP: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                throw;
            }
        }
    }
}
