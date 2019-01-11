using Newtonsoft.Json;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using WebApiCommon;
using WebApiCommon.Binder;
using WebApiDemo.Models;

namespace WebApiDemo.Controllers
{
    public class UserController : BaseController
    {
        private static readonly JsonSerializerSettings _MicrosoftDateFormatSettings = new JsonSerializerSettings
        {
#if DEBUG
            Formatting = Formatting.Indented,
#endif
            DateFormatString = @"yyyy/MM/dd HH:mm:ss"
        };
        [HttpGet]
        public string GetName()
        {
            return "qiuxainhu";
        }
        [HttpPost]
        public HttpResponseMessage GetUserInfo([ModelBinder(typeof(FormRawBinder<User>))] User user)
        {
            User userResult = new User()
            {
                Name = "dachong",
                Age = "26"
            };
            return JsonManager.Get(new JsonModel()
            {
                Code = 0,
                Data = userResult,
                Error = null,
                Success = true,
                Message = "成功"
            }, _MicrosoftDateFormatSettings);
        }
    }
}
