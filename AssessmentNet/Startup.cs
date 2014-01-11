using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AssessmentNet.Startup))]
namespace AssessmentNet
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
