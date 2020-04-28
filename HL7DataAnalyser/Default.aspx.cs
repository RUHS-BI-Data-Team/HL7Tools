using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HL7DataAnalyser
{
    public partial class _Default : Page
    {
        List<String> SqlFieldList = new List<string>();
        protected void Page_Load(object sender, EventArgs e)
        {
            CreateSQLFieldList();
            if (!IsPostBack)
            {
                
                Label lblTmp = Master.FindControl("lblAppName") as Label;
                lblTmp.Text = "HL7 Data Analyzer";
                txtStartDate_CalendarExtender.StartDate = Convert.ToDateTime("5/31/2018");
                txtEndDate_CalendarExtender.StartDate = Convert.ToDateTime("5/31/2018");
                txtStartDate_CalendarExtender.EndDate = DateTime.Now;
                txtEndDate_CalendarExtender.EndDate = DateTime.Now;
                SetDateFilterValues(DateTime.Now.AddDays(-1), DateTime.Now, true);

                //objHL7Data.SelectMethod = "SelectHL7Data";
                //objHL7Data.DataObjectTypeName = "DataAccess";
            }
            sqlHL7Data.SelectCommand = "Select " + SetupDataFields() + " from tblADT " + BuildWhereClause();

            //SetupDataview();
            grdHL7Data.DataBind();
        }

        private void CreateSQLFieldList()
        {
            //SqlFieldList.Add("Id");
            SqlFieldList.Add("HL7MessageDate");
            //SqlFieldList.Add("HL7Event");
            SqlFieldList.Add("HL7ControlId");
            SqlFieldList.Add("MRN");
        }
        private string BuildWhereClause()
        {
            string qry = "";
            if (chkUseDateFilter.Checked)
            {
                qry = "Where HL7MessageDate > '" + Convert.ToDateTime(txtStartDate.Text + " " + txtStartTime.Text) + "'";
                qry = qry + " and HL7MessageDate < '" + Convert.ToDateTime(txtEndDate.Text + " " + txtEndTime.Text) + "'";
            }

            return qry;
        }
        private string SetupDataFields()
        {
            string flds = "Id";
            grdHL7Data.Columns.Clear();
            foreach (String fd in SqlFieldList){
                BoundField bf = new BoundField();
                bf.HeaderText = fd;
                bf.DataField = fd;
                grdHL7Data.Columns.Add(bf);
                flds = flds + "," + fd;
            }
            
            BoundField bfId = new BoundField();
            bfId.HeaderText = "Id";
            bfId.DataField = "Id";
            bfId.Visible = false;
            grdHL7Data.Columns.Add(bfId);
            return flds;
        }
        void SetDateFilterValues(DateTime StartDateTime, DateTime EndDateTime, Boolean Enabled)
        {
            txtStartDate.Text = StartDateTime.ToShortDateString();
            txtStartTime.Text = StartDateTime.ToLongTimeString();
            txtEndDate.Text = EndDateTime.ToShortDateString();
            txtEndTime.Text = EndDateTime.ToLongTimeString();
            chkUseDateFilter.Checked = Enabled;
        }
    }
}