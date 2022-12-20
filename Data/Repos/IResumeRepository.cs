using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;

namespace EduSciencePro.Data.Repos
{
   public interface IResumeRepository
   {
      Task<Resume[]> GetResumes();
      Task<Resume> GetResumeById(Guid id);
      Task<Resume> GetResumeByUserId(Guid userId);
      //Task<ResumeViewModel[]> GetResumeViewModels();
      //Task<ResumeViewModel> GetResumeViewModelById(Guid id);
      //Task<ResumeViewModel> GetResumeViewModelByUserUd(Guid userId);
      Task Save(AddResumeViewModel resume);
      Task Update(Resume editResume, AddResumeViewModel newResume);
      Task Delete(Resume resume);

      Task<Skill[]> GetSkills();
   }
}
