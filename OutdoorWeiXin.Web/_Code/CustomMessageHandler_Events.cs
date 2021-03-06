﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Senparc.Weixin.Context;
using Senparc.Weixin.MP.Agent;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Helpers;
using Senparc.Weixin.MP.MessageHandlers;
using AutoRadio.RadioSmart.Common.Security;
using Senparc.Weixin.MP.AdvancedAPIs;
using AutoRadio.RadioSmart.Common;
using System.Configuration;
using Senparc.Weixin.MP.CommonAPIs;

namespace OutdoorWeiXin.Web._Code
{
    /// <summary>
    /// 自定义MessageHandler
    /// </summary>
    public partial class CustomMessageHandler
    {
        private string appId = ConfigurationManager.AppSettings["AppId"];
        private string secret = ConfigurationManager.AppSettings["AppSecret"];
        //public override IResponseMessageBase OnTextOrEventRequest(RequestMessageText requestMessage)
        //{
        //    // 预处理文字或事件类型请求。
        //    // 这个请求是一个比较特殊的请求，通常用于统一处理来自文字或菜单按钮的同一个执行逻辑，
        //    // 会在执行OnTextRequest或OnEventRequest之前触发，具有以下一些特征：
        //    // 1、如果返回null，则继续执行OnTextRequest或OnEventRequest
        //    // 2、如果返回不为null，则终止执行OnTextRequest或OnEventRequest，返回最终ResponseMessage
        //    // 3、如果是事件，则会将RequestMessageEvent自动转为RequestMessageText类型，其中RequestMessageText.Content就是RequestMessageEvent.EventKey

        //    if (requestMessage.Content == "OneClick")
        //    {
        //        var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
        //        strongResponseMessage.Content = "您点击了底部按钮。\r\n为了测试微信软件换行bug的应对措施，这里做了一个——\r\n换行";
        //        return strongResponseMessage;
        //    }
        //    return null;//返回null，则继续执行OnTextRequest或OnEventRequest
        //}

        //public override IResponseMessageBase OnEvent_ClickRequest(RequestMessageEvent_Click requestMessage)
        //{
        //    IResponseMessageBase reponseMessage = null;
        //    //菜单点击，需要跟创建菜单时的Key匹配
        //    switch (requestMessage.EventKey)
        //    {
        //        case "OneClick":
        //            {
        //                //这个过程实际已经在OnTextOrEventRequest中完成，这里不会执行到。
        //                var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
        //                reponseMessage = strongResponseMessage;
        //                strongResponseMessage.Content = "您点击了底部按钮。\r\n为了测试微信软件换行bug的应对措施，这里做了一个——\r\n换行";
        //            }
        //            break;
        //        case "SubClickRoot_Text":
        //            {
        //                var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
        //                reponseMessage = strongResponseMessage;
        //                strongResponseMessage.Content = "您点击了子菜单按钮。";
        //            }
        //            break;
        //        case "SubClickRoot_News":
        //            {
        //                var strongResponseMessage = CreateResponseMessage<ResponseMessageNews>();
        //                reponseMessage = strongResponseMessage;
        //                strongResponseMessage.Articles.Add(new Article()
        //                {
        //                    Title = "您点击了子菜单图文按钮",
        //                    Description = "您点击了子菜单图文按钮，这是一条图文信息。",
        //                    PicUrl = "http://weixin.radiobuy.cn/Images/qrcode.jpg",
        //                    Url = "http://weixin.radiobuy.cn"
        //                });
        //            }
        //            break;
        //        case "SubClickRoot_Music":
        //            {
        //                var strongResponseMessage = CreateResponseMessage<ResponseMessageMusic>();
        //                reponseMessage = strongResponseMessage;
        //                strongResponseMessage.Music.MusicUrl = "http://weixin.radiobuy.cn/Content/music1.mp3";
        //            }
        //            break;
        //        case "SubClickRoot_Image":
        //            {
        //                var strongResponseMessage = CreateResponseMessage<ResponseMessageImage>();
        //                reponseMessage = strongResponseMessage;
        //                strongResponseMessage.Image.MediaId = "Mj0WUTZeeG9yuBKhGP7iR5n1xUJO9IpTjGNC4buMuswfEOmk6QSIRb_i98do5nwo";
        //            }
        //            break;
        //        case "SubClickRoot_Agent"://代理消息
        //            {
        //                //获取返回的XML
        //                DateTime dt1 = DateTime.Now;
        //                reponseMessage = MessageAgent.RequestResponseMessage(this, agentUrl, agentToken, RequestDocument.ToString());
        //                //上面的方法也可以使用扩展方法：this.RequestResponseMessage(this,agentUrl, agentToken, RequestDocument.ToString());

