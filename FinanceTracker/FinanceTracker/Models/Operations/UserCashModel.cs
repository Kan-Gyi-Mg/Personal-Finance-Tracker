using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Models.Operations
{
    public class UserCashModel
    {
        [Key]
        public int cashId { get; set; }
        [Required]
        public string Category {  get; set; }
        [Required]
        public int Amount {  get; set; }
        [Required]
        public string UserId { get; set; }
    }
}
