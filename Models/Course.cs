namespace EduSciencePro.Models
{
    public class Course
    {
      public Guid Id { get; set; } = Guid.NewGuid();
      public Guid EducationId { get; set; }
      public Guid PlaceWorkId { get; set; }
      public string? Specialization { get; set; }
      public string? CompletedCourses { get; set; }
      public int NeedSkills { get; set; }
      public Guid UserId { get; set; }
   }
}
