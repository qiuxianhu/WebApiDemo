using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using Microsoft.Security.Application;

namespace WebApiCommon
{
    public class UtilHelper
    {
        /// <summary>
        /// 构建Url地址栏参数的方法(id=123&name=wyp)
        /// </summary>
        /// <param name="parameters">打开页面请求的参数</param>
        /// <returns>id=123&name=wyp</returns>
        public static string BuildQueryString(NameValueCollection parameters)
        {
            List<string> pairs = new List<string>();
            foreach (string key in parameters.Keys)
            {
                string value = parameters[key];
                if (!string.IsNullOrEmpty(value))
                {
                    pairs.Add(string.Format("{0}={1}", Uri.EscapeDataString(key), Encoder.UrlEncode(value)));
                }
            }
            return string.Join("&", pairs.ToArray());
        }

        /// <summary>
        /// 得到URL中的文件名称（包含扩展名）
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetUrlFileName(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return "";
            }
            string[] @string = url.Split(new char[] { '/' });
            return @string[@string.Length - 1].Split(new char[] { '?' })[0];
        }
        /// <summary>
        /// 将同一种类型的指定对象向目标对象的所有属性赋值
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="source">要复制的对象</param>
        /// <param name="desc">被赋值的对象</param>
        public static void SetPropertyInfo<T>(T source, T desc)
        {
            PropertyInfo[] props = typeof(T).GetProperties();
            if (props != null && props.Length > 0)
            {
                foreach (PropertyInfo info in props)
                {
                    object new_value = info.GetValue(source, null);
                    object old_value = info.GetValue(desc, null);
                    if ((new_value == null && old_value != null)
                        || (new_value != null && old_value == null))
                    {
                        try
                        {
                            info.SetValue(desc, new_value, null);
                        }
                        catch (Exception)
                        { }
                    }
                    else if (new_value != null && old_value != null && !new_value.Equals(old_value))
                    {
                        try
                        {
                            info.SetValue(desc, new_value, null);
                        }
                        catch (Exception)
                        { }
                    }
                }
                Array.Clear(props, 0, props.Length);
                props = null;
            }
        }
        /// <summary>
        /// 将一种类型的对象向另一种类型的对象通过相同属性名称的所有属性进行赋值操作
        /// 只有MYSQL里面的实体类可以使用，因为MYSQL中没有Boolean类型，需要将Boolean专成Int32
        /// </summary>
        /// <typeparam name="S">源类型</typeparam>
        /// <typeparam name="D">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <param name="desc">目标对象</param>
        public static void SetPropertyInfoMySQL<S, D>(S source, D desc)
        {
            PropertyInfo[] propertyInfoSources = typeof(S).GetProperties();
            PropertyInfo[] propertyInfoDescs = typeof(D).GetProperties();
            if (propertyInfoSources != null && propertyInfoSources.Length > 0)
            {
                foreach (PropertyInfo propertyInfoSource in propertyInfoSources)
                {
                    foreach (PropertyInfo propertyInfoDesc in propertyInfoDescs)
                    {
                        if (string.Compare(propertyInfoSource.Name, propertyInfoDesc.Name, true) == 0)
                        {
                            object sourceValue = propertyInfoSource.GetValue(source, null);
                            object descValue = propertyInfoDesc.GetValue(desc, null);
                            if ((sourceValue == null && descValue != null)
                                || (sourceValue != null && descValue == null))
                            {
                                try
                                {
                                    if (sourceValue != null && sourceValue.GetType() == typeof(Boolean))
                                    {
                                        propertyInfoDesc.SetValue(desc, TypeParseHelper.StrToBoolean(sourceValue) ? 1 : 0, null);
                                    }
                                    else
                                    {
                                        propertyInfoDesc.SetValue(desc, sourceValue, null);
                                    }
                                }
                                catch (Exception)
                                { }
                            }
                            else if (sourceValue != null && descValue != null && !sourceValue.Equals(descValue))
                            {
                                try
                                {
                                    if (sourceValue.GetType() == typeof(Boolean))
                                    {
                                        propertyInfoDesc.SetValue(desc, TypeParseHelper.StrToBoolean(sourceValue) ? 1 : 0, null);
                                    }
                                    else
                                    {
                                        propertyInfoDesc.SetValue(desc, sourceValue, null);
                                    }
                                }
                                catch (Exception)
                                { }
                            }
                            break;
                        }
                    }
                }
                Array.Clear(propertyInfoSources, 0, propertyInfoSources.Length);
                propertyInfoSources = null;
                Array.Clear(propertyInfoDescs, 0, propertyInfoDescs.Length);
                propertyInfoDescs = null;
            }
        }

        /// <summary>
        /// 将一种类型的对象向另一种类型的对象通过相同属性名称的所有属性进行赋值操作
        /// 只有SQLServer里面的实体类可以使用，因为SQLServer中有Boolean类型，需要将Int32专成Boolean
        /// </summary>
        /// <typeparam name="S">源类型</typeparam>
        /// <typeparam name="D">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <param name="desc">目标对象</param>
        public static void SetPropertyInfoSQLServer<S, D>(S source, D desc)
        {
            PropertyInfo[] propertyInfoSources = typeof(S).GetProperties();
            PropertyInfo[] propertyInfoDescs = typeof(D).GetProperties();
            if (propertyInfoSources != null && propertyInfoSources.Length > 0)
            {
                foreach (PropertyInfo propertyInfoSource in propertyInfoSources)
                {
                    foreach (PropertyInfo propertyInfoDesc in propertyInfoDescs)
                    {
                        if (string.Compare(propertyInfoSource.Name, propertyInfoDesc.Name, true) == 0)
                        {
                            object sourceValue = propertyInfoSource.GetValue(source, null);
                            object descValue = propertyInfoDesc.GetValue(desc, null);
                            if ((sourceValue == null && descValue != null)
                                || (sourceValue != null && descValue == null))
                            {
                                try
                                {
                                    if (sourceValue.GetType() == typeof(Boolean))
                                    {
                                        propertyInfoDesc.SetValue(desc, TypeParseHelper.StrToInt32(sourceValue) == 1 ? true : false, null);
                                    }
                                    else
                                    {
                                        propertyInfoDesc.SetValue(desc, sourceValue, null);
                                    }
                                }
                                catch (Exception)
                                { }
                            }
                            else if (sourceValue != null && descValue != null && !sourceValue.Equals(descValue))
                            {
                                try
                                {
                                    if (sourceValue.GetType() == typeof(Boolean))
                                    {
                                        propertyInfoDesc.SetValue(desc, TypeParseHelper.StrToInt32(sourceValue) == 1 ? true : false, null);
                                    }
                                    else
                                    {
                                        propertyInfoDesc.SetValue(desc, sourceValue, null);
                                    }
                                }
                                catch (Exception)
                                { }
                            }
                            break;
                        }
                    }
                }
                Array.Clear(propertyInfoSources, 0, propertyInfoSources.Length);
                propertyInfoSources = null;
                Array.Clear(propertyInfoDescs, 0, propertyInfoDescs.Length);
                propertyInfoDescs = null;
            }
        }
        /// <summary>
        /// 转换十六进制数
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ConvertHex(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            System.Text.StringBuilder sb = new System.Text.StringBuilder("");
            char[] src = text.ToCharArray();
            for (int i = 0; i < src.Length; i++)
            {
                byte[] bytes = System.Text.Encoding.Unicode.GetBytes(src[i].ToString());
                sb.Append(string.Format(@"\u{0}{1}", bytes[1].ToString("X2"), bytes[0].ToString("X2")));
            }
            return sb.ToString();
        }
    }
}
