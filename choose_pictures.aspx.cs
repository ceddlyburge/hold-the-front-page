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
using System.Web.Hosting;
using System.Drawing;

public partial class choose_pictures : System.Web.UI.Page
{

 protected int fpictureindex;
 public int pictureindex
 {
  get { return fpictureindex; }
 }

 protected int fpicturestotal;
 public int picturestotal
 {
  get { return fpicturestotal; }
 }

 protected string fpictureprompt;
 public string pictureprompt
 {
  get { return fpictureprompt; }
 }

 protected string fpicturename;
 public string picturename
 {
  get { return fpicturename; }
 }
 
 protected int imgwidth
 {
  get { if (picinfo == null) return 0; else return (int) (picinfo.imagewidth * pagescale); }
 }

 protected int imgheight
 {
  get { if (picinfo == null) return 0; else  return (int) (picinfo.imageheight * pagescale); }
 }

 protected int imgx
 {
  get { if (picinfo == null) return 0; else return (int) (picinfo.left * pagescale); }
 }

 protected int imgy
 {
  get { if (picinfo == null) return 0; else  return (int) (picinfo.top * pagescale); }
 }

 protected string imgsrc
 {
  get { if (picinfo == null) return ""; else  return picinfo.filename; }
 }

 protected float imgscale
 {
  get { if (picinfo == null) return 1; else return picinfo.scale; }
 }

 public string frontpagetemplateimage
 {
  get { return pl.backgroundimagefilename;}
 }

 public string articleoverlayimage
 {
  get { try { return Session["articleoverlayfilename"].ToString(); } catch { return ""; } }
 }

 public int layoutx
 {
  get { return (int) (layoutinfo.left * pagescale); }
 }

 public int layouty
 {
  get { return (int) (layoutinfo.top * pagescale); }
 }

 public int layoutwidth
 {
  get { return (int) (layoutinfo.width * pagescale); }
 }

 public int layoutheight
 {
  get { return (int) (layoutinfo.height * pagescale); }
 }

 public int previewx
 {
  get { return (int)(layoutinfo.left * pl.previewwidth / pl.backgroundwidth); } 
 }

 public int previewy
 {
  get { return (int)(layoutinfo.top * pl.previewheight / pl.backgroundheight); }
 }

 public int previewwidth
 {
  get { return (int)(layoutinfo.width * pl.previewwidth / pl.backgroundwidth); }
 }

 public int previewheight
 {
  get { return (int)(layoutinfo.height * pl.previewheight / pl.backgroundheight); }
 }

 public int frontpagewidth
 {
  get { return (int) (pl.backgroundwidth * pagescale); }
 }

 public int frontpageheight
 {
  get { return (int)(pl.backgroundheight * pagescale); }
 }

 public string scaleexplanation
 {
  get { if (pagescale > 1) return "This view of the front page has been scaled up."; else return "This view of the front page has been scaled down."; }
 }

 protected sessionpagelayout spl;
 protected pagelayout pl = null;
 protected sessionpicture picinfo;
 protected picturelayout layoutinfo;
 protected sessionpictures sps;
 protected Single pagescale;

