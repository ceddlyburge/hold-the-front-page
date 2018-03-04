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
using System.Drawing;

public partial class picturetinkerer : System.Web.UI.UserControl
{
 //protected string fplaceholdername;
 protected sessionpicture fpicinfo;
 protected picturelayout flayoutinfo;
 protected pagelayout flayout;
 protected float layoutscale;
 protected float minzoom, maxzoom;

 public sessionpicture picinfo
 {
  get { return fpicinfo; }
  set { fpicinfo = value; }
 }

 public picturelayout layoutinfo
 {
  get { return flayoutinfo; }
  set { flayoutinfo = value; }
 }

 public pagelayout layout
 {
  get { return flayout; }
  set { flayout = value; }
 }

 //public string placeholdername
 //{
 // get { return fplaceholdername; }
 // set { fplaceholdername = value; }
 //}

 protected float calcminzoom()
 {
  return Math.Max((float)layoutinfo.width / (float)picinfo.imagewidth, (float)layoutinfo.height / (float)picinfo.imageheight); 
 }

 protected void Page_Load(object sender, EventArgs e)
 {
  if (Page.IsPostBack == true) return;

  if (Session["frontpagethumbnail"] != null) 
  {
   ppreview.BackImageUrl = "~/frontpagethumbnails/" + Session["frontpagethumbnail"].ToString();
  }

  // work out the scaling for the representation of the layout area
  layoutscale = (float)ppreview.Height.Value / (float)layout.backgroundheight;

  // position and size the rectangle indication the size of the layout boundary
  positionandsizepictureplaceholder();

  // set button stuff up so that we know what was clicked
  bupload.CommandName = layoutinfo.name;
  boffset.CommandName = layoutinfo.name;
  bzoom.CommandName = layoutinfo.name;

  if (picinfo != null)
  {
   // position and size the thumbnail to indicate which part of the image is drawn in the layout boundary
   positionandsizethumbnail();

   // add code to respond the end of a drag event
   addjavascript();
  }
 }

 protected void positionandsizethumbnail()
 {
  // work out the scaling for the representation of the layout area
  //float layoutscale = (float)playoutboundary.Width.Value / (float)layoutinfo.width;

  /*  // set the size of the thumbnail
  ithumbnail.Width = (int)(picinfo.imagewidth * picinfo.zoom * layoutscale);
  ithumbnail.Attributes["Width"] = ithumbnail.Width.ToString(); 
  ithumbnail.Height = (int)(picinfo.imageheight * picinfo.zoom * layoutscale);
  ithumbnail.Attributes["Height"] = ithumbnail.Height.ToString();
  hfx.Value = "0";
  hfy.Value = "0"; 
  
  // position the image
  // (0, 0) is the top left of the layout boundary
  ithumbnail.Style["left"] = Convert.ToString((int)(-picinfo.offsetx * layoutscale)) + "px";
  ithumbnail.Style["top"] = Convert.ToString((int)(-picinfo.offsety * layoutscale)) + "px";

  // set the picture
  ithumbnail.ImageUrl = "~/uploadedpictures/" + picinfo.thumbnailfilename;
  //ithumbnail.Visible = false;*/
 }

 protected void positionandsizepictureplaceholder()
 {
  playoutboundary.Width = (int)(layoutinfo.width * layoutscale);
  playoutboundary.Height = (int)(layoutinfo.height * layoutscale);
  playoutboundary.Style["left"] = Convert.ToString((int)(layoutinfo.left * layoutscale)) + "px";
  playoutboundary.Style["top"] = Convert.ToString((int)(layoutinfo.top * layoutscale)) + "px";

  ppreview.Width = (int)(layout.backgroundwidth * layoutscale);

/*  int size;
  // we want the layoutboundary to take up roughly one thrid of each dimension and to be the same relative dimensions as the placeholder from the layout
  // largest dimension is to be 50 pixels
  if (layoutinfo.height > layoutinfo.width)
  {
   size = (int)((50 * ((float)layoutinfo.width / (float)layoutinfo.height)));
   playoutboundary.Width = size;
   layoutleft = (int)((150.0 - (float)size) / 2.0);
   layouttop = 50;
   playoutboundary.Style["left"] = Convert.ToString(layoutleft) + "px";
  }
  else
  {
   size = (int)((50 * ((float)layoutinfo.height / (float)layoutinfo.width)));
   playoutboundary.Height = size;
   layouttop = (int)((150.0 - (float)size) / 2.0);
   layoutleft = 50;
   playoutboundary.Style["top"] = Convert.ToString(layouttop) + "px";
  }
 */
 }

