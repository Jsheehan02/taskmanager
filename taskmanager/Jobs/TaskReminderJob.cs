namespace taskmanager.Jobs
{
    using Microsoft.EntityFrameworkCore;
    using Quartz;
    using taskmanager.Data;
    using taskmanager.Models;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class TaskReminderJob : IJob
    {
        private readonly ApplicationDbContext _context;

        public TaskReminderJob(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var now = DateTime.UtcNow;
            var reminderTime = now.AddHours(24); // Look for tasks due in the next 24 hours

            var tasks = await _context.ProjectTasks
                                      .Where(t => t.Deadline.HasValue &&
                                                  t.Deadline.Value <= reminderTime &&
                                                  t.Deadline.Value > now)
                                      .ToListAsync();

            foreach (var task in tasks)
            {
                if (!string.IsNullOrEmpty(task.AssignedUserID))
                {
                    var notification = new Notification
                    {
                        UserID = task.AssignedUserID,
                        Message = $"Reminder: Your task '{task.Title}' is due on {task.Deadline.Value.ToShortDateString()}",
                        TaskID = task.ProjectTaskID
                    };

                    _context.Notifications.Add(notification);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