    protected void Page_Load(object sender, EventArgs e)
    {
     Single xscale, yscale;

     // get the article index to choose from the query string
     try
     {
      fpictureindex = Convert.ToInt16(Request.QueryString["picture"]);
     }
     catch
     {
      fpictureindex = 0;
     }

     // get layout and picture info from the seesion, redirect to home page if we don't have all that we need
     try 
     {
      spl = (sessionpagelayout)Session["sessionpagelayout"];
      if (spl != null) pl = ceddio.loadpagelayout(spl.pagelayoutfilename);
      layoutinfo = pl.pictures[pictureindex];
      // get the total number of articles
      fpicturestotal = pl.pictures.Count;

      // get the prompt for the current article
      fpicturename = pl.pictures[pictureindex].name;
      fpictureprompt = pl.pictures[pictureindex].prompt;
      sps = (sessionpictures)Session["sessionpictures"];
      if (sps == null)
      {
       sps = new sessionpictures();
       Session["sessionpictures"] = sps;
      }
     }
     catch 
     { 
      Response.Redirect("default.aspx", true); 
     }

     // if this is the first load of this page then save an image containing just the text of the articles. This will be overlaid on the uploaded pictures so that the user can line up the picture and the headline nicely.
     if ((Request.UrlReferrer == null) || (Request.Url.AbsolutePath != Request.UrlReferrer.AbsolutePath))
     {
      Session["articleoverlayfilename"] = "";
      // get an image containing just the text overlay
      Bitmap barticles = articlesdrawer.drawarticles(pl, (sessionarticles)Session["sessionarticles"], (sessioncustomisations)Session["sessioncustomisation"], sps, true);
      string articleoverlayfilename = Guid.NewGuid().ToString() + ".png";
      barticles.Save(Server.MapPath("~/articleoverlays/") + articleoverlayfilename, ImageFormat.Png);
      Session["articleoverlayfilename"] = articleoverlayfilename;
     }

     // work out the scaling for this picture so that all pictures take up the same space on screen when positioning. This is so that small pictures are easily visible and so large pictures are not unmanageably large
     xscale = (400f / (Single)layoutinfo.width);
     yscale = (300f / (Single)layoutinfo.height);
     pagescale = Math.Min(xscale, yscale);

     // get the session info for this picture, if there is any
     try { picinfo = sps.picture(fpicturename); }
     catch { picinfo = null; }

     if (Page.IsPostBack == true) return;

     // show hide the controls for an optional picture
     poptional.Visible = layoutinfo.optional;

     // hide the javascript if there is no picture to work with
     if ((picinfo == null) || (System.IO.File.Exists(HostingEnvironment.ApplicationPhysicalPath + "\\uploadedpictures\\" + picinfo.filename)) == false)
     {
      pimgscript.Visible = false;
      //pcrop.Visible = false;
     }
     else
     {
      //
     }

     DataBind();

    }

 protected void redirectnext()
 {

  // go back to review page if we came from there, otherwise move on to next picture or next step
  if (Request.QueryString["return"] == "true") Response.Redirect("~/review.aspx");
  else if (pictureindex >= pl.pictures.Count - 1)
   Response.Redirect("~/review.aspx");
  else Response.Redirect("~/choose_pictures.aspx?picture=" + (pictureindex + 1).ToString());
 }

 protected void bskip_Click(object sender, EventArgs e)
 {
  if (picinfo != null)
  {
   picinfo.filename = "";
   picinfo.thumbnailfilename = "";
   picinfo.haspicture = false;
  }

  // move on to next picture or next step
  redirectnext();
 }

 protected void bcrop_Click(object sender, EventArgs e)
 {
  // would be strange if no picinfo as would mean that there was no picture either, but check anyway
  if (picinfo == null)
  {
   picinfo = new sessionpicture();
   picinfo.name = layoutinfo.name;
   sps.Add(picinfo);
  }

  // update position and scale
  try { picinfo.left = (int) (Convert.ToSingle(Request.Form["imagex"]) / pagescale); }
  catch { picinfo.left = 0;  }
  try { picinfo.top = (int) (Convert.ToSingle(Request.Form["imagey"]) / pagescale);}
  catch { picinfo.top = 0;  }
  try { picinfo.scale = Convert.ToSingle(Request.Form["imagesize"]); }
  catch { picinfo.scale = 1; }

  // save picture info
  sps.setcookie(Response);
  Session["sessionpictures"] = sps;

  // move on to next picture or next step
  redirectnext();
 }

