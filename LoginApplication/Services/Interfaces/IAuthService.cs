using LoginApplication.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApplication.Services.Interfaces
{
    public interface IAuthService
    {
        Task<Boolean> LoginAsync(LoginRequestDTO request);
    }
}
