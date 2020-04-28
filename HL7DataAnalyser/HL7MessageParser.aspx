<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HL7MessageParser.aspx.cs" Inherits="HL7DataAnalyser.HL7MessageParser" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:CheckBox ID="chkShowDefinitions" runat="server" Text="Show Definitions" />
            <asp:GridView ID="gdvParsedMessage" runat="server" GridLines="Vertical" Font-Size="Small" HeaderStyle-HorizontalAlign="Left" AutoGenerateColumns="False" 
                DataKeyNames="Id" EmptyDataText="HL7 Message was not found in warehouse" OnRowCommand="gdvParsedMessage_RowCommand" Font-Names="Arial">
                <AlternatingRowStyle BackColor="#E2E2E2" Wrap ="false" />
                        <RowStyle BackColor="White" Wrap="false" />
                
                <Columns>
                    <asp:BoundField DataField="SegId">
                    <ItemStyle Font-Bold="True" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:TemplateField ShowHeader="False" HeaderText="Segments">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnSegment" runat="server" CommandArgument='<%# Eval("Id") %>' Font-Underline="False" ForeColor="Black" Text='<%# Eval("SegmentValue") %>' CommandName="SelectSegment" ToolTip='<%# Eval("Definition") %>'></asp:LinkButton>
                            <asp:HiddenField ID="hdnSelected" runat="server" Value="false" />
                            <asp:HiddenField ID="hdnSegId" runat="server" Value='<%# Eval("SegId") %>' />
                            <asp:HiddenField ID="hdnParseSegmentLocation" Value='<%# Eval("ParseSegmentLocation") %>' runat="server" />
                            <asp:GridView ID="gdvParsedSegment" runat="server" Visible="False" GridLines="Vertical" Font-Size="Small" HeaderStyle-HorizontalAlign="Left" AutoGenerateColumns="False"
                                DataKeyNames="Id" OnRowCommand="gdvParsedSegment_RowCommand" ShowHeader="False">
                                <AlternatingRowStyle BackColor="#E2E2E2" Wrap ="false" />
                                <SelectedRowStyle BackColor="#e2d859" Wrap="false" Font-Bold="true" />
                                <RowStyle BackColor="White" Wrap="false" />
                                <Columns>
                                    <asp:BoundField DataField="Definition" HeaderText="Definition" />
                                    <asp:BoundField DataField="FieldId" >
                                    <ItemStyle Font-Bold="True" VerticalAlign="Top" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ParseFieldLocation" HeaderText="Location" Visible="False" />
                                    <asp:TemplateField HeaderText="Fields">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnFiels" runat="server" CommandArgument='<%# Eval("Id") %>' Font-Underline="False" ForeColor="Black" Text='<%# Eval("FieldValue") %>' CommandName="SelectField" ToolTip='<%# Eval("Definition") %>'></asp:LinkButton>
                                            <asp:HiddenField ID="hdnSelected" runat="server" Value="false" />
                                            <asp:HiddenField ID="hdnParseSegmentLocation" runat="server" Value='<%# Eval("ParseSegmentLocation") %>' />
                                            <asp:HiddenField ID="hdnParseFieldLocation" Value='<%# Eval("ParseFieldLocation") %>' runat="server" />
                            
                                            <asp:GridView ID="gdvParsedField" runat="server" AutoGenerateColumns="False" GridLines="Vertical" ShowHeader="False">
                                                 <AlternatingRowStyle BackColor="#E2E2E2" Wrap ="false" />
                                                   <RowStyle BackColor="White" Wrap="false" />
                                                <Columns>
                                                    <asp:BoundField DataField="ComponentId" >
                                                    <ItemStyle Font-Bold="True" VerticalAlign="Top" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="ParseComponentLocation" HeaderText="Location" Visible="False" />
                                                    <asp:TemplateField HeaderText="Value">
                                                        <EditItemTemplate>
                                                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("ComponentValue") %>'></asp:TextBox>
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("ComponentValue") %>' ToolTip='<%# Eval("Definition") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                                 <HeaderStyle BackColor="#8a3a81" Font-Size="Small" ForeColor="White" Wrap="False" />  
                                            </asp:GridView>
                            
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <HeaderStyle BackColor="#8a3a81" Font-Size="Small" ForeColor="White" Wrap="False" />  
                            </asp:GridView>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
        <HeaderStyle BackColor="#8a3a81" Font-Size="Small" ForeColor="White" Wrap="False" />  
            </asp:GridView>
        </div>
    </form>
</body>
</html>
