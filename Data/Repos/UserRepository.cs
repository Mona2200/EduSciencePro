using AutoMapper;
using EduSciencePro.Data.Repos;
using EduSciencePro.Data.Services;
using EduSciencePro.Models;
using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServiceStack.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using Swashbuckle.Swagger;
using System.Drawing.Imaging;
using System.Reflection.Metadata;
using Type = EduSciencePro.Models.User.TypeModel;

namespace EduSciencePro.Data.Repos;

/// <summary>
/// Репозиторий для работы с пользователями.
/// </summary>
public class UserRepository : IUserRepository
{
    /// <summary>
    /// Контекст базы данных.
    /// </summary>
    private readonly ApplicationDbContext _db;

    /// <summary>
    /// Маппер для преобразования типов.
    /// </summary>
    private readonly IMapper _mapper;

    /// <summary>
    /// Репозиторий для работы с резюме.
    /// </summary>
    private readonly IResumeRepository _resumes;

    /// <summary>
    /// Репозиторий для работы с типами пользователей.
    /// </summary>
    private readonly ITypeRepository _types;

    /// <summary>
    /// Репозиторий для работы с публикациями.
    /// </summary>
    private readonly IPostRepository _posts;

    /// <summary>
    /// Сервис для преобразование формата времени.
    /// </summary>
    private readonly DateTimeService _timeService;

    public UserRepository(ApplicationDbContext db, IMapper mapper, IResumeRepository resumes, ITypeRepository types, IPostRepository posts, DateTimeService timeService)
    {
        _db = db;
        _mapper = mapper;
        _resumes = resumes;
        _types = types;
        _posts = posts;
        _timeService = timeService;
    }

    /// <inheritdoc/>
    public async Task<List<User>> GetUsers()
    {
        return await _db.Users.ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<User?> GetUserById(Guid id)
    {
        return await _db.Users.SingleOrDefaultAsync(u => u.Id == id);
    }

    /// <inheritdoc/>
    public async Task<User?> GetUserByEmail(string email)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    /// <inheritdoc/>
    public async Task<List<UserViewModel>> GetUserViewModels(int take = 5, int skip = 0)
    {
        List<User> users = await GetUsers();
        users.Take(take).Skip(skip);
        List<UserViewModel> userViewModels = new();
        foreach (var user in users)
        {
            UserViewModel userViewModel = _mapper.Map<User, UserViewModel>(user);
            userViewModel.Birthday = _timeService.GetDate(DateOnly.Parse(user.Birthday));
            userViewModel.TypeUsers = await _types.GetTypesByUserId(user.Id);

            List<Link> links = await _db.Links.Where(l => l.UserId == user.Id).ToListAsync();
            links.ForEach(l => l.Url.Replace(" ", ""));
            userViewModel.Links = links;

            if (user.ResumeId != null)
                userViewModel.Resume = await _resumes.GetResumeViewModelById(user.ResumeId.Value);
            userViewModel.Role = await _db.Roles.SingleOrDefaultAsync(r => r.Id == user.RoleId);

            userViewModels.Add(userViewModel);
        }
        return userViewModels;
    }

    /// <inheritdoc/>
    public async Task<List<UserViewModel>> ShortInfoUserViewModels()
    {
        List<User> users = await GetUsers();
        List<UserViewModel> userViewModels = new();
        foreach (User user in users)
        {
            UserViewModel userViewModel = _mapper.Map<User, UserViewModel>(user);
            userViewModel.TypeUsers = await _types.GetTypesByUserId(user.Id);
            userViewModels.Add(userViewModel);
        }
        return userViewModels;
    }

    /// <inheritdoc/>
    public async Task<UserViewModel?> GetUserViewModelById(Guid id)
    {
        User? user = await GetUserById(id);
        if (user is null)
            return null;

        UserViewModel userViewModel = _mapper.Map<User, UserViewModel>(user);
        userViewModel.Birthday = _timeService.GetDate(DateOnly.Parse(user.Birthday));
        userViewModel.TypeUsers = await _types.GetTypesByUserId(user.Id);

        List<Link> links = await _db.Links.Where(l => l.UserId == user.Id).ToListAsync();
        links.ForEach(l => l.Url.Replace(" ", ""));
        userViewModel.Links = links;

        if (user.ResumeId != null)
            userViewModel.Resume = await _resumes.GetResumeViewModelById(user.ResumeId.Value);
        userViewModel.Role = await _db.Roles.SingleOrDefaultAsync(r => r.Id == user.RoleId);

        userViewModel.Posts = await _posts.GetPostViewModelsByUserId(user.Id);

        return userViewModel;
    }

    /// <inheritdoc/>
    public async Task<UserViewModel?> GetUserViewModelByEmail(string email)
    {
        User? user = await GetUserByEmail(email);
        if (user is null)
            return null;

        UserViewModel userViewModel = _mapper.Map<User, UserViewModel>(user);
        userViewModel.Birthday = _timeService.GetDate(DateOnly.Parse(user.Birthday));
        userViewModel.TypeUsers = await _types.GetTypesByUserId(user.Id);

        List<Link> links = await _db.Links.Where(l => l.UserId == user.Id).ToListAsync();
        links.ForEach(l => l.Url.Replace(" ", ""));
        userViewModel.Links = links;

        if (user.ResumeId != null)
            userViewModel.Resume = await _resumes.GetResumeViewModelById(user.ResumeId.Value);
        userViewModel.Role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == user.RoleId);

        userViewModel.Posts = await _posts.GetPostViewModelsByUserId(user.Id);

        return userViewModel;
    }

