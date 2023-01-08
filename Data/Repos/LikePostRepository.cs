using AutoMapper;
using EduSciencePro.Models;
using EduSciencePro.Models.User;
using Microsoft.EntityFrameworkCore;

namespace EduSciencePro.Data.Repos
{
   public class LikePostRepository : ILikePostRepository
   {
      private readonly ApplicationDbContext _db;

      public LikePostRepository(ApplicationDbContext db)
      {
         _db = db;
      }

      public async Task<LikePost> GetLikePostByPostIdUserId(Guid postId, Guid userId)
      {
         return await _db.LikePosts.FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);
      }

      public async Task<LikePost[]> GetLikePostsByPostId(Guid postId)
      {
         return await _db.LikePosts.Where(l => l.PostId == postId).ToArrayAsync();
      }

      public async Task Save(Guid postId, Guid userId)
      {
         if (await GetLikePostByPostIdUserId(postId, userId) == null)
         {
            var likePost = new LikePost() { PostId = postId, UserId = userId };

            var entry = _db.Entry(likePost);
            if (entry.State == EntityState.Detached)
               await _db.LikePosts.AddAsync(likePost);

            await _db.SaveChangesAsync();
         }
      }

      public async Task Delete(Guid postId, Guid userId)
      {
         var likePost = await GetLikePostByPostIdUserId(postId, userId);
         if (likePost != null)
         {
            _db.LikePosts.Remove(likePost);
            await _db.SaveChangesAsync();
         }
      }
   }

   public interface ILikePostRepository
   {
      Task<LikePost> GetLikePostByPostIdUserId(Guid postId, Guid userId);
      Task<LikePost[]> GetLikePostsByPostId(Guid postId);
      Task Save(Guid postId, Guid userId);
      Task Delete(Guid postId, Guid userId);
   }
}
