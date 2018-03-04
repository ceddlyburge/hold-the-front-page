using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Specialized;

/// <summary>
/// Info on which page layout a user has chosen. A separate class to store this does seem like overkill but we might want to expand it later
/// </summary>
public class sessionpagelayout
{
 string fpagelayoutfilename;

 public string pagelayoutfilename
 {
  get { return fpagelayoutfilename; }
  set { fpagelayoutfilename = value; }
 }

 public void setcookie(HttpResponse Response)
 {
  XmlSerializer serializer = new XmlSerializer(typeof(sessionpagelayout));
  StringWriter tw = new StringWriter();
  serializer.Serialize(tw, this);
  HttpCookie cookie = new HttpCookie("pagelayout", HttpUtility.UrlEncode(tw.ToString()));
  cookie.Expires = DateTime.Now.AddYears(1);
  cookie.HttpOnly = true;
  Response.SetCookie(cookie);
 }

}

/// <summary>
/// Info stored in the session describing an article the user has chosen and the name of the articlelayout where it should be displayed
/// </summary>
public class sessionarticle
{
 protected string farticlelayoutname = "";
 protected string farticlefilename = "";

 public string articlelayoutname
 {
  get { return farticlelayoutname; }
  set { farticlelayoutname = value; }
 }

 public string articlefilename
 {
  get { return farticlefilename; }
  set { farticlefilename = value; }
 }
}

/// <summary>
/// Info stored in the session describing which articles a user has chosen to put where
/// </summary>
public class sessionarticles : CollectionBase
{
 protected Hashtable farticles = new Hashtable();

 public sessionarticle getarticle(string articlelayoutname)
 {
  return (sessionarticle)farticles[articlelayoutname];
 }

 public void addsessionarticle(string articlelayoutname, string filename)
 {
  sessionarticle sa = getarticle(articlelayoutname);
  if (sa == null)
  {
   sa = new sessionarticle();
   // must set layoutname before adding so it gets added to the hastable with the correct key
   sa.articlefilename = filename;
   sa.articlelayoutname = articlelayoutname;
   Add(sa);
  }
  else
  {
   sa.articlefilename = filename;
   sa.articlelayoutname = articlelayoutname;
  }
 }

 // overrides colectionbase.add, mainly for use by xmlserialiser
 public virtual void Add(sessionarticle sa)
 {
  this.List.Add(sa);
  farticles.Add(sa.articlelayoutname, sa);
 }

 // overrides colectionbase indexer, mainly for use by xmlserialiser
 public virtual sessionarticle this[int Index]
 {
  get { return (sessionarticle)this.List[Index]; }
 }

 public void setcookie(HttpResponse Response)
 {
  XmlSerializer serializer = new XmlSerializer(typeof(sessionarticles));
  StringWriter tw = new StringWriter();
  serializer.Serialize(tw, this);
  HttpCookie cookie = new HttpCookie("articles", HttpUtility.UrlEncode(tw.ToString()));
  cookie.Expires = DateTime.Now.AddYears(1);
  cookie.HttpOnly = true;
  Response.SetCookie(cookie);
 }
}

public class sessioncustomisation
{
 protected string fname;
 protected string fvalue;
 protected string farticle;
 protected bool finuse;

 public string name
 {
  get { return fname; }
  set { fname = value; }
 }

 public string value
 {
  get { return fvalue; }
  set { fvalue = value; }
 }

 public string article
 {
  get { return farticle; }
  set { farticle = value; }
 }

 public bool inuse
 {
  get { return finuse; }
  set { finuse = value; }
 }

}

public class sessioncustomisations : CollectionBase
{
 protected Hashtable fcustomisations = new Hashtable();

 public void setinactive()
 {
  foreach (sessioncustomisation sc in this)
  {
   sc.inuse = false;
  }
 }

 public sessioncustomisation getsessioncustomisation(string article, string name)
 {
  return (sessioncustomisation)fcustomisations[name.ToLower() + article.ToLower()];
 }

 public void addsessioncustomisation(string article, string name, string value, bool inuse, bool overwriteexistingvalue, bool findsimilar)
 {
  sessioncustomisation sc = getsessioncustomisation(article, name);

  if (sc == null)
  {
   // find a similar customisation from a different article if available
   // this will overwrite value if it is passed so could cause confusion
   if (findsimilar == true)
   {
    foreach (sessioncustomisation fssc in this)
    {
     if (fssc.name.ToLower() == name.ToLower()) { value = fssc.value; break; }
    }
   }

   // add this new customisation
   sc = new sessioncustomisation();
   sc.article = article;
   sc.name = name;
   sc.value = value;
   sc.inuse = inuse;
   Add(sc);
  }
  else
  {
   // not sure what this is for: sc.name = name;
   if (overwriteexistingvalue) sc.value = value;
   sc.inuse = inuse;
  }
 }

 // overrides colectionbase.add, mainly for use by xmlserialiser
 public virtual void Add(sessioncustomisation sc)
 {
  this.List.Add(sc);
  fcustomisations.Add(sc.name.ToLower() + sc.article.ToLower(), sc);
 }

 // overrides colectionbase indexer, mainly for use by xmlserialiser
 public virtual sessioncustomisation this[int Index]
 {
  get { return (sessioncustomisation)this.List[Index]; }
 }

 // returns an ienumerable to only go through the in use customisations
 public sessioncustomisationsinuseenumerator inuseonly()
 {
   return new sessioncustomisationsinuseenumerator(this);
 }

