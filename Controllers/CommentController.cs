using EduSciencePro.Data.Repos;
using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduSciencePro.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentRepository _comments;
        private readonly IUserRepository _users;
        private readonly IPostRepository _posts;

        public CommentController(ICommentRepository comments, IUserRepository users, IPostRepository posts)
        {
            _comments = comments;
            _users = users;
            _posts = posts;
        }

        [HttpGet]
        [Route("Comments")]
        public async Task<IActionResult> Comments()
        {
            ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
            var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
            var user = await _users.GetUserByEmail(claimEmail);

            List<CommentViewModel> comments = new();

            var posts = await _posts.GetPostViewModelsByUserId(user.Id);

            foreach (var post in posts)
            {
                var commentsPost = await _comments.GetCommentViewModelsByPostId(post.Id);
                comments = comments.Concat(commentsPost).ToList();
            }
            return View(comments.Take(5).ToArray());
        }

        [HttpPost]
        [Route("CommentsByUserMore/{take}/{skip}")]
        public async Task<CommentViewModel[]> CommentsByUserMore([FromRoute] int take, [FromRoute] int skip)
        {
            ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
            var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
            var user = await _users.GetUserByEmail(claimEmail);

            List<CommentViewModel> comments = new();

            var posts = await _posts.GetPostViewModelsByUserId(user.Id);

            foreach (var post in posts)
            {
                var commentsPost = await _comments.GetCommentViewModelsByPostId(post.Id);
                comments = comments.Concat(commentsPost).ToList();
            }

            return comments.Take(take).Skip(skip).ToArray();
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

        [HttpPost]
        [Route("CommentsMore/{postId}/{take}/{skip}")]
        public async Task<CommentViewModel[]> CommentsMore([FromRoute] Guid postId, [FromRoute] int take, [FromRoute] int skip)
        {
            var comments = await _comments.GetCommentViewModelsByPostId(postId);
            return comments.Take(take).Skip(skip).ToArray();
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
