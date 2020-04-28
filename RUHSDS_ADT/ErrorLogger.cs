using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUHSDS_ADT
{
    class ErrorLogger
    {

   
        public void ProcessError(string sError)
        {
            //SqlConnection oConn = new SqlConnection(RUHSDS_ADT.Properties.Settings.Default.connectionStr);
            SqlConnection oConn = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["RUHSDS_ADT.Properties.Settings.connectionStr"]);
            SqlCommand oComm = new SqlCommand();
            oComm.CommandType = CommandType.StoredProcedure;
            oComm.CommandText = "usp_insert_error";
            oComm.Parameters.Add("@ErrorText", SqlDbType.NVarChar).Value = sError;
            oComm.Parameters.Add("@ErrorDate", SqlDbType.DateTime).Value = DateTime.Now;
            oComm.Connection = oConn;
            
            try
            {
                oConn.Open();
                oComm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                oConn.Close();
                oConn.Dispose();
            }


        }
    }
}
