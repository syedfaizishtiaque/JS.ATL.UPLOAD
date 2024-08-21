using ATL_UPLOAD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATL_UPLOAD.Repository
{
    public class AppSessionRepo
    {

        public AppSessionRepo()
        {
            GetSessionData();
        }

        public void GetSessionData()
        {
          
            try
            {

                if (AppSession.Session != null)
                {
                    if (!string.IsNullOrEmpty(AppSession.Session.username))
                    {
                        return;
                    }
                    else
                    {
                        AppSession.Session = null;
                    }
                }
                if (AppSession.Session == null)
                { 
                    //To un-comment for deployment
                    //var username = System.Web.HttpContext.Current.User.Identity.Name;
                    //FOR development
                    string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;//Request.LogonUserIdentity.Name;
                    username = username.Replace("JSBL\\", "").ToLower();
                   
                    AppSession.Session = new SessionRepositoryModel()
                    {
                        username = username
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