///<Copyright> Cross-Tab  </Copyright>
///<ProjectName>SICT </ProjectName>
///<FileName> DownloadServices.svc </FileName>
///<CreatedOn>5th March 2015</CreatedOn>

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
    /// This class Implements the API's related to Download,
    /// </summary>
    public class DownloadServices : IDownloadServices
    {
        private static readonly string CLASS_NAME = "DownloadServices";


        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, UriTemplate = "Form/Download")]
        public DownloadResponse DownloadFormDetails(string Instance, string SessionId, string Version, DepartureFormFilterDetails FormFilterDetails)
        {
            const string FUNCTION_NAME = "DownloadFormDetails";
            DownloadResponse DownloadResponse = new DownloadResponse();
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start");
            try
            {
                UserDetailsBusiness ObjSessionValidation = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
                if (ObjSessionValidation.IsSessionIdValid(SessionId))
                {
                    DownloadBusiness ObjDownloadBusiness = new FactoryBusiness().GetDownloadBusiness(Version);
                    DownloadResponse = ObjDownloadBusiness.FormExcelExport(Instance,SessionId, FormFilterDetails);
                }
                else
                {
                    DownloadResponse.ReturnCode = 0;
                    DownloadResponse.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(CLASS_NAME, FUNCTION_NAME, "Invalid session ");
                }
            }
            catch (Exception ex)
            {
                DownloadResponse.ReturnCode = -1;
                DownloadResponse.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End");
            return DownloadResponse;
        }


        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "CheckDownloadStatus/{Instance}/{Version}/{SessionId}/{FilePath}")]
        public DownloadStatusResponse CheckDownloadStatus(string Instance, string Version, string SessionId, string FilePath)
        {
            const string FUNCTION_NAME = "CheckDownloadStatus";
            DownloadStatusResponse DownloadStatusResponse = new DownloadStatusResponse();
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start");
            try
            {
                UserDetailsBusiness ObjSessionValidation = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
                if (ObjSessionValidation.IsSessionIdValid(SessionId))
                {
                    DownloadBusiness ObjDownloadBusiness = new FactoryBusiness().GetDownloadBusiness(Version);
                    DownloadStatusResponse = ObjDownloadBusiness.CheckDownloadStatus(FilePath);
                }
                else
                {
                    DownloadStatusResponse.ReturnCode = 0;
                    DownloadStatusResponse.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(CLASS_NAME, FUNCTION_NAME, "Invalid session ");
                }
            }
            catch (Exception ex)
            {
                DownloadStatusResponse.ReturnCode = -1;
                DownloadStatusResponse.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End");
            return DownloadStatusResponse;
        }

    }
}

