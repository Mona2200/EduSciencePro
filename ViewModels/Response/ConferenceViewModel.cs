using EduSciencePro.Models;
using EduSciencePro.Models.User;

namespace EduSciencePro.ViewModels.Response
{
   public class ConferenceViewModel
   {
      public Guid Id { get; set; }
      public string Title { get; set; }
      public string EventDate { get; set; }
      public string ParticipationForm { get; set; }
      public Organization Organization { get; set; }
      public string Goals { get; set; }
      public string? Information { get; set; }
      public string? Program { get; set; }
      public string Contacts { get; set; }
      public Tag[] Tags { get; set; }
   }
}
