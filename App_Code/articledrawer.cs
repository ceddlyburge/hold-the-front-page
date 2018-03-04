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
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using System.Xml;
using System.IO;


/// <summary>
/// Helper class for drawing many articles
/// </summary>
public class articlesdrawer
{
 public static Bitmap drawarticles(pagelayout pl, sessionarticles sas,sessioncustomisations sc, sessionpictures sps, bool protectoptionalpicturespace) 
 {
  // get the background image
  Bitmap barticles = new Bitmap(pl.backgroundwidth, pl.backgroundheight);
  Graphics garticles = Graphics.FromImage(barticles);
  article a;
  articledrawer drawer;
  sessionarticle sa;
  
  // loop through articlelayouts in the page layout and draw any matching articles that we have
  drawer = new articledrawer();
  for (int i = 0; i < pl.articles.Count; i++)
  {
   if (sas != null)
   {
    // get the sessionarticle for this articlelayout
    sa = sas.getarticle(pl.articles[i].name);
    if (sa != null)
    {
     // load the article
     a = ceddio.loadarticle(sa.articlefilename);
     if (a != null)
     {
      if (sc != null) a.customise(sa.articlefilename, sc);
      drawer.drawarticle(garticles, a, pl.articles[i], sps, protectoptionalpicturespace);
     }
    }
   }
  }

  return barticles;
 }
}

/// <summary>
/// Class to handle the drawing of an article
/// </summary>
public class articledrawer
{
 protected articlestyles fstyle;
 protected Graphics fgrapher;
 protected PointF zerozero = new PointF(0, 0);
 protected PointF twentytwenty = new PointF(20, 20);
 protected Matrix underlinematrix = new Matrix();
 protected Matrix minustwentytwenty = new Matrix();

 public articledrawer()
	{
  minustwentytwenty.Translate(-20, -20);
	}

 public articlestyles styles
 {
  get { return fstyle; }
  set { fstyle = value; }
 }

 protected Graphics grapher
 {
  get { return fgrapher; }
  set { fgrapher = value; }
 }

 // measure the words in a piece of same style text
 protected void measuretext(sometext st)
 {
  textstyle s = styles.text.style(st.style);
  // need to measure each word individually to work out paragraph heights
  // this is because the style can change within the paragraph so windows
  // can't do it for us. We could check for style changes I guess but may
  // just do it all ourselves.
  st.split();
  // the GenericTypographic seems not to measure the space so we use
  // a dot instead. ANything other type of string format leaves a lot
  // of space either side
  st.spacewidth = Convert.ToInt16(Math.Ceiling(grapher.MeasureString(".", s.fontdef.getfont, zerozero , StringFormat.GenericTypographic).Width));

  for (int i = 0; i < st.words.Length; i++)
  {
   // if font.height doesn't work for us then we can measure it here
   st.widths[i] = (int)Math.Ceiling(grapher.MeasureString(st.words[i], s.fontdef.getfont, zerozero, StringFormat.GenericTypographic).Width);
   //GraphicsPath ph = new GraphicsPath();
   //ph.AddString(st.words[i], s.fontdef.getfont.FontFamily, (int)s.fontdef.getfont.Style, (s.fontdef.getfont.SizeInPoints / 72.0f) * grapher.DpiY, twentytwenty, StringFormat.GenericDefault);
   //st.widths[i] = (int)Math.Ceiling(ph.GetBounds().Width); 
  }
 }

 // do the special measuring for a headline that wants to space and squash letters so that all words are fully justified even if there is only one word per line.
 // this measuring only uses the chooseoneparagraphs, if there is other text in the article part then an exception is raised. This shouldn't be the case logically as the headline should only be one line.
 // we want the largest chooseoneparagraph that fits with the minimum of squahsing / stretching

 
/* 
 cedd
 dont think we need this procedure
 might need to as choosing the correct chooseoneparagraph is a bit tricker.
 don't only want to pick on length we also want to pick on how squased
 everything is. Not sure if this will practically be a problem as there may not be two headlines so similar that they take up the same space.
 protected void measureheadlinelayoutpart(article a, articlelayoutpart al)
 {
  // just one set of choose one paragraphs is what we are after
  if (a.Count != 1) notifydrawerror("Headline layouts can only have one article part. Please reformat the layout", a, al);
  if (a[0].paragraphs.Count > 0) notifydrawerror("Headline layouts can only have one article part which can only contain fine tuning paragraphs. Please reformat the article / layout", a, al);

  // for each paragraph
  // record if it doesn't fit
  // work out how much stretching is required (want as little as possible)
  // work out how much squashing is required (want as little as possible but less important than stretching)
  // work out how close to the layout width the actual width is. Not sure how important this is yet.

  // measure the words and characters within the paragraph
  for (int i = 0; i < a[0].finetune.Count; i++)
  {
   if (a[0].finetune[i].height <= al.textheight)
   {
    measureheadlineparagraph(a[0].finetune[i], al.textwidth);
   }
  }
 }
 */

 // measure all the text in a paragraph
 // going to assume that text is all in the same style here so we don't have to worry about baseline descent and suchlikt
 protected void measureheadlineparagraph(paragraph p, int width, int layoutheight)
 {
  // count the number of decks and work out the height. If this is the right number of decks then carry on with the measuring
  p.countdecks();
  int onedeck = (int)(styles.text.style(p.textparts[0].style).fontdef.getfont.Height * styles.paragraph.style(p.style).linespacing);
  p.height = p.decks * onedeck;
  if ((p.height > layoutheight) || (p.height + onedeck < layoutheight))
  {
   p.height = int.MaxValue;
   return;
  }

  p.headlinelines = new headlinehelperlines();
 
  // measure the individual words and characters within the string. Calculate the kernings and the max and min that we can squash / stretch by.
  for (int i = 0; i < p.textparts.Count; i++)
  {
   measuretext(p.textparts[i]); // still use this to split the words and suchlike
   measureheadlinetext(p.textparts[i]);
  }

  // work out the min space width. we are saying that it is the same for all the text parts in the paragraph
  p.headlinelines.minspacewidth = getminspacewidth(styles.text.style(p.textparts[0].style).fontdef.getfont, p.textparts[0].spacewidth);
  p.headlinelines.maxspacewidth = getmaxspacewidth(styles.text.style(p.textparts[0].style).fontdef.getfont, p.textparts[0].spacewidth);

  // position the words onto the lines, use a pessimistic method using maximum squashing to see which words will fit on which line. Then expand as near as possible to the normal width for all lines
  positionheadlinewords(p, width);
  
  // can then return the thinnest (smallest width) line to its normal size (or to the size of the layout width if smaller. Other lines then squash to this value.
  positionheadlineletters(p, width);
 }

 protected void positionheadlineletters(paragraph p, int width)
 {
  int linepositionwidth, headlinewidth;
  headlinehelperline smallestline;
  int maxminwidth = int.MinValue;
  int minmaxwidth = int.MaxValue;
  int minnormalwidth = int.MaxValue;
  int smallestlinewidth;

  // allow smallest line (not the first?) to not line up exactly and be a little smaller
  smallestline = null; // avoid compiler error
  smallestlinewidth = p.headlinelines.lines[0].normaltotalwidth;
  for (int i = 1; i < p.headlinelines.lines.Count; i++)
  {
   if (p.headlinelines.lines[i].normaltotalwidth < smallestlinewidth)
   {
    smallestline = p.headlinelines.lines[i];
    smallestlinewidth = p.headlinelines.lines[i].normaltotalwidth;
   }
  }
  
  // first find extents of the lines
  for (int i = 0; i < p.headlinelines.lines.Count; i++)
  {
   if (p.headlinelines.lines[i] == smallestline) continue;

   if (p.headlinelines.lines[i].mintotalwidth > maxminwidth) maxminwidth = p.headlinelines.lines[i].mintotalwidth;
   if (p.headlinelines.lines[i].maxtotalwidth < minmaxwidth) minmaxwidth = p.headlinelines.lines[i].maxtotalwidth;
   if (p.headlinelines.lines[i].normaltotalwidth < minnormalwidth)minnormalwidth = p.headlinelines.lines[i].normaltotalwidth;
  }

  // first of all look at a width that requires no stretching. See if the min widths and normal widths. Check to see if there is a common width range that the layout width is within or above. If so take the maximum of this range to squash as little as possible as long as it <= the layout width.
  if ((maxminwidth <= minnormalwidth) && (maxminwidth <= width))
  {
   headlinewidth = Math.Min(minnormalwidth, width);
  }
  // if this hasn't worked then look at the min widths and max widths. If there is a comman range and the layout width is within orabove this then use the smallest width in the range.
  else if ((maxminwidth <= minmaxwidth) && (maxminwidth <= width))
  {
   headlinewidth = Math.Min(maxminwidth, width);
  }
  else
  {
   // this isn't going to fit!
   p.height = int.MaxValue;
   return;
  }

  // update left and right extents of this paragraph
  p.minleft = 0;
  p.maxright = headlinewidth;

  // now squash / stretch all the lines to this width. this will set the positions of all the characters on the line for easy drawing later
  for (int i = 0; i < p.headlinelines.lines.Count; i++)
  {
   if (p.headlinelines.lines[i] == smallestline) linepositionwidth = p.headlinelines.lines[i].normaltotalwidth; else linepositionwidth = headlinewidth;
   scalelinetowidth(p, p.headlinelines.lines[i], linepositionwidth, p.headlinelines.minspacewidth, p.headlinelines.maxspacewidth, p.textparts[0].spacewidth);
  }
 }

