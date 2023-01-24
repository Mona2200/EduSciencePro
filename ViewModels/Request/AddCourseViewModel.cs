using System.ComponentModel.DataAnnotations;

namespace EduSciencePro.ViewModels.Request
{
    public class AddCourseViewModel
    {
      [Display(Name = "Образование")]
      public string Education { get; set; }

      [Display(Name = "Место работы")]
      public string PlaceWork { get; set; }

      [Display(Name = "Специализация")]
      public string? Specialization { get; set; }

      [Display(Name = "Пройденные курсы/программы повышения квалификации и переподготовки")]
      public string? CompletedCourses { get; set; }

      [Display(Name = "Необходимость в получении дополнительных навыков")]
      public int NeedSkills { get; set; }

      [Display(Name = "Навыки")]
      public string? Skills { get; set; }
   }
}
