using AutoMapper;
using EduSciencePro.Models.User;
using Microsoft.EntityFrameworkCore;

namespace EduSciencePro.Data.Repos
{
   public class EducationRepository : IEducationRepository
   {
      private readonly ApplicationDbContext _db;
      private readonly IMapper _mapper;

      public EducationRepository(ApplicationDbContext db, IMapper mapper)
      {
         _db = db;
         _mapper = mapper;
      }
      public async Task<Education[]> GetEducations() => await _db.Educations.ToArrayAsync();

      public async Task<Education> GetEducationByName(string name) => await _db.Educations.FirstOrDefaultAsync(e => e.Name == name);

      public async Task<Education[]> GetEducationsSearch(string search) => await _db.Educations.Where(e => e.Name.ToLower().Contains(search.ToLower())).ToArrayAsync();

      public async Task Save(Education education)
      {
         var entry = _db.Entry(education);
         if (entry.State == EntityState.Detached)
            await _db.Educations.AddAsync(education);

         await _db.SaveChangesAsync();
      }
   }

   public interface IEducationRepository
   {
      Task<Education[]> GetEducations();
      //Task<Education> GetEducationById(int id);
      Task<Education> GetEducationByName(string name);
      Task<Education[]> GetEducationsSearch(string search);
      Task Save(Education education);
      //Task Update(Education education, string newName);
      //Task Delete(int id);
   }
}
