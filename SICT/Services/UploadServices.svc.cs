///<Copyright> Cross-Tab  </Copyright>
///<ProjectName>SICT </ProjectName>
///<FileName> UploadServices.svc </FileName>
///<CreatedOn>5th march 2015</CreatedOn>

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
using System.IO;


namespace SICT.Service
{
    /// <summary>
    /// This class Implements the API's related to Document upload,
    /// </summary>
    public class UploadServices : IUploadServices
    {
        private static readonly string CLASS_NAME = "UploadServices";  

        [WebInvoke(Method = "POST",
         BodyStyle = WebMessageBodyStyle.Wrapped,
         ResponseFormat = WebMessageFormat.Json,
         UriTemplate = "Upload/{Instance}/{Version}/{SessionId}")]
        public ReturnValue UploadFile(string Instance, string Version, string SessionId, Stream FileStream)
        {
            const string FUNCTION_NAME = "UploadFile";
            ReturnValue Returnvalue = new ReturnValue();
            string Error = string.Empty;
            string UserId = string.Empty;
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start of UploadSPSSFile");
            try
            {
                UserDetailsBusiness ObjSessionValidation = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
                if (ObjSessionValidation.IsSessionIdValid(SessionId))
                {
                    UploadBusiness ObjUploadBusiness = new FactoryBusiness().GetUploadBusiness(Version);
                    Returnvalue = ObjUploadBusiness.UploadFile(FileStream, Instance);
                }
                else
                {
                    Returnvalue.ReturnCode = 0;
                    Returnvalue.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(CLASS_NAME, FUNCTION_NAME, "Invalid session ");
                }
            }
            catch (Exception Ex)
            {
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, Ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End for UploadSPSSFile");
            return Returnvalue;
        }
    }
}


