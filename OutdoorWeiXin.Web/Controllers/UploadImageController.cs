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

namespace OutdoorWeiXin.Web.Controllers
{
    public class UploadImageController : BaseController
    {
        private string appId = ConfigurationManager.AppSettings["AppId"];
        private string secret = ConfigurationManager.AppSettings["AppSecret"];
        public ActionResult Index(int tpid)
        {
            if (CurrentUserID > 0)
            {
                ViewBag.TpId = tpid;
                return View();
            }
            else
            {
                return Redirect("/Login/Index");
            }
        }


        /// <summary>
        /// ajax上传图片
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxUpload(string MediaId)
        {
            //if (Request.IsAjaxRequest())
            //{
            //    string Token = AccessTokenContainer.TryGetToken(appId, secret);
            //    string jsonString = Model.User.GetMultimedia(CurrentUserID, Token, MediaId);
            //    return Json(jsonString, "text/html", JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    return Json("", JsonRequestBehavior.AllowGet);
            //}
                string rootImage = ConfigurationManager.AppSettings["WeiXinUploadImage"];//上传目录  
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
                string targetFile = rootImage + System.IO.Path.DirectorySeparatorChar + parcialFile;//生成上传文件的全路径
                string targetpath = System.IO.Path.GetDirectoryName(targetFile);
                if (!System.IO.Directory.Exists(targetpath))
                    System.IO.Directory.CreateDirectory(targetpath);
                try
                {
                    myAdFile.SaveAs(targetFile);
                }
                catch
                {
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

        /// <summary>
        /// 保存图片路径
        /// </summary>
        /// <returns></returns>
        public JsonResult SaveImagePath(int TpId, string ImagePath)
        {
            if (Request.IsAjaxRequest())
            {
                if (ImagePath != "," && !string.IsNullOrEmpty(ImagePath))
                {
                    int result = Model.User.SaveImagePath(TpId, CurrentUserID, ImagePath);
                    return Json(result > 0 ? "success" : "err", "text/html", JsonRequestBehavior.AllowGet);
                }
            }
            return Json("err", JsonRequestBehavior.AllowGet);
        }
    }
}