 protected void scalelinetowidth(paragraph p, headlinehelperline line, int width, int minspacewidth, int maxspacewidth, int spacewidth)
 {
  int spacediff = 0;
  double x;
  
  // howmuch space do we need to lose / gain?
  spacediff = width - line.normaltotalwidth;
    
  // nothing to do
  if (spacediff == 0)
  {
  }
  // need to make smaller
  else if (spacediff < 0)
  {
   // can we do this solely with kerning and word spacing?
   // if so then do it this way
   if (line.mintotalwidthkerning <= width)
   {
    // specify percentage of max kerning that we want to use
    line.kerningsqueeze = -(double)spacediff / (double)(line.normaltotalwidth - line.mintotalwidthkerning);
   }
   else
   {
    // otherwise max out the kerning and word spacing and add as much letter squashing as is necessary
    // specify max kerning
    line.kerningsqueeze = 1.0f;
    // specify amount of letter squashing required
    line.lettersqueeze = (double)(width - line.mintotalwidthkerning) / (double)(line.mintotalwidth - line.mintotalwidthkerning);
   }
  }
  else
  {
   // if we need to stretch then by how much
   // can we do this solely with kerning and word spacing?
   // if so then do it this way
   if (line.maxtotalwidthkerning > width)
   {
    // specify percentage of max kerning that we want to use
    line.kerningstretch = (double)spacediff / (double)(line.maxtotalwidthkerning - line.normaltotalwidth);
   }
   else
   {
    // otherwise max out the kerning and word spacing and add as much letter squashing as is necessary
    // specify max kerning
    line.kerningstretch = 1.0f;
    // specify amount of letter squashing required
    line.letterstretch = (double)(width - line.maxtotalwidthkerning) / (double)(line.maxtotalwidth - line.maxtotalwidthkerning);
   }
  }

  // now work out where to draw each letter, we have percentage of max kerning and squashing to do so just apply this
  x = 0;
  for (int i = 0; i < line.words.Count; i++)
  {
   for (int j = 0; j < line.words[i].characters.Length; j++)
   {
    // position this letter
    line.words[i].xpositions[j] = (int)Math.Round(x);

    // add kerning 
    // use percentage of max possible kerning 
    if (j < line.words[i].kernings.Count)
    {
     if (spacediff <= 0)
     {
      x += (double)line.words[i].kernings[j] - ((double)(line.words[i].kernings[j] - line.words[i].minkernings[j]) * line.kerningsqueeze);
     }
     else
     {
      x += (double)line.words[i].kernings[j] + ((double)(line.words[i].maxkernings[j] - line.words[i].kernings[j]) * line.kerningstretch);
     }
    }

    // add letter width, use percentage of max possible stretching / squashing
    if (spacediff <= 0)
    {
     x += (double)line.words[i].widths[j] - ((double)(line.words[i].widths[j] - line.words[i].minwidths[j]) * line.lettersqueeze);
    }
    else
    {
     x += (double)line.words[i].widths[j] + ((double)(line.words[i].maxwidths[j] - line.words[i].widths[j]) * line.letterstretch);
    }
   }

   // if there is another word then a gap for the space
   // there is a space between words so take account of this
   // the size is done in the same way as the kerning
   if (i < line.words.Count - 1)
   {
    if (spacediff <= 0)
    {
     x += (double)spacewidth - ((double)(spacewidth - minspacewidth) * line.kerningsqueeze);
    }
    else
    {
     x += (double)spacewidth + ((double)(maxspacewidth - spacewidth) * line.kerningstretch);
    }
   }
  }

  int maxr = (int)Math.Ceiling(x); 
  if (x > p.maxright) p.maxright = maxr;
 }

 protected void positionheadlinewords(paragraph p, int width)
 {
  int xposminkerning;
  int xposmin;
  int xpos;
  int xposmax;
  int xposmaxkerning;
  // loop words within the text
  headlinehelperline curline = new headlinehelperline();
  p.headlinelines.lines.Add(curline);

  // remember position in line of words
  xpos = 0;
  xposmin = 0;
  xposminkerning = 0;
  xposmax = 0;
  xposmaxkerning = 0;
  for (int i = 0; i < p.textparts.Count; i++)
  {
   // start a new deck if asked to do so. Ignore the start deck property of the first part as we are already on a line at this point
   if ((p.textparts[i].startdeck == true) && (i != 0))
   {
    curline = new headlinehelperline();
    p.headlinelines.lines.Add(curline);

    // move this word onto the next line
    xposmaxkerning = 0;// p.textparts[i].headlinehelpers[j].maxtotalwidthkerning();
    xposmax = 0;// p.textparts[i].headlinehelpers[j].maxtotalwidth();
    xposminkerning = 0;// p.textparts[i].headlinehelpers[j].mintotalwidthkerning();
    xposmin = 0;// p.textparts[i].headlinehelpers[j].mintotalwidth();
    xpos = 0;// p.textparts[i].widths[j];

    // if this word is bigger than the line then stop measuring and say that this paragraph won't fit here
    //if (xposmin > width)
    //{
    // p.height = int.MaxValue;
    // return;
    //}
   }

   for (int j = 0; j < p.textparts[i].widths.Count; j++)
   {
    // set xposition for this word for a left justified paragraph
    if (xposmin > 0)
    {
     xposmaxkerning += p.headlinelines.maxspacewidth;
     xposmax += p.headlinelines.maxspacewidth;
     xposminkerning += p.headlinelines.minspacewidth;
     xposmin += p.headlinelines.minspacewidth;
     xpos += p.textparts[i].spacewidth;
    }
    xposmaxkerning += p.textparts[i].headlinehelpers[j].maxtotalwidthkerning();
    xposmax += p.textparts[i].headlinehelpers[j].maxtotalwidth();
    xposminkerning += p.textparts[i].headlinehelpers[j].mintotalwidthkerning();
    xposmin += p.textparts[i].headlinehelpers[j].mintotalwidth();
    xpos += p.textparts[i].widths[j]; 
     
    // update line variables. 
    curline.mintotalwidth = xposmin;
    curline.mintotalwidthkerning = xposminkerning;
    curline.normaltotalwidth = xpos;
    curline.maxtotalwidth = xposmax;
    curline.maxtotalwidthkerning = xposmaxkerning;

    // add this word to the current line
    curline.words.Add(p.textparts[i].headlinehelpers[j]);
   }
  }

  // work out height
  //p.height = (int)Math.Ceiling(p.headlinelines.lines.Count * styles.text.style(p.textparts[0].style).fontdef.getfont.Height * styles.paragraph.style(p.style).linespacing); 
 }

 // measure the words in a piece of same style text
 protected void measureheadlinetext(sometext st)
 {
  st.headlinehelpers.Clear();
  for (int i = 0; i < st.words.Length; i++)
  {
   measureheadlineword(st.words[i], st);
   // change the width of this word as measured more accurately by us
   st.widths[i] = st.headlinehelpers[i].normalwidth(); 
  }
 }

