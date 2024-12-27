using MonitPro.Common.Library;
using MonitPro.Models.Incident;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitPro.Models.MOC;
using MonitPro.Models.Account;
using System.Xml.Serialization;
using System.IO;
using MonitPro.Models;

namespace MonitPro.DAL
{
    public class MOCDAL
    {
        string constring = AppConfig.ConnectionString;
        SqlCommand objCom;
        SqlDataReader reader;

      
        public List<MOCReasonForChange> GetMOCReasonForChanges()
        {
            List<MOCReasonForChange> ReasonList = new List<MOCReasonForChange>();
            try
            {
                using (SqlConnection connection = new SqlConnection(constring))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "GetMOCReasonForChange";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = connection;
                    connection.Open();
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        MOCReasonForChange reason = new MOCReasonForChange();
                        reason.ReasonID = int.Parse(reader["ID"].ToString());
                        reason.ReasonForChangeName = reader["ReasonForChange"].ToString();
                        ReasonList.Add(reason);
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);

            }
            return ReasonList;
        }
        public List<MOCClassCount> GetMOCClassCount(DateTime startDate, DateTime endDate)
        {
            List<MOCClassCount> mocclasslist = new List<MOCClassCount>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetMonthlyMOCClassCount]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@StartDate", startDate);
                    objCom.Parameters.AddWithValue("@EndDate", endDate);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();
                    while (reader.Read())
                    {
                        MOCClassCount mocclass = new MOCClassCount();
                        mocclass.MOCMonth = reader["MOCMonth"].ToString();
                        mocclass.Temp = int.Parse(reader["Temporary"].ToString());
                        mocclass.Permant = int.Parse(reader["Permanent"].ToString());
                        mocclasslist.Add(mocclass);
                    }
                }

            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return mocclasslist;
        }

        public List<MOCPlantCount> GetMOCPlantCount(DateTime startDate, DateTime endDate)
        {
            List<MOCPlantCount> mocplantlist = new List<MOCPlantCount>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetMOCPlantCount]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@StartDate", startDate);
                    objCom.Parameters.AddWithValue("@EndDate", endDate);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();
                    while (reader.Read())
                    {
                        MOCPlantCount mocplant = new MOCPlantCount();
                        mocplant.PlantName = reader["Plant"].ToString();
                        mocplant.TotalCount = int.Parse(reader["PlantCount"].ToString());
                        if (mocplant.TotalCount > 0)
                            mocplantlist.Add(mocplant);
                    }
                }

            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return mocplantlist;
        }

        public List<MOCCategoryCount> GetMOCCategoryCount(DateTime startDate, DateTime endDate)
        {
            List<MOCCategoryCount> moccategorylist = new List<MOCCategoryCount>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetMOCCategoryofChangeCount]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@StartDate", startDate);
                    objCom.Parameters.AddWithValue("@EndDate", endDate);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();
                    while (reader.Read())
                    {
                        MOCCategoryCount moccategory = new MOCCategoryCount();
                        moccategory.CategoryName = reader["Category"].ToString();
                        moccategory.TotalCount = int.Parse(reader["CategoryCount"].ToString());
                        if (moccategory.TotalCount > 0)
                            moccategorylist.Add(moccategory);
                    }
                }

            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return moccategorylist;
        }

        public List<MOCStatusCount> GetMOCStatusCount(DateTime startDate, DateTime endDate)
        {
            List<MOCStatusCount> mocstatuslist = new List<MOCStatusCount>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetMOCStatusCount]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@StartDate", startDate);
                    objCom.Parameters.AddWithValue("@EndDate", endDate);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();
                    while (reader.Read())
                    {
                        MOCStatusCount mocstatus = new MOCStatusCount();
                        mocstatus.StatusName = reader["MOCStatus"].ToString();
                        mocstatus.TotalCount = int.Parse(reader["StatusCount"].ToString());
                        if (mocstatus.TotalCount > 0)
                            mocstatuslist.Add(mocstatus);
                    }
                }

            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return mocstatuslist;
        }

        public List<MOCPriorityCount> GetMOCPriorityCount(DateTime startDate, DateTime endDate)
        {
            List<MOCPriorityCount> mocprioritylist = new List<MOCPriorityCount>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetMOCPriorityCount]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@StartDate", startDate);
                    objCom.Parameters.AddWithValue("@EndDate", endDate);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();
                    while (reader.Read())
                    {
                        MOCPriorityCount mocpriority = new MOCPriorityCount();
                        mocpriority.PriorityName = reader["Priority"].ToString();
                        mocpriority.TotalCount = int.Parse(reader["TotalCount"].ToString());
                        mocpriority.Open = int.Parse(reader["opened"].ToString());
                        mocpriority.Closed = int.Parse(reader["closed"].ToString());
                        if ((mocpriority.Open > 0) || (mocpriority.Closed > 0))
                            mocprioritylist.Add(mocpriority);
                    }
                }

            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return mocprioritylist;
        }

        public List<MOCRecomStatusCount> GetMOCRecomStatusCount(DateTime startDate, DateTime endDate)
        {
            List<MOCRecomStatusCount> mocrecomstatuslist = new List<MOCRecomStatusCount>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetMOCRecommendStatusCount]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@StartDate", startDate);
                    objCom.Parameters.AddWithValue("@EndDate", endDate);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();
                    while (reader.Read())
                    {
                        MOCRecomStatusCount mocrecomstatus = new MOCRecomStatusCount();
                        mocrecomstatus.RecomStatusName = reader["RecomPriority"].ToString();
                        mocrecomstatus.TotalCount = int.Parse(reader["TotalCount"].ToString());
                        mocrecomstatus.Overdue = int.Parse(reader["overdue"].ToString());
                        mocrecomstatus.Pending = int.Parse(reader["pending"].ToString());
                        mocrecomstatus.Completed = int.Parse(reader["Completed"].ToString());
                        if ((mocrecomstatus.Overdue > 0) || (mocrecomstatus.Pending > 0) || (mocrecomstatus.Completed > 0))
                            mocrecomstatuslist.Add(mocrecomstatus);
                    }
                }

            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return mocrecomstatuslist;
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

                    if (reader.HasRows)
                    {

                        PlantsList.Add(new Plants { ID = -1, Name = "All" });
                    }

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

        public List<MOCClassification> GetMOCClassification()
        {
            List<MOCClassification> mocclass = new List<MOCClassification>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetMOCClassification]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    if (reader.HasRows)
                    {

                        mocclass.Add(new MOCClassification { ID = 0, Name = "All" });
                    }

                    while (reader.Read())
                    {
                        mocclass.Add(new MOCClassification { ID = int.Parse(reader["ID"].ToString()), Name = reader["Name"].ToString(), Description = reader["Description"].ToString() });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return mocclass;
        }


        public List<MOCType> GetMOCType()
        {
            List<MOCType> moctype = new List<MOCType>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetMOCType]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    if (reader.HasRows)
                    {

                        moctype.Add(new MOCType { ID = -1, Name = "All" });
                    }

                    while (reader.Read())
                    {
                        moctype.Add(new MOCType { ID = int.Parse(reader["ID"].ToString()), Name = reader["Name"].ToString(), Description = reader["Description"].ToString() });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return moctype;
        }

        public List<Employee> GetMOCFunMgr(int Deptid)
        {
            List<Employee> DeptManager = new List<Employee>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetMOCFunctionalManager]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@Dept", Deptid);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();
                    if (reader.HasRows)
                    {
                        DeptManager.Add(new Employee { ID = 0, FullName = "All" });
                    }
                    while (reader.Read())
                    {
                        DeptManager.Add(new Employee
                        {
                            ID = int.Parse(reader["UserID"].ToString()),
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

            return DeptManager;
        }


        public List<MOCCategory> GetMOCCategory()
        {
            List<MOCCategory> moccategory = new List<MOCCategory>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetMOCCategory]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    if (reader.HasRows)
                    {

                        moccategory.Add(new MOCCategory { ID = -1, Name = "All" });
                    }

                    while (reader.Read())
                    {
                        moccategory.Add(new MOCCategory { ID = int.Parse(reader["ID"].ToString()), Name = reader["Name"].ToString(), Description = reader["Description"].ToString() });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return moccategory;
        }

        public List<MOCStatus> GetMOCStatus()
        {
            List<MOCStatus> mocstatus = new List<MOCStatus>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetMOCStatus]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    if (reader.HasRows)
                    {

                        mocstatus.Add(new MOCStatus { ID = -1, Name = "All" });
                    }

                    while (reader.Read())
                    {
                        mocstatus.Add(new MOCStatus { ID = int.Parse(reader["ID"].ToString()), Name = reader["Name"].ToString(), Description = reader["Description"].ToString() });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return mocstatus;
        }
        public List<MOCTempStatus> GetMOCTempStatus()
        {
            List<MOCTempStatus> TempStatus = new List<MOCTempStatus>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetMOCTemporaryStatus]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    if (reader.HasRows)
                    {

                        TempStatus.Add(new MOCTempStatus { TempID = -1, TempName = "All" });
                    }

                    while (reader.Read())
                    {
                        TempStatus.Add(new MOCTempStatus { TempID = int.Parse(reader["ID"].ToString()), TempName = reader["Name"].ToString(), TempDescription = reader["Description"].ToString() });
                    }
                }

            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return TempStatus;
        }
        public List<UserProfile> GetMOCApprover()
        {
            using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
            {
                SqlCommand objCom = new SqlCommand();
                objCom.CommandText = "GetApprover";
                objCom.CommandType = CommandType.StoredProcedure;
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();
                var approver = new List<UserProfile>();
                if (objReader.HasRows)
                {
                    approver.Add(new UserProfile { UserID = 0, DisplayUserName = "All" });
                }
                while (objReader.Read())
                {
                    approver.Add(new UserProfile
                    {

                        UserID = int.Parse(objReader["UserID"].ToString()),

                        DisplayUserName = objReader["UserFullName"].ToString(),

                    });

                }

                return approver;
            }
        }


        public List<MOCPriority> GetMOCPriority()
        {
            List<MOCPriority> mocpriority = new List<MOCPriority>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetMOCPriority]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    if (reader.HasRows)
                    {

                        mocpriority.Add(new MOCPriority { ID = -1, Name = "All" });
                    }

                    while (reader.Read())
                    {
                        mocpriority.Add(new MOCPriority { ID = int.Parse(reader["ID"].ToString()), Name = reader["Name"].ToString(), Description = reader["Description"].ToString() });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return mocpriority;
        }

        public List<MOCRecomPriority> GetMOCRecomPriority()
        {
            List<MOCRecomPriority> mocrecompriority = new List<MOCRecomPriority>();
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

                    if (reader.HasRows)
                    {

                        mocrecompriority.Add(new MOCRecomPriority { ID = -1, Name = "All" });
                    }

                    while (reader.Read())
                    {
                        mocrecompriority.Add(new MOCRecomPriority { ID = int.Parse(reader["ID"].ToString()), Name = reader["Name"].ToString(), Description = reader["Description"].ToString() });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return mocrecompriority;
        }

        public List<MOCRecomCategory> GetMOCRecomCategory()
        {
            List<MOCRecomCategory> mocrecomcategory = new List<MOCRecomCategory>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetMOCRecommCategory]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    if (reader.HasRows)
                    {

                        mocrecomcategory.Add(new MOCRecomCategory { ID = -1, Name = "All" });
                    }

                    while (reader.Read())
                    {
                        mocrecomcategory.Add(new MOCRecomCategory { ID = int.Parse(reader["ID"].ToString()), Name = reader["Name"].ToString(), Description = reader["Description"].ToString() });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return mocrecomcategory;
        }
        public int MOCInsertUpdate(NewMOCModel newMOC, string fileName, int currentuser)
        {
            int affectedRecordCount = 0;
            List<MOCCategory> moccategories = new List<MOCCategory>();
            List<MOCChangeXML> changexml = new List<MOCChangeXML>();
            List<MOCSecondaryChangeXML> secondaryChangeXMLs = new List<MOCSecondaryChangeXML>();
            string mocchange = string.Empty;
            string secondarychange = string.Empty;
            newMOC.moca.MOCStatusIdentify = newMOC.moca.MOCStatusIdentify != null ? newMOC.moca.MOCStatusIdentify : string.Empty;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "MOCInsertUpdate";
                    objCom.CommandType = CommandType.StoredProcedure;
                    var Emergency = newMOC.moca.Emergency == true ? 1 : 0;
                    var verifyrisk = newMOC.moca.VerifyRiskAssessment == true ? 1 : 0;
                    var crossbussiness = newMOC.moca.CrossBussinessIdea == true ? 1 : 0;
                    objCom.Parameters.AddWithValue("@MOCCreatedDate", DateTime.ParseExact(newMOC.moca.CreatedDate, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture));
                    //  objCom.Parameters.AddWithValue("@MOCNumber ", newMOC.moca.MOCNumber);
                    objCom.Parameters.AddWithValue("@PlantID", newMOC.moca.PlantID);
                    objCom.Parameters.AddWithValue("@MOCStatusID ", newMOC.moca.MOCStatusID);
                    objCom.Parameters.AddWithValue("@MOCCategoryID", newMOC.moca.MOCClassificationID);
                    objCom.Parameters.AddWithValue("@MOCTypeID", newMOC.moca.MOCTypeID);
                    objCom.Parameters.AddWithValue("@FunctionalMgrID", newMOC.moca.MOCFunCMgrID);
                    objCom.Parameters.AddWithValue("@MOCPriority", newMOC.moca.MOCPriorityID);
                    objCom.Parameters.AddWithValue("@MOCTitle", newMOC.moca.MOCTitle);
                    objCom.Parameters.AddWithValue("@MOCDescription", newMOC.moca.MOCDescription);
                    objCom.Parameters.AddWithValue("@Process", newMOC.moca.Process);
                    objCom.Parameters.AddWithValue("@Electrical", newMOC.moca.Electrical);
                    objCom.Parameters.AddWithValue("@Civil", newMOC.moca.Civil);
                    objCom.Parameters.AddWithValue("@Mechanical ", newMOC.moca.Mechanical);
                    objCom.Parameters.AddWithValue("@Instrument", newMOC.moca.Instrument);
                    objCom.Parameters.AddWithValue("@Others", newMOC.moca.Others);
                    objCom.Parameters.AddWithValue("@MOCRequiredOrNot", newMOC.moca.MOCRequiredOrNot);
                    objCom.Parameters.AddWithValue("@MOCRequiredDetails", newMOC.moca.MOCRequiredDetails);
                    objCom.Parameters.AddWithValue("@FileName", fileName);
                    objCom.Parameters.AddWithValue("@UserID", currentuser);
                    objCom.Parameters.AddWithValue("@DRCost", newMOC.moca.DRCost);
                    objCom.Parameters.AddWithValue("@MOCID", newMOC.moca.MOCID);
                    objCom.Parameters.AddWithValue("@FunMgrComment", newMOC.moca.FunMgrComment);
                    objCom.Parameters.AddWithValue("@AssetID", newMOC.moca.AssetID);
                    //objCom.Parameters.AddWithValue("@ExpiryDate", DateTime.ParseExact(newMOC.moca.ExpiryDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    //objCom.Parameters.AddWithValue("@EffectiveDate", DateTime.ParseExact(newMOC.moca.EffectiveDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    objCom.Parameters.AddWithValue("@Emergency", Emergency);
                    objCom.Parameters.AddWithValue("@VerifyRiskAssessment", verifyrisk);
                    objCom.Parameters.AddWithValue("@CrossBussinessIdea", crossbussiness);
                    objCom.Parameters.AddWithValue("@ConditionForMOCApprove", newMOC.moca.ConditionforMOCApprove);
                    objCom.Parameters.AddWithValue("@PSSRSignOffDecision", newMOC.moca.PSSRSignOFFDecision);
                    objCom.Parameters.AddWithValue("@Consequence", newMOC.moca.Consequences);
                    objCom.Parameters.AddWithValue("@Likelihood", newMOC.moca.Likelihood);
                    SqlParameter outPutParameter = new SqlParameter();
                    outPutParameter.ParameterName = "@NewMOCID";
                    outPutParameter.SqlDbType = System.Data.SqlDbType.Int;
                    outPutParameter.Direction = System.Data.ParameterDirection.Output;
                    objCom.Parameters.Add(outPutParameter);

                    objCom.Connection = objCon;
                    objCon.Open();
                    affectedRecordCount = objCom.ExecuteNonQuery();
                    objCom.Parameters.Clear();
                    newMOC.moca.MOCID = int.Parse(outPutParameter.Value.ToString());
                    foreach (var item in newMOC.moca.mocCategory)
                    {
                        if (item.YesNo == true)
                        {
                            var xml = new MOCChangeXML
                            {
                                MOCChangeID = item.ID

                            };
                            changexml.Add(xml);
                        }
                    }
                    XmlSerializer xmlSerializer = new XmlSerializer(changexml.GetType());

                    using (StringWriter textWriter = new StringWriter())
                    {
                        xmlSerializer.Serialize(textWriter, changexml);
                        mocchange = textWriter.ToString();
                    }

                    objCom.CommandText = "MOCCategoryInsert";
                    objCom.Parameters.AddWithValue("@MOCID", newMOC.moca.MOCID);
                    objCom.Parameters.AddWithValue("@MOCChangeDetails", mocchange);
                    affectedRecordCount = objCom.ExecuteNonQuery();
                    objCom.Parameters.Clear();
                    //foreach (var item in newMOC.moca.GetMocReasonForChange)
                    //{
                    //    if (item.YesNo == true)
                    //    {
                    //        var secondaryxml = new MOCSecondaryChangeXML
                    //        {
                    //            SecondaryChangeID = item.ReasonID

                    //        };
                    //        secondaryChangeXMLs.Add(secondaryxml);
                    //    }
                    //}
                    //XmlSerializer xmlSerializer1 = new XmlSerializer(secondaryChangeXMLs.GetType());

                    //using (StringWriter textWriter = new StringWriter())
                    //{
                    //    xmlSerializer1.Serialize(textWriter, secondaryChangeXMLs);
                    //    secondarychange = textWriter.ToString();
                    //}

                    //objCom.CommandText = "[MOC_ReasonForChangeInsert]";
                    //objCom.Parameters.AddWithValue("@MOCID", newMOC.moca.MOCID);
                    //objCom.Parameters.AddWithValue("@MOCReasonChangeDetails", secondarychange);
                    //affectedRecordCount = objCom.ExecuteNonQuery();

                    objCon.Close();
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return newMOC.moca.MOCID;
        }

        public List<MOCViewModel> GetOpenMOC()
        {
            List<MOCViewModel> MOCList = new List<MOCViewModel>();
            try
            {
                int RecordCount = 1;
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    var objCom = new SqlCommand();
                    objCom.CommandText = "[GetAllOpenMoc]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    var objreader = objCom.ExecuteReader();


                    while (objreader.Read())
                    {
                        MOCViewModel moclist = new MOCViewModel();
                        moclist.SNo = RecordCount++;
                        moclist.MOCID = int.Parse(objreader["ID"].ToString());
                        moclist.PlantArea = objreader["PlantName"].ToString();
                        moclist.MOCType = objreader["TypeName"].ToString();
                        if (objreader["MOCCategory"] != DBNull.Value)
                        {
                            moclist.MOCCategory = objreader["MOCCategory"].ToString();
                        }
                        if (objreader["MOCNumber"] != DBNull.Value)
                        {
                            moclist.MOCNumber = objreader["MOCNumber"].ToString();
                        }
                        moclist.MocStatus = objreader["StatusName"].ToString();
                        //   moclist.ActionTaken = reader["ActionTaken"].ToString();
                        moclist.MOCTitle = objreader["MOCTitle"].ToString();
                        moclist.FileName = objreader["Attachments"].ToString();
                        if (objreader["ApprovedDate"] != DBNull.Value)
                        {
                            moclist.MOCCreated = objreader["ApprovedDate"].ToString();
                        }
                        if (objreader["UserFullName"] != DBNull.Value)
                        {
                            moclist.Approver = objreader["UserFullName"].ToString();
                        }
                        if (objreader["TargetDate"] != DBNull.Value)
                        {
                            moclist.TargetDate = objreader["TargetDate"].ToString();
                        }
                        moclist.FuncationalManagerID = int.Parse(objreader["FuncationalMangerID"].ToString());
                        moclist.MOCCreatedBy = int.Parse(objreader["CreatedBy"].ToString());
                        moclist.ClassName = objreader["ClassName"].ToString();
                        if (objreader["CompletedDate"] != DBNull.Value)
                        {
                            moclist.MOCClosedDate = objreader["CompletedDate"].ToString();
                        }
                        if (objreader["MOCCoOrdinate"] != DBNull.Value)
                        {
                            moclist.MOCCOOrdinate = objreader["MOCCoOrdinate"].ToString();
                        }
                        if (objreader["TemporaryStatus"] != DBNull.Value)
                        {
                            moclist.TempMOCStatus = objreader["TemporaryStatus"].ToString();
                        }
                        MOCList.Add(moclist);

                    }
                    objCon.Close();
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return MOCList;
        }

        public List<ApproverModel> GetApproverForMOCPageList()
        {
            List<ApproverModel> app = new List<ApproverModel>();
            try
            {
                int RecordCount = 1;
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetApproverForMOCPageList]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();


                    while (reader.Read())
                    {
                        ApproverModel aplist = new ApproverModel();
                        aplist.SNo = RecordCount++;
                        aplist.MOCID = int.Parse(reader["MOCID"].ToString());

                        if (reader["EmployeeID"] != DBNull.Value)
                        {
                            aplist.ApproverUser = int.Parse(reader["EmployeeID"].ToString());
                        }

                        app.Add(aplist);

                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return app;
        }

        public List<TemporaryMOCList> GetTemporaryList()
        {
            List<TemporaryMOCList> tempList = new List<TemporaryMOCList>();
            try
            {

                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetTemporaryMOC]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();
                    while (reader.Read())
                    {
                        TemporaryMOCList al = new TemporaryMOCList();

                        al.MOCID = int.Parse(reader["ID"].ToString());
                        al.InitiationDate = reader["InitiationDate"].ToString();
                        al.FirstTargetDate = reader["FirstTargetDate"].ToString();
                        al.CloseComments = reader["CloseComments"].ToString();

                        if (reader["RevisedTargetDate"] != DBNull.Value)
                        {
                            al.RevisedTargetDate = reader["RevisedTargetDate"].ToString();
                        }
                        if (reader["ReasonForExtension"] != DBNull.Value)
                        {
                            al.ReasonExtension = reader["ReasonForExtension"].ToString();
                        }
                        if (reader["TempStatus"] != DBNull.Value)
                        {
                            al.TempStatus = reader["TempStatus"].ToString();
                        }
                        if (reader["ApproverComments"] != DBNull.Value)
                        {
                            al.ApproverComments = reader["ApproverComments"].ToString();
                        }
                        if (reader["FactoryManagerName"] != DBNull.Value)
                        {
                            al.ApproverName = reader["FactoryManagerName"].ToString();
                        }
                        al.MOCCoordinate = reader["MOCCoordinate"].ToString();
                        al.plant = reader["Plant"].ToString();
                        al.MOCNumber = reader["MOCNumber"].ToString();
                        al.MOCTitle = reader["MOCTitle"].ToString();
                        al.FactoryManagerID = int.Parse(reader["FactoryManagerID"].ToString());
                        tempList.Add(al);

                    }

                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return tempList;
        }

        public MOCa GetMOC(int MOCID)
        {
            MOCa moca = new MOCa();
            List<MOCCategory> mc = new List<MOCCategory>();
            List<MOCReasonForChange> ReasonChangeList = new List<MOCReasonForChange>();
            List<MOCAdvisor> mOCAdvisors = new List<MOCAdvisor>();
            string Moctemp = null;
            string Mocemail = null;
            string MocemailContent = null;
            string MOCAdvisorName = null;
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetMOC]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@MOCID", MOCID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    if (reader.Read())
                    {
                        moca.MOCNumber = reader["MOCNumber"].ToString();
                        moca.MOCID = int.Parse(reader["ID"].ToString());
                        moca.MOCDescription = reader["MOCDescription"].ToString();
                        moca.CreatedDate = reader["CreateDate"].ToString();
                        moca.PlantID = int.Parse(reader["PlantID"].ToString());
                        moca.MOCTitle = reader["MOCTitle"].ToString();
                        moca.MOCDescription = reader["MOCDescription"].ToString();
                        moca.MOCClassificationID = int.Parse(reader["MOCCategoryID"].ToString());
                        moca.MOCFunCMgrID = int.Parse(reader["FuncationalMangerID"].ToString());
                        moca.MOCTypeID = int.Parse(reader["MOCTypeID"].ToString());
                        moca.Process = reader["Process"].ToString();
                        moca.Electrical = reader["Electrical"].ToString();
                        moca.Mechanical = reader["Mechanical"].ToString();
                        moca.Civil = reader["Civil"].ToString();
                        moca.Instrument = reader["Instrument"].ToString();
                        moca.Others = reader["Others"].ToString();
                        moca.FileName = reader["Attachments"].ToString();
                        moca.MOCPriorityID = int.Parse(reader["MOCPriorityID"].ToString());
                        moca.CreatedBy = reader["CreatedName"].ToString();
                        moca.MOCStatusID = int.Parse(reader["MOCStatusID"].ToString());
                        moca.PlantName = reader["PlantName"].ToString();
                        if (reader["FunMgrComments"] != DBNull.Value)
                        {
                            moca.FunMgrComment = reader["FunMgrComments"].ToString();
                        }
                        if (reader["ApproverUserID"] != DBNull.Value)
                        {
                            moca.ApproverUserID = int.Parse(reader["ApproverUserID"].ToString());
                        }
                        if (reader["ApprovalStageID"] != DBNull.Value)
                        {
                            moca.ApproverStageID = int.Parse(reader["ApprovalStageID"].ToString());
                        }
                        moca.MOCRequiredOrNot = reader["MOCRequiredOrNot"].ToString();
                        moca.MOCRequiredDetails = reader["MOCRequiredDetails"].ToString();
                        if (reader["DRCost"] != DBNull.Value)
                        {
                            moca.DRCost = int.Parse(reader["DRCost"].ToString());
                        }
                        if (reader["MOCTypeName"] != DBNull.Value)
                        {
                            moca.MOCTypeName = reader["MOCTypeName"].ToString();

                        }
                        if (reader["MOCCategory"] != DBNull.Value)
                        {
                            moca.MOCCategoryName = reader["MOCCategory"].ToString();
                        }
                        if (reader["MOCClassName"] != DBNull.Value)
                        {
                            moca.MOCClassName = reader["MOCClassName"].ToString();
                        }
                        if (reader["CreatedByEmail"] != DBNull.Value)
                        {
                            moca.CreatedByEmail = reader["CreatedByEmail"].ToString();
                        }
                        if (reader["FunctionalMgrEmail"] != DBNull.Value)
                        {
                            moca.FunMgrEmail = reader["FunctionalMgrEmail"].ToString();
                        }
                        if (reader["MOCTeamAssignCheck"] != DBNull.Value)
                        {
                            moca.MOCTeamEmailCheck = reader["MOCTeamAssignCheck"].ToString();
                        }
                        moca.FunMgrName = reader["FunMgrName"].ToString();
                        if (reader["AssetID"] != DBNull.Value)
                        {
                            moca.AssetID = int.Parse(reader["AssetID"].ToString());
                        }
                        moca.ExpiryDate = reader["MOCExpiryDate"].ToString();
                        moca.EffectiveDate = reader["MOCChangeEffectiveDate"].ToString();
                        if (reader["Emergency"] != DBNull.Value)
                        {
                            moca.Emergency = int.Parse(reader["Emergency"].ToString()) == 1 ? true : false;
                        }
                        if (reader["VerifyRiskAssessment"] != DBNull.Value)
                        {
                            moca.VerifyRiskAssessment = int.Parse(reader["VerifyRiskAssessment"].ToString()) == 1 ? true : false;
                        }
                        if (reader["CrossBusinessIdea"] != DBNull.Value)
                        {
                            moca.CrossBussinessIdea = int.Parse(reader["CrossBusinessIdea"].ToString()) == 1 ? true : false;
                        }
                        if (reader["ConditionForMOCApprove"] != DBNull.Value)
                        {
                            moca.ConditionforMOCApprove = int.Parse(reader["ConditionForMOCApprove"].ToString());
                        }
                        if (reader["PSSRSignDecision"] != DBNull.Value)
                        {
                            moca.PSSRSignOFFDecision = int.Parse(reader["PSSRSignDecision"].ToString());
                        }
                        if (reader["Consequence"] != DBNull.Value)
                        {
                            var consequence = reader["Consequence"].ToString();
                            moca.Consequences = consequence.Trim();

                        }
                        if (reader["Likelihood"] != DBNull.Value)
                        {
                            var likelihood = reader["Likelihood"].ToString();
                            moca.Likelihood = likelihood.Trim();

                        }
                        if (reader["RARating"] != DBNull.Value)
                        {
                            moca.RARating = reader["RARating"].ToString();
                        }
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                MOCCategory mOCCategory = new MOCCategory();
                                mOCCategory.ID = int.Parse(reader["ID"].ToString());
                                mOCCategory.YesNo = int.Parse(reader["ChkBox"].ToString()) > 0 ? true : false;
                                mOCCategory.Name = reader["Name"].ToString();
                                mc.Add(mOCCategory);

                            }
                            moca.mocCategory = mc;


                        }
                        reader.NextResult();
                        if (reader.Read())
                        {
                            moca.MOCStatusInList = reader["StatusName"].ToString();
                        }
                        reader.NextResult();
                        while (reader.Read())
                        {
                            var email = new MOCAdvisor();
                            email.MOCAdvisorEmailAddress = reader["MOCAdvisorEmail"].ToString();
                            email.MOCAdvisorName = reader["MOCAdvisorName"].ToString();
                            mOCAdvisors.Add(email);
                        }
                        foreach (MOCAdvisor hc in mOCAdvisors)
                        {

                            Mocemail = hc.MOCAdvisorEmailAddress.ToString() + ',';

                            Moctemp = Moctemp + Mocemail;
                            MocemailContent = Moctemp.TrimEnd(',');
                            MOCAdvisorName = hc.MOCAdvisorName.ToString();
                        }

                        moca.MOCAdvisorEmail = MocemailContent;
                        moca.MOCAdvisorName = MOCAdvisorName;
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                MOCReasonForChange reasonCategory = new MOCReasonForChange();
                                reasonCategory.ReasonID = int.Parse(reader["ID"].ToString());
                                reasonCategory.YesNo = int.Parse(reader["ChkBox"].ToString()) > 0 ? true : false;
                                reasonCategory.ReasonForChangeName = reader["ReasonForChange"].ToString();
                                ReasonChangeList.Add(reasonCategory);

                            }
                            moca.GetMocReasonForChange = ReasonChangeList;

                        }
                    }
                    reader.Close();
                    objCon.Close();
                }

            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return moca;
        }

        public List<MOCViewModel> SearchOpenMOC(MOCSearchViewModel mocsearchviewmodel)
        {
            List<MOCViewModel> moclist = new List<MOCViewModel>();
            try
            {
                int RecordCount = 1;


                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[SearchOpenMOC]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@MOCPlant", mocsearchviewmodel.Plant);
                    objCom.Parameters.AddWithValue("@MOCStatus", mocsearchviewmodel.MOCStatus);
                    objCom.Parameters.AddWithValue("@FromDate", mocsearchviewmodel.MOCFromDate == null ? string.Empty : mocsearchviewmodel.MOCFromDate);
                    objCom.Parameters.AddWithValue("@ToDate", mocsearchviewmodel.MOCToDate == null ? string.Empty : mocsearchviewmodel.MOCToDate);
                    objCom.Parameters.AddWithValue("@MOCType", mocsearchviewmodel.MOCType);
                    objCom.Parameters.AddWithValue("@MOCCategory", mocsearchviewmodel.MOCCategory);
                    objCom.Parameters.AddWithValue("@Class", mocsearchviewmodel.ClassID);
                    objCom.Parameters.AddWithValue("@Approver", mocsearchviewmodel.ActionerID);
                    objCom.Parameters.AddWithValue("@MOCcoor", mocsearchviewmodel.MOCcoordinator);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        MOCViewModel moclist1 = new MOCViewModel();
                        moclist1.SNo = RecordCount++;
                        moclist1.MOCID = int.Parse(reader["ID"].ToString());
                        moclist1.PlantArea = reader["PlantName"].ToString();
                        moclist1.MOCType = reader["TypeName"].ToString();
                        if (reader["MOCCategory"] != DBNull.Value)
                        {
                            moclist1.MOCCategory = reader["MOCCategory"].ToString();
                        }
                        if (reader["MOCNumber"] != DBNull.Value)
                        {
                            moclist1.MOCNumber = reader["MOCNumber"].ToString();
                        }
                        moclist1.MocStatus = reader["StatusName"].ToString();

                        // moclist1.ActionTaken = reader["ActionTaken"].ToString();
                        if (reader["MOCTitle"] != DBNull.Value)
                        {
                            moclist1.MOCTitle = reader["MOCTitle"].ToString();
                        }

                        moclist1.FileName = reader["Attachments"].ToString();
                        if (reader["ApprovedDate"] != DBNull.Value)
                        {
                            moclist1.MOCCreated = reader["ApprovedDate"].ToString();
                        }

                        if (reader["UserFullName"] != DBNull.Value)
                        {
                            moclist1.Approver = reader["UserFullName"].ToString();
                        }
                        if (reader["TargetDate"] != DBNull.Value)
                        {
                            moclist1.TargetDate = reader["TargetDate"].ToString();
                        }
                        moclist1.FuncationalManagerID = int.Parse(reader["FuncationalMangerID"].ToString());
                        moclist1.MOCCreatedBy = int.Parse(reader["CreatedBy"].ToString());
                        moclist1.ClassName = reader["ClassName"].ToString();
                        if (reader["CompletedDate"] != DBNull.Value)
                        {
                            moclist1.MOCClosedDate = reader["CompletedDate"].ToString();
                        }
                        if (reader["MOCCoOrdinate"] != DBNull.Value)
                        {
                            moclist1.MOCCOOrdinate = reader["MOCCoOrdinate"].ToString();
                        }
                        if (reader["TemporaryStatus"] != DBNull.Value)
                        {
                            moclist1.TempMOCStatus = reader["TemporaryStatus"].ToString();
                        }
                        moclist.Add(moclist1);

                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return moclist;
        }

        public void SaveMocAttachments(MOCAttachment mocattachment, string fileName, int currentUser)
        {
            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "SaveMOCAttachments";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@MOCID", mocattachment.MOCId);
                    objCom.Parameters.AddWithValue("@ImageName", mocattachment.ImageFile.FileName);
                    objCom.Parameters.AddWithValue("@FileDescription", mocattachment.ImageDescription == null ? string.Empty : mocattachment.ImageDescription);
                    objCom.Parameters.AddWithValue("@FileName", fileName);
                    objCom.Parameters.AddWithValue("@UploadedUser", currentUser);

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

        public List<MOCAttachment> GetMOCAttachments(int MOCID)
        {
            List<MOCAttachment> mocattachments = new List<MOCAttachment>();
            int SNo = 1;
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetMOCAttachments]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@MOCID", MOCID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        MOCAttachment attachment = new MOCAttachment()
                        {
                            SNo = SNo++,
                            MOCAttachmentId = Convert.ToInt32(reader["ID"].ToString()),
                            ImageName = reader["ImageName"].ToString(),
                            ImageDescription = reader["ImageDescription"].ToString(),
                            FileName = reader["FileName"].ToString()
                        };
                        mocattachments.Add(attachment);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return mocattachments;
        }

        public void DeleteAttachments(int AttachmentID)
        {
            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "DeleteMOCAttachments";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@AttachmentsID", AttachmentID);

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



        public ApprovalList GetApprovalStageApprovar(int MOCID)
        {

            ApprovalList al = new ApprovalList();

            using (SqlConnection objCon = new SqlConnection(constring))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "[GetApprovalStage]";

                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@MOCID", MOCID);
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();

                while (objReader.Read())
                {

                    al.ID = int.Parse(objReader["ID"].ToString());
                    if (objReader["UpdateID"] != DBNull.Value)
                    {
                        al.UpdateID = int.Parse(objReader["UpdateID"].ToString());
                    }
                    al.DRName = objReader["FullName"].ToString();
                    al.DRUserID = int.Parse(objReader["EmployeeID"].ToString());
                    if (objReader["Remarks"] != DBNull.Value)
                    {
                        al.DRRemarks = objReader["Remarks"].ToString();
                    }
                    if (objReader["ApprovalDate"] != DBNull.Value)
                    {
                        al.DRApprovalDate = objReader["ApprovalDate"].ToString();
                    }
                    al.DRTargetDate = objReader["TargetDate"].ToString();



                }

                objReader.NextResult();

                while (objReader.Read())
                {


                    al.ID = int.Parse(objReader["ID"].ToString());
                    if (objReader["UpdateID"] != DBNull.Value)
                    {
                        al.UpdateID = int.Parse(objReader["UpdateID"].ToString());
                    }
                    al.RiskName = objReader["FullName"].ToString();
                    al.RiskUserID = int.Parse(objReader["EmployeeID"].ToString());
                    if (objReader["Remarks"] != DBNull.Value)
                    {
                        al.RiskRemarks = objReader["Remarks"].ToString();
                    }
                    if (objReader["ApprovalDate"] != DBNull.Value)
                    {
                        al.RiskApprovalDate = objReader["ApprovalDate"].ToString();
                    }
                    al.RiskTargetDate = objReader["TargetDate"].ToString();



                }
                objReader.NextResult();
                while (objReader.Read())
                {


                    al.ID = int.Parse(objReader["ID"].ToString());
                    if (objReader["UpdateID"] != DBNull.Value)
                    {
                        al.UpdateID = int.Parse(objReader["UpdateID"].ToString());
                    }
                    al.TechName = objReader["FullName"].ToString();
                    al.TechUserID = int.Parse(objReader["EmployeeID"].ToString());
                    if (objReader["Remarks"] != DBNull.Value)
                    {
                        al.TechRemarks = objReader["Remarks"].ToString();
                    }
                    if (objReader["ApprovalDate"] != DBNull.Value)
                    {
                        al.TechApprovalDate = objReader["ApprovalDate"].ToString();
                    }

                    al.TechTargetDate = objReader["TargetDate"].ToString();


                }
                objReader.NextResult();
                while (objReader.Read())
                {

                    al.ID = int.Parse(objReader["ID"].ToString());
                    if (objReader["UpdateID"] != DBNull.Value)
                    {
                        al.UpdateID = int.Parse(objReader["UpdateID"].ToString());
                    }
                    al.ExcivilName = objReader["FullName"].ToString();
                    al.CivilUserID = int.Parse(objReader["EmployeeID"].ToString());
                    if (objReader["Remarks"] != DBNull.Value)
                    {
                        al.ExcivilRemarks = objReader["Remarks"].ToString();
                    }
                    if (objReader["ApprovalDate"] != DBNull.Value)
                    {
                        al.ExcivilApprovalDate = objReader["ApprovalDate"].ToString();
                    }

                    al.ExcivilTargetDate = objReader["TargetDate"].ToString();


                }
                objReader.NextResult();
                while (objReader.Read())
                {

                    al.ID = int.Parse(objReader["ID"].ToString());
                    if (objReader["UpdateID"] != DBNull.Value)
                    {
                        al.UpdateID = int.Parse(objReader["UpdateID"].ToString());
                    }
                    al.ExMechName = objReader["FullName"].ToString();
                    al.MechUserID = int.Parse(objReader["EmployeeID"].ToString());
                    if (objReader["Remarks"] != DBNull.Value)
                    {
                        al.ExMechRemarks = objReader["Remarks"].ToString();
                    }
                    if (objReader["ApprovalDate"] != DBNull.Value)
                    {
                        al.ExMechApprovalDate = objReader["ApprovalDate"].ToString();
                    }

                    al.ExMechTargetDate = objReader["TargetDate"].ToString();


                }
                objReader.NextResult();
                while (objReader.Read())
                {

                    al.ID = int.Parse(objReader["ID"].ToString());
                    if (objReader["UpdateID"] != DBNull.Value)
                    {
                        al.UpdateID = int.Parse(objReader["UpdateID"].ToString());
                    }
                    al.ExElecName = objReader["FullName"].ToString();
                    al.ElecUserID = int.Parse(objReader["EmployeeID"].ToString());
                    if (objReader["Remarks"] != DBNull.Value)
                    {
                        al.ExElecRemarks = objReader["Remarks"].ToString();
                    }
                    if (objReader["ApprovalDate"] != DBNull.Value)
                    {
                        al.ExElecApprovalDate = objReader["ApprovalDate"].ToString();
                    }

                    al.ExElecTargetDate = objReader["TargetDate"].ToString();


                }
                objReader.NextResult();

                while (objReader.Read())
                {

                    al.ID = int.Parse(objReader["ID"].ToString());
                    if (objReader["UpdateID"] != DBNull.Value)
                    {
                        al.UpdateID = int.Parse(objReader["UpdateID"].ToString());
                    }
                    al.FacMgrName = objReader["FullName"].ToString();
                    al.FacMgrUserID = int.Parse(objReader["EmployeeID"].ToString());
                    if (objReader["Remarks"] != DBNull.Value)
                    {
                        al.FacMgrRemarks = objReader["Remarks"].ToString();
                    }
                    if (objReader["ApprovalDate"] != DBNull.Value)
                    {
                        al.FacMgrApprovalDate = objReader["ApprovalDate"].ToString();
                    }

                    al.FacMgrTargetDate = objReader["TargetDate"].ToString();


                }
                
               
               
                objReader.NextResult();

                while (objReader.Read())
                {

                    al.ID = int.Parse(objReader["ID"].ToString());
                    if (objReader["UpdateID"] != DBNull.Value)
                    {
                        al.UpdateID = int.Parse(objReader["UpdateID"].ToString());
                    }
                    al.PSSRName = objReader["FullName"].ToString();
                    al.PSSRUserID = int.Parse(objReader["EmployeeID"].ToString());
                    if (objReader["Remarks"] != DBNull.Value)
                    {
                        al.PSSRRemarks = objReader["Remarks"].ToString();
                    }
                    if (objReader["ApprovalDate"] != DBNull.Value)
                    {
                        al.PSSRApprovalDate = objReader["ApprovalDate"].ToString();
                    }

                    al.PSSRTargetDate = objReader["TargetDate"].ToString();


                }

                objReader.NextResult();

                while (objReader.Read())
                {

                    al.ID = int.Parse(objReader["ID"].ToString());
                    if (objReader["UpdateID"] != DBNull.Value)
                    {
                        al.UpdateID = int.Parse(objReader["UpdateID"].ToString());
                    }
                    al.PSSRSignName = objReader["FullName"].ToString();
                    al.PSSRSignUserID = int.Parse(objReader["EmployeeID"].ToString());
                    if (objReader["Remarks"] != DBNull.Value)
                    {
                        al.PSSRSignRemarks = objReader["Remarks"].ToString();
                    }
                    if (objReader["ApprovalDate"] != DBNull.Value)
                    {
                        al.PSSRSignApprovalDate = objReader["ApprovalDate"].ToString();
                    }

                    al.PSSRSignTargetDate = objReader["TargetDate"].ToString();


                }


                objReader.NextResult();
                while (objReader.Read())
                {
                    al.ID = int.Parse(objReader["ID"].ToString());
                    if (objReader["UpdateID"] != DBNull.Value)
                    {
                        al.UpdateID = int.Parse(objReader["UpdateID"].ToString());
                    }
                    al.BDocName = objReader["FullName"].ToString();
                    al.BDocUserID = int.Parse(objReader["EmployeeID"].ToString());
                    if (objReader["Remarks"] != DBNull.Value)
                    {
                        al.BDocRemarks = objReader["Remarks"].ToString();
                    }
                    if (objReader["ApprovalDate"] != DBNull.Value)
                    {
                        al.BDocApprovalDate = objReader["ApprovalDate"].ToString();
                    }
                    al.BDocTargetDate = objReader["TargetDate"].ToString();


                }
                objReader.Close();

                objCon.Close();



            }
            return al;
        }





        public void SaveMOCObservation(MOCObservation cpObservation)
        {
            int affectedRecordCount = 0;
            int CompletedBy = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "SaveMOCObservation";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@MOCID", cpObservation.MOCID);
                    objCom.Parameters.AddWithValue("@ObservationID", cpObservation.ObservationID);
                    objCom.Parameters.AddWithValue("@Recommendation", cpObservation.Recommendation == null ? String.Empty : cpObservation.Recommendation);
                    objCom.Parameters.AddWithValue("@ActionTaken", cpObservation.ActionTaken == null ? String.Empty : cpObservation.ActionTaken);
                    //objCom.Parameters.AddWithValue("@Comments", insObservation.Comments);
                    objCom.Parameters.AddWithValue("@TargetDate", DateTime.ParseExact(cpObservation.TargetDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    objCom.Parameters.AddWithValue("@UserID", cpObservation.CurrentUser);
                    objCom.Parameters.AddWithValue("@CategoryID", cpObservation.CategoryID);
                    objCom.Parameters.AddWithValue("@PriorityID", cpObservation.RecomPriorityID);

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


        public MCObservationViewModel EditMOCObservation(int ObsID)
        {
            MCObservationViewModel mcObservationVM = new MCObservationViewModel();
            MOCObservation observation = new MOCObservation();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetEditMOCObservation]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@ObservationID", ObsID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        observation.MOCID = int.Parse(reader["MOCID"].ToString());
                        observation.ObservationID = int.Parse(reader["ID"].ToString());
                        observation.CategoryID = int.Parse(reader["CategoryID"].ToString());
                        observation.RecomPriorityID = int.Parse(reader["PriorityID"].ToString());
                        observation.TargetDate = reader["TargetDate"].ToString();
                        observation.Recommendation = reader["Recommendation"].ToString();
                        observation.ActionTaken = reader["ActionTaken"].ToString();
                        observation.CompletedBy = int.Parse(reader["CompletedBy"].ToString());
                        observation.CompletedDate = reader["CompletedDate"].ToString();
                        observation.Remarks = reader["Remarks"].ToString();
                    }
                    mcObservationVM.mocobservation = observation;
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return mcObservationVM;
        }
        public MOCObservationEmail CheckCriticalRecomm(int MOCID)
        {
            MOCObservationEmail MOCObEm = new MOCObservationEmail();
            List<ObservationViewModelMOC> obList = new List<ObservationViewModelMOC>();

            int SNo = 1;

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[CheckMOCCriticalObservations]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@MOCID", MOCID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        var observationList = new ObservationViewModelMOC();

                        observationList.SNo = SNo++;
                        if (reader["MOCNumber"] != DBNull.Value)
                        {
                            observationList.MOCNo = reader["MOCNumber"].ToString();
                        }
                        observationList.MOCID = int.Parse(reader["MOCID"].ToString());
                        observationList.ObservationID = int.Parse(reader["ID"].ToString());
                        observationList.PriorityName = reader["Name"].ToString();
                        observationList.CompletedDate = reader["CompletedDate"].ToString();
                        observationList.TargetDate = reader["TargetDate"].ToString();
                        observationList.RecomCategory = reader["MOCRecomCategory"].ToString();
                        observationList.Recommendation = reader["Recommendation"].ToString();
                        observationList.ActionByEmail = reader["ActionByEmail"].ToString();
                        observationList.CompletedBy = int.Parse(reader["CompletedBy"].ToString());

                        obList.Add(observationList);
                    }
                    MOCObEm.obserlist = obList;
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return MOCObEm;
        }
        public MOCObservationEmail CheckMOCObservation(int MOCID)
        {
            MOCObservationEmail MOCObEm = new MOCObservationEmail();
            List<ActionBYEmailList> actionBYEmailLists = new List<ActionBYEmailList>();
            List<ObservationViewModelMOC> obList = new List<ObservationViewModelMOC>();
            string actionby = null;
            string actionbytemp = null;
            string actionbyemailcontent = null;
            int SNo = 1;

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[CheckMOCObservations]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@MOCID", MOCID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        var observationList = new ObservationViewModelMOC();

                        observationList.SNo = SNo++;
                        if (reader["MOCNumber"] != DBNull.Value)
                        {
                            observationList.MOCNo = reader["MOCNumber"].ToString();
                        }
                        observationList.MOCID = int.Parse(reader["MOCID"].ToString());
                        observationList.ObservationID = int.Parse(reader["ID"].ToString());
                        observationList.PriorityName = reader["Name"].ToString();
                        observationList.CompletedDate = reader["CompletedDate"].ToString();
                        observationList.TargetDate = reader["TargetDate"].ToString();
                        observationList.RecomCategory = reader["MOCRecomCategory"].ToString();
                        observationList.Recommendation = reader["Recommendation"].ToString();
                        observationList.ActionByEmail = reader["ActionByEmail"].ToString();
                        observationList.CompletedBy = int.Parse(reader["CompletedBy"].ToString());
                        observationList.ActionByName = reader["ActionBy"].ToString();
                        obList.Add(observationList);
                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        MOCObEm.DREmail = reader["DREmail"].ToString();
                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        MOCObEm.RAEmail = reader["RAEmail"].ToString();
                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        ActionBYEmailList actionBYEmail = new ActionBYEmailList();
                        actionBYEmail.Actionby = reader["ActionByEmail"].ToString();
                        actionBYEmail.CompletedBYID = int.Parse(reader["CompletedBy"].ToString());
                        actionBYEmailLists.Add(actionBYEmail);
                    }
                    foreach (ActionBYEmailList hc in actionBYEmailLists)
                    {

                        actionby = hc.Actionby.ToString() + ',';

                        actionbytemp = actionbytemp + actionby;
                        actionbyemailcontent = actionbytemp.TrimEnd(',');

                    }
                    MOCObEm.ActionByEmail = actionbyemailcontent;
                    MOCObEm.actionBYEmailLists = actionBYEmailLists;
                    MOCObEm.obserlist = obList;

                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return MOCObEm;
        }
        public MOCObservationEmail CheckPSSRCriticalRecomm(int MOCID)
        {
            MOCObservationEmail MocObEmail = new MOCObservationEmail();
            List<ObservationViewModelMOC> obList = new List<ObservationViewModelMOC>();

            int SNo = 1;

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[CheckMOCPSSRCriticalObservations]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@MOCID", MOCID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        var observationList = new ObservationViewModelMOC();

                        observationList.SNo = SNo++;
                        if (reader["MOCNumber"] != DBNull.Value)
                        {
                            observationList.MOCNo = reader["MOCNumber"].ToString();
                        }
                        observationList.MOCID = int.Parse(reader["MOCID"].ToString());
                        observationList.ObservationID = int.Parse(reader["ID"].ToString());
                        observationList.PriorityName = reader["Name"].ToString();
                        observationList.CompletedDate = reader["CompletedDate"].ToString();
                        observationList.TargetDate = reader["TargetDate"].ToString();
                        observationList.Recommendation = reader["Recommendation"].ToString();
                        observationList.RecomCategory = reader["RecomCategory"].ToString();

                        observationList.ActionByEmail = reader["ActionByEmail"].ToString();
                        observationList.CompletedBy = int.Parse(reader["CompletedBy"].ToString());

                        obList.Add(observationList);
                    }
                    MocObEmail.obserlist = obList;

                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return MocObEmail;
        }
        public MOCObservationEmail CheckMOCPSSRObservation(int MOCID)
        {
            MOCObservationEmail MocObEmail = new MOCObservationEmail();
            List<ActionBYEmailList> actionbyList = new List<ActionBYEmailList>();
            List<ObservationViewModelMOC> obList = new List<ObservationViewModelMOC>();
            string actionby = null;
            string actionbytemp = null;
            string actionbyemailcontent = null;
            int SNo = 1;

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[CheckMOCPSSRObservations]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@MOCID", MOCID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        var observationList = new ObservationViewModelMOC();

                        observationList.SNo = SNo++;
                        if (reader["MOCNumber"] != DBNull.Value)
                        {
                            observationList.MOCNo = reader["MOCNumber"].ToString();
                        }
                        observationList.MOCID = int.Parse(reader["MOCID"].ToString());
                        observationList.ObservationID = int.Parse(reader["ID"].ToString());
                        observationList.PriorityName = reader["Name"].ToString();
                        observationList.CompletedDate = reader["CompletedDate"].ToString();
                        observationList.TargetDate = reader["TargetDate"].ToString();
                        observationList.Recommendation = reader["Recommendation"].ToString();
                        observationList.RecomCategory = reader["RecomCategory"].ToString();

                        observationList.ActionByEmail = reader["ActionByEmail"].ToString();
                        observationList.CompletedBy = int.Parse(reader["CompletedBy"].ToString());

                        obList.Add(observationList);
                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        MocObEmail.PSSREmail = reader["PSSREmail"].ToString();
                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        MocObEmail.ExecMgr = reader["ExecMgrEmail"].ToString();
                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        ActionBYEmailList actionBYEmail = new ActionBYEmailList();
                        actionBYEmail.Actionby = reader["ActionByEmail"].ToString();
                        actionBYEmail.CompletedBYID = int.Parse(reader["CompletedBy"].ToString());
                        actionbyList.Add(actionBYEmail);
                    }
                    foreach (ActionBYEmailList hc in actionbyList)
                    {

                        actionby = hc.Actionby.ToString() + ',';

                        actionbytemp = actionbytemp + actionby;
                        actionbyemailcontent = actionbytemp.TrimEnd(',');

                    }
                    MocObEmail.ActionByEmail = actionbyemailcontent;
                    MocObEmail.actionBYEmailLists = actionbyList;
                    MocObEmail.obserlist = obList;

                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return MocObEmail;
        }

        public List<ObservationViewModelMOC> GetObservationModel(int MOCID, int ObservID)
        {
            List<ObservationViewModelMOC> obList = new List<ObservationViewModelMOC>();

            int SNo = 1;

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetMOCObservations]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@MOCID", MOCID);
                    objCom.Parameters.AddWithValue("@ObservID", ObservID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        var observationList = new ObservationViewModelMOC();

                        observationList.SNo = SNo++;
                        if (reader["MOCNumber"] != DBNull.Value)
                        {
                            observationList.MOCNo = reader["MOCNumber"].ToString();
                        }
                        observationList.MOCID = int.Parse(reader["MOCID"].ToString());
                        observationList.ObservationID = int.Parse(reader["ID"].ToString());
                        observationList.PriorityName = reader["Name"].ToString();
                        observationList.TargetDate = reader["TargetDate"].ToString();
                        observationList.Recommendation = reader["Recommendation"].ToString();
                        observationList.ActionTaken = reader["ActionTaken"].ToString();
                        observationList.CompletedUser = reader["UserFullName"].ToString();
                        observationList.CompletedDate = reader["CompletedDate"].ToString();
                        if (reader["CompletedBy"] != DBNull.Value)
                        {
                            observationList.ActionBy = int.Parse(reader["CompletedBy"].ToString());
                        }
                        obList.Add(observationList);

                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return obList;

        }

        public List<ObservationViewModelMOC> GetAllMOCObservation()
        {
            List<ObservationViewModelMOC> observationList = new List<ObservationViewModelMOC>();

            int SNo = 1;

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetAllMOCListObservation]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        var ObList = new ObservationViewModelMOC();
                        ObList.SNo = SNo++;
                        ObList.PlantName = reader["PlantName"].ToString();
                        ObList.MOCID = int.Parse(reader["MOCID"].ToString());
                        if (reader["ID"] != DBNull.Value)
                        {
                            ObList.ObservationID = int.Parse(reader["ID"].ToString());
                        }
                        if (reader["MOCNumber"] != DBNull.Value)
                        {
                            ObList.MOCNo = reader["MOCNumber"].ToString();
                        }
                        ObList.CompletedUser = reader["UserFullName"].ToString();
                        ObList.TargetDate = reader["TargetDate"].ToString();
                        ObList.Recommendation = reader["Recommendation"].ToString();
                        ObList.ActionStatus = reader["Actionstatus"].ToString();
                        ObList.ActionTaken = reader["ActionTaken"].ToString();
                        ObList.CompletedDate = reader["CompletedDate"].ToString();
                        ObList.PriorityName = reader["Name"].ToString();
                        ObList.CategoryName = reader["CategoryName"].ToString();

                        if (reader["CompletedBy"] != DBNull.Value)
                        {
                            ObList.CompletedBy = int.Parse(reader["CompletedBy"].ToString());
                        }
                        ObList.PriorityID = int.Parse(reader["PriorityID"].ToString());

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

        public void UpdateMOCStatus(int MocID, int StatusID, string CloseComments, int userID)
        {
            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "UpdateMOCStatus";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@MOCID", MocID);
                    objCom.Parameters.AddWithValue("@StatusID", StatusID);
                    objCom.Parameters.AddWithValue("@Comments", CloseComments);
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

        public List<ObservationViewModelMOC> SearchOpenMOCForObservation(MOCSearchViewModel mocsearch)
        {

            List<ObservationViewModelMOC> observationList = new List<ObservationViewModelMOC>();
            int SNo = 1;
            try
            {

                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[SearchOpenMOCForObservation]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@Plant", mocsearch.Plant);
                    objCom.Parameters.AddWithValue("@RecommPriority", mocsearch.RecomPriorityID);
                    objCom.Parameters.AddWithValue("@RecommCategory", mocsearch.RecomCategoryID);
                    objCom.Parameters.AddWithValue("@FromDate", mocsearch.MOCFromDate == null ? string.Empty : mocsearch.MOCFromDate);
                    objCom.Parameters.AddWithValue("@ToDate", mocsearch.MOCToDate == null ? string.Empty : mocsearch.MOCToDate);
                    objCom.Parameters.AddWithValue("@ActionBy", mocsearch.ActionerID);
                    objCom.Parameters.AddWithValue("@RecommStatus", mocsearch.RecomStatus);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        var ObList = new ObservationViewModelMOC();
                        ObList.SNo = SNo++;
                        ObList.PlantName = reader["PlantName"].ToString();
                        ObList.MOCID = int.Parse(reader["MOCID"].ToString());
                        if (reader["ID"] != DBNull.Value)
                        {
                            ObList.ObservationID = int.Parse(reader["ID"].ToString());
                        }
                        if (reader["MOCNumber"] != DBNull.Value)
                        {
                            ObList.MOCNo = reader["MOCNumber"].ToString();
                        }
                        ObList.ActionStatus = reader["Actionstatus"].ToString();
                        ObList.CompletedUser = reader["UserFullName"].ToString();
                        ObList.TargetDate = reader["TargetDate"].ToString();
                        ObList.Recommendation = reader["Recommendation"].ToString();
                        ObList.ActionTaken = reader["ActionTaken"].ToString();
                        ObList.CompletedDate = reader["CompletedDate"].ToString();

                        ObList.CategoryName = reader["CategoryName"].ToString();
                        ObList.PriorityName = reader["Name"].ToString();
                        if (reader["CompletedBy"] != DBNull.Value)
                        {
                            ObList.CompletedBy = int.Parse(reader["CompletedBy"].ToString());
                        }
                        ObList.PriorityID = int.Parse(reader["PriorityID"].ToString());
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

        public List<MOCViewModel> GetAllClosedMOC()
        {
            List<MOCViewModel> moclist = new List<MOCViewModel>();
            try
            {
                int RecordCount = 1;
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetAllClosedMOC]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        moclist.Add(
                            new MOCViewModel
                            {
                                SNo = RecordCount++,
                                MOCID = int.Parse(reader["ID"].ToString()),
                                MOCNumber = reader["MOCNumber"].ToString(),
                                Description = reader["MOCDescription"].ToString(),
                                MOCCreated = reader["OpenDate"].ToString(),
                                PlantArea = reader["PlantName"].ToString(),
                                MOCCategory = reader["MOCCategory"].ToString(),
                                MOCType = reader["MOCType"].ToString(),
                                FileName = reader["Attachments"].ToString(),
                                MocStatus = reader["StatusName"].ToString(),
                                ClassName = reader["ClassName"].ToString(),
                                MOCTitle = reader["MOCTitle"].ToString(),
                                MOCClosedDate = reader["CompletedDate"].ToString(),
                            });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return moclist;
        }
        public List<MOCViewModel> SearchClosedMOC(MOCSearchViewModel mocsearchviewmodel)
        {
            List<MOCViewModel> moclist = new List<MOCViewModel>();
            try
            {
                int RecordCount = 1;
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[SearchClosedMOC]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@MOCPlant", mocsearchviewmodel.Plant);
                    objCom.Parameters.AddWithValue("@FromDate", mocsearchviewmodel.MOCFromDate);
                    objCom.Parameters.AddWithValue("@ToDate", mocsearchviewmodel.MOCToDate);
                    objCom.Parameters.AddWithValue("@MOCCategory", mocsearchviewmodel.MOCCategory);
                    objCom.Parameters.AddWithValue("@Class", mocsearchviewmodel.ClassID);
                    objCom.Parameters.AddWithValue("@MOCType", mocsearchviewmodel.MOCType);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        moclist.Add(
                            new MOCViewModel
                            {
                                SNo = RecordCount++,
                                MOCID = int.Parse(reader["ID"].ToString()),
                                MOCNumber = reader["MOCNumber"].ToString(),
                                Description = reader["MOCDescription"].ToString(),
                                MOCCreated = reader["OpenDate"].ToString(),
                                PlantArea = reader["PlantName"].ToString(),
                                MOCCategory = reader["MOCCategory"].ToString(),
                                MOCType = reader["MOCType"].ToString(),
                                FileName = reader["Attachments"].ToString(),
                                MocStatus = reader["StatusName"].ToString(),
                                ClassName = reader["ClassName"].ToString(),
                                MOCTitle = reader["MOCTitle"].ToString(),
                                MOCClosedDate = reader["CompletedDate"].ToString(),
                            });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return moclist;
        }

        public MOCApproverList GetApprovalStages()
        {
            MOCApproverList mal = new MOCApproverList();
            List<ApprovalList> appl = new List<ApprovalList>();
            using (SqlConnection objCon = new SqlConnection(constring))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "[ApprovalStagesGet]";

                objCom.CommandType = CommandType.StoredProcedure;
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();

                while (objReader.Read())
                {
                    ApprovalList al = new ApprovalList();

                    al.ID = int.Parse(objReader["ID"].ToString());

                    al.ApprovalName = objReader["ApprovalName"].ToString();
                    al.IsTeamApprover = int.Parse(objReader["IsTeamApprover"].ToString());
                    al.UserID = int.Parse(objReader["DefaultAssignUser"].ToString());
                    appl.Add(al);
                    mal.ApprovalList = appl;

                }

                objReader.Close();


                objCon.Close();

                return mal;

            }
        }

        public MOCApproverList GetApprovalStagesSave(int MOCID)
        {
            MOCApproverList mal = new MOCApproverList();
            List<ApprovalList> appl = new List<ApprovalList>();
            using (SqlConnection objCon = new SqlConnection(constring))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "[ApprovalStagesSave]";

                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@MOCID", MOCID);
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();

                while (objReader.Read())
                {
                    ApprovalList al = new ApprovalList();

                    al.ID = int.Parse(objReader["ID"].ToString());

                    al.ApprovalName = objReader["ApprovalName"].ToString();

                    al.UserID = int.Parse(objReader["EmployeeID"].ToString());
                    al.ApprovalTargetDate = objReader["TargetDate"].ToString();
                    if (objReader["IsTeamApprover"] != DBNull.Value)
                    {
                        al.IsTeamApprover = int.Parse(objReader["IsTeamApprover"].ToString());
                    }
                    appl.Add(al);
                    mal.ApprovalList = appl;

                }

                objReader.Close();


                objCon.Close();

                return mal;

            }
        }

        public List<ApprovalList> SaveApprovals(MOCApproverList approverList, List<ApprovalList> ApprovalList)
        {

            string approverString = string.Empty;
            using (SqlConnection objCon = new SqlConnection(constring))
            {
                SqlCommand objCom = new SqlCommand();
                int affectedRecordCount = 0;
                List<ApproverSaveXML> approverxml = new List<ApproverSaveXML>();

                foreach (var aplist in ApprovalList)
                {

                    var xml = new ApproverSaveXML
                    {
                        ApprovalStagesID = aplist.ID,
                        UserID = aplist.UserID,
                        TargetDate = aplist.ApprovalTargetDate,
                        IsTeamApproval = aplist.IsTeamApprover

                    };
                    approverxml.Add(xml);

                }
                XmlSerializer xmlSerializer = new XmlSerializer(approverxml.GetType());

                using (StringWriter textWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(textWriter, approverxml);

                    approverString = textWriter.ToString();
                }

                objCom.CommandText = "MOCApprover";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Connection = objCon;
                objCon.Open();
                objCom.Parameters.AddWithValue("@MOCID", approverList.MOCID);
                objCom.Parameters.AddWithValue("@ApprovalListInsert", approverString);
                affectedRecordCount = objCom.ExecuteNonQuery();
                objCom.Parameters.Clear();
                objCon.Close();

            }

            return ApprovalList;
        }

        public void ApproverAdd(int MOCID, int ApproverStagesID, int EmployeeID, string TargetDate)
        {
            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "ApproverADD";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@MOCID", MOCID);
                    objCom.Parameters.AddWithValue("@ApproverStagesID", ApproverStagesID);
                    objCom.Parameters.AddWithValue("@EmployeeID", EmployeeID);
                    objCom.Parameters.AddWithValue("@TargetDate", DateTime.ParseExact(TargetDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));

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
        public MOCApproverEmail GetMOCApproverEmail(int MOCID, int UserID)
        {
            string OPEmail = null;
            string OPtemp = null;
            string OPEmailContent = null;
            string cememail = null;
            string cemtemp = null;
            string cememailcontent = null;
            MOCApproverEmail mOCApproverEmail = new MOCApproverEmail();
            List<OPHTEEmailList> oPHTEEmail = new List<OPHTEEmailList>();
            List<CivilElecMechEmail> civilElecMechEmails = new List<CivilElecMechEmail>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "GetApprovalStagesEmailAddress";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@MOCID", MOCID);
                    objCom.Parameters.AddWithValue("@UserID", UserID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();
                    while (reader.Read())
                    {
                        mOCApproverEmail.MOCID = int.Parse(reader["ID"].ToString());
                        mOCApproverEmail.ApproverEmail = reader["ApproverEmailAddress"].ToString();
                        mOCApproverEmail.NextApproverEmail = reader["NextApproverEmailAddress"].ToString();
                        mOCApproverEmail.MoccoordinateEmail = reader["MoccoordinateEmailAddress"].ToString();
                        mOCApproverEmail.FunMgrEmail = reader["FunMgrEmailAddress"].ToString();
                        mOCApproverEmail.TargetDate = reader["TargetDate"].ToString();
                        mOCApproverEmail.PriorityName = reader["PriorityName"].ToString();
                        mOCApproverEmail.ApproverName = reader["ApproverName"].ToString();
                        mOCApproverEmail.ExeMgrEmail = reader["ExecuMgrEmail"].ToString();
                        mOCApproverEmail.DREmail = reader["DesignReviewEmail"].ToString();
                        mOCApproverEmail.FacMgrEmail = reader["FacMgrEmail"].ToString();
                        mOCApproverEmail.MOCCoordinator = reader["MOCCoordinator"].ToString();
                        mOCApproverEmail.PSSRSignOffEmail = reader["PSSRSignoffEmail"].ToString();

                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        OPHTEEmailList op = new OPHTEEmailList();
                        op.OPHTEEmail = reader["EmailAddress"].ToString();
                        oPHTEEmail.Add(op);
                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        mOCApproverEmail.DRRemarks = reader["DesignRemarks"].ToString();
                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        mOCApproverEmail.DRARemarks = reader["DesignApproRemarks"].ToString();

                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        mOCApproverEmail.RiskRemarks = reader["RiskRemarks"].ToString();
                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        mOCApproverEmail.TechRemarks = reader["TechRemarks"].ToString();
                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        CivilElecMechEmail cem = new CivilElecMechEmail();
                        cem.CEMEmail = reader["EmailAddress"].ToString();
                        civilElecMechEmails.Add(cem);
                    }
                    foreach (OPHTEEmailList hc in oPHTEEmail)
                    {

                        OPEmail = hc.OPHTEEmail.ToString() + ',';

                        OPtemp = OPtemp + OPEmail;
                        OPEmailContent = OPtemp.TrimEnd(',');

                    }
                    foreach (CivilElecMechEmail ce in civilElecMechEmails)
                    {
                        cememail = ce.CEMEmail.ToString() + ',';
                        cemtemp = cemtemp + cememail;
                        cememailcontent = cemtemp.TrimEnd(',');
                    }
                    mOCApproverEmail.CEMEmail = cememailcontent;
                    mOCApproverEmail.OPHTE = OPEmailContent;
                    objCon.Close();
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return mOCApproverEmail;
        }

        public int FuncationalManagerApprovers(FuncationalManagerApprove funap, int MOCID)
        {
            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "FunctionalApproverSave";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@MOCID", MOCID);
                    objCom.Parameters.AddWithValue("@FunctionalUser", funap.UserID);
                    objCom.Parameters.AddWithValue("@FunctionalApproverID", funap.FuncationalManagerID);
                    objCom.Parameters.AddWithValue("@FunStatus", funap.ApproveStatus);
                    objCom.Parameters.AddWithValue("@Remarks", funap.Remarks);
                    objCom.Parameters.AddWithValue("@FunTargetDate", DateTime.ParseExact(funap.TargetDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    objCom.Parameters.AddWithValue("@FunApprovalDate", DateTime.ParseExact(funap.FunApprovalDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    objCom.Parameters.AddWithValue("@UpdateID", funap.ID);

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
            return affectedRecordCount;
        }


        public int TemporaryMOCApprove(TemporaryMOCList temp)
        {
            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "TemporaryInsert";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@MOCID", temp.MOCID);
                    objCom.Parameters.AddWithValue("@InitationDate ", DateTime.ParseExact(temp.InitiationDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    objCom.Parameters.AddWithValue("@FirstTargetDate", DateTime.ParseExact(temp.FirstTargetDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    objCom.Parameters.AddWithValue("@RevisedTargetDate", DateTime.ParseExact(temp.RevisedTargetDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    objCom.Parameters.AddWithValue("@ReasonExtension", temp.ReasonExtension);
                    objCom.Parameters.AddWithValue("@TemporaryStatus", temp.TempStatus);
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
            return affectedRecordCount;
        }

        public void UpdateTemporaryMOCStatus(int MOCID, int StatusID, string CloseComments, int userID)
        {
            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "UpdateTemporaryMOCStatus";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@MOCID", MOCID);
                    objCom.Parameters.AddWithValue("@StatusID", StatusID);
                    objCom.Parameters.AddWithValue("@Comments", CloseComments);
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

        public List<GetMOCClosureList> GetMOCClosureList(int MOCID)
        {
            List<GetMOCClosureList> MOCClosure = new List<GetMOCClosureList>();
            try
            {

                using (SqlConnection objcon = new SqlConnection(AppConfig.ConnectionString))
                {
                    var objCom = new SqlCommand();
                    objCom.CommandText = "[GetMOCClosureList]";
                    objCom.Parameters.AddWithValue("@MOCID", MOCID);
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objcon;
                    objcon.Open();
                    reader = objCom.ExecuteReader();
                    while (reader.Read())
                    {
                        GetMOCClosureList getMOCClosure = new GetMOCClosureList();
                        getMOCClosure.MOCID = reader["MOCID"].GetHashCode();
                        getMOCClosure.MOCClosureId = reader["MOCClosureId"].GetHashCode();
                        getMOCClosure.Status = reader["ClosureStatus"].GetHashCode();
                        getMOCClosure.Name = reader["NAME"].ToString();
                        getMOCClosure.Remarks = reader["Remarks"].ToString();
                        MOCClosure.Add(getMOCClosure);

                    }
                    objcon.Close();
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return MOCClosure;
        }

        public List<GetMOCClosureList> SaveMOCClosureList(MOCClosureList mocclosure, List<GetMOCClosureList> GetMOCClosureList)
        {

            string getMOCliststring = string.Empty;

            using (SqlConnection objcon = new SqlConnection(constring))
            {
                SqlCommand objCom = new SqlCommand();
                int RecordCount = 0;
                List<MOCClosureXML> MOCClosurexml = new List<MOCClosureXML>();
                foreach (var moclist in GetMOCClosureList)
                {

                    var xml = new MOCClosureXML
                    {
                        MOCClosureId = moclist.MOCClosureId,
                        SaveStatus = moclist.Status,
                        SaveRemarks = moclist.Remarks


                    };
                    MOCClosurexml.Add(xml);

                }

                XmlSerializer xmlserializer = new XmlSerializer(MOCClosurexml.GetType());
                using (StringWriter textWriter = new StringWriter())
                {
                    xmlserializer.Serialize(textWriter, MOCClosurexml);
                    getMOCliststring = textWriter.ToString();
                }

                objCom.CommandText = "MOCClosureInsert";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Connection = objcon;
                objcon.Open();
                objCom.Parameters.AddWithValue("@MOCID", mocclosure.MOCID);
                objCom.Parameters.AddWithValue("@MOCClosureSave", getMOCliststring);
                RecordCount = objCom.ExecuteNonQuery();
                objCom.Parameters.Clear();
                objcon.Close();
            }

            return GetMOCClosureList;
        }

        public List<MOCMonthlyChart> GetMonthlyMOCStatusCount()
        {
            List<MOCMonthlyChart> monthlycount = new List<MOCMonthlyChart>();
            try
            {
                using (SqlConnection objcon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetMonthlyMOCStatuschart]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objcon;
                    objcon.Open();
                    reader = objCom.ExecuteReader();
                    while (reader.Read())
                    {
                        MOCMonthlyChart monthcount = new MOCMonthlyChart();
                        {
                            monthcount.MonthName = reader["MocMonth"].ToString();
                            monthcount.TotalCount = int.Parse(reader["MonthlyCount"].ToString());
                            monthcount.Permanent = int.Parse(reader["Permanent"].ToString());
                            monthcount.Temporary = int.Parse(reader["Temporary"].ToString());
                        };
                        monthlycount.Add(monthcount);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return monthlycount;
        }


        public List<PlantWise> GetPlantwiseCount()
        {
            List<PlantWise> objplantcount = new List<PlantWise>();
            try
            {
                using (SqlConnection con = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "GetPlantWise";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = con;
                    con.Open();
                    reader = objCom.ExecuteReader();
                    while (reader.Read())
                    {
                        PlantWise plant = new PlantWise()
                        {
                            PlantName = reader["PlantName"].ToString(),
                            TotalCount = int.Parse(reader["Categorycount"].ToString())

                        };
                        if (plant.TotalCount > 0)
                            objplantcount.Add(plant);
                    }

                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return objplantcount;
        }

        public List<MocCategoryCount> GetMocCategoryCount()
        {
            List<MocCategoryCount> objcategory = new List<MocCategoryCount>();
            try
            {
                using (SqlConnection con = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "GetMOCCategoryOfChange";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = con;
                    con.Open();
                    reader = objCom.ExecuteReader();
                    while (reader.Read())
                    {
                        MocCategoryCount objcount = new MocCategoryCount()
                        {
                            CategoryName = reader["CategoryName"].ToString(),
                            TotalCount = int.Parse(reader["Categorycount"].ToString())
                        };
                        if (objcount.TotalCount > 0)
                            objcategory.Add(objcount);
                    }

                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return objcategory;
        }

        public List<MOCpriorityCount> GetMocPriorityCount()
        {
            List<MOCpriorityCount> PriorityCount = new List<MOCpriorityCount>();
            try
            {
                using (SqlConnection objcon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "GetMOCpriorityStatuschart";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objcon;
                    objcon.Open();
                    reader = objCom.ExecuteReader();
                    while (reader.Read())
                    {
                        MOCpriorityCount priocount = new MOCpriorityCount();
                        {
                            priocount.Priority = reader["Name"].ToString();
                            priocount.TotalCount = int.Parse(reader["CategoryCount"].ToString());
                            priocount.StatusOpen = int.Parse(reader["StatusOpen"].ToString());
                            priocount.StatusClose = int.Parse(reader["StatusClosed"].ToString());
                        };
                        PriorityCount.Add(priocount);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return PriorityCount;
        }

        public List<MOCOverallStatus> GetMOCOverallStatus()
        {
            List<MOCOverallStatus> mocstatus = new List<MOCOverallStatus>();
            try
            {
                using (SqlConnection objcon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "GetOverallMOCStatus";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objcon;
                    objcon.Open();
                    reader = objCom.ExecuteReader();
                    while (reader.Read())
                    {
                        MOCOverallStatus objstatus = new MOCOverallStatus()
                        {
                            StatusName = reader["StatusName"].ToString(),
                            totalCount = int.Parse(reader["TotalCount"].ToString())
                        };
                        if (objstatus.totalCount > 0)
                            mocstatus.Add(objstatus);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }

            return mocstatus;
        }

        public List<MOCRecommandStatus> GetMOCRecommandStatus()
        {
            List<MOCRecommandStatus> mocrecommand = new List<MOCRecommandStatus>();
            try
            {
                using (SqlConnection objcon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetMOCRecommendStatuscChart]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objcon;
                    objcon.Open();
                    reader = objCom.ExecuteReader();
                    while (reader.Read())
                    {
                        MOCRecommandStatus moccount = new MOCRecommandStatus();
                        {
                            moccount.StatusName = reader["Name"].ToString();
                            moccount.TotalCount = int.Parse(reader["TotalCount"].ToString());
                            moccount.StatusOpen = int.Parse(reader["opened"].ToString());

                            moccount.StatusClose = int.Parse(reader["closed"].ToString());
                            moccount.Overdue = int.Parse(reader["overdue"].ToString());
                        };
                        mocrecommand.Add(moccount);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return mocrecommand;
        }

        public List<TemporaryMOCList> SearchTempMOC(MOCSearchViewModel mocsearchviewmodel)
        {
            List<TemporaryMOCList> templist = new List<TemporaryMOCList>();
            try
            {



                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[SearchTemporaryMOC]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@FromDate", mocsearchviewmodel.MOCFromDate == null ? string.Empty : mocsearchviewmodel.MOCFromDate);
                    objCom.Parameters.AddWithValue("@ToDate", mocsearchviewmodel.MOCToDate == null ? string.Empty : mocsearchviewmodel.MOCToDate);
                    objCom.Parameters.AddWithValue("@MOCNumber", mocsearchviewmodel.MOCNumber == null ? string.Empty : mocsearchviewmodel.MOCNumber);
                    objCom.Parameters.AddWithValue("@Plant", mocsearchviewmodel.Plant);
                    objCom.Parameters.AddWithValue("@MOCCoordinate", mocsearchviewmodel.MOCcoordinator);
                    objCom.Parameters.AddWithValue("@TempStatus", mocsearchviewmodel.MOCTempStatus);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        TemporaryMOCList al = new TemporaryMOCList();

                        al.MOCID = int.Parse(reader["ID"].ToString());
                        al.InitiationDate = reader["InitiationDate"].ToString();
                        al.FirstTargetDate = reader["FirstTargetDate"].ToString();
                        al.CloseComments = reader["CloseComments"].ToString();

                        if (reader["RevisedTargetDate"] != DBNull.Value)
                        {
                            al.RevisedTargetDate = reader["RevisedTargetDate"].ToString();
                        }
                        if (reader["ReasonForExtension"] != DBNull.Value)
                        {
                            al.ReasonExtension = reader["ReasonForExtension"].ToString();
                        }
                        if (reader["TempStatus"] != DBNull.Value)
                        {
                            al.TempStatus = reader["TempStatus"].ToString();
                        }
                        if (reader["ApproverComments"] != DBNull.Value)
                        {
                            al.ApproverComments = reader["ApproverComments"].ToString();
                        }
                        if (reader["FactoryManagerName"] != DBNull.Value)
                        {
                            al.ApproverName = reader["FactoryManagerName"].ToString();
                        }
                        al.MOCCoordinate = reader["MOCCoordinate"].ToString();
                        al.plant = reader["Plant"].ToString();
                        al.MOCNumber = reader["MOCNumber"].ToString();
                        al.MOCTitle = reader["MOCTitle"].ToString();
                        al.FactoryManagerID = int.Parse(reader["FactoryManagerID"].ToString());
                        templist.Add(al);

                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return templist;
        }
    }
}




