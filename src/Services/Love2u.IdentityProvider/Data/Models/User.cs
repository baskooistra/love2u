using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Love2u.IdentityProvider.Data.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string FamilyName { get; set; }
    }
}