 protected void measureheadlineword(String word, sometext st)
 {
  headlinewordhelper hh = new headlinewordhelper();
  textstyle s = styles.text.style(st.style);
  st.headlinehelpers.Add(hh);
  GraphicsPath ph = new GraphicsPath();
  
  hh.characters = new char[word.Length];
  
  for (int i = 0; i < word.Length; i++)
  {
   hh.characters[i] = word[i];

   // measure this character. Using graphics path for decent accuracy sizing. GDI a bit rubbish at text sizing, tends to only give rough sizes. This is better but still not perfect. Need to measure using StringFormat.GenericDefault, doin't know why. For best matching need to fillpath with addstring StringFormat.GenericTypographic and translate back from getbounds()x, y as some spacing will have been added in from of the word.
   ph.Reset(); 
   ph.AddString(hh.characters[i].ToString(), s.fontdef.getfont.FontFamily, (int)s.fontdef.getfont.Style, (s.fontdef.getfont.SizeInPoints / 72.0f) * grapher.DpiY, zerozero, StringFormat.GenericDefault);
   hh.widths.Add((int)Math.Ceiling(ph.GetBounds().Width)); 

   // get max and min size for this character with squeezing and stretching
   hh.minwidths.Add(getmincharwidth(s.fontdef.getfont, hh.characters[i], hh.widths[i]));
   hh.maxwidths.Add(getmaxcharwidth(s.fontdef.getfont, hh.characters[i], hh.widths[i]));

   hh.xpositions.Add(0);

   // get the actual kerning for this character and the one before it. This isn't perfect either. For example, “rav” and “bravo”, when drawn by GDI+ have different spacing between the “a” and the “v” for some reason.
   if (i > 0)
   {
    ph.Reset(); 
    ph.AddString(hh.characters[i - 1].ToString()  + hh.characters[i].ToString(), s.fontdef.getfont.FontFamily, (int)s.fontdef.getfont.Style, (s.fontdef.getfont.SizeInPoints / 72.0f) * grapher.DpiY, zerozero, StringFormat.GenericDefault);
    hh.kernings.Add((int)Math.Ceiling(ph.GetBounds().Width) - hh.widths[i] - hh.widths[i - 1]);

    hh.minkernings.Add(getminkerning(s.fontdef.getfont, hh.characters[i - 1], hh.characters[i], hh.kernings[i - 1]));
    hh.maxkernings.Add(getmaxkerning(s.fontdef.getfont, hh.characters[i - 1], hh.characters[i], hh.kernings[i - 1]));  
   }
  }
 }

 protected int getminspacewidth(Font f, int w)
 {
  // blanket reduce all by a max of 20% at the moment
  return (int)Math.Round(w * 0.4);
 }

 protected int getmaxspacewidth(Font f, int w)
 {
  // blanket increase all by a max of 20% at the moment
  return (int)Math.Round(w * 1.2);
 }

 protected int getmincharwidth(Font f, Char c, int w)
 {
  // blanket reduce all by a max of 7% at the moment
  return (int)Math.Round(w * 0.8);
 }

 protected int getmaxcharwidth(Font f, Char c, int w)
 {
  // blanket increase all by a max of 2% at the moment
  // letters not generally stretched so only use as a last resort
  return (int)Math.Round(w * 1.02);
 }

 protected int getminkerning(Font f, Char c1, Char c2, int k)
 {
  // blanket reduce all by a max of 20% at the moment
  return (int)Math.Round(k * 0.4);
 }

 protected int getmaxkerning(Font f, Char c1, Char c2, int k)
 {
  // blanket increase all by a max of 6% at the moment
  return (int)Math.Round(k * 1.06);
 }

 protected void drawheadlineparagraph(paragraph p, articlelayoutpart al, int x, int y, int width)
 {
  // have already measured all the positions of the letters (including position after squashing)
  // need to draw the letters at these positions and squash them as required.
  // special effects may overlap with each other so probably best to draw entire lines / words at a time
  
  // get and set the graphics properties for the paragraph
  // it is not allowed to change during the paragraph for headline justification
  textstyle ts = styles.text.style(p.textparts[0].style);
  paragraphstyle ps = styles.paragraph.style(p.style); 
  GraphicsPath ph = new GraphicsPath();
  GraphicsPath phchar = new GraphicsPath();
  Matrix squash = new Matrix();
  headlinehelperline line;
  headlinewordhelper word;
  float xscale;

  // loop through p.headlinelines to do the business
  for (int i = 0; i < p.headlinelines.lines.Count; i++)
  {
   line = p.headlinelines.lines[i];

   for (int j = 0; j < line.words.Count; j++)
   {
    word = line.words[j];

    for (int k = 0; k < word.characters.Length; k++)
    {
     // add the character to a path
     // Using graphics path for decent accuracy sizing. GDI a bit rubbish at text sizing, tends to only give rough sizes. This is better but still not perfect. Need to measure using StringFormat.GenericDefault, doin't know why. For best matching need to fillpath with addstring StringFormat.GenericTypographic and translate back from getbounds()x, y as some spacing will have been added in front of the character.
     phchar.Reset();
     phchar.AddString(word.characters[k].ToString(), ts.fontdef.getfont.FontFamily, (int)ts.fontdef.getfont.Style, (ts.fontdef.getfont.SizeInPoints / 72.0f) * grapher.DpiY, zerozero, StringFormat.GenericTypographic);

     // put character at exactly (0, 0) or as close as is possible
     squash.Reset();
     squash.Translate(-phchar.GetBounds().X, 0);
     phchar.Transform(squash);

     // squash the character if required
     if (line.lettersqueeze  > 0)
     {
      // squash the path. Percentage of squashing required is the original width of the character divided by the desired width of the character
      xscale = (float)((word.widths[k] - 
          ((word.widths[k] - word.minwidths[k]) * line.lettersqueeze)) 
          /
           word.widths[k]);

      squash.Reset();
      squash.Scale(xscale, 1);
      phchar.Transform(squash);
     }

     // stretch the character if required
     if (line.letterstretch > 0)
     {
      // squash the path. Percentage of squashing required is the original width of the character divided by the desired width of the character
      xscale = (float)((word.widths[k] + 
       ((word.maxwidths[k] - word.widths[k]) * line.letterstretch)) 
       /
      word.widths[k]);

      squash.Reset();
      squash.Scale(xscale, 1);
      phchar.Transform(squash);
     }

     squash.Reset();
     //squash.Translate(word.xpositions[k] - (word.xpositions[k] * xscale), 0);
     squash.Translate(20 + word.xpositions[k], 20);
     phchar.Transform(squash);


     // add this character to the path for the whole line
     ph.AddPath(phchar, false);
    }
   }

   // draw this line
   drawheadlineline(ph, x, y + (int)Math.Floor(i * ts.fontdef.getfont.Height * ps.linespacing), width, ts.fontdef.getfont.Height, ts);
   // underline the line, remove 20 pixel padding from path. This is used so that the special effects (fuzz etc) can go beyone the extents of the letters, which they obviously need to
   ph.Transform(minustwentytwenty);
   underlineline(p, ps, ts.foreground.getsolidbrush, ts.foreground.getdefaultpen, al, ph, x, y + (int)Math.Floor(i * ts.fontdef.getfont.Height * ps.linespacing), p.maxright - p.minleft, ts.fontdef.getfont.Height, ts.fontdef.baselineoffsetpixels(grapher), p.headlinelines.minspacewidth);
   ph.Reset();
  }
 }

 protected void underlineline(paragraph p, paragraphstyle ps, Brush b, Pen pp, articlelayoutpart al, GraphicsPath ph, int x, int y, int linewidth, int lineheight, int descentheight, int spacewidth)
 {
  int ileft, iwidth;
  int itop;
  if (ps.underlining == underlineposition.none) return;
  Region clip = grapher.Clip;
  GraphicsPathIterator gpiunderline;
  GraphicsPath gpunderline, gptemp;
  RectangleF rchar;
  float lastcharright = 0;
  bool lastdescender = false;

  // work out the x positions of the line
  if (ps.underlinewhat == underlineextents.line)
  {
   // just underline under the words
   ileft = x;
   iwidth = linewidth;
  }
  else if (ps.underlinewhat == underlineextents.paragraph)
  {
   // underline each line the same width to the width of the 
   // biggest line in the paragraph
   ileft = al.left + al.offsetleft + p.minleft;
   iwidth = p.maxright - p.minleft;
  }
  else  // underlineextents.layout
  {
   // underline the entire width of the layout area
   ileft = al.drawleft + al.offsetleft;
   iwidth = al.drawwidth - al.offsetleft - al.offsetright;
  }

  // work out the ypositions of the line
  if (ps.underlining == underlineposition.bottom)
  {
   // position underlining with the bottom at the level of the descenders
   itop = y + lineheight - ps.underlinewidth + 1;
  }
  else
  {
   // position underlining under the characters
   itop = y + lineheight + 1;
  }

  // if neccesary add some clipping around the descenders so that they stand out properly from the underline
  if (ps.underlining == underlineposition.bottom)
  {
   // we set the amount of space to leave around the descenders to be the same as the underline width. this should make sense visually. the graphics path is drawn at 0, 0 so we must move it to where the line has actually been drawn
   underlinematrix.Reset();
   underlinematrix.Translate(x, y);
   gpiunderline = new GraphicsPathIterator(ph);
   gptemp = new GraphicsPath();
   gpunderline = new GraphicsPath();
   gpiunderline.Rewind();
   for (int current = 0; current < gpiunderline.SubpathCount; current++)
   {
    // get region within the path (a letter or part of a letter)
    gptemp.Reset();
    gpiunderline.NextMarker(gptemp);
    // add this as a rectangle to the clip area we will use if required
    // If this region has a descender then inflate the clip area down to the bottom and add a little padding
    rchar = gptemp.GetBounds();
    if (Math.Floor(rchar.Bottom) > ((lineheight + descentheight) / 2))
    {
     rchar.Height = lineheight;
     // if the last character is next to this one and also has a descender then work out the kerning and use this for the inflation, if there was a space before this character then do something else
     if (((rchar.Left - lastcharright) >= spacewidth) || (lastdescender == false)) rchar.Inflate(1, 0);
     else rchar.Inflate(rchar.Left - lastcharright, 0);
     //rchar.Inflate(1, 0);
     gpunderline.AddRectangle(rchar);

     lastdescender = true;
    }
    else lastdescender = false;

    lastcharright = rchar.Right; 
   }
   gpunderline.Transform(underlinematrix);
   // can test the clip area visually using this line
   //grapher.FillPath(new SolidBrush(Color.Red), gpunderline);
   grapher.SetClip(gpunderline, CombineMode.Exclude);
  }

  // now draw the line
  if (ps.underlinewidth == 1) grapher.DrawLine(pp , ileft, itop, ileft + iwidth, itop);
  else grapher.FillRectangle(b, ileft, itop, iwidth, ps.underlinewidth);
  // restore the original clipping
  grapher.Clip = clip;
 }

 protected void drawheadlineline(GraphicsPath ph, int x, int y, int width, int height, textstyle ts)
 {
  // the path is drawn at 0, 0 so move it where we want to draw
  grapher.TranslateTransform(x, y);

  if (ts.effect == texteffecttype.none) 
  {
   grapher.FillPath(ts.foreground.getsolidbrush, ph);
  }
  else if (ts.effect == texteffecttype.fuzz)
  {
   drawpathfuzzy(ph, ts, 0, 0, width, height);
  }
  else if (ts.effect == texteffecttype.dropshadow)
  {
   drawpathdropshadow(ph, ts, 0, 0, width, height);
  }

  // restore the transform
  grapher.TranslateTransform(-x, -y);
 }

 // measure all paragraphs in an articlepart
 protected void measurelayoutpart(article a, articlelayoutpart al)
 {
  int allparagraphsheight = 0;
  int compulsoryparagraphsheight = 0;
  int finetuneminheight = 0;
  int finetunemaxheight = 0;
  int h;

  a.minleft = int.MaxValue;
  a.maxright = 0;

  if (a.Count == 0) notifydrawwarning("No article", a, al);

  // loop bits
  foreach (articlepart ap in a)
  {
   // measure the paragraphs in this bit
   for (int i = 0; i < ap.paragraphs.Count; i++)
   {
    ap.paragraphs[i].draw = true;

    if (styles.paragraph.style(ap.paragraphs[i].style).justification == paragraphjustification.headline) measureheadlineparagraph(ap.paragraphs[i], al.textwidth, al.textheight);
    else measureparagraph(ap.paragraphs[i], al.textwidth);

    allparagraphsheight += ap.paragraphs[i].height;
    if (ap.paragraphs[i].optional == false) compulsoryparagraphsheight += ap.paragraphs[i].height;
   }

   // measure the fine tuning paragraphs
   ap.finetuneminheight = 1000000;
   ap.finetunemaxheight = 0;
   for (int i = 0; i < ap.finetune.Count; i++)
   {
    if (styles.paragraph.style(ap.finetune[i].style).justification == paragraphjustification.headline) measureheadlineparagraph(ap.finetune[i], al.textwidth, al.textheight);
    else measureparagraph(ap.finetune[i], al.textwidth);

    ap.finetune[i].draw = false;
    // set max / min height of all paragraphs within finetune
    if (ap.finetune[i].height < ap.finetuneminheight) ap.finetuneminheight = ap.finetune[i].height;
    if (ap.finetune[i].height > ap.finetunemaxheight) ap.finetunemaxheight = ap.finetune[i].height;
   }
   if (ap.finetune.Count > 0)
   {
    finetuneminheight += ap.finetuneminheight;
    finetunemaxheight += ap.finetunemaxheight;
   }
  }

  // check article is big enough
  if ((allparagraphsheight + finetunemaxheight) < al.textheight) notifydrawwarning("Article too small in this layout position, Compulsory paragraphs height: " + allparagraphsheight.ToString() + ", Fine tune paragraphs max height: " + finetunemaxheight.ToString() + ", Layout height: " + al.textheight.ToString(), a, al);

  // check article is small enough
  if ((compulsoryparagraphsheight + finetuneminheight) > al.textheight)
   notifydrawerror("Article too big in this layout position, Compulsory paragraphs height: " + allparagraphsheight.ToString() + ", Fine tune paragraphs min height: " + finetuneminheight.ToString() + ", Layout height: " + al.textheight.ToString(), a, al);

  // remove optional paragraphs from the end first so that the article
  // fits in the specified height
  // This method perfers to keep in optional paragraphs in favour of
  // large fine tuning paragraphs.
  h = allparagraphsheight;
  foreach (articlepart ap in a)
  {
   for (int i = ap.paragraphs.Count - 1; i >= 0; i--)
   {
    if (ap.paragraphs[i].optional == true)
    {
     if ((h + finetuneminheight) < al.textheight) break;
     h -= ap.paragraphs[i].height;
     ap.paragraphs[i].draw = false;
    }
    else
    {
     if (ap.paragraphs[i].minleft < a.minleft) a.minleft = ap.paragraphs[i].minleft;
     if (ap.paragraphs[i].maxright > a.maxright) a.maxright = ap.paragraphs[i].maxright;
    }
   }
  }

  // choose which of the optional paragraphs to display
  if ((a.Count > 0) && (a[0].finetune.Count > 0) && (styles.paragraph.style(a[0].finetune[0].style).justification == paragraphjustification.headline))
  {
   h += choosefinetuneheadlineparagraph(a, al, h, finetuneminheight, finetunemaxheight);
  }
  else
  {
   h += choosefinetuneparagraph(a, al, h, finetuneminheight, finetunemaxheight);
  }
  
    a.totalheight = h;
    if (a.minleft == int.MaxValue) a .minleft = 0;
   }

   protected int choosefinetuneparagraph(article a, articlelayoutpart al, int curheight, int finetuneminheight,int finetunemaxheight)
   {
    // choose the finetuneparagraphs to use that gets as close as possible
    // to the height without going over it.
    // This method maximises the size of the current fine tune and the following ones use what is left. The means that fine tunes will tend to get progressively smaller as we progress.
    // at least one of these paragraphs should be displayed if the layout and article are designed properly. If nothing wil fit then draw nothing
    foreach (articlepart ap in a)
    {
     finetunemaxheight -= ap.finetunemaxheight;
     finetuneminheight -= ap.finetuneminheight;

     // sort fine tune paragraphs by height
     ap.finetune.Sort(new GenericComparer<paragraph>("height", GenericComparer<paragraph>.SortOrder.Ascending));
     
     for (int i = 0; i < ap.finetune.Count; i++)
     {
      // check to see that the first one fits on. Can then find the biggest one that doesn't fit and use the one before that
      if (i == 0)
      {
       if ((finetuneminheight + ap.finetune[i].height + curheight) > al.textheight)
       {
        // if the smallest one doesn't fit then we have a problem
        notifydrawwarning("All of the fine tuning paragraphs are too large, please add a smaller one", a, al);
        finetunemaxheight = 0;
        break;
       }
      }

      // if this one is bigger and last one wasn't then use last one
      if (((finetuneminheight + ap.finetune[i].height + curheight) > al.textheight) && (i > 0))
      {
       choosethisfinetuneparagraph(ap.finetune[i - 1], a);
       finetunemaxheight += ap.finetune[i - 1].height;
       finetuneminheight += ap.finetune[i - 1].height;
       break;
      }

      // use the biggest if necessary.
      if (i == (ap.finetune.Count - 1))
      {
       choosethisfinetuneparagraph(ap.finetune[i], a);
       finetunemaxheight += ap.finetune[i].height;
       finetuneminheight += ap.finetune[i].height;
      }
     }
    }

    return finetunemaxheight;
   }

