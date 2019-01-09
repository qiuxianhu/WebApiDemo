using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ValueProviders;

namespace WebApiCommon.Provider
{
    /// <summary>
    /// 获取POST方式提交的数据提取器
    /// </summary>
    public class PostDataValueProvider : IValueProvider
    {
        internal const string FORM_KEY = "__FORM_DATA__";
        private Dictionary<string, string> _dict;
        public PostDataValueProvider(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }
            this._dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            HttpRequestMessage request = actionContext.Request;
            if (request.Method == HttpMethod.Post)
            {
                this._dict[FORM_KEY] = request.Content.ReadAsStringAsync().Result;
            }
        }

        public bool ContainsPrefix(string prefix)
        {
            return this._dict.Keys.Contains(prefix);
        }

        public ValueProviderResult GetValue(string key)
        {
            string value;
            if (this._dict.TryGetValue(key, out value))
            {
                return new ValueProviderResult(value, value, CultureInfo.InvariantCulture);
            }
            return null;
        }
    }
}
