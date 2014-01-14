using System;
using System.Collections.Generic;
using AssessmentNet.Models;

namespace AssessmentNet.ViewModels.TestRunViewModels
{
    public class TestRunResponseViewModel
    {
        public int TestRunId { get; set; }
        public int QuestionResponseId { get; set; }
        public IEnumerable<AnswerSelectionViewModel> Answers { get; set; }
        public TestRun TestRun { get; set; }
        public QuestionResponse QuestionResponse { get; set; }
        public TimeSpan Remaining { get; set; }
    }
}