using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MonitPro.Models.PSSR;
using System.IO;
using MonitPro.Common.Library;
using System.Data.SqlClient;
using System.Xml.Serialization;

namespace MonitPro.DAL
{
    public class PSSRDAL
    {
        string constring = AppConfig.ConnectionString;
        SqlCommand objCom;
        SqlDataReader reader;

        public List<ActionByRecomStatus> GetActionByRecomStatusCount(DateTime startDate, DateTime endDate)
        {
            List<ActionByRecomStatus> Actionbystatuslist = new List<ActionByRecomStatus>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[PSSR_ActionByRecomStatusCount]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@StartDate", startDate);
                    objCom.Parameters.AddWithValue("@EndDate", endDate);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();
                    while (reader.Read())
                    {
                        ActionByRecomStatus actionby = new ActionByRecomStatus();
                        actionby.ActionBy = reader["ActionBy"].ToString();
                        actionby.Overdue = int.Parse(reader["overdue"].ToString());
                        actionby.Pending = int.Parse(reader["Pending"].ToString());
                        actionby.Completed = int.Parse(reader["Completed"].ToString());
                        if (actionby.Overdue > 0 || actionby.Pending > 0 || actionby.Completed > 0)
                            Actionbystatuslist.Add(actionby);
                    }
                }

            }
            catch (Exception Ex)
            {
                LogManager.Instance.Error(Ex);
                throw new Exception(Ex.Message);
            }
            return Actionbystatuslist;
        }

        public List<CAPAPlantwiseCount> GetCAPAPlantCountDAL(DateTime startDate, DateTime endDate)
        {
            List<CAPAPlantwiseCount> capastatuslist = new List<CAPAPlantwiseCount>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "GetCAPAPlantWiseObservationChart";
                    objCom.CommandType = CommandType.StoredProcedure;
                    //objCom.Parameters.AddWithValue("@StartDate", startDate);
                    //objCom.Parameters.AddWithValue("@EndDate", endDate);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();
                    while (reader.Read())
                    {
                        CAPAPlantwiseCount priority = new CAPAPlantwiseCount();
                        priority.Plantname = reader["PlantName"].ToString();
                        priority.TotalCount = int.Parse(reader["TotalCount"].ToString());
                        capastatuslist.Add(priority);
                    }
                }

            }
            catch (Exception Ex)
            {
                LogManager.Instance.Error(Ex);
                throw new Exception(Ex.Message);
            }
            return capastatuslist;
        }

        public List<MonthwiseStatus> GetMonthwiseStatusCount(DateTime startDate, DateTime endDate)
        {
            List<MonthwiseStatus> statuslist = new List<MonthwiseStatus>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[PSSR_MonthwiseStatusCount]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@StartDate", startDate);
                    objCom.Parameters.AddWithValue("@EndDate", endDate);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();
                    while (reader.Read())
                    {
                        MonthwiseStatus status = new MonthwiseStatus();
                        status.MonthName = reader["PSSRMonth"].ToString();
                        status.Draft = int.Parse(reader["Draft"].ToString());
                        status.Schedule = int.Parse(reader["PSSR Scheduled"].ToString());
                        status.Submittedforapproval = int.Parse(reader["PSSR Submitted For Approval"].ToString());
                        status.Approved = int.Parse(reader["PSSR Approved"].ToString());
                        status.Closed = int.Parse(reader["PSSR Closed"].ToString());

                        if (status.Draft > 0 || status.Schedule > 0 || status.Submittedforapproval > 0 || status.Approved > 0 || status.Closed > 0)
                            statuslist.Add(status);
                    }
                }

            }
            catch (Exception Ex)
            {
                LogManager.Instance.Error(Ex);
                throw new Exception(Ex.Message);
            }
            return statuslist;
        }



        public List<PSSRCategoryModel> GetPSSRCatory()
        {
            List<PSSRCategoryModel> CategoryList = new List<PSSRCategoryModel>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetPSSRCategory]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();
                    while (reader.Read())
                    {
                        PSSRCategoryModel category = new PSSRCategoryModel();
                        category.CategoryID = int.Parse(reader["PCID"].ToString());
                        category.CategoryName = reader["PSSR_CategoryName"].ToString();
                        CategoryList.Add(category);
                    }
                }

            }
            catch (Exception Ex)
            {
                LogManager.Instance.Error(Ex);
                throw new Exception(Ex.Message);
            }
            return CategoryList;
        }
        public List<Priority> GetPriority()
        {
            List<Priority> PriorityList = new List<Priority>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetMOCRecommPriority]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        PriorityList.Add(new Priority
                        {
                            PriorityID = int.Parse(reader["ID"].ToString()),
                            PriorityName = reader["Name"].ToString()
                        });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return PriorityList;
        }

        public List<Plants> GetPlants()
        {
            List<Plants> PlantsList = new List<Plants>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetPlants]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();


                    while (reader.Read())
                    {
                        PlantsList.Add(new Plants { ID = int.Parse(reader["ID"].ToString()), Name = reader["Name"].ToString(), Description = reader["Description"].ToString() });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return PlantsList;
        }
        public List<MOCNumberListModel> GetMocNumberList(int? PlantID)
        {
            List<MOCNumberListModel> Mocnumberlist = new List<MOCNumberListModel>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[PSSR_GetMOCNumberDD]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@PlantID", PlantID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();
                    while (reader.Read())
                    {
                        Mocnumberlist.Add(new MOCNumberListModel
                        {
                            ID = int.Parse(reader["ID"].ToString()),
                            MOCNo = reader["MOCNumber"].ToString()
                        });

                    }
                }

            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return Mocnumberlist;
        }
        public List<PSSRType> GetPSSRType()
        {
            List<PSSRType> TypeList = new List<PSSRType>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetPSSRType]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        TypeList.Add(new PSSRType
                        {
                            PTID = int.Parse(reader["PTID"].ToString()),
                            PTName = reader["PSSR_Type"].ToString()
                        });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return TypeList;
        }

        public List<PSSRStatus> GetPSSRStatus()
        {
            List<PSSRStatus> StatusList = new List<PSSRStatus>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetPSSRStatus]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        StatusList.Add(new PSSRStatus
                        {
                            StatusID = int.Parse(reader["PSSRStatusID"].ToString()),
                            StatusName = reader["PSSRStatusName"].ToString()
                        });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return StatusList;
        }
        public List<Employee> GetAllEmployees()
        {
            List<Employee> EmployeeList = new List<Employee>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetAllEmployees]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        EmployeeList.Add(new Employee
                        {
                            UserID = int.Parse(reader["UserID"].ToString()),
                            FirstName = reader["FIRSTNAME"].ToString(),
                            LastName = reader["LASTNAME"].ToString(),
                            FullName = reader["FIRSTNAME"].ToString() + ' ' + reader["LASTNAME"].ToString(),
                            Designation = reader["DESIGNATION"].ToString()
                        });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return EmployeeList;
        }

        public List<Employee> GetAssignTeamMembers(int PSSRID)
        {
            List<Employee> TeamMemberList = new List<Employee>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetPSSRAssignTeamMembers]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@PSSRID", PSSRID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        TeamMemberList.Add(new Employee
                        {
                            UserID = int.Parse(reader["UserID"].ToString()),
                            FirstName = reader["FIRSTNAME"].ToString(),
                            LastName = reader["LASTNAME"].ToString(),
                            FullName = reader["FullName"].ToString(),
                            Designation = reader["DESIGNATION"].ToString()
                        });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return TeamMemberList;
        }
        public PSSRAssignEmail GetPSSRAssignEmail(int PSSRID)
        {
            PSSRAssignEmail pSSRAssignEmail = new PSSRAssignEmail();
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "PSSR_AssignEmailAdress";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@PSSRID", PSSRID);
                    objCon.Open();
                    objCom.Connection = objCon;
                    SqlDataReader objReader = objCom.ExecuteReader();

                    while (objReader.Read())
                    {
                        pSSRAssignEmail.HSELeadEmail = objReader["HSELead"].ToString();
                        pSSRAssignEmail.EnggLeadEmail = objReader["EnggLead"].ToString();
                        pSSRAssignEmail.OpLeadEmail = objReader["OpLead"].ToString();
                        pSSRAssignEmail.PSSRLeadEmail = objReader["PSSRLead"].ToString();
                        pSSRAssignEmail.ChairPersonEmail = objReader["chairperson"].ToString();
                        pSSRAssignEmail.TeamMembersEmail = objReader["TeamMembers"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return pSSRAssignEmail;
        }
        public List<Department> GetDepartmentList()
        {
            List<Department> departList = null;
            try
            {
                departList = new List<Department>();

                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "DepartmentSelect";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    objCom.Connection = objCon;
                    SqlDataReader objReader = objCom.ExecuteReader();

                    while (objReader.Read())
                    {
                        var dept = new Department();
                        dept.DeptID = int.Parse(objReader["DeptID"].ToString());
                        dept.DeptName = objReader["DeptName"].ToString();
                        departList.Add(dept);
                    }
                    objCon.Close();
                }


            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);
            }
            return departList;

        }
        public int PSSRInsertUpdate(CreatePSSRModel createmodel, string fileName)
        {
            int affect = 0;
            List<ChecklistAssignXML> ChAssignList = new List<ChecklistAssignXML>();
            string PSSRCheck = string.Empty;
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[PSSRInsertUpdate]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@PlantID", createmodel.PlantID);
                    if (createmodel.MOCNo > 0)
                    {
                        objCom.Parameters.AddWithValue("@MOCNo", createmodel.MOCNo);
                    }
                    objCom.Parameters.AddWithValue("@CategoryID", createmodel.CategoryID);
                    objCom.Parameters.AddWithValue("@SystemDesc", createmodel.SystemDesc);
                    objCom.Parameters.AddWithValue("@PSSRType", createmodel.PSSRType);
                    objCom.Parameters.AddWithValue("@CreatedBy", createmodel.CreatedByID);

                    if (!string.IsNullOrEmpty(createmodel.AssessmentDatetime))
                    {
                        objCom.Parameters.AddWithValue("@ScheduledDatetime", DateTime.ParseExact(createmodel.AssessmentDatetime,
                            "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture));

                    }
                    objCom.Parameters.AddWithValue("@PSSRID", createmodel.PSSRID);
                    objCom.Parameters.AddWithValue("@PSSRStatus", createmodel.PSSRStatus);

                    objCom.Parameters.AddWithValue("@ChairPersonComments", createmodel.ChairPersonComments);
                    objCom.Parameters.AddWithValue("@OperationHeadComments", createmodel.OperationHeadComments);
                    objCom.Parameters.AddWithValue("@HSELeadComments", createmodel.HSELeadComments);
                    objCom.Parameters.AddWithValue("@EnggLeadComments", createmodel.EnggLeadComments);
                    objCom.Parameters.AddWithValue("@PSSRLeadComments", createmodel.PSSRLeadComments);

                    if (!string.IsNullOrEmpty(createmodel.ChairPersonDateTime))
                    {
                        objCom.Parameters.AddWithValue("@ChairPersonDateTime", DateTime.ParseExact(createmodel.ChairPersonDateTime,
                            "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture));
                    }

                    if (!string.IsNullOrEmpty(createmodel.OperationHeadDateTime))
                    {
                        objCom.Parameters.AddWithValue("@OperationHeadDateTime", DateTime.ParseExact(createmodel.OperationHeadDateTime, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture));
                    }
                    if (!string.IsNullOrEmpty(createmodel.HSELeadDateTime))
                    {
                        objCom.Parameters.AddWithValue("@HSELeadDateTime", DateTime.ParseExact(createmodel.HSELeadDateTime, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture));
                    }
                    if (!string.IsNullOrEmpty(createmodel.EnggLeadDateTime))
                    {
                        objCom.Parameters.AddWithValue("@EnggLeadDateTime", DateTime.ParseExact(createmodel.EnggLeadDateTime, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture));
                    }
                    if (!string.IsNullOrEmpty(createmodel.PSSRLeadDateTime))
                    {
                        objCom.Parameters.AddWithValue("@PSSRLeadDateTime", DateTime.ParseExact(createmodel.PSSRLeadDateTime, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture));
                    }
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        objCom.Parameters.AddWithValue("@PSSRAttach", fileName);
                    }
                    objCom.Parameters.AddWithValue("@UserID", createmodel.UserID);
                    SqlParameter outPutParameter = new SqlParameter();
                    outPutParameter.ParameterName = "@NewPSSRID";
                    outPutParameter.SqlDbType = System.Data.SqlDbType.Int;
                    outPutParameter.Direction = System.Data.ParameterDirection.Output;
                    objCom.Parameters.Add(outPutParameter);

                    objCom.Connection = objCon;
                    objCon.Open();
                    affect = objCom.ExecuteNonQuery();
                    createmodel.PSSRID = int.Parse(outPutParameter.Value.ToString());
                    objCom.Parameters.Clear();


                    createmodel.PSSRID = int.Parse(outPutParameter.Value.ToString());
                    if (createmodel.GetCheckLists != null)
                    {
                        foreach (var item in createmodel.GetCheckLists)
                        {
                            if (item.yesNo == true)
                            {
                                var xml = new ChecklistAssignXML
                                {
                                    chID = item.PCMID

                                };
                                ChAssignList.Add(xml);
                            }
                        }
                    }
                    XmlSerializer xmlSerializer = new XmlSerializer(ChAssignList.GetType());

                    using (StringWriter textWriter = new StringWriter())
                    {
                        xmlSerializer.Serialize(textWriter, ChAssignList);
                        PSSRCheck = textWriter.ToString();
                    }

                    objCom.CommandText = "PSSR_AssignedChecklistInsert";
                    objCom.Parameters.AddWithValue("@PSSRID", createmodel.PSSRID);
                    objCom.Parameters.AddWithValue("@Checklistdetails", PSSRCheck);
                    objCom.Parameters.AddWithValue("@UserID", createmodel.CreatedByID);
                    affect = objCom.ExecuteNonQuery();


                    objCon.Close();


                }

            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return createmodel.PSSRID;
        }




        public List<PSSRListViewModel> GetAllPSSRList()
        {
            List<PSSRListViewModel> PSSRList = new List<PSSRListViewModel>();
            int affect = 1;
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {

                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[GetAllPSSRList]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    objCom.Connection = objCon;
                    var objreader = objCom.ExecuteReader();
                    while (objreader.Read())
                    {
                        PSSRListViewModel pSSRListView = new PSSRListViewModel();
                        pSSRListView.SNO = affect++;
                        pSSRListView.PSSRID = int.Parse(objreader["PSSR_ID"].ToString());
                        pSSRListView.PlantName = objreader["PlantName"].ToString();
                        pSSRListView.ScheduledDatetime = objreader["ScheduledDatetime"].ToString();
                        pSSRListView.SystemDesc = objreader["System_Description"].ToString();
                        pSSRListView.PSSRType = objreader["PSSR_Type"].ToString();
                        pSSRListView.Category = objreader["PSSR_CategoryName"].ToString();
                        pSSRListView.PSSRStatus = objreader["PSSRStatusName"].ToString();
                        pSSRListView.PSSRLead = objreader["PSSRLead"].ToString();
                        pSSRListView.Attachment = objreader["Attachments"].ToString();
                        if (objreader["ChairPerson"] != DBNull.Value)
                        {
                            pSSRListView.chairper = int.Parse(objreader["ChairPerson"].ToString());
                        }
                        if (objreader["HSELead"] != DBNull.Value)
                        {
                            pSSRListView.HSELead = int.Parse(objreader["HSELead"].ToString());

                        }
                        if (objreader["EnggLead"] != DBNull.Value)
                        {
                            pSSRListView.EnggLead = int.Parse(objreader["EnggLead"].ToString());
                        }
                        if (objreader["OPLead"] != DBNull.Value)
                        {
                            pSSRListView.OPLead = int.Parse(objreader["OPLead"].ToString());
                        }
                        PSSRList.Add(pSSRListView);
                    }
                    objCon.Close();
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return PSSRList;
        }

        public List<PSSRListViewModel> SearchPSSRList(SearchList searchModel)
        {
            List<PSSRListViewModel> PSSRList = new List<PSSRListViewModel>();
            int affect = 1;
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {

                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[PSSR_SearchList]";
                    objCom.CommandType = CommandType.StoredProcedure;

                    objCom.Parameters.AddWithValue("@FromDate", searchModel.FromDate == null ? string.Empty : searchModel.FromDate);
                    objCom.Parameters.AddWithValue("@ToDate", searchModel.ToDate == null ? string.Empty : searchModel.ToDate);
                    objCom.Parameters.AddWithValue("@PSSR_Status", searchModel.Status);
                    objCom.Parameters.AddWithValue("@PlantArea", searchModel.PlantID);
                    objCom.Parameters.AddWithValue("@PSSR_Category", searchModel.Category);
                    objCom.Parameters.AddWithValue("@PSSR_Type", searchModel.Type);

                    objCom.Connection = objCon;
                    objCon.Open();
                    objCom.Connection = objCon;
                    var objreader = objCom.ExecuteReader();
                    while (objreader.Read())
                    {
                        PSSRListViewModel pSSRListView = new PSSRListViewModel();
                        pSSRListView.SNO = affect++;
                        pSSRListView.PSSRID = int.Parse(objreader["PSSR_ID"].ToString());
                        pSSRListView.PlantName = objreader["PlantName"].ToString();
                        pSSRListView.SystemDesc = objreader["System_Description"].ToString();
                        pSSRListView.PSSRType = objreader["PSSR_Type"].ToString();
                        pSSRListView.Category = objreader["PSSR_CategoryName"].ToString();
                        pSSRListView.PSSRStatus = objreader["PSSRStatusName"].ToString();
                        pSSRListView.ScheduledDatetime = objreader["ScheduledDatetime"].ToString();
                        pSSRListView.PSSRLead = objreader["PSSRLead"].ToString();
                        pSSRListView.Attachment = objreader["Attachments"].ToString();
                        PSSRList.Add(pSSRListView);
                    }
                    objCon.Close();
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return PSSRList;
        }


        public CreatePSSRModel GetPSSR(int PSSRID)
        {
            CreatePSSRModel getCreatepssr = new CreatePSSRModel();
            try
            {

                using (SqlConnection objCon = new SqlConnection(constring))
                {

                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[GetPSSR]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@PSSRID", PSSRID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    objCom.Connection = objCon;
                    var objreader = objCom.ExecuteReader();
                    while (objreader.Read())
                    {
                        getCreatepssr.PSSRID = int.Parse(objreader["PSSR_ID"].ToString());
                        getCreatepssr.PlantID = int.Parse(objreader["PlantID"].ToString());
                        getCreatepssr.CategoryID = int.Parse(objreader["PSSRCategory"].ToString());
                        getCreatepssr.PSSRType = int.Parse(objreader["PSSRType"].ToString());
                        getCreatepssr.SystemDesc = objreader["System_Description"].ToString();
                        if (objreader["PSSRStatus"] != DBNull.Value)
                        {
                            getCreatepssr.PSSRStatus = int.Parse(objreader["PSSRStatus"].ToString());
                        }
                        if (objreader["PSSR_ScheduledDatetime"] != DBNull.Value)
                        {
                            getCreatepssr.AssessmentDatetime = objreader["PSSR_ScheduledDatetime"].ToString();
                        }
                        if (objreader["PlantName"] != DBNull.Value)
                        {
                            getCreatepssr.PlantName = objreader["PlantName"].ToString();
                        }
                        if (objreader["PSSR_CategoryName"] != DBNull.Value)
                        {
                            getCreatepssr.CategoryName = objreader["PSSR_CategoryName"].ToString();
                        }
                        if (objreader["CreatedBy"] != DBNull.Value)
                        {
                            getCreatepssr.CreatedBy = objreader["CreatedBy"].ToString();
                        }
                        if (objreader["CreatedDatetime"] != DBNull.Value)
                        {
                            getCreatepssr.CreatedDateTime = objreader["CreatedDatetime"].ToString();
                        }
                        if (objreader["Attachments"] != DBNull.Value)
                        {
                            getCreatepssr.FileName = objreader["Attachments"].ToString();
                        }
                        if (objreader["MOCNo"] != DBNull.Value)
                        {
                            getCreatepssr.MOCNo = int.Parse(objreader["MOCNo"].ToString());
                        }
                        getCreatepssr.ChairPersonComments = objreader["PSSR_ChairPersonComments"].ToString();
                        getCreatepssr.OperationHeadComments = objreader["PSSR_OperationLeadComments"].ToString();
                        getCreatepssr.HSELeadComments = objreader["PSSR_HSELeadComments"].ToString();
                        getCreatepssr.EnggLeadComments = objreader["PSSR_EnggLeadComments"].ToString();
                        getCreatepssr.PSSRLeadComments = objreader["PSSR_LeadComments"].ToString();

                        getCreatepssr.ChairPersonDateTime = objreader["PSSR_ChairPersonDatetime"].ToString();
                        getCreatepssr.OperationHeadDateTime = objreader["PSSR_OperationLeadDatetime"].ToString();
                        getCreatepssr.HSELeadDateTime = objreader["PSSR_HSELeadDatetime"].ToString();
                        getCreatepssr.EnggLeadDateTime = objreader["PSSR_EnggLeadDatetime"].ToString();
                        getCreatepssr.PSSRLeadDateTime = objreader["PSSR_LeadDatetime"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return getCreatepssr;
        }


        public void PSSRAssignTeamInsert(int PSSRID, int Coordinator, int ChairPerson, int OL, int HSELead, int EnggLead, int PSSRLead, string MemberList, int UserID)
        {
            int affect = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {

                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[PSSRAssignTeamInsert]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@PSSRID", PSSRID);
                    // objCom.Parameters.AddWithValue("@Coordinator", Coordinator);
                    objCom.Parameters.AddWithValue("@ChairPerson", ChairPerson);
                    objCom.Parameters.AddWithValue("@OperationLead", OL);
                    objCom.Parameters.AddWithValue("@HSELead", HSELead);
                    objCom.Parameters.AddWithValue("@EnggLead", EnggLead);
                    objCom.Parameters.AddWithValue("@PSSRLead", PSSRLead);
                    objCom.Parameters.AddWithValue("@TeamMembers", MemberList);
                    objCom.Parameters.AddWithValue("@UserID", UserID);

                    objCom.Connection = objCon;
                    objCon.Open();
                    affect = objCom.ExecuteNonQuery();
                    objCon.Close();
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }

        }
        public AssignTeamViewModel GetPSSRAssign(int PSSRID)
        {
            AssignTeamViewModel assignTeam = new AssignTeamViewModel();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {

                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[GetPSSRAssignTeam]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@PSSRID", PSSRID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    objCom.Connection = objCon;
                    var objreader = objCom.ExecuteReader();
                    while (objreader.Read())
                    {
                        assignTeam.PSSRID = int.Parse(objreader["PSSRID"].ToString());
                        if (objreader["Coordinator"] != DBNull.Value)
                        {
                            assignTeam.Coordinator = int.Parse(objreader["Coordinator"].ToString());
                        }
                        assignTeam.ChairPerson = int.Parse(objreader["ChairPerson"].ToString());
                        assignTeam.ChairPersonName = objreader["ChairPersonName"].ToString();

                        assignTeam.HSELead = int.Parse(objreader["HSELead"].ToString());
                        assignTeam.HSELeadName = objreader["HSELeadName"].ToString();

                        assignTeam.OperationLead = int.Parse(objreader["OperationLead"].ToString());
                        assignTeam.OperationLeadName = objreader["OperationLeadName"].ToString();

                        assignTeam.EnggLead = int.Parse(objreader["EngineeringLead"].ToString());
                        assignTeam.EnggLeadName = objreader["EnggLeadName"].ToString();

                        assignTeam.PSSRLead = int.Parse(objreader["PSSRLead"].ToString());
                        assignTeam.PSSRLeadName = objreader["PSSRLeadName"].ToString();


                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return assignTeam;

        }
        public List<AssignTeamViewModel> GetAllPSSRAssign()
        {
            List<AssignTeamViewModel> assignTeamList = new List<AssignTeamViewModel>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {

                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[PSSRGetAllAssignTeam]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    objCom.Connection = objCon;
                    var objreader = objCom.ExecuteReader();
                    while (objreader.Read())
                    {
                        AssignTeamViewModel assignTeam = new AssignTeamViewModel();

                        assignTeam.PSSRID = int.Parse(objreader["PSSRID"].ToString());
                        if (objreader["Coordinator"] != DBNull.Value)
                        {
                            assignTeam.Coordinator = int.Parse(objreader["Coordinator"].ToString());
                        }
                        assignTeam.ChairPerson = int.Parse(objreader["ChairPerson"].ToString());
                        assignTeam.ChairPersonName = objreader["ChairPersonName"].ToString();
                        assignTeam.ChairPersonComments = objreader["PSSR_ChairPersonComments"].ToString();

                        assignTeam.HSELead = int.Parse(objreader["HSELead"].ToString());
                        assignTeam.HSELeadName = objreader["HSELeadName"].ToString();
                        assignTeam.HSELeadComments = objreader["PSSR_HSELeadComments"].ToString();

                        assignTeam.OperationLead = int.Parse(objreader["OperationLead"].ToString());
                        assignTeam.OperationLeadName = objreader["OperationLeadName"].ToString();
                        assignTeam.OperationHeadComments = objreader["PSSR_OperationLeadComments"].ToString();

                        assignTeam.EnggLead = int.Parse(objreader["EngineeringLead"].ToString());
                        assignTeam.EnggLeadName = objreader["EnggLeadName"].ToString();
                        assignTeam.EnggLeadComments = objreader["PSSR_EnggLeadComments"].ToString();

                        assignTeam.PSSRLead = int.Parse(objreader["PSSRLead"].ToString());
                        assignTeam.PSSRLeadName = objreader["PSSRLeadName"].ToString();
                        assignTeam.PSSRLeadComments = objreader["PSSR_LeadComments"].ToString();

                        assignTeamList.Add(assignTeam);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return assignTeamList;

        }




        public List<AllPSSR_Observation> GetAllPSSRObservation()
        {
            int affect = 1;
            List<AllPSSR_Observation> observationlist = new List<AllPSSR_Observation>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {

                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[GetAllPSSRObservation]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    objCom.Connection = objCon;
                    var objreader = objCom.ExecuteReader();
                    while (objreader.Read())
                    {
                        AllPSSR_Observation observation = new AllPSSR_Observation();
                        observation.SNO = affect++;
                        observation.PSSRID = int.Parse(objreader["PSSRID"].ToString());
                        observation.RecommID = int.Parse(objreader["RecomID"].ToString());
                        if (objreader["Recommendation"] != DBNull.Value)
                        {
                            observation.RecommText = objreader["Recommendation"].ToString();
                        }
                        if (objreader["TargetDate"] != DBNull.Value)
                        {
                            observation.TargetDate = objreader["TargetDate"].ToString();
                        }
                        if (objreader["CompletedDate"] != DBNull.Value)
                        {
                            observation.CompletedDate = objreader["CompletedDate"].ToString();
                        }
                        if (objreader["ActionBy"] != DBNull.Value)
                        {
                            observation.ActionByName = objreader["ActionBy"].ToString();
                        }
                        if (objreader["PlantName"] != DBNull.Value)
                        {
                            observation.PlantName = objreader["PlantName"].ToString();
                        }
                        if (objreader["PSSR_CategoryName"] != DBNull.Value)
                        {
                            observation.CategoryName = objreader["PSSR_CategoryName"].ToString();
                        }
                        if (objreader["Priority"] != DBNull.Value)
                        {
                            observation.PriorityName = objreader["Priority"].ToString();
                        }
                        if (objreader["Recomstatus"] != DBNull.Value)
                        {
                            observation.RecommStatusName = objreader["Recomstatus"].ToString();
                        }
                        if (objreader["ActionTaken"] != DBNull.Value)
                        {
                            observation.ActionTaken = objreader["ActionTaken"].ToString();
                        }
                        if (objreader["Attachment"] != DBNull.Value)
                        {
                            observation.AttachmentName = objreader["Attachment"].ToString();
                        }
                        if (objreader["RequestIdentity"] != DBNull.Value)
                        {
                            observation.RequestIdentity = int.Parse(objreader["RequestIdentity"].ToString());
                        }
                        observationlist.Add(observation);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return observationlist;
        }
        public List<AllPSSR_Observation> SearchPSSRObservation(SearchPSSRObservation searchModel)
        {
            int affect = 1;
            List<AllPSSR_Observation> observationlist = new List<AllPSSR_Observation>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {

                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[PSSR_SearchALLOB]";
                    objCom.CommandType = CommandType.StoredProcedure;

                    objCom.Parameters.AddWithValue("@FromDate", searchModel.FromDate == null ? string.Empty : searchModel.FromDate);
                    objCom.Parameters.AddWithValue("@ToDate", searchModel.ToDate == null ? string.Empty : searchModel.ToDate);
                    objCom.Parameters.AddWithValue("@PlantArea", searchModel.Plant);
                    objCom.Parameters.AddWithValue("@Category", searchModel.Category);
                    objCom.Parameters.AddWithValue("@Priority", searchModel.Priority);
                    objCom.Parameters.AddWithValue("@ActionBy", searchModel.ActionBy);

                    objCom.Connection = objCon;
                    objCon.Open();
                    objCom.Connection = objCon;
                    var objreader = objCom.ExecuteReader();
                    while (objreader.Read())
                    {
                        AllPSSR_Observation observation = new AllPSSR_Observation();
                        observation.SNO = affect++;
                        observation.PSSRID = int.Parse(objreader["PSSRID"].ToString());
                        observation.RecommID = int.Parse(objreader["RecomID"].ToString());
                        if (objreader["Recommendation"] != DBNull.Value)
                        {
                            observation.RecommText = objreader["Recommendation"].ToString();
                        }
                        if (objreader["TargetDate"] != DBNull.Value)
                        {
                            observation.TargetDate = objreader["TargetDate"].ToString();
                        }
                        if (objreader["CompletedDate"] != DBNull.Value)
                        {
                            observation.CompletedDate = objreader["CompletedDate"].ToString();
                        }
                        if (objreader["ActionBy"] != DBNull.Value)
                        {
                            observation.ActionByName = objreader["ActionBy"].ToString();
                        }
                        if (objreader["PlantName"] != DBNull.Value)
                        {
                            observation.PlantName = objreader["PlantName"].ToString();
                        }
                        if (objreader["PSSR_CategoryName"] != DBNull.Value)
                        {
                            observation.CategoryName = objreader["PSSR_CategoryName"].ToString();
                        }
                        if (objreader["Priority"] != DBNull.Value)
                        {
                            observation.PriorityName = objreader["Priority"].ToString();
                        }
                        if (objreader["Recomstatus"] != DBNull.Value)
                        {
                            observation.RecommStatusName = objreader["Recomstatus"].ToString();
                        }
                        if (objreader["ActionTaken"] != DBNull.Value)
                        {
                            observation.ActionTaken = objreader["ActionTaken"].ToString();
                        }
                        if (objreader["Attachment"] != DBNull.Value)
                        {
                            observation.AttachmentName = objreader["Attachment"].ToString();
                        }
                        if (objreader["RequestIdentity"] != DBNull.Value)
                        {
                            observation.RequestIdentity = int.Parse(objreader["RequestIdentity"].ToString());
                        }
                        observationlist.Add(observation);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return observationlist;
        }

        public int PSSRObservationInsertUpdate(PSSR_Observation createmodel)
        {
            int affect = 0;

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[PSSR_InsertObservations]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@PSSRID", createmodel.PSSRID);
                    objCom.Parameters.AddWithValue("@RecommID", createmodel.RecommID);
                    objCom.Parameters.AddWithValue("@Recomm", createmodel.RecommText);
                    objCom.Parameters.AddWithValue("@PSSRCategory", createmodel.PSSRCategory);
                    objCom.Parameters.AddWithValue("@Priority", createmodel.PriorityID);
                    objCom.Parameters.AddWithValue("@ActionBy", createmodel.ActionBy);
                    objCom.Parameters.AddWithValue("@TargetDate", DateTime.ParseExact(createmodel.TargetDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    if (!string.IsNullOrEmpty(createmodel.CompletedDate))
                    {
                        objCom.Parameters.AddWithValue("@CompletedDate", DateTime.ParseExact(createmodel.CompletedDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    }
                    //objCom.Parameters.AddWithValue("@CompletedDate",createmodel.CompletedDate != null? createmodel.CompletedDate:string.Empty);
                    objCom.Parameters.AddWithValue("@ActionTaken", createmodel.ActionTaken);
                    objCom.Parameters.AddWithValue("@Remarks", createmodel.Remarks);
                    objCom.Parameters.AddWithValue("@RecommStatus", createmodel.RecommStatus);
                    objCom.Parameters.AddWithValue("@UserID", createmodel.RecommUserID);
                    objCom.Parameters.AddWithValue("@filename", createmodel.PSSRObAttachmentName);

                    objCom.Connection = objCon;
                    objCon.Open();
                    affect = objCom.ExecuteNonQuery();



                    objCon.Close();
                }

            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return affect;
        }

        public void DeletePSSROBImage(int obid)
        {
            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[DeletePSSROBImage]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@OBID", obid);

                    objCom.Connection = objCon;
                    objCon.Open();
                    affectedRecordCount = objCom.ExecuteNonQuery();
                    objCon.Close();
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
        }
        public List<RecommStatusModel> RecommStatusListDD()
        {
            List<RecommStatusModel> List = new List<RecommStatusModel>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetCAPAObservationStatus]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        List.Add(new RecommStatusModel
                        {
                            RecommStatusID = int.Parse(reader["ID"].ToString()),
                            RecommStatusName = reader["Name"].ToString()
                        });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return List;
        }


        public List<AllPSSR_Observation> GetOBList(int PSSRID, int RecomID)
        {
            List<AllPSSR_Observation> OBList = new List<AllPSSR_Observation>();
            int affect = 1;
            try
            {

                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[PSSR_GETObservation]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@PSSRID", PSSRID);
                    objCom.Parameters.AddWithValue("@RecomID", RecomID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        OBList.Add(new AllPSSR_Observation
                        {
                            SNO = affect++,
                            PSSRID = int.Parse(reader["PSSRID"].ToString()),
                            RecommID = int.Parse(reader["RecomID"].ToString()),
                            RecommText = reader["Recommendation"].ToString(),
                            PriorityName = reader["PriorityName"].ToString(),
                            ActionByName = reader["ActionBy"].ToString(),
                            CompletedDate = reader["CompletedDate"].ToString(),
                            TargetDate = reader["TargetDate"].ToString(),
                            ActionTaken = reader["ActionTaken"].ToString(),
                            AttachmentName = reader["Attachment"].ToString()
                        });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return OBList;
        }

        public PSSR_Observation EditObservation(int RecommID)
        {
            PSSR_Observation EditOB = new PSSR_Observation();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[PSSR_ObservationEdit]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@RecommID", RecommID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {

                        EditOB.RecommID = int.Parse(reader["RecomID"].ToString());
                        EditOB.RecommText = reader["Recommendation"].ToString();
                        if (reader["PSSRCategory"] != DBNull.Value)
                        {
                            EditOB.PSSRCategory = int.Parse(reader["PSSRCategory"].ToString());
                        }
                        if (reader["Priority"] != DBNull.Value)
                        {
                            EditOB.PriorityID = int.Parse(reader["Priority"].ToString());
                        }
                        EditOB.TargetDate = reader["TargetDate"].ToString();
                        if (reader["ActionBy"] != DBNull.Value)
                        {
                            EditOB.ActionBy = int.Parse(reader["ActionBy"].ToString());
                        }
                        EditOB.CompletedDate = reader["CompletedDate"].ToString();
                        EditOB.ActionTaken = reader["ActionTaken"].ToString();
                        EditOB.Remarks = reader["Remarks"].ToString();
                        if (reader["RequestIdentity"] != DBNull.Value)
                        {
                            EditOB.RequestIdentity = int.Parse(reader["RequestIdentity"].ToString());

                        }

                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return EditOB;
        }
        public List<CheckListDD> GetCheckListDD()
        {
            List<CheckListDD> List = new List<CheckListDD>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[PSSR_GetCheckListDD]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        List.Add(new CheckListDD
                        {
                            PCMID = int.Parse(reader["PCMID"].ToString()),
                            Category = reader["Category"].ToString()
                        });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return List;
        }

        public List<CheckList> GetCheckList(int pcmID)
        {
            List<CheckList> List = new List<CheckList>();
            int sno = 1;
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[PSSR_GetCheckList]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@PCMID", pcmID);

                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        CheckList ch = new CheckList();
                        ch.SNO = sno++;
                        ch.PCMID = int.Parse(reader["PCMID"].ToString());
                        ch.CheckListID = int.Parse(reader["ChID"].ToString());
                        ch.Description = reader["Description"].ToString();


                        List.Add(ch);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return List;
        }

        public List<CheckListDD> GetAssignedChecklist(int PSSRID)
        {
            List<CheckListDD> checkLists = new List<CheckListDD>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[PSSR_GetAssignedChecklist]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@PSSRID", PSSRID);

                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();
                    while (reader.Read())
                    {
                        CheckListDD check = new CheckListDD();
                        check.PCMID = int.Parse(reader["PCMID"].ToString());
                        check.Category = reader["ChecklistMasterDescription"].ToString();
                        check.yesNo = int.Parse(reader["ChBox"].ToString()) > 0 ? true : false;
                        checkLists.Add(check);
                    }
                }

            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return checkLists;
        }

        public List<CheckList> PSSRSaveCheckList(int pcmID, int PSSRID)
        {
            List<CheckList> List = new List<CheckList>();
            int sno = 1;
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[PSSR_SaveCheckList]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@PCMID", pcmID);
                    objCom.Parameters.AddWithValue("@PSSRID", PSSRID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        CheckList ch = new CheckList();
                        ch.SNO = sno++;
                        ch.PCMID = int.Parse(reader["PCMID"].ToString());
                        ch.CheckListID = int.Parse(reader["ChID"].ToString());
                        ch.Description = reader["Description"].ToString();
                        ch.Consequences = reader["Consequences"].ToString();
                        ch.Remarks = reader["Remarks"].ToString();
                        ch.EditedBy = reader["EditedBy"].ToString();
                        ch.EditedDateTime = reader["EditedDatetime"].ToString();
                        if (reader["Status"] != DBNull.Value)
                        {
                            ch.Ischecked = int.Parse(reader["Status"].ToString());
                        }

                        List.Add(ch);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return List;
        }
        public List<CheckList> GetOverallCheckList(int PSSRID)
        {
            List<CheckList> List = new List<CheckList>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[PSSR_GetOverallCheckList]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@PSSRID", PSSRID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        CheckList ch = new CheckList();
                        ch.PSSRID = int.Parse(reader["PSSR_ID"].ToString());
                        ch.PCMID = int.Parse(reader["PCMID"].ToString());
                        ch.Ischecked = int.Parse(reader["Status"].ToString());
                        ch.CheckListID = int.Parse(reader["ChID"].ToString());

                        List.Add(ch);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return List;
        }
        public int CheckListInsert(MainCheckListModel checklistModel)
        {
            string planDataString = string.Empty;
            int affectedRecordCount = 0;
            int PCMID = 0;

            using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
            {
                SqlCommand objCom = new SqlCommand();
                List<CheckListSaveXML> CheckListXML = new List<CheckListSaveXML>();
                foreach (var list in checklistModel.CheckLists)
                {

                    PCMID = list.PCMID;
                    var list1 = new CheckListSaveXML
                    {
                        PCMID = list.PCMID,
                        ChID = list.CheckListID,
                        Status = list.Ischecked,
                        Description = list.Description,
                        Consequences = list.Consequences,
                        Remarks = list.Remarks,
                        EditedBy = list.EditedBy,
                        EditedDatetime = list.EditedDateTime


                    };
                    CheckListXML.Add(list1);


                }

                XmlSerializer xmlSerializer = new XmlSerializer(CheckListXML.GetType());

                using (StringWriter textWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(textWriter, CheckListXML);
                    planDataString = textWriter.ToString();
                }
                objCom.CommandText = "[PSSR_CheckListInsert]";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@ChecklistData", planDataString);
                // objCom.Parameters.AddWithValue("@UserId", checklistModel.UserID);
                objCom.Parameters.AddWithValue("@PSSRID", checklistModel.PSSRID);
                objCom.Parameters.AddWithValue("@PCMID", PCMID);
                objCom.Connection = objCon;
                objCon.Open();
                affectedRecordCount = objCom.ExecuteNonQuery();
                objCon.Close();
                objCom.Parameters.Clear();

                if (checklistModel.CheckLists != null)
                {
                    var list = checklistModel.CheckLists.Where(x => x.Ischecked == 2).ToList();
                    foreach (var item1 in list)
                    {
                        objCom.CommandText = "PSSRAutoRecommendationInsert";
                        objCom.Parameters.AddWithValue("@checklistname", item1.Description);
                        objCom.Parameters.AddWithValue("@UserID", checklistModel.UserID);
                        objCom.Parameters.AddWithValue("@PSSRID", checklistModel.PSSRID);
                        objCom.Connection = objCon;
                        objCon.Open();
                        affectedRecordCount = objCom.ExecuteNonQuery();
                        objCon.Close();
                        objCom.Parameters.Clear();
                    }
                }

            }

            return affectedRecordCount;
        }
        public void UpdatePSSRStatus(int PSSRID, int StatusID, string Comments, int userID)
        {
            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[UpdatePSSRStatus]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@PSSRID", PSSRID);
                    objCom.Parameters.AddWithValue("@StatusID", StatusID);
                    objCom.Parameters.AddWithValue("@Comments", Comments);
                    objCom.Parameters.AddWithValue("@UserID", userID);

                    objCom.Connection = objCon;
                    objCon.Open();
                    affectedRecordCount = objCom.ExecuteNonQuery();
                    objCon.Close();
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
        }

        public void UpdateTargetDateRequest(int Recomid, int identity, string Comments = null, string RevisedTargetDate = null)
        {
            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[PSSR_UpdateRecomRequestTargetDate]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@RecomID", Recomid);
                    objCom.Parameters.AddWithValue("@RequestIdentity", identity);
                    objCom.Parameters.AddWithValue("@Comments", Comments);
                    if (RevisedTargetDate != null)
                    {
                        objCom.Parameters.AddWithValue("@RevisedTargetDate", DateTime.ParseExact(RevisedTargetDate,
                                "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        objCom.Parameters.AddWithValue("@RevisedTargetDate", DBNull.Value);
                    }
                    objCom.Connection = objCon;
                    objCon.Open();
                    affectedRecordCount = objCom.ExecuteNonQuery();
                    objCon.Close();
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
        }

        public TargetDateApprove GetRequestedTargetDateDetails(int Recommid)
        {
            TargetDateApprove targetdetatils = new TargetDateApprove();
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "PSSR_GetRequestTargetDateDetails";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RecommID", Recommid);
                    cmd.Connection = objCon;
                    objCon.Open();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        targetdetatils.RequestTargetDate = reader["RequestedTargetDate"].ToString();
                        targetdetatils.RequestRemarks = reader["RequestedComments"].ToString();
                        targetdetatils.RecomID = int.Parse(reader["RecomID"].ToString());
                        targetdetatils.ExsistingTargetDate = reader["ExsistingTargetDate"].ToString();
                    }
                    objCon.Close();
                }

            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);

            }
            return targetdetatils;
        }

        public List<PSSRHistoryViewModel> GetAllPSSRHistoryList()
        {
            List<PSSRHistoryViewModel> HistoryList = new List<PSSRHistoryViewModel>();
            int affect = 1;
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {

                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[GetAllPSSRHistoryList]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    objCom.Connection = objCon;
                    var objreader = objCom.ExecuteReader();
                    while (objreader.Read())
                    {
                        PSSRHistoryViewModel HistoryView = new PSSRHistoryViewModel();
                        HistoryView.SNO = affect++;
                        HistoryView.PSSRID = int.Parse(objreader["PSSR_ID"].ToString());
                        HistoryView.Plant = objreader["PlantName"].ToString();
                        HistoryView.ScheduledDate = objreader["ScheduledDatetime"].ToString();
                        HistoryView.SystemDesc = objreader["System_Description"].ToString();
                        HistoryView.PSSRType = objreader["PSSR_Type"].ToString();
                        HistoryView.ClosedDate = objreader["ClosureDateTime"].ToString();
                        HistoryView.PSSRLead = objreader["PSSRLead"].ToString();
                        HistoryView.Attachment = objreader["Attachments"].ToString();
                        HistoryList.Add(HistoryView);
                    }
                    objCon.Close();
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return HistoryList;
        }


        public List<PSSRHistoryViewModel> SearchPSSRHistoryList(PSSRSearchHistory searchModel)
        {
            List<PSSRHistoryViewModel> PSSRList = new List<PSSRHistoryViewModel>();
            int affect = 1;
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {

                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[PSSR_SearchHistoryList]";
                    objCom.CommandType = CommandType.StoredProcedure;

                    objCom.Parameters.AddWithValue("@FromDate", searchModel.FromDate == null ? string.Empty : searchModel.FromDate);
                    objCom.Parameters.AddWithValue("@ToDate", searchModel.ToDate == null ? string.Empty : searchModel.ToDate);
                    objCom.Parameters.AddWithValue("@PlantArea", searchModel.PlantID);
                    objCom.Parameters.AddWithValue("@PSSR_Type", searchModel.PSSRType);

                    objCom.Connection = objCon;
                    objCon.Open();

                    var objreader = objCom.ExecuteReader();
                    while (objreader.Read())
                    {
                        PSSRHistoryViewModel pSSRListView = new PSSRHistoryViewModel();
                        pSSRListView.SNO = affect++;
                        pSSRListView.PSSRID = int.Parse(objreader["PSSR_ID"].ToString());
                        pSSRListView.Plant = objreader["PlantName"].ToString();
                        pSSRListView.SystemDesc = objreader["System_Description"].ToString();
                        pSSRListView.PSSRType = objreader["PSSR_Type"].ToString();
                        pSSRListView.ScheduledDate = objreader["ScheduledDatetime"].ToString();
                        pSSRListView.ClosedDate = objreader["ClosureDateTime"].ToString();
                        pSSRListView.PSSRLead = objreader["PSSRLead"].ToString();
                        pSSRListView.Attachment = objreader["Attachments"].ToString();
                        PSSRList.Add(pSSRListView);
                    }
                    objCon.Close();
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return PSSRList;
        }






    }
}
