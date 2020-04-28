using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.ComponentModel;
using System.Configuration;

namespace HL7DataAnalyser
{
    [DataObject(true)]
    public class DataAccess
    {
        private SqlConnection cn;

        [DataObjectMethod(DataObjectMethodType.Select)]
        public DataTable SelectHL7Data(string WhereClause)
        {
            if (cn.State==ConnectionState.Closed)
            {
                cn.Open();
            }
            string sql = "Select * from hl7data where hl7messageDate < '"
                + DateTime.Now.ToString() + "' and h17MessageDate > '" + DateTime.Now.AddDays(-1).ToString() + "'";
            SqlCommand cm = new SqlCommand(sql);
            DataTable dt = new DataTable("HL7Data");
            SqlDataAdapter da = new SqlDataAdapter(sql, cn);
            da.Fill(dt);
            return dt;
        }
        public void CreateConnection()
        {
           String ConnectionString = ConfigurationManager.ConnectionStrings["HL7DataConnectionStringADAuth"].ConnectionString;
            cn = new SqlConnection(ConnectionString);
        }
        //public DataTable GetDataTableColumnList()
        //{

        //    if (cn.State == ConnectionState.Closed)
        //    {
        //        cn.Open();
        //    }
        //}

    }
}