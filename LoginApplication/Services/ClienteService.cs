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

        public ClienteService(
            IOptionsSnapshot<Endpoints> endpoints, // Usar IOptionsSnapshot para configuración recargable
            IOptionsSnapshot<AppSettings> appSettings)
        {
            _endpoints = endpoints.Value ?? throw new ArgumentNullException(nameof(endpoints));
            _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));
            ConfigureHttpClient();
        }

        private void ConfigureHttpClient()
        {
            _httpClient = new HttpClient();
            var apiCredentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_appSettings.PublicUser}:{_appSettings.PublicPass}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", apiCredentials);
        }

        public async Task<Cliente> AgregarCliente(Cliente cliente, Stream photo)
        {
            
            try
            {
                var apiUrl = $"{_appSettings.BaseUrl}{_endpoints.AddClient}";
                var content = HttpUtils.CreateClientMultipartContent(cliente, photo);
                var response = await HttpUtils.PostToApiAsync(_httpClient, apiUrl, content);
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var authResponse = JsonConvert.DeserializeObject<Cliente>(jsonResponse) ??
                    throw new InvalidOperationException("La respuesta de la API no pudo ser deserializada.");
                return authResponse;
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

        public async Task<FacialAuthResponseDTO> Authenticate(Stream photo)
        {
            try
            {
                var apiUrl = $"{_appSettings.BaseUrl}{_endpoints.Authenticate}";
                var content = HttpUtils.CreateMultipartContent(photo);
                var response = await HttpUtils.PostToApiAsync(_httpClient, apiUrl, content);
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var authResponse = JsonConvert.DeserializeObject<FacialAuthResponseDTO>(jsonResponse) ??
                    throw new InvalidOperationException("La respuesta de la API no pudo ser deserializada.");
                return authResponse;
            } catch(Exception ex)
            {
                throw HttpUtils.HandleServiceException("Error al llamar a la API de extracción", ex);
            }
            throw new NotImplementedException();
        }
    }
}