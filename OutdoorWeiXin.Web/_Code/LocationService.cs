using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.BaiduMap;
using Senparc.Weixin.MP.Entities.GoogleMap;
using Senparc.Weixin.MP.Helpers;

namespace OutdoorWeiXin.Web._Code
{
    public class LocationService
    {
        public ResponseMessageNews GetResponseMessage(RequestMessageLocation requestMessage)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNews>(requestMessage);

            //var markersList = new List<GoogleMapMarkers>();
            //markersList.Add(new GoogleMapMarkers()
            //{
            //    X = requestMessage.Location_X,
            //    Y = requestMessage.Location_Y,
            //    Color = "red",
            //    Label = "S",
            //    Size = GoogleMapMarkerSize.Default,
            //});
            //var mapSize = "480x600";
            //var mapUrl = GoogleMapHelper.GetGoogleStaticMap(19 /*requestMessage.Scale*//*微信和GoogleMap的Scale不一致，这里建议使用固定值*/,
            //                                                markersList, mapSize);
            var markersList = new List<BaiduMarkers>();
            markersList.Add(new BaiduMarkers()
            {
                Latitude = requestMessage.Location_X,
                Longitude = requestMessage.Location_Y,
                Color = "red",
                Label = "S",
                Size = BaiduMarkerSize.Default,
            });
            var mapUrl = BaiduMapHelper.GetBaiduStaticMap(requestMessage.Location_Y, requestMessage.Location_X, 1, 11, markersList);
            responseMessage.Articles.Add(new Article()
            {
                Description = string.Format("您刚才发送了地理位置信息。Location_X：{0}，Location_Y：{1}，Scale：{2}，标签：{3}",
                              requestMessage.Location_X, requestMessage.Location_Y,
                              requestMessage.Scale, requestMessage.Label),
                PicUrl = mapUrl,
                Title = "定位地点周边地图",
                Url = mapUrl
            });
            //responseMessage.Articles.Add(new Article()
            //{
            //    Title = "RadioBuy公众平台 官网链接",
            //    Description = "RadioBuy 官网地址",
            //    PicUrl = "http://weixin.radiobuy.cn/images/logo.jpg",
            //    Url = "http://weixin.radiobuy.cn"
            //});

            return responseMessage;
        }
    }
}