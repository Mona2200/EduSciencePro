using EduSciencePro.Data.Repos;
using EduSciencePro.Models;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduSciencePro.Controllers
{
    public class InternshipController : Controller
    {
      private readonly IInternshipRepository _internships;
      private readonly IOrganizationRepository _organizations;
      private readonly IUserRepository _users;
      public InternshipController(IInternshipRepository internships, IOrganizationRepository organizations, IUserRepository users)
      {
         _internships = internships;
         _organizations = organizations;
         _users = users;
      }

      [HttpGet]
      [Route("Internships")]
      public async Task<IActionResult> Internships(string? tagNamesString)
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

            var internshipViewModels = await _internships.GetInternshipViewModels(tagNames, 5, 0);

         var organization = await _organizations.GetOrganizationByUserId(user.Id);

         if (organization != null)
         {
            return View(new IntershipsAndIsOrgViewModel() { IsOrg = true, Internships = internshipViewModels.Where(p => p.Organization.Id != organization.Id).ToArray(), Tags = tags });
         }
         else
            return View(new IntershipsAndIsOrgViewModel() { IsOrg = false, Internships = internshipViewModels, Tags = tags });

      }

        [HttpGet]
        [Route("InternshipsTag/{tags}")]
        public async Task<IActionResult> InternshipsTag([FromRoute] string? tags)
        {
            return RedirectToAction("Internships", "Internship", new { tagNamesString = tags });
        }

        [HttpPost]
        [Route("InternshipsMore/{take}/{skip}/{tags?}")]
        public async Task<InternshipViewModel[]> InternshipsMore([FromRoute] int take, [FromRoute] int skip, [FromRoute] string? tags = null)
        {
            string[] tagNames = null;
            if (tags != null)
            {
                tags = tags.Replace('_', '/');
                tagNames = tags.Split('/', StringSplitOptions.RemoveEmptyEntries);
            }

            var news = await _internships.GetInternshipViewModels(tagNames, take, skip);
            return news;
        }

        [HttpGet]
      [Route("YourInternships")]
      public async Task<IActionResult> YourInternships()
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _users.GetUserByEmail(claimEmail);

         var organization = await _organizations.GetOrganizationByUserId(user.Id);
         if (organization == null)
            return View(null);

         var internshipViewModels = await _internships.GetInternshipViewModelsByOrganizationId(organization.Id, 5, 0);
         return View(internshipViewModels);
      }

      [HttpGet]
      [Route("AddInternship")]
      public async Task<IActionResult> AddInternship()
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _users.GetUserByEmail(claimEmail);

         var organization = await _organizations.GetOrganizationByUserId(user.Id);
         var minStartDate = DateTime.Now;
         var minEndDate = new DateTime(minStartDate.Year, minStartDate.Month, minStartDate.Day);
            minEndDate.AddDays(7);

         var addInternshipViewModel = new AddIntershipViewModel()
         {
            OrganizationName = organization.Name,
            minStartDate = FromDateToString(minStartDate),
            minEndDate = FromDateToString(minEndDate)
         };

         return View(addInternshipViewModel);
      }

      [HttpPost]
      [Route("AddInternship")]
      public async Task<IActionResult> AddInternship(AddIntershipViewModel model)
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _users.GetUserByEmail(claimEmail);

         if (!ValidInternship(model))
         {
            var organization = await _organizations.GetOrganizationByUserId(user.Id);
            model.OrganizationName = organization.Name;

            var minStartDate = DateTime.Now;
            var minEndDate = new DateTime(minStartDate.Year, minStartDate.Month, minStartDate.Day);
            minEndDate.AddDays(7);
            model.minStartDate = FromDateToString(minStartDate);
            model.minEndDate = FromDateToString(minEndDate);
            return View(model);
         }
         try
         {
            await _internships.Save(model);
         }
         catch (Exception ex)
         {

         };
         return RedirectToAction("Internships");
      }

      [HttpGet]
      [Route("LookingInternship")]
      public async Task<IActionResult> LookingInternship(Guid internshipId)
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _users.GetUserByEmail(claimEmail);

         var organization = await _organizations.GetOrganizationByUserId(user.Id);

         var internshipViewModel = await _internships.GetInternshipViewModelById(internshipId);

         var yourProject = internshipViewModel.Organization.Id == organization?.Id;

         var lookingInternship = new KeyValuePair<bool, InternshipViewModel>(yourProject, internshipViewModel);

         return View(lookingInternship);
      }

      [HttpGet]
      [Route("DeleteInternship")]
      public async Task<IActionResult> DeleteInternship(Guid internshipId)
      {
         await _internships.Delete(internshipId);
         return RedirectToAction("YourInternships");
      }

      private bool ValidInternship(AddIntershipViewModel model)
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
