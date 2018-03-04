using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Web.Hosting;

/// <summary>
/// Summary description for picturedrawer
/// </summary>
public class picturedrawer
{
 Graphics grapher;

	public void drawpictures(Graphics g, pagelayout pl, sessionpictures sps)
	{
  grapher = g;
  sessionpicture sp;

  // look at all the pictures in the page layout. check the name property against all pictures in the sessionpictures object
  for (int i = 0; i < pl.pictures.Count; i++)
  {
   sp = sps.picture(pl.pictures[i].name);
   if (sp != null) drawpicture(pl.pictures[i], sp); 
  }
	}

 protected void drawpicture(picturelayout pl, sessionpicture sp)
 {
  Bitmap fpicture = new Bitmap(HostingEnvironment.ApplicationPhysicalPath + "\\uploadedpictures\\" + sp.filename);

  // the hard way is to cope with the panning and zooming and to also draw the border.
  Region clip = grapher.Clip; 
  grapher.SetClip(new Rectangle(pl.left, pl.top, pl.width, pl.height));
  //code to use if we have rect of image to draw
  //grapher.DrawImage(fpicture, new Rectangle(pl.left, pl.top, pl.width, pl.height), new Rectangle(sp.left, sp.top, sp.right - sp.left, sp.bottom - sp.top), GraphicsUnit.Pixel);   
  // code to use it we have position and zoom
  grapher.DrawImage(fpicture, pl.left + sp.left, pl.top + sp.top, sp.imagewidth * sp.scale, sp.imageheight * sp.scale); 
  //grapher.DrawImage(fpicture, pl.left, pl.top, sp.imagewidth * sp.scale, sp.imageheight * sp.scale); 
  grapher.SetClip(clip, CombineMode.Replace);

  if (sp.borderfilename != null)
  {
   fpicture = new Bitmap(HostingEnvironment.ApplicationPhysicalPath + "\\borders\\" + sp.borderfilename);
   grapher.DrawImage(fpicture, pl.left, pl.top, pl.width, pl.height);
  }

  if (pl.borderwidth > 0)
  {
   // top edge
   grapher.FillRectangle(pl.bordercolour.getsolidbrush, pl.left - pl.borderwidth, pl.top - pl.borderwidth, pl.width + pl.borderwidth, pl.borderwidth);
   // bottom edge
   grapher.FillRectangle(pl.bordercolour.getsolidbrush, pl.left - pl.borderwidth, pl.top + pl.height, pl.width + pl.borderwidth, pl.borderwidth);
   // left edge
   grapher.FillRectangle(pl.bordercolour.getsolidbrush, pl.left - pl.borderwidth, pl.top, pl.borderwidth, pl.height);
   // right edge
   grapher.FillRectangle(pl.bordercolour.getsolidbrush, pl.left + pl.width, pl.top - pl.borderwidth, pl.borderwidth, pl.height + pl.borderwidth + pl.borderwidth); 
  }

  // also want to conditionally draw the low res or the full res picture
 }
}
