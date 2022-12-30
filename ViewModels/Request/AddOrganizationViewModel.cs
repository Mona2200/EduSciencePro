using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace EduSciencePro.ViewModels.Request
{
   public class AddOrganizationViewModel
   {
      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Название")]
      public string Name { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Описание")]
      public string Description { get; set; }
   }
}
