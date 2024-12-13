using ATL_UPLOAD;
using ATL_UPLOAD.Repository;
using SessionMenu.Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATL_UPLOAD.Controllers
{
    [SessionAuth]
    public class SearchController : Controller
    {


        [HttpPost]
        public JsonResult GetMenu()
        {
            try
            {

                return Json(AppSession.Session.SessionRepo.Where(x => x.can_slct != false).ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.WriteException(ex.ToString(), "SearchController.GetMenu");
                return null;
            }

        }

        [AllowAnonymous]
        public ActionResult LogOut()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Logout()
        {
            try
            {
                Session.Abandon();
                Session.RemoveAll();
                string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
                Request.ApplicationPath.TrimEnd('/') + "//Search/LogOut";
                return Json(baseUrl, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.WriteException(ex.ToString(), "SearchController.LogOut");
                throw ex;
            }

        }

        public ActionResult UnAuthorized()
        {
            return View();
        }
    }
}