using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AssessmentNet.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        public virtual IDbSet<Answer> Answers { get; set; }
        public virtual IDbSet<Question> Questions { get; set; }
        public virtual IDbSet<QuestionResponse> Responses { get; set; }
        public virtual IDbSet<Test> Tests { get; set; }
        public virtual IDbSet<TestRun> TestRun { get; set; }
    }
}