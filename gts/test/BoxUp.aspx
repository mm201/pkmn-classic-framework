<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeBehind="BoxUp.aspx.cs" Inherits="PkmnFoundations.GTS.debug.BoxUp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHead" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cpMain" runat="server">
<form id="theForm" runat="server">
    <div>
    <p>Uplaod your Generation IV box upload, battle video, and dressup capture:</p>
    <asp:FileUpload ID="fuBox" runat="server" />
    &nbsp;Pad offset:
    <asp:TextBox ID="txtOffset" runat="server" />
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
