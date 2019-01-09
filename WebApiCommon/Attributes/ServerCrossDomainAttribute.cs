using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http.Filters;

namespace WebApiCommon.Attributes
{
    /// <summary>
    /// 支持WebAPI服务器端跨域 Cors
    /// </summary>
    public class ServerCrossDomainAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            HttpRequestHeaders requestHeaders = actionExecutedContext.Request.Headers;
            if (requestHeaders.Contains("Origin"))
            {
                string originHeader = requestHeaders.GetValues("Origin").FirstOrDefault();
                if (!string.IsNullOrEmpty(originHeader))
                {
                    HttpResponseHeaders responseHeaders = actionExecutedContext.Response.Headers;
                    responseHeaders.Add("Access-Control-Allow-Origin", originHeader);
                    responseHeaders.Add("Access-Control-Allow-Credentials", "true");
                    actionExecutedContext.Response.Headers.Add("Access-Control-Allow-Methods", "POST, GET, OPTIONS");
                    actionExecutedContext.Response.Headers.Add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
                }
            }
        }

    }
}
