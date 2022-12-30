using EduSciencePro.Models.User;
using Newtonsoft.Json.Serialization;
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
      [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Используйте формат example@email.ru")]
      [EmailAddress]
      public string Email { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [DataType(DataType.Password)]
      [Display(Name = "Пароль")]
      [StringLength(20, ErrorMessage = "Поле {0} должно иметь минимум {2} и максимум {1} символов", MinimumLength = 8)]
      [RegularExpression("^(?=.*?[A-Za-zа-яА-Я])(?=.*?[0-9]).{8,}$", ErrorMessage = "Пароль должен содержать цифры и буквы любого регистра")]
      public string Password { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Compare("Password", ErrorMessage = "Пароли не совпадают")]
      [DataType(DataType.Password)]
      [Display(Name = "Подтвердить пароль")]
      public string PasswordConfirm { get; set; }

      [RegularExpression(@"^(?=.{5,32}$)(?!.*__)(?!^(telegram|admin|support))[a-z][a-z0-9_]*[a-z0-9]$", ErrorMessage = "Вводите имя пользователя без @")]
      public string? TelegramLink { get; set; } = "";

      [RegularExpression("^\\+?\\d{1,4}?[-.\\s]?\\(?\\d{1,3}?\\)?[-.\\s]?\\d{1,4}[-.\\s]?\\d{1,4}[-.\\s]?\\d{1,9}$", ErrorMessage = "Используйте формат 8 (999) 999 99 99")]
      public string? WhatsAppLink { get; set; } = "";

      [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Используйте формат example@email.ru")]
      public string? EmailLink { get; set; } = "";

      [RegularExpression(@"^(http|https|ftp|)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$", ErrorMessage = "Используйте формат https://example.ru")]    
      public string? AnotherLink { get; set; } = "";

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
