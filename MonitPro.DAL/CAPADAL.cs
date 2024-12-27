using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;
using MonitPro.Models.Incident;
using MonitPro.Models.Account;
using MonitPro.Models.IncidentViewModels;
using MonitPro.Common.Library;
using System.Web.Mvc;
using System.Xml.Serialization;
using IncidentReportSystem.Models;
using MonitPro.Models.CAPA;
using MonitPro.Models.CAPAViewModel;
using MonitPro.Models;
using System.Web.Management;
using System.Runtime.Remoting.Messaging;

namespace MonitPro.DAL
{
    public class CAPADAL
    {
        string constring = AppConfig.ConnectionString;
        SqlCommand objCom;
        SqlDataReader reader;

        public List<AuditType> GetAuditType()
        {
            List<AuditType> AuditTypeList = new List<AuditType>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetAuditType]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    if (reader.HasRows)
                    {
                        AuditTypeList.Add(new AuditType { ID = -1, Name = "--Select Audit Type--" });
                        AuditTypeList.Add(new AuditType { ID = 0, Name = "All" });
                       
                    }

                    while (reader.Read())
                    {
                        AuditTypeList.Add(new AuditType { ID = int.Parse(reader["ID"].ToString()), Name = reader["Name"].ToString(), Description = reader["Description"].ToString() });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return AuditTypeList;
        }
        public List<CAPAPlants> GetcapaPlants()
        {
            List<CAPAPlants> obPlantsList = new List<CAPAPlants>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetCAPAPlants]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    if (reader.HasRows)
                    {

                        obPlantsList.Add(new CAPAPlants { ID = -1, Name = "All" });
                    }

                    while (reader.Read())
                    {
                        obPlantsList.Add(new CAPAPlants { ID = int.Parse(reader["ID"].ToString()), Name = reader["Name"].ToString(), Description = reader["Description"].ToString() });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return obPlantsList;
        }

        public List<CAPASource> GetAuditCAPAsource(int? AuditID)
        {
            List<CAPASource> sourcelist = new List<CAPASource>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "GetCAPASourceSelect";

                    if (AuditID > 0)
                        objCom.Parameters.AddWithValue("@AuditID", AuditID);
                    else
                        objCom.Parameters.AddWithValue("@AuditID", DBNull.Value);

                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();

                    SqlDataReader Results = objCom.ExecuteReader();

                    while (Results.Read())
                    {
                        CAPASource cs = new CAPASource();
                        cs.AuditCSID = Results[0].ToString();
                        cs.Name = Results[1].ToString();
                        sourcelist.Add(cs);
                    }
                    objCon.Close();
                    Results.Close();
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return sourcelist;

        }
        public List<ActionerModel> GetAllCAPAObservations()
        {
            List<ActionerModel> Action = new List<ActionerModel>();
            try
            {
                int RecordCount = 1;
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetAllCAPAObservations]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        var Action1 = new ActionerModel();

                        Action1.CAPAID = int.Parse(reader["ID"].ToString());

                        if (reader["CompletedBy"] != DBNull.Value)
                        {
                            Action1.CompletedBy = int.Parse(reader["CompletedBy"].ToString());
                        }
                        if (reader["DeptManager"] != DBNull.Value)
                        {
                            Action1.DeptManager = int.Parse(reader["DeptManager"].ToString());
                        }
                        Action.Add(Action1);

                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return Action;
        }
        public List<CAPASource> GetCAPASource()
        {
            List<CAPASource> CAPASourceList = new List<CAPASource>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetCAPASource]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    if (reader.HasRows)
                    {

                        CAPASourceList.Add(new CAPASource { ID = 0, Name = "All" });
                    }

                    while (reader.Read())
                    {
                        CAPASourceList.Add(new CAPASource { ID = int.Parse(reader["ID"].ToString()), Name = reader["Name"].ToString(), Description = reader["Description"].ToString() });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return CAPASourceList;
        }
        #region "get Last month ObservationCount chart"
        public List<ObservationCount> GetLastMonthObservationCount()
        {
            List<ObservationCount> observationcounts = new List<ObservationCount>();

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetLastMonthObservationsCount]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        ObservationCount observationcount = new ObservationCount()
                        {
                            MonthName = reader["ObMonth"].ToString(),

                            MonthlyCount = int.Parse(reader["TotalCount"].ToString())

                        };

                        observationcounts.Add(observationcount);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return observationcounts;
        }
        #endregion

        #region "get Last month CapaSourceCount chart"
        public List<CapaSourceCounts> GetLastMonthCapaSourceCount()
        {
            List<CapaSourceCounts> Capasourcecount = new List<CapaSourceCounts>();

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetLastMonthCAPASourceCounttemp]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        CapaSourceCounts actionCount = new CapaSourceCounts()
                        {
                            SourceName = reader["CapaSourceName"].ToString(),

                            TotalCount = int.Parse(reader["TotalCount"].ToString())


                        };

