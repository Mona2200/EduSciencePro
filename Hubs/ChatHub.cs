using EduSciencePro.Models;
using EduSciencePro.Models.User;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace EduSciencePro.Hubs
{
   public class ChatHub : Hub
   {
      public async Task SendMessage(string user, string message)
      {

         await Clients.All.SendAsync("ReceiveMessage", message, user);
      }
      //public async Task SendMessage(string userEmail, string message)
      //{
      //   ClaimsIdentity ident = Context.User.Identity as ClaimsIdentity;
      //   var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;

      //   await Clients.Clients(claimEmail, userEmail).SendAsync("ReceiveMessage", message);
      //}

      //public async Task<string> WaitForMessage(string connectionId)
      //{
      //   var message = await Clients.Client(connectionId).InvokeAsync<string>(
      //       "GetMessage");
      //   return message;
      //}
   }
}
