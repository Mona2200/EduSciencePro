using AutoMapper;
using EduSciencePro.Models;
using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;
using Microsoft.EntityFrameworkCore;
using ServiceStack;

namespace EduSciencePro.Data.Repos
{
   public class ProjectRepository : IProjectRepository
   {
      private readonly ApplicationDbContext _db;
      private readonly IMapper _mapper;

      private readonly IOrganizationRepository _organizations;

      public ProjectRepository(ApplicationDbContext db, IMapper mapper, IOrganizationRepository organizations)
      {
         _db = db;
         _mapper = mapper;
         _organizations = organizations;
      }

      public async Task<Project[]> GetProjects() => await _db.Projects.OrderByDescending(p => p.StartDate).Where(p => p.EndDate > DateTime.Now).ToArrayAsync();

      public async Task<Project[]> GetProjectsByOrganizationId(Guid organizationId) => await _db.Projects.OrderByDescending(p => p.StartDate).Where(p => p.OrganizationId == organizationId).ToArrayAsync();

      public async Task<ProjectViewModel[]> GetProjectViewModels(string[] tagNames = null, int take = 5, int skip = 0)
      {
            List<Project> projectList = new();
            if (tagNames != null && tagNames.Length != 0)
            {
                foreach (var tagName in tagNames)
                {
                    var tag = await _db.Skills.FirstOrDefaultAsync(t => t.Name == tagName);
                    if (tag != null)
                    {
                        var tagPosts = await _db.ProjectSkills.Where(t => t.SkillId == tag.Id).ToListAsync();
                        foreach (var tagPost in tagPosts)
                        {
                            var tagNew = await _db.Projects.FirstOrDefaultAsync(p => p.Id == tagPost.ProjectId);
                            if (tagNew != null && projectList.FirstOrDefault(n => n.Id == tagNew.Id) == null)
                                projectList.Add(tagNew);
                        }
                    }
                }
            }
            else
                projectList = await _db.Projects.ToListAsync();
            projectList = projectList.OrderByDescending(n => n.StartDate).Take(take).Skip(skip).ToList();

         var projectViewModels = new List<ProjectViewModel>();

         foreach (var project in projectList)
         {
            var projectViewModel = _mapper.Map<Project, ProjectViewModel>(project);

            string startDate = "";

            if (project.StartDate.Day.ToString().Length == 1)
               startDate += "0" + project.StartDate.Day.ToString() + ".";
            else
               startDate += project.StartDate.Day.ToString() + ".";

            if (project.StartDate.Month.ToString().Length == 1)
               startDate += "0" + project.StartDate.Month.ToString() + ".";
            else
               startDate += project.StartDate.Month.ToString() + ".";
            startDate += project.StartDate.Year.ToString();

            projectViewModel.StartDate = startDate;

            string endDate = "";

            if (project.EndDate.Day.ToString().Length == 1)
               endDate += "0" + project.EndDate.Day.ToString() + ".";
            else
               endDate += project.EndDate.Day.ToString() + ".";

            if (project.EndDate.Month.ToString().Length == 1)
               endDate += "0" + project.EndDate.Month.ToString() + ".";
            else
               endDate += project.EndDate.Month.ToString() + ".";
            endDate += project.EndDate.Year.ToString();

            projectViewModel.EndDate = endDate;

            projectViewModel.Organization = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == project.OrganizationId);

            var projectSkills = await _db.ProjectSkills.Where(p => p.ProjectId == project.Id).ToArrayAsync();

            var skills = new List<Skill>();
            foreach (var projectSkill in projectSkills)
               skills.Add(await _db.Skills.FirstOrDefaultAsync(s => s.Id == projectSkill.SkillId));

            projectViewModel.Skills = skills.Take(4).ToArray();

            projectViewModels.Add(projectViewModel);
         }
         return projectViewModels.ToArray();
      }

