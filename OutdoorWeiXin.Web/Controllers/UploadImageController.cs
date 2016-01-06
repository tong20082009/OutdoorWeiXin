using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Senparc.Weixin.MP.CommonAPIs;
using System.Configuration;
using System.Text;
using AutoRadio.RadioSmart.Common;
using System.IO;
using OutdoorWeiXin.Web._Code;
using System.Drawing;
using System.Drawing.Imaging;

namespace OutdoorWeiXin.Web.Controllers
{
    public class UploadImageController : BaseController
    {
        public ActionResult Index(int tpid)
        {
            if (CurrentUserID > 0)
            {
                ViewBag.TpId = tpid;
                String requestHeader = Request.UserAgent;
                if (Request.Url != null)
                {
                    ViewBag.Config = WeiXinHelper.Sign(Request.Url.AbsoluteUri);
                    // ViewBag.SystemType = CheckIsIos(requestHeader) ? "ios" : "android";
                }
                return View();
            }
            else
            {
                return Redirect("/Login/Index");
            }
        }

        /// <summary>
        /// 判断是否ios移动端
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        private bool CheckIsIos(string agent)
        {
            bool flag = false;
            string[] keywords = { "iPhone", "iPod", "iPad" };
            foreach (string item in keywords)
            {
                if (agent.Contains(item))
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }

        /// <summary>
        /// ajax上传图片
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxUpload()
        {
            string rootImage = ConfigurationManager.AppSettings["WeiXinUploadImage"];//上传目录  
            int tpid = ConvertHelper.GetInteger(Request.Params["TpId"]);
            string longitude = ConvertHelper.GetString(Request.Params["longitude"]);
            string latitude = ConvertHelper.GetString(Request.Params["latitude"]);
            if (string.IsNullOrEmpty(rootImage))
            {
                return Json(GetJson("0", "目录配置错误", "", "", ""), JsonRequestBehavior.AllowGet);
            }
            if (Request.Files.Count == 0)
            {
                return Json(GetJson("0", "请选择图片文件", "", "", ""), JsonRequestBehavior.AllowGet);
            }
            HttpPostedFileBase myAdFile = Request.Files[0];
            if (myAdFile.ContentLength > 10 * 1024 * 1024)
            {
                return Json(GetJson("0", "请选择小于10M的图片文件", "", "", ""), JsonRequestBehavior.AllowGet);
            }
            string fileName = Path.GetFileName(myAdFile.FileName);
            string fileType = Path.GetExtension(fileName).ToLower().Trim('.');
            string contentType = myAdFile.ContentType.ToLower();
            Loger.Current.Write(fileName + "的contentType=" + contentType + "；fileType=" + fileType);
            if (!(fileType == "jpg" || fileType == "jpeg" || fileType == "png"))//判断是否可上传的文件类型
            {
                return Json(GetJson("0", "只能上传图片文件", "", "", ""), JsonRequestBehavior.AllowGet);
            }


            string parcialFile = FileHelper.GetADFileSubPath(ConvertHelper.GetLong(CurrentUserID + DateTime.Now.Ticks), fileType);//按规则生成目录格式
            string newParcialFile = parcialFile.Substring(0, parcialFile.LastIndexOf('.')) + "s." + fileType;//按规则生成缩略图目录格式
            string targetFile = rootImage + System.IO.Path.DirectorySeparatorChar + parcialFile;//生成上传文件的全路径
            string newTargetFile = rootImage + System.IO.Path.DirectorySeparatorChar + newParcialFile;//生成上传文件缩略图的全路径
            string targetpath = System.IO.Path.GetDirectoryName(targetFile);
            if (!System.IO.Directory.Exists(targetpath))
                System.IO.Directory.CreateDirectory(targetpath);
            try
            {
                // myAdFile.SaveAs(targetFile);
                Image originalImage = Image.FromStream(myAdFile.InputStream);
                if (originalImage.PropertyIdList.Contains(0x0112))
                {
                    int rotationValue = originalImage.GetPropertyItem(0x0112).Value[0];
                    Loger.Current.Write("UploadImage.AjaxUpload() rotationValue=" + rotationValue);
                    switch (rotationValue)
                    {
                        case 1: // landscape, do nothing
                            break;
                        case 2:
                            originalImage.RotateFlip(RotateFlipType.RotateNoneFlipX);//horizontal flip  
                            break;
                        case 3:
                            originalImage.RotateFlip(RotateFlipType.Rotate180FlipNone);//right-top  
                            break;
                        case 4:
                            originalImage.RotateFlip(RotateFlipType.RotateNoneFlipY);//vertical flip  
                            break;
                        case 5:
                            originalImage.RotateFlip(RotateFlipType.Rotate90FlipX);
                            break;
                        case 6:
                            originalImage.RotateFlip(RotateFlipType.Rotate90FlipNone);//right-top  
                            break;
                        case 7:
                            originalImage.RotateFlip(RotateFlipType.Rotate270FlipX);
                            break;
                        case 8:
                            originalImage.RotateFlip(RotateFlipType.Rotate270FlipNone);//left-bottom  
                            break;
                    }
                }
                originalImage.Save(targetFile);
                MakeThumbnail(targetFile, newTargetFile, 240, 240);//生成缩略图
                Model.User.SaveImagePath(tpid, CurrentUserID, parcialFile, newParcialFile, longitude + "," + latitude);

            }
            catch (Exception e)
            {
                Loger.Current.Write("UploadImage.AjaxUpload() err=" + e.ToString());
                return Json(GetJson("0", "上传出错，请重新上传", "", "", ""), JsonRequestBehavior.AllowGet);
            }
            return Json(GetJson("1", CurrentUserID.ToString(), parcialFile, StringHelper.SubString(fileName, 20, true), fileName), JsonRequestBehavior.AllowGet);
        }

        public string GetJson(string status, string note, string filePath, string shortFileName, string fileName)
        {
            string json = "status:'{0}',note:'{1}',filePath:'{2}',shortFileName:'{3}',fileName:'{4}'";
            json = string.Format(json, status, note, filePath, shortFileName, fileName);
            json = "{" + json + "}";
            return json;
        }

        private void MakeThumbnail(string sourcePath, string newPath, int width, int height)
        {
            System.Drawing.Image ig = System.Drawing.Image.FromFile(sourcePath);
            int towidth = width;
            int toheight = height;
            int x = 0;
            int y = 0;
            int ow = ig.Width;
            int oh = ig.Height;
            if ((double)ig.Width / (double)ig.Height > (double)towidth / (double)toheight)
            {
                oh = ig.Height;
                ow = ig.Height * towidth / toheight;
                y = 0;
                x = (ig.Width - ow) / 2;

            }
            else
            {
                ow = ig.Width;
                oh = ig.Width * height / towidth;
                x = 0;
                y = (ig.Height - oh) / 2;
            }
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.Clear(System.Drawing.Color.Transparent);
            g.DrawImage(ig, new System.Drawing.Rectangle(0, 0, towidth, toheight), new System.Drawing.Rectangle(x, y, ow, oh), System.Drawing.GraphicsUnit.Pixel);
            try
            {
                bitmap.Save(newPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ig.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }

        }

        /// <summary>
        /// 保存图片路径
        /// </summary>
        /// <returns></returns>
        //[HttpPost]
        //public ActionResult SaveImagePath(int TpId, string ImagePath)
        //{
        //    if (ImagePath != "," && !string.IsNullOrEmpty(ImagePath))
        //    {
        //        int result = Model.User.SaveImagePath(TpId, CurrentUserID, ImagePath);
        //        if (result > 0)
        //        {
        //            return Redirect("/MyViewTaskProject/Index");
        //        }
        //    }
        //    return Redirect("/UploadImage/Index");
        //}
    }
}
