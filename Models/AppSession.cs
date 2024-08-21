using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATL_UPLOAD.Models
{
    public class AppSession
    {
        public static SessionRepositoryModel Session
        {
            get
            {
                if (HttpContext.Current.Session["App"] == null)
                {
                    return null;
                }
                else
                {
                    return JsonConvert.DeserializeObject<SessionRepositoryModel>(HttpContext.Current.Session["App"].ToString());
                }
            }
            set
            {
                HttpContext.Current.Session.Add("App", JsonConvert.SerializeObject(value));
            }
        }
    }
}