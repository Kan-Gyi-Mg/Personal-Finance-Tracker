using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Models.Operations
{
    public class FBankModel
    {
        [Key]
        public int FBankid { get; set; }
        [Required]
        public int BankAmount { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}
