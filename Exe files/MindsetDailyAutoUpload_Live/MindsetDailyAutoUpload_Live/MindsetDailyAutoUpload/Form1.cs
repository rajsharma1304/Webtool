using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Xml;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Xml.Linq;
using System.Data.OleDb;
using ClosedXML.Excel;
using System.Net;
using System.IO.Compression;
using System.Net.Mail;
using WinSCP;

namespace MindsetDailyAutoUpload
{
    public partial class Form1 : Form
    {

        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString);
        SqlCommand sql = new SqlCommand();
        public Form1()
        {
            InitializeComponent();
            Load += Form1_Shown;

        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            button1.PerformClick();
            Application.Exit();
        }

        private void UpdateMindsetDB(int eventType)
        {
            try
            {
                connection.Open();
                sql = new SqlCommand("proctruncate", connection);
                sql.CommandTimeout = 800;
                sql.CommandType = CommandType.StoredProcedure;
                sql.Parameters.Add("@eventType", SqlDbType.Int).Value = eventType;
                sql.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("UpdateMindsetDB " + eventType + " Event Type " + ex.ToString());
                // SendMail("UpdateMindsetDB", ex.Message.ToString());18/7/2018
                Application.Exit();
            }
        }

        private void ExcelComparisionUpload(string Excel, string table)
        {
            try
            {
                String s1 = "";
                s1 = @"provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + Excel + @"';Extended Properties='Excel 12.0 Xml;IMEX=1'";
                string[] sheetname = GetExcelSheetNames(Excel, s1);
                OleDbConnection oledbcon = new OleDbConnection(s1);
                DataSet ds = new DataSet();
                OleDbCommand MyCommand = new OleDbCommand("select * from [" + sheetname[0] + "]", oledbcon);
                OleDbDataAdapter sda = new OleDbDataAdapter(MyCommand);
                sda.Fill(ds);
                SqlBulkCopy sqlBulk = new SqlBulkCopy(connection);
                sqlBulk.DestinationTableName = table;

                foreach (DataColumn col in ds.Tables[0].Columns)
                {
                    if (col.ColumnName.ToString() == "responseid")
                    {
                        sqlBulk.ColumnMappings.Add("responseid", "responseid");
                    }
                    else if (col.ColumnName.ToString() == "respid")
                    {
                        sqlBulk.ColumnMappings.Add("respid", "respid");
                    }
                    else if (col.ColumnName.ToString().Contains("Login_Page"))
                    {
                        sqlBulk.ColumnMappings.Add(col.ColumnName, "Login_Page");
                    }
                    else if (col.ColumnName.ToString().Contains("userid"))
                    {
                        sqlBulk.ColumnMappings.Add(col.ColumnName, "Login_Page");
                    }
                    else if (col.ColumnName.ToString().Contains("Q8"))
                    {
                        sqlBulk.ColumnMappings.Add(col.ColumnName, "HidQ30Final");
                    }
                    else if (col.ColumnName.ToString().Contains("HidQ40Final"))
                    {
                        sqlBulk.ColumnMappings.Add(col.ColumnName, "HidQ30Final");
                    }
                    else if (col.ColumnName.ToString().Contains("HidQ30Final"))
                    {
                        sqlBulk.ColumnMappings.Add(col.ColumnName, "HidQ30Final");
                    }
                    else if (col.ColumnName.ToString().Contains("hQ35Final"))
                    {
                        sqlBulk.ColumnMappings.Add(col.ColumnName, "HidQ30Final");
                    }
                    else if (col.ColumnName.ToString() == "status")
                    {
                        sqlBulk.ColumnMappings.Add("status", "status");
                    }
                    else if (col.ColumnName.ToString() == "interview_end")
                    {
                        sqlBulk.ColumnMappings.Add("interview_end", "interview_end");
                    }
                    else if (col.ColumnName.ToString() == "interview_start")
                    {
                        sqlBulk.ColumnMappings.Add("interview_start", "interview_start");
                    }
                }
                connection.Open();
                sqlBulk.BulkCopyTimeout = 1000;
                sqlBulk.WriteToServer(ds.Tables[0]);
                connection.Close();
            }
            catch (Exception ex)
            {
                //  SendMail("ExcelComparisionUpload", ex.Message.ToString()); 18/7/2018
                Application.Exit();
            }
            finally
            {

            }
        }