 protected void bupload_Click(object sender, EventArgs e)
 {
/*  // find the name of the picture layout
  string name = (sender as Button).CommandName;
  // find the file upload control
  FileUpload fu = (FileUpload)FindControl("fupicture");

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
  sessionpictures sps = (sessionpictures)Session["sessionpictures"];
  sessionpagelayout spl = (sessionpagelayout)Session["sessionpagelayout"];
  pagelayout pl = null;
  if (spl != null) pl = ceddio.loadpagelayout(spl.pagelayoutfilename);
  if (pl == null) Response.Redirect("~/default.aspx", true);
  
  picinfo = sps.picture(name);
  layoutinfo = pl.pictures.picture(name);

  if (fu.HasFile == false) return;

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

  // update the session picture with this uploaded picture, set the zoom so that the picture fits in the layout (one dimension can be cropped), set the panning to nothing, save as a maximum size (say 10meg), set original dimensions of seesion picture
  if (picinfo == null) 
  {
   picinfo = new sessionpicture();
   picinfo.name = layoutinfo.name;  
   ((sessionpictures)Session["sessionpictures"]).Add(picinfo); 
  }
   
  picinfo.filename = filename + ".jpg";
  picinfo.thumbnailfilename = filename + "thumbnail.jpg";
  picinfo.offsetx = 0;
  picinfo.offsety = 0;
  picinfo.imageheight = outpic.Height;
  picinfo.imagewidth = outpic.Width;
  picinfo.thumbnailheight = thumbnailpic.Height;
  picinfo.thumbnailwidth = thumbnailpic.Width;
  picinfo.zoom = calcminzoom(); 

  // store these changes in the users cookie, the data in session should be a pointer and be updated already 
  sps.setcookie(Response); 

  inpic = null;
  outpic = null;
  thumbnailpic = null;
  GC.Collect();

  // follow the redirect on post pattern
  Response.Redirect(Request.Url.ToString(), true);
 */}

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

 protected void addjavascript()
 {/*
  string s;

  // function to sort out the panning
  s = "function " + ithumbnail.Attributes["name"] + "dropped(name) \n{ \n";
  s += "document.getElementById('" + hfx.ClientID + "').value = (dd.obj.x - dd.obj.defx) / " + layoutscale.ToString()   + ";\n";
  s += "document.getElementById('" + hfy.ClientID + "').value = (dd.obj.y - dd.obj.defy)  / " + layoutscale.ToString() + ";\n";
  s += "document.getElementById('" + boffset.ClientID + "').click();\n";  
  s += "}\n";
  ldropfunc.Text = s;

  // function to sort out the dragging
  // defx of slider must be the same as its initial value so must work out values around this
  s = "function " + ithumb.Attributes["name"] + "dragged(name) \n{ \n";
  s += "var zoom = " + picinfo.zoom.ToString() + " + (((dd.obj.x - dd.obj.defx) / 100) * " + Convert.ToString(maxzoom - minzoom) + ");\n";
  s += "document.getElementById('" + hfzoom.ClientID + "').value = zoom;\n";
  // also resize the thumbnail image
  s += "dd.elements." + ithumbnail.Attributes["name"] + ".resizeTo(zoom * " + Convert.ToString(picinfo.imagewidth * layoutscale) + ", zoom * " + Convert.ToString(picinfo.imageheight * layoutscale) + ");\n";
  // and reposition it to take up full layout area if required
  s += "var x = " + "dd.elements." + ithumbnail.Attributes["name"] + ".x;";
  s += "var y = " + "dd.elements." + ithumbnail.Attributes["name"] + ".y;";
  s += String.Format("if (({0}.y + {0}.h) < ({1}.y + {1}.h)) {{ y = {1}.y + {1}.h - {0}.h; }}\n", "dd.elements." + ithumbnail.Attributes["name"], "dd.elements." + playoutboundary.ClientID);
  s += String.Format("if (({0}.x + {0}.w) < ({1}.x + {1}.w)) {{ x = {1}.x + {1}.w - {0}.w; }}\n", "dd.elements." + ithumbnail.Attributes["name"], "dd.elements." + playoutboundary.ClientID);
  s += String.Format("{0}.moveTo(x, y); \n",  "dd.elements." + ithumbnail.Attributes["name"]);
  
  s += "}\n";

  // function to auto postback the zoom when done and set any change in the panning as a result of the zoom
  s += "function " + ithumb.Attributes["name"] + "dropped(name) \n{ \n";
  s += String.Format("document.getElementById('" + hfx.ClientID + "').value = ({0}.x - {0}.defx) / " + layoutscale.ToString() + ";\n", "dd.elements." + ithumbnail.Attributes["name"]);
  s += String.Format("document.getElementById('" + hfy.ClientID + "').value = ({0}.y - {0}.defy) / " + layoutscale.ToString() + ";\n", "dd.elements." + ithumbnail.Attributes["name"]);
  s += "document.getElementById('" + bzoom.ClientID + "').click();\n";     s += "}\n";

  lzoomfunc.Text = s;
  
  lzoomfunc.Text = s;
  */
 }

