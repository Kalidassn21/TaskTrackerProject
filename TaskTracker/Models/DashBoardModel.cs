
using System;
using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models
{  

    public class DashboardModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [StringLength(1000)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        [Required]
        [Display(Name = "Assigned To")]
        public string AssignedTo { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; } // Example values: Pending, In Progress, Completed
    }
}
