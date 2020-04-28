<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HL7DataAnalyser.aspx.cs" Inherits="HL7DataAnalyser.HL7DataAnalyser" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxToolkit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        body{
            font-family:Arial;
        }
        a{
            text-decoration:none;
            color:black; 
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
         input[type=button]:disabled{
             background-color:lightgray;
         }
         input[type=submit]{
            border:none;
            border-radius:5px 5px;
            color:white;
            background-color:#213f8c;

        }
         input[type=submit]:hover {
            background-color: #5b71a3;
            color: white;
        }
         .header{
            position:sticky;
     
            top:0;
            right:0;
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
        
        
       #tools {
           float:left;
           width: 331px;
           /*display:none;*/
       }
        #data {
            float: left;
           z-index:2;
            
        }
        #status {
            clear: both;
        }
        #divider {
            float:left; 
            width:10px;
            background-color: #bcd631;
        }
        #header {
            height: 59px;
            position:sticky;
            top:0;
            right:0;
            width:100%;
            z-index:1;
        }
        .FilterPopupWindow{
            border:none;
            font-size:smaller;
            width: 265px;
            display:none;
        }
        .CreateOpenQueryPopupWindow{
            font-size:smaller;
            width: 405px;
            border: solid 1px;
            border-color: black;
            display:none;
            background-color:white;
        }
        .PopupHeader{
            background-color:orange;
            font-size:larger;
            
        }
         Table.FilterTable {
            width: 100%;
            padding: 0px;
            border: solid 1px;
            border-color: black;
            font-size:smaller;
            border-collapse: collapse;
            background-color:white;
        }
        Table.ToolsTable {
            width: 330px;
            padding: 0px;
            border: solid 1px;
            border-color: black;
            font-size:smaller;
            border-collapse: collapse;
            background-color:white;
        }
        Table.DividerTable {
            height:600px;
            width:8px;
            border-collapse: collapse;
            padding: 0px;
            border-width:0px;
            cursor:pointer;
        }
        td.Rotate{
            transform:rotate(90deg);
            text-align: center;
            font-size:xx-small; font-weight: bold; 
            height: 2px;
        }
        
        table.small {
            margin: 0px;
            border-collapse: collapse;
            padding: 0px;
            border-width:0px;
            font-size:small;
        }
        
        .ToolsGrid {
            width: 300px;
        }
        .ToolsPanelHeader{
            background-color:#e2d859;
            font-size:small;
            cursor:pointer;
            font-weight:bold;
        }   
        .ToolsPanelHeaderSelected{
            background-color:#545454;
            color:white;
            cursor:pointer;
            font-weight:bold;
        }
       .HL7DataPopupMenu{
           background-color:white;
           font-size:smaller;
            width: 300px;
            border: solid 1px;
            border-color: black;
            display:none;
       }
       .ToolsPanelHeader{
           font-size:small;
           font-weight:bolder;
           background-color:#e2d859;
           color:black;
           width: 330px;
       }
       </style>
   
    <script type="text/javascript">
        
        function fnShowTools() {
            $("#tools").show();
            $("#ToolsHidden").val("false");
            $("#ShowTools1").html("H");
            $("#ShowTools2").html("i");
            $("#ShowTools3").html("d");
            $("#ShowTools4").html("e");
        }
        function fnHideTools() {
            $("#tools").hide();
            $("#ToolsHidden").val("true");
            $("#ShowTools1").html("S");
            $("#ShowTools2").html("h");
            $("#ShowTools3").html("o");
            $("#ShowTools4").html("w");
        }
        function onClickHideTools() {
            if ($("#tools").is(":visible")==true){
                fnHideTools();
            } else {
                fnShowTools();
            }
            return false;
        }
        function ShowParsedOptions(lblHL7, txtHl7, lblLocation, txtLocation, drpFieldName){
            
            if ($("#" + drpFieldName).val() == "Parsed Value") {
                $("#" + lblHL7).show();
                $("#" + txtHl7).show();
                $("#" + lblLocation).show();
                $("#" + txtLocation).show();
            } else {
                $("#" + lblHL7).hide();
                $("#" + txtHl7).hide();
                $("#" + lblLocation).hide();
                $("#" + txtLocation).hide();
            }
        }
        function EnableToolsResultOrderBy(txtOrderBy, drpDirection) {
            if ($("#" + drpDirection).val() == "0") {
                $("#" + txtOrderBy).prop('disabled', true);
            } else {
                $("#" + txtOrderBy).prop('disabled', false);
            }
        }
        function SelectExistingQuery(Id, QueryName){
            $('#lblExistingQuerySelected').html(QueryName);
            $('#btnLoadExistingQuery').prop('disabled', false);
            $('#hdnExisitingQueryId').val(Id);
            return false;
        }
        function ShowPopupFilterWindow(FieldName, FieldValue, ControlId, isParsingField) {
            var e = window.event;
            if (e.button == 2) {
                $('#pnlAddToFilter').show();
                $('#txtFilterPopupFieldName').val(FieldName);
                $('#txtFilterPopupFieldValue').val(FieldValue);
                if (!isParsingField) {
                    $('#txtFilterPopupParseLocation').hide();
                    $('#txtFilterPopupParseLocation').val(-1);
                } else {
                    $('#txtFilterPopupParseLocation').show();
                    $('#txtFilterPopupParseLocation').val(1);
                }
                var o = { left: e.clientX, top: e.clientY };
                $('#pnlAddToFilter').offset(o);
                CloseCreateOpenQueryPopupWindow();
                CloseHL7DataPopupMenu();
                return false;
            }
        }
        function ShowCreateOpenQueryPopupWindow() {
            var left = (screen.width / 2) - ($('#pnlCreateOpenQuery').width() / 2);
            var top = (screen.height / 2) - ($('#pnlCreateOpenQuery').height() / 2);
            var o = { left: left, top: top -100 };
            $('#pnlCreateOpenQuery').show();
            $('#pnlCreateOpenQuery').offset(o);
            $('#lblExistingQuerySelected').html('');
            $('#btnLoadExistingQuery').prop('disabled', true);
            CloseHL7DataPopupMenu();
            ClosePopupFilterWindow();
            return false;
            }
        function ShowPopupHL7ParsingWindow(id, type) {
            var MessageURL = 'HL7MessageParser.aspx?HL7MessageId=' + id + '&HL7MessageType=' + type
            $('#lblHL7DataRecordId').html(MessageURL);
            $('#btnOpenHL7Data').off('click');
            $('#btnOpenHL7Data').click(function () {
                return OpenNewHL7DataMessageinNewWindow(MessageURL,"_" + id);
            }); 
            var e = window.event;
            var o = { left: e.clientX, top: e.clientY };
            if (e.button == 2) {
                $('#pnlHL7DataPopupView').show();
                $('#pnlHL7DataPopupView').offset(o);
                CloseCreateOpenQueryPopupWindow();
                ClosePopupFilterWindow();
            }

            return false;
            }
        function ClosePopupFilterWindow() {
            $('#pnlAddToFilter').hide();
            return false;
        }
        function CloseCreateOpenQueryPopupWindow() {
            $('#pnlCreateOpenQuery').hide();
            return false;
        }
        
        function CloseHL7DataPopupMenu() {
            $('#pnlHL7DataPopupView').hide();
            return false;
        }
        function OpenNewHL7DataMessageinNewWindow(id, type) {
             var MessageURL = 'HL7MessageParser.aspx?HL7MessageId=' + id + '&HL7MessageType=' + type
            window.open(MessageURL, "_" + id);
            CloseHL7DataPopupMenu();
            return false;
        }
    </script>
