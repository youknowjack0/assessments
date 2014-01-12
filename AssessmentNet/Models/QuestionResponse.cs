using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AssessmentNet.Models
{
    public class QuestionResponse
    {
        public int Id { get; set; }
        public TestRun TestRun { get; set; }
        public Question Question { get; set; }
        public virtual ICollection<QuestionResponseAnswer> Answers { get; set; }

        public bool HasStarted { get; set; }
        public DateTime Started { get; set; }

        public bool HasFinished { get; set; }
        public DateTime Finished { get; set; }

        public bool CanAnswer()
        {
            return !HasStarted || (!HasFinished && GetRemaining() > new TimeSpan(0));
        }

        public TimeSpan GetRemaining()
        {
            TimeSpan ts;
            if (!HasStarted)
                ts = Question.AllowedTime;
            else if (HasFinished)
                ts = new TimeSpan(0);
            else 
                ts = Question.AllowedTime - (DateTime.UtcNow - Started);

            var testtotal = TestRun.TimeToLive();

            if (ts < testtotal)
                return testtotal;
            else
                return ts;
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

    public class QuestionResponseAnswer
    {
        public int Id { get; set; }

        [Required]
        public virtual Answer Answer { get; set; }

        [Required]
        public virtual QuestionResponse Response { get; set; }
    }
}