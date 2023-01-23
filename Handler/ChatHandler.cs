using EduSciencePro.Models;
using Microsoft.AspNetCore.SignalR;

namespace EduSciencePro.Handler
{
   public class ChatHandler : Hub
   {
      public Task SendMessage1(Guid recipientId, string content)               // Two parameters accepted
      {
         return Clients.All.SendAsync("ReceiveOne", recipientId, content);    // Note this 'ReceiveOne' 
      }

   }
}
