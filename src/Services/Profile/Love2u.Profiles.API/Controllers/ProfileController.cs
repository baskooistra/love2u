using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Love2u.Profiles.API;
using Love2u.Profiles.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Love2u.ProfileAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("user/profile")]
    public class ProfileController : ControllerBase
    {
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(ILogger<ProfileController> logger)
        {
            _logger = logger;
        }

        [HttpGet("userprofile")]
        public IEnumerable<string> Get()
        {
            try
            {
                _logger.LogInformation("Start fetching user profile.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            return new[] { "Test value 1", "Test value 2" };
        }

        [HttpPost("userprofile")]
        public async Task<UserProfile> Post()
        {
            var userId = User.Claims.Single(c => c.Type == "sub").Value;

            var profile = new UserProfile
            {
                Id = new Guid(), UserId = Guid.Parse(userId), Description = "Test description"
            };

            try
            {
                _logger.LogInformation("Start indexing user profile.");
                var repo = new ElasticRepository();
                await repo.SaveProfile(profile);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return profile;
        }
    }
}
