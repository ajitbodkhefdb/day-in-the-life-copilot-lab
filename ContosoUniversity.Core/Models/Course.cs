using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Core.Models
{
    public class Course
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Number")]
        public int CourseID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Required]
        public string Title { get; set; } = string.Empty;

        [Range(0, 5)]
        public int Credits { get; set; }

        public int DepartmentID { get; set; }

        [Display(Name = "Teaching Material Image")]
        [StringLength(255)]
        public string? TeachingMaterialImagePath { get; set; }

        /// <summary>
        /// Maximum number of students that can enroll in this course.
        /// Defaults to 30.
        /// </summary>
        [Range(1, 1000, ErrorMessage = "MaxEnrollment must be between 1 and 1000.")]
        [Display(Name = "Max Enrollment")]
        public int MaxEnrollment { get; set; } = 30;

        public virtual Department Department { get; set; } = null!;
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual ICollection<CourseAssignment> CourseAssignments { get; set; } = new List<CourseAssignment>();
    }
}
