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
using System.Collections.Specialized;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Xml;

/// <summary>
/// Summary description for article
/// </summary>

public enum articleparttype { headline, subhead, strap, extra, body };

public enum textcapitalisation { allupper, alllower, firstletter, none };

// text styles for an an article / layout element
public class articleparttypes : CollectionBase
{
 // this will do us a typed collection
 //Collection<articlestyle> fstyles = new Collection<articlestyle>();
 // this is an untyped hashtable
 Hashtable farticleparts = new Hashtable();

 // overrides colectionbase.add, mainly for use by xmlserialiser
 public virtual void Add(articleparttype apt)
 {
  this.List.Add(apt);
  farticleparts.Add(apt, apt);
 }

 // overrides colectionbase indexer, mainly for use by xmlserialiser
 public virtual articleparttype this[int Index]
 {
  get { return (articleparttype)this.List[Index]; }
 }

 public bool contains(articleparttype apt)
 {
  return farticleparts.ContainsKey(apt);
 }
}

// class to store variables that are useful when measuring and drawing headlines with squeezing and stretching logic.
// want one of these for each word
public class headlinewordhelper
{
 // characers in this word
 public char[] characters;
 // character actual widths
 public Collection<int> widths = new Collection<int>();
 // max / min widths after squeezing / stretching
 public Collection<int> minwidths = new Collection<int>();
 public Collection<int> maxwidths = new Collection<int>();
 // actual kerning between characters (one less item than widths)
 public Collection<int> kernings = new Collection<int>();
 // max / min kerning after squeezing / stretching
 public Collection<int> minkernings = new Collection<int>();
 public Collection<int> maxkernings = new Collection<int>();
 // positions of characters
 public Collection<int> xpositions = new Collection<int>();

 public int mintotalwidth()
 {
  int r = 0;

  for (int i = 0; i < characters.Length; i++)
  {
   r = r + minwidths[i];
  }

  for (int i = 0; i < kernings.Count; i++)
  {
   r = r + minkernings[i];
  }

  return r;
 }

 public int normalwidth()
 {
  int r = 0;

  for (int i = 0; i < characters.Length; i++)
  {
   r = r + widths[i];
  }

  for (int i = 0; i < kernings.Count; i++)
  {
   r = r + kernings[i];
  }

  return r;
 }
 
 public int mintotalwidthkerning()
 {
  int r = 0;

  for (int i = 0; i < characters.Length; i++)
  {
   r = r + widths[i];
  }

  for (int i = 0; i < kernings.Count; i++)
  {
   r = r + minkernings[i];
  }

  return r;
 }

 public int maxtotalwidth()
 {
  int r = 0;

  for (int i = 0; i < characters.Length; i++)
  {
   r = r + maxwidths[i];
  }

  for (int i = 0; i < kernings.Count; i++)
  {
   r = r + maxkernings[i];
  }

  return r;
 }

 public int maxtotalwidthkerning()
 {
  int r = 0;

  for (int i = 0; i < characters.Length; i++)
  {
   r = r + widths[i];
  }

  for (int i = 0; i < kernings.Count; i++)
  {
   r = r + maxkernings[i];
  }

  return r;
 }

}

public class headlinehelperline
{
 public Collection<headlinewordhelper> words = new Collection<headlinewordhelper>();
 public int mintotalwidthkerning;
 public int mintotalwidth;
 public int maxtotalwidthkerning;
 public int maxtotalwidth;
 public int normaltotalwidth;
 // percentage of total space we can change through kerning that we want to use
 public double kerningsqueeze = 0;
 public double kerningstretch = 0;
 // percentage of total space we can change through letter stretching / squashing that we want to use
 public double lettersqueeze = 0;
 public double letterstretch = 0;
}

public class headlinehelperlines
{
 public Collection<headlinehelperline> lines = new Collection<headlinehelperline>();
 public int minspacewidth;
 public int maxspacewidth;
 public double maxkerningstretch;
 public double maxkerningsqueeze;
 public double maxletterstretch;
 public double maxlettersqueeze;

