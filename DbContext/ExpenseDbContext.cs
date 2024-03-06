using Auth_API.Model.Expenses;
using Microsoft.EntityFrameworkCore;

namespace Auth_API.DbContext
{
    public class ExpenseDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public ExpenseDbContext(DbContextOptions<ExpenseDbContext> options) : base(options)
        {

        }

        public DbSet<ExpenseModel> ExpenseDB { get; set; }
    }
}
