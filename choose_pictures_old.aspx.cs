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

public partial class choose_pictures_old : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
     picturetinkerer pt;
     sessionpagelayout spl;
     pagelayout pl = null;

     // picture data now in session so retrieve from there
     sessionpictures sps = (sessionpictures)Session["sessionpictures"];
     if (sps == null) sps = new sessionpictures();
     spl = (sessionpagelayout)Session["sessionpagelayout"];
     if (spl != null) pl = ceddio.loadpagelayout(spl.pagelayoutfilename);
     if (pl == null) Response.Redirect("~/default.aspx", true); 

     if ((pl == null) || (sps == null)) Response.Redirect("~/default.aspx", true);
     foreach (picturelayout p in pl.pictures)
     {
      pt = (picturetinkerer) LoadControl("~/picturetinkerer.ascx");
      pt.picinfo = sps.picture(p.name); 
      pt.layoutinfo = p;
      pt.layout = pl;
      ptinkerers.Controls.Add(pt);
      if (pt.picinfo != null) ladddhtml.Text += pt.getparentjavascript() + "\n";
     }
    }
}
