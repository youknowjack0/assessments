using System;
using System.Collections;
using System.Collections.Generic;

namespace AssessmentNet.Models
{
    public abstract class Question
    {
        private int _weight = 1;

        public int Id { get; set; }

        public string QuestionHtml { get; set; }

        public virtual Test Test { get; set; }

        

        public int Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }

        public abstract int GetScore(QuestionResponse response);

        public abstract bool IsCorrect(QuestionResponse response);

        public TimeSpan AllowedTime { get; set; }
    }
}