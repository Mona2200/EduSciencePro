using EduSciencePro.Models;

namespace EduSciencePro.ViewModels.Response
{
    public class PostsAndUserIdViewModel
    {
    public Guid? UserId { get; set; }
    public List<PostViewModel> Posts { get; set; }
        public List<string> Tags { get; set; } = new();
    public string? AddTag { get; set; }
    }
}
