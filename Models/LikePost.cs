namespace EduSciencePro.Models
{
    public class LikePost
    {
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    }
}
