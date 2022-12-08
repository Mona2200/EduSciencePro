using AutoMapper;
using EduSciencePro.Data.Repos;
using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Type = EduSciencePro.Models.User.TypeModel;

namespace EduSciencePro.Data.Repos.UserRepos
{
   public class UserRepository : IUserRepository
   {
      private readonly ApplicationDbContext _db;
      private readonly IMapper _mapper;

      public UserRepository(ApplicationDbContext db, IMapper mapper)
      {
         _db = db;
         _mapper = mapper;
      }

      public async Task<User[]> GetUsers()
      {
         return await _db.Users.ToArrayAsync();
      }

      public async Task<User> GetUserById(Guid id)
      {
         return await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
      }

      public async Task<User> GetUserByEmail(string email)
      {
         return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
      }

      public async Task<UserViewModel[]> GetUserViewModels()
      {
         var users = await _db.Users.ToArrayAsync();
         var userViewModels = new UserViewModel[users.Length];
         int i = 0;
         foreach (var user in users)
         {
            userViewModels[i] = _mapper.Map<User, UserViewModel>(user);
            var datebirth = DateOnly.Parse(user.Birthday);
            userViewModels[i].Birthday = $"{datebirth.Day}.{datebirth.Month}.{datebirth.Year}";
            var typeUsers = await _db.TypeUsers.Where(t => t.UserId == user.Id).ToArrayAsync();
            var types = new TypeModel[typeUsers.Length];
            int j = 0;
            foreach (var typeUser in typeUsers)
            {
               types[j++] = await _db.TypeModels.FirstOrDefaultAsync(t => t.Id == typeUser.TypeId);
            }
            userViewModels[i].TypeUsers = types;
            var links = await _db.Links.Where(l => l.UserId == user.Id).ToArrayAsync();
            userViewModels[i].Links = links;
            userViewModels[i].Resume = await _db.Resumes.FirstOrDefaultAsync(r => r.Id == user.ResumeId);
            userViewModels[i++].Role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == user.RoleId);
         }
         return userViewModels;
      }

      public async Task<UserViewModel> GetUserViewModelById(Guid id)
      {
         var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
         var userViewModel = _mapper.Map<User, UserViewModel>(user);
         var datebirth = DateOnly.Parse(user.Birthday);
         userViewModel.Birthday = $"{datebirth.Day}.{datebirth.Month}.{datebirth.Year}";
         var typeUsers = await _db.TypeUsers.Where(t => t.UserId == user.Id).ToArrayAsync();
         var types = new TypeModel[typeUsers.Length];
         int j = 0;
         foreach (var typeUser in typeUsers)
         {
            types[j++] = await _db.TypeModels.FirstOrDefaultAsync(t => t.Id == typeUser.TypeId);
         }
         userViewModel.TypeUsers = types;
         var links = await _db.Links.Where(l => l.UserId == user.Id).ToArrayAsync();
         userViewModel.Links = links;
         userViewModel.Resume = await _db.Resumes.FirstOrDefaultAsync(r => r.Id == user.ResumeId);
         userViewModel.Role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == user.RoleId);
         return userViewModel;
      }

      public async Task<UserViewModel> GetUserViewModelByEmail(string email)
      {
         var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
         var userViewModel = _mapper.Map<User, UserViewModel>(user);
         var datebirth = DateOnly.Parse(user.Birthday);
         userViewModel.Birthday = $"{datebirth.Day}.{datebirth.Month}.{datebirth.Year}";
         var typeUsers = await _db.TypeUsers.Where(t => t.UserId == user.Id).ToArrayAsync();
         var types = new TypeModel[typeUsers.Length];
         int j = 0;
         foreach (var typeUser in typeUsers)
         {
            types[j++] = await _db.TypeModels.FirstOrDefaultAsync(t => t.Id == typeUser.TypeId);
         }
         userViewModel.TypeUsers = types;
         var links = await _db.Links.Where(l => l.UserId == user.Id).ToArrayAsync();
         userViewModel.Links = links;
         userViewModel.Resume = await _db.Resumes.FirstOrDefaultAsync(r => r.Id == user.ResumeId);
         userViewModel.Role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == user.RoleId);
         return userViewModel;
      }

      public async Task Save(AddUserViewModel model)
      {
         var user = _mapper.Map<AddUserViewModel, User>(model);

         var typesId = model.TypeUsers.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(t => Guid.Parse(t)).ToArray();
         foreach (var typeId in typesId)
         {
            var typeUser = new TypeUser() { TypeId = typeId, UserId = user.Id };
            await _db.TypeUsers.AddAsync(typeUser);
         }

         var role = await _db.Roles.FirstOrDefaultAsync(r => r.Name == "Пользователь");
         user.RoleId = role.Id;

         var entry = _db.Entry(user);
         if (entry.State == EntityState.Detached)
            await _db.Users.AddAsync(user);
         await _db.SaveChangesAsync();
      }

      public async Task Update(User updateUser, User newUser)
      {
         updateUser.FirstName = newUser.FirstName;
         updateUser.LastName = newUser.LastName;
         updateUser.MiddleName = newUser.MiddleName;
         updateUser.Gender = newUser.Gender;
         updateUser.Birthday = newUser.Birthday;
         updateUser.Email = newUser.Email;

         if (!String.IsNullOrEmpty(newUser.Password))
            updateUser.Password = newUser.Password;

         var entry = _db.Entry(updateUser);
         if (entry.State == EntityState.Detached)
            _db.Users.Update(updateUser);

         await _db.SaveChangesAsync();
      }

      public async Task Delete(User user)
      {
         _db.Users.Remove(user);

         var typeUsers = await _db.TypeUsers.Where(t => t.UserId == user.Id).ToArrayAsync();
         foreach (var typeUser in typeUsers)
         {
            _db.TypeUsers.Remove(typeUser);
         }

         var links = await _db.Links.Where(l => l.UserId == user.Id).ToArrayAsync();
         foreach (var link in links)
         {
            _db.Links.Remove(link);
         }

         var resume = await _db.Resumes.FirstOrDefaultAsync(r => r.Id == user.ResumeId);
         if (resume != null)
            _db.Resumes.Remove(resume);

         await _db.SaveChangesAsync();
      }
   }
}
