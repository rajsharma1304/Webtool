using SICT.Constants;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace SICT.SMTPUtils
{
    public class SMTPUtil
    {
        private static readonly string CLASS_NAME = "SMTPUtil";

        private string FromAddress = string.Empty;

        private string RedirectAddress = string.Empty;

        private string SMTPPassword = string.Empty;

        private string SMTPUser = string.Empty;

        private int SMTPPortNo;

        private string SMTPHostAddress = string.Empty;

        private string DomainName = string.Empty;

        private bool IsRedirectMails;

        public SMTPUtil()
        {
            try
            {
                this.FromAddress = ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_SMTP_FROMADDRESS].ToString();
                this.SMTPPassword = ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_SMTP_SMTPPASSWORD].ToString();
                this.SMTPUser = ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_SMTP_SMTPUSER].ToString();
                this.SMTPPortNo = Convert.ToInt32(ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_SMTP_SMTPPORT].ToString());
                this.SMTPHostAddress = ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_SMTP_SMTPSERVER].ToString();
                this.DomainName = ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_SMTP_DOMAINNAME].ToString();
                this.IsRedirectMails = Convert.ToBoolean(ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_SMTP_REDIRECTALLMAILS]);
                this.RedirectAddress = Convert.ToString(ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_SMTP_REDIRECTADDRESS]);
            }
            catch (Exception)
            {
            }
        }

        private SmtpClient GetSMTPClient()
        {
            SmtpClient smtpClient = new SmtpClient(this.SMTPHostAddress, this.SMTPPortNo);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            if (this.DomainName != string.Empty)
            {
                smtpClient.Credentials = new NetworkCredential(this.SMTPUser, this.SMTPPassword, this.DomainName);
            }
            else
            {
                smtpClient.Credentials = new NetworkCredential(this.SMTPUser, this.SMTPPassword);
            }
            return smtpClient;
        }

        public void SendMail(string MailTo, string MailBody, string Title)
        {
            SICTLogger.WriteInfo(SMTPUtil.CLASS_NAME, "SendMail", "Started sending mail");
            try
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(this.FromAddress);
                if (this.IsRedirectMails || string.IsNullOrEmpty(this.FromAddress))
                {
                    mailMessage.To.Add(new MailAddress(this.RedirectAddress));
                }
                else
                {
                    mailMessage.To.Add(new MailAddress(MailTo));
                }
                mailMessage.Subject = Title;
                mailMessage.Body = MailBody;
                mailMessage.IsBodyHtml = true;
                SmtpClient sMTPClient = this.GetSMTPClient();
                sMTPClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(SMTPUtil.CLASS_NAME, "SendMail", ex);
            }
            SICTLogger.WriteInfo(SMTPUtil.CLASS_NAME, "SendMail", "Completed sending mail");
        }

        public void SendMailWithAttachment(string MailTo, string MailBody, string Title, string[] FilePath)
        {
            SICTLogger.WriteInfo(SMTPUtil.CLASS_NAME, "SendMailWithAttachment", "Started sending mail");
            try
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(this.FromAddress);
                if (this.IsRedirectMails || string.IsNullOrEmpty(this.FromAddress))
                {
                    mailMessage.To.Add(new MailAddress(this.RedirectAddress));
                }
                else
                {
                    mailMessage.To.Add(new MailAddress(MailTo));
                }
                mailMessage.Subject = Title;
                for (int i = 0; i < FilePath.Length; i++)
                {
                    string path = FilePath[i];
                    mailMessage.Attachments.Add(new Attachment(Path.GetFullPath(path)));
                }
                mailMessage.Body = MailBody;
                mailMessage.IsBodyHtml = true;
                SmtpClient sMTPClient = this.GetSMTPClient();
                sMTPClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(SMTPUtil.CLASS_NAME, "SendMailWithAttachment", ex);
            }
        }

        public void SendMailToMany(string[] MailTo, string MailBody, string Title)
        {
            SICTLogger.WriteInfo(SMTPUtil.CLASS_NAME, "SendMailToMany", "Started sending mail");
            try
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(this.FromAddress);
                if (this.IsRedirectMails || string.IsNullOrEmpty(this.FromAddress))
                {
                    mailMessage.To.Add(new MailAddress(this.RedirectAddress));
                }
                else
                {
                    for (int i = 0; i < MailTo.Length; i++)
                    {
                        string address = MailTo[i];
                        mailMessage.To.Add(new MailAddress(address));
                    }
                }
                mailMessage.Subject = Title;
                mailMessage.Body = MailBody;
                mailMessage.IsBodyHtml = true;
                SmtpClient sMTPClient = this.GetSMTPClient();
                sMTPClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(SMTPUtil.CLASS_NAME, "SendMailToMany", ex);
            }
            SICTLogger.WriteInfo(SMTPUtil.CLASS_NAME, "SendMailToMany", "Completed sending mail");
        }
    }
}
