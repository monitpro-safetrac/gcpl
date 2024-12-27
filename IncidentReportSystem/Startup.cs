using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WorkPermitSystem.Startup))]
namespace WorkPermitSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
