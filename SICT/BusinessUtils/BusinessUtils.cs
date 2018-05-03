using SICT.Constants;
using System;
using System.Configuration;
using System.IO;

namespace SICT.BusinessUtils
{
    public class BusinessUtil
    {
        public void GetInterviewerFilePath(string LoginId, ref string FilePath, ref string FolderPath)
        {
            FilePath = string.Format("{0}/InterviewerList_{1}.json", ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_INTERVIEWERLIST_FILEPATH].ToString(), LoginId);
            FolderPath = string.Format("{0}", ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_INTERVIEWERLIST_FILEPATH].ToString());
        }

        public void GetAirportFilePath(string LoginId, ref string FilePath, ref string FolderPath)
        {
            FilePath = string.Format("{0}/AirportList_{1}.json", ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_AIRPORTLIST_FILEPATH].ToString(), LoginId);
            FolderPath = string.Format("{0}", ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_AIRPORTLIST_FILEPATH].ToString());
        }

        public void AirportIdVstLoginId(ref string FilePath, ref string FolderPath)
        {
            FilePath = string.Format("{0}/AirportIdVstLoginId.json", ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_CACHE_FILEPATH].ToString());
            FolderPath = string.Format("{0}", ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_CACHE_FILEPATH].ToString());
        }

        public void GetAirportAirlineFilePath(string AirportId, ref string FilePath, ref string FolderPath, bool IsDepartureForm)
        {
            if (IsDepartureForm)
            {
                FilePath = string.Format("{0}/{2}_AirportAirlineList_{1}.json", ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_AIRPORT_AIRLINELIST_FILEPATH].ToString(), AirportId, "Departure");
            }
            else
            {
                FilePath = string.Format("{0}/{2}_AirportAirlineList_{1}.json", ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_AIRPORT_AIRLINELIST_FILEPATH].ToString(), AirportId, "Arrival");
            }
            FolderPath = string.Format("{0}", ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_AIRPORT_AIRLINELIST_FILEPATH].ToString());
        }

        public void GetTargetVsCompletesChartsFilePath(string LoginId, ref string FilePath, ref string FolderPath)
        {
            FilePath = string.Format("{0}/TargetsVsCompletes_{1}.json", ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_TARGETVSCOMPLETESCHART_FILEPATH].ToString(), LoginId);
            FolderPath = string.Format("{0}", ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_TARGETVSCOMPLETESCHART_FILEPATH].ToString());
        }

        public void GetMissingTargetsVsBusinessClassChartsFilePath(string LoginId, ref string FilePath, ref string FolderPath)
        {
            FilePath = string.Format("{0}/MissingTargetsVsBusinessClass_{1}.json", ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_TARGETVSCOMPLETESCHART_FILEPATH].ToString(), LoginId);
            FolderPath = string.Format("{0}", ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_TARGETVSCOMPLETESCHART_FILEPATH].ToString());
        }

        public void GetLanguageCachetFilePath(ref string FilePath, ref string FolderPath)
        {
            FilePath = string.Format("{0}/LanguageList.json", ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_CACHE_FILEPATH].ToString());
            FolderPath = string.Format("{0}", ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_CACHE_FILEPATH].ToString());
        }

        public void GetRouteAndDirectionCachetFilePath(ref string RouteFilePath, ref string DirectionFilePath, ref string FolderPath)
        {
            RouteFilePath = string.Format("{0}/RouteList.json", ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_CACHE_FILEPATH].ToString());
            DirectionFilePath = string.Format("{0}/DirectionList.json", ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_CACHE_FILEPATH].ToString());
            FolderPath = string.Format("{0}", ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_CACHE_FILEPATH].ToString());
        }

        public void GetAirportReportFilePath(ref string FilePath, ref string FolderPath, string FileName, bool IsOrigin)
        {
            if (IsOrigin)
            {
                FilePath = string.Format("{0}/OriginAndDestinationList_{1}.json", ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_AIRPORT_REPORT_CACHE_FILEPATH].ToString(), FileName);
                FolderPath = string.Format("{0}", ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_AIRPORT_REPORT_CACHE_FILEPATH].ToString());
                return;
            }
            FilePath = string.Format("{0}/AirlineList_{1}.json", ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_AIRPORT_REPORT_CACHE_FILEPATH].ToString(), FileName);
            FolderPath = string.Format("{0}", ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_AIRPORT_REPORT_CACHE_FILEPATH].ToString());
        }

        public void GetCachetFilePath(string FileName, ref string FilePath, ref string FolderPath)
        {
            FilePath = string.Format("{0}/{1}.json", ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_CACHE_FILEPATH].ToString(), FileName);
            FolderPath = string.Format("{0}", ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_CACHE_FILEPATH].ToString());
        }

        public void GetFormsExcelExportFilePath(string Instance, ref string FilePath, ref string FileLink, ref string FileName)
        {
            string path = ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_CUSTOM_DOWNLOAD_PATH].ToString();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string arg = string.Empty;
            if (Instance == "US")
            {
                arg = "Airsat_International_Data";
            }
            else if (Instance == "USD")
            {
                arg = "Airsat_Europe_Data";
            }
            else if (Instance == "EUR")
            {
                arg = "Airsat_USDomestic_Data";
            }
            else if (Instance == "AIR")
            {
                arg = "Airsat_Aircraft_Data";
            }
            FileName = string.Format("{0}_{1}", arg, DateTime.Now.ToString("MM-dd-yyyy_HH-mm-ss"));
            FilePath = string.Format("{0}/{1}.xlsx", ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_CUSTOM_DOWNLOAD_PATH].ToString(), FileName);
            FileLink = string.Format("{0}/{1}.xlsx", ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_CUSTOM_DOWNLOAD_URL].ToString(), FileName);
        }

        public string GetDownloadFilePathByName(string FileName)
        {
            return string.Format("{0}/{1}.xlsx", ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_CUSTOM_DOWNLOAD_PATH].ToString(), FileName);
        }

        public void GetUploadFilePath(ref string FilePath, string FileName, string Extension)
        {
            string path = ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_UPLOAD_PATH].ToString();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string arg = string.Format("{0}_{1}", FileName, DateTime.Now.ToString("MM-dd-yyyy HH_mm_ss"));
            FilePath = string.Format("{0}/{1}{2}", ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_UPLOAD_PATH].ToString(), arg, Extension);
        }
    }
}
