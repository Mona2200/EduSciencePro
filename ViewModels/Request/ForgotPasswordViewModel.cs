using System.ComponentModel.DataAnnotations;
namespace EduSciencePro.ViewModels.Request
{
   public class ForgotPasswordViewModel
   {
      [Required(ErrorMessage = "Необходимо указать вашу электронную почту, указанную при регистрации")]
      [DataType(DataType.EmailAddress)]
      [Display(Name = "Email")]
      public string Email { get; set; }

      [Display(Name = "Код подтверждения")]
      [Required(ErrorMessage = "Введите код подтверждения")]
      public string ConfirmationCode { get; set; }
   }

   public class ChangePasswordViewModel
   {
      public string Email { get; set; }
      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [DataType(DataType.Password)]
      [Display(Name = "Новый пароль")]
      [StringLength(20, ErrorMessage = "Поле {0} должно иметь минимум {2} и максимум {1} символов.", MinimumLength = 8)]
      [RegularExpression("^(?=.*?[A-Za-zа-яА-Я])(?=.*?[0-9]).{8,}$", ErrorMessage = "Пароль должен содержать цифры и буквы любого регистра")]
      public string NewPassword { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
      [DataType(DataType.Password)]
      [Display(Name = "Подтвердить пароль")]
      public string PasswordConfirm { get; set; }
   }
}
