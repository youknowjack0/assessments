using System.Collections.Generic;

namespace AssessmentNet.Models
{
    public class MultiChoiceQuestion : Question
    {
        public ICollection<MultiChoiceAnswer> Answer { get; set; }
        public bool FullMarksForAnyCorrect { get; set; }
    }
}