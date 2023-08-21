using AutoMapper;
using EduSciencePro.Data.Repos;
using EduSciencePro.Data.Services;
using EduSciencePro.Hubs;
using EduSciencePro.Models;
using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using static MailKit.Net.Imap.ImapEvent;
using static ServiceStack.Diagnostics.Events;

namespace EduSciencePro.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class MessageController : Controller
    {
        private readonly IMessageRepository _messages;
        private readonly IUserRepository _users;
        private readonly IResumeRepository _resumes;
        private readonly IEducationRepository _educations;
        private readonly IPlaceWorkRepository _placeWorks;
        private readonly IOrganizationRepository _organizations;
        private readonly IMapper _mapper;

        private string _connectionId;

        private readonly IHubContext<ChatHub> _hubContext;

        private static Dictionary<Guid, List<string>> Clients = new();

        public MessageController(IMessageRepository messages, IUserRepository users, IResumeRepository resumes, IEducationRepository educations, IPlaceWorkRepository placeWorks, IOrganizationRepository organizations, IMapper mapper, IHubContext<ChatHub> hubContext)
        {
            _messages = messages;
            _users = users;
            _resumes = resumes;
            _educations = educations;
            _placeWorks = placeWorks;
            _organizations = organizations;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Messages()
        {
            ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
            var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
            var user = await _users.GetUserByEmail(claimEmail);

            DialogueViewModel[] messages = await _messages.GetDialogViewModelsByUserId(user.Id);
            return View(messages);
        }

        [HttpPost]
        [Route("Connect/{connectionId}")]
        public async void Connect([FromRoute] string connectionId)
        {
            ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
            var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
            var user = await _users.GetUserByEmail(claimEmail);

            var client = Clients.FirstOrDefault(c => c.Key == user.Id);
            if (client.Value == null)
            {
                Clients.Add(user.Id, new List<string> { connectionId });
            }
            else
            {
                client.Value.Add(connectionId);
            }

        }

        [HttpGet]
        [Route("OpenMessage")]
        public async Task<IActionResult> OpenMessage(Guid interlocutorId)
        {
            ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
            var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
            var user = await _users.GetUserByEmail(claimEmail);

            var dial = await _messages.GetDialogByInterlocutordId(interlocutorId);
            if (dial != null) 
            {
                await _messages.LookedDialog(user.Id, dial);
            }

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

        [HttpPost]
        [Route("MoreMessage/{interlocutorId}/{take}/{skip}")]
        public async Task<MessageViewModel[]> MoreMessage([FromRoute]Guid interlocutorId, [FromRoute] int take, [FromRoute] int skip)
        {
        return await _messages.GetMessagesMore(interlocutorId, take, skip);
        }

        [HttpPost]
        [Route("AddMessage")]
        public async Task AddMessage(AddMessageViewModel model)
        {
            if (!String.IsNullOrEmpty(model.content))
            {
                ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
                var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
                var user = await _users.GetUserByEmail(claimEmail);

                var dialog = await _messages.GetDialogByInterlocutordId(model.recipientId);
                if (dialog != null)
                {
                    var message = new Message() { Content = model.content, CreateTime = DateTime.Now, RecipientId = model.recipientId, SenderId = user.Id, DialogId = dialog.Id };
                    await _messages.Save(message, dialog);

                    var messageView = _mapper.Map<Message, MessageViewModel>(message);

                    var recipient = await _users.GetUserById(model.recipientId);

                    var recipientClient = Clients.FirstOrDefault(c => c.Key == recipient.Id);
                    if (recipientClient.Value != null)
                    {
                        foreach (var connect in recipientClient.Value)
                        {
                            await _hubContext.Clients.Client(connect).SendAsync("ReceiveMessage", messageView, user);
                        }

                    }

                    var userClient = Clients.FirstOrDefault(c => c.Key == user.Id);
                    if (userClient.Value != null)
                    {
                        foreach (var connect in userClient.Value)
                        {
                            await _hubContext.Clients.Client(connect).SendAsync("ReceiveMessage", messageView, user);
                        }
                    }
                }
                else
                {
                    var message = new Message() { Content = model.content, CreateTime = DateTime.Now, RecipientId = model.recipientId, SenderId = user.Id };
                    dialog = new Dialog() { InterlocutorFirstId = model.recipientId, InterlocutorSecondId = user.Id, isLooked = false, LastMessageId = message.Id };
                    message.DialogId = dialog.Id;
                    await _messages.Save(message, dialog);

                    var messageView = _mapper.Map<Message, MessageViewModel>(message);

                    var recipient = await _users.GetUserById(model.recipientId);

                    var recipientClient = Clients.FirstOrDefault(c => c.Key == recipient.Id);
                    if (recipientClient.Value != null)
                    {
                        foreach (var connect in recipientClient.Value)
                        {
                            await _hubContext.Clients.Client(connect).SendAsync("ReceiveMessage", messageView, user);
                        }

                    }

                    var userClient = Clients.FirstOrDefault(c => c.Key == user.Id);
                    if (userClient.Value != null)
                    {
                        foreach (var connect in userClient.Value)
                        {
                            await _hubContext.Clients.Client(connect).SendAsync("ReceiveMessage", messageView, user);
                        }
                    }
                }
            }
        }



        //[HttpPost]
        //[Route("Connect")]
        //public async Task Connect(string connectionId)
        //{
        //    _connectionId = connectionId;
        //    await _hubContext.Clients.Users.ConnectAsync(connectionId);
        //}

        //[HttpPost]
        //[Route("AddMessage")]
        //public async Task AddMessage(Guid organizationId, string theme)
        //{
        //   ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
        //   var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
        //   var user = await _users.GetUserByEmail(claimEmail);

        //   var organization = await _organizations.GetOrganizationById(organizationId);
        //   var liger = await _users.GetUserById(organization.LeaderId);

        //   var fullname = $"{user.FirstName} {user.LastName} {user.MiddleName}";

        //   var content = $"{fullname} предлагает вам сотрудничать.\nИнформация о пользователе:\nПол: {user.Gender},\nДата рождения: {user.Birthday},\nEmail: {user.Email}";

        //   if (user.ResumeId != null)
        //   {
        //      Resume resume = await _resumes.GetResumeById((Guid)user.ResumeId);
        //      if (resume.EducationId != null)
        //      {
        //         Education education = await _educations.GetEducationById((Guid)resume.EducationId);
        //         content += $"\nОбразование: {education.Name},";
        //      }
        //      if (resume.Specialization != null)
        //      content += $"\nСпециализация: {resume.Specialization},";
        //      if (resume.PlaceWorkId != null)
        //      {
        //         PlaceWork placeWork = await _placeWorks.GetPlaceWorkById((Guid)resume.PlaceWorkId);
        //         content += $"\nМесто работы: {placeWork.Name},";
        //      }
        //      if (resume.AboutYourself != null)
        //         content += $"\nИнформация о себе: {resume.AboutYourself},";

        //   }
        //   EmailService emailService = new EmailService();
        //   await emailService.SendEmailAsync(liger.Email, $"Заявка на тему '{theme}' EduSciencePro", content);
        //}
    }
}
