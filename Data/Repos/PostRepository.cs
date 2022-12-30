using EduSciencePro.Models;
using EduSciencePro.ViewModels.Request;

namespace EduSciencePro.Data.Repos
{
    public class PostRepository
    {

    }

    public interface IPostRepository
    {
      Task<Post[]> GetPosts();
      Task<Post> GetPostById(int id);
      Task<Post[]> GetPostsByUserId(Guid userId);

      Task<Post[]> GetPostsByTags(Tag[] tags);
      Task Save(AddPostViewModel post);
    }
}
