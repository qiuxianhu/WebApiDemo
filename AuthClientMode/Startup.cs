using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(AuthClientMode.Startup))]
namespace AuthClientMode
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}