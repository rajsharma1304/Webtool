using SICT.BusinessLayer.V1;
using System;

namespace SICT.CommonBusiness
{
    public class FactoryBusiness
    {
        private const string Version_V1 = "V1";

        public UserDetailsBusiness GetUserDetailsBusiness(string Version)
        {
            UserDetailsBusiness result;
            if (string.IsNullOrEmpty(Version) || Version.Equals("V1", StringComparison.InvariantCultureIgnoreCase))
            {
                result = new UserDetailsBusiness();
            }
            else
            {
                result = new UserDetailsBusiness();
            }
            return result;
        }

        public DepartureFormBusiness GetDepartureFormBusiness(string Version)
        {
            DepartureFormBusiness result;
            if (string.IsNullOrEmpty(Version) || Version.Equals("V1", StringComparison.InvariantCultureIgnoreCase))
            {
                result = new DepartureFormBusiness();
            }
            else
            {
                result = new DepartureFormBusiness();
            }
            return result;
        }

        public ReportingBusiness GetReportingBusiness(string Version)
        {
            ReportingBusiness result;
            if (string.IsNullOrEmpty(Version) || Version.Equals("V1", StringComparison.InvariantCultureIgnoreCase))
            {
                result = new ReportingBusiness();
            }
            else
            {
                result = new ReportingBusiness();
            }
            return result;
        }

        public ManagementBusiness GetManagementBusiness(string Version)
        {
            ManagementBusiness result;
            if (string.IsNullOrEmpty(Version) || Version.Equals("V1", StringComparison.InvariantCultureIgnoreCase))
            {
                result = new ManagementBusiness();
            }
            else
            {
                result = new ManagementBusiness();
            }
            return result;
        }

        public DownloadBusiness GetDownloadBusiness(string Version)
        {
            DownloadBusiness result;
            //if (string.IsNullOrEmpty(Version) || Version.Equals("V1", StringComparison.InvariantCultureIgnoreCase))
            //{
            //    result = new DownloadBusiness();
            //}
            //else
            //{
                result = new DownloadBusiness();
            //}
            return result;
        }

        public UploadBusiness GetUploadBusiness(string Version)
        {
            UploadBusiness result;
            if (string.IsNullOrEmpty(Version) || Version.Equals("V1", StringComparison.InvariantCultureIgnoreCase))
            {
                result = new UploadBusiness();
            }
            else
            {
                result = new UploadBusiness();
            }
            return result;
        }
    }
}
