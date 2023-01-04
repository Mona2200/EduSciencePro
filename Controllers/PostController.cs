using EduSciencePro.Data.Repos;
using EduSciencePro.Models;
using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Request;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Text;

namespace EduSciencePro.Controllers
{
   public class PostController : Controller
   {
      private readonly ITagRepository _tags;
      private readonly IUserRepository _users;
      private readonly IPostRepository _posts;

      public PostController(ITagRepository tags, IUserRepository users, IPostRepository posts)
      {
         _tags = tags;
         _users = users;
         _posts = posts;
      }

      public IActionResult Index()
      {
         return View();
      }

      [HttpGet]
      [Route("AddPost")]
      public async Task<IActionResult> AddPost() => View(new AddPostViewModel());

      [HttpPost]
      [Route("GetTagsSearch")]
      public async Task<Tag[]> GetTagsSearch(string search) => await _tags.GetTagsSearch(search);

      [HttpPost]
      [Route("AddPost")]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> AddPost(AddPostViewModel model)
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _users.GetUserByEmail(claimEmail);

         model.UserId = user.Id;

         if (!PostValid(model))
         {
            return View(model);
         }

         IFormFileCollection files = Request.Form.Files;
         if (files != null && files.Count != 0)
         {
            model.Cover = files[0];
         }
         await _posts.Save(model);

         return RedirectToAction("Main", "User");
      }

      private bool PostValid(AddPostViewModel model)
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
