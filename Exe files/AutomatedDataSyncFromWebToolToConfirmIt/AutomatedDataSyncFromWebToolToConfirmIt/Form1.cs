using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Web;
using System.Net.Mail;

namespace AutomatedDataSyncFromWebToolToConfirmIt
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        //private void btnInternational_Click(object sender, EventArgs e)
        //{
        //    string SICTconnetionString = null;
        //    SqlDataAdapter da = new SqlDataAdapter();
        //    DataSet DSR = new DataSet();
        //    SICTconnetionString = ConfigurationManager.ConnectionStrings["SICTCon"].ToString();
        //    SqlCommand sqlCmd;
        //    SqlConnection SqlCon = new SqlConnection(SICTconnetionString);
        //    try
        //    {
        //        SqlCon.Open();

        //        DateTime Yesterday = DateTime.Today.AddDays(-1);
        //        string strYesterday = Yesterday.ToShortDateString().Replace('/', '-');

        //        sqlCmd = new SqlCommand("spr_AutomatedDataSyncFromWebtoolToConfirmIt", SqlCon);
        //        sqlCmd.Parameters.AddWithValue("@YearDiff", Convert.ToInt32(ConfigurationManager.AppSettings["YearDifference"]));
        //        sqlCmd.Parameters.AddWithValue("@PreDayFilePath", Convert.ToString(ConfigurationManager.AppSettings["FileSavePAth"]) + strYesterday + "\\PreviousDay.txt");
        //        sqlCmd.CommandType = CommandType.StoredProcedure;
        //        sqlCmd.CommandTimeout = 5000;
        //        da.SelectCommand = sqlCmd;
        //        da.Fill(DSR);

        //        DataTable dtFinal = DSR.Tables[0];
        //        // DataTable dtPreviousDay = DSR.Tables[1];

        //        SqlCon.Close();

        //        StringBuilder builderFinalFile = new StringBuilder();
        //        StringBuilder builderPreviousDayFile = new StringBuilder();
        //        builderFinalFile = ConvertDatatableToTabDeliFile(dtFinal);
        //        // builderPreviousDayFile = ConvertDatatableToTabDeliFile(dtPreviousDay);


        //        string filepath = ConfigurationManager.AppSettings["FileSavePAth"].ToString();
        //        string fileName = ConfigurationManager.AppSettings["Filename"].ToString();
        //        string FTPLink = ConfigurationManager.AppSettings["FTPLink"].ToString();
        //        string FTPUsername = ConfigurationManager.AppSettings["FTPUsername"].ToString();
        //        string FTPPassword = ConfigurationManager.AppSettings["FTPPassword"].ToString();
        //        string TodaysDate = DateTime.Today.Date.ToShortDateString().Replace('/', '-');
        //        DateTime ThreeDaysAgo = DateTime.Today.AddDays(-3);
        //        string strThreeDaysAgo = ThreeDaysAgo.ToShortDateString().Replace('/', '-');
        //        if (Directory.Exists(filepath + "\\" + strThreeDaysAgo))
        //        {
        //            DirectoryInfo di = new DirectoryInfo(filepath + "\\" + strThreeDaysAgo);
        //            foreach (FileInfo file in di.GetFiles())
        //            {
        //                file.Delete();
        //            }
        //            di.Delete();
        //            //foreach (DirectoryInfo dir in di.GetDirectories())
        //            //{
        //            //    dir.Delete(true);
        //            //}
        //        }

        //        ////code to save Previous Day file
        //        //if (!Directory.Exists(filepath + "\\" + TodaysDate))
        //        //{
        //        //    Directory.CreateDirectory(filepath + "\\" + TodaysDate);
        //        //}
        //        //if (File.Exists(filepath + "\\" + TodaysDate + "\\" + "PreviousDay.txt"))
        //        //{
        //        //    File.Delete(filepath + "\\" + TodaysDate + "\\" + "PreviousDay.txt");
        //        //}
        //        ////Encoding encPreFile = new UTF32Encoding();
        //        //// File.WriteAllText(filepath + "\\" + TodaysDate + "\\" + "PreviousDay.txt", builderPreviousDayFile.ToString(), encPreFile);
        //        //CreateCSVFile(dtPreviousDay, filepath + "\\" + TodaysDate + "\\" + "PreviousDay.txt");
        //        ////Code Ended for saving Prevoius day file


        //        //code to save final file
        //        if (!Directory.Exists(filepath + "\\" + TodaysDate))
        //        {
        //            Directory.CreateDirectory(filepath + "\\" + TodaysDate);
        //        }
        //        if (File.Exists(filepath + "\\" + TodaysDate + "\\" + fileName))
        //        {
        //            File.Delete(filepath + "\\" + TodaysDate + "\\" + fileName);
        //        }

        //        // Encoding encFinalFile = new UTF8Encoding();
        //        // CreateCSVFile(dtFinal, filepath + "\\" + TodaysDate + "\\" + fileName);
        //        File.WriteAllText(filepath + "\\" + TodaysDate + "\\" + fileName, builderFinalFile.ToString(), Encoding.GetEncoding("iso-8859-1"));
        //        //Code Ended for saving Final file




        //        //Code to uplod file on Ftp

        //        byte[] fileBytes = null;

        //        // To delete file
        //        FtpWebRequest delRequest = (FtpWebRequest)WebRequest.Create(FTPLink + "International/" + fileName);
        //        delRequest.Credentials = new NetworkCredential(FTPUsername, FTPPassword);
        //        delRequest.Method = WebRequestMethods.Ftp.DeleteFile;

        //        using (StreamReader fileStream = new StreamReader(filepath + TodaysDate + "\\" + fileName))
        //        {
        //            fileBytes = Encoding.UTF8.GetBytes(fileStream.ReadToEnd());
        //            fileStream.Close();
        //        }
        //        //Create FTP Request.
        //        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FTPLink + "International/" + fileName);
        //        request.Method = WebRequestMethods.Ftp.UploadFile;

        //        //Enter FTP Server credentials.
        //        request.Credentials = new NetworkCredential(FTPUsername, FTPPassword);
        //        request.ContentLength = fileBytes.Length;
        //        request.UsePassive = true;
        //        request.UseBinary = true;
        //        request.ServicePoint.ConnectionLimit = fileBytes.Length;
        //        request.EnableSsl = false;

        //        using (Stream requestStream = request.GetRequestStream())
        //        {
        //            requestStream.Write(fileBytes, 0, fileBytes.Length);
        //            requestStream.Close();
        //        }

        //        FtpWebResponse response = (FtpWebResponse)request.GetResponse();


        //        response.Close();
        //        //Code ended upload file on FTP

        //        // MessageBox.Show("Completed");


        //    }
        //    catch (Exception ex)
        //    {
        //        SendMail("International", ex.Message.ToString());
        //    }
        //}
        public void funInternational()
        {
            string INTLconnetionString = null;
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet DSR = new DataSet();
            INTLconnetionString = ConfigurationManager.ConnectionStrings["InternationalCon"].ToString();
            SqlCommand sqlCmd;
            SqlConnection SqlCon = new SqlConnection(INTLconnetionString);
            try
            {
                SqlCon.Open();

                DateTime Yesterday = DateTime.Today.AddDays(-1);
                string strYesterday = Yesterday.ToShortDateString().Replace('/', '-');

                sqlCmd = new SqlCommand("spr_AutomatedDataSyncFromWebtoolToConfirmIt", SqlCon);
                sqlCmd.Parameters.AddWithValue("@YearDiff", Convert.ToInt32(ConfigurationManager.AppSettings["YearDifference"]));
                // sqlCmd.Parameters.AddWithValue("@PreDayFilePath", Convert.ToString(ConfigurationManager.AppSettings["FileSavePAth"]) + strYesterday + "\\PreviousDay.txt");
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandTimeout = 5000;
                da.SelectCommand = sqlCmd;
                da.Fill(DSR);
                DataTable dtFinal = DSR.Tables[0];
                DataTable dtInvalidId = DSR.Tables[1];
                DataTable dtCntNullRecord = DSR.Tables[2];
                SqlCon.Close();



                StringBuilder builderFinalFile = new StringBuilder();
                //StringBuilder builderPreviousDayFile = new StringBuilder();
                StringBuilder builderInvalidIdsFile = new StringBuilder();
                builderFinalFile = ConvertDatatableToTabDeliFile(dtFinal);
                builderInvalidIdsFile = ConvertDatatableToTabDeliFile(dtInvalidId);
                // builderPreviousDayFile = ConvertDatatableToTabDeliFile(dtPreviousDay);


                string filepath = ConfigurationManager.AppSettings["FileSavePAth"].ToString();
                string fileName = ConfigurationManager.AppSettings["Filename"].ToString();
                string FTPLink = ConfigurationManager.AppSettings["FTPLink"].ToString();
                string FTPUsername = ConfigurationManager.AppSettings["FTPUsername"].ToString();
                string FTPPassword = ConfigurationManager.AppSettings["FTPPassword"].ToString();
                string TodaysDate = DateTime.Today.Date.ToShortDateString().Replace('/', '-');
                DateTime ThreeDaysAgo = DateTime.Today.AddDays(-3);
                string strThreeDaysAgo = ThreeDaysAgo.ToShortDateString().Replace('/', '-');
                if (Directory.Exists(filepath + "\\International\\" + strThreeDaysAgo))
                {
                    DirectoryInfo di = new DirectoryInfo(filepath + "\\International\\" + strThreeDaysAgo);
                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                    di.Delete();
                    //foreach (DirectoryInfo dir in di.GetDirectories())
                    //{
                    //    dir.Delete(true);
                    //}
                }



                //code to save final file
                if (!Directory.Exists(filepath + "\\International\\" + TodaysDate))
                {
                    Directory.CreateDirectory(filepath + "\\International\\" + TodaysDate);
                }
                if (File.Exists(filepath + "\\International\\" + TodaysDate + "\\" + fileName))
                {
                    File.Delete(filepath + "\\International\\" + TodaysDate + "\\" + fileName);
                }

                // Encoding encFinalFile = new UTF8Encoding();
                // CreateCSVFile(dtFinal, filepath + "\\" + TodaysDate + "\\" + fileName);
                File.WriteAllText(filepath + "\\International\\" + TodaysDate + "\\" + fileName, builderFinalFile.ToString(), Encoding.GetEncoding("iso-8859-1"));
                File.WriteAllText(filepath + "\\International\\" + TodaysDate + "\\InvalidIds.txt", builderInvalidIdsFile.ToString(), Encoding.GetEncoding("iso-8859-1"));
                //Code Ended for saving Final file




                //Code to uplod file on Ftp

                byte[] fileBytes = null;





                //FtpWebRequest ftpRenameRequest = null;
                //FtpWebResponse ftpRenameResponse = null;
                //ftpRenameRequest = (FtpWebRequest)WebRequest.Create(FTPLink + "International/" + fileName);
                //ftpRenameRequest.Credentials = new NetworkCredential(FTPUsername, FTPPassword);
                //ftpRenameRequest.UseBinary = true;
                //ftpRenameRequest.UsePassive = true;
                //ftpRenameRequest.KeepAlive = true;
                //ftpRenameRequest.RenameTo = "123.txt";
                //ftpRenameRequest.Method = WebRequestMethods.Ftp.Rename;

                //ftpRenameResponse = (FtpWebResponse)ftpRenameRequest.GetResponse();
                //ftpRenameResponse.Close();
                if (dtCntNullRecord.Rows.Count > 0)
                {
                    if (Convert.ToInt32(dtCntNullRecord.Rows[0]["NullCount"].ToString()) > 0)
                    {
                        SendMailForNullValues("International : File not uploaded to FTP because null values found into file. Please add new code into table and rerun exec.");
                    }
                    else
                    {


                        // To delete file
                        FtpWebRequest delRequest = (FtpWebRequest)WebRequest.Create(FTPLink + "International/" + fileName);
                        delRequest.Credentials = new NetworkCredential(FTPUsername, FTPPassword);
                        delRequest.Method = WebRequestMethods.Ftp.DeleteFile;


                        using (StreamReader fileStream = new StreamReader(filepath + "\\International\\" + TodaysDate + "\\" + fileName, Encoding.GetEncoding("iso-8859-1")))
                        {
                            fileBytes = Encoding.GetEncoding("iso-8859-1").GetBytes(fileStream.ReadToEnd());
                            fileStream.Close();
                        }
                        //Create FTP Request.
                        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FTPLink + "International/" + fileName);
                        request.Method = WebRequestMethods.Ftp.UploadFile;

                        //Enter FTP Server credentials.
                        request.Credentials = new NetworkCredential(FTPUsername, FTPPassword);
                        request.ContentLength = fileBytes.Length;
                        request.UsePassive = true;
                        request.UseBinary = true;
                        request.ServicePoint.ConnectionLimit = fileBytes.Length;
                        request.EnableSsl = false;

                        using (Stream requestStream = request.GetRequestStream())
                        {
                            requestStream.Write(fileBytes, 0, fileBytes.Length);
                            requestStream.Close();
                        }

                        FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                        SendMailWithAttachment("PFA invalid ids file for International", filepath + "\\International\\" + TodaysDate + "\\InvalidIds.txt");

                        response.Close();
                        //Code ended upload file on FTP

                        // MessageBox.Show("Completed");
                    }
                }


            }
            catch (Exception ex)
            {
                SendMail("International", ex.Message.ToString() + "   Stack Strace:" + ex.StackTrace.ToString());
            }
        }
        public void funEurope()
        {
            string EUROPEconnetionString = null;
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet DSR = new DataSet();
            EUROPEconnetionString = ConfigurationManager.ConnectionStrings["EuropeCon"].ToString();
            SqlCommand sqlCmd;
            SqlConnection SqlCon = new SqlConnection(EUROPEconnetionString);
            try
            {
                SqlCon.Open();

                DateTime Yesterday = DateTime.Today.AddDays(-1);
                string strYesterday = Yesterday.ToShortDateString().Replace('/', '-');

                sqlCmd = new SqlCommand("spr_AutomatedDataSyncFromWebtoolToConfirmIt", SqlCon);
                sqlCmd.Parameters.AddWithValue("@YearDiff", Convert.ToInt32(ConfigurationManager.AppSettings["YearDifference"]));
                // sqlCmd.Parameters.AddWithValue("@PreDayFilePath", Convert.ToString(ConfigurationManager.AppSettings["FileSavePAth"]) + strYesterday + "\\PreviousDay.txt");
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandTimeout = 5000;
                da.SelectCommand = sqlCmd;
                da.Fill(DSR);

                DataTable dtFinal = DSR.Tables[0];
                DataTable dtInvalidId = DSR.Tables[1];
                DataTable dtCntNullRecord = DSR.Tables[2];
                SqlCon.Close();

                StringBuilder builderFinalFile = new StringBuilder();
                //StringBuilder builderPreviousDayFile = new StringBuilder();
                StringBuilder builderInvalidIdsFile = new StringBuilder();
                builderFinalFile = ConvertDatatableToTabDeliFile(dtFinal);
                builderInvalidIdsFile = ConvertDatatableToTabDeliFile(dtInvalidId);
                // builderPreviousDayFile = ConvertDatatableToTabDeliFile(dtPreviousDay);


                string filepath = ConfigurationManager.AppSettings["FileSavePAth"].ToString();
                string fileName = ConfigurationManager.AppSettings["Filename"].ToString();
                string FTPLink = ConfigurationManager.AppSettings["FTPLink"].ToString();
                string FTPUsername = ConfigurationManager.AppSettings["FTPUsername"].ToString();
                string FTPPassword = ConfigurationManager.AppSettings["FTPPassword"].ToString();
                string TodaysDate = DateTime.Today.Date.ToShortDateString().Replace('/', '-');
                DateTime ThreeDaysAgo = DateTime.Today.AddDays(-3);
                string strThreeDaysAgo = ThreeDaysAgo.ToShortDateString().Replace('/', '-');
                if (Directory.Exists(filepath + "\\Europe\\" + strThreeDaysAgo))
                {
                    DirectoryInfo di = new DirectoryInfo(filepath + "\\Europe\\" + strThreeDaysAgo);
                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                    di.Delete();
                    //foreach (DirectoryInfo dir in di.GetDirectories())
                    //{
                    //    dir.Delete(true);
                    //}
                }

                ////code to save Previous Day file
                //if (!Directory.Exists(filepath + "\\" + TodaysDate))
                //{
                //    Directory.CreateDirectory(filepath + "\\" + TodaysDate);
                //}
                //if (File.Exists(filepath + "\\" + TodaysDate + "\\" + "PreviousDay.txt"))
                //{
                //    File.Delete(filepath + "\\" + TodaysDate + "\\" + "PreviousDay.txt");
                //}
                ////Encoding encPreFile = new UTF32Encoding();
                //// File.WriteAllText(filepath + "\\" + TodaysDate + "\\" + "PreviousDay.txt", builderPreviousDayFile.ToString(), encPreFile);
                //CreateCSVFile(dtPreviousDay, filepath + "\\" + TodaysDate + "\\" + "PreviousDay.txt");
                ////Code Ended for saving Prevoius day file


                //code to save final file
                if (!Directory.Exists(filepath + "\\Europe\\" + TodaysDate))
                {
                    Directory.CreateDirectory(filepath + "\\Europe\\" + TodaysDate);
                }
                if (File.Exists(filepath + "\\Europe\\" + TodaysDate + "\\" + fileName))
                {
                    File.Delete(filepath + "\\Europe\\" + TodaysDate + "\\" + fileName);
                }

                // Encoding encFinalFile = new UTF8Encoding();
                // CreateCSVFile(dtFinal, filepath + "\\" + TodaysDate + "\\" + fileName);
                File.WriteAllText(filepath + "\\Europe\\" + TodaysDate + "\\" + fileName, builderFinalFile.ToString(), Encoding.GetEncoding("iso-8859-1"));
                File.WriteAllText(filepath + "\\Europe\\" + TodaysDate + "\\InvalidIds.txt", builderInvalidIdsFile.ToString(), Encoding.GetEncoding("iso-8859-1"));
                //Code Ended for saving Final file




                //Code to uplod file on Ftp

                byte[] fileBytes = null;





                //FtpWebRequest ftpRenameRequest = null;
                //FtpWebResponse ftpRenameResponse = null;
                //ftpRenameRequest = (FtpWebRequest)WebRequest.Create(FTPLink + "International/" + fileName);
                //ftpRenameRequest.Credentials = new NetworkCredential(FTPUsername, FTPPassword);
                //ftpRenameRequest.UseBinary = true;
                //ftpRenameRequest.UsePassive = true;
                //ftpRenameRequest.KeepAlive = true;
                //ftpRenameRequest.RenameTo = "123.txt";
                //ftpRenameRequest.Method = WebRequestMethods.Ftp.Rename;

                //ftpRenameResponse = (FtpWebResponse)ftpRenameRequest.GetResponse();
                //ftpRenameResponse.Close();
                if (dtCntNullRecord.Rows.Count > 0)
                {
                    if (Convert.ToInt32(dtCntNullRecord.Rows[0]["NullCount"].ToString()) > 0)
                    {
                        SendMailForNullValues("Europe : File not uploaded to FTP because null values found into file. Please add new code into table and rerun exec.");
                    }
                    else
                    {
                        // To delete file

                        FtpWebRequest delRequest = (FtpWebRequest)WebRequest.Create(FTPLink + "\\Europe\\" + fileName);
                        delRequest.Credentials = new NetworkCredential(FTPUsername, FTPPassword);
                        delRequest.Method = WebRequestMethods.Ftp.DeleteFile;


                        using (StreamReader fileStream = new StreamReader(filepath + "\\Europe\\" + TodaysDate + "\\" + fileName, Encoding.GetEncoding("iso-8859-1")))
                        {
                            fileBytes = Encoding.GetEncoding("iso-8859-1").GetBytes(fileStream.ReadToEnd());
                            fileStream.Close();
                        }
                        //Create FTP Request.
                        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FTPLink + "\\Europe\\" + fileName);
                        request.Method = WebRequestMethods.Ftp.UploadFile;

                        //Enter FTP Server credentials.
                        request.Credentials = new NetworkCredential(FTPUsername, FTPPassword);
                        request.ContentLength = fileBytes.Length;
                        request.UsePassive = true;
                        request.UseBinary = true;
                        request.ServicePoint.ConnectionLimit = fileBytes.Length;
                        request.EnableSsl = false;

                        using (Stream requestStream = request.GetRequestStream())
                        {
                            requestStream.Write(fileBytes, 0, fileBytes.Length);
                            requestStream.Close();
                        }
                        FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                        response.Close();
                        SendMailWithAttachment("PFA invalid ids file for Europe", filepath + "\\Europe\\" + TodaysDate + "\\InvalidIds.txt");
                    }




                    //Code ended upload file on FTP

                    // MessageBox.Show("Completed");
                }

            }
            catch (Exception ex)
            {
                SendMail("Europe", ex.Message.ToString() + "   Stack Strace:" + ex.StackTrace.ToString());
            }
        }
        public void CreateCSVFile(DataTable dtDataTablesList, string strFilePath)

        {
            // Create the CSV file to which grid data will be exported.

            StreamWriter sw = new StreamWriter(strFilePath, false);

            //First we will write the headers.

            int iColCount = dtDataTablesList.Columns.Count;

            for (int i = 0; i < iColCount; i++)
            {
                sw.Write(dtDataTablesList.Columns[i]);
                if (i < iColCount - 1)
                {
                    sw.Write("\t");
                }
            }
            sw.Write(sw.NewLine);

            // Now write all the rows.

            foreach (DataRow dr in dtDataTablesList.Rows)
            {
                for (int i = 0; i < iColCount; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        sw.Write(dr[i].ToString());
                    }
                    if (i < iColCount - 1)

                    {
                        sw.Write("\t");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }
        public StringBuilder ConvertDatatableToTabDeliFile(DataTable Dt)
        {
            StringBuilder builder = new StringBuilder();
            int xxx = 0;
            foreach (DataColumn dc in Dt.Columns)
            {
                if (xxx == 0)
                {
                    builder.Append(dc.ColumnName);
                }
                else
                {
                    builder.Append("\t" + dc.ColumnName);
                }
                xxx++;
            }

            builder.Append("\n");


            foreach (DataRow row in Dt.Rows)
            {
                builder.AppendLine(String.Join("\t", row.ItemArray));
            }
            return builder;
        }
        //private void btnEurope_Click(object sender, EventArgs e)
        //{
        //    string EuropeconnetionString = null;
        //    SqlDataAdapter da = new SqlDataAdapter();
        //    DataSet DSR = new DataSet();
        //    EuropeconnetionString = ConfigurationManager.ConnectionStrings["EuropeCon"].ToString();
        //    SqlCommand sqlCmd;
        //    SqlConnection SqlCon = new SqlConnection(EuropeconnetionString);
        //    try
        //    {
        //        SqlCon.Open();

        //        sqlCmd = new SqlCommand("spr_AutomatedDataSyncFromWebtoolToConfirmIt", SqlCon);

        //        sqlCmd.CommandType = CommandType.StoredProcedure;
        //        sqlCmd.CommandTimeout = 5000;
        //        da.SelectCommand = sqlCmd;
        //        da.Fill(DSR);

        //        DataTable dtTabDeli = DSR.Tables[0];
        //        SqlCon.Close();

        //        StringBuilder builder = new StringBuilder();

        //        int xxx = 0;
        //        foreach (DataColumn dc in dtTabDeli.Columns)
        //        {
        //            if (xxx == 0)
        //            {
        //                builder.Append(dc.ColumnName);
        //            }
        //            else
        //            {
        //                builder.Append("\t" + dc.ColumnName);
        //            }
        //            xxx++;
        //        }

        //        builder.Append("\n");


        //        foreach (DataRow row in dtTabDeli.Rows)
        //        {
        //            builder.AppendLine(String.Join("\t", row.ItemArray));
        //        }
        //        // Encoding enc = new UTF32Encoding();
        //        //  File.WriteAllText("D:\\Sagar Sawant_Data\\D\\Webtool\\Airs@t\\WebToolExe\\abc.txt", builder.ToString(), enc);

        //        //string ftpAddress = "192.168.1.22";
        //        //string username = "ctos";
        //        //string password = "Crosstab@45";
        //        //string fileName = "abc.txt";
        //        string filepath = ConfigurationManager.AppSettings["FileSavePAth"].ToString();
        //        string fileName = ConfigurationManager.AppSettings["Filename"].ToString();
        //        string FTPLink = ConfigurationManager.AppSettings["FTPLink"].ToString();
        //        string FTPUsername = ConfigurationManager.AppSettings["FTPUsername"].ToString();
        //        string FTPPassword = ConfigurationManager.AppSettings["FTPPassword"].ToString();
        //        using (StreamReader stream = new StreamReader(filepath + fileName))
        //        {
        //            byte[] buffer = Encoding.Default.GetBytes(stream.ReadToEnd());
        //            // WebRequest request = WebRequest.Create("ftp://" + ftpAddress + "/" + "myfolder" + "/" + fileName);

        //            WebRequest request = WebRequest.Create(FTPLink + "TestingCTOS" + "/" + fileName);
        //            request.Method = WebRequestMethods.Ftp.UploadFile;
        //            request.Credentials = new NetworkCredential(FTPUsername, FTPPassword);
        //            Stream reqStream = request.GetRequestStream();
        //            reqStream.Write(buffer, 0, buffer.Length);
        //            reqStream.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}
        private void SendMail(string function, string message)
        {
            string mailTo = @System.Configuration.ConfigurationManager.AppSettings["mailTo"];
            string mailCC1 = @System.Configuration.ConfigurationManager.AppSettings["mailCC1"];
            string mailCC2 = @System.Configuration.ConfigurationManager.AppSettings["mailCC2"];

            MailMessage mail = new MailMessage(
            new MailAddress("ctos@cross-tab.com", "MIndset Auto Upload Alerts"),
            new MailAddress(mailTo, mailTo)
            );

            mail.CC.Add(new MailAddress(mailCC1, mailCC1));
            mail.CC.Add(new MailAddress(mailCC2, mailCC2));

            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            string password = "<CY33iA&quot;";
            client.Credentials = new System.Net.NetworkCredential("ctos@cross-tab.com", WebUtility.HtmlDecode(password));
            mail.IsBodyHtml = true;
            client.Host = "smtp.office365.com";
            mail.Subject = "Alerts : Error On Mindset Auto Upload Webtool to confirmit";
            mail.Body = @"<table><tr><td colspan='2'><h4>Error Information</h4></td></tr>
                          <tr><td><h5>Function Name:</h5></td><td><p>" + function + @"</p></td></tr>
                          <tr><td><h5>Error Message:</h5></td><td>" + message + "</td></tr></table>";
            client.Send(mail);
        }

        private void SendMailForNullValues(string message)
        {
            string mailTo = @System.Configuration.ConfigurationManager.AppSettings["mailTo"];
            string mailCC1 = @System.Configuration.ConfigurationManager.AppSettings["mailCC1"];
            string mailCC2 = @System.Configuration.ConfigurationManager.AppSettings["mailCC2"];

            MailMessage mail = new MailMessage(
            new MailAddress("ctos@cross-tab.com", "MIndset Auto Upload Alerts"),
            new MailAddress(mailTo, mailTo)
            );

            mail.CC.Add(new MailAddress(mailCC1, mailCC1));
            mail.CC.Add(new MailAddress(mailCC2, mailCC2));

            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            string password = "<CY33iA&quot;";
            client.Credentials = new System.Net.NetworkCredential("ctos@cross-tab.com", WebUtility.HtmlDecode(password));
            mail.IsBodyHtml = true;
            client.Host = "smtp.office365.com";
            mail.Subject = "Alerts : Null values found in file";
            mail.Body = @"<span>" + message + "</span>";
            client.Send(mail);
        }
        private void SendMailWithAttachment(string message, string filepath)
        {
            string mailTo = @System.Configuration.ConfigurationManager.AppSettings["mailTo"];
            string mailCC1 = @System.Configuration.ConfigurationManager.AppSettings["mailCC1"];
            string mailCC2 = @System.Configuration.ConfigurationManager.AppSettings["mailCC2"];

            MailMessage mail = new MailMessage(
            new MailAddress("ctos@cross-tab.com", "MIndset Auto Upload Alerts"),
            new MailAddress(mailTo, mailTo)
            );

            mail.CC.Add(new MailAddress(mailCC1, mailCC1));
            mail.CC.Add(new MailAddress(mailCC2, mailCC2));

            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            string password = "<CY33iA&quot;";
            client.Credentials = new System.Net.NetworkCredential("ctos@cross-tab.com", WebUtility.HtmlDecode(password));
            mail.IsBodyHtml = true;
            client.Host = "smtp.office365.com";
            mail.Subject = "Webtool: Invalid Ids file";
            mail.Attachments.Add(new Attachment(filepath));
            mail.Body = @"<table><tr><td>" + message + "</td></tr></table>";
            client.Send(mail);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            try
            {
                funInternational();
                funEurope();
                // funRaw();
            }
            catch (Exception Ex)
            {
            }
            finally
            {
                this.Close();
            }
        }

        public void funRaw()
        {
            string EUROPEconnetionString = null;
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet DSR = new DataSet();
            EUROPEconnetionString = ConfigurationManager.ConnectionStrings["EuropeCon"].ToString();
            SqlCommand sqlCmd;
            SqlConnection SqlCon = new SqlConnection(EUROPEconnetionString);
            try
            {
                SqlCon.Open();

                DateTime Yesterday = DateTime.Today.AddDays(-1);
                string strYesterday = Yesterday.ToShortDateString().Replace('/', '-');

                sqlCmd = new SqlCommand(@"select distinct rtrim(ltrim(startcode1)) as userid,rtrim(ltrim(NULL)) as btime,rtrim(ltrim(AL.AirlineId)) as  bairline, rtrim(ltrim(AO.OriginId)) as borigin ,rtrim(ltrim(AD.DestinationId)) as bdestination, rtrim(ltrim(day(date1))) as bdate,
                                         rtrim(ltrim(cast(datename(month, (date1)) as varchar(20)))) as bmonth, rtrim(ltrim(year(date1))) as byear, rtrim(ltrim(interviewer_id)) as binterview, rtrim(ltrim(flight_number)) as bflight, rtrim(ltrim(lang1)) as blang into #FinalDump from (select *From previousday_tab20180312 where  startcode1 in(645907,645908,645909,645910,645911,645912,645913,645914,645915,645916,645917,645918,645919,645920,645921,645922,645923,645924,645925,645926,645927,645928,922250,922251,922252,922253,922254,922255,922256,922257,922258,922259,922260,922261,922262,922263,922264,922265)) D1 
                                        inner join tbl_AutoOrigin AO on D1.airportid like  '%' + AO.AirportCode + '%'  COLLATE FRENCH_CI_AS
                                        inner join tbl_AutoDestination AD on D1.dept_id like  '%' + AD.AirportCode + '%' COLLATE FRENCH_CI_AS
                                        inner join tbl_AutoAirline AL on D1.airlines_id like  '%' + AL.AirlineCode + '%' COLLATE FRENCH_CI_AS


                                        select * from #FinalDump", SqlCon);
                sqlCmd.Parameters.AddWithValue("@YearDiff", Convert.ToInt32(ConfigurationManager.AppSettings["YearDifference"]));
                // sqlCmd.Parameters.AddWithValue("@PreDayFilePath", Convert.ToString(ConfigurationManager.AppSettings["FileSavePAth"]) + strYesterday + "\\PreviousDay.txt");
                //sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandTimeout = 5000;
                da.SelectCommand = sqlCmd;
                da.Fill(DSR);

                DataTable dtFinal = DSR.Tables[0];
                // DataTable dtInvalidId = DSR.Tables[1];

                SqlCon.Close();

                StringBuilder builderFinalFile = new StringBuilder();
                //StringBuilder builderPreviousDayFile = new StringBuilder();
                StringBuilder builderInvalidIdsFile = new StringBuilder();
                builderFinalFile = ConvertDatatableToTabDeliFile(dtFinal);
                // builderInvalidIdsFile = ConvertDatatableToTabDeliFile(dtInvalidId);
                // builderPreviousDayFile = ConvertDatatableToTabDeliFile(dtPreviousDay);


                string filepath = ConfigurationManager.AppSettings["FileSavePAth"].ToString();
                string fileName = "Raw_Europe.txt";
                string FTPLink = ConfigurationManager.AppSettings["FTPLink"].ToString();
                string FTPUsername = ConfigurationManager.AppSettings["FTPUsername"].ToString();
                string FTPPassword = ConfigurationManager.AppSettings["FTPPassword"].ToString();
                string TodaysDate = DateTime.Today.Date.ToShortDateString().Replace('/', '-');
                DateTime ThreeDaysAgo = DateTime.Today.AddDays(-3);
                string strThreeDaysAgo = ThreeDaysAgo.ToShortDateString().Replace('/', '-');
                //if (Directory.Exists(filepath + "\\Europe\\" + strThreeDaysAgo))
                //{
                //    DirectoryInfo di = new DirectoryInfo(filepath + "\\Europe\\" + strThreeDaysAgo);
                //    foreach (FileInfo file in di.GetFiles())
                //    {
                //        file.Delete();
                //    }
                //    di.Delete();
                //    //foreach (DirectoryInfo dir in di.GetDirectories())
                //    //{
                //    //    dir.Delete(true);
                //    //}
                //}

                ////code to save Previous Day file
                //if (!Directory.Exists(filepath + "\\" + TodaysDate))
                //{
                //    Directory.CreateDirectory(filepath + "\\" + TodaysDate);
                //}
                //if (File.Exists(filepath + "\\" + TodaysDate + "\\" + "PreviousDay.txt"))
                //{
                //    File.Delete(filepath + "\\" + TodaysDate + "\\" + "PreviousDay.txt");
                //}
                ////Encoding encPreFile = new UTF32Encoding();
                //// File.WriteAllText(filepath + "\\" + TodaysDate + "\\" + "PreviousDay.txt", builderPreviousDayFile.ToString(), encPreFile);
                //CreateCSVFile(dtPreviousDay, filepath + "\\" + TodaysDate + "\\" + "PreviousDay.txt");
                ////Code Ended for saving Prevoius day file


                //code to save final file
                if (!Directory.Exists(filepath + "\\Europe\\" + TodaysDate))
                {
                    Directory.CreateDirectory(filepath + "\\Europe\\" + TodaysDate);
                }
                if (File.Exists(filepath + "\\Europe\\" + TodaysDate + "\\" + fileName))
                {
                    File.Delete(filepath + "\\Europe\\" + TodaysDate + "\\" + fileName);
                }

                // Encoding encFinalFile = new UTF8Encoding();
                // CreateCSVFile(dtFinal, filepath + "\\" + TodaysDate + "\\" + fileName);
                File.WriteAllText(filepath + "\\Europe\\" + TodaysDate + "\\" + fileName, builderFinalFile.ToString(), Encoding.GetEncoding("iso-8859-1"));
                File.WriteAllText(filepath + "\\Europe\\" + TodaysDate + "\\InvalidIds.txt", builderInvalidIdsFile.ToString(), Encoding.GetEncoding("iso-8859-1"));
                //Code Ended for saving Final file




                //Code to uplod file on Ftp

                byte[] fileBytes = null;





                //FtpWebRequest ftpRenameRequest = null;
                //FtpWebResponse ftpRenameResponse = null;
                //ftpRenameRequest = (FtpWebRequest)WebRequest.Create(FTPLink + "International/" + fileName);
                //ftpRenameRequest.Credentials = new NetworkCredential(FTPUsername, FTPPassword);
                //ftpRenameRequest.UseBinary = true;
                //ftpRenameRequest.UsePassive = true;
                //ftpRenameRequest.KeepAlive = true;
                //ftpRenameRequest.RenameTo = "123.txt";
                //ftpRenameRequest.Method = WebRequestMethods.Ftp.Rename;

                //ftpRenameResponse = (FtpWebResponse)ftpRenameRequest.GetResponse();
                //ftpRenameResponse.Close();

                // To delete file
                FtpWebRequest delRequest = (FtpWebRequest)WebRequest.Create(FTPLink + "\\Europe\\" + fileName);
                delRequest.Credentials = new NetworkCredential(FTPUsername, FTPPassword);
                delRequest.Method = WebRequestMethods.Ftp.DeleteFile;


                using (StreamReader fileStream = new StreamReader(filepath + "\\Europe\\" + TodaysDate + "\\" + fileName, Encoding.GetEncoding("iso-8859-1")))
                {
                    fileBytes = Encoding.GetEncoding("iso-8859-1").GetBytes(fileStream.ReadToEnd());
                    fileStream.Close();
                }
                //Create FTP Request.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FTPLink + "\\Europe\\" + fileName);
                request.Method = WebRequestMethods.Ftp.UploadFile;

                //Enter FTP Server credentials.
                request.Credentials = new NetworkCredential(FTPUsername, FTPPassword);
                request.ContentLength = fileBytes.Length;
                request.UsePassive = true;
                request.UseBinary = true;
                request.ServicePoint.ConnectionLimit = fileBytes.Length;
                request.EnableSsl = false;

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileBytes, 0, fileBytes.Length);
                    requestStream.Close();
                }

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                // SendMailWithAttachment("PFA invalid ids file for Europe", filepath + "\\Europe\\" + TodaysDate + "\\InvalidIds.txt");

                response.Close();
                //Code ended upload file on FTP

                // MessageBox.Show("Completed");


            }
            catch (Exception ex)
            {
                // SendMail("Europe", ex.Message.ToString() + "   Stack Strace:" + ex.StackTrace.ToString());
            }
        }
    }
}


