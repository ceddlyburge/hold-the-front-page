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

public partial class review : System.Web.UI.Page
{
 protected sessionpagelayout spl;
 protected pagelayout pl = null;

 public int reviewwidth
 {
  get { return (int)((float)pl.backgroundwidth * scaletofit); }
 }

 public float scaletofit
 {
  get { return 480f / (float)pl.backgroundheight; }
 }
 
 protected void Page_Load(object sender, EventArgs e)
    {
     int i;
     HyperLink a;
     Literal l;

     try
     {
      spl = (sessionpagelayout)Session["sessionpagelayout"];
      pl = ceddio.loadpagelayout(spl.pagelayoutfilename);
     }
     catch
     {
      Response.Redirect("default.aspx", true);
     }

     DataBind();

     // add links to customise the various parts of the page
     // step 2 - choose articles
     // and step 3 - customise articles
     for (i = 0; i < pl.articles.Count; i++)
     {
      a = new HyperLink();
      a.Text = "Change the " + pl.articles[i].prompt;
      a.NavigateUrl = "~/choose_articles.aspx?article=" + Convert.ToString(i) + "&return=true";
      phchoosearticles.Controls.Add(a);

      if (i < pl.articles.Count - 1)
      {
       l = new Literal();
       l.Text = "<br />";
       phchoosearticles.Controls.Add(l);
      }

      a = new HyperLink();
      a.Text = "Customise the " + pl.articles[i].prompt;
      a.NavigateUrl = "~/customise_articles.aspx?article=" + Convert.ToString(i) + "&return=true";
      phcustomisearticles.Controls.Add(a);

      if (i < pl.articles.Count - 1)
      {
       l = new Literal();
       l.Text = "<br />";
       phcustomisearticles.Controls.Add(l);
      }
     }

     // and step 4 - choose pictures
     for (i = 0; i < pl.pictures.Count; i++)
     {
      a = new HyperLink();
      a.Text = "Change the " + pl.pictures[i].prompt;
      a.NavigateUrl = "~/choose_pictures.aspx?picture=" + Convert.ToString(i) + "&return=true";
      phchoosepictures.Controls.Add(a);

      if (i < pl.pictures.Count - 1)
      {
       l = new Literal();
       l.Text = "<br />";
       phchoosepictures.Controls.Add(l);
      }
     }

     /*
       sessionpicture sp = new sessionpicture();
       sp.name = "main";
       sp.filename = "wahey.jpg";
       sp.thumbnailfilename = "wahey.jpg";
       sp.borderfilename = "border3.png";
       sp.imageheight = 536;
       sp.imagewidth = 800;
       sp.thumbnailheight = 800;
       sp.thumbnailwidth = 800;
       sp.offsetx = 270;
       sp.offsety = 0;
       sp.setzoom(1.2f);
       //sp.setzoom(3f);
       //sp.setzoom(1.5f);
       sessionpictures sps = new sessionpictures();
       sps.Add(sp);
       sps.setcookie(Response);

       sessionarticles sas = new sessionarticles();
       sas.addsessionarticle("main", "\\articles\\testarticle.xml");
       sas.setcookie(Response);

       sessionpagelayout spl = new sessionpagelayout();
       spl.pagelayoutfilename = "\\layouts\\testlayout3.xml";
       spl.setcookie(Response);
  
       Global.getcookiedata(Request, Session);

       articlestyles a = new articlestyles();
       //a.brush = new SolidBrush(Color.Black);
    
       textstyle ts = new textstyle();
       ts.style = textstyletype.headline;
       ts.fontdef.family = "Arial";
       ts.fontdef.style = FontStyle.Regular;
       ts.fontdef.size = 8;
       a.text.Add(ts);  

       paragraphstyle ps = new paragraphstyle();
       ps.style = paragraphstyletype.headline;  
       ps.justification = paragraphjustification.justified;
       a.paragraph.Add(ps);

       // serialise the object
       XmlSerializer serializer = new XmlSerializer(typeof(articlestyles));
       StringWriter tw = new StringWriter();
       serializer.Serialize(tw, a);
       Literal1.Text = Server.HtmlEncode(tw.ToString());

       // unserialise it, just to make sure that it works
       StringReader sr = new StringReader(Server.HtmlDecode(Literal1.Text));
       a = (articlestyles)serializer.Deserialize(sr);

       // serialise it again so we can see the results
       tw = new StringWriter();
       serializer.Serialize(tw, a);
       Literal2.Text = Server.HtmlEncode(tw.ToString());

       articlelayout al = new articlelayout();
       articlelayoutpart alp = new articlelayoutpart();
       al.layoutparts.Add(alp);

       alp.parttypes.Add(articleparttype.body);  
   
       al.styles.text.Add(ts);
       al.styles.paragraph.Add(ps);

       pagelayout pl = new pagelayout();
       pl.articles.Add(al);
       pl.backgroundimagefilename = "frontpage.jpg";

       // serialise the object
  
       serializer = new XmlSerializer(typeof(pagelayout));
       tw = new StringWriter();
       serializer.Serialize(tw, pl);
       Literal3.Text = Server.HtmlEncode(tw.ToString());

       // unserialise it, just to make sure that it works
       sr = new StringReader(Server.HtmlDecode(Literal3.Text));
       pl = (pagelayout)serializer.Deserialize(sr);

       // serialise it again so we can see the results
       tw = new StringWriter();
       serializer.Serialize(tw, pl);
       Literal4.Text = Server.HtmlEncode(tw.ToString());
  
       sometextstatic sts = new sometextstatic();
       sts.statictext = "Good morning ";
       sts.style = textstyletype.intro;

       sometextreplace str = new sometextreplace();
       str.initialtext = "cedd";
       str.maxlength = 10;
       str.prompt = "First name";
       str.style = textstyletype.intro;

       paragraph p = new paragraph();
       p.style = paragraphstyletype.intro;  
       p.textparts.Add(sts);
       p.textparts.Add(str);

       article art = new article();
       articlepart artp = new articlepart();
       articlepart artp2 = new articlepart();
       artp2.parttype = articleparttype.subhead; 
       art.Add(artp);
       art.Add(artp2);
  
       artp.paragraphs.Add(p);
       artp.finetune.Add(p);
       artp.finetune.Add(p);

       artp2.paragraphs.Add(p);
       artp2.finetune.Add(p);
       artp2.finetune.Add(p);

       // serialise the article 
       tw = new StringWriter();
       serializer = new XmlSerializer(typeof(article));
       serializer.Serialize(tw, art);
       Literal5.Text = Server.HtmlEncode(tw.ToString());

       // now deserialise it to make sure its the same!
       sr = new StringReader(Server.HtmlDecode(Literal5.Text));
       art = (article)serializer.Deserialize(sr);

       // serialise it again so we can see the results
       tw = new StringWriter();
       serializer.Serialize(tw, art);
       Literal6.Text = Server.HtmlEncode(tw.ToString());

       Bitmap bm = new Bitmap(40, 40);

       Graphics g = Graphics.FromImage(bm); 
       GraphicsPath ph = new GraphicsPath();
       Font f = new Font("Arial", 16);
       ph.AddString("Cedditisj" , f.FontFamily, (int)f.Style, f.Size, new Point(0, 0), StringFormat.GenericTypographic);
       ph.Flatten(new Matrix(), (float)0.01); 

       for (int i = 0; i < ph.PathPoints.Length; i++)
       {
        Literal6.Text += "<BR>" + ph.PathPoints[i].X.ToString() + ", " + ph.PathPoints[i].Y.ToString() + ", " + ph.PathTypes[i].ToString(); 
       }

       GraphicsPathIterator gpi = new GraphicsPathIterator(ph);

       Literal6.Text += "<br>" + ph.GetBounds().ToString() + "<br>";
       Literal6.Text += g.MeasureString("C", f, 1000, StringFormat.GenericTypographic).ToString()  + "<br>";
       Literal6.Text += gpi.SubpathCount + "<br>";

       foreach (FontFamily ff in FontFamily.Families)
       {
           if (ff.IsStyleAvailable(FontStyle.Regular))
           {
               Literal7.Text += ff.Name+"<br>";
           }
       }
       */
 
    }
   }
