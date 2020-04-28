using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HL7DataAnalyser
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            Intgn.Libraries.Security.UserAccount usr = (Intgn.Libraries.Security.UserAccount)(this.Parent.Page.User.Identity);
            //lblTeam.Text = usr.FullName;
        }
    }
}