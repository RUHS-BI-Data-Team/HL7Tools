<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script type="text/javascript" src="script.js"></script>
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Panel ID="pnlNoFile" runat="server">
            <table class="auto-style1">
                <tr>
                    <td colspan="4" style="text-align: center">
                        <asp:Label ID="Label1" runat="server" Font-Bold="True" ForeColor="Red" Text="Configuration File does not exist! Do ypu want to create a new one?"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td style="text-align: right">
                        <asp:Button ID="btnNewFileYes" runat="server" OnClick="btnNewFileYes_Click" Text="Yes" Width="150px" />
                    </td>
                    <td>
                        
                        <asp:Button ID="btnNewFileNo" runat="server" CausesValidation="False" EnableViewState="False" Text="No" Width="150px" OnClientClick="alert('File not created!');" />
                    </td>
                    <td>&nbsp;</td>
                </tr>
            </table>
        </asp:Panel>
    </form>
</body>
</html>
