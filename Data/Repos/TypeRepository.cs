using EduSciencePro.Models.User;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using TypeModel = EduSciencePro.Models.User.TypeModel;

namespace EduSciencePro.Data.Repos
{
   public class TypeRepository : ITypeRepository
   {
      private readonly ApplicationDbContext _db;

      public TypeRepository(ApplicationDbContext db)
      {
         _db = db;
      }

      public async Task<TypeModel[]> GetTypes()
      {
         return await _db.TypeModels.ToArrayAsync();
      }

      public async Task<TypeModel> GetTypeById(Guid id)
      {
         return await _db.TypeModels.FirstOrDefaultAsync(t => t.Id == id);
      }

      public async Task<TypeModel[]> GetTypesByUserId(Guid id)
      {
         var typeUsers = await _db.TypeUsers.Where(t => t.UserId == id).ToArrayAsync();
         var types = new TypeModel[typeUsers.Length];
         int i = 0;
         foreach (var typeUser in typeUsers)
         {
            types[i++] = await GetTypeById(typeUser.TypeId);
         }
         return types;
      }

      public async Task<TypeModel> GetTypeByName(string name)
      {
         return await _db.TypeModels.FirstOrDefaultAsync(t => t.Name == name);
      }

      public async Task Save(TypeModel type)
      {
         var entry = _db.Entry(type);
         if (entry.State == EntityState.Detached)
            await _db.TypeModels.AddAsync(type);
         await _db.SaveChangesAsync();
      }

      public async Task Update(TypeModel updateType, TypeModel newType)
      {
         updateType.Name = newType.Name;

         var entry = _db.Entry(updateType);
         if (entry.State == EntityState.Detached)
            _db.TypeModels.Update(updateType);

         await _db.SaveChangesAsync();
      }

      public async Task Delete(TypeModel type)
      {
         _db.TypeModels.Remove(type);
         var typeUsers = await _db.TypeUsers.Where(t => t.TypeId == type.Id).ToArrayAsync();
         foreach (var typeUser in typeUsers)
         {
            _db.TypeUsers.Remove(typeUser);
         }
         await _db.SaveChangesAsync();
      }
   }

   public interface ITypeRepository
    {
      Task<TypeModel[]> GetTypes();
      Task<TypeModel> GetTypeById(Guid id);
      Task<TypeModel> GetTypeByName(string name);
      Task<TypeModel[]> GetTypesByUserId(Guid id);
      Task Save(TypeModel type);
      Task Update(TypeModel updateType, TypeModel newType);
      Task Delete(TypeModel type);
    }
}
