<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeBehind="BattleVideo.aspx.cs" Inherits="PkmnFoundations.GTS.BattleVideo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHead" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cpMain" runat="server">
<h1>Battle videos</h1>
<p>Please enter your battle video's ID (eg. 12-34567-89012) into this form to have it backed up
onto Foundations GTS.</p>
<form id="theForm" runat="server">
<div>
<h2>Generation IV (Platinum, Heart Gold, Soul Silver)</h2>
<asp:TextBox ID="txtBattleVideo4" runat="server" />
<asp:Button ID="btnSend4" Text="Send" OnClick="btnSend4_Click" runat="server" />
</div>
<div class="stats">
<asp:Literal ID="litQueued4" runat="server" />
videos queued,
<asp:Literal ID="litTotal4" runat="server" />
videos saved in total.
</div>

<div>
<h2>Generation V (Black, White, Black 2, White 2)</h2>
<asp:TextBox ID="txtBattleVideo5" runat="server" />
<asp:Button ID="btnSend5" Text="Send" OnClick="btnSend5_Click" runat="server" />
<div class="stats">
<asp:Literal ID="litQueued5" runat="server" />
videos queued,
<asp:Literal ID="litTotal5" runat="server" />
videos saved in total.
</div>
</div>
<asp:Literal ID="litMessage" runat="server" />
</form>
</asp:Content>
