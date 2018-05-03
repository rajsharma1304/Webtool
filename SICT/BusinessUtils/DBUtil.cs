using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using SICT.Constants;
using System;

namespace SICT.DBUtils
{
    public class DBUtil
    {
        public SqlDatabase GetDataBase()
        {
            return new SqlDatabase(BusinessConstants.DBConnection);
        }
    }
}
