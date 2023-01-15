using EduSciencePro.Models.User;
using System.ComponentModel.DataAnnotations;

namespace EduSciencePro.ViewModels.Request
{
   public class AddProjectViewModel
   {
      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Название")]
      [MinLength(8, ErrorMessage  = "Поле {0} должно иметь минимум {1} символов")]
      public string Title { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Дата начала")]
      [DataType(DataType.Date)]
      public string StartDate { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Дата окончания")]
      [DataType(DataType.Date)]
      public string EndDate { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Название организации")]
      public string OrganizationName { get; set; }

      [Display(Name = "Предлагаемый доход")]
      [DataType(DataType.Duration)]
      public int? Income { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Описание")]
      [MinLength(10, ErrorMessage = "Поле {0} должно иметь минимум {1} символов")]
      public string Description { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Необходимые компетенции")]
      [MinLength(10, ErrorMessage = "Поле {0} должно иметь минимум {1} символов")]
      public string Competencies { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Условия по выполнению работ")]
      [MinLength(10, ErrorMessage = "Поле {0} должно иметь минимум {1} символов")]
      public string Conditions { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Электронная почта для связи")]
      [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Используйте формат example@email.ru")]
      public string Contacts { get; set; }

      [Display(Name = "Требуемые навыки")]
      public string? Skills { get; set; }

      public string? minStartDate { get; set; }
      public string? minEndDate { get; set; }
   }
}
