using EduSciencePro.Data.Repos;
using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduSciencePro.Controllers
{
   public class CourseController : Controller
   {
      private readonly ICourseRepository _courses;
      private readonly IUserRepository _users;
      public CourseController(ICourseRepository courses, IUserRepository users)
      {
         _courses = courses;
         _users = users;
      }

      [HttpGet]
      [Route("Courses")]
      public async Task<IActionResult> Courses()
      {
         var courses = await _courses.GetCourseViewModels();
         return View(courses);
      }

      [HttpGet]
      [Route("YourCourse")]
      public async Task<IActionResult> YourCourse()
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _users.GetUserByEmail(claimEmail);

         var course = await _courses.GetCourseViewModelByUserId(user.Id);
         return View(course);
      }

      [HttpGet]
      [Route("AddCourse")]
      public async Task<IActionResult> AddCourse()
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _users.GetUserByEmail(claimEmail);

         var course = new AddCourseViewModel();
         return View(course);
      }

      [HttpPost]
      [Route("AddCourse")]
      public async Task<IActionResult> AddCourse(AddCourseViewModel model)
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _users.GetUserByEmail(claimEmail);

         if (!ValidCourse(model))
         {
            return View(model);
         }

         await _courses.Save(model, user.Id);
         return RedirectToAction("Courses");
      }

      [HttpGet]
      [Route("EditCourse")]
      public async Task<IActionResult> EditCourse()
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _users.GetUserByEmail(claimEmail);

         var courseViewModel = await _courses.GetCourseViewModelByUserId(user.Id);
         var addCourseViewModel = new AddCourseViewModel();
         return View(new KeyValuePair<CourseViewModel, AddCourseViewModel>(courseViewModel, addCourseViewModel));
      }

      [HttpPost]
      [Route("EditCourse")]
      public async Task<IActionResult> EditCourse(AddCourseViewModel model)
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _users.GetUserByEmail(claimEmail);

         if (!ValidCourse(model))
         {
            var courseViewModel = await _courses.GetCourseViewModelByUserId(user.Id);
            return View(new KeyValuePair<CourseViewModel, AddCourseViewModel>(courseViewModel, model));
         }

         await _courses.Update(model, user.Id);
         return RedirectToAction("Courses");
      }

      [HttpGet]
      [Route("DeleteCourse")]
      public async Task<IActionResult> DeleteCourse(Guid id)
      {
         await _courses.Delete(id);
         return RedirectToAction("Courses");
      }

      private bool ValidCourse(AddCourseViewModel model)
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
   }
}
