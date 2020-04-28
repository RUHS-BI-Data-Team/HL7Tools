using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace DataStoreHL7Listener
{
    public partial class HL7ListenerService : ServiceBase
    {
        private string PublicFolderLocation;
        private string DatabaseConnectionString;
        private string StoredProcedureName;
        private string LogFileName;
        private string FolderName;
        private Int32 port;
        private Int32 logHistoryLimit;
        private Int32 errorHistroyLimit;
        private HL7TCPListener listenerHL7;
        private Int32 SQlCommandTimeout; 

        public HL7ListenerService()
        {
            InitializeComponent();
            /// Installer Code
            HL7ListenerEvents = new System.Diagnostics.EventLog("DatabaseHL7Listener");
            if (!System.Diagnostics.EventLog.SourceExists("DatabaseHL7Listener"))
            {
                System.Diagnostics.EventLog.CreateEventSource("DatabaseHL7Listener", "DatabaseHL7ListenerEvents");
            }
            HL7ListenerEvents.Source = "DatabaseHL7Listener";

           
            ///           

        }

        protected override void OnStart(string[] args)
        {
            AppSettingsReader reader = new AppSettingsReader();
            LogFileName = Convert.ToString(reader.GetValue("LogFileName", typeof(string)));
            FolderName = Convert.ToString(reader.GetValue("FolderName", typeof(string))); // \HL7ListenerData\ 
            StoredProcedureName = Convert.ToString(reader.GetValue("StoredProcedureName", typeof(string)));
            logHistoryLimit = Convert.ToInt32(reader.GetValue("LogHistoryLimit", typeof(int)));
            errorHistroyLimit = Convert.ToInt32(reader.GetValue("ErrorHistroyLimit", typeof(int)));
            //string FolderName = Convert.ToString(reader.GetValue("FolderName", typeof(string)));
            DatabaseConnectionString = ConfigurationManager.ConnectionStrings["HL7DatabaseConnection"].ConnectionString;
            PublicFolderLocation = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            SQlCommandTimeout = Convert.ToInt32(reader.GetValue("SQlCommandTimeout", typeof(int)));
            port = Convert.ToInt32(reader.GetValue("Port", typeof(int)));
            string LogFileLocation = PublicFolderLocation + FolderName + LogFileName;
            MakeFolderLocation();
            listenerHL7 = new HL7TCPListener(port, DatabaseConnectionString, StoredProcedureName, SQlCommandTimeout, LogFileLocation, logHistoryLimit, errorHistroyLimit);
            listenerHL7.SendACK = true;
            listenerHL7.Start();
        }

        protected override void OnStop()
        {
            listenerHL7.RequestStop();
        }
        private void MakeFolderLocation()
        {
            if (!Directory.Exists(PublicFolderLocation + FolderName))
            {
                Directory.CreateDirectory(PublicFolderLocation + FolderName);
            }
        }

        private void serviceInstaller1_AfterInstall(object sender, System.Configuration.Install.InstallEventArgs e)
        {

        }

        private void HL7ListenerEvents_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }
    }
}
