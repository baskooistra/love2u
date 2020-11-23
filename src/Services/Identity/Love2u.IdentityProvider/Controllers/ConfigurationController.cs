using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using Love2u.IdentityProvider.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Love2u.IdentityProvider.Controllers
{
    public class ConfigurationController : Controller
    {
        private readonly ILogger<ConfigurationController> logger;

        public ConfigurationController(IConfiguration configuration, ILogger<ConfigurationController> _logger)
        {
            Configuration = configuration;
            logger = _logger;
        }

        public IConfiguration Configuration { get; }

        [HttpGet("_configuration/{clientId}")]
        public IActionResult GetClientRequestParameters([FromRoute]string clientId)
        {
            var parameters = new Dictionary<string, string>();
            var client = IdentityServerConfiguration.Clients(Configuration).Single(c => c.ClientId == clientId);
            if (client == null)
                return NotFound("No client was found with client ID.");
            string responseType = GetResponseType(client.AllowedGrantTypes);
            if (responseType == null)
                responseType = "";
            
            parameters.Add("authority", HttpContext.GetIdentityServerIssuerUri());
            parameters.Add("client_id", clientId);
            parameters.Add("redirect_uri", client.RedirectUris.First());
            parameters.Add("post_logout_redirect_uri", client.PostLogoutRedirectUris.First());
            parameters.Add("response_type", responseType);
            parameters.Add("scope", string.Join(" ", client.AllowedScopes));
            
            return Ok(parameters);
        }

        private string GetResponseType(ICollection<string> grantTypes) 
        {
            if (grantTypes.Contains(GrantType.AuthorizationCode))
                return "code";
            else if (grantTypes.Contains(GrantType.Hybrid))
                return "code id_token";
            else if (grantTypes.Contains(GrantType.Implicit))
                return "id_token token";
            else
                return null;
        }
    }
}