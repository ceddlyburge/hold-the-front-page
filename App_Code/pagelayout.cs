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
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.Hosting;

public enum verticalalign
{
 top,
 center,
 bottom,
}

public enum horizontalalign
{
 left,
 center,
 right,
}

/// <summary>
/// Size, position and styles of a portion of a pagelayout where an 
/// article part is to be drawn.
/// </summary>
public class articlelayoutpart
{
 protected sessionpictures fsps;
 protected bool fprotectoptionalpicturespace;
 protected articleparttypes fparttypes = new articleparttypes();
 // coordintates of rect in layout
 protected int fleft;
 protected int ftop; 
 protected int fwidth;
 protected int fheight;

 // coordintates of rect in layout if an optional picture is omitted
 protected int fnopictureleft;
 protected int fnopicturetop;
 protected int fnopicturewidth;
 protected int fnopictureheight;
 // name of the optional picture
 protected string foptionalpicturename = "";

 // coordintates of rect in once sized to text / aligned
 protected int fdrawleft;
 protected int fdrawtop;
 protected int fdrawwidth;
 protected int fdrawheight;

 // space from edge of rect to text
 protected int foffsetleft;
 protected int foffsettop;
 protected int foffsetright;
 protected int foffsetbottom;

 // border
 protected colordef fbordercolour = new colordef();
 protected int fborderwidth = 0;
 protected int fbordercorners = 0;

 // draw background / border at text + spacing extent or at rectangle extent.
 protected bool fsizetotextwidth = true;
 protected bool fsizetotextheight = true;

 // horizontal aligning text within a rectangle can be done (for a single line) with center aligned text and sizing the layout rectange to the text
 // vetical alignment needs to be done explicitly
 protected verticalalign farticlevalign = verticalalign.top;
 protected horizontalalign farticlehalign = horizontalalign.left;
 protected verticalalign fbordervalign = verticalalign.top;
 protected horizontalalign fborderhalign = horizontalalign.left;
 // add a warning if there is not enough text for the rect. This might not matter depening on the layout
 protected bool funderfullwarning = true;
 
 protected int fangle = 0;
 protected double fangleradians = 0;
 
 protected colordef fbackground = new colordef();

 // list of articleparttypes that are in this
 //[XmlElement("contents")]
 //[XmlElement(ElementName = "contains")]
 [XmlArrayItem(Type = typeof(articleparttype), ElementName = "articlepart"),XmlArray("contains")]
 public articleparttypes parttypes
 {
  get { return fparttypes; }
 }

 [XmlIgnoreAttribute]
 public sessionpictures sps
 {
  set { fsps = value; }
 }
 
 [XmlIgnoreAttribute]
 public bool protectoptionalpicturespace
 {
  set { fprotectoptionalpicturespace = value; }
 }
 
 [XmlElement(ElementName = "sizetotextwidth")]
 public bool sizetotextwidth
 {
  get { return fsizetotextwidth; }
  set { fsizetotextwidth = value; }
 }

 [XmlElement(ElementName = "sizetotextheight")]
 public bool sizetotextheight
 {
  get { return fsizetotextheight; }
  set { fsizetotextheight = value; }
 }

 [XmlElement(ElementName = "articleverticalalign")]
 public verticalalign articlevalign
 {
  get { return farticlevalign; }
  set { farticlevalign = value; }
 }

 [XmlElement(ElementName = "articlehorizontalalign")]
 public horizontalalign articlehalign
 {
  get { return farticlehalign; }
  set { farticlehalign = value; }
 }

 [XmlElement(ElementName = "borderverticalalign")]
 public verticalalign bordervalign
 {
  get { return fbordervalign; }
  set { fbordervalign = value; }
 }

 [XmlElement(ElementName = "borderhorizontalalign")]
 public horizontalalign borderhalign
 {
  get { return fborderhalign; }
  set { fborderhalign = value; }
 }

