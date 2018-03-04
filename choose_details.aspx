<%@ Page Language="C#" MasterPageFile="~/htsp.master" AutoEventWireup="true" CodeFile="choose_details.aspx.cs" Inherits="choose_details" Title="Choose Details" %>
<asp:Content ID="Content1" ContentPlaceHolderID="main" Runat="Server">
<h1>Choose Details</h1>
<p>Please enter some details about the start of this front page. These details are common to all of the stories. Each story also has more details which can be customised. This can be done after you choose the stories.</p>

<table>
<tr><td>
Nickname / First name</td><td>
 <asp:TextBox ID="tbnickname" runat="server"></asp:TextBox>
 <asp:RequiredFieldValidator ControlToValidate="tbnickname" ID="rvnickname" runat="server" ErrorMessage="*">Please enter the Nickname / First name</asp:RequiredFieldValidator> </td></tr>
<tr><td>
First name</td><td>
 <asp:TextBox ID="tbfirstname" runat="server"></asp:TextBox>
<asp:RequiredFieldValidator ControlToValidate="tbfirstname" ID="rvfirstname" runat="server" ErrorMessage="*">Please enter the First name</asp:RequiredFieldValidator>  </td></tr>
<tr><td>
Surname</td><td>
 <asp:TextBox ID="tbsurname" runat="server"></asp:TextBox>
<asp:RequiredFieldValidator ControlToValidate="tbsurname" ID="rvsurname" runat="server" ErrorMessage="*">Plesae enter the Surname</asp:RequiredFieldValidator>  </td></tr>
 </table>
 <p>
 <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="BulletList" />
 </p>
 
 <asp:Button ID="bok" runat="server" Text='Continue to "Choose a front page"' OnClick="bok_Click" /> 
</asp:Content>

