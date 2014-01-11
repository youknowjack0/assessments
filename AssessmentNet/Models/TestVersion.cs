using System;
using System.Collections.Generic;

namespace AssessmentNet.Models
{
    public class TestVersion
    {
        public int Id { get; set; }
        public Test Test { get; set; }
        public int Version { get; set; }
        public DateTime Created { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
    }
}