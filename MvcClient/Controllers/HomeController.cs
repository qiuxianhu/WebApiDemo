using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Mvc;
using MvcClient.Models;
using Newtonsoft.Json.Linq;
using WebApiCommon;

namespace MvcClient.Controllers
{
    public class HomeController : Controller
    {      
        private static string _AccessToken;
        HttpClient _httpClient = new HttpClient();
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public string ApplyToken()
        {
            string clientId = "123456";
            string secret = "qiuxianhu";
            
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("grant_type", "client_credentials");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
               "Basic",
               Convert.ToBase64String(Encoding.ASCII.GetBytes(clientId + ":" + secret))
               );
            var response = _httpClient.PostAsync(new Uri(new Uri("http://localhost:8013/"), "/Token"), new FormUrlEncodedContent(parameters)).Result;
            var responseValue = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _AccessToken = JObject.Parse(responseValue)["access_token"].Value<string>();
            }
            return _AccessToken;
        }
        [HttpGet]
        public string GetResource()
        {
            User user = new User()
            {
                Name = "qiuxianhu",
                Age = "24"
            };          
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _AccessToken);
            string returnResult= _httpClient.GetAsync(new Uri(new Uri("http://localhost:8011/"),string.Format("api/user?{0}", JsonHelper.ConvertJsonToStr(user)) )).Result.Content.ReadAsStringAsync().Result;
            //将returnResult解密并转化为json
            JsonModel jsonModel = JsonHelper.ConvertStrToJson<JsonModel>(Assistant.UnBoxing(returnResult));
            User userResult = JsonHelper.ConvertStrToJson<User>(jsonModel.Data.ToString());
            return userResult.Name+"的岁数是："+userResult.Age;
        }
    }
}