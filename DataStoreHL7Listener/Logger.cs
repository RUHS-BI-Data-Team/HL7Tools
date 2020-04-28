using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStoreHL7Listener
{
    public class Logger : IDisposable
    {
        string fileLocation;
        DataSet dsLog = new DataSet();
        DateTime LogWriteTime;
        private Int32 logHistroyLimit;
        private Int32 errorHistroyLimit;
        public Logger(String FileLocation, Int32 LogHistroyLimit, Int32 ErrorHistroyLimit)
        {
            fileLocation = FileLocation;
            LogWriteTime = DateTime.Now;
            logHistroyLimit = LogHistroyLimit;
            errorHistroyLimit = ErrorHistroyLimit;
            GetLogFile();
        }
        private void GetLogFile()
        {
            if (File.Exists(fileLocation) == true)
            {
                dsLog.ReadXml(fileLocation, XmlReadMode.ReadSchema);
                SaveLogger(true);
            }
            else
            {
                CreateXMLLogFormat();
            }
        }
        public void AddLog(string LogMessage)
        {
            DataRow newRow = dsLog.Tables["Logs"].NewRow();
            newRow["Description"] = LogMessage;
            newRow["TimeStamp"] = DateTime.Now;
            dsLog.Tables["Logs"].Rows.Add(newRow);
            SaveLogger();
        }
        public void AddError(string ErrorMessage)
        {
            DataRow newRow = dsLog.Tables["Errors"].NewRow();
            newRow["Description"] = ErrorMessage;
            newRow["TimeStamp"] = DateTime.Now;
            dsLog.Tables["Errors"].Rows.Add(newRow);
            SaveLogger(true);
        }
        private void CreateXMLLogFormat()
        {
            DataTable tblErrors = new DataTable("Errors");
            DataTable tblLogs = new DataTable("Logs");
            tblErrors.Columns.Add(new DataColumn("Description", typeof(string)));
            tblErrors.Columns.Add(new DataColumn("TimeStamp", typeof(DateTime)));
            tblLogs.Columns.Add(new DataColumn("Description", typeof(string)));
            tblLogs.Columns.Add(new DataColumn("TimeStamp", typeof(DateTime))); 
            dsLog.Tables.Add(tblErrors);
            dsLog.Tables.Add(tblLogs);
            SaveLogger(true);
        }
        public void SaveLogger(bool OverRideTime = false)
        {
            if (DateTime.Now.Subtract(LogWriteTime).TotalMinutes > 9 || OverRideTime == true)
            {
                ClearLog();
                dsLog.WriteXml(fileLocation, XmlWriteMode.WriteSchema);
                LogWriteTime = DateTime.Now;
            }
        }
        private void ClearLog()
        {
            var logs = from logRow in dsLog.Tables["Logs"].AsEnumerable()
                       where logRow.Field<DateTime>("TimeStamp").AddHours(logHistroyLimit) < DateTime.Now
                       select logRow;
            DataView logView = logs.AsDataView();
            for (int i = logView.Count - 1; i > -1; i--)
            {
                logView[i].Delete();
            }
            var errors = from errorRow in dsLog.Tables["Errors"].AsEnumerable()
                       where errorRow.Field<DateTime>("TimeStamp").AddHours(errorHistroyLimit) < DateTime.Now
                       select errorRow;
            DataView errorView = errors.AsDataView();
            for (int i = errorView.Count - 1; i > -1; i--)
            {
                errorView[i].Delete();
            }





            /*
          var results = from myRow in tblCurrentStock.AsEnumerable()
          where myRow.Field<string>("item_name").ToUpper().StartsWith(tbSearchItem.Text.ToUpper())
          select myRow;
DataView view = results.AsDataView();
*/
        }
        void IDisposable.Dispose()
        {
            SaveLogger(true);
        }
    }
}
