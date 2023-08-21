using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Response;

namespace EduSciencePro.ViewModels.Request
{
   public class EditUserViewModel
   {
   public UserViewModel UserViewModel { get; set; }
   public AddUserViewModel AddUserViewModel { get; set; }
   public EditResumeViewModel EditResumeViewModel { get; set; }
      public List<TypeModel>? Types { get; set; }

      public bool UserConsent { get; set; }
   }
}
