using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApplication.ImageProcessing
{
    internal interface IImageHandler
    {
        IImageHandler SetNext(IImageHandler next);
        Task<Stream> ProcessAsync(Stream image);
    }
}
