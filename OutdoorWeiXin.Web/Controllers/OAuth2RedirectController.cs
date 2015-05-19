using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using AutoRadio.RadioSmart.Common;
using AutoRadio.RadioSmart.Common.Security;
using Senparc.Weixin;
using Senparc.Weixin.MP.AdvancedAPIs;

namespace OutdoorWeiXin.Web.Controllers
{
    public class OAuth2RedirectController : Controller
    {
        private string appId = ConfigurationManager.AppSettings["AppId"];
        private string secret = ConfigurationManager.AppSettings["AppSecret"];

        public ActionResult Index(string refUrl)
        {
            string authorizeUrl = OAuth.GetAuthorizeUrl(appId, ConfigurationManager.AppSettings["WeiXinUrl"] + "oauth2redirect/BaseCallback", refUrl, OAuthScope.snsapi_base);
            Loger.Current.Write("OAuth2RedirectController.Index() begin authorizeUrl=" + authorizeUrl);
            return Redirect(authorizeUrl);
        }

        public ActionResult BaseCallback(string code, string state)
        {
            Loger.Current.Write("OAuth2RedirectController.BaseCallback() begin code=" + code + ",state=" + state);
            if (!string.IsNullOrEmpty(code))
            {
                var result = OAuth.GetAccessToken(appId, secret, code);
                if (result.errcode == ReturnCode.请求成功)
                {
                    Loger.Current.Write("OAuth2RedirectController.BaseCallback() begin openid=" + result.openid);
                    //openid存到cookie
                    var sOpenId = new HttpCookie("openid")
                    {
                        //Value = DESEncryptor.Encrypt(result.openid),
                        Value = result.openid,
                        Domain = defaultDomain,
                        Expires = DateTime.MinValue
                    };
                    Response.Cookies.Add(sOpenId);
                    // 根据OpenId找MemberId存到cookie
                    var uid = Model.User.GetUserIdByOpenId(result.openid);
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
            Loger.Current.Write("OAuth2RedirectController.BaseCallback() end");
            return Redirect(state);
        }

        /// <summary>
        /// 登录后写cookie的damain
        /// </summary>
        protected string defaultDomain
        {
            get
            {
                return ConfigurationManager.AppSettings["logindomain"];
            }
        }
    }
}
