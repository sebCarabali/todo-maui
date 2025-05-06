using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoginApplication.Dtos;

namespace LoginApplication.Services.Interfaces
{
    public interface IFacialAuthenticationService
    {
        Task<FacialAuthResponseDTO> AuthenticateAsync(Stream image);
    }
}