 public void calcmaxsquashingandstretching()
 {
  maxkerningstretch = 0;
  maxkerningsqueeze = 0;
  maxletterstretch = 0;
  maxlettersqueeze = 0;

  for (int i = 0; i < lines.Count; i++)
  {
   if (lines[i].kerningsqueeze > maxkerningsqueeze) maxkerningsqueeze = lines[i].kerningsqueeze;
   if (lines[i].kerningstretch > maxkerningstretch) maxkerningstretch = lines[i].kerningstretch;
   if (lines[i].lettersqueeze > maxlettersqueeze) maxlettersqueeze = lines[i].lettersqueeze;
   if (lines[i].letterstretch > maxletterstretch) maxletterstretch = lines[i].letterstretch;
  }
 }
}


// a piece of text that forms part of a paragraph
public abstract class sometext
{
 protected textstyletype fstyle;
 protected abstract string gettext();
 protected String[] fwords;
 protected Collection<int> fwidths = new Collection<int>();
 protected Collection<int> fxpositions = new Collection<int>();
 protected Collection<int> fypositions = new Collection<int>();
 protected Collection<headlinewordhelper> fheadlinehelpers = new Collection<headlinewordhelper>();
 protected int fspacewidth;
 protected bool fstartdeck;


 [XmlIgnoreAttribute]
 public string text
 {
  get { return gettext(); }
 }

 [XmlIgnoreAttribute]
 public int spacewidth
 {
  get { return fspacewidth; }
  set { fspacewidth = value; }
 }

 // fill the words array from the text property and size the measuring
 // collections.
 public void split()
 {
  //Regex r = new Regex("\s");
  //fwords = Regex.Split(text, @"\s", RegexOptions.Compiled);
     fwords = text.Replace("\n", " ").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
  for (int i = 0; i < fwords.Length; i++) fwords[i].Trim();

  fwidths.Clear();
  for (int i = 0; i < fwords.Length; i++) fwidths.Add(0);

  fxpositions.Clear();
  for (int i = 0; i < fwords.Length; i++) fxpositions.Add(0);

  fypositions.Clear();
  for (int i = 0; i < fwords.Length; i++) fypositions.Add(0);
 }

 [XmlElement(ElementName = "style")]
 public textstyletype style
 {
  get { return fstyle; }
  set { fstyle = value; }
 }

 [XmlElement(ElementName = "startdeck")]
 public bool startdeck
 {
  get { return fstartdeck; }
  set { fstartdeck = value; }
 }
 
 [XmlIgnoreAttribute]
 public String[] words
 {
  get { return fwords; }
 }

 [XmlIgnoreAttribute]
 public Collection<int> widths
 {
  get { return new Collection<int>(fwidths); }
 }

 [XmlIgnoreAttribute]
 public Collection<headlinewordhelper> headlinehelpers
 {
  get { return new Collection<headlinewordhelper>(fheadlinehelpers); }
 }

 [XmlIgnoreAttribute]
 public Collection<int> xpositions
 {
  get { return fxpositions; }
 }

 [XmlIgnoreAttribute]
 public Collection<int> ypositions
 {
  get { return fypositions; }
 }

  // customises customisable text in the article with the passed customisations
 public virtual void customise(string article, sessioncustomisations sc)
 {
 }

 // adds customisable items to the session for the user to change
 public virtual void addcustomisables(string article, sessioncustomisations sc)
 {
 }

}

// a piece of text that cannot be changed by the user
public class sometextstatic : sometext
{
 protected string fstatictext = "";

 [XmlElement(ElementName = "text")]
 public string statictext
 {
  get { return fstatictext; }
  set { fstatictext = value; }
 }

 protected override string gettext()
 {
  return fstatictext;
 }
}

// a piece of text that can be changed by the user
public class sometextreplace : sometext
{
 // maximum length of string (0 is unlimiited)
 protected int fmaxlength = 0;
 // initial value before user changes
 protected string finitialtext = "[enter something here ...]";
 // changed / current value
 protected string fcurrenttext = "";
 // prompt to show to user when asking for a new value of this piece of text
 protected string fprompt = "Enter something here";
 protected textcapitalisation fcapitalise = textcapitalisation.none;

