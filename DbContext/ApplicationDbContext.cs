using Auth_API.Model.Entities;
using Auth_API.Model.Expenses;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auth_API.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<AddUserFieldModel>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<ExpenseModel> ExpenseDB {get; set;}
    }
}