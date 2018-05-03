using SICT.BusinessUtils;
using SICT.Constants;
using SICT.DataAccessLayer;
using SICT.DataContracts;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace SICT.BusinessLayer.V1
{
    public class UploadBusiness
    {
        private static readonly string CLASS_NAME = "UploadBusiness";

        public ReturnValue UploadFile(System.IO.Stream FileStream, string Instance)
        {
            ReturnValue ReturnValue = new ReturnValue();
            BusinessUtil ObjBusinessUtil = new BusinessUtil();
            SICTLogger.WriteInfo(UploadBusiness.CLASS_NAME, "UploadFile", "Start ");
            try
            {
                MultiPartParser Parser = this.ParseFile(FileStream);
                string UploadFilePath = string.Empty;
                ObjBusinessUtil.GetUploadFilePath(ref UploadFilePath, System.IO.Path.GetFileNameWithoutExtension(Parser.Filename), System.IO.Path.GetExtension(Parser.Filename));
                using (System.IO.FileStream FileToupload = new System.IO.FileStream(UploadFilePath, System.IO.FileMode.Create))
                {
                    FileToupload.Write(Parser.FileContents, 0, Parser.FileContents.Length);
                }
                bool IsFileValid = this.ValidateFile(UploadFilePath);
                if (IsFileValid)
                {
                    Task.Factory.StartNew(delegate
                    {
                        this.BulkInsert(UploadFilePath, Instance);
                    });
                    ReturnValue.ReturnCode = 1;
                    ReturnValue.ReturnMessage = "Upload Successful";
                }
                else
                {
                    ReturnValue.ReturnCode = 5;
                    ReturnValue.ReturnMessage = "Invalid File ";
                }
                FileStream.Close();
            }
            catch (System.Exception Ex)
            {
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in Function ";
                SICTLogger.WriteException(UploadBusiness.CLASS_NAME, "UploadFile", Ex);
            }
            SICTLogger.WriteInfo(UploadBusiness.CLASS_NAME, "UploadFile", "UploadFileEnd");
            return ReturnValue;
        }

        private void BulkInsert(string UploadFilePath, string Instance)
        {
            try
            {
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                int MaxCnt = System.Convert.ToInt32(ConfigurationManager.AppSettings[BusinessConstants.UPLOAD_ROWS_CNT].ToString());
                DataTable Dt = new DataTable();
                Dt.Columns.Add(BusinessConstants.CONFIRMIT_CARD_NUMBER, typeof(long));
                Dt.Columns.Add(BusinessConstants.CONFIRMIT_CLASS, typeof(string));
                Dt.Columns.Add(BusinessConstants.CONFIRMIT_STATUS, typeof(string));
                Dt.Columns.Add(BusinessConstants.CONFIRMIT_DATETIME, typeof(System.DateTime));
                using (System.IO.StreamReader ReadFile = new System.IO.StreamReader(UploadFilePath))
                {
                    int CurIndex = 0;
                    string line;
                    while ((line = ReadFile.ReadLine()) != null)
                    {
                        string[] RowValues = line.Split(new char[]
                        {
                            ','
                        });
                        if (!string.IsNullOrEmpty(RowValues[0]) && !string.IsNullOrEmpty(RowValues[1]) && !string.IsNullOrEmpty(RowValues[2]) && !string.IsNullOrEmpty(RowValues[3]))
                        {
                            DataRow Dr = Dt.NewRow();
                            Dr[BusinessConstants.CONFIRMIT_CARD_NUMBER] = System.Convert.ToInt64(RowValues[0]);
                            Dr[BusinessConstants.CONFIRMIT_CLASS] = RowValues[1].ToString();
                            Dr[BusinessConstants.CONFIRMIT_STATUS] = RowValues[2].ToString();
                            Dt.Rows.Add(Dr);
                        }
                        CurIndex++;
                        if (CurIndex == MaxCnt || ReadFile.EndOfStream)
                        {
                            bool IsLastbatch = false;
                            if (ReadFile.EndOfStream)
                            {
                                IsLastbatch = true;
                            }
                            SICTLogger.WriteWarning(UploadBusiness.CLASS_NAME, "BulkInsert", "Bulk Copy Initiated for current index" + CurIndex);
                            DBLayer.BulkCopy(Dt, IsLastbatch);
                            if (IsLastbatch)
                            {
                                DBLayer.OnSqlRowsCopied(null, null);
                            }
                            CurIndex = 0;
                            Dt.Clear();
                        }
                    }
                }
                Task.Factory.StartNew<ReturnValue>(() => new CacheFileBusiness().CreateCacheFileforTargetVsCompletesCharts(Instance));
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(UploadBusiness.CLASS_NAME, "BulkInsert", Ex);
            }
        }

        private MultiPartParser ParseFile(System.IO.Stream Stream)
        {
            SICTLogger.WriteInfo(UploadBusiness.CLASS_NAME, "ParseFile ", "Start");
            MultiPartParser Parser = null;
            try
            {
                Parser = new MultiPartParser(Stream);
                if (!Parser.Success)
                {
                    SICTLogger.WriteError(UploadBusiness.CLASS_NAME, "ParseFile ", "Unable to parse the file");
                    Parser = null;
                }
                else
                {
                    SICTLogger.WriteVerbose(UploadBusiness.CLASS_NAME, "ParseFile ", "Successfully parsed file - Filename is - " + Parser.Filename);
                }
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(UploadBusiness.CLASS_NAME, "ParseFile ", Ex);
            }
            SICTLogger.WriteInfo(UploadBusiness.CLASS_NAME, "ParseFile ", "End");
            return Parser;
        }

        private bool ValidateFile(string FilePath)
        {
            SICTLogger.WriteInfo(UploadBusiness.CLASS_NAME, "ValidateFile", "Start");
            bool IsValid = false;
            try
            {
                string Type = System.IO.Path.GetExtension(FilePath);
                if (Type.Equals(BusinessConstants.EXTENSION_CSV, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    System.IO.StreamReader File = new System.IO.StreamReader(FilePath);
                    string Line = File.ReadLine();
                    if (!string.IsNullOrEmpty(Line))
                    {
                        string[] Parameter = Line.Split(new char[]
                        {
                            ','
                        });
                        int Length = Parameter.Length;
                        if (Parameter.Length >= 4)
                        {
                            IsValid = true;
                            SICTLogger.WriteVerbose(UploadBusiness.CLASS_NAME, "ValidateFile", "Valid CSV File");
                        }
                    }
                }
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(UploadBusiness.CLASS_NAME, "ValidateFile", Ex);
            }
            SICTLogger.WriteInfo(UploadBusiness.CLASS_NAME, "ValidateFile", "End");
            return IsValid;
        }
    }
}