   protected int choosefinetuneheadlineparagraph(article a, articlelayoutpart al, int curheight, int finetuneminheight, int finetunemaxheight)
   {
    // choose the finetuneparagraphs to use that gets as close as possible to the height without going over it.
    // headline justfied things should only have a single fine tune section. All the text styles within this should be the same.
    // choose the headline that fits the height best and that requires the least manipulation to fit.
    // at least one of these paragraphs should be displayed if the layout and article are designed properly. If nothing wil fit then draw nothing
    articlepart ap = new articlepart();
    bool firstvalid = true;
    paragraph p;

    // create a list of valid fine tune paragraphs. These are ones that are the correct number of decks and can fit in the width.
    // need a list of only possible ones as the sorting uses some of the headline helper stuff which isn't created / calculated unless the paragraph fits. This saves processor time but makes things a little more complicated here.
    for (int i = 0; i < a[0].finetune.Count; i++)
    {
     if (a[0].finetune[i].height <= al.textheight) ap.finetune.Add(a[0].finetune[i]);
    }

    // add warning if none of the headlines fit
    if (ap.finetune.Count == 0)
    {
     notifydrawwarning("None of the fine tuning paragraphs have the right number of decks or they cannot squeeze / stretch into the available width.", a, al);
    }

    // sort fine tune paragraphs by height first and then by the amount of squashing and stretching that was required
    ap.finetune.Sort(new sortparagrapsbyheadline()); 

     finetunemaxheight -= a[0].finetunemaxheight;
     finetuneminheight -= a[0].finetuneminheight;
     for (int i = 0; i < ap.finetune.Count; i++)
     {
     p = ap.finetune[i];

      // check to see that the first one fits on. Can then find the biggest one that doesn't fit and use the one before that
      if (firstvalid == true)
      {
       firstvalid = false;
       if ((finetuneminheight + p.height + curheight) > al.textheight)
       {
        // if the smallest one doesn't fit then we have a problem
        notifydrawwarning("All of the fine tuning paragraphs are too large, please add a smaller one", a, al);
        finetunemaxheight = 0;
        break;
       }
      }

      // if this one is bigger and last one wasn't then use last one
      // we have created our own article part with the acceptable fine tune paragraphs in. These are added as pointers and not as copies so can set the propertied of the fine tune paragraphs and they will be recognised by other functions without any problems
      if (((finetuneminheight + p.height + curheight) > al.textheight) && (i > 0))
      {
       choosethisfinetuneparagraph(ap.finetune[i - 1], a);
       finetunemaxheight += ap.finetune[i - 1].height;
       finetuneminheight += ap.finetune[i - 1].height;

       break;
      }

      // use the biggest if necessary.
      if (i == (ap.finetune.Count - 1))
      {
       choosethisfinetuneparagraph(p, a);
       finetunemaxheight += p.height;
       finetuneminheight += p.height;
      }
     }
    
    return finetunemaxheight;
   }

 protected void choosethisfinetuneparagraph(paragraph p, article a)
 {
  p.draw = true;

  if (p.minleft < a.minleft) a.minleft = p.minleft;
  if (p.maxright > a.maxright) a.maxright = p.maxright;
 }

 // measure all the text in a paragraph
   protected void measureparagraph(paragraph p, int width)
   {
    textstyle ts = null;
    paragraphstyle ps = styles.paragraph.style(p.style);
    int xpos = 0;
    int maxbaselineheight;
    int maxdescentheight;
    int starttext, endtext = 0;
    int startword, endword = 0;
    int wordswidth = 0; // total width of all words on a line (no spaces)
    int linewidth = 0; // total width of all words on a line (with spaces)
    int wordcount = 0; // number of words on a line

    p.minleft = int.MaxValue;
    p.maxright = 0;

    // find word widths for the text;
    for (int i = 0; i < p.textparts.Count; i++)
    {
     measuretext(p.textparts[i]);
    }

    // need to take account of line spacing and 
    // measure height of the paragraph at the passed width
    // have current xpos
    // look at next textpart
    // loop words (adding space width before if not first on line)
    // until goes beyond width
    p.height = 0; 
    maxbaselineheight = 0;
    maxdescentheight = 0;
    starttext = 0;
    startword = 0;
    // loop text parts
    for (int i = 0; i < p.textparts.Count; i++)
    {
     // set the lineheight to the largest font on this line
     ts = styles.text.style(p.textparts[i].style);
     if (ts.fontdef.baselineoffsetpixels(grapher) > maxbaselineheight) maxbaselineheight = ts.fontdef.baselineoffsetpixels(grapher);
     if (ts.fontdef.descentpixels(grapher) > maxdescentheight) maxdescentheight = ts.fontdef.descentpixels(grapher);
     // loop words within the text
     for (int j = 0; j < p.textparts[i].widths.Count; j++)
     {
      // set xposition for this word for a left justified paragraph
      if (xpos > 0) xpos += p.textparts[i].spacewidth;
      p.textparts[i].xpositions[j] = xpos;
      xpos += p.textparts[i].widths[j];
      // this word too long for the line
      if (xpos > width)
      {
       // set positions for each word on this line
       positionline(starttext, startword, endtext, endword, maxbaselineheight, maxdescentheight, wordswidth, linewidth, width, wordcount, p, ts, ps, false);
       // move this word onto the next line
       xpos = p.textparts[i].widths[j];
       // if this word is bigger than the line then stop measuring and say that this paragraph won't fit here
       if (xpos > width)
       {
        p.height = int.MaxValue;
        return;
       }

       p.textparts[i].xpositions[j] = 0;
       // not sure how this works without the graphic object to specify
       // how inches (or points or whatever) relate to pixels.
       // the linespacing is taken for the last textpart of the line.
       // could make it the max or min or something like that but it doesnt
       // make sense to change it mid paragraph so its not very important.
       p.height += (int)Math.Ceiling((maxdescentheight + maxbaselineheight) * ps.linespacing);
       // start off with a lineheight of the current text part, will
       // be overwritten later if a taller font is encountered.
       maxbaselineheight = ts.fontdef.baselineoffsetpixels(grapher);
       maxdescentheight = ts.fontdef.descentpixels(grapher);
       // reset number of words on the next line
       wordcount = 0;
       // reset width variables for next line
       wordswidth = 0;
       linewidth = 0;
       // set the indexes for the first word on the next line
       starttext = i;
       startword = j;
      }
    
      // update width of all the words on a line
      wordswidth += p.textparts[i].widths[j];
      linewidth = xpos;
      // how many words on this line
      wordcount++;
      // keep a track of the end indices for a line
      endtext = i;
      endword = j;
     }
    }

    // position the last line
    positionline(starttext, startword, endtext, endword, maxbaselineheight, maxdescentheight, wordswidth, linewidth, width, wordcount, p, ts, ps, true);

    p.height += (int)Math.Ceiling((maxdescentheight + maxbaselineheight) * ps.linespacing);

    if (p.minleft == int.MaxValue) p.minleft = 0;
   }

