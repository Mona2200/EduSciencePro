﻿using AutoMapper;
using EduSciencePro.Models;
using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace EduSciencePro.Data.Repos
{
    public class PostRepository : IPostRepository
    {
      private readonly ApplicationDbContext _db;
      private readonly IMapper _mapper;

      private readonly ITagRepository _tags;

      public PostRepository(ApplicationDbContext db, IMapper mapper, ITagRepository tags)
      {
         _db = db;
         _mapper = mapper;
         _tags = tags;
      }

      public async Task<Post[]> GetPosts() => await _db.Posts.ToArrayAsync();

      public async Task<PostViewModel[]> GetPostViewModels()
      {
         var posts = await GetPosts();
         var postViewModels = new PostViewModel[posts.Length];
         int i = 0;
         foreach (var post in posts)
         {
            postViewModels[i] = _mapper.Map<Post, PostViewModel>(post);

            var day = post.CreatedDate.Day.ToString();
            var month = post.CreatedDate.Month.ToString();
            string date = "";

            if (day.Length == 1)
               date += "0" + day + ".";
            else
               date += day + ".";

            if (month.Length == 1)
               date += "0" + month + ".";
            else
               date += month + ".";
            date += post.CreatedDate.Year;
            postViewModels[i].CreatedDate = date;

            postViewModels[i++].Tags = await _tags.GetTagsByPostId(post.Id);
         }
         return postViewModels;
      }

      public async Task<Post[]> GetPostsByUserId(Guid userId) => await _db.Posts.Where(p => p.UserId == userId).ToArrayAsync();

      public async Task<PostViewModel[]> GetPostViewModelsByUserId(Guid userId)
      {
         var posts = await GetPostsByUserId(userId);
         var postViewModels = new PostViewModel[posts.Length];
         int i = 0;
         foreach (var post in posts)
         {
            postViewModels[i] = _mapper.Map<Post, PostViewModel>(post);

            var day = post.CreatedDate.Day.ToString();
            var month = post.CreatedDate.Month.ToString();
            string date = "";

            if (day.Length == 1)
               date += "0" + day + ".";
            else
               date += day + ".";

            if (month.Length == 1)
               date += "0" + month + ".";
            else
               date += month + ".";
            date += post.CreatedDate.Year;
            postViewModels[i].CreatedDate = date;

            postViewModels[i++].Tags = await _tags.GetTagsByPostId(post.Id);
         }
         return postViewModels;
      }

      public async Task<PostViewModel[]> GetPostViewModelsNews()
      {
      var news = await _db.Posts.Where(p => p.IsNews == true).Take(4).ToArrayAsync();
         var postViewModels = new PostViewModel[news.Length];
         int i = 0;
         foreach (var post in news)
         {
            postViewModels[i] = _mapper.Map<Post, PostViewModel>(post);

            var day = post.CreatedDate.Day.ToString();
            var month = post.CreatedDate.Month.ToString();
            string date = "";

            if (day.Length == 1)
               date += "0" + day + ".";
            else
               date += day + ".";

            if (month.Length == 1)
               date += "0" + month + ".";
            else
               date += month + ".";
            date += post.CreatedDate.Year;
            postViewModels[i].CreatedDate = date;

            postViewModels[i++].Tags = await _tags.GetTagsByPostId(post.Id);
         }
         return postViewModels;
      }

      public async Task<PostViewModel[]> GetPostViewModelsDiscussions()
      {
         var discus = await _db.Posts.Where(p => p.IsNews == false).Take(4).ToArrayAsync();
         var postViewModels = new PostViewModel[discus.Length];
         int i = 0;
         foreach (var post in discus)
         {
            postViewModels[i] = _mapper.Map<Post, PostViewModel>(post);

            var day = post.CreatedDate.Day.ToString();
            var month = post.CreatedDate.Month.ToString();
            string date = "";

            if (day.Length == 1)
               date += "0" + day + ".";
            else
               date += day + ".";

            if (month.Length == 1)
               date += "0" + month + ".";
            else
               date += month + ".";
            date += post.CreatedDate.Year;
            postViewModels[i].CreatedDate = date;

            postViewModels[i++].Tags = await _tags.GetTagsByPostId(post.Id);
         }
         return postViewModels;
      }

      public async Task Save(AddPostViewModel model)
      {

         var post = _mapper.Map<AddPostViewModel, Post>(model);

         post.CreatedDate = DateTime.Now;

         await _tags.Save(model.Tags.Split(new char[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries), post.Id);

         if (model.Cover != null)
         {
            byte[] imageData = null;
            using (var fs1 = model.Cover.OpenReadStream())
            using (var ms1 = new MemoryStream())
            {
               fs1.CopyTo(ms1);
               imageData = ms1.ToArray();
            }
            post.Cover = imageData;
         }

         post.IsNews = model.IsNew;

         var entry = _db.Entry(post);
         if (entry.State == EntityState.Detached)
            await _db.Posts.AddAsync(post);

         await _db.SaveChangesAsync();
      }
   }

    public interface IPostRepository
    {
      Task<Post[]> GetPosts();
      Task<PostViewModel[]> GetPostViewModels();
      Task<Post[]> GetPostsByUserId(Guid userId);
      Task<PostViewModel[]> GetPostViewModelsByUserId(Guid userId);
      Task<PostViewModel[]> GetPostViewModelsNews();
      Task<PostViewModel[]> GetPostViewModelsDiscussions();
      //Task<Post> GetPostById(int id);
      //Task<Post[]> GetPostsByUserId(Guid userId);

      //Task<Post[]> GetPostsByTags(Tag[] tags);
      Task Save(AddPostViewModel post);
    }
}
