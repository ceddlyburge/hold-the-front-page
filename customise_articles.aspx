<%@ Page Language="C#" MasterPageFile="~/htsp.master" AutoEventWireup="true" CodeFile="customise_articles.aspx.cs" Inherits="customise_articles" Title="Customise Text" EnableViewState="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" Runat="Server">
<h1>Customise the <%# articleprompt %> (<%# articleindex + 1 %> of <%# articlestotal %>)</h1>
<p>All the customisable text from this article is shown below. Change any that you want and then click "Continue".</p>

<div id="customisescrollcontainer">
		<div id="customisescrollitemscontainer">
		<table cellspacing="0" cellpadding="3" width="100%">
 <asp:Repeater ID="rcustomise" runat="server">
 <ItemTemplate>
 <tr class="<%# getrowclass() %>"><td class="customisetextprompt">
<%# Eval("name") %>
</td><td>
  <input name='htsp<%# Eval("name") %>' type="text" value='<%# Eval("value") %>'  />
  
  </td></tr>
 </ItemTemplate>
 <SeparatorTemplate>
 <tr class="customisetextseparator"><td></td><td></td></tr>
 </SeparatorTemplate> 
</asp:Repeater>
</table>
</div>
</div>
<p>
 <asp:Button ID="bok" runat="server" Text="Continue" />
 </p>
</asp:Content>

