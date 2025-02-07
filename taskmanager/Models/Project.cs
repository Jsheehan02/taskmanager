using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace taskmanager.Models
{
    public class Project
    {
        [Key]
        public int ProjectID { get; set; }

        [Required]
        [StringLength(100)]
        public string ProjectName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        // Nullable CreatedByUserID (automatically set in the controller)
        public string? CreatedByUserID { get; set; }

        // Navigation property for the user who created the project
        public ApplicationUser? CreatedByUser { get; set; }

        // Navigation property for tasks associated with this project
        public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    }
}
