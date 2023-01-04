namespace EduSciencePro.Models
{
   public class Post
   {
      public Guid Id { get; set; } = Guid.NewGuid();
      public string Title { get; set; }
      public byte[] Content { get; set; }
      public DateTime CreatedDate { get; set; }
      public byte[]? Cover { get; set; }
      public bool IsNews { get; set; }
      public Guid UserId { get; set; }
   }
}
