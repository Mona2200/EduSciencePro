using System.ComponentModel.DataAnnotations.Schema;

namespace EduSciencePro.Models.User
{
   public class Link
    {
    public Guid Id { get; set; } = Guid.NewGuid();
      public string Name { get; set; } = "";
    public string Url { get; set; }
    public Guid UserId { get; set; }
    }
}
