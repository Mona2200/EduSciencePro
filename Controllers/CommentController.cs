using EduSciencePro.Data.Repos;
using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Request;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduSciencePro.Controllers
{
   public class CommentController : Controller
   {
      private readonly ICommentRepository _comments;
      private readonly IUserRepository _users;

      public CommentController(ICommentRepository comments, IUserRepository users)
      {
         _comments = comments;
         _users = users;
      }

      [HttpPost]
      [Route("AddComment")]
      public async Task<IActionResult> AddComment(AddCommentViewModel model)
      {
         //if (!ModelState.IsValid)
         //{
         //   foreach (var key in ModelState.Keys)
         //   {
         //      if (ModelState[key].Errors.Count > 0)
         //         ModelState.AddModelError($"{key}", $"{ModelState[key].Errors[0].ErrorMessage}");
         //   }
         //   return RedirectToAction("LookingPost", "Post", new {postId = model.PostId});
         //}

         if (model.Content == null || model.Content.Replace(" ", "") == "")
         {
            ModelState.AddModelError("AddCommentViewModel.Content", "Данное поле обязательно для заполнения");
            return RedirectToAction("LookingPost", "Post", new { postId = model.PostId });
         }

         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _users.GetUserByEmail(claimEmail);

         model.UserId = user.Id;
         await _comments.Save(model);
         return RedirectToAction("LookingPost", "Post", new { postId = model.PostId });
      }

      [HttpGet]
      [Route("DeleteComment")]
      public async Task<IActionResult> DeleteComment(Guid commentId, Guid postId)
      {
         await _comments.Delete(commentId);
         return RedirectToAction("LookingPost", "Post", new { postId = postId });
      }
   }
}