    protected void bupload_Click(object sender, EventArgs e)
    {
     FileUpload fu = fupicture;
     Single xscale, yscale, scale;

     if (fu.HasFile == false)
     {
      // follow the redirect on post pattern
      Response.Redirect(Request.Url.ToString(), true);
     }

     Guid gfilename = Guid.NewGuid();
     string filename = Convert.ToString(gfilename);
     String fileextension = System.IO.Path.GetExtension(fu.FileName).ToLower();
     bool fileok = false;
     System.Drawing.Image inpic, outpic, thumbnailpic;
     string path = Server.MapPath("~/uploadedpictures/");

     // validate the picture (high security!)
     String[] allowedExtensions = { ".gif", ".png", ".jpeg", ".jpg", ".tif", ".tiff", ".bmp" };
     for (int i = 0; i < allowedExtensions.Length; i++)
     {
      if (fileextension == allowedExtensions[i]) fileok = true;
     }

     if (fileok == false)
     {
      //lpicerror.Text = "Please specify a valid picture (.gif, .png, .jpeg, .jpg, .tif, .tiff, .bmp)";
      //ppicture.Visible = true;
      return;
     }

     // save the picture and a thumbnail, ensure unique filenames
     inpic = System.Drawing.Image.FromStream(fu.PostedFile.InputStream);
     outpic = savepic(path + filename + ".jpg", inpic, 3000);
     thumbnailpic = savepic(path + filename + "thumbnail.jpg", inpic, 50);

     // update the session picture with this uploaded picture, set the cropping so that it is roughly half the size of the picture and is the correct proportions for the layoutpicture it is going into
     if (picinfo == null)
     {
      picinfo = new sessionpicture();
      picinfo.name = layoutinfo.name;
      sps.Add(picinfo);
     }

     picinfo.filename = filename + ".jpg";
     picinfo.thumbnailfilename = filename + "thumbnail.jpg";
     picinfo.imageheight = outpic.Height;
     picinfo.imagewidth = outpic.Width;
     picinfo.thumbnailheight = thumbnailpic.Height;
     picinfo.thumbnailwidth = thumbnailpic.Width;
     picinfo.left = 0;
     picinfo.top = 0;
     xscale = (layoutinfo.width / picinfo.imagewidth);
     yscale = (layoutinfo.height / picinfo.imageheight);
     scale = Math.Max(xscale, yscale);
     picinfo.scale = Math.Max(1, scale);

     sps.setcookie(Response);
     Session["sessionpictures"] = sps;

     inpic = null;
     outpic = null;
     thumbnailpic = null;
     GC.Collect();

     // follow the redirect on post pattern
     Response.Redirect(Request.Url.AbsolutePath + Request.Url.Query, true);
    }

    protected System.Drawing.Image savepic(string filename, System.Drawing.Image pic, int maxdimension)
    {
     ImageCodecInfo codecinfo = ceddutils.getencoderinfo("image/jpeg");
     EncoderParameters encoderparams = new EncoderParameters(2);
     encoderparams.Param[0] = new EncoderParameter(Encoder.Compression,
   (long)EncoderValue.CompressionLZW);
     encoderparams.Param[1] = new EncoderParameter(Encoder.Quality,
     (long)100);
     float scaling;
     int width, height;
     System.Drawing.Image constrainedpic;

     // resize the largest dimension to maxdimension and resize the other
     // one by the same amount
     if (pic.Width > pic.Height)
     {
      scaling = (float)pic.Width / (float)maxdimension;
     }
     else
     {
      scaling = (float)pic.Height / (float)maxdimension;
     }
     width = (int)(pic.Width / scaling);
     height = (int)(pic.Height / scaling);

     if (scaling <= 1)
     {
      constrainedpic = pic;
      constrainedpic.Save(filename, codecinfo, encoderparams);
     }
     else
     {
      constrainedpic = ceddutils.generatethumbnail(pic, width, height);
      constrainedpic.Save(filename, codecinfo, encoderparams);
     }

     return constrainedpic;
    }

}
