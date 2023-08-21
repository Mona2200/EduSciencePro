using EduSciencePro.Models;

namespace EduSciencePro.ViewModels.Response
{
    public class MaterialViewModel
    {
      public Guid Id { get; set; }
      public string Title { get; set; }
      public string Specialization { get; set; }
      public string Type { get; set; }
      public string Url { get; set; }
      public string Annotation { get; set; }
      public string Publication { get; set; }
      public List<Tag>? Tags { get; set; }
   }
}
