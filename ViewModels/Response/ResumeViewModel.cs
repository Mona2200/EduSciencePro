using EduSciencePro.Models.User;

namespace EduSciencePro.ViewModels.Response
{
   public class ResumeViewModel
   {
      public Guid Id { get; set; }
      public Education Education { get; set; }
      public string DateGraduationEducation { get; set; }
      public string Specialization { get; set; }
      public PlaceWork PlaceWork { get; set; }
      public Organization Organization { get; set; }
      public Skill[] Skills { get; set; }
      public string AboutYourself { get; set; }
   }
}
