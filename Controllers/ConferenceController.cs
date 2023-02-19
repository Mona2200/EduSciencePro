using EduSciencePro.Data.Repos;
using EduSciencePro.Models;
using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduSciencePro.Controllers
{
   public class ConferenceController : Controller
   {
      private readonly IConferenceRepository _conferences;
      private readonly IUserRepository _users;
      private readonly IOrganizationRepository _organizations;
      public ConferenceController(IConferenceRepository conferences, IUserRepository users, IOrganizationRepository organizations)
      {
         _conferences = conferences;
         _users = users;
         _organizations = organizations;
      }

      [HttpGet]
      [Route("Conferences")]
      public async Task<IActionResult> Conferences(string? tagNamesString)
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name)?.Value;

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

            if (claimEmail != null)
         {
            var user = await _users.GetUserByEmail(claimEmail);

            var conferences = await _conferences.GetConferenceViewModels(tagNames, 5, 0);

            var organization = await _organizations.GetOrganizationByUserId(user.Id);

            if (organization != null)
            {
               return View(new ConferencesAndIsOrgViewModel() { IsOrg = true, Conferences = conferences.Where(c => c.Organization.Id != organization.Id).ToArray(), Tags = tags });
            }
            else
               return View(new ConferencesAndIsOrgViewModel() { IsOrg = false, Conferences = conferences, Tags = tags });
         }
         else
         {
            var conferences = await _conferences.GetConferenceViewModels(tagNames, 5, 0);
            return View(new ConferencesAndIsOrgViewModel() { IsOrg = false, Conferences = conferences, Tags = tags });
         }
      }

        [HttpGet]
        [Route("ConferencesTag/{tags}")]
        public async Task<IActionResult> ConferencesTag([FromRoute] string? tags)
        {
            return RedirectToAction("Conferences", "Conference", new { tagNamesString = tags });
        }

        [HttpPost]
        [Route("ConferencesMore/{take}/{skip}/{tags?}")]
        public async Task<ConferenceViewModel[]> ConferencesMore([FromRoute] int take, [FromRoute] int skip, [FromRoute] string? tags = null)
        {
            string[] tagNames = null;
            if (tags != null)
            {
                tags = tags.Replace('_', '/');
                tagNames = tags.Split('/', StringSplitOptions.RemoveEmptyEntries);
            }

            var news = await _conferences.GetConferenceViewModels(tagNames, take, skip);
            return news;
        }

        [HttpGet]
      [Route("YourConferences")]
      public async Task<IActionResult> YourConferences()
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _users.GetUserByEmail(claimEmail);

         var organization = await _organizations.GetOrganizationByUserId(user.Id);
         if (organization == null)
            return View(null);

         var conferenceViewModels = await _conferences.GetConferenceViewModelsByOrganizationId(organization.Id);
         return View(conferenceViewModels);
      }

        [HttpPost]
        [Route("YouConferencesMore/{take}/{skip}")]
        public async Task<ConferenceViewModel[]> YourConferencesMore([FromRoute] int take, [FromRoute] int skip)
        {
            ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
            var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
            var user = await _users.GetUserByEmail(claimEmail);

            var organization = await _organizations.GetOrganizationByUserId(user.Id);
            if (organization == null)
            {
                var projects = new ConferenceViewModel[0];
                return projects;
            }

            var projectViewModels = await _conferences.GetConferenceViewModelsByOrganizationId(organization.Id, take, skip);
            return projectViewModels;
        }

        [HttpGet]
      [Route("AddConference")]
      public async Task<IActionResult> AddConference()
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _users.GetUserByEmail(claimEmail);

         var organization = await _organizations.GetOrganizationByUserId(user.Id);
         var minEventDate = DateTime.Now;

         var addConferenceViewModel = new AddConferenceViewModel()
         {
            OrganizationName = organization.Name,
            MinEventDate = FromDateToString(minEventDate)
         };

         return View(addConferenceViewModel);
      }

      [HttpPost]
      [Route("AddConference")]
      public async Task<IActionResult> AddConference(AddConferenceViewModel model)
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _users.GetUserByEmail(claimEmail);

         if (!ValidConference(model))
         {
            var organization = await _organizations.GetOrganizationByUserId(user.Id);
            model.OrganizationName = organization.Name;

            var minEventDate = DateTime.Now;
            model.MinEventDate = FromDateToString(minEventDate);
            return View(model);
         }
         try
         {
            await _conferences.Save(model);
         }
         catch (Exception ex)
         {

         };
         return RedirectToAction("Conferences");
      }

      [HttpGet]
      [Route("LookingConference")]
      public async Task<IActionResult> LookingConference(Guid conferenceId)
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name)?.Value;
         if (claimEmail != null)
         {
            var user = await _users.GetUserByEmail(claimEmail);

            var organization = await _organizations.GetOrganizationByUserId(user.Id);

            var conferenceViewModel = await _conferences.GetConferenceViewModelById(conferenceId);

            var yourConference = conferenceViewModel.Organization.Id == organization?.Id;
            var lookingConference = new KeyValuePair<bool, ConferenceViewModel>(yourConference, conferenceViewModel);

            return View(lookingConference);
         }
         else
         {
            var conferenceViewModel = await _conferences.GetConferenceViewModelById(conferenceId);

            var yourConference = true;
            var lookingConference = new KeyValuePair<bool, ConferenceViewModel>(yourConference, conferenceViewModel);

            return View(lookingConference);
         }
      }

      [HttpGet]
      [Route("DeleteConference")]
      public async Task<IActionResult> DeleteConference(Guid conferenceId)
      {
         await _conferences.Delete(conferenceId);
         return RedirectToAction("YourConferences");
      }

      private bool ValidConference(AddConferenceViewModel model)
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
