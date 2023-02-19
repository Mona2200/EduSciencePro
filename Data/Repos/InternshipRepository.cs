using AutoMapper;
using EduSciencePro.Models.User;
using EduSciencePro.Models;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;
using Microsoft.EntityFrameworkCore;

namespace EduSciencePro.Data.Repos
{
    public class InternshipRepository : IInternshipRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IOrganizationRepository _organizations;

        public InternshipRepository(ApplicationDbContext db, IMapper mapper, IOrganizationRepository organizations)
        {
            _db = db;
            _mapper = mapper;
            _organizations = organizations;
        }

        public async Task<Internship[]> GetInternships() => await _db.Internships.OrderByDescending(p => p.StartDate).Where(p => p.EndDate > DateTime.Now).ToArrayAsync();
        public async Task<InternshipViewModel[]> GetInternshipViewModels(string[] tagNames = null, int take = 5, int skip = 0)
        {
            List<Internship> internships = new();
            if (tagNames != null && tagNames.Length != 0)
            {
                foreach (var tagName in tagNames)
                {
                    var tag = await _db.Skills.FirstOrDefaultAsync(t => t.Name == tagName);
                    if (tag != null)
                    {
                        var tagPosts = await _db.SkillInternships.Where(t => t.SkillId == tag.Id).ToListAsync();
                        foreach (var tagPost in tagPosts)
                        {
                            var tagNew = await _db.Internships.FirstOrDefaultAsync(p => p.Id == tagPost.IntershipId);
                            if (tagNew != null && internships.FirstOrDefault(n => n.Id == tagNew.Id) == null)
                                internships.Add(tagNew);
                        }
                    }
                }
            }
            else
                internships = await _db.Internships.ToListAsync();
            internships = internships.OrderByDescending(n => n.StartDate).Take(take).Skip(skip).ToList();

            List<InternshipViewModel> internshipViewModels = new List<InternshipViewModel>();
            foreach (var internship in internships)
            {
                InternshipViewModel internshipViewModel = _mapper.Map<Internship, InternshipViewModel>(internship);

                string StartDate = "";
                if (internship.StartDate.Day.ToString().Length == 1)
                    StartDate += "0" + internship.StartDate.Day.ToString() + ".";
                else
                    StartDate += internship.StartDate.Day.ToString() + ".";

                if (internship.StartDate.Month.ToString().Length == 1)
                    StartDate += "0" + internship.StartDate.Month.ToString() + ".";
                else
                    StartDate += internship.StartDate.Month.ToString() + ".";
                StartDate += internship.StartDate.Year.ToString();

                internshipViewModel.StartDate = StartDate;

                string Enddate = "";
                if (internship.EndDate.Day.ToString().Length == 1)
                    Enddate += "0" + internship.EndDate.Day.ToString() + ".";
                else
                    Enddate += internship.EndDate.Day.ToString() + ".";

                if (internship.EndDate.Month.ToString().Length == 1)
                    Enddate += "0" + internship.EndDate.Month.ToString() + ".";
                else
                    Enddate += internship.EndDate.Month.ToString() + ".";
                Enddate += internship.EndDate.Year.ToString();

                internshipViewModel.EndDate = Enddate;

                var skillInternships = await _db.SkillInternships.Where(t => t.IntershipId == internship.Id).ToArrayAsync();
                List<Skill> skills = new List<Skill>();
                foreach (var skillInternship in skillInternships)
                {
                    var skill = await _db.Skills.FirstOrDefaultAsync(t => t.Id == skillInternship.SkillId);
                    skills.Add(skill);
                }
                internshipViewModel.Skills = skills.Take(4).ToArray();
                internshipViewModel.Organization = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == internship.OrganizationId);

                internshipViewModels.Add(internshipViewModel);
            }
            return internshipViewModels.ToArray();
        }

        public async Task<Internship[]> GetInternshipsByOrganizationId(Guid organizationid) => await _db.Internships.OrderByDescending(p => p.StartDate).Where(t => t.OrganizationId == organizationid).ToArrayAsync();

        public async Task<InternshipViewModel[]> GetInternshipViewModelsByOrganizationId(Guid organizationid, int take = 5, int skip = 0)
        {
            List<Internship> internships = new();
            internships = await _db.Internships.OrderByDescending(p => p.StartDate).Where(p => p.OrganizationId == organizationid).ToListAsync();
            internships = internships.Take(take).Skip(skip).ToList();

            List<InternshipViewModel> internshipViewModels = new List<InternshipViewModel>();
            foreach (var internship in internships)
            {
                InternshipViewModel internshipViewModel = _mapper.Map<Internship, InternshipViewModel>(internship);

                string StartDate = "";
                if (internship.StartDate.Day.ToString().Length == 1)
                    StartDate += "0" + internship.StartDate.Day.ToString() + ".";
                else
                    StartDate += internship.StartDate.Day.ToString() + ".";

                if (internship.StartDate.Month.ToString().Length == 1)
                    StartDate += "0" + internship.StartDate.Month.ToString() + ".";
                else
                    StartDate += internship.StartDate.Month.ToString() + ".";
                StartDate += internship.StartDate.Year.ToString();

                internshipViewModel.StartDate = StartDate;

                string Enddate = "";
                if (internship.EndDate.Day.ToString().Length == 1)
                    Enddate += "0" + internship.EndDate.Day.ToString() + ".";
                else
                    Enddate += internship.EndDate.Day.ToString() + ".";

                if (internship.EndDate.Month.ToString().Length == 1)
                    Enddate += "0" + internship.EndDate.Month.ToString() + ".";
                else
                    Enddate += internship.EndDate.Month.ToString() + ".";
                Enddate += internship.EndDate.Year.ToString();

                internshipViewModel.EndDate = Enddate;

                var skillInternships = await _db.SkillInternships.Where(t => t.IntershipId == internship.Id).ToArrayAsync();
                List<Skill> skills = new List<Skill>();
                foreach (var skillInternship in skillInternships)
                {
                    var skill = await _db.Skills.FirstOrDefaultAsync(t => t.Id == skillInternship.SkillId);
                    skills.Add(skill);
                }
                internshipViewModel.Skills = skills.ToArray();
                internshipViewModel.Organization = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == internship.OrganizationId);

                internshipViewModels.Add(internshipViewModel);
            }
            return internshipViewModels.ToArray();
        }

        public async Task<Internship> GetInternshipById(Guid id) => await _db.Internships.FirstOrDefaultAsync(c => c.Id == id);

        public async Task<InternshipViewModel> GetInternshipViewModelById(Guid id)
        {
            var internship = await GetInternshipById(id);
            InternshipViewModel internshipViewModel = _mapper.Map<Internship, InternshipViewModel>(internship);

            string StartDate = "";
            if (internship.StartDate.Day.ToString().Length == 1)
                StartDate += "0" + internship.StartDate.Day.ToString() + ".";
            else
                StartDate += internship.StartDate.Day.ToString() + ".";

            if (internship.StartDate.Month.ToString().Length == 1)
                StartDate += "0" + internship.StartDate.Month.ToString() + ".";
            else
                StartDate += internship.StartDate.Month.ToString() + ".";
            StartDate += internship.StartDate.Year.ToString();

            internshipViewModel.StartDate = StartDate;

            string Enddate = "";
            if (internship.EndDate.Day.ToString().Length == 1)
                Enddate += "0" + internship.EndDate.Day.ToString() + ".";
            else
                Enddate += internship.EndDate.Day.ToString() + ".";

            if (internship.EndDate.Month.ToString().Length == 1)
                Enddate += "0" + internship.EndDate.Month.ToString() + ".";
            else
                Enddate += internship.EndDate.Month.ToString() + ".";
            Enddate += internship.EndDate.Year.ToString();

            internshipViewModel.EndDate = Enddate;

            var skillInternships = await _db.SkillInternships.Where(t => t.IntershipId == internship.Id).ToArrayAsync();
            List<Skill> skills = new List<Skill>();
            foreach (var skillInternship in skillInternships)
            {
                var skill = await _db.Skills.FirstOrDefaultAsync(t => t.Id == skillInternship.SkillId);
                skills.Add(skill);
            }
            internshipViewModel.Skills = skills.ToArray();
            internshipViewModel.Organization = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == internship.OrganizationId);

            return internshipViewModel;
        }

        public async Task Save(AddIntershipViewModel model)
        {
            Internship internship = _mapper.Map<AddIntershipViewModel, Internship>(model);
            var organization = await _organizations.GetOrganizationByName(model.OrganizationName);
            if (organization != null)
            {
                internship.OrganizationId = organization.Id;
                if (model.Skills != null)
                {
                    string[] skillNames = model.Skills.Split('/', StringSplitOptions.RemoveEmptyEntries);
                    List<Skill> skills = new List<Skill>();
                    foreach (var skillName in skillNames)
                    {
                        Skill? skill = await _db.Skills.FirstOrDefaultAsync(t => t.Name == skillName);
                        if (skill != null)
                        {
                            SkillInternship skillInternship = new SkillInternship() { IntershipId = internship.Id, SkillId = skill.Id };
                            var entry = _db.SkillInternships.Entry(skillInternship);
                            if (entry.State == EntityState.Detached)
                                await _db.SkillInternships.AddAsync(skillInternship);
                        }
                        else
                        {
                            Skill newSkill = new Skill() { Name = skillName };
                            var entry = _db.Skills.Entry(newSkill);
                            if (entry.State == EntityState.Detached)
                                await _db.Skills.AddAsync(newSkill);

                            SkillInternship skillInternship = new SkillInternship() { IntershipId = internship.Id, SkillId = newSkill.Id };
                            var newentry = _db.SkillInternships.Entry(skillInternship);
                            if (newentry.State == EntityState.Detached)
                                await _db.SkillInternships.AddAsync(skillInternship);
                        }
                    }
                }

                var internshipEntry = _db.Internships.Entry(internship);
                if (internshipEntry.State == EntityState.Detached)
                    await _db.Internships.AddAsync(internship);
                await _db.SaveChangesAsync();
            }
        }

        public async Task Delete(Guid id)
        {
            var internship = await GetInternshipById(id);
            if (internship != null)
            {
                var skillInternships = await _db.SkillInternships.Where(t => t.IntershipId == internship.Id).ToArrayAsync();
                foreach (var skillInternship in skillInternships)
                {
                    _db.SkillInternships.Remove(skillInternship);
                }
                _db.Internships.Remove(internship);
                await _db.SaveChangesAsync();
            }
        }
    }

    public interface IInternshipRepository
    {
        Task<Internship[]> GetInternships();
        Task<InternshipViewModel[]> GetInternshipViewModels(string[] tagNames = null, int take = 5, int skip = 0);
        Task<Internship[]> GetInternshipsByOrganizationId(Guid organizationid);
        Task<InternshipViewModel[]> GetInternshipViewModelsByOrganizationId(Guid organizationid, int take = 5, int skip = 0);
        Task<Internship> GetInternshipById(Guid id);
        Task<InternshipViewModel> GetInternshipViewModelById(Guid id);
        Task Save(AddIntershipViewModel model);
        Task Delete(Guid id);
    }
}
