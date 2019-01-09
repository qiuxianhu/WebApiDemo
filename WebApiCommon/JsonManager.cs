using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebApiCommon
{
    public static class JsonManager
    {
        private const string CONTENT_TYPE = "application/json";
        public static HttpResponseMessage Get(JsonModel jsonModel, JsonSerializerSettings settings)
        {

            if (jsonModel == null)
            {
                throw new ArgumentNullException("jsonModel");
            }
            string json = null;
            if (settings == null)
            {
                json = JsonHelper.ConvertJsonToStr(jsonModel);
            }
            else
            {
                json = JsonHelper.ConvertJsonToStr(jsonModel, settings);
            }
            json = Assistant.Boxing(json);//装箱
            return new HttpResponseMessage()
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, CONTENT_TYPE),
            };
        }
    }
    /// <summary>
    /// json通信实体
    /// </summary>
    [Serializable]
    public class JsonModel
    {
        /// <summary>
        /// 错误码(0表示无错误)
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string[] Error { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Json数据
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// 服务器响应时间 
        /// </summary>
        public long ResponseTicks
        {
            get
            {
                return DateTime.Now.Ticks;
            }
        }
    }
}
