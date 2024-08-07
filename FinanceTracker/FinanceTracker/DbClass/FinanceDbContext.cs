using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FinanceTracker.Models.User;
using FinanceTracker.Models.News;

namespace FinanceTracker.DbClass
{
    public class FinanceDbContext : IdentityDbContext<FinanceUser>
    {
        public FinanceDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<FinanceUser> financeusers { get; set; }
        public DbSet<NewsModel> news { get; set; }
    }
}
