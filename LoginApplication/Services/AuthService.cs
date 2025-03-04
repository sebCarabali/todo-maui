using LoginApplication.Dtos;
using LoginApplication.Services.Interfaces;
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

        public AuthService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<bool> LoginAsync(LoginRequestDTO request)
        {
            var url = "https://run.mocky.io/v3/8646e76b-dd3e-414e-8236-1f92c9b4a67b";
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            // Deseralizar la clase Login response dto, también desencriptar la respusta usando la llave del front
            try
            {
                var response = await _httpClient.PostAsync(url, content);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
    }
}
