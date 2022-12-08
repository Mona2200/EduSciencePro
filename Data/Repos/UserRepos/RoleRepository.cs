using AutoMapper;
using EduSciencePro.Models.User;
using Microsoft.EntityFrameworkCore;

namespace EduSciencePro.Data.Repos.UserRepos
{
   public class RoleRepository : IRoleRepository
   {
      private readonly ApplicationDbContext _db;

      public RoleRepository(ApplicationDbContext db)
      {
         _db = db;
      }

      public async Task<Role[]> GetRoles()
      {
         return await _db.Roles.ToArrayAsync();
      }

      public async Task<Role> GetRoleById(Guid id)
      {
         return await _db.Roles.FirstOrDefaultAsync(r => r.Id == id);
      }

      public async Task<Role> GetRoleByName(string name)
      {
         return await _db.Roles.FirstOrDefaultAsync(r => r.Name == name);
      }

      public async Task Save(Role role)
      {
         var entry = _db.Entry(role);
         if (entry.State == EntityState.Detached)
            await _db.Roles.AddAsync(role);
         await _db.SaveChangesAsync();
      }

      public async Task Delete(Role role)
      {
         _db.Roles.Remove(role);
         await _db.SaveChangesAsync();
      }
   }
}
