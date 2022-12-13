using AutoMapper;
using EduSciencePro.Data.Repos;
using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
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

      public async Task Update(AddUserViewModel model, User editUser)
      {
         if (!String.IsNullOrEmpty(model.FirstName))
            editUser.FirstName = model.FirstName;
         if (!String.IsNullOrEmpty(model.LastName))
            editUser.LastName = model.LastName;
         if (!String.IsNullOrEmpty(model.MiddleName))
            editUser.MiddleName = model.MiddleName;
         if (!String.IsNullOrEmpty(model.Gender))
            editUser.Gender = model.Gender;
         if (!String.IsNullOrEmpty(model.Birthday))
            editUser.Birthday = model.Birthday;
         if (!String.IsNullOrEmpty(model.TypeUsers))
         {
            var typesUser = await _db.TypeUsers.Where(t => t.UserId == editUser.Id).ToArrayAsync();
            foreach (var typeUser in typesUser)
            {
               _db.TypeUsers.Remove(typeUser);
            }

            var typesArray = model.TypeUsers.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray();
            foreach (var type in typesArray)
            {
               var typeUser = new TypeUser() { TypeId = Guid.Parse(type), UserId = editUser.Id };
               await _db.TypeUsers.AddAsync(typeUser);
            }

         }
         //if (!String.IsNullOrEmpty(model.Links))
         //if (img != null)
         //{
         //   var i = img;
         //   editUser.Image = GetByteArrayFromImage(i);
         //}

         if (model.Img != null)
         {
            byte[] imageData = null;
            using (var fs1 = model.Img.OpenReadStream())
            using (var ms1 = new MemoryStream())
            {
               fs1.CopyTo(ms1);
               imageData = ms1.ToArray();
            }
            editUser.Image = imageData;
         }

         var entry = _db.Entry(editUser);
         if (entry.State == EntityState.Detached)
            _db.Users.Update(editUser);

         await _db.SaveChangesAsync();
      }

      private byte[] GetByteArrayFromImage(IFormFile file)
      {
         using (var target = new MemoryStream())
         {
            file.CopyTo(target);
            return target.ToArray();
         }
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

      public async Task DeleteImage(Guid userId)
      {
         var user = await GetUserById(userId);
         user.Image = null;

         var entry = _db.Entry(user);
         if (entry.State == EntityState.Detached)
            _db.Users.Update(user);

         await _db.SaveChangesAsync();
      }
   }
}
