using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Response;

namespace EduSciencePro.Data.Repos
{
   public interface IRoleRepository
   {
      Task<Role[]> GetRoles();
      Task<Role> GetRoleById(Guid id);

      Task<Role> GetRoleByName(string name);
      Task Save(Role user);
      Task Delete(Role user);
   }
}
