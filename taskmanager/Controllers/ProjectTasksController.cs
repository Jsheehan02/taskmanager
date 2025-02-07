using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using taskmanager.Data;
using taskmanager.Models;
using taskmanager.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace taskmanager.Controllers
{
    public class ProjectTasksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // Constructor to inject ApplicationDbContext and UserManager
        public ProjectTasksController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ProjectTasks
        public async Task<IActionResult> Index()
        {
            var tasks = await _context.ProjectTasks
                                      .Include(t => t.AssignedUser)
                                      .Include(t => t.Project)
                                      .ToListAsync();

            // Ensure we get the authenticated user's ID correctly
            var currentUser = await _userManager.GetUserAsync(User); // Get the current user
            var currentUserId = currentUser?.Id; // Extract the User ID safely

            // Pass the tasks and currentUserId to the view
            ViewData["CurrentUserId"] = currentUserId;

            return View(tasks);
        }


        // GET: ProjectTasks/Create
        public IActionResult Create()
        {
            // Ensure to pass necessary data to the view (e.g., users or projects)
            ViewData["Users"] = _context.Users.ToList(); // Add Users for dropdown
            ViewData["Projects"] = _context.Projects.ToList(); // Add Projects for dropdown
            return View();
        }

        // POST: ProjectTasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskViewModel model)
        {
            if (ModelState.IsValid)
            {
                var task = new ProjectTask
                {
                    Title = model.Title,
                    Description = model.Description,
                    Status = model.Status,
                    Deadline = model.Deadline,
                    AssignedUserID = model.AssignedUserID,
                    ProjectID = model.ProjectID
                };

                _context.ProjectTasks.Add(task);
                await _context.SaveChangesAsync();

                if (!string.IsNullOrEmpty(model.AssignedUserID))
                {
                    var notification = new Notification
                    {
                        UserID = model.AssignedUserID,
                        Message = $"You have been assigned a new task: {task.Title}",
                        TaskID = task.ProjectTaskID,
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.Notifications.Add(notification);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["Users"] = await _userManager.Users.ToListAsync();
            ViewData["Projects"] = await _context.Projects.ToListAsync();
            return View(model);
        }

        // GET: ProjectTasks/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var task = await _context.ProjectTasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            // Prepare data for the dropdowns
            ViewData["Users"] = await _userManager.Users.ToListAsync();
            ViewData["Projects"] = await _context.Projects.ToListAsync();

            var model = new TaskViewModel
            {
                ProjectID = task.ProjectID,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Deadline = task.Deadline,
                AssignedUserID = task.AssignedUserID
            };

            return View(model);
        }

        // POST: ProjectTasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaskViewModel model)
        {
            if (id != model.ProjectID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var task = await _context.ProjectTasks.FindAsync(id);
                if (task == null)
                {
                    return NotFound();
                }

                task.Title = model.Title;
                task.Description = model.Description;
                task.Status = model.Status;
                task.Deadline = model.Deadline;
                task.AssignedUserID = model.AssignedUserID;
                task.ProjectID = model.ProjectID;

                _context.Update(task);
                await _context.SaveChangesAsync();

                // Notify the assigned user about task updates
                if (!string.IsNullOrEmpty(model.AssignedUserID))
                {
                    var notification = new Notification
                    {
                        UserID = model.AssignedUserID,
                        Message = $"Your task '{task.Title}' has been updated.",
                        TaskID = task.ProjectTaskID
                    };

                    _context.Notifications.Add(notification);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["Users"] = await _userManager.Users.ToListAsync();
            ViewData["Projects"] = await _context.Projects.ToListAsync();
            return View(model);
        }

        // POST: ProjectTasks/MarkComplete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkComplete(int id)
        {
            var task = await _context.ProjectTasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            // Update the task status
            task.Status = "Completed";
            await _context.SaveChangesAsync();

            // Redirect back to the task list
            return RedirectToAction("Index");
        }

        // POST: ProjectTasks/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _context.ProjectTasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            // Get the current logged-in user
            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserId = currentUser?.Id;  // Ensure safe access

            // Ensure only the assigned user can delete the task
            if (task.AssignedUserID != currentUserId)
            {
                return Forbid(); // Prevents unauthorized users from deleting the task
            }

            _context.ProjectTasks.Remove(task);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
