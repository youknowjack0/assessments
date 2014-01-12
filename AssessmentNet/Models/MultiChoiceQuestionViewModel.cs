using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AssessmentNet.Models
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