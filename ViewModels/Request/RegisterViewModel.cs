using System.ComponentModel.DataAnnotations;
using TypeModel = EduSciencePro.Models.User.TypeModel;

namespace EduSciencePro.ViewModels.Request
{
   public class RegisterViewModel
   {
      public AddUserViewModel AddUserViewModel { get; set; }

      public List<TypeModel>? Types { get; set; }

      public bool Consent { get; set; }
   }
}
