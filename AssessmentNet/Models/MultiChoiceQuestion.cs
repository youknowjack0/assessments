using System.Collections.Generic;
using System.Linq;

namespace AssessmentNet.Models
{
    /// <summary>
    /// multiple answers, must get exactly correct or else 0 points
    /// </summary>
    public class MultiChoiceQuestion : Question
    {

        public virtual ICollection<MultiChoiceAnswer> Answers { get; set; }

        public override int GetScore(QuestionResponse response)
        {
            return IsCorrect(response) ? Weight : 0;
        }

        public override bool IsCorrect(QuestionResponse response)
        {
            foreach (var item in Answers)
            {
                if (response.Answers.Any(x => x.Answer == item))
                {
                    if (!item.IsCorrect)
                        return false;
                }
                else
                {
                    if (item.IsCorrect)
                        return false;
                }
            }
            return true;
        }
    }
}