using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using AutoRadio.RadioSmart.Common;
using System.Net;
using System.Web;

namespace OutdoorWeiXin.Model
{
    public class User
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 1男性，2女性，0未知
        /// </summary>
        public int Sex { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 微信关注者Id
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 绑定微信号时间
        /// </summary>
        public DateTime BindWeiXinTime { get; set; }


        /// <summary>
        /// 添加微信关注者用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static int AddUser(User user)
        {
            return new UserDao().AddUser(user);
        }

        /// <summary>
        /// 根据openid获取用户id
        /// </summary>
        /// <param name="OpenId"></param>
        /// <returns></returns>
        public static int GetUserIdByOpenId(string OpenId)
        {
            int userID = userDataBind(OpenId).UserId;
            if (userID > 0)
            {
                return userID;
            }
            return 0;
        }

        /// <summary>
        /// 绑定用户信息
        /// </summary>
        /// <param name="openid">openid</param>
        /// <returns></returns>
        private static User userDataBind(string openid)
        {
            User user = new User();
            DataTable dt = new UserDao().GetUserByOpenId(openid);
            if (dt.Rows.Count > 0)
            {
                user.UserId = ConvertHelper.GetInteger(dt.Rows[0]["UserId"]);
                user.NickName = ConvertHelper.GetString(dt.Rows[0]["NickName"]);
                user.PassWord = ConvertHelper.GetString(dt.Rows[0]["uPassWord"]);
                user.Sex = ConvertHelper.GetInteger(dt.Rows[0]["Sex"]);
                user.City = ConvertHelper.GetString(dt.Rows[0]["City"]);
                user.OpenId = ConvertHelper.GetString(dt.Rows[0]["OpenId"]);
                user.BindWeiXinTime = ConvertHelper.GetDateTime(dt.Rows[0]["BindWeiXinTime"]);
            }
            return user;
        }

        /// <summary>
        /// 查询任务项目列表
        /// </summary>
        /// <param name="SqlWhere"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static DataTable GetTaskProjectList(string SqlWhere, int PageIndex, int PageSize)
        {
            return new UserDao().GetTaskProjectList(SqlWhere, PageIndex, PageSize);
        }

        /// <summary>
        /// 领取任务
        /// </summary>
        /// <param name="TpId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public static int ReceiveTask(int TpId, int UserId)
        {
            return new UserDao().ReceiveTask(TpId, UserId);
        }

        /// <summary>
        /// 获取地区/城市列表
        /// </summary>
        /// <returns></returns>
        public static DataTable GetRegionList()
        {
            return new UserDao().GetRegionList();
        }


        /// <summary>
        /// 下载保存多媒体文件,返回多媒体保存路径
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="ServerId"></param>
        /// <returns></returns>
        public static string GetMultimedia(int UserId,string Token, string MediaId)
        {
            string stUrl = "http://file.api.weixin.qq.com/cgi-bin/media/get?access_token=" + Token + "&media_id=" + MediaId;
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(stUrl);
            string savepath = string.Empty;
            req.Method = "GET";
            using (WebResponse wr = req.GetResponse())
            {
                HttpWebResponse myResponse = (HttpWebResponse)req.GetResponse();
                string  strpath = myResponse.ResponseUri.ToString();
                WebClient mywebclient = new WebClient();
               savepath = HttpContext.Current.Server.MapPath("Image") + "\\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + (new Random()).Next().ToString().Substring(0, 4) + ".jpg";
                try
                {
                    mywebclient.DownloadFile(strpath, savepath);
                }
                catch (Exception ex)
                {
                    savepath = ex.ToString();
                }

            }
            string jsonString = "{savepath:'" + savepath + "'}";
            return jsonString;
        }

        /// <summary>
        /// 保存图片路径
        /// </summary>
        /// <param name="TpId"></param>
        /// <param name="UserId"></param>
        /// <param name="ImagePath"></param>
        /// <returns></returns>
        public static int SaveImagePath(int TpId, int UserId, string ImagePath)
        {
            DateTime? ShootDate = null;
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(ImagePath))
            {
                for (int i = 0; i < image.PropertyItems.Length; i++)
                {
                    Loger.Current.Write("Use.SaveImagePath PropertyItems[" + i + "].Id=" + ShootDate);
                    Loger.Current.Write("Use.SaveImagePath PropertyItems[" + i + "].Value=" + ShootDate);
                    if (image.PropertyItems[i].Id == 0x0132)
                    {
                        //拍照日期
                        string value = Encoding.ASCII.GetString(image.PropertyItems[i].Value);
                        string[] dtParts = value.Split(new string[2] { " ", ":" },
                            StringSplitOptions.RemoveEmptyEntries);
                        if (Convert.ToInt32(dtParts[0]) > 0)
                        {
                            //有拍照日期
                            ShootDate = new DateTime(Convert.ToInt32(dtParts[0]), Convert.ToInt32(dtParts[1]), Convert.ToInt32(dtParts[2]),
                                Convert.ToInt32(dtParts[3]), Convert.ToInt32(dtParts[4]), Convert.ToInt32(dtParts[5]), DateTimeKind.Local);
                            break;
                        }
                    }
                }
            }
            return new UserDao().SaveImagePath(TpId, UserId, ImagePath);
        }

         /// <summary>
        /// 获取区域
        /// </summary>
        /// <param name="RegionId"></param>
        /// <returns></returns>
        public static DataTable GetRegionListByRegionId(string RegionId)
        {
            return new UserDao().GetRegionListByRegionId(RegionId);
        }
    }
}
