using EduSciencePro.Data.Repos;
using EduSciencePro.Models;
using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;
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
      private readonly ILikePostRepository _likePosts;

      public PostController(ITagRepository tags, IUserRepository users, IPostRepository posts, ILikePostRepository likePosts)
      {
         _tags = tags;
         _users = users;
         _posts = posts;
         _likePosts = likePosts;
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

      [HttpGet]
      [Route("AllPosts")]
      public async Task<IActionResult> AllPosts()
      {
         var posts = new AllPostsViewModel()
         {
            News = await _posts.GetPostViewModelsNews(),
            Discuss = await _posts.GetPostViewModelsDiscussions()
         };
         return View(posts);
      }

      [HttpGet]
      [Route("NewsPosts")]
      public async Task<IActionResult> NewsPosts()
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _users.GetUserByEmail(claimEmail);

         var news = await _posts.GetPostViewModelsNews();

         var posts = new PostsAndUserIdViewModel() { Posts = news, UserId = user.Id };
         return View(posts);
      }

      [HttpGet]
      [Route("DiscussionPosts")]
      public async Task<IActionResult> DiscussionPosts()
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _users.GetUserByEmail(claimEmail);

         var discuss = await _posts.GetPostViewModelsDiscussions();
         var posts = new PostsAndUserIdViewModel() { Posts = discuss, UserId = user.Id };
         return View(posts);
      }

      [HttpGet]
      [Route("EditPost")]
      public async Task<IActionResult> EditPost(Guid postId)
      {
         var post = await _posts.GetPostViewModelById(postId);
         return View(post);
      }

      [HttpPost]
      [Route("EditPost")]
      public async Task<IActionResult> EditPost(EditPostViewModel model)
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
         await _posts.Update(model);

         return RedirectToAction("Main", "User");
      }

      [HttpGet]
      [Route("DeletePost")]
      public async Task<IActionResult> DeletePost(Guid postId)
      {
         await _posts.Delete(postId);
         return RedirectToAction("Main", "User");
      }

      [HttpGet]
      [Route("LookingPost")]
      public async Task<IActionResult> LookingPost(Guid postId)
      {
         var postViewModel = await _posts.GetPostViewModelById(postId);

         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name)?.Value;
         User user;
         if (claimEmail != null)
         {
            user = await _users.GetUserByEmail(claimEmail);
            var pair = new LookingPostViewModel() { Post = postViewModel, UserId = user.Id, AddComment = new AddCommentViewModel() { PostId = postId} };
            return View(pair);
         }
         else
         {
            var pair = new LookingPostViewModel() { Post = postViewModel, UserId = Guid.Empty, AddComment = new AddCommentViewModel() { PostId = postId} };
            return View(pair);
         }            
      }

      [HttpPost]
      [Route("LikePost")]
      public async Task<int> LikePost(Guid postId)
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _users.GetUserByEmail(claimEmail);

         var likePost = await _likePosts.GetLikePostByPostIdUserId(postId, user.Id);
         if (likePost != null)
            await _likePosts.Delete(postId, user.Id);
         else
            await _likePosts.Save(postId, user.Id);

         var likes = await _likePosts.GetLikePostsByPostId(postId);
         return likes.Length;
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

      private bool PostValid(EditPostViewModel model)
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
