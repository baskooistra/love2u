﻿using IdentityServer4.Models;
using IdentityServer4.Services;
using Love2u.IdentityProvider.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static IdentityServer4.IdentityServerConstants;

namespace Love2u.IdentityProvider.Services
{
    internal class ProfileService : IProfileService
    {
        protected readonly ILogger Logger;
        protected UserManager<User> UserManager;

        public ProfileService(ILogger<DefaultProfileService> logger, UserManager<User> userManager)
        {
            Logger = logger;
            UserManager = userManager;
        }

        async Task IProfileService.GetProfileDataAsync(ProfileDataRequestContext context)
        {
            context.LogProfileRequest(Logger);
            var user = await UserManager.GetUserAsync(context.Subject);
            var scopes = context.RequestedResources.IdentityResources.Select(resource => resource.Name);
            List<Claim> claims = new List<Claim>();
            foreach (Claim claim in context.Subject.Claims) 
            {
                claims.Add(claim);
            }
            
            if (scopes.Contains(StandardScopes.Email))
                claims.Append(new Claim(ClaimTypes.Email, user.Email));
            context.IssuedClaims.AddRange(claims);
            context.LogIssuedClaims(Logger);
        }

        Task IProfileService.IsActiveAsync(IsActiveContext context)
        {
            Logger.LogDebug("IsActive called from: {caller}", context.Caller);
            context.IsActive = true;
            return Task.CompletedTask;
        }
    }
}
