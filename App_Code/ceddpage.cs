using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Collections;
using System.Text;
using System.Globalization;
using System.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;
/// <summary>
/// Summary description for ceddpage
/// </summary>

public class ceddutils : Object
{

 public static string striphtmlxmltags(string content)
 {
  return Regex.Replace(content, "<[^>]+>", "", RegexOptions.IgnoreCase | RegexOptions.Compiled);
 }

 public static string possnulltostring(Object s, String nullstr)
 {
  if ((s == Convert.DBNull) || (s == null))
   return nullstr;
  else
   return Convert.ToString(s);
 }

 public static string possnulltoformatstring(String fmt, Object d, String nullstr)
 {
  if ((d == Convert.DBNull) || (d == null))
   return nullstr;
  else
   return String.Format(fmt, d);
 }

 public static void addparameter(SqlCommand cmd, String paramname, System.Data.SqlDbType paramtype, int paramsize, ParameterDirection paramdir, Object paramval)
 {
  SqlParameter objParameter;

  objParameter = cmd.Parameters.Add(paramname, paramtype, paramsize);
  objParameter.Direction = paramdir;
  objParameter.Value = paramval;
 }

 public static void addparameters(SqlCommand cmd, String[] paramnames, Object[] paramvals, SqlDbType[] paramtypes, int[] paramsizes)
 {
  if ((paramnames.Length != paramvals.Length) | (paramnames.Length != paramtypes.Length) | (paramnames.Length != paramsizes.Length))
   throw new ArgumentException("Number of parameter names and values do not match.");

  for (int i = 0; i < paramnames.Length; i++)
   addparameter(cmd, paramnames[i], paramtypes[i], paramsizes[i], ParameterDirection.Input, paramvals[i]);
 }

 public static object getstoredprocvalue(String storedproc, String fieldname, String paramname, Object paramval, SqlDbType paramtype, int paramsize, String webconfigconn)
 {
  SqlConnection objConnection = new SqlConnection(ConfigurationManager.AppSettings[webconfigconn]);
  SqlCommand objCommand = new SqlCommand(storedproc, objConnection);
  SqlDataReader objDataReader;
  Object result;

  // Create a new command to return the data
  objCommand.CommandType = CommandType.StoredProcedure;
  addparameter(objCommand, paramname, paramtype, paramsize, ParameterDirection.Input, paramval);

  // Open the connection and execute the command to populate a datareader.
  objConnection.Open();
  try
  { objDataReader = objCommand.ExecuteReader(CommandBehavior.CloseConnection); }
  catch
  {
   objConnection.Close();
   return null;
  }

  if (objDataReader.Read() == false) result = null;
  result = objDataReader[fieldname];

  objDataReader.Close();

  return result;
 }


 public static SqlDataReader getstoredprocreader(String storedproc, String webconfigconn)
 {
  String[] paramnames = {};
  Object[] paramvals = {};
  int[] paramsizes = {};
  SqlDbType[] paramtypes = {};

  return getstoredprocreader(storedproc, paramnames, paramvals, paramtypes, paramsizes, webconfigconn);
 }
 
 public static SqlDataReader getstoredprocreader(String storedproc, String paramname, Object paramval, SqlDbType paramtype, int paramsize, String webconfigconn)
 {
  String[] paramnames = { paramname };
  Object[] paramvals = { paramval };
  int[] paramsizes = { paramsize };
  SqlDbType[] paramtypes = { paramtype };

  return getstoredprocreader(storedproc, paramnames, paramvals, paramtypes, paramsizes, webconfigconn);
 }

 public static SqlDataReader getstoredprocreader(String storedproc, String[] paramnames, Object[] paramvals, SqlDbType[] paramtypes, int[] paramsizes, String webconfigconn)
 {
  SqlConnection objconnection = new SqlConnection(ConfigurationManager.AppSettings[webconfigconn]);
  SqlCommand objcommand = new SqlCommand(storedproc, objconnection);
  SqlDataReader objdatareader;

  // Fill in data from the getreference stored proc.
  // returns everything useful from tblMain in references database
  objcommand.CommandType = CommandType.StoredProcedure;
  ceddutils.addparameters(objcommand, paramnames, paramvals, paramtypes, paramsizes);
  objconnection.Open();
  try
  {
   objdatareader = objcommand.ExecuteReader(CommandBehavior.CloseConnection);
   if (objdatareader.Read() == false) throw new Exception("No data");
  }
  catch
  {
   objconnection.Close();
   return null;
  }

  return objdatareader;
 }

