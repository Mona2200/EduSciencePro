namespace EduSciencePro.Models
{
   public class ProjectSkill
   {
   public Guid Id { get; set; } = Guid.NewGuid();
   public Guid ProjectId { get; set; }
   public Guid SkillId { get; set; }
   }
}
