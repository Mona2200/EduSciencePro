using System.ComponentModel.DataAnnotations;

namespace EduSciencePro.ViewModels.Request
{
    public class AuthenticateViewModel
    {
      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Email")]
      public string Email { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [DataType(DataType.Password)]
      [StringLength(20, ErrorMessage = "Поле {0} должно иметь минимум {2} и максимум {1} символов.", MinimumLength = 8)]
      [Display(Name = "Пароль")]
      public string Password { get; set; }

      public bool RememberMe { get; set; }
   }
}
