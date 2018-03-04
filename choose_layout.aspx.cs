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

public partial class choose_layout : System.Web.UI.Page
{
 protected bool alternatingrow = true;

 protected void Page_Load(object sender, EventArgs e)
    {
     if (Page.IsPostBack == true) return;

     //ceddutils.databindgridview("spLayoutsGetAll", gvlayouts, "htsp");
     ceddutils.databindrepeater("spLayoutsGetAll", rlayouts, "htsp");
    }

 public string getrowclass()
 {
  alternatingrow = !alternatingrow;
  if (alternatingrow == true) return "layoutpreviewrow"; else return "layoutpreviewalternatingrow"; 

 }
 
 protected void chooselayoutclick(object sender, EventArgs e)
 {
  // the command name is the id of the layout to choose.
  // command argument is the filename of layout to use
  sessionpagelayout spl;
  spl = (sessionpagelayout)Session["sessionpagelayout"];
  if (spl == null) spl = new sessionpagelayout();
  spl.pagelayoutfilename = (sender as LinkButton).CommandName;
  spl.setcookie(Response);
  Session["sessionpagelayout"] = spl;

  // go back to review page if we came from there, otherwise start choosing articles
  if (Request.QueryString["return"] == "true") Response.Redirect("~/review.aspx");
  else Response.Redirect("~/choose_articles.aspx");
 }
}
