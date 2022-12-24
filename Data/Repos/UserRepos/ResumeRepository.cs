using AutoMapper;
using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;
using Microsoft.EntityFrameworkCore;

namespace EduSciencePro.Data.Repos.UserRepos
{
   public class ResumeRepository : IResumeRepository
   {
      private readonly ApplicationDbContext _db;
      private readonly IMapper _mapper;

      public ResumeRepository(ApplicationDbContext db, IMapper mapper)
      {
         _db = db;
         _mapper = mapper;
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

      public async Task<ResumeViewModel[]> GetResumeViewModels()
      {
         var resumes = await GetResumes();
         var models = new ResumeViewModel[resumes.Length];
         int i = 0;
         foreach (var res in resumes)
         {
            models[i] = _mapper.Map<Resume, ResumeViewModel>(res);
            models[i].PlaceWork = await _db.PlaceWorks.FirstOrDefaultAsync(p => p.Id == res.PlaceWorkId);
            models[i].Education = await _db.Educations.FirstOrDefaultAsync(e => e.Id == res.EducationId);
            models[i].Organization = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == res.OrganizationId);

            var resumeSkills = await _db.ResumeSkills.Where(rs => rs.ResumeId == res.Id).ToArrayAsync();
            var skills = new Skill[resumeSkills.Length];
            int j = 0;
            foreach (var resumeSkill in resumeSkills)
            {
               skills[j++] = await _db.Skills.FirstOrDefaultAsync(s => s.Id == resumeSkill.SkillId);
            }

            models[i++].Skills = skills;
         }
         return models;
      }

      public async Task<ResumeViewModel> GetResumeViewModelById(Guid id)
      {
         var resume = await GetResumeById(id);
         var model = _mapper.Map<Resume, ResumeViewModel>(resume);
         model.PlaceWork = await _db.PlaceWorks.FirstOrDefaultAsync(p => p.Id == resume.PlaceWorkId);
         model.Education = await _db.Educations.FirstOrDefaultAsync(e => e.Id == resume.EducationId);
         model.Organization = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == resume.OrganizationId);

         var resumeSkills = await _db.ResumeSkills.Where(rs => rs.ResumeId == resume.Id).ToArrayAsync();
         var skills = new Skill[resumeSkills.Length];
         int i = 0;
         foreach (var resumeSkill in resumeSkills)
         {
            skills[i++] = await _db.Skills.FirstOrDefaultAsync(s => s.Id == resumeSkill.SkillId);
         }

         model.Skills = skills;
         return model;
      }

      public async Task<ResumeViewModel> GetResumeViewModelByUserId(Guid userId)
      {
         var resume = await GetResumeByUserId(userId);
         var model = _mapper.Map<Resume, ResumeViewModel>(resume);
         model.PlaceWork = await _db.PlaceWorks.FirstOrDefaultAsync(p => p.Id == resume.PlaceWorkId);
         model.Education = await _db.Educations.FirstOrDefaultAsync(e => e.Id == resume.EducationId);
         model.Organization = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == resume.OrganizationId);

         var resumeSkills = await _db.ResumeSkills.Where(rs => rs.ResumeId == resume.Id).ToArrayAsync();
         var skills = new Skill[resumeSkills.Length];
         int i = 0;
         foreach (var resumeSkill in resumeSkills)
         {
            skills[i++] = await _db.Skills.FirstOrDefaultAsync(s => s.Id == resumeSkill.SkillId);
         }

         model.Skills = skills;
         return model;
      }

      public async Task Save(User user, AddResumeViewModel model)
      {
         var resume = new Resume();

         user.ResumeId = resume.Id;

         if (!String.IsNullOrEmpty(model.Education))
         {
            var tryEdu = await _db.Educations.FirstOrDefaultAsync(e => e.Name == model.Education);
            if (tryEdu != null)
               resume.EducationId = tryEdu.Id;
            else
            {
               var education = new Education() { Name = model.Education };
               await _db.Educations.AddAsync(education);
               resume.EducationId = education.Id;
            }

         }

         if (!String.IsNullOrEmpty(model.DateGraduationEducation))
            resume.DateGraduationEducation = model.DateGraduationEducation;

         if (!String.IsNullOrEmpty(model.Specialization))
            resume.Specialization = model.Specialization;

         if (!String.IsNullOrEmpty(model.PlaceWork))
         {
            var tryplace = await _db.PlaceWorks.FirstOrDefaultAsync(p => p.Name == model.PlaceWork);
            if (tryplace != null)
               resume.PlaceWorkId = tryplace.Id;
            else
            {
               var placeWork = new PlaceWork() { Name = model.PlaceWork };
               await _db.AddAsync(placeWork);
               resume.PlaceWorkId = placeWork.Id;
            }

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

         var uentry = _db.Entry(user);
         if (uentry.State == EntityState.Detached)
            _db.Users.Update(user);

         await _db.SaveChangesAsync();
      }

      public async Task Update(Resume editResume, AddResumeViewModel newResume)
      {
         if (!String.IsNullOrEmpty(newResume.Education))
         {
            var tryEdu = await _db.Educations.FirstOrDefaultAsync(e => e.Name == newResume.Education);
            if (tryEdu != null)
               editResume.EducationId = tryEdu.Id;
            else
            {
               var education = new Education() { Name = newResume.Education };
               await _db.Educations.AddAsync(education);
               editResume.EducationId = education.Id;
            }
         }
         else
         {
            var tryEdu = await _db.Educations.FirstOrDefaultAsync(e => e.Name == newResume.Education);
            if (tryEdu != null)
               _db.Educations.Remove(tryEdu);
         }

         if (!String.IsNullOrEmpty(newResume.DateGraduationEducation))
            editResume.DateGraduationEducation = newResume.DateGraduationEducation;
         else
            editResume.DateGraduationEducation = null;

         if (!String.IsNullOrEmpty(newResume.Specialization))
            editResume.Specialization = newResume.Specialization;
         else
            editResume.Specialization = null;

         if (!String.IsNullOrEmpty(newResume.PlaceWork))
         {
            var tryplace = await _db.PlaceWorks.FirstOrDefaultAsync(p => p.Name == newResume.PlaceWork);
            if (tryplace != null)
               editResume.PlaceWorkId = tryplace.Id;
            else
            {
               var placeWork = new PlaceWork() { Name = newResume.PlaceWork };
               await _db.AddAsync(placeWork);
               editResume.PlaceWorkId = placeWork.Id;
            }

         }
         else
         {
            var tryplace = await _db.PlaceWorks.FirstOrDefaultAsync(p => p.Name == newResume.PlaceWork);
            if (tryplace != null)
               _db.PlaceWorks.Remove(tryplace);
         }
         //if (!String.IsNullOrEmpty(newResume.Organization))
         //editResume.Organization = new Organization() { Name = newResume.Organization };
         var oldResumeSkills = await _db.ResumeSkills.Where(rs => rs.ResumeId == editResume.Id).ToArrayAsync();
         foreach (var rs in oldResumeSkills)
         {
            _db.ResumeSkills.Remove(rs);
         }
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
         else
            editResume.AboutYourself = null;

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
