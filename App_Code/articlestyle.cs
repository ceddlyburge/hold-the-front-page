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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.Drawing.Drawing2D;

// names of text styles.
// articles define which style should be used where
// layouts define the font, color etc for each style
public enum textstyletype
{
 headline, subhead, strap, byline, intro, paragraph2, body, caption, crossref, introbold, paragraph2bold, bodybold
}

// names of paragraph styles.
public enum paragraphstyletype
{
 headline, subhead, strap, byline, intro, paragraph2, body, caption, crossref
}

// text justification for a paragraph
public enum paragraphjustification
{
 left, center, right, justified, headline
}

// font definition, this can be serialised which fonts cannot
// this class can create the font when you need it
public class fontdefinition
{
 protected FontStyle fstyle;
 protected string ffamily;
 protected float fsize;
 protected GraphicsUnit funit;
 protected byte fcharset;
 protected bool fvertical;
 protected Font ffont;
 protected Single fbaselineoffset;

 protected Font fgetfont()
 {
  if (ffont == null)
  {
   ffont = CreateFont();
   fbaselineoffset = ffont.SizeInPoints / ffont.FontFamily.GetEmHeight(ffont.Style) * ffont.FontFamily.GetCellAscent(ffont.Style);
  }
  return ffont;
 }

 [XmlIgnoreAttribute]
 public Font getfont
 {
  get { return fgetfont(); }
 }

 [XmlIgnoreAttribute]
 public Single baselineoffset
 {
  get { fgetfont(); return fbaselineoffset; }
 }

 public int baselineoffsetpixels(Graphics g)
 {

  return (int)Math.Round( g.DpiY / 72.0F * baselineoffset); 
 }

 public int descentpixels(Graphics g)
 {
  return getfont.Height - baselineoffsetpixels(g);
 }

 [XmlElement(ElementName = "style")]
 public FontStyle style
 {
  get { return fstyle; }
  set { fstyle = value; ffont = null; }
 }

 [XmlElement(ElementName = "family")]
 public string family
 {
  get { return ffamily; }
  set { ffamily = value; ffont = null; }
 }

 [XmlElement(ElementName = "size")]
 public float size
 {
  get { return fsize; }
  set { fsize = value; ffont = null; }
 }

 [XmlElement(ElementName = "unit")]
 public GraphicsUnit unit
 {
  get { return funit; }
  set { funit = value; ffont = null; }
 }

 [XmlElement(ElementName = "charset")]
 public byte charset
 {
  get { return fcharset; }
  set { fcharset = value; ffont = null; }
 }

 [XmlElement(ElementName = "vertical")]
 public bool vertical
 {
  get { return fvertical; }
  set { fvertical = value; ffont = null; }
 }

 public Font CreateFont()
 {
  return new Font(ffamily, fsize, fstyle, funit);
 }
}

// definition of a color / solid brush, this class can be serialised whereas brushes cannot
public class colordef
{
 protected byte fa = 255, fr = 255, fg = 255, fb = 255;
 protected Brush fbrush;
 protected Pen fpen;

 [XmlElement(ElementName = "alphablend")]
 public byte a
 {
  get { return fa; }
  set { fa = value; fbrush = null; }
 }

 [XmlElement(ElementName = "red")]
 public byte r
 {
  get { return fr; }
  set { fr = value; fbrush = null; }
 }

 [XmlElement(ElementName = "green")]
 public byte g
 {
  get { return fg; }
  set { fg = value; fbrush = null; }
 }

 [XmlElement(ElementName = "blue")]
 public byte b
 {
  get { return fb; }
  set { fb = value; fbrush = null; }
 }

 [XmlIgnoreAttribute]
 public Brush getsolidbrush
 {
  get { if (fbrush == null) fbrush = CreateSolidBrush(); return fbrush; }
 }

 [XmlIgnoreAttribute]
 public Pen getdefaultpen
 {
  get { if (fpen == null) fpen = CreateSolidPen(); return fpen; }
 }

 public SolidBrush CreateSolidBrush()
 {
  return new SolidBrush(Color.FromArgb(a, r, g, b));
 }

 public Pen CreateSolidPen()
 {
  return new Pen(getsolidbrush);
 }
}

public enum texteffecttype { none, fuzz, dropshadow }

// A class to define how text of a certain stylketype is to be drawn
[XmlType("textstyle")]
public class textstyle
{
 protected textstyletype fstyle;
 protected fontdefinition ffontdef = new fontdefinition();
 protected colordef fforeground = new colordef();
 protected colordef feffectcolour = new colordef();
 protected texteffecttype feffect = texteffecttype.none;
 protected float fdropshadowmagnification = 1; // 1 means that the shadow will have hard edges, higher numbers increase the fuzziness of the shadow but decrease the quality
 protected int fdropshadowoffset = 1; // how far the shadow is dropped in pixels
 protected float ffuzzmagnification = 1; // < 1 no fuzz. Otherise this is the scaling factor to use when making the fuzz. The higher the number the higher the amount of fuzz but the lowe the edge definition quality.
 protected int ffuzzwidth = 1; // width of line used when drawing the smaller image. if this is one and fuzzmagnification is one then there is no fuzz. If this is 2 and fuzzmagnification is 1 then there is a 1 pixel solid border. If this is 4 and fuzzmagnification is 2 then there will be a roughly 2 pixel border that is antialised into the background

 [XmlElement(ElementName = "style")]
 public textstyletype style
 {
  get { return fstyle; }
  set { fstyle = value; }
 }

 [XmlElement(ElementName = "font")]
 public fontdefinition fontdef
 {
  get { return ffontdef; }
  set { ffontdef = value; }
 }

 [XmlElement(ElementName = "effect")]
 public texteffecttype effect
 {
  get { return feffect; }
  set { feffect = value; }
 }

 [XmlElement(ElementName = "foregroundcolour")]
 public colordef foreground
 {
  get { return fforeground; }
  set { fforeground = value; }
 }

 [XmlElement(ElementName = "effectcolour")]
 public colordef effectcolour
 {
  get { return feffectcolour; }
  set { feffectcolour = value; }
 }

 [XmlElement(ElementName = "fuzzwidth")]
 public int fuzzwidth
 {
  get { return ffuzzwidth; }
  set { ffuzzwidth = value; }
 }

 [XmlElement(ElementName = "fuzzmagnification")]
 public float fuzzmagnification
 {
  get { return ffuzzmagnification; }
  set { ffuzzmagnification = value; }
 }

 [XmlElement(ElementName = "shadowoffset")]
 public int dropshadowoffset
 {
  get { return fdropshadowoffset; }
  set { fdropshadowoffset = value; }
 }

 [XmlElement(ElementName = "shadowmagnification")]
 public float dropshadowmagnification
 {
  get { return fdropshadowmagnification; }
  set { fdropshadowmagnification = value; }
 }

 public void setstyle(Graphics g)
 {
  //g.
 }
}

public enum underlineposition { none, bottom, under }
public enum underlineextents { line, layout, paragraph }

// A class to define how paragraphs of a certain styletype are to be drawn
[XmlType("paragraphstyle")]
public class paragraphstyle
{
 protected paragraphstyletype fstyle;
 protected paragraphjustification fjustification = paragraphjustification.justified;
 protected Single flinespacing = 1;
    protected underlineposition funderlining = underlineposition.none;
    protected underlineextents funderlinewhat = underlineextents.line;
    protected int funderlinewidth = 0;

 [XmlElement(ElementName = "style")]
 public paragraphstyletype style
 {
  get { return fstyle; }
  set { fstyle = value; }
 }

 [XmlElement(ElementName = "justification")]
 public paragraphjustification justification
 {
  get { return fjustification; }
  set { fjustification = value; }
 }

 [XmlElement(ElementName = "linespacing")]
 public Single linespacing
 {
  get { return flinespacing; }
  set { flinespacing = value; }
 }

 [XmlElement(ElementName = "underlining")]
 public underlineposition underlining
 {
     get { return funderlining; }
     set { funderlining = value; }
 }

 [XmlElement(ElementName = "underlinewidth")]
 public int underlinewidth
 {
     get { return funderlinewidth; }
     set { funderlinewidth = value; }
 }

 [XmlElement(ElementName = "underlinewhat")]
 public underlineextents  underlinewhat
 {
     get { return funderlinewhat; }
     set { funderlinewhat = value; }
 }
 
    public void setstyle(Graphics g)
 {
  //g.
 }
}

// text styles for an an article / layout element
public class textstyles : CollectionBase
{
 // this will do us a typed collection
 //Collection<articlestyle> fstyles = new Collection<articlestyle>();
 // this is an untyped hashtable
 Hashtable fstyles = new Hashtable();

 // overrides colectionbase.add, mainly for use by xmlserialiser
 public virtual void Add(textstyle ts)
 { 
  this.List.Add(ts);
  fstyles.Add(ts.style, ts);
 }

 // overrides colectionbase indexer, mainly for use by xmlserialiser
 public virtual textstyle this[int Index]
 {
  get { return (textstyle)this.List[Index]; } 
 }

 public textstyle style(textstyletype ast) 
 {
  return ((textstyle)fstyles[ast]);
 }
}

// paragraph styles for an article / layout element
public class paragraphstyles : CollectionBase
{
 // this will do us a typed collection
 //Collection<articlestyle> fstyles = new Collection<articlestyle>();
 // this is an untyped hashtable
 Hashtable fstyles = new Hashtable();

 // overrides colectionbase.add, mainly for use by xmlserialiser
 public virtual void Add(paragraphstyle ps)
 {
  this.List.Add(ps);
  fstyles.Add(ps.style, ps);
 }

 // overrides colectionbase indexer, mainly for use by xmlserialiser
 public virtual paragraphstyle this[int Index]
 {
  get { return (paragraphstyle)this.List[Index]; }
 }

 public paragraphstyle style(paragraphstyletype ast)
 {
  return ((paragraphstyle)fstyles[ast]);
 }
}

// a class to hold all the text and paragraph styles for an article / layout element
public class articlestyles
{
 protected textstyles ftext = new textstyles();
 protected paragraphstyles fparagraph = new paragraphstyles();

 [XmlArray(ElementName = "textstyles"),
  XmlArrayItem("textstyle")]
 public textstyles text
 {
  get { return ftext; }
  set { ftext = value; }
 }

 [XmlArray(ElementName = "paragraphstyles"),
  XmlArrayItem("paragraphstyle")]
 public paragraphstyles paragraph
 {
  get { return fparagraph; }
  set { fparagraph = value; }
 }
}
