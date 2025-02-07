using System;
using System.ComponentModel.DataAnnotations;

namespace taskmanager.ViewModels
{
    public class ProjectViewModel
    {
        [Required]
        [StringLength(100)]
        public string ProjectName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; } = DateTime.Today;

        public DateTime? EndDate { get; set; }
    }
}
