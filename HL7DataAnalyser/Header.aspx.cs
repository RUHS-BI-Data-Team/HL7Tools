using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HL7DataAnalyser
{
    public partial class Header : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(Request.QueryString["UserName"] == null || Request.QueryString["UserName"] == "")
            {
                lblTeam.Text = "No User";
            }
            else
            {
                lblTeam.Text = Request.QueryString["UserName"];
            }
            lblDate.Text = DateTime.Now.ToLongDateString();
            if (Request.QueryString["AppName"] == null || Request.QueryString["AppName"] == "")
            {
                lblAppName.Text = "No Application";
            }
            else
            {
                lblAppName.Text = Request.QueryString["AppName"];
            }
        }
    }
}