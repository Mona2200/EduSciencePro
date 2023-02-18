using AutoMapper;
using EduSciencePro.Data.Repos;
using EduSciencePro.Models;
using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduSciencePro.Controllers
{
    public class ProjectController : Controller
    {
        private readonly IProjectRepository _projects;
        private readonly IUserRepository _users;
        private readonly IOrganizationRepository _organizations;

        public ProjectController(IProjectRepository projects, IUserRepository users, IOrganizationRepository organizations)
        {
            _projects = projects;
            _users = users;
            _organizations = organizations;
        }

        [HttpGet]
        [Route("Projects")]
        public async Task<IActionResult> Projects(string? tagNamesString)
        {
            ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
            var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
            var user = await _users.GetUserByEmail(claimEmail);

            List<string> tags = new();
            string[] tagNames = null;
            if (tagNamesString != null)
            {
                tagNamesString = tagNamesString.Replace('_', '/');
                tagNames = tagNamesString.Split('/', StringSplitOptions.RemoveEmptyEntries);

                foreach (var tagName in tagNames)
                {
                    tags.Add(tagName);
                }
            }

            var projectViewModels = await _projects.GetProjectViewModels(tagNames, 5, 0);

            var organization = await _organizations.GetOrganizationByUserId(user.Id);

            if (organization != null)
            {
                return View(new ProjectsAndIsOrgViewModel() { IsOrg = true, Projects = projectViewModels.Where(p => p.Organization.Id != organization.Id).ToArray(), Tags = tags });
            }
            else
                return View(new ProjectsAndIsOrgViewModel() { IsOrg = false, Projects = projectViewModels, Tags = tags });

        }

        [HttpGet]
        [Route("ProjectsTag/{tags}")]
        public async Task<IActionResult> ProjectsTag([FromRoute] string? tags)
        {
            return RedirectToAction("Projects", "Project", new { tagNamesString = tags });
        }

        [HttpPost]
        [Route("ProjectsMore/{take}/{skip}/{tags?}")]
        public async Task<ProjectViewModel[]> ProjectsMore([FromRoute] int take, [FromRoute] int skip, [FromRoute] string? tags = null)
        {
            string[] tagNames = null;
            if (tags != null)
            {
                tags = tags.Replace('_', '/');
                tagNames = tags.Split('/', StringSplitOptions.RemoveEmptyEntries);
            }

            var news = await _projects.GetProjectViewModels(tagNames, take, skip);
            return news;
        }

        [HttpGet]
        [Route("YourProjects")]
        public async Task<IActionResult> YourProjects()
        {
            ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
            var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
            var user = await _users.GetUserByEmail(claimEmail);

            var organization = await _organizations.GetOrganizationByUserId(user.Id);
            if (organization == null)
                return View(null);

            var projectViewModels = await _projects.GetProjectViewModelsByOrganizationId(organization.Id, 5, 0);
            return View(projectViewModels);
        }

        [HttpPost]
        [Route("YourProjectsMore/{take}/{skip}")]
        public async Task<ProjectViewModel[]> YourProjectsMore([FromRoute] int take, [FromRoute] int skip)
        {
            ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
            var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
            var user = await _users.GetUserByEmail(claimEmail);

            var organization = await _organizations.GetOrganizationByUserId(user.Id);
            if (organization == null)
            {
                var projects = new ProjectViewModel[0];
                return projects;
            }
              
            var projectViewModels = await _projects.GetProjectViewModelsByOrganizationId(organization.Id, take, skip);
            return projectViewModels;
        }

        [HttpGet]
        [Route("AddProject")]
        public async Task<IActionResult> AddProject()
        {
            ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
            var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
            var user = await _users.GetUserByEmail(claimEmail);

            var organization = await _organizations.GetOrganizationByUserId(user.Id);
            var minStartDate = DateTime.Now;
            var minEndDate = new DateTime(minStartDate.Year, minStartDate.Month, minStartDate.Day + 7);

            var addProjectViewModel = new AddProjectViewModel()
            {
                OrganizationName = organization.Name,
                minStartDate = FromDateToString(minStartDate),
                minEndDate = FromDateToString(minEndDate)
            };

            return View(addProjectViewModel);
        }

        [HttpPost]
        [Route("AddProject")]
        public async Task<IActionResult> AddProject(AddProjectViewModel model)
        {
            ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
            var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
            var user = await _users.GetUserByEmail(claimEmail);

            if (!ValidProject(model))
            {
                var organization = await _organizations.GetOrganizationByUserId(user.Id);
                model.OrganizationName = organization.Name;

                var minStartDate = DateTime.Now;
                var minEndDate = new DateTime(minStartDate.Year, minStartDate.Month, minStartDate.Day + 7);
                model.minStartDate = FromDateToString(minStartDate);
                model.minEndDate = FromDateToString(minEndDate);
                return View(model);
            }
            try
            {
                await _projects.Save(model);
            }
            catch (Exception ex)
            {

            };
            return RedirectToAction("Projects");
        }

        [HttpGet]
        [Route("LookingProject")]
        public async Task<IActionResult> LookingProject(Guid projectId)
        {
            ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
            var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
            var user = await _users.GetUserByEmail(claimEmail);

            var organization = await _organizations.GetOrganizationByUserId(user.Id);

            var projectViewModel = await _projects.GetProjectViewModelById(projectId);

            var lookingProject = new LookingProjectViewModel() { Project = projectViewModel };
            lookingProject.YourProject = projectViewModel.Organization.Id == organization?.Id;

            return View(lookingProject);
        }

        [HttpGet]
        [Route("DeleteProject")]
        public async Task<IActionResult> DeleteProject(Guid projectId)
        {
            await _projects.Delete(projectId);
            return RedirectToAction("YourProjects");
        }

        private bool ValidProject(AddProjectViewModel model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    if (ModelState[key].Errors.Count > 0)
                        ModelState.AddModelError($"{key}", $"{ModelState[key].Errors[0].ErrorMessage}");
                }
                return false;
            }
            return true;
        }

        private string FromDateToString(DateTime date)
        {
            var day = date.Day.ToString();
            if (day.Length == 1)
                day = "0" + day;
            var month = date.Month.ToString();
            if (month.Length == 1)
                month = "0" + month;
            return $"{date.Year}-{month}-{day}";
        }
    }
}
