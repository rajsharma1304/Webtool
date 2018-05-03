using SICT.BusinessLayer.V1;
using SICT.CommonBusiness;
using SICT.Constants;
using SICT.DataContracts;
using SICT.Interface;
using System;
using System.ServiceModel.Web;

namespace SICT.Service
{
    public class DepartureFormServices : IDepartureService
    {
        private static readonly string CLASS_NAME = "DepartureFormServices";

        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, UriTemplate = "Departure/GetDepartureFormDetails")]
        public FinalDepartureFormDetails GetDepartureFormDetails(string Instance, string SessionId, string Version, DepartureFormFilterDetails DepartureFormFilterDetails)
        {
            FinalDepartureFormDetails finalDepartureFormDetails = new FinalDepartureFormDetails();
            SICTLogger.WriteInfo(DepartureFormServices.CLASS_NAME, "GetDepartureFormDetails", "Start");
            try
            {
                UserDetailsBusiness userDetailsBusiness = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
                if (userDetailsBusiness.IsSessionIdValid(SessionId, false))
                {
                    DepartureFormBusiness departureFormBusiness = new FactoryBusiness().GetDepartureFormBusiness(Version);
                    finalDepartureFormDetails = departureFormBusiness.GetDepartureFormDetails(Instance, SessionId, DepartureFormFilterDetails);
                }
                else
                {
                    finalDepartureFormDetails.ReturnCode = 0;
                    finalDepartureFormDetails.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(DepartureFormServices.CLASS_NAME, "GetDepartureFormDetails", "Invalid session ");
                }
            }
            catch (Exception ex)
            {
                finalDepartureFormDetails.ReturnCode = -1;
                finalDepartureFormDetails.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(DepartureFormServices.CLASS_NAME, "GetDepartureFormDetails", ex);
            }
            SICTLogger.WriteInfo(DepartureFormServices.CLASS_NAME, "GetDepartureFormDetails", "End");
            return finalDepartureFormDetails;
        }

        [WebInvoke(Method = "PUT", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "Form/Add")]
        public FormSubmitDetails SaveFormDetails(string Instance, string SessionId, string Version, FormDetails FormDetails, string AirportId)
        {
            FormSubmitDetails formSubmitDetails = new FormSubmitDetails();
            SICTLogger.WriteInfo(DepartureFormServices.CLASS_NAME, "SaveFormDetails", "Start");
            try
            {
                UserDetailsBusiness userDetailsBusiness = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
                if (userDetailsBusiness.IsSessionIdValid(SessionId, false))
                {
                    DepartureFormBusiness departureFormBusiness = new FactoryBusiness().GetDepartureFormBusiness(Version);
                    formSubmitDetails = departureFormBusiness.SetFormDetails(Instance, SessionId, ref FormDetails);
                }
                else
                {
                    formSubmitDetails.ReturnCode = 0;
                    formSubmitDetails.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(DepartureFormServices.CLASS_NAME, "SaveFormDetails", "Invalid session ");
                }
            }
            catch (Exception ex)
            {
                formSubmitDetails.ReturnCode = -1;
                formSubmitDetails.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(DepartureFormServices.CLASS_NAME, "SaveFormDetails", ex);
            }
            SICTLogger.WriteInfo(DepartureFormServices.CLASS_NAME, "SaveFormDetails", "End");
            return formSubmitDetails;
        }

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "Form/Update")]
        public FormSubmitDetails UpdateFormDetails(string Instance, string SessionId, string Version, FormDetails FormDetails)
        {
            FormSubmitDetails formSubmitDetails = new FormSubmitDetails();
            SICTLogger.WriteInfo(DepartureFormServices.CLASS_NAME, "UpdateFormDetails", "Start");
            try
            {
                UserDetailsBusiness userDetailsBusiness = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
                if (userDetailsBusiness.IsSessionIdValid(SessionId, false))
                {
                    DepartureFormBusiness departureFormBusiness = new FactoryBusiness().GetDepartureFormBusiness(Version);
                    formSubmitDetails = departureFormBusiness.UpdateFormDetails(Instance, FormDetails, SessionId);
                }
                else
                {
                    formSubmitDetails.ReturnCode = 0;
                    formSubmitDetails.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(DepartureFormServices.CLASS_NAME, "UpdateFormDetails", "Invalid session ");
                }
            }
            catch (Exception ex)
            {
                formSubmitDetails.ReturnCode = -1;
                formSubmitDetails.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(DepartureFormServices.CLASS_NAME, "UpdateFormDetails", ex);
            }
            SICTLogger.WriteInfo(DepartureFormServices.CLASS_NAME, "UpdateFormDetails", "End");
            return formSubmitDetails;
        }

        [WebInvoke(Method = "DELETE", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "Form/Delete")]
        public ReturnValue DeleteFormDetails(string Instance, string SessionId, string Version, string FormId)
        {
            ReturnValue returnValue = new ReturnValue();
            SICTLogger.WriteInfo(DepartureFormServices.CLASS_NAME, "DeleteFormDetails", "Start");
            try
            {
                UserDetailsBusiness userDetailsBusiness = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
                if (userDetailsBusiness.IsSessionIdValid(SessionId, false))
                {
                    DepartureFormBusiness departureFormBusiness = new FactoryBusiness().GetDepartureFormBusiness(Version);
                    returnValue = departureFormBusiness.DeleteFormDetails(Convert.ToInt32(FormId));
                }
                else
                {
                    returnValue.ReturnCode = 0;
                    returnValue.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(DepartureFormServices.CLASS_NAME, "DeleteFormDetails", "Invalid session ");
                }
            }
            catch (Exception ex)
            {
                returnValue.ReturnCode = -1;
                returnValue.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(DepartureFormServices.CLASS_NAME, "DeleteFormDetails", ex);
            }
            new AuditLogBusiness().DeleteFormEntryAuditLog(Convert.ToBoolean(returnValue.ReturnCode), SessionId, Convert.ToInt32(FormId));
            SICTLogger.WriteInfo(DepartureFormServices.CLASS_NAME, "DeleteFormDetails", "End");
            return returnValue;
        }

        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, UriTemplate = "Departure/GetFormsbySerialNo")]
        public FinalDepartureFormDetails GetFormsbySerialNo(string Instance, string SessionId, string Version, SerialNoFilterDetails SerialNoFilterDetails)
        {
            FinalDepartureFormDetails finalDepartureFormDetails = new FinalDepartureFormDetails();
            SICTLogger.WriteInfo(DepartureFormServices.CLASS_NAME, "GetFormsbySerialNo", "Start");
            try
            {
                UserDetailsBusiness userDetailsBusiness = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
                if (userDetailsBusiness.IsSessionIdValid(SessionId, false))
                {
                    DepartureFormBusiness departureFormBusiness = new FactoryBusiness().GetDepartureFormBusiness(Version);
                    finalDepartureFormDetails = departureFormBusiness.GetFormsbySerialNo(Instance, SessionId, SerialNoFilterDetails);
                }
                else
                {
                    finalDepartureFormDetails.ReturnCode = 0;
                    finalDepartureFormDetails.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(DepartureFormServices.CLASS_NAME, "GetFormsbySerialNo", "Invalid session ");
                }
            }
            catch (Exception ex)
            {
                finalDepartureFormDetails.ReturnCode = -1;
                finalDepartureFormDetails.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(DepartureFormServices.CLASS_NAME, "GetFormsbySerialNo", ex);
            }
            SICTLogger.WriteInfo(DepartureFormServices.CLASS_NAME, "GetFormsbySerialNo", "End");
            return finalDepartureFormDetails;
        }
    }
}
