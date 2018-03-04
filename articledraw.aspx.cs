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
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Web.Hosting;

public partial class articledraw : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
     sessionpagelayout spl;
     pagelayout pl = null;
     Graphics gbackground;//, gthumbnail;
     System.Drawing.Bitmap bbackground, bbackgroundimage, barticles;//, bthumbnail;
     //string thumbnailfilename;
     sessionpictures sps;

     Response.ClearContent();
		   Response.ContentType = "image/jpeg";

     spl = (sessionpagelayout)Session["sessionpagelayout"];
     if (spl != null) pl = ceddio.loadpagelayout(spl.pagelayoutfilename);

     // if there is no layout specified the redirect back to the home page
     if (pl == null) Response.Redirect("~/default.aspx", true);
     sps = (sessionpictures)Session["sessionpictures"];

     // some pictures seem to have some funny information in them which messes up the dimensions and suchlike, Couldn't find a way to work out what was doing this or to reset it so we create our own bitmap which will have default settings which we can rely on
     bbackgroundimage = pl.backgroundimage;
     bbackground = new Bitmap(bbackgroundimage.Width, bbackgroundimage.Height);
     gbackground = Graphics.FromImage(bbackground);
     gbackground.PageUnit = GraphicsUnit.Pixel;
     gbackground.PageScale = 1;
     gbackground.ResetTransform();
     gbackground.PixelOffsetMode = PixelOffsetMode.None;
     gbackground.DrawImage(bbackgroundimage, new Rectangle(0, 0, bbackgroundimage.Width, bbackgroundimage.Height));

     // get an image containing just the text overlay
     barticles = articlesdrawer.drawarticles(pl, (sessionarticles)Session["sessionarticles"], (sessioncustomisations)Session["sessioncustomisation"], sps, false);  

     // save thumbnails of the background and the text
     /*
     bthumbnail = (System.Drawing.Bitmap)  ceddutils.generatethumbnail((System.Drawing.Image) bbackground, (int)((float)bbackground.Width / ((float)bbackground.Height / 150.0)), 150);
     gthumbnail = Graphics.FromImage(bthumbnail);
     gthumbnail.DrawImage(barticles, 0, 0, bthumbnail.Width, bthumbnail.Height);

     thumbnailfilename = Guid.NewGuid().ToString() + ".jpg";
     bthumbnail.Save(Server.MapPath("~/frontpagethumbnails/") + thumbnailfilename, ImageFormat.Jpeg);
     Session["frontpagethumbnail"] = thumbnailfilename;
     */
     try
     {
      //serializer = new XmlSerializer(typeof(sessionpictures));
      //StringReader sr = new StringReader(HttpUtility.UrlDecode(Request.Cookies["pictures"].Value));
      //sps = (sessionpictures)serializer.Deserialize(sr);
      picturedrawer p = new picturedrawer();
      p.drawpictures(gbackground, pl, sps);
     }
     catch { }

     // combine the background and the text and return it
     gbackground.DrawImage(barticles, new Rectangle(0, 0, bbackground.Width, bbackground.Height)); 
     bbackground.Save(Response.OutputStream, ImageFormat.Jpeg);

     Response.End();
    }
}
