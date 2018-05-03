using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace SICT.DataContracts
{
    [DataContract]
    public class AircraftQuotaReportResponse : ReturnValue
    {
        [DataMember]
        public List<AirlineQuota> Airlines;
    }
    [DataContract]
    public class AircraftReportDetail
    {
        [DataMember]
        public string AircraftType;

        [DataMember]
        public double Target;

        [DataMember]
        public double BCompletes;

        [DataMember]
        public double TotalCompletes;

        [DataMember]
        public double Incompletes;

        [DataMember]
        public double Distributed;

        [DataMember]
        public double ResponseRate;

        [DataMember]
        public double BResponseRate;

        [DataMember]
        public double TargetAchieved;

        [DataMember]
        public double BTargetAchieved;

        [DataMember]
        public double MissingTarget;

        [DataMember]
        public double MissingBTarget;

        [DataMember]
        public double ToDistribute;

        [DataMember]
        public double ToBDistribute;

        [DataMember]
        public int RCardsDistributed;

        [DataMember]
        public int RTotalCompletes;
    }
    [DataContract]
    public class AircraftReportResponse : ReturnValue
    {
        [DataMember]
        public List<AircraftReportDetail> AircraftReportDetail;
    }
    [DataContract]
    public class Airline
    {
        [DataMember]
        public int AirlineId;

        [DataMember]
        public string FlightNumber;

        [DataMember]
        public int DestinationId;

        [DataMember]
        public int OriginId;

        [DataMember]
        public string Route;

        [DataMember]
        public string Direction;

        [DataMember]
        public int BCardsDistributed;

        [DataMember]
        public Language[] Languages;

        [DataMember]
        public string FlightType;

        [DataMember]
        public string AircraftType;
    }
    [DataContract]
    public class AirlineDetail
    {
        [DataMember]
        public int FormId;

        [DataMember]
        public int AirlineId;

        [DataMember]
        public bool IsSuccess;

        [DataMember]
        public bool IsSerialNoValid;

        [DataMember]
        public List<string> InvalidLanguages;
    }
    [DataContract]
    public class AirlineQuota
    {
        [DataMember]
        public string AirlineName;

        [DataMember]
        public Dictionary<string, int> Aircrafts;
    }
    [DataContract]
    public class AirlineReportDetail
    {
        [DataMember]
        public string AirlineName;

        [DataMember]
        public double Target;

        [DataMember]
        public double BCompletes;

        [DataMember]
        public double ECompletes;

        [DataMember]
        public double PECompletes;

        [DataMember]
        public double FCCompletes;

        [DataMember]
        public double TotalCompletes;

        [DataMember]
        public double Incompletes;

        [DataMember]
        public double Distributed;

        [DataMember]
        public double ResponseRate;

        [DataMember]
        public double BResponseRate;

        [DataMember]
        public double TargetAchieved;

        [DataMember]
        public double BTargetAchieved;

        [DataMember]
        public double MissingTarget;

        [DataMember]
        public double MissingBTarget;

        [DataMember]
        public int RCardsDistributed;

        [DataMember]
        public int RTotalCompletes;

        [DataMember]
        public string Type;
    }
    [DataContract]
    public class AirlineReportResponse : ReturnValue
    {
        [DataMember]
        public List<AirlineReportDetail> AirlineReportDetails;
    }
    [DataContract]
    public class AirportAirlineDetail
    {
        [DataMember]
        public int FlightId;

        [DataMember]
        public int AirlineId;

        [DataMember]
        public string AirlineName;

        [DataMember]
        public string AirlineCode;

        [DataMember]
        public int OriginId;

        [DataMember]
        public string OriginName;

        [DataMember]
        public int DestinationId;

        [DataMember]
        public string DestinationName;

        [DataMember]
        public string Type;

        [DataMember]
        public string Route;

        [DataMember]
        public string Direction;

        [DataMember]
        public string FlightType;
    }
    [DataContract]
    public class AirportDetail
    {
        [DataMember]
        public int AirportId;

        [DataMember]
        public string AirportName;

        [DataMember]
        public string Code;
    }
    [DataContract]
    public class AirportIdVstLoginId
    {
        [DataMember]
        public int AId;

        [DataMember]
        public string AName;

        [DataMember]
        public string Code;

        [DataMember]
        public int LId;
    }
    [DataContract]
    public class AirportReportResponse : ReturnValue
    {
        [DataMember]
        public List<AirportReportDetail> AirportReportDetails;
    }
    [DataContract]
    public class AirportReportDetail
    {
        [DataMember]
        public string OriginName;

        [DataMember]
        public string FlightNumber;

        [DataMember]
        public string AirlineName;

        [DataMember]
        public double Target;

        [DataMember]
        public int BCompletes;

        [DataMember]
        public int TotalCompletes;

        [DataMember]
        public int Distributed;

        [DataMember]
        public double ResponseRate;

        [DataMember]
        public double BResponseRate;

        [DataMember]
        public int RCardsDistributed;

        [DataMember]
        public int RTotalCompletes;

        [DataMember]
        public double TargetAchieved;

        [DataMember]
        public double BTargetAchieved;

        [DataMember]
        public string Type;
    }
    [DataContract]
    public class ConfirmitCountsResponse : ReturnValue
    {
        [DataMember]
        public long Completes;

        [DataMember]
        public long BusinessCompletes;
    }
    [DataContract]
    public class ConfirmitDetail
    {
        [DataMember]
        public long RowCnt;

        [DataMember]
        public long CardNumber;

        [DataMember]
        public string Class;

        [DataMember]
        public string UploadeDate;

        [DataMember]
        public string AirportName;

        [DataMember]
        public string Status;
    }
    [DataContract]
    public class ConfirmitDetailsResponse : ReturnValue
    {
        [DataMember]
        public List<ConfirmitDetail> ConfirmitDetails;

        [DataMember]
        public long RecordsCnt;
    }
    [DataContract]
    public class DepartureFormDetails
    {
        [DataMember]
        public int FormId;

        [DataMember]
        public int AirportId;

        [DataMember]
        public string AirportCode;

        [DataMember]
        public int InterviewerId;

        [DataMember]
        public string Interviewer;

        [DataMember]
        public int AirlineId;

        [DataMember]
        public string Airline;

        [DataMember]
        public string FlightNumber;

        [DataMember]
        public int DestinationId;

        [DataMember]
        public string Destination;

        [DataMember]
        public DateTime DistributionDate;

        [DataMember]
        public int BusinessCards;

        [DataMember]
        public DateTime LastUpdatedDate;

        [DataMember]
        public List<Language> Languages;

        [DataMember]
        public string Type;

        [DataMember]
        public string AircraftType;
    }
    [DataContract]
    public class DepartureFormFilterDetails
    {
        [DataMember]
        public int StartIndex;

        [DataMember]
        public int OffSet;

        [DataMember]
        public string IsDepartureForm;

        [DataMember]
        public int AirportId;

        [DataMember]
        public string FilterValue;

        [DataMember]
        public string Sort;

        [DataMember]
        public bool IsSortByAsc;

        [DataMember]
        public FormFilters FormFilters;
    }
    [DataContract]
    public class Detail
    {
        [DataMember]
        public int Id;

        [DataMember]
        public string Name;
    }
    [DataContract]
    public class DirectionDetail
    {
        [DataMember]
        public int DirectionId;

        [DataMember]
        public string DirectionName;
    }
    [DataContract]
    public class DODReportDetail
    {
        [DataMember]
        public string DOD;

        [DataMember]
        public int BCompletes;

        [DataMember]
        public int TotalCompletes;

        [DataMember]
        public int Incompletes;

        [DataMember]
        public int Distributed;

        [DataMember]
        public int RCardsDistributed;

        [DataMember]
        public int RTotalCompletes;

        [DataMember]
        public double ResponseRate;

        [DataMember]
        public double BResponseRate;
    }
    [DataContract]
    public class DODReportResponse : ReturnValue
    {
        [DataMember]
        public List<DODReportDetail> DODReportDetails;
    }
    [DataContract]
    public class DownloadResponse : ReturnValue
    {
        [DataMember]
        public string FileLink;

        [DataMember]
        public string FileName;
    }
    [DataContract]
    public class DownloadStatusResponse : ReturnValue
    {
        [DataMember]
        public bool IsDownloadComplete;
    }
    [DataContract]
    public class FinalDepartureFormDetails : ReturnValue
    {
        [DataMember]
        public int TotalRecords;

        [DataMember]
        public List<DepartureFormDetails> DepartureFormDetails;
    }
    [DataContract]
    public class FlightCombinationResponse : ReturnValue
    {
        [DataMember]
        public List<FlightDetail> FlightDetails;

        [DataMember]
        public int RecordsCnt;
    }
    [DataContract]
    public class FlightDeleteResponse : ReturnValue
    {
        [DataMember]
        public bool IsTargetDeleteRequired;

        [DataMember]
        public int TargetId;
    }
    [DataContract]
    public class FlightDetail
    {
        [DataMember]
        public int FlightCombinationId;

        [DataMember]
        public int AirlineId;

        [DataMember]
        public string AirlineName;

        [DataMember]
        public int OriginId;

        [DataMember]
        public string Origin;

        [DataMember]
        public int DestinationId;

        [DataMember]
        public string Destination;

        [DataMember]
        public string Type;

        [DataMember]
        public string Route;

        [DataMember]
        public string Direction;

        [DataMember]
        public string FlightType;
    }
    [DataContract]
    public class FlightReportDetail
    {
        [DataMember]
        public string FlightNumber;

        [DataMember]
        public string FlightType;

        [DataMember]
        public string AirlineName;

        [DataMember]
        public string OriginName;

        [DataMember]
        public string DestinationName;

        [DataMember]
        public int BCompletes;

        [DataMember]
        public int TotalCompletes;

        [DataMember]
        public int Incompletes;

        [DataMember]
        public int Distributed;

        [DataMember]
        public int RCardsDistributed;

        [DataMember]
        public int RTotalCompletes;


        [DataMember]
        public double ResponseRate;

        [DataMember]
        public double BResponseRate;
    }
    [DataContract]
    public class FlightReportResponse : ReturnValue
    {
        [DataMember]
        public List<FlightReportDetail> FlightReportDetails;
    }
    [DataContract]
    public class FlightResponse : ReturnValue
    {
        [DataMember]
        public bool IsTargetPresent;
    }
    [DataContract]
    public class FormDetails
    {
        [DataMember]
        public int FormId;

        [DataMember]
        public bool IsDepartureForm;

        [DataMember]
        public int AirportId;

        [DataMember]
        public string FieldWorkDate;

        [DataMember]
        public int InterviewerId;

        [DataMember]
        public Airline[] Airlines;
    }
    [DataContract]
    public class FormFilters
    {
        [DataMember]
        public string StartDate;

        [DataMember]
        public string EndDate;

        [DataMember]
        public int DestinationId;

        [DataMember]
        public int AirlineId;

        [DataMember]
        public int InterviewerId;

        [DataMember]
        public string AirportName;

        [DataMember]
        public string DestinationName;

        [DataMember]
        public string AirlineName;

        [DataMember]
        public string InterviewerName;

        [DataMember]
        public string FlightNumber;
    }
    [DataContract]
    public class FormSubmitDetails : ReturnValue
    {
        [DataMember]
        public List<AirlineDetail> AirlineDetails;
    }
    [DataContract]
    public class InterviewerDetail
    {
        [DataMember]
        public int InterviewerId;

        [DataMember]
        public string InterviewerName;

        [DataMember]
        public int AirportId;

        [DataMember]
        public string AirportName;

        [DataMember]
        public string Code;

        [DataMember]
        public bool IsActive;
    }
    [DataContract]
    public class InterviewerDetailResponse : ReturnValue
    {
        [DataMember]
        public List<InterviewerDetail> InterviewerDetail;

        [DataMember]
        public int RecordsCnt;
    }
    [DataContract]
    public class InterviewerReportDetail
    {
        [DataMember]
        public int InterviewerId;

        [DataMember]
        public string InterviewerName;

        [DataMember]
        public int BCompletes;

        [DataMember]
        public int TotalCompletes;

        [DataMember]
        public int Incompletes;

        [DataMember]
        public int Distributed;

        [DataMember]
        public int RCardsDistributed;

        [DataMember]
        public int RTotalCompletes;

        [DataMember]
        public double ResponseRate;

        [DataMember]
        public double BResponseRate;
    }
    [DataContract]
    public class InterviewerReportResponse : ReturnValue
    {
        [DataMember]
        public List<InterviewerReportDetail> InterviewerReportDetails;
    }
    [DataContract]
    public class Language
    {
        [DataMember]
        public int LanguageId;

        [DataMember]
        public int OrderId;

        [DataMember]
        public long FirstSerialNo;

        [DataMember]
        public long LastSerialNo;
    }
    [DataContract]
    public class LanguageDetail
    {
        [DataMember]
        public int LanguageId;

        [DataMember]
        public string LanguageName;
    }
    [DataContract]
    public class LoginInformation : ReturnValue
    {
        [DataMember]
        public bool IsValidUser;

        [DataMember]
        public int AirportLoginId;

        [DataMember]
        public int AirportId;

        [DataMember]
        public string RoleId;

        [DataMember]
        public string SessionId;

        [DataMember]
        public bool DepartureFormAccess;

        [DataMember]
        public bool ArivalFormAccess;

        [DataMember]
        public string AirportName;

        [DataMember]
        public bool IsSuperAdmin;

        [DataMember]
        public bool IsSpecialUser;

        [DataMember]
        public string LastUploadDate;
    }
    [DataContract]
    public class MissingTargetsVsBusinessClass
    {
        [DataMember]
        public int AirlineId;

        [DataMember]
        public string AirlineName;

        [DataMember]
        public string Code;

        [DataMember]
        public int MissingTarget;

        [DataMember]
        public int MissingCompletes;
    }
    [DataContract]
    public class ReturnValue
    {
        [DataMember]
        public int ReturnCode
        {
            get;
            set;
        }

        [DataMember]
        public string ReturnMessage
        {
            get;
            set;
        }

        public ReturnValue()
        {
            this.ReturnCode = 0;
            this.ReturnMessage = "";
        }
    }
    [DataContract]
    public class RouteDetail
    {
        [DataMember]
        public int RouteId;

        [DataMember]
        public string RouteName;
    }
    [DataContract]
    public class ResponseUserDetail : ReturnValue
    {
        [DataMember]
        public List<UserDetail> UserDetails;
    }
    [DataContract]
    public class SerialNoFilterDetails
    {
        [DataMember]
        public int StartIndex;

        [DataMember]
        public int OffSet;

        [DataMember]
        public string FilterValue;

        [DataMember]
        public string Sort;

        [DataMember]
        public bool IsSortByAsc;

        [DataMember]
        public long StartSerialNo;

        [DataMember]
        public long EndSerialNo;
    }
    [DataContract]
    public class SessionUpdateResponse : ReturnValue
    {
        [DataMember]
        public string SessionId;
    }
    [DataContract]
    public class TargetDetail
    {
        [DataMember]
        public int DistributionTargetId;

        [DataMember]
        public int AirlineId;

        [DataMember]
        public string AirlineName;

        [DataMember]
        public int OriginId;

        [DataMember]
        public string Origin;

        [DataMember]
        public string Code;

        [DataMember]
        public int Target;

        [DataMember]
        public string Type;

        [DataMember]
        public string Route;

        [DataMember]
        public string Direction;

        [DataMember]
        public string FlightType;

        [DataMember]
        public string AircraftType;
    }
    [DataContract]
    public class TargetDetailResponse : ReturnValue
    {
        [DataMember]
        public List<TargetDetail> TargetDetails;

        [DataMember]
        public int RecordsCnt;
    }
    [DataContract]
    public class TargetsVsCompletes
    {
        [DataMember]
        public int AirlineId;

        [DataMember]
        public string AirlineName;

        [DataMember]
        public string Code;

        [DataMember]
        public int Target;

        [DataMember]
        public int Completes;
    }
    [DataContract]
    public class TargetUpdateDetail
    {
        [DataMember]
        public int DistributionTargetId;

        [DataMember]
        public int Target;
    }
    [DataContract]
    public class UserDetail
    {
        [DataMember]
        public int UserId;

        [DataMember]
        public int AirportId;

        [DataMember]
        public string AirportName;

        [DataMember]
        public string UserName;

        [DataMember]
        public string Password;

        [DataMember]
        public bool IsActive;

        [DataMember]
        public bool ArrivalFormAccess;

        [DataMember]
        public bool DepartureFormAccess;

        [DataMember]
        public int RoleId;

        [DataMember]
        public bool IsLogin;
    }
}