   // set the x, y position for each word on this line according to
   // the textstyle
   protected void positionline(int starttext, int startword, int endtext, int endword, int baselineheight, int descentheight, int wordswidth, int linewidth, int width, int wordcount, paragraph p, textstyle s, paragraphstyle ps, bool lastline) 
   {
    textstyle ts = null;
    int startw, endw;
    Single jpos = 0, jspace;
    int temp;

    if (wordcount <= 1) jspace = 0;
    else jspace = ((Single)width - (Single)wordswidth) / ((Single)wordcount - (Single)1);

    // loop from start text to end text
    for (int i = starttext; i <= endtext; i++)
    {
     if (i == endtext) endw = endword; else endw = p.textparts[i].widths.Count - 1;
     if (i == starttext) startw = startword; else startw = 0;

     for (int j = startw; j <= endw; j++)
     {
      // set y position of this piece of text
      ts = styles.text.style(p.textparts[i].style);
      p.textparts[i].ypositions[j] = p.height + baselineheight - ts.fontdef.baselineoffsetpixels(grapher);
    
      // if left justified then everything done already
      if (ps.justification == paragraphjustification.left)
      {
       p.minleft = 0;
       if (linewidth > p.maxright) p.maxright = linewidth;
      }

      // if right justified then move everything by the total leftover
      // space on the line
      if (ps.justification == paragraphjustification.right)
      {
       p.maxright = width;
       temp = width - linewidth;
       if (temp < p.minleft) p.minleft = temp;
       p.textparts[i].xpositions[j] += temp;
      }
 
      // if centre jusitied move along by half this space
      if (ps.justification == paragraphjustification.center)
      {
       temp = (width - linewidth) / 2;
       if (temp < p.minleft) p.minleft = temp;
       if ((temp + linewidth) > p.maxright) p.maxright = temp + linewidth;
       p.textparts[i].xpositions[j] += (width - linewidth) / 2;
      }

      // if justified the position with jspace imbetween each word
      // (jspace already worked out). If this is the last line then
      // do nothing and the text will remain left justified
      if (ps.justification == paragraphjustification.justified)
      {
       if (lastline == true)
       {
        p.minleft = 0;
        if (linewidth > p.maxright) p.maxright = linewidth;
       }
       else
      {
       p.minleft = 0;
          if (jpos > 0) jpos += jspace;
           p.textparts[i].xpositions[j] = (int)Math.Round(jpos);
           jpos += p.textparts[i].widths[j];
           if (jpos > p.maxright) p.maxright = (int)Math.Ceiling(jpos);
      }
      }
     }
    }
   }

   public void drawarticle(Graphics g, article a, articlelayout al, sessionpictures sps, bool protectoptionalpicturespace)
   {
    grapher = g;
    styles = al.styles;
    article temp = new article();

    // loop through the article layout parts and draw the required part of the article in each one
    foreach (articlelayoutpart alp in al.layoutparts)
    {
     // find article bits to be drawn here
     foreach (articlepart ap in a)
     {
      // work out which bits of the article to draw here
      if (alp.parttypes.contains(ap.parttype)) temp.Add(ap);
     }

     // draw these article parts in this layout part
     alp.sps = sps; // the layout part needs the session pictures to know whether to use space from an optional picture
     alp.protectoptionalpicturespace = protectoptionalpicturespace;
     drawlayoutpart(temp, alp);

     temp.Clear(); 
    }

   }

   protected void drawlayoutpart(article a, articlelayoutpart alp)
   {
    Matrix existingtransform = grapher.Transform.Clone();
    int y = alp.texttop;

    measurelayoutpart(a, alp);

    // set up the angle of this layout part if required
    if (alp.angle != 0)
    {
     // rotate text by the required amount, this transform is applied from the origin but these translates sort that out to rotate around the top left instead
     grapher.TranslateTransform((float)alp.left, (float)alp.top);
     grapher.RotateTransform(alp.angle);
     grapher.TranslateTransform(-(float)alp.left, -(float)alp.top);
     // this didn't quite work for some reason. Probably because the
     // translate interacted with the rotate. Anyway found the method
     // above which is much easier
     // we still want to draw at bitmap x, y after the transform so 
     // translate back appropriately
     // radius of circle from origin to where we are
     //double l = Math.Sqrt((alp.left * alp.left) + (alp.top * alp.top));
     // angle from x, y to rotated from origin x, y (our angle)
     //double d = Math.Atan(alp.left / alp.top) - (alp.angleradians / 2.0);
     // distance from x, y to rotated from origin x, y (our hypotenuse)
     //double h = 2 * Math.Sin(alp.angleradians / 2.0) * l;
     // x, y translation easy to work out now from right hand triangle
     // with known hypotenuse and angle
     //g.TranslateTransform((double)(h * Math.Cos(d)), (double)(h * Math.Sin(d)));
    }

    // draw the border and background. This is after the angling so will be angled as well. It is before aligning so this will be drawn at its calculated extents and the text is aligned within it
    alp.drawheight = alp.height;
    alp.drawwidth = alp.width;
    alp.drawleft = alp.left;
    alp.drawtop = alp.top;
    if (alp.sizetotextwidth == true)
    {
     // centred text can not go fromthe very left
     alp.drawwidth = alp.offsetleft + alp.offsetright + a.maxright - a.minleft;
     alp.drawleft = alp.left + a.minleft;
    }

    if (alp.sizetotextheight == true)
    {
     // text is always drawn from the top so no equivalent of minleft
     alp.drawheight = alp.offsettop + a.totalheight + alp.offsetbottom;
    }

    Matrix prebordertransform = grapher.Transform.Clone();

    // take account of vertical alignment of the border
    if (alp.bordervalign == verticalalign.center)
     grapher.TranslateTransform(0, (alp.height - alp.drawheight) / 2);
    else if (alp.bordervalign == verticalalign.bottom)
     grapher.TranslateTransform(0, (alp.height - alp.drawheight));

    // take account of vertical alignment of the border
    if (alp.borderhalign == horizontalalign.center)
     grapher.TranslateTransform((alp.width - alp.drawwidth) / 2, 0);
    else if (alp.borderhalign == horizontalalign.right)
     grapher.TranslateTransform((alp.width - alp.drawwidth), 0);

    // draw border at calculated extents
    drawborderandbackground(alp);

    // restore transform after border operations
    grapher.Transform = prebordertransform;

    // take account of vertical alignment of this article part
    if (alp.articlevalign == verticalalign.center)
     grapher.TranslateTransform(0, (alp.textheight - a.totalheight) / 2);
    else if (alp.articlevalign == verticalalign.bottom)
     grapher.TranslateTransform(0, (alp.textheight - a.totalheight));

    // take account of vertical alignment of this article part
    if (alp.articlehalign == horizontalalign.center)
     grapher.TranslateTransform((alp.textwidth - a.maxright) / 2, 0);
    else if (alp.articlehalign == horizontalalign.right)
     grapher.TranslateTransform((alp.textwidth - a.maxright), 0);

    // draw the article parts in this layout part
    foreach (articlepart ap in a)
    {
     // draw all paragraphs marked for drawing
     for (int i = 0; i < ap.paragraphs.Count; i++)
     {
      if (ap.paragraphs[i].draw == true)
      {
       if (styles.paragraph.style(ap.paragraphs[i].style).justification == paragraphjustification.headline) drawheadlineparagraph(ap.paragraphs[i], alp, alp.textleft, y, alp.textwidth);
       else drawparagraph(ap.paragraphs[i], alp, alp.textleft, y, alp.textwidth);
       y = y + ap.paragraphs[i].height;
      }
     }

    // draw all fine tuning paragraphs marked for drawing
    for (int i = 0; i < ap.finetune.Count; i++)
    {
     if (ap.finetune[i].draw == true)
     {
      if (styles.paragraph.style(ap.finetune[i].style).justification == paragraphjustification.headline) drawheadlineparagraph(ap.finetune[i], alp, alp.textleft, y, alp.textwidth);
      else drawparagraph(ap.finetune[i], alp, alp.textleft, y, alp.textwidth);
      y = y + ap.finetune[i].height;
     }
    }
   }

   // reset any angling and aligning
   grapher.Transform = existingtransform;
  }

   protected void drawborderandbackground(articlelayoutpart alp)
   {
    alp.bordercolour.getdefaultpen.Width = alp.borderwidth;
    if (alp.bordercorners == 0)
    {
     grapher.FillRectangle(alp.background.getsolidbrush, alp.drawleft, alp.drawtop, alp.drawwidth, alp.drawheight);
     if (alp.borderwidth > 0) grapher.DrawRectangle(alp.bordercolour.getdefaultpen, alp.drawleft, alp.drawtop, alp.drawwidth, alp.drawheight);
    }
    else
    {
     fillroundrectangle(alp.background.getsolidbrush, alp.drawleft, alp.drawtop, alp.drawwidth, alp.drawheight, alp.bordercorners);
     if (alp.borderwidth > 0) drawroundrectangle(alp.bordercolour.getdefaultpen, alp.drawleft, alp.drawtop, alp.drawwidth, alp.drawheight, alp.bordercorners);
    }
   }

