using LoginApplication.Dtos;
using LoginApplication.Services.Interfaces;
using LoginApplication.Utils;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApplication.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ISecureStorage _secureStorage;
        private readonly string _keySecApi;
        private readonly string _apiBaseUrl;
        private readonly string _basicAuthToken;
        private readonly string _userPublicApi;
        private readonly string _passPublicApi;

        public AuthService(ISecureStorage secureStorage, IConfiguration config)
        {
            _httpClient = new HttpClient(); ;
            _secureStorage = secureStorage;
            _keySecApi = config["KEY_SEC_API"] ?? "";
            _apiBaseUrl = config["API_BASE_URL"] ?? "";
            _userPublicApi = config["Usuario_Public_Api"] ?? "";
            _passPublicApi = config["Contrasena_Public_Api"] ?? "";
        }

        public async Task<bool> LoginAsync(LoginRequestDTO request)
        {
            var url = $"{_apiBaseUrl}api/Login";
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var apiCredentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_userPublicApi}:{_passPublicApi}"));
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", apiCredentials);
                var response = await _httpClient.PostAsync(url, content);


                if (!response.IsSuccessStatusCode)
                {

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) // 401
                    {
                        throw new UnauthorizedAccessException($"Credenciales inválidas. Por favor, verifica tu usuario y contraseña. {response.ToString()}");
                    }
                    else // Other else
                    {
                        throw new HttpRequestException($"Error en la solicitud: {response.StatusCode}");
                    }
                }

                // Decrypt the response
                string encryptedResponse = await response.Content.ReadAsStringAsync();
                LoginResponseDTO loginResponse = EncryptionUtils.Decrypt<LoginResponseDTO>(encryptedResponse, _keySecApi);

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
