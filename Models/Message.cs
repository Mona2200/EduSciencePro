namespace EduSciencePro.Models;

public class Message : IModel
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public DateTime CreateTime { get; set; }
  public string Content { get; set; }
  public bool isLooked { get; set; }
  public Guid DialogId { get; set; }
  public Guid SenderId { get; set; }
  public Guid RecipientId { get; set; }
}
