<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeBehind="AllPokemon.aspx.cs" Inherits="PkmnFoundations.GTS.AllPokemon" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHead" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cpMain" runat="server">

<h1>Generation IV</h1>
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

<h1>Generation V</h1>
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
