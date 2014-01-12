namespace AssessmentNet.Models
{
    public class MultiChoiceAnswer : Answer
    {
        public MultiChoiceQuestion Question { get; set; }
        public bool IsCorrect { get; set; }
        public string AnswerHtml { get; set; }
    }
}