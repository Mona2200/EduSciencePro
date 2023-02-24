namespace EduSciencePro.Models
{
    public class Notification
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Content { get; set; } = "";
        public Guid UserId { get; set; }
    }
}
