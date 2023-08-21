using EduSciencePro.Models;
using EduSciencePro.Models.User;

namespace EduSciencePro.ViewModels.Response
{
   public class ProjectViewModel
   {
      public Guid Id { get; set; }
      public string Title { get; set; }
      public string StartDate { get; set; }
      public string EndDate { get; set; }
      public Organization Organization { get; set; }
      public int? Income { get; set; }
      public string Description { get; set; }
      public string Competencies { get; set; }
      public string Conditions { get; set; }
      public string Contacts { get; set; }
      public List<Skill>? Skills { get; set; }
   }
}