 [XmlElement(ElementName = "underfullwarning")]
 public bool underfullwarning
 {
  get { return funderfullwarning; }
  set { funderfullwarning = value; }
 }

 [XmlElement(ElementName = "bordercolour")]
 public colordef bordercolour
 {
  get { return fbordercolour; }
  set { fbordercolour = value; }
 }

 [XmlElement(ElementName = "borderwidth")]
 public int borderwidth
 {
  get { return fborderwidth; }
  set { fborderwidth = value; }
 }

 [XmlElement(ElementName = "bordercorners")]
 public int bordercorners
 {
  get { return fbordercorners; }
  set { fbordercorners = value; }
 }

 [XmlElement(ElementName = "optionalpicturename")]
 public string optionalpicturename
 {
  get { return foptionalpicturename; }
  set { foptionalpicturename = value; }
 }

 protected int choosecoordinate(int cnopicture, int cdefault)
 {
  if (optionalpicturename == "") return cdefault;

  if (fprotectoptionalpicturespace == true) return cdefault;

  if ((fsps.picture(optionalpicturename) == null) || (fsps.picture(optionalpicturename).haspicture == false))
   return cnopicture;
  else return cdefault;
 }

 [XmlIgnoreAttribute]
 public int left
 {
  get { return choosecoordinate(fnopictureleft, fleft); }
 }

 [XmlIgnoreAttribute]
 public int top
 {
  get { return choosecoordinate(fnopicturetop, ftop); }
 }

 [XmlIgnoreAttribute]
 public int width
 {
  get { return choosecoordinate(fnopicturewidth, fwidth); }
 }

 [XmlIgnoreAttribute]
 public int height
 {
  get { return choosecoordinate(fnopictureheight, fheight); }
 }

 [XmlElement(ElementName = "left")]
 public int withpictureleft
 {
  get { return fleft; }
  set { fleft = value; }
 }

 [XmlElement(ElementName = "top")]
 public int withpicturetop
 {
  get { return ftop; }
  set { ftop = value; }
 }

 [XmlElement(ElementName = "width")]
 public int withpicturewidth
 {
  get { return fwidth; }
  set { fwidth = value; }
 }

 [XmlElement(ElementName = "height")]
 public int withpictureheight
 {
  get { return fheight; }
  set { fheight = value; }
 }

 [XmlElement(ElementName = "nopictureleft")]
 public int nopictureleft
 {
  get { return fnopictureleft; }
  set { fnopictureleft = value; }
 }

 [XmlElement(ElementName = "nopicturetop")]
 public int nopicturetop
 {
  get { return fnopicturetop; }
  set { fnopicturetop = value; }
 }

 [XmlElement(ElementName = "nopicturewidth")]
 public int nopicturewidth
 {
  get { return fnopicturewidth; }
  set { fnopicturewidth = value; }
 }

 [XmlElement(ElementName = "nopictureheight")]
 public int nopictureheight
 {
  get { return fnopictureheight; }
  set { fnopictureheight = value; }
 }

 [XmlIgnoreAttribute]
 public int drawleft
 {
  get { return fdrawleft; }
  set { fdrawleft = value; }
 }

 [XmlIgnoreAttribute]
 public int drawtop
 {
  get { return fdrawtop; }
  set { fdrawtop = value; }
 }

 [XmlIgnoreAttribute]
 public int drawwidth
 {
  get { return fdrawwidth; }
  set { fdrawwidth = value; }
 }

 [XmlIgnoreAttribute]
 public int drawheight
 {
  get { return fdrawheight; }
  set { fdrawheight = value; }
 }

 [XmlElement(ElementName = "offsetleft")]
 public int offsetleft
 {
  get { return foffsetleft; }
  set { foffsetleft = value; }
 }

 [XmlElement(ElementName = "offsettop")]
 public int offsettop
 {
  get { return foffsettop; }
  set { foffsettop = value; }
 }

 [XmlElement(ElementName = "offsetright")]
 public int offsetright
 {
  get { return foffsetright; }
  set { foffsetright = value; }
 }

 [XmlElement(ElementName = "offsetbottom")]
 public int offsetbottom
 {
  get { return foffsetbottom; }
  set { foffsetbottom = value; }
 }

 public int textleft
 {
  get { return left + foffsetleft; }
 }

 public int textwidth
 {
  get { return width - foffsetleft - foffsetright; }
 }

 public int texttop
 {
  get { return top + foffsettop; }
 }

 public int textheight
 {
  get { return height - foffsettop - foffsetbottom; }
 }
 
 [XmlElement(ElementName = "backgroundcolour")]
 public colordef background
 {
  get { return fbackground; }
  set { fbackground = value; }
 }
 
 [XmlElement(ElementName = "angle")]
 public int angle
 {
  get { return fangle; }
  set { fangle = value; fangleradians = fangle * Math.PI / 180; }
 }

 [XmlIgnoreAttribute]
 public double angleradians
 {
  get { return fangleradians; }
 }

}

// this class stores information about where and how an article should be displayed on a page, the article class stores the words. This class stores the fonts and positions where the article is to be displayed
public class articlelayout
{
 protected string fprompt = "";
 protected string fname = "";
 protected articlestyles fstyles = new articlestyles();
 protected Collection<articlelayoutpart> flayoutparts = new Collection<articlelayoutpart>();

 [XmlElement(ElementName = "styles")]
 public articlestyles styles
 {
  get { return fstyles; }
  // set accessor required for serialisation, although we don't really want it
  set { fstyles = value; }
 }

 [XmlArrayItem("layoutpart")]
 public Collection<articlelayoutpart> layoutparts
 {
  get { return flayoutparts; }
 }

 // defines a name for this article position, such as "main", "sidebar1" etc. Used so that then session can keep track of which article is displayed where. Also useful when a user changes a layout having picked articles. The articles chosen will be transferred to the new layout as long as the names match. Names used should be as consistent as possible for this reason.
 [XmlElement(ElementName = "name")]
 public string name
 {
  get { return fname; }
  set { fname = value; }
 }

 [XmlElement(ElementName = "prompt")]
 public string prompt
 {
  get { return fprompt; }
  set { fprompt = value; }
 }
}

/// <summary>
/// Definition of picture placeholder in the layout
/// Picture can be added into these
/// </summary>
public class picturelayout
{
 // coordintates of rect in layout
 protected int fleft;
 protected int ftop;
 protected int fwidth;
 protected int fheight;
 protected string fname;
 protected string fprompt;
 protected bool foptional = false;
 protected int fborderwidth = 0;
 protected colordef fbordercolour = new colordef();

 [XmlElement(ElementName = "left")]
 public int left
 {
  get { return fleft; }
  set { fleft = value; }
 }

 [XmlElement(ElementName = "top")]
 public int top
 {
  get { return ftop; }
  set { ftop = value; }
 }

 [XmlElement(ElementName = "width")]
 public int width
 {
  get { return fwidth; }
  set { fwidth = value; }
 }

 [XmlElement(ElementName = "height")]
 public int height
 {
  get { return fheight; }
  set { fheight = value; }
 }

 [XmlElement(ElementName = "borderwidth")]
 public int borderwidth
 {
  get { return fborderwidth; }
  set { fborderwidth = value; }
 }

 [XmlElement(ElementName = "bordercolour")]
 public colordef bordercolour
 {
  get { return fbordercolour; }
  set { fbordercolour = value; }
 }

 [XmlElement(ElementName = "name")]
 public string name
 {
  get { return fname; }
  set { fname = value; }
 }

 [XmlElement(ElementName = "prompt")]
 public string prompt
 {
  get { return fprompt; }
  set { fprompt = value; }
 }

 [XmlElement(ElementName = "optional")]
 public bool optional
 {
  get { return foptional; }
  set { foptional = value; }
 }

}

public class layoutpictures : CollectionBase
{
 // this is an untyped hashtable
 Hashtable fpictures = new Hashtable();

 // overrides colectionbase.add, mainly for use by xmlserialiser
 public virtual void Add(picturelayout pl)
 {
  this.List.Add(pl);
  fpictures.Add(pl.name, pl);
 }

 // overrides colectionbase indexer, mainly for use by xmlserialiser
 public virtual picturelayout this[int Index]
 {
  get { return (picturelayout)this.List[Index]; }
 }

 public picturelayout picture(string name)
 {
  return ((picturelayout)fpictures[name]);
 }

 /*
 public void setcookie(HttpResponse Response)
 {
  XmlSerializer serializer = new XmlSerializer(typeof(sessionpictures));
  StringWriter tw = new StringWriter();
  serializer.Serialize(tw, this);
  HttpCookie cookie = new HttpCookie("pictures", HttpUtility.UrlEncode(tw.ToString()));
  cookie.Expires = DateTime.Now.AddYears(1);
  cookie.HttpOnly = true;
  Response.SetCookie(cookie);
 }*/
}

/// <summary>
/// Definition of an page with a bakground image and list of  
/// article layouts. Articles to go in the layout are set elsewhere.
/// </summary>
[XmlType("pagelayout")]
public class pagelayout
{
 protected Collection<articlelayout> farticles = new Collection<articlelayout>();
 protected layoutpictures fpictures = new layoutpictures();
 protected string fbackgroundimagefilename;
 protected Bitmap fbackgroundimage;
 protected string fpreviewimagefilename;
 protected Bitmap fpreviewimage;
 protected int fbackgroundheight;
 protected int fbackgroundwidth;
 protected int fpreviewheight;
 protected int fpreviewwidth;

 [XmlArrayItem("article")]
 public Collection<articlelayout> articles
 {
  get { return farticles; }
 }

 [XmlArrayItem("picture")]
 public layoutpictures pictures
 {
  get { return fpictures; }
 }

 [XmlElement(ElementName = "backgroundimage")]
 public string backgroundimagefilename
 {
  get { return fbackgroundimagefilename; }
  set 
  { 
   fbackgroundimagefilename = value;
   createbackgroundimage();
  }
 }

 [XmlElement(ElementName = "previewimage")]
 public string previewimagefilename
 {
  get { return fpreviewimagefilename; }
  set
  {
   fpreviewimagefilename = value;
   createpreviewimage();
  }
 }

 [XmlIgnoreAttribute]
 public int backgroundheight
 {
  get { return fbackgroundheight; }
 }

 [XmlIgnoreAttribute]
 public int backgroundwidth
 {
  get { return fbackgroundwidth; }
 }

 [XmlIgnoreAttribute]
 public int previewheight
 {
  get { return fpreviewheight; }
 }

 [XmlIgnoreAttribute]
 public int previewwidth
 {
  get { return fpreviewwidth; }
 }

 [XmlIgnoreAttribute]
 public Bitmap backgroundimage
 {
  get { return fbackgroundimage; }
 }

 [XmlIgnoreAttribute]
 public Bitmap previewimage
 {
  get { return fpreviewimage; }
 }

 protected void createbackgroundimage()
 {
  fbackgroundimage = new Bitmap(HostingEnvironment.ApplicationPhysicalPath + "\\pagebackgrounds\\" + fbackgroundimagefilename);
  fbackgroundheight = fbackgroundimage.Height;
  fbackgroundwidth = fbackgroundimage.Width;  
 }

 protected void createpreviewimage()
 {
  fpreviewimage = new Bitmap(HostingEnvironment.ApplicationPhysicalPath + "\\layoutpreviews\\" + fpreviewimagefilename);
  fpreviewheight = fpreviewimage.Height;
  fpreviewwidth = fpreviewimage.Width;
 }
}
