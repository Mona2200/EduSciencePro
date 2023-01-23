namespace EduSciencePro.Models
{
    public class Dialog
    {
    public Guid Id { get; set; } = Guid.NewGuid();
    public bool isLooked { get; set; }
    public Guid LastMessageId { get; set; }
    public Guid InterlocutorFirstId { get; set; }
      public Guid InterlocutorSecondId { get; set; }
   }
}
