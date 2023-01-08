using AutoMapper;
using Azure;
using EduSciencePro.Models;
using Microsoft.EntityFrameworkCore;

namespace EduSciencePro.Data.Repos
{
   public class TagRepository : ITagRepository
   {
      private readonly ApplicationDbContext _db;
      private readonly IMapper _mapper;

      public TagRepository(ApplicationDbContext db, IMapper mapper)
      {
         _db = db;
         _mapper = mapper;
      }

      public async Task<Tag[]> GetTags() => await _db.Tags.ToArrayAsync();

      public async Task<Tag[]> GetTagsByPostId(Guid postId)
      {
         var tagPosts = await _db.TagPosts.Where(t => t.PostId == postId).ToArrayAsync();
         var tags = new Tag[tagPosts.Length];
         int i = 0;
         foreach (var tagPost in tagPosts)
         {
            tags[i++] = await GetTagById(tagPost.TagId);
         }
         return tags;
      }

      public async Task<Tag> GetTagById(Guid id) => await _db.Tags.FirstOrDefaultAsync(t => t.Id == id);

      public async Task<Tag> GetTagByName(string name) => await _db.Tags.FirstOrDefaultAsync(t => t.Name == name);

      public async Task<Tag[]> GetTagsSearch(string search) => await _db.Tags.Where(t => t.Name.ToLower().Contains(search.ToLower())).ToArrayAsync();

      public async Task Save(string[] tags, Guid postId)
      {
         var oldTagPosts = await _db.TagPosts.Where(t => t.PostId == postId).ToArrayAsync();

         foreach (var tagPost in oldTagPosts)
         {
            _db.TagPosts.Remove(tagPost);
         }

         foreach (var tag in tags)
         {
            var tryTag = await GetTagByName(tag);
            if (tryTag == null)
            {
               var newTag = new Tag() { Name = tag };
               await _db.Tags.AddAsync(newTag);

               var tagPost = new TagPost() { PostId = postId, TagId = newTag.Id };
               await _db.TagPosts.AddAsync(tagPost);
            }
            else
            {
               var newTagPost = new TagPost() { PostId = postId, TagId = tryTag.Id };
               await _db.TagPosts.AddAsync(newTagPost);
            }
         }
      }
   }

   public interface ITagRepository
   {
      Task<Tag[]> GetTags();
      Task<Tag[]> GetTagsByPostId(Guid postId);
      Task<Tag> GetTagById(Guid id);
      Task<Tag[]> GetTagsSearch(string search);
      Task Save(string[] tags, Guid postId);
   }
}
