namespace EduSciencePro.Models
{
    public class CourseSkill
    {
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CourseId { get; set; }
    public Guid SkillId { get; set; }
    }
}
