using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HL7DataAnalyser
{
    public partial class HL7MessageParser : System.Web.UI.Page
    {
        Int32 HL7MessageId;
        string HL7MessageType;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["HL7MessageId"] == null || Request.QueryString["HL7MessageId"] == "")
                {

                }
                else
                {
                    
                    HL7MessageId = Convert.ToInt32(Request.QueryString["HL7MessageId"]);
                    ViewState.Add("HL7MessageId", HL7MessageId);
                    HL7MessageType = Request.QueryString["HL7MessageType"];
                    ViewState.Add("HL7MessageType", HL7MessageType);
                    SqlConnection cn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["HL7DataConnectionString"].ToString());
                    SqlCommand cm = new SqlCommand("uspSelectHL7MessageSegments", cn);
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.Parameters.Add(new SqlParameter("@HL7MessageId", SqlDbType.Int, 4));
                    cm.Parameters.Add(new SqlParameter("@HL7MessageType", SqlDbType.NVarChar, 5));
                    cm.Parameters["@HL7MessageId"].Value = HL7MessageId;
                    cm.Parameters["@HL7MessageType"].Value = HL7MessageType;
                    cm.CommandTimeout = 0;
                    SqlCommand cmm = new SqlCommand("uspSelectHL7Message", cn);
                    cmm.CommandType = CommandType.StoredProcedure;
                    cmm.Parameters.Add(new SqlParameter("@HL7MessageId", SqlDbType.Int, 4));
                    cmm.Parameters.Add(new SqlParameter("@HL7MessageType", SqlDbType.NVarChar, 5));
                    cmm.Parameters["@HL7MessageId"].Value = HL7MessageId;
                    cmm.Parameters["@HL7MessageType"].Value = HL7MessageType;
                    cn.Open();
                    SqlDataReader dr;
                    dr = cm.ExecuteReader();
                    gdvParsedMessage.DataSource = dr;
                    gdvParsedMessage.DataBind();
                    dr.Close();
                    cm.Dispose();
                    SqlDataReader drm;
                    drm = cmm.ExecuteReader();
                    if (drm.Read())
                    {
                        ViewState["HL7Message"] = drm.GetValue(0).ToString();
                    }

                    cmm.Dispose();
                    cn.Close();
                    cn.Dispose();
                    
                }

            }
            string ScriptStart = "\n<script type=\"text/javascript\" language=\"Javascript\" id=\"EventScriptBlock\">\n";
            string ScriptEnd = "\n </script>";
            string JavaScript = "function CopyMessageToClipBoard(){var HL7Message = '" + ViewState["HL7Message"] + "'}";
            this.ClientScript.RegisterStartupScript(this.GetType(), "MasterKey", ScriptStart + JavaScript + ScriptEnd);
        }

        protected void gdvParsedMessage_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectSegment")
            {
                GridView gvSeg = sender as GridView;
                HiddenField hdDisplay = gvSeg.Rows[Convert.ToInt32(e.CommandArgument)].FindControl("hdnSelected") as HiddenField;
                HiddenField hdnParseSegmentLocation = gvSeg.Rows[Convert.ToInt32(e.CommandArgument)].FindControl("hdnParseSegmentLocation") as HiddenField;
                GridView gdFld = gvSeg.Rows[Convert.ToInt32(e.CommandArgument)].FindControl("gdvParsedSegment") as GridView;

                if (hdDisplay.Value == "false")
                {
                    gdFld.Visible = true;
                    hdDisplay.Value = "true";
                    SqlConnection cn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["HL7DataConnectionString"].ToString());
                    SqlCommand cm = new SqlCommand("uspSelectHL7MessageSegmentFields", cn);
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.Parameters.Add(new SqlParameter("@HL7MessageId", SqlDbType.Int, 4));
                    cm.Parameters.Add(new SqlParameter("@SegmentId", SqlDbType.Int, 4));
                    cm.Parameters.Add(new SqlParameter("@HL7MessageType", SqlDbType.NVarChar, 5));
                    cm.Parameters["@HL7MessageId"].Value = ViewState["HL7MessageId"];
                    cm.Parameters["@SegmentId"].Value = Convert.ToInt32(hdnParseSegmentLocation.Value);
                    cm.Parameters["@HL7MessageType"].Value = ViewState["HL7MessageType"];
                    cm.CommandTimeout = 0;
                    cn.Open();
                    SqlDataReader dr;
                    dr = cm.ExecuteReader();
                    gdFld.DataSource = dr;
                    gdFld.DataBind();
                    cm.Dispose();
                    cn.Close();
                    cn.Dispose();
                    gdFld.Columns[0].Visible = chkShowDefinitions.Checked;
                }
                else
                {
                    gdFld.Visible = false;
                    hdDisplay.Value = "false";
                }
            }
        }

        protected void gdvParsedSegment_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectField")
            {
                GridView gvSeg = sender as GridView;
                HiddenField hdDisplay = gvSeg.Rows[Convert.ToInt32(e.CommandArgument)].FindControl("hdnSelected") as HiddenField;
                HiddenField hdnParseFieldLocation = gvSeg.Rows[Convert.ToInt32(e.CommandArgument)].FindControl("hdnParseFieldLocation") as HiddenField;
                HiddenField hdnParseSegmentLocation = gvSeg.Rows[Convert.ToInt32(e.CommandArgument)].FindControl("hdnParseSegmentLocation") as HiddenField;
                GridView gdFld = gvSeg.Rows[Convert.ToInt32(e.CommandArgument)].FindControl("gdvParsedField") as GridView;
               
               
                if (hdDisplay.Value == "false")
                {
                    gdFld.Visible = true;
                    hdDisplay.Value = "true";
                    SqlConnection cn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["HL7DataConnectionString"].ToString());
                    SqlCommand cm = new SqlCommand("uspSelectHL7MessageSegmentFieldComponents", cn);
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.Parameters.Add(new SqlParameter("@HL7MessageId", SqlDbType.Int, 4));
                    cm.Parameters.Add(new SqlParameter("@SegmentId", SqlDbType.Int, 4));
                    cm.Parameters.Add(new SqlParameter("@FieldId", SqlDbType.Int, 4));
                    cm.Parameters.Add(new SqlParameter("@HL7MessageType", SqlDbType.NVarChar, 5));
                    cm.Parameters["@HL7MessageId"].Value = ViewState["HL7MessageId"];
                    cm.Parameters["@SegmentId"].Value = Convert.ToInt32(hdnParseSegmentLocation.Value);
                    cm.Parameters["@FieldId"].Value = Convert.ToInt32(hdnParseFieldLocation.Value);
                    cm.Parameters["@HL7MessageType"].Value = ViewState["HL7MessageType"];
                    cm.CommandTimeout = 0;
                    cn.Open();
                    SqlDataReader dr;
                    dr = cm.ExecuteReader();
                    gdFld.DataSource = dr;
                    gdFld.DataBind();
                    cm.Dispose();
                    cn.Close();
                    cn.Dispose();
                }
                else
                {
                    gdFld.Visible = false;
                    hdDisplay.Value = "false";
                }
            }
        }



        // The return type can be changed to IEnumerable, however to support
        // paging and sorting, the following parameters must be added:
        //     int maximumRows
        //     int startRowIndex
        //     out int totalRowCount
        //     string sortByExpression

    }
}