using System.ComponentModel.DataAnnotations;

namespace EduSciencePro.ViewModels.Request
{
    public class AddPostViewModel
    {
      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Название")]
      public string Title{ get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Содержание")]
      public string Content{ get; set; }

      [Display(Name = "Обложка")]
      public byte[]? Cover { get; set; }

      public string? Tags { get; set; }
    }
}
