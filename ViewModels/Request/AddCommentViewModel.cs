using System.ComponentModel.DataAnnotations;

namespace EduSciencePro.ViewModels.Request
{
    public class AddCommentViewModel
    {
      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Комментарий")]
      public string Content { get; set; }
      public Guid PostId { get; set; }
      public Guid UserId { get; set; }
   }
}
