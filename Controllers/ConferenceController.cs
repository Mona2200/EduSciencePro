using EduSciencePro.Data.Repos;
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
      public async Task<IActionResult> Conferences()
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _users.GetUserByEmail(claimEmail);

         var conferences = await _conferences.GetConferenceViewModels();

         var organization = await _organizations.GetOrganizationByUserId(user.Id);

         if (organization != null)
         {
            return View(new KeyValuePair<bool, ConferenceViewModel[]>(true, conferences.Where(c => c.Organization.Id != organization.Id).ToArray()));
         }
         else
            return View(new KeyValuePair<bool, ConferenceViewModel[]>(false, conferences));
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

         var projectViewModels = await _conferences.GetConferenceViewModelsByOrganizationId(organization.Id);
         return View(projectViewModels);
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
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _users.GetUserByEmail(claimEmail);

         var organization = await _organizations.GetOrganizationByUserId(user.Id);

         var conferenceViewModel = await _conferences.GetConferenceViewModelById(conferenceId);

         var yourConference = conferenceViewModel.Organization.Id == organization.Id;
         var lookingConference = new KeyValuePair<bool, ConferenceViewModel>(yourConference, conferenceViewModel);

         return View(lookingConference);
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
