<%@ Page Title="" Language="C#" MasterPageFile="~/masters/MasterPage.master" AutoEventWireup="true" CodeBehind="NameEncode.aspx.cs" Inherits="PkmnFoundations.Web.test.NameEncode" %>
<%@ Register TagPrefix="pf" Namespace="PkmnFoundations.Web" Assembly="PkmnFoundations.Web" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHead" runat="server">
    <pf:HeaderColour ID="HeaderColour1" CssClass="test" runat="server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cpMain" runat="server">
<form id="theForm" runat="server">
<div>Enter text to encode:
<asp:TextBox ID="txtName" Width="600px" runat="server" />
<asp:Button ID="btnSubmit" Text="Encode" OnClick="btnSubmit_Click" Width="60px" runat="server" />
</div>
<div>
<asp:Literal ID="litName" runat="server" />
</div>
</form>
</asp:Content>
