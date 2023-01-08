using System.ComponentModel.DataAnnotations;

namespace EduSciencePro.ViewModels.Request
{
   public class EditPostViewModel
   {
      public Guid Id { get; set; }
      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Название")]
      public string Title { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Содержание")]
      public string Content { get; set; }

      [Display(Name = "Обложка")]
      public IFormFile? Cover { get; set; }

      [Display(Name = "Теги")]
      public string? Tags { get; set; }

      public bool IsNew { get; set; }

      public Guid? UserId { get; set; }
   }
}
