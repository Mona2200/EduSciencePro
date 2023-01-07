using EduSciencePro.Models;
using EduSciencePro.Models.User;

namespace EduSciencePro.ViewModels.Response
{
    public class PostViewModel
    {
      public string Title { get; set; }
      public string Content { get; set; }
      public string CreatedDate { get; set; }
      public byte[]? Cover { get; set; }
      public bool IsNews { get; set; }
      public Tag[]? Tags { get; set; }
      public User User { get; set; }
   }
}
