﻿using EduSciencePro.Data.Repos;
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
        private readonly IConferenceRepository _conferences;

        public PostController(ITagRepository tags, IUserRepository users, IPostRepository posts, ILikePostRepository likePosts, IConferenceRepository conferences)
        {
            _tags = tags;
            _users = users;
            _posts = posts;
            _likePosts = likePosts;
            _conferences = conferences;
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
                News = (await _posts.GetPostViewModelsNews(null, 4, 0)).ToArray(),
                Discuss = (await _posts.GetPostViewModelsDiscussions(null, 6, 0)).ToArray(),
                Conferences = (await _conferences.GetConferenceViewModels()).Take(4).ToArray()
            };
            return View(posts);
        }

        [HttpGet]
        [Route("NewsPosts")]
        public async Task<IActionResult> NewsPosts(string? tagNamesString)
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

                var news = await _posts.GetPostViewModelsNews(tagNames, 5, 0);

                var posts = new PostsAndUserIdViewModel() { Posts = news, UserId = user.Id, Tags = tags };
                return View(posts);
            }
            else
            {
                var news = await _posts.GetPostViewModelsNews(tagNames, 5, 0);

                var posts = new PostsAndUserIdViewModel() { Posts = news, UserId = null, Tags = tags };
                return View(posts);
            }
        }

        [HttpGet]
        [Route("NewsPostsTag/{tags}")]
        public async Task<IActionResult> NewsPostsTag([FromRoute]string? tags)
        {
            return RedirectToAction("NewsPosts", "Post", new { tagNamesString = tags });
        }

        [HttpPost]
        [Route("NewsPostsMore/{take}/{skip}/{tags?}")]
        public async Task<PostViewModel[]> NewsPostsMore( [FromRoute] int take, [FromRoute] int skip, [FromRoute] string? tags = null)
        {           
            string[] tagNames = null;
            if (tags != null)
            {
                tags = tags.Replace('_', '/');
                tagNames = tags.Split('/', StringSplitOptions.RemoveEmptyEntries);
            }

            var news = await _posts.GetPostViewModelsNews(tagNames, take, skip);
            return news;
        }

        [HttpGet]
        [Route("DiscussionPosts")]
        public async Task<IActionResult> DiscussionPosts(string? tagNamesString)
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

                var discuss = await _posts.GetPostViewModelsDiscussions(tagNames, 5, 0);
                var posts = new PostsAndUserIdViewModel() { Posts = discuss, UserId = user.Id, Tags = tags };
                return View(posts);
            }
            else
            {
                var discuss = await _posts.GetPostViewModelsDiscussions(tagNames, 5, 0);
                var posts = new PostsAndUserIdViewModel() { Posts = discuss, UserId = null, Tags = tags };
                return View(posts);
            }

        }

        [HttpGet]
        [Route("DiscussionsPostsTag/{tags}")]
        public async Task<IActionResult> DiscussionsPostsTag([FromRoute] string? tags)
        {
            return RedirectToAction("DiscussionPosts", "Post", new { tagNamesString = tags });
        }

        [HttpPost]
        [Route("DiscussionsPostsMore/{take}/{skip}/{tags?}")]
        public async Task<PostViewModel[]> DiscussionsPostsMore([FromRoute] int take, [FromRoute] int skip, [FromRoute] string? tags = null)
        {
            string[] tagNames = null;
            if (tags != null)
            {
                tags = tags.Replace('_', '/');
                tagNames = tags.Split('/', StringSplitOptions.RemoveEmptyEntries);
            }

            var news = await _posts.GetPostViewModelsDiscussions(tagNames, take, skip);
            return news;
        }

        [HttpPost]
        [Route("PostsMore/{take}/{skip}")]
        public async Task<PostViewModel[]> PostsMore([FromRoute] int take, [FromRoute] int skip)
        {
            ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
            var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
            var user = await _users.GetUserByEmail(claimEmail);

            var news = await _posts.GetPostViewModelsByUserId(user.Id, take, skip);
            return news;
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

            postViewModel.Comments = postViewModel.Comments.Take(5).ToArray();

            ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
            var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name)?.Value;
            User user;
            if (claimEmail != null)
            {
                user = await _users.GetUserByEmail(claimEmail);
                var pair = new LookingPostViewModel() { Post = postViewModel, UserId = user.Id, AddComment = new AddCommentViewModel() { PostId = postId } };
                return View(pair);
            }
            else
            {
                var pair = new LookingPostViewModel() { Post = postViewModel, UserId = Guid.Empty, AddComment = new AddCommentViewModel() { PostId = postId } };
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
