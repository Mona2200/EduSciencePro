using EduSciencePro.Models;

namespace EduSciencePro.ViewModels.Response
{
    public class DialogueViewModel
    {
      public Guid Id { get; set; }
      public bool isLooked { get; set; }
      public MessageViewModel LastMessage { get; set; }
   }
}
