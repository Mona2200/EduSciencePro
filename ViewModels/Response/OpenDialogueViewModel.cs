using EduSciencePro.Models.User;

namespace EduSciencePro.ViewModels.Response
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
