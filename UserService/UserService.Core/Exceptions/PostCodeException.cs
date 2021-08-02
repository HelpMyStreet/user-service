using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Core.Exceptions
{
    public class PostCodeException : Exception
    {
        public PostCodeException() : base("Invalid postcode")
        {
        }
    }
}
