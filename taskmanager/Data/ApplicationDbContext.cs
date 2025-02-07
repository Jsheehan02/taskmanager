using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using taskmanager.Models;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ProjectTask> ProjectTasks { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Notification> Notifications { get; set; }


    // Override OnModelCreating to configure the relationships
 protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Configure relationship between Project and ApplicationUser (CreatedByUserID)
    modelBuilder.Entity<Project>()
        .HasOne(p => p.CreatedByUser)
        .WithMany()
        .HasForeignKey(p => p.CreatedByUserID)
        .OnDelete(DeleteBehavior.NoAction);  // Prevents circular delete issues

        modelBuilder.Entity<ProjectTask>()
            .HasOne(pt => pt.Project)  // Each task belongs to one project
            .WithMany(p => p.Tasks)  // A project can have multiple tasks
            .HasForeignKey(pt => pt.ProjectID)
            .OnDelete(DeleteBehavior.Cascade);  // If a project is deleted, delete its tasks

        // Configure ProjectTask relationship with ApplicationUser (AssignedUserID)
        modelBuilder.Entity<ProjectTask>()
        .HasOne(pt => pt.AssignedUser)  // A task is assigned to one user
        .WithMany()  // User can have multiple tasks (but no navigation property in ApplicationUser)
        .HasForeignKey(pt => pt.AssignedUserID)
        .OnDelete(DeleteBehavior.NoAction);  // Prevent accidental user deletions

        // Configure Notification relationship with ProjectTask (One-to-Many)
        modelBuilder.Entity<Notification>()
          .HasOne(n => n.ProjectTask)
          .WithMany(pt => pt.Notifications)
          .HasForeignKey(n => n.TaskID)
          .OnDelete(DeleteBehavior.Cascade);
    }

}
