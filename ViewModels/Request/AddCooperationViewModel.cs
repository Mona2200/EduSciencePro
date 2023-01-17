using EduSciencePro.Models.User;
using System.ComponentModel.DataAnnotations;

namespace EduSciencePro.ViewModels.Request
{
    public class AddCooperationViewModel
    {
      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Название")]
      [MinLength(8, ErrorMessage = "Поле {0} должно иметь минимум {1} символов")]
      public string Name { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Организация")]
      public string OrganizationName { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Роль")]
      public string Role { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Дата окончания работ")]
      [DataType(DataType.Date)]
      public string EndDate { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Описание услуги, продукции")]
      [MinLength(10, ErrorMessage = "Поле {0} должно иметь минимум {1} символов")]
      public string Description { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Описание необходимых работ")]
      [MinLength(10, ErrorMessage = "Поле {0} должно иметь минимум {1} символов")]
      public string Requirement { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Условия выполнения")]
      [MinLength(10, ErrorMessage = "Поле {0} должно иметь минимум {1} символов")]
      public string Conditions { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Стоимость выполнения работ")]
      [DataType(DataType.Duration)]
      public int Cost { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Электронная почта для связи")]
      [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Используйте формат example@email.ru")]
      public string Contacts { get; set; }

      [Display(Name = "Ключевые навыки")]
      public string? Skills { get; set; }

      public string? MinEndDate { get; set; }
   }
}
