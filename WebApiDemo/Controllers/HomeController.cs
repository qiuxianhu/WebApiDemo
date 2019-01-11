using System.Web.Http;

namespace WebApiDemo.Controllers
{
    public class HomeController : ApiController
    {
        [HttpGet]
        public string GetName()
        {
            return "qiuxainhu";
        }
    }
}
