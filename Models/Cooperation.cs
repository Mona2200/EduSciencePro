using EduSciencePro.Models.User;

namespace EduSciencePro.Models
{
   public class Cooperation
   {
      public Guid Id { get; set; } = Guid.NewGuid();
      public string Name { get; set; }
      public Guid OrganizationId { get; set; }
      public string Role { get; set; }
      public DateTime EndDate { get; set; }
      public string Description { get; set; }
      public string Requirement { get; set; }
      public string Conditions { get; set; }
      public int Cost { get; set; }
      public string Contacts { get; set; }
   }
}
