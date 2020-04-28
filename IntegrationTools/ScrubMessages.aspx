<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ScrubMessages.aspx.cs" Inherits="IntegrationTools.ScrubMessages" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxToolkit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    </head>
<body>
    <form id="form1" runat="server">
        <div style="text-align: center; width: 100%;">
            <table cellspacing="1" style="width: 100%">
                <tr>
                    <td style="text-align: left">
                        <asp:Button ID="btnCreateFile" runat="server" OnClick="btnCreateFile_Click" Text="Create File for OPENlink" />
                    </td>
                </tr>
                <tr>
                    <td>
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="sqlScubData" Caption="Scubbed Messages" HorizontalAlign="Center" ShowHeader="False" Width="80%" >
                <Columns>
                    <asp:TemplateField>
                        <AlternatingItemTemplate>
                            <asp:Label ID="Label2" runat="server" Font-Bold="True" ForeColor="#FF99FF" Text='<%# Eval("ScrubedMessages") %>'></asp:Label>
                        </AlternatingItemTemplate>
                        <EditItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# Eval("ScrubedMessages") %>'></asp:Label>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:TextBox ID="TextBox1" runat="server" ReadOnly="True" Rows="10" Text='<%# Eval("ScrubedMessages") %>' TextMode="MultiLine" Width="98%" Wrap="False"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:CommandField ShowEditButton="True" />
                </Columns>
            </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                </tr>
            </table>
            <asp:SqlDataSource ID="sqlScubData" runat="server" ConnectionString="<%$ ConnectionStrings:HL7DataScrubConnectionString %>" SelectCommand="SelectScrubbedMessages" SelectCommandType="StoredProcedure" DeleteCommandType="StoredProcedure" UpdateCommandType="StoredProcedure"></asp:SqlDataSource>
        </div>
    </form>
</body>
</html>
