using Microsoft.AspNetCore.Identity;

namespace FinanceTracker.Models.User
{
    public class FinanceUser : IdentityUser
    {
        public bool? ForgotPassword { get; set; } = false;
    }
}
