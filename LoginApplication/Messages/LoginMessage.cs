using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApplication.Messages
{
    public partial class LoginMessage
    {
        public Boolean IsSuccess { get; set; }
        public String Message { get; set; }

        public LoginMessage() { }

        public LoginMessage(Boolean isSuccess, String message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

    }
}
