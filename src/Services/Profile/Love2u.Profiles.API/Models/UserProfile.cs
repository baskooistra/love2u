using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Love2u.Profiles.API.Models
{
    public class UserProfile
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Description { get; set; }
    }
}
