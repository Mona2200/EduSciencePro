using System.ComponentModel.DataAnnotations.Schema;

namespace EduSciencePro.Models.User
{
   public class TypeUser
    {
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid TypeId { get; set; }
    }
}
