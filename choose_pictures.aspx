<%@ Page Language="C#" MasterPageFile="~/htsp.master" AutoEventWireup="true" CodeFile="choose_pictures.aspx.cs" Inherits="choose_pictures" Title="Choose Pictures" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
  <script src="script/prototype-1.6.0.2.js" type="text/javascript"></script>
  <script src="script/scriptaculous.js?load=slider" type="text/javascript"></script>
  <!-- cedd has changed this file, careful putting newer versions on -->
  <script src="script/image.js" type="text/javascript"></script>

  <!-- required for IE, i hate you IE -->
  <meta http-equiv="imagetoolbar" content="no" />
<!--[if IE 6]>
<style type="text/css">
img {
   behavior: url("script/pngbehavior.htc");
}
</style>
<![endif]-->

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" Runat="Server">

<h1>Please choose the <%# pictureprompt %> (<%# pictureindex + 1 %> of <%# picturestotal %>)</h1>

<table>
<tr>
<td valign="top">
 <asp:PlaceHolder ID="particle" runat="server">
<p class="choosepictureszoomimageinstructions">
Click / Drag on the scale below to size the image.
</p>
<div style="width: <%# layoutwidth + 100 %>px;">
<div class="scalecontainer" style='width: <%# layoutwidth %>px; height: 1.5em;'>
<div id="scale-track" class="scaletrack" style='width: <%# layoutwidth %>px; height: 1.5em;'>
  <div id="scale-handle" class="scalehandle" style='width: 0.75em; height: 1.5em;'></div>
</div>
</div>
</div>
<p class="choosepicturespanimageinstructions" style="width: <%# layoutwidth + 100 %>px;">
Click and drag the image to position it.<br />
<span class="choosepicturesscaleexplanation">
<asp:Literal ID="lscale" runat="server" Text="<%# scaleexplanation %>"></asp:Literal>
</span></p>

<div class="choosepicturespanimagecontainer" style="position: relative; 
 overflow: hidden; width: <%# layoutwidth + 100 %>px; height:<%# layoutheight + 80 %>px;">
  <img src='<%# "pagebackgrounds/" + frontpagetemplateimage %>' id="frontpagetemplate" alt="Front Page Background" style="position: absolute; left:<%# -(layoutx - 50) %>px; top:<%# -(layouty - 40) %>px; width: <%# frontpagewidth %>px; height:<%# frontpageheight %>px; z-index: 0;" />
  <img src='<%# "articleoverlays/" + articleoverlayimage %>'  id="articleimage" alt="Article Text" style="position: absolute; left:<%# -(layoutx - 50) %>px; top:<%# -(layouty - 40) %>px; width: <%# frontpagewidth %>px; height:<%# frontpageheight %>px; z-index: 2; cursor: move; " />

 <div style="position: relative; overflow: hidden; left:50px; top:40px; width: <%# layoutwidth %>px; height:<%# layoutheight %>px;">
  <img src='<%# "uploadedpictures/" + imgsrc %>' id="uploadedimage" alt="Your Picture" style="position: absolute; left: <%# imgx %>px; top: <%# imgy %>px; width: <%# imgwidth %>px; height: <%# imgheight %>px; z-index: 1;" />


</div>
</div>
 </asp:PlaceHolder>

</td>

<td valign="top">
<div class="choosepicturesimagepaninstructions">

<asp:PlaceHolder ID="poptional" runat="server">
<h2>Optional Picture</h2>
<p>This picture is optional, if you don't wish to add a picture the article will expand to fill the space. Click the button below to skip this picture.</p>
 <asp:Button ID="bskip" runat="server" Text="Skip this picture" OnClick="bskip_Click" CausesValidation="false" />
</asp:PlaceHolder>
<h2>Step 1 - Upload a picture</h2>
<p>

</p>
<asp:FileUpload ID="fupicture" runat="server" /><br />
<asp:RequiredFieldValidator ID="rfpicture"
 runat="server" ErrorMessage="*" ValidationGroup="upload" Display="dynamic" ControlToValidate="fupicture">Please choose a picture to upload<br /></asp:RequiredFieldValidator>

<asp:Button ID="bupload" runat="server"
 Text="Upload" OnClick="bupload_Click" ValidationGroup="upload" />
 
<h2>Step 2 - Position and size the picture</h2>
<table>
<tr>
<td valign="top">
<p>
Click on the picture and drag to reposition. Click on the slider bar at the top to change the size.</p>
<p>
The preview on the right shows where the picture will go.
</p></td>
 <td valign="top">
 <div style="position: relative; width: <%# pl.previewwidth + 10 %>px; height: <%# pl.previewheight + 10 %>px; ">
 <div style="position: absolute; left:0px; top 0px; width: <%# pl.previewwidth %>px; height: <%# pl.previewheight %>px; overflow:hidden;">
 <img style="position: absolute; left: 0px; top: 0px;" class="choosepicturespreview" src='<%# "layoutpreviews/" + pl.previewimagefilename %>' id="preview" alt="Preview of picture location" />


 
 <div class="dim" style="position: absolute; left: 0px; top: 0px; width: <%# pl.previewwidth %>px; height: <%# previewy %>px; line-height: <%# previewy %>px;" ></div>

 <div class="dim" style="position: absolute; left: 0px; width: <%# previewx %>px; top: <%# previewy %>px; height: <%# pl.previewheight - previewy %>px; line-height: <%# pl.previewheight - previewy %>px;"></div>

 <div class="dim" style="position: absolute; left: <%# previewx + previewwidth %>px; width: <%# pl.previewwidth - previewx - previewwidth + 1 %>px; top: <%# previewy %>px; height: <%# pl.previewheight - previewy + 1%>px; line-height: <%# pl.previewheight - previewy + 1 %>px;"></div>

 <div class="dim" style="position: absolute; left: <%# previewx %>px; width: <%# previewwidth + 1%>px; top: <%# previewy + previewheight %>px; height: <%# pl.previewheight - previewheight - previewy + 1 %>px; line-height: <%# pl.previewheight - previewheight - previewy + 1%>px;"></div>


</div>
 <div class="choosepicturespreviewhighlight" style="position: absolute; left: <%# previewx - 10 %>px; top: <%# previewy - 10 %>px; width: <%# previewwidth %>px; height: <%# previewheight %>px; line-height: <%# previewheight %>px;" ></div>

</div>
</td>
</tr>
</table>
<h2>Step 3 - Save</h2>
 <asp:TextBox CssClass="displaynone" ID="tbpicavailable" runat="server" Text="<%# imgsrc %>"></asp:TextBox>
 <asp:RequiredFieldValidator ID="rfcrop" runat="server"  ValidationGroup="crop" ControlToValidate="tbpicavailable" Display="dynamic">Please upload and position a picure<br /></asp:RequiredFieldValidator>
 <asp:Button ID="bcrop" runat="server" Text="Save" onclick="bcrop_Click" CausesValidation="true" ValidationGroup="crop" />
</div>
</td>
</tr>
</table>

<input type="text" class="displaynone" name="imagex" id="imagex" value="<%# imgx %>" />
<input type="text" class="displaynone" name="imagey" id="imagey" value="<%# imgy %>" />
<input type="text" class="displaynone" name="imagesize" id="imagesize" value="<%# imgscale %>" />

 <script type="text/javascript" >
 <asp:PlaceHolder ID="pimgscript" runat="server">
  // <![CDATA[
  var image_edit = new ImageResize('articleimage', 'uploadedimage', <%# layoutwidth %>, <%# layoutheight %>, 2);
  // ]]>
</asp:PlaceHolder> 
</script>


</asp:Content>

