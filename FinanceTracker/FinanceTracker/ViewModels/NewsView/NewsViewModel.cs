using FinanceTracker.Models.News;
using FinanceTracker.Models.User;

namespace FinanceTracker.ViewModels.NewsView
{
    public class NewsViewModel
    {
        public NewsModel news { get; set; }
        public List<CommentModel> Comments { get; set; }
        public string? cbody { get; set; }
    }
}
