using SICT.Constants;
using SICT.DataAccessLayer;
using SICT.DataContracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;

namespace SICT.BusinessLayer.V1
{
	public class DepartureFormBusiness
	{
		public enum FilterColForDB
		{
			[Description("I.interviewer_name")]
			Interviewer,
			[Description("AR.airline_name")]
			Airline,
			[Description("F.flight_number")]
			FlightNumber,
			[Description("A.airport_name")]
			Destination,
			[Description("F.distribution_date")]
			DistributionDate,
			[Description("L{0}.language_name")]
			Language,
			[Description("F.start_code_{0}")]
			StartCode,
			[Description("F.end_code_{0}")]
			EndCode,
			[Description("F.airline_id")]
			AirlineId,
			[Description("F.interviewer_id")]
			InterviewerId,
			[Description("F.destination_id")]
			DestinationId,
			[Description("F.last_updated_timestamp")]
			LastUpdatedTime,
			[Description("F.type")]
			Type,
			[Description("F.distribution_date {0}, F.form_entry_id {0}")]
			DistributionDateSort,
			[Description("L.username")]
			AirportCode,
			[Description("F.form_entry_id")]
			FormId
		}

		private static readonly string CLASS_NAME = "DepartureFormBusiness";

		public static string GetEnumDescription(System.Enum value)
		{
			System.Reflection.FieldInfo fi = value.GetType().GetField(value.ToString());
			DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
			string result;
			if (attributes != null && attributes.Length > 0)
			{
				result = attributes[0].Description;
			}
			else
			{
				result = value.ToString();
			}
			return result;
		}

		public FinalDepartureFormDetails GetDepartureFormDetails(string Instance, string SessionId, DepartureFormFilterDetails DepartureFormFilterDetails)
		{
			FinalDepartureFormDetails FinalDepartureFormDetails = new FinalDepartureFormDetails();
			SICTLogger.WriteInfo(DepartureFormBusiness.CLASS_NAME, "GetDepartureFormDetails", "Start ");
			try
			{
				DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
				System.Collections.Generic.List<DepartureFormDetails> DepartureFormDetails = new System.Collections.Generic.List<DepartureFormDetails>();
				string OrderByCondition = string.Empty;
				string WhereCondition = string.Empty;
				this.BuildOrderByandWhereConditions(DepartureFormFilterDetails, ref OrderByCondition, ref WhereCondition);
				DataSet DSCardDetails = new DataSet();
				SICTLogger.WriteVerbose(DepartureFormBusiness.CLASS_NAME, "GetDepartureFormDetails", "Retrieving Departure Form Details From DB Start");
				if (DepartureFormFilterDetails.IsDepartureForm != BusinessConstants.DEFAULT_SELECTION_VALUE)
				{
					UserDetailsBusiness Udb = new UserDetailsBusiness();
					if (!Udb.CheckIsSpecialUserOrNot(DBLayer.GetUserNameBySessionId(SessionId)))
					{
						DSCardDetails = DBLayer.GetDepartureFormDetailst(SessionId, DepartureFormFilterDetails.StartIndex, DepartureFormFilterDetails.OffSet, OrderByCondition, WhereCondition, DepartureFormFilterDetails.AirportId, DepartureFormFilterDetails.IsDepartureForm, false, 0L, 0L);
					}
					else
					{
						DSCardDetails = DBLayer.GetDepartureFormDetailstForSpecialUser(SessionId, DepartureFormFilterDetails.StartIndex, DepartureFormFilterDetails.OffSet, OrderByCondition, WhereCondition, DepartureFormFilterDetails.AirportId, DepartureFormFilterDetails.IsDepartureForm, false, 0L, 0L);
					}
				}
				else
				{
					DSCardDetails = DBLayer.GetGlobalDepartureFormDetailst(SessionId, DepartureFormFilterDetails.StartIndex, DepartureFormFilterDetails.OffSet, OrderByCondition, WhereCondition, DepartureFormFilterDetails.AirportId, DepartureFormFilterDetails.IsDepartureForm, false, 0L, 0L);
				}
				FinalDepartureFormDetails = this.GetFormDetailsFromDataSet(Instance, DSCardDetails);
			}
			catch (System.Exception Ex)
			{
				FinalDepartureFormDetails.ReturnCode = -1;
				FinalDepartureFormDetails.ReturnMessage = "Error in Function ";
				SICTLogger.WriteException(DepartureFormBusiness.CLASS_NAME, "GetDepartureFormDetails", Ex);
			}
			SICTLogger.WriteInfo(DepartureFormBusiness.CLASS_NAME, "GetDepartureFormDetails", "GetDepartureFormDetailsEnd");
			return FinalDepartureFormDetails;
		}

