using System;
using System.Collections.Generic;

namespace AssessmentNet.Models
{
    public class QuestionResponse
    {
        public int Id { get; set; }
        public virtual TestRun TestRun { get; set; }
        public virtual Question Question { get; set; }
        public virtual ICollection<QuestionResponseAnswer> Answers { get; set; }

        public DateTime? Started { get; set; }

        public DateTime? Finished { get; set; }

        public bool CanAnswer()
        {
            return Started==null || (Finished==null && GetRemaining() > TimeSpan.Zero);
        }

        public TimeSpan GetRemaining()
        {
            TimeSpan ts;
            if (Started == null)
                ts = Question.AllowedTime;
            else if (Finished != null)
                ts = new TimeSpan(0);
            else 
                ts = Question.AllowedTime - (DateTime.UtcNow - Started.Value);

            var testtotal = TestRun.TimeToLive();

            if (ts < testtotal)
                return ts;
            else
                return testtotal;
        }

        public int GetScore()
        {
            return Question.GetScore(this);
        }

        public bool IsCorrect()
        {
            return Question.IsCorrect(this);
        }
    }
}