 protected override string gettext()
 {
  if (capitalise == textcapitalisation.none)
  {
   return fcurrenttext;
  }
  else if (capitalise == textcapitalisation.allupper)
  {
   return fcurrenttext.ToUpper();
  }
  else if (capitalise == textcapitalisation.alllower)
  {
   return fcurrenttext.ToLower();
  }
  else if (capitalise == textcapitalisation.firstletter)
  {
   return char.ToUpper(fcurrenttext[0]) + fcurrenttext.Substring(1).ToLower();
  }
  else
  {
   throw new Exception("Replace Text Prompt: " + fprompt + " textcapitalisation not recognised."); 
  }

 }

 [XmlElement(ElementName = "defaulttext")]
 public string initialtext
 {
  get { return finitialtext; }
  set { 
   finitialtext = value; 
   if (currenttext == "") currenttext = value;
  }
 }

 [XmlIgnoreAttribute]
 public string currenttext
 {
  get { return fcurrenttext; }
  set { fcurrenttext = value; }
 }

 [XmlElement(ElementName = "maxlength")]
 public int maxlength
 {
  get { return fmaxlength; }
  set { fmaxlength = value; }
 }

 [XmlElement(ElementName = "prompt")]
 public string prompt
 {
  get { return fprompt; }
  set { fprompt = value; }
 }

 [XmlElement(ElementName = "capitalise")]
 public textcapitalisation capitalise
 {
  get { return fcapitalise; }
  set { fcapitalise = value; }
 }

 // customises customisable text in the article with the passed customisations
 public override void customise(string article, sessioncustomisations sc)
 {
  fcurrenttext = sc.customise(article, prompt, finitialtext);
 }

 // adds customisable items to the session to be filled in by the user
 public override void addcustomisables(string article, sessioncustomisations sc)
 {
  sc.addsessioncustomisation(article, prompt, finitialtext, true, false, true);
 }

}


// a paragraph (no line breaks) to be shown as part of an article
public class paragraph
{
 protected bool foptional = false;
 protected bool fdraw;
 protected paragraphstyletype fstyle;
 protected Collection<sometext> ftextparts = new Collection <sometext>();
 protected int fdecks;
 // height of the text at a given width, to be calculated elsewhere
 protected int fheight;
 // maximum right position of paragraph, calculated elsewhere
 public int maxright;
 // minimum left position of paragraph, calculated elsewhere
 public int minleft;
 // used when justifying with the headline style which sizes and draws each letter
 // individually
 public headlinehelperlines headlinelines;

 [XmlElement(ElementName = "optional")]
 public bool optional
 {
  get { return foptional; }
  set { foptional = value; }
 }

 [XmlIgnoreAttribute]
 public bool draw
 {
  get { return fdraw; }
  set { fdraw = value; }
 }

 [XmlIgnoreAttribute]
 public int decks
 {
  get { return fdecks; }
 }

 [XmlElement(ElementName = "style")]
 public paragraphstyletype style
 {
  get { return fstyle; }
  set { fstyle = value; }
 }

 [XmlIgnoreAttribute]
 public int height
 {
  get { return fheight; }
  set { fheight = value; }
 }

 [XmlArrayItem(Type = typeof(sometextstatic), ElementName = "statictext"),
  XmlArrayItem(Type = typeof(sometextreplace), ElementName = "replacetext"),
   XmlArrayItem("textparts")]
 public Collection<sometext> textparts
 {
  get { return ftextparts; }
 }

 public paragraph()
 {
 }

 public void countdecks()
 {
  fdecks = 1;
  for (int i = 1; i < textparts.Count; i++)
  {
   if (textparts[i].startdeck == true) fdecks++;
  }
 }

  // customises customisable text in the article with the passed customisations
 public void customise(string article, sessioncustomisations sc)
 {
  foreach (sometext st in textparts)
  {
   st.customise(article, sc);
  }
 }

 // add customisable items to the session for the user to fill in
 public void addcustomisables(string article, sessioncustomisations sc)
 {
  foreach (sometext st in textparts)
  {
   st.addcustomisables(article, sc);
  }
 }

}

