using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AssessmentNet.Models
{
    public class Test
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Creator { get; set; }
        [Required]
        public DateTime? Created { get; set; }

        public virtual ICollection<Question> Questions { get; set; }

        [Required]
        public double MaxDurationInHours { get; set; }
    }
}