   // draw a paragraph at x, y
   // paragraph must already have been measured
   protected void drawparagraph(paragraph p, articlelayoutpart al, int x, int y, int width)
   {
    textstyle ts = null;
    paragraphstyle ps = styles.paragraph.style(p.style);
    int ythisline = 0;
    int xthisline = 0;
    int widththisline = 0;
    GraphicsPath gpunderline = new GraphicsPath();
    Point punderline = new Point();

    // loop text parts
    for (int i = 0; i < p.textparts.Count; i++)
    {
     // get and set the graphics properties for this bit of text
     ts = styles.text.style(p.textparts[i].style);

     // loop words within the text
     for (int j = 0; j < p.textparts[i].widths.Count; j++)
     {
      // initialise line positions for the first word in the paragraph
      if ((i == 0) && (j == 0))
      {
       ythisline = p.textparts[i].ypositions[j];
       xthisline = p.textparts[i].xpositions[j];
       widththisline = 0;
      }

      // used to test the positions, heights and widths of the words
      // easy to see whats gonig on when can see bounding rectangle
      //grapher.DrawRectangle(new Pen(Color.Gray), x + p.textparts[i].xpositions[j], y + p.textparts[i].ypositions[j], p.textparts[i].widths[j], ts.fontdef.getfont.Height);

      // draw the word at its measured postition
      drawword(p.textparts[i].words[j], ts, x + p.textparts[i].xpositions[j], y + p.textparts[i].ypositions[j], p.textparts[i].widths[j], ts.fontdef.getfont.Height);

      // if we are underlining then need to know when there is a new line
      // if we are underlining at descent level then we must create a graphics path with the line in it starting at 0, 0.
      if (ythisline != p.textparts[i].ypositions[j])
      {
       // new line here so underline the last one
       underlineline(p, ps, ts.foreground.getsolidbrush, ts.foreground.getdefaultpen, al, gpunderline, x + xthisline, y + ythisline, widththisline, ts.fontdef.getfont.Height, ts.fontdef.baselineoffsetpixels(grapher), p.textparts[i].spacewidth);

       // record the start of the next line
       xthisline = p.textparts[i].xpositions[j];
       ythisline = p.textparts[i].ypositions[j];
       gpunderline.Reset(); 
      }

      // add a path for this word if we are underlining at bottom height so that we can clip out the descenders
      if (ps.underlining == underlineposition.bottom)
      {
       // position at which to add the path for the word
       punderline.X = p.textparts[i].xpositions[j] - xthisline;
       punderline.Y = p.textparts[i].ypositions[j] - ythisline;
       // add the word to the path
       gpunderline.AddString(p.textparts[i].words[j], ts.fontdef.getfont.FontFamily, (int)ts.fontdef.getfont.Style, ts.fontdef.getfont.SizeInPoints * grapher.DpiY / 72.0f, punderline, StringFormat.GenericTypographic);
      }

      // save the line width at this point for the next underline. We set this for every word so we already know the required width if the next word happens to be on a new line
      widththisline = p.textparts[i].xpositions[j] + p.textparts[i].widths[j] - xthisline;
     }
    }

    // underline the last line
    if (ts != null) underlineline(p, ps, ts.foreground.getsolidbrush, ts.foreground.getdefaultpen, al, gpunderline, x + xthisline, y + ythisline, widththisline, ts.fontdef.getfont.Height, ts.fontdef.baselineoffsetpixels(grapher), p.textparts[p.textparts.Count - 1].spacewidth);
   }

   // draw a single word in a page. Take account of various options
   protected void drawword(string word, textstyle ts, int x, int y, int width, int height)
   {
    if (ts.effect == texteffecttype.none)
    {
     grapher.DrawString(word, ts.fontdef.getfont, ts.foreground.getsolidbrush, x, y, StringFormat.GenericTypographic);
     // used this when testing measurements
     //GraphicsPath ph = new GraphicsPath();
     //ph.AddString(word, ts.fontdef.getfont.FontFamily, (int)ts.fontdef.getfont.Style, (ts.fontdef.getfont.SizeInPoints / 72.0f) * g.DpiY, new Point(x, y), StringFormat.GenericTypographic);
     //Matrix m = new Matrix();
     //m.Translate(x-ph.GetBounds().X, y-ph.GetBounds().Y);
     //ph.Transform(m);
     //g.FillPath(ts.foreground.getsolidbrush, ph);
    }
    else if (ts.effect == texteffecttype.fuzz)
    {
     drawwordfuzzy(word, ts, x, y, width, height);
    }
    else
    {
     drawworddropshadow(word, ts, x, y, width, height);
    }
   }

   // draw a word with a fuzzy halo. This makes it readable over pictures
   // of any color. Eg, white text over a white picture is possible. The 
   // fuzz colour should be set to something contrasting to the foreground
   // colour.
   protected void drawwordfuzzy(string word, textstyle ts, int x, int y, int width, int height)
   {
    // Create a GraphicsPath object. 
    GraphicsPath pth = new GraphicsPath();

    // Add the string in the chosen style. This needs the font size in 
    // pixels  whereas font.height returns the size in points. We cope 
    // with this later using the page units
    pth.AddString(word, ts.fontdef.getfont.FontFamily, (int)ts.fontdef.getfont.Style, ts.fontdef.getfont.SizeInPoints * grapher.DpiY / 72.0f, twentytwenty, StringFormat.GenericTypographic);

    drawpathfuzzy(pth, ts, x, y, width, height);

    // and you're done. 
    //pth.Dispose();
   }

 protected void drawpathfuzzy(GraphicsPath pth, textstyle ts, int x, int y, int width, int height)
 {
  // don't seem to be able to optimise this much. Can't resize the bitmap
  // so must create new for every word (or possibly could have a very 
  // large one and reuse it by painting over it each time but a pain)
  // since new bitmap required new Graphics also required each time.
  // I tried clearing the graphics path and reusing but it didn't work
  // either.
  Matrix existingtransform = grapher.Transform.Clone();

  // Create a bitmap in a fixed ratio to the original drawing area.
  Bitmap bm = new Bitmap((int)((width + 40) / ts.fuzzmagnification), (int)((height + 40) / ts.fuzzmagnification));

  // Get the graphics object for the image. 
  Graphics gt = Graphics.FromImage(bm);

  ts.effectcolour.getdefaultpen.Width = ts.fuzzwidth;

  // Choose an appropriate smoothing mode for the halo. 
  gt.SmoothingMode = SmoothingMode.AntiAlias;
  // Transform the graphics object so that the same path may be used for both halo and text output. 
  gt.ScaleTransform(1.0f / ts.fuzzmagnification, 1.0f / ts.fuzzmagnification);

  // Draw around the outline of the path
  gt.DrawPath(ts.effectcolour.getdefaultpen, pth);

  // and then fill in for good measure. 
  gt.FillPath(ts.effectcolour.getsolidbrush, pth);

  // We no longer need this graphics object
  gt.Dispose();

  // setup the smoothing mode for path drawing
  grapher.SmoothingMode = SmoothingMode.AntiAlias;

  // and the interpolation mode for the expansion of the halo bitmap
  grapher.InterpolationMode = InterpolationMode.HighQualityBicubic;

  // expand the halo making the edges nice and fuzzy. 
  grapher.TranslateTransform(x - 20, y - 20);
  grapher.ScaleTransform(ts.fuzzmagnification, ts.fuzzmagnification);
  grapher.DrawImage(bm, 0, 0);

  //Redraw the original text
  //g.ResetTransform();
  grapher.Transform = existingtransform;
  grapher.TranslateTransform(x - 20, y - 20);
  grapher.FillPath(ts.foreground.getsolidbrush, pth);

  //g.ResetTransform();
  grapher.Transform = existingtransform;
 }
   // draw a word with a fuzzy halo. This makes it readable over pictures
   // of any color. Eg, white text over a white picture is possible. The 
   // fuzz colour should be set to something contrasting to the foreground
   // colour.
   protected void drawworddropshadow(string word, textstyle ts, int x, int y, int width, int height)
   {
    // Create a GraphicsPath object. 
    GraphicsPath pth = new GraphicsPath();

    // Add the string in the chosen style. This needs the font size in 
    // pixels  whereas font.height returns the size in points. We cope 
    // with this later using the page units
    pth.AddString(word, ts.fontdef.getfont.FontFamily, (int)ts.fontdef.getfont.Style, ts.fontdef.getfont.SizeInPoints * grapher.DpiY / 72.0f, twentytwenty, StringFormat.GenericTypographic);

    drawpathdropshadow(pth, ts, x, y, width, height);
    // and you're done. 
    //pth.Dispose();
   }

