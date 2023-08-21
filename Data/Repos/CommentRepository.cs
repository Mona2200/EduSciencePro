using AutoMapper;
using EduSciencePro.Data.Services;
using EduSciencePro.Models;
using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.Design;

namespace EduSciencePro.Data.Repos;

/// <summary>
/// Репозиторий для работы с комментариями.
/// </summary>
public class CommentRepository : ICommentRepository
{
    /// <summary>
    /// Контекст базы данных
    /// </summary>
    private readonly ApplicationDbContext _db;

    /// <summary>
    /// Маппер для преобразования типов.
    /// </summary>
    private readonly IMapper _mapper;

    /// <summary>
    /// Сервис для преобразование формата времени.
    /// </summary>
    private readonly DateTimeService _timeService;

    public CommentRepository(ApplicationDbContext db, IMapper mapper, DateTimeService timeService)
    {
        _db = db;
        _mapper = mapper;
        _timeService = timeService;
    }

    /// <inheritdoc/>
    public async Task<List<Comment>> GetComments()
    {
        return await _db.Comments.OrderByDescending(c => c.CreatedDate).ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<Comment?> GetCommentById(Guid id)
    {
        return await _db.Comments.SingleOrDefaultAsync(c => c.Id == id);
    }

    /// <inheritdoc/>
    public async Task<List<Comment>> GetCommentsByUserId(Guid userId)
    {
        return await _db.Comments.Where(c => c.UserId == userId).OrderByDescending(c => c.CreatedDate).ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<List<CommentViewModel>> GetCommentViewModelsByUserId(Guid userId, int take = 5, int skip = 0)
    {
        List<Comment> comments = await GetCommentsByUserId(userId);
        comments = comments.Take(take).SkipLast(skip).ToList();
        List<CommentViewModel> commentViewModels = new();
        foreach (Comment comment in comments)
        {
            CommentViewModel commentViewModel = _mapper.Map<Comment, CommentViewModel>(comment);
            commentViewModel.User = await _db.Users.SingleOrDefaultAsync(u => u.Id == comment.UserId);
            commentViewModel.CreatedDate = _timeService.GetDate(comment.CreatedDate);
            commentViewModels.Add(commentViewModel);
        }
        return commentViewModels;
    }

    /// <inheritdoc/>
    public async Task<List<Comment>> GetCommentsByPostId(Guid postId)
    {
        return await _db.Comments.OrderByDescending(p => p.CreatedDate).Where(c => c.PostId == postId).ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<List<CommentViewModel>> GetCommentViewModelsByPostId(Guid postId)
    {
        List<Comment> comments = await GetCommentsByPostId(postId);
        List<CommentViewModel> commentViewModels = new();
        foreach (Comment comment in comments)
        {
            CommentViewModel commentViewModel = _mapper.Map<Comment, CommentViewModel>(comment);
            commentViewModel.User = await _db.Users.FirstOrDefaultAsync(u => u.Id == comment.UserId);
            commentViewModel.CreatedDate = _timeService.GetDate(comment.CreatedDate);
            commentViewModels.Add(commentViewModel);
        }
        return commentViewModels;
    }

    /// <inheritdoc/>
    public async Task Save(AddCommentViewModel model)
    {
        var comment = _mapper.Map<AddCommentViewModel, Comment>(model);
        comment.CreatedDate = DateTime.Now;

        var entry = _db.Entry(comment);
        if (entry.State == EntityState.Detached)
            await _db.Comments.AddAsync(comment);
        await _db.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task Delete(Guid commentId)
    {
        Comment? comment = await GetCommentById(commentId);
        if (comment is null)
            return;

        _db.Comments.Remove(comment);
        await _db.SaveChangesAsync();
    }
}

/// <summary>
/// Интерфейс для работы с комментариями.
/// </summary>
public interface ICommentRepository
{
    /// <summary>
    /// Возвращает все комментарии, сортированные по убыванию даты создания.
    /// </summary>
    Task<List<Comment>> GetComments();

    /// <summary>
    /// Возвращает комментарий по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор комментария</param>
    Task<Comment?> GetCommentById(Guid id);

    /// <summary>
    /// Возвращает комментарии пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    Task<List<Comment>> GetCommentsByUserId(Guid userId);

    /// <summary>
    /// Возвращает комментарии пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="take">Количество извлекаемых комментариев</param>
    /// <param name="skip">Количество пропускаемых комментариев</param>
    Task<List<CommentViewModel>> GetCommentViewModelsByUserId(Guid userId, int take = 5, int skip = 0);

    /// <summary>
    /// Возвращает комментарии к публикации.
    /// </summary>
    /// <param name="postId">Идентификатор побликации</param>
    Task<List<Comment>> GetCommentsByPostId(Guid postId);

    /// <summary>
    /// Возвращает комментарии к публикации.
    /// </summary>
    /// <param name="postId">Идентификатор побликации</param>
    Task<List<CommentViewModel>> GetCommentViewModelsByPostId(Guid postId);

    /// <summary>
    /// Сохраняет комментарий.
    /// </summary>
    /// <param name="comment">Комментарий</param>
    Task Save(AddCommentViewModel comment);

    /// <summary>
    /// Удаляет комментарий.
    /// </summary>
    /// <param name="commentId">Идентификатор комментария</param>
    Task Delete(Guid commentId);
}
