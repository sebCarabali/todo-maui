using LoginApplication.Dtos.Acceso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApplication.Services.Interfaces
{
    public interface IRegistroAccesoService
    {
        Task<AccesoResponseDTO> RegistrarAccesoAsync(AccesoRequestDTO request);
    }
}
