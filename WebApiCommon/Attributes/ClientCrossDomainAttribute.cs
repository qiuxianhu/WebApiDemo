using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Filters;

namespace WebApiCommon.Attributes
{
    /// <summary>
    /// 支持WebAPI客户端跨域,jsonp
    /// http://www.cnblogs.com/chiangchou/p/jsonp.html
    /// </summary>
    public class ClientCrossDomainAttribute : ActionFilterAttribute
    {
        private const string CALL_BACK_QUERY_PARAMETER = "callback";
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            string callback;
            if (IsJsonp(out callback))
            {
                var jsonBuilder = new StringBuilder(callback);
                jsonBuilder.AppendFormat("({0})", actionExecutedContext.Response.Content.ReadAsStringAsync().Result);
                actionExecutedContext.Response.Content = new StringContent(jsonBuilder.ToString());
            }
            base.OnActionExecuted(actionExecutedContext);
        }
        private bool IsJsonp(out string callback)
        {
            callback = HttpContext.Current.Request.QueryString[CALL_BACK_QUERY_PARAMETER];
            return !string.IsNullOrEmpty(callback);
        }
    }
}
