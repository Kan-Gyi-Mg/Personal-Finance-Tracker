using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.ViewModels.FinanceUserView
{
    public class OTPViewModel
    {
        [Required]
        public string OTP { get; set; }
    }
}
