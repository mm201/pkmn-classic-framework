<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeBehind="VideoId.aspx.cs" Inherits="PkmnFoundations.GTS.test.VideoId" %>
<%@ Register TagPrefix="pf" Namespace="PkmnFoundations.GTS" Assembly="PkmnFoundations.GTS" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHead" runat="server">
    <pf:HeaderColour ID="HeaderColour1" CssClass="test" runat="server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cpMain" runat="server">
<br />
<form id="theForm" runat="server">
Battle video ID:
<asp:TextBox ID="txtBattleVideo" runat="server" />
<br />
<asp:Button ID="btnToSerial" Text="↓" OnClick="btnToSerial_Click" runat="server" />
<asp:Button ID="btnToVideo" Text="↑" OnClick="btnToVideo_Click" runat="server" />
<br />
Serial number:
<asp:TextBox ID="txtSerial" runat="server" />
</form>
</asp:Content>
