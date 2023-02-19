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
        public async Task<IActionResult> Courses(string? tagNamesString)
        {
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

            var courses = await _courses.GetCourseViewModels(tagNames, 5, 0);
            return View(new CoursesTagsViewModel() { Courses = courses, Tags = tags });
        }

        [HttpGet]
        [Route("CoursesTag/{tags}")]
        public async Task<IActionResult> CoursesTag([FromRoute] string? tags)
        {
            return RedirectToAction("Courses", "Course", new { tagNamesString = tags });
        }

        [HttpPost]
        [Route("CoursesMore/{take}/{skip}/{tags?}")]
        public async Task<CourseViewModel[]> CoursesMore([FromRoute] int take, [FromRoute] int skip, [FromRoute] string? tags = null)
        {
            string[] tagNames = null;
            if (tags != null)
            {
                tags = tags.Replace('_', '/');
                tagNames = tags.Split('/', StringSplitOptions.RemoveEmptyEntries);
            }

            var news = await _courses.GetCourseViewModels(tagNames, take, skip);
            return news;
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
            return View(new EditCourseViewModel() { CourseViewModel = courseViewModel, AddCourseViewModel = addCourseViewModel });
        }

        [HttpPost]
        [Route("EditCourse")]
        public async Task<IActionResult> EditCourse(EditCourseViewModel edit)
        {
            var model = edit.AddCourseViewModel;
            ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
            var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
            var user = await _users.GetUserByEmail(claimEmail);

            if (!ValidCourse(model))
            {
                var courseViewModel = await _courses.GetCourseViewModelByUserId(user.Id);
                return View(new EditCourseViewModel() { CourseViewModel = courseViewModel, AddCourseViewModel = model });
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
