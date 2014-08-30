<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="PkmnFoundations.GTS.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHead" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cpMain" runat="server">

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

<h1>What is Foundations GTS?</h1>
<p>Foundations GTS aims to re-create the same functionality of the original 
<abbr title="Global Trading Station">GTS</abbr> service found in the games, Pokémon Diamond,
Pearl, Platinum, Heart Gold, Soul Silver, Black, White, Black 2, and White 2, in an open source
project.</p>
<p>The main reason I chose to write my own GTS is because of the 
<a href="http://www.nintendo.com/whatsnew/detail/vyWpoM6CBIe6FjW8NIY7bvzOrgBURhzw">shutdown 
    of Nintendo Wi-fi Connection services</a> on May 20. To allow players to
continue trading, someone would need to make a replacement server, so I decided to help out.</p>

<p><strong style="color: #cc0000;">This is a WORK IN PROGRESS. I make no promises that your
Pokémon will be safe or that you will get them back. If you find any problems, please
report them in the
<asp:HyperLink ID="hlIssueTracker" NavigateUrl="https://github.com/mm201/pkmnFoundations/issues" runat="server">
issue tracker</asp:HyperLink> with complete replication steps, and Wireshark captures if possible.
Thanks.
</strong></p>

<h1>What is Pokémon Foundations?</h1>
<p>Pokémon Foundations is going to be a collection of utilities dealing with the logic, math,
and data related to the main series of Pokémon RPG videogames. Planned is a Pokédex, stat calculator,
damage calculator, and possibly more! (For now, it’s just a GTS.)</p>

<h1>How do I use this GTS?</h1>
<p>Connections are made through the <a href="https://github.com/polaris-/nintendo_dwc_emulator/wiki">
DWC Network Server Emulator (altWFC)</a>. Complete instructions on how to patch your game are found at the
above link.</p>
<p>To preview the latest features, you can use this alternate DNS:</p>
<p class="code">191.236.98.208</p>
<p>Please note that using this DNS server may interfere with the operation of some other
    titles besides Pokémon.</p>
</asp:Content>
