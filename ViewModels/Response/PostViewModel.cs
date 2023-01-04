using EduSciencePro.Models;

namespace EduSciencePro.ViewModels.Response
{
    public class PostViewModel
    {
      public string Title { get; set; }
      public string Content { get; set; }
      public DateTime CreatedDate { get; set; }
      public byte[]? Cover { get; set; }
      public bool IsNews { get; set; }
      public Tag[]? Tags { get; set; }
      public Guid UserId { get; set; }
   }
}
