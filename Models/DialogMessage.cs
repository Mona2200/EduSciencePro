namespace EduSciencePro.Models
{
   public class DialogMessage
   {
      public Guid Id { get; set; } = Guid.NewGuid();
      public Guid MessageId { get; set; }
      public Guid DialogId { get; set; }
   }
}
