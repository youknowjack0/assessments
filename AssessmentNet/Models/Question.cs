namespace AssessmentNet.Models
{
    public class Question
    {
        private int _weight = 1;

        public int Id { get; set; }

        public string QuestionHtml { get; set; }

        public int Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }
    }
}