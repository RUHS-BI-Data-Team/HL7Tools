<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HL7DataAnalyser._Default" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxToolkit" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        table.menu{
            background-color: #213f8c;
            font-size:smaller;
            border-collapse:collapse;
            width:100%;
            border: solid thin white;
            
        }
        .MenuItem{
            border: solid 1px;
            border-color: orange;
            color:white;
            background-color:#213f8c;
            text-align:center;
            cursor:pointer;
        }
        .PopupHeader{
            background-color:orange;
            font-size:larger;
        }
        .PopupWindow{
            border:1px solid;
            border-color:greenyellow;
            font-size:smaller;

        }
        .PopupFooter{
            text-align:center;
            width:50%;
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
        
        td.leftMenuCol{
            width: 20px;
            background-color:lightgray;
            height: 20px
        }
       
        Table.dropDownMenu {
            width: 100%;
            padding: 0px;
            border: solid 1px;
            border-color: black;
            font-size:smaller;
            border-collapse: collapse;
            background-color:white;
        }
        .selectable{
            cursor:pointer;
            color:#213f8c;
        }
              
                      
    </style>
    <div>
    <table class="menu">
        <tr>
            <td>
               
            <asp:Label ID="mnuQuries" runat="server" Text="Queries" CssClass="MenuItem" Width="80px"></asp:Label>
            <ajaxToolkit:DropDownExtender ID="mnuQuries_DropDownExtender" runat="server" BehaviorID="mnuQuries_DropDownExtender" DropDownControlID="pnlQueriesMenu" DynamicServicePath="" TargetControlID="mnuQuries" HighlightBackColor="#5B71A3">
            </ajaxToolkit:DropDownExtender>
            <asp:Label ID="mnuConditions" runat="server" CssClass="MenuItem" Text="Conditions" Width="100px"></asp:Label>
            <ajaxToolkit:DropDownExtender ID="mnuConditions_DropDownExtender" runat="server" BehaviorID="mnuConditions_DropDownExtender" DropDownControlID="pnlConditionsMenu" DynamicServicePath="" TargetControlID="mnuConditions" HighlightBackColor="#5B71A3">
            </ajaxToolkit:DropDownExtender>
                        
            </td>
        
        </tr>
    </table>
        </div>
    <asp:GridView ID="grdHL7Data" runat="server" AllowPaging="True" AutoGenerateColumns="False" DataKeyNames="Id" DataSourceID="sqlHL7Data" BackColor="White" BorderColor="#336666" BorderStyle="Double" BorderWidth="3px" CellPadding="4" GridLines="Horizontal">
        <FooterStyle BackColor="White" ForeColor="#333333" />
        <HeaderStyle BackColor="#336666" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#336666" ForeColor="White" HorizontalAlign="Center" />
        <RowStyle BackColor="White" ForeColor="#333333" />
        <SelectedRowStyle BackColor="#339966" Font-Bold="True" ForeColor="White" />
        <SortedAscendingCellStyle BackColor="#F7F7F7" />
        <SortedAscendingHeaderStyle BackColor="#487575" />
        <SortedDescendingCellStyle BackColor="#E5E5E5" />
        <SortedDescendingHeaderStyle BackColor="#275353" />
    </asp:GridView>

    <asp:SqlDataSource ID="sqlHL7Data" runat="server" ConnectionString="<%$ ConnectionStrings:HL7DataConnectionStringADAuth %>" SelectCommand="SELECT [Id], [HL7Data], [InsertDateTime], [HL7ControlId], [HL7SendingApplication], [HL7MessageDate], [MRN], [HL7SendingFacility], [HL7Event], [HL7PatientClass], [HL7Encounter] FROM [tblADT] WHERE ([HL7MessageDate] &gt; @HL7MessageDate)">
        <SelectParameters>
            <asp:Parameter DefaultValue="8/16/2018" Name="HL7MessageDate" Type="DateTime" />
        </SelectParameters>
    </asp:SqlDataSource>

    <asp:SqlDataSource ID="sqlHL7TableColumns" runat="server" ConnectionString="<%$ ConnectionStrings:HL7DataConnectionStringADAuth %>" SelectCommand="Select column_name  as 'Column' from INFORMATION_SCHEMA.COLUMNS
where table_name = 'tblADT'
And COLUMN_NAME not in ('Id','InsertDateTime')"></asp:SqlDataSource>

    <asp:Panel ID="pnlDateFilter" runat="server" Width="250px">
        <table style="width: 100%" class="PopupWindow">
            <tr>
                <td colspan="3" class="PopupHeader">
                    <asp:Label ID="lblDateFilterTitle" runat="server" Text="Date Filter"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblStartDate" runat="server" Text="Start Date:"></asp:Label>
                </td>
                <td style="width: 80px">
                    <asp:TextBox ID="txtStartDate" runat="server" Width="80px"></asp:TextBox>
                    <ajaxToolkit:CalendarExtender ID="txtStartDate_CalendarExtender" runat="server" BehaviorID="txtStartDate_CalendarExtender" TargetControlID="txtStartDate" />
                    <ajaxToolkit:MaskedEditExtender ID="txtStartDate_MaskedEditExtender" runat="server" BehaviorID="txtStartDate_MaskedEditExtender" Century="2000" CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" CultureDateFormat="" CultureDatePlaceholder="" CultureDecimalPlaceholder="" CultureThousandsPlaceholder="" CultureTimePlaceholder="" Mask="99/99/9999" MaskType="Date" TargetControlID="txtStartDate" />
                </td>
                <td style="width: 60px">
                    <asp:TextBox ID="txtStartTime" runat="server" Width="60">00:00:00</asp:TextBox>
                    <ajaxToolkit:MaskedEditExtender ID="txtStartTime_MaskedEditExtender" runat="server" BehaviorID="txtStartTime_MaskedEditExtender" Century="2000" CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" CultureDateFormat="" CultureDatePlaceholder="" CultureDecimalPlaceholder="" CultureThousandsPlaceholder="" CultureTimePlaceholder="" Mask="99:99:99" MaskType="Time" TargetControlID="txtStartTime" UserTimeFormat="TwentyFourHour" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblEndDate" runat="server" Text="End Date:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtEndDate" runat="server" Width="80px"></asp:TextBox>
                    <ajaxToolkit:CalendarExtender ID="txtEndDate_CalendarExtender" runat="server" BehaviorID="txtEndDate_CalendarExtender" TargetControlID="txtEndDate" />
                    <ajaxToolkit:MaskedEditExtender ID="txtEndDate_MaskedEditExtender" runat="server" BehaviorID="txtEndDate_MaskedEditExtender" Century="2000" CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" CultureDateFormat="" CultureDatePlaceholder="" CultureDecimalPlaceholder="" CultureThousandsPlaceholder="" CultureTimePlaceholder="" Mask="99/99/9999" MaskType="Date" TargetControlID="txtEndDate" />
                </td>
                <td>
                    <asp:TextBox ID="txtEndTime" runat="server" Width="60px">00:00:00</asp:TextBox>
                    <ajaxToolkit:MaskedEditExtender ID="txtEndTime_MaskedEditExtender" runat="server" BehaviorID="txtEndTime_MaskedEditExtender" Century="2000" CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" CultureDateFormat="" CultureDatePlaceholder="" CultureDecimalPlaceholder="" CultureThousandsPlaceholder="" CultureTimePlaceholder="" TargetControlID="txtEndTime" Mask="99:99:99" MaskType="Time" />
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    <asp:CheckBox ID="chkUseDateFilter" runat="server" Text="Use Date Filter:" Checked="True" />
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    <table align="center" class="menuTable">
                        <tr>
                            <td class="PopupFooter">
                                <asp:Button ID="btnCancelDateFilter" runat="server" CommandName="Cancel" Text="Cancel" />
                            </td>
                            <td class="PopupFooter">
                                <asp:Button ID="btnApplyDateFilter" runat="server" Text="Apply" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="pnlQueriesMenu" runat="server">
        <table class="dropDownMenu">
            <tr>
                <td class="leftMenuCol">&nbsp;&nbsp;&nbsp;</td>
                <td>
                    <asp:Label ID="Label1" runat="server" Text="Save Query" CssClass="selectable"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="leftMenuCol">&nbsp;</td>
                <td>
                    <asp:Label ID="Label2" runat="server" Text="Open Query" CssClass="selectable"></asp:Label>
                </td>
            </tr>
        </table>

    </asp:Panel>
    <asp:Panel ID="pnlConditionsMenu" runat="server" >
        <table class="dropDownMenu">
            <tr>
                <td class="leftMenuCol">&nbsp;</td>
                <td>
                    <asp:Label ID="lblDateFilter" runat="server" Text="Date Filter" CssClass="selectable"></asp:Label>
                    <ajaxToolkit:ModalPopupExtender ID="lblDateFilter_ModalPopupExtender" runat="server" BehaviorID="lblDateFilter_ModalPopupExtender" CancelControlID="btnCancelDateFilter" DynamicServicePath="" OkControlID="" PopupControlID="pnlDateFilter" TargetControlID="lblDateFilter">
                    </ajaxToolkit:ModalPopupExtender>
                </td>
            </tr>
            <tr>
                <td class="leftMenuCol">&nbsp;</td>
                <td>
                    <asp:Label ID="lblFieldsFilter" runat="server" CssClass="selectable" Text="Fields Filter"></asp:Label>
                    <ajaxToolkit:ModalPopupExtender ID="lblFieldsFilter_ModalPopupExtender" runat="server" BehaviorID="lblFieldsFilter_ModalPopupExtender" CancelControlID="btnCancelFieldsFilter" DynamicServicePath="" OkControlID="btnApplyFieldsFilter" PopupControlID="pnlFieldsFilter" TargetControlID="lblFieldsFilter">
                    </ajaxToolkit:ModalPopupExtender>
                </td>
            </tr>
            <tr>
                <td class="leftMenuCol">&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td class="leftMenuCol">&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
        </table>

    </asp:Panel>
    <asp:Panel ID="pnlFieldsFilter" runat="server" Width="200px">

    </asp:Panel>
</asp:Content>
