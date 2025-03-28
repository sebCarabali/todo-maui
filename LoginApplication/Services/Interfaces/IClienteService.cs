using LoginApplication.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApplication.Services.Interfaces
{
    public interface IClienteService
    {
        Task<string> GetEncodingAsync(string identifier);
        Task<bool> SetEncodingAsync(string identifier, byte[] photoBytes);
    }
}
