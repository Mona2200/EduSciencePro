using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Response;

namespace EduSciencePro.ViewModels
{
   public class OpenDialogueViewModel
   {
      public Guid Id { get; set; }
      public bool isLooked { get; set; }
      public MessageViewModel[] Messages { get; set; }
      public User InterlocutorFirst { get; set; }
      public User InterlocutorSecond { get; set; }
   }
}