</head>
<body>
    
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
            <Scripts>
                <%--To learn more about bundling scripts in ScriptManager see https://go.microsoft.com/fwlink/?LinkID=301884 --%>
                <%--Framework Scripts--%>
                <asp:ScriptReference Name="MsAjaxBundle" />
                <asp:ScriptReference Name="jquery" />
                <asp:ScriptReference Name="bootstrap" />
                <asp:ScriptReference Name="WebForms.js" Assembly="System.Web" Path="~/Scripts/WebForms/WebForms.js" />
                <asp:ScriptReference Name="WebUIValidation.js" Assembly="System.Web" Path="~/Scripts/WebForms/WebUIValidation.js" />
                <asp:ScriptReference Name="MenuStandards.js" Assembly="System.Web" Path="~/Scripts/WebForms/MenuStandards.js" />
                <asp:ScriptReference Name="GridView.js" Assembly="System.Web" Path="~/Scripts/WebForms/GridView.js" />
                <asp:ScriptReference Name="DetailsView.js" Assembly="System.Web" Path="~/Scripts/WebForms/DetailsView.js" />
                <asp:ScriptReference Name="TreeView.js" Assembly="System.Web" Path="~/Scripts/WebForms/TreeView.js" />
                <asp:ScriptReference Name="WebParts.js" Assembly="System.Web" Path="~/Scripts/WebForms/WebParts.js" />
                <asp:ScriptReference Name="Focus.js" Assembly="System.Web" Path="~/Scripts/WebForms/Focus.js" />
                <asp:ScriptReference Name="WebFormsBundle" />
                <%--Site Scripts--%>
            </Scripts>
        </asp:ScriptManager>
        <asp:HiddenField ID="ToolsHidden" runat="server" />
        <div id="header">
            <%--<iframe id="iHeader" name="_top" style="width: 100%; height:59px; border:none; left:0; top:0; position:sticky;" src="Header.html"></iframe>--%>
            <object type="text/html" data='<%=GetHeaderURL() %>'  style="width: 100%; height:59px; border:none; left:0; top:0; position:sticky;"></object>
        </div>
       
        <div id="tools">
            <table class="small"><tr><td>
                <table>
                    <tr>
                        <td><asp:Button ID="btnToolsSaveQuery" runat="server" Text="Save to DB" OnClick="btnToolsSaveQuery_Click"/></td>
                        <td class="auto-style1">
                            <asp:Button ID="btnToolsCreateNew" runat="server" Text="Create/Open Query" OnClientClick="return ShowCreateOpenQueryPopupWindow()" />
                        </td>
                    </tr>
                </table>
                <asp:Panel ID="pnlToolsQueryControl" runat="server" CssClass="ToolsPanelHeader">
                    <asp:Label ID="lblToolsQueryControl" runat="server" Text=""></asp:Label></asp:Panel>
                <asp:Panel ID="pnlToolsQuery" runat="server"><table class="ToolsTable">
                            <tr>
                                <td colspan="2"><asp:Label ID="lblQueryName" runat="server" Text="Name:"></asp:Label></td></tr>
                            <tr><td colspan="2"><asp:TextBox ID="txtToolsQueryName" runat="server" Width="290" MaxLength="50"></asp:TextBox>
                                    </td></tr>
                            <tr>
                                <td colspan="2">
                                    <asp:Label ID="lblQueryDescription" runat="server" Text="Description:"></asp:Label></td></tr><tr><td colspan="2">
                                        <asp:TextBox ID="txtToolsQueryDescription" runat="server" Rows="2" TextMode="MultiLine" MaxLength="255" Width="290"></asp:TextBox></td>
                            </tr>
                            <tr> <td colspan="2">
                                <asp:CheckBox ID="chkToolsUseDateRange" runat="server" Text="Use Date Range:" TextAlign="Left" OnClick="EnableToolsDateRange()" />
                            </td></tr>
                            <tr><td colspan="2">
                                <asp:Label ID="Label2" runat="server" Text="Start"></asp:Label></td></tr>
                            <tr><td>
                                <asp:Label ID="Label3" runat="server" Text="Date:"></asp:Label></td><td>
                                    <asp:Label ID="Label4" runat="server" Text="Time:"></asp:Label></td></tr>
                            <tr><td>
                               <asp:TextBox ID="txtToolsStartDate" runat="server" TextMode="Date" Width="150"></asp:TextBox>
                                <asp:HiddenField ID="hdnToolsStartDate" runat="server" />
                                </td>
                                <td>
                                    
                                    <asp:TextBox ID="txtToolsStartTime" runat="server" Width="130" TextMode="Time"></asp:TextBox>
                                    <asp:HiddenField ID="hdnToolsStartTime" runat="server" />
                                </td>
                            </tr>
                             <tr><td colspan="2">
                                <asp:Label ID="Label5" runat="server" Text="End"></asp:Label></td></tr>
                            <tr><td>
                                <asp:Label ID="Label6" runat="server" Text="Date:"></asp:Label></td><td>
                                <asp:Label ID="Label7" runat="server" Text="Time:"></asp:Label></td></tr>
                            <tr>
                                <td>
                                <asp:TextBox ID="txtToolsEndDate"  TextMode="Date" runat="server" Width="150"></asp:TextBox>
                                    <asp:HiddenField ID="hdnToolsEndDate" runat="server" />
                                </td><td>
                                    <asp:TextBox ID="txtToolsEndTime" runat="server" Width="130" TextMode="Time"></asp:TextBox>
                                    <asp:HiddenField ID="hdnToolsEndTime" runat="server" />
                                </td></tr>
                            <tr><td colspan="2"><table style="width: 100%; border-collapse:collapse; padding: 0; border-width: 0px; text-align:center;">
                                <tr>
                                    <td><asp:Button ID="btnToolsCancelQuery" runat="server" Text="Cancel Changes" OnClick="btnToolsCancelQuery_Click"/></td>
                                    <td><asp:Button ID="btnToolsUpdateQuery" runat="server" Text="Apply Changes" OnClick="btnToolsUpdateQuery_Click"/></td>
                                </tr>
                                    </table>
                                
                                  </td></tr>
                        </table></asp:Panel>
                <ajaxToolkit:CollapsiblePanelExtender ID="pnlToolsQuery_CollapsiblePanelExtender" runat="server" BehaviorID="pnlToolsQuery_CollapsiblePanelExtender" CollapsedText="+ Query Information" ExpandedText="- Query Information" TargetControlID="pnlToolsQuery" TextLabelID="lblToolsQueryControl" CollapseControlID="pnlToolsQueryControl" ExpandControlID="pnlToolsQueryControl" />
                <asp:Panel ID="pnlToolsResultsControl" runat="server" CssClass="ToolsPanelHeader">
                    <asp:Label ID="lblToolsResultsControl" runat="server" Text=""></asp:Label></asp:Panel>
                <asp:Panel ID="pnlToolsResults" runat="server">
                    <asp:UpdatePanel ID="upnlToolsResults" runat="server">
            <ContentTemplate>
                       <asp:GridView ID="grdToolsResults" runat="server" AutoGenerateColumns="False" GridLines="Horizontal" CssClass="ToolsGrid" 
            OnRowCommand="grdToolsResults_RowCommand" 
            OnRowEditing="grdToolsResults_RowEditing" 
            OnRowUpdating="grdToolsResults_RowUpdating"
            OnRowDataBound="grdToolsResults_RowDataBound"
            OnRowDeleting="grdToolsResults_RowDeleting"
            OnRowCancelingEdit="grdToolsResults_RowCancelingEdit"
                          
            DataKeyNames="Id">
            <HeaderStyle BackColor="#8a3a81" Font-Size="Smaller" ForeColor="White" />
            <RowStyle Font-Size="Smaller" />
            <EditRowStyle BackColor="orange" />
            <Columns>
                
                <asp:TemplateField>
                    <ItemTemplate>
                        <table class="small">
                            <tr><td style="transform:rotate(90deg); text-align: center; font-size:x-small; font-weight:bold; color:black; " >
                                <asp:LinkButton ID="btnMoveUp" runat="server" CommandName="ToolsMoveUp" CommandArgument='<%#Bind("QueryResultOrder")%>'><</asp:LinkButton></td><td></tr>
                            <tr><td style="transform:rotate(90deg); text-align: center; font-size:x-small; font-weight:bold;">
                                <asp:LinkButton ID="btnMoveDown" runat="server" CommandName="ToolsMoveDown" CommandArgument='<%#Bind("QueryResultOrder")%>' >></asp:LinkButton></td></tr>
                        </table>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Field Name" HeaderStyle-HorizontalAlign="Left" HeaderStyle-Font-Size="Smaller">
                    <EditItemTemplate>
                        <table class="small">
                            <tr>
                                <td colspan="2">
                                    <asp:DropDownList ID="drpToolsResultsFieldNames" runat="server" Width="105px" Font-Size="Smaller">
                                    </asp:DropDownList>
                                </td>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblToolsResultsHL7" runat="server" Text="HL7" Font-Size="Smaller"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblToolsParseLocation" runat="server" Text="Location" Font-Size="Smaller"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtToolsResultsHL7" runat="server" Width="50px" Font-Size="Smaller"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtToolsParseLocation" runat="server" Width="45px" TextMode="Number" Font-Size="Smaller"></asp:TextBox>
                                    </td>
                                </tr>
                            </tr>
                        </table>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblToolsResultsFileName" runat="server" Text='<%# FormatStringLength(Eval("FieldName"),15) %>' Width="106" ToolTip='<%# Eval("FieldName") %>' Font-Size="Smaller"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Column Alias" HeaderStyle-HorizontalAlign="Left" HeaderStyle-Font-Size="Smaller"> 
                    <EditItemTemplate>
                        <asp:TextBox ID="txtToolsResultsColumnAlias" runat="server" Text='<%# Eval("ColumnAlias") %>' Width="100" ToolTip='<%# Eval("ColumnAlias") %>' Font-Size="Smaller"></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblToolsResultsColumnAlias" runat="server" Text='<%# FormatStringLength(Eval("ColumnAlias"),25) %>' Width="104" ToolTip='<%# Eval("ColumnAlias") %>' Font-Size="Smaller"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
               <asp:TemplateField HeaderText="Sort" HeaderStyle-HorizontalAlign="Left" HeaderStyle-Font-Size="Smaller">
                   <EditItemTemplate>
                        <asp:TextBox ID="txtToolsResultsOrderBy" runat="server" Text='<%# Eval("OrderBy") %>' Width="30" TextMode="Number" Font-Size="Smaller"></asp:TextBox>
                   </EditItemTemplate>
                  <ItemTemplate>
                      <asp:Label ID="lblToolsResultsOrderBy" runat="server" Text='<%# Eval("OrderBy") %>' Width="34" Font-Size="Smaller"></asp:Label>
                      </ItemTemplate>
               </asp:TemplateField>
                <asp:TemplateField HeaderText="Dir" HeaderStyle-HorizontalAlign="Left" HeaderStyle-Font-Size="Smaller">
                    <EditItemTemplate>
                        <asp:DropDownList ID="drpToolsResultsOrderDirection" runat="server" Width="50"></asp:DropDownList>
                    </EditItemTemplate>
                  <ItemTemplate>
                      <asp:Label ID="lblToolsResultsOrderDirection" runat="server" Text='<%# FormatOrderByDirectionForTools(Eval("OrderByDirection")) %>' Width="52" Font-Size="Smaller"></asp:Label>
                      </ItemTemplate>
               </asp:TemplateField>
               <asp:TemplateField ShowHeader="False">
                    <EditItemTemplate>
                        <table class="small"><tr><td style="font-size: small"><asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="True" CommandName="Update" Text="U" Font-Size="Smaller"></asp:LinkButton>
                                   </td></tr><tr><td style="font-size: small"><asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" CommandName="Cancel" Text="C" Font-Size="Smaller"></asp:LinkButton></td></tr></table>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <table class="small"><tr><td style="font-size: small"><asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Edit" Text="E" Font-Size="Smaller"></asp:LinkButton>
                             </td></tr><tr><td style="font-size: small"><asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="True" CommandName="Delete" Text="D" Font-Size="Smaller" OnClientClick="return confirm('Do you want to delete this field?');"></asp:LinkButton></td></tr></table>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
                                
                                    </asp:GridView>
                 </ContentTemplate>
                              <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="grdToolsResults" />
                                  <asp:AsyncPostBackTrigger  ControlID="btnToolsResultsAdd" />
                                <asp:AsyncPostBackTrigger  ControlID="btnToolsResultsCancel"  />
                </Triggers>
        </asp:UpdatePanel>
        <table class="small"><tr><td>
                <asp:Button ID="btnToolsResultsAdd" runat="server" Text="Add New Field" OnClick="btnToolsResultsAdd_Click" /></td><td>
                <asp:Button ID="btnToolsResultsCancel" runat="server" Text="Cancel Changes" OnClick="btnToolsResultsCancel_Click" /></td><td>
                    <asp:Button ID="btnToolsResultsApply" runat="server" Text="Apply Changes" OnClick="btnToolsResultsApply_Click" /></td></tr></table>
                </asp:Panel>
                
                <ajaxToolkit:CollapsiblePanelExtender ID="pnlToolsResults_CollapsiblePanelExtender" runat="server" BehaviorID="pnlToolsResults_CollapsiblePanelExtender" CollapseControlID="pnlToolsResultsControl" CollapsedText=" + Results" ExpandControlID="pnlToolsResultsControl" ExpandedText=" - Results" TargetControlID="pnlToolsResults" TextLabelID="lblToolsResultsControl" />
                
                <asp:Panel ID="pnlToolsConditionsControl" runat="server" CssClass="ToolsPanelHeader">
                    <asp:Label ID="lblToolsConditionsControl" runat="server" Text=""></asp:Label></asp:Panel>
                <asp:Panel ID="pnlToolsConditions" runat="server">
                    <asp:UpdatePanel ID="upnlToolsCondition" runat="server"><ContentTemplate>
  <asp:GridView ID="grdToolsCondition" runat="server" AutoGenerateColumns="False" GridLines="Horizontal" CssClass="ToolsGrid" 
            OnRowCommand="grdToolsCondition_RowCommand"
            OnRowEditing="grdToolsCondition_RowEditing"
            OnRowUpdating="grdToolsCondition_RowUpdating"
            OnRowDataBound="grdToolsCondition_RowDataBound"
            OnRowDeleting="grdToolsCondition_RowDeleting"
            OnRowCancelingEdit="grdToolsCondition_RowCancelingEdit"
          DataKeyNames="Id">
            <HeaderStyle BackColor="#8a3a81" Font-Size="Smaller" ForeColor="White" />
            <RowStyle Font-Size="Smaller" />
            <EditRowStyle BackColor="orange" VerticalAlign="Top"/>
            <Columns>
                
                <asp:TemplateField>
                    <ItemTemplate>
                        <table class="small">
                            <tr><td style="transform:rotate(90deg); text-align: center; font-size:x-small; font-weight:bold; color:black; " >
                                <asp:LinkButton ID="btnToolsConditionMoveUp" runat="server" CommandName="ToolsMoveUp" CommandArgument='<%#Bind("QueryConditionOrder")%>'><</asp:LinkButton></td><td></tr>
                            <tr><td style="transform:rotate(90deg); text-align: center; font-size:x-small; font-weight:bold;">
                                <asp:LinkButton ID="btnToolsConditionMoveDown" runat="server" CommandName="ToolsMoveDown" CommandArgument='<%#Bind("QueryConditionOrder")%>' >></asp:LinkButton></td></tr>
                        </table>
                    </ItemTemplate>
                </asp:TemplateField>
                 <asp:TemplateField HeaderText="Join" HeaderStyle-HorizontalAlign="Left" HeaderStyle-Font-Size="Smaller">
                    <EditItemTemplate>
                        <asp:DropDownList ID="drpToolsConditionJoinType" runat="server" Width="42" Font-Size="Smaller"></asp:DropDownList>
                    </EditItemTemplate>
                  <ItemTemplate>
                      <asp:Label ID="lblToolsConditionJoinType" runat="server" Text='<%# FormatConditionTypesForTools(Eval("JoinType")) %>' Width="42" Font-Size="Smaller"></asp:Label>
                      </ItemTemplate>
               </asp:TemplateField>
                <asp:TemplateField HeaderText="Field Name" HeaderStyle-HorizontalAlign="Left" HeaderStyle-Font-Size="Smaller">
                    <EditItemTemplate>
                        <table class="small">
                            <tr>
                                <td colspan="2">
                                    <asp:DropDownList ID="drpToolsConditionFieldNames" runat="server" Width="95px" Font-Size="Smaller">
                                    </asp:DropDownList>
                                </td>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblToolsConditionHL7" runat="server" Text="HL7" Font-Size="Smaller"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblToolsConditionParseLocation" runat="server" Text="Location" Font-Size="Smaller"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtToolsConditionHL7" runat="server" Width="40px" Font-Size="Smaller"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtToolsConditionParseLocation" runat="server" Width="35px" TextMode="Number" Font-Size="Smaller"></asp:TextBox>
                                    </td>
                                </tr>
                            </tr>
                        </table>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblToolsConditionFileName" runat="server" Text='<%# FormatStringLength(Eval("FieldName"),15) %>' Width="97" ToolTip='<%# Eval("FieldName") %>' Font-Size="Smaller"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                 <asp:TemplateField ShowHeader="False">
                    <EditItemTemplate>
                        <asp:DropDownList ID="drpToolsConditionCompareType" runat="server" Width="40" Font-Size="Smaller"></asp:DropDownList>
                    </EditItemTemplate>
                  <ItemTemplate>
                      <asp:Label ID="lblToolsConditionCompareType" runat="server" Text='<%# FormatConditionTypesForTools(Eval("CompareType")) %>' Width="42" Font-Size="Smaller"></asp:Label>
                      </ItemTemplate>
               </asp:TemplateField>
                <asp:TemplateField HeaderText="Value" HeaderStyle-HorizontalAlign="Left" HeaderStyle-Font-Size="Smaller"> 
                    <EditItemTemplate>
                        <asp:TextBox ID="txtToolsConditionValue" runat="server" Text='<%# Eval("ConditionValue") %>' Width="89" ToolTip='<%# Eval("ConditionValue") %>' Font-Size="Smaller"></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblToolsConditionColumnAlias" runat="server" Text='<%# FormatStringLength(Eval("ConditionValue"),25) %>' Width="93" ToolTip='<%# Eval("ConditionValue") %>' Font-Size="Smaller"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Use" HeaderStyle-HorizontalAlign="Left" HeaderStyle-Font-Size="Smaller">
                    <EditItemTemplate>
                        <asp:CheckBox ID="chkToolsConditionUse" Checked='<%# Eval("UseCondition") %>' runat="server"  Font-Size="Smaller"/>
                    </EditItemTemplate>
                  <ItemTemplate>
                      <asp:CheckBox ID="chkToolsConditionUse" Checked='<%# Eval("UseCondition") %>' runat="server"  Font-Size="Smaller" Enabled="False" />
                      </ItemTemplate>
               </asp:TemplateField>
               <asp:TemplateField ShowHeader="False">
                    <EditItemTemplate>
                        <table class="small"><tr><td style="font-size: small"><asp:LinkButton ID="btnToolsConditionUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="U" Font-Size="Smaller"></asp:LinkButton>
                                   </td></tr><tr><td style="font-size: small"><asp:LinkButton ID="btnToolsConditionCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="C" Font-Size="Smaller"></asp:LinkButton></td></tr></table>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <table class="small"><tr><td style="font-size: small"><asp:LinkButton ID="btnToolsConditionEdit" runat="server" CausesValidation="False" CommandName="Edit" Text="E" Font-Size="Smaller"></asp:LinkButton>
                             </td></tr><tr><td style="font-size: small"><asp:LinkButton ID="btnToolsConditionDelete" runat="server" CausesValidation="True" CommandName="Delete" Text="D" Font-Size="Smaller" OnClientClick="return confirm('Do you want to delete this field?');"></asp:LinkButton></td></tr></table>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
                            </ContentTemplate>
                            <Triggers>
                    <asp:AsyncPostBackTrigger  ControlID="grdToolsCondition" />
                                <asp:AsyncPostBackTrigger  ControlID="btnToolsConditionAdd" />
                                <asp:AsyncPostBackTrigger  ControlID="btnToolsConditionCancel"  />
                </Triggers>
                        </asp:UpdatePanel>
        <table class="small"><tr><td>
                <asp:Button ID="btnToolsConditionAdd" runat="server" Text="Add New Field" OnClick="btnToolsConditionAdd_Click"/></td><td>
                <asp:Button ID="btnToolsConditionCancel" runat="server" Text="Cancel Changes" OnClick="btnToolsConditionCancel_Click" /></td><td>
                    <asp:Button ID="btnToolsConditionApply" runat="server" Text="Apply Changes" OnClick="btnToolsConditionApply_Click"/></td></tr></table>
        
                </asp:Panel>
                <ajaxToolkit:CollapsiblePanelExtender ID="pnlToolsConditions_CollapsiblePanelExtender" runat="server" BehaviorID="pnlToolsConditions_CollapsiblePanelExtender" CollapseControlID="pnlToolsConditionsControl" CollapsedText=" + Conditions" ExpandControlID="pnlToolsConditionsControl" ExpandedText=" - Conditions" TargetControlID="pnlToolsConditions" TextLabelID="lblToolsConditionsControl" />
                </td></tr></table>
            
            
            </div>
               
        <div id="divider">
            <table onclick="onClickHideTools();" class="DividerTable"><tr><td>
                <br />
                <br />
                <br />
                <br />
                <br />
                <br />
                <br />
                <br />
                </td></tr><tr><td id="ShowTools1" class="Rotate">H</td></tr><tr><td id="ShowTools2" class="Rotate">i</td></tr><tr><td id="ShowTools3" class="Rotate">d</td></tr><tr><td id="ShowTools4" class="Rotate">e</td></tr><tr><td>&nbsp;</td></tr><tr><td class="Rotate">T</td></tr><tr><td class="Rotate">o</td></tr><tr><td class="Rotate">o</td></tr><tr><td class="Rotate">l</td></tr><tr><td class="Rotate">s</td></tr><tr><td>
                <br />
                <br />
                <br />
                <br />
                    <br />
                <br />
                <br />
                <br />
                </td></tr></table>
            
            </div>
        <div id="data">
            <asp:UpdatePanel ID="upnlHL7Data" runat="server" ViewStateMode="Enabled" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:GridView ID="grdHL7Data" runat="server" AutoGenerateColumns="False" AllowPaging="True" PageSize="30" OnPageIndexChanging="grdHL7Data_PageIndexChanging" GridLines="Horizontal" Font-Size="Small" HeaderStyle-HorizontalAlign="Left" DataKeyNames="Id" EnablePersistedSelection="True" EnableViewState="True" ViewStateMode="Enabled" ShowHeaderWhenEmpty="True">
            <AlternatingRowStyle BackColor="#E2E2E2" Wrap ="false" />
                        <RowStyle BackColor="White" Wrap="false" />
        <HeaderStyle BackColor="#8a3a81" Font-Size="Small" ForeColor="White" Wrap="False" />    
        </asp:GridView>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="grdHL7Data" EventName="PageIndexChanging" />
                </Triggers>
            </asp:UpdatePanel>
            <ajaxToolkit:UpdatePanelAnimationExtender ID="upnlHL7Data_UpdatePanelAnimationExtender" runat="server" BehaviorID="upnlHL7Data_UpdatePanelAnimationExtender" TargetControlID="upnlHL7Data">
                      <Animations>
                          <OnUpdating><Sequence>
                              <ScriptAction Script="var b = $find('upnlHL7Data_UpdatePanelAnimationExtender'); b._originalHeight = b._element.offsetHeight;" />
                              <StyleAction Attribute="overflow" Value="hidden" />
                              <Parallel duration=".25" Fps="30">
                                <FadeOut AnimationTarget="data" minimumOpacity=".2" />
                                <%--<Resize Height="0" />--%>
                           </Parallel>
                                      </Sequence></OnUpdating>
                          <OnUpdated><Sequence>
                              <Parallel duration=".25" Fps="30">
                                  <FadeIn AnimationTarget="data" minimumOpacity=".2" />
                                  <%--<Resize HeightScript="$find('upnlHL7Data_UpdatePanelAnimationExtender')._originalHeight" />--%>
                                  </Parallel>
                                     </Sequence></OnUpdated>
                      </Animations>      
            </ajaxToolkit:UpdatePanelAnimationExtender>

        </div>
        <div id="status"> 
            <asp:Label ID="lblSql" runat="server" Text="SQL" Font-Size="Smaller"></asp:Label>
        </div>
        <asp:Panel ID="pnlAddToFilter" runat="server" CssClass="FilterPopupWindow">
            <table class="FilterTable">
                <tr class="PopupHeader">
                    <td colspan="5">
                        <asp:Label ID="lblAddFilter" runat="server" Text="Add Data Filter"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right">
                        <asp:TextBox ID="txtFilterPopupFieldName" runat="server" Width="100px"></asp:TextBox>
                    </td>
                    <td style="text-align: right">
                        <asp:TextBox ID="txtFilterPopupParseLocation" runat="server" MaxLength="2" Width="20px">0</asp:TextBox>
                    </td>
                    <td colspan="2" style="width: 25px">
                        <asp:DropDownList ID="drpFilterPopupCompareType" runat="server">
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:TextBox ID="txtFilterPopupFieldValue" runat="server" Width="100px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align: right">
                        <asp:Label ID="Label1" runat="server" Text="Join Type:"></asp:Label>
                    </td>
                    <td colspan="2" style="width: 25px">
                        <asp:DropDownList ID="drpFilterPopupJoinType" runat="server">
                        </asp:DropDownList>
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="5" style="text-align: center">
                        <asp:CheckBox ID="chkFilterPopupInclude" runat="server" Text="Also add to active query." />
                    </td>
                </tr>
                <tr>
                    <td colspan="3" style="text-align: center">
                        <asp:Button ID="btnCancelFilter" runat="server" CausesValidation="False" EnableViewState="False" OnClientClick="return ClosePopupFilterWindow();" Text="Cancel" Width="60px" />
                    </td>
                    <td colspan="2" style="text-align: center">
                        <asp:Button ID="btnAddFilter" runat="server" OnClick="btnAddFilter_Click" Text="Add" Width="60px" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        
        
      
        <asp:Panel ID="pnlCreateOpenQuery" runat="server" CssClass="CreateOpenQueryPopupWindow">
            <table class="small">
                <tr>
                    <td class="PopupHeader">
                        <asp:Label ID="Label8" runat="server" Text="Create/Open Query"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblSelectExistingQuery" runat="server" Text="Select Existing Query:"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td><div style="height: 67px; overflow-y:scroll; border-color: #bcd631; border-style:solid; border-width: 2px;">
                        <asp:GridView ID="grdExistingQuery" runat="server" AutoGenerateColumns="False" DataKeyNames="Id" GridLines="None" OnRowDataBound="grdExistingQuery_RowDataBound" ShowHeader="False" >
                            <Columns>
                                <asp:TemplateField ItemStyle-Width="130">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnSelectExistingQuery" runat="server" Font-Bold="True" Text='<%# FormatStringLength(Eval("QueryName"),20) %>' ToolTip='<%# Eval("QueryName") %>'></asp:LinkButton>
                                    </ItemTemplate>
                                    
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-Width="200">
                                    
                                    <ItemTemplate>
                                        <asp:Label ID="Label1" runat="server" Text='<%# FormatStringLength(Eval("QueryDescription"),40) %>' ToolTip='<%# Eval("QueryDescription") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-Width="75" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Label ID="Label2" runat="server" Text='<%# Bind("QueryDate", "{0:d}") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox ID="chkToolsNewQuerySavePreviousQuery" runat="server" Text="Save current query" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox ID="chkToolsNewQueryUpdateChanges" runat="server" Text="Update current query with all changes" />
                    </td>
                </tr>
                <tr>
                    <td><table style="border-style: solid; border-width: 1px; border-top-color:  #bcd631; border-right-color:  #bcd631; border-bottom-color:  #bcd631; border-left-color:  #bcd631; width: 100%;"><tr><td>
                        <asp:Label ID="lblSelected" runat="server" Text="Selected Query:"></asp:Label></td>
                    <td >
                        <asp:Label ID="lblExistingQuerySelected" runat="server" Text="No Query Selected" Width="273px" Font-Bold="True"></asp:Label>
                        <asp:HiddenField ID="hdnExisitingQueryId" runat="server" />
                        </td></tr></table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table style="width: 100%">
                            <tr>
                                <td style="text-align: center">
                                    <asp:Button ID="btnToolsNewQueryCancel" runat="server" OnClientClick="return CloseCreateOpenQueryPopupWindow()" Text="Cancel" Width="70px" />
                                </td>
                                <td style="text-align: center">
                                    <asp:Button ID="btnToolsNewQueryOK" runat="server" OnClick="btnToolsNewQueryOK_Click" Text="Create New" Width="80px" />
                                </td><td>
                                    <asp:Button ID="btnLoadExistingQueryAsNew" runat="server" Text="Load Existing as New" OnClick="btnLoadExistingQueryAsNew_Click"/>
                                     </td>
                                <td style="text-align: center">
                                    <asp:Button ID="btnLoadExistingQuery" runat="server" Text="Load Existing" OnClick="btnLoadExistingQuery_Click" Enabled="False" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnlHL7DataPopupView" runat="server" CssClass="HL7DataPopupMenu">
<table class="small"><tr><td>
    <asp:Button ID="btnOpenHL7Data" runat="server" Text="Retrieve HL7 Message"  />
    <asp:Label ID="lblHL7DataRecordId" runat="server" Text="Label"></asp:Label></td></tr>
    <tr><td>
        <asp:Button ID="btnCloseHL7DataPopupMenu" runat="server" Text="Close" OnClientClick="return CloseHL7DataPopupMenu()" /></td></tr>
</table>

        </asp:Panel>
      
    </form>
</body>
</html>
