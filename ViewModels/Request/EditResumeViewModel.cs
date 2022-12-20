using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Response;

namespace EduSciencePro.ViewModels.Request
{
   public class EditResumeViewModel
   {
      public ResumeViewModel Resume {get;set;}
      public AddResumeViewModel AddResumeViewModel {get;set;}
      public Education[] AllEducations {get;set;}
      public PlaceWork[] AllPlaceWorks { get; set; }
      public Organization[] AllOrganizations { get; set; }
      public Skill[] AllSkills { get; set; }
      public bool Consent { get; set; }
   }
}
