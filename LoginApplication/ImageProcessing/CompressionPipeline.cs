using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApplication.ImageProcessing
{
    internal class CompressionPipeline
    {
        private IImageHandler _firstHandler;
        private IImageHandler _lastHandler;

        public CompressionPipeline AddHandler(IImageHandler handler)
        {
            if (_firstHandler == null)
            {
                _firstHandler = handler;
                _lastHandler = handler;
            }
            else
            {
                _lastHandler.SetNext(handler);
                _lastHandler = handler;
            }

            return this;
        }

        public Task<Stream> ProcessAsync(Stream imageStream)
        {
            if (_firstHandler == null)
                throw new InvalidOperationException("Debe agregar al menos 1 handler al pipeline.");

            return _firstHandler.ProcessAsync(imageStream);
        }
    }
}
