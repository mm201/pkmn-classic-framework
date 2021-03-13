<%@ Page Title="" Language="C#" MasterPageFile="~/masters/MasterPage.master" AutoEventWireup="true" CodeBehind="BoxUp.aspx.cs" Inherits="PkmnFoundations.GTS.debug.BoxUp" %>
<%@ Register TagPrefix="pf" Namespace="PkmnFoundations.Web" Assembly="PkmnFoundations.Web" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHead" runat="server">
    <pf:HeaderColour ID="HeaderColour1" CssClass="test" runat="server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cpMain" runat="server">
<form id="theForm" runat="server">
    <div>
    <p>Uplaod your Generation IV box upload, battle video, or dressup capture:</p>
        <div>
            <asp:Label Text="Format:" AssociatedControlID="rblFormat" runat="server" />
            <asp:RadioButtonList ID="rblFormat" RepeatDirection="Horizontal" runat="server">
                <asp:ListItem Text="Hex dump" Value="hd" Selected="True" />
                <asp:ListItem Text="C array" Value="ca" />
            </asp:RadioButtonList>
        </div>
        <div>
            <asp:FileUpload ID="fuBox" runat="server" />
            <asp:Button ID="btnSend" Text="Send" OnClick="btnSend_Click" runat="server" />
        </div>
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
