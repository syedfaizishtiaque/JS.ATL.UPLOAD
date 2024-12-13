using ATL_UPLOAD.Models;
using SessionMenu.Lib.Models;
using SessionMenu.Lib.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ATL_UPLOAD.Repository
{
    public class AppSessionRepo
    {
        readonly ILogin sess;
        public AppSessionRepo()
        {
            try
            {
                if (AppSession.Session != null)
                {
                    if (AppSession.Session.SessionRepo != null && AppSession.Session.SessionRepo.Count > 0)
                    {
                        return;
                    }
                    else
                    {
                        if (AppSession.Session.SessionRepo.Count == 0)
                        {
                            AppSession.Session = null;
                        }
                    }
                }
                if (AppSession.Session == null)
                {
                    sess = new RepoLogin();

                    //to un-comment for deployment
                    var username = System.Web.HttpContext.Current.User.Identity.Name;

                    //FOR development
                    //string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;//Request.LogonUserIdentity.Name;
                    username = username.Replace("JSBL\\", "");
                    // username = "syed.hafeezuddin";
                    // username = "siddiqui.ahmed"; // for test pupose only
                    int AppId = Convert.ToInt32(ConfigurationManager.AppSettings["AppId"]);
                    List<SessionModel> list = sess.GetData(username, AppId);
                    AppSession.Session = new SessionRepositoryModel()
                    {
                        SessionRepo = list
                    };
                }
            }
            catch (Exception ex)
            {
                ErrorLog.WriteException(ex.ToString(), "AppSessionRepo.LogOut");
                throw ex;
            }
        }
    }
}