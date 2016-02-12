using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Web.MVC.Minimal.Startup))]
namespace Web.MVC.Minimal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
