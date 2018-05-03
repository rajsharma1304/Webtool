using SICT.Constants;
using SICT.DataAccessLayer;
using System;
using System.Threading.Tasks;

namespace SICT.BusinessLayer.V1
{
    public class AuditLogBusiness
    {
        private static readonly string CLASS_NAME = "AuditLogBusiness";

        public void AddLoginAuditLog(bool RetValue, string SessionId, string UserNAme, string PassWord)
        {
            string Message = string.Empty;
            if (RetValue)
            {
                Message = string.Format(BusinessConstants.MESSAGE_LOGIN_SUCCESSFULL, SessionId, UserNAme);
            }
            else
            {
                Message = string.Format(BusinessConstants.MESSAGE_LOGIN_UNSUCCESSFULL, UserNAme, PassWord);
            }
            this.AddAuditLog(SessionId, BusinessConstants.AUDITLOG_SOURCE_LOGIN, BusinessConstants.AUDITLOG_TYPE_LOGIN, Message, null, null);
        }

        public void AddAuditLog(string SessionId, string SourceType, string AuditType, string Description, string BrowserDetail = null, string DataRecieved = null)
        {
            try
            {
               DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                Task.Factory.StartNew(delegate
                {
                    DBLayer.AddAuditLogInfo(SessionId, SourceType, AuditType, Description, BrowserDetail, DataRecieved);
                });
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(AuditLogBusiness.CLASS_NAME, "AddAuditLog", Ex);
            }
        }

        public void AddFormEntryAuditLog(bool RetValue, string SessionId, int FormId, bool IsADD)
        {
            string Message = string.Empty;
            string AddorUpdate = string.Empty;
            if (IsADD)
            {
                AddorUpdate = "Creation";
            }
            else
            {
                AddorUpdate = "Updation";
            }
            if (RetValue)
            {
                Message = string.Format(BusinessConstants.MESSAGE_FORM_ADD_SUCCESSFULL, FormId, AddorUpdate);
            }
            else
            {
                Message = string.Format(BusinessConstants.MESSAGE_FORM_ADD_UNSUCCESSFULL, AddorUpdate);
            }
            this.AddAuditLog(SessionId, BusinessConstants.AUDITLOG_SOURCE_FORM, BusinessConstants.AUDITLOG_TYPE_ADD, Message, null, null);
        }

        public void DeleteFormEntryAuditLog(bool RetValue, string SessionId, int FormId)
        {
            string Message = string.Empty;
            if (RetValue)
            {
                Message = string.Format(BusinessConstants.MESSAGE_FORM_DELETE_SUCCESSFULL, FormId);
            }
            else
            {
                Message = string.Format(BusinessConstants.MESSAGE_FORM_DELETE_UNSUCCESSFULL, FormId);
            }
            this.AddAuditLog(SessionId, BusinessConstants.AUDITLOG_SOURCE_FORM, BusinessConstants.AUDITLOG_TYPE_DELETE, Message, null, null);
        }
    }
}