 public static void execcommand(String storedproc, String[] paramnames, Object[] paramvals, SqlDbType[] paramtypes, int[] paramsizes, String webconfigconn)
 {
  SqlConnection objconnection = new SqlConnection(ConfigurationManager.AppSettings[webconfigconn]);
  SqlCommand objcommand = new SqlCommand(storedproc, objconnection);

  // Fill in data from the getreference stored proc.
  // returns everything useful from tblMain in references database
  objcommand.CommandType = CommandType.StoredProcedure;
  ceddutils.addparameters(objcommand, paramnames, paramvals, paramtypes, paramsizes);
  objconnection.Open();
  objcommand.ExecuteNonQuery();
  objconnection.Close();
 }

 public static void databindcontrol(String storedproc, String paramname, Object paramval, SqlDbType paramtype, int paramsize, BaseDataBoundControl c, String webconfigconn)
 {
  String[] paramnames = { paramname };
  Object[] paramvals = { paramval };
  int[] paramsizes = { paramsize };
  SqlDbType[] paramtypes = { paramtype };

  databindcontrol(storedproc, paramnames, paramvals, paramtypes, paramsizes, c, webconfigconn);
 }

 public static void databindcontrol(String storedproc, BaseDataBoundControl c, String webconfigconn)
 {
  String[] paramnames = { };
  Object[] paramvals = { };
  int[] paramsizes = { };
  SqlDbType[] paramtypes = { };

  databindcontrol(storedproc, paramnames, paramvals, paramtypes, paramsizes, c, webconfigconn);
 }

 public static void databindcontrol(String storedproc, String[] paramnames, Object[] paramvals, SqlDbType[] paramtypes, int[] paramsizes, BaseDataBoundControl c, String webconfigconn)
 {
  SqlConnection objConnection = new SqlConnection(ConfigurationManager.AppSettings[webconfigconn]);
  SqlCommand objCommand = new SqlCommand(storedproc, objConnection);
  SqlDataReader objDataReader;

  // Create a new command to return the data
  objCommand.CommandType = CommandType.StoredProcedure;
  addparameters(objCommand, paramnames, paramvals, paramtypes, paramsizes);

  // Open the connection and execute the command to populate a datareader.
  objConnection.Open();
  objDataReader = objCommand.ExecuteReader(CommandBehavior.CloseConnection);

  c.DataSource = objDataReader;
  c.DataBind();
  objConnection.Close();
 }

 public static void databindlistcontrol(String storedproc, String idfield, String namefield, ListControl ls, String webconfigconn)
 {
  String[] paramnames = { };
  Object[] paramvals = { };
  int[] paramsizes = { };
  SqlDbType[] paramtypes = { };

  databindlistcontrol(storedproc, paramnames, paramvals, paramtypes, paramsizes, idfield, namefield, ls, webconfigconn);
 }

 public static void databindlistcontrol(String storedproc, String[] paramnames, Object[] paramvals, SqlDbType[] paramtypes, int[] paramsizes, String idfield, String namefield, ListControl ls, String webconfigconn)
 {
  SqlConnection objConnection = new SqlConnection(ConfigurationManager.AppSettings[webconfigconn]);
  SqlCommand objCommand = new SqlCommand(storedproc, objConnection);
  SqlDataReader objDataReader;

  // Create a new command to return the data
  objCommand.CommandType = CommandType.StoredProcedure;
  addparameters(objCommand, paramnames, paramvals, paramtypes, paramsizes);

  // Open the connection and execute the command to populate a datareader.
  objConnection.Open();
  objDataReader = objCommand.ExecuteReader(CommandBehavior.CloseConnection);

  ls.DataSource = objDataReader;
  ls.DataValueField = idfield;
  ls.DataTextField = namefield;
  ls.DataBind();
  objConnection.Close();
 }

 public static void databindrepeater(String storedproc, String[] paramnames, Object[] paramvals, SqlDbType[] paramtypes, int[] paramsizes, Repeater rp, String webconfigconn)
 {
  SqlConnection objConnection = new SqlConnection( ConfigurationManager.AppSettings[webconfigconn]);
  SqlCommand objCommand = new SqlCommand(storedproc, objConnection);
  SqlDataReader objDataReader;

  // Create a new command to return the data
  objCommand.CommandType = CommandType.StoredProcedure;
  objCommand.CommandTimeout = 60;
  addparameters(objCommand, paramnames, paramvals, paramtypes, paramsizes);

  // Open the connection and execute the command to populate a datareader.
  objConnection.Open();
  objDataReader = objCommand.ExecuteReader(CommandBehavior.CloseConnection);

  rp.DataSource = objDataReader;
  rp.DataBind();
  objConnection.Close();
 }

 public static void databindrepeater(String storedproc, Repeater rp, String webconfigconn)
 {
  String[] paramnames = { };
  Object[] paramvals = { };
  SqlDbType[] paramtypes = { };
  int[] paramsizes = { };

  databindrepeater(storedproc, paramnames, paramvals, paramtypes, paramsizes, rp, webconfigconn);
 }

 public static void databindgrid(String storedproc, DataGrid dg, String webconfigconn)
 {
  String[] paramnames = { };
  Object[] paramvals = { };
  SqlDbType[] paramtypes = { };
  int[] paramsizes = { };

  databindgrid(storedproc, paramnames, paramvals, paramtypes, paramsizes, dg, webconfigconn);
 }

 public static void databindgrid(String storedproc, String[] paramnames, Object[] paramvals, SqlDbType[] paramtypes, int[] paramsizes, DataGrid dg, String webconfigconn)
 {
  SqlConnection objConnection = new SqlConnection(ConfigurationManager.AppSettings[webconfigconn]); ;
  SqlCommand objCommand = new SqlCommand(storedproc, objConnection); ;
  SqlDataReader objDataReader;

  //Create a new command to return the data
  objCommand.CommandType = CommandType.StoredProcedure;

  addparameters(objCommand, paramnames, paramvals, paramtypes, paramsizes);

  //Open the connection and execute the command to populate a datareader.
  objConnection.Open();
  objDataReader = objCommand.ExecuteReader(CommandBehavior.CloseConnection);

  dg.DataSource = objDataReader;
  dg.DataBind();
  objConnection.Close();
 }

 public static void databindgridview(String storedproc, GridView dg, String webconfigconn)
 {
  String[] paramnames = { };
  Object[] paramvals = { };
  SqlDbType[] paramtypes = { };
  int[] paramsizes = { };

  databindgridview(storedproc, paramnames, paramvals, paramtypes, paramsizes, dg, webconfigconn);
 }

 public static void databindgridview(String storedproc, String[] paramnames, Object[] paramvals, SqlDbType[] paramtypes, int[] paramsizes, GridView dg, String webconfigconn)
 {
  SqlConnection objConnection = new SqlConnection(ConfigurationManager.AppSettings[webconfigconn]); ;
  SqlCommand objCommand = new SqlCommand(storedproc, objConnection); ;
  SqlDataReader objDataReader;

  //Create a new command to return the data
  objCommand.CommandType = CommandType.StoredProcedure;

  addparameters(objCommand, paramnames, paramvals, paramtypes, paramsizes);

  //Open the connection and execute the command to populate a datareader.
  objConnection.Open();
  objDataReader = objCommand.ExecuteReader(CommandBehavior.CloseConnection);

  dg.DataSource = objDataReader;
  dg.DataBind();
  objConnection.Close();
 }

 public static void databinddatalist(String storedproc, DataList dl, String webconfigconn)
 {
  String[] paramnames = { };
  Object[] paramvals = { };
  SqlDbType[] paramtypes = { };
  int[] paramsizes = { };

  databinddatalist(storedproc, paramnames, paramvals, paramtypes, paramsizes, dl, webconfigconn);
 }

 public static void databinddatalist(String storedproc, String[] paramnames, Object[] paramvals, SqlDbType[] paramtypes, int[] paramsizes, DataList dl, String webconfigconn)
 {
  SqlConnection objConnection = new SqlConnection(ConfigurationManager.AppSettings[webconfigconn]); ;
  SqlCommand objCommand = new SqlCommand(storedproc, objConnection); ;
  SqlDataReader objDataReader;

  //Create a new command to return the data
  objCommand.CommandType = CommandType.StoredProcedure;

  addparameters(objCommand, paramnames, paramvals, paramtypes, paramsizes);

  //Open the connection and execute the command to populate a datareader.
  objConnection.Open();
  objDataReader = objCommand.ExecuteReader(CommandBehavior.CloseConnection);

  dl.DataSource = objDataReader;
  dl.DataBind();
  objConnection.Close();
 }

 public static Boolean savePicture(FileUpload pic, String filenamenoextension)
 {
  // save the picture and icon now we have the id
  // validate the picture (high security!)
  Boolean fileok = false;
  String fileextension = System.IO.Path.GetExtension(pic.FileName).ToLower();

  if (pic.HasFile == true)
  {
   String[] allowedExtensions = { ".gif", ".png", ".jpeg", ".jpg" };
   for (int i = 0; i < allowedExtensions.Length; i++)
   {
    if (fileextension == allowedExtensions[i]) fileok = true;
   }

   if (fileok == false)
   {
    //lpicerror.Text = "Please specify a valid picture (.gif, .png, .jpeg, .jpg)";
    return false;
   }

   pic.SaveAs(filenamenoextension + fileextension);
   return true;
  }
  return false;
 }

 public static string hashtabletostring(Hashtable s, String separator)
 {
  String r = "";
  IDictionaryEnumerator id = s.GetEnumerator();
  while (id.MoveNext())
  {
   r = r + id.Key.ToString() + " - " + id.Value.ToString() + separator;
  }

  return r;
 }

 public static string replaceextendedchars(String s, Hashtable asciilookup)
 {
  // This StringBuilder holds the output results.
  StringBuilder sb = new StringBuilder();

  // Use the enumerator returned from GetTextElementEnumerator 
  // method to examine each real character.
  TextElementEnumerator charEnum = StringInfo.GetTextElementEnumerator(s);
  while (charEnum.MoveNext())
  {
   //sb.Append(char.ConvertToUtf32(charEnum.GetTextElement(), 0).ToString());
   //sb.Append(", ");
   if (asciilookup.ContainsKey(char.ConvertToUtf32(charEnum.GetTextElement(), 0)))
    sb.Append(asciilookup[char.ConvertToUtf32(charEnum.GetTextElement(), 0)]);
   else sb.Append(charEnum.GetTextElement());
  }

  return sb.ToString();
 }

 public static Hashtable getextendedcharacterlookup()
 {
  // might want to have this as global type variable to avoid creating it every time it is needed
  // might also want a page showing these with a bit of explanation
  Hashtable n = new Hashtable();
  n.Add(160, " ");
  n.Add(161, "!");
  n.Add(162, "c");
  n.Add(163, "£");
  n.Add(165, "y");
  n.Add(166, "|");
  n.Add(167, "s");
  n.Add(168, "\"");
  n.Add(169, "c");
  n.Add(170, "a");
  n.Add(171, "<<");
  n.Add(172, "-");
  n.Add(173, "");
  n.Add(174, "r");
  n.Add(175, "-");
  n.Add(176, "o");
  n.Add(177, "+-");
  n.Add(178, "2");
  n.Add(179, "3");
  n.Add(180, "'");
  n.Add(181, "u");
  n.Add(182, "n");
  n.Add(183, ".");
  n.Add(184, ",");
  n.Add(185, "1");
  n.Add(186, "o");
  n.Add(187, ">>");
  n.Add(188, "1/4");
  n.Add(189, "1/2");
  n.Add(190, "3/4");
  n.Add(191, "?");
  n.Add(192, "A");
  n.Add(193, "A");
  n.Add(194, "A");
  n.Add(195, "A");
  n.Add(196, "A");
  n.Add(197, "A");
  n.Add(198, "AE");
  n.Add(199, "C");
  n.Add(200, "E");
  n.Add(201, "E");
  n.Add(202, "E");
  n.Add(203, "E");
  n.Add(204, "I");
  n.Add(205, "I");
  n.Add(206, "I");
  n.Add(207, "I");
  n.Add(208, "D");
  n.Add(209, "N");
  n.Add(210, "O");
  n.Add(211, "O");
  n.Add(212, "O");
  n.Add(213, "O");
  n.Add(214, "O");
  n.Add(215, "x");
  n.Add(216, "O");
  n.Add(217, "U");
  n.Add(218, "U");
  n.Add(219, "U");
  n.Add(220, "U");
  n.Add(221, "Y");
  n.Add(222, "P");
  n.Add(223, "b");
  n.Add(224, "a");
  n.Add(225, "a");
  n.Add(226, "a");
  n.Add(227, "a");
  n.Add(228, "a");
  n.Add(229, "a");
  n.Add(230, "ae");
  n.Add(231, "c");
  n.Add(232, "e");
  n.Add(233, "e");
  n.Add(234, "e");
  n.Add(235, "e");
  n.Add(236, "i");
  n.Add(237, "i");
  n.Add(238, "i");
  n.Add(239, "i");
  n.Add(240, "o");
  n.Add(241, "n");
  n.Add(242, "o");
  n.Add(243, "o");
  n.Add(244, "o");
  n.Add(245, "o");
  n.Add(246, "o");
  n.Add(247, "/");
  n.Add(248, "o");
  n.Add(249, "u");
  n.Add(250, "u");
  n.Add(251, "u");
  n.Add(252, "u");
  n.Add(253, "y");
  n.Add(254, "p");
  n.Add(255, "y");
  n.Add(402, "f");
  n.Add(913, "a");
  n.Add(914, "B");
  n.Add(915, "L");
  n.Add(916, "");
  n.Add(917, "e");
  n.Add(918, "z");
  n.Add(919, "h");
  n.Add(920, "o");
  n.Add(921, "i");
  n.Add(922, "k");
  n.Add(923, "");
  n.Add(924, "m");
  n.Add(925, "n");
  n.Add(926, "e");
  n.Add(927, "o");
  n.Add(928, "pi");
  n.Add(929, "p");
  n.Add(931, "e");
  n.Add(932, "t");
  n.Add(933, "y");
  n.Add(934, "o");
  n.Add(935, "x");
  n.Add(936, "y");
  n.Add(937, "o");
  n.Add(945, "a");
  n.Add(946, "b");
  n.Add(947, "y");
  n.Add(948, "d");
  n.Add(949, "e");
  n.Add(950, "z");
  n.Add(951, "n");
  n.Add(952, "o");
  n.Add(953, "i");
  n.Add(954, "k");
  n.Add(955, "n");
  n.Add(956, "u");
  n.Add(957, "nu");
  n.Add(958, "e");
  n.Add(959, "o");
  n.Add(960, "pi");
  n.Add(961, "p");
  n.Add(962, "c");
  n.Add(963, "o");
  n.Add(964, "t");
  n.Add(965, "u");
  n.Add(966, "y");
  n.Add(967, "x");
  n.Add(968, "y");
  n.Add(969, "w");
  n.Add(977, "0");
  n.Add(978, "y");
  n.Add(982, "pi");
  n.Add(8226, "*");
  n.Add(8230, "...");
  n.Add(8242, "'");
  n.Add(8243, "\"");
  n.Add(8254, "_");
  n.Add(8260, "/");
  n.Add(8472, "^");
  n.Add(8465, "I");
  n.Add(8476, "R");
  n.Add(8482, "tm");
  n.Add(8501, "n");
  n.Add(8592, "<");
  n.Add(8594, ">");
  n.Add(8595, "");
  n.Add(8596, "<>");
  n.Add(8629, "");
  n.Add(8656, "<");
  n.Add(8593, "");
  n.Add(8657, "");
  n.Add(8658, ">");
  n.Add(8659, "");
  n.Add(8660, "<>");
  n.Add(8704, "A");
  n.Add(8706, "d");
  n.Add(8707, "e");
  n.Add(8709, "o");
  n.Add(8711, "");
  n.Add(8712, "e");
  n.Add(8715, "e");
  n.Add(8719, "n");
  n.Add(8721, "e");
  n.Add(8722, "-");
  n.Add(8727, "*");
  n.Add(8730, "y");
  n.Add(8733, "");
  n.Add(8734, "");
  n.Add(8736, "");
  n.Add(8743, "");
  n.Add(8744, "");
  n.Add(8745, "");
  n.Add(8747, "f");
  n.Add(8756, "");
  n.Add(8764, "~");
  n.Add(8773, "~");
  n.Add(8776, "~");
  n.Add(8800, "");
  n.Add(8801, "=");
  n.Add(8804, "<=");
  n.Add(8805, ">=");
  n.Add(8834, "c");
  n.Add(8835, "c");
  n.Add(8836, "!");
  n.Add(8838, "=");
  n.Add(8839, "=");
  n.Add(8853, "+");
  n.Add(8855, "o");
  n.Add(8869, "|");
  n.Add(8901, ".");
  n.Add(8968, ",");
  n.Add(8969, "");
  n.Add(8970, "");
  n.Add(8971, "");
  n.Add(9001, "<");
  n.Add(9002, ">");
  n.Add(9674, "<>");
  n.Add(9824, "");
  n.Add(9827, "club");
  n.Add(9829, "heart");
  n.Add(9830, "diamond");
  n.Add(34, "\"");
  n.Add(38, "&");
  n.Add(60, "<");
  n.Add(62, ">");
  n.Add(338, "OE");
  n.Add(339, "oe");
  n.Add(352, "S");
  n.Add(353, "s");
  n.Add(376, "Y");
  n.Add(710, "^");
  n.Add(8194, " ");
  n.Add(8195, " ");
  n.Add(8201, " ");
  n.Add(8204, " ");
  n.Add(8205, " ");
  n.Add(8206, "<>");
  n.Add(8207, "><");
  n.Add(8211, "-");
  n.Add(8212, "-");
  n.Add(8216, "'");
  n.Add(8217, "'");
  n.Add(8218, ",");
  n.Add(8220, "\"");
  n.Add(8221, "\"");
  n.Add(8222, "\"");
  n.Add(8224, "+");
  n.Add(8225, "+");
  n.Add(8249, "<");
  n.Add(8250, ">");
  n.Add(8364, "E");

  return n;
 }

 public static void enforcesingleclick(Button b)
 {
  // look out for a double call to this proc 
  if (b.Parent.FindControl(b.ID + "process") != null) return;

  // add another button to show a message when submitting
  Button b2 = new Button();
  b2.Text = "Processing ...";
  b2.Style.Add("display", "none");
  b2.Enabled = false;
  b2.ID = b.ID + "process";
  int indx = b.Parent.Controls.IndexOf(b);
  b.Parent.Controls.AddAt(indx, b2);

  // if the button does cause validation then the validation might fail in which case we wouldn't want to hide the button
  // hide the button
  String cstext = "if (typeof(ValidatorOnSubmit) == 'function' && ValidatorOnSubmit() == false) return false; else { try { document.getElementById('" + b.ClientID + "').style.display = 'none'; document.getElementById('" + b2.ClientID + "').style.display = 'inline'} catch(e){}} ";
  //b.Page.RegisterOnSubmitStatement(b.ID, cstext);
  b.Page.ClientScript.RegisterOnSubmitStatement(b.Page.GetType(), b.ID, cstext);
 }

 public static ImageCodecInfo getencoderinfo(string mimeType)
 {
  int i;
  ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
  for (i = 0; i < encoders.Length; i++)
  {
   if (encoders[i].MimeType == mimeType)
   {
    return encoders[i];
   }
  }

  return null;
 }

 public static System.Drawing.Image generatethumbnail(System.Drawing.Image oimg, int w, int h)
 {
  System.Drawing.Image othumbnail = new Bitmap(w, h);
  Graphics ographic = Graphics.FromImage(othumbnail);
  ographic.CompositingQuality = CompositingQuality.HighQuality;
  ographic.SmoothingMode = SmoothingMode.HighQuality;
  ographic.InterpolationMode = InterpolationMode.HighQualityBicubic;

  Rectangle orectangle = new Rectangle(0, 0, w, h);
  ographic.DrawImage(oimg, orectangle);

  return othumbnail;
 }
}