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

      public async Task Save(AddResumeViewModel resume)
      {

      }

      public async Task Update(Resume editResume, AddResumeViewModel newResume)
      {

      }

      public async Task Delete(Resume resume)
      {

      }
   }
}
