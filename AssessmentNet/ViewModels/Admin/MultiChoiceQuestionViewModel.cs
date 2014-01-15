using System.Collections.Generic;

namespace AssessmentNet.ViewModels.Admin
{
    public class MultiChoiceQuestionViewModel
    {
        private double _allowedTimeInMinutes = 2;
        public int TestId { get; set; }

        public double AllowedTimeInMinutes
        {
            get { return _allowedTimeInMinutes; }
            set { _allowedTimeInMinutes = value; }
        }

        public string QuestionHtml { get; set; }
        public int Weight { get; set; }
        public virtual IEnumerable<SimpleAnswerViewModel> Answers { get; set; }
        public int QuestionId { get; set; }
    }
}