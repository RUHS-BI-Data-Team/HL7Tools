<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="HL7AnalyserWeb.Login" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="prepend-8 span-12">    &nbsp;&nbsp;
    </div>
    <div class="prepend-8 span-12">
        <asp:Label ID="lblUserName" runat="server" Text="Employee ID :" AssociatedControlID="UsernameTextBox" CssClass="span-3"></asp:Label><asp:TextBox ID="UsernameTextBox" runat="server" CssClass="span-4 bypass-state-tracking" />
    </div>
    <div class="prepend-8 span-12">    
        <asp:Label ID="lblPassword" runat="server" Text="Password :" AssociatedControlID="PasswordTextBox" CssClass="span-3"></asp:Label><asp:TextBox ID="PasswordTextBox" runat="server" TextMode="Password" CssClass="span-4 bypass-state-tracking" />
    </div>
    <div class="span-20"></div>    
    <div class="prepend-12 span-8">
        <asp:Button ID="SignInButton" runat="server" Text="Sign In" OnClick="SignInButton_Click"/>
    </div>
</asp:Content>
