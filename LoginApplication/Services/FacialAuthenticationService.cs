using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LoginApplication.Dtos;
using LoginApplication.Services.Interfaces;

namespace LoginApplication.Services
{
    public class FacialAuthenticationService : IFacialAuthenticationService
    {
        private readonly IClienteService _clienteService;

        public FacialAuthenticationService(
            IClienteService clienteService)
        {
            _clienteService = clienteService ?? throw new ArgumentNullException(nameof(clienteService));
        }

        public async Task<FacialAuthResponseDTO> AuthenticateAsync(Stream image)
        {
            try
            {
                if (image == null)
                {
                    throw new ArgumentNullException(nameof(image));
                }

                var authResponse = await _clienteService.Authenticate(image);
                return authResponse;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}