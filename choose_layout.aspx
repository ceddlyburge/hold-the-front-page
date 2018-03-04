<%@ Page Language="C#" MasterPageFile="~/htsp.master" AutoEventWireup="true" CodeFile="choose_layout.aspx.cs" Inherits="choose_layout" Title="Choose Layout" %>
<asp:Content ID="Content2" ContentPlaceHolderID="head" Runat="Server">
    <link media="screen" href="serialscroller.css" type="text/css" rel="StyleSheet"/>
	<script type="text/javascript" src="script/jquery-1.2.6.js"></script>
	<script type="text/javascript" src="script/jquery.scrollTo.js"></script>
	<script type="text/javascript" src="script/jquery.serialScroll.js"></script>
	
	
	</asp:Content>
	<asp:Content ID="Content1" ContentPlaceHolderID="main" Runat="Server">
<h1>Please choose a template</h1>

<div id="layoutpreviewscrollcontainer">
<table><tr><td>
		<img class="prev" src="images/prev.gif" alt="prev" />
</td><td>
		<div id="layoutpreviewscrollitemscontainer">
			<ul class="scrolling">
 <asp:Repeater ID="rlayouts" runat="server">
  <ItemTemplate>
  <li class="scrolling">
  
 <div class='<%# getrowclass() %>'>
 <table><tr><td>
 <div class="layoutpreviewthumbnail">
<asp:Image ID="ilayouthumbnail" runat="server" AlternateText="Thumbnail preview of this layout" ImageUrl='<%# Eval("previewfilename") %>' CssClass="layoutpreviewthumbnail" style="" /> 
</div>
</td>
<td>
 <div class="layoutpreviewbody">
 <p class="layoutpreviewtitle"><%# Eval("Title") %></p>
 <p class="layoutpreviewbody"><%# Eval("Body") %></p>
 <p class="layoutpreviewreccomendedpicturesize">Recommended picture size: <%# Eval("mainpicturesize") %></p>
 <p class="layoutpreviewchoose">
  <asp:LinkButton ID="lbchoose" runat="server" CommandArgument='<%# Eval("id") %>' CommandName='<%# Eval("layoutfilename") %>' OnClick="chooselayoutclick"  >Choose this layout</asp:LinkButton>
  </p>
  </div>
  </td>
  <td>

  <div class="subjectpositionpicture">
 <asp:Image ID="isubjectposition" runat="server" AlternateText='<%# Eval("subjectpositionalternatetext") %>' ImageUrl='<%# Eval("subjectpositionpicture") %>' CssClass="subjectpositionpicture" style="" />
 </div>
</td></tr></table>
 </li>
 </ItemTemplate> 
 </asp:Repeater>
			</ul>
<div class="layoutpreviewspacer"></div>
		</div>
</td><td>
		<img class="next" src="images/next.gif" alt="next" />
		</td></tr> </table>
	</div>	
 
<script> 
$('#layoutpreviewscrollcontainer').serialScroll({
		target:'#layoutpreviewscrollitemscontainer',
		items:'li', // Selector to the items ( relative to the matched elements, '#sections' in this case )
		prev:'img.prev',// Selector to the 'prev' button (absolute!, meaning it's relative to the document)
		next:'img.next',// Selector to the 'next' button (absolute too)
		axis:'y',// The default is 'y' scroll on both ways
		duration:400,// Length of the animation (if you scroll 2 axes and use queue, then each axis take half this time)
		force:true, // Force a scroll to the element specified by 'start' (some browsers don't reset on refreshes)
		lock:false, // Ignore events if already animating (true by default)		
		cycle:false// Cycle endlessly ( constant velocity, true is the default )
		
	
	});
	
	</script>
</asp:Content>

