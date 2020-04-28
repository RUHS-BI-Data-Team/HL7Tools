using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace HL7DataAnalyser
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        //Priti Start
        public void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started
            string[] partsOfUserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split("\\".ToCharArray());
            string domainName = partsOfUserName[0];
            string userName = partsOfUserName[1];
            Session["CurrentUser"] = userName;
        }

        protected void Application_AcquireRequestState(object sender, System.EventArgs e)
        {
            if (HttpContext.Current.Handler is IRequiresSessionState)
            {
                System.Security.Principal.IPrincipal principal = default(System.Security.Principal.IPrincipal);
                principal = (System.Security.Principal.IPrincipal)Session["Intgn.Libraries.Security.Principal"];
                if (principal == null)
                {
                    if (User.Identity.IsAuthenticated && User.Identity is System.Web.Security.FormsIdentity)
                    {
                        // We should only get here when the session expires after
                        // we have logged in (have a valid FormsIdentity)
                        FormsAuthentication.SignOut();
                        Response.Redirect(Request.Url.PathAndQuery);
                    }
                    // didn't get a principal from Session, so
                    // set it to the unathenticated FWRPrincipal
                    Intgn.Libraries.Security.Principal.Logout();
                }
                else
                {
                    // use the principal from session
                    HttpContext.Current.User = principal;
                }

            }
        }
        //Priti End
    }
}