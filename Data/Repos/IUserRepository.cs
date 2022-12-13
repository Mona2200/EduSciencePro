using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;

namespace EduSciencePro.Data.Repos
{
   public interface IUserRepository
   {
      Task<User[]> GetUsers();
      Task<User> GetUserById(Guid id);
      Task<User> GetUserByEmail(string email);
      Task<UserViewModel[]> GetUserViewModels();
      Task<UserViewModel> GetUserViewModelById(Guid id);
      Task<UserViewModel> GetUserViewModelByEmail(string email);
      Task Save(AddUserViewModel model);
      Task Update(AddUserViewModel model, User editUser);
      Task Delete(User user);
      Task DeleteImage(Guid userId);
   }
}
