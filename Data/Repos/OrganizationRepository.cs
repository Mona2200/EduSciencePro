using AutoMapper;
using EduSciencePro.Models.User;
using Microsoft.EntityFrameworkCore;

namespace EduSciencePro.Data.Repos
{
   public class OrganizationRepository : IOrganizationRepository
   {
      private readonly ApplicationDbContext _db;
      private readonly IMapper _mapper;

      public OrganizationRepository(ApplicationDbContext db, IMapper mapper)
      {
         _db = db;
         _mapper = mapper;
      }

      public async Task<Organization[]> GetOrganizationsSearch(string search) => await _db.Organizations.Where(o => o.Name.ToLower().Contains(search.ToLower())).ToArrayAsync();

      public async Task<Organization?> GetOrganizationByUserId(Guid userId)
      {
         var userOrganization = await _db.UserOrganizations.FirstOrDefaultAsync(o => o.IdUser == userId);
         if (userOrganization == null)
            return null;
         else
            return await _db.Organizations.FirstOrDefaultAsync(o => o.Id == userOrganization.IdOrganization);
      }

      public async Task<Organization?> GetOrganizationByName(string name)
      {
         return await _db.Organizations.FirstOrDefaultAsync(o => o.Name == name);
      }

      public async Task Save(User user, Organization organization)
      {
         var entry = _db.Entry(organization);
         if (entry.State == EntityState.Detached)
            await _db.Organizations.AddAsync(organization);

         var userOrganization = new UserOrganization() { IdOrganization = organization.Id, IdUser = user.Id };
         await _db.UserOrganizations.AddAsync(userOrganization);

         await _db.SaveChangesAsync();
      }
   }

   public interface IOrganizationRepository
   {
      Task<Organization[]> GetOrganizationsSearch(string search);
      Task<Organization?> GetOrganizationByUserId(Guid userId);
      Task<Organization?> GetOrganizationByName(string name);
      Task Save(User user, Organization organization);
   }
}
