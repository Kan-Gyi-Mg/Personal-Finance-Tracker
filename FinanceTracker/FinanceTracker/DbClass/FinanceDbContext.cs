using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FinanceTracker.Models.User;

namespace FinanceTracker.DbClass
{
    public class FinanceDbContext : IdentityDbContext<FinanceUser>
    {
        public FinanceDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<FinanceUser> financeusers { get; set; }
    }
}
