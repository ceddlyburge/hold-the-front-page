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

public partial class customise_articles : System.Web.UI.Page
{
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

 protected bool alternatingrow = true;
 public string getrowclass()
 {
  alternatingrow = !alternatingrow;
  if (alternatingrow == true) return "customisetextrow"; else return "customisetextalternatingrow";
 }

 protected sessionpagelayout spl;
 protected pagelayout pl = null;
 protected sessioncustomisations scs;
 protected sessionarticles sas;
 protected sessionarticle sa;
 
 protected void Page_Load(object sender, EventArgs e)
    {
     // get the article index to choose from the query string
     try
     {
      farticleindex = Convert.ToInt16(Request.QueryString["article"]);
     }
     catch
     {
      farticleindex = 0;
     }

  // get the page layout
     spl = (sessionpagelayout)Session["sessionpagelayout"];
     if (spl == null) Response.Redirect("~/default.aspx", true);
     pl = ceddio.loadpagelayout(spl.pagelayoutfilename);
     // if there is no layout specified the redirect back to the home page
     if (pl == null) Response.Redirect("~/default.aspx", true);

     // get the customisation
     scs = (sessioncustomisations)Session["sessioncustomisation"];
     if (scs == null) scs = new sessioncustomisations();

     // get all the articles and add all customisable items
     sas = (sessionarticles)Session["sessionarticles"];
     if (sas == null) Response.Redirect("~/default.aspx", true);
     sa = sas.getarticle(pl.articles[articleindex].name);
     if (sa == null) Response.Redirect("~/default.aspx", true);

  if (Page.IsPostBack == false)
     {
      setuppage();
     }
     else
     {
      savecustomisations();
     }

    }

 protected void savecustomisations()
 {
  // get the customisation
  sessioncustomisations scs;
  scs = (sessioncustomisations)Session["sessioncustomisation"];
  if (scs == null) scs = new sessioncustomisations();

  string s;

  // loop form variables, look for the ones that we are after and save these
  for (int i = 0; i < Request.Form.Count; i++)
  {
   s = Request.Form.GetKey(i);
   if (s.StartsWith("htsp"))
   {
    s = s.Substring(4);
    scs.addsessioncustomisation(sa.articlefilename, s, Request.Form[i], true, true, false); 
   }
  }

  // save these values in a cookie and in the session
  scs.setcookie(Response);
  Session["sessioncustomisation"] = scs;

  if (Request.QueryString["return"] == "true") Response.Redirect("~/review.aspx");
  // otherwise if we have finished choosing articles go on to customise them
  else if (articleindex >= pl.articles.Count - 1) 
  Response.Redirect("~/choose_pictures.aspx");
  // otherwise choose the next article
  else Response.Redirect("~/customise_articles.aspx?article=" + (articleindex + 1).ToString()); 

 }

 protected void setuppage()
 {
  article a;

  // get the total number of articles
  farticlestotal = pl.articles.Count;
  // get the prompt for the current article
  farticleprompt = pl.articles[articleindex].prompt;
  // make sure the title and suchlike are displayed ok
  DataBind();

  // display the customisation
  // get the sessionarticle for this articlelayout
  scs.setinactive();
  if (sa != null)
  {
   // load the article
   a = ceddio.loadarticle(sa.articlefilename);
   if (a != null)
   {
    // get customisable items from the article
    a.addcustomisables(sa.articlefilename, scs);  
   }
  }

  rcustomise.DataSource = scs.inuseonly();
  rcustomise.DataBind();


 }
}
