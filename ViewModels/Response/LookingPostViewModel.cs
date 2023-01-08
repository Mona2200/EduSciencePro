using EduSciencePro.ViewModels.Request;

namespace EduSciencePro.ViewModels.Response
{
    public class LookingPostViewModel
    {
    public Guid UserId { get; set; }
    public PostViewModel Post { get; set; }
    public AddCommentViewModel AddComment { get; set; }
    }
}
