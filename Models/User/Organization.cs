using System.ComponentModel.DataAnnotations.Schema;

namespace EduSciencePro.Models.User
{
   public class Organization
   {
   public Guid Id { get; set; } = Guid.NewGuid();
   public string Name { get; set; }
   public string Description { get; set; }

   public Guid LeaderId { get; set; }
   }
}
