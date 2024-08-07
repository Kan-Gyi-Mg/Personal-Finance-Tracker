using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Models.News
{
    public class NewsModel
    {
        [Key]
        [Required]
        public string NewsId { get; set; }
        [Required]
        public string NewsTitle { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string NewsBody { get; set; }
        [Required]
        public string UserId {  get; set; }

    }
}
