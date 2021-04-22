<%@ Page Title="Poké Classic Network" Language="C#" MasterPageFile="~/masters/ThreeColumn.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="PkmnFoundations.GTS.Default" %>
<%@ Register TagPrefix="pf" Namespace="PkmnFoundations.Web" Assembly="PkmnFoundations.Web" %>
<%@ Register TagPrefix="pf" TagName="LabelTextBox" Src="~/controls/LabelTextBox.ascx" %>
<%@ Register TagPrefix="pf" TagName="DnsAddress" Src="~/controls/DnsAddress.ascx" %>

<asp:Content ID="conHead" ContentPlaceHolderID="cpHead" runat="server">
    <pf:HeaderColour ID="HeaderColour1" CssClass="home" runat="server" />
</asp:Content>

<asp:Content ID="conHeadingArea" ContentPlaceHolderID="cpHeadingArea" runat="server">
</asp:Content>

<asp:Content ID="conLeft" ContentPlaceHolderID="cpLeft" runat="server">
    &nbsp;
    <pf:RequireCss Key="login" CssUrl="~/css/login.css" After="main" runat="server" />
    <asp:PlaceHolder Visible="false" runat="server">
    <div class="gtsSection gtsLogin">
    <form id="theForm" runat="server">
        <pf:LabelTextBox ID="txtUsername" Label="Username" runat="server" />
        <pf:LabelTextBox ID="txtPassword" Label="Password" TextMode="Password" runat="server" />
        <asp:Button ID="btnSubmit" CssClass="large" Text="Login" runat="server" />
    </form>

    <asp:HyperLink ID="hlRegister" Text="Create an account" NavigateUrl="#" runat="server" />
    </div>
    </asp:PlaceHolder>

</asp:Content>

<asp:Content ID="conRight" ContentPlaceHolderID="cpRight" runat="server">

    <div class="gtsSection gtsTwitter">
        <a class="twitter-timeline" data-width="236" data-height="600" href="https://twitter.com/pcnstatus?ref_src=twsrc%5Etfw">Tweets by pcnstatus</a> <script async src="https://platform.twitter.com/widgets.js" charset="utf-8"></script> 
        <script>!function (d, s, id) { var js, fjs = d.getElementsByTagName(s)[0], p = /^http:/.test(d.location) ? 'http' : 'https'; if (!d.getElementById(id)) { js = d.createElement(s); js.id = id; js.src = p + "://platform.twitter.com/widgets.js"; fjs.parentNode.insertBefore(js, fjs); } }(document, "script", "twitter-wjs");</script>
    </div>

</asp:Content>

<asp:Content ID="conMain" ContentPlaceHolderID="cpMain" runat="server">

    <h2>About</h2>
    <p>Poké Classic Network provides GTS, battle videos, and other related
    services for Generation IV and V games. It runs in combination with Wiimmfi
    and Kaeru WFC which provide general purpose NWFC emulation and SSL
    offloading, respectively. (For reasons beyond my control, I cannot
    guarantee compatibility with AltWFC servers at this time.)</p>

    <h2>Getting started</h2>
    <div class="gtsFloatRight">
        
    </div>
    <p>All you need to do is change the <strong>primary</strong> DNS to
        <code><pf:DnsAddress runat="server" /></code> on your DS. Make sure the
        <strong>secondary</strong> DNS is either <code>0.0.0.0</code> or
        otherwise the same as the primary.
        <em>Failure to follow these instructions may lead to connection
            instability and communication errors.</em></p>
        <p></p>
</asp:Content>
