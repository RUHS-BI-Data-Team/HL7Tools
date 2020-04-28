using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data;
using System.Configuration;

public partial class _Default : System.Web.UI.Page
{
    private bool ConfigurationFileExist = false;
    private DataSet dsConfigurationData = new DataSet("ConfigurationData");
    private string ConfigurationFileName;
    private string FolderLocationName;
    private string PublicFolderLocation;
    protected void Page_Load(object sender, EventArgs e)
    {

        AppSettingsReader reader = new AppSettingsReader();
        string FileName = Convert.ToString(reader.GetValue("FileName", typeof(string))); // ConfigurationData.xml
        FolderLocationName = Convert.ToString(reader.GetValue("FolderName", typeof(string))); // \HL7ListenerData\
        PublicFolderLocation = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        ConfigurationFileName = PublicFolderLocation + FolderLocationName + FileName;
        if (!this.IsPostBack)
        {

            if (File.Exists(ConfigurationFileName))
            {
                ConfigurationFileExist = true;
                dsConfigurationData.ReadXml(ConfigurationFileName);
                pnlNoFile.Visible = false;
            }
            else
            {
                pnlNoFile.Visible = true;
            }
        }
        //lblFolder.Text = myFolder;
    }

    protected void btnNewFileYes_Click(object sender, EventArgs e)
    {
        //string myFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        if (!File.Exists(ConfigurationFileName))
        {
            CreateCongurationFile();
            ConfigurationFileExist = true;
            pnlNoFile.Visible = false;

        }
    }
    protected void SaveConfigurationFile()
    {
        Directory.CreateDirectory(PublicFolderLocation + FolderLocationName);
        dsConfigurationData.WriteXml(ConfigurationFileName, XmlWriteMode.WriteSchema);
    }
    private void CreateCongurationFile()
    {
        DataTable tbConfig = new DataTable("Listeners");
        tbConfig.Columns.Add(new DataColumn("Id", typeof(int)));
        tbConfig.Columns.Add(new DataColumn("ListenerName", typeof(string)));
        tbConfig.Columns.Add(new DataColumn("Port", typeof(int)));
        tbConfig.Columns.Add(new DataColumn("MessageType", typeof(string)));
        tbConfig.Columns.Add(new DataColumn("Active", typeof(bool)));
        tbConfig.Columns["ListenerName"].Unique = true;
        tbConfig.Columns["Port"].Unique = true;
        tbConfig.Columns["MessageType"].Unique = true;
        tbConfig.Columns["Id"].AutoIncrement = true;
        tbConfig.Columns["Id"].AutoIncrementSeed = 1;
        tbConfig.Columns["Id"].AutoIncrementStep = 1;
        
        DataTable tbDBSettings = new DataTable("Settings");
        tbDBSettings.Columns.Add(new DataColumn("DatabaseConnectionString", typeof(string)));
        tbDBSettings.Columns.Add(new DataColumn("StoreProcedureName", typeof(string)));
        tbDBSettings.Columns.Add(new DataColumn("LogFileName", typeof(string)));
        dsConfigurationData.Tables.Add(tbConfig);
        dsConfigurationData.Tables.Add(tbDBSettings);
        SaveConfigurationFile();
    }
}