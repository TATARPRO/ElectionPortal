using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ElectionPortal.Startup))]
namespace ElectionPortal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
