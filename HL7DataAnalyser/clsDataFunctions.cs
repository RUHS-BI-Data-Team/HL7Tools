using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace HL7DataAnalyser
{
    internal class clsDataFunctions
    {
        public DataSet RetrieveQueryFromDatabase(int QueryId = 0, String QueryName = "", String OperatorId = "")
        {
            DataSet NewQuery = new DataSet("Query");
            SqlConnection cn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["HL7DataAnalyserConnectionString"].ToString());
            SqlCommand cm = new SqlCommand("uspSelectQueryRecordset", cn);
            cm.CommandType = CommandType.StoredProcedure;
            cm.Parameters.Add(new SqlParameter("@QueryName", SqlDbType.NVarChar, 50));
            cm.Parameters.Add(new SqlParameter("@OperatorId", SqlDbType.NVarChar, 50));
            cm.Parameters.Add(new SqlParameter("@QueryId", SqlDbType.Int, 4));
            cm.Parameters["@QueryName"].Value = QueryName;
            cm.Parameters["@OperatorId"].Value = OperatorId;
            cm.Parameters["@QueryId"].Value = QueryId;
            cn.Open();
            SqlDataAdapter da = new SqlDataAdapter(cm);
            da.Fill(NewQuery);
            cn.Close();
            NewQuery.Tables[0].TableName = "tblQueries";
            NewQuery.Tables[1].TableName = "tblResultFields";
            NewQuery.Tables[2].TableName = "tblConditionFields";
            NewQuery.Tables[3].TableName = "tblkQueryControls";
            foreach (DataTable tbl in NewQuery.Tables)
            {
                if (tbl.TableName == "tblResultFields" || tbl.TableName == "tblConditionFields")
                {
                    tbl.Columns["Id"].AllowDBNull = false;
                    tbl.Columns["Id"].AutoIncrement = true;
                    tbl.Columns["Id"].AutoIncrementStep = 1;
                    if (tbl.Rows.Count == 0)
                    {
                        tbl.Columns["Id"].AutoIncrementSeed = 1;
                    }
                    else
                    {
                        tbl.Columns["Id"].AutoIncrementSeed = GetIdMaxfromTable(tbl) + 1;
                    }
                }
            }
            NewQuery.Relations.Add(new DataRelation("ResultFieldsId", NewQuery.Tables["tblQueries"].Columns["Id"], NewQuery.Tables["tblResultFields"].Columns["QueryId"]));
            NewQuery.Relations.Add(new DataRelation("ConditionFieldsId", NewQuery.Tables["tblQueries"].Columns["Id"], NewQuery.Tables["tblConditionFields"].Columns["QueryId"]));
            CreateTempTablesForXML(NewQuery);
            return NewQuery;
        }
        public DataSet CreateTempTablesForXML(DataSet ds)
        {
            bool tblResultsExist = false;
            bool tblConditionsExist = false;
            foreach (DataTable dt in ds.Tables)
            {
                if(dt.TableName == "tmpResultFields")
                {
                    tblResultsExist = true;
                }
                if (dt.TableName == "tmpConditionFields")
                {
                    tblConditionsExist = true;
                }
            }
            if (!tblResultsExist)
            {
                DataTable tmpR = tmpR = ds.Tables["tblResultFields"].Copy();
                tmpR.TableName = "tmpResultFields";
                ds.Tables.Add(tmpR);
            }
            if (!tblConditionsExist)
            {
                DataTable tmpC = tmpC = ds.Tables["tblConditionFields"].Copy();
                tmpC.TableName = "tmpConditionFields";
                ds.Tables.Add(tmpC);
            }
            return ds;
        }
        private int GetIdMaxfromTable(DataTable dt)
        {
            var i = (from tbl in dt.AsEnumerable()
                     orderby tbl.Field<int>("Id") descending
                     select tbl.Field<int>("Id")).ElementAt(0);

            return Convert.ToInt16(i);
        }

        public DataSet SetupNewQuery(String OperatorId)
        {
            DataSet NewQuery = RetrieveQueryFromDatabase();
            DataRow qdr = NewQuery.Tables["tblQueries"].NewRow();
            qdr["Id"] = 0;
            qdr["QueryName"] = "New Query 01";
            qdr["StartDate"] = DateTime.Now.AddDays(-1);
            qdr["EndDate"] = DateTime.Now;
            qdr["QueryDescription"] = "This is a new Query";
            qdr["OperatorId"] = OperatorId;
            qdr["UseDates"] = 1;
            qdr["QueryDate"] = DateTime.Now;
            NewQuery.Tables["tblQueries"].Rows.Add(qdr);
            AddResultField(ref NewQuery, Convert.ToInt16(NewQuery.Tables["tblQueries"].Rows[0]["Id"].ToString()), "MRN", 1);
            AddResultField(ref NewQuery, Convert.ToInt16(NewQuery.Tables["tblQueries"].Rows[0]["Id"].ToString()), "HL7Event", 2);
            AddResultField(ref NewQuery, Convert.ToInt16(NewQuery.Tables["tblQueries"].Rows[0]["Id"].ToString()), "HL7Encounter", 3);
            //AddConditionField(ref NewQuery, Convert.ToInt16(NewQuery.Tables["tblQueries"].Rows[0]["Id"].ToString()), "HL7Event", "A01", 5, 3,true);

            //DataRow 
            IEnumerable<DataRow> dr = from qr in NewQuery.Tables["tblResultFields"].AsEnumerable()
                                      where qr.Field<int>("QueryId") == 0
                                      orderby qr.Field<int>("QueryResultOrder") ascending
                                      select qr;
            foreach (DataRow r in dr)
            {
                DataRow tmp = NewQuery.Tables["tmpResultFields"].NewRow();
                for (int i = 0; i < tmp.Table.Columns.Count; i++)
                {
                    tmp[i] = r[i];
                }
                NewQuery.Tables["tmpResultFields"].Rows.Add(tmp);
            }
            NewQuery.Tables["tmpResultFields"].AcceptChanges();
            return NewQuery;
        }
        public void AddResultField(ref DataSet Query, int QueryId, String FieldName, int QueryResultOrder, string ColumnAlias = "", int OrderBy = -1, int OrderByDirection = -1, int HL7ParseLocation = -1)
        {
            DataRow dr = Query.Tables["tblResultFields"].NewRow();
            dr["FieldName"] = FieldName;
            dr["QueryId"] = QueryId;
            dr["QueryResultOrder"] = QueryResultOrder;
            dr["ColumnAlias"] = ColumnAlias;
            dr["OrderBy"] = SetValueToDBNull(OrderBy);
            dr["OrderByDirection"] = SetValueToDBNull(OrderByDirection);
            dr["HL7ParseLocation"] = SetValueToDBNull(HL7ParseLocation);
            Query.Tables["tblResultFields"].Rows.Add(dr);
        }
        public void AddConditionField(ref DataSet Query, int QueryId, String FieldName, String ConditionValue, int CompareType, int JoinType, bool UseCondition = true, int HL7ParseLocation = -1)
        {
            DataRow dr = Query.Tables["tblConditionFields"].NewRow();
            dr["FieldName"] = FieldName;
            dr["QueryId"] = QueryId;
            dr["ConditionValue"] = ConditionValue;
            dr["CompareType"] = CompareType;
            dr["JoinType"] = JoinType;
           dr["UseCondition"] = UseCondition;
            dr["HL7ParseLocation"] = SetValueToDBNull(HL7ParseLocation);
            dr["QueryConditionOrder"] = Query.Tables["tblConditionFields"].Rows.Count + 1;
            Query.Tables["tblConditionFields"].Rows.Add(dr);
        }
        private object SetValueToDBNull(int Value)
        {
            if(Value == -1)
            {
                return DBNull.Value;
            }
            else
            {
                return Value;
            }
        }
        public void UpdateQueryNameInDatabase(string OldQueryName, string NewQueryName, string OperatorId)
        {

        }
        public int SaveQueryToDatabase(DataSet Query)
        {
            SqlConnection cn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["HL7DataAnalyserConnectionString"].ToString());
            SqlCommand cm = new SqlCommand("uspInsertQuery", cn);
            cm.CommandType = CommandType.StoredProcedure;
            cm.Parameters.Add(new SqlParameter("@Query", SqlDbType.Structured));
            cm.Parameters.Add(new SqlParameter("@ResultFields", SqlDbType.Structured));
            cm.Parameters.Add(new SqlParameter("@ConditionFields", SqlDbType.Structured));
            cm.Parameters.Add(new SqlParameter("@NewQueryId", SqlDbType.Int));
            cm.Parameters["@NewQueryId"].Direction = ParameterDirection.Output;
            cm.Parameters["@Query"].TypeName = "dbo.utblQueries";
            cm.Parameters["@ResultFields"].TypeName = "dbo.utblResultFields";
            cm.Parameters["@ConditionFields"].TypeName = "dbo.utblConditionFields";
            cm.Parameters["@Query"].Value = Query.Tables["tblQueries"];
            cm.Parameters["@ResultFields"].Value = Query.Tables["tblResultFields"];
            cm.Parameters["@ConditionFields"].Value = Query.Tables["tblConditionFields"];
            
            cn.Open();
            cm.ExecuteNonQuery();
            cm.Dispose();
            cn.Close();
            cn.Dispose();

            return Convert.ToInt32(cm.Parameters["@NewQueryId"].Value);
        }
        public String BuildQuerySyntax(DataSet dsQuery, String QueryName)
        {
            //DataTable dtQueries = dsQuery.Tables["tblQueries"];
            var q = (from query in dsQuery.Tables["tblQueries"].AsEnumerable()
                     where query.Field<string>("QueryName") == QueryName
                     select query.Field<int>("Id")).ElementAt(0);
            
            return BuildQuerySyntax(dsQuery, q);
        }
        public String BuildQuerySyntax(DataSet dsQuery, int QueryId)
        {
            String SQL = BuildSelectStatement(dsQuery, QueryId);
            SQL = SQL + " " + BuildWhereStatement(dsQuery, QueryId);
            SQL = SQL + " " + BuildOrderByStatement(dsQuery, QueryId);
            return SQL;
        }
        private string BuildSelectStatement(DataSet dsQuery, int QueryId)
        {
            String QuerySyntax = "Select Id,";
            IEnumerable<DataRow> r = from qr in dsQuery.Tables["tblResultFields"].AsEnumerable()
                                     where qr.Field<int>("QueryId") == QueryId
                                     orderby qr.Field<int>("QueryResultOrder")
                                     select qr;
            foreach (DataRow qr in r)
            {
                if((qr.Field<int?>("HL7ParseLocation") ?? -1) > -1)
                {
                    QuerySyntax = QuerySyntax + " dbo.ufnParseHL7Value(HL7Data,'" + qr.Field<string>("FieldName") + "'," + qr.Field<int?>("HL7ParseLocation") + ") As '" + qr.Field<string>("FieldName").Replace(".", "") + "',";
                }
                else
                {
                    QuerySyntax = QuerySyntax + " " + qr.Field<string>("FieldName") + ",";
                }
                
            }
            QuerySyntax = QuerySyntax.Substring(0, QuerySyntax.Length - 1) + " From tblAdt";
            return QuerySyntax;
        }
        private string GetColumnAlias(DataRow r)
        {
            if(r.Field<string>("ColumnAlias") == "")
            {
                return " as '" + r.Field<string>("FieldName").Replace(".", "") + "'";
            }
            else
            {
                return " as '" + r.Field<string>("ColumnAlias") + "'";
            }
        }
        private string BuildWhereStatement(DataSet dsQuery, int QueryId)
        {
            String QuerySyntax = "Where ";
            
            var dr = (from q in dsQuery.Tables["tblQueries"].AsEnumerable()
                                      where q.Field<int>("Id") == QueryId
                                      select q).FirstOrDefault();
            if ((dr.Field<Boolean?>("UseDates") ?? false))
            {
                QuerySyntax = QuerySyntax + "(HL7MessageDate>'" + dr.Field<DateTime>("StartDate") + "' And HL7MessageDate<'" + dr.Field<DateTime>("EndDate") + "') ";
            }


            IEnumerable<DataRow> r = from qr in dsQuery.Tables["tblConditionFields"].AsEnumerable()
                                     where qr.Field<int>("QueryId") == QueryId && (qr.Field<bool?>("UseCondition") ?? false) == true
                                     //group qr by qr.Field<string>("FieldName")
                                     //orderby qr.Field<string>("FieldName"), qr.Field<int>("ConditionOrder")
                                     orderby qr.Field<string>("FieldName")
                                     select qr;
            string tmpQuerySyntax = "";
            string previousFieldName = "";
            r.Count();
            for(int i=0; i < r.Count(); i++)
            {

                if (ConditionIsPartOfGroup(r, i))
                {
                    if(r.ElementAt(i).Field<string>("FieldName") != previousFieldName)
                    {
                        QuerySyntax = QuerySyntax + AddConditionGroupSyntax(tmpQuerySyntax);
                        tmpQuerySyntax = " " + GetQueryControlValue(dsQuery, r.ElementAt(i).Field<int>("JoinType")) + " ";
                    }
                    if ((r.ElementAt(i).Field<int?>("HL7ParseLocation") ?? -1) > -1)
                    {
                        tmpQuerySyntax = tmpQuerySyntax + "dbo.ufnParseHL7Value(HL7Data,'" + r.ElementAt(i).Field<string>("FieldName") + "'," + r.ElementAt(i).Field<int?>("HL7ParseLocation") + ") " + GetQueryControlValue(dsQuery, r.ElementAt(i).Field<int>("CompareType")) + " '" + r.ElementAt(i).Field<string>("ConditionValue") + "' Or ";
                    }
                    else
                    {
                        tmpQuerySyntax = tmpQuerySyntax + r.ElementAt(i).Field<string>("FieldName") + " " + GetQueryControlValue(dsQuery, r.ElementAt(i).Field<int>("CompareType")) + " '" + r.ElementAt(i).Field<string>("ConditionValue") + "' Or ";
                    }
                    if(i == r.Count() - 1)
                    {
                        QuerySyntax = QuerySyntax + AddConditionGroupSyntax(tmpQuerySyntax);
                        tmpQuerySyntax = "";
                    }
                }
                else
                {
                    if(tmpQuerySyntax != "")
                    {
                        if (QuerySyntax == "Where ")
                        {
                            QuerySyntax = QuerySyntax + tmpQuerySyntax;
                        }
                        else
                        {
                            QuerySyntax = QuerySyntax + AddConditionGroupSyntax(tmpQuerySyntax);
                        }
                        tmpQuerySyntax = "";
                    }
                    if ((r.ElementAt(i).Field<int?>("HL7ParseLocation") ?? -1) > -1)
                    {
                        if (QuerySyntax == "Where ")
                        {
                            QuerySyntax = QuerySyntax + " dbo.ufnParseHL7Value(HL7Data,'" + r.ElementAt(i).Field<string>("FieldName") + "'," + r.ElementAt(i).Field<int?>("HL7ParseLocation") + ") " + GetQueryControlValue(dsQuery, r.ElementAt(i).Field<int>("CompareType")) + " '" + r.ElementAt(i).Field<string>("ConditionValue") + "' ";
                        }
                        else
                        {
                            QuerySyntax = QuerySyntax + GetQueryControlValue(dsQuery, r.ElementAt(i).Field<int>("JoinType")) + " dbo.ufnParseHL7Value(HL7Data,'" + r.ElementAt(i).Field<string>("FieldName") + "'," + r.ElementAt(i).Field<int?>("HL7ParseLocation") + ") " + GetQueryControlValue(dsQuery, r.ElementAt(i).Field<int>("CompareType")) + " '" + r.ElementAt(i).Field<string>("ConditionValue") + "' ";
                        }
                    }
                    else
                    {
                        if (QuerySyntax=="Where ") {
                            QuerySyntax = QuerySyntax + " " + r.ElementAt(i).Field<string>("FieldName") + " " + GetQueryControlValue(dsQuery, r.ElementAt(i).Field<int>("CompareType")) + " '" + r.ElementAt(i).Field<string>("ConditionValue") + "' ";
                        } else {
                            QuerySyntax = QuerySyntax + GetQueryControlValue(dsQuery, r.ElementAt(i).Field<int>("JoinType")) + " " + r.ElementAt(i).Field<string>("FieldName") + " " + GetQueryControlValue(dsQuery, r.ElementAt(i).Field<int>("CompareType")) + " '" + r.ElementAt(i).Field<string>("ConditionValue") + "' ";
                        }

                        
                    }
                }
                previousFieldName = r.ElementAt(i).Field<string>("FieldName");
            }
            if(QuerySyntax.Substring(QuerySyntax.Length - 4) == " Or ")
            {
                QuerySyntax = QuerySyntax.Substring(0, QuerySyntax.Length - 4);
            }else if(QuerySyntax.Substring(QuerySyntax.Length - 5) == " And ")
            {
                QuerySyntax = QuerySyntax.Substring(0, QuerySyntax.Length - 5);
            }
                return QuerySyntax;
        }
        private string AddConditionGroupSyntax(string QuerySyntax)
        {
            if (QuerySyntax != "")
            {
                
                if (QuerySyntax.Substring(0, 4) == " Or ")
                {
                    QuerySyntax = " Or (" + QuerySyntax.Substring(4);
                }
                else if (QuerySyntax.Substring(0, 5) == " And ")
                {
                    QuerySyntax = " And (" + QuerySyntax.Substring(5);
                }
                if (QuerySyntax.Substring(QuerySyntax.Length - 4) == " Or ")
                {
                    QuerySyntax = QuerySyntax.Substring(0, QuerySyntax.Length - 4) + ")";
                }
                else if (QuerySyntax.Substring(QuerySyntax.Length - 5) == " And ")
                {
                    QuerySyntax = QuerySyntax.Substring(0, QuerySyntax.Length - 5) + ")";
                }
            }
            return QuerySyntax;
        }
        private bool ConditionIsPartOfGroup(IEnumerable<DataRow> r, int i)
        {
            if (r.Count() < 2)
            {
                return false;
            }
            else if (i == 0)
            {
            if (r.ElementAt(i).Field<string>("FieldName") == r.ElementAt(i + 1).Field<string>("FieldName"))
                {
                    return true;
                }
                else { return false; }
            }
            else if (i == r.Count() - 1)
            {
                if (r.ElementAt(i).Field<string>("FieldName") == r.ElementAt(i - 1).Field<string>("FieldName"))
                {
                    return true;
                }
                else { return false; }
            }
            else if (i > 0 && i < r.Count() - 1)
            {
                if (r.ElementAt(i).Field<string>("FieldName") == r.ElementAt(i + 1).Field<string>("FieldName") || r.ElementAt(i).Field<string>("FieldName") == r.ElementAt(i - 1).Field<string>("FieldName"))
                {
                    return true;
                }
                else { return false; }
            }
            else { return false; }
        }
        private string BuildOrderByStatement(DataSet dsQuery, int QueryId)
        {
            String QuerySyntax = "Order By ";
            IEnumerable<DataRow> r = from qr in dsQuery.Tables["tblResultFields"].AsEnumerable()
                                     where qr.Field<int>("QueryId") == QueryId && (qr.Field<int?>("OrderBy") ?? -1) > 0
                                     orderby qr.Field<int>("OrderBy")
                                     select qr;
            foreach (DataRow qr in r)
            {
                QuerySyntax = QuerySyntax + qr.Field<string>("FieldName") + " " + GetQueryControlValue(dsQuery, qr.Field<int>("OrderByDirection")) + ",";
            }
            QuerySyntax = QuerySyntax.Substring(0, QuerySyntax.Length - 1);
            if(QuerySyntax == "Order By")
            {
                QuerySyntax = "";
            }
            return QuerySyntax;
        }

        private string GetQueryControlValue(DataSet dsQuery, int Id)
        {
            var qcv = (from QueryControl in dsQuery.Tables["tblkQueryControls"].AsEnumerable()
                     where QueryControl.Field<int>("Id") == Id
                     select QueryControl.Field<String>("ItemDescription")).ElementAt(0);
            return qcv;
        }
        public DataRow GetQueryInformation(DataSet dsQuery, int QueryId)
        {
            IEnumerable<DataRow> r = from qr in dsQuery.Tables["tblQueries"].AsEnumerable()
                                     where qr.Field<int>("Id") == QueryId
                                     select qr;
            return r.ElementAt(0);
        }
    }
}