namespace EduSciencePro.Models
{
    public class Internship
    {
      public Guid Id { get; set; } = Guid.NewGuid();
      public string Name { get; set; }
      public Guid OrganizationId { get; set; }
      public string? Division { get; set; }
      public DateTime StartDate { get; set; }
      public DateTime EndDate { get; set; }
      public string EmploymentType { get; set; }
      public int Salary { get; set; }
      public string Description { get; set; }
      public string Competencies { get; set; }
      public string Conditions { get; set; }
      public string Responsibility { get; set; }
      public string Development { get; set; }
      public string Contacts { get; set; }
   }
}
