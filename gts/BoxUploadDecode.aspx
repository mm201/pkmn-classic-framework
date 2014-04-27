<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BoxUploadDecode.aspx.cs" Inherits="PokeFoundations.GTS.BoxUploadDecode" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    
    <p>Please upload your captured TCP request here:</p>
    <div>
    <asp:FileUpload ID="fuRequest" runat="server" />
    <asp:Button ID="btnUpload" Text="Upload" runat="server" />
    </div>
    <div>
    <asp:Literal ID="litDecrypted" runat="server" />
    </div>

    
    </form>
</body>
</html>
