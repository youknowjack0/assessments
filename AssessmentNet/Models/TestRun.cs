using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssessmentNet.Models
{
    public class TestRun
    {
        public int Id { get; set; }
        public ApplicationUser Testee { get; set; }
        public Test Test { get; set; }

        public DateTime Created { get; set; }
        public DateTime Started { get; set; }

        public DateTime Expires { get; set; }

        public virtual ICollection<QuestionResponse> Responses { get; set; } 

        public TimeSpan TimeToLive()
        {
            TimeSpan ttl1 = Expires - DateTime.UtcNow;
            TimeSpan ttl2 = (Started + TimeSpan.FromHours(Test.MaxDurationInHours)) - DateTime.UtcNow;

            return ttl1 < ttl2 ? ttl1 : ttl2;
        }
    }
}