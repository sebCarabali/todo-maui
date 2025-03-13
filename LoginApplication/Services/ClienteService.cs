using LoginApplication.Config;
using LoginApplication.Dtos;
using LoginApplication.Services.Interfaces;
using LoginApplication.Utils;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LoginApplication.Services
{
    public class ClienteService : IClienteService
    {
        private HttpClient _httpClient;
        private readonly AppSettings _appSettings;
        private readonly Endpoints _endpoints;

        public ClienteService(
            IOptionsSnapshot<Endpoints> endpoints, // Usar IOptionsSnapshot para configuración recargable
            IOptionsSnapshot<AppSettings> appSettings)
        {
            _endpoints = endpoints.Value ?? throw new ArgumentNullException(nameof(endpoints));
            _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));

            ConfigureHttpClient();
        }

        public async Task<string> GetFeatureExtractionDataAsyncByIdentification(string identification)
        {
            if (string.IsNullOrWhiteSpace(identification))
            {
                throw new ArgumentException("La identificación no puede estar vacía o ser nula.", nameof(identification));
            }

            string apiEndpoint = $"{_appSettings.BaseUrl}{_endpoints.EncodingByIdentification}";
            return await GetFeatureExtractionDataAsync(apiEndpoint, identification);
        }

        public async Task<string> GetFeatureExtractionDataAsyncByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("El correo electrónico no puede estar vacío o ser nulo.", nameof(email));
            }

            string apiEndpoint = $"{_appSettings.BaseUrl}{_endpoints.EncodingByEmail}";
            return await GetFeatureExtractionDataAsync(apiEndpoint, email);
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

        private void ConfigureHttpClient()
        {
            _httpClient = new HttpClient();
            var apiCredentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_appSettings.PublicUser}:{_appSettings.PublicPass}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", apiCredentials);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}