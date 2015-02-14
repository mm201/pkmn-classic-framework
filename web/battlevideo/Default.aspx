<%@ Page Title="" Language="C#" MasterPageFile="~/masters/MasterPage.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="PkmnFoundations.GTS.BattleVideo" %>
<%@ Register TagPrefix="pf" Namespace="PkmnFoundations.Web" Assembly="PkmnFoundations.Web" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHead" runat="server">
    <pf:HeaderColour ID="HeaderColour1" CssClass="bv" runat="server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cpMain" runat="server">
<h1>Battle videos</h1>
<p>Since the official Battle Video server has gone offline, you can no longer
queue your battle videos for retrieval. You can still type your battle video
ID into one of the below forms to check if I have it backed up. If not, you can
try <a href="https://www.pokecheck.org/?p=search&vid=">Pokécheck</a>. If it’s
not there, your video is gone. If it’s still on your game cartridge, you can
reupload it to fGTS under a new ID.</p>
<form id="theForm" runat="server">
<div>
<h2>Generation IV (Platinum, Heart Gold, Soul Silver)</h2>
<asp:TextBox ID="txtBattleVideo4" runat="server" />
<asp:Button ID="btnSend4" Text="Check" OnClick="btnSend4_Click" runat="server" />
<asp:Literal ID="litMessage4" runat="server" />
</div>
<div class="stats">
<asp:Literal ID="litTotal4" runat="server" />
videos saved in total.
</div>

<div>
<h2>Generation V (Black, White, Black 2, White 2)</h2>
<asp:TextBox ID="txtBattleVideo5" runat="server" />
<asp:Button ID="btnSend5" Text="Check" OnClick="btnSend5_Click" runat="server" />
<asp:Literal ID="litMessage5" runat="server" />
<div class="stats">
<asp:Literal ID="litTotal5" runat="server" />
videos saved in total.
</div>
</div>
</form>
</asp:Content>
