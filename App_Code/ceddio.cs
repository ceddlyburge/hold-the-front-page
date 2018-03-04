using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Web.Hosting;


/// <summary>
/// Summary description for ceddio
/// </summary>
public class ceddio
{
	public ceddio()
	{
		//
		// TODO: Add constructor logic here
		//
	}

 public static article loadarticle(string filename)
 {
  XmlSerializer serializer;
  article a;
  FileStream fs;
  TextReader tr;

  serializer = new XmlSerializer(typeof(article));
  fs = new FileStream(HostingEnvironment.ApplicationPhysicalPath + filename, FileMode.Open);
  tr = new StreamReader(fs);
  try
  {
   a = (article)serializer.Deserialize(tr);
  }
  finally
  {
   // need to do this so that the file handle is released and the file can be deleted, modified etc. One of the downsides of garbage collection!
   tr = null;
   fs = null;
   GC.Collect();
  }

  return a;
 }

 public static pagelayout loadpagelayout(string filename)
 {
  XmlSerializer serializer;
  FileStream fs;
  TextReader tr;
  pagelayout pl;

  serializer = new XmlSerializer(typeof(pagelayout));
  fs = new FileStream(HostingEnvironment.ApplicationPhysicalPath + filename, FileMode.Open);
  tr = new StreamReader(fs);
  try
  {
   pl = (pagelayout)serializer.Deserialize(tr);
  }
  finally
  {
   // need to do this so that the file handle is released and the file can be deleted, modified etc. One of the downsides of garbage collection!
   tr = null;
   fs = null;
   GC.Collect();
  }

  return pl;
 }
}
