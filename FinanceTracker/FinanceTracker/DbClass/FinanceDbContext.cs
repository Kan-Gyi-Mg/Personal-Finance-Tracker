using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FinanceTracker.Models.User;
using FinanceTracker.Models.News;
using FinanceTracker.Models.Operations;

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
        public DbSet<CommentModel> commentss { get; set; }
        public DbSet<UserCashModel> usercash { get; set; }
        public DbSet<FBankModel> fbank { get; set; }
    }
}
