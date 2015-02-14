<%@ Page Title="" Language="C#" MasterPageFile="~/masters/ThreeColumn.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="PkmnFoundations.GTS.Default" %>
<%@ Register TagPrefix="pf" Namespace="PkmnFoundations.Web" Assembly="PkmnFoundations.Web" %>
<%@ Register TagPrefix="pf" TagName="LabelTextBox" Src="~/controls/LabelTextBox.ascx" %>

<asp:Content ID="conHead" ContentPlaceHolderID="cpHead" runat="server">
    <pf:HeaderColour ID="HeaderColour1" CssClass="home" runat="server" />
</asp:Content>

<asp:Content ID="conLeft" ContentPlaceHolderID="cpLeft" runat="server">

    <pf:RequireCss Key="login" CssUrl="~/css/login.css" After="main" runat="server" />
    <div class="gtsSection gtsLogin">
    <form id="theForm" runat="server">

        <pf:LabelTextBox ID="txtUsername" Label="Username" runat="server" />
        <pf:LabelTextBox ID="txtPassword" Label="Password" TextMode="Password" runat="server" />
        <asp:Button ID="btnSubmit" CssClass="large" Text="Login" runat="server" />
    </form>

    <asp:HyperLink ID="hlRegister" Text="Create an account" NavigateUrl="#" runat="server" />
    </div>

    <div class="gtsSection gtsInfo">
        <p>Maintenance periods are <strong>Thursdays</strong> at
           <strong>UTC 0500</strong>.</p>
    </div>

    <div class="gtsSection gtsInfo">
        <p>Supported games: <strong>Diamond</strong>, <strong>Pearl</strong>,
           <strong>Platinum</strong>, <strong>Heart Gold</strong>,
           <strong>Soul Silver</strong>, <strong>Black</strong>,
           <strong>White</strong>, <strong>Black 2</strong>,
           <strong>White 2</strong>.</p>
    </div>
    <div class="gtsSection gtsInfo">
        <p>Supported features: <strong>Wi-Fi Club</strong><sup title="Support provided through AltWFC servers">altwfc</sup>,
           <strong>GTS</strong>, <strong>Battle Videos</strong>,
           <strong>Random Matchup</strong></p>
    </div>

    <div class="gtsSection gtsTwitter">
        <a class="twitter-timeline" data-dnt="true" 
            href="https://twitter.com/pcnstatus" data-widget-id="559566696667561986"
            height="400px">Tweets by @pcnstatus</a>
        <script>!function(d,s,id){var js,fjs=d.getElementsByTagName(s)[0],p=/^http:/.test(d.location)?'http':'https';if(!d.getElementById(id)){js=d.createElement(s);js.id=id;js.src=p+"://platform.twitter.com/widgets.js";fjs.parentNode.insertBefore(js,fjs);}}(document,"script","twitter-wjs");</script>
    </div>

</asp:Content>

<asp:Content ID="conRight" ContentPlaceHolderID="cpRight" runat="server">

</asp:Content>

<asp:Content ID="conMain" ContentPlaceHolderID="cpMain" runat="server">

    <h1>How to connect</h1>
    <ol>
        <li>Obtain an Action Replay code for your game. (complete list coming
            soon)<br />If you are using an emulator, you can patch your game with
            <a href="https://github.com/AdmiralCurtiss/WfcPatcher/releases">WfcPatcher</a>.</li>
        <li>If applicable, enable the code and launch your game.</li>
        <li>On your DS, set the DNS to <strong>104.131.93.87</strong>.</li>
        <li>Connect!</li>
    </ol>

    <p>Complete instructions can be found at
        <a href="https://github.com/polaris-/dwc_network_server_emulator/wiki#nintendo-dsdsi3ds2ds-configuration">the
            AltWFC Wiki</a>.</p>

    <hr />

    <h1>Update - August 29:</h1>
    <p>Wi-fi Battle Tower and Battle Subway are now available!
    </p>

    <h1>Update - July 11:</h1>
    <p>The battle video server is up and running! View any of the
        thousands of battle videos from Nintendo WFC 
        <asp:HyperLink ID="hlBattleVideos" NavigateUrl="~/BattleVideo.aspx" runat="server">
            which I was able to save</asp:HyperLink>
        and upload your own!
    </p>
    <p>Dressup photos, box uploads, and musical photos are also available.</p>


</asp:Content>
