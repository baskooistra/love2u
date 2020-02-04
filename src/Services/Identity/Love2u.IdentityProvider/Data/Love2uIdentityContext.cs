using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Interfaces;
using Love2u.IdentityProvider.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Love2u.IdentityProvider.Data
{
    public class Love2uIdentityContext : IdentityDbContext<User>
    {
        public Love2uIdentityContext(DbContextOptions<Love2uIdentityContext> options)
            : base(options)
        {

        }
    }
}
