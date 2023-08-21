using System.ComponentModel.DataAnnotations.Schema;

namespace EduSciencePro.Models.User;

public class Education : IModel
{
public Guid Id { get; set; } = Guid.NewGuid();
public string Name { get; set; }
}
