using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AssessmentNet.Models
{
    public class TestRun
    {
        public const int GraceSeconds = 10;

        public int Id { get; set; }
        public virtual ApplicationUser Testee { get; set; }
        public virtual Test Test { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? Started { get; set; }
        public bool HasStarted { get; set; }

        public DateTime Expires { get; set; }

        public virtual ICollection<QuestionResponse> Responses { get; set; } 

        public TimeSpan TimeToLive()
        {
            TimeSpan ttl1 = Expires - DateTime.UtcNow;
            if (Started != null)
            {
                TimeSpan ttl2 = (Started.Value + TimeSpan.FromHours(Test.MaxDurationInHours)) - DateTime.UtcNow;
                return ttl1 < ttl2 ? ttl1 : ttl2;
            }

            return ttl1;
        }
    }
}