using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Web.MVC.Full.Startup))]
namespace Web.MVC.Full
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
