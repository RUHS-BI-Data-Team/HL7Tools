using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace HL7DataAnalyser
{
    public partial class HL7DataAnalyser : System.Web.UI.Page
    {
        private clsDataFunctions df = new clsDataFunctions();
        private DataSet WorkingQuery;//= new DataSet;
        string OperatorId; // "E160268";
        int CurrentQueryId;
        DataTable dtHL7FieldNameList = new DataTable("FieldNames");
        DataTable dtHL7FieldNameListCond = new DataTable("FieldNames");
        protected void Page_Load(object sender, EventArgs e)
        {
            OperatorId = Session["CurrentUser"].ToString();
            PopulateToolsFieldsList();
            if (!Page.IsPostBack)
            {
                OpenOrCreateXML();
                PopulateToolsQuery();
                BuildSQlStatement();
                DataBindHL7Grid();
                ToolsHidden.Value = "false";
                PopulateToolsResultGrid();
                PopulateToolConditionGrid();
                PopulateExistingQueryList();
            }
            else
            {
                if (!chkToolsUseDateRange.Checked)
                {
                    txtToolsStartDate.Text = hdnToolsStartDate.Value;
                    txtToolsStartTime.Text = hdnToolsStartTime.Value;
                    txtToolsEndDate.Text = hdnToolsEndDate.Value;
                    txtToolsEndTime.Text = hdnToolsEndTime.Value;
                }
            }
            CreateClientScriptsOnControls();
            if (!Page.IsPostBack)
            {
                FillFilterDefaults();
            }
        }
        protected string GetHeaderURL()
        {
            return "Header.aspx?UserName=" + Session["CurrentUser"].ToString() + "&AppName=HL7 Data Analyser";
           
        }
        protected void PopulateToolsFieldsList()
        {
            SqlConnection cn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["HL7DataAnalyserConnectionString"].ToString());
            SqlCommand cm = new SqlCommand("uspSelecttblADTColumnNames", cn);
            cm.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(cm);
            cn.Open();
            da.Fill(dtHL7FieldNameList);
            cn.Close();
            cm.Dispose();
            cn.Dispose();
            DataRow dr = dtHL7FieldNameList.NewRow();
            dr["COLUMN_NAME"] = "Parsed Value";
            dtHL7FieldNameList.Rows.Add(dr);
            dtHL7FieldNameListCond = dtHL7FieldNameList.Copy();
            foreach(DataRow r in dtHL7FieldNameListCond.Rows)
            {
                if(r[0].ToString() == "HL7Data")
                {
                    r.Delete();
                    break;
                }
            }
            dtHL7FieldNameListCond.AcceptChanges();
        }
        private void OpenOrCreateXML()
        {
            if (!File.Exists(System.Web.Configuration.WebConfigurationManager.AppSettings["userXMLQueriesPath"].ToString() + OperatorId + " Queries.XML"))
            {
                WorkingQuery = df.SetupNewQuery(OperatorId);
                //SaveXMLQueryFile();
            }
            else
            {
                WorkingQuery = new DataSet();
                WorkingQuery.ReadXml(System.Web.Configuration.WebConfigurationManager.AppSettings["userXMLQueriesPath"].ToString() + OperatorId + " Queries.XML", XmlReadMode.ReadSchema);
            }

            CurrentQueryId = GetCurrentQuery();
            WorkingQuery = df.CreateTempTablesForXML(WorkingQuery);
            SaveXMLQueryFile();
        }

        private void BuildSQlStatement()
        {
            string SQL = df.BuildQuerySyntax(WorkingQuery, CurrentQueryId);
            lblSql.Text = SQL;
        }
        private void DeleteXMLQueryFileCreateNew()
        {
            if (!File.Exists(System.Web.Configuration.WebConfigurationManager.AppSettings["userXMLQueriesPath"].ToString() + OperatorId + " Queries.XML"))
            {
                File.Delete(System.Web.Configuration.WebConfigurationManager.AppSettings["userXMLQueriesPath"].ToString() + OperatorId + " Queries.XML");
            }
           
        }
        private void SaveXMLQueryFile()
        {
            WorkingQuery.WriteXml(System.Web.Configuration.WebConfigurationManager.AppSettings["userXMLQueriesPath"].ToString() + OperatorId + " Queries.XML", XmlWriteMode.WriteSchema);
        }

        private void DataBindHL7Grid()
        {
            SqlConnection cn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["HL7DataConnectionString"].ToString());
            //SqlCommand cm = new SqlCommand(lblSql.Text, cn);
            SqlCommand cm = new SqlCommand("uspExcuteSQL", cn);
            cm.CommandType = CommandType.StoredProcedure;
            cm.Parameters.Add(new SqlParameter("@SQL", SqlDbType.NVarChar, -1));
            cm.Parameters.Add(new SqlParameter("@OperatorId",SqlDbType.NVarChar,25));
            cm.Parameters["@SQL"].Value = lblSql.Text;
            cm.Parameters["@OperatorId"].Value = OperatorId;
            cm.CommandTimeout = 0;
            cn.Open();
            DataTable dtHL7 = new DataTable("HL7Data");
            SqlDataAdapter da = new SqlDataAdapter(cm);
            da.Fill(dtHL7);
            //SqlDataReader dr = cm.ExecuteReader();
            grdHL7Data.DataSource = dtHL7;
            SetupGridView(CurrentQueryId);
            grdHL7Data.DataBind();
            cn.Close();
        }
        private int GetCurrentQuery()
        {
            var q = (from query in WorkingQuery.Tables["tblQueries"].AsEnumerable()
                     orderby query.Field<DateTime?>("QueryDate") ?? DateTime.Now descending
                     select query.Field<int>("Id")).ElementAt(0);
            return q;
        }
        private void FillFilterDefaults()
        {
            IEnumerable<DataRow> ctr = from qc in WorkingQuery.Tables["tblkQueryControls"].AsEnumerable()
                                       where (qc.Field<int?>("GroupId") ?? -1) == 2
                                       select qc;
            DataTable ctdt = ctr.CopyToDataTable<DataRow>();
            drpFilterPopupCompareType.DataSource = ctdt;
            drpFilterPopupCompareType.DataTextField = "ItemDescription";
            drpFilterPopupCompareType.DataValueField = "Id";
            drpFilterPopupCompareType.DataBind();

            IEnumerable<DataRow> jtr = from qc in WorkingQuery.Tables["tblkQueryControls"].AsEnumerable()
                                       where (qc.Field<int?>("GroupId") ?? -1) == 1
                                       select qc;
            DataTable jtdt = jtr.CopyToDataTable<DataRow>();
            drpFilterPopupJoinType.DataSource = jtdt;
            drpFilterPopupJoinType.DataTextField = "ItemDescription";
            drpFilterPopupJoinType.DataValueField = "Id";
            drpFilterPopupJoinType.DataBind();
        }
        private void SetupGridView(int QueryId)
        {
            grdHL7Data.Columns.Clear();
            BoundField bfId = new BoundField();
            bfId.HeaderText = "Id";
            bfId.DataField = "Id";
            bfId.Visible = false;
            grdHL7Data.Columns.Add(bfId);
            TemplateField tfBtn = new TemplateField();
            tfBtn.HeaderText ="View";
            tfBtn.ItemTemplate = new HL7DataTemplete(ListItemType.Item, "ADT");
            grdHL7Data.Columns.Add(tfBtn);
            IEnumerable<DataRow> r = from qr in WorkingQuery.Tables["tblResultFields"].AsEnumerable()
                                     where qr.Field<int>("QueryId") == QueryId
                                     orderby qr.Field<int>("QueryResultOrder")
                                     select qr;
            TemplateField tblk = new TemplateField();
            tblk.ItemTemplate = new GridSpacerTemplete(ListItemType.Item, " ");
            grdHL7Data.Columns.Add(tblk);

            foreach (DataRow qr in r)
            {
                //BoundField bf = new BoundField();
                TemplateField tf = new TemplateField();
                tf.HeaderText = GetColumnAlias(qr);
                if (qr.Field<string>("FieldName") == "HL7Data")
                {
                    tf.ItemTemplate = new HL7DataTemplete(ListItemType.Item,"ADT");
                }
                else
                {
                    if ((qr.Field<int?>("HL7ParseLocation") ?? -1) > -1)
                    {
                        tf.ItemTemplate = new HL7ParsedDataTemplete(ListItemType.Item, qr.Field<string>("FieldName").Replace(".", ""), qr.Field<string>("FieldName"));
                    }
                    else
                    {
                        tf.ItemTemplate = new HL7ParsedDataTemplete(ListItemType.Item, qr.Field<string>("FieldName"));
                    }
                    //bf.HeaderText = GetColumnAlias(qr);
                    //bf.Visible = true;
                    
                }
                grdHL7Data.Columns.Add(tf);
                grdHL7Data.Columns.Add(tblk);
            }
            grdHL7Data.EnableViewState = true;
        }
        private void PopulateToolsQuery()
        {
            DataRow q = df.GetQueryInformation(WorkingQuery, CurrentQueryId);
            txtToolsQueryName.Text = q.Field<string>("QueryName").ToString();
            txtToolsQueryDescription.Text = q.Field<string>("QueryDescription").ToString();
            txtToolsStartDate.Text = q.Field<DateTime>("StartDate").ToString("yyyy-MM-dd");
            txtToolsStartTime.Text = q.Field<DateTime>("StartDate").ToString("HH:mm");
            txtToolsEndDate.Text = q.Field<DateTime>("EndDate").ToString("yyyy-MM-dd");
            txtToolsEndTime.Text = q.Field<DateTime>("EndDate").ToString("HH:mm");
            chkToolsUseDateRange.Checked = q.Field<bool>("UseDates");
            txtToolsStartDate.Enabled = chkToolsUseDateRange.Checked;
            txtToolsStartTime.Enabled = chkToolsUseDateRange.Checked;
            txtToolsEndDate.Enabled = chkToolsUseDateRange.Checked;
            txtToolsEndTime.Enabled = chkToolsUseDateRange.Checked;
            hdnToolsStartDate.Value = txtToolsStartDate.Text;
            hdnToolsStartTime.Value = txtToolsStartTime.Text;
            hdnToolsEndDate.Value = txtToolsEndDate.Text;
            hdnToolsEndTime.Value = txtToolsEndTime.Text;
        }
        private string GetColumnAlias(DataRow r)
        {
            if (r["ColumnAlias"].ToString() == "")
            {
                return r.Field<string>("FieldName").Replace(".", "");
            }
            else
            {
                return r.Field<string>("ColumnAlias");
            }
        }
        private void CreateClientScriptsOnControls()
        {
            string ScriptStart = "\n<script type=\"text/javascript\" language=\"Javascript\" id=\"EventScriptBlock\">\n";
            string ScriptEnd = "\n </script>";
            string JavaScript = "$('#" + pnlAddToFilter.ClientID + "').hide();\n" +
                "$('#" + pnlCreateOpenQuery.ClientID + "').hide();\n";// +
           // "function ShowPopupFilterWindow(FieldName, FieldValue, ControlId, isParsingField){\n" +
           // "var e = window.event;\n" +
           //"if(e.button==2){\n" +
           // "$('#" + pnlAddToFilter.ClientID + "').show();\n" +
           // "$('#" + txtFilterPopupFieldName.ClientID + "').val(FieldName);\n" +
           // "$('#" + txtFilterPopupFieldValue.ClientID + "').val(FieldValue);\n" +
           // "if(!isParsingField){\n" +
           // "      $('#" + txtFilterPopupParseLocation.ClientID + "').hide();\n" +
           // "      $('#" + txtFilterPopupParseLocation.ClientID + "').val(-1);\n" +
           // "  }else{\n" +
           // "     $('#" + txtFilterPopupParseLocation.ClientID + "').show();\n" +
           // "     $('#" + txtFilterPopupParseLocation.ClientID + "').val(1);\n" +
           // "  }\n" +
           // "var o = { left: e.clientX, top: e.clientY };\n" +
           // "$('#pnlAddToFilter').offset(o);\n" +
           // "return false;\n" +
           // "}\n}\n";
            //add tools visible property based on last setting
            if (ToolsHidden.Value == "true")
            {
                JavaScript = JavaScript + "fnHideTools();\n";
            }
            else
            {
                JavaScript = JavaScript + "fnShowTools();\n";
            }
            JavaScript = JavaScript + "function EnableToolsDateRange(){\n" +
                "$('#" + txtToolsEndDate.ClientID + "').prop('disabled', !$('#" + chkToolsUseDateRange.ClientID + "').prop('checked'));\n" +
                "$('#" + txtToolsStartDate.ClientID + "').prop('disabled', !$('#" + chkToolsUseDateRange.ClientID + "').prop('checked'));\n" +
                "$('#" + txtToolsEndTime.ClientID + "').prop('disabled', !$('#" + chkToolsUseDateRange.ClientID + "').prop('checked'));\n" +
                "$('#" + txtToolsStartTime.ClientID + "').prop('disabled', !$('#" + chkToolsUseDateRange.ClientID + "').prop('checked'));\n" +
                "}\n";
            JavaScript = JavaScript + "EnableToolsDateRange();\n";

            this.ClientScript.RegisterStartupScript(this.GetType(), "MasterKey", ScriptStart + JavaScript + ScriptEnd);
            //btnCancelFilter.Attributes.Add("onClick", "$('#" + pnlAddToFilter.ClientID + "').hide(); return false;");
        }
        private void UpdateQueryInformation()
        {

            DataRow[] dr = WorkingQuery.Tables["tblQueries"].Select("Id = " + CurrentQueryId);
            dr[0]["QueryName"] = txtToolsQueryName.Text;
            dr[0]["QueryDescription"] = txtToolsQueryDescription.Text;
            if (chkToolsUseDateRange.Checked)
            {
                dr[0]["StartDate"] = Convert.ToDateTime(txtToolsStartDate.Text + " " + txtToolsStartTime.Text);
                dr[0]["EndDate"] = Convert.ToDateTime(txtToolsEndDate.Text + " " + txtToolsEndTime.Text);
                hdnToolsStartDate.Value = txtToolsStartDate.Text;
                hdnToolsStartTime.Value = txtToolsStartTime.Text;
                hdnToolsEndDate.Value = txtToolsEndDate.Text;
                hdnToolsEndTime.Value = txtToolsEndTime.Text;
            }
            else
            {
                txtToolsStartDate.Text = hdnToolsStartDate.Value;
                txtToolsStartTime.Text = hdnToolsStartTime.Value;
                txtToolsEndDate.Text = hdnToolsEndDate.Value;
                txtToolsEndTime.Text = hdnToolsEndTime.Value;
            }
            dr[0]["UseDates"] = chkToolsUseDateRange.Checked;
            WorkingQuery.Tables["tblQueries"].AcceptChanges();
            SaveXMLQueryFile();
        }
        protected String FormatOrderByDirectionForTools(object OrderByDirection)
        {
            if (OrderByDirection == DBNull.Value)
            {
                return "N/A";
            }
            var q = (from query in WorkingQuery.Tables["tblkQueryControls"].AsEnumerable()
                     where query.Field<int>("Id") == Convert.ToInt16(OrderByDirection)
                     select query.Field<string>("ItemDescription")).ElementAt(0);
            return q.ToString();
        }
        protected String FormatConditionTypesForTools(object ConditionType)
        {
            var q = (from query in WorkingQuery.Tables["tblkQueryControls"].AsEnumerable()
                     where query.Field<int>("Id") == Convert.ToInt16(ConditionType)
                     select query.Field<string>("ItemDescription")).ElementAt(0);
            return q.ToString();
        }
        protected string FormatFieldNameForTools(object FieldName, object HL7ParseLocation)
        {
            if (HL7ParseLocation == DBNull.Value)
            {
                HL7ParseLocation = -1;
            }
            if (Convert.ToInt16(HL7ParseLocation) > 0)
            {
                return Convert.ToString(FieldName) + "(" + HL7ParseLocation + ")";
            }
            else
            {
                return Convert.ToString(FieldName);
            }
        }
        protected void btnAddFilter_Click(object sender, EventArgs e)
        {
            OpenOrCreateXML();
            DataRow dr = WorkingQuery.Tables["tmpConditionFields"].NewRow();
            dr["FieldName"] = txtFilterPopupFieldName.Text;
            dr["ConditionValue"] = txtFilterPopupFieldValue.Text;
            dr["QueryId"] = CurrentQueryId;
            dr["QueryConditionOrder"] = WorkingQuery.Tables["tmpConditionFields"].Rows.Count + 1;
            dr["JoinType"] = Convert.ToInt16(drpFilterPopupJoinType.SelectedValue);
            dr["CompareType"] = Convert.ToInt16(drpFilterPopupCompareType.SelectedValue);
            dr["UseCondition"] = true;
            dr["HL7ParseLocation"] = txtFilterPopupParseLocation.Text;
            WorkingQuery.Tables["tmpConditionFields"].Rows.Add(dr);
            WorkingQuery.Tables["tmpConditionFields"].AcceptChanges();
            if (chkFilterPopupInclude.Checked)
            {
                df.AddConditionField(ref WorkingQuery, GetCurrentQuery(), txtFilterPopupFieldName.Text, txtFilterPopupFieldValue.Text,
                    Convert.ToInt16(drpFilterPopupCompareType.SelectedValue),
                    Convert.ToInt16(drpFilterPopupJoinType.SelectedValue),
                    true, Convert.ToInt16(txtFilterPopupParseLocation.Text));
                SaveXMLQueryFile();
                BuildSQlStatement();
            }
            else
            {
                SaveXMLQueryFile();
            }
            PopulateToolsResultGrid();
            PopulateToolConditionGrid();
            DataBindHL7Grid();
            //acrnTools.SelectedIndex = 2;
        }

        protected void grdHL7Data_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grdHL7Data.PageIndex = e.NewPageIndex;
            OpenOrCreateXML();
            DataBindHL7Grid();
        }

        protected void btnToolsCancelQuery_Click(object sender, EventArgs e)
        {
            OpenOrCreateXML();
            DataBindHL7Grid();
            PopulateToolsQuery();
            PopulateToolConditionGrid();
        }

        protected void btnToolsUpdateQuery_Click(object sender, EventArgs e)
        {
            OpenOrCreateXML();
            UpdateQueryInformation();
            BuildSQlStatement();
            DataBindHL7Grid();
        }

        protected void btnToolsSaveQuery_Click(object sender, EventArgs e)
        {
            OpenOrCreateXML();
            UpdateQueryInformation();
            DataRow[] dr = WorkingQuery.Tables["tblQueries"].Select("Id = " + CurrentQueryId);
            CurrentQueryId = df.SaveQueryToDatabase(WorkingQuery);
            dr[0]["Id"] = CurrentQueryId;
            foreach (DataRow tdr in WorkingQuery.Tables["tmpResultFields"].Rows)
            {
                tdr["QueryId"] = CurrentQueryId;
            }
            foreach (DataRow tdr in WorkingQuery.Tables["tmpConditionFields"].Rows)
            {
                tdr["QueryId"] = CurrentQueryId;
            }
            WorkingQuery.Tables["tmpResultFields"].AcceptChanges();
            WorkingQuery.Tables["tmpConditionFields"].AcceptChanges();
            WorkingQuery.Tables["tblQueries"].AcceptChanges();
            SaveXMLQueryFile();
            PopulateExistingQueryList();
            DataBindHL7Grid();
        }
        private void PopulateToolsResultGrid()
        {
            IEnumerable<DataRow> r = from qr in WorkingQuery.Tables["tmpResultFields"].AsEnumerable()
                                     where qr.Field<int>("QueryId") == CurrentQueryId
                                     orderby qr.Field<int>("QueryResultOrder")
                                     select qr;
            grdToolsResults.DataSource = null;
            if (r.Any())
            {
                grdToolsResults.DataSource = r.CopyToDataTable();
            }
            grdToolsResults.DataBind();
        }
        private void PopulateToolConditionGrid()
        {
            IEnumerable<DataRow> r = from qr in WorkingQuery.Tables["tmpConditionFields"].AsEnumerable()
                                     where qr.Field<int>("QueryId") == CurrentQueryId
                                     orderby qr.Field<int>("QueryConditionOrder")
                                     select qr;
            //DataTable dt = new DataTable("ConditionFields");
            grdToolsCondition.DataSource = null;
            if (r.Any())
            {

                grdToolsCondition.DataSource = r.CopyToDataTable();

            }
            grdToolsCondition.DataBind();
        }

        protected void grdToolsResults_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ToolsMoveUp" || e.CommandName == "ToolsMoveDown")
            {
                OpenOrCreateXML();

                if (e.CommandName == "ToolsMoveUp" && Convert.ToInt32(e.CommandArgument) != 1)
                {
                    DataRow[] dr = WorkingQuery.Tables["tmpResultFields"].Select("QueryResultOrder <= " + e.CommandArgument);
                    foreach (DataRow r in dr)
                    {
                        if (Convert.ToInt32(r["QueryResultOrder"]) == Convert.ToInt32(e.CommandArgument))
                        {
                            r["QueryResultOrder"] = Convert.ToInt32(r["QueryResultOrder"]) - 1;
                        }
                        else if (Convert.ToInt32(r["QueryResultOrder"]) == Convert.ToInt32(e.CommandArgument) - 1)
                        {
                            r["QueryResultOrder"] = Convert.ToInt32(r["QueryResultOrder"]) + 1;
                        }
                    }
                    WorkingQuery.Tables["tmpResultFields"].AcceptChanges();
                    SaveXMLQueryFile();
                    PopulateToolsResultGrid();
                    //PopulateToolConditionGrid();
                }
                else if (e.CommandName == "ToolsMoveDown" && Convert.ToInt32(e.CommandArgument) != WorkingQuery.Tables["tmpResultFields"].Rows.Count)
                {
                    DataRow[] dr = WorkingQuery.Tables["tmpResultFields"].Select("QueryResultOrder >= " + e.CommandArgument);
                    foreach (DataRow r in dr)
                    {
                        if (Convert.ToInt32(r["QueryResultOrder"]) == Convert.ToInt32(e.CommandArgument))
                        {
                            r["QueryResultOrder"] = Convert.ToInt32(r["QueryResultOrder"]) + 1;
                        }
                        else if (Convert.ToInt32(r["QueryResultOrder"]) == Convert.ToInt32(e.CommandArgument) + 1)
                        {
                            r["QueryResultOrder"] = Convert.ToInt32(r["QueryResultOrder"]) - 1;
                        }
                    }
                    WorkingQuery.Tables["tmpResultFields"].AcceptChanges();
                    SaveXMLQueryFile();
                    PopulateToolsResultGrid();
                    //PopulateToolConditionGrid();
                }
                //BuildSQlStatement();
                //DataBindHL7Grid();
            }
        }

        protected void grdToolsResults_RowEditing(object sender, GridViewEditEventArgs e)
        {
            OpenOrCreateXML();
            grdToolsResults.EditIndex = e.NewEditIndex;
            PopulateToolsResultGrid();
            //PopulateToolConditionGrid();
            //DataBindHL7Grid();
        }

        protected void grdToolsResults_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            OpenOrCreateXML();
            DataRow[] dr = WorkingQuery.Tables["tmpResultFields"].Select("Id = " + grdToolsResults.DataKeys[e.RowIndex].Value.ToString());
            GridViewRow grd = (GridViewRow)grdToolsResults.Rows[e.RowIndex];
            DropDownList drpFieldName = (DropDownList)grd.FindControl("drpToolsResultsFieldNames");
            DropDownList drpOrderByDirection = (DropDownList)grd.FindControl("drpToolsResultsOrderDirection");
            TextBox txtOrderBy = (TextBox)grd.FindControl("txtToolsResultsOrderBy");
            TextBox txtColumnAlias = (TextBox)grd.FindControl("txtToolsResultsColumnAlias");
            TextBox txtHL7 = (TextBox)grd.FindControl("txtToolsResultsHL7");
            TextBox txtLocation = (TextBox)grd.FindControl("txtToolsParseLocation");
            if (drpFieldName.SelectedValue != "Parsed Value")
            {
                dr[0]["FieldName"] = drpFieldName.SelectedValue;
                dr[0]["HL7ParseLocation"] = DBNull.Value;
            }
            else if (txtHL7.Text.Length < 4)
            {
                e.Cancel = true;
                return;
            }
            else
            {
                dr[0]["FieldName"] = txtHL7.Text;
                if (txtLocation.Text == "")
                {
                    dr[0]["HL7ParseLocation"] = 0;
                }
                else
                {
                    dr[0]["HL7ParseLocation"] = txtLocation.Text;
                }
            }
            if (txtColumnAlias.Text == "")
            {
                dr[0]["ColumnAlias"] = DBNull.Value;
            }
            else
            {
                dr[0]["ColumnAlias"] = txtColumnAlias.Text;
            }
            if (txtOrderBy.Text == "")
            {
                dr[0]["OrderBy"] = DBNull.Value;
                dr[0]["OrderByDirection"] = DBNull.Value;
            }
            else
            {
                dr[0]["OrderBy"] = txtOrderBy.Text;
                dr[0]["OrderByDirection"] = drpOrderByDirection.SelectedValue;
            }
            WorkingQuery.Tables["tmpResultFields"].AcceptChanges();
            SaveXMLQueryFile();
            grdToolsResults.EditIndex = -1;
            PopulateToolsResultGrid();
            //PopulateToolConditionGrid();
            //DataBindHL7Grid();
        }

        protected void grdToolsResults_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && e.Row.RowState.HasFlag(DataControlRowState.Edit))
            {

                DropDownList drpFields = (e.Row.FindControl("drpToolsResultsFieldNames") as DropDownList);
                drpFields.DataSource = dtHL7FieldNameList;
                drpFields.DataValueField = "COLUMN_NAME";
                drpFields.DataTextField = "COLUMN_NAME";
                drpFields.DataBind();
                DataRowView drv = (DataRowView)e.Row.DataItem;
                TextBox txtHL7 = (e.Row.FindControl("txtToolsResultsHL7") as TextBox);
                TextBox txtLocation = (e.Row.FindControl("txtToolsParseLocation") as TextBox);
                TextBox txtOrderBy = (e.Row.FindControl("txtToolsResultsOrderBy") as TextBox);
                Label lblHL7 = (e.Row.FindControl("lblToolsResultsHL7") as Label);
                Label lblLocation = (e.Row.FindControl("lblToolsParseLocation") as Label);
                DropDownList drpDirection = (e.Row.FindControl("drpToolsResultsOrderDirection") as DropDownList);
                DataTable dtDirection = WorkingQuery.Tables["tblkQueryControls"].Select("GroupId = 8").CopyToDataTable();
                DataRow drBlankDirection = dtDirection.NewRow();
                drBlankDirection["Id"] = 0;
                drBlankDirection["ItemDescription"] = "N/A";
                dtDirection.Rows.InsertAt(drBlankDirection, 0);
                drpDirection.DataSource = dtDirection;
                drpDirection.DataValueField = "Id";
                drpDirection.DataTextField = "ItemDescription";
                drpDirection.DataBind();
                if (drv["HL7ParseLocation"] == DBNull.Value)
                {
                    drpFields.SelectedValue = drv["FieldName"].ToString();
                    txtHL7.Style.Add("display", "none");
                    txtLocation.Style.Add("display", "none");
                    lblHL7.Style.Add("display", "none");
                    lblLocation.Style.Add("display", "none");

                }
                else
                {
                    drpFields.SelectedValue = "Parsed Value";//FormatFieldNameForTools(drv["FieldName"].ToString(), drv["HL7ParseLocation"].ToString());
                    txtLocation.Text = drv["HL7ParseLocation"].ToString();
                    txtHL7.Text = drv["FieldName"].ToString();
                }
                if (drv["OrderByDirection"] == DBNull.Value)
                {
                    drpDirection.SelectedValue = "0";
                    txtOrderBy.Enabled = false;
                }
                else
                {
                    drpDirection.SelectedValue = drv["OrderByDirection"].ToString();
                    txtOrderBy.Enabled = true;
                }


                drpFields.Attributes.Add("onchange", "ShowParsedOptions('" + lblHL7.ClientID +
                        "','" + txtHL7.ClientID + "','" + lblLocation.ClientID + "','" + txtLocation.ClientID +
                        "','" + drpFields.ClientID + "');");
                drpDirection.Attributes.Add("onchange", "EnableToolsResultOrderBy('" + txtOrderBy.ClientID + "','" + drpDirection.ClientID + "');");
            }
        }
        private void ReorderResultDisplaySequence()
        {
            IEnumerable<DataRow> dr = from qr in WorkingQuery.Tables["tmpResultFields"].AsEnumerable()
                                      where qr.Field<int>("QueryId") == CurrentQueryId
                                      orderby qr.Field<int>("QueryResultOrder") ascending
                                      select qr;
            int i = 1;
            foreach (DataRow r in dr)
            {
                r["QueryResultOrder"] = i;
                i++;
            }
        }
        private void ReorderConditionDisplaySequence()
        {
            IEnumerable<DataRow> dr = from qr in WorkingQuery.Tables["tmpConditionFields"].AsEnumerable()
                                      where qr.Field<int>("QueryId") == CurrentQueryId
                                      orderby qr.Field<int>("QueryConditionOrder") ascending
                                      select qr;
            int i = 1;
            foreach (DataRow r in dr)
            {
                r["QueryConditionOrder"] = i;
                i++;
            }
        }
        protected void grdToolsResults_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            OpenOrCreateXML();
            var dr = WorkingQuery.Tables["tmpResultFields"].Select("Id = " + e.Keys[0].ToString());
            foreach (var r in dr)
            {
                r.Delete();
            }
            ReorderResultDisplaySequence();
            SaveXMLQueryFile();
            PopulateToolsResultGrid();
            //PopulateToolConditionGrid();
            //DataBindHL7Grid();
        }

        protected void btnToolsResultsAdd_Click(object sender, EventArgs e)
        {
            OpenOrCreateXML();
            DataRow dr = WorkingQuery.Tables["tmpResultFields"].NewRow();
            dr["FieldName"] = "HL7Data";
            dr["ColumnAlias"] = "";
            dr["QueryId"] = CurrentQueryId;
            dr["QueryResultOrder"] = WorkingQuery.Tables["tmpResultFields"].Rows.Count + 1;
            WorkingQuery.Tables["tmpResultFields"].Rows.Add(dr);
            WorkingQuery.Tables["tmpResultFields"].AcceptChanges();
            SaveXMLQueryFile();
            grdToolsResults.EditIndex = WorkingQuery.Tables["tmpResultFields"].Rows.Count - 1;
            PopulateToolsResultGrid();
            //PopulateToolConditionGrid();
            //DataBindHL7Grid();
        }

        protected void drpToolsResultsFieldNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList drp = sender as DropDownList;


        }

        protected void grdToolsResults_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            OpenOrCreateXML();
            grdToolsResults.EditIndex = -1;
            PopulateToolsResultGrid();
            //PopulateToolConditionGrid();
            //DataBindHL7Grid();
        }

        protected void btnToolsResultsApply_Click(object sender, EventArgs e)
        {
            OpenOrCreateXML();
            WorkingQuery.Tables["tblResultFields"].Clear();
            IEnumerable<DataRow> dr = from qr in WorkingQuery.Tables["tmpResultFields"].AsEnumerable()
                                      where qr.Field<int>("QueryId") == CurrentQueryId
                                      orderby qr.Field<int>("QueryResultOrder") ascending
                                      select qr;
            foreach (DataRow r in dr)
            {
                DataRow tmp = WorkingQuery.Tables["tblResultFields"].NewRow();
                for (int i = 0; i < tmp.Table.Columns.Count; i++)
                {
                    tmp[i] = r[i];
                }
                WorkingQuery.Tables["tblResultFields"].Rows.Add(tmp);
            }
            WorkingQuery.Tables["tblResultFields"].AcceptChanges();
            SaveXMLQueryFile();
            BuildSQlStatement();
            PopulateToolsResultGrid();
            //PopulateToolConditionGrid();
            DataBindHL7Grid();
        }

        protected void btnToolsResultsCancel_Click(object sender, EventArgs e)
        {
            OpenOrCreateXML();
            WorkingQuery.Tables["tmpResultFields"].Clear();
            IEnumerable<DataRow> dr = from qr in WorkingQuery.Tables["tblResultFields"].AsEnumerable()
                                      where qr.Field<int>("QueryId") == CurrentQueryId
                                      orderby qr.Field<int>("QueryResultOrder") ascending
                                      select qr;
            foreach (DataRow r in dr)
            {
                DataRow tmp = WorkingQuery.Tables["tmpResultFields"].NewRow();
                for (int i = 0; i < tmp.Table.Columns.Count; i++)
                {
                    tmp[i] = r[i];
                }
                WorkingQuery.Tables["tmpResultFields"].Rows.Add(tmp);
            }
            WorkingQuery.Tables["tmpResultFields"].AcceptChanges();
            SaveXMLQueryFile();
            //PopulateToolConditionGrid();
            PopulateToolsResultGrid();
            //DataBindHL7Grid();
        }
        protected string FormatStringLength(object value, Int16 MaxLength)
        {
            string s = Convert.ToString(value);
            if (s.Length <= MaxLength)
            {
                return s;
            }
            else
            {
                return s.Substring(0, MaxLength - 4) + "...";
            }
        }

        protected void btnToolsConditionAdd_Click(object sender, EventArgs e)
        {
            OpenOrCreateXML();
            DataRow dr = WorkingQuery.Tables["tmpConditionFields"].NewRow();
            dr["FieldName"] = "MRN";
            dr["ConditionValue"] = "";
            dr["QueryId"] = CurrentQueryId;
            dr["QueryConditionOrder"] = WorkingQuery.Tables["tmpConditionFields"].Rows.Count + 1;
            dr["JoinType"] = 3;
            dr["CompareType"] = 5;
            dr["UseCondition"] = true;
            WorkingQuery.Tables["tmpConditionFields"].Rows.Add(dr);
            WorkingQuery.Tables["tmpConditionFields"].AcceptChanges();
            SaveXMLQueryFile();
            grdToolsCondition.EditIndex = WorkingQuery.Tables["tmpConditionFields"].Rows.Count - 1;
            //PopulateToolsResultGrid();
            PopulateToolConditionGrid();
            //DataBindHL7Grid();
        }

        protected void btnToolsConditionCancel_Click(object sender, EventArgs e)
        {
            OpenOrCreateXML();
            WorkingQuery.Tables["tmpConditionFields"].Clear();
            IEnumerable<DataRow> dr = from qr in WorkingQuery.Tables["tblConditionFields"].AsEnumerable()
                                      where qr.Field<int>("QueryId") == CurrentQueryId
                                      orderby qr.Field<int>("QueryConditionOrder") ascending
                                      select qr;
            foreach (DataRow r in dr)
            {
                DataRow tmp = WorkingQuery.Tables["tmpConditionFields"].NewRow();
                for (int i = 0; i < tmp.Table.Columns.Count; i++)
                {
                    tmp[i] = r[i];
                }
                WorkingQuery.Tables["tmpConditionFields"].Rows.Add(tmp);
            }
            WorkingQuery.Tables["tmpConditionFields"].AcceptChanges();
            SaveXMLQueryFile();
            PopulateToolConditionGrid();
            //PopulateToolsResultGrid();
            //DataBindHL7Grid();
        }

        protected void btnToolsConditionApply_Click(object sender, EventArgs e)
        {
            OpenOrCreateXML();
            WorkingQuery.Tables["tblConditionFields"].Clear();
            IEnumerable<DataRow> dr = from qr in WorkingQuery.Tables["tmpConditionFields"].AsEnumerable()
                                      where qr.Field<int>("QueryId") == CurrentQueryId
                                      orderby qr.Field<int>("QueryConditionOrder") ascending
                                      select qr;
            foreach (DataRow r in dr)
            {
                DataRow tmp = WorkingQuery.Tables["tblConditionFields"].NewRow();
                for (int i = 0; i < tmp.Table.Columns.Count; i++)
                {
                    tmp[i] = r[i];
                }
                WorkingQuery.Tables["tblConditionFields"].Rows.Add(tmp);
            }
            WorkingQuery.Tables["tblConditionFields"].AcceptChanges();
            SaveXMLQueryFile();
            BuildSQlStatement();
            //PopulateToolsResultGrid();
            PopulateToolConditionGrid();
            DataBindHL7Grid();
        }

        protected void grdToolsCondition_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ToolsMoveUp" || e.CommandName == "ToolsMoveDown")
            {
                OpenOrCreateXML();

                if (e.CommandName == "ToolsMoveUp" && Convert.ToInt32(e.CommandArgument) != 1)
                {
                    DataRow[] dr = WorkingQuery.Tables["tmpConditionFields"].Select("QueryConditionOrder <= " + e.CommandArgument);
                    foreach (DataRow r in dr)
                    {
                        if (Convert.ToInt32(r["QueryConditionOrder"]) == Convert.ToInt32(e.CommandArgument))
                        {
                            r["QueryConditionOrder"] = Convert.ToInt32(r["QueryConditionOrder"]) - 1;
                        }
                        else if (Convert.ToInt32(r["QueryConditionOrder"]) == Convert.ToInt32(e.CommandArgument) - 1)
                        {
                            r["QueryConditionOrder"] = Convert.ToInt32(r["QueryConditionOrder"]) + 1;
                        }
                    }
                    WorkingQuery.Tables["tmpConditionFields"].AcceptChanges();
                    SaveXMLQueryFile();
                    //PopulateToolsResultGrid();
                    PopulateToolConditionGrid();
                }
                else if (e.CommandName == "ToolsMoveDown" && Convert.ToInt32(e.CommandArgument) != WorkingQuery.Tables["tmpConditionFields"].Rows.Count)
                {
                    DataRow[] dr = WorkingQuery.Tables["tmpConditionFields"].Select("QueryConditionOrder >= " + e.CommandArgument);
                    foreach (DataRow r in dr)
                    {
                        if (Convert.ToInt32(r["QueryConditionOrder"]) == Convert.ToInt32(e.CommandArgument))
                        {
                            r["QueryConditionOrder"] = Convert.ToInt32(r["QueryConditionOrder"]) + 1;
                        }
                        else if (Convert.ToInt32(r["QueryConditionOrder"]) == Convert.ToInt32(e.CommandArgument) + 1)
                        {
                            r["QueryConditionOrder"] = Convert.ToInt32(r["QueryConditionOrder"]) - 1;
                        }
                    }
                    WorkingQuery.Tables["tmpConditionFields"].AcceptChanges();
                    SaveXMLQueryFile();
                    //PopulateToolsResultGrid();
                    PopulateToolConditionGrid();
                }
                //DataBindHL7Grid();
            }
        }

        protected void grdToolsCondition_RowEditing(object sender, GridViewEditEventArgs e)
        {
            OpenOrCreateXML();
            grdToolsCondition.EditIndex = e.NewEditIndex;
            //PopulateToolsResultGrid();
            PopulateToolConditionGrid();
            //DataBindHL7Grid();
        }

        protected void grdToolsCondition_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            OpenOrCreateXML();
            DataRow[] dr = WorkingQuery.Tables["tmpConditionFields"].Select("Id = " + grdToolsCondition.DataKeys[e.RowIndex].Value.ToString());
            GridViewRow grd = (GridViewRow)grdToolsCondition.Rows[e.RowIndex];
            DropDownList drpFieldName = (DropDownList)grd.FindControl("drpToolsConditionFieldNames");
            DropDownList drpJoin = (grd.FindControl("drpToolsConditionJoinType") as DropDownList);
            DropDownList drpCompare = (grd.FindControl("drpToolsConditionCompareType") as DropDownList);
            TextBox txtValue = (TextBox)grd.FindControl("txtToolsConditionValue");
            TextBox txtHL7 = (grd.FindControl("txtToolsConditionHL7") as TextBox);
            TextBox txtLocation = (grd.FindControl("txtToolsConditionParseLocation") as TextBox);
            CheckBox chkUse = (grd.FindControl("chkToolsConditionUse") as CheckBox);
            if (drpFieldName.SelectedValue != "Parsed Value")
            {
                dr[0]["FieldName"] = drpFieldName.SelectedValue;
                dr[0]["HL7ParseLocation"] = DBNull.Value;
            }
            else if (txtHL7.Text.Length < 4)
            {
                e.Cancel = true;
                return;
            }
            else
            {
                dr[0]["FieldName"] = txtHL7.Text;
                if (txtLocation.Text == "")
                {
                    dr[0]["HL7ParseLocation"] = 0;
                }
                else
                {
                    dr[0]["HL7ParseLocation"] = txtLocation.Text;
                }
            }
            dr[0]["ConditionValue"] = txtValue.Text;
            dr[0]["JoinType"] = drpJoin.SelectedValue;
            dr[0]["CompareType"] = drpCompare.SelectedValue;
            dr[0]["UseCondition"] = chkUse.Checked;
            WorkingQuery.Tables["tmpConditionFields"].AcceptChanges();
            SaveXMLQueryFile();
            grdToolsCondition.EditIndex = -1;
            //PopulateToolsResultGrid();
            PopulateToolConditionGrid();
            //DataBindHL7Grid();
        }

        protected void grdToolsCondition_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && e.Row.RowState.HasFlag(DataControlRowState.Edit))
            {

                DropDownList drpFields = (e.Row.FindControl("drpToolsConditionFieldNames") as DropDownList);
                drpFields.DataSource = dtHL7FieldNameListCond;
                drpFields.DataValueField = "COLUMN_NAME";
                drpFields.DataTextField = "COLUMN_NAME";
                drpFields.DataBind();
                DataRowView drv = (DataRowView)e.Row.DataItem;
                TextBox txtHL7 = (e.Row.FindControl("txtToolsConditionHL7") as TextBox);
                TextBox txtLocation = (e.Row.FindControl("txtToolsConditionParseLocation") as TextBox);
                //TextBox txtOrderBy = (e.Row.FindControl("txtToolsResultsOrderBy") as TextBox);
                Label lblHL7 = (e.Row.FindControl("lblToolsConditionHL7") as Label);
                Label lblLocation = (e.Row.FindControl("lblToolsConditionParseLocation") as Label);
                DropDownList drpJoin = (e.Row.FindControl("drpToolsConditionJoinType") as DropDownList);
                DropDownList drpCompare = (e.Row.FindControl("drpToolsConditionCompareType") as DropDownList);
                DataTable dtJoin = WorkingQuery.Tables["tblkQueryControls"].Select("GroupId = 1").CopyToDataTable();
                DataTable dtCompare = WorkingQuery.Tables["tblkQueryControls"].Select("GroupId = 2").CopyToDataTable();
                drpJoin.DataSource = dtJoin;
                drpJoin.DataValueField = "Id";
                drpJoin.DataTextField = "ItemDescription";
                drpJoin.DataBind();
                drpCompare.DataSource = dtCompare;
                drpCompare.DataValueField = "Id";
                drpCompare.DataTextField = "ItemDescription";
                drpCompare.DataBind();
                drpJoin.SelectedValue = drv["JoinType"].ToString();
                drpCompare.SelectedValue = drv["CompareType"].ToString();
                int ParsedLoc;
                if (drv["HL7ParseLocation"] == DBNull.Value)
                {
                    ParsedLoc = -1;
                }
                else
                {
                    ParsedLoc = Convert.ToInt16(drv["HL7ParseLocation"]);
                }
                    if (ParsedLoc == -1)
                {
                    drpFields.SelectedValue = drv["FieldName"].ToString();
                    txtHL7.Style.Add("display", "none");
                    txtLocation.Style.Add("display", "none");
                    lblHL7.Style.Add("display", "none");
                    lblLocation.Style.Add("display", "none");

                }
                else
                {
                    drpFields.SelectedValue = "Parsed Value";//FormatFieldNameForTools(drv["FieldName"].ToString(), drv["HL7ParseLocation"].ToString());
                    txtLocation.Text = drv["HL7ParseLocation"].ToString();
                    txtHL7.Text = drv["FieldName"].ToString();
                }

                drpFields.Attributes.Add("onchange", "ShowParsedOptions('" + lblHL7.ClientID +
                        "','" + txtHL7.ClientID + "','" + lblLocation.ClientID + "','" + txtLocation.ClientID +
                        "','" + drpFields.ClientID + "');");
                //drpDirection.Attributes.Add("onchange", "EnableToolsResultOrderBy('" + txtOrderBy.ClientID + "','" + drpDirection.ClientID + "');");
            }
        }

        protected void grdToolsCondition_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            OpenOrCreateXML();
            var dr = WorkingQuery.Tables["tmpConditionFields"].Select("Id = " + e.Keys[0].ToString());
            foreach (var r in dr)
            {
                r.Delete();
            }
            ReorderConditionDisplaySequence();
            SaveXMLQueryFile();
            //PopulateToolsResultGrid();
            PopulateToolConditionGrid();
            //DataBindHL7Grid();
        }

        protected void grdToolsCondition_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            OpenOrCreateXML();
            grdToolsCondition.EditIndex = -1;
            //PopulateToolsResultGrid();
            PopulateToolConditionGrid();
            //DataBindHL7Grid();
        }
        private void PopulateExistingQueryList()
        {
            SqlConnection cn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["HL7DataAnalyserConnectionString"].ToString());
            SqlCommand cm = new SqlCommand("uspSelectOperatorQueries", cn);
            cm.CommandType = CommandType.StoredProcedure;
            cm.Parameters.Add(new SqlParameter("@OperatorId", SqlDbType.NVarChar, 50));
            cm.Parameters["@OperatorId"].Value = OperatorId;
            SqlDataAdapter da = new SqlDataAdapter(cm);
            cn.Open();
            DataTable dt = new DataTable("Queries");
            da.Fill(dt);
            cn.Close();
            cm.Dispose();
            cn.Dispose();
            grdExistingQuery.DataSource = dt;
            grdExistingQuery.DataBind();
        }
        protected void btnToolsNewQueryOK_Click(object sender, EventArgs e)
        {
            if (chkToolsNewQuerySavePreviousQuery.Checked)
            {
                if (chkToolsNewQueryUpdateChanges.Checked)
                {
                    btnToolsConditionApply_Click(sender, e);
                    btnToolsResultsApply_Click(sender, e);
                }
                btnToolsSaveQuery_Click(sender, e);
            }
            DeleteXMLQueryFileCreateNew();
            WorkingQuery = df.SetupNewQuery(OperatorId);
            CurrentQueryId = GetCurrentQuery();
            WorkingQuery = df.CreateTempTablesForXML(WorkingQuery);
            SaveXMLQueryFile();
            PopulateToolsQuery();
            grdToolsCondition.EditIndex = -1;
            grdToolsResults.EditIndex = -1;
            grdHL7Data.PageIndex = 0;
           // acrnTools.SelectedIndex = 0;
            PopulateToolsResultGrid();
            PopulateToolConditionGrid();
            DataBindHL7Grid();
        }



        protected void grdExistingQuery_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView drv = (DataRowView)e.Row.DataItem;
                LinkButton lnk = e.Row.FindControl("btnSelectExistingQuery") as LinkButton;
                lnk.Attributes.Add("onClick", "return SelectExistingQuery('" + drv["Id"] + "','" + drv["QueryName"] + "')");
                //onClientClick = "return SelectExistingQuery(this)"
            }
        }

        protected void btnLoadExistingQuery_Click(object sender, EventArgs e)
        {
            if (chkToolsNewQuerySavePreviousQuery.Checked)
            {
                if (chkToolsNewQueryUpdateChanges.Checked)
                {
                    btnToolsConditionApply_Click(sender, e);
                    btnToolsResultsApply_Click(sender, e);
                }
                btnToolsSaveQuery_Click(sender, e);
            }
            DeleteXMLQueryFileCreateNew();
            WorkingQuery = df.RetrieveQueryFromDatabase(Convert.ToInt32(hdnExisitingQueryId.Value));
            CurrentQueryId = Convert.ToInt32(hdnExisitingQueryId.Value);
            SaveXMLQueryFile();
            grdToolsCondition.EditIndex = -1;
            grdToolsResults.EditIndex = -1;
            grdHL7Data.PageIndex = 0;
           // acrnTools.SelectedIndex = 0;
            PopulateToolsQuery();
            PopulateToolsResultGrid();
            PopulateToolConditionGrid();
            BuildSQlStatement();
            DataBindHL7Grid();
        }

        protected void btnLoadExistingQueryAsNew_Click(object sender, EventArgs e)
        {
            if (chkToolsNewQuerySavePreviousQuery.Checked)
            {
                if (chkToolsNewQueryUpdateChanges.Checked)
                {
                    btnToolsConditionApply_Click(sender, e);
                    btnToolsResultsApply_Click(sender, e);
                }
                btnToolsSaveQuery_Click(sender, e);
            }
            DeleteXMLQueryFileCreateNew();
            WorkingQuery = df.RetrieveQueryFromDatabase(Convert.ToInt32(hdnExisitingQueryId.Value));
            CurrentQueryId = 0;// Convert.ToInt32(hdnExisitingQueryId.Value);
            WorkingQuery.Tables["tblQueries"].Rows[0]["Id"] = 0;
            WorkingQuery.Tables["tblQueries"].AcceptChanges();
            foreach (DataRow r in WorkingQuery.Tables["tblResultFields"].Rows)
            {
                r["QueryId"] = 0;
            }
            WorkingQuery.Tables["tblResultFields"].AcceptChanges();
            foreach (DataRow r in WorkingQuery.Tables["tblConditionFields"].Rows)
            {
                r["QueryId"] = 0;
            }
            WorkingQuery.Tables["tblConditionFields"].AcceptChanges();
            foreach (DataRow r in WorkingQuery.Tables["tmpResultFields"].Rows)
            {
                r["QueryId"] = 0;
            }
            WorkingQuery.Tables["tmpResultFields"].AcceptChanges();
            foreach (DataRow r in WorkingQuery.Tables["tmpConditionFields"].Rows)
            {
                r["QueryId"] = 0;
            }
            WorkingQuery.Tables["tmpConditionFields"].AcceptChanges();
            SaveXMLQueryFile();
            grdToolsCondition.EditIndex = -1;
            grdToolsResults.EditIndex = -1;
            grdHL7Data.PageIndex = 0;
            //acrnTools.SelectedIndex = 0;
            PopulateToolsQuery();
            PopulateToolsResultGrid();
            PopulateToolConditionGrid();
            BuildSQlStatement();
            DataBindHL7Grid();
        }
    }
}
