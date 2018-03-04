<%@ Page Language="C#" MasterPageFile="~/htsp.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" Title="Create a customised newspaper 'Front Page'" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" Runat="Server">

<h1>Hold the Front Page!</h1>

<table>
<tr>
<td valign="top" style="padding-right: 2em;">
<p>Welcome to Hold the Front Page, the website that allows you to easily create a customised Newspaper Front Page in 4 Easy steps, just like the one on the right.</p>
<p>Creating a Front Page is easy, all you have to do is choose the layout, add some pictures and tell us a few details. You will be able to see how the front page will look and be able to change it until you are happy.</p>
<h2>Get Started Now!</h2>
<p>Enter some details and then move on to Step 1 - Choosing a Layout.</p>
<asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="BulletList" />

<table>
<tr><td>
Nickname / First name</td><td>
 <asp:TextBox ID="tbnickname" runat="server"></asp:TextBox>
 <asp:RequiredFieldValidator ControlToValidate="tbnickname" ID="rvnickname" runat="server" ErrorMessage="*" display="dynamic" >Please enter the Nickname / First name</asp:RequiredFieldValidator> </td></tr>
<tr><td>
First name</td><td>
 <asp:TextBox ID="tbfirstname" runat="server"></asp:TextBox>
<asp:RequiredFieldValidator ControlToValidate="tbfirstname" ID="rvfirstname" runat="server" ErrorMessage="*" display="dynamic">Please enter the First name</asp:RequiredFieldValidator>  </td></tr>
<tr><td>
Surname</td><td>
 <asp:TextBox ID="tbsurname" runat="server"></asp:TextBox>
<asp:RequiredFieldValidator ControlToValidate="tbsurname" ID="rvsurname" runat="server" ErrorMessage="*" display="dynamic">Plesae enter the Surname</asp:RequiredFieldValidator>  </td></tr>
 </table>
 
 <asp:Button ID="bok" runat="server" Text='Choose a front page!' OnClick="bok_Click" /> 
</td>

<td valign="top">
<img src="images/examplefrontpage1.gif" alt="Example Customised Newspaper Front Page" title="Example Customised Newspaper Front Page" />
</td>


</tr>
</table>

</asp:Content>

