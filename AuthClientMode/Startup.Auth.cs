using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace AuthClientMode
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions()
            {
                TokenEndpointPath = new PathString("/Token"),//获取 access_token 授权服务请求地址,即http://localhost:端口号/token；
                ApplicationCanDisplayErrors = true,
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(10),
#if DEBUG
                AllowInsecureHttp=true,
#endif
                Provider = new OAuthAuthorizationServerProvider
                {
                    OnValidateClientAuthentication = ValidateClientAuthentication,
                    OnGrantClientCredentials = GrantClientCredetails
                }
            });
        }
        /// <summary>
        /// ValidateClientAuthentication方法用来对third party application 认证，
        /// 获取客户端的 client_id 与 client_secret 进行验证
        /// context.Validated(); 表示允许此third party application请求。
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId = null;
            string clientSecret = null;
            if (context.TryGetBasicCredentials(out clientId, out clientSecret) ||
                context.TryGetFormCredentials(out clientId, out clientSecret))
            {
                if (clientId == "123456" && clientSecret == "qiuxianhu")
                {
                    context.Validated();
                }
            }
            return Task.FromResult(0);
        }

        /// <summary>
        /// 该方法是对客户端模式进行授权的时候使用的        
        /// 对客户端进行授权，授了权就能发 access token 。
        /// 只有这两个方法(ValidateClientAuthentication和GrantClientCredetails)同时认证通过才会颁发token。
        private Task GrantClientCredetails(OAuthGrantClientCredentialsContext context)
        {
            GenericIdentity genericIdentity = new GenericIdentity(context.ClientId, OAuthDefaults.AuthenticationType);
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(genericIdentity, context.Scope.Select(x => new Claim("urn:oauth:scope", x)));
            context.Validated(claimsIdentity);
            return Task.FromResult(0);
        }
    }
}