using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using AutoRadio.RadioSmart.Common.Security;
using System.Xml;
using AutoRadio.RadioSmart.Common;

namespace OutdoorWeiXin.Web
{
    public class BaseController : Controller
    {
        /// <summary>
        /// 方法过滤器，在执行方法之前设置默认数据，视图直接调用
        /// zhous,2014.02.22
        /// </summary>
        /// <param name="filterContext">上下文</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.WebRootCss = System.Configuration.ConfigurationManager.AppSettings["WebRootCss"];
            ViewBag.AppVersion = System.Configuration.ConfigurationManager.AppSettings["AppVersion"];
            ViewBag.DefaultPwd = System.Configuration.ConfigurationManager.AppSettings["DefaultPwd"];
            ViewBag.ErrorMessage = "纳尼！出错啦？";
            HttpContextBase httpContext = filterContext.HttpContext;
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }
            var IsVerificationOpenid = System.Configuration.ConfigurationManager.AppSettings["IsVerificationOpenid"].ToString();
            if (IsVerificationOpenid == "true")
            {
                var openid = HttpContext.Request.Cookies["openid"];
                if (openid == null || openid.Value == null)
                {
                    httpContext.Response.Redirect(ViewBag.WeiXinUrl + "oauth2redirect/Index" + "?refUrl=" + httpContext.Server.UrlEncode(httpContext.Request.Url.ToString()));
                    //阻止继续执行Action
                    filterContext.Result = new HttpUnauthorizedResult();
                    return;
                }
                else
                {
                    var uid = Model.User.GetUserIdByOpenId(openid.Value);
                    if (uid > 0)
                    {
                        if (HttpContext.Request.Cookies["uid"] == null)
                        {
                            var userMemberId = new HttpCookie("uid")
                                                   {
                                                       Value = DESEncryptor.Encrypt(uid.ToString()),
                                                       Domain = defaultDomain,
                                                       Expires = DateTime.MinValue
                                                   };
                            Response.Cookies.Add(userMemberId);
                        }
                    }
                    else
                    {
                        var httpCookie = Request.Cookies["uid"];
                        if (httpCookie != null)
                        {
                            httpCookie.Value = string.Empty;
                            httpCookie.Expires = DateTime.Now.AddDays(-1);
                        }
                    }
                }
            }
            else
            {
                var openid = HttpContext.Request.Cookies["openid"];
                if (openid == null || openid.Value == null)
                {
                    var sOpenId = new HttpCookie("openid")
                  {
                      Value = "oTrd0jrCQHrPSMn7YG69uoI7YcWo",
                      Domain = defaultDomain,
                      Expires = DateTime.MinValue
                  };
                    Response.Cookies.Add(sOpenId);
                }
                openid = HttpContext.Request.Cookies["openid"];
                var uid = Model.User.GetUserIdByOpenId(openid.Value);
                if (uid > 0)
                {
                    var userMemberId = new HttpCookie("uid")
                    {
                        Value = DESEncryptor.Encrypt(uid.ToString()),
                        Domain = defaultDomain,
                        Expires = DateTime.MinValue
                    };
                    Response.Cookies.Add(userMemberId);
                }
            }
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
        }

        /// <summary>
        /// 当前用户
        /// </summary>
        protected int CurrentUserID
        {
            get
            {
                var reu = HttpContext.Request.Cookies["uid"];
                var uid = 0;
                if (reu != null)
                {
                    uid = ConvertHelper.GetInteger(DESEncryptor.Decrypt(reu.Value));
                }
                return uid;
            }
        }

        /// <summary>
        /// 记录用户登录信息
        /// </summary>
        /// <param name="model">用户登录返回信息类</param>
        protected void RecordUserCookies(Model.User user)
        {
            var userMemberId = new HttpCookie("uid")
            {
                Value = DESEncryptor.Encrypt(user.UserId.ToString()),
                Domain = defaultDomain,
                Expires = DateTime.MinValue
            };
            Response.Cookies.Add(userMemberId);
        }

        /// <summary>
        /// 登录后写cookie的damain
        /// </summary>
        protected string defaultDomain
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["logindomain"];
            }
        }

        /// <summary>
        /// 获取短信提醒模版内容
        /// </summary>
        /// <param name="xmlPath">XML模版文件路径</param>
        /// <param name="nodeName">节点名称</param>
        /// <returns></returns>
        public string GetSmsContent(string xmlPath, string nodeName)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);
            var smsNode = xmlDoc.SelectSingleNode("/sms/" + nodeName);
            if (smsNode == null)
            {
                return string.Empty;
            }
            else
            {
                return smsNode.InnerText;
            }
        }
    }
}