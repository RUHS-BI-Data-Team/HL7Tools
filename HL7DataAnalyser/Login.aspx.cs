using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.ApplicationServices;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HL7AnalyserWeb
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if ((Session["CurrentUser"] == null) || ((Session["CurrentUser"].ToString() != null) && Session["ExplicitLogin"] == null))
            {
                // Validate the user with the membership system.
                string[] partsOfUserName = this.Context.Request.LogonUserIdentity.Name.Split("\\".ToCharArray()); 
                string domainName = partsOfUserName[0];
                string userName = partsOfUserName[1];
                Session["CurrentUser"] = userName;
                // Create our connection and command objects
                if (Membership.ValidateUser(userName, null))
                {
                    FormsAuthentication.RedirectFromLoginPage(userName, false);
                    Intgn.Libraries.Security.UserAccount usr = (Intgn.Libraries.Security.UserAccount)(User.Identity);
                    if (((usr.Roles.Contains(System.Web.Configuration.WebConfigurationManager.AppSettings["grpIntegrationAdmin"].ToString()))))
                    {
                        Response.Redirect("HL7DataAnalyser.aspx");
                    }
                    else
                    {
                        FormsAuthentication.RedirectToLoginPage();
                    }
                }
                else
                {
                    Response.Write("User does not have permissions to access this website " + this.Context.Request.LogonUserIdentity.Name + " " + Request.LogonUserIdentity.Name);
                }
            }
        }

        protected void SignInButton_Click(object sender, EventArgs e)
        {
            if (Membership.ValidateUser(UsernameTextBox.Text, PasswordTextBox.Text))
            {
                FormsAuthentication.RedirectFromLoginPage(UsernameTextBox.Text, false);
                Intgn.Libraries.Security.UserAccount usr = (Intgn.Libraries.Security.UserAccount)(User.Identity);
                if ((usr.Roles.Contains(System.Web.Configuration.WebConfigurationManager.AppSettings["grpIntegrationAdmin"].ToString())))
                {
                   Response.Redirect("HL7DataAnalyser.aspx");
                }
                else
                {
                    FormsAuthentication.RedirectToLoginPage();
                }
            }
            else
            {
                Response.Write("There was a problem signing you in.");
            }
        }
    }
}