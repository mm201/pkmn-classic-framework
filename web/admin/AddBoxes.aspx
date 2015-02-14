<%@ Page Title="" Language="C#" MasterPageFile="~/masters/MasterPage.master" AutoEventWireup="true" CodeBehind="AddBoxes.aspx.cs" Inherits="PkmnFoundations.GTS.admin.AddBoxes" %>
<%@ Register TagPrefix="pf" Namespace="PkmnFoundations.Web" Assembly="PkmnFoundations.Web" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHead" runat="server">
    <pf:HeaderColour ID="HeaderColour1" CssClass="admin" runat="server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cpMain" runat="server">
<form id="theForm" runat="server">
    <div>
    <p>Upload a wiresharked box search response here to add it to the database:</p>
    <asp:FileUpload ID="fuBox" runat="server" />
    <asp:Button ID="btnSend" Text="Send" OnClick="btnSend_Click" runat="server" />
    </div>
    <asp:Literal ID="litMessage" runat="server" />
</form>
</asp:Content>
