using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class choose_details : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
     if (Page.IsPostBack == true) return;

     // the command name is the id of the layout to choose.
     // command argument is the filename of layout to use
     sessioncustomisations sc;
     sc = (sessioncustomisations)Session["sessioncustomisation"];
     if (sc == null) return;
     tbnickname.Text = sc.customise("", "Nickname", "");
     tbfirstname.Text = sc.customise("", "First Name", "");
     tbsurname.Text = sc.customise("", "Surname", "");
    }

 protected void bok_Click(object sender, EventArgs e)
 {
  // the command name is the id of the layout to choose.
  // command argument is the filename of layout to use
  sessioncustomisations sc;
  sc = (sessioncustomisations)Session["sessioncustomisation"];
  if (sc == null) sc = new sessioncustomisations();
  sc.addsessioncustomisation("", "Nickname", tbnickname.Text, false, true, false);
  sc.addsessioncustomisation("", "First Name", tbfirstname.Text, false, true, false);
  sc.addsessioncustomisation("", "Surname", tbsurname.Text, false, true, false);
  sc.setcookie(Response);
  Session["sessioncustomisation"] = sc;

  Response.Redirect("~/choose_layout.aspx");
 }
}