        private string[] GetExcelSheetNames(object newXmlfile1, string s1)
        {
            OleDbConnection objConn = null;
            System.Data.DataTable dt = null;
            try
            {
                // Connection String. Change the excel file to the file you
                // will search.
                String connString = s1;
                // Create connection object by using the preceding connection string.
                objConn = new OleDbConnection(connString);
                // Open connection with the database.
                objConn.Open();
                // Get the data table containg the schema guid.
                dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                if (dt == null)
                {
                    return null;
                }

                String[] excelSheets = new String[dt.Rows.Count];
                int i = 0;

                // Add the sheet name to the string array.
                foreach (DataRow row in dt.Rows)
                {
                    excelSheets[i] = row["TABLE_NAME"].ToString();
                    i++;
                }
                return excelSheets;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                // Clean up.
                if (objConn != null)
                {
                    objConn.Close();
                    objConn.Dispose();
                }
                if (dt != null)
                {
                    dt.Dispose();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // MessageBox.Show("We are in");

                // connection.Open();
                //connection.Close();

                string ZipFilepath = @System.Configuration.ConfigurationManager.AppSettings["ZipFilepath"] + "\\";

                string[] filepaths = Directory.GetFiles(ZipFilepath);
                foreach (string filePath in filepaths)
                {
                    File.Delete(filePath);
                }

                DownloadFilesFTP("INTL");
                DownloadFilesFTP("EURO");
                DownloadFilesFTP("BOE");
                DownloadFilesFTP("USDomestic");

                progressBar1.Minimum = 0;
                progressBar1.Maximum = 100;
                lblplswt.Visible = true;
                Application.DoEvents();

                string Excel2 = @System.Configuration.ConfigurationManager.AppSettings["Filepath"] + "/" + "DailyExport_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx";
                //string Excel3 = @System.Configuration.ConfigurationManager.AppSettings["Filepath2"] + "/" + "DailyExport_" + DateTime.Now.AddDays(-1).ToString("ddMMyyyy") + ".xlsx";
                string Excel4 = @System.Configuration.ConfigurationManager.AppSettings["Filepath2"] + "/" + "DailyExport_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx";
                //string Excel5 = @System.Configuration.ConfigurationManager.AppSettings["Filepath3"] + "/" + "DailyExport_" + DateTime.Now.AddDays(-1).ToString("ddMMyyyy") + ".xlsx";
                string Excel6 = @System.Configuration.ConfigurationManager.AppSettings["Filepath3"] + "/" + "DailyExport_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx";

                string Excel7 = @System.Configuration.ConfigurationManager.AppSettings["Filepath4"] + "/" + "DailyExport_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx";


                // MessageBox.Show("CHECK FILE STARTED");

                //checkfileexist(Excel1);
                checkfileexist(Excel2);
                //checkfileexist(Excel3);
                checkfileexist(Excel4);
                //checkfileexist(Excel5);
                checkfileexist(Excel6);

                checkfileexist(Excel7);

                // MessageBox.Show("International Auto upload Started");

                // International Auto Data Updated
                sql = new SqlCommand("proctruncate", connection);
                sql.CommandType = CommandType.StoredProcedure;
                sql.Parameters.Add("@eventType", SqlDbType.Int).Value = 0;
                connection.Open();
                sql.ExecuteNonQuery();
                progressBar1.Value = 10;
                Application.DoEvents();
                connection.Close();

                //MessageBox.Show("International Comp Started");

                //ExcelComparisionUpload(Excel1, "Excel1");
                ExcelComparisionUpload(Excel2, "Excel2");
                progressBar1.Value = 20;
                Application.DoEvents();
                progressBar1.Value = 30;
                //calling proctruncate procedure
                //MessageBox.Show("International UpdateMindsetDB Started");
                UpdateMindsetDB(1);
                // MessageBox.Show("International UpdateMindsetDB Completed");

                // MessageBox.Show("Europe Auto upload Started");

                // Europe Auto Data Updated
                sql = new SqlCommand("proctruncate", connection);
                sql.CommandType = CommandType.StoredProcedure;
                sql.Parameters.Add("@eventType", SqlDbType.Int).Value = 0;
                connection.Open();
                sql.ExecuteNonQuery();
                progressBar1.Value = 40;
                Application.DoEvents();
                connection.Close();

                // MessageBox.Show("Europe Comp Auto upload Started");

                //ExcelComparisionUpload(Excel3, "Excel1");
                ExcelComparisionUpload(Excel4, "Excel2");
                progressBar1.Value = 50;
                Application.DoEvents();
                UpdateMindsetDB(2);
                progressBar1.Value = 60;

                Application.DoEvents();

                // MessageBox.Show("Aircraft Auto upload Started");

                // Aircraft Boeing Auto Data Updated
                sql = new SqlCommand("proctruncate", connection);
                sql.CommandType = CommandType.StoredProcedure;
                sql.Parameters.Add("@eventType", SqlDbType.Int).Value = 0;
                connection.Open();
                sql.ExecuteNonQuery();
                progressBar1.Value = 70;
                Application.DoEvents();
                connection.Close();

                //ExcelComparisionUpload(Excel5, "Excel1");
                ExcelComparisionUpload(Excel6, "Excel2");
                progressBar1.Value = 80;
                Application.DoEvents();
                UpdateMindsetDB(3);
                progressBar1.Value = 90;
                Application.DoEvents();

                // Domestic Auto Data Updated
                sql = new SqlCommand("proctruncate", connection);
                sql.CommandType = CommandType.StoredProcedure;
                sql.Parameters.Add("@eventType", SqlDbType.Int).Value = 0;
                connection.Open();
                sql.ExecuteNonQuery();
                progressBar1.Value = 70;
                Application.DoEvents();
                connection.Close();

                //ExcelComparisionUpload(Excel5, "Excel1");
                ExcelComparisionUpload(Excel7, "Excel2");
                progressBar1.Value = 80;
                Application.DoEvents();
                UpdateMindsetDB(4);
                progressBar1.Value = 90;
                Application.DoEvents();


                sql = new SqlCommand("proctruncate", connection);
                sql.CommandType = CommandType.StoredProcedure;
                sql.Parameters.Add("@eventType", SqlDbType.Int).Value = 0;
                connection.Open();
                sql.ExecuteNonQuery();
                connection.Close();


                //MessageBox.Show("please wait for a while, as the Consolidate data files are downloading...");
                // backgroundWorker1.RunWorkerAsync();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://80.80.229.30/International/UserDetailsServices.svc/CreateCacheFiles/US");
                WebResponse response = request.GetResponse();
                response.Close();

                //special user service for han1 login (hanoi).

                request = (HttpWebRequest)WebRequest.Create(@"http://80.80.229.30/AutoUpload/UserDetailsServices.svc/CacheFileAirportAirlineList/US");
                response = request.GetResponse();
                response.Close();

                request = (HttpWebRequest)WebRequest.Create(@"http://80.80.229.30/Europe/UserDetailsServices.svc/CreateCacheFiles/EUR");
                response = request.GetResponse();
                response.Close();

                request = (HttpWebRequest)WebRequest.Create(@"http://80.80.229.30/aircraft/UserDetailsServices.svc/CreateCacheFiles/AIR");
                response = request.GetResponse();
                response.Close();

                ExportConsolidateFile();
                //ExportCombinedFile();

                Application.DoEvents();
                progressBar1.Value = 100;
                lblcomsuc.Visible = true;
                lblplswt.Visible = false;
                Application.DoEvents();

                string msg = GetLastConfirmItDataUpdatedDate();
                //SendMailForLastUpdatedDate(msg);   18/7/2018
            }
            catch (Exception ex)
            {
                MessageBox.Show("issue" + ex.ToString());
                // SendMail("button1_click", ex.StackTrace); 18/7/2018
            }
            finally
            {

            }

        }

        private void ExtractDownloadFile(string zipPath, string extractPath, string file)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(extractPath);

                foreach (FileInfo files in di.GetFiles())
                {
                    files.Delete();
                }
                ZipFile.ExtractToDirectory(zipPath, extractPath);
                string[] filename = Directory.GetFiles(extractPath).Where(s => s.Contains(file)).ToArray();
                System.IO.File.Move(filename[0].ToString(), extractPath + "/DailyExport_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx");
            }
            catch (Exception ex)
            {
                //SendMail("ExtractDownloadFile", ex.Message.ToString()); 18/7/2018
                Application.Exit();
            }
        }

        private void DownloadFilesFTP(string Folder)
        {
            try
            {
                // Get the object used to communicate with the server.
                string ZipFilepath = @System.Configuration.ConfigurationManager.AppSettings["ZipFilepath"] + "\\";

                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = "fileshare.cross-tab.com",
                    UserName = "mindset",
                    Password = @":vJ\N~2,AD",
                    SshHostKeyFingerprint = "ssh-rsa 2048 58:b4:93:e9:ae:5c:e1:53:16:d6:03:8d:79:2b:ad:84"
                };


                using (Session session = new Session())
                {
                    //Connect
                    session.Open(sessionOptions);

                    //Download files
                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.TransferMode = TransferMode.Binary;

                    TransferOperationResult transferResult;
                    transferResult =
                        session.GetFiles("/ConfirmitAutoExport/" + Folder + "/", ZipFilepath, false, transferOptions);

                    //Throw on any error
                    transferResult.Check();


                    //Print results
                    foreach (TransferEventArgs transfer in transferResult.Transfers)
                    {
                        
                        if (transfer.FileName.Contains("International"))
                        {
                            string Filepath = @System.Configuration.ConfigurationManager.AppSettings["Filepath"];
                            ExtractDownloadFile(transfer.Destination, Filepath, "International");
                        }
                        else if (transfer.FileName.Contains("Europe"))
                        {
                            string Filepath2 = @System.Configuration.ConfigurationManager.AppSettings["Filepath2"];
                            ExtractDownloadFile(transfer.Destination, Filepath2, "Europe");
                        }
                        else if (transfer.FileName.Contains("BOEING"))
                        {
                            string Filepath3 = @System.Configuration.ConfigurationManager.AppSettings["Filepath3"];
                            ExtractDownloadFile(transfer.Destination, Filepath3, "BOEING");
                        }
                        else if (transfer.FileName.Contains("Domestic"))
                        {
                            string Filepath4 = @System.Configuration.ConfigurationManager.AppSettings["Filepath4"];
                            ExtractDownloadFile(transfer.Destination, Filepath4, "Domestic");
                        }
                        //Console.WriteLine("Download of {0} succeeded", transfer.FileName);
                    }
                }
            }
            catch (WebException ex)
            {
                //String status = ((FtpWebResponse)ex.Response).StatusDescription;
                //SendMail("DownloadFilesFTP", ex.Message.ToString()); 18/7/2018
            }
        }



        private void DownloadFilesFTP1111(string Folder)
        {
            try
            {
                // Get the object used to communicate with the server.
                string ZipFilepath = @System.Configuration.ConfigurationManager.AppSettings["ZipFilepath"] + "\\";


                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = "fileshare.cross-tab.com",
                    UserName = "mindset",
                    Password = @":vJ\N~2,AD",
                    SshHostKeyFingerprint = "ssh-rsa 2048 58:b4:93:e9:ae:5c:e1:53:16:d6:03:8d:79:2b:ad:84"
                };


                using (Session session = new Session())
                {
                    //Connect
                    session.Open(sessionOptions);

                    //Download files
                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.TransferMode = TransferMode.Binary;

                    TransferOperationResult transferResult;
                    transferResult =
                        session.GetFiles("/ConfirmitAutoExport/" + Folder + "/", @"d:\download\", false, transferOptions);

                    //Throw on any error
                    transferResult.Check();

                    //Print results
                    foreach (TransferEventArgs transfer in transferResult.Transfers)
                    {
                        Console.WriteLine("Download of {0} succeeded", transfer.FileName);
                    }
                }

                // FtpWebRequest request = (FtpWebRequest)WebRequest.Create("sftp://fileshare.cross-tab.com/ConfirmitAutoExport/" + Folder + "/");
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://192.168.0.7/ConfirmitAutoExport/" + Folder + "/");
                request.Method = WebRequestMethods.Ftp.ListDirectory;

                // This example assumes the FTP site uses anonymous logon.
                request.Credentials = new NetworkCredential("mindset", @":vJ\N~2,AD");

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);

                if (!reader.EndOfStream)
                {
                    String filename = reader.ReadLine();

                    // String RemoteFtpPath = "@TempDistributed" + Folder + "/"+ filename;
                    String RemoteFtpPath = "ftp://192.168.0.7/ConfirmitAutoExport/" + Folder + "/" + filename;

                    String LocalDestinationPath = ZipFilepath + filename;
                    String Username = "mindset";
                    String Password = @":vJ\N~2,AD";
                    Boolean UseBinary = true; // use true for .zip file or false for a text file
                    Boolean UsePassive = false;

                    FtpWebRequest requestfile = (FtpWebRequest)WebRequest.Create(RemoteFtpPath);
                    FtpWebRequest requestdatetime = (FtpWebRequest)WebRequest.Create(RemoteFtpPath);
                    requestdatetime.Method = WebRequestMethods.Ftp.GetDateTimestamp;
                    requestfile.Method = WebRequestMethods.Ftp.DownloadFile;
                    requestfile.KeepAlive = true;
                    requestfile.UsePassive = UsePassive;
                    requestfile.UseBinary = UseBinary;

                    requestfile.Credentials = new NetworkCredential(Username, Password);

                    requestdatetime.Credentials = new NetworkCredential(Username, Password);
                    FtpWebResponse responsedatetime = (FtpWebResponse)requestdatetime.GetResponse();

                    FtpWebResponse responsefile = (FtpWebResponse)requestfile.GetResponse();
                    DateTime lastmodified = responsedatetime.LastModified;

                    Stream responseStreamfile = responsefile.GetResponseStream();
                    StreamReader readerfile = new StreamReader(responseStreamfile);

                    if (DateTime.Now.ToString("dd/MM/yyyy") == lastmodified.ToString("dd/MM/yyyy"))
                    {
                        using (FileStream writer = new FileStream(LocalDestinationPath, FileMode.Create))
                        {

                            long length = responsefile.ContentLength;
                            int bufferSize = 2048;
                            int readCount;
                            byte[] buffer = new byte[2048];

                            readCount = responseStreamfile.Read(buffer, 0, bufferSize);
                            while (readCount > 0)
                            {
                                writer.Write(buffer, 0, readCount);
                                readCount = responseStreamfile.Read(buffer, 0, bufferSize);
                            }
                        }
                    }
                    readerfile.Close();
                    responsefile.Close();
                    responsedatetime.Close();
                    reader.Close();
                    response.Close();

                    if (filename.Contains("International"))
                    {
                        string Filepath = @System.Configuration.ConfigurationManager.AppSettings["Filepath"];
                        ExtractDownloadFile(LocalDestinationPath, Filepath, "International");
                    }
                    else if (filename.Contains("Europe"))
                    {
                        string Filepath2 = @System.Configuration.ConfigurationManager.AppSettings["Filepath2"];
                        ExtractDownloadFile(LocalDestinationPath, Filepath2, "Europe");
                    }
                    else if (filename.Contains("BOEING"))
                    {
                        string Filepath3 = @System.Configuration.ConfigurationManager.AppSettings["Filepath3"];
                        ExtractDownloadFile(LocalDestinationPath, Filepath3, "BOEING");
                    }
                    else if (filename.Contains("Domestic"))
                    {
                        string Filepath4 = @System.Configuration.ConfigurationManager.AppSettings["Filepath4"];
                        ExtractDownloadFile(LocalDestinationPath, Filepath4, "Domestic");
                    }

                }

            }
            catch (WebException ex)
            {
                //String status = ((FtpWebResponse)ex.Response).StatusDescription;
                //SendMail("DownloadFilesFTP", ex.Message.ToString()); 18/7/2018
            }
        }




        private void SendMail(string function, string message)
        {
            //string mailTo = @System.Configuration.ConfigurationManager.AppSettings["mailTo"];
            //string mailCC1 = @System.Configuration.ConfigurationManager.AppSettings["mailCC1"];
            //string mailCC2 = @System.Configuration.ConfigurationManager.AppSettings["mailCC2"];

            //    MailMessage mail = new MailMessage (
            //    new MailAddress("ctos@cross-tab.com","MIndset Auto Upload Alerts"),
            //    new MailAddress(mailTo,mailTo)
            //    );

            //mail.CC.Add(new MailAddress(mailCC1,mailCC1));
            //mail.CC.Add(new MailAddress(mailCC2, mailCC2));

            //SmtpClient client = new SmtpClient();
            //client.Port = 587;
            //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //client.EnableSsl = true;
            //client.UseDefaultCredentials = false;
            //string password = "<CY33iA&quot;";
            //client.Credentials = new System.Net.NetworkCredential("ctos@cross-tab.com", WebUtility.HtmlDecode(password));
            //mail.IsBodyHtml = true;
            //client.Host = "smtp.gmail.com";
            //mail.Subject ="Alerts : Error On Mindset Auto Upload";
            //mail.Body = @"<table><tr><td colspan='2'><h4>Error Information</h4></td></tr>
            //              <tr><td><h5>Function Name:</h5></td><td><p>"+ function + @"</p></td></tr>
            //              <tr><td><h5>Error Message:</h5></td><td>" + message + "</td></tr></table>";
            //client.Send(mail);

            string MailBody = @"<table><tr><td colspan='2'><h4>Error Information</h4></td></tr>
                          <tr><td><h5>Function Name:</h5></td><td><p>" + function + @"</p></td></tr>
                          <tr><td><h5>Error Message:</h5></td><td>" + message + "</td></tr></table>";

            string MailTo = "";
            string FromAddress = @System.Configuration.ConfigurationManager.AppSettings["FromAddress"];
            string DomainName = @System.Configuration.ConfigurationManager.AppSettings["DomainName"];
            string SMTPHostAddress = @System.Configuration.ConfigurationManager.AppSettings["SmtpServer"];
            Int32 SMTPPortNo = Convert.ToInt32(@System.Configuration.ConfigurationManager.AppSettings["SmtpPort"]);
            string MailToGroup = @System.Configuration.ConfigurationManager.AppSettings["MailToGroup"];

            string Cc = @System.Configuration.ConfigurationManager.AppSettings["Cc"];

            string SMTPUser = @System.Configuration.ConfigurationManager.AppSettings["SmtpUser"];
            string SMTPPassword = @System.Configuration.ConfigurationManager.AppSettings["SmtpPassword"];
            string Title = "MIndset Auto Upload Alerts";


            StringBuilder MailContent = new StringBuilder();
            MailContent.Append(MailBody);
            SmtpClient Client = new SmtpClient(SMTPHostAddress, SMTPPortNo);
            MailMessage Message = new MailMessage();
            Message.From = new MailAddress(FromAddress);
            string Debug = Convert.ToString(ConfigurationManager.AppSettings["IsDebug"]);
            if (Debug.ToLower() == "true")
            {
                //  LocalLog.LogInformation(FUNCTION_NAME, "Inside Send Mail Debug Mode");
                //MailTo = string.Empty;
                MailToGroup = Convert.ToString(ConfigurationManager.AppSettings["DebugMailId"]);
                //LocalLog.LogInformation(FUNCTION_NAME, "Sending Mail To " + MailToGroup);
            }
            if (MailTo == string.Empty)
            {
                if (MailToGroup.Length <= 0)
                {
                    //  LocalLog.LogInformation(FUNCTION_NAME, "No recepient");
                    return;
                }
                string[] ArrRecepient = MailToGroup.Split('|');
                foreach (var item in ArrRecepient)
                {
                    Message.To.Add(new MailAddress(item));
                }
            }
            else
            {
                //disabling any emails to external users until we complete all tests
                if (MailTo.Contains("@cross-tab.com") || MailTo.Contains("@kantar.com") || MailTo.Contains("@surveysampling.com"))
                {
                    Message.To.Add(new MailAddress(MailTo));
                }
                else
                {
                    //non cross-tab account do not send email right now
                    Message.To.Add(new MailAddress(Convert.ToString(ConfigurationManager.AppSettings["adminMailid"])));
                }
            }

            if (!string.IsNullOrEmpty(Cc))
                Message.CC.Add(new MailAddress(Cc));
            Client.DeliveryMethod = SmtpDeliveryMethod.Network;
            Client.EnableSsl = true;
            Client.UseDefaultCredentials = true;
            if (DomainName != string.Empty)
                Client.Credentials = new NetworkCredential(SMTPUser, SMTPPassword, DomainName);
            else
                Client.Credentials = new NetworkCredential(SMTPUser, SMTPPassword);

            Message.Subject = Title;
            Message.Body = MailContent.ToString();
            Message.IsBodyHtml = true;
            Client.Send(Message);

        }

        private void ExportCombinedFile()
        {
            try
            {
                SqlCommand cmd;
                SqlDataAdapter sqlda;
                StreamWriter CsvfileWriter;
                string folderPath = @System.Configuration.ConfigurationManager.AppSettings["ExportPath"] + "/";
                string ExportConsolidate = @System.Configuration.ConfigurationManager.AppSettings["ExportConsolidate"] + "/";
                // Export Combined International File 

                cmd = new SqlCommand("proc_export_combined", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@eventType", SqlDbType.Int).Value = 0;
                CsvfileWriter = new StreamWriter(folderPath + "Combined_INTL_" + DateTime.Now.ToString("ddMMyyyy") + ".csv");
                cmd.CommandTimeout = 1000;
                connection.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                using (CsvfileWriter)
                {
                    //This Block of code for getting the Table Headers
                    DataTable Tablecolumns = new DataTable();

                    for (int i = 0; i < sdr.FieldCount; i++)
                    {
                        Tablecolumns.Columns.Add(sdr.GetName(i));
                    }
                    CsvfileWriter.WriteLine(string.Join(",", Tablecolumns.Columns.Cast<DataColumn>().Select(csvfile => csvfile.ColumnName)));
                    //This block of code for getting the Table Headers

                    while (sdr.Read())
                        //based on your Table columns you can increase and decrese columns
                        CsvfileWriter.WriteLine(sdr[0].ToString() + "," + sdr[1].ToString() + "," + sdr[2].ToString() + "," + sdr[3].ToString() + "," + sdr[4].ToString() + "," + sdr[5].ToString() + "," + sdr[6].ToString() + "," + sdr[7].ToString() + "," + sdr[8].ToString() + "," + sdr[9].ToString() + ",");

                }

                // Export Combined Europe File 

                using (XLWorkbook wb = new XLWorkbook())
                {
                    cmd = new SqlCommand("proc_export_combined", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@eventType", SqlDbType.Int).Value = 1;
                    sqlda = new SqlDataAdapter(cmd);
                    cmd.CommandTimeout = 1000;
                    DataTable dt = new DataTable();
                    sqlda.Fill(dt);
                    wb.Worksheets.Add(dt, "Europe");
                    wb.SaveAs(folderPath + "Combined_Europe_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx");
                    //Application.DoEvents();
                }
                // Export Combined Boeing File 

                using (XLWorkbook wb = new XLWorkbook())
                {
                    cmd = new SqlCommand("proc_export_combined", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@eventType", SqlDbType.Int).Value = 2;
                    sqlda = new SqlDataAdapter(cmd);
                    cmd.CommandTimeout = 1000;
                    DataTable dt = new DataTable();
                    sqlda.Fill(dt);
                    wb.Worksheets.Add(dt, "boeing");
                    wb.SaveAs(folderPath + "Combined_boeing_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx");
                    //Application.DoEvents();
                }
                lblINTLFile.Text = folderPath + "Combined_INTL_" + DateTime.Now.ToString("ddMMyyyy") + ".csv";
                lblEuropefile.Text = folderPath + "Combined_europe_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx";
                lblboeingfile.Text = folderPath + "Combined_boeing_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx";
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message.ToString());
                //SendMail("ExportCombinedFile", ex.Message.ToString()); 18/7/2018
            }
        }

        private void checkfileexist(string filepath)
        {
            if (!File.Exists(filepath))
            {
                MessageBox.Show("File not Exist :" + filepath);
                lblplswt.Visible = false;
                Application.DoEvents();
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;
            backgroundWorker1.RunWorkerAsync();
            //ExportCombinedFile();
        }

        private void backgroundWorker1_DoWork(object sender, EventArgs e)
        {
            // ExportCombinedFile();
            ExportConsolidateFile();
        }

        private void ExportConsolidateFile()
        {
            try
            {
                SqlCommand cmd;
                //SqlDataAdapter sqlda;
                StreamWriter CsvfileWriter;
                //string folderPath = @System.Configuration.ConfigurationManager.AppSettings["ExportPath"] + "/";
                string ExportConsolidate = @System.Configuration.ConfigurationManager.AppSettings["ExportConsolidate"] + "/";
                // Export Consolidate INTL File 

                cmd = new SqlCommand("proc_export_combined", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@eventType", SqlDbType.Int).Value = 3;
                CsvfileWriter = new StreamWriter(ExportConsolidate + "Consolidate_INTL_" + DateTime.Now.ToString("ddMMyyyy") + ".csv");
                cmd.CommandTimeout = 1000;
                connection.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                using (CsvfileWriter)
                {
                    //This Block of code for getting the Table Headers
                    DataTable Tablecolumns = new DataTable();

                    for (int i = 0; i < sdr.FieldCount; i++)
                    {
                        Tablecolumns.Columns.Add(sdr.GetName(i));
                    }
                    CsvfileWriter.WriteLine(string.Join(",", Tablecolumns.Columns.Cast<DataColumn>().Select(csvfile => csvfile.ColumnName)));
                    //This block of code for getting the Table Headers

                    while (sdr.Read())
                        //based on your Table columns you can increase and decrese columns
                        CsvfileWriter.WriteLine(sdr[0].ToString() + "," + sdr[1].ToString() + "," + sdr[2].ToString() + "," + sdr[3].ToString() + ",");

                }
                connection.Close();
                // Export Consolidate Europe File 

                cmd = new SqlCommand("proc_export_combined", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@eventType", SqlDbType.Int).Value = 4;
                CsvfileWriter = new StreamWriter(ExportConsolidate + "Consolidate_Europe_" + DateTime.Now.ToString("ddMMyyyy") + ".csv");
                cmd.CommandTimeout = 1000;
                connection.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                using (CsvfileWriter)
                {
                    //This Block of code for getting the Table Headers
                    DataTable Tablecolumns = new DataTable();

                    for (int i = 0; i < sdr.FieldCount; i++)
                    {
                        Tablecolumns.Columns.Add(sdr.GetName(i));
                    }
                    CsvfileWriter.WriteLine(string.Join(",", Tablecolumns.Columns.Cast<DataColumn>().Select(csvfile => csvfile.ColumnName)));
                    //This block of code for getting the Table Headers

                    while (sdr.Read())
                        //based on your Table columns you can increase and decrese columns
                        CsvfileWriter.WriteLine(sdr[0].ToString() + "," + sdr[1].ToString() + "," + sdr[2].ToString() + "," + sdr[3].ToString() + ",");

                }
                connection.Close();

                // Export Consolidate Boeing File 

                cmd = new SqlCommand("proc_export_combined", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@eventType", SqlDbType.Int).Value = 5;
                CsvfileWriter = new StreamWriter(ExportConsolidate + "Consolidate_Boeing_" + DateTime.Now.ToString("ddMMyyyy") + ".csv");
                cmd.CommandTimeout = 1000;
                connection.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                using (CsvfileWriter)
                {
                    //This Block of code for getting the Table Headers
                    DataTable Tablecolumns = new DataTable();

                    for (int i = 0; i < sdr.FieldCount; i++)
                    {
                        Tablecolumns.Columns.Add(sdr.GetName(i));
                    }
                    CsvfileWriter.WriteLine(string.Join(",", Tablecolumns.Columns.Cast<DataColumn>().Select(csvfile => csvfile.ColumnName)));
                    //This block of code for getting the Table Headers

                    while (sdr.Read())
                        //based on your Table columns you can increase and decrese columns
                        CsvfileWriter.WriteLine(sdr[0].ToString() + "," + sdr[1].ToString() + "," + sdr[2].ToString() + "," + sdr[3].ToString() + ",");

                }
                connection.Close();


                // Export Consolidate Domestic File 

                cmd = new SqlCommand("proc_export_combined", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@eventType", SqlDbType.Int).Value = 7;
                CsvfileWriter = new StreamWriter(ExportConsolidate + "Consolidate_DOM_" + DateTime.Now.ToString("ddMMyyyy") + ".csv");
                cmd.CommandTimeout = 1000;
                connection.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                using (CsvfileWriter)
                {
                    //This Block of code for getting the Table Headers
                    DataTable Tablecolumns = new DataTable();

                    for (int i = 0; i < sdr.FieldCount; i++)
                    {
                        Tablecolumns.Columns.Add(sdr.GetName(i));
                    }
                    CsvfileWriter.WriteLine(string.Join(",", Tablecolumns.Columns.Cast<DataColumn>().Select(csvfile => csvfile.ColumnName)));
                    //This block of code for getting the Table Headers

                    while (sdr.Read())
                        //based on your Table columns you can increase and decrese columns
                        CsvfileWriter.WriteLine(sdr[0].ToString() + "," + sdr[1].ToString() + "," + sdr[2].ToString() + "," + sdr[3].ToString() + ",");

                }
                connection.Close();

                lblConsolidateINTL.Text = ExportConsolidate + "Consolidate_INTL_" + DateTime.Now.ToString("ddMMyyyy") + ".csv";
                lblConsolidateEurope.Text = ExportConsolidate + "Consolidate_Europe_" + DateTime.Now.ToString("ddMMyyyy") + ".csv";
                lblConsolidateBoeing.Text = ExportConsolidate + "Consolidate_Boeing_" + DateTime.Now.ToString("ddMMyyyy") + ".csv";
                lblConsolidateBoeing.Text = ExportConsolidate + "Consolidate_DOM_" + DateTime.Now.ToString("ddMMyyyy") + ".csv";

            }
            catch (Exception ex)
            {
                //SendMail("ExportConsolidateFile", ex.Message.ToString());18/7/2018
                Application.Exit();
            }
        }


        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //  string folderPath = @System.Configuration.ConfigurationManager.AppSettings["ExportPath"] + "/";
            //   string ExportConsolidate = @System.Configuration.ConfigurationManager.AppSettings["ExportConsolidate"] + "/";

            pictureBox1.Visible = false;
            //lblINTLFile.Text = folderPath + "Combined_INTL_" + DateTime.Now.ToString("ddMMyyyy") + ".csv";
            //lblEuropefile.Text = folderPath + "Combined_europe_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx";
            //lblboeingfile.Text = folderPath + "Combined_boeing_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx";

            // lblConsolidateINTL.Text = ExportConsolidate + "Consolidate_INTL_" + DateTime.Now.ToString("ddMMyyyy") + ".csv";
            //  lblConsolidateEurope.Text = ExportConsolidate + "Consolidate_Europe_" + DateTime.Now.ToString("ddMMyyyy") + ".csv";
            // lblConsolidateBoeing.Text = ExportConsolidate + "Consolidate_Boeing_" + DateTime.Now.ToString("ddMMyyyy") + ".csv";
        }

        /// <summary>
        /// Get last date when confirm It data updated into webtool for all four International,Europe,Domestic,Aircraft
        /// </summary>
        /// <returns></returns>
        private string GetLastConfirmItDataUpdatedDate()
        {
            string LUDate = string.Empty;
            try
            {

                connection.Open();
                DataSet Ds = new DataSet();

                sql = new SqlCommand("GetlastUpdatedDateComfirmIt", connection);
                SqlDataAdapter sda = new SqlDataAdapter(sql);
                sql.CommandTimeout = 800;
                sql.CommandType = CommandType.StoredProcedure;
                sda.Fill(Ds);
                if (Ds.Tables.Count > 0)
                {
                    if (Ds.Tables[0].Rows.Count > 0)
                    {
                        DateTime parsed;
                        string intdate = (DateTime.TryParse(Ds.Tables[0].Rows[0]["International"].ToString(), out parsed)) ? Convert.ToDateTime(Ds.Tables[0].Rows[0]["International"]).ToString("dd/MM/yyyy HH:mm:ss") + " CEST" : "";
                        string eupdate = (DateTime.TryParse(Ds.Tables[0].Rows[0]["Europe"].ToString(), out parsed)) ? Convert.ToDateTime(Ds.Tables[0].Rows[0]["Europe"]).ToString("dd/MM/yyyy HH:mm:ss") + " CEST" : "";
                        string domdate = (DateTime.TryParse(Ds.Tables[0].Rows[0]["Domestic"].ToString(), out parsed)) ? Convert.ToDateTime(Ds.Tables[0].Rows[0]["Domestic"]).ToString("dd/MM/yyyy HH:mm:ss") + " CEST" : "";
                        string airdate = (DateTime.TryParse(Ds.Tables[0].Rows[0]["Aircraft"].ToString(), out parsed)) ? Convert.ToDateTime(Ds.Tables[0].Rows[0]["Aircraft"]).ToString("dd/MM/yyyy HH:mm:ss") + " CEST" : "";
                        LUDate += "International : " + intdate + "  <br/>";
                        LUDate += "Europe : " + eupdate + "  <br/>";
                        LUDate += "Domestic : " + domdate + "  <br/>";
                        LUDate += "Aircraft : " + airdate + "  <br/>";
                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("GetLastConfirmItDataUpdatedDate " + ex.ToString());
                // SendMail("GetLastConfirmItDataUpdatedDate", ex.Message.ToString());18/7/2018
                Application.Exit();
            }
            return LUDate;
        }






        /// <summary>
        /// Send email to users of last updated date when confirm it data was updated.
        /// </summary>
        /// <param name="message"> Message contains last updated dates</param>
        private void SendMailForLastUpdatedDate(string message)
        {

            string MailBody = @"Hello Team, <br/><br/> Last uploaded date time is: <br/> " + message + "  <br/><br/> " +
                " Regards, <br/> Web Tool";
            //string MailBody1 = @"<table><tr><td colspan='2'><h4>Error Information</h4></td></tr>
            //              <tr><td><h5>Function Name:</h5></td><td><p>" + function + @"</p></td></tr>
            //              <tr><td><h5>Error Message:</h5></td><td>" + message + "</td></tr></table>";

            string MailTo = "";
            string FromAddress = @System.Configuration.ConfigurationManager.AppSettings["FromAddress"];
            string DomainName = @System.Configuration.ConfigurationManager.AppSettings["DomainName"];
            string SMTPHostAddress = @System.Configuration.ConfigurationManager.AppSettings["SmtpServer"];
            Int32 SMTPPortNo = Convert.ToInt32(@System.Configuration.ConfigurationManager.AppSettings["SmtpPort"]);
            string MailToGroup = @System.Configuration.ConfigurationManager.AppSettings["mailToLUD"];

            string Cc = @System.Configuration.ConfigurationManager.AppSettings["Cc"];

            string SMTPUser = @System.Configuration.ConfigurationManager.AppSettings["SmtpUser"];
            string SMTPPassword = @System.Configuration.ConfigurationManager.AppSettings["SmtpPassword"];
            string Title = "Uploaded Date/Time - Web Tool ";


            StringBuilder MailContent = new StringBuilder();
            MailContent.Append(MailBody);
            SmtpClient Client = new SmtpClient(SMTPHostAddress, SMTPPortNo);
            MailMessage Message = new MailMessage();
            Message.From = new MailAddress(FromAddress);
            string Debug = Convert.ToString(ConfigurationManager.AppSettings["IsDebug"]);
            if (Debug.ToLower() == "true")
            {
                //  LocalLog.LogInformation(FUNCTION_NAME, "Inside Send Mail Debug Mode");
                //MailTo = string.Empty;
                MailToGroup = Convert.ToString(ConfigurationManager.AppSettings["DebugMailId"]);
                //LocalLog.LogInformation(FUNCTION_NAME, "Sending Mail To " + MailToGroup);
            }
            if (MailTo == string.Empty)
            {
                if (MailToGroup.Length <= 0)
                {
                    //  LocalLog.LogInformation(FUNCTION_NAME, "No recepient");
                    return;
                }
                string[] ArrRecepient = MailToGroup.Split('|');
                foreach (var item in ArrRecepient)
                {
                    Message.To.Add(new MailAddress(item));
                }
            }
            else
            {
                //disabling any emails to external users until we complete all tests
                if (MailTo.Contains("@cross-tab.com") || MailTo.Contains("@kantar.com") || MailTo.Contains("@surveysampling.com"))
                {
                    Message.To.Add(new MailAddress(MailTo));
                }
                else
                {
                    //non cross-tab account do not send email right now
                    Message.To.Add(new MailAddress(Convert.ToString(ConfigurationManager.AppSettings["adminMailid"])));
                }
            }

            if (!string.IsNullOrEmpty(Cc))
                Message.CC.Add(new MailAddress(Cc));
            Client.DeliveryMethod = SmtpDeliveryMethod.Network;
            Client.EnableSsl = true;
            Client.UseDefaultCredentials = true;
            if (DomainName != string.Empty)
                Client.Credentials = new NetworkCredential(SMTPUser, SMTPPassword, DomainName);
            else
                Client.Credentials = new NetworkCredential(SMTPUser, SMTPPassword);

            Message.Subject = Title;
            Message.Body = MailContent.ToString();
            Message.IsBodyHtml = true;
            Client.Send(Message);

        }

    }
}


