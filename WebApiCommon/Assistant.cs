using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WebApiCommon
{

    /// <summary>
    /// WebAPI辅助类型
    /// 用来封装WebAPI数据通信时数据必须要经过拆箱或者装箱的处理
    /// </summary>
    public class Assistant
    {
        private const string MIME_TYPE = "application/json";
        /// <summary>
        /// 配置文件里面是否使用RSA加密密钥对
        /// </summary>
        private static readonly bool _KEY = false;
        static Assistant()
        {
            _KEY = !string.IsNullOrWhiteSpace(ConfigSetting.AESKey);
            //_KEY = !string.IsNullOrWhiteSpace(ConfigSetting.XXTEAKey);
            //_KEY = !string.IsNullOrWhiteSpace(ConfigSetting.RSAPublicKey)
            //        && !string.IsNullOrWhiteSpace(ConfigSetting.RSAPrivateKey);
        }

        #region 装箱、拆箱数据
        /// <summary>
        /// 装箱(将服务器端数据发送给APP客户端需要先将服务器端数据进行装箱)
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static string Boxing(string json)
        {
            if (!string.IsNullOrWhiteSpace(json))
            {
                if (_KEY)
                {
                    json = AESCryptoHelper.Encrypt(json, ConfigSetting.AESKey);//加密
                    //json = EncryptHelper.CustomEncrypt(json);//加密
                    //json = XXTEA1CryptoHelper.Encrypt(json, ConfigSetting.XXTEAKey);//加密
                    //json = RSACryptoServiceHelper.RSAEncryptXML(ConfigSetting.RSAPublicKey, json);//加密
                }
                //json = ZipHelper.GZipCompress(json);//压缩,wangyunpeng,2017-2-10,不使用压缩方式.
            }
            return json;
        }
        /// <summary>
        /// 拆箱(接收从APP客户端发送过来的数据需要先将接收到的数据进行拆箱)
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static string UnBoxing(string json)
        {
            if (!string.IsNullOrWhiteSpace(json))
            {
                //json = ZipHelper.GZipDeCompress(json);//解压缩,wangyunpeng,2017-2-10,不使用压缩方式.

                if (_KEY)
                {
                    json = AESCryptoHelper.Decrypt(json, ConfigSetting.AESKey);//解密
                    //json = EncryptHelper.CustomEncrypt(json);//解密
                    //json = XXTEA1CryptoHelper.Decrypt(json, ConfigSetting.XXTEAKey);//解密
                    //json = RSACryptoServiceHelper.RSADecryptXML(ConfigSetting.RSAPrivateKey, json);//解密
                }
            }
            return json;
        }
        #endregion
        #region 使用签名来保证ASP.NET MVC OR WEBAPI的接口安全（OAuth2.0的替代）。http://www.cnblogs.com/kklldog/p/webapi-signature.html
        /// <summary>
        /// 制作加密的签名结果
        /// </summary>
        /// <param name="secretKey"></param>
        /// <param name="signPlain">签名信息（明文）</param>
        /// <returns>签名信息（密文）</returns>
        public static string SignByHmacSHA256(string secretKey, string signPlain)
        {
            byte[] secretKeys = Encoding.UTF8.GetBytes(secretKey);
            byte[] signPlains = Encoding.UTF8.GetBytes(signPlain);
            using (HMACSHA256 hmacsha256 = new HMACSHA256(secretKeys))
            {
                StringBuilder sb = new StringBuilder();
                byte[] hashValues = hmacsha256.ComputeHash(signPlains);
                foreach (byte x in hashValues)
                {
                    sb.Append(string.Format("{0:x2}", x));
                }
                return sb.ToString();
            }
        }
        /// <summary>
        /// 服务器端验证签名
        /// </summary>
        /// <param name="request"></param>
        /// <param name="secretKey"></param>
        /// <returns></returns>
        public static bool ValidByHmacSHA256(HttpRequestMessage request, string secretKey)
        {
            string time = string.Join(",", request.Headers.GetValues("time"));
            string random = string.Join(",", request.Headers.GetValues("random"));
            long requestTime = 0L;
            if (long.TryParse(time, out requestTime))
            {
                DateTime now = DateTime.Now;
                if (!(long.Parse(now.AddMinutes(5).ToString("yyyyMMddHHmmss")) >= requestTime
                    && long.Parse(now.AddMinutes(-5).ToString("yyyyMMddHHmmss")) <= requestTime))
                {//最好验证下时间戳，跟服务端时间比较前后不能相差5分钟。这也是一个简单的防Replay Attack的手段。
                 //预防Replay Attack（重放攻击）
                 // 预防重放攻击主要有两点：
                 //  1、校验时间戳的范围：时间戳跟服务器时间相差在一个合理的范围内视为合法。
                 //  2、缓存签名：每次请求都去判断下签名是否出现过。如果出现过则视为非法请求。因为有时间戳跟随机数的存在，所以理论上每次请求的签名是不可能重复的。
                    return false;
                }
            }
            else
            {
                return false;
            }
            string requestSign = string.Join(",", request.Headers.GetValues("sign"));
            string signPlain = MakeSignPlain(request.GetQueryNameValuePairs(), time, random);
            if (string.IsNullOrEmpty(requestSign)
                || string.IsNullOrEmpty(signPlain))
            {
                return false;
            }
            //hashmac
            string signSecret = SignByHmacSHA256(secretKey, signPlain);
            return requestSign.Equals(signSecret, StringComparison.CurrentCultureIgnoreCase);
        }
        /// <summary>
        /// 制作签名信息（明文）
        /// </summary>
        /// <param name="queryString">客户端需要传：需要给服务器端传的QueryString，服务器端：从Request的QueryString里面取。</param>
        /// <param name="time">客户端需要传：DateTime.Now.ToString("yyyyMMddHHmmss")，服务器端：从Request的Headers里面取。</param>
        /// <param name="random">客户端需要传：Guid.NewGuid().ToString()，服务器端：从Request的Headers里面取。</param>
        /// <returns>签名信息（明文）</returns>
        public static string MakeSignPlain(IEnumerable<KeyValuePair<string, string>> queryString, string time, string random)
        {
            var sb = new StringBuilder();
            foreach (KeyValuePair<string, string> keyValuePair in queryString)
            {
                sb.AppendFormat("{0}={1}&", keyValuePair.Key, keyValuePair.Value);
            }
            if (sb.Length > 1)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append(time);
            sb.Append(random);
            return sb.ToString().ToUpper();
        }
        #endregion
        #region 获取WebAPI数据
        /// <summary>
        /// Get方式获取数据
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="urlToken"></param>
        /// <param name="urlApi"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string Get(Dictionary<string, string> auth, string urlToken, string urlApi, NameValueCollection parameters)
        {
            string responseText = null;
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                handler.UseProxy = false;
                using (HttpClient client = new HttpClient(handler))
                {
                    string accessToken = null;
                    using (FormUrlEncodedContent content = new FormUrlEncodedContent(auth))
                    {
                        using (HttpResponseMessage responseMessage = client.PostAsync(urlToken, content).Result)
                        {
#if !DEBUG
                            responseMessage.EnsureSuccessStatusCode();
#endif
                            responseText = responseMessage.Content.ReadAsStringAsync().Result;
                            if (string.IsNullOrWhiteSpace(responseText))
                            {
                                throw new Exception("授权失败！");
                            }
                            else
                            {
                                JObject jObject = JsonHelper.ConvertStrToJson(responseText);
                                accessToken = jObject.Get<string>("access_token");
                                if (string.IsNullOrWhiteSpace(accessToken))
                                {
                                    throw new Exception("令牌无效！");
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(accessToken))
                    {
                        string authorization = "Bearer " + accessToken;
                        client.DefaultRequestHeaders.Add("Authorization", authorization);
                        UriBuilder uriBuilder = new UriBuilder(urlApi);
                        if (parameters != null && parameters.Count > 0)
                        {
                            uriBuilder.Query = UtilHelper.BuildQueryString(parameters);
                        }
                        using (HttpResponseMessage responseMessage = client.GetAsync(uriBuilder.Uri).Result)
                        {
#if !DEBUG
                            responseMessage.EnsureSuccessStatusCode();
#endif
                            responseText = responseMessage.Content.ReadAsStringAsync().Result;
                        }
                    }
                }
            }
            return responseText;
        }
        /// <summary>
        /// Get方式获取数据(支持GZip)
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="urlToken"></param>
        /// <param name="urlApi"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string GetGZip(Dictionary<string, string> auth, string urlToken, string urlApi, NameValueCollection parameters)
        {
            string responseText = null;
            using (HttpClientHandler handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip })
            {
                handler.UseProxy = false;
                using (HttpClient client = new HttpClient(handler))
                {
                    string accessToken = null;
                    using (FormUrlEncodedContent content = new FormUrlEncodedContent(auth))
                    {
                        using (HttpResponseMessage responseMessage = client.PostAsync(urlToken, content).Result)
                        {
#if !DEBUG
                            responseMessage.EnsureSuccessStatusCode();
#endif
                            responseText = responseMessage.Content.ReadAsStringAsync().Result;
                            if (string.IsNullOrWhiteSpace(responseText))
                            {
                                throw new Exception("授权失败！");
                            }
                            else
                            {
                                JObject jObject = JsonHelper.ConvertStrToJson(responseText);
                                accessToken = jObject.Get<string>("access_token");
                                if (string.IsNullOrWhiteSpace(accessToken))
                                {
                                    throw new Exception("令牌无效！");
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(accessToken))
                    {
                        string authorization = "Bearer " + accessToken;
                        client.DefaultRequestHeaders.Add("Authorization", authorization);
                        UriBuilder uriBuilder = new UriBuilder(urlApi);
                        if (parameters != null && parameters.Count > 0)
                        {
                            uriBuilder.Query = UtilHelper.BuildQueryString(parameters);
                        }
                        using (HttpResponseMessage responseMessage = client.GetAsync(uriBuilder.Uri).Result)
                        {
#if !DEBUG
                            responseMessage.EnsureSuccessStatusCode();
#endif
                            responseText = responseMessage.Content.ReadAsStringAsync().Result;
                        }
                    }
                }
            }
            return responseText;
        }

        /// <summary>
        /// Post方式提交数据(仅支持RAW方式)
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="urlToken"></param>
        /// <param name="urlApi"></param>
        /// <param name="raw"></param>
        /// <returns></returns>
        public static string Post(Dictionary<string, string> auth, string urlToken, string urlApi, string raw)
        {
            string responseText = null;
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                handler.UseProxy = false;
                using (HttpClient client = new HttpClient(handler))
                {
                    string accessToken = null;
                    using (FormUrlEncodedContent content = new FormUrlEncodedContent(auth))
                    {
                        using (HttpResponseMessage responseMessage = client.PostAsync(urlToken, content).Result)
                        {
#if !DEBUG
                            responseMessage.EnsureSuccessStatusCode();
#endif
                            responseText = responseMessage.Content.ReadAsStringAsync().Result;
                            if (string.IsNullOrWhiteSpace(responseText))
                            {
                                throw new Exception("授权失败！");
                            }
                            else
                            {
                                JObject jObject = JsonHelper.ConvertStrToJson(responseText);
                                accessToken = jObject.Get<string>("access_token");
                                if (string.IsNullOrWhiteSpace(accessToken))
                                {
                                    throw new Exception("令牌无效！");
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(accessToken))
                    {
                        string authorization = "Bearer " + accessToken;
                        client.DefaultRequestHeaders.Add("Authorization", authorization);
                        using (StringContent content = new StringContent(raw))
                        {
                            content.Headers.ContentType = new MediaTypeHeaderValue(MIME_TYPE);
                            using (HttpResponseMessage responseMessage = client.PostAsync(urlApi, content).Result)
                            {
#if !DEBUG
                                responseMessage.EnsureSuccessStatusCode();
#endif
                                responseText = responseMessage.Content.ReadAsStringAsync().Result;
                            }
                        }
                    }
                }
            }
            return responseText;
        }
        /// <summary>
        /// Post方式提交数据(仅支持RAW方式)(支持GZip)
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="urlToken"></param>
        /// <param name="urlApi"></param>
        /// <param name="raw"></param>
        /// <returns></returns>
        public static string PostGZip(Dictionary<string, string> auth, string urlToken, string urlApi, string raw)
        {
            string responseText = null;
            using (HttpClientHandler handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip })
            {
                handler.UseProxy = false;
                using (HttpClient client = new HttpClient(handler))
                {
                    string accessToken = null;
                    using (FormUrlEncodedContent content = new FormUrlEncodedContent(auth))
                    {
                        using (HttpResponseMessage responseMessage = client.PostAsync(urlToken, content).Result)
                        {
#if !DEBUG
                            responseMessage.EnsureSuccessStatusCode();
#endif
                            responseText = responseMessage.Content.ReadAsStringAsync().Result;
                            if (string.IsNullOrWhiteSpace(responseText))
                            {
                                throw new Exception("授权失败！");
                            }
                            else
                            {
                                JObject jObject = JsonHelper.ConvertStrToJson(responseText);
                                accessToken = jObject.Get<string>("access_token");
                                if (string.IsNullOrWhiteSpace(accessToken))
                                {
                                    throw new Exception("令牌无效！");
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(accessToken))
                    {
                        string authorization = "Bearer " + accessToken;
                        client.DefaultRequestHeaders.Add("Authorization", authorization);
                        using (StringContent content = new StringContent(raw))
                        {
                            content.Headers.ContentType = new MediaTypeHeaderValue(MIME_TYPE);
                            using (HttpResponseMessage responseMessage = client.PostAsync(urlApi, content).Result)
                            {
#if !DEBUG
                                responseMessage.EnsureSuccessStatusCode();
#endif
                                responseText = responseMessage.Content.ReadAsStringAsync().Result;
                            }
                        }
                    }
                }
            }
            return responseText;
        }


        /// <summary>
        /// Post方式提交数据(仅支持FormUrlEncoded方式)
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="urlToken"></param>
        /// <param name="urlApi"></param>
        /// <param name="nameValueCollection">key,value键值对</param>
        /// <returns></returns>
        public static string Post(Dictionary<string, string> auth, string urlToken, string urlApi, Dictionary<string, string> nameValueCollection)
        {
            string responseText = null;
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                handler.UseProxy = false;
                using (HttpClient client = new HttpClient(handler))
                {
                    string accessToken = null;
                    using (FormUrlEncodedContent content = new FormUrlEncodedContent(auth))
                    {
                        using (HttpResponseMessage responseMessage = client.PostAsync(urlToken, content).Result)
                        {
#if !DEBUG
                            responseMessage.EnsureSuccessStatusCode();
#endif
                            responseText = responseMessage.Content.ReadAsStringAsync().Result;
                            if (string.IsNullOrWhiteSpace(responseText))
                            {
                                throw new Exception("授权失败！");
                            }
                            else
                            {
                                JObject jObject = JsonHelper.ConvertStrToJson(responseText);
                                accessToken = jObject.Get<string>("access_token");
                                if (string.IsNullOrWhiteSpace(accessToken))
                                {
                                    throw new Exception("令牌无效！");
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(accessToken))
                    {
                        string authorization = "Bearer " + accessToken;
                        client.DefaultRequestHeaders.Add("Authorization", authorization);
                        using (FormUrlEncodedContent content = new FormUrlEncodedContent(nameValueCollection))
                        {
                            content.Headers.ContentType = new MediaTypeHeaderValue(MIME_TYPE);
                            using (HttpResponseMessage responseMessage = client.PostAsync(urlApi, content).Result)
                            {
#if !DEBUG
                                responseMessage.EnsureSuccessStatusCode();
#endif
                                responseText = responseMessage.Content.ReadAsStringAsync().Result;
                            }
                        }
                    }
                }
            }
            return responseText;
        }

        /// <summary>
        /// Post方式提交数据(仅支持FormUrlEncoded方式)(支持GZip)
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="urlToken"></param>
        /// <param name="urlApi"></param>
        /// <param name="nameValueCollection">key,value键值对</param>
        /// <returns></returns>
        public static string PostGZip(Dictionary<string, string> auth, string urlToken, string urlApi, Dictionary<string, string> nameValueCollection)
        {
            string responseText = null;
            using (HttpClientHandler handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip })
            {
                handler.UseProxy = false;
                using (HttpClient client = new HttpClient(handler))
                {
                    string accessToken = null;
                    using (FormUrlEncodedContent content = new FormUrlEncodedContent(auth))
                    {
                        using (HttpResponseMessage responseMessage = client.PostAsync(urlToken, content).Result)
                        {
#if !DEBUG
                            responseMessage.EnsureSuccessStatusCode();
#endif
                            responseText = responseMessage.Content.ReadAsStringAsync().Result;
                            if (string.IsNullOrWhiteSpace(responseText))
                            {
                                throw new Exception("授权失败！");
                            }
                            else
                            {
                                JObject jObject = JsonHelper.ConvertStrToJson(responseText);
                                accessToken = jObject.Get<string>("access_token");
                                if (string.IsNullOrWhiteSpace(accessToken))
                                {
                                    throw new Exception("令牌无效！");
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(accessToken))
                    {
                        string authorization = "Bearer " + accessToken;
                        client.DefaultRequestHeaders.Add("Authorization", authorization);
                        using (FormUrlEncodedContent content = new FormUrlEncodedContent(nameValueCollection))
                        {
                            content.Headers.ContentType = new MediaTypeHeaderValue(MIME_TYPE);
                            using (HttpResponseMessage responseMessage = client.PostAsync(urlApi, content).Result)
                            {
#if !DEBUG
                                responseMessage.EnsureSuccessStatusCode();
#endif
                                responseText = responseMessage.Content.ReadAsStringAsync().Result;
                            }
                        }
                    }
                }
            }
            return responseText;
        }

        /// <summary>
        /// Post方式上传文件
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="urlToken"></param>
        /// <param name="urlApi"></param>
        /// <param name="formDataContent"></param>
        /// <returns></returns>
        public static string PostFile(Dictionary<string, string> auth, string urlToken, string urlApi, MultipartFormDataContent formDataContent)
        {
            string responseText = null;
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                handler.UseProxy = false;
                using (HttpClient client = new HttpClient(handler))
                {
                    string accessToken = null;
                    using (FormUrlEncodedContent content = new FormUrlEncodedContent(auth))
                    {
                        using (HttpResponseMessage responseMessage = client.PostAsync(urlToken, content).Result)
                        {
#if !DEBUG
                            responseMessage.EnsureSuccessStatusCode();
#endif
                            responseText = responseMessage.Content.ReadAsStringAsync().Result;
                            if (string.IsNullOrWhiteSpace(responseText))
                            {
                                throw new Exception("授权失败！");
                            }
                            else
                            {
                                JObject jObject = JsonHelper.ConvertStrToJson(responseText);
                                accessToken = jObject.Get<string>("access_token");
                                if (string.IsNullOrWhiteSpace(accessToken))
                                {
                                    throw new Exception("令牌无效！");
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(accessToken))
                    {
                        string authorization = "Bearer " + accessToken;
                        client.DefaultRequestHeaders.Add("Authorization", authorization);
                        using (HttpResponseMessage responseMessage = client.PostAsync(urlApi, formDataContent).Result)
                        {
#if !DEBUG
                            responseMessage.EnsureSuccessStatusCode();
#endif
                            responseText = responseMessage.Content.ReadAsStringAsync().Result;
                        }
                    }
                }
            }
            return responseText;
        }
        #endregion
    }
}
