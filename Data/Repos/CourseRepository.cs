using AutoMapper;
using EduSciencePro.Models;
using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;
using Microsoft.EntityFrameworkCore;

namespace EduSciencePro.Data.Repos
{
   public class CourseRepository : ICourseRepository
   {
      private readonly ApplicationDbContext _db;
      private readonly IMapper _mapper;
      public CourseRepository(ApplicationDbContext db, IMapper mapper)
      {
         _db = db;
         _mapper = mapper;
      }

      public async Task<Course[]> GetCourses() => await _db.Courses.ToArrayAsync();

      public async Task<CourseViewModel[]> GetCourseViewModels(string[] tagNames = null, int take = 5, int skip = 0)
      {
            List<Course> courses = new();
            if (tagNames != null && tagNames.Length != 0)
            {
                foreach (var tagName in tagNames)
                {
                    var tag = await _db.Skills.FirstOrDefaultAsync(t => t.Name == tagName);
                    if (tag != null)
                    {
                        var tagPosts = await _db.CourseSkills.Where(t => t.SkillId == tag.Id).ToListAsync();
                        foreach (var tagPost in tagPosts)
                        {
                            var tagNew = await _db.Courses.FirstOrDefaultAsync(p => p.Id == tagPost.CourseId);
                            if (tagNew != null && courses.FirstOrDefault(n => n.Id == tagNew.Id) == null)
                                courses.Add(tagNew);
                        }
                    }
                }
            }
            else
                courses = await _db.Courses.ToListAsync();
            courses = courses.Take(take).Skip(skip).ToList();

         var courseViewModels = new List<CourseViewModel>();
         foreach (var course in courses)
         {
            var courseViewModel = _mapper.Map<Course, CourseViewModel>(course);
            courseViewModel.Education = await _db.Educations.FirstOrDefaultAsync(e => e.Id == course.EducationId);
            courseViewModel.PlaceWork = await _db.PlaceWorks.FirstOrDefaultAsync(p => p.Id == course.PlaceWorkId);
            courseViewModel.User = await _db.Users.FirstOrDefaultAsync(u => u.Id == course.UserId);
            var courseSkills = await _db.CourseSkills.Where(s => s.CourseId == course.Id).ToListAsync();
            var skills = new List<Skill>();
            foreach (var courseSkill in courseSkills)
            {
               var skill = await _db.Skills.FirstOrDefaultAsync(s => s.Id == courseSkill.SkillId);
               skills.Add(new Skill() { Name = skill.Name });
            }
            courseViewModel.Skills = skills.ToArray();
            courseViewModels.Add(courseViewModel);
         }
         return courseViewModels.ToArray();
      }

      public async Task<Course> GetCourseById(Guid id) => await _db.Courses.FirstOrDefaultAsync(c => c.Id == id);

      public async Task<CourseViewModel> GetCourseViewModelById(Guid id)
      {
         var course = await GetCourseById(id);
         var courseViewModel = _mapper.Map<Course, CourseViewModel>(course);
         courseViewModel.Education = await _db.Educations.FirstOrDefaultAsync(e => e.Id == course.EducationId);
         courseViewModel.PlaceWork = await _db.PlaceWorks.FirstOrDefaultAsync(p => p.Id == course.PlaceWorkId);
         courseViewModel.User = await _db.Users.FirstOrDefaultAsync(u => u.Id == course.UserId);
         var courseSkills = await _db.CourseSkills.Where(s => s.CourseId == course.Id).ToListAsync();
         var skills = new List<Skill>();
         foreach (var courseSkill in courseSkills)
         {
            var skill = await _db.Skills.FirstOrDefaultAsync(s => s.Id == courseSkill.SkillId);
            skills.Add(new Skill() { Name = skill.Name });
         }
         courseViewModel.Skills = skills.ToArray();
         return courseViewModel;
      }

      public async Task<Course> GetCourseByUserId(Guid userId) => await _db.Courses.FirstOrDefaultAsync(u => u.UserId == userId);

      public async Task<CourseViewModel> GetCourseViewModelByUserId(Guid userId)
      {
         var course = await GetCourseByUserId(userId);
         if (course == null) return null;
         var courseViewModel = _mapper.Map<Course, CourseViewModel>(course);
         courseViewModel.Education = await _db.Educations.FirstOrDefaultAsync(e => e.Id == course.EducationId);
         courseViewModel.PlaceWork = await _db.PlaceWorks.FirstOrDefaultAsync(p => p.Id == course.PlaceWorkId);
         courseViewModel.User = await _db.Users.FirstOrDefaultAsync(u => u.Id == course.UserId);
         var courseSkills = await _db.CourseSkills.Where(s => s.CourseId == course.Id).ToListAsync();
         var skills = new List<Skill>();
         foreach (var courseSkill in courseSkills)
         {
            var skill = await _db.Skills.FirstOrDefaultAsync(s => s.Id == courseSkill.SkillId);
            skills.Add(new Skill() { Name = skill.Name });
         }
         courseViewModel.Skills = skills.ToArray();
         return courseViewModel;
      }

