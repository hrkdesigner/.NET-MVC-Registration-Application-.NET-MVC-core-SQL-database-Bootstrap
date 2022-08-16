using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lab5.Models.DataAccess
{
    public partial class AcademicRecord
    {
        public string CourseCode { get; set; } = null!;
        public string StudentId { get; set; } = null!; 
        [Range(0,100, ErrorMessage="Grade must be between 0 and 100")]
        public int? Grade { get; set; }

        public virtual Course CourseCodeNavigation { get; set; } = null!;
        public virtual Student Student { get; set; } = null!;
    }
}
