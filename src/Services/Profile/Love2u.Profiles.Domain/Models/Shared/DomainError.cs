using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Love2u.Profiles.Domain.Models.Shared
{
    public class DomainError
    {
        public ErrorType Type { get; set; }

        public string Message { get; set; }
    }
}