    /// <inheritdoc/>
    public async Task Save(AddUserViewModel model)
    {
        User user = _mapper.Map<AddUserViewModel, User>(model);

        List<Guid> typeIds = model.TypeUsers.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(t => Guid.Parse(t)).ToList();
        foreach (var typeId in typeIds)
        {
            TypeUser typeUser = new TypeUser() { TypeId = typeId, UserId = user.Id };
            await _db.TypeUsers.AddAsync(typeUser);
        }

        Role? role = await _db.Roles.FirstOrDefaultAsync(r => r.Name == "Пользователь");
        user.RoleId = role.Id;

        var entry = _db.Entry(user);
        if (entry.State == EntityState.Detached)
            await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task Update(AddUserViewModel model, User editUser)
    {
        if (!String.IsNullOrEmpty(model.FirstName))
            editUser.FirstName = model.FirstName;
        if (!String.IsNullOrEmpty(model.LastName))
            editUser.LastName = model.LastName;
        if (!String.IsNullOrEmpty(model.MiddleName))
            editUser.MiddleName = model.MiddleName;
        if (!String.IsNullOrEmpty(model.Gender))
            editUser.Gender = model.Gender;
        if (!String.IsNullOrEmpty(model.Birthday))
            editUser.Birthday = model.Birthday;
        if (!String.IsNullOrEmpty(model.TypeUsers))
        {
            List<TypeUser> typesUser = await _db.TypeUsers.Where(t => t.UserId == editUser.Id).ToListAsync();
            foreach (TypeUser typeUser in typesUser)
                _db.TypeUsers.Remove(typeUser);

            List<string> typesArray = model.TypeUsers.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (string type in typesArray)
            {
                TypeUser typeUser = new TypeUser() { TypeId = Guid.Parse(type), UserId = editUser.Id };
                await _db.TypeUsers.AddAsync(typeUser);
            }

        }

        Link? oldLinkTelegram = await _db.Links.FirstOrDefaultAsync(l => l.UserId == editUser.Id && l.Name == "Telegram");
        if (!String.IsNullOrEmpty(model.TelegramLink))
        {
            if (oldLinkTelegram != null)
                _db.Links.Remove(oldLinkTelegram);
            Link link = new Link() { Name = "Telegram", Url = model.TelegramLink, UserId = editUser.Id };
            await _db.AddAsync(link);
        }
        else if (oldLinkTelegram != null)
            _db.Links.Remove(oldLinkTelegram);

        Link? oldLinkWhatsApp = await _db.Links.FirstOrDefaultAsync(l => l.UserId == editUser.Id && l.Name == "WhatsApp");
        if (!String.IsNullOrEmpty(model.WhatsAppLink))
        {
            if (oldLinkWhatsApp != null)
                _db.Links.Remove(oldLinkWhatsApp);
            var linck = new Link() { Name = "WhatsApp", Url = model.WhatsAppLink, UserId = editUser.Id };
            await _db.AddAsync(linck);
        }
        else if (oldLinkWhatsApp != null)
            _db.Links.Remove(oldLinkWhatsApp);

        Link? oldLinkEmail = await _db.Links.FirstOrDefaultAsync(l => l.UserId == editUser.Id && l.Name == "Email");
        if (!String.IsNullOrEmpty(model.EmailLink))
        {
            if (oldLinkEmail != null)
                _db.Links.Remove(oldLinkEmail);
            var linck = new Link() { Name = "Email", Url = model.EmailLink, UserId = editUser.Id };
            await _db.AddAsync(linck);
        }
        else if (oldLinkEmail != null)
            _db.Links.Remove(oldLinkEmail);

        var oldLinkAnother = await _db.Links.FirstOrDefaultAsync(l => l.UserId == editUser.Id && l.Name == "");
        if (!String.IsNullOrEmpty(model.AnotherLink))
        {
            if (oldLinkAnother != null)
                _db.Links.Remove(oldLinkAnother);
            var linck = new Link() { Name = "", Url = model.AnotherLink, UserId = editUser.Id };
            await _db.AddAsync(linck);
        }
        else if (oldLinkAnother != null)
            _db.Links.Remove(oldLinkAnother);

        if (model.Img != null)
        {
            Image img = Image.Load(model.Img.OpenReadStream());
            img.Mutate(h => h.Resize(300, 300));

            byte[]? imageData = null;
            using (var ms1 = new MemoryStream())
            {
                img.Save(ms1, new PngEncoder());
                imageData = ms1.ToArray();
            }
            editUser.Image = imageData;
        }

        var entry = _db.Entry(editUser);
        if (entry.State == EntityState.Detached)
            _db.Users.Update(editUser);

        await _db.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task UpdatePassword(User user)
    {
        var entry = _db.Entry(user);
        if (entry.State == EntityState.Detached)
            _db.Users.Update(user);

        await _db.SaveChangesAsync();
    }

    /// <inheritdoc/>
    private byte[] GetByteArrayFromImage(IFormFile file)
    {
        using (var target = new MemoryStream())
        {
            file.CopyTo(target);
            return target.ToArray();
        }
    }

    /// <inheritdoc/>
    public async Task Delete(Guid id)
    {
        User? user = await GetUserById(id);
        if (user is null)
            return;

        _db.Users.Remove(user);

        List<TypeUser> typeUsers = await _db.TypeUsers.Where(t => t.UserId == user.Id).ToListAsync();
        foreach (TypeUser typeUser in typeUsers)
            _db.TypeUsers.Remove(typeUser);

        List<Link> links = await _db.Links.Where(l => l.UserId == user.Id).ToListAsync();
        foreach (var link in links)
            _db.Links.Remove(link);

        if (user.ResumeId != null)
        {
            Resume? resume = await _resumes.GetResumeById(user.ResumeId.Value);
            if (resume != null)
                _db.Resumes.Remove(resume);
        }

        await _db.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task DeleteImage(Guid userId)
    {
        User? user = await GetUserById(userId);
        if (user is null)
            return;

        user.Image = null;

        var entry = _db.Entry(user);
        if (entry.State == EntityState.Detached)
            _db.Users.Update(user);

        await _db.SaveChangesAsync();
    }

    public async Task UpdateBase()
    {
        Guid id = Guid.Parse("939C6AB6-BE98-4ED9-BEAB-0557B46FF941");
        var user = _db.Users.FirstOrDefault(u => u.Id == id);
        var resume = _db.Resumes.FirstOrDefault(r => r.Id == user.ResumeId);
        if (resume != null)
        {
            var skillsResume = _db.ResumeSkills.Where(r => r.ResumeId == resume.Id).ToList();
            foreach (var skillResume in skillsResume)
                if (skillResume != null)
                    _db.ResumeSkills.Remove(skillResume);

            _db.Resumes.Remove(resume);
        }


        var userOrganization = _db.UserOrganizations.FirstOrDefault(u => u.IdUser == id);
        if (userOrganization != null)
            _db.UserOrganizations.Remove(userOrganization!);

        var typeUsers = _db.TypeUsers.Where(t => t.UserId == id).ToList();
        foreach (var typeUser in typeUsers)
            if (typeUser != null)
                _db.TypeUsers.Remove(typeUser);

        var userlikePosts = _db.LikePosts.Where(l => l.UserId == user.Id);
        foreach (var likePost in userlikePosts)
            if (likePost != null)
                _db.LikePosts.Remove(likePost);

        var usercomments = _db.Comments.Where(c => c.UserId == user.Id);
        foreach (var comment in usercomments)
            if (comment != null)
                _db.Comments.Remove(comment);

        var notifications = _db.Notifications.Where(n => n.UserId == user.Id).ToList();
        foreach (var notification in notifications)
            if (notification != null)
                _db.Notifications.Remove(notification);

        var messages = _db.Messages.Where(m => m.SenderId == user.Id || m.RecipientId == user.Id).ToList();
        foreach (var message in messages)
            if (message != null)
                _db.Messages.Remove(message);

        var links = _db.Links.Where(l => l.UserId == user.Id).ToList();
        foreach (var link in links)
            if (link != null)
                _db.Links.Remove(link);

        var dialogs = _db.Dialogs.Where(m => m.InterlocutorSecondId == user.Id || m.InterlocutorFirstId == user.Id).ToList();
        foreach (var dialog in dialogs)
            if (dialog != null)
                _db.Dialogs.Remove(dialog);

        var codes = _db.Codes.Where(c => c.Email == user.Email).ToList();
        foreach (var code in codes)
            if (code != null)
                _db.Codes.Remove(code);

        var posts = _db.Posts.Where(p => p.UserId == id).ToList();
        foreach (var post in posts)
        {
            if (post != null)
            {
                var tags = _db.TagPosts.Where(t => t.PostId == post.Id);
                foreach (var tag in tags)
                    if (tag != null)
                        _db.TagPosts.Remove(tag);

                var likePosts = _db.LikePosts.Where(l => l.PostId == post.Id);
                foreach (var likePost in likePosts)
                    if (likePost != null)
                        _db.LikePosts.Remove(likePost);

                var comments = _db.Comments.Where(c => c.PostId == post.Id);
                foreach (var comment in comments)
                    if (comment != null)
                        _db.Comments.Remove(comment);
                _db.Posts.Remove(post);
            }

        }

        var courses = _db.Courses.Where(c => c.UserId == user.Id).ToList();
        foreach (var course in courses)
        {
            if (course != null)
            {
                var skills = _db.CourseSkills.Where(c => c.CourseId == course.Id).ToList();
                foreach (var skill in skills)
                    if (skill != null)
                        _db.CourseSkills.Remove(skill);
                _db.Courses.Remove(course);
            }

        }

        //var organizations = _db.Organizations.Where(o => o.LeaderId == id).ToList();
        //foreach (var organization in organizations)
        //    if (organization != null)
        //        _db.Organizations.Remove(organization);

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
    }
}

/// <summary>
/// Интерфейс для работы с пользователями.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Возвращает всех пользователей.
    /// </summary>
    Task<List<User>> GetUsers();

    /// <summary>
    /// Возвращает пользователя по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    Task<User?> GetUserById(Guid id);

    /// <summary>
    /// Возвращает пользователя по email.
    /// </summary>
    /// <param name="email">email</param>
    Task<User?> GetUserByEmail(string email);

    /// <summary>
    /// Возвращает всех пользователей.
    /// </summary>
    /// <param name="take">Количество возвращаемых пользователей</param>
    /// <param name="skip">Количество пропускаемых пользователей</param>
    Task<List<UserViewModel>> GetUserViewModels(int take = 5, int skip = 0);

    /// <summary>
    /// Возвращает краткую информацию обо всех пользователях.
    /// </summary>
    Task<List<UserViewModel>> ShortInfoUserViewModels();

    /// <summary>
    /// Возвращает пользователя по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    Task<UserViewModel?> GetUserViewModelById(Guid id);

    /// <summary>
    /// Возвращает пользователя по email.
    /// </summary>
    /// <param name="email">email</param>
    Task<UserViewModel?> GetUserViewModelByEmail(string email);

    /// <summary>
    /// Сохраняет пользователя.
    /// </summary>
    /// <param name="model">Пользователь</param>
    Task Save(AddUserViewModel model);

    /// <summary>
    /// Обновляет информацию о пользователе.
    /// </summary>
    /// <param name="model">Новая информация о пользователе</param>
    /// <param name="editUser">Текущая информация о пользователе</param>
    Task Update(AddUserViewModel model, User editUser);

    /// <summary>
    /// Обновляет пароль пользователя.
    /// </summary>
    /// <param name="user">Пользователь</param>
    Task UpdatePassword(User user);

    /// <summary>
    /// Удаляет пользователя.
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    Task Delete(Guid id);

    /// <summary>
    /// Удаляет фотографию пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    Task DeleteImage(Guid userId);
    Task UpdateBase();
}
