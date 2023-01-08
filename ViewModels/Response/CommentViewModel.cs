using EduSciencePro.Models.User;

namespace EduSciencePro.ViewModels.Response
{
    public class CommentViewModel
    {
      public Guid Id { get; set; }
      public string Content { get; set; }
      public string CreatedDate { get; set; }
      public Guid PostId { get; set; }
      public User User { get; set; }
   }
}
