using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quartz;
using taskmanager.Data;
using taskmanager.Models;
using taskmanager.Jobs;

var builder = WebApplication.CreateBuilder(args);

// Configure Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Configure Identity options
    options.SignIn.RequireConfirmedAccount = false; // Change to true if email confirmation is required
    options.Password.RequireDigit = true;  // Requires a digit in the password
    options.Password.RequireLowercase = true;  // Requires a lowercase letter
    options.Password.RequireNonAlphanumeric = true;  // Requires a non-alphanumeric character
    options.Password.RequireUppercase = true;  // Requires an uppercase letter
    options.Password.RequiredLength = 6;  // Minimum length of the password
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();  // This adds the default Identity UI (including login, register, etc.)

// Configure Application Cookie (for redirecting unauthorized users)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";  // Redirect here if user is not logged in
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";  // Optional: Redirect here for access denied
});

// Configure Quartz.NET for background jobs
builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("TaskReminderJob");
    q.AddJob<TaskReminderJob>(opts => opts.WithIdentity(jobKey));
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("TaskReminderTrigger")
        .StartNow()
        .WithSimpleSchedule(x => x
            .WithIntervalInHours(1) // Run every hour
            .RepeatForever()));
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// Add MVC and Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Build the app
var app = builder.Build();

// Middleware Configuration
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();  // HTTP Strict Transport Security (HSTS) to secure the app
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Enable Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map Identity UI Pages (Required for login, register, etc.)
app.MapRazorPages();  // This is required to enable Identity UI

// Configure Endpoint Routing for controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Start the application
app.Run();
