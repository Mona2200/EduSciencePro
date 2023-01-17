namespace EduSciencePro.Models
{
    public class SkillInternship
    {
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid IntershipId { get; set; }
    public Guid SkillId { get; set; }
    }
}
