using AutoMapper;
using EduSciencePro.Models;
using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;
using Microsoft.EntityFrameworkCore;

namespace EduSciencePro.Data.Repos
{
   public class CooperationRepository : ICooperationRepository
   {
      private readonly ApplicationDbContext _db;
      private readonly IMapper _mapper;
      private readonly IOrganizationRepository _organizations;

      public CooperationRepository(ApplicationDbContext db, IMapper mapper, IOrganizationRepository organizations)
      {
         _db = db;
         _mapper = mapper;
         _organizations = organizations;
      }

      public async Task<Cooperation[]> GetCooperations() => await _db.Cooperations.OrderByDescending(p => p.EndDate).Where(p => p.EndDate > DateTime.Now).ToArrayAsync();
      public async Task<CooperationViewModel[]> GetCooperationViewModels(string[] tagNames = null, int take = 5, int skip = 0)
      {
            List<Cooperation> cooperations = new();
            if (tagNames != null && tagNames.Length != 0)
            {
                foreach (var tagName in tagNames)
                {
                    var tag = await _db.Skills.FirstOrDefaultAsync(t => t.Name == tagName);
                    if (tag != null)
                    {
                        var tagPosts = await _db.SkillCooperations.Where(t => t.SkillId == tag.Id).ToListAsync();
                        foreach (var tagPost in tagPosts)
                        {
                            var tagNew = await _db.Cooperations.FirstOrDefaultAsync(p => p.Id == tagPost.CooperationId);
                            if (tagNew != null && cooperations.FirstOrDefault(n => n.Id == tagNew.Id) == null)
                                cooperations.Add(tagNew);
                        }
                    }
                }
            }
            else
                cooperations = await _db.Cooperations.ToListAsync();
            cooperations = cooperations.OrderByDescending(n => n.EndDate).Where(p => p.EndDate > DateTime.Now).Take(take).Skip(skip).ToList();

         List<CooperationViewModel> cooperationViewModels = new List<CooperationViewModel>();
         foreach (var cooperation in cooperations)
         {
            CooperationViewModel cooperationViewModel = _mapper.Map<Cooperation, CooperationViewModel>(cooperation);

            string date = "";

            if (cooperation.EndDate.Day.ToString().Length == 1)
               date += "0" + cooperation.EndDate.Day.ToString() + ".";
            else
               date += cooperation.EndDate.Day.ToString() + ".";

            if (cooperation.EndDate.Month.ToString().Length == 1)
               date += "0" + cooperation.EndDate.Month.ToString() + ".";
            else
               date += cooperation.EndDate.Month.ToString() + ".";
            date += cooperation.EndDate.Year.ToString();

            cooperationViewModel.EndDate = date;

            var skillCooperations = await _db.SkillCooperations.Where(t => t.CooperationId == cooperation.Id).ToArrayAsync();
            List<Skill> skills = new List<Skill>();
            foreach (var skillCooperation in skillCooperations)
            {
               var skill = await _db.Skills.FirstOrDefaultAsync(t => t.Id == skillCooperation.SkillId);
               skills.Add(skill);
            }
            cooperationViewModel.Skills = skills.Take(4).ToArray();
            cooperationViewModel.Organization = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == cooperation.OrganizationId);

            cooperationViewModels.Add(cooperationViewModel);
         }
         return cooperationViewModels.ToArray();
      }

      public async Task<Cooperation[]> GetCooperationsByOrganizationId(Guid organizationid) => await _db.Cooperations.OrderByDescending(p => p.EndDate).Where(t => t.OrganizationId == organizationid).ToArrayAsync();

