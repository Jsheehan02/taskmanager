using taskmanager.Models;

namespace taskmanager.Models { 
public class ProjectTask
{
    public int ProjectTaskID { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public DateTime? Deadline { get; set; }

    // Foreign Key for Assigned User
    public string AssignedUserID { get; set; }
    public ApplicationUser AssignedUser { get; set; }

    // Foreign Key for Project
    public int ProjectID { get; set; }
    public Project Project { get; set; }

    // Navigation Property for Notifications
    public ICollection<Notification> Notifications { get; set; }
}
}