<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
</head>
<body>
	<form id="form1" runat="server">
	<div>
	<asp:Literal ID="results" runat="server"></asp:Literal>
		
	</div>
		<div><asp:Button ID="action" runat="server" Text="Poll" OnClick="action_Click" /></div>
	</form>
</body>
</html>
