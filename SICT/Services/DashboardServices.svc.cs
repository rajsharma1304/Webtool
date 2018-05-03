///<Copyright> Celstream Technologies Pvt. Ltd. </Copyright>
///<ProjectName>SICT </ProjectName>
///<FileName> DashboardServices.svc </FileName>
///<Author>David Boon</Author>
///<CreatedOn> 11 Feb 2015</CreatedOn>

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
    /// This class Implements the API's related to Dasboards
    /// </summary>
    public class DashboardServices : IDashboardServices
    {
        private static readonly string CLASS_NAME = "DashboardServices";

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "CreateTargetVsCompletesCacheFiles/{SessionId}")]
        public ReturnValue CreateTargetVsCompletesCacheFiles(string SessionId)
        {
            const string FUNCTION_NAME = "CreateTargetVsCompletesCacheFiles";
            UserDetailsBusiness ObjSessionValidation = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
            ReturnValue ReturnValue = new ReturnValue();
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start for SessionId - " + SessionId);
            try
            {
                if (ObjSessionValidation.IsSessionIdValid(SessionId))
                {
                    DashboardBusiness ObjDashboardBusiness = new FactoryBusiness().GetDashboardBusiness(BusinessConstants.VERSION_BASE);
                    ObjDashboardBusiness.CreateCacheFileforTargetVsCompletesCharts();
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
    }
}
