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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IDashboardServices" in both code and config file together.
    [ServiceContract]
    public interface IDashboardServices
    {
        [OperationContract]
        ReturnValue CreateTargetVsCompletesCacheFiles(string SessionId);
    }
}