      public async Task<ProjectViewModel[]> GetProjectViewModelsByOrganizationId(Guid organizationId, int take = 5, int skip = 0)
      {
            List<Project> projects = new();
            projects = await _db.Projects.OrderByDescending(p => p.StartDate).Where(p => p.OrganizationId == organizationId).ToListAsync();
            projects = projects.Take(take).Skip(skip).ToList();

         var projectViewModels = new List<ProjectViewModel>();

         foreach (var project in projects)
         {
            var projectViewModel = _mapper.Map<Project, ProjectViewModel>(project);

            string startDate = "";

            if (project.StartDate.Day.ToString().Length == 1)
               startDate += "0" + project.StartDate.Day.ToString() + ".";
            else
               startDate += project.StartDate.Day.ToString() + ".";

            if (project.StartDate.Month.ToString().Length == 1)
               startDate += "0" + project.StartDate.Month.ToString() + ".";
            else
               startDate += project.StartDate.Month.ToString() + ".";
            startDate += project.StartDate.Year.ToString();

            projectViewModel.StartDate = startDate;

            string endDate = "";

            if (project.EndDate.Day.ToString().Length == 1)
               endDate += "0" + project.EndDate.Day.ToString() + ".";
            else
               endDate += project.EndDate.Day.ToString() + ".";

            if (project.EndDate.Month.ToString().Length == 1)
               endDate += "0" + project.EndDate.Month.ToString() + ".";
            else
               endDate += project.EndDate.Month.ToString() + ".";
            endDate += project.EndDate.Year.ToString();

            projectViewModel.EndDate = endDate;

            projectViewModel.Organization = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == project.OrganizationId);

            var projectSkills = await _db.ProjectSkills.Where(p => p.ProjectId == project.Id).ToArrayAsync();

            var skills = new List<Skill>();
            foreach (var projectSkill in projectSkills)
               skills.Add(await _db.Skills.FirstOrDefaultAsync(s => s.Id == projectSkill.SkillId));

            projectViewModel.Skills = skills.ToArray();

            projectViewModels.Add(projectViewModel);
         }
         return projectViewModels.ToArray();
      }

      public async Task<Project> GetProjectById(Guid id) => await _db.Projects.FirstOrDefaultAsync(p => p.Id == id);

      public async Task<ProjectViewModel> GetProjectViewModelById(Guid id)
      {
         var project = await GetProjectById(id);

            var projectViewModel = _mapper.Map<Project, ProjectViewModel>(project);

            string startDate = "";

            if (project.StartDate.Day.ToString().Length == 1)
               startDate += "0" + project.StartDate.Day.ToString() + ".";
            else
               startDate += project.StartDate.Day.ToString() + ".";

            if (project.StartDate.Month.ToString().Length == 1)
               startDate += "0" + project.StartDate.Month.ToString() + ".";
            else
               startDate += project.StartDate.Month.ToString() + ".";
            startDate += project.StartDate.Year.ToString();

            projectViewModel.StartDate = startDate;

            string endDate = "";

            if (project.EndDate.Day.ToString().Length == 1)
               endDate += "0" + project.EndDate.Day.ToString() + ".";
            else
               endDate += project.EndDate.Day.ToString() + ".";

            if (project.EndDate.Month.ToString().Length == 1)
               endDate += "0" + project.EndDate.Month.ToString() + ".";
            else
               endDate += project.EndDate.Month.ToString() + ".";
            endDate += project.EndDate.Year.ToString();

            projectViewModel.EndDate = endDate;

            projectViewModel.Organization = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == project.OrganizationId);

            var projectSkills = await _db.ProjectSkills.Where(p => p.ProjectId == project.Id).ToArrayAsync();

            var skills = new List<Skill>();
            foreach (var projectSkill in projectSkills)
               skills.Add(await _db.Skills.FirstOrDefaultAsync(s => s.Id == projectSkill.SkillId));

            projectViewModel.Skills = skills.ToArray();


         return projectViewModel;
      }

      public async Task Save(AddProjectViewModel model)
      {
         Project project = _mapper.Map<AddProjectViewModel, Project>(model);
         var organization = await _organizations.GetOrganizationByName(model.OrganizationName);
         if (organization != null)
         {
            project.OrganizationId = organization.Id;
            if (model.Skills != null)
            {
               string[] skillNames = model.Skills.Split('/', StringSplitOptions.RemoveEmptyEntries);
               List<Skill> skills = new List<Skill>();
               foreach (var skillName in skillNames)
               {
                  Skill? skill = await _db.Skills.FirstOrDefaultAsync(s => s.Name == skillName);
                  if (skill != null)
                  {
                     ProjectSkill projectSkill = new ProjectSkill() { ProjectId = project.Id, SkillId = skill.Id };
                     var entry = _db.ProjectSkills.Entry(projectSkill);
                     if (entry.State == EntityState.Detached)
                        await _db.ProjectSkills.AddAsync(projectSkill);
                  }
                  else
                  {
                     Skill newSkill = new Skill() { Name = skillName };
                     var entry = _db.Skills.Entry(newSkill);
                     if (entry.State == EntityState.Detached)
                        await _db.Skills.AddAsync(newSkill);

                     ProjectSkill projectSkill = new ProjectSkill() { ProjectId = project.Id, SkillId = newSkill.Id };
                     var newEntry = _db.ProjectSkills.Entry(projectSkill);
                     if (newEntry.State == EntityState.Detached)
                        await _db.ProjectSkills.AddAsync(projectSkill);
                  }
               }
            }       
            var projectEntry = _db.Projects.Entry(project);
            if (projectEntry.State == EntityState.Detached)
               await _db.Projects.AddAsync(project);
            await _db.SaveChangesAsync();
         }
      }

      public async Task Delete(Guid id)
      {
         var project = await GetProjectById(id);
         if (project != null)
         {
            var skillsProject = await _db.ProjectSkills.Where(p => p.ProjectId == project.Id).ToArrayAsync();
            foreach (var skillProject in skillsProject)
            {
            _db.ProjectSkills.Remove(skillProject);
            }
            _db.Projects.Remove(project);
            await _db.SaveChangesAsync();
         }
      }
   }

   public interface IProjectRepository
   {
      Task<Project[]> GetProjects();
      Task<Project> GetProjectById(Guid id);
      Task<ProjectViewModel> GetProjectViewModelById(Guid id);
      Task<Project[]> GetProjectsByOrganizationId(Guid organizationId);
      Task<ProjectViewModel[]> GetProjectViewModels(string[] tagNames = null, int take = 10, int skip = 0);
      Task<ProjectViewModel[]> GetProjectViewModelsByOrganizationId(Guid organizationId, int take = 10, int skip = 0);
      Task Save(AddProjectViewModel model);
      Task Delete(Guid id);
   }
}
