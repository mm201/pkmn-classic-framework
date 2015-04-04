<%@ Page Title="" Language="C#" MasterPageFile="~/masters/MasterPage.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="PkmnFoundations.GTS.AllPokemon" %>
<%@ Register TagPrefix="pf" Namespace="PkmnFoundations.Web" Assembly="PkmnFoundations.Web" %>
<%@ Register TagPrefix="pf" TagName="PokemonPicker" Src="~/controls/PokemonPicker.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHead" runat="server">
    <pf:HeaderColour ID="HeaderColour1" CssClass="gts" runat="server" />
    <pf:RequireScript Key="jquery" ScriptUrl="~/scripts/jquery-1.11.1.min.js" runat="server" />
    <pf:RequireScript Key="jquery-ui" ScriptUrl="~/scripts/jquery-ui.min.js" After="jquery" runat="server" />
    <pf:RequireCss Key="form" CssUrl="~/css/form.css" After="main" runat="server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cpMain" runat="server">

    <h1>Available Pokémon</h1>

    <script type="text/javascript">
        $(document).ready(function ()
        {
            $("#<%= txtLevelMin.ClientID %>").spinner();
            $("#<%= txtLevelMax.ClientID %>").spinner();
        });
    </script>

    <form id="theForm" runat="server">
    <div class="gtsBox pfColGroup" style="width: 640px;">
        <div class="pfColumn">
            <table class="pfFormGroup">
                <tr class="pfFormPair">
                    <th class="pfFormKey">Species</th>
                    <td class="pfFormValue">
                        <pf:PokemonPicker ID="ppSpecies" runat="server" />
                    </td>
                </tr>
                <tr class="pfFormPair">
                    <th class="pfFormKey">Level</th>
                    <td class="pfFormValue">
                        <asp:TextBox ID="txtLevelMin" min="1" max="100" Width="3em" Text="1" runat="server" />
                        to
                        <asp:TextBox ID="txtLevelMax" min="1" max="100" Width="3em" Text="100" runat="server" />
                    </td>
                </tr>
                <tr class="pfFormPair">
                    <th class="pfFormKey">Gender</th>
                    <td class="pfFormValue">
                        <asp:CheckBox ID="chkMale" Text="Male" Checked="true" runat="server" />
                        <asp:CheckBox ID="chkFemale" Text="Female" Checked="true" runat="server" />
                    </td>
                </tr>
            </table>
        </div>
        <div class="pfColumn">
            <table class="pfFormGroup">
                <tr class="pfFormPair">
                    <th class="pfFormKey">Version</th>
                    <td class="pfFormValue">
                        <asp:RadioButton ID="rbGen4" GroupName="grpGeneration" Checked="true" runat="server" />
                        <asp:Label ID="lblGen4" AssociatedControlID="rbGen4" runat="server">
                            <asp:Image ImageUrl="~/images/ver-icon/10.png" CssClass="sprite" 
                                Width="32" Height="32" AlternateText="Diamond" runat="server" />
                            <asp:Image ImageUrl="~/images/ver-icon/11.png" CssClass="sprite" 
                                Width="32" Height="32" AlternateText="Pearl" runat="server" />
                            <asp:Image ImageUrl="~/images/ver-icon/12.png" CssClass="sprite" 
                                Width="32" Height="32" AlternateText="Platinum" runat="server" />
                            <asp:Image ImageUrl="~/images/ver-icon/7.png" CssClass="sprite" 
                                Width="32" Height="32" AlternateText="Heart Gold" runat="server" />
                            <asp:Image ImageUrl="~/images/ver-icon/8.png" CssClass="sprite" 
                                Width="32" Height="32" AlternateText="Soul Silver" runat="server" />
                        </asp:Label>
                        <br />

                        <asp:RadioButton ID="rbGen5" GroupName="grpGeneration" runat="server" />
                        <asp:Label ID="lblGen5" AssociatedControlID="rbGen5" runat="server">
                            <asp:Image ImageUrl="~/images/ver-icon/21.png" CssClass="sprite" 
                                Width="32" Height="32" AlternateText="Black" runat="server" />
                            <asp:Image ImageUrl="~/images/ver-icon/20.png" CssClass="sprite" 
                                Width="32" Height="32" AlternateText="White" runat="server" />
                            <asp:Image ImageUrl="~/images/ver-icon/23.png" CssClass="sprite" 
                                Width="32" Height="32" AlternateText="Black 2" runat="server" />
                            <asp:Image ImageUrl="~/images/ver-icon/22.png" CssClass="sprite" 
                                Width="32" Height="32" AlternateText="White 2" runat="server" />
                        </asp:Label>
                    </td>
                </tr>
            </table>
        </div>
        <div class="pfFormSubmit">
        <asp:Button ID="btnSearch" Width="8em" Text="Search" runat="server" />
        </div>
    </div>
    </form>

<asp:Repeater ID="rptPokemon4" runat="server">

<HeaderTemplate>
<table>
<thead>
<tr>
<td>Offer</td>
<td>Wanted</td>
<td>Trainer</td>
</tr>
</thead>
</HeaderTemplate>



<ItemTemplate>
<tr>
<td><%# CreateOffer4(Container.DataItem) %></td>
<td><%# CreateWanted4(Container.DataItem) %></td>
<td><%# CreateTrainer4(Container.DataItem) %></td>
</tr>
</ItemTemplate>


<FooterTemplate>
</table>
</FooterTemplate>

</asp:Repeater>


<asp:Repeater ID="rptPokemon5" runat="server">

<HeaderTemplate>
<table>
<thead>
<tr>
<td>Offer</td>
<td>Wanted</td>
<td>Trainer</td>
</tr>
</thead>
</HeaderTemplate>



<ItemTemplate>
<tr>
<td><%# CreateOffer5(Container.DataItem) %></td>
<td><%# CreateWanted5(Container.DataItem) %></td>
<td><%# CreateTrainer5(Container.DataItem) %></td>
</tr>
</ItemTemplate>


<FooterTemplate>
</table>
</FooterTemplate>

</asp:Repeater>

</asp:Content>
