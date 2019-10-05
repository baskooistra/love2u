using IdentityServer4;
using Love2u.IdentityProvider.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Love2u.IdentityProvider.Services
{
    internal class ClaimsPrincipalFactory : UserClaimsPrincipalFactory<User>, IUserClaimsPrincipalFactory<User>
    {
        public ClaimsPrincipalFactory(UserManager<User> userManager, IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, optionsAccessor)
        {

        }

        public async override Task<ClaimsPrincipal> CreateAsync(User user)
        {
            var principal = await base.CreateAsync(user);

            ((ClaimsIdentity)principal.Identity).AddClaims(new[] 
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id)
            });

            return principal;
        }
    }
}
