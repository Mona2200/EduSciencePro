using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;
using Microsoft.EntityFrameworkCore;

namespace EduSciencePro.Data.Repos.UserRepos
{
   public class ResumeRepository : IResumeRepository
   {
      private readonly ApplicationDbContext _db;

      public ResumeRepository(ApplicationDbContext db)
      {
         _db = db;
      }

      public async Task<Resume[]> GetResumes()
      {
         return await _db.Resumes.ToArrayAsync();
      }

      public async Task<Resume> GetResumeById(Guid id)
      {
         return await _db.Resumes.FirstOrDefaultAsync(r => r.Id == id);
      }

      public async Task<Resume> GetResumeByUserId(Guid userId)
      {
         var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
         return await _db.Resumes.FirstOrDefaultAsync(r => r.Id == user.ResumeId);
      }

      //public async Task<ResumeViewModel[]> GetResumeViewModels()
      //{
      //var resumes = await _db.Resumes.ToArrayAsync();
      //var resumeViewModels = 
      //}

      //public async Task<ResumeViewModel> GetResumeViewModelById(Guid id)
      //{

      //}

      //public async Task<ResumeViewModel> GetResumeViewModelByUserUd(Guid userId)
      //{

      //}

      public async Task Save(AddResumeViewModel model)
      {
         var resume = new Resume();

         if (!String.IsNullOrEmpty(model.Education))
         {
            var tryEdu = await _db.Educations.FirstOrDefaultAsync(e => e.Name == model.Education);
            if (tryEdu != null)
               resume.Education = tryEdu;
            else
               resume.Education = new Education() { Name = model.Education };
         }

         if (!String.IsNullOrEmpty(model.DateGraduationEducation))
            resume.DateGraduationEducation = model.DateGraduationEducation;

         if (!String.IsNullOrEmpty(model.Specialization))
            resume.Specialization = model.Specialization;

         if (!String.IsNullOrEmpty(model.PlaceWork))
         {
            var tryplace = await _db.PlaceWorks.FirstOrDefaultAsync(p => p.Name == model.PlaceWork);
            if (tryplace != null)
               resume.PlaceWork = tryplace;
            else
               resume.PlaceWork = new PlaceWork() { Name = model.PlaceWork };
         }
         //if (!String.IsNullOrEmpty(model.Organization))
         //resume.Organization = new Organization() { Name = model.Organization };
         if (!String.IsNullOrEmpty(model.Skills))
         {
            var nameskills = model.Skills.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var name in nameskills)
            {
               var trySkill = await _db.Skills.FirstOrDefaultAsync(s => s.Name == name);
               if (trySkill != null)
               {
                  var resumeSkill = new ResumeSkill() { ResumeId = resume.Id, SkillId = trySkill.Id };
                  await _db.ResumeSkills.AddAsync(resumeSkill);
               }
               else
               {
                  var skill = new Skill() { Name = name };
                  await _db.Skills.AddAsync(skill);

                  var resumeSkill = new ResumeSkill() { ResumeId = resume.Id, SkillId = skill.Id };
                  await _db.ResumeSkills.AddAsync(resumeSkill);
               }
            }
         }
         if (!String.IsNullOrEmpty(model.AboutYourself))
            resume.AboutYourself = model.AboutYourself;

         var entry = _db.Entry(resume);
         if (entry.State == EntityState.Detached)
            await _db.Resumes.AddAsync(resume);
         await _db.SaveChangesAsync();
      }

      public async Task Update(Resume editResume, AddResumeViewModel newResume)
      {
         if (!String.IsNullOrEmpty(newResume.Education))
         {
            var tryEdu = await _db.Educations.FirstOrDefaultAsync(e => e.Name == newResume.Education);
            if (tryEdu != null)
               editResume.Education = tryEdu;
            else
               editResume.Education = new Education() { Name = newResume.Education };
         }

         if (!String.IsNullOrEmpty(newResume.DateGraduationEducation))
            editResume.DateGraduationEducation = newResume.DateGraduationEducation;

         if (!String.IsNullOrEmpty(newResume.Specialization))
            editResume.Specialization = newResume.Specialization;

         if (!String.IsNullOrEmpty(newResume.PlaceWork))
         {
            var tryplace = await _db.PlaceWorks.FirstOrDefaultAsync(p => p.Name == newResume.PlaceWork);
            if (tryplace != null)
               editResume.PlaceWork = tryplace;
            else
               editResume.PlaceWork = new PlaceWork() { Name = newResume.PlaceWork };
         }
         //if (!String.IsNullOrEmpty(newResume.Organization))
         //editResume.Organization = new Organization() { Name = newResume.Organization };
         if (!String.IsNullOrEmpty(newResume.Skills))
         {
            var nameskills = newResume.Skills.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var name in nameskills)
            {
               var trySkill = await _db.Skills.FirstOrDefaultAsync(s => s.Name == name);
               if (trySkill != null)
               {
                  var resumeSkill = new ResumeSkill() { ResumeId = editResume.Id, SkillId = trySkill.Id };
                  await _db.ResumeSkills.AddAsync(resumeSkill);
               }
               else
               {
                  var skill = new Skill() { Name = name };
                  await _db.Skills.AddAsync(skill);

                  var resumeSkill = new ResumeSkill() { ResumeId = editResume.Id, SkillId = skill.Id };
                  await _db.ResumeSkills.AddAsync(resumeSkill);
               }
            }
         }
         if (!String.IsNullOrEmpty(newResume.AboutYourself))
            editResume.AboutYourself = newResume.AboutYourself;

         var entry = _db.Entry(editResume);
         if (entry.State == EntityState.Detached)
            _db.Resumes.Update(editResume);

         await _db.SaveChangesAsync();
      }

      public async Task Delete(Resume resume)
      {

      }

      public async Task<Skill[]> GetSkills()
      {
         return await _db.Skills.ToArrayAsync();
      }
   }
}
