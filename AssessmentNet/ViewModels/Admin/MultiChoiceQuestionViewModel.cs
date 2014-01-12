using System.Collections.Generic;

namespace AssessmentNet.ViewModels.Admin
{
    public class MultiChoiceQuestionViewModel
    {
        public int TestId { get; set; }
        public double AllowedTimeInMinutes { get; set; }
        public string QuestionHtml { get; set; }
        public int Weight { get; set; }
        public virtual IEnumerable<SimpleAnswerViewModel> Answers { get; set; } 
    }
}