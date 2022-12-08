namespace EduSciencePro.Models.User
{
   public class UserOrganization
   {
   public Guid Id { get; set; } = Guid.NewGuid();
   public Guid IdOrganization { get; set; }
   public Guid IdUser { get; set; }
   }
}
