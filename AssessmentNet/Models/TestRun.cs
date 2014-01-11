using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssessmentNet.Models
{
    public class TestRun
    {
        public int Id { get; set; }
        public string StudentName { get; set; }
        public TestVersion Test { get; set; }
        public virtual ICollection<QuestionResponse> Responses { get; set; }
    }
}