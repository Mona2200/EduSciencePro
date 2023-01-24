using AutoMapper;
using EduSciencePro.Data.Repos;
using EduSciencePro.Data.Services;
using EduSciencePro.Handler;
using EduSciencePro.Models;
using EduSciencePro.Models.User;
using EduSciencePro.ViewModels;
using EduSciencePro.ViewModels.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace EduSciencePro.Controllers
{
   public class MessageController : Controller
   {
      private readonly IMessageRepository _messages;
      private readonly IUserRepository _users;
      private readonly IMapper _mapper;



      private readonly IHubContext<ChatHandler> _hubContext;

      public MessageController(IMessageRepository messages, IUserRepository users, IMapper mapper, IHubContext<ChatHandler> hubContext)
      {
         _messages = messages;
         _users = users;
         _mapper = mapper;
         _hubContext = hubContext;
      }

      [HttpGet]
      [Route("Messages")]
      public async Task<IActionResult> Messages()
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _users.GetUserByEmail(claimEmail);

         DialogueViewModel[] messages = await _messages.GetDialogViewModelsByUserId(user.Id);
         return View(messages);
      }

      [HttpGet]
      [Route("OpenMessage")]
      public async Task<IActionResult> OpenMessage(Guid interlocutorId)
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _users.GetUserByEmail(claimEmail);

         OpenDialogueViewModel dialog = await _messages.GetOpenDialogViewModelByInterlocutordId(interlocutorId);
         if (dialog != null)
         {
            dialog.InterlocutorFirst = await _users.GetUserById(interlocutorId);
            dialog.InterlocutorSecond = user;
            dialog.isLooked = true;
            //await _messages.UpdateDialog(dialog);
            return View(dialog);
         }

         dialog = new OpenDialogueViewModel() { InterlocutorFirst = await _users.GetUserById(interlocutorId), InterlocutorSecond = user };
         return View(dialog);
      }

      //[HttpPost]
      //[Route("AddMessage")]
      //public async Task<MessageViewModel> AddMessage(Guid recipientId, string content)
      //{
      //   ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
      //   var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
      //   var user = await _users.GetUserByEmail(claimEmail);

      //   var dialog = await _messages.GetDialogByInterlocutordId(recipientId);
      //   if (dialog != null)
      //   {
      //      var message = new Message() { Content = content, CreateTime = DateTime.Now, RecipientId = recipientId, SenderId = user.Id, DialogId = dialog.Id };
      //      await _messages.Save(message, dialog);

      //      await _hubContext.Clients.All.SendAsync("ReceiveOne", user, content);
      //      return _mapper.Map<Message, MessageViewModel>(message);
      //   }
      //   else
      //   {
      //      var message = new Message() { Content = content, CreateTime = DateTime.Now, RecipientId = recipientId, SenderId = user.Id };
      //      dialog = new Dialog() { InterlocutorFirstId = recipientId, InterlocutorSecondId = user.Id, isLooked = false, LastMessageId = message.Id };
      //      message.DialogId = dialog.Id;
      //      await _messages.Save(message, dialog);

      //      await _hubContext.Clients.All.SendAsync("ReceiveOne", user, content);
      //      return _mapper.Map<Message, MessageViewModel>(message);
      //   }
      //}

      [HttpPost]
      [Route("AddMessage")]
      public async Task<MessageViewModel> AddMessage(Guid userId, string message)
      {
         var user = await _users.GetUserById(userId);
         EmailService emailService = new EmailService();
         await emailService.SendEmailAsync(user.Email, "Код подтверждения для сайта EduSciencePro", $"Ваш код подтверждения: {code}\nНе отвечайте на это письмо");
      }
   }
}
