using AutoMapper;
using EduSciencePro.Models;
using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Request;
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

      public async Task<Guid> SaveNews(AddPostViewModel post)
      {

         var newsPost = _mapper.Map<AddPostViewModel, NewsPost>(post);

         newsPost.CreatedDate = DateTime.Now;

         await _tags.Save(post.Tags.Split(new char[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries), newsPost.Id);

         if (post.Cover != null)
         {
            byte[] imageData = null;
            using (var fs1 = post.Cover.OpenReadStream())
            using (var ms1 = new MemoryStream())
            {
               fs1.CopyTo(ms1);
               imageData = ms1.ToArray();
            }
            newsPost.Cover = imageData;
         }

         var entry = _db.Entry(newsPost);
         if (entry.State == EntityState.Detached)
            await _db.News.AddAsync(newsPost);

         await _db.SaveChangesAsync();
         return newsPost.Id;
      }

      public async Task<Guid> SaveDiscussion(AddPostViewModel discussion)
      {

         var discussionPost = _mapper.Map<AddPostViewModel, DiscussionPost>(discussion);

         await _tags.Save(discussion.Tags.Split(new char[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries), discussionPost.Id);

         discussionPost.CreatedDate = DateTime.Now;

         var entry = _db.Entry(discussionPost);
         if (entry.State == EntityState.Detached)
            await _db.Discussions.AddAsync(discussionPost);

         await _db.SaveChangesAsync();
         return discussionPost.Id;
      }
   }

    public interface IPostRepository
    {
      //Task<Post[]> GetPosts();
      //Task<Post> GetPostById(int id);
      //Task<Post[]> GetPostsByUserId(Guid userId);

      //Task<Post[]> GetPostsByTags(Tag[] tags);
      Task<Guid> SaveNews(AddPostViewModel post);
      Task<Guid> SaveDiscussion(AddPostViewModel discussion);
    }
}