 public string getparentjavascript()
 {
  return "";
  /*
  string s;
  int offtop, offbot, offleft, offright;
  int zoomx, zoomleft, zoomright;

  // if there is no thumbnail yet then we don't need any javascript
  if (picinfo == null) return "";

  // work out the scaling for the representation of the layout area
  layoutscale = (float)ppreview.Height.Value / (float)layout.totalheight;

  // work out the zoom limits and the current position of the zoom slider
  // min zoom is the zoom at which the picture minimally fits into the layout area
  // max zoom is twice this
  minzoom = calcminzoom();
  maxzoom = minzoom * 2;
  // track is 100 pixels and thumb is 20
  zoomx = (int) (((picinfo.zoom - minzoom) / (maxzoom - minzoom)) * 100) - 10;
  zoomleft = Math.Max(zoomx + 10, 0);
  zoomright = Math.Max(90 - zoomx, 0);


  // set unique names for the dhtml stuff as there may be more than one of these controls on a page
  // images use the name property whereas layers (div and panels etc) use the id property
  ithumbnail.Attributes["name"] = "ithumbnail" + picinfo.name;
  ithumb.Attributes["name"] = "izoomthumb" + picinfo.name;
  itrack.Attributes["name"] = "izoomtrack" + picinfo.name;

  // set the panning limits so that all the layout picture has a bit of picture in it
  offtop = (int)Math.Floor((picinfo.imageheight * picinfo.zoom * layoutscale) - (layoutinfo.height * layoutscale) - (picinfo.offsety * layoutscale));
  offbot = (int) Math.Floor(picinfo.offsety * layoutscale);
  offleft = (int)Math.Floor((picinfo.imagewidth * picinfo.zoom * layoutscale) - (layoutinfo.width * layoutscale) - (picinfo.offsetx * layoutscale));
  offright = (int)Math.Floor(picinfo.offsetx * layoutscale);
  offtop = Math.Max(offtop, 0);
  offleft = Math.Max(offleft, 0);
  offbot = Math.Max(offbot, 0);
  offright = Math.Max(offright, 0);

  // add javascript stuff to make all the fancy stuff work
  // make the thumbnail moveable
  s = "";
  s += "SET_DHTML(\"" + playoutboundary.ClientID + "\"+NO_DRAG+RESET_Z);\n";
  s += "SET_DHTML(\"" + ithumbnail.Attributes["name"] + "\"+RESET_Z";
    s += "+MAXOFFTOP+" + Convert.ToString(offtop);
    s += "+MAXOFFBOTTOM+" + Convert.ToString(offbot);
    s += "+MAXOFFLEFT+" + Convert.ToString(offleft);
    s += "+MAXOFFRIGHT+" + Convert.ToString(offright);
    s += ");\n ";
  // make the zoom thumb movable horizontally within bounds of track and add the track for relative positioning
  s += "SET_DHTML(\"" + ithumb.Attributes["name"]+ "\"+MAXOFFLEFT+" + Convert.ToString(zoomleft) + "+MAXOFFRIGHT+" + Convert.ToString(zoomright) + "+HORIZONTAL);\n";  
  s += "SET_DHTML(\"" + itrack.Attributes["name"]+ "\"+NO_DRAG);\n";  
  // set function to record panning of the thumbnail
  s += "dd.elements." + ithumbnail.Attributes["name"] + ".setDropFunc(" + ithumbnail.Attributes["name"] + "dropped);\n";
  // set the thumbnail to be seethrough
  s += "dd.elements." + ithumbnail.Attributes["name"] + ".setOpacity(0.5);\n";
  // set the thumbnail to eb a child of the layout boundary
  // this didn't seem to help s += "dd.elements." + playoutboundary.ClientID + ".addChild(\"" + ithumbnail.Attributes["name"] + "\");\n";
  // sort out the zoom slider - set initial position of thumb
  s += "dd.elements." + ithumb.Attributes["name"] + ".moveTo(dd.elements." + itrack.Attributes["name"] + ".x + " + zoomx.ToString() + ", dd.elements." + itrack.Attributes["name"] + ".y);\n";
  // set thumb to be on top
  s += "dd.elements." + ithumb.Attributes["name"] + ".setZ(dd.elements." + itrack.Attributes["name"] + ".z + 1);\n";
  // set thumb as child of track so they move relatvie to one another when resizing the window
  s += "dd.elements." + itrack.Attributes["name"] + ".addChild(\"" + ithumb.Attributes["name"] + "\");\n";
  // set the default (initial) value of the thumb to be the left hand extreme to make it easy to work out zooms
  s += "dd.elements." + ithumb.Attributes["name"] + ".defx = dd.elements." + itrack.Attributes["name"] + ".x + " + zoomx.ToString() + ";\n"; 
  // set the function to call while dragging
  s += "dd.elements." + ithumb.Attributes["name"] + ".setDragFunc(" + ithumb.Attributes["name"] + "dragged);\n";
  s += "dd.elements." + ithumb.Attributes["name"] + ".setDropFunc(" + ithumb.Attributes["name"] + "dropped);\n";
  return s;
 */}

