<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeBehind="Decode.aspx.cs" Inherits="PkmnFoundations.GTS.debug.Decode" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHead" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cpMain" runat="server">
<form id="theForm" runat="server">
    <div>
    <p>Enter data querystring to decode:</p>
    <asp:TextBox ID="txtData" Width="600" runat="server" />
    <asp:Button ID="btnDecode" Text="Decode" OnClick="btnDecode_Click" runat="server" />
    </div>
    <asp:Literal ID="litMessage" runat="server" />
    <asp:PlaceHolder ID="phDecoded" Visible="false" runat="server">
    <div>
    <p>Generation: <asp:Literal ID="litGeneration" runat="server" /></p>
    <div class="code">
    <asp:Literal ID="litDecoded" runat="server" />
    </div>
    </div>
    </asp:PlaceHolder>
</form>
</asp:Content>
