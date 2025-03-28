using LoginApplication.Config;
using LoginApplication.Dtos;
using LoginApplication.Services.Interfaces;
using LoginApplication.Utils;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LoginApplication.Services
{
    public class ClienteService : IClienteService
    {
        private HttpClient _httpClient;
        private readonly AppSettings _appSettings;
        private readonly Endpoints _endpoints;
        private readonly IFeatureExtractionService _featureExtractionService;

        public ClienteService(
            IOptionsSnapshot<Endpoints> endpoints, // Usar IOptionsSnapshot para configuración recargable
            IOptionsSnapshot<AppSettings> appSettings,
            IFeatureExtractionService featureExtractionService)
        {
            _endpoints = endpoints.Value ?? throw new ArgumentNullException(nameof(endpoints));
            _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));
            _featureExtractionService = featureExtractionService;
            ConfigureHttpClient();
        }

        public Task<string> GetEncodingAsync(string identifier)
        {
            if (IsValidEmail(identifier))
            {
                return GetEncodingByEmailAsync(identifier);
            }
            else
            {
                return GetEncodingByIdentificationAsync(identifier);
            }
        }


        private async Task<string> GetEncodingByIdentificationAsync(string identification)
        {
            if (string.IsNullOrWhiteSpace(identification))
            {
                throw new ArgumentException("La identificación no puede estar vacía o ser nula.", nameof(identification));
            }

            string apiEndpoint = $"{_appSettings.BaseUrl}{_endpoints.EncodingByIdentification}";
            return await GetFeatureExtractionDataAsync(apiEndpoint, identification);
        }

        private async Task<string> GetEncodingByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("El correo electrónico no puede estar vacío o ser nulo.", nameof(email));
            }

            string apiEndpoint = $"{_appSettings.BaseUrl}{_endpoints.EncodingByEmail}";
            return await GetFeatureExtractionDataAsync(apiEndpoint, email);
        }


        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }


        private async Task<string> GetFeatureExtractionDataAsync(string endpoint, string data)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                throw new ArgumentException("El endpoint no puede estar vacío o ser nulo.", nameof(endpoint));
            }

            try
            {
                var response = await _httpClient.GetAsync($"{endpoint}/{data}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error en la solicitud HTTP. Código de estado: {response.StatusCode}");
                }

                var encryptedResponse = await response.Content.ReadAsStringAsync();
                if (!IsBase64String(encryptedResponse))
                {
                    throw new ArgumentException("La cadena no es un Base-64 válido.");
                }
                var resData = EncryptionUtils.Decrypt<string>(encryptedResponse, _appSettings.Key);

                return resData;
            }
            catch (HttpRequestException ex)
            {
                throw new ApplicationException("Error al obtener los datos de extracción de características.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error inesperado al procesar la solicitud.", ex);
            }
        }

        private static bool IsBase64String(string base64)
        {
            Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out _);
        }

        private void ConfigureHttpClient()
        {
            _httpClient = new HttpClient();
            var apiCredentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_appSettings.PublicUser}:{_appSettings.PublicPass}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", apiCredentials);
        }

        public async Task<bool> SetEncodingAsync(string identifier, byte[] photoBytes)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new ArgumentException("El identificador no puede estar vacío o ser nulo.", nameof(identifier));
            }
            if (photoBytes == null)
            {
                throw new ArgumentException("La foto no puede ser nula.", nameof(photoBytes));
            }

            try
            {
                var encoding = await _featureExtractionService.ExtractFeaturesAsync(new MemoryStream(photoBytes));

                var request = new SetEncodingRequest
                {
                    Identificador = identifier,
                    Encoding = encoding
                };

                var jsonRequest = System.Text.Json.JsonSerializer.Serialize(request);
                var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var apiEndpoint = $"{_appSettings.BaseUrl}{_endpoints.SaveEncoding}";

                var response = await _httpClient.PostAsync(apiEndpoint, httpContent);
                response.EnsureSuccessStatusCode();

                var encryptedResponse = await response.Content.ReadAsStringAsync();
                var result = EncryptionUtils.Decrypt<SetEncodingResponse>(encryptedResponse, _appSettings.Key);

                return result?.Success == true;

            }
            catch (HttpRequestException httpEx)
            {
                throw new ApplicationException($"Error de comunicación con el servidor: {httpEx.Message}", httpEx);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error inesperado al guardar el encoding", ex);
            }
        }

    }
}