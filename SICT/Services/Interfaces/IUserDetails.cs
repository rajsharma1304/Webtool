using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using SICT.DataContracts;

namespace SICT.Interface
{

    [ServiceContract]
    public interface IUserDetails
    {

        [OperationContract]
        LoginInformation Login(string Instance, string HashString);

        [OperationContract]
        ReturnValue IsSessionIdValid(string Instance, string SessionId);


        [OperationContract]
        ReturnValue CreateCacheFiles(string Instance);

        [OperationContract]
        ReturnValue CreateCacheFilesTargetVsComplete(string Instance);

        [OperationContract]
        ReturnValue CacheFileAirportAirlineList(string Instance);

        [OperationContract]
        SessionUpdateResponse UpdateSession(string Instance, string Version, string UserId);

        [OperationContract]
        ReturnValue AddUser(string Instance, string SessionId, string Version, UserDetail UserDetail);

        [OperationContract]
        ReturnValue UpdateUser(string Instance, string SessionId, string Version, UserDetail UserDetail);

        [OperationContract]
        ReturnValue DeleteUser(string Instance, string SessionId, string Version, UserDetail UserDetail);

        [OperationContract]
        ResponseUserDetail GetAllUsers(string Instance, string Version, string SessionId);

        [OperationContract]
        ReturnValue ForgotPassword(string Instance, string Version, string UserName, string MailId);
        

    }
   


}