        //                DateTime dt2 = DateTime.Now;

        //                if (reponseMessage is ResponseMessageNews)
        //                {
        //                    (reponseMessage as ResponseMessageNews)
        //                        .Articles[0]
        //                        .Description += string.Format("\r\n\r\n代理过程总耗时：{0}毫秒", (dt2 - dt1).Milliseconds);
        //                }
        //            }
        //            break;
        //        case "Member"://托管代理会员信息
        //            {
        //                //原始方法为：MessageAgent.RequestXml(this,agentUrl, agentToken, RequestDocument.ToString());//获取返回的XML
        //                reponseMessage = this.RequestResponseMessage(agentUrl, agentToken, RequestDocument.ToString());
        //            }
        //            break;
        //        case "OAuth"://OAuth授权测试
        //            {
        //                var strongResponseMessage = CreateResponseMessage<ResponseMessageNews>();
        //                strongResponseMessage.Articles.Add(new Article()
        //                {
        //                    Title = "OAuth2.0测试",
        //                    Description = "点击【查看全文】进入授权页面。\r\n注意：此页面仅供测试（是专门的一个临时测试账号的授权，并非Senparc.Weixin.MP SDK官方账号，所以如果授权后出现错误页面数正常情况），测试号随时可能过期。请将此DEMO部署到您自己的服务器上，并使用自己的appid和secret。",
        //                    Url = "http://weixin.radiobuy.cn/oauth2",
        //                    PicUrl = "http://weixin.radiobuy.cn/Images/qrcode.jpg"
        //                });
        //                reponseMessage = strongResponseMessage;
        //            }
        //            break;
        //        case "Description":
        //            {
        //                var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
        //                strongResponseMessage.Content = "";
        //                reponseMessage = strongResponseMessage;
        //            }
        //            break;
        //        default:
        //            {
        //                var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
        //                strongResponseMessage.Content = "您点击了按钮，EventKey：" + requestMessage.EventKey;
        //                reponseMessage = strongResponseMessage;
        //            }
        //            break;
        //    }

        //    return reponseMessage;
        //}

        //public override IResponseMessageBase OnEvent_EnterRequest(RequestMessageEvent_Enter requestMessage)
        //{
        //    var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
        //    responseMessage.Content = "您刚才发送了ENTER事件请求。";
        //    return responseMessage;
        //}

        public override IResponseMessageBase OnEvent_LocationRequest(RequestMessageEvent_Location requestMessage)
        {
            Loger.Current.Write("OnEvent_LocationRequest begin");
            if (!string.IsNullOrEmpty(requestMessage.Latitude.ToString()) && !string.IsNullOrEmpty(requestMessage.Longitude.ToString()))
            {
                Model.User user = Model.User.UserDataBind(requestMessage.FromUserName);
                if (user != null)
                {
                    var baiduModel = OutdoorWeiXin.Web._Code.WeiXinHelper.GeoCoder(requestMessage.Latitude.ToString(), requestMessage.Longitude.ToString());
                    Loger.Current.Write("OnEvent_LocationRequest NickName=" + user.NickName + " Latitude=" + requestMessage.Latitude + " Longitude" + requestMessage.Longitude);
                    if (baiduModel.Status == 0)
                    {
                        Loger.Current.Write("OnEvent_LocationRequest City=" + baiduModel.Result.AddressComponent.City.TrimEnd('市') + " District=" + baiduModel.Result.AddressComponent.District);
                        Model.User.UpdateCityByOpenId(user.OpenId, Model.User.GetRegionIdByRegionName(baiduModel.Result.AddressComponent.City.TrimEnd('市')), baiduModel.Result.AddressComponent.District);
                    }
                }
            }
            Loger.Current.Write("OnEvent_LocationRequest end");
            //这里是微信客户端（通过微信服务器）自动发送过来的位置信息
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "";
            return responseMessage;//这里也可以返回null（需要注意写日志时候null的问题）
        }

        public override IResponseMessageBase OnEvent_ScanRequest(RequestMessageEvent_Scan requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            return responseMessage;
        }

        /// <summary>
        /// 生成随机数
        /// </summary>
        /// <param name="Length"></param>
        /// <returns></returns>
        public string GenerateRandomNumber(int Length)
        {
            char[] constant = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            System.Text.StringBuilder newRandom = new System.Text.StringBuilder(62);
            Random rd = new Random();
            for (int i = 0; i < Length; i++)
            {
                newRandom.Append(constant[rd.Next(10)]);
            }
            return newRandom.ToString();
        }

        public string GetToken()
        {
            return GetTokenByAppId(appId, secret);
        }