 protected void drawpathdropshadow(GraphicsPath pth, textstyle ts, int x, int y, int width, int height)
 {
  // don't seem to be able to optimise this much. Can't resize the bitmap
  // so must create new for every word (or possibly could have a very 
  // large one and reuse it by painting over it each time but a pain)
  // since new bitmap required new Graphics also required each time.
  // I tried clearing the graphics path and reusing but it didn't work
  // either.
  Matrix existingtransform = grapher.Transform.Clone();

  // Create a bitmap in a fixed ratio to the original drawing area.
  Bitmap bm = new Bitmap((int)((width + 40) / ts.dropshadowmagnification), (int)((height + 40) / ts.dropshadowmagnification));

  // Get the graphics object for the image. 
  Graphics gt = Graphics.FromImage(bm);

  // Choose an appropriate smoothing mode for the halo. 
  gt.SmoothingMode = SmoothingMode.AntiAlias;
  // Transform the graphics object so that the same path may be used for both halo and text output. 
  gt.ScaleTransform(1.0f / ts.dropshadowmagnification, 1.0f / ts.dropshadowmagnification);

  // Draw around the outline of the path
  gt.DrawPath(ts.effectcolour.getdefaultpen, pth);

  // and then fill in for good measure. 
  gt.FillPath(ts.effectcolour.getsolidbrush, pth);

  // We no longer need this graphics object
  gt.Dispose();

  // setup the smoothing mode for path drawing
  grapher.SmoothingMode = SmoothingMode.AntiAlias;

  // and the interpolation mode for the expansion of the halo bitmap
  grapher.InterpolationMode = InterpolationMode.HighQualityBicubic;

  // expand the halo making the edges nice and fuzzy. 
  grapher.TranslateTransform(x - 20 + ts.dropshadowoffset, y - 20 + ts.dropshadowoffset);
  grapher.ScaleTransform(ts.dropshadowmagnification, ts.dropshadowmagnification);
  grapher.DrawImage(bm, 0, 0);

  //Redraw the original text
  //g.ResetTransform();
  grapher.Transform = existingtransform;
  grapher.TranslateTransform(x - 20, y - 20);
  grapher.FillPath(ts.foreground.getsolidbrush, pth);

  //g.ResetTransform();
  grapher.Transform = existingtransform;

 }
   // notify an administrator when an article cannot fit into an area
   // that it is asked to do so, either when too big or too small
   protected void notifydrawerror(string err, article a, articlelayoutpart al)
   {
    XmlSerializer serializer = new XmlSerializer(typeof(article));
    StringWriter tw = new StringWriter();
    serializer.Serialize(tw, a);
    err += "\n\n" + tw.ToString();

    serializer = new XmlSerializer(typeof(articlelayoutpart));
    tw = new StringWriter();
    serializer.Serialize(tw, al);
    err += "\n\n" + tw.ToString();

    // do nothing for the time being. Maybe send an email later on.
    // cedd throw new Exception(err);
   }

   protected void notifydrawwarning(string err, article a, articlelayoutpart al)
   {
    /*
      XmlSerializer serializer = new XmlSerializer(typeof(article));
    StringWriter tw = new StringWriter();
    serializer.Serialize(tw, a);
    err += "\n\n" + tw.ToString();

    serializer = new XmlSerializer(typeof(articlelayoutpart));
    tw = new StringWriter();
    serializer.Serialize(tw, al);
    err += "\n\n" + tw.ToString();

    // do nothing for the time being. Maybe send an email later on.
    throw new Exception(err);
     */
 }

 #region Fills a Rounded Rectangle with integers. 
 public void fillroundrectangle(System.Drawing.Brush brush, int x, int y, int width, int height, int radius) 
 {
  float fx = Convert.ToSingle(x);
  float fy = Convert.ToSingle(y);
  float fwidth = Convert.ToSingle(width);
  float fheight = Convert.ToSingle(height);
  float fradius = Convert.ToSingle(radius); 
  fillroundrectangle(brush, fx, fy, fwidth, fheight, fradius); 
 } 
#endregion 

#region Fills a Rounded Rectangle with continuous numbers. 
 public void fillroundrectangle(System.Drawing.Brush brush, float x, float y, float width, float height, float radius) 
{ 
 RectangleF rectangle = new RectangleF(x, y, width, height); 
 GraphicsPath path = getroundedrect(rectangle, radius);
 grapher.FillPath(brush, path); 
} 
#endregion 

#region Draws a Rounded Rectangle border with integers. 
public void drawroundrectangle(System.Drawing.Pen pen, int x, int y, int width, int height, int radius) 
{
 float fx = Convert.ToSingle(x);
 float fy = Convert.ToSingle(y);
 float fwidth = Convert.ToSingle(width);
 float fheight = Convert.ToSingle(height);
 float fradius = Convert.ToSingle(radius); 
 drawroundrectangle(pen, fx, fy, fwidth, fheight, fradius); 
}

#endregion 
#region Draws a Rounded Rectangle border with continuous numbers. 
 public void drawroundrectangle(System.Drawing.Pen pen, float x, float y, float width, float height, float radius) 
 {
  RectangleF rectangle = new RectangleF(x, y, width, height);
  GraphicsPath path = getroundedrect(rectangle, radius);
  grapher.DrawPath(pen, path); 
 }
#endregion 

#region Get the desired Rounded Rectangle path. 
 private GraphicsPath getroundedrect(RectangleF baseRect, float radius) 
 { // if corner radius is less than or equal to zero, 
  // return the original rectangle 
  if( radius<=0.0F ) 
  { 
   GraphicsPath mPath = new GraphicsPath(); 
   mPath.AddRectangle(baseRect); mPath.CloseFigure(); 
   return mPath; 
  } // if the corner radius is greater than or equal to 
  // half the width, or height (whichever is shorter) 
  // then return a capsule instead of a lozenge 
  if( radius>=(Math.Min(baseRect.Width, baseRect.Height))/2.0) return getcapsule( baseRect ); 
  // create the arc for the rectangle sides and declare 
  // a graphics path object for the drawing 
  float diameter = radius * 2.0F; SizeF sizeF = new SizeF(diameter, diameter); 
  RectangleF arc = new RectangleF( baseRect.Location, sizeF );
  GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath(); 
  // top left arc 
  path.AddArc( arc, 180, 90 ); 
  // top right arc 
  arc.X = baseRect.Right-diameter; path.AddArc( arc, 270, 90 ); 
  // bottom right arc 
  arc.Y = baseRect.Bottom-diameter; 
  path.AddArc( arc, 0, 90 ); 
  // bottom left arc 
  arc.X = baseRect.Left; 
  path.AddArc( arc, 90, 90 ); 
  path.CloseFigure(); return path; 
 } 
#endregion 

#region Gets the desired Capsular path. 
 private GraphicsPath getcapsule( RectangleF baseRect ) 
 {
  float diameter; 
  RectangleF arc; 
  GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath(); 
  try 
  {
   if( baseRect.Width>baseRect.Height ) 
   { // return horizontal capsule 
    diameter = baseRect.Height; 
    SizeF sizeF = new SizeF(diameter, diameter); 
    arc = new RectangleF( baseRect.Location, sizeF ); 
    path.AddArc( arc, 90, 180); 
    arc.X = baseRect.Right-diameter; 
    path.AddArc( arc, 270, 180); 
   } 
   else if( baseRect.Width < baseRect.Height ) 
   { // return vertical capsule 
    diameter = baseRect.Width; 
    SizeF sizeF = new SizeF(diameter, diameter); 
    arc = new RectangleF( baseRect.Location, sizeF ); 
    path.AddArc( arc, 180, 180 ); 
    arc.Y = baseRect.Bottom-diameter; 
    path.AddArc( arc, 0, 180 ); 
   }
   else 
   { // return circle 
    path.AddEllipse( baseRect ); 
   } 
  }
  catch
  {
   path.AddEllipse( baseRect ); 
  } 
  finally 
  {
   path.CloseFigure();
  }
  return path; 
 }
 #endregion 

}
