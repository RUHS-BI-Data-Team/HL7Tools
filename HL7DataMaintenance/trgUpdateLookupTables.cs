using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.SqlServer.Server;
using System.Data.SqlTypes;

public partial class Triggers
{        
    // Enter existing table or view for the target and uncomment the attribute line
    [Microsoft.SqlServer.Server.SqlTrigger (Name="trgUpdateLookupTables", Target="OpenLinkLookup.dbo.tblFacilities", Event="FOR INSERT")]
    public static void trgUpdateLookupTables ()
    {
        // Replace with your own code
        SqlTriggerContext triggContext = SqlContext.TriggerContext;
        SqlCommand cmd;
        SqlPipe pipe = SqlContext.Pipe;
        SqlDataReader rdr;
        //SqlContext.Pipe.Send("Trigger FIRED");
        SqlConnection cnn = new SqlConnection(@"context connection=true");
        cnn.Open();
        cmd = new SqlCommand(@"SELECT * FROM INSERTED;", cnn);
        rdr = cmd.ExecuteReader();
        SqlConnection remoteCnn = new SqlConnection("Data Source=Cpc-intsqlprd01; Initial Catalog=OpenLinkLookup; Integrated Security=SSPI");
        SqlCommand remoteCmd = new SqlCommand("ufnInsertUpdateFacility", remoteCnn);
        remoteCmd.CommandType = CommandType.StoredProcedure;


    }
}