      public async Task Save(AddCourseViewModel model, Guid userId)
      {
         var education = await _db.Educations.FirstOrDefaultAsync(e => e.Name == model.Education);
         var placeWork = await _db.PlaceWorks.FirstOrDefaultAsync(p => p.Name == model.PlaceWork);
         var course = new Course() { EducationId = education.Id, PlaceWorkId = placeWork.Id, Specialization = model.Specialization, CompletedCourses = model.CompletedCourses, NeedSkills = model.NeedSkills, UserId = userId };

         if (model.Skills != null)
         {
            var skills = model.Skills.Split('/', StringSplitOptions.RemoveEmptyEntries);
            foreach (var skill in skills)
            {
               var trySkill = await _db.Skills.FirstOrDefaultAsync(s => s.Name == skill);
               if (trySkill != null)
               {
                  var courseSkill = new CourseSkill() { CourseId = course.Id, SkillId = trySkill.Id };
                  var skillentry = _db.Entry(courseSkill);
                  if (skillentry.State == EntityState.Detached)
                     await _db.CourseSkills.AddAsync(courseSkill);
               }
               else
               {
                  var newSkill = new Skill() { Name = skill };
                  var newskillentry = _db.Entry(newSkill);
                  if (newskillentry.State == EntityState.Detached)
                     await _db.Skills.AddAsync(newSkill);

                  var courseSkill = new CourseSkill() { CourseId = course.Id, SkillId = newSkill.Id };
                  var skillentry = _db.Entry(courseSkill);
                  if (skillentry.State == EntityState.Detached)
                     await _db.CourseSkills.AddAsync(courseSkill);
               }
            }
         }
         

         var entry = _db.Entry(course);
         if (entry.State == EntityState.Detached)
            await _db.Courses.AddAsync(course);

         await _db.SaveChangesAsync();
      }

      public async Task Update(AddCourseViewModel model, Guid userId)
      {
         var course = await GetCourseByUserId(userId);
         if (!String.IsNullOrEmpty(model.CompletedCourses))
         {
            course.CompletedCourses = model.CompletedCourses;
         }

         var education = await _db.Educations.FirstOrDefaultAsync(e => e.Name == model.Education);
         course.EducationId = education.Id;

         var placeWork = await _db.PlaceWorks.FirstOrDefaultAsync(p => p.Name == model.PlaceWork);
         course.PlaceWorkId = placeWork.Id;

         if (!String.IsNullOrEmpty(model.Specialization))
         {
            course.Specialization = model.Specialization;
         }
         course.NeedSkills = model.NeedSkills;

         var courseSkills = await _db.CourseSkills.Where(c => c.CourseId == course.Id).ToListAsync();
         foreach (var courseSkill in courseSkills)
         {
         _db.CourseSkills.Remove(courseSkill);
         }
         if (model.Skills != null)
         {
            var skills = model.Skills.Split('/', StringSplitOptions.RemoveEmptyEntries);
            foreach (var skill in skills)
            {
               var trySkill = await _db.Skills.FirstOrDefaultAsync(s => s.Name == skill);
               if (trySkill != null)
               {
                  var courseSkill = new CourseSkill() { CourseId = course.Id, SkillId = trySkill.Id };
                  var skillentry = _db.Entry(courseSkill);
                  if (skillentry.State == EntityState.Detached)
                     await _db.CourseSkills.AddAsync(courseSkill);
               }
               else
               {
                  var newSkill = new Skill() { Name = skill };
                  var newskillentry = _db.Entry(newSkill);
                  if (newskillentry.State == EntityState.Detached)
                     await _db.Skills.AddAsync(newSkill);

                  var courseSkill = new CourseSkill() { CourseId = course.Id, SkillId = newSkill.Id };
                  var skillentry = _db.Entry(courseSkill);
                  if (skillentry.State == EntityState.Detached)
                     await _db.CourseSkills.AddAsync(courseSkill);
               }
            }
         }

         var entry = _db.Entry(course);
         if (entry.State == EntityState.Detached)
            _db.Courses.Update(course);

         await _db.SaveChangesAsync();
      }

      public async Task Delete(Guid id)
      {
         var course = await GetCourseById(id);

         if (course != null)
         {
            var courseSkills = await _db.CourseSkills.Where(c => c.CourseId == course.Id).ToListAsync();
            foreach (var courseSkill in courseSkills)
            {
               _db.CourseSkills.Remove(courseSkill);
            }

            _db.Courses.Remove(course);
            await _db.SaveChangesAsync();
         }
      }
   }

   public interface ICourseRepository
   {
      Task<Course[]> GetCourses();
      Task<CourseViewModel[]> GetCourseViewModels(string[] tagNames = null, int take = 5, int skip = 0);
      Task<Course> GetCourseById(Guid id);
      Task<CourseViewModel> GetCourseViewModelById(Guid id);
      Task<Course> GetCourseByUserId(Guid userId);
      Task<CourseViewModel> GetCourseViewModelByUserId(Guid userId);
      Task Save(AddCourseViewModel model, Guid userId);
      Task Update(AddCourseViewModel model, Guid userId);
      Task Delete(Guid id);
   }
}
