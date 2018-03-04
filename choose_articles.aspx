<%@ Page Language="C#" MasterPageFile="~/htsp.master" AutoEventWireup="true" CodeFile="choose_articles.aspx.cs" Inherits="choose_articles" Title="Choose Articles" %>
<asp:Content ID="Content2" ContentPlaceHolderID="head" Runat="Server">
    <link media="screen" href="serialscroller.css" type="text/css" rel="StyleSheet"/>
	<script type="text/javascript" src="script/jquery-1.2.6.js"></script>
	<script type="text/javascript" src="script/jquery.scrollTo.js"></script>
	<script type="text/javascript" src="script/jquery.serialScroll.js"></script>
	
	
	</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="main" Runat="Server">
<h1>Please choose the <%# articleprompt %> (<%# articleindex + 1 %> of <%# articlestotal %>)</h1>
<p>Words that are <span class="emphasisecustomisabletext">emphasised</span> can be customised later on. Stories are automatically edited later to fit on the page. The text of the edited version might not be exactly the same as the preview shown here, but will be pretty similar. Only the first part of the story is shown.</p>

     
<div id="articlepreviewscrollcontainer">
<table><tr><td>
		<img class="prev" src="images/prev.gif" alt="prev" />
</td><td>
		<div id="articlepreviewscrollitemscontainer">
			<ul class="scrolling">
 <asp:Repeater ID="rarticles" runat="server">
  <ItemTemplate>
  <li class="scrolling">
  
 <div class='<%# getrowclass() %>'>
<table cellspacing="0" >
 
 <tr class='articleheadline'>

 <td valign="top">
<asp:LinkButton ID="lbchoose" runat="server" CommandArgument='<%# Eval("id") %>' CommandName='<%# Eval("articlefilename") %>' OnClick="choosearticleclick" ToolTip="Click to choose this article" CssClass="articlepreviewheadline"><%# getheadline(Eval("headline").ToString())%></asp:LinkButton>
</td><tr>
<tr><td>
 <p class="articlepreviewbody">
 <%# getbody(Eval("body").ToString())%>
 </p>
 </td>
 <td valign="top">
  </td>
 </tr>
 </table>
 </ItemTemplate> 
 </asp:Repeater>
			</ul>
<div class="articlepreviewspacer"></div>
		</div>
</td><td>
		<img class="next" src="images/next.gif" alt="next" />
		</td></tr> </table>
	</div>	
     



<script> 
$('#articlepreviewscrollcontainer').serialScroll({
		target:'#articlepreviewscrollitemscontainer',
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

