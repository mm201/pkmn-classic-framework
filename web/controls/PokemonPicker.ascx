<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PokemonPicker.ascx.cs" Inherits="PkmnFoundations.Web.controls.PokemonPicker" %>
<%@ Register TagPrefix="pf" TagName="ForeignLookup" Src="~/controls/ForeignLookup.ascx" %>

<pf:ForeignLookup ID="theLookup" MaxRows="8" SourceUrl="~/controls/PokemonSource.ashx" runat="server" />
