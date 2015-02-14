<%@ Page Title="" Language="C#" MasterPageFile="~/masters/MasterPage.master" AutoEventWireup="true" CodeBehind="NameDecode.aspx.cs" Inherits="PkmnFoundations.GTS.test.NameDecode" %>
<%@ Register TagPrefix="pf" Namespace="PkmnFoundations.Web" Assembly="PkmnFoundations.Web" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHead" runat="server">
    <pf:HeaderColour ID="HeaderColour1" CssClass="test" runat="server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cpMain" runat="server">
<form id="theForm" runat="server">
<div>Enter name in hex to decode:
<asp:TextBox ID="txtName" Width="600px" runat="server" />
<asp:Button ID="btnSubmit" Text="Decode" OnClick="btnSubmit_Click" Width="60px" runat="server" />
</div>
<div>
<asp:Literal ID="litName" runat="server" />
</div>
</form>
</asp:Content>
