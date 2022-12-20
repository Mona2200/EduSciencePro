namespace EduSciencePro.Models.User
{
    public class ResumeSkill
    {
      public Guid Id { get; set; } = Guid.NewGuid();
      public Guid ResumeId { get; set; }
      public Guid SkillId { get; set; }
    }
}
