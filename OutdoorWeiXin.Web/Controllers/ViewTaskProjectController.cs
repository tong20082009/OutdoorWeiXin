using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OutdoorWeiXin.Web.Models;
using AutoRadio.RadioSmart.Common;
using System.Data;

namespace OutdoorWeiXin.Web.Controllers
{
    public class ViewTaskProjectController : BaseController
    {
        public ActionResult Index()
        {
            if (CurrentUserID > 0)
            {
                ViewBag.UserID = CurrentUserID;
                TaskProjectViewModel model = new TaskProjectViewModel();
                List<RegionViewModel> regionList = new List<RegionViewModel>();
                RegionViewModel regionViewModel = null;
                DataTable dt = Model.User.GetRegionList();
                foreach (DataRow dr in dt.Rows)
                {
                    regionViewModel = new RegionViewModel();
                    regionViewModel.RegionId = ConvertHelper.GetString(dr["RegionId"]);
                    regionViewModel.RegionName = ConvertHelper.GetString(dr["RegionName"]);
                    regionList.Add(regionViewModel);
                }
                model.RegionList = regionList;
                var openid = HttpContext.Request.Cookies["openid"];
                Model.User user = Model.User.UserDataBind(openid.Value);
                model.City = user.City;
                model.AreaName = user.AreaName;
                return View(model);
            }
            else
            {
                return Redirect("/Login/Index");
            }
        }

        /// <summary>
        /// 参数提交查询
        /// </summary>
        /// <param name="RegionId"></param>
        /// <param name="AreaName"></param>
        /// <param name="ProjectName"></param>
        /// <param name="Type"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ParamSearchList(string RegionId, string AreaName, string ProjectName, string Type, int PageIndex, int PageSize)
        {
            if (Request.IsAjaxRequest())
            {
                Loger.Current.Write("ViewTaskProjectController.ParamSearchList() PageIndex=" + PageIndex + ",PageSize=" + PageSize);
                TaskProjectViewModel model = new TaskProjectViewModel();
                string SqlWhere = " t.Status in (1,2) AND tp.Status=0";
                if (!string.IsNullOrEmpty(RegionId))
                {
                    SqlWhere += " AND tp.RegionId = '" + RegionId + "'";
                }
                if (!string.IsNullOrEmpty(AreaName))
                {
                    SqlWhere += " AND tp.AreaName = '" + AreaName + "'";
                }
                if (!string.IsNullOrEmpty(ProjectName) && ProjectName != "请输入楼宇名称")
                {
                    SqlWhere += " AND (tp.BlockName LIKE '%" + ProjectName + "%' OR t.TaskName LIKE '%" + ProjectName + "%')";
                }
                if (Type == "WIFI")
                {
                    SqlWhere += " AND tp.MediaType LIKE '%wifi%'";
                }
                else if (Type == "GM")
                {
                    SqlWhere += " AND tp.MediaType LIKE '%框架%'";
                }
                DataTable dt = Model.User.GetTaskProjectList(SqlWhere, PageIndex, PageSize);
                if (dt.Rows.Count > 0)
                {
                    List<TaskProjectListViewModel> taskViewModelList = new List<TaskProjectListViewModel>();
                    TaskProjectListViewModel taskViewModel = null;
                    foreach (DataRow dr in dt.Rows)
                    {
                        taskViewModel = new TaskProjectListViewModel();
                        taskViewModel.TId = ConvertHelper.GetInteger(dr["TId"]);
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
                        taskViewModel.TaskStatus = ConvertHelper.GetInteger(dr["TaskStatus"]);
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
        /// 领取任务
        /// </summary>
        /// <param name="TpId">任务项目id</param>
        /// <returns></returns>
        public JsonResult ReceiveTask(int TId, int TpId, int TaskStatus)
        {
            if (Request.IsAjaxRequest())
            {
                if (Model.User.GetTaskProjectStatus(TpId) > 0)
                {
                    return Json("in", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Model.User.ReceiveTask(TpId, CurrentUserID);
                    if (TaskStatus == 1)
                    {
                        Model.User.UpdateTaskStatus(TId, 2);
                    }
                    return Json("success", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("err", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取区域
        /// </summary>
        /// <param name="RegionId">地区id</param>
        /// <returns></returns>
        public ActionResult GetArea(string RegionId)
        {
            if (Request.IsAjaxRequest())
            {
                TaskProjectViewModel model = new TaskProjectViewModel();
                model.RegionList = ModelHelper.GetListModelByTable<RegionViewModel>(Model.User.GetRegionListByRegionId(RegionId));
                return PartialView("_Region", model);
            }
            else
            {
                return Json("err", JsonRequestBehavior.AllowGet);
            }
        }
    }
}
