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
      Task Save(User user, Organization organization);
   }
}
