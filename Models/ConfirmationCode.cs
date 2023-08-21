namespace EduSciencePro.Models;

public class ConfirmationCode : IModel
{
public Guid Id { get; set; } = Guid.NewGuid();
public string Email { get; set; }
public string Code { get; set; }
}
