using EduSciencePro.Models.User;
using System.ComponentModel.DataAnnotations;

namespace EduSciencePro.ViewModels.Request
{
   public class AddResumeViewModel
   {
      [Display(Name = "Образование")]
      public string? Education { get; set; }

      [Display(Name = "Год выпуска")]
      [RegularExpression(@"^(1|2)\d{3}$", ErrorMessage = "Неверный формат ввода")]
      public string? DateGraduationEducation { get; set; }

      [Display(Name = "Специализация")]
      public string? Specialization { get; set; }

      [Display(Name = "Место работы")]
      public string? PlaceWork { get; set; }

      [Display(Name = "Организация")]
      public string? Organization { get; set; }

      [Display(Name = "Навыки")]
      public string Skills { get; set; } = "";

      [Display(Name = "О себе")]
      public string? AboutYourself { get; set; }
   }
}
