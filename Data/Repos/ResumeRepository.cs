using AutoMapper;
using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;
using Microsoft.EntityFrameworkCore;

namespace EduSciencePro.Data.Repos;

public class ResumeRepository : IResumeRepository
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public ResumeRepository(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<Resume>> GetResumes()
    {
        return await _db.Resumes.ToListAsync();
    }

    public async Task<Resume?> GetResumeById(Guid id)
    {
        return await _db.Resumes.SingleOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Resume?> GetResumeByUserId(Guid userId)
    {
        User? user = await _db.Users.SingleOrDefaultAsync(u => u.Id == userId);
        if (user is null || user.ResumeId is null)
            return null;

        return await GetResumeById(user.ResumeId.Value);
    }

    public async Task<List<ResumeViewModel>> GetResumeViewModels()
    {
        List<Resume> resumes = await GetResumes();
        List<ResumeViewModel> resumeViewModels = new();
        foreach (Resume resume in resumes)
        {
            User? user = await _db.Users.FirstOrDefaultAsync(u => u.ResumeId == resume.Id);
            if (user is null)
                continue;

            ResumeViewModel resumeViewModel = _mapper.Map<Resume, ResumeViewModel>(resume);
            resumeViewModel.PlaceWork = await _db.PlaceWorks.SingleOrDefaultAsync(p => p.Id == resume.PlaceWorkId);
            resumeViewModel.Education = await _db.Educations.SingleOrDefaultAsync(e => e.Id == resume.EducationId);

            UserOrganization? userOrg = await _db.UserOrganizations.FirstOrDefaultAsync(u => u.IdUser == user.Id);
            if (userOrg != null)
                resumeViewModel.Organization = await _db.Organizations.SingleOrDefaultAsync(o => o.Id == userOrg.IdOrganization);

            List<ResumeSkill> resumeSkills = await _db.ResumeSkills.Where(rs => rs.ResumeId == resume.Id).ToListAsync();
            List<Skill> skills = new();
            foreach (ResumeSkill resumeSkill in resumeSkills)
            {
                Skill? skill = await _db.Skills.SingleOrDefaultAsync(s => s.Id == resumeSkill.SkillId);
                if (skill != null)
                    skills.Add(skill);
            }

            resumeViewModel.Skills = skills;
            resumeViewModels.Add(resumeViewModel);
        }
        return resumeViewModels;
    }

    public async Task<ResumeViewModel?> GetResumeViewModelById(Guid id)
    {
        User? user = await _db.Users.FirstOrDefaultAsync(u => u.ResumeId == id);
        if (user is null)
            return null;

        Resume? resume = await GetResumeById(id);
        if (resume is null)
            return null;

        ResumeViewModel resumeViewModel = _mapper.Map<Resume, ResumeViewModel>(resume);
        resumeViewModel.PlaceWork = await _db.PlaceWorks.SingleOrDefaultAsync(p => p.Id == resume.PlaceWorkId);
        resumeViewModel.Education = await _db.Educations.SingleOrDefaultAsync(e => e.Id == resume.EducationId);

        UserOrganization? userOrg = await _db.UserOrganizations.FirstOrDefaultAsync(u => u.IdUser == user.Id);
        if (userOrg != null)
            resumeViewModel.Organization = await _db.Organizations.SingleOrDefaultAsync(o => o.Id == userOrg.IdOrganization);

        List<ResumeSkill> resumeSkills = await _db.ResumeSkills.Where(rs => rs.ResumeId == resume.Id).ToListAsync();
        List<Skill> skills = new();
        foreach (ResumeSkill resumeSkill in resumeSkills)
        {
            Skill? skill = await _db.Skills.SingleOrDefaultAsync(s => s.Id == resumeSkill.SkillId);
            if (skill != null)
                skills.Add(skill);
        }
        resumeViewModel.Skills = skills;
        return resumeViewModel;
    }

    public async Task<ResumeViewModel?> GetResumeViewModelByUserId(Guid userId)
    {
        User? user = await _db.Users.SingleOrDefaultAsync(u => u.Id == userId);
        if (user is null)
            return null;

        var resume = await GetResumeByUserId(userId);
        if (resume is null)
            return null;

        ResumeViewModel resumeViewModel = _mapper.Map<Resume, ResumeViewModel>(resume);
        resumeViewModel.PlaceWork = await _db.PlaceWorks.SingleOrDefaultAsync(p => p.Id == resume.PlaceWorkId);
        resumeViewModel.Education = await _db.Educations.SingleOrDefaultAsync(e => e.Id == resume.EducationId);

        UserOrganization? userOrg = await _db.UserOrganizations.FirstOrDefaultAsync(u => u.IdUser == user.Id);
        if (userOrg != null)
            resumeViewModel.Organization = await _db.Organizations.SingleOrDefaultAsync(o => o.Id == userOrg.IdOrganization);

        List<ResumeSkill> resumeSkills = await _db.ResumeSkills.Where(rs => rs.ResumeId == resume.Id).ToListAsync();
        List<Skill> skills = new();
        foreach (ResumeSkill resumeSkill in resumeSkills)
        {
            Skill? skill = await _db.Skills.SingleOrDefaultAsync(s => s.Id == resumeSkill.SkillId);
            if (skill != null)
                skills.Add(skill);
        }
        resumeViewModel.Skills = skills;
        return resumeViewModel;
    }

    public async Task Save(User user, AddResumeViewModel model)
    {
        Resume resume = new();
        user.ResumeId = resume.Id;

        if (!String.IsNullOrEmpty(model.Education))
        {
            Education? tryEdu = await _db.Educations.FirstOrDefaultAsync(e => e.Name == model.Education);
            if (tryEdu != null)
                resume.EducationId = tryEdu.Id;
            else
            {
                Education education = new Education() { Name = model.Education };
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
            PlaceWork? tryplace = await _db.PlaceWorks.FirstOrDefaultAsync(p => p.Name == model.PlaceWork);
            if (tryplace != null)
                resume.PlaceWorkId = tryplace.Id;
            else
            {
                PlaceWork placeWork = new PlaceWork() { Name = model.PlaceWork };
                await _db.PlaceWorks.AddAsync(placeWork);
                resume.PlaceWorkId = placeWork.Id;
            }

        }
        if (!String.IsNullOrEmpty(model.Organization))
        {
            Organization? tryOrganization = await _db.Organizations.FirstOrDefaultAsync(o => o.Name == model.Organization);
            if (tryOrganization != null)
            {
                UserOrganization userOrganization = new UserOrganization() { IdOrganization = tryOrganization.Id, IdUser = user.Id };
                await _db.UserOrganizations.AddAsync(userOrganization);
            }
        }
        if (!String.IsNullOrEmpty(model.Skills))
        {
            string[] nameskills = model.Skills.Split('/', StringSplitOptions.RemoveEmptyEntries);
            foreach (var name in nameskills)
            {
                Skill? trySkill = await _db.Skills.FirstOrDefaultAsync(s => s.Name == name);
                if (trySkill != null)
                {
                    ResumeSkill resumeSkill = new ResumeSkill() { ResumeId = resume.Id, SkillId = trySkill.Id };
                    await _db.ResumeSkills.AddAsync(resumeSkill);
                }
                else
                {
                    Skill skill = new Skill() { Name = name };
                    await _db.Skills.AddAsync(skill);

                    ResumeSkill resumeSkill = new ResumeSkill() { ResumeId = resume.Id, SkillId = skill.Id };
                    await _db.ResumeSkills.AddAsync(resumeSkill);
                }
            }
        }
        if (!String.IsNullOrEmpty(model.AboutYourself))
            resume.AboutYourself = model.AboutYourself;

        await _db.Resumes.AddAsync(resume);
        _db.Users.Update(user);

        await _db.SaveChangesAsync();
    }

    public async Task Update(User user, Resume editResume, AddResumeViewModel newResume)
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
            editResume.EducationId = null;
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
            editResume.PlaceWorkId = null;
        }
        if (!String.IsNullOrEmpty(newResume.Organization))
        {
            var tryOrganization = await _db.Organizations.FirstOrDefaultAsync(o => o.Name == newResume.Organization);
            if (tryOrganization != null)
            {
                var userOrganization = new UserOrganization() { IdOrganization = tryOrganization.Id, IdUser = user.Id };
                await _db.UserOrganizations.AddAsync(userOrganization);
            }
        }
        var oldResumeSkills = await _db.ResumeSkills.Where(rs => rs.ResumeId == editResume.Id).ToArrayAsync();
        foreach (var rs in oldResumeSkills)
        {
            _db.ResumeSkills.Remove(rs);
        }
        if (!String.IsNullOrEmpty(newResume.Skills))
        {
            var nameskills = newResume.Skills.Split('/', StringSplitOptions.RemoveEmptyEntries);
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

    public async Task<List<Skill>> GetSkills()
    {
        return await _db.Skills.ToListAsync();
    }
}

public interface IResumeRepository
{
    Task<List<Resume>> GetResumes();
    Task<Resume?> GetResumeById(Guid id);
    Task<Resume?> GetResumeByUserId(Guid userId);
    Task<List<ResumeViewModel>> GetResumeViewModels();
    Task<ResumeViewModel?> GetResumeViewModelById(Guid id);
    Task<ResumeViewModel?> GetResumeViewModelByUserId(Guid userId);

    Task Save(User user, AddResumeViewModel resume);
    Task Update(User user, Resume editResume, AddResumeViewModel newResume);
    Task Delete(Resume resume);

    Task<List<Skill>> GetSkills();
}
