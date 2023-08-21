namespace EduSciencePro.Models;

public class Comment : IModel
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string Content { get; set; }
  public DateTime CreatedDate { get; set; }
  public Guid PostId { get; set; }
  public Guid UserId { get; set; }
}
