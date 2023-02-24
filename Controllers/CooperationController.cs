using EduSciencePro.Data.Repos;
using EduSciencePro.Models;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduSciencePro.Controllers
{
    public class CooperationController : Controller
    {
        private readonly ICooperationRepository _cooperations;
        private readonly IUserRepository _users;
        private readonly IOrganizationRepository _organizations;
        private readonly INotificationRepository _notifications;
        public CooperationController(ICooperationRepository cooperations, IUserRepository users, IOrganizationRepository organizations, INotificationRepository notifications)
        {
            _cooperations = cooperations;
            _users = users;
            _organizations = organizations;
            _notifications = notifications;
        }

        [HttpGet]
        [Route("Cooperations")]
        public async Task<IActionResult> Cooperations(string? tagNamesString)
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

            var cooperations = await _cooperations.GetCooperationViewModels(tagNames, 5, 0);

            var organization = await _organizations.GetOrganizationByUserId(user.Id);

            if (organization != null)
            {
                return View(new CooperationsAndTagsViewModel() { IsOrg = true, Cooperations = cooperations.Where(c => c.Organization.Id != organization.Id).ToArray(), Tags = tags });
            }
            else
                return View(new CooperationsAndTagsViewModel() { IsOrg = false, Cooperations = cooperations, Tags = tags });
        }

        [HttpGet]
        [Route("CooperationsTag/{tags}")]
        public async Task<IActionResult> CooperationsTag([FromRoute] string? tags)
        {
            return RedirectToAction("Cooperations", "Cooperation", new { tagNamesString = tags });
        }

        [HttpPost]
        [Route("CooperationsMore/{take}/{skip}/{tags?}")]
        public async Task<CooperationViewModel[]> CooperationsMore([FromRoute] int take, [FromRoute] int skip, [FromRoute] string? tags = null)
        {
            string[] tagNames = null;
            if (tags != null)
            {
                tags = tags.Replace('_', '/');
                tagNames = tags.Split('/', StringSplitOptions.RemoveEmptyEntries);
            }

            var news = await _cooperations.GetCooperationViewModels(tagNames, take, skip);
            return news;
        }

        [HttpGet]
        [Route("YourCooperations")]
        public async Task<IActionResult> YourCooperations()
        {
            ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
            var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
            var user = await _users.GetUserByEmail(claimEmail);

            var organization = await _organizations.GetOrganizationByUserId(user.Id);
            if (organization == null)
                return View(null);

            var cooperationViewModels = await _cooperations.GetCooperationViewModelsByOrganizationId(organization.Id, 5, 0);
            return View(cooperationViewModels);
        }

        [HttpPost]
        [Route("YourCooperationsMore/{take}/{skip}")]
        public async Task<CooperationViewModel[]> YourCooperationsMore([FromRoute] int take, [FromRoute] int skip)
        {
            ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
            var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
            var user = await _users.GetUserByEmail(claimEmail);

            var organization = await _organizations.GetOrganizationByUserId(user.Id);
            if (organization == null)
            {
                var projects = new CooperationViewModel[0];
                return projects;
            }

            var projectViewModels = await _cooperations.GetCooperationViewModelsByOrganizationId(organization.Id, take, skip);
            return projectViewModels;
        }

        [HttpGet]
        [Route("AddCooperation")]
        public async Task<IActionResult> AddCooperation()
        {
            ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
            var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
            var user = await _users.GetUserByEmail(claimEmail);

            var organization = await _organizations.GetOrganizationByUserId(user.Id);
            var minEndDate = DateTime.Now;

            var addCooperationViewModel = new AddCooperationViewModel()
            {
                OrganizationName = organization.Name,
                MinEndDate = FromDateToString(minEndDate)
            };

            return View(addCooperationViewModel);
        }

        [HttpPost]
        [Route("AddCooperation")]
        public async Task<IActionResult> AddCooperation(AddCooperationViewModel model)
        {
            ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
            var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
            var user = await _users.GetUserByEmail(claimEmail);

            if (!ValidCooperation(model))
            {
                var organization = await _organizations.GetOrganizationByUserId(user.Id);
                model.OrganizationName = organization.Name;

                var minEndDate = DateTime.Now;
                model.MinEndDate = FromDateToString(minEndDate);
                return View(model);
            }
            try
            {
                await _cooperations.Save(model);
            }
            catch (Exception ex)
            {

            };
            return RedirectToAction("Cooperations");
        }

        [HttpGet]
        [Route("LookingCooperation")]
        public async Task<IActionResult> LookingCooperation(Guid cooperationId)
        {
            ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
            var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
            var user = await _users.GetUserByEmail(claimEmail);

            var organization = await _organizations.GetOrganizationByUserId(user.Id);

            var cooperationViewModel = await _cooperations.GetCooperationViewModelById(cooperationId);

            var yourCooperation = cooperationViewModel.Organization.Id == organization?.Id;
            var lookingCooperation = new KeyValuePair<bool, CooperationViewModel>(yourCooperation, cooperationViewModel);

            return View(lookingCooperation);
        }

        [HttpGet]
        [Route("DeleteCooperation")]
        public async Task<IActionResult> DeleteCooperation(Guid cooperationId)
        {
            await _cooperations.Delete(cooperationId);
            return RedirectToAction("YourCooperations");
        }

        [HttpPost]
        [Route("SendNotificationCooperation/{cooperationId}")]
        public async Task SendNotificationCooperation([FromRoute] Guid cooperationId)
        {
            ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
            var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
            var user = await _users.GetUserByEmail(claimEmail);

            var cooperation = await _cooperations.GetCooperationById(cooperationId);
            if (cooperation != null)
            {
                var organization = await _organizations.GetOrganizationById(cooperation.OrganizationId);
                if (organization != null)
                {
                    var notification = new Notification()
                    {
                        UserId = organization.LeaderId,
                        Content = $"<a href='/GetUser?userId={user.Id}'>{user.FirstName} {user.LastName} {user.MiddleName}</a> подал(а) заявку на участие в сотрудничестве <a href='/LookingCooperation?cooperationId={cooperation.Id}'>{cooperation.Name}</a>"
                    };
                    await _notifications.Save(notification);
                }
            }

        }

        private bool ValidCooperation(AddCooperationViewModel model)
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
