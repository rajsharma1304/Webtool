///<Copyright> Cross-Tab  </Copyright>
///<ProjectName>SICT </ProjectName>
///<FileName> UserDetailsServices.svc </FileName>
///<CreatedOn> 6 Jan 2015</CreatedOn>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using SICT.DataContracts;
using SICT.BusinessLayer.V1;
using SICT.Interface;
using SICT.CommonBusiness;
using SICT.Constants;

namespace SICT.Service
{

    /// <summary>
    /// This class API's related to UserManagement like Login, Session Validation Etc  
    /// </summary>
    public class UserDetailsService : IUserDetails
    {        
        private static readonly string CLASS_NAME = "UserDetailsService";

        #region User
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, UriTemplate = "UserManagement/Login")]
        public LoginInformation Login(string Instance, string HashString)
        {
            const string FUNCTION_NAME = "Login";
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start");
            LoginInformation Result = new LoginInformation();
            try
            {                
                UserDetailsBusiness ObjUserDetails = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
                Result = ObjUserDetails.CompareHashAndAuthenticate(HashString);
            }
            catch (Exception ex)
            {
                Result.ReturnCode = -1;
                Result.ReturnMessage = "Error in Function Execution";
                SICTLogger.WriteException(CLASS_NAME, "Login", ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, "Login", "End");
            return Result;
        }


        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "UserManagement/ValidateSession/{Instance}/{SessionId}")]
        public ReturnValue IsSessionIdValid(string Instance, string SessionId)
        {
            const string FUNCTION_NAME = "IsSessionIdValid";
            UserDetailsBusiness ObjSessionValidation = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
            ReturnValue ReturnValue = new ReturnValue();            
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start for SessionId - " + SessionId);
            try
            {
                if (ObjSessionValidation.IsSessionIdValid(SessionId, true))//true not to update the session updated time 
                {
                    ReturnValue.ReturnCode = 1;
                    ReturnValue.ReturnMessage ="Session Valid";
                }
                else
                {
                    ReturnValue.ReturnCode = 0;
                    ReturnValue.ReturnMessage = "Session InValid";
                    SICTLogger.WriteWarning(CLASS_NAME, FUNCTION_NAME, "Invalid session ");
                }
            }
            catch (Exception Ex)
            {
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, Ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End for SessionId- " + SessionId);
            return ReturnValue;
        }


        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "UserManagement/UpdateSession/{Instance}/{Version}/{UserId}")]
        public SessionUpdateResponse UpdateSession(string Instance, string Version, string UserId)
        {
            const string FUNCTION_NAME = "UpdateSession";
            UserDetailsBusiness ObjSessionValidation = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
            SessionUpdateResponse SessionUpdateResponse = new SessionUpdateResponse();
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start for UserId - " + UserId);
            try
            {
                UserDetailsBusiness ObjUserDetails = new FactoryBusiness().GetUserDetailsBusiness(Version);
                SessionUpdateResponse = ObjUserDetails.UpdateSessionId(Convert.ToInt32(UserId));

            }
            catch (Exception Ex)
            {
                SessionUpdateResponse.ReturnCode = -1;
                SessionUpdateResponse.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, Ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End for UserId- " + UserId);
            return SessionUpdateResponse;
        }

        #endregion User


        #region CacheFileCreation
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "CreateCacheFiles/{Instance}")]
        public ReturnValue CreateCacheFiles(string Instance)
        {
            const string FUNCTION_NAME = "CreateCacheFiles";
            UserDetailsBusiness ObjSessionValidation = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);       
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start");
            ReturnValue ReturnValue = new ReturnValue();
            try
            {
                ReturnValue = new CacheFileBusiness().CreateAllCacheFiles(Instance);

            }
            catch (Exception Ex)
            {
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, Ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End ");
            return ReturnValue;
           
        }

        #endregion CacheFileCreation

        #region CacheFileCreationTargetVsComplete
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "CreateCacheFilesTargetVsComplete/{Instance}")]
        public ReturnValue CreateCacheFilesTargetVsComplete(string Instance)
        {
            const string FUNCTION_NAME = "CreateCacheFilesTargetVsComplete";
            UserDetailsBusiness ObjSessionValidation = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start");
            ReturnValue ReturnValue = new ReturnValue();
            try
            {
                ReturnValue = new CacheFileBusiness().CreateCacheFileforTargetVsCompletesCharts(Instance);
                ReturnValue.ReturnCode = 1;
                ReturnValue.ReturnMessage = "Cache file Successfull ";

            }
            catch (Exception Ex)
            {
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, Ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End ");
            return ReturnValue;
        }

        #endregion CacheFileCreationTargetVsComplete

        #region CacheFileAirportAirlineList
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "CacheFileAirportAirlineList/{Instance}")]
        public ReturnValue CacheFileAirportAirlineList(string Instance)
        {
            const string FUNCTION_NAME = "CacheFileAirportAirlineList";
            UserDetailsBusiness ObjSessionValidation = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start");
            ReturnValue ReturnValue = new ReturnValue();
            try
            {
                ReturnValue = new CacheFileBusiness().CreateCacheFileforAiprortAndAirline(Instance,true);
                ReturnValue.ReturnCode = 1;
                ReturnValue.ReturnMessage = "Cache file Successfull ";

            }
            catch (Exception Ex)
            {
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, Ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End ");
            return ReturnValue;
        }
        #endregion CacheFileAirportAirlineList

        #region User

        [WebInvoke(
           Method = "PUT",
           BodyStyle = WebMessageBodyStyle.WrappedRequest,
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "User/Add")]
        public ReturnValue AddUser(string Instance,string SessionId, string Version, UserDetail UserDetail)
        {
            const string FUNCTION_NAME = "AddUser";
            ReturnValue ReturnValue = new ReturnValue();
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start");
            try
            {
                UserDetailsBusiness ObjSessionValidation = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
                UserDetailsBusiness ObjUserDetailsBusiness = new FactoryBusiness().GetUserDetailsBusiness(Version);
                if (ObjSessionValidation.IsSessionIdValid(SessionId))
                {
                    ReturnValue = ObjUserDetailsBusiness.AddUser(UserDetail, Instance);
                }
                else
                {
                    ReturnValue.ReturnCode = 0;
                    ReturnValue.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(CLASS_NAME, FUNCTION_NAME, "Invalid session ");
                }
            }
            catch (Exception ex)
            {
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End");
            return ReturnValue;
        }

        [WebInvoke(
           Method = "POST",
           BodyStyle = WebMessageBodyStyle.WrappedRequest,
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "User/Update")]
        public ReturnValue UpdateUser(string Instance,string SessionId, string Version, UserDetail UserDetail)
        {
            const string FUNCTION_NAME = "UpdateUser";
            ReturnValue ReturnValue = new ReturnValue();
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start");
            try
            {
                UserDetailsBusiness ObjSessionValidation = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
                UserDetailsBusiness ObjUserDetailsBusiness = new FactoryBusiness().GetUserDetailsBusiness(Version);
                if (ObjSessionValidation.IsSessionIdValid(SessionId))
                {
                    ReturnValue = ObjUserDetailsBusiness.UpdateUser(UserDetail, Instance);
                }
                else
                {
                    ReturnValue.ReturnCode = 0;
                    ReturnValue.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(CLASS_NAME, FUNCTION_NAME, "Invalid session ");
                }
            }
            catch (Exception ex)
            {
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End");
            return ReturnValue;
        }

        [WebInvoke(
           Method = "DELETE",
           BodyStyle = WebMessageBodyStyle.WrappedRequest,
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "User/Delete")]
        public ReturnValue DeleteUser(string Instance, string SessionId, string Version, UserDetail UserDetail)
        {
            const string FUNCTION_NAME = "DeleteUser";
            ReturnValue ReturnValue = new ReturnValue();
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start");
            try
            {
                UserDetailsBusiness ObjSessionValidation = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
                UserDetailsBusiness ObjUserDetailsBusiness = new FactoryBusiness().GetUserDetailsBusiness(Version);
                if (ObjSessionValidation.IsSessionIdValid(SessionId))
                {
                   // ReturnValue = ObjUserDetailsBusiness.DeleteUser(UserDetail, Instance);
                }
                else
                {
                    ReturnValue.ReturnCode = 0;
                    ReturnValue.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(CLASS_NAME, FUNCTION_NAME, "Invalid session ");
                }
            }
            catch (Exception ex)
            {
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End");
            return ReturnValue;
        }

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "User/GetAllUsers/{Instance}/{Version}/{SessionId}")]
        public ResponseUserDetail GetAllUsers(string Instance,string Version, string SessionId)
        {
            const string FUNCTION_NAME = "GetAllUsers";
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start");
            ResponseUserDetail ResponseUserDetail = new ResponseUserDetail();
            try
            {
                UserDetailsBusiness ObjSessionValidation = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
                UserDetailsBusiness ObjUserDetailsBusiness = new FactoryBusiness().GetUserDetailsBusiness(Version);
                if (ObjSessionValidation.IsSessionIdValid(SessionId))
                {
                    ResponseUserDetail = ObjUserDetailsBusiness.GetAllUser();
                }
                else
                {
                    ResponseUserDetail.ReturnCode = 0;
                    ResponseUserDetail.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(CLASS_NAME, FUNCTION_NAME, "Invalid session ");
                }

            }
            catch (Exception Ex)
            {
                ResponseUserDetail.ReturnCode = -1;
                ResponseUserDetail.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, Ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End ");
            return ResponseUserDetail;

        }

        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, UriTemplate = "UserManagement/ForgotPassword")]
        public ReturnValue ForgotPassword(string Instance, string Version, string UserName, string MailId)
        {
            const string FUNCTION_NAME = "UpdateUser";
            ReturnValue ReturnValue = new ReturnValue();
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start");
            try
            {
                UserDetailsBusiness ObjSessionValidation = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
                UserDetailsBusiness ObjUserDetailsBusiness = new FactoryBusiness().GetUserDetailsBusiness(Version);
                ReturnValue = ObjUserDetailsBusiness.SendForgotPasswordMail(Instance, UserName, MailId);
            }
            catch (Exception ex)
            {
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End");
            return ReturnValue;
        }

        #endregion User


    }
}
