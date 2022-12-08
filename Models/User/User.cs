using System.ComponentModel.DataAnnotations.Schema;

namespace EduSciencePro.Models.User
{
   public class User
   {
      public Guid Id { get; set; } = Guid.NewGuid();
      public string FirstName { get; set; }
      public string LastName { get; set; }
      public string MiddleName { get; set; }
      public string Gender { get; set; }
      public string Birthday { get; set; }
      public string Email { get; set; }
      public string Password { get; set; }
      public byte[]? Image { get; set; }
      public Guid? ResumeId { get; set; }
      public Guid RoleId { get; set; }
   }
}
