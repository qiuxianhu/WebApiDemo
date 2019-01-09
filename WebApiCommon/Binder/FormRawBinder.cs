using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Http.ValueProviders;
using WebApiCommon.Provider;

namespace WebApiCommon.Binder
{
    /// <summary> 
    /// 针对POST方式发送raw格式的数据，服务器端获取请求信息的绑定处理类型
    /// </summary> 
    /// <typeparam name="T"></typeparam> 
    public class FormRawBinder<T> : System.Web.Http.ModelBinding.IModelBinder
    {
        private const RegexOptions OPTIONS = RegexOptions.IgnoreCase | RegexOptions.Compiled;
        public const string PARAMETER_KEY_VALUE = "([^=&]+)=([^&]*)";
        private static readonly Regex _RegexParameterKeyValue = new Regex(PARAMETER_KEY_VALUE, OPTIONS);

        public bool BindModel(System.Web.Http.Controllers.HttpActionContext actionContext, System.Web.Http.ModelBinding.ModelBindingContext bindingContext)
        {
            Type type = typeof(T);
            if (bindingContext.ModelType != type)
            {
                return false;
            }
            IValueProvider valueProvider = bindingContext.ValueProvider;
            ValueProviderResult valueProviderResult = valueProvider.GetValue(PostDataValueProvider.FORM_KEY);//先取POST方式发送过来的参数
            if (valueProviderResult == null)
            {
                //从请求URL或Cookie或UrlRoute里面获取提交的参数数据，ModelName名称为当前方法里面形参的名称
                valueProviderResult = valueProvider.GetValue(bindingContext.ModelName);
            }
            if (valueProviderResult == null)
            {
                return false;
            }
            //从请求中获取提交的参数数据
            object rawValue = valueProviderResult.RawValue;
            if (rawValue == null)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Wrong value type");
                return false;
            }
            string @string = rawValue as string;
            if (@string != null)
            {
                object @object = null;
                @string = Assistant.UnBoxing(@string);//拆箱。
                if (@string.StartsWith("{") && @string.EndsWith("}"))
                {
                    //提交参数是对象
                    @object = new JsonSerializer().Deserialize(JObject.Parse(@string).CreateReader(), type);
                }
                else if (@string.StartsWith("[") && @string.EndsWith("]"))
                {
                    //提交参数是数组
                    List<T> list = new List<T>();
                    JArray jArray = JArray.Parse(@string);
                    if (jArray != null)
                    {
                        int count = jArray.Count;
                        for (int i = 0; i < count; i++)
                        {
                            list.Add((T)new JsonSerializer().Deserialize(jArray[i].CreateReader(), type));
                        }
                    }
                    @object = list;
                }
                else if (_RegexParameterKeyValue.IsMatch(@string))
                {
                    //提交参数是Query格式
                    @object = System.Web.HttpUtility.ParseQueryString(@string);
                }
                else
                {
                    //提交参数是普通字符串格式
                    @object = @string;
                }
                bindingContext.Model = @object;
                return true;
            }
            bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Cannot convert value to Location");
            return false;
        }

    }
}