 protected void boffset_Click(object sender, EventArgs e)
 {
 /* // find the name of the picture layout
  string name = (sender as Button).CommandName;
  sessionpictures sps = (sessionpictures)Session["sessionpictures"];
  sessionpagelayout spl = (sessionpagelayout)Session["sessionpagelayout"];
  pagelayout pl = null;
  if (spl != null) pl = ceddio.loadpagelayout(spl.pagelayoutfilename);
  if (pl == null) Response.Redirect("~/default.aspx", true);
  picinfo = sps.picture(name);
  layoutinfo = pl.pictures.picture(name);

  picinfo.offsetx -= (int)Convert.ToDouble(hfx.Value);
  picinfo.offsety -= (int)Convert.ToDouble(hfy.Value);

  // store these changes in the users cookie, the data in session should be a pointer and be updated already 
  sps.setcookie(Response);

  // follow the redirect on post pattern
  Response.Redirect(Request.Url.ToString(), true);
*/ }

 protected void bzoom_Click(object sender, EventArgs e)
 {/*
  // find the name of the picture layout
  string name = (sender as Button).CommandName;
  sessionpictures sps = (sessionpictures)Session["sessionpictures"];
  sessionpagelayout spl = (sessionpagelayout)Session["sessionpagelayout"];
  pagelayout pl = null;
  if (spl != null) pl = ceddio.loadpagelayout(spl.pagelayoutfilename);
  if (pl == null) Response.Redirect("~/default.aspx", true);
  picinfo = sps.picture(name);
  layoutinfo = pl.pictures.picture(name);

  picinfo.zoom = Convert.ToSingle(hfzoom.Value);

  // also set the offset as the zooming might have changed this
  picinfo.offsetx -= (int)Convert.ToDouble(hfx.Value);
  picinfo.offsety -= (int)Convert.ToDouble(hfy.Value);

  // store these changes in the users cookie, the data in session should be a pointer and be updated already 
  sps.setcookie(Response);

  // follow the redirect on post pattern
  Response.Redirect(Request.Url.ToString(), true);
 */}
}
