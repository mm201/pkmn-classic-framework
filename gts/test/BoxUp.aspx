<%@ Page Title="" Language="C#" MasterPageFile="~/masters/MasterPage.master" AutoEventWireup="true" CodeBehind="BoxUp.aspx.cs" Inherits="PkmnFoundations.GTS.debug.BoxUp" %>
<%@ Register TagPrefix="pf" Namespace="PkmnFoundations.Web" Assembly="PkmnFoundations.GTS" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHead" runat="server">
    <pf:HeaderColour ID="HeaderColour1" CssClass="test" runat="server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cpMain" runat="server">
<form id="theForm" runat="server">
    <div>
    <p>Uplaod your Generation IV box upload, battle video, or dressup capture:</p>
    <asp:FileUpload ID="fuBox" runat="server" />
    <asp:Button ID="btnSend" Text="Send" OnClick="btnSend_Click" runat="server" />
    </div>
    <asp:Literal ID="litMessage" runat="server" />
    <asp:PlaceHolder ID="phDecoded" Visible="false" runat="server">
    <div>
    <div class="code">
    <asp:Literal ID="litDecoded" runat="server" />
    </div>
    </div>
    </asp:PlaceHolder>
</form>
</asp:Content>
