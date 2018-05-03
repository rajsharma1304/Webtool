using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using SICT.DataContracts;

namespace SICT.Interface
{
    
    [ServiceContract]
    public interface IUploadServices
    {
        [OperationContract]
        ReturnValue UploadFile(string Instance, string Version, string SessionId, Stream FileStream);
    }
}
