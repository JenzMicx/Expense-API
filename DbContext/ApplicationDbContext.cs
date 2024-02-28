using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth_API.Model.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auth_API.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<AddUserFieldModel>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
    }
}