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
    public class MyViewTaskProjectController : BaseController
    {
        string showList = string.Empty;
        public ActionResult Index()
        {
            if (CurrentUserID > 0)
            {
                ViewBag.UserID = CurrentUserID;
                showList = Request["type"];
                if (string.IsNullOrEmpty(showList))
                {
                    showList = "In";
                }
                ViewBag.ShowList = showList;
                TaskProjectViewModel model = new TaskProjectViewModel();
                model.Type = showList;
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
        /// <param name="ProjectName"></param>
        /// <param name="Type"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ParamSearchList(string ProjectName, string Type, int PageIndex, int PageSize)
        {
            if (Request.IsAjaxRequest())
            {
                Loger.Current.Write("MyViewTaskProjectController.ParamSearchList() PageIndex=" + PageIndex + ",PageSize=" + PageSize);
                if (string.IsNullOrEmpty(Type))
                {
                    Type = "In";
                }
                string relation = "0";//All
                switch (Type)
                {
                    case "In":
                        relation = "0";
                        break;
                    case "AuditPass":
                        relation = "1";
                        break;
                    case "AuditNoPass":
                        relation = "2";
                        break;
                    case "Overdue":
                        relation = "3";
                        break;
                }
                TaskProjectViewModel model = new TaskProjectViewModel();
                string SqlWhere = " tp.Status<>0  AND tp.TPId IN (SELECT TPId FROM dbo.TaskProjectUserRelation WHERE UserId=" + CurrentUserID + " AND Relation = " + relation + ")";
                if (!string.IsNullOrEmpty(ProjectName) && ProjectName != "请输入楼宇名称")
                {
                    SqlWhere += " AND tp.BlockName LIKE '%" + ProjectName + "%'";
                }
                DataTable dt = Model.User.GetTaskProjectList(SqlWhere, PageIndex, PageSize);
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
                       // taskViewModel.Relation = ConvertHelper.GetInteger(dr["Relation"]);
                        taskViewModelList.Add(taskViewModel);
                    }
                    model.TaskProjectList = taskViewModelList;
                    return PartialView("_List", model);
                }
                else
                {
                    return Json("err", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("err", JsonRequestBehavior.AllowGet);
            }
        }
    }
}