public class sortparagrapsbyheadline : IComparer<paragraph>
{
 // paragraphs sorter by height
 public int Compare(paragraph p1, paragraph p2)
 {
  // this assumes that the max spacings and squashings have already been worked out. It is quicker to do it once before sorting than for every compare
  // paragraphs sorted by height (ascending) and then in order of preference with respect to the amount of squashing and stretching that had to be done (descending)
  if (p1.height != p2.height) return p1.height.CompareTo(p2.height);
  else if (p1.headlinelines.maxletterstretch != p2.headlinelines.maxletterstretch) return p2.headlinelines.maxletterstretch.CompareTo(p1.headlinelines.maxletterstretch);
  else if (p1.headlinelines.maxlettersqueeze != p2.headlinelines.maxlettersqueeze) return p2.headlinelines.maxlettersqueeze.CompareTo(p1.headlinelines.maxlettersqueeze);
  else if (p1.headlinelines.maxkerningstretch != p2.headlinelines.maxkerningstretch) return p2.headlinelines.maxkerningstretch.CompareTo(p1.headlinelines.maxkerningstretch);
  else if (p2.headlinelines.maxkerningsqueeze != p1.headlinelines.maxkerningsqueeze) return p2.headlinelines.maxkerningsqueeze.CompareTo(p1.headlinelines.maxkerningsqueeze);
  else if (p2.maxright != p1.maxright) return p1.maxright.CompareTo(p2.maxright);
  else return 0;
   }
}

// one bit of an article
public class articlepart
{
 protected articleparttype fparttype;
 // collection of compulsory and optional paragraphs potentially with
 // replaceabletext
 protected Collection<paragraph> fparagraphs = new Collection<paragraph>();
 // collection of paragraphs that all say the same thing using a 
 // different number of words / letters. We use the one that fits best
 // in the space available.
 protected List<paragraph> ffinetune = new List<paragraph>();
 // used by the drawing routines
 [XmlIgnoreAttribute]
 public int finetuneminheight;
 [XmlIgnoreAttribute]
 public int finetunemaxheight;

 [XmlElement(ElementName = "content")]
 public articleparttype parttype
 {
  get { return fparttype; }
  set { fparttype = value; }
 }

 [XmlArray("paragraphs"), XmlArrayItem("paragraph")]
 public Collection<paragraph> paragraphs
 {
  get { return fparagraphs; }
 }

 [XmlArray("chooseoneparagraphs")]
 public List<paragraph> finetune
 {
  get { return ffinetune; }
 }

  // customises customisable text in the article with the passed customisations
 public void customise(string article, sessioncustomisations sc)
 {
  foreach (paragraph p in fparagraphs)
  {
   p.customise(article, sc);
  }

  foreach (paragraph p in ffinetune)
  {
   p.customise(article, sc);
  }
 }

 // add custimisable items to the session object to show to the user and ask for new values
 public void addcustomisables(string article, sessioncustomisations sc)
 {
  foreach (paragraph p in fparagraphs)
  {
   p.addcustomisables(article, sc);
  }

  foreach (paragraph p in ffinetune)
  {
   p.addcustomisables(article, sc);
  }
 }

}

// a full article, containing all the (potenitally displayed separately) bits of the article.
[XmlType("article")]
public class article : CollectionBase
{
 Hashtable fparts = new Hashtable();
 public int totalheight; // used by drawing classes
 public int maxright;
 public int minleft;

 // overrides colectionbase.add, mainly for use by xmlserialiser
 public virtual void Add(articlepart ap)
 {
  fparts.Add(ap.parttype, ap);
  this.List.Add(ap);
 }

 // overrides colectionbase indexer, mainly for use by xmlserialiser
 [XmlArrayItem("articlepart")]
 public virtual articlepart this[int Index]
 {
  get { return (articlepart)this.List[Index]; }
 }

 // customises customisable text in the article with the passed customisations
 public void customise(string article, sessioncustomisations sc)
 {
  if (sc == null) return;

  foreach (articlepart ap in List)
  {
   ap.customise(article, sc);
  }
 }

 // adds items to sc for all customisable items
 public void addcustomisables(string article, sessioncustomisations sc)
 {
  if (sc == null) return;

  foreach (articlepart ap in List)
  {
   ap.addcustomisables(article, sc);
  }
 }

 // used within the code to get an article part for the specified part type
 public articlepart style(articleparttype apt)
 {
  return ((articlepart)fparts[apt]);
 }
}
