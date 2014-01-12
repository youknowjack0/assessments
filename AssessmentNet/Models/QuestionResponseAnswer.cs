using System.ComponentModel.DataAnnotations;

namespace AssessmentNet.Models
{
    public class QuestionResponseAnswer
    {
        public int Id { get; set; }

        [Required]
        public virtual Answer Answer { get; set; }

        [Required]
        public virtual QuestionResponse Response { get; set; }
    }
}