using Owin;

namespace WebApiDemo
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            // 这句是资源服务器认证token的关键，认证逻辑在里边封装好了，我们看不到
            app.UseOAuthBearerAuthentication(new Microsoft.Owin.Security.OAuth.OAuthBearerAuthenticationOptions());
        }
    }
}