using taskmanager.Models;

public class Notification
{
    public int NotificationID { get; set; }
    public string UserID { get; set; }
    public ApplicationUser User { get; set; }
    public string Message { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }

    // Foreign Key for ProjectTask
    public int TaskID { get; set; }
    public ProjectTask ProjectTask { get; set; }
}
