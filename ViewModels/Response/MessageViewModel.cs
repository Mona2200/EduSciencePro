using EduSciencePro.Models.User;

namespace EduSciencePro.ViewModels.Response
{
    public class MessageViewModel
    {
      public Guid Id { get; set; }
      public DateTime CreateTime { get; set; }
      public string Content { get; set; }
      public bool isLooked { get; set; }
      public Guid DialogId { get; set; }
      public User Sender { get; set; }
      public User Recipient { get; set; }
   }
}
