using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using AutoRadio.RadioSmart.Common;
using System.Net;
using System.Web;
using System.Configuration;

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
        /// 区县
        /// </summary>
        public string AreaName { get; set; }

        /// <summary>
        /// 关联账号ID
        /// </summary>
        public int CusId { get; set; }


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
        /// 根据openid修改city
        /// </summary>
        /// <param name="OpenId"></param>
        /// <param name="City"></param>
        ///  <param name="AreaName"></param>
        /// <returns></returns>
        public static int UpdateCityByOpenId(string OpenId, string City, string AreaName)
        {
            return new UserDao().UpdateCityByOpenId(OpenId, City, AreaName);
        }

        /// <summary>
        /// 根据openid获取用户id
        /// </summary>
        /// <param name="OpenId"></param>
        /// <returns></returns>
        public static int GetUserIdByOpenId(string OpenId)
        {
            int userID = UserDataBind(OpenId).UserId;
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
        public static User UserDataBind(string openid)
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
                user.AreaName = ConvertHelper.GetString(dt.Rows[0]["AreaName"]);
                user.CusId = ConvertHelper.GetInteger(dt.Rows[0]["CusId"]);
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
        /// 查询任务项目列表
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="CusId"></param>
        /// <param name="SqlWhere"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static DataTable GetTaskProjectList(int UserId,int CusId, string SqlWhere, int PageIndex, int PageSize)
        {
            return new UserDao().GetTaskProjectList(UserId,CusId, SqlWhere, PageIndex, PageSize);
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
        /// 获取任务状态
        /// </summary>
        /// <param name="TpId"></param>
        /// <returns></returns>
        public static int GetTaskProjectStatus(int TpId)
        {
            return new UserDao().GetTaskProjectStatus(TpId);
        }

        /// <summary>
        /// 修改任务状态
        /// </summary>
        /// <param name="TId"></param>
        /// <param name="TaskStatus"></param>
        /// <returns></returns>
        public static int UpdateTaskStatus(int TId, int TaskStatus)
        {
            return new UserDao().UpdateTaskStatus(TId, TaskStatus);
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
        /// 根据区域名称获取区域id
        /// </summary>
        /// <param name="RegionName"></param>
        /// <returns></returns>
        public static string GetRegionIdByRegionName(string RegionName)
        {
            return new UserDao().GetRegionIdByRegionName(RegionName);
        }

        /// <summary>
        /// 下载保存多媒体文件,返回多媒体保存路径
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="ServerId"></param>
        /// <returns></returns>
        public static string GetMultimedia(int UserId, string Token, string MediaId)
        {
            string stUrl = "http://file.api.weixin.qq.com/cgi-bin/media/get?access_token=" + Token + "&media_id=" + MediaId;
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(stUrl);
            string savepath = string.Empty;
            req.Method = "GET";
            using (WebResponse wr = req.GetResponse())
            {
                HttpWebResponse myResponse = (HttpWebResponse)req.GetResponse();
                string strpath = myResponse.ResponseUri.ToString();
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
        /// <param name="ThumbnailImgPath"></param>
        /// <returns></returns>
        public static int SaveImagePath(int TpId, int UserId, string ImagePath, string ThumbnailImgPath, string position)
        {
            DateTime ShootDate = new DateTime();
            Model.UserDao UserBao = new UserDao();
            string imagePath = ImagePath;
            string thumbnailImgPath = ThumbnailImgPath;


            string rootImage = ConfigurationManager.AppSettings["WeiXinUploadImage"] + System.IO.Path.DirectorySeparatorChar + ImagePath;//上传目录  
            Exif exif = new Exif(rootImage);
            string[] dtParts = exif.DateTime.Split(new string[2] { " ", ":" },
                StringSplitOptions.RemoveEmptyEntries);
            if (dtParts.Length > 0)
            {
                if (Convert.ToInt32(dtParts[0]) > 0)
                {
                    //有拍照日期
                    ShootDate = new DateTime(Convert.ToInt32(dtParts[0]), Convert.ToInt32(dtParts[1]), Convert.ToInt32(dtParts[2]),
                        Convert.ToInt32(dtParts[3]), Convert.ToInt32(dtParts[4]), Convert.ToInt32(dtParts[5]), DateTimeKind.Local);
                }
            }
            else
            {
                ShootDate = DateTime.Now;
            }
            Loger.Current.Write("ShootDate=" + ShootDate);
            Loger.Current.Write("exif.DateTime=" + exif.DateTime);
            string ShootPosition = ConvertDegreesToDigital(exif.GPSLongitude) + "," + ConvertDegreesToDigital(exif.GPSLatitude);
            if (string.IsNullOrEmpty(exif.GPSLongitude) || string.IsNullOrEmpty(exif.GPSLatitude))
            {
                ShootPosition = position;
            }
            Loger.Current.Write("exif.GPSLongitude=" + exif.GPSLongitude + "  exif.GPSLatitude=" + exif.GPSLatitude);
            Loger.Current.Write("ConvertDegreesToDigital.exif.GPSLongitude=" + ConvertDegreesToDigital(exif.GPSLongitude) + "  ConvertDegreesToDigital.exif.GPSLatitude=" + ConvertDegreesToDigital(exif.GPSLatitude));
            Loger.Current.Write("weixin.GPS=" + position);
            int CusId = GetCusIdByUserId(UserId);
            DataTable dt = new UserDao().GetTaskProjectUserRelation(TpId, UserId, CusId);
            if (dt.Rows.Count > 0)
            {
                ImagePath += "|" + dt.Rows[0]["ImgPath"];
                ThumbnailImgPath += "|" + dt.Rows[0]["ThumbnailImgPath"];

                //DataTable dtimages = UserBao.GetImagesByTPUId(ConvertHelper.GetInteger(dt.Rows[0]["TPUId"]));
                var Sort = ImagePath.Split(',').Length;
                var ExportImgPath = UserBao.GetIdByTPUId(ConvertHelper.GetInteger(dt.Rows[0]["TPUId"])) + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + imagePath.Substring(imagePath.LastIndexOf('.'));

                UserBao.AddImagePath(ConvertHelper.GetInteger(dt.Rows[0]["TPUId"]), ConvertHelper.GetDateTime(ShootDate), imagePath, thumbnailImgPath, Sort, ExportImgPath);
            }


            return new UserDao().SaveImagePath(TpId, UserId, CusId, ImagePath.Trim('|'), ThumbnailImgPath.Trim('|'), ConvertHelper.GetDateTime(ShootDate), ShootPosition);
        }

        /// <summary>
        /// 度分秒经纬度(必须含有'°')和数字经纬度转换
        /// </summary>
        /// <param name="digitalDegree">度分秒经纬度</param>
        /// <return>数字经纬度</return>
        private static double ConvertDegreesToDigital(string degrees)
        {
            double num = 60;
            double digitalDegree = 0.0;
            int d = degrees.IndexOf('°');           //度的符号对应的 Unicode 代码为：00B0[1]（六十进制），显示为°。
            if (d < 0)
            {
                return digitalDegree;
            }
            string degree = degrees.Substring(0, d);
            digitalDegree += Convert.ToDouble(degree);

            int m = degrees.IndexOf('′');           //分的符号对应的 Unicode 代码为：2032[1]（六十进制），显示为′。
            if (m < 0)
            {
                return digitalDegree;
            }
            string minute = degrees.Substring(d + 1, m - d - 1);
            digitalDegree += ((Convert.ToDouble(minute)) / num);

            int s = degrees.IndexOf('″');           //秒的符号对应的 Unicode 代码为：2033[1]（六十进制），显示为″。
            if (s < 0)
            {
                return digitalDegree;
            }
            string second = degrees.Substring(m + 1, s - m - 1);
            digitalDegree += (Convert.ToDouble(second) / (num * num));

            return digitalDegree;
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

        /// <summary>
        /// 保存图片路径
        /// </summary>
        /// <param name="ImagePath"></param>
        /// <param name="ThumbnailImgPath"></param>
        /// <param name="OldImagePath"></param>
        /// <returns></returns>
        public static int UpdateImagePath(string ImagePath, string ThumbnailImgPath, string OldImagePath)
        {
            return new UserDao().UpdateImagePath(ImagePath, ThumbnailImgPath, OldImagePath);
        }

        /// <summary>
        /// 根据微信用户id获取账号信息
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public static DataTable GetCustomerByUserId(int UserId)
        {
            return new UserDao().GetCustomerByUserId(UserId);
        }


        /// <summary>
        /// 根据账号密码获取账号Id
        /// </summary>
        /// <param name="CusName"></param>
        /// <param name="PassWord"></param>
        /// <returns></returns>
        public static int GetCusId(string CusName, string PassWord)
        {
            return new UserDao().GetCusId(CusName, PassWord);
        }

        /// <summary>
        /// 绑定账号 
        /// </summary>
        /// <param name="CusId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public static int BindCustomer(int CusId, int UserId)
        {
            return new UserDao().BindCustomer(CusId, UserId);
        }

        /// <summary>
        /// 退回任务
        /// </summary>
        /// <param name="TpId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public static int ReturnTask(int TpId, int UserId)
        {
            return new UserDao().ReturnTask(TpId, UserId);
        }


        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="TpId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public static int DeleteImageByImgPath(string ImgPath)
        {
            return new UserDao().DeleteImageByImgPath(ImgPath);
        }

        /// <summary>
        /// 根据微信用户id获取cusid
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static int GetCusIdByUserId(int userId)
        {
            return new UserDao().GetCusIdByUserId(userId);
        }
    }
}
