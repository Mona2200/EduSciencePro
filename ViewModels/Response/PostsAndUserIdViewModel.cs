using EduSciencePro.Models;

namespace EduSciencePro.ViewModels.Response
{
    public class PostsAndUserIdViewModel
    {
    public Guid? UserId { get; set; }
    public PostViewModel[] Posts { get; set; }
    public Tag[] Tags { get; set; }
    public string? AddTag { get; set; }
    }
}
