using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OutdoorWeiXin.Web.Controllers
{
    public class CustomerBindController : BaseController
    {
        //
        // GET: /CustomerBind/

        public ActionResult Index()
        {
            if (CurrentUserID > 0)
            {
                DataTable dt = Model.User.GetCustomerByUserId(CurrentUserID);
                if (dt.Rows.Count > 0)
                {
                    ViewBag.CusName = dt.Rows[0]["CusName"].ToString();
                    ViewBag.FullName = dt.Rows[0]["FullName"].ToString();
                    return View();
                }
                else
                {
                    return View("Login");
                }
            }
            else
            {
                return Redirect("/Login/Index");
            }
        }

        public JsonResult Bind(string CusName, string PassWord)
        {
            if (Request.IsAjaxRequest())
            {
                if (!string.IsNullOrEmpty(CusName) && !string.IsNullOrEmpty(PassWord))
                {
                    int cusId = Model.User.GetCusId(CusName, PassWord);
                    if (cusId > 0)
                    {
                        Model.User.BindCustomer(cusId, CurrentUserID);
                        return Json("success", JsonRequestBehavior.DenyGet);
                    }
                    else
                    {
                        return Json("用户名或密码错误", JsonRequestBehavior.DenyGet);
                    }
                }
                else
                {
                    return Json("用户名或密码不能为空", JsonRequestBehavior.DenyGet);
                }
            }
            else
            {
                return Json("err", JsonRequestBehavior.DenyGet);
            }
        }

    }
}