 public void setcookie(HttpResponse Response)
 {
  XmlSerializer serializer = new XmlSerializer(typeof(sessioncustomisations));
  StringWriter tw = new StringWriter();
  serializer.Serialize(tw, this);
  HttpCookie cookie = new HttpCookie("customisation", HttpUtility.UrlEncode(tw.ToString()));
  cookie.Expires = DateTime.Now.AddYears(1);
  cookie.HttpOnly = true;
  Response.SetCookie(cookie);
 }

 public string customise(string article, string name, string defaulttext)
 {
  sessioncustomisation sc = getsessioncustomisation(article, name);
  if (sc == null) return defaulttext; else return sc.value;
 }
}

// iterator to move through only the session customisations that are actually in use
public class sessioncustomisationsinuseenumerator : IEnumerator, IEnumerable 
{
 int index = -1;
 int inusecount;
 int inusetotal;
 sessioncustomisations fscs;

 public sessioncustomisationsinuseenumerator(sessioncustomisations scs)
 {
  fscs = scs;
  Reset();
 }

 public IEnumerator GetEnumerator()
	{
  return this; 
	}

 public void Reset()
 {
  index = -1;
  inusecount = 0;
  inusetotal = 0;

  foreach (sessioncustomisation sc in fscs)
  {
   if (sc.inuse == true) inusetotal++;
  }
 }

 public bool MoveNext()
 {
  index++;

  while ((index < fscs.Count - 1) && (fscs[index].inuse == false)) index++;

  if ((index < fscs.Count) && (fscs[index].inuse == true)) inusecount++;
  if ((inusecount > inusetotal) || (index >= fscs.Count)) return false;
  return true;
 }

 public Object Current
    {
        get
        {
            if (index < 0)
                throw new InvalidOperationException();
            if (index >= fscs.Count)
                throw new InvalidOperationException();
            return fscs[index];
        }
    }
}

/// <summary>
/// Info stored in the session describing a single picture chosen by the user
/// </summary>
public class sessionpicture
{
 protected string ffilename;
 protected string fthumbnailfilename;
 protected string fborderfilename;
 protected string fname;
 protected int fleft = 0;
 protected int ftop = 0;
 protected Single fscale = 1;
 protected bool fhaspicture = false;
 //protected int fright = 0;
 //protected int fbottom = 0;

 // origimal image dimesions so that off layout panning and zooming can be checked easily
 int fimagewidth;
 int fimageheight;
 int fthumbnailwidth;
 int fthumbnailheight;

 public int left
 {
  get { return fleft; }
  set { fleft = value; }
 }

 public int top
 {
  get { return ftop; }
  set { ftop = value; }
 }

 public Single scale
 {
  get { return fscale; }
  set { fscale = value; }
 }
 
 /* public int right
  {
   get { return fright; }
   set { fright = value; }
  }

  public int bottom
  {
   get { return fbottom; }
   set { fbottom = value; }
  }
  */
 public int imagewidth
 {
  get { return fimagewidth; }
  set { fimagewidth = value; }
 }

 public int imageheight
 {
  get { return fimageheight; }
  set { fimageheight = value; }
 }

 public int thumbnailwidth
 {
  get { return fthumbnailwidth; }
  set { fthumbnailwidth = value; }
 }

 public int thumbnailheight
 {
  get { return fthumbnailheight; }
  set { fthumbnailheight = value; }
 }

 public string filename
 {
  get { return ffilename; }
  set 
  { 
   ffilename = value;
   if (ffilename == "") haspicture = false; else haspicture = true;
  }
 }

 public string borderfilename
 {
  get { return fborderfilename; }
  set { fborderfilename = value; }
 }

 public string thumbnailfilename
 {
  get { return fthumbnailfilename; }
  set { fthumbnailfilename = value; }
 }

 public string name
 {
  get { return fname; }
  set { fname = value; }
 }

 public bool haspicture
 {
  get { return fhaspicture; }
  set { fhaspicture = value; }
 }
}

public class sessionpictures : CollectionBase
 {
  //protected Collection<sessionpicture> fpictures = new Collection<sessionpicture>();
  // this is an untyped hashtable
  Hashtable fpictures = new Hashtable();

  // overrides colectionbase.add, mainly for use by xmlserialiser
  public virtual void Add(sessionpicture sp)
  {
   this.List.Add(sp);
   fpictures.Add(sp.name, sp);
  }

  // overrides colectionbase indexer, mainly for use by xmlserialiser
  public virtual sessionpicture this[int Index]
  {
   get { return (sessionpicture)this.List[Index]; }
  }

  public sessionpicture picture(string name)
  {
   return ((sessionpicture)fpictures[name]);
  }

  public void setcookie(HttpResponse Response)
  {
  XmlSerializer serializer = new XmlSerializer(typeof(sessionpictures));
  StringWriter tw = new StringWriter();
  serializer.Serialize(tw, this);
  HttpCookie cookie = new HttpCookie("pictures", HttpUtility.UrlEncode(tw.ToString()));
  cookie.Expires = DateTime.Now.AddYears(1);
  cookie.HttpOnly = true;
   Response.SetCookie(cookie); 
  }

  //[XmlArray("pictures"), XmlArrayItem("picture")]
  //public Collection<sessionpicture> pictures
  //{
  // get { return fpictures ; }
  //}
 }



