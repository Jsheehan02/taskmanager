using taskmanager.Models;

    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        // Navigation property for tasks assigned to the user
        public ICollection<ProjectTask> AssignedTasks { get; set; }  // This is the missing property
    }

