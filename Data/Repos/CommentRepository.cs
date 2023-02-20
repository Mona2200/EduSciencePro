using AutoMapper;
using EduSciencePro.Models;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace EduSciencePro.Data.Repos
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public CommentRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<CommentViewModel[]> GetCommentViewModelsByUserId(Guid userId, int take = 5, int skip = 0)
        {
            var comments = await _db.Comments.Where(c => c.UserId == userId).OrderByDescending(c => c.CreatedDate).ToListAsync();
            comments = comments.Take(take).SkipLast(skip).ToList();
            var commentViewModels = new CommentViewModel[comments.Count()];
            int i = 0;
            foreach (var comm in comments)
            {
                commentViewModels[i] = _mapper.Map<Comment, CommentViewModel>(comm);
                commentViewModels[i].User = await _db.Users.FirstOrDefaultAsync(u => u.Id == comm.UserId);

                var day = comm.CreatedDate.Day.ToString();
                var month = comm.CreatedDate.Month.ToString();
                string date = "";

                if (day.Length == 1)
                    date += "0" + day + ".";
                else
                    date += day + ".";

                if (month.Length == 1)
                    date += "0" + month + ".";
                else
                    date += month + ".";
                date += comm.CreatedDate.Year;
                commentViewModels[i++].CreatedDate = date;
            }
            return commentViewModels;
        }

        public async Task<Comment[]> GetCommentsByPostId(Guid postId)
        {
            return await _db.Comments.OrderByDescending(p => p.CreatedDate).Where(c => c.PostId == postId).ToArrayAsync();
        }

        public async Task<CommentViewModel[]> GetCommentViewModelsByPostId(Guid postId)
        {
            var comments = await GetCommentsByPostId(postId);
            var commentViewModels = new CommentViewModel[comments.Length];
            int i = 0;
            foreach (var comm in comments)
            {
                commentViewModels[i] = _mapper.Map<Comment, CommentViewModel>(comm);
                commentViewModels[i].User = await _db.Users.FirstOrDefaultAsync(u => u.Id == comm.UserId);

                var day = comm.CreatedDate.Day.ToString();
                var month = comm.CreatedDate.Month.ToString();
                string date = "";

                if (day.Length == 1)
                    date += "0" + day + ".";
                else
                    date += day + ".";

                if (month.Length == 1)
                    date += "0" + month + ".";
                else
                    date += month + ".";
                date += comm.CreatedDate.Year;
                commentViewModels[i++].CreatedDate = date;
            }
            return commentViewModels;
        }

        public async Task Save(AddCommentViewModel model)
        {
            var comment = _mapper.Map<AddCommentViewModel, Comment>(model);
            comment.CreatedDate = DateTime.Now;

            var entry = _db.Entry(comment);
            if (entry.State == EntityState.Detached)
                await _db.Comments.AddAsync(comment);
            await _db.SaveChangesAsync();
        }

        public async Task Delete(Guid commentId)
        {
            var comment = await _db.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
            if (comment != null)
            {
                _db.Comments.Remove(comment);
                await _db.SaveChangesAsync();
            }
        }
    }

    public interface ICommentRepository
    {
        //Task<Comment[]> GetComments();
        Task<CommentViewModel[]> GetCommentViewModelsByUserId(Guid userId, int take = 5, int skip = 0);
        Task<Comment[]> GetCommentsByPostId(Guid postId);
        Task<CommentViewModel[]> GetCommentViewModelsByPostId(Guid postId);
        Task Save(AddCommentViewModel comment);
        //Task Update(Comment comment);
        Task Delete(Guid commentId);
    }
}
