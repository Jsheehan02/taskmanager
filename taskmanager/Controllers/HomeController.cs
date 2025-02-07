using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using taskmanager.Data;
using taskmanager.Models;
using System.Diagnostics;
using System.Threading.Tasks;

namespace taskmanager.Controllers
{
    [Authorize] // Requires login for all actions in HomeController
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // Constructor to inject ApplicationDbContext and UserManager
        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Home/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                // Fetch tasks and include related entities (AssignedUser and Project)
                var tasks = await _context.ProjectTasks
                                          .Include(t => t.AssignedUser)
                                          .Include(t => t.Project)
                                          .ToListAsync();

                // Get the current user (if signed in)
                var user = await _userManager.GetUserAsync(User);
                ViewData["CurrentUser"] = user; // Pass user data to the view

                return View(tasks);
            }
            catch (Exception ex)
            {
                // Log the exception and show a user-friendly error message
                Console.Error.WriteLine($"Error fetching tasks: {ex.Message}");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        // Error Handling Page
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
