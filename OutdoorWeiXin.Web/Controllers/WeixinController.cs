using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Senparc.Weixin.MP.MvcExtension;
using Senparc.Weixin.MP;
using OutdoorWeiXin.Web._Code;
using AutoRadio.RadioSmart.Common;
using System.Configuration;
using Senparc.Weixin.MP.Entities.Request;

namespace OutdoorWeiXin.Web.Controllers
{
    public class WeixinController : Controller
    {
        public static readonly string Token = ConfigurationManager.AppSettings["Token"];//与微信公众账号后台的Token设置保持一致，区分大小写。

        public WeixinController()
        {

        }

        /// <summary>
        /// 微信后台验证地址（使用Get），微信后台的“接口配置信息”的Url填写如：http://weixin.radiobuy.cn/weixin
        /// </summary>
        [HttpGet]
        [ActionName("Index")]
        public ActionResult Get(string signature, string timestamp, string nonce, string echostr)
        {
            if (CheckSignature.Check(signature, timestamp, nonce, Token))
            {
                Loger.Current.Write("WeixinController.Get() CheckSignature.Check=true");
                return Content(echostr); //返回随机字符串则表示验证通过
            }
            else
            {
                Loger.Current.Write("WeixinController.Get() CheckSignature.Check=false");
                return Content("failed:" + signature + "," + CheckSignature.GetSignature(timestamp, nonce, Token) + "。" +
                    "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
            }
        }

        /// <summary>
        /// 用户发送消息后，微信平台自动Post一个请求到这里，并等待响应XML。
        /// PS：此方法为简化方法，效果与OldPost一致。
        /// v0.8之后的版本可以结合Senparc.Weixin.MP.MvcExtension扩展包，使用WeixinResult，见MiniPost方法。
        /// </summary>
        [HttpPost]
        [ActionName("Index")]
        public ActionResult Post(PostModel postModel)
        {
            //Loger.Current.Write("WeixinController.Post() begin");
            try
            {
                if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
                {
                    return Content("参数错误！");
                }

                postModel.Token = Token;
                postModel.EncodingAESKey = ConfigurationManager.AppSettings["EncodingAESKey"];//根据自己后台的设置保持一致
                postModel.AppId = ConfigurationManager.AppSettings["AppId"];//根据自己后台的设置保持一致

                //v4.2.2之后的版本，可以设置每个人上下文消息储存的最大数量，防止内存占用过多，如果该参数小于等于0，则不限制
                var maxRecordCount = 10;

                //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
                var messageHandler = new CustomMessageHandler(Request.InputStream, postModel, maxRecordCount);

                try
                {
                    //测试时可开启此记录，帮助跟踪数据，使用前请确保App_Data文件夹存在，且有读写权限。
                    //Loger.Current.Write("WeixinController.Post() RequestDocument=\r\n" + messageHandler.RequestDocument.ToString());
                    if (messageHandler.UsingEcryptMessage)
                    {
                        //Loger.Current.Write("WeixinController.Post() EcryptRequestDocument=\r\n" + messageHandler.EcryptRequestDocument.ToString());
                    }

                    /* 如果需要添加消息去重功能，只需打开OmitRepeatedMessage功能，SDK会自动处理。
                     * 收到重复消息通常是因为微信服务器没有及时收到响应，会持续发送2-5条不等的相同内容的RequestMessage*/
                    messageHandler.OmitRepeatedMessage = true;

                    //执行微信处理过程
                    messageHandler.Execute();

                    //测试时可开启，帮助跟踪数据
                    //Loger.Current.Write("WeixinController.Post() ResponseDocument=\r\n" + messageHandler.ResponseDocument.ToString());
                    if (messageHandler.UsingEcryptMessage)
                    {
                        //记录加密后的响应信息
                        //Loger.Current.Write("WeixinController.Post() FinalResponseDocument=\r\n" + messageHandler.FinalResponseDocument.ToString());
                    }

                    //return Content(messageHandler.ResponseDocument.ToString());//v0.7-
                    return new FixWeixinBugWeixinResult(messageHandler);//为了解决官方微信5.0软件换行bug暂时添加的方法，平时用下面一个方法即可
                    //return new WeixinResult(messageHandler);//v0.8+
                }
                catch (Exception ex)
                {
                    Loger.Current.Write("WeixinController.Post() err=" + ex.Message);
                    if (messageHandler.ResponseDocument != null)
                    {
                        //Loger.Current.Write("WeixinController.Post() ResponseDocument=\r\n" + messageHandler.ResponseDocument.ToString());
                    }
                    return Content("");
                }
            }
            catch (Exception ex)
            {
                Loger.Current.Write("WeixinController.Post() err=" + ex.Message);
                return Content("");
            }
        }


        /// <summary>
        /// 最简化的处理流程（不加密）
        /// </summary>
        [HttpPost]
        [ActionName("MiniPost")]
        public ActionResult MiniPost(string signature, string timestamp, string nonce, string echostr)
        {
            Loger.Current.Write("WeixinController.MiniPost() begin");
            try
            {
                if (!CheckSignature.Check(signature, timestamp, nonce, Token))
                {
                    //return Content("参数错误！");//v0.7-
                    return new WeixinResult("参数错误！");//v0.8+
                }

                var messageHandler = new CustomMessageHandler(Request.InputStream, null, 10);

                messageHandler.Execute();//执行微信处理过程

                //return Content(messageHandler.ResponseDocument.ToString());//v0.7-
                return new FixWeixinBugWeixinResult(messageHandler);//v0.8+
                //return new WeixinResult(messageHandler);//v0.8+
            }
            catch (Exception ex)
            {
                Loger.Current.Write("WeixinController.Post() err=" + ex.Message);
                return Content("");
            }
        }

    }
}
