using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Love2u.Profiles.InfraStructure.Extensions
{
    internal static class HttpStatusCodeExtensions
    {
        internal static bool IsSuccessStatusCode(this HttpStatusCode statusCode)
        {
            int status = (int)statusCode;
            return status >= 200 && status < 300;
        }
    }
}
