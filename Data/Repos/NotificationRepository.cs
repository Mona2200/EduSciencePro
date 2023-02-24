using EduSciencePro.Models;
using Microsoft.EntityFrameworkCore;

namespace EduSciencePro.Data.Repos
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _db;

        public NotificationRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Notification[]> GetNotificationsByUserId(Guid userId, int take = 5, int skip = 0) => await _db.Notifications.Where(n => n.UserId == userId).Take(take).Skip(skip).ToArrayAsync();

        public async Task Save(Notification notification)
        {
            if (!String.IsNullOrEmpty(notification.Content))
                _db.Notifications.Add(notification);
            await _db.SaveChangesAsync();
        }
    }

    public interface INotificationRepository
    {
        Task<Notification[]> GetNotificationsByUserId(Guid userId, int take = 5, int skip = 0);
        Task Save(Notification notification);
    }
}
