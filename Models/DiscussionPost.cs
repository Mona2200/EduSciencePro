namespace EduSciencePro.Models
{
   public class DiscussionPost
   {
      public Guid Id { get; set; } = Guid.NewGuid();
      public string Title { get; set; }
      public byte[] Content { get; set; }
      public DateTime CreatedDate { get; set; }
      public Guid UserId { get; set; }
   }
}
