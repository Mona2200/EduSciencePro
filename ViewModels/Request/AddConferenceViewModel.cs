using System.ComponentModel.DataAnnotations;

namespace EduSciencePro.ViewModels.Request
{
    public class AddConferenceViewModel
    {
      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Название")]
      [MinLength(8, ErrorMessage = "Поле {0} должно иметь минимум {1} символов")]
      public string Title { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Дата проведения")]
      [DataType(DataType.Date)]
      public string EventDate { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Название организации")]
      public string OrganizationName { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Форма участия")]
      public string ParticipationForm { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Цели и задачи")]
      [MinLength(10, ErrorMessage = "Поле {0} должно иметь минимум {1} символов")]
      public string Goals { get; set; }

      [Display(Name = "Дополнительная информация")]
      [MinLength(10, ErrorMessage = "Поле {0} должно иметь минимум {1} символов")]
      public string? Information { get; set; }

      [Display(Name = "Программа конференции")]
      [MinLength(10, ErrorMessage = "Поле {0} должно иметь минимум {1} символов")]
      public string? Program { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Электронная почта для связи")]
      [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Используйте формат example@email.ru")]
      public string Contacts { get; set; }

      [Display(Name = "Основные направления")]
      public string? Tags { get; set; }

      public string? MinEventDate { get; set; }
   }
}
