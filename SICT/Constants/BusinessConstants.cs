///<Copyright> Cross-Tab  </Copyright>
///<ProjectName>SICT </ProjectName>
///<FileName>BusinessConstants.cs</FileName>
///<CreatedOn> 6 Jan 2015</CreatedOn>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace SICT.Constants
{
    /// <summary>
    /// This class Defined all the Read only  Constants used Across other classes and Functions
    /// </summary>
    public class BusinessConstants
    {
        #region Instance

        public enum Instance
        {
            US = 1, USD = 2, EUR = 3, AIR = 4
        };

        #endregion Instance

        #region DB

        public static readonly string LOCALDBCONNECTION = "LocalDBConnection";


        public static string DBConnection
        {
            get
            {
                return ConfigurationManager.ConnectionStrings[LOCALDBCONNECTION].ToString();
            }
        }

        #endregion DB

        #region Common
        public static readonly string VERSION_BASE = "V1";
        public static readonly string USERNAME = "UserName";
        public static readonly string PASSWORD = "Password";
        public static readonly string LASTUPLOADDATE = "LastUploadDate";
        public static readonly string SESSIONEXPIRYTIME = "SessionExpiryTime";
        public static readonly string SEESIONID = "SessionId";
        public static readonly string ROLE = "role";
        public static readonly string SPLIT_OR = " OR ";
        public static readonly string SPLIT_AND = " AND ";
        public static readonly string SPLIT_COMMA = " , ";
        public static readonly string BROWSERDETAILS = "BrowserDetails";
        public static readonly string DATARECIEVED = "DataRecieved";
        public static readonly string USERID = "UserId";
        public static readonly string SESSIONID = "SessionId";
        public static readonly string AIRLINEID = "AirLineId";
        public static readonly string ROLEID = "RoleId";
        public static readonly string ACCESSCONTROLLIST = "AccessControlList";
        public static readonly string LICENSETYPEID = "LicenseTypeId";
        public static readonly string FORM_TOTALRECORDS = "TotalRecords";
        public static readonly string FORM_DISTRIBUTIONDATE = "DistributionDate";
        public static readonly string FORM_LASTUPDATEDTIME = "LastUpdatedTime";
        public static readonly string FORM_BUSINESSCARDS = "BusinessCards";
        public static readonly string FORM_INTERVIEWERID = "InterviewerId";
        public static readonly string FORM_TYPE = "Type";
        public static readonly string FORM_DESTINATIONID = "DestinationId";
        public static readonly string FORM_ORIGINID = "OriginId";
        public static readonly string FORM_ROUTE = "Route";
        public static readonly string FORM_DIRECTION = "Direction";
        public static readonly string FORM_ISCARDNOVALID = "IsCardNOValid";
        public static readonly string FORM_INVALIDLANGUAGES = "InValidLanguages";
        public static readonly string FORM_FLIGHTNUMBER = "FlightNumber";
        public static readonly string FORM_UPDATETIME = "UpdateTime";
        public static readonly string FORM_UPDATEDBYUSERID = "UpdatedByUserId";
        public static readonly string FORM_CLIENTCOMMENTS = "ClientComments";
        public static readonly string FORM_LANGAUGES = "Langauges";
        public static readonly string FORM_STARTCODES = "StartCodes";
        public static readonly string FORM_ENDCODES = "EndCodes";
        public static readonly string FORM_RETVALUE = "RetValue";
        public static readonly string FORM_LANGAUGE = "Language{0}";
        public static readonly string FORM_LANGAUGEID = "LanguageId{0}";
        public static readonly string FORM_STARTCODE = "StartCode{0}";
        public static readonly string FORM_ENDCODE = "EndCode{0}";
        public static readonly string FORM_INTERVIEWERNAME = "InterviewerName";
        public static readonly string FORM_AIRLINENAME = "AirLineName";
        public static readonly string FORM_AIRPORTNAME = "AirportName";
        public static readonly string FORM_DESTINATIONNAME = "DestinationName";
        public static readonly string FORM_FORMID = "FormId";
        public static readonly string FORM_TYPE_DEPARTURE = "D";
        public static readonly string FORM_TYPE_ARRIVAL = "A";
        public static readonly string STARTINDEX = "StartIndex";
        public static readonly string OFFSET = "OffSet";
        public static readonly string ORDERBYCONDITION = "OrderByCondition";
        public static readonly string WHERECONDITION = "WhereCondition";
        public static readonly string TARGETCONDITION = "TargetCondition";
        public static readonly string RESPONSEDATEWHERECONDITION = "ResponseDateWhereCondition";
        public static readonly string ISDEPARTUREFORM = "IsDepartureForm";
        public static readonly string ISSERIALNORETRIEVAL = "IsSerialNoRetrieval";
        public static readonly string DEPARTUREFORMACCESS = "DepartureFormAccess";
        public static readonly string ARIVALFORMACCESS = "ArivalFormAccess";
        public static readonly string ISSUPERADMIN = "IsSuperAdmin";
        public static readonly string AIRPORTID = "AirportId";
        public static readonly string AIRPORTCODE = "AirportCode";
        public static readonly string INTERVIEWERNAME = "InterviewerName";
        public static readonly string INTERVIEWERID = "InterviewerId";
        public static readonly string ISACTIVE = "IsActive";
        public static readonly string AIRPORTNAME = "AirportName";
        public static readonly string ORIGINID = "OriginId";
        public static readonly string ORIGINNAME = "OriginName";
        public static readonly string TARGET = "Target";
        public static readonly string ROUTE = "Route";
        public static readonly string DIRECTION = "Direction";
        public static readonly string AIRLINENAME = "AirLineName";
        public static readonly string ORIGIN = "Origin";
        public static readonly string SELECT = "Select";
        public static readonly string DISTRIBUTIONTARGETID = "DistributionTargetId";
        public static readonly string AIRPORTLOGINID = "AirportLoginId";
        public static readonly string COMPLETES = "Completes";
        public static readonly string BUSINESSCOMPLETES = "BusinessCompletes";
        public static readonly string DESTINATIONID = "DestinationId";
        public static readonly string DESTINATIONNAME = "DestinationName";
        public static readonly string FLIGHTCOMBINATIONID = "FlightCombinationId";
        public static readonly string DESTINATION = "Destination";
        public static readonly string LANGUAGEID = "LanguageId";
        public static readonly string LANGUAGENAME = "LanguageName";
        public static readonly string REPORT_INTERVIEWERID = "InterviewerId";
        public static readonly string REPORT_INTERVIEWERNAME = "InterviewerName";
        public static readonly string REPORT_CARDSDISTRIBUTED = "CardsDistributed";
        public static readonly string REPORT_CARDSDISTRIBUTEDRESPONSE = "CardsDistributedResponse";
        public static readonly string REPORT_BCARDSDISTRIBUTED = "BCardsDistributed";
        public static readonly string REPORT_COMPLETES = "Completes";
        public static readonly string REPORT_COMPLETESRESPONSE = "CompletesResponse";
        public static readonly string REPORT_BUSINESSCOMPLETES = "BusinessCompletes";
        public static readonly string REPORT_ECOMPLETES = "ECompletes";
        public static readonly string REPORT_PECOMPLETES = "PECompletes";
        public static readonly string REPORT_FCCOMPLETES = "FCCompletes";
        public static readonly string REPORT_DOD = "DOD";
        public static readonly string REPORT_FLIGHTNUMBER = "FlightNumber";
        public static readonly string REPORT_FLIGHTTYPE = "FlightType";
        public static readonly string REPORT_DESTINATIONNAME = "DestinationName";
        public static readonly string REPORT_ORIGINNAME = "OriginName";
        public static readonly string REPORT_AIRLINENAME = "AirlineName";
        public static readonly string REPORT_TARGET = "Target";
        public static readonly string ADMIN_CACHEFILE_NAME = "Admin";
        public static readonly string FLIGHTID = "FlightId";
        public static readonly string AIRLINECODE = "AirlineCode";
        public static readonly string ROUTEID = "RouteId";
        public static readonly string ROUTENAME = "RouteName";
        public static readonly string DIRECTIONID = "DirectionId";
        public static readonly string DIRECTIONNAME = "DirectionName";
        public static readonly string ADMINLOGINID = "AdminLoginId";
        public static readonly string SEARCHTEXT = "SearchText";
        public static readonly string DEFAULT_SELECTION_VALUE = "-1";
        public static readonly string ORIGINORDESTNAME = "OriginorDestName";
        public static readonly string DUPLICATE = "Duplicate";
        public static readonly string ISTARGETPRESENT = "IsTargetPresent";
        public static readonly string ISTARGETDELETEREQUIRED = "IsTargetDeleteRequired";
        public static readonly string TARGETID = "TargetId";
        public static readonly string EXTENSION_CSV = ".csv";
        public static readonly string DOWNLOAD_ROWS_CNT = "DownloadCnt";
        public static readonly string CODE = "Code";
        public static readonly string ISTARGETZEROREQUIRED = "IsTargetZeroRequired";
        public static readonly string STATUS = "Status";
        public static readonly string QUARTER = "Quarter";
        public static readonly string YEAR = "Year";
        public static readonly string CONFIG_DB_COMMANDTIMEOUT = "CommandTimeout";
        public static readonly string CONFIG_FORM_DATE_FORMAT = "FormDateFormat";

        public static readonly string FORM_DOWNLOAD_ID = "id";
        public static readonly string FORM_DOWNLOAD_AIRPORT_ID = "airportid";
        public static readonly string FORM_DOWNLOAD_DATE = "date";
        public static readonly string FORM_DOWNLOAD_INTERVIEWER_ID = "interviewer_id";
        public static readonly string FORM_DOWNLOAD_AIRLINES_ID = "airlines_id";
        public static readonly string FORM_DOWNLOAD_DEPTARR = "deptarr";
        public static readonly string FORM_DOWNLOAD_DEPT_ID = "dept_id";
        public static readonly string FORM_DOWNLOAD_FLIGHT_NUMBER = "flight_number";
        public static readonly string FORM_DOWNLOAD_LANG1 = "lang1";
        public static readonly string FORM_DOWNLOAD_STARTCODE1 = "startcode1";
        public static readonly string FORM_DOWNLOAD_ENDCODE1 = "endcode1";
        public static readonly string FORM_DOWNLOAD_LANG2 = "lang2";
        public static readonly string FORM_DOWNLOAD_STARTCODE2 = "startcode2";
        public static readonly string FORM_DOWNLOAD_ENDCODE2 = "endcode2";
        public static readonly string FORM_DOWNLOAD_LANG3 = "lang3";
        public static readonly string FORM_DOWNLOAD_STARTCODE3 = "startcode3";
        public static readonly string FORM_DOWNLOAD_ENDCODE3 = "endcode3";
        public static readonly string FORM_DOWNLOAD_LANG4 = "lang4";
        public static readonly string FORM_DOWNLOAD_STARTCODE4 = "startcode4";
        public static readonly string FORM_DOWNLOAD_ENDCODE4 = "endcode4";
        public static readonly string FORM_DOWNLOAD_LANG5 = "lang5";
        public static readonly string FORM_DOWNLOAD_STARTCODE5 = "startcode5";
        public static readonly string FORM_DOWNLOAD_ENDCODE5 = "endcode5";
        public static readonly string FORM_DOWNLOAD_BUSS_CLASS = "buss_class";
        public static readonly string FORM_DOWNLOAD_CHANGE_TIME = "change_time";
        public static readonly string CONFIGURATION_LICENSING_STATUS = "IsLicensingEnabled";
        public static readonly string DATE1 = "Date1";
        public static readonly string DATE2 = "Date2";
        public static readonly string ISDEPARTURE = "IsDeparture";

        #endregion Common

        #region Eupore
        public static readonly string FLIGHTTYPE = "FlightType";
        public static readonly string FLIGHTTYPEID = "FlightTypeId";
        public static readonly string FLIGHTTYPENAME = "FlightTypeName";
        #endregion Europe

        #region Aircraft
        public static readonly string AIRCRAFTTYPE = "AircraftType";
        public static readonly string AIRCRAFTTYPE_ID = "AircraftId";
        public static readonly string AIRCRAFTTYPE_NAME = "AircraftName";
        #endregion Aircraft

        #region Upload

        public static readonly string CONFIRMIT_CARD_NUMBER = "card_number";
        public static readonly string CONFIRMIT_CLASS = "class";
        public static readonly string CONFIRMIT_STATUS = "status";
        public static readonly string CONFIRMIT_DATETIME = "datetime";
        public static readonly string UPLOAD_ROWS_CNT = "UploadCnt";
        public static readonly string CONFIRMIT_COMPLETESCOUNT = "CompletesCount";
        public static readonly string CONFIRMIT_BUSINESSCOUNT = "BusinessCount";
        public static readonly string CONFIRMIT_CARDNUMBER = "CardNumber";
        public static readonly string CONFIRMIT_CLSS = "Class";
        public static readonly string CONFIRMIT_UPLOADEDATE = "UploadeDate";
        public static readonly string CONFIRMIT_ROWNUMBER = "RowNumber";

        #endregion Upload

        #region User
        public static readonly string RETVALUE = "RetValue";
        public static readonly string USER_AIRPORTNAME = "AirportName";
        public static readonly string USER_USERNAME = "UserName";
        public static readonly string USER_PASSWORD = "Password";
        public static readonly string USER_ARRIVALFORMACCESS = "ArrivalFormAccess";
        public static readonly string USER_DEPARTUREFORMACCESS = "DepartureFormAccess";
        public static readonly string USER_USERID = "UserId";
        public static readonly string USER_ISACTIVE = "IsActive";
        public static readonly string EVENTTYPE = "EventType";
        public static readonly string USER_ROLE = "Role";
        public static readonly string USER_ISLOGIN = "IsLogin";


        #endregion User

        #region AuditLog
        public static readonly string AUDITLOG_SOURCE_LOGIN = "Login";
        public static readonly string AUDITLOG_SOURCE_FORM = "Form";
        public static readonly string AUDITLOG_TYPE_ADD = "Add";
        public static readonly string AUDITLOG_TYPE_UPDATE = "Update";
        public static readonly string AUDITLOG_TYPE_DELETE = "Delete";
        public static readonly string AUDITLOG_TYPE_LOGIN = "Authenticate";
        public static readonly string AUDITLOG_SOURCE = "Source";
        public static readonly string AUDITLOG_DESCRIPTION = "Description";
        public static readonly string TYPE = "Type";
        public static readonly string MESSAGE_LOGIN_SUCCESSFULL = "Successfully authenticate - SessionId - {0}, UserName - {1}";
        public static readonly string MESSAGE_LOGIN_UNSUCCESSFULL = "Couldn't authenticate - UserId - {0}, Password - {1}";
        public static readonly string MESSAGE_FORM_ADD_SUCCESSFULL = "Form Entry {1} Successful , FormEntryId -{0}";
        public static readonly string MESSAGE_FORM_ADD_UNSUCCESSFULL = "Form Entry {0} Unsuccessful";
        public static readonly string MESSAGE_FORM_DELETE_SUCCESSFULL = "Form Entry Delete Successful , FormEntryId - {0}";
        public static readonly string MESSAGE_FORM_DELETE_UNSUCCESSFULL = "Form Entry Delete Unsuccessful, FormEntryId - {0}";
        public static readonly string WHERECONDITIONFORMAT = "{0} like '%{1}%'";


        #endregion AuditLog

        #region Path

        public static readonly string CONFIGURATION_INTERVIEWERLIST_FILEPATH = "InterviwerListFilePath";
        public static readonly string CONFIGURATION_AIRPORTLIST_FILEPATH = "AirportListFilePath";
        public static readonly string CONFIGURATION_AIRPORT_AIRLINELIST_FILEPATH = "AirportAirlineListFilePath";
        public static readonly string CONFIGURATION_TARGETVSCOMPLETESCHART_FILEPATH = "TargetsVsCompletesChartFilePath";
        public static readonly string CONFIGURATION_TARGETVSCOMPLETESCHART_EUR_FILEPATH = "TargetsVsCompletesChartFilePath_EUR";
        public static readonly string CONFIGURATION_TARGETVSCOMPLETESCHART_AIR_FILEPATH = "TargetsVsCompletesChartFilePath_AIR";
        public static readonly string CONFIGURATION_CACHE_FILEPATH = "CacheFilePath";
        public static readonly string CONFIGURATION_AIRPORT_REPORT_CACHE_FILEPATH = "AirportReportCacheFilePath";
        public static readonly string CONFIGURATION_CUSTOM_DOWNLOAD_PATH = "CustomDownloads";
        public static readonly string CONFIGURATION_CUSTOM_DOWNLOAD_URL = "CustomDownloadsUrl";
        public static readonly string CONFIGURATION_UPLOAD_PATH = "Upload";

        #endregion Path

        #region DBColumnNames

        public static readonly string TABLE_INDCARDENTRY_ID = "ind_card_entry_id";
        public static readonly string TABLE_INDCARDENTRY_DISTRIBUTION_DATE = "distribution_date";
        public static readonly string TABLE_INDCARDENTRY_ORIGINID = "origin_id";
        public static readonly string TABLE_INDCARDENTRY_DESTINATIONID = "destination_id";
        public static readonly string TABLE_INDCARDENTRY_TYPE = "type";
        public static readonly string TABLE_INDCARDENTRY_AIRLINEID = "airline_id";
        public static readonly string TABLE_INDCARDENTRY_ROUTE = "route";
        public static readonly string TABLE_INDCARDENTRY_DIRECTION = "direction";
        public static readonly string TABLE_INDCARDENTRY_INTERVIEWERID = "interviewer_id";
        public static readonly string TABLE_INDCARDENTRY_FLIGHTTYPE = "type_of_flight";
        public static readonly string TABLE_INDCARDENTRY_AIRCRAFT_TYPE = "aircraft_type";

        public static readonly string TABLE_CONFIRMIT_CARDNUMBER = "I.card_number";
        public static readonly string TABLE_CONFIRMIT_CLASS = "I.class";
        public static readonly string TABLE_CONFIRMIT_STATUS = "I.status";
        public static readonly string TABLE_CONFIRMIT_DATE = "I.confirmit_upload_date";
        public static readonly string TABLE_CONFIRMIT_AIRPORTNAME = "A.username";

        public static readonly string TABLE_FLIGHT_FLIGHTCOMBINATIONID = "F.flight_id";
        public static readonly string TABLE_FLIGHT_AIRLINEID = "F.airline_id";
        public static readonly string TABLE_FLIGHT_AIRLINENAME = "AL.airline_name";
        public static readonly string TABLE_FLIGHT_ORIGINID = "F.origin_id";
        public static readonly string TABLE_FLIGHT_ORIGIN = "O.code";
        public static readonly string TABLE_FLIGHT_DESTINATIONID = "F.destination_id";
        public static readonly string TABLE_FLIGHT_DESTINATION = "D.code";
        public static readonly string TABLE_FLIGHT_TYPE = "F.type";
        public static readonly string TABLE_FLIGHT_ROUTE = "F.route";
        public static readonly string TABLE_FLIGHT_DIRECTION = "F.direction";
        public static readonly string TABLE_FLIGHT_FLIGHTTYPE = "F.type";
        public static readonly string TABLE_TARGET = "target";


        #endregion DBColumnNames

        #region Mail

        public static readonly string CONFIGURATION_SMTP_FROMADDRESS = "FromAddress";
        public static readonly string CONFIGURATION_SMTP_SMTPPASSWORD = "SmtpPassword";
        public static readonly string CONFIGURATION_SMTP_SMTPUSER = "SmtpUser";
        public static readonly string CONFIGURATION_SMTP_SMTPPORT = "SmtpPort";
        public static readonly string CONFIGURATION_SMTP_SMTPSERVER = "SmtpServer";
        public static readonly string CONFIGURATION_SMTP_REGARDS = "RegardsAddress";
        public static readonly string CONFIGURATION_SMTP_APPNAME = "AppName";
        public static readonly string CONFIGURATION_SMTP_LOGOURL = "LogoImageURL";
        public static readonly string CONFIGURATION_SMTP_DOMAINNAME = "DomainName";
        public static readonly string CONFIGURATION_SMTP_ADMINMAILID = "adminMailid";

        public static readonly string CONFIGURATION_SMTP_REDIRECTALLMAILS = "RedirectMails";
        public static readonly string CONFIGURATION_SMTP_REDIRECTADDRESS = "RedirectAddress";

        public static readonly string MAIL_FORGOT_PASSWORD_BODY = "<table width='100%' cellspacing='0' cellpadding='8' border='0'><tr><td align=\"center\"><img src='@logoPath' /></td></tr></table><style type=\"text/css\">.blacktext {	font-family: Verdana, Arial, Helvetica, sans-serif; font-size: 10pt;color: #000000;	text-decoration: none;} </style><p class=\"blacktext\">Dear Admin</p><p class=\"blacktext\">There was a request placed in @appName for resetting the password for following user.<br><br><b>Instance Name</b> : @InstanceName <br><b>User Name</b> : @UserId <br><b>User Email Id</b> : @emailId<br><br><span class=\"blacktext\">Regards</span></p><span class=\"blacktext\">@regardsAddress</span><br><br><b>Note</b>: This is an auto generated email triggered by your request on the @appName application.";
        public static readonly string CONFIGURATION_MAIL_FORGOTPASSWORDFORMATPATH = "ForgotPasswordFormatPath";
        #endregion Mail

    }
}
