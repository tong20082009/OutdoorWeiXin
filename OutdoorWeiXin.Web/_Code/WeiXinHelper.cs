using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using OutdoorWeiXin.Web.Cache;
using Newtonsoft.Json;
using System.Configuration;
using System.Globalization;

namespace OutdoorWeiXin.Web._Code
{
    public class WeiXinHelper
    {
        #region Get请求的简单封装

        /// <summary>发起GET请求</summary>
        /// <param name="url">请求URL</param>
        /// <param name="errmsg">错误信息</param>
        /// <param name="parameters">请求参数</param>
        /// <returns></returns>
        public static string Get(string url, out string errmsg, Dictionary<string, object> parameters)
        {
            errmsg = null;
            var strUrl = new StringBuilder(url);
            if (parameters != null && parameters.Count > 0)
            {
                //拼接参数
                strUrl.Append("?");
                foreach (var keyValuePair in parameters)
                {
                    strUrl.AppendFormat("{0}={1}&",
                        HttpUtility.UrlEncode(keyValuePair.Key, Encoding.UTF8),
                        HttpUtility.UrlEncode(keyValuePair.Value.ToString(), Encoding.UTF8));
                }
                strUrl.Remove(strUrl.Length - 1, 1); //移除最后一位多出的“&”
            }
            var request = (HttpWebRequest)WebRequest.Create(strUrl.ToString());
            request.Method = "GET";
            request.Timeout = 10000;
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        var reader = new StreamReader(stream);
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                errmsg = "请求异常：" + ex.Message;
            }
            return null;
        }

        #endregion

        #region SHA1加密算法

        /// <summary>
        ///     SHA1加密算法
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <returns></returns>
        public static string GetSha1Str(string str)
        {
            byte[] strRes = Encoding.UTF8.GetBytes(str);
            HashAlgorithm iSha = new SHA1CryptoServiceProvider();
            strRes = iSha.ComputeHash(strRes);
            var enText = new StringBuilder();
            foreach (byte iByte in strRes)
            {
                enText.AppendFormat("{0:x2}", iByte);
            }
            return enText.ToString();
        }

        #endregion

        public static string token = string.Empty;

        /// <summary>
        ///     获取调用JS SDK时所需的access_token
        ///     文档地址：http://mp.weixin.qq.com/wiki/15/54ce45d8d30b6bf6758f68d2e95bc627.html
        /// </summary>
        /// <returns></returns>
        public static string GetAccessToken()
        {
            var cache = new AspNetCache();
            if (cache.Get<string>("access_token") == null)
            {
                string errmsg;
                string appid = ConfigurationManager.AppSettings["AppId"];
                string appsecret = ConfigurationManager.AppSettings["AppSecret"];
                string apiUrl =
                    string.Format(
                        "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}",
                        appid, appsecret);
                string responseStr = Get(apiUrl, out errmsg, null);
                if (responseStr != null)
                {
                    //var dic = responseStr.JSONDeserialize<Dictionary<string, object>>();
                    var dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseStr);
                    if (dic.ContainsKey("access_token"))
                    {
                        cache.Add("access_token", dic["access_token"].ToString());
                        token = dic["access_token"].ToString();
                        return dic["access_token"].ToString();
                    }
                }
            }
            else
            {
                return cache.Get<string>("access_token");
            }

            return null;
        }

        /// <summary>
        ///     获取调用JS SDK时所需的票据
        ///     文档地址：http://mp.weixin.qq.com/wiki/7/aaa137b55fb2e0456bf8dd9148dd613f.html#.E9.99.84.E5.BD.951-JS-SDK.E4.BD.BF.E7.94.A8.E6.9D.83.E9.99.90.E7.AD.BE.E5.90.8D.E7.AE.97.E6.B3.95
        /// </summary>
        /// <returns></returns>
        public static string GetJsApiTicket()
        {
            var cache = new AspNetCache();
            if (cache.Get<string>("ticket") == null)
            {
                string errmsg;
                string apiUrl =
                    string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi",
                        GetAccessToken());
                string responseStr = Get(apiUrl, out errmsg, null);
                if (responseStr != null)
                {
                    //var dic = responseStr.JSONDeserialize<Dictionary<string, object>>();
                    var dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseStr);
                    if (dic.ContainsKey("ticket"))
                    {
                        cache.Add("ticket", dic["ticket"].ToString());
                        return dic["ticket"].ToString();
                    }
                }
            }
            else
            {
                return cache.Get<string>("ticket");
            }

            return null;
        }

        /// <summary>
        /// JS SDK使用权限签名算法
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public static Dictionary<string, string> Sign(string url)
        {
            string nonceStr = Guid.NewGuid().ToString().Replace("-", "");
            string timestamp = ConvertDateTimeInt(DateTime.Now).ToString(CultureInfo.InvariantCulture);
            string jsapiTicket = GetJsApiTicket();
            string str = string.Format("jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}", jsapiTicket, nonceStr,
                timestamp, url);
            string signature = GetSha1Str(str); //SHA1加密
            return new Dictionary<string, string>
            {
                {"url", url},
                {"noncestr", nonceStr},
                {"timestamp", timestamp},
                {"signature", signature}
            };
        }

        /// <summary>
        /// 讲时间转换成数字
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int ConvertDateTimeInt(DateTime time)
        {
            double intResult = 0;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            intResult = (time - startTime).TotalSeconds;
            return (int)intResult;
        }

        #region 根据经纬度 获取地址信息 BaiduApi

        /// <summary>
        /// 根据经纬度  获取 地址信息
        /// </summary>
        /// <param name="lat">纬度</param>
        /// <param name="lng">经度</param>
        /// <returns></returns>
        public static BaiDuGeoCoding GeoCoder(string lat, string lng)
        {
            string errmsg;
            string apiUrl =
                string.Format("http://api.map.baidu.com/geocoder/v2/?ak=D550932d199664a711f9b2d42c9253d1&callback=renderReverse&location={0}&output=json&pois=1",
                    lat + "," + lng);
            string responseStr = Get(apiUrl, out errmsg, null);
            var model = new BaiDuGeoCoding();
            if (!string.IsNullOrEmpty(responseStr))
            {
                responseStr = responseStr.Substring(responseStr.IndexOf('(') + 1);
                model = JsonConvert.DeserializeObject<BaiDuGeoCoding>(responseStr.Trim(')'));
            }
            return model;
        }

        #endregion

        //BaiduGeoCoding是针对Api相应结果封装的对象：
        public class BaiDuGeoCoding
        {
            public int Status { get; set; }
            public Result Result { get; set; }
        }

        public class Result
        {
            public Location Location { get; set; }

            public string Formatted_Address { get; set; }

            public string Business { get; set; }

            public AddressComponent AddressComponent { get; set; }

            public string CityCode { get; set; }
        }

        public class AddressComponent
        {
            /// <summary>
            /// 省份
            /// </summary>
            public string Province { get; set; }
            /// <summary>
            /// 城市名
            /// </summary>
            public string City { get; set; }

            /// <summary>
            /// 区县名
            /// </summary>
            public string District { get; set; }

            /// <summary>
            /// 街道名
            /// </summary>
            public string Street { get; set; }

            public string Street_number { get; set; }

        }

        public class Location
        {
            public string Lat { get; set; }
            public string Lng { get; set; }
        }
    }
}