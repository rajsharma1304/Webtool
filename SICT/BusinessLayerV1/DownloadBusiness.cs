using Aspose.Cells;
using SICT.BusinessUtils;
using SICT.Constants;
using SICT.DataAccessLayer;
using SICT.DataContracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace SICT.BusinessLayer.V1
{
    public class DownloadBusiness
    {
        private static readonly string CLASS_NAME = "DownloadBusiness";

        public DownloadBusiness()
        {
            try
            {
                bool IsLicenseEnabled = System.Convert.ToBoolean(ConfigurationManager.AppSettings[BusinessConstants.CONFIGURATION_LICENSING_STATUS]);
                if (IsLicenseEnabled)
                {
                    License CellsLicense = new License();
                    CellsLicense.SetLicense("Aspose.Cells.lic");
                }
            }
            catch (System.Exception)
            {
            }
        }

        public DownloadResponse FormExcelExport(string Instance, string SessionId, DepartureFormFilterDetails DepartureFormFilterDetails)
        {
            DownloadResponse DownloadResponse = new DownloadResponse();
            BusinessUtil ObjBusinessUtil = new BusinessUtil();
            SICTLogger.WriteInfo(DownloadBusiness.CLASS_NAME, "FormExcelExport", "Start ");
            try
            {
                string FilePath = string.Empty;
                string FileLink = string.Empty;
                string FileName = string.Empty;
                ObjBusinessUtil.GetFormsExcelExportFilePath(Instance, ref FilePath, ref FileLink, ref FileName);
                DownloadResponse.ReturnCode = 1;
                DownloadResponse.ReturnMessage = "Downloaded Started";
                DownloadResponse.FileLink = FileLink;
                DownloadResponse.FileName = FileName;
                Task.Factory.StartNew(delegate
                {
                    this.DownloadDataInBackground(SessionId, DepartureFormFilterDetails, FilePath);
                });
            }
            catch (System.Exception Ex)
            {
                DownloadResponse.ReturnCode = -1;
                DownloadResponse.ReturnMessage = "Error in Function ";
                SICTLogger.WriteException(DownloadBusiness.CLASS_NAME, "FormExcelExport", Ex);
            }
            SICTLogger.WriteInfo(DownloadBusiness.CLASS_NAME, "FormExcelExport", "FormExcelExportEnd");
            return DownloadResponse;
        }

        private void DownloadDataInBackground(string SessionId, DepartureFormFilterDetails DepartureFormFilterDetails, string FilePath)
        {
            DownloadResponse DownloadResponse = new DownloadResponse();
            SICTLogger.WriteInfo(DownloadBusiness.CLASS_NAME, "DownloadDataInBackground", "Start ");
            try
            {
                DepartureFormBusiness ObjDepartureFormBusiness = new DepartureFormBusiness();
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                System.Collections.Generic.List<DepartureFormDetails> DepartureFormDetails = new System.Collections.Generic.List<DepartureFormDetails>();
                string OrderByCondition = string.Empty;
                string WhereCondition = string.Empty;
                Workbook Wb = new Workbook();
                Worksheet DataSheet = null;
                this.AddDataSheetToWorkBook(ref Wb, ref DataSheet, "Forms");
                this.InsertFormFilterstoWorkBook(ref DataSheet, DepartureFormFilterDetails);
                ObjDepartureFormBusiness.BuildOrderByandWhereConditions(DepartureFormFilterDetails, ref OrderByCondition, ref WhereCondition);
                int RecordsPerLoop = System.Convert.ToInt32(ConfigurationManager.AppSettings[BusinessConstants.UPLOAD_ROWS_CNT].ToString());
                int StartIndex = 0;
                int Count = 0;
                bool IsFirstRecord = true;
                int DataRow = 1;
                do
                {
                    DataSet DSCardDetails = new DataSet();
                    DSCardDetails = DBLayer.GetGlobalDepartureFormDetailst(SessionId, StartIndex, RecordsPerLoop, OrderByCondition, WhereCondition, DepartureFormFilterDetails.AirportId, DepartureFormFilterDetails.IsDepartureForm, false, 0L, 0L);
                    if (DSCardDetails.Tables.Count == 2)
                    {
                        if (IsFirstRecord)
                        {
                            Count = System.Convert.ToInt32(DSCardDetails.Tables[0].Rows[0][BusinessConstants.FORM_TOTALRECORDS].ToString());
                            IsFirstRecord = false;
                        }
                        this.InsertFormDataToWorksheet(DSCardDetails.Tables[1], ref DataSheet, ref DataRow);
                        StartIndex += RecordsPerLoop;
                        if (StartIndex < Count && StartIndex + RecordsPerLoop > Count)
                        {
                            RecordsPerLoop = Count - StartIndex;
                        }
                    }
                }
                while (StartIndex < Count);
                Wb.Save(FilePath, new OoxmlSaveOptions(SaveFormat.Xlsx));
            }
            catch (System.Exception Ex)
            {
                DownloadResponse.ReturnCode = -1;
                DownloadResponse.ReturnMessage = "Error in Function ";
                SICTLogger.WriteException(DownloadBusiness.CLASS_NAME, "DownloadDataInBackground", Ex);
            }
        }

        private void InsertFormDataToWorksheet(DataTable DtForm, ref Worksheet DataSheet, ref int DataRow)
        {
            FinalDepartureFormDetails FinalDepartureFormDetails = new FinalDepartureFormDetails();
            SICTLogger.WriteInfo(DownloadBusiness.CLASS_NAME, "InsertFormDataToWorksheet", "Start ");
            try
            {
                foreach (DataRow Dr in DtForm.Rows)
                {
                    this.SetExcelCellStyle(ref DataSheet, DataRow, 0, 12, System.Drawing.Color.White, false, false, 0, false);
                    DataSheet.Cells[DataRow, 0].PutValue(System.Convert.ToInt32(Dr[BusinessConstants.FORM_FORMID].ToString()));
                    this.SetExcelCellStyle(ref DataSheet, DataRow, 1, 12, System.Drawing.Color.White, false, false, 0, false);
                    string AirportName = string.Empty;
                    string AirportCode = string.Empty;
                    if (!string.IsNullOrEmpty(Dr[BusinessConstants.FORM_AIRPORTNAME].ToString()))
                    {
                        AirportName = Dr[BusinessConstants.FORM_AIRPORTNAME].ToString();
                    }
                    if (!string.IsNullOrEmpty(Dr[BusinessConstants.AIRPORTCODE].ToString()))
                    {
                        AirportCode = Dr[BusinessConstants.AIRPORTCODE].ToString();
                    }
                    DataSheet.Cells[DataRow, 1].PutValue(string.Format("{0} ({1})", AirportName, AirportCode));
                    this.SetExcelCellStyle(ref DataSheet, DataRow, 2, 12, System.Drawing.Color.White, false, false, 14, false);
                    if (!string.IsNullOrEmpty(Dr[BusinessConstants.FORM_DISTRIBUTIONDATE].ToString()))
                    {
                        System.DateTime Date = System.Convert.ToDateTime(Dr[BusinessConstants.FORM_DISTRIBUTIONDATE].ToString()).Date;
                        DataSheet.Cells[DataRow, 2].PutValue(Date);
                    }
                    this.SetExcelCellStyle(ref DataSheet, DataRow, 3, 12, System.Drawing.Color.White, false, false, 0, false);
                    if (!string.IsNullOrEmpty(Dr[BusinessConstants.FORM_INTERVIEWERNAME].ToString()))
                    {
                        DataSheet.Cells[DataRow, 3].PutValue(Dr[BusinessConstants.FORM_INTERVIEWERNAME].ToString());
                    }
                    this.SetExcelCellStyle(ref DataSheet, DataRow, 4, 12, System.Drawing.Color.White, false, false, 0, false);
                    if (!string.IsNullOrEmpty(Dr[BusinessConstants.FORM_AIRLINENAME].ToString()))
                    {
                        DataSheet.Cells[DataRow, 4].PutValue(Dr[BusinessConstants.FORM_AIRLINENAME].ToString());
                    }
                    this.SetExcelCellStyle(ref DataSheet, DataRow, 5, 12, System.Drawing.Color.White, false, false, 0, false);
                    if (!string.IsNullOrEmpty(Dr[BusinessConstants.FORM_TYPE].ToString()))
                    {
                        string Type = Dr[BusinessConstants.FORM_TYPE].ToString();
                        if (Type == BusinessConstants.FORM_TYPE_DEPARTURE)
                        {
                            DataSheet.Cells[DataRow, 5].PutValue("Departure");
                        }
                        else
                        {
                            DataSheet.Cells[DataRow, 5].PutValue("Arrival");
                        }
                    }
                    this.SetExcelCellStyle(ref DataSheet, DataRow, 6, 12, System.Drawing.Color.White, false, false, 0, false);
                    if (!string.IsNullOrEmpty(Dr[BusinessConstants.FORM_DESTINATIONNAME].ToString()))
                    {
                        DataSheet.Cells[DataRow, 6].PutValue(Dr[BusinessConstants.FORM_DESTINATIONNAME].ToString());
                    }
                    this.SetExcelCellStyle(ref DataSheet, DataRow, 7, 12, System.Drawing.Color.White, false, false, 0, false);
                    if (!string.IsNullOrEmpty(Dr[BusinessConstants.FORM_FLIGHTNUMBER].ToString()))
                    {
                        DataSheet.Cells[DataRow, 7].PutValue(Dr[BusinessConstants.FORM_FLIGHTNUMBER].ToString());
                    }
                    this.SetExcelCellStyle(ref DataSheet, DataRow, 8, 12, System.Drawing.Color.White, false, false, 0, false);
                    if (!string.IsNullOrEmpty(Dr["Language1"].ToString()))
                    {
                        DataSheet.Cells[DataRow, 8].PutValue(Dr["Language1"].ToString());
                    }
                    this.SetExcelCellStyle(ref DataSheet, DataRow, 9, 12, System.Drawing.Color.White, false, false, 0, false);
                    if (!string.IsNullOrEmpty(Dr["StartCode1"].ToString()))
                    {
                        DataSheet.Cells[DataRow, 9].PutValue((double)System.Convert.ToInt64(Dr["StartCode1"].ToString()));
                    }
                    this.SetExcelCellStyle(ref DataSheet, DataRow, 10, 12, System.Drawing.Color.White, false, false, 0, false);
                    if (!string.IsNullOrEmpty(Dr["EndCode1"].ToString()))
                    {
                        DataSheet.Cells[DataRow, 10].PutValue((double)System.Convert.ToInt64(Dr["EndCode1"].ToString()));
                    }
                    this.SetExcelCellStyle(ref DataSheet, DataRow, 11, 12, System.Drawing.Color.White, false, false, 0, false);
                    if (!string.IsNullOrEmpty(Dr["Language2"].ToString()))
                    {
                        DataSheet.Cells[DataRow, 11].PutValue(Dr["Language2"].ToString());
                    }
                    this.SetExcelCellStyle(ref DataSheet, DataRow, 12, 12, System.Drawing.Color.White, false, false, 0, false);
                    if (!string.IsNullOrEmpty(Dr["StartCode2"].ToString()))
                    {
                        DataSheet.Cells[DataRow, 12].PutValue((double)System.Convert.ToInt64(Dr["StartCode2"].ToString()));
                    }
                    this.SetExcelCellStyle(ref DataSheet, DataRow, 13, 12, System.Drawing.Color.White, false, false, 0, false);
                    if (!string.IsNullOrEmpty(Dr["EndCode2"].ToString()))
                    {
                        DataSheet.Cells[DataRow, 13].PutValue((double)System.Convert.ToInt64(Dr["EndCode2"].ToString()));
                    }
                    this.SetExcelCellStyle(ref DataSheet, DataRow, 14, 12, System.Drawing.Color.White, false, false, 0, false);
                    if (!string.IsNullOrEmpty(Dr["Language3"].ToString()))
                    {
                        DataSheet.Cells[DataRow, 14].PutValue(Dr["Language3"].ToString());
                    }
                    this.SetExcelCellStyle(ref DataSheet, DataRow, 15, 12, System.Drawing.Color.White, false, false, 0, false);
                    if (!string.IsNullOrEmpty(Dr["StartCode3"].ToString()))
                    {
                        DataSheet.Cells[DataRow, 15].PutValue((double)System.Convert.ToInt64(Dr["StartCode3"].ToString()));
                    }
                    this.SetExcelCellStyle(ref DataSheet, DataRow, 16, 12, System.Drawing.Color.White, false, false, 0, false);
                    if (!string.IsNullOrEmpty(Dr["EndCode3"].ToString()))
                    {
                        DataSheet.Cells[DataRow, 16].PutValue((double)System.Convert.ToInt64(Dr["EndCode3"].ToString()));
                    }
                    this.SetExcelCellStyle(ref DataSheet, DataRow, 17, 12, System.Drawing.Color.White, false, false, 0, false);
                    if (!string.IsNullOrEmpty(Dr["Language4"].ToString()))
                    {
                        DataSheet.Cells[DataRow, 17].PutValue(Dr["Language4"].ToString());
                    }
                    this.SetExcelCellStyle(ref DataSheet, DataRow, 18, 12, System.Drawing.Color.White, false, false, 0, false);
                    if (!string.IsNullOrEmpty(Dr["StartCode4"].ToString()))
                    {
                        DataSheet.Cells[DataRow, 18].PutValue((double)System.Convert.ToInt64(Dr["StartCode4"].ToString()));
                    }
                    this.SetExcelCellStyle(ref DataSheet, DataRow, 19, 12, System.Drawing.Color.White, false, false, 0, false);
                    if (!string.IsNullOrEmpty(Dr["EndCode4"].ToString()))
                    {
                        DataSheet.Cells[DataRow, 19].PutValue((double)System.Convert.ToInt64(Dr["EndCode4"].ToString()));
                    }
                    this.SetExcelCellStyle(ref DataSheet, DataRow, 20, 12, System.Drawing.Color.White, false, false, 0, false);
                    if (!string.IsNullOrEmpty(Dr["Language5"].ToString()))
                    {
                        DataSheet.Cells[DataRow, 20].PutValue(Dr["Language5"].ToString());
                    }
                    this.SetExcelCellStyle(ref DataSheet, DataRow, 21, 12, System.Drawing.Color.White, false, false, 0, false);
                    if (!string.IsNullOrEmpty(Dr["StartCode5"].ToString()))
                    {
                        DataSheet.Cells[DataRow, 21].PutValue((double)System.Convert.ToInt64(Dr["StartCode5"].ToString()));
                    }
                    this.SetExcelCellStyle(ref DataSheet, DataRow, 22, 12, System.Drawing.Color.White, false, false, 0, false);
                    if (!string.IsNullOrEmpty(Dr["EndCode5"].ToString()))
                    {
                        DataSheet.Cells[DataRow, 22].PutValue((double)System.Convert.ToInt64(Dr["EndCode5"].ToString()));
                    }
                    this.SetExcelCellStyle(ref DataSheet, DataRow, 23, 12, System.Drawing.Color.White, false, false, 0, false);
                    if (!string.IsNullOrEmpty(Dr["BusinessCards"].ToString()))
                    {
                        DataSheet.Cells[DataRow, 23].PutValue((double)System.Convert.ToInt64(Dr["BusinessCards"].ToString()));
                    }
                    string DateFormat = ConfigurationManager.AppSettings[BusinessConstants.CONFIG_FORM_DATE_FORMAT].ToString();
                    System.IFormatProvider enUsDateFormat = new System.Globalization.CultureInfo(DateFormat).DateTimeFormat;
                    this.SetExcelCellStyle(ref DataSheet, DataRow, 24, 12, System.Drawing.Color.White, false, false, 22, false);
                    if (!string.IsNullOrEmpty(Dr[BusinessConstants.FORM_LASTUPDATEDTIME].ToString()))
                    {
                        System.DateTime Date = System.Convert.ToDateTime(Dr[BusinessConstants.FORM_LASTUPDATEDTIME].ToString());
                        DataSheet.Cells[DataRow, 24].PutValue(Date);
                    }
                    DataRow++;
                }
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(DownloadBusiness.CLASS_NAME, "InsertFormDataToWorksheet", Ex);
            }
            SICTLogger.WriteInfo(DownloadBusiness.CLASS_NAME, "InsertFormDataToWorksheet", "End");
        }

        private void AddDataSheetToWorkBook(ref Workbook Wb, ref Worksheet DataSheet, string SheetName)
        {
            try
            {
                int SheetIndex = Wb.Worksheets.Count;
                DataSheet = Wb.Worksheets[SheetIndex - 1];
                DataSheet.Name = SheetName;
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(DownloadBusiness.CLASS_NAME, "AddDataSheetToWorkBook", Ex);
            }
        }

        private void InsertFormFilterstoWorkBook(ref Worksheet DataSheet, DepartureFormFilterDetails DepartureFormFilterDetails)
        {
            DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
            try
            {
                int HeaderRow = 0;
                int HeaderCol = 0;
                this.SetExcelCellStyle(ref DataSheet, HeaderRow, HeaderCol, 12, System.Drawing.Color.White, false, false, 0, false);
                DataSheet.Cells[HeaderRow, HeaderCol++].PutValue(BusinessConstants.FORM_DOWNLOAD_ID);
                this.SetExcelCellStyle(ref DataSheet, HeaderRow, HeaderCol, 12, System.Drawing.Color.White, false, false, 0, false);
                DataSheet.Cells[HeaderRow, HeaderCol++].PutValue(BusinessConstants.FORM_DOWNLOAD_AIRPORT_ID);
                this.SetExcelCellStyle(ref DataSheet, HeaderRow, HeaderCol, 12, System.Drawing.Color.White, false, false, 14, false);
                DataSheet.Cells[HeaderRow, HeaderCol++].PutValue(BusinessConstants.FORM_DOWNLOAD_DATE);
                this.SetExcelCellStyle(ref DataSheet, HeaderRow, HeaderCol, 12, System.Drawing.Color.White, false, false, 0, false);
                DataSheet.Cells[HeaderRow, HeaderCol++].PutValue(BusinessConstants.FORM_DOWNLOAD_INTERVIEWER_ID);
                this.SetExcelCellStyle(ref DataSheet, HeaderRow, HeaderCol, 12, System.Drawing.Color.White, false, false, 0, false);
                DataSheet.Cells[HeaderRow, HeaderCol++].PutValue(BusinessConstants.FORM_DOWNLOAD_AIRLINES_ID);
                this.SetExcelCellStyle(ref DataSheet, HeaderRow, HeaderCol, 12, System.Drawing.Color.White, false, false, 0, false);
                DataSheet.Cells[HeaderRow, HeaderCol++].PutValue(BusinessConstants.FORM_DOWNLOAD_DEPTARR);
                this.SetExcelCellStyle(ref DataSheet, HeaderRow, HeaderCol, 12, System.Drawing.Color.White, false, false, 0, false);
                DataSheet.Cells[HeaderRow, HeaderCol++].PutValue(BusinessConstants.FORM_DOWNLOAD_DEPT_ID);
                this.SetExcelCellStyle(ref DataSheet, HeaderRow, HeaderCol, 12, System.Drawing.Color.White, false, false, 0, false);
                DataSheet.Cells[HeaderRow, HeaderCol++].PutValue(BusinessConstants.FORM_DOWNLOAD_FLIGHT_NUMBER);
                this.SetExcelCellStyle(ref DataSheet, HeaderRow, HeaderCol, 12, System.Drawing.Color.White, false, false, 0, false);
                DataSheet.Cells[HeaderRow, HeaderCol++].PutValue(BusinessConstants.FORM_DOWNLOAD_LANG1);
                this.SetExcelCellStyle(ref DataSheet, HeaderRow, HeaderCol, 12, System.Drawing.Color.White, false, false, 0, false);
                DataSheet.Cells[HeaderRow, HeaderCol++].PutValue(BusinessConstants.FORM_DOWNLOAD_STARTCODE1);
                this.SetExcelCellStyle(ref DataSheet, HeaderRow, HeaderCol, 12, System.Drawing.Color.White, false, false, 0, false);
                DataSheet.Cells[HeaderRow, HeaderCol++].PutValue(BusinessConstants.FORM_DOWNLOAD_ENDCODE1);
                this.SetExcelCellStyle(ref DataSheet, HeaderRow, HeaderCol, 12, System.Drawing.Color.White, false, false, 0, false);
                DataSheet.Cells[HeaderRow, HeaderCol++].PutValue(BusinessConstants.FORM_DOWNLOAD_LANG2);
                this.SetExcelCellStyle(ref DataSheet, HeaderRow, HeaderCol, 12, System.Drawing.Color.White, false, false, 0, false);
                DataSheet.Cells[HeaderRow, HeaderCol++].PutValue(BusinessConstants.FORM_DOWNLOAD_STARTCODE2);
                this.SetExcelCellStyle(ref DataSheet, HeaderRow, HeaderCol, 12, System.Drawing.Color.White, false, false, 0, false);
                DataSheet.Cells[HeaderRow, HeaderCol++].PutValue(BusinessConstants.FORM_DOWNLOAD_ENDCODE2);
                this.SetExcelCellStyle(ref DataSheet, HeaderRow, HeaderCol, 12, System.Drawing.Color.White, false, false, 0, false);
                DataSheet.Cells[HeaderRow, HeaderCol++].PutValue(BusinessConstants.FORM_DOWNLOAD_LANG3);
                this.SetExcelCellStyle(ref DataSheet, HeaderRow, HeaderCol, 12, System.Drawing.Color.White, false, false, 0, false);
                DataSheet.Cells[HeaderRow, HeaderCol++].PutValue(BusinessConstants.FORM_DOWNLOAD_STARTCODE3);
                this.SetExcelCellStyle(ref DataSheet, HeaderRow, HeaderCol, 12, System.Drawing.Color.White, false, false, 0, false);
                DataSheet.Cells[HeaderRow, HeaderCol++].PutValue(BusinessConstants.FORM_DOWNLOAD_ENDCODE3);
                this.SetExcelCellStyle(ref DataSheet, HeaderRow, HeaderCol, 12, System.Drawing.Color.White, false, false, 0, false);
                DataSheet.Cells[HeaderRow, HeaderCol++].PutValue(BusinessConstants.FORM_DOWNLOAD_LANG4);
                this.SetExcelCellStyle(ref DataSheet, HeaderRow, HeaderCol, 12, System.Drawing.Color.White, false, false, 0, false);
                DataSheet.Cells[HeaderRow, HeaderCol++].PutValue(BusinessConstants.FORM_DOWNLOAD_STARTCODE4);
                this.SetExcelCellStyle(ref DataSheet, HeaderRow, HeaderCol, 12, System.Drawing.Color.White, false, false, 0, false);
                DataSheet.Cells[HeaderRow, HeaderCol++].PutValue(BusinessConstants.FORM_DOWNLOAD_ENDCODE4);
                this.SetExcelCellStyle(ref DataSheet, HeaderRow, HeaderCol, 12, System.Drawing.Color.White, false, false, 0, false);
                DataSheet.Cells[HeaderRow, HeaderCol++].PutValue(BusinessConstants.FORM_DOWNLOAD_LANG5);
                this.SetExcelCellStyle(ref DataSheet, HeaderRow, HeaderCol, 12, System.Drawing.Color.White, false, false, 0, false);
                DataSheet.Cells[HeaderRow, HeaderCol++].PutValue(BusinessConstants.FORM_DOWNLOAD_STARTCODE5);
                this.SetExcelCellStyle(ref DataSheet, HeaderRow, HeaderCol, 12, System.Drawing.Color.White, false, false, 0, false);
                DataSheet.Cells[HeaderRow, HeaderCol++].PutValue(BusinessConstants.FORM_DOWNLOAD_ENDCODE5);
                this.SetExcelCellStyle(ref DataSheet, HeaderRow, HeaderCol, 12, System.Drawing.Color.White, false, false, 0, false);
                DataSheet.Cells[HeaderRow, HeaderCol++].PutValue(BusinessConstants.FORM_DOWNLOAD_BUSS_CLASS);
                this.SetExcelCellStyle(ref DataSheet, HeaderRow, HeaderCol, 12, System.Drawing.Color.White, false, false, 22, false);
                DataSheet.Cells[HeaderRow, HeaderCol++].PutValue(BusinessConstants.FORM_DOWNLOAD_CHANGE_TIME);
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(DownloadBusiness.CLASS_NAME, "InsertFormFilterstoWorkBook", Ex);
            }
        }

        private void SetExcelCellStyle(ref Worksheet DataSheet, int Row, int Col, int FontSize, System.Drawing.Color Color, bool IsBackGroundColor = false, bool IsBold = false, int CellDataTypeNo = 0, bool IsTextWrap = false)
        {
            try
            {
                Cell cell = DataSheet.Cells[Row, Col];
                Style style = cell.GetStyle();
                style.Font.Name = "Calibri";
                style.Font.IsBold = IsBold;
                style.Font.Size = FontSize;
                if (IsBackGroundColor)
                {
                    style.Pattern = BackgroundType.Solid;
                    style.ForegroundColor = Color;
                }
                style.Number = CellDataTypeNo;
                if (IsTextWrap)
                {
                    style.IsTextWrapped = true;
                }
                cell.SetStyle(style);
                if (cell.IsMerged)
                {
                    cell.GetMergedRange().SetOutlineBorder(BorderType.TopBorder, CellBorderType.Thin, System.Drawing.Color.Black);
                    cell.GetMergedRange().SetOutlineBorder(BorderType.BottomBorder, CellBorderType.Thin, System.Drawing.Color.Black);
                    cell.GetMergedRange().SetOutlineBorder(BorderType.LeftBorder, CellBorderType.Thin, System.Drawing.Color.Black);
                    cell.GetMergedRange().SetOutlineBorder(BorderType.RightBorder, CellBorderType.Thin, System.Drawing.Color.Black);
                }
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(DownloadBusiness.CLASS_NAME, "SetExcelCellAttributes", Ex);
            }
        }

        public DownloadStatusResponse CheckDownloadStatus(string FileName)
        {
            DownloadStatusResponse DownloadStatusResponse = new DownloadStatusResponse();
            BusinessUtil ObjBusinessUtil = new BusinessUtil();
            SICTLogger.WriteInfo(DownloadBusiness.CLASS_NAME, "CheckDownloadStatus", "Start ");
            try
            {
                string FilePath = string.Empty;
                FilePath = ObjBusinessUtil.GetDownloadFilePathByName(FileName);
                if (System.IO.File.Exists(FilePath))
                {
                    DownloadStatusResponse.IsDownloadComplete = true;
                    DownloadStatusResponse.ReturnCode = 1;
                    DownloadStatusResponse.ReturnMessage = "Download Completed";
                }
                else
                {
                    DownloadStatusResponse.IsDownloadComplete = false;
                    DownloadStatusResponse.ReturnCode = 5;
                    DownloadStatusResponse.ReturnMessage = "Download still in progress Completed";
                }
            }
            catch (System.Exception Ex)
            {
                DownloadStatusResponse.ReturnCode = 1;
                DownloadStatusResponse.ReturnMessage = "Error in Function ";
                SICTLogger.WriteException(DownloadBusiness.CLASS_NAME, "CheckDownloadStatus", Ex);
            }
            SICTLogger.WriteInfo(DownloadBusiness.CLASS_NAME, "CheckDownloadStatus", "CheckDownloadStatusEnd");
            return DownloadStatusResponse;
        }
    }
}
