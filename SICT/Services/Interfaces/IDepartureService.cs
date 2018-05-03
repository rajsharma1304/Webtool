using SICT.DataContracts;
using System;
using System.ServiceModel;

namespace SICT.Interface
{
    [ServiceContract]
    public interface IDepartureService
    {
        [OperationContract]
        FinalDepartureFormDetails GetDepartureFormDetails(string Instance, string SessionId, string Version, DepartureFormFilterDetails DepartureFormFilterDetails);

        [OperationContract]
        FormSubmitDetails SaveFormDetails(string Instance, string SessionId, string Version, FormDetails FormDetails, string AirportId);

        [OperationContract]
        FormSubmitDetails UpdateFormDetails(string Instance, string SessionId, string Version, FormDetails FormDetails);

        [OperationContract]
        ReturnValue DeleteFormDetails(string Instance, string SessionId, string Version, string FormId);

        [OperationContract]
        FinalDepartureFormDetails GetFormsbySerialNo(string Instance, string SessionId, string Version, SerialNoFilterDetails SerialNoFilterDetails);
    }
}
