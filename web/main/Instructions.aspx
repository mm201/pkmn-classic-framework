<%@ Page Title="Getting started - Pokémon Classic Network" Language="C#" MasterPageFile="~/masters/MasterPage.master" AutoEventWireup="true" CodeBehind="Instructions.aspx.cs" Inherits="PkmnFoundations.Main.Instructions" %>
<%@ Register TagPrefix="pf" Namespace="PkmnFoundations.Web" Assembly="PkmnFoundations.Web" %>
<%@ Register TagPrefix="pf" TagName="DnsAddress" Src="~/controls/DnsAddress.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHead" runat="server">
    <pf:RequireScript Key="jquery" ScriptUrl="~/scripts/jquery-1.11.1.min.js" runat="server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cpMain" runat="server">

    <h1>How to Connect</h1>
    <p>Instructions provided are for Action Replay devices. If you’re not using
        an Action Replay, please visit the
        <asp:HyperLink ID="hlAltWfc" NavigateUrl="https://github.com/polaris-/dwc_network_server_emulator/wiki#nintendo-dsdsi3ds2ds-configuration" runat="server">
            AltWFC wiki
        </asp:HyperLink>
        for a list of other supported devices and connection methods.
    </p>
    <p>
        <strong>You must use a special device to connect to the server.</strong> You will
        receive a connection error if you try to connect with just a DS and game card.
    </p>

    <h2>Requirements</h2>
    <ul>
        <li>Nintendo DS system</li>
        <li><a href="http://www.amazon.com/gp/product/B00A1AQJAO/">Action Replay</a> for DS or DSi</li>
        <li>One of the <span class="pfToolTip" title="Diamond, Pearl, Platinum, Heart Gold, Soul Silver, Black, White, Black 2, White 2">supported
            games</span></li>
        <li>Wi-Fi access point (unprotected or WEP only), or Nintendo Wi-Fi
            USB Connector
        </li>
    </ul>

    <h2 id="ar">Action Replay codes</h2>
    <p>Please select your game and region to continue:</p>

    <div class="phChoiceGroup pfChoiceGroupVersions">
        <div class="pfChoice">
            <div class="pfChoiceIcon">
                <asp:Image ID="imgDiamond" ImageUrl="~/images/ver-icon/10.png" CssClass="sprite" AlternateText="" Width="32" Height="32" runat="server" />
            </div>
            <div class="pfChoiceLabel"><div class="inner">
                Diamond
            </div></div>
        </div>
        <div class="pfChoice">
            <div class="pfChoiceIcon">
                <asp:Image ID="imgPearl" ImageUrl="~/images/ver-icon/11.png" CssClass="sprite" AlternateText="" Width="32" Height="32" runat="server" />
            </div>
            <div class="pfChoiceLabel"><div class="inner">
                Pearl
            </div></div>
        </div>
        <div class="pfChoice">
            <div class="pfChoiceIcon">
                <asp:Image ID="imgPlatinum" ImageUrl="~/images/ver-icon/12.png" CssClass="sprite" AlternateText="" Width="32" Height="32" runat="server" />
            </div>
            <div class="pfChoiceLabel"><div class="inner">
                Platinum
            </div></div>
        </div>
        <div class="pfChoice">
            <div class="pfChoiceIcon">
                <asp:Image ID="imgHeartGold" ImageUrl="~/images/ver-icon/7.png" CssClass="sprite" AlternateText="" Width="32" Height="32" runat="server" />
            </div>
            <div class="pfChoiceLabel"><div class="inner">
                Heart Gold
            </div></div>
        </div>
        <div class="pfChoice">
            <div class="pfChoiceIcon">
                <asp:Image ID="imgSoulSilver" ImageUrl="~/images/ver-icon/8.png" CssClass="sprite" AlternateText="" Width="32" Height="32" runat="server" />
            </div>
            <div class="pfChoiceLabel"><div class="inner">
                Soul Silver
            </div></div>
        </div>
        <div class="clear"></div>
        <div class="pfChoice">
            <div class="pfChoiceIcon">
                <asp:Image ID="imgBlack" ImageUrl="~/images/ver-icon/21.png" CssClass="sprite" AlternateText="" Width="32" Height="32" runat="server" />
            </div>
            <div class="pfChoiceLabel"><div class="inner">
                Black
            </div></div>
        </div>
        <div class="pfChoice">
            <div class="pfChoiceIcon">
                <asp:Image ID="imgWhite" ImageUrl="~/images/ver-icon/20.png" CssClass="sprite" AlternateText="" Width="32" Height="32" runat="server" />
            </div>
            <div class="pfChoiceLabel"><div class="inner">
                White
            </div></div>
        </div>
        <div class="pfChoice">
            <div class="pfChoiceIcon">
                <asp:Image ID="imgBlack2" ImageUrl="~/images/ver-icon/23.png" CssClass="sprite" AlternateText="" Width="32" Height="32" runat="server" />
            </div>
            <div class="pfChoiceLabel"><div class="inner">
                Black 2
            </div></div>
        </div>
        <div class="pfChoice">
            <div class="pfChoiceIcon">
                <asp:Image ID="imgWhite2" ImageUrl="~/images/ver-icon/22.png" CssClass="sprite" AlternateText="" Width="32" Height="32" runat="server" />
            </div>
            <div class="pfChoiceLabel"><div class="inner">
                White 2
            </div></div>
        </div>

    </div>
    <div class="clear"></div>

        <h2>Instructions</h2>

    <div class="pfRight">
        <asp:Image ImageUrl="~/images/help/shot-wifi-settings.png" Width="256" Height="384" CssClass="sprite" runat="server" />
    </div>

    <ul class="pfSplitList">
        <li>
            Install the appropriate <a href="#ar">Action Replay code</a> for
            your game.
        </li>
        <li>
            Launch the game with your AR and select the appropriate code.
        </li>
        <li>
            Under the main menu, select <strong>Nintendo Wi-Fi Connection
            Settings</strong>.
        </li>
        <li>Go to <strong>Options</strong> and select <strong>Erase
            Nintendo WFC Configuration</strong>.</li>
        <li>
            Go back one level and into <strong>Nintendo Wi-Fi Connection
            Settings</strong>.
        </li>
        <li>Pick a connection and select <strong>Search for an Access
            Point</strong>.</li>
        <li>Choose the correct access point and, if applicable, enter your WEP
            password.</li>
        <li>Select the newly created connection to access settings.</li>
        <li>
            Set <code>Auto-obtain DNS</code> to <code>No</code> and change the
            primary DNS to
            <code><pf:DnsAddress runat="server" /></code>.
            Leave the secondary DNS blank.
        </li>
        <li>Exit the Wi-Fi settings and enjoy your game!</li>
    </ul>

    <p>If you see error 52200, turn your DS off and on and try again. Soft
        resetting your game (L+R+Start+Select) will render the Wi-Fi enabler
        code ineffective. Turn your DS off and on again to reset.
    </p>

    <div class="clear"></div>

</asp:Content>
