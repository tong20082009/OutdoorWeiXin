using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Senparc.Weixin.MP.AdvancedAPIs;
using System.Configuration;
using Senparc.Weixin;
using Senparc.Weixin.Exceptions;

namespace OutdoorWeiXin.Web.Controllers
{
    public class LoginController : BaseController
    {
        private string appId = ConfigurationManager.AppSettings["AppId"];
        private string secret = ConfigurationManager.AppSettings["AppSecret"];

        public ActionResult Index()
        {
            if (CurrentUserID == 0)
            {
                //此页面引导用户点击授权
                return Redirect(OAuth.GetAuthorizeUrl(appId, ViewBag.WeiXinUrl + "Login/UserInfoCallback", "RadioBuy", OAuthScope.snsapi_userinfo));
            }
            else
            {
                return Content("该微信号已绑定户外监测账户！");
            }
        }
        public ActionResult UserInfoCallback(string code, string state)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Content("您拒绝了授权！");
            }

            if (state != "RadioBuy")
            {
                return Content("验证失败！请从正规途径进入！");
            }

            //通过，用code换取access_token
            var result = OAuth.GetAccessToken(appId, secret, code);
            if (result.errcode != ReturnCode.请求成功)
            {
                return Content("错误：" + result.errmsg);
            }

            //因为第一步选择的是OAuthScope.snsapi_userinfo，这里可以进一步获取用户详细信息
            try
            {
                var userInfo = OAuth.GetUserInfo(result.access_token, result.openid);
                Model.User user = new Model.User()
                {
                    PassWord = "radiobuy",
                    NickName = userInfo.nickname,
                    Sex = userInfo.sex,
                    City = userInfo.city,
                    OpenId = userInfo.openid
                };
                if (Model.User.GetUserIdByOpenId(user.OpenId) == 0)
                {
                    user.UserId = Model.User.AddUser(user);
                }
                if (user.UserId > 0)
                {
                    RecordUserCookies(user);
                    ViewBag.SuccessMessage = "您已成功绑定户外监测会员!";
                    return View("BindSuccess");
                }
                else
                {
                    ViewBag.ErrorMessage = "纳尼？出错啦！";
                    return View("Error");
                }
            }
            catch (ErrorJsonResultException ex)
            {
                return Content(ex.Message);
            }
        }
    }
}
