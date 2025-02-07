using System;
using System.ComponentModel.DataAnnotations;

namespace taskmanager.ViewModels
{
    public class TaskViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Status { get; set; }  // Status like "Pending", "In Progress", "Completed"

        [DataType(DataType.Date)]
        public DateTime? Deadline { get; set; }  // Optional deadline

        [Required]
        public string AssignedUserID { get; set; }  // The ID of the user assigned to the task

        [Required]
        public int ProjectID { get; set; }  // The ID of the project the task belongs to
    }
}
