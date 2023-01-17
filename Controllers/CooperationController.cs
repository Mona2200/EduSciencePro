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
      public CooperationController(ICooperationRepository cooperations, IUserRepository users, IOrganizationRepository organizations)
      {
         _cooperations = cooperations;
         _users = users;
         _organizations = organizations;
      }

      [HttpGet]
      [Route("Cooperations")]
      public async Task<IActionResult> Cooperations()
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _users.GetUserByEmail(claimEmail);

         var cooperations = await _cooperations.GetCooperationViewModels();

         var organization = await _organizations.GetOrganizationByUserId(user.Id);

         if (organization != null)
         {
            return View(new KeyValuePair<bool, CooperationViewModel[]>(true, cooperations.Where(c => c.Organization.Id != organization.Id).ToArray()));
         }
         else
            return View(new KeyValuePair<bool, CooperationViewModel[]>(false, cooperations));
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

         var cooperationViewModels = await _cooperations.GetCooperationViewModelsByOrganizationId(organization.Id);
         return View(cooperationViewModels);
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
