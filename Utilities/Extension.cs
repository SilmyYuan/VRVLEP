using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Linq;

namespace VRVLEP.Utilities
{    /// <summary>
     /// 扩展类
     ///     用于扩展部分特殊功能 如序列化、时间字符串格式化等
     /// </summary>
    public static class Extension
    {
        /// <summary>
        /// 格式化为规定时间的yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToFormat(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        #region  Json


        /// <summary>
        /// 任何对象转换json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        /// <summary>
        /// 处理JSON的特殊字符
        /// </summary>
        /// <param name="strJson"></param>
        /// <returns></returns>
        public static string JsonCharacter(string strJson)
        {
            if (string.IsNullOrEmpty(strJson))
            {
                return string.Empty;
            }
            strJson = strJson.Replace("\\", "\\\\");
            strJson = strJson.Replace("\"", "\\\"");
            strJson = strJson.Replace("/", "\\/");
            strJson = strJson.Replace("\b", "\\b");
            strJson = strJson.Replace("\f", "\\f");
            strJson = strJson.Replace("\n", "\\n");
            strJson = strJson.Replace("\r", "\\r");
            strJson = strJson.Replace("\t", "\\t");
            return strJson;
        }

        #endregion

        #region 字符串分隔

        /// <summary>
        /// 逗号分隔
        /// </summary>
        /// <returns></returns>
        public static List<string> SplitComma(this string s)
        {
            if (String.IsNullOrEmpty(s))
                return new List<string>();
            if (!s.Contains(","))
                return new List<string>() { s };
            return s.Split(',').ToList();

        }

        #endregion

        #region 类型转换
        public static int ToInt(this object o)
        {
            try
            {
                return Convert.ToInt32(o);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public static DateTime? ToDateTime(this object o)
        {
            try
            {
                return Convert.ToDateTime(0);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static decimal ToDecimal(this object o)
        {
            try
            {
                return Convert.ToDecimal(o);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public static double ToDouble(this object o)
        {
            try
            {
                return Convert.ToDouble(o);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }



        /// <summary>
        /// 返回结果,只保存两位小数
        /// </summary>
        /// <returns></returns>
        public static string ToRate(this object value)
        {
            try
            {
                return String.Format("{0:N2}", double.Parse(value.ToString()));
            }
            catch (Exception ex)
            {
                return "0.00";
            }
        }
        #endregion

        /// <summary>
        /// 将json对象反序列化为list对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strJson"></param>
        /// <returns></returns>
        public static T JsonToModel<T>(this string strJson)
        {
            T objList = JsonConvert.DeserializeObject<T>(strJson);
            return objList;
        }
    }
}
