<%@ Page Language="C#" AutoEventWireup="true" CodeFile="choose_pictures_old.aspx.cs" Inherits="choose_pictures_old" %>
<%@ Reference Control="~/picturetinkerer.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Choose Pictures</title>
</head>
<body>
<script type="text/javascript" src="script/wz_dragdrop.js"></script>

    <form id="form1" runat="server">
     <table>
    <tr>
    <td>
    <img src="articledraw.aspx" style="z-index: 1;" />
    </td>
    <td valign="top">
     <asp:Panel ID="ptinkerers" runat="server">
     </asp:Panel>
    </td>
    </tr>
    </table>
    </form>

<script type="text/javascript">
//<!--
SET_DHTML(CURSOR_MOVE);

 <asp:Literal ID="ladddhtml" runat="server"></asp:Literal>

//-->
</script>

</body>
</html>
