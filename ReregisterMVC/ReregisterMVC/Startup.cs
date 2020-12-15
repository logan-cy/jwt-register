using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ReregisterMVC.Startup))]
namespace ReregisterMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
