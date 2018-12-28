using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(proiectDAW.Startup))]
namespace proiectDAW
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