                        Capasourcecount.Add(actionCount);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return Capasourcecount;
        }
        #endregion

        #region "get Last month CategoryCount chart"
        public List<CategoryCount> GetLastMonthCategoryCount()
        {
            List<CategoryCount> Counts = new List<CategoryCount>();

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetLastMonthCAPACategoryCount]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        CategoryCount actionCount = new CategoryCount()
                        {
                            SourceName = reader["CategoryName"].ToString(),

                            TotalCount = int.Parse(reader["TotalCount"].ToString())


                        };

                        Counts.Add(actionCount);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return Counts;
        }
        #endregion

        #region "get Last month ActionStatusCount chart"
        public List<ActionsCount> GetLastMonthActionStatusCount()
        {
            List<ActionsCount> ActionCounts = new List<ActionsCount>();

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetLastMonthCAPAActionsCount]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        ActionsCount actionCount = new ActionsCount()
                        {
                            StatusName = reader["Actions"].ToString(),
                            TotalCount = int.Parse(reader["ActionsCount"].ToString())
                        };
                      
                            ActionCounts.Add(actionCount);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return ActionCounts;
        }
        #endregion

        #region "get Last month CapaPriorityCount chart"
        public List<PriorityCount> GetLastMonthCapaPriorityCount()
        {
            List<PriorityCount> prioritycount = new List<PriorityCount>();

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetLastMonthCAPAPriorityCount]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        PriorityCount actionCount = new PriorityCount()
                        {
                            PriorityName = reader["Name"].ToString(),
                            Overdue = int.Parse(reader["Overdue"].ToString()),
                            Closed = int.Parse(reader["Closed"].ToString()),
                            Opened = int.Parse(reader["Opened"].ToString()),
                           // ReOpen = int.Parse(reader["ReOpen"].ToString()),
                            New = int.Parse(reader["New"].ToString()),


                        };
                        if ((actionCount.Overdue > 0) || (actionCount.Closed > 0) || (actionCount.Opened > 0) || (actionCount.New > 0))
                            prioritycount.Add(actionCount);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return prioritycount;
        }
        #endregion

        #region GetCapaFunctionalManagerCount

        public List<PriorityCount> GetLastMonthCapaFunctionalManagerCount()
        {
            List<PriorityCount> prioritieslist = new List<PriorityCount>();

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[prGetLastMonthCapaFunctionalManagerCount]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        PriorityCount Count = new PriorityCount()
                        {
                            Name = reader["FunctionalManager"].ToString(),
                            Overdue = int.Parse(reader["overdue"].ToString()),
                            Closed = int.Parse(reader["closed"].ToString()),
                            Opened = int.Parse(reader["opened"].ToString()),
                            //ReOpen = int.Parse(reader["ReOpen"].ToString()),
                            New = int.Parse(reader["New"].ToString()),

                        };
                        if ((Count.Overdue > 0) || (Count.Closed > 0) || (Count.Opened > 0)  || (Count.New > 0))
                            prioritieslist.Add(Count);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return prioritieslist;
        }
        #endregion

        public List<CAPACategory> GetCAPACategory()
        {
            List<CAPACategory> capacategory = new List<CAPACategory>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetCAPACategory]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    if (reader.HasRows)
                    {
                        capacategory.Add(new CAPACategory { ID = 0, Name = "All" });
                        //capacategory.Add(new CAPACategory { ID = 0, Name = "--Select--" });

                    }

                    while (reader.Read())
                    {
                        capacategory.Add(new CAPACategory { ID = int.Parse(reader["ID"].ToString()), Name = reader["Name"].ToString(), Description = reader["Description"].ToString() });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return capacategory;
        }

        public List<CAPAPriority> GetCAPAPriority()
        {
            List<CAPAPriority> capapriority = new List<CAPAPriority>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetCAPAPriority]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();
                    if (reader.HasRows)
                    {
                        capapriority.Add(new CAPAPriority { ID = 0, Name = "All" });

                    }


                    while (reader.Read())
                    {
                        capapriority.Add(new CAPAPriority { ID = int.Parse(reader["ID"].ToString()), Name = reader["Name"].ToString(), Description = reader["Description"].ToString() });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return capapriority;
        }


        public List<CAPAObservationStatus> GetCAPAObservationStatus()
        {
            List<CAPAObservationStatus> CAPAObservationList = new List<CAPAObservationStatus>();
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

                    if (reader.HasRows)
                    {
                       
                        CAPAObservationList.Add(new CAPAObservationStatus { ID = 0, Name = "All" });
                    }

                    while (reader.Read())
                    {
                        CAPAObservationList.Add(new CAPAObservationStatus { ID = int.Parse(reader["ID"].ToString()), Name = reader["Name"].ToString(), Description = reader["Description"].ToString() });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return CAPAObservationList;
        }
   public List<Plants> UpdateAreaOwnerDAL(string PlantID, string AreaOwnerID)
     {
     List<Plants> PlantsList = new List<Plants>();
     try
     {
         using (SqlConnection objCon = new SqlConnection(constring))
         {
             objCom = new SqlCommand();
             objCom.CommandText = "[UpdateAreaOwner]";
             objCom.CommandType = CommandType.StoredProcedure;
             objCom.Parameters.AddWithValue("@PlantID", PlantID);
             objCom.Parameters.AddWithValue("@AreaOwnerID", AreaOwnerID);
             objCom.Connection = objCon;
             objCon.Open();
             reader = objCom.ExecuteReader();

         }
     }
     catch (Exception exception)
     {
         LogManager.Instance.Error(exception);
         throw new Exception(exception.Message);
     }

     return PlantsList;
 }


        public CreateCAPA GetCAPADetails(int capaID)
        {
            CreateCAPA createcapa = new CreateCAPA();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetCAPADetails]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@capaID", capaID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        createcapa.CAPAID = int.Parse(reader["ID"].ToString());
                        createcapa.CAPANumber = reader["CAPANumber"].ToString();
                        createcapa.Description = reader["Description"].ToString();
                        createcapa.AuditTime = reader["CAPADateTime"].ToString();
                        createcapa.CAPAPlantID = int.Parse(reader["PlantID"].ToString());
                        createcapa.AuditTypeID = int.Parse(reader["AuditTypeID"].ToString());
                        createcapa.CAPASourceID = int.Parse(reader["CAPASourceID"].ToString());
                        createcapa.ContractorEmpID = int.Parse(reader["ReportedBy"].ToString());
                        createcapa.ReportedDetail = reader["ReportedDetail"].ToString();
                        createcapa.StatusID = int.Parse(reader["StatusID"].ToString());
                        createcapa.FileName = reader["Attachments"].ToString();
                        createcapa.CreatedBy = int.Parse(reader["CreatedBy"].ToString());
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return createcapa;
        }
        public CreateCAPA GetMyCAPADetails(int ObservID)
        {
            CreateCAPA createcapa = new CreateCAPA();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetMyCAPADetails]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@ObservID", ObservID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        createcapa.CAPAID = int.Parse(reader["CAPAID"].ToString());
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return createcapa;
        }
        public List<CAPADetails> GetCAPAMailDetails(int capaID)
        {
            // Logic to fetch CAPA details from your database
            // This can be from your database context or a service layer
            // Just return mock data for now
            List<CAPADetails> details = new List<CAPADetails>();
            try
            {
                SqlConnection con = new SqlConnection(constring);

                SqlCommand cmd = new SqlCommand();

                cmd.CommandText = "ReminderMailingforCAPAActioner";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = con;
                con.Open();

                var emailsActionercapa = new List<CAPADetails>();
                var functionalmgr = new List<CAPADetails>();


                using (SqlDataReader reader = cmd.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        var temp = reader["OBID"];
                        if (reader["OBID"] != DBNull.Value)
                        {
                            emailsActionercapa.Add(new CAPADetails()
                            {

                                RecomID = int.Parse(reader["OBID"].ToString()),

                                Actioner = reader["ActionerEmail"].ToString(),
                                FunctionalManager = reader["FunctionalManager"].ToString(),
                                List = Mystatus(int.Parse(reader["Actioner"].ToString())), // Ensure Actioner exists
                                //CAPAAdvisor = reader["CAPAAdvisor"].ToString(),
                                //PlantHead = reader["PlantHead"].ToString(),
                                //Director = reader["Director"].ToString()

                            });
                        }
                    }

                    // Move to the second result set
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            functionalmgr.Add(new CAPADetails()
                            {
                                /*FactoryName1 = reader["FactoryName"]?.ToString(), */// Check here
                                RecomID = int.Parse(reader["OBID"].ToString()),
                                FunctionalManager = reader["FunctionalManager"]?.ToString(),
                                //CAPAAdvisor = reader["CAPAAdvisor"]?.ToString(),
                            });
                        }
                    }
                }
               con.Close();
               return emailsActionercapa;
            }
            catch (Exception objException)
            {
                throw objException;
            }

           
        }
        public List<MyActionStatus> Mystatus(int id)
        {
            List<MyActionStatus> MyList = new List<MyActionStatus>();
            try
            {
                SqlConnection con = new SqlConnection(constring);
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "MyActionforCAPAMail";
                cmd.Parameters.AddWithValue("@UserID", id);
                cmd.Connection = con;
                SqlDataReader reader = cmd.ExecuteReader();


                while (reader.Read())
                {
                    MyList.Add(new MyActionStatus
                    {
                        //FactoryName = reader["FactoryName"].ToString(),
                        CAPAID = int.Parse(reader["CAPAID"].ToString()),
                        Status = (reader["Status"].ToString() == "Y") ? "Pending" : "Overdue",
                        RecomID = int.Parse(reader["OBID"].ToString()),
                        Targetdate = reader["TargetDate"].ToString(),
                        Source = reader["Source"].ToString(),
                        Recommendation = reader["Recommendation"].ToString(),
                        Priority = reader["PriorityName"].ToString(),
                    });
                }

                con.Close();

            }
            catch (Exception exception)
            {

                throw exception;
            }

            return MyList;
        }

        public MyactionDashboardCount GetDashboardOverallCount()
        {
            MyactionDashboardCount dashcount = new MyactionDashboardCount();
            try
            {
                using(SqlConnection objCon = new SqlConnection(constring)) {
                    objCom = new SqlCommand();
                    objCom.CommandText = "DashBoard_OverAllCount";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();
                    while(reader.Read())
                    {
                        if (reader["IncidentCount"] != null)
                        {
                            dashcount.IncidentCount = int.Parse(reader["IncidentCount"].ToString());
                        }
                        dashcount.InciPlant = reader["Plant"].ToString();
                        dashcount.InciClassfication = reader["IncidentClass"].ToString();

                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        if (reader["CAPACount"] != null)
                        {
                            dashcount.CapaOVerdueCount = int.Parse(reader["CAPACount"].ToString());

                        }
                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        if (reader["CategoryACount"] != null)
                        {
                            dashcount.PSSRCategoryACount = int.Parse(reader["CategoryACount"].ToString());
                        }
                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        if (reader["ScheduledCount"] != null)
                        {
                            dashcount.PSSRScheduledCount = int.Parse(reader["ScheduledCount"].ToString());
                        }
                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        if (reader["T1"] != null)
                        {
                            dashcount.T1 = int.Parse(reader["T1"].ToString());
                        }
                        if (reader["T2"] != null)
                        {
                            dashcount.T2 = int.Parse(reader["T2"].ToString());
                        }
                        if (reader["T3"] != null) {
                            dashcount.T3 = int.Parse(reader["T3"].ToString());
                        }
                        if (reader["T4"] != null)
                        {
                            dashcount.T4 = int.Parse(reader["T4"].ToString());
                        }
                        if (reader["Near_Miss"] != null)
                        {
                            dashcount.Near_Miss = int.Parse(reader["Near_Miss"].ToString());
                        }
                        if (reader["TotalCount"] != null)
                        {
                            dashcount.TotalIncident = int.Parse(reader["TotalCount"].ToString());
                        }
                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        if (reader["TotalMOCCount"] != null)
                        {
                            dashcount.MOCTotalCount = int.Parse(reader["TotalMOCCount"].ToString());
                        }
                        if (reader["Permanent"] != null)
                        {
                            dashcount.PermanentCount = int.Parse(reader["Permanent"].ToString());
                        }
                        if (reader["Temporary"] != null)
                        {
                            dashcount.TemporaryCount = int.Parse(reader["Temporary"].ToString());
                        }
                    }
                    objCon.Close();
                }
            }
            catch(Exception ex) {

                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return dashcount;
        }
        public MyApprovalCount GetDashboardApprovalCount(int UserID)
        {
            MyApprovalCount approvalCount=new MyApprovalCount();
            try
            {
                using(SqlConnection conn = new SqlConnection(constring))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "Dashboard_OverallApprovalCount";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Connection = conn;
                    conn.Open();
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        approvalCount.IncidentApprovalCount = int.Parse(reader["IncidentApproveCount"].ToString());
                      }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        approvalCount.PSSRApprovalCount = int.Parse(reader["PSSRApproveCount"].ToString());

                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        approvalCount.MOCApprovalCount = int.Parse(reader["MOCApproveCount"].ToString());

                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        approvalCount.PermitApprovalCount = int.Parse(reader["Workpermitapprovecount"].ToString());

                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        approvalCount.CMSApprovalCount = int.Parse(reader["CMSApproveCount"].ToString());

                    }
                   
                    conn.Close();
                }
            }
            catch(Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return approvalCount;
        }
        public CreateCAPA GetMyInciDetails(int ObservID)
        {
            CreateCAPA createcapa = new CreateCAPA();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetMyInciDetails]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@ObservID", ObservID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {

                        createcapa.IncidentID = int.Parse(reader["InciID"].ToString());
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return createcapa;
        }
        public int CAPAUpdate(NewCAPAModel newcapa, string fileName, int currentuser)
        {
            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "CAPAInsert";
                    objCom.CommandType = CommandType.StoredProcedure;

                    objCom.Parameters.AddWithValue("@AuditDate", DateTime.ParseExact(newcapa.CreateCAPA.AuditTime, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    objCom.Parameters.AddWithValue("@AuditTypeID", newcapa.CreateCAPA.AuditTypeID);
                    objCom.Parameters.AddWithValue("@StatusID", newcapa.CreateCAPA.StatusID);
                    objCom.Parameters.AddWithValue("@CAPASourceID", newcapa.CreateCAPA.CAPASourceID);
                    objCom.Parameters.AddWithValue("@PlantID", newcapa.CreateCAPA.CAPAPlantID);
                    objCom.Parameters.AddWithValue("@ReportedBy", newcapa.CreateCAPA.ContractorEmpID);
                    objCom.Parameters.AddWithValue("@ReportedDetail", newcapa.CreateCAPA.ReportedDetail == null ? String.Empty : newcapa.CreateCAPA.ReportedDetail);
                    objCom.Parameters.AddWithValue("@FileName", fileName);
                    objCom.Parameters.AddWithValue("@UserID", currentuser);
                    objCom.Parameters.AddWithValue("@CAPAID", newcapa.CreateCAPA.CAPAID);
                    SqlParameter outPutParameter = new SqlParameter();
                    outPutParameter.ParameterName = "@NewcapaID";
                    outPutParameter.SqlDbType = System.Data.SqlDbType.Int;
                    outPutParameter.Direction = System.Data.ParameterDirection.Output;
                    objCom.Parameters.Add(outPutParameter);

                    objCom.Connection = objCon;
                    objCon.Open();
                    affectedRecordCount = objCom.ExecuteNonQuery();
                    newcapa.CreateCAPA.CAPAID = int.Parse(outPutParameter.Value.ToString());
                    objCon.Close();
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return affectedRecordCount;
        }

        public List<CAPAViewModel> GetOpenCapa()
        {
            List<CAPAViewModel> CAPAList = new List<CAPAViewModel>();
            try
            {
                int RecordCount = 1;
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetAllOpenCAPA]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {

                        CAPAList.Add(
                             new CAPAViewModel
                             {
                                 SNo = RecordCount++,
                                 CAPAID = int.Parse(reader["ID"].ToString()),
                                 CAPANumber = reader["CAPANumber"].ToString(),
                                 AuditTime = DateTime.Parse(reader["AuditDate"].ToString()),
                                 AuditType = reader["AuditName"].ToString(),
                                 CAPASource = reader["CapaSourceName"].ToString(),
                                 CurrentStatus = reader["StatusName"].ToString() == "Assigned" ? "Assigned" : "New",
                                 FileName = reader["Attachments"].ToString(),
                                 ActionTaken = reader["ActionTaken"].ToString(),
                                 CreatedBy = int.Parse(reader["CreatedBy"].ToString()),
                                 CreatedByName = reader["CreatedByName"].ToString()
                             });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return CAPAList;
        }

        public List<CAPAViewModel> SearchOpenCapa(CAPASearchViewModel capasearchviewmodel)
        {
            List<CAPAViewModel> capalist = new List<CAPAViewModel>();
            try
            {
                int RecordCount = 1;


                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[SearchOpenCAPA]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@CAPAStatus", capasearchviewmodel.CAPAStatus);
                    objCom.Parameters.AddWithValue("@FromDate", capasearchviewmodel.CAPAFromDate == null ? string.Empty : capasearchviewmodel.CAPAFromDate);
                    objCom.Parameters.AddWithValue("@ToDate", capasearchviewmodel.CAPAToDate == null ? string.Empty : capasearchviewmodel.CAPAToDate);
                    objCom.Parameters.AddWithValue("@AuditType", capasearchviewmodel.AuditType);
                    objCom.Parameters.AddWithValue("@CAPASource", capasearchviewmodel.CAPASource);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        capalist.Add(
                             new CAPAViewModel
                             {
                                 SNo = RecordCount++,
                                 CAPAID = int.Parse(reader["ID"].ToString()),
                                 CAPANumber = reader["CAPANumber"].ToString(),
                                 AuditType = reader["AuditName"].ToString(),
                                 AuditTime = DateTime.Parse(reader["AuditDate"].ToString()),
                                 CAPASource = reader["CAPASource"].ToString(),
                                 CurrentStatus = reader["StatusName"].ToString() == "Assigned" ? "Assigned" : "New",
                                 ClosedStatus = reader["ClosedStatus"].ToString(),
                                 FileName = reader["Attachments"].ToString(),
                                 ClosedTime = DateTime.Parse(reader["ClosedDate"].ToString()),
                                 ActionTaken = reader["ActionTaken"].ToString(),
                                 CreatedBy = int.Parse(reader["CreatedBy"].ToString()),
                                 CreatedByName = reader["CreatedByName"].ToString(),
                             });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return capalist;
        }

        public List<CAPAViewModel> GetAllClosedCapa()
        {
            List<CAPAViewModel> capalist = new List<CAPAViewModel>();
            try
            {
                int RecordCount = 1;
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetAllClosedCAPA]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        capalist.Add(
                            new CAPAViewModel
                            {
                                SNo = RecordCount++,
                                CAPAID = int.Parse(reader["ID"].ToString()),
                                CAPANumber = reader["CAPANumber"].ToString(),
                                AuditType = reader["AuditName"].ToString(),
                                AuditTime = DateTime.Parse(reader["AuditDate"].ToString()),
                                CAPASource = reader["CAPASourceName"].ToString(),
                                ClosedStatus = reader["StatusName"].ToString(),
                                FileName = reader["Attachments"].ToString(),
                                ClosedTime = DateTime.Parse(reader["ClosedDate"].ToString()),
                            });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return capalist;
        }

        public void UpdateCAPAStatus(int capaID, int StatusID, string Comments, int userID)
        {
            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "UpdateCAPAStatus";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@capaID", capaID);
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

        public List<ObservationViewModelCapa> GetObservations(int capaID, int OBID)
        {
            List<ObservationViewModelCapa> observationList = new List<ObservationViewModelCapa>();

            int SNo = 1;

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetCAPAObservations]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@CAPAID", capaID);
                    objCom.Parameters.AddWithValue("@ObservID", OBID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        var ObList = new ObservationViewModelCapa();
                        ObList.SNo = SNo++;
                        ObList.CAPAID = int.Parse(reader["CAPAID"].ToString());
                        ObList.CAPANumber = reader["CAPANumber"].ToString();

                        if (reader["OBID"] != DBNull.Value)
                        {
                            ObList.OBID = int.Parse(reader["OBID"].ToString());
                        }
                        if (reader["ID"] != DBNull.Value)
                        {
                            ObList.ObservationID = int.Parse(reader["ID"].ToString());
                        }
                        ObList.Observation = reader["Title"].ToString();
                        ObList.CompletedUser = reader["UserFullName"].ToString();
                        ObList.TargetDate = reader["TargetDate"].ToString();
                        ObList.Recommendation = reader["Recommendation"].ToString();
                        ObList.ActionTaken = reader["ActionTaken"].ToString();
                        ObList.ObPlantName = reader["ObPlantName"].ToString();
                        ObList.CurrentStatus = reader["Status"].ToString();
                        ObList.CompletedDate = reader["CompletedDate"].ToString();
                        ObList.CAPASourceName = reader["CAPASourceName"].ToString();
                        ObList.FunctionalMgr = reader["FunctionalMgr"].ToString();
                        ObList.PriorityName = reader["Priority"].ToString();  
                        if (reader["CompletedBy"] != DBNull.Value)
                        {
                            ObList.CompletedBy = int.Parse(reader["CompletedBy"].ToString());
                        }
                        if (reader["DeptManager"] != DBNull.Value)
                        {
                            ObList.DptID = int.Parse(reader["DeptManager"].ToString());
                        }
                        ObList.AtachmentName = reader["AttachmentName"].ToString();
                        observationList.Add(ObList);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return observationList;
        }
        public List<ObservationViewModelCapa> GetMyObservations(int ObservID)
        {
            List<ObservationViewModelCapa> observationList = new List<ObservationViewModelCapa>();

            int SNo = 1;

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetMyCAPAObservations]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@ObservID", ObservID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        var ObList = new ObservationViewModelCapa();
                        ObList.SNo = SNo++;
                        ObList.CAPAID = int.Parse(reader["CAPAID"].ToString());
                        if (reader["OBID"] != DBNull.Value)
                        {
                            ObList.OBID = int.Parse(reader["OBID"].ToString());
                        }
                        if (reader["ID"] != DBNull.Value)
                        {
                            ObList.ObservationID = int.Parse(reader["ID"].ToString());
                        }
                        ObList.Observation = reader["Title"].ToString();
                        ObList.CompletedUser = reader["UserFullName"].ToString();
                        ObList.TargetDate = reader["TargetDate"].ToString();
                        ObList.Recommendation = reader["Recommendation"].ToString();
                        ObList.ActionTaken = reader["ActionTaken"].ToString();
                        ObList.CompletedDate = reader["CompletedDate"].ToString();
                        ObList.PlantName = reader["PlantName"].ToString();
                        ObList.CAPASourceName = reader["CAPASourceName"].ToString();
                        if (reader["CompletedBy"] != DBNull.Value)
                        {
                            ObList.CompletedBy = int.Parse(reader["CompletedBy"].ToString());
                        }
                        observationList.Add(ObList);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return observationList;
        }
        public CAPAObservation GetCAPAPlantObservations(int capaID)
        {
            CAPAObservation capaobservation = new CAPAObservation();

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetCAPAPlantObservations]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@CAPAID", capaID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        capaobservation.CAPAID = int.Parse(reader["CAPAID"].ToString());
                        capaobservation.CAPANumber = reader["CAPANumber"].ToString();
                     
                        capaobservation.CAPASourceName = reader["CAPASourceName"].ToString();
                        capaobservation.CreatedBy = int.Parse(reader["CreatedBy"].ToString());
                        capaobservation.CompletedUser = reader["CAPACreatedby"].ToString();

                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return capaobservation;
        }

       
        public List<UserProfile> GetActionList()
        {
            using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
            {
                SqlCommand objCom = new SqlCommand();
                objCom.CommandText = "ActionUserList";
                objCom.CommandType = CommandType.StoredProcedure;
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();
                var sender = new List<UserProfile>();
                if (objReader.HasRows)
                {
                    sender.Add(new UserProfile { UserID = 0, DisplayUserName = "All" });
                }
                while (objReader.Read())
                {
                    sender.Add(new UserProfile
                    {

                        UserID = int.Parse(objReader["UserID"].ToString()),

                        DisplayUserName = objReader["UserFullName"].ToString(),

                    });

                }

                return sender;
            }
        }

        public List<CAPAEmail> GetActionerForMailing(int obID)
        {
            List<CAPAEmail> emailList = new List<CAPAEmail>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetActionerForMailing]";
                    objCom.CommandType = CommandType.StoredProcedure;

                    objCom.Parameters.AddWithValue("@obID", obID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        var capaemail = new CAPAEmail();
                           capaemail.ID = int.Parse(reader["ID"].ToString());
                        if (reader["FunctionalMgr"] != DBNull.Value)
                        {
                            capaemail.FunctionalMgr = reader["FunctionalMgr"].ToString();
                        }
                        capaemail.ActionerEmail = reader["ActionerEmail"].ToString();
                        capaemail.CAPAID = int.Parse(reader["CAPAID"].ToString());
                        capaemail.Area = reader["PlantName"].ToString();
                        capaemail.Observation = reader["Title"].ToString();
                        capaemail.Recommendation = reader["Recommendation"].ToString();
                        capaemail.Priority = reader["Priority"].ToString();
                        capaemail.Category = reader["Category"].ToString();
                        capaemail.TargetDate = reader["TargetDate"].ToString();
                        capaemail.FirstownerEmail = reader["FirstownerEmail"].ToString();
                        capaemail.SecondownerEmail = reader["SecondownerEmail"].ToString();
                        capaemail.HSEMgrEmail = reader["HSEManager"].ToString();
                        emailList.Add(capaemail);
                     }
                  
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return emailList;
        }
        public void SaveCAPAObservation(CAPAObservation cpObservation, string filename)
        {
            int affectedRecordCount = 0;
            int CompletedBy = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "SaveCAPAObservation";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@CAPAID", cpObservation.CAPAID);
                    objCom.Parameters.AddWithValue("@ObservationID", cpObservation.ObservationID);
                    objCom.Parameters.AddWithValue("@Title", cpObservation.Title == null ? String.Empty : cpObservation.Title);
                    objCom.Parameters.AddWithValue("@Recommendation", cpObservation.Recommendation == null ? String.Empty : cpObservation.Recommendation);
                    objCom.Parameters.AddWithValue("@ActionTaken", cpObservation.ActionTaken == null ? String.Empty : cpObservation.ActionTaken);
                    //objCom.Parameters.AddWithValue("@Comments", insObservation.Comments);
                    if (!string.IsNullOrEmpty(cpObservation.TargetDate))
                    {
                        objCom.Parameters.AddWithValue("@TargetDate", DateTime.ParseExact(cpObservation.TargetDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));

                    }
                    objCom.Parameters.AddWithValue("@UserID", cpObservation.CurrentUser);
                    objCom.Parameters.AddWithValue("@ObPlantID", cpObservation.OBPlantID);
                    objCom.Parameters.AddWithValue("@CategoryID", cpObservation.CategoryID);
                    objCom.Parameters.AddWithValue("@PriorityID", cpObservation.PriorityID);
                    objCom.Parameters.AddWithValue("@DeptManager", cpObservation.DeptManager);
                    objCom.Parameters.AddWithValue("@CapStatus", cpObservation.CAPStatus);
                    objCom.Parameters.AddWithValue("@FileName", filename);

                    if (cpObservation.UserID > 0)
                    {
                        objCom.Parameters.AddWithValue("@CompletedBy", cpObservation.UserID);
                    }
                    else
                    {
                        objCom.Parameters.AddWithValue("@CompletedBy", CompletedBy);
                    }
                    if (!string.IsNullOrEmpty(cpObservation.CompletedDate))
                    {
                        objCom.Parameters.AddWithValue("@CompletedDate", DateTime.ParseExact(cpObservation.CompletedDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    }
                    objCom.Parameters.AddWithValue("@Remarks", cpObservation.Remarks);

                    SqlParameter outPutParameter = new SqlParameter();
                    outPutParameter.ParameterName = "@OBID";
                    outPutParameter.SqlDbType = System.Data.SqlDbType.Int;
                    outPutParameter.Direction = System.Data.ParameterDirection.Output;
                    objCom.Parameters.Add(outPutParameter);

                    objCom.Connection = objCon;
                    objCon.Open();
                    affectedRecordCount = objCom.ExecuteNonQuery();
                    if (cpObservation.ObservationID == 0)
                    {
                        cpObservation.ObservationID = int.Parse(outPutParameter.Value.ToString());
                    }
                    objCom.Parameters.Clear();
                    objCon.Close();
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
        }
        public void DeleteOBImage(int obid)
        {
            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[DeleteCAPAOBImage]";
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



        public cpObservationViewModel EditCAPAObservation(int ObsID)
        {
            cpObservationViewModel incObservationVM = new cpObservationViewModel();
            CAPAObservation observation = new CAPAObservation();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetCAPAEditObservation]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@ObservationID", ObsID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {

                        observation.CAPAID = int.Parse(reader["CAPAID"].ToString());
                        observation.ObservationID = int.Parse(reader["ID"].ToString());
                        observation.Title = reader["Title"].ToString();
                        observation.OBPlantID = int.Parse(reader["ObPlantID"].ToString());
                        if (reader["CategoryID"] != DBNull.Value)
                        {
                            observation.CategoryID = int.Parse(reader["CategoryID"].ToString());
                        }
                        if (reader["PriorityID"] != DBNull.Value)
                        {
                            observation.PriorityID = int.Parse(reader["PriorityID"].ToString());
                        }
                        if (reader["DeptManager"] != DBNull.Value)
                        {
                            observation.DeptManager = int.Parse(reader["DeptManager"].ToString());
                        }
                        if (reader["CAPStatus"] != DBNull.Value)
                        {
                            observation.CAPStatus = int.Parse(reader["CAPStatus"].ToString());
                        }
                        if (reader["TargetDate"] != DBNull.Value)
                        {
                            observation.TargetDate = reader["TargetDate"].ToString();
                        }
                        if (reader["Recommendation"] != DBNull.Value)
                        {
                            observation.Recommendation = reader["Recommendation"].ToString();
                        }
                        if (reader["ActionTaken"] != DBNull.Value)
                        {
                            observation.ActionTaken = reader["ActionTaken"].ToString();
                        }
                        if (reader["CompletedBy"] != DBNull.Value)
                        {
                            observation.CompletedBy = int.Parse(reader["CompletedBy"].ToString());
                        }
                        observation.CompletedDate = reader["CompletedDate"].ToString();
                        observation.Remarks = reader["Remarks"].ToString();
                        observation.cpobservationattachement = reader["Attachment"].ToString();
                    }
                    incObservationVM.capaobs = observation;
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return incObservationVM;
        }



        public void DeleteCAPAObservation(int ObservationID)
        {
            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "DeleteCAPAObservervation";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@ObservationID", ObservationID);

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

        public List<ObservationViewModelCapa> GetAllCAPAObservation()
        {
            List<ObservationViewModelCapa> observationList = new List<ObservationViewModelCapa>();

            int SNo = 1;

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetAllCAPAListObservation]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        var ObList = new ObservationViewModelCapa();
                        ObList.SNo = SNo++;
                        ObList.CAPAID = int.Parse(reader["CAPAID"].ToString());
                        ObList.CAPANumber = reader["CAPANumber"].ToString();
                        if (reader["ID"] != DBNull.Value)
                        {
                            ObList.ObservationID = int.Parse(reader["ID"].ToString());
                        }
                        ObList.Observation = reader["Title"].ToString();
                        ObList.CompletedUser = reader["UserFullName"].ToString();
                        ObList.TargetDate = reader["TargetDate"].ToString();
                        ObList.Recommendation = reader["Recommendation"].ToString();
                        ObList.ActionTaken = reader["ActionTaken"].ToString();
                        ObList.CompletedDate = reader["CompletedDate"].ToString();
                        ObList.PlantName = reader["PlantName"].ToString();
                        ObList.CAPASourceName = reader["CAPASourceName"].ToString();
                        ObList.CategoryName = reader["CategoryName"].ToString();
                        ObList.CPStatus = reader["Status"].ToString();
                        if (reader["CompletedBy"] != DBNull.Value)
                        {
                            ObList.CompletedBy = int.Parse(reader["CompletedBy"].ToString());
                        }

                        if (reader["DeptManager"] != DBNull.Value)
                        {
                            ObList.DptID = int.Parse(reader["DeptManager"].ToString());
                        }
                        ObList.FunctionalMgr = reader["ManagerName"].ToString();
                        ObList.AtachmentName = reader["AttachmentName"].ToString();
                        observationList.Add(ObList);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return observationList;
        }
        public List<ObservationViewModelCapa> GetMyActionStatus(int UserID)
        {
            List<ObservationViewModelCapa> MyActionStatusList = new List<ObservationViewModelCapa>();

            int SNo = 1;

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetMyActionStatus]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@UserID", UserID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {

                        var ObList = new ObservationViewModelCapa();
                        ObList.SNo = SNo++;
                        if (reader["ID"] != DBNull.Value)
                        {
                            ObList.ObservationID = int.Parse(reader["ID"].ToString());
                        }
                        ObList.MOCID = int.Parse(reader["MOCNumber"].ToString());
                        ObList.PSSRID = int.Parse(reader["PSSRID"].ToString());
                        ObList.CAPAID = int.Parse(reader["CAPAID"].ToString());
                        ObList.InciID = int.Parse(reader["InciID"].ToString());
                        ObList.OBID = int.Parse(reader["OBID"].ToString());
                        ObList.InciStatusID = int.Parse(reader["StatusID"].ToString());
                        ObList.GetStatus = reader["Status"].ToString();
                        // ObList.Observation = reader["Title"].ToString();
                        ObList.TargetDate = reader["TargetDate"].ToString();
                        ObList.Recommendation = reader["Recommendation"].ToString();
                        ObList.ActionTaken = reader["ActionTaken"].ToString();
                        ObList.PriorityName = reader["PriorityName"].ToString();
                        ObList.CAPASourceName = reader["Source"].ToString();
                        MyActionStatusList.Add(ObList);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return MyActionStatusList;
        }


        public List<ActionsCount> GetActionStatusCount()
        {
            List<ActionsCount> ActionCounts = new List<ActionsCount>();

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetCAPAActionsCount]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        ActionsCount actionCount = new ActionsCount()
                        {
                            StatusName = reader["Actions"].ToString(),
                            TotalCount = int.Parse(reader["ActionsCount"].ToString())
                        };
                    
                            ActionCounts.Add(actionCount);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return ActionCounts;
        }
        public List<CapaSourceCounts> GetCapaSourceCount()
        {
            List<CapaSourceCounts> Capasourcecount = new List<CapaSourceCounts>();

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetCAPASourceCounttemp]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        CapaSourceCounts actionCount = new CapaSourceCounts()
                        {
                            SourceName = reader["CapaSourceName"].ToString(),
                          
                            TotalCount = int.Parse(reader["TotalCount"].ToString())


                        };
                
                            Capasourcecount.Add(actionCount);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return Capasourcecount;
        }

        public List<CategoryCount> GetCategoryCount()
        {
            List<CategoryCount> Counts= new List<CategoryCount>();

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetCAPACategoryCount]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                       CategoryCount actionCount = new CategoryCount()
                        {
                            SourceName = reader["CategoryName"].ToString(),

                            TotalCount = int.Parse(reader["TotalCount"].ToString())


                        };

                      Counts.Add(actionCount);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return Counts;
        }
        public List<ObservationCount> GetObservationCount()
        {
            List<ObservationCount> observationcounts = new List<ObservationCount>();

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetObservationsCount]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                    ObservationCount observationcount = new ObservationCount()
                        {
                            MonthName = reader["ObMonth"].ToString(),

                            MonthlyCount = int.Parse(reader["TotalCount"].ToString())

                        };

                        observationcounts.Add(observationcount);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return observationcounts;
        }
        #region GetCapaFunctionalManagerCount
        public List<PriorityCount> GetCapaFunctionalManagerCount()
        {
            List<PriorityCount> counts = new List<PriorityCount>();

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetCapaFunctionalManagerCount]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        PriorityCount actionCount = new PriorityCount()
                        {
                            Name = reader["FunctionalManager"].ToString(),
                            Overdue = int.Parse(reader["overdue"].ToString()),
                            Closed = int.Parse(reader["closed"].ToString()),
                            Opened = int.Parse(reader["opened"].ToString()),
                            //ReOpen = int.Parse(reader["ReOpen"].ToString()),
                            New = int.Parse(reader["New"].ToString()),


                        };
                        if ((actionCount.Overdue > 0) || (actionCount.Closed > 0) || (actionCount.Opened > 0) || (actionCount.New > 0))
                            counts.Add(actionCount);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return counts;
        }
        #endregion

        public List<PriorityCount> GetCapaPriorityCount()
        {
            List<PriorityCount> prioritycount = new List<PriorityCount>();

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetCAPAPriorityCount]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                       PriorityCount actionCount = new PriorityCount()
                        {
                            PriorityName = reader["Name"].ToString(),
                            Overdue = int.Parse(reader["Overdue"].ToString()),
                            Closed = int.Parse(reader["Closed"].ToString()),
                            Opened = int.Parse(reader["Opened"].ToString()),
                           //ReOpen = int.Parse(reader["ReOpen"].ToString()),
                           New = int.Parse(reader["New"].ToString()),

                           TotalCount = int.Parse(reader["TotalCount"].ToString())


                        };
                        if ((actionCount.Overdue > 0) || (actionCount.Closed > 0) || (actionCount.Opened > 0) || (actionCount.New > 0))
                            prioritycount.Add(actionCount);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return prioritycount;
        }

        public List<ObservationViewModelCapa> SearchOpenCapaForObservation(CAPASearchViewModel capasearchviewmodel)
        {

            List<ObservationViewModelCapa> observationList = new List<ObservationViewModelCapa>();
            int SNo = 1;
            try
            {

                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[SearchOpenCAPAForObservation]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@CAPAPlant", capasearchviewmodel.CAPAPlant);
                    objCom.Parameters.AddWithValue("@CAPAStatus", capasearchviewmodel.CAPAStatus);
                    objCom.Parameters.AddWithValue("@FromDate", capasearchviewmodel.CAPAFromDate == null ? string.Empty : capasearchviewmodel.CAPAFromDate);
                    objCom.Parameters.AddWithValue("@ToDate", capasearchviewmodel.CAPAToDate == null ? string.Empty : capasearchviewmodel.CAPAToDate);
                    objCom.Parameters.AddWithValue("@DptManager", capasearchviewmodel.DeptManager);
                    objCom.Parameters.AddWithValue("@CAPASource", capasearchviewmodel.CAPASource);
                    objCom.Parameters.AddWithValue("@Category", capasearchviewmodel.CategoryID);
                    objCom.Parameters.AddWithValue("@ActionBy", capasearchviewmodel.ActionerID);
                    objCom.Parameters.AddWithValue("@Priority", capasearchviewmodel.PriorityID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        var ObList = new ObservationViewModelCapa();
                        ObList.SNo = SNo++;
                        ObList.CAPAID = int.Parse(reader["CAPAID"].ToString());
                        if (reader["ID"] != DBNull.Value)
                        {
                            ObList.ObservationID = int.Parse(reader["ID"].ToString());
                        }
                        ObList.CAPANumber = reader["CAPANumber"].ToString();
                        ObList.Observation = reader["Title"].ToString();
                        ObList.CompletedUser = reader["UserFullName"].ToString();
                        ObList.TargetDate = reader["TargetDate"].ToString();
                        ObList.Recommendation = reader["Recommendation"].ToString();
                        ObList.ActionTaken = reader["ActionTaken"].ToString();
                        ObList.CompletedDate = reader["CompletedDate"].ToString();
                        ObList.PlantName = reader["PlantName"].ToString();
                        ObList.CAPASourceName = reader["CAPASourceName"].ToString();
                        ObList.CategoryName = reader["CategoryName"].ToString();
                        ObList.PriorityName = reader["PriorityName"].ToString();
                        ObList.CPStatus = reader["Status"].ToString();
                       
                        if (reader["CompletedBy"] != DBNull.Value)
                        {
                            ObList.CompletedBy = int.Parse(reader["CompletedBy"].ToString());
                        }
                        if (reader["DeptManager"] != DBNull.Value)
                        {
                            ObList.DptID = int.Parse(reader["DeptManager"].ToString());
                        }
                        ObList.FunctionalMgr = reader["ManagerName"].ToString();
                        ObList.AtachmentName = reader["AttachmentName"].ToString();
                        observationList.Add(ObList);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return observationList;
        }


    }


}
