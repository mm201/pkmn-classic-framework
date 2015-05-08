<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ForeignLookup.ascx.cs" Inherits="PkmnFoundations.Web.controls.ForeignLookup" %>
<%@ Register TagPrefix="pf" Namespace="PkmnFoundations.Web" Assembly="PkmnFoundations.Web" %>

<pf:RequireCss Key="form" CssUrl="~/css/form.css" After="main" runat="server" />
<pf:RequireScript Key="jquery" ScriptUrl="~/scripts/jquery-1.11.1.min.js" runat="server" />
<pf:RequireScript Key="form" ScriptUrl="~/scripts/form.js" runat="server" />

<div id="main" class="pfLookup" tabindex="0" runat="server">
    <div class="pfLookupOuter">
    <asp:TextBox ID="txtInput" class="input" autocomplete="off" runat="server" />
    </div>
    <div id="results" class="results" style="visibility: hidden;" runat="server">
        <div style="text-align: center; padding: 8px 0;">
        <pf:RetinaImage ID="imgLoading" ImageUrl="~/images/working.gif" AlternateSizes="2" Width="32" Height="32" style="margin: 0 auto;" runat="server" />
        </div>
    </div>
    <input type="hidden" ID="hdSelectedValue" runat="server" />
</div>
