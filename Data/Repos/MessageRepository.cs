using AutoMapper;
using EduSciencePro.Models;
using EduSciencePro.ViewModels.Response;
using Microsoft.EntityFrameworkCore;

namespace EduSciencePro.Data.Repos
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        private readonly IUserRepository _users;

        public MessageRepository(ApplicationDbContext db, IMapper mapper, IUserRepository users)
        {
            _db = db;
            _mapper = mapper;
            _users = users;
        }

        public async Task<Message[]> GetMessages() => await _db.Messages.ToArrayAsync();
        public async Task<Dialog[]> GetDialogs() => await _db.Dialogs.ToArrayAsync();

        //public async Task<MessageViewModel[]> GetMessageViewModels()
        //{
        //   Message[] messages = await GetMessages();
        //   List<MessageViewModel> messagesViewModels = new List<MessageViewModel>();
        //   foreach (var message in messages)
        //   {
        //      MessageViewModel messageViewModel = _mapper.Map<Message, MessageViewModel>(message);
        //      messageViewModel.Sender = await _users.GetUserById(message.SenderId);
        //      messageViewModel.Recipient = await _users.GetUserById(message.RecipientId);
        //      messagesViewModels.Add(messageViewModel);
        //   }
        //   return messagesViewModels.ToArray();
        //}

        //public async Task<Message[]> GetMessagesBySenderId(Guid senderId) => await _db.Messages.Where(m => m.SenderId == senderId).ToArrayAsync();

        public async Task<DialogueViewModel[]> GetDialogViewModelsByUserId(Guid userId)
        {
            var dialogs = await _db.Dialogs.Where(d => d.InterlocutorFirstId == userId || d.InterlocutorSecondId == userId).ToListAsync();
            List<DialogueViewModel> messages = new List<DialogueViewModel>();
            foreach (var dialog in dialogs)
            {
                Message? mess = await _db.Messages.Where(m => m.DialogId == dialog.Id && (m.RecipientId == userId || m.SenderId == userId)).OrderBy(m => m.CreateTime).LastOrDefaultAsync();
                if (mess != null)
                {
                    var dial = _mapper.Map<Dialog, DialogueViewModel>(dialog);
                    var messModel = _mapper.Map<Message, MessageViewModel>(mess);
                    messModel.Sender = await _db.Users.FirstOrDefaultAsync(u => u.Id == mess.SenderId);
                    messModel.Recipient = await _db.Users.FirstOrDefaultAsync(u => u.Id == mess.RecipientId);
                    dial.LastMessage = messModel;
                    messages.Add(dial);
                }
            }
            return messages.ToArray();
        }

        public async Task<OpenDialogueViewModel> GetOpenDialogViewModelById(Guid dialogId)
        {
            var dial = await _db.Dialogs.FirstOrDefaultAsync(d => d.Id == dialogId);
            var messages = await _db.Messages.Where(m => m.DialogId == dialogId).ToListAsync();
            OpenDialogueViewModel dialog = new OpenDialogueViewModel();
            dialog.Id = dialogId;
            dialog.isLooked = dial.isLooked;

            List<MessageViewModel> messageModels = new List<MessageViewModel>();

            foreach (var message in messages)
            {
                var mess = _mapper.Map<Message, MessageViewModel>(message);
                mess.Sender = await _db.Users.FirstOrDefaultAsync(u => u.Id == message.SenderId);
                mess.Recipient = await _db.Users.FirstOrDefaultAsync(u => u.Id == message.RecipientId);
                messageModels.Add(mess);
            }
            dialog.Messages = messageModels.ToArray();
            return dialog;
        }

        public async Task<OpenDialogueViewModel?> GetOpenDialogViewModelByInterlocutordId(Guid interlocutorId)
        {
            var dial = await _db.Dialogs.FirstOrDefaultAsync(d => d.InterlocutorFirstId == interlocutorId || d.InterlocutorSecondId == interlocutorId);
            if (dial == null) return null;
            var messages = await _db.Messages.Where(m => m.DialogId == dial.Id).ToListAsync();

            messages = messages.OrderBy(m => m.CreateTime).ToList();

            OpenDialogueViewModel dialog = new OpenDialogueViewModel();
            dialog.Id = dial.Id;
            dialog.isLooked = dial.isLooked;
            dialog.InterlocutorFirst = await _users.GetUserById(interlocutorId);

            List<MessageViewModel> messageModels = new List<MessageViewModel>();

            foreach (var message in messages.TakeLast(10).SkipLast(0))
            {
                var mess = _mapper.Map<Message, MessageViewModel>(message);
                mess.Sender = await _db.Users.FirstOrDefaultAsync(u => u.Id == message.SenderId);
                mess.Recipient = await _db.Users.FirstOrDefaultAsync(u => u.Id == message.RecipientId);
                messageModels.Add(mess);
            }
            dialog.Messages = messageModels.ToArray();
            return dialog;
        }

        public async Task<MessageViewModel[]> GetMessagesMore(Guid interlocutorId, int take, int skip)
        {
            var dial = await _db.Dialogs.FirstOrDefaultAsync(d => d.InterlocutorFirstId == interlocutorId || d.InterlocutorSecondId == interlocutorId);
            if (dial == null) return null;
            var messages = await _db.Messages.Where(m => m.DialogId == dial.Id).ToListAsync();

            messages = messages.OrderBy(m => m.CreateTime).ToList();

            List<MessageViewModel> messageModels = new List<MessageViewModel>();

            foreach (var message in messages.TakeLast(take).SkipLast(skip))
            {
                var mess = _mapper.Map<Message, MessageViewModel>(message);
                mess.Sender = await _db.Users.FirstOrDefaultAsync(u => u.Id == message.SenderId);
                mess.Recipient = await _db.Users.FirstOrDefaultAsync(u => u.Id == message.RecipientId);
                messageModels.Add(mess);
            }

            return messageModels.ToArray();
        }

        public async Task<Dialog?> GetDialogByInterlocutordId(Guid interlocutorId)
        {
            var dial = await _db.Dialogs.FirstOrDefaultAsync(d => d.InterlocutorFirstId == interlocutorId || d.InterlocutorSecondId == interlocutorId);
            return dial;
        }

        //public async Task<Message[]> GetLastMessages()
        //{
        //   var dialogs = await _db.Dialogs.ToListAsync();
        //   List<DialogueViewModel> messages = new List<DialogueViewModel>();
        //   foreach (var dialog in dialogs)
        //   {
        //      var mess = await _db.Messages.Where(m => m.DialogId == dialog.Id).ToListAsync();
        //      var dialog = new List<Me>
        //   messages.Add(new DialogueViewModel() { Messages = })
        //   }
        //}

        //public async Task<MessageViewModel[]> GetMessageViewModelsBySenderId(Guid senderId)
        //{
        //   Message[] messages = await GetMessagesBySenderId(senderId);
        //   List<MessageViewModel> messagesViewModels = new List<MessageViewModel>();
        //   foreach (var message in messages)
        //   {
        //      MessageViewModel messageViewModel = _mapper.Map<Message, MessageViewModel>(message);
        //      messageViewModel.Sender = await _users.GetUserById(message.SenderId);
        //      messageViewModel.Recipient = await _users.GetUserById(message.RecipientId);
        //      messagesViewModels.Add(messageViewModel);
        //   }
        //   return messagesViewModels.ToArray();
        //}

        //public async Task<DialogueViewModel[]> GetDialogueViewModelsByUserId(Guid id)
        //{
        //   var messages = await _db.Messages.Where(m => m.SenderId == id || m.RecipientId == id).ToListAsync();
        //   var MessagesModel = new List<DialogueViewModel>();
        //   foreach (var message in messages)
        //   {
        //   MessagesModel.Add(new DialogueViewModel() {  Messages = messages.Where(m => m.RecipientId == message.RecipientId)]})
        //   }
        //}

        public async Task LookedDialog(Guid userId, Dialog dialog)
        {
            var lastMessage = await _db.Messages.FirstOrDefaultAsync(m => m.Id == dialog.LastMessageId);
            if (lastMessage.SenderId == userId)
            {
                dialog.isLooked = true;
                _db.Dialogs.Update(dialog);
                await _db.SaveChangesAsync();
            }
        }

        public async Task Save(Message message, Dialog dialog)
        {
            var dial = await _db.Dialogs.FirstOrDefaultAsync(d => d.Id == dialog.Id);
            if (dial == null)
            {
                var Dialentry = _db.Entry(dialog);
                if (Dialentry.State == EntityState.Detached)
                    await _db.Dialogs.AddAsync(dialog);
            }

            var entry = _db.Entry(message);
            if (entry.State == EntityState.Detached)
                await _db.Messages.AddAsync(message);

            await _db.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            Message? message = await _db.Messages.FirstOrDefaultAsync(m => m.Id == id);
            if (message != null)
            {
                _db.Remove(message);
                await _db.SaveChangesAsync();
            }
        }
    }

    public interface IMessageRepository
    {
        Task<Message[]> GetMessages();
        Task<Dialog[]> GetDialogs();
        //Task<MessageViewModel[]> GetMessageViewModels();
        //Task<Message[]> GetMessagesBySenderId(Guid senderId);
        //Task<MessageViewModel[]> GetMessageViewModelsBySenderId(Guid senderId);
        //Task<DialogueViewModel[]> GetDialogueViewModelsByUserId(Guid id);
        Task<DialogueViewModel[]> GetDialogViewModelsByUserId(Guid userId);
        Task<OpenDialogueViewModel> GetOpenDialogViewModelById(Guid dialogId);
        Task<Dialog?> GetDialogByInterlocutordId(Guid interlocutorId);
        Task<OpenDialogueViewModel?> GetOpenDialogViewModelByInterlocutordId(Guid interlocutorId);
        Task Save(Message message, Dialog dialog);
        Task Delete(Guid id);
        Task<MessageViewModel[]> GetMessagesMore(Guid interlocutorId, int take, int skip);
        Task LookedDialog(Guid userId, Dialog dialog);
    }
}
