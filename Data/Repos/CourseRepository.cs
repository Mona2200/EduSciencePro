using AutoMapper;
using EduSciencePro.Models;
using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;
using Microsoft.EntityFrameworkCore;

namespace EduSciencePro.Data.Repos;

/// <summary>
/// Репозиторий для работы с курсами.
/// </summary>
public class CourseRepository : ICourseRepository
{
    /// <summary>
    /// Контекст базы данных.
    /// </summary>
    private readonly ApplicationDbContext _db;

    /// <summary>
    /// Маппер для преобразования типов.
    /// </summary>
    private readonly IMapper _mapper;

    public CourseRepository(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    /// <inheritdoc/>
    public async Task<List<Course>> GetCourses()
    {
        return await _db.Courses.ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<List<CourseViewModel>> GetCourseViewModels(string[]? skillNames = null, int take = 5, int skip = 0)
    {
        List<Course> courses = new();
        if (skillNames != null && skillNames.Length != 0)
        {
            foreach (string skillName in skillNames)
            {
                Skill? skill = await _db.Skills.FirstOrDefaultAsync(t => t.Name == skillName);
                if (skill != null)
                {
                    List<CourseSkill> courseSkills = await _db.CourseSkills.Where(t => t.SkillId == skill.Id).ToListAsync();
                    foreach (CourseSkill courseSkill in courseSkills)
                    {
                        Course? course = await _db.Courses.SingleOrDefaultAsync(p => p.Id == courseSkill.CourseId);
                        if (course != null && courses.SingleOrDefault(n => n.Id == course.Id) == null)
                            courses.Add(course);
                    }
                }
            }
        }
        else
            courses = await _db.Courses.ToListAsync();
        courses = courses.Take(take).Skip(skip).ToList();

        List<CourseViewModel> courseViewModels = new();
        foreach (Course course in courses)
        {
            CourseViewModel courseViewModel = _mapper.Map<Course, CourseViewModel>(course);
            courseViewModel.Education = await _db.Educations.SingleOrDefaultAsync(e => e.Id == course.EducationId);
            courseViewModel.PlaceWork = await _db.PlaceWorks.SingleOrDefaultAsync(p => p.Id == course.PlaceWorkId);
            courseViewModel.User = await _db.Users.SingleOrDefaultAsync(u => u.Id == course.UserId);
            List<CourseSkill> courseSkills = await _db.CourseSkills.Where(s => s.CourseId == course.Id).ToListAsync();
            List<Skill> skills = new();
            foreach (CourseSkill courseSkill in courseSkills)
            {
                Skill? skill = await _db.Skills.SingleOrDefaultAsync(s => s.Id == courseSkill.SkillId);
                if (skill != null)
                    skills.Add(skill);
            }
            courseViewModel.Skills = skills;
            courseViewModels.Add(courseViewModel);
        }
        return courseViewModels;
    }

    /// <inheritdoc/>
    public async Task<Course?> GetCourseById(Guid id)
    {
        return await _db.Courses.SingleOrDefaultAsync(c => c.Id == id);
    }

    /// <inheritdoc/>
    public async Task<CourseViewModel?> GetCourseViewModelById(Guid id)
    {
        Course? course = await GetCourseById(id);
        if (course is null)
            return null;

        CourseViewModel courseViewModel = _mapper.Map<Course, CourseViewModel>(course);
        courseViewModel.Education = await _db.Educations.SingleOrDefaultAsync(e => e.Id == course.EducationId);
        courseViewModel.PlaceWork = await _db.PlaceWorks.SingleOrDefaultAsync(p => p.Id == course.PlaceWorkId);
        courseViewModel.User = await _db.Users.SingleOrDefaultAsync(u => u.Id == course.UserId);
        List<CourseSkill> courseSkills = await _db.CourseSkills.Where(s => s.CourseId == course.Id).ToListAsync();
        List<Skill> skills = new();
        foreach (CourseSkill courseSkill in courseSkills)
        {
            Skill? skill = await _db.Skills.SingleOrDefaultAsync(s => s.Id == courseSkill.SkillId);
            if (skill != null)
                skills.Add(skill);
        }
        courseViewModel.Skills = skills;
        return courseViewModel;
    }

    /// <inheritdoc/>
    public async Task<Course?> GetCourseByUserId(Guid userId)
    {
        return await _db.Courses.SingleOrDefaultAsync(u => u.UserId == userId);
    }

    /// <inheritdoc/>
    public async Task<CourseViewModel?> GetCourseViewModelByUserId(Guid userId)
    {
        Course? course = await GetCourseByUserId(userId);
        if (course is null)
            return null;

        CourseViewModel courseViewModel = _mapper.Map<Course, CourseViewModel>(course);
        courseViewModel.Education = await _db.Educations.SingleOrDefaultAsync(e => e.Id == course.EducationId);
        courseViewModel.PlaceWork = await _db.PlaceWorks.SingleOrDefaultAsync(p => p.Id == course.PlaceWorkId);
        courseViewModel.User = await _db.Users.SingleOrDefaultAsync(u => u.Id == course.UserId);
        List<CourseSkill> courseSkills = await _db.CourseSkills.Where(s => s.CourseId == course.Id).ToListAsync();
        List<Skill> skills = new();
        foreach (CourseSkill courseSkill in courseSkills)
        {
            Skill? skill = await _db.Skills.SingleOrDefaultAsync(s => s.Id == courseSkill.SkillId);
            if (skill != null)
                skills.Add(skill);
        }
        courseViewModel.Skills = skills;
        return courseViewModel;
    }

    /// <inheritdoc/>
    public async Task Save(AddCourseViewModel model, Guid userId)
    {
        Education? education = await _db.Educations.FirstOrDefaultAsync(e => e.Name == model.Education);
        PlaceWork? placeWork = await _db.PlaceWorks.FirstOrDefaultAsync(p => p.Name == model.PlaceWork);
        Course course = new Course() { EducationId = education?.Id, PlaceWorkId = placeWork?.Id, Specialization = model.Specialization, CompletedCourses = model.CompletedCourses, NeedSkills = model.NeedSkills, UserId = userId };

        if (model.Skills != null)
        {
            string[] skills = model.Skills.Split('/', StringSplitOptions.RemoveEmptyEntries);
            foreach (string skill in skills)
            {
                Skill? trySkill = await _db.Skills.FirstOrDefaultAsync(s => s.Name == skill);
                if (trySkill != null)
                {
                    CourseSkill courseSkill = new CourseSkill() { CourseId = course.Id, SkillId = trySkill.Id };
                    await _db.CourseSkills.AddAsync(courseSkill);
                }
                else
                {
                    Skill newSkill = new Skill() { Name = skill };
                    await _db.Skills.AddAsync(newSkill);

                    CourseSkill courseSkill = new CourseSkill() { CourseId = course.Id, SkillId = newSkill.Id };
                    await _db.CourseSkills.AddAsync(courseSkill);
                }
            }
        }
        await _db.Courses.AddAsync(course);
        await _db.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task Update(AddCourseViewModel model, Guid userId)
    {
        Course? course = await GetCourseByUserId(userId);
        if (course is null)
            return;

        if (!String.IsNullOrEmpty(model.CompletedCourses))
        {
            course.CompletedCourses = model.CompletedCourses;
        }

        Education? education = await _db.Educations.FirstOrDefaultAsync(e => e.Name == model.Education);
        if (education != null)
            course.EducationId = education.Id;

        PlaceWork? placeWork = await _db.PlaceWorks.FirstOrDefaultAsync(p => p.Name == model.PlaceWork);
        if (placeWork != null)
            course.PlaceWorkId = placeWork.Id;

        if (!String.IsNullOrEmpty(model.Specialization))
        {
            course.Specialization = model.Specialization;
        }
        course.NeedSkills = model.NeedSkills;

        List<CourseSkill> courseSkills = await _db.CourseSkills.Where(c => c.CourseId == course.Id).ToListAsync();
        foreach (CourseSkill courseSkill in courseSkills)
            _db.CourseSkills.Remove(courseSkill);

        if (model.Skills != null)
        {
            string[] skills = model.Skills.Split('/', StringSplitOptions.RemoveEmptyEntries);
            foreach (string skill in skills)
            {
                Skill? trySkill = await _db.Skills.FirstOrDefaultAsync(s => s.Name == skill);
                if (trySkill != null)
                {
                    CourseSkill courseSkill = new CourseSkill() { CourseId = course.Id, SkillId = trySkill.Id };
                    await _db.CourseSkills.AddAsync(courseSkill);
                }
                else
                {
                    Skill newSkill = new Skill() { Name = skill };
                    await _db.Skills.AddAsync(newSkill);

                    CourseSkill courseSkill = new CourseSkill() { CourseId = course.Id, SkillId = newSkill.Id };
                    await _db.CourseSkills.AddAsync(courseSkill);
                }
            }
        }
        _db.Courses.Update(course);
        await _db.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task Delete(Guid id)
    {
        Course? course = await GetCourseById(id);
        if (course is null)
            return;

        List<CourseSkill> courseSkills = await _db.CourseSkills.Where(c => c.CourseId == course.Id).ToListAsync();
        foreach (CourseSkill courseSkill in courseSkills)
            _db.CourseSkills.Remove(courseSkill);

        _db.Courses.Remove(course);
        await _db.SaveChangesAsync();
    }
}

/// <summary>
/// Интерфейс для работы с курсами.
/// </summary>
public interface ICourseRepository
{
    /// <summary>
    /// Возвращает все курсы.
    /// </summary>
    Task<List<Course>> GetCourses();

    /// <summary>
    /// Возвращает все курсы.
    /// </summary>
    /// <param name="skillNames">Навыки</param>
    /// <param name="take">Количество возвращаемых курсов</param>
    /// <param name="skip">Количество пропускаемых курсов</param>
    Task<List<CourseViewModel>> GetCourseViewModels(string[]? skillNames = null, int take = 5, int skip = 0);

    /// <summary>
    /// Возвращает курс по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор курса</param>
    Task<Course?> GetCourseById(Guid id);

    /// <summary>
    /// Возвращает курс по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор курса</param>
    Task<CourseViewModel?> GetCourseViewModelById(Guid id);

    /// <summary>
    /// Возвращает курс пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    Task<Course?> GetCourseByUserId(Guid userId);

    /// <summary>
    /// Возвращает курс пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    Task<CourseViewModel?> GetCourseViewModelByUserId(Guid userId);

    /// <summary>
    /// Сохраняет курс.
    /// </summary>
    /// <param name="model">Курс</param>
    /// <param name="userId">Идентификатор пользователя</param>
    Task Save(AddCourseViewModel model, Guid userId);

    /// <summary>
    /// Обновляет курс.
    /// </summary>
    /// <param name="model">Новый курс</param>
    /// <param name="userId">Идентификатор пользователя</param>
    Task Update(AddCourseViewModel model, Guid userId);

    /// <summary>
    /// Удаляет курс.
    /// </summary>
    /// <param name="id">Идентификатор курса</param>
    Task Delete(Guid id);
}
