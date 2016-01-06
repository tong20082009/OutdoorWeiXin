using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OutdoorWeiXin.Web.Models;
using AutoRadio.RadioSmart.Common;
using System.Data;
using System.Configuration;

namespace OutdoorWeiXin.Web.Controllers
{
    public class MyViewTaskProjectController : BaseController
    {
        private string commonImagePath = ConfigurationManager.AppSettings["PreviewWeiXinImage"] + "/";
        public ActionResult Index()
        {
            if (CurrentUserID > 0)
            {
                ViewBag.UserID = CurrentUserID;
                return View();
            }
            else
            {
                return Redirect("/Login/Index");
            }
        }

        /// <summary>
        /// 参数提交查询
        /// </summary>
        /// <param name="ProjectName"></param>
        /// <param name="MediaType"></param>
        /// <param name="Type"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ParamSearchList(string ProjectName, string MediaType, string Type, int PageIndex, int PageSize)
        {
            if (Request.IsAjaxRequest())
            {
                Loger.Current.Write("MyViewTaskProjectController.ParamSearchList() PageIndex=" + PageIndex + ",PageSize=" + PageSize);
                if (string.IsNullOrEmpty(Type))
                {
                    Type = "In";
                }
                string relation = "0";//All
                string status = "1";
                switch (Type)
                {
                    case "In":
                        relation = "0";
                        ViewBag.Relation = "0";
                        status = "1";
                        break;
                    case "Upload":
                        relation = "0";
                        ViewBag.Relation = "0";
                        status = "2";
                        break;
                    case "AuditPass":
                        relation = "1";
                        ViewBag.Relation = "1";
                        status = "3";
                        break;
                    case "AuditNoPass":
                        relation = "2";
                        ViewBag.Relation = "2";
                        status = "4";
                        break;
                    case "Overdue":
                        relation = "3";
                        ViewBag.Relation = "3";
                        status = "5";
                        break;
                }
                TaskProjectViewModel model = new TaskProjectViewModel();
                int CusId = Model.User.GetCusIdByUserId(CurrentUserID);
                string SqlWhere = " tp.TPId IN (SELECT TPId FROM dbo.TaskProjectUserRelation WHERE (UserId=" + CurrentUserID + " or CusId=" + CusId + ") AND Relation = " + relation + ")";
                if (status == "1" || status == "2")
                {
                    SqlWhere += " AND tp.Status = " + status + "";
                }
                if (!string.IsNullOrEmpty(ProjectName) && ProjectName != "请输入楼宇名称")
                {
                    SqlWhere += " AND tp.BlockName LIKE '%" + ProjectName + "%'";
                }
                if (MediaType == "WIFI")
                {
                    SqlWhere += " AND tp.MediaType LIKE '%wifi%'";
                }
                else if (MediaType == "GM")
                {
                    SqlWhere += " AND tp.MediaType LIKE '%框架%'";
                }
                DataTable dt = Model.User.GetTaskProjectList(CurrentUserID, CusId, SqlWhere, PageIndex, PageSize);
                if (dt.Rows.Count > 0)
                {
                    List<TaskProjectListViewModel> taskViewModelList = new List<TaskProjectListViewModel>();
                    TaskProjectListViewModel taskViewModel = null;
                    foreach (DataRow dr in dt.Rows)
                    {
                        taskViewModel = new TaskProjectListViewModel();
                        taskViewModel.TPId = ConvertHelper.GetInteger(dr["TPId"]);
                        taskViewModel.StreetAddress = ConvertHelper.GetString(dr["StreetAddress"]);
                        taskViewModel.AreaName = ConvertHelper.GetString(dr["AreaName"]);
                        taskViewModel.RegionId = ConvertHelper.GetString(dr["RegionId"]);
                        taskViewModel.AreaName = ConvertHelper.GetString(dr["AreaName"]);
                        taskViewModel.BlockName = ConvertHelper.GetString(dr["BlockName"]);
                        taskViewModel.PointName = ConvertHelper.GetString(dr["PointName"]);
                        taskViewModel.MediaType = ConvertHelper.GetString(dr["MediaType"]);
                        taskViewModel.AdProductName = ConvertHelper.GetString(dr["AdProductName"]);
                        taskViewModel.BeginDate = ConvertHelper.GetShortDateString(dr["BeginDate"]);
                        taskViewModel.EndDate = ConvertHelper.GetShortDateString(dr["EndDate"]);
                        taskViewModel.PhotoRequire = ConvertHelper.GetString(dr["PhotoRequire"]);
                        taskViewModel.Status = ConvertHelper.GetInteger(dr["Status"]);
                        taskViewModel.Price = ConvertHelper.GetInteger(dr["Price"]);
                        taskViewModel.AuditReason = ConvertHelper.GetString(dr["AuditReason"]);
                        taskViewModel.ImgPath = ConvertHelper.GetString(dr["ImgPath"]);
                        taskViewModelList.Add(taskViewModel);
                    }
                    model.TaskProjectList = taskViewModelList;
                    return PartialView("_List", model);
                }
                else
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("err", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 预览图片
        /// </summary>
        /// <returns></returns>
        public ActionResult PreviewImage(string path)
        {
            if (CurrentUserID > 0)
            {
                ViewBag.ImgPath = path;
                ViewBag.Directory = commonImagePath;
                return View("PreviewImage");
            }
            else
            {
                return Redirect("/Login/Index");
            }
        }

        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="imagePath"></param>
        /// <param name="deletePath"></param>
        /// <returns></returns>
        public string DeleteImage(string imagePath, string deletePath)
        {
            if (CurrentUserID > 0)
            {
                if (!string.IsNullOrEmpty(imagePath) && !string.IsNullOrEmpty(deletePath))
                {
                    string newImagePath = imagePath.Replace(deletePath + "|", "").Replace(deletePath, "");
                    string[] arrImgPath = imagePath.Split('|');
                    string thumbnailImgPath = string.Empty;
                    for (int i = 0; i < arrImgPath.Length; i++)
                    {
                        var arrPath = arrImgPath[i].Split('.');
                        if (arrPath.Length > 1)
                        {
                            thumbnailImgPath += (arrPath[0] + "s." + arrPath[1] + "|");
                        }
                    }
                    thumbnailImgPath = thumbnailImgPath.Replace(deletePath.Substring(0, deletePath.LastIndexOf('.')) + "s" + deletePath.Substring(deletePath.LastIndexOf('.')) + "|", "").Replace(deletePath.Substring(0, deletePath.LastIndexOf('.')) + "s" + deletePath.Substring(deletePath.LastIndexOf('.')), "");
                    Model.User.UpdateImagePath(newImagePath, thumbnailImgPath, imagePath);
                    Model.User.DeleteImageByImgPath(deletePath);

                    return "ok";
                }
                else
                {
                    return "err";
                }

            }
            else
            {
                return "login";
            }
        }

        /// <summary>
        /// 退回任务
        /// </summary>
        /// <param name="TpId">任务项目id</param>
        /// <returns></returns>
        public JsonResult ReturnTask(int TpId)
        {
            if (Request.IsAjaxRequest())
            {
                if (TpId > 0)
                {
                    Model.User.ReturnTask(TpId, CurrentUserID);
                    return Json("success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("操作出错", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("err", JsonRequestBehavior.AllowGet);
            }
        }
    }
}
