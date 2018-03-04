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

public partial class choose_articles : System.Web.UI.Page
{
 protected bool alternatingrow = true;

 protected int farticleindex;
 public int articleindex
 {
  get { return farticleindex; }
 }

 protected int farticlestotal;
 public int articlestotal
 {
  get { return farticlestotal; }
 }

 protected string farticleprompt;
 public string articleprompt
 {
  get { return farticleprompt; }
 }

 protected string farticlename;
 public string articlename
 {
  get { return farticlename; }
 }

 public string getrowclass()
 {
  alternatingrow = !alternatingrow;
  if (alternatingrow == true) return "articlepreviewrow"; else return "articlepreviewalternatingrow";
 }

 protected sessionpagelayout spl;
 protected pagelayout pl = null;
 protected sessioncustomisations sc = null;

 protected void Page_Load(object sender, EventArgs e)
    {
     // get the existing customisations
     sc = (sessioncustomisations)Session["sessioncustomisation"];
     if (sc == null) Response.Redirect("~/default.aspx", true);
     if (sc.getsessioncustomisation("", "Nickname") == null) Response.Redirect("~/default.aspx", true);
     if (sc.getsessioncustomisation("", "First Name") == null) Response.Redirect("~/default.aspx", true);
     if (sc.getsessioncustomisation("", "Surname") == null) Response.Redirect("~/default.aspx", true); 

     // get the layout, if there is no layout specified then redirect back to the home page
     spl = (sessionpagelayout)Session["sessionpagelayout"];
     if (spl != null) pl = ceddio.loadpagelayout(spl.pagelayoutfilename);
     if (pl == null) Response.Redirect("~/default.aspx", true); 

     // get the article index to choose from the query string
     try
     {
      farticleindex = Convert.ToInt16(Request.QueryString["article"]);
     }
     catch
     {
      farticleindex = 0;
     }

     // get the total number of articles
     farticlestotal = pl.articles.Count;  

     // get the prompt for the current article
     farticleprompt = pl.articles[articleindex].prompt;

     farticlename = pl.articles[articleindex].name;

  if (Page.IsPostBack == true) return;

     DataBind();

     ceddutils.databindrepeater("spArticlesGetAll", rarticles, "htsp"); 
    }

 public string getheadline(string headline)
 {
  // replace #nickname#, #firstname# and #surname# in the string
  // have done replacement session stuff yet
  return headline.Replace("#nickname#", sc.getsessioncustomisation("","Nickname").value).Replace("#firstname#", sc.getsessioncustomisation("","First Name").value).Replace("#Surname#", sc.getsessioncustomisation("", "Surname").value).ToUpper();
 }

 public string getbody(string body)
 {
  // replace #nickname#, #firstname# and #surname# in the string
  return body.Replace("#nickname#", sc.getsessioncustomisation("", "Nickname").value).Replace("#firstname#", sc.getsessioncustomisation("","First Name").value).Replace("#surname#", sc.getsessioncustomisation("","Surname").value);
 }

 protected void choosearticleclick(object sender, EventArgs e)
 {
  // the command name is the id of the layout to choose.
  // command argument is the filename of layout to use
  sessionarticles sa;
  sa = (sessionarticles)Session["sessionarticles"];
  if (sa == null) sa = new sessionarticles();
  sa.addsessionarticle(articlename, (sender as LinkButton).CommandName);
  sa.setcookie(Response);
  Session["sessionarticles"] = sa;

  // if we are coming from the review page then jump straight to the customise page and ask it to jump straight back to the review afterwards
  if (Request.QueryString["return"] == "true") Response.Redirect("~/customise_articles.aspx?article=" + articleindex.ToString() + "?return=true");
  // otherwise if we have finished choosing articles go on to customise them
  else if (articleindex >= pl.articles.Count - 1) 
  Response.Redirect("~/customise_articles.aspx");
  // otherwise choose the next article
  else Response.Redirect("~/choose_articles.aspx?article=" + (articleindex + 1).ToString());
 }
}
