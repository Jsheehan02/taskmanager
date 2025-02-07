using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using taskmanager.Data;
using taskmanager.Models;
using System.Linq;
using System.Threading.Tasks;

namespace taskmanager.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Notifications/GetUnread
        [HttpGet]
        public async Task<IActionResult> GetUnread()
        {
            var userId = User.Identity.Name;
            var notifications = await _context.Notifications
                .Where(n => n.UserID == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new { n.NotificationID, n.Message })
                .ToListAsync();

            return Json(notifications);
        }

        // POST: Notifications/MarkAsRead/{id}
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null && notification.UserID == User.Identity.Name)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }

            return Ok();
        }
    }
}
