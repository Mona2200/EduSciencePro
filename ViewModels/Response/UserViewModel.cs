using EduSciencePro.Models.User;

namespace EduSciencePro.ViewModels.Response
{
   public class UserViewModel
   {
      public Guid Id { get; set; }
      public string FullName { get; set; }
      public string Gender { get; set; }
      public string Birthday { get; set; }
      public List<TypeModel> TypeUsers { get; set; }
      public string Email { get; set; }
      public byte[] Image { get; set; }
      public List<Link>? Links { get; set; }
      public ResumeViewModel? Resume { get; set; }
      public Role Role { get; set; }

      public List<PostViewModel>? Posts { get; set; }
   }
}
