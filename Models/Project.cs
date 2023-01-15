namespace EduSciencePro.Models
{
   public class Project
   {
   public Guid Id { get; set; } = Guid.NewGuid();
   public string Title { get; set; }
   public DateTime StartDate { get; set; }
   public DateTime EndDate { get; set; }
   public Guid OrganizationId { get; set; }
   public int? Income { get; set; }
   public string Description { get; set; }
   public string Competencies { get; set; }
   public string Conditions { get; set; }
   public string Contacts { get; set; }
   }
}
