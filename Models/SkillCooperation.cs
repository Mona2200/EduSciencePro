namespace EduSciencePro.Models
{
    public class SkillCooperation
    {
    public Guid Id { get; set; } = Guid.NewGuid();
      public Guid SkillId { get; set; }
      public Guid CooperationId { get; set; }
    }
}
