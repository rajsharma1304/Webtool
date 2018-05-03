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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IDepartureFormServices" in both code and config file together.
    [ServiceContract]
    public interface IDownloadServices
    {

        [OperationContract]
        DownloadResponse DownloadFormDetails(string Instance, string SessionId, string Version, DepartureFormFilterDetails FormFilterDetails);

        [OperationContract]
        DownloadStatusResponse CheckDownloadStatus(string Instance, string Version, string SessionId, string FilePath);
    }
}


