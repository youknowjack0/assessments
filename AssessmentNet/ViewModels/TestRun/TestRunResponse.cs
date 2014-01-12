using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssessmentNet.ViewModels.TestRun
{
    public class TestRunResponseViewModel
    {
        public IEnumerable<AnswerSelectionViewModel> Answers { get; set; }
    }

    public class AnswerSelectionViewModel
    {
        public int AnswerId { get; set; }
        public bool Selected { get; set; }
    }
}