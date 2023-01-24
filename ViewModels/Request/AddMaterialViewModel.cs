using System.ComponentModel.DataAnnotations;

namespace EduSciencePro.ViewModels.Request
{
    public class AddMaterialViewModel
    {
      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Название")]
      [MinLength(8, ErrorMessage = "Поле {0} должно иметь минимум {1} символов")]
      public string Title { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Специализация")]
      public string Specialization { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Тип учебного материала")]
      public string Type { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Адрес электронного ресурса")]
      [DataType(DataType.Url)]
      public string Url { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Аннотация")]
      [MinLength(8, ErrorMessage = "Поле {0} должно иметь минимум {1} символов")]
      public string Annotation { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Место издания (организация)")]
      public string Publication { get; set; }

      [Display(Name = "Теги")]
      public string? Tags { get; set; }
   }
}
