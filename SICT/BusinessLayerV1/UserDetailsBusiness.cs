using SICT.Constants;
using SICT.DataAccessLayer;
using SICT.DataContracts;
using SICT.SMTPUtils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SICT.BusinessLayer.V1
{
    public class UserDetailsBusiness
    {
        private static readonly string CLASS_NAME = "UserDetailsBusiness";

        public int Add(int a, int b)
        {
            return a + b;
        }

        public int Subtract(int a, int b)
        {
            return a - b;
        }

        public int Multiply(int a, int b)
        {
            return a * b;
        }

        public bool IsSessionIdValid(string SessionId, bool IsValidCheck = false)
        {
            bool IsValid = false;
            SICTLogger.WriteInfo(UserDetailsBusiness.CLASS_NAME, "IsSessionIdValid", " Start for Session id-" + SessionId);
            DataTable DT = new DataTable();
            try
            {
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                IsValid = DBLayer.CheckSessionIdExisistOrNot(SessionId, IsValidCheck);
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(UserDetailsBusiness.CLASS_NAME, "IsSessionIdValid", Ex);
            }
            SICTLogger.WriteInfo(UserDetailsBusiness.CLASS_NAME, "IsSessionIdValid", "End for SessonId -" + SessionId);
            return IsValid;
        }

        public LoginInformation CompareHashAndAuthenticate(string HashString)
        {
            LoginInformation LoginInformation = new LoginInformation();
            SICTLogger.WriteInfo(UserDetailsBusiness.CLASS_NAME, "CompareHashAndAuthenticate ", "Start ");
            try
            {
                string UName = string.Empty;
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                DataTable DataTable = new DataTable();
                DataTable DtUpload = new DataTable();
                bool IsAuthenticatedUser = false;
                DataTable = DBLayer.GetAllUserNameAndPassword();
                DtUpload = DBLayer.GetLastUploadDate();
                foreach (DataRow Dr in DataTable.Rows)
                {
                    string UserName = Dr[BusinessConstants.USERNAME].ToString();
                    string Password = Dr[BusinessConstants.PASSWORD].ToString();
                    string UserNameAndPassword = string.Format("{0}&{1}", UserName, Password);
                    if (string.Compare(this.CreateMD5Hash(UserNameAndPassword), HashString, true) == 0)
                    {
                        UName = UserName;
                        LoginInformation = this.Authenticate(Dr);
                        IsAuthenticatedUser = true;
                        break;
                    }
                }
                if (!IsAuthenticatedUser)
                {
                    LoginInformation.ReturnCode = 0;
                    LoginInformation.SessionId = null;
                    LoginInformation.ReturnMessage = "Invalid Username or Password";
                }
                if (DtUpload.Rows.Count > 0)
                {
                    System.Collections.Generic.List<string> UploadDates = new System.Collections.Generic.List<string>();
                    string UploadDate = string.Empty;
                    if (!string.IsNullOrEmpty(DtUpload.Rows[0][BusinessConstants.LASTUPLOADDATE].ToString()))
                    {
                        UploadDate = System.Convert.ToDateTime(DtUpload.Rows[0][BusinessConstants.LASTUPLOADDATE].ToString()).ToUniversalTime().ToString();
                        UploadDate = System.Convert.ToDateTime(UploadDate).ToString("MM/dd/yyyy/HH/mm");
                        UploadDate = UploadDate.Replace(".", "/");
                        UploadDate = UploadDate.Replace(":", "/");
                        LoginInformation.LastUploadDate = UploadDate;
                    }
                }
                LoginInformation.IsSpecialUser = this.CheckIsSpecialUserOrNot(UName);
                LoginInformation.AirportId = System.Convert.ToInt32(DBLayer.GetAirportIdByUserName(UName));
            }
            catch (System.Exception Ex)
            {
                LoginInformation.ReturnCode = -1;
                LoginInformation.ReturnMessage = "Error in Function Execution";
                SICTLogger.WriteException(UserDetailsBusiness.CLASS_NAME, "CompareHashAndAuthenticate ", Ex);
            }
            SICTLogger.WriteInfo(UserDetailsBusiness.CLASS_NAME, "CompareHashAndAuthenticate ", "End");
            return LoginInformation;
        }

        public bool CheckIsSpecialUserOrNot(string UserName)
        {
            bool IsSpecialUser = false;
            try
            {
                string[] SUserList = System.Convert.ToString(ConfigurationManager.AppSettings["SpecialUsers"]).Split(new char[]
                {
                    ','
                });
                string[] array = SUserList;
                for (int i = 0; i < array.Length; i++)
                {
                    string s = array[i];
                    if (s.Equals(UserName, System.StringComparison.OrdinalIgnoreCase))
                    {
                        IsSpecialUser = true;
                        break;
                    }
                }
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(UserDetailsBusiness.CLASS_NAME, "CheckIsSpecialUserOrNot", Ex);
            }
            return IsSpecialUser;
        }

        private string CreateMD5Hash(string UserNameAndPassword)
        {
            SICTLogger.WriteInfo(UserDetailsBusiness.CLASS_NAME, "CreateMD5Hash ", "Start ");
            System.Text.StringBuilder StringBuilder = new System.Text.StringBuilder();
            try
            {
                System.Security.Cryptography.MD5 mD = System.Security.Cryptography.MD5.Create();
                byte[] Bytes = System.Text.Encoding.ASCII.GetBytes(UserNameAndPassword);
                byte[] Array = mD.ComputeHash(Bytes);
                for (int i = 0; i < Array.Length; i++)
                {
                    StringBuilder.Append(Array[i].ToString("x2"));
                }
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(UserDetailsBusiness.CLASS_NAME, "CreateMD5Hash ", Ex);
            }
            SICTLogger.WriteInfo(UserDetailsBusiness.CLASS_NAME, "CreateMD5Hash ", "End");
            return StringBuilder.ToString();
        }

        private LoginInformation Authenticate(DataRow UserDetails)
        {
            LoginInformation LoginReturnValue = new LoginInformation();
            SICTLogger.WriteInfo(UserDetailsBusiness.CLASS_NAME, "Authenticate ", "Start ");
            try
            {
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                SICTLogger.WriteInfo(UserDetailsBusiness.CLASS_NAME, "Authenticate ", "Entered Authentication: - UserName-" + UserDetails["UserName"].ToString());
                int UserId = System.Convert.ToInt32(UserDetails[BusinessConstants.USERID].ToString());
                string UserName = UserDetails[BusinessConstants.USERNAME].ToString();
                string Password = UserDetails[BusinessConstants.PASSWORD].ToString();
                string Role = UserDetails[BusinessConstants.ROLE].ToString();
                string AirportName = string.Empty;
                bool DepartureFormAccess = false;
                bool ArivalFormAccess = false;
                bool IsSuperAdmin = false;
                if (!string.IsNullOrEmpty(UserDetails[BusinessConstants.DEPARTUREFORMACCESS].ToString()))
                {
                    DepartureFormAccess = System.Convert.ToBoolean(UserDetails[BusinessConstants.DEPARTUREFORMACCESS].ToString());
                }
                if (!string.IsNullOrEmpty(UserDetails[BusinessConstants.ARIVALFORMACCESS].ToString()))
                {
                    ArivalFormAccess = System.Convert.ToBoolean(UserDetails[BusinessConstants.ARIVALFORMACCESS].ToString());
                }
                if (!string.IsNullOrEmpty(UserDetails[BusinessConstants.AIRPORTNAME].ToString()))
                {
                    AirportName = UserDetails[BusinessConstants.AIRPORTNAME].ToString();
                }
                if (!string.IsNullOrEmpty(UserDetails[BusinessConstants.ISSUPERADMIN].ToString()))
                {
                    IsSuperAdmin = System.Convert.ToBoolean(UserDetails[BusinessConstants.ISSUPERADMIN].ToString());
                }
                double LoggedInDuration = 0.0;
                bool IsAuthenticatedUser = false;
                SICTLogger.WriteVerbose(UserDetailsBusiness.CLASS_NAME, "Authenticate ", "Checking if the user as already logged in - ");
                bool IsUserLoggedIn = DBLayer.IsUserAlreadyLoggedIn(UserId, ref LoggedInDuration);
                if (!IsUserLoggedIn || (IsUserLoggedIn && LoggedInDuration > System.Convert.ToDouble(ConfigurationManager.AppSettings[BusinessConstants.SESSIONEXPIRYTIME])))
                {
                    LoginReturnValue.SessionId = this.GenerateSessionId(UserId);
                }
                else if (IsUserLoggedIn && LoggedInDuration < System.Convert.ToDouble(ConfigurationManager.AppSettings[BusinessConstants.SESSIONEXPIRYTIME]))
                {
                    LoginReturnValue.SessionId = DBLayer.GetExistingSessionIdAndUpdateTime(UserId);
                }
                SICTLogger.WriteVerbose(UserDetailsBusiness.CLASS_NAME, "Authenticate ", "Assinging values for the LoginInformation Class");
                LoginReturnValue.AirportLoginId = UserId;
                LoginReturnValue.AirportName = AirportName;
                LoginReturnValue.RoleId = Role.ToString();
                LoginReturnValue.DepartureFormAccess = DepartureFormAccess;
                LoginReturnValue.ArivalFormAccess = ArivalFormAccess;
                LoginReturnValue.IsSuperAdmin = IsSuperAdmin;
                LoginReturnValue.ReturnCode = 1;
                LoginReturnValue.IsValidUser = true;
                LoginReturnValue.ReturnMessage = "Login Successful";
                new AuditLogBusiness().AddLoginAuditLog(IsAuthenticatedUser, LoginReturnValue.SessionId, UserName, Password);
            }
            catch (System.Exception Ex)
            {
                LoginReturnValue.ReturnCode = -1;
                LoginReturnValue.IsValidUser = false;
                LoginReturnValue.ReturnMessage = "Login UnSuccessful - Error in API";
                SICTLogger.WriteException(UserDetailsBusiness.CLASS_NAME, "Authenticate ", Ex);
            }
            SICTLogger.WriteInfo(UserDetailsBusiness.CLASS_NAME, "Authenticate ", "End ");
            return LoginReturnValue;
        }

        private string GenerateSessionId(int UserId)
        {
            string IsInsertSuccessful = string.Empty;
            SICTLogger.WriteInfo(UserDetailsBusiness.CLASS_NAME, "GenerateSessionId", "Start For UserID -" + UserId);
            try
            {
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                SICTLogger.WriteInfo(UserDetailsBusiness.CLASS_NAME, "GenerateSessionId", "Generating SessionId for UserId: " + UserId);
                string Sessionid = System.Guid.NewGuid().ToString();
                SICTLogger.WriteVerbose(UserDetailsBusiness.CLASS_NAME, "GenerateSessionId", "Inserting Session Id into the Session Table for UserId- " + UserId);
                IsInsertSuccessful = DBLayer.InsertValuesToSessionTable(Sessionid, UserId);
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(UserDetailsBusiness.CLASS_NAME, "GenerateSessionId", Ex);
            }
            SICTLogger.WriteInfo(UserDetailsBusiness.CLASS_NAME, "GenerateSessionId", "End For UserId " + UserId);
            return IsInsertSuccessful;
        }

        public SessionUpdateResponse UpdateSessionId(int UserId)
        {
            string SessionId = string.Empty;
            SessionUpdateResponse SessionUpdateResponse = new SessionUpdateResponse();
            SICTLogger.WriteInfo(UserDetailsBusiness.CLASS_NAME, "UpdateSessionId", "Start For UserID -" + UserId);
            try
            {
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                SICTLogger.WriteInfo(UserDetailsBusiness.CLASS_NAME, "UpdateSessionId", "Generating SessionId for UserId: " + UserId);
                string Sessionid = System.Guid.NewGuid().ToString();
                SICTLogger.WriteVerbose(UserDetailsBusiness.CLASS_NAME, "UpdateSessionId", "Inserting Session Id into the Session Table for UserId- " + UserId);
                SessionId = DBLayer.InsertValuesToSessionTable(Sessionid, UserId);
                if (!string.IsNullOrEmpty(SessionId))
                {
                    SessionUpdateResponse.ReturnCode = 1;
                    SessionUpdateResponse.ReturnMessage = "Updationg SessionId Successful";
                    SessionUpdateResponse.SessionId = SessionId;
                }
                else
                {
                    SessionUpdateResponse.ReturnCode = -1;
                    SessionUpdateResponse.ReturnMessage = "Updationg SessionId Failed in DB";
                }
            }
            catch (System.Exception Ex)
            {
                SessionUpdateResponse.ReturnCode = -1;
                SessionUpdateResponse.ReturnMessage = "Error in API";
                SICTLogger.WriteException(UserDetailsBusiness.CLASS_NAME, "UpdateSessionId", Ex);
            }
            SICTLogger.WriteInfo(UserDetailsBusiness.CLASS_NAME, "UpdateSessionId", "End For UserId " + UserId);
            return SessionUpdateResponse;
        }

        public ReturnValue AddUser(UserDetail UserDetail, string Instance)
        {
            ReturnValue ReturnValue = new ReturnValue();
            SICTLogger.WriteInfo(UserDetailsBusiness.CLASS_NAME, "AddUser", "Start ");
            try
            {
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                SICTLogger.WriteVerbose(UserDetailsBusiness.CLASS_NAME, "AddUser", "Inserting User Details to DB Start");
                ReturnValue = DBLayer.AddUser(UserDetail.AirportName, UserDetail.UserName, UserDetail.Password, UserDetail.RoleId, UserDetail.IsActive, UserDetail.ArrivalFormAccess, UserDetail.DepartureFormAccess, UserDetail.IsLogin);
                Task.Factory.StartNew<ReturnValue>(() => new CacheFileBusiness().CreateCacheFileforAiprort());
                Task.Factory.StartNew<ReturnValue>(() => new CacheFileBusiness().CreateCacheFileforTargetVsCompletesCharts(Instance));
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(UserDetailsBusiness.CLASS_NAME, "AddUser", Ex);
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in API";
            }
            SICTLogger.WriteInfo(UserDetailsBusiness.CLASS_NAME, "AddUser", "End");
            return ReturnValue;
        }

        public ReturnValue UpdateUser(UserDetail UserDetail, string Instance)
        {
            ReturnValue ReturnValue = new ReturnValue();
            SICTLogger.WriteInfo(UserDetailsBusiness.CLASS_NAME, "UpdateUser", "Start ");
            try
            {
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                SICTLogger.WriteVerbose(UserDetailsBusiness.CLASS_NAME, "UpdateUser", "Inserting Interviewer to DB Start");
                ReturnValue = DBLayer.UpdateUser(UserDetail);
                Task.Factory.StartNew<ReturnValue>(() => new CacheFileBusiness().CreateCacheFileforAiprort());
                Task.Factory.StartNew<ReturnValue>(() => new CacheFileBusiness().CreateCacheFileforTargetVsCompletesCharts(Instance));
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(UserDetailsBusiness.CLASS_NAME, "UpdateUser", Ex);
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in API";
            }
            SICTLogger.WriteInfo(UserDetailsBusiness.CLASS_NAME, "UpdateUser", "End");
            return ReturnValue;
        }

        public ResponseUserDetail GetAllUser()
        {
            ResponseUserDetail ResponseUserDetail = new ResponseUserDetail();
            SICTLogger.WriteInfo(UserDetailsBusiness.CLASS_NAME, "GetAllUser", "Start ");
            try
            {
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                SICTLogger.WriteVerbose(UserDetailsBusiness.CLASS_NAME, "GetAllUser", "Retrieving All User Details from DB Start");
                DataTable DtUser = DBLayer.GetAllUseDetails();
                System.Collections.Generic.List<UserDetail> UserDetails = new System.Collections.Generic.List<UserDetail>();
                foreach (DataRow Dr in DtUser.Rows)
                {
                    UserDetail TempUserDetails = new UserDetail();
                    if (!string.IsNullOrEmpty(Dr[BusinessConstants.USER_USERID].ToString()))
                    {
                        TempUserDetails.UserId = System.Convert.ToInt32(Dr[BusinessConstants.USER_USERID].ToString());
                    }
                    else
                    {
                        TempUserDetails.UserId = -1;
                    }
                    TempUserDetails.IsLogin = System.Convert.ToBoolean(Dr[BusinessConstants.USER_ISLOGIN].ToString());
                    if (!string.IsNullOrEmpty(Dr[BusinessConstants.USER_USERNAME].ToString()))
                    {
                        TempUserDetails.UserName = Dr[BusinessConstants.USER_USERNAME].ToString();
                    }
                    if (!string.IsNullOrEmpty(Dr[BusinessConstants.USER_PASSWORD].ToString()))
                    {
                        TempUserDetails.Password = Dr[BusinessConstants.USER_PASSWORD].ToString();
                    }
                    if (!string.IsNullOrEmpty(Dr[BusinessConstants.AIRPORTID].ToString()))
                    {
                        TempUserDetails.AirportId = System.Convert.ToInt32(Dr[BusinessConstants.AIRPORTID].ToString());
                    }
                    if (!string.IsNullOrEmpty(Dr[BusinessConstants.USER_AIRPORTNAME].ToString()))
                    {
                        string AirportName = Dr[BusinessConstants.USER_AIRPORTNAME].ToString();
                        int Index = AirportName.LastIndexOf('(');
                        if (Index > 0)
                        {
                            AirportName = AirportName.Remove(AirportName.LastIndexOf('('));
                        }
                        AirportName = AirportName.Trim();
                        TempUserDetails.AirportName = AirportName;
                    }
                    if (!string.IsNullOrEmpty(Dr[BusinessConstants.USER_ISACTIVE].ToString()))
                    {
                        TempUserDetails.IsActive = System.Convert.ToBoolean(Dr[BusinessConstants.USER_ISACTIVE].ToString());
                    }
                    if (!string.IsNullOrEmpty(Dr[BusinessConstants.USER_DEPARTUREFORMACCESS].ToString()))
                    {
                        TempUserDetails.DepartureFormAccess = System.Convert.ToBoolean(Dr[BusinessConstants.USER_DEPARTUREFORMACCESS].ToString());
                    }
                    if (!string.IsNullOrEmpty(Dr[BusinessConstants.USER_ARRIVALFORMACCESS].ToString()))
                    {
                        TempUserDetails.ArrivalFormAccess = System.Convert.ToBoolean(Dr[BusinessConstants.USER_ARRIVALFORMACCESS].ToString());
                    }
                    if (!string.IsNullOrEmpty(Dr[BusinessConstants.USER_ROLE].ToString()))
                    {
                        TempUserDetails.RoleId = System.Convert.ToInt32(Dr[BusinessConstants.USER_ROLE].ToString());
                    }
                    UserDetails.Add(TempUserDetails);
                }
                ResponseUserDetail.UserDetails = UserDetails;
                ResponseUserDetail.ReturnCode = 1;
                ResponseUserDetail.ReturnMessage = "Retrieved Successfully";
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(UserDetailsBusiness.CLASS_NAME, "GetAllUser", Ex);
                ResponseUserDetail.ReturnCode = -1;
                ResponseUserDetail.ReturnMessage = "Error in API";
            }
            SICTLogger.WriteInfo(UserDetailsBusiness.CLASS_NAME, "GetAllUser", "End");
            return ResponseUserDetail;
        }

        public ReturnValue SendForgotPasswordMail(string Instance, string UserName, string UserMailId)
        {
            ReturnValue ReturnValue = new ReturnValue();
            SICTLogger.WriteInfo(UserDetailsBusiness.CLASS_NAME, "SendForgotPasswordMail", "Start");
            try
            {
                string ToMailAdress = string.Empty;
                ToMailAdress = ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_SMTP_ADMINMAILID].ToString();
                string InstanceName = string.Empty;
                string logoImageURL = string.Empty;
                if (Instance == BusinessConstants.Instance.US.ToString())
                {
                    InstanceName = "US International";
                }
                else if (Instance == BusinessConstants.Instance.EUR.ToString())
                {
                    InstanceName = "Europe";
                }
                else if (Instance == BusinessConstants.Instance.USD.ToString())
                {
                    InstanceName = "US Domestic";
                }
                else if (Instance == BusinessConstants.Instance.AIR.ToString())
                {
                    InstanceName = "Aircraft ";
                }
                string Title = string.Empty;
                Title = "Forgot Password Request ";
                SMTPUtil MailUtil = new SMTPUtil();
                string MailBody = string.Empty;
                string TemplatePath = BusinessConstants.CONFIGURATION_MAIL_FORGOTPASSWORDFORMATPATH;
                using (System.IO.StreamReader InStream = new System.IO.StreamReader(ConfigurationManager.AppSettings[TemplatePath].ToString()))
                {
                    MailBody = InStream.ReadToEnd();
                }
                MailBody = BusinessConstants.MAIL_FORGOT_PASSWORD_BODY;
                MailBody = MailBody.Replace("@emailId", UserMailId);
                MailBody = MailBody.Replace("@UserId", UserName);
                MailBody = MailBody.Replace("@InstanceName", InstanceName);
                MailBody = MailBody.Replace("@regardsAddress", ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_SMTP_REGARDS].ToString());
                MailBody = MailBody.Replace("@logoPath", ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_SMTP_LOGOURL].ToString());
                MailBody = MailBody.Replace("@appName", ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_SMTP_APPNAME].ToString());
                MailUtil.SendMail(ToMailAdress, MailBody, Title);
                ReturnValue.ReturnCode = 1;
                ReturnValue.ReturnMessage = "Mail Sent Successfully ";
            }
            catch (System.Exception ex)
            {
                SICTLogger.WriteException(UserDetailsBusiness.CLASS_NAME, "SendForgotPasswordMail", ex);
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Mail Sending Failed";
            }
            SICTLogger.WriteInfo(UserDetailsBusiness.CLASS_NAME, "SendForgotPasswordMail", "End");
            return ReturnValue;
        }
    }
}
