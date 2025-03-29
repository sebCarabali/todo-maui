using LoginApplication.Config;
using LoginApplication.Dtos.Acceso;
using LoginApplication.Services.Interfaces;
using LoginApplication.Utils;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LoginApplication.Services
{
    public class RegistroAccesoService : IRegistroAccesoService
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AppSettings _appSettings;
        private readonly Endpoints _endpoints;

        public RegistroAccesoService(IOptionsSnapshot<Endpoints> endpoints,
            IOptionsSnapshot<AppSettings> appSettings,
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _endpoints = endpoints.Value ?? throw new ArgumentNullException(nameof(endpoints));
            _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));
        }

        public async Task<AccesoResponseDTO> RegistrarAccesoAsync(AccesoRequestDTO request)
        {
            var client = _httpClientFactory.CreateClient();

            var apiCredentials = Convert.ToBase64String(
            Encoding.ASCII.GetBytes($"{_appSettings.PublicUser}:{_appSettings.PublicPass}"));

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", apiCredentials);

            client.BaseAddress = new Uri(_appSettings.BaseUrl);

            try
            {
                var jsonRequest = System.Text.Json.JsonSerializer.Serialize(request);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(_endpoints.SaveAccessLog, content);

                var encryptedResponse = await response.Content.ReadAsStringAsync();

                return EncryptionUtils.Decrypt<AccesoResponseDTO>(encryptedResponse, _appSettings.Key);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al registrar el acceso.", ex);
            }
        }
    }
}
