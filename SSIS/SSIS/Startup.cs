using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SSIS.Startup))]
namespace SSIS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