      public async Task<CooperationViewModel[]> GetCooperationViewModelsByOrganizationId(Guid organizationid, int take = 5, int skip = 0)
      {
            List<Cooperation> cooperations = new();
            cooperations = await _db.Cooperations.OrderByDescending(p => p.EndDate).Where(p => p.OrganizationId == organizationid).ToListAsync();
            cooperations = cooperations.Take(take).Skip(skip).ToList();

         List<CooperationViewModel> cooperationViewModels = new List<CooperationViewModel>();
         foreach (var cooperation in cooperations)
         {
            CooperationViewModel cooperationViewModel = _mapper.Map<Cooperation, CooperationViewModel>(cooperation);

            string date = "";

            if (cooperation.EndDate.Day.ToString().Length == 1)
               date += "0" + cooperation.EndDate.Day.ToString() + ".";
            else
               date += cooperation.EndDate.Day.ToString() + ".";

            if (cooperation.EndDate.Month.ToString().Length == 1)
               date += "0" + cooperation.EndDate.Month.ToString() + ".";
            else
               date += cooperation.EndDate.Month.ToString() + ".";
            date += cooperation.EndDate.Year.ToString();

            cooperationViewModel.EndDate = date;

            var skillCooperations = await _db.SkillCooperations.Where(t => t.CooperationId == cooperation.Id).ToArrayAsync();
            List<Skill> skills = new List<Skill>();
            foreach (var skillCooperation in skillCooperations)
            {
               var skill = await _db.Skills.FirstOrDefaultAsync(t => t.Id == skillCooperation.SkillId);
               skills.Add(skill);
            }
            cooperationViewModel.Skills = skills.ToArray();
            cooperationViewModel.Organization = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == cooperation.OrganizationId);

            cooperationViewModels.Add(cooperationViewModel);
         }
         return cooperationViewModels.ToArray();
      }

      public async Task<Cooperation> GetCooperationById(Guid id) => await _db.Cooperations.FirstOrDefaultAsync(c => c.Id == id);

      public async Task<CooperationViewModel> GetCooperationViewModelById(Guid id)
      {
         var cooperation = await GetCooperationById(id);
         CooperationViewModel cooperationViewModel = _mapper.Map<Cooperation, CooperationViewModel>(cooperation);

         string date = "";

         if (cooperation.EndDate.Day.ToString().Length == 1)
            date += "0" + cooperation.EndDate.Day.ToString() + ".";
         else
            date += cooperation.EndDate.Day.ToString() + ".";

         if (cooperation.EndDate.Month.ToString().Length == 1)
            date += "0" + cooperation.EndDate.Month.ToString() + ".";
         else
            date += cooperation.EndDate.Month.ToString() + ".";
         date += cooperation.EndDate.Year.ToString();

         cooperationViewModel.EndDate = date;

         var skillCooperations = await _db.SkillCooperations.Where(t => t.CooperationId == cooperation.Id).ToArrayAsync();
         List<Skill> skills = new List<Skill>();
         foreach (var skillCooperation in skillCooperations)
         {
            var skill = await _db.Skills.FirstOrDefaultAsync(t => t.Id == skillCooperation.SkillId);
            skills.Add(skill);
         }
         cooperationViewModel.Skills = skills.ToArray();
         cooperationViewModel.Organization = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == cooperation.OrganizationId);

         return cooperationViewModel;
      }

      public async Task Save(AddCooperationViewModel model)
      {
         Cooperation cooperation = _mapper.Map<AddCooperationViewModel, Cooperation>(model);
         var organization = await _organizations.GetOrganizationByName(model.OrganizationName);
         if (organization != null)
         {
            cooperation.OrganizationId = organization.Id;
            if (model.Skills != null)
            {
               string[] skillNames = model.Skills.Split('/', StringSplitOptions.RemoveEmptyEntries);
               List<Skill> skills = new List<Skill>();
               foreach (var skillName in skillNames)
               {
                  Skill? skill = await _db.Skills.FirstOrDefaultAsync(t => t.Name == skillName);
                  if (skill != null)
                  {
                     SkillCooperation skillCooperation = new SkillCooperation() { CooperationId = cooperation.Id, SkillId = skill.Id };
                     var entry = _db.SkillCooperations.Entry(skillCooperation);
                     if (entry.State == EntityState.Detached)
                        await _db.SkillCooperations.AddAsync(skillCooperation);
                  }
                  else
                  {
                     Skill newSkill = new Skill() { Name = skillName };
                     var entry = _db.Skills.Entry(newSkill);
                     if (entry.State == EntityState.Detached)
                        await _db.Skills.AddAsync(newSkill);

                     SkillCooperation skillCooperation = new SkillCooperation() { CooperationId = cooperation.Id, SkillId = newSkill.Id };
                     var newentry = _db.SkillCooperations.Entry(skillCooperation);
                     if (newentry.State == EntityState.Detached)
                        await _db.SkillCooperations.AddAsync(skillCooperation);
                  }
               }
            }
            
            var cooperationEntry = _db.Cooperations.Entry(cooperation);
            if (cooperationEntry.State == EntityState.Detached)
               await _db.Cooperations.AddAsync(cooperation);
            await _db.SaveChangesAsync();
         }
      }

      public async Task Delete(Guid id)
      {
         var cooperation = await GetCooperationById(id);
         if (cooperation != null)
         {
            var skillCooperations = await _db.SkillCooperations.Where(t => t.CooperationId == cooperation.Id).ToArrayAsync();
            foreach (var skillCooperation in skillCooperations)
            {
               _db.SkillCooperations.Remove(skillCooperation);
            }
            _db.Cooperations.Remove(cooperation);
            await _db.SaveChangesAsync();
         }
      }
   }

   public interface ICooperationRepository
   {
      Task<Cooperation[]> GetCooperations();
      Task<CooperationViewModel[]> GetCooperationViewModels(string[] tagNames = null, int take = 5, int skip = 0);
      Task<Cooperation[]> GetCooperationsByOrganizationId(Guid organizationid);
      Task<CooperationViewModel[]> GetCooperationViewModelsByOrganizationId(Guid organizationid, int take = 5, int skip = 0);
      Task<Cooperation> GetCooperationById(Guid id);
      Task<CooperationViewModel> GetCooperationViewModelById(Guid id);
      Task Save(AddCooperationViewModel model);
      Task Delete(Guid id);
   }
}
