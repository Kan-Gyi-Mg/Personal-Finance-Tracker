using FinanceTracker.Models.Operations;
using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.ViewModels.operationView
{
    public class UserCashViewModel
    {
        [Required]
        public List<UserCashModel> usercash {  get; set; }
        [Required]
        public int total { get; set; } = 0;
    }
}
