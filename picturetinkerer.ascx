<%@ Control Language="C#" AutoEventWireup="true" CodeFile="picturetinkerer.ascx.cs" Inherits="picturetinkerer" %>

<asp:FileUpload ID="fupicture" runat="server" style="z-index: 1;" /><br />
<asp:Button ID="bupload" runat="server"
 Text="Upload" OnClick="bupload_Click" style="z-index: 1;" />
 
 <script type="text/javascript">
<!--
<asp:Literal runat="server" ID="ldropfunc"></asp:Literal>
<asp:Literal ID="lzoomfunc" runat="server"></asp:Literal>
//-->
</script>


<asp:Panel ID="ppreview" runat="server" Height="150px" Width="150px" style="border: solid 1px black; background-repeat: no-repeat; z-index: 0;" >
 <asp:Panel ID="playoutboundary" runat="server" style="position: relative; border: groove 2px black; z-index: 1; overflow:hidden;">
 <asp:Image ID="ithumbnail" runat="server" style="position: relative; z-index: 0;"/>
 </asp:Panel>
</asp:Panel>

<asp:HiddenField ID="hfx" runat="server" />
<asp:HiddenField ID="hfy" runat="server" />
<asp:HiddenField ID="hfzoom" runat="server" />
<asp:Button ID="boffset" runat="server" Text="Offset" OnClick="boffset_Click" style="visibility: hidden;" /><br />
<asp:Image ID="itrack" ImageUrl="~/images/track.gif" runat="server" AlternateText="picture zoom track bar" width="100" Height="35" style="z-index: 1;" />
<asp:Image ID="ithumb" ImageUrl="~/images/thumb.gif" runat="server" AlternateText="picture zoom thumb" width="20" Height="35" style="z-index: 1;" />
<br />
<asp:Button ID="bzoom" runat="server" Text="Zoom" OnClick="bzoom_Click" style="visibility: hidden;" />
