using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.IO;

namespace HL7ListenerDatabase
{
    class Program
    {
        static private string PublicFolderLocation;
        static private string DatabaseConnectionString;
        static private string StoredProcedureName;
        static private string LogFileName;
        static private string FolderName;
        static private Int32 port;
        static private Int32 logHistoryLimit;
        static private Int32 errorHistroyLimit;

        static void Main(string[] args)
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
           
            port = Convert.ToInt32(reader.GetValue("Port", typeof(int)));
            string LogFileLocation = PublicFolderLocation + FolderName + LogFileName;
            MakeFolderLocation();
            HL7TCPListener listenerHL7 = new HL7TCPListener(port, DatabaseConnectionString, StoredProcedureName, LogFileLocation, logHistoryLimit, errorHistroyLimit);
            listenerHL7.SendACK = true; //sendACK;
            if (listenerHL7.Start())
            {
                Console.Write("Listener Started");
                Console.Write("Type 'Y' to stop listener.");

                while (true)
                {
                    if(Console.ReadLine() == "Y")
                    {
                        listenerHL7.RequestStop();
                        break;
                    }
                }


            }
                    
        }
        static private void MakeFolderLocation()
        {
            if (!Directory.Exists(PublicFolderLocation + FolderName))
            {
                Directory.CreateDirectory(PublicFolderLocation + FolderName);
            }
        }
    }
}
