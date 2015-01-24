<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LabelTextBox.ascx.cs" Inherits="PkmnFoundations.GTS.controls.LabelTextBox" %>
<%@ Register TagPrefix="pf" Namespace="PkmnFoundations.Web" Assembly="PkmnFoundations.GTS" %>

<pf:RequireCss Key="form" CssUrl="~/css/form.css" runat="server" />
<pf:RequireScript Key="jquery" ScriptUrl="~/script/jquery-1.11.1.min.js" runat="server" />
<pf:RequireScript Key="form" ScriptUrl="~/script/form.js" runat="server" />

<div class="pfLabelTextBox">
<asp:Label ID="theLabel" AssociatedControlID="theTextBox" runat="server" />
<asp:TextBox ID="theTextBox" runat="server" />
</div>
