using EduSciencePro.Models.User;

namespace EduSciencePro.ViewModels.Response
{
    public class InternshipViewModel
    {
      public Guid Id { get; set; }
      public string Name { get; set; }
      public Organization Organization { get; set; }
      public string EmploymentType { get; set; }
      public int Salary { get; set; }
      public string? Division { get; set; }
      public string StartDate { get; set; }
      public string EndDate { get; set; }
      public string Description { get; set; }
      public string Competencies { get; set; }
      public string Conditions { get; set; }
      public string Responsibility { get; set; }
      public string Development { get; set; }
      public string Contacts { get; set; }
      public Skill[] Skills { get; set; }
   }
}