		public void BuildOrderByandWhereConditions(DepartureFormFilterDetails DepartureFormFilterDetails, ref string OrderByCondition, ref string WhereCondition)
		{
			SICTLogger.WriteInfo(DepartureFormBusiness.CLASS_NAME, "BuildOrderByandWhereConditions", "BuildOrderByandWhereConditionsStart");
			try
			{
				SICTLogger.WriteVerbose(DepartureFormBusiness.CLASS_NAME, "BuildOrderByandWhereConditions", "Building Sort By Condition");
				string FilterCondition = string.Empty;
				string SearchCondition = string.Empty;
				if (null != DepartureFormFilterDetails.Sort)
				{
					bool IsASC = DepartureFormFilterDetails.IsSortByAsc;
					string sort = DepartureFormFilterDetails.Sort;
					switch (sort)
					{
					case "DistributionDate":
						if (IsASC)
						{
							OrderByCondition = string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.DistributionDateSort), " Asc ");
						}
						else
						{
							OrderByCondition = string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.DistributionDateSort), " Desc ");
						}
						break;
					case "Interviewer":
						if (IsASC)
						{
							OrderByCondition = DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.Interviewer) + " Asc ";
						}
						else
						{
							OrderByCondition = DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.Interviewer) + " Desc ";
						}
						break;
					case "Airline":
						if (IsASC)
						{
							OrderByCondition = DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.Airline) + " Asc ";
						}
						else
						{
							OrderByCondition = DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.Airline) + " Desc ";
						}
						break;
					case "FlightNumber":
						if (IsASC)
						{
							OrderByCondition = DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.FlightNumber) + " Asc ";
						}
						else
						{
							OrderByCondition = DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.FlightNumber) + " Desc ";
						}
						break;
					case "Destination":
						if (IsASC)
						{
							OrderByCondition = DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.Destination) + " Asc ";
						}
						else
						{
							OrderByCondition = DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.Destination) + " Desc ";
						}
						break;
					case "LastUpdatedDate":
						if (IsASC)
						{
							OrderByCondition = DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.LastUpdatedTime) + " Asc ";
						}
						else
						{
							OrderByCondition = DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.LastUpdatedTime) + " Desc ";
						}
						break;
					case "AirportCode":
						if (IsASC)
						{
							OrderByCondition = DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.AirportCode) + " Asc ";
						}
						else
						{
							OrderByCondition = DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.AirportCode) + " Desc ";
						}
						break;
					case "FormId":
						if (IsASC)
						{
							OrderByCondition = DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.FormId) + " Asc ";
						}
						else
						{
							OrderByCondition = DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.FormId) + " Desc ";
						}
						break;
					}
				}
				SICTLogger.WriteVerbose(DepartureFormBusiness.CLASS_NAME, "BuildOrderByandWhereConditions", "Building Wyhere Condition");
				System.Collections.Generic.List<string> FilterConditions = new System.Collections.Generic.List<string>();
				string FilterFormat = "{0} = {1}";
				if (null != DepartureFormFilterDetails.FormFilters)
				{
					if (DepartureFormFilterDetails.FormFilters.AirlineId != -1)
					{
						string Filter = string.Format(FilterFormat, DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.AirlineId), DepartureFormFilterDetails.FormFilters.AirlineId);
						FilterConditions.Add(Filter);
					}
					if (DepartureFormFilterDetails.FormFilters.DestinationId != -1)
					{
						string Filter = string.Format(FilterFormat, DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.DestinationId), DepartureFormFilterDetails.FormFilters.DestinationId);
						FilterConditions.Add(Filter);
					}
					if (DepartureFormFilterDetails.FormFilters.InterviewerId != -1)
					{
						string Filter = string.Format(FilterFormat, DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.InterviewerId), DepartureFormFilterDetails.FormFilters.InterviewerId);
						FilterConditions.Add(Filter);
					}
					if (!string.IsNullOrEmpty(DepartureFormFilterDetails.FormFilters.FlightNumber))
					{
						string Filter = string.Format("{0} like {1}", DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.FlightNumber), "'%" + DepartureFormFilterDetails.FormFilters.FlightNumber + "%'");
						FilterConditions.Add(Filter);
					}
					if (!string.IsNullOrEmpty(DepartureFormFilterDetails.FormFilters.StartDate) && !string.IsNullOrEmpty(DepartureFormFilterDetails.FormFilters.EndDate))
					{
						string Filter = string.Format(" {0} between '{1}' and '{2}' ", DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.DistributionDate), DepartureFormFilterDetails.FormFilters.StartDate, DepartureFormFilterDetails.FormFilters.EndDate);
						FilterConditions.Add(Filter);
					}
					else if (!string.IsNullOrEmpty(DepartureFormFilterDetails.FormFilters.StartDate))
					{
						string Filter = string.Format(" {0} >= '{1}' ", DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.DistributionDate), DepartureFormFilterDetails.FormFilters.StartDate);
						FilterConditions.Add(Filter);
					}
					else if (!string.IsNullOrEmpty(DepartureFormFilterDetails.FormFilters.EndDate))
					{
						string Filter = string.Format(" {0} <= '{1}' ", DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.DistributionDate), DepartureFormFilterDetails.FormFilters.EndDate);
						FilterConditions.Add(Filter);
					}
					FilterCondition = string.Join(BusinessConstants.SPLIT_AND, FilterConditions);
				}
				System.Collections.Generic.List<string> SearchFilterConditions = new System.Collections.Generic.List<string>();
				if (!string.IsNullOrEmpty(DepartureFormFilterDetails.FilterValue))
				{
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.Interviewer), DepartureFormFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.Airline), DepartureFormFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.FlightNumber), DepartureFormFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.Destination), DepartureFormFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.DistributionDate), DepartureFormFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.LastUpdatedTime), DepartureFormFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.Type), DepartureFormFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.AirportCode), DepartureFormFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.FormId), DepartureFormFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.Language), 1), DepartureFormFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.Language), 2), DepartureFormFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.Language), 3), DepartureFormFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.Language), 4), DepartureFormFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.Language), 5), DepartureFormFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.StartCode), 1), DepartureFormFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.StartCode), 2), DepartureFormFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.StartCode), 3), DepartureFormFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.StartCode), 4), DepartureFormFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.StartCode), 5), DepartureFormFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.EndCode), 1), DepartureFormFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.EndCode), 2), DepartureFormFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.EndCode), 3), DepartureFormFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.EndCode), 4), DepartureFormFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.EndCode), 5), DepartureFormFilterDetails.FilterValue));
					SearchCondition = string.Join(BusinessConstants.SPLIT_OR, SearchFilterConditions);
				}
				if (!string.IsNullOrEmpty(FilterCondition) && !string.IsNullOrEmpty(SearchCondition))
				{
					WhereCondition = string.Format("(({0}) and ({1}))", FilterCondition, SearchCondition);
				}
				else if (!string.IsNullOrEmpty(FilterCondition))
				{
					WhereCondition = string.Format("({0})", FilterCondition);
				}
				else if (!string.IsNullOrEmpty(SearchCondition))
				{
					WhereCondition = string.Format("({0})", SearchCondition);
				}
			}
			catch (System.Exception Ex)
			{
				SICTLogger.WriteException(DepartureFormBusiness.CLASS_NAME, "BuildOrderByandWhereConditions", Ex);
			}
			SICTLogger.WriteInfo(DepartureFormBusiness.CLASS_NAME, "BuildOrderByandWhereConditions", "BuildOrderByandWhereConditionsEnd");
		}

		public FormSubmitDetails SetFormDetails(string Instance, string SessionId, ref FormDetails FormDetails)
		{
			FormSubmitDetails FormSubmitDetails = new FormSubmitDetails();
			SICTLogger.WriteInfo(DepartureFormBusiness.CLASS_NAME, "SetFormDetails", "SetFormDetailsStart");
			try
			{
				System.Collections.Generic.List<int> FormIds = new System.Collections.Generic.List<int>();
				DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
				SICTLogger.WriteVerbose(DepartureFormBusiness.CLASS_NAME, "SetFormDetails", "Inserting Form Details into the DataBase Start");
				FormSubmitDetails = DBLayer.SetFormDetails(Instance, FormDetails);
				FormSubmitDetails.ReturnCode = 1;
				FormSubmitDetails.ReturnMessage = "Adding Form Details Successful";
				foreach (AirlineDetail ObjAirlineDetail in FormSubmitDetails.AirlineDetails)
				{
					new AuditLogBusiness().AddFormEntryAuditLog(ObjAirlineDetail.IsSuccess, SessionId, ObjAirlineDetail.FormId, true);
				}
			}
			catch (System.Exception Ex)
			{
				FormSubmitDetails.ReturnCode = -1;
				FormSubmitDetails.ReturnMessage = "Error in API Execution";
				SICTLogger.WriteException(DepartureFormBusiness.CLASS_NAME, "SetFormDetails", Ex);
			}
			SICTLogger.WriteInfo(DepartureFormBusiness.CLASS_NAME, "SetFormDetails", "SetFormDetailsEnd");
			return FormSubmitDetails;
		}

		public FormSubmitDetails UpdateFormDetails(string Instance, FormDetails FormDetails, string SessionId)
		{
			FormSubmitDetails FormSubmitDetails = new FormSubmitDetails();
			SICTLogger.WriteInfo(DepartureFormBusiness.CLASS_NAME, "UpdateFormDetails", "UpdateFormDetailsStart");
			try
			{
				DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
				SICTLogger.WriteVerbose(DepartureFormBusiness.CLASS_NAME, "UpdateFormDetails", "Updating Form Details in the DataBase Start");
				UserDetailsBusiness Udb = new UserDetailsBusiness();
				bool IsSpecialUser = Udb.CheckIsSpecialUserOrNot(DBLayer.GetUserNameBySessionId(SessionId));
				if (IsSpecialUser)
				{
					FormSubmitDetails = DBLayer.UpdateFormDetailsForSpecialUser(Instance, SessionId, FormDetails);
				}
				else
				{
					FormSubmitDetails = DBLayer.UpdateFormDetails(Instance, SessionId, FormDetails);
				}
				FormSubmitDetails.ReturnCode = 1;
				FormSubmitDetails.ReturnMessage = "Updating Form Details Successful";
				foreach (AirlineDetail ObjAirlineDetail in FormSubmitDetails.AirlineDetails)
				{
					new AuditLogBusiness().AddFormEntryAuditLog(ObjAirlineDetail.IsSuccess, SessionId, ObjAirlineDetail.FormId, false);
				}
				Task.Factory.StartNew<ReturnValue>(() => new CacheFileBusiness().CreateCacheFileforTargetVsCompletesChartsForaAirport(Instance, FormDetails.AirportId));
			}
			catch (System.Exception Ex)
			{
				FormSubmitDetails.ReturnCode = -1;
				FormSubmitDetails.ReturnMessage = "Error in API Execution";
				SICTLogger.WriteException(DepartureFormBusiness.CLASS_NAME, "UpdateFormDetails", Ex);
			}
			SICTLogger.WriteInfo(DepartureFormBusiness.CLASS_NAME, "UpdateFormDetails", "UpdateFormDetailsEnd");
			return FormSubmitDetails;
		}

		public ReturnValue DeleteFormDetails(int FormId)
		{
			ReturnValue ReturnValue = new ReturnValue();
			SICTLogger.WriteInfo(DepartureFormBusiness.CLASS_NAME, "DeleteFormDetails", "DeleteFormDetailsStart");
			try
			{
				DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
				SICTLogger.WriteVerbose(DepartureFormBusiness.CLASS_NAME, "DeleteFormDetails", "Deleting Form Details from the DataBase Start");
				bool IsSuccess = DBLayer.DeleteFormDetails(FormId);
				if (IsSuccess)
				{
					ReturnValue.ReturnCode = 1;
					ReturnValue.ReturnMessage = "Form Details Deleted Successfully";
				}
				else
				{
					ReturnValue.ReturnCode = -1;
					ReturnValue.ReturnMessage = "Form Details Delete UnSuccessfull - Error While Deleting in DB ";
				}
			}
			catch (System.Exception Ex)
			{
				ReturnValue.ReturnCode = -1;
				ReturnValue.ReturnMessage = "Error in API Execution";
				SICTLogger.WriteException(DepartureFormBusiness.CLASS_NAME, "DeleteFormDetails", Ex);
			}
			SICTLogger.WriteInfo(DepartureFormBusiness.CLASS_NAME, "DeleteFormDetails", "DeleteFormDetailsEnd");
			return ReturnValue;
		}

		private FinalDepartureFormDetails GetFormDetailsFromDataSet(string Instance, DataSet DSForm)
		{
			FinalDepartureFormDetails FinalDepartureFormDetails = new FinalDepartureFormDetails();
			SICTLogger.WriteInfo(DepartureFormBusiness.CLASS_NAME, "GetFormDetailsFromDataSrt", "Start ");
			try
			{
				if (DSForm.Tables.Count > 1)
				{
					System.Collections.Generic.List<DepartureFormDetails> DepartureFormDetails = new System.Collections.Generic.List<DepartureFormDetails>();
					int RecordsCnt = 0;
					if (DSForm.Tables[0].Rows.Count > 0)
					{
						RecordsCnt = System.Convert.ToInt32(DSForm.Tables[0].Rows[0][BusinessConstants.FORM_TOTALRECORDS].ToString());
					}
					if (DSForm.Tables[1].Rows.Count > 0)
					{
						foreach (DataRow Dr in DSForm.Tables[1].Rows)
						{
							DepartureFormDetails TempCardDetails = new DepartureFormDetails();
							System.Collections.Generic.List<Language> Languages = new System.Collections.Generic.List<Language>();
							if (!string.IsNullOrEmpty(Dr[BusinessConstants.FORM_FORMID].ToString()))
							{
								TempCardDetails.FormId = System.Convert.ToInt32(Dr[BusinessConstants.FORM_FORMID].ToString());
							}
							if (!string.IsNullOrEmpty(Dr[BusinessConstants.AIRPORTID].ToString()))
							{
								TempCardDetails.AirportId = System.Convert.ToInt32(Dr[BusinessConstants.AIRPORTID].ToString());
							}
							if (!string.IsNullOrEmpty(Dr[BusinessConstants.AIRPORTCODE].ToString()))
							{
								TempCardDetails.AirportCode = Dr[BusinessConstants.AIRPORTCODE].ToString();
							}
							if (!string.IsNullOrEmpty(Dr[BusinessConstants.FORM_INTERVIEWERID].ToString()))
							{
								TempCardDetails.InterviewerId = System.Convert.ToInt32(Dr[BusinessConstants.FORM_INTERVIEWERID].ToString());
							}
							if (!string.IsNullOrEmpty(Dr[BusinessConstants.FORM_INTERVIEWERNAME].ToString()))
							{
								TempCardDetails.Interviewer = Dr[BusinessConstants.FORM_INTERVIEWERNAME].ToString();
							}
							if (!string.IsNullOrEmpty(Dr[BusinessConstants.AIRLINEID].ToString()))
							{
								TempCardDetails.AirlineId = System.Convert.ToInt32(Dr[BusinessConstants.AIRLINEID].ToString());
							}
							if (!string.IsNullOrEmpty(Dr[BusinessConstants.FORM_AIRLINENAME].ToString()))
							{
								TempCardDetails.Airline = Dr[BusinessConstants.FORM_AIRLINENAME].ToString();
							}
							if (!string.IsNullOrEmpty(Dr[BusinessConstants.FORM_FLIGHTNUMBER].ToString()))
							{
								TempCardDetails.FlightNumber = Dr[BusinessConstants.FORM_FLIGHTNUMBER].ToString().Trim();
							}
							if (!string.IsNullOrEmpty(Dr[BusinessConstants.FORM_DESTINATIONID].ToString()))
							{
								TempCardDetails.DestinationId = System.Convert.ToInt32(Dr[BusinessConstants.FORM_DESTINATIONID].ToString());
							}
							if (!string.IsNullOrEmpty(Dr[BusinessConstants.FORM_DESTINATIONNAME].ToString()))
							{
								TempCardDetails.Destination = Dr[BusinessConstants.FORM_DESTINATIONNAME].ToString();
							}
							if (!string.IsNullOrEmpty(Dr[BusinessConstants.FORM_DISTRIBUTIONDATE].ToString()))
							{
								string DistDate = System.Convert.ToDateTime(Dr[BusinessConstants.FORM_DISTRIBUTIONDATE].ToString()).ToString("MM/dd/yyyy");
								DistDate = DistDate.Replace(".", "/");
								DistDate = DistDate.Replace(":", "/");
								TempCardDetails.DistributionDate = Convert.ToDateTime(DistDate);
                            }
							if (!string.IsNullOrEmpty(Dr[BusinessConstants.FORM_BUSINESSCARDS].ToString()))
							{
								TempCardDetails.BusinessCards = System.Convert.ToInt32(Dr[BusinessConstants.FORM_BUSINESSCARDS].ToString());
							}
							if (!string.IsNullOrEmpty(Dr[BusinessConstants.FORM_LASTUPDATEDTIME].ToString()))
							{
								string UpdateDate = System.Convert.ToDateTime(Dr[BusinessConstants.FORM_LASTUPDATEDTIME].ToString()).ToString("MM/dd/yyyy");
								UpdateDate = UpdateDate.Replace(".", "/");
								UpdateDate = UpdateDate.Replace(":", "/");
								TempCardDetails.LastUpdatedDate = Convert.ToDateTime(UpdateDate);

                            }
							if (!string.IsNullOrEmpty(Dr[BusinessConstants.FORM_TYPE].ToString()))
							{
								TempCardDetails.Type = Dr[BusinessConstants.FORM_TYPE].ToString();
							}
							SICTLogger.WriteVerbose(DepartureFormBusiness.CLASS_NAME, "GetFormDetailsFromDataSrt", "Start For Language Details Assignment");
							for (int LanguageCnt = 0; LanguageCnt < 5; LanguageCnt++)
							{
								string LanguageColName = string.Empty;
								string LanguageIdColName = string.Empty;
								string StartCodeColName = string.Empty;
								string EndCodeColName = string.Empty;
								LanguageColName = string.Format(BusinessConstants.FORM_LANGAUGE, LanguageCnt + 1);
								LanguageIdColName = string.Format(BusinessConstants.FORM_LANGAUGEID, LanguageCnt + 1);
								StartCodeColName = string.Format(BusinessConstants.FORM_STARTCODE, LanguageCnt + 1);
								EndCodeColName = string.Format(BusinessConstants.FORM_ENDCODE, LanguageCnt + 1);
								if (!string.IsNullOrEmpty(Dr[LanguageColName].ToString()))
								{
									Language TempLanguage = new Language();
									if (!string.IsNullOrEmpty(Dr[LanguageIdColName].ToString()))
									{
										TempLanguage.LanguageId = System.Convert.ToInt32(Dr[LanguageIdColName].ToString());
									}
									if (!string.IsNullOrEmpty(Dr[StartCodeColName].ToString()))
									{
										TempLanguage.FirstSerialNo = System.Convert.ToInt64(Dr[StartCodeColName].ToString());
									}
									if (!string.IsNullOrEmpty(Dr[EndCodeColName].ToString()))
									{
										TempLanguage.LastSerialNo = System.Convert.ToInt64(Dr[EndCodeColName].ToString());
									}
									Languages.Add(TempLanguage);
								}
							}
							TempCardDetails.Languages = Languages;
							if (Instance == BusinessConstants.Instance.AIR.ToString())
							{
								if (!string.IsNullOrEmpty(Dr[BusinessConstants.AIRCRAFTTYPE].ToString()))
								{
									TempCardDetails.AircraftType = Dr[BusinessConstants.AIRCRAFTTYPE].ToString();
								}
							}
							DepartureFormDetails.Add(TempCardDetails);
						}
						FinalDepartureFormDetails.TotalRecords = RecordsCnt;
						FinalDepartureFormDetails.DepartureFormDetails = DepartureFormDetails;
						FinalDepartureFormDetails.ReturnCode = 1;
						FinalDepartureFormDetails.ReturnMessage = "Records Retrieved Successfully ";
					}
					else
					{
						SICTLogger.WriteVerbose(DepartureFormBusiness.CLASS_NAME, "GetFormDetailsFromDataSrt", "Table Doesnt Contain AnyRows");
						FinalDepartureFormDetails.ReturnCode = 1;
						FinalDepartureFormDetails.ReturnMessage = "No Records Available ";
					}
				}
				else
				{
					SICTLogger.WriteVerbose(DepartureFormBusiness.CLASS_NAME, "GetFormDetailsFromDataSrt", "DataSet Retrieved Doesnt Contain Table");
					FinalDepartureFormDetails.ReturnCode = 1;
					FinalDepartureFormDetails.ReturnMessage = "No Records Found";
				}
			}
			catch (System.Exception Ex)
			{
				SICTLogger.WriteException(DepartureFormBusiness.CLASS_NAME, "GetFormDetailsFromDataSrt", Ex);
			}
			SICTLogger.WriteInfo(DepartureFormBusiness.CLASS_NAME, "GetFormDetailsFromDataSrt", "End");
			return FinalDepartureFormDetails;
		}

		public FinalDepartureFormDetails GetFormsbySerialNo(string Instance, string SessionId, SerialNoFilterDetails SerialNoFilterDetails)
		{
			FinalDepartureFormDetails FinalDepartureFormDetails = new FinalDepartureFormDetails();
			SICTLogger.WriteInfo(DepartureFormBusiness.CLASS_NAME, "GetFormsbySerialNo", "Start ");
			try
			{
				DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
				System.Collections.Generic.List<DepartureFormDetails> DepartureFormDetails = new System.Collections.Generic.List<DepartureFormDetails>();
				string OrderByCondition = string.Empty;
				string WhereCondition = string.Empty;
				this.BuildOrderByandWhereConditionforSerialNoFilterDetails(SerialNoFilterDetails, ref OrderByCondition, ref WhereCondition);
				DataSet DSCardDetails = new DataSet();
				SICTLogger.WriteVerbose(DepartureFormBusiness.CLASS_NAME, "GetFormsbySerialNo", "Retrieving Departure Form Details From DB Start");
				DSCardDetails = DBLayer.GetDepartureFormDetailst(SessionId, SerialNoFilterDetails.StartIndex, SerialNoFilterDetails.OffSet, OrderByCondition, WhereCondition, -1, BusinessConstants.DEFAULT_SELECTION_VALUE, true, SerialNoFilterDetails.StartSerialNo, SerialNoFilterDetails.EndSerialNo);
				FinalDepartureFormDetails = this.GetFormDetailsFromDataSet(Instance, DSCardDetails);
			}
			catch (System.Exception Ex)
			{
				FinalDepartureFormDetails.ReturnCode = -1;
				FinalDepartureFormDetails.ReturnMessage = "Error in Function ";
				SICTLogger.WriteException(DepartureFormBusiness.CLASS_NAME, "GetFormsbySerialNo", Ex);
			}
			SICTLogger.WriteInfo(DepartureFormBusiness.CLASS_NAME, "GetFormsbySerialNo", "GetFormsbySerialNoEnd");
			return FinalDepartureFormDetails;
		}

		private void BuildOrderByandWhereConditionforSerialNoFilterDetails(SerialNoFilterDetails SerialNoFilterDetails, ref string OrderByCondition, ref string WhereCondition)
		{
			SICTLogger.WriteInfo(DepartureFormBusiness.CLASS_NAME, "BuildOrderByandWhereConditionforSerialNoFilterDetails", "BuildOrderByandWhereConditionforSerialNoFilterDetailsStart");
			try
			{
				if (SerialNoFilterDetails.StartSerialNo > SerialNoFilterDetails.EndSerialNo)
				{
					long TempSerialNo = SerialNoFilterDetails.StartSerialNo;
					SerialNoFilterDetails.StartSerialNo = SerialNoFilterDetails.EndSerialNo;
					SerialNoFilterDetails.EndSerialNo = TempSerialNo;
				}
				SICTLogger.WriteVerbose(DepartureFormBusiness.CLASS_NAME, "BuildOrderByandWhereConditionforSerialNoFilterDetails", "Building Sort Condition");
				string FilterCondition = string.Empty;
				string SearchCondition = string.Empty;
				if (null != SerialNoFilterDetails.Sort)
				{
					bool IsASC = SerialNoFilterDetails.IsSortByAsc;
					string sort = SerialNoFilterDetails.Sort;
					switch (sort)
					{
					case "DistributionDate":
						if (IsASC)
						{
							OrderByCondition = string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.DistributionDateSort), " Asc ");
						}
						else
						{
							OrderByCondition = string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.DistributionDateSort), " Desc ");
						}
						break;
					case "Interviewer":
						if (IsASC)
						{
							OrderByCondition = DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.Interviewer) + " Asc ";
						}
						else
						{
							OrderByCondition = DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.Interviewer) + " Desc ";
						}
						break;
					case "Airline":
						if (IsASC)
						{
							OrderByCondition = DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.Airline) + " Asc ";
						}
						else
						{
							OrderByCondition = DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.Airline) + " Desc ";
						}
						break;
					case "FlightNumber":
						if (IsASC)
						{
							OrderByCondition = DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.FlightNumber) + " Asc ";
						}
						else
						{
							OrderByCondition = DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.FlightNumber) + " Desc ";
						}
						break;
					case "Destination":
						if (IsASC)
						{
							OrderByCondition = DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.Destination) + " Asc ";
						}
						else
						{
							OrderByCondition = DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.Destination) + " Desc ";
						}
						break;
					case "LastUpdatedDate":
						if (IsASC)
						{
							OrderByCondition = DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.LastUpdatedTime) + " Asc ";
						}
						else
						{
							OrderByCondition = DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.LastUpdatedTime) + " Desc ";
						}
						break;
					case "AirportCode":
						if (IsASC)
						{
							OrderByCondition = DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.AirportCode) + " Asc ";
						}
						else
						{
							OrderByCondition = DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.AirportCode) + " Desc ";
						}
						break;
					case "FormId":
						if (IsASC)
						{
							OrderByCondition = DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.FormId) + " Asc ";
						}
						else
						{
							OrderByCondition = DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.FormId) + " Desc ";
						}
						break;
					case "Type":
						if (IsASC)
						{
							OrderByCondition = DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.Type) + " Asc ";
						}
						else
						{
							OrderByCondition = DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.Type) + " Desc ";
						}
						break;
					}
				}
				SICTLogger.WriteVerbose(DepartureFormBusiness.CLASS_NAME, "BuildOrderByandWhereConditionforSerialNoFilterDetails", "Building Wyhere Condition");
				System.Collections.Generic.List<string> SearchFilterConditions = new System.Collections.Generic.List<string>();
				if (!string.IsNullOrEmpty(SerialNoFilterDetails.FilterValue))
				{
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.Interviewer), SerialNoFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.Airline), SerialNoFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.FlightNumber), SerialNoFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.Destination), SerialNoFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.DistributionDate), SerialNoFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.LastUpdatedTime), SerialNoFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.Type), SerialNoFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.AirportCode), SerialNoFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.FormId), SerialNoFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.Language), 1), SerialNoFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.Language), 2), SerialNoFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.Language), 3), SerialNoFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.Language), 4), SerialNoFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.Language), 5), SerialNoFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.StartCode), 1), SerialNoFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.StartCode), 2), SerialNoFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.StartCode), 3), SerialNoFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.StartCode), 4), SerialNoFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.StartCode), 5), SerialNoFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.EndCode), 1), SerialNoFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.EndCode), 2), SerialNoFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.EndCode), 3), SerialNoFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.EndCode), 4), SerialNoFilterDetails.FilterValue));
					SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, string.Format(DepartureFormBusiness.GetEnumDescription(DepartureFormBusiness.FilterColForDB.EndCode), 5), SerialNoFilterDetails.FilterValue));
					SearchCondition = string.Join(BusinessConstants.SPLIT_OR, SearchFilterConditions);
				}
				if (!string.IsNullOrEmpty(FilterCondition) && !string.IsNullOrEmpty(SearchCondition))
				{
					WhereCondition = string.Format("(({0}) and ({1}))", FilterCondition, SearchCondition);
				}
				else if (!string.IsNullOrEmpty(FilterCondition))
				{
					WhereCondition = string.Format("({0})", FilterCondition);
				}
				else if (!string.IsNullOrEmpty(SearchCondition))
				{
					WhereCondition = string.Format("({0})", SearchCondition);
				}
			}
			catch (System.Exception Ex)
			{
				SICTLogger.WriteException(DepartureFormBusiness.CLASS_NAME, "BuildOrderByandWhereConditionforSerialNoFilterDetails", Ex);
			}
			SICTLogger.WriteInfo(DepartureFormBusiness.CLASS_NAME, "BuildOrderByandWhereConditionforSerialNoFilterDetails", "BuildOrderByandWhereConditionforSerialNoFilterDetailsEnd");
		}
	}
}
