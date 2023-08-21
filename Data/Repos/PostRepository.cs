using AutoMapper;
using EduSciencePro.Data.Services;
using EduSciencePro.Models;
using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using System.Text;

namespace EduSciencePro.Data.Repos;

public class PostRepository : IPostRepository
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    private readonly ITagRepository _tags;
    private readonly ICommentRepository _comments;

    /// <summary>
    /// Сервис для преобразование формата времени.
    /// </summary>
    private readonly DateTimeService _timeService;

    public PostRepository(ApplicationDbContext db, IMapper mapper, ITagRepository tags, ICommentRepository comments, DateTimeService timeService)
    {
        _db = db;
        _mapper = mapper;
        _tags = tags;
        _comments = comments;
        _timeService = timeService;
    }

    private async Task<PostViewModel> FromPostToPostViewModel(Post post)
    {
        PostViewModel postViewModel = _mapper.Map<Post, PostViewModel>(post);
        postViewModel.CreatedDate = _timeService.GetDate(post.CreatedDate);
        postViewModel.User = await _db.Users.SingleOrDefaultAsync(u => u.Id == post.UserId);
        postViewModel.Likes = await _db.LikePosts.Where(l => l.PostId == post.Id).ToListAsync();
        postViewModel.Comments = await _comments.GetCommentViewModelsByPostId(post.Id);
        postViewModel.Tags = await _tags.GetTagsByPostId(post.Id);
        return postViewModel;
    }

    public async Task<List<Post>> GetPosts()
    {
        return await _db.Posts.OrderByDescending(p => p.CreatedDate).ToListAsync();
    }

    public async Task<List<PostViewModel>> GetPostViewModels(int take = 5, int skip = 0)
    {
        List<Post> posts = await GetPosts();
        posts = posts.TakeLast(take).Skip(skip).ToList();
        List<PostViewModel> postViewModels = new();
        foreach (Post post in posts)
        {
            PostViewModel postViewModel = await FromPostToPostViewModel(post);
            postViewModels.Add(postViewModel);
        }
        return postViewModels;
    }

    public async Task<Post?> GetPostById(Guid postId)
    {
        return await _db.Posts.SingleOrDefaultAsync(u => u.Id == postId);
    }

    public async Task<PostViewModel?> GetPostViewModelById(Guid postId)
    {
        Post? post = await GetPostById(postId);
        if (post is null)
            return null;

        PostViewModel postViewModel = await FromPostToPostViewModel(post);
        return postViewModel;
    }

    public async Task<List<Post>> GetPostsByUserId(Guid userId)
    {
        return await _db.Posts.OrderByDescending(p => p.CreatedDate).Where(p => p.UserId == userId).ToListAsync();
    }

    public async Task<List<PostViewModel>> GetPostViewModelsByUserId(Guid userId, int take = 5, int skip = 0)
    {
        List<Post> posts = await GetPostsByUserId(userId);
        posts = posts.Take(take).Skip(skip).ToList();
        List<PostViewModel> postViewModels = new();
        foreach (Post post in posts)
        {
            PostViewModel postViewModel = await FromPostToPostViewModel(post);
            postViewModels.Add(postViewModel);
        }
        return postViewModels;
    }

    public async Task<List<PostViewModel>> GetPostViewModelsNews(string[] tagNames = null, int take = 10, int skip = 0)
    {
        List<Post> news = new();
        if (tagNames != null && tagNames.Length != 0)
        {
            foreach (string tagName in tagNames)
            {
                Tag? tag = await _tags.GetTagByName(tagName);
                if (tag != null)
                {
                    List<TagPost> tagPosts = await _db.TagPosts.Where(t => t.TagId == tag.Id).ToListAsync();
                    foreach (TagPost tagPost in tagPosts)
                    {
                        Post? tagNew = await _db.Posts.FirstOrDefaultAsync(p => p.IsNews && p.Id == tagPost.PostId);
                        if (tagNew != null && news.SingleOrDefault(n => n.Id == tagNew.Id) == null)
                            news.Add(tagNew);
                    }
                }
            }
        }
        else
            news = await _db.Posts.Where(p => p.IsNews == true).ToListAsync();
        news = news.OrderByDescending(n => n.CreatedDate).TakeLast(take).Skip(skip).ToList();
        List<PostViewModel> postViewModels = new();
        foreach (Post post in news)
        {
            PostViewModel postViewModel = await FromPostToPostViewModel(post);
            postViewModels.Add(postViewModel);
        }
        return postViewModels;
    }

    public async Task<List<PostViewModel>> GetPostViewModelsDiscussions(string[] tagNames = null, int take = 10, int skip = 0)
    {
        List<Post> discus = new();
        if (tagNames != null && tagNames.Length != 0)
        {
            foreach (string tagName in tagNames)
            {
                Tag? tag = await _tags.GetTagByName(tagName);
                if (tag != null)
                {
                    List<TagPost> tagPosts = await _db.TagPosts.Where(t => t.TagId == tag.Id).ToListAsync();
                    foreach (TagPost tagPost in tagPosts)
                    {
                        Post? tagNew = await _db.Posts.FirstOrDefaultAsync(p => !p.IsNews && p.Id == tagPost.PostId);
                        if (tagNew != null && discus.FirstOrDefault(n => n.Id == tagNew.Id) == null)
                            discus.Add(tagNew);
                    }
                }
            }
        }
        else
            discus = await _db.Posts.Where(p => p.IsNews == false).ToListAsync();
        discus = discus.OrderByDescending(n => n.CreatedDate).TakeLast(take).Skip(skip).ToList();
        List<PostViewModel> postViewModels = new();
        foreach (Post post in discus)
        {
            PostViewModel postViewModel = await FromPostToPostViewModel(post);
            postViewModels.Add(postViewModel);
        }
        return postViewModels;
    }

    public async Task Save(AddPostViewModel model)
    {
        Post post = _mapper.Map<AddPostViewModel, Post>(model);
        post.CreatedDate = DateTime.Now;

        if (model.Tags != null)
            await _tags.Save(model.Tags.Split('/', StringSplitOptions.RemoveEmptyEntries), post.Id);

        if (model.Cover != null)
        {
            byte[]? imageData = null;
            using (Stream fs1 = model.Cover.OpenReadStream())
            using (MemoryStream ms1 = new MemoryStream())
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

    public async Task Update(EditPostViewModel model)
    {
        Post? post = await GetPostById(model.Id);
        if (post is null)
            return;

        if (model.Tags == null) model.Tags = "";
        await _tags.Save(model.Tags.Split('/', StringSplitOptions.RemoveEmptyEntries), model.Id);
        if (model.Cover != null)
        {
            byte[]? imageData = null;
            using (Stream fs1 = model.Cover.OpenReadStream())
            using (MemoryStream ms1 = new MemoryStream())
            {
                fs1.CopyTo(ms1);
                imageData = ms1.ToArray();
            }
            post.Cover = imageData;
        }

        post.Title = model.Title;
        post.Content = Encoding.UTF8.GetBytes(model.Content);

        post.IsNews = model.IsNew;

        var entry = _db.Entry(post);
        if (entry.State == EntityState.Detached)
            _db.Posts.Update(post);

        await _db.SaveChangesAsync();
    }

    public async Task Delete(Guid postId)
    {
        Post? post = await GetPostById(postId);
        if (post is null)
            return;

        _db.Posts.Remove(post);

        List<TagPost> tagPosts = await _db.TagPosts.Where(t => t.PostId == postId).ToListAsync();
        foreach (TagPost tag in tagPosts)
            _db.TagPosts.Remove(tag);

        List<LikePost> likes = await _db.LikePosts.Where(l => l.PostId == postId).ToListAsync();
        foreach (var like in likes)
            _db.LikePosts.Remove(like);

        List<Comment> comments = await _db.Comments.Where(c => c.PostId == postId).ToListAsync();
        foreach (var comm in comments)
            _db.Comments.Remove(comm);

        await _db.SaveChangesAsync();
    }
}

public interface IPostRepository
{
    Task<List<Post>> GetPosts();
    Task<List<PostViewModel>> GetPostViewModels(int take = 5, int skip = 0);
    Task<Post?> GetPostById(Guid postId);
    Task<PostViewModel?> GetPostViewModelById(Guid postId);
    Task<List<Post>> GetPostsByUserId(Guid userId);
    Task<List<PostViewModel>> GetPostViewModelsByUserId(Guid userId, int take = 5, int skip = 0);
    Task<List<PostViewModel>> GetPostViewModelsNews(string[] tagNames, int take, int skip);
    Task<List<PostViewModel>> GetPostViewModelsDiscussions(string[] tagNames, int take, int skip);
    //Task<Post> GetPostById(int id);
    //Task<Post[]> GetPostsByUserId(Guid userId);

    //Task<Post[]> GetPostsByTags(Tag[] tags);
    Task Save(AddPostViewModel post);
    Task Update(EditPostViewModel post);
    Task Delete(Guid postId);
}
