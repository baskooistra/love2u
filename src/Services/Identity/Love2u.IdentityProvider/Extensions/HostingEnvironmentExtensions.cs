using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Love2u.IdentityProvider.Extensions
{
    internal static class HostingEnvironmentExtensions
    {
        internal static bool IsDockerHosted(this IWebHostEnvironment hostingEnvironment)
        {
            if (hostingEnvironment == null)
            {
                throw new ArgumentNullException(nameof(hostingEnvironment));
            }

            return hostingEnvironment.IsHostedIn(HostingEnvironmentConstants.DOCKER);
        }

        internal static bool IsHostedIn(this IWebHostEnvironment hostingEnvironment, string hostName) 
        {
            if (hostingEnvironment == null)
            {
                throw new ArgumentNullException(nameof(hostingEnvironment));
            }

            string host = Environment.GetEnvironmentVariable("ASPNETCORE_HOST");

            if (host == null) 
            {
                throw new ArgumentNullException(nameof(host));
            }

            return string.Equals(
                host,
                hostName,
                StringComparison.OrdinalIgnoreCase);
        }
    }

    internal static class HostingEnvironmentConstants 
    {
        internal const string DOCKER = "Docker";
        internal const string IIS = "IIS";
        internal const string PROJECT = "Project";
    }
}
