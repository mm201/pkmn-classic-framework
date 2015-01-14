<%@ Page Title="" Language="C#" MasterPageFile="~/masters/MasterPage.master" AutoEventWireup="true" CodeBehind="Sessions.aspx.cs" Inherits="PkmnFoundations.GTS.admin.Sessions" %>
<%@ Register TagPrefix="pf" Namespace="PkmnFoundations.Web" Assembly="PkmnFoundations.GTS" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHead" runat="server">
    <pf:HeaderColour ID="HeaderColour1" CssClass="admin" runat="server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cpMain" runat="server">
<asp:Literal ID="litDebug" runat="server" />
</asp:Content>
