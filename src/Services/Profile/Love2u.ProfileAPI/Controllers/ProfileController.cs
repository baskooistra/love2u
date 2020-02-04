using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Love2u.ProfileAPI.Controllers
{
    [ApiController]
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
            return new[] { "Test value 1", "Test value 2" };
        }
    }
}
