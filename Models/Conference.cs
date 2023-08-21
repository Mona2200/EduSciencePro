namespace EduSciencePro.Models;

public class Conference : IModel
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string Title { get; set; }
  public DateTime EventDate { get; set; }
  public string ParticipationForm { get; set; }
  public Guid OrganizationId { get; set; }
  public string Goals { get; set; }
  public string? Information { get; set; }
  public string? Program { get; set; }
  public string Contacts { get; set; }
}
