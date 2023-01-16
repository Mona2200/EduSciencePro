namespace EduSciencePro.Models
{
   public class TagConference
   {
      public Guid Id { get; set; } = Guid.NewGuid();
      public Guid ConferenceId { get; set; }
      public Guid TagId { get; set; }
   }
}
