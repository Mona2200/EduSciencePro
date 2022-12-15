using EduSciencePro.Models.User;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduSciencePro.ViewModels.Request
{
   public class AddUserViewModel
   {
      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Имя")]
      [StringLength(50, ErrorMessage = "Поле {0} должно иметь минимум {2} и максимум {1} символов.", MinimumLength = 2)]
      [DataType(DataType.Text)]
      public string FirstName { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Фамилия")]
      [StringLength(50, ErrorMessage = "Поле {0} должно иметь минимум {2} и максимум {1} символов.", MinimumLength = 2)]
      [DataType(DataType.Text)]
      public string LastName { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Отчество")]
      [StringLength(50, ErrorMessage = "Поле {0} должно иметь минимум {2} и максимум {1} символов.", MinimumLength = 2)]
      [DataType(DataType.Text)]
      public string MiddleName { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Пол")]
      [DataType(DataType.Text)]
      public string Gender { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Дата рождения")]
      [DataType(DataType.Date)]
      public string Birthday { get; set; }

      [Required(ErrorMessage = "Выберите тип пользователя")]
      [Display(Name = "Тип пользователя")]
      public string TypeUsers { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Email")]
      [EmailAddress]
      public string Email { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [DataType(DataType.Password)]
      [Display(Name = "Пароль")]
      [StringLength(20, ErrorMessage = "Поле {0} должно иметь минимум {2} и максимум {1} символов.", MinimumLength = 8)]
      public string Password { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Compare("Password", ErrorMessage = "Пароли не совпадают")]
      [DataType(DataType.Password)]
      [Display(Name = "Подтвердить пароль")]
      public string PasswordConfirm { get; set; }

      [Display(Name = "Ссылки")]
      public string Links { get; set; } = "";

      [Display(Name = "Фото профиля")]
      [NotMapped]
      public IFormFile? Img { get; set; }

      public AddUserViewModel()
      {
         var month = $"{DateTime.Now.Month}";
         if (month.Length == 1)
            month = $"0{month}";
         var day = $"{DateTime.Now.Day}";
         if (day.Length == 1)
            day = $"0{day}";
         Birthday = $"{DateTime.Now.Year}-{month}-{day}";
      }
   }
}
