using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.SessionState;
using System.Security;
using System.Security.Principal;
using System.Web.Security;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Web.Hosting;

 /// <summary>
 /// Summary description for Global.
 /// </summary>
 public class Global : System.Web.HttpApplication
 {
  /// <summary>
  /// Required designer variable.
  /// </summary>
  private System.ComponentModel.IContainer components = null;


  public static void getcookiedata(HttpRequest Request, HttpSessionState Session)
  {
   XmlSerializer serializer;

   // get the session layout if not already in session
   if (Session["sessionpagelayout"] == null)
   {
    sessionpagelayout spl;
    try
    {
     serializer = new XmlSerializer(typeof(sessionpagelayout));
     StringReader sr = new StringReader(HttpUtility.UrlDecode(Request.Cookies["pagelayout"].Value));
     spl = (sessionpagelayout)serializer.Deserialize(sr);
     Session["sessionpagelayout"] = spl;
    }
    catch { }
   }

   // get the session layout if not already in session
   if (Session["sessioncustomisation"] == null)
   {
    sessioncustomisations sc;
    try
    {
     serializer = new XmlSerializer(typeof(sessioncustomisations));
     StringReader sr = new StringReader(HttpUtility.UrlDecode(Request.Cookies["customisation"].Value));
     sc = (sessioncustomisations)serializer.Deserialize(sr);
     Session["sessioncustomisation"] = sc;
    }
    catch { }
   }

   // get the session pictures if not already in session
   if (Session["sessionarticles"] == null)
   {
    sessionarticles sas;
    try
    {
     serializer = new XmlSerializer(typeof(sessionarticles));
     StringReader sr = new StringReader(HttpUtility.UrlDecode(Request.Cookies["articles"].Value));
     sas = (sessionarticles)serializer.Deserialize(sr);
     Session["sessionarticles"] = sas;
    }
    catch { }
   }

   // get the session pictures if not already in session
   if (Session["sessionpictures"] == null)
   {
    sessionpictures sps;
    try
    {
     serializer = new XmlSerializer(typeof(sessionpictures));
     StringReader sr = new StringReader(HttpUtility.UrlDecode(Request.Cookies["pictures"].Value));
     sps = (sessionpictures)serializer.Deserialize(sr);
     Session["sessionpictures"] = sps;
    }
    catch { }
   }
  }

  public Global()
  {
   InitializeComponent();
  }

  protected void Application_Start(Object sender, EventArgs e)
  {

  }

  protected void Session_Start(Object sender, EventArgs e)
  {
   Global.getcookiedata(Request, Session);
  }

  protected void Application_BeginRequest(Object sender, EventArgs e)
  {
  }

  protected void Application_EndRequest(Object sender, EventArgs e)
  {
  }

  protected void Application_AuthenticateRequest(Object sender, EventArgs e)
  {
  }

  protected void Application_Error(Object sender, EventArgs e)
  {
  }

  protected void Session_End(Object sender, EventArgs e)
  {
   // could save session stuff to the cookie here
  }

  protected void Application_End(Object sender, EventArgs e)
  {

  }

  #region Web Form Designer generated code
  /// <summary>
  /// Required method for Designer support - do not modify
  /// the contents of this method with the code editor.
  /// </summary>
  private void InitializeComponent()
  {
   this.components = new System.ComponentModel.Container();
  }
  #endregion
 }


