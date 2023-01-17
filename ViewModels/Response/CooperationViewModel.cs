using EduSciencePro.Models.User;

namespace EduSciencePro.ViewModels.Response
{
    public class CooperationViewModel
    {
      public Guid Id { get; set; }
      public string Name { get; set; }
      public Organization Organization { get; set; }
      public string Role { get; set; }
      public string EndDate { get; set; }
      public string Description { get; set; }
      public string Requirement { get; set; }
      public string Conditions { get; set; }
      public int Cost { get; set; }
      public string Contacts { get; set; }
      public Skill[] Skills { get; set; }
   }
}