        public string GetTokenByAppId(string appId, string appSecret)
        {
            try
            {
                if (!AccessTokenContainer.CheckRegistered(appId))
                {
                    AccessTokenContainer.Register(appId, appSecret);
                }
                var result = AccessTokenContainer.GetTokenResult(appId); //CommonAPIs.CommonApi.GetToken(appId, appSecret);
                //也可以直接一步到位：
                //var result = AccessTokenContainer.TryGetToken(appId, appSecret);
                Loger.Current.Write("WeiXinService.GetToken() accessToken=" + result.access_token);
                return result.access_token;
            }
            catch (Exception)
            {
                return "error";
            }
        }
        //public override IResponseMessageBase OnEvent_ViewRequest(RequestMessageEvent_View requestMessage)
        //{
        //    //说明：这条消息只作为接收，下面的responseMessage到达不了客户端，类似OnEvent_UnsubscribeRequest
        //    var responseMessage = CreateResponseMessage<ResponseMessageText>();
        //    responseMessage.Content = "您点击了view按钮，将打开网页：" + requestMessage.EventKey;
        //    return responseMessage;
        //}

        //public override IResponseMessageBase OnEvent_MassSendJobFinishRequest(RequestMessageEvent_MassSendJobFinish requestMessage)
        //{
        //    var responseMessage = CreateResponseMessage<ResponseMessageText>();
        //    responseMessage.Content = "接收到了群发完成的信息。";
        //    return responseMessage;
        //}

        /// <summary>
        /// 订阅（关注）事件
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)
        {
            var weiXinUrl = System.Configuration.ConfigurationManager.AppSettings["WeiXinUrl"];
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            if (Model.User.GetUserIdByOpenId(requestMessage.FromUserName) > 0)
            {
                responseMessage.Content = @"您已经是户外监测用户，可以去领取任务。";
            }
            else
            {
                responseMessage.Content = @"请先绑定户外监测账户，绑定成功后即可领取任务。<a href='" + weiXinUrl + "Login/Index'>点此立即绑定。</a>";
            }
            return responseMessage;
        }

        /// <summary>
        /// 退订
        /// 实际上用户无法收到非订阅账号的消息，所以这里可以随便写。
        /// unsubscribe事件的意义在于及时删除网站应用中已经记录的OpenID绑定，消除冗余数据。并且关注用户流失的情况。
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_UnsubscribeRequest(RequestMessageEvent_Unsubscribe requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "有空再来";
            return responseMessage;
        }

        /// <summary>
        /// 事件之扫码推事件(scancode_push)
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_ScancodePushRequest(RequestMessageEvent_Scancode_Push requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之扫码推事件";
            return responseMessage;
        }

        /// <summary>
        /// 事件之扫码推事件且弹出“消息接收中”提示框(scancode_waitmsg)
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_ScancodeWaitmsgRequest(RequestMessageEvent_Scancode_Waitmsg requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之扫码推事件且弹出“消息接收中”提示框";
            return responseMessage;
        }

        ///// <summary>
        ///// 事件之弹出拍照或者相册发图（pic_photo_or_album）
        ///// </summary>
        ///// <param name="requestMessage"></param>
        ///// <returns></returns>
        //public override IResponseMessageBase OnEvent_PicPhotoOrAlbumRequest(RequestMessageEvent_Pic_Photo_Or_Album requestMessage)
        //{
        //    var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
        //    responseMessage.Content = "事件之弹出拍照或者相册发图";
        //    return responseMessage;
        //}

        /// <summary>
        /// 事件之弹出系统拍照发图(pic_sysphoto)
        /// 实际测试时发现微信并没有推送RequestMessageEvent_Pic_Sysphoto消息，只能接收到用户在微信中发送的图片消息。
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        //public override IResponseMessageBase OnEvent_PicSysphotoRequest(RequestMessageEvent_Pic_Sysphoto requestMessage)
        //{
        //    var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
        //    responseMessage.Content = "事件之弹出系统拍照发图";
        //    return responseMessage;
        //}

        /// <summary>
        /// 事件之弹出微信相册发图器(pic_weixin)
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        //public override IResponseMessageBase OnEvent_PicWeixinRequest(RequestMessageEvent_Pic_Weixin requestMessage)
        //{
        //    var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
        //    responseMessage.Content = "事件之弹出微信相册发图器";
        //    return responseMessage;
        //}

        /// <summary>
        /// 事件之弹出地理位置选择器（location_select）
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        //public override IResponseMessageBase OnEvent_LocationSelectRequest(RequestMessageEvent_Location_Select requestMessage)
        //{
        //    var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
        //    responseMessage.Content = "事件之弹出地理位置选择器";
        //    return responseMessage;
        //}
    }
}