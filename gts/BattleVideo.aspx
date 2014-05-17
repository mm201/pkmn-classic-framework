<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeBehind="BattleVideo.aspx.cs" Inherits="PkmnFoundations.GTS.BattleVideo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHead" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cpMain" runat="server">
<h1>Battle videos</h1>
<p>Please enter your battle video's ID (eg. 12-34567-89012) into this form to have it backed up
onto Foundations GTS. <strong>Only Generation IV is supported for now!</strong>:</p>
<form id="theForm" runat="server">
<div><asp:TextBox ID="txtBattleVideo" runat="server" />
<asp:Button ID="btnSend" Text="Send" OnClick="btnSend_Click" runat="server" /></div>
<asp:Literal ID="litMessage" runat="server" />
</form>
</asp:Content>
