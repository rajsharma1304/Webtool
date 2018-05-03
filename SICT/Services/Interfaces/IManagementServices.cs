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
    public interface IManagementServices
    {
        [OperationContract]
        ReturnValue InsertInterviewer(string Instance, string SessionId, string Version, InterviewerDetail InterviewerDetail);

        [OperationContract]
        ReturnValue UpdateInterviewer(string Instance, string SessionId, string Version, InterviewerDetail InterviewerDetail);

        [OperationContract]
        InterviewerDetailResponse GetAllInterviewers(string Instance, string Version, string SessionId);

        [OperationContract]
        InterviewerDetailResponse GetInterviewersByRange(string Instance, string Version, string SessionId, string StartIndex, string Offset, string AirportId, string InterviewerName);

        [OperationContract]
        ReturnValue InsertTarget(string Instance, string SessionId, string Version, TargetDetail TargetDetail);

        [OperationContract]
        ReturnValue UpdateTarget(string Instance, string SessionId, string Version, List<TargetUpdateDetail> TargetUpdateDetails);

        [OperationContract]
        TargetDetailResponse GetTargetsByRange(string Instance, string Version, string SessionId, string StartIndex, string Offset, string FormType, string OriginId, string AirlineId, string Route, string Direction, string FlightType, string AircraftType, string Search, string Sort, string IsAsc);

        [OperationContract]
        FlightResponse InsertFlightCombination(string Instance, string SessionId, string Version, FlightDetail FlightDetail);

        [OperationContract]
        FlightResponse UpdateFlightCombination(string Instance, string SessionId, string Version, FlightDetail FlightDetail);

        [OperationContract]
        FlightDeleteResponse DeleteFlightCombination(string Instance, string SessionId, string Version, string FlightCombinationId);

        [OperationContract]
        FlightCombinationResponse GetFlightCombinationsByRange(string Instance, string Version, string SessionId, string StartIndex, string Offset, string FormType, string OriginId, string DestinationId, string AirlineId, string Route, string Direction, string FlightType, string SearchVal, string SortVal, string SortOrder);

        [OperationContract]
        ReturnValue DeleteTarget(string Instance, string SessionId, string Version, string DistributionTargetId);

        [OperationContract]
        ConfirmitDetailsResponse GetConfirmitCardDetails(string Instance, string Version, string SessionId, string StartIndex, string Offset, string SearchVal, string SortVal, string SortOrder);

        [OperationContract]
        ConfirmitCountsResponse GetConfirmitCounts(string Instance, string Version, string SessionId);
    }
}
    
