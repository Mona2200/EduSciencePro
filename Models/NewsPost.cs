namespace EduSciencePro.Models
{
   public class NewsPost
   {
   public Guid Id { get; set; } = Guid.NewGuid();
   public string Title { get; set; }
   public byte[] Content { get; set; }
   public DateTime CreatedDate { get; set; }
   public byte[] Cover { get; set; }
   public Guid UserId { get; set; }
   }
}
