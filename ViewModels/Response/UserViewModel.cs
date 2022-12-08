using EduSciencePro.Models.User;

namespace EduSciencePro.ViewModels.Response
{
   public class UserViewModel
   {
      public Guid Id { get; set; }
      public string FullName { get; set; }
      public string Gender { get; set; }
      public string Birthday { get; set; }
      public TypeModel[] TypeUsers { get; set; }
      public string Email { get; set; }
      public Link[] Links { get; set; }
      public Resume Resume { get; set; }
      public Role Role { get; set; }
   }
}
