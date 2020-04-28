<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Header.aspx.cs" Inherits="HL7DataAnalyser.Header" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        body{
            font-family:Arial;
        }
        input[type=button]{
            border:none;
            border-radius:5px 5px;
            color:white;
            background-color:#213f8c;

        }
         input[type=button]:hover{
            background-color:#5b71a3;
            color:white;

        }
         .header{
            position:sticky;
            padding: 0px 0px 0px 0px;
            top:0;
            right:0;
            bottom:0;
            height:50px;
            width:100%;
            background-color:white;
            border-bottom-width:4px;
            border-bottom-color:#003569;
            border-bottom-style:solid;
            color:#003569;
        }
        
        .Logo {
            width: 115px;
            height: 45px;
        }
        body{
            margin-left:0;
            margin-top:0;
            margin-right:0;
            margin-bottom:0;
        }
        td.Logo {
            width: 120px;
        }
        .labelHeader{
            font-family:Arial;
            font-size:x-large;
            font-weight:800;
            margin-left: 15px;
        }
        Table.Info {
            width: 300px;
            text-align:right;
            font-family:Arial;
            font-size:smaller;
        }
        td.App{
            width:80%;
        }
        
        
        </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
             <table class="header">
            <tr>
                <td class="Logo">
                    <img alt="RUHS" class="Logo" src="Images/RUHS%20logo%204c.png" /></td>
                <td class="App">
                 <div class="labelHeader">
                     <asp:Label ID="lblAppName" runat="server" Text="Application Name"></asp:Label> </div>
            

                </td>
                <td>
                    <table class="Info">
                        <tr>
                            <td>
                                <asp:Label ID="lblTeam" runat="server" Text="Integration Services"></asp:Label>
                                
                            </td>
                            <td>
                                <asp:ImageButton ID="btnHome" runat="server" src="Images/Home.png"/>
                                
                            </td>
                        </tr>
                        <tr>
                            <td>
                                
                            </td>
                            <td>
                                <asp:Label ID="lblDate" runat="server" Text="DateTime"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        </div>
    </form>
</body>
</html>
