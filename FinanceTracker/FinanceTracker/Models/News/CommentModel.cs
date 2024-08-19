using FinanceTracker.Models.User;
using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Models.News
{
    public class CommentModel
    {
        [Key]
        [Required]
        public int Commentid {  get; set; }
        [Required]
        public int newsid {  get; set; }
        [Required]
        public string userid { get; set; }
        [Required]
        public string username { get; set; }
        [Required]
        public string CommentBody{get; set; }

    }
}
