using System.Web;
using System.Web.Mvc;

namespace ATL_UPLOAD.Attribute
{
   public class SessionAuth : AuthorizeAttribute
        {
            protected override bool AuthorizeCore(HttpContextBase httpContext)
            {
                return httpContext.Session["App"] != null;
            }
            protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
            {
                HttpContext context = HttpContext.Current;
                string baseUrl = context.Request.Url.Scheme + "://" + context.Request.Url.Authority +
                context.Request.ApplicationPath.TrimEnd('/') + "/LogOut/Index";
                filterContext.Result = new RedirectResult(baseUrl);
            }

        }
   
}