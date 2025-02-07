using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using taskmanager.Data;
using taskmanager.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using taskmanager.ViewModels;

namespace taskmanager.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // Constructor with dependency injection for UserManager
        public ProjectsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Projects/Index
        public async Task<IActionResult> Index()
        {
            var projects = await _context.Projects
                                         .Include(p => p.CreatedByUser)  // Include the user who created the project
                                         .ToListAsync();

            return View(projects);  // Return the list of projects to the view
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            // Check if the user is authenticated before allowing them to create a project
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");  // Redirect to login page if the user is not logged in
            }

            // Create a new ProjectViewModel (empty or with default values)
            var projectViewModel = new ProjectViewModel();

            return View(projectViewModel);  // Pass the ViewModel to the Create view
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProjectID,ProjectName,Description,StartDate,EndDate")] ProjectViewModel projectViewModel)
        {
            if (ModelState.IsValid)
            {
                // Ensure the user is authenticated
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Error: Unable to get user details. Please login again.";
                    return RedirectToAction("Login", "Account");
                }

                // Map ProjectViewModel to Project
                var project = new Project
                {
                    ProjectName = projectViewModel.ProjectName,
                    Description = projectViewModel.Description,
                    StartDate = projectViewModel.StartDate,
                    EndDate = projectViewModel.EndDate,
                    CreatedByUserID = user.Id
                };

                // Add the project to the database and save changes
                _context.Add(project);
                await _context.SaveChangesAsync();

                TempData["ProjectSuccessMessage"] = "Project created successfully!";
                return RedirectToAction(nameof(Index));  // Redirect to the projects list after creation
            }

            TempData["ErrorMessage"] = "Error: Please check the form for errors.";
            return View(projectViewModel);  // Return the same view with the validation errors
        }
        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();  // If the id is null, return NotFound error.
            }
            var project = await _context.Projects
                                        .Include(p => p.CreatedByUser)  // ✅ Ensure CreatedByUser is loaded
                                        .FirstOrDefaultAsync(m => m.ProjectID == id);

            if (project == null)
            {
                return NotFound();  // If the project is not found, return NotFound error.
            }

            return View(project);  // Return the project to the Delete view for confirmation.
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project != null)
            {
                _context.Projects.Remove(project);  // Remove the project from the context
                await _context.SaveChangesAsync();  // Save changes to the database

                TempData["ProjectSuccessMessage"] = "Project deleted successfully!";
            }

            return RedirectToAction(nameof(Index));  // Redirect back to the projects list after deletion
        }
        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);  // Pass the project to the view
        }

        // POST: Projects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProjectID,ProjectName,Description,StartDate,EndDate")] Project project)
        {
            if (id != project.ProjectID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProject = await _context.Projects.FindAsync(id);
                    if (existingProject == null)
                    {
                        return NotFound();
                    }

                    // ✅ Manually update only allowed fields
                    existingProject.ProjectName = project.ProjectName;
                    existingProject.Description = project.Description;
                    existingProject.StartDate = project.StartDate;
                    existingProject.EndDate = project.EndDate;

                    _context.Update(existingProject);
                    await _context.SaveChangesAsync();

                    TempData["ProjectSuccessMessage"] = "Project updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Projects.Any(e => e.ProjectID == project.ProjectID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Error updating the project. Please check the form.";
            return View(project);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                                        .Include(p => p.CreatedByUser)  // ✅ Load CreatedByUser
                                        .FirstOrDefaultAsync(m => m.ProjectID == id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);  // ✅ Pass the project to the Details view
        }


    }
}

    

