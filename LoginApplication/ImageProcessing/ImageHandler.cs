using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApplication.ImageProcessing
{
    internal abstract class ImageHandler : IImageHandler
    {

        private IImageHandler _next;

        public IImageHandler SetNext(IImageHandler handler)
        {
            _next = handler;
            return handler;
        }

        public async Task<Stream> ProcessAsync(Stream imageStream)
        {
            // Process the current step
            var processedStream = await HandleAsync(imageStream);

            // Pass to the next handler in the chain
            if (_next != null)
            {
                return await _next.ProcessAsync(processedStream);
            }

            return processedStream;
        }

        protected abstract Task<Stream> HandleAsync(Stream image);
    }
}
