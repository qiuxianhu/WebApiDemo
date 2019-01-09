using System.Web.Http;
using WebApiCommon;

namespace WebApiDemo.Models
{
    [Authorize]
    public class BaseController : ApiControllerEx
    {

    }
}