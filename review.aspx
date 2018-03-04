<%@ Page Language="C#" MasterPageFile="~/htsp.master" AutoEventWireup="true" CodeFile="review.aspx.cs" Inherits="review" Title="Your Customised Front Page!" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
  <script src="script/prototype-1.6.0.2.js" type="text/javascript"></script>
  <script src="script/scriptaculous.js?load=slider" type="text/javascript"></script>
  <!-- cedd has changed this file, careful putting newer versions on -->
  <script src="script/image.js" type="text/javascript"></script>

  <!-- required for IE, i hate you IE -->
  <meta http-equiv="imagetoolbar" content="no" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" Runat="Server">

<h1>Review Your Front Page!</h1>

<table><tr>

<td valign="top" style="padding-right: 2em; width:420px">
<p>
Your customised Front Page is now complete! It is shown on the right and can be zoomed in to see further detail. If you want to change anything then use the links below.
</p>
 
 <h2>Step 1</h2>   
 <p><a href="choose_layout.aspx">Choose Layout</a></p>
 <h2>Step 2 - Choose Articles</h2>
 <asp:PlaceHolder ID="phchoosearticles" runat="server"></asp:PlaceHolder>
 <h2>Step 3 - Customise Articles</h2>
 <asp:PlaceHolder ID="phcustomisearticles" runat="server"></asp:PlaceHolder>
 <h2>Step 4 - Customise Pictures</h2>
 <asp:PlaceHolder ID="phchoosepictures" runat="server"></asp:PlaceHolder>
</td>

<td valign="top">
<div style="width: <%# reviewwidth %>px;">
<div class="scalecontainer" style='width: <%# reviewwidth %>px; height: 1.5em;'>
<div id="scale-track" class="scaletrack" style='width: <%# reviewwidth %>px; height: 1.5em;'>
  <div id="scale-handle" class="scalehandle" style='width: 0.75em; height: 1.5em;'></div>
</div>
</div>
</div>

<div class="choosepicturespanimagecontainer" style="position: relative; 
 overflow: hidden; width: <%# reviewwidth %>px; height:480px; cursor:move;">
  <img src='transparentpixel.gif' style="position: absolute; left: 0px; top: 0px; height:480px; width: <%# reviewwidth %>px;" id="foregroundimage"/>

 <div style="position: relative; overflow: hidden; width: <%# reviewwidth %>px; height:480px;">
  <img src='articledraw.aspx' style="position: absolute; left: 0px; top: 0px; height:480px; width: <%# reviewwidth %>px;" id="articleimage" alt="Your Customised Front Page" />

</div>
</div>
</td></tr></table>

 <input type="text" class="displaynone" name="imagex" id="imagex" value="0" />
<input type="text" class="displaynone" name="imagey" id="imagey" value="0" />
<input type="text" class="displaynone" name="imagesize" id="imagesize" value="1" />

<script type="text/javascript" >
  // <![CDATA[
  var image_edit = new ImageResize('foregroundimage', 'articleimage', <%# reviewwidth %>, 480, 3);
  // ]]>
</script>



<asp:PlaceHolder ID="phdebug" runat="server" Visible="false">    
   <asp:Literal ID="Literal1" runat="server"></asp:Literal>
        <br /><br />
        <asp:Literal ID="Literal2" runat="server"></asp:Literal>
        <br /><br />
        <asp:Literal ID="Literal3" runat="server"></asp:Literal>
        <br /><br />
        <asp:Literal ID="Literal4" runat="server"></asp:Literal>
        <br /><br />
        <asp:Literal ID="Literal5" runat="server"></asp:Literal>
        <br /><br />
        <asp:Literal ID="Literal6" runat="server"></asp:Literal>
        <asp:Literal ID="Literal7" runat="server"></asp:Literal>
</asp:PlaceHolder>    
</asp:Content>

