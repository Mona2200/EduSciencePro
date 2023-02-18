using AutoMapper;
using EduSciencePro.Models;
using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using System.Text;

namespace EduSciencePro.Data.Repos
{
    public class PostRepository : IPostRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        private readonly ITagRepository _tags;
        private readonly ICommentRepository _comments;

        public PostRepository(ApplicationDbContext db, IMapper mapper, ITagRepository tags, ICommentRepository comments)
        {
            _db = db;
            _mapper = mapper;
            _tags = tags;
            _comments = comments;
        }

        public async Task<Post[]> GetPosts() => await _db.Posts.OrderByDescending(p => p.CreatedDate).ToArrayAsync();

        public async Task<PostViewModel[]> GetPostViewModels(int take = 5, int skip = 0)
        {
            var posts = await _db.Posts.OrderByDescending(p => p.CreatedDate).ToListAsync();
            posts = posts.TakeLast(take).Skip(skip).ToList();
            var postViewModels = new PostViewModel[posts.Count()];
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

                postViewModels[i].User = await _db.Users.FirstOrDefaultAsync(u => u.Id == post.UserId);
                var likePosts = await _db.LikePosts.Where(l => l.PostId == post.Id).ToArrayAsync();
                postViewModels[i].Likes = likePosts;
                postViewModels[i].Comments = await _comments.GetCommentViewModelsByPostId(post.Id);

                postViewModels[i++].Tags = await _tags.GetTagsByPostId(post.Id);
            }
            return postViewModels;
        }

        public async Task<Post> GetPostById(Guid postId) => await _db.Posts.FirstOrDefaultAsync(u => u.Id == postId);

        public async Task<PostViewModel> GetPostViewModelById(Guid postId)
        {
            var post = await GetPostById(postId);
            var postViewModel = _mapper.Map<Post, PostViewModel>(post);
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
            postViewModel.CreatedDate = date;

            postViewModel.User = await _db.Users.FirstOrDefaultAsync(u => u.Id == post.UserId);
            var likePosts = await _db.LikePosts.Where(l => l.PostId == post.Id).ToArrayAsync();
            postViewModel.Likes = likePosts;
            postViewModel.Comments = await _comments.GetCommentViewModelsByPostId(post.Id);

            postViewModel.Tags = await _tags.GetTagsByPostId(post.Id);
            return postViewModel;
        }

        public async Task<Post[]> GetPostsByUserId(Guid userId) => await _db.Posts.OrderByDescending(p => p.CreatedDate).Where(p => p.UserId == userId).ToArrayAsync();

        public async Task<PostViewModel[]> GetPostViewModelsByUserId(Guid userId, int take = 5, int skip = 0)
        {
            var posts = await _db.Posts.OrderByDescending(p => p.CreatedDate).Where(p => p.UserId == userId).ToListAsync();
            posts = posts.Take(take).Skip(skip).ToList();
            var postViewModels = new PostViewModel[posts.Count()];
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

                postViewModels[i].User = await _db.Users.FirstOrDefaultAsync(u => u.Id == post.UserId);
                var likePosts = await _db.LikePosts.Where(l => l.PostId == post.Id).ToArrayAsync();
                postViewModels[i].Likes = likePosts;
                postViewModels[i].Comments = await _comments.GetCommentViewModelsByPostId(post.Id);

                postViewModels[i++].Tags = await _tags.GetTagsByPostId(post.Id);
            }
            return postViewModels;
        }

        public async Task<PostViewModel[]> GetPostViewModelsNews(string[] tagNames = null, int take = 10, int skip = 0)
        {
            List<Post> news = new();
            if (tagNames != null && tagNames.Length != 0)
            {
                foreach (var tagName in tagNames)
                {
                    var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                    if (tag != null)
                    {
                        var tagPosts = await _db.TagPosts.Where(t => t.TagId == tag.Id).ToListAsync();
                        foreach (var tagPost in tagPosts)
                        {
                            var tagNew = await _db.Posts.FirstOrDefaultAsync(p => p.IsNews && p.Id == tagPost.PostId);
                            if (tagNew != null && news.FirstOrDefault(n => n.Id == tagNew.Id) == null)
                                news.Add(tagNew);
                        }
                    }
                }
            }
            else
                news = await _db.Posts.Where(p => p.IsNews == true).ToListAsync();
            news = news.OrderByDescending(n => n.CreatedDate).TakeLast(take).Skip(skip).ToList();
            var postViewModels = new PostViewModel[news.Count];
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

                postViewModels[i].User = await _db.Users.FirstOrDefaultAsync(u => u.Id == post.UserId);
                var likePosts = await _db.LikePosts.Where(l => l.PostId == post.Id).ToArrayAsync();
                postViewModels[i].Likes = likePosts;
                postViewModels[i].Comments = await _comments.GetCommentViewModelsByPostId(post.Id);

                postViewModels[i++].Tags = (await _tags.GetTagsByPostId(post.Id)).Take(4).ToArray();
            }
            return postViewModels;
        }

        public async Task<PostViewModel[]> GetPostViewModelsDiscussions(string[] tagNames = null, int take = 10, int skip = 0)
        {
            List<Post> discus = new();
            if (tagNames != null && tagNames.Length != 0)
            {
                foreach (var tagName in tagNames)
                {
                    var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                    if (tag != null)
                    {
                        var tagPosts = await _db.TagPosts.Where(t => t.TagId == tag.Id).ToListAsync();
                        foreach (var tagPost in tagPosts)
                        {
                            var tagNew = await _db.Posts.FirstOrDefaultAsync(p => !p.IsNews && p.Id == tagPost.PostId);
                            if (tagNew != null && discus.FirstOrDefault(n => n.Id == tagNew.Id) == null)
                                discus.Add(tagNew);
                        }
                    }
                }
            }
            else
                discus = await _db.Posts.Where(p => p.IsNews == false).ToListAsync();
            discus = discus.OrderByDescending(n => n.CreatedDate).TakeLast(take).Skip(skip).ToList();
            var postViewModels = new PostViewModel[discus.Count];
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

                postViewModels[i].User = await _db.Users.FirstOrDefaultAsync(u => u.Id == post.UserId);
                var likePosts = await _db.LikePosts.Where(l => l.PostId == post.Id).ToArrayAsync();
                postViewModels[i].Likes = likePosts;
                postViewModels[i].Comments = await _comments.GetCommentViewModelsByPostId(post.Id);

                postViewModels[i++].Tags = (await _tags.GetTagsByPostId(post.Id)).Take(4).ToArray();
            }
            return postViewModels;
        }

        public async Task Save(AddPostViewModel model)
        {

            var post = _mapper.Map<AddPostViewModel, Post>(model);

            post.CreatedDate = DateTime.Now;

            if (model.Tags != null)
                await _tags.Save(model.Tags.Split('/', StringSplitOptions.RemoveEmptyEntries), post.Id);

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

        public async Task Update(EditPostViewModel model)
        {
            var post = await GetPostById(model.Id);
            if (model.Tags == null) model.Tags = "";
            await _tags.Save(model.Tags.Split('/', StringSplitOptions.RemoveEmptyEntries), model.Id);
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
            var post = await GetPostById(postId);
            if (post != null)
            {
                _db.Remove(post);

                var tagPosts = await _db.TagPosts.Where(t => t.PostId == postId).ToArrayAsync();
                foreach (var tag in tagPosts)
                    _db.TagPosts.Remove(tag);

                var likes = await _db.LikePosts.Where(l => l.PostId == postId).ToArrayAsync();
                foreach (var like in likes)
                    _db.LikePosts.Remove(like);

                var comments = await _db.Comments.Where(c => c.PostId == postId).ToArrayAsync();
                foreach (var comm in comments)
                    _db.Comments.Remove(comm);

                await _db.SaveChangesAsync();
            }
        }
    }

    public interface IPostRepository
    {
        Task<Post[]> GetPosts();
        Task<PostViewModel[]> GetPostViewModels(int take = 5, int skip = 0);
        Task<Post> GetPostById(Guid postId);
        Task<PostViewModel> GetPostViewModelById(Guid postId);
        Task<Post[]> GetPostsByUserId(Guid userId);
        Task<PostViewModel[]> GetPostViewModelsByUserId(Guid userId, int take = 5, int skip = 0);
        Task<PostViewModel[]> GetPostViewModelsNews(string[] tagNames, int take, int skip);
        Task<PostViewModel[]> GetPostViewModelsDiscussions(string[] tagNames, int take, int skip);
        //Task<Post> GetPostById(int id);
        //Task<Post[]> GetPostsByUserId(Guid userId);

        //Task<Post[]> GetPostsByTags(Tag[] tags);
        Task Save(AddPostViewModel post);
        Task Update(EditPostViewModel post);
        Task Delete(Guid postId);
    }
}
