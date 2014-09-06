<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeBehind="AddDressup.aspx.cs" Inherits="PkmnFoundations.GTS.test.AddDressup" %>
<%@ Register TagPrefix="pf" Namespace="PkmnFoundations.GTS" Assembly="PkmnFoundations.GTS" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHead" runat="server">
    <pf:HeaderColour ID="HeaderColour1" CssClass="admin" runat="server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cpMain" runat="server">
<form id="theForm" runat="server">
    <div>
    <p>Upload a wiresharked Dressup search response here to add it to the database:</p>
    <asp:FileUpload ID="fuBox" runat="server" />
    <asp:Button ID="btnSend" Text="Send" OnClick="btnSend_Click" runat="server" />
    </div>
    <asp:Literal ID="litMessage" runat="server" />
</form>
</asp:Content>
