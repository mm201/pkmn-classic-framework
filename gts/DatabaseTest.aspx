<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DatabaseTest.aspx.cs" Inherits="PokeFoundations.GTS.DatabaseTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    <asp:Label AssociatedControlID="txtRecord" Text="Paste record data here (hex):" runat="server" />
    <asp:TextBox ID="txtRecord" TextMode="MultiLine" runat="server" />
    <asp:Button ID="btnSend" OnClick="btnSend_Click" Text="Send" runat="server" />

    <br />

    <asp:Label AssociatedControlID="txtPid" Text="Enter your PID to retrieve raw record:" runat="server" />
    <asp:TextBox ID="txtPid" runat="server" />
    <asp:Button ID="btnReceive" OnClick="btnReceive_Click" Text="Receive" runat="server" />
    <asp:Literal ID="litRecord" runat="server" />

    </div>
    </form>
</body>
</html>
