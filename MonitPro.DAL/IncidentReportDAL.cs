
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
using MonitPro.Models.CAPAViewModel;
using MonitPro.Models;
using System.IO;
using MonitPro.Models.MOC;

namespace MonitPro.DAL
{
    public class IncidentReportDAL
    {
        string constring = AppConfig.ConnectionString;
        SqlCommand objCom;
        SqlDataReader reader;

        #region "Get Stored Procs"
        public List<IncidentType> GetIncidentTypes()
        {
            List<IncidentType> IncidentTypeList = new List<IncidentType>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetIncidentType]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    if (reader.HasRows)
                    {

                        IncidentTypeList.Add(new IncidentType { ID = -1, Name = "All" });
                    }

                    while (reader.Read())
                    {
                        IncidentTypeList.Add(new IncidentType { ID = int.Parse(reader["ID"].ToString()), Name = reader["Name"].ToString(), Description = reader["Description"].ToString() });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return IncidentTypeList;
        }

        public List<IncidentCategoryDecision> GetIncidentCategoryDecisions(int DecisionType, int IncidentID)
        {
            List<IncidentCategoryDecision> decisionList = new List<IncidentCategoryDecision>();
            try
            {
                using (SqlConnection conn = new SqlConnection(constring))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "GetIncidentCategoryDecision";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DecisionTypeID", DecisionType);
                    cmd.Parameters.AddWithValue("@IncidentID", IncidentID);
                    cmd.Connection = conn;
                    conn.Open();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        IncidentCategoryDecision decision = new IncidentCategoryDecision();
                        decision.QuestionID = int.Parse(reader["QuestionID"].ToString());
                        decision.Description = reader["Description"].ToString();
                        decision.UserValue = int.Parse(reader["UserValue"].ToString());
                        if (reader["Remarks"] != DBNull.Value)
                        {
                            decision.Remarks = reader["Remarks"].ToString();
                        }
                        decision.DescriptionIdentity = int.Parse(reader["DescriptionIdentity"].ToString());
                        decisionList.Add(decision);
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return decisionList;
        }
        public List<ChemicalQTY> GetIncidentChemicalQTYDetails(int IncidentID)
        {
            List<ChemicalQTY> chemicalList = new List<ChemicalQTY>();
            try
            {
                using (SqlConnection conn = new SqlConnection(constring))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "GetIncidentChemicalQTYDetails";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IncidentID", IncidentID);
                    cmd.Connection = conn;
                    conn.Open();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ChemicalQTY chemical = new ChemicalQTY();
                        chemical.ChemicalID = int.Parse(reader["ChemicalID"].ToString());
                        chemical.ChemicalName = reader["ChemicalName"].ToString();
                        chemical.UserValue = int.Parse(reader["UserValue"].ToString());
                        chemical.Tier1Indoor = int.Parse(reader["Tier1Indoor"].ToString());
                        chemical.Tier1Outdoor = int.Parse(reader["Tier1Outdoor"].ToString());
                        chemical.Tier2Indoor = int.Parse(reader["Tier2Indoor"].ToString());
                        chemical.Tier1Outdoor = int.Parse(reader["Tier2Outdoor"].ToString());
                        chemicalList.Add(chemical);
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);

            }
            return chemicalList;
        }
        public CategoryCalculation GetCategoryCalculation(int IncidentID, int DecisionTypeID)
        {
            CategoryCalculation calculation = new CategoryCalculation();
            try
            {
                using (SqlConnection conn = new SqlConnection(constring))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "[IncidentCategoryDecisionCalculation]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IncidentID", IncidentID);
                    cmd.Parameters.AddWithValue("@DecisionTypeID", DecisionTypeID);
                    cmd.Connection = conn;
                    conn.Open();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        calculation.Total = int.Parse(reader["CalculationValue"].ToString());
                        calculation.CategoryName = reader["DecisionCategory"].ToString();
                    }
                    conn.Close();

                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return calculation;
        }

        public int IncidentCategoryInsert(IncidentMaincategoryModel mainmodel)
        {
            int affect = 0;
            string planDataString = null;
            try
            {
                if (mainmodel.DecisionTypeID != 5 && mainmodel.DecisionTypeID != 6)
                {
                    using (SqlConnection conn = new SqlConnection(constring))
                    {
                        SqlCommand cmd = new SqlCommand();
                        List<IncidentCategoryDecisionXML> decisionxml = new List<IncidentCategoryDecisionXML>();
                        if (mainmodel.decisionlist.Count > 0)
                        {

                            foreach (var i in mainmodel.decisionlist)
                            {
                                
                                    var xmllist = new IncidentCategoryDecisionXML
                                    {

                                        descriptionIdentity = i.DescriptionIdentity,
                                        QuestionID = i.QuestionID,
                                        Description = i.Description,
                                        UserValue = i.UserValue,
                                        Remarks = i.Remarks,
                                    };
                                    decisionxml.Add(xmllist);
                                
                            }
                        }
                        XmlSerializer xmlSerializer = new XmlSerializer(decisionxml.GetType());

                        using (StringWriter textWriter = new StringWriter())
                        {
                            xmlSerializer.Serialize(textWriter, decisionxml);
                            planDataString = textWriter.ToString();
                        }

                        cmd.CommandText = "[IncidentCategoryDecisionInsert]";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@DecisionData", planDataString);
                        cmd.Parameters.AddWithValue("@IncidentID", mainmodel.IncidentID);
                        cmd.Parameters.AddWithValue("@UserID", mainmodel.CurrentUserID);
                        cmd.Parameters.AddWithValue("@DecisionTypeID", mainmodel.DecisionTypeID);
                        cmd.Connection = conn;
                        conn.Open();
                        affect = cmd.ExecuteNonQuery();
                        conn.Close();


                    }
                }
                else if (mainmodel.DecisionTypeID == 5)
                {
                    using (SqlConnection conn = new SqlConnection(constring))
                    {
                        SqlCommand cmd = new SqlCommand();
                        List<API754XML> apixml = new List<API754XML>();
                        if (mainmodel.Api754List.Count > 0)
                        {

                            foreach (var i in mainmodel.Api754List)
                            {
                               
                                    var xmllist = new API754XML
                                    {
                                        QuestionID = i.QID,
                                        Description = i.Description,
                                        UserValue = i.UserValue
                                    };
                                    apixml.Add(xmllist);
                                
                            }
                        }
                        XmlSerializer xmlSerializer = new XmlSerializer(apixml.GetType());

                        using (StringWriter textWriter = new StringWriter())
                        {
                            xmlSerializer.Serialize(textWriter, apixml);
                            planDataString = textWriter.ToString();
                        }

                        cmd.CommandText = "[IncidentAPI754Insert]";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@APIData", planDataString);
                        cmd.Parameters.AddWithValue("@IncidentID", mainmodel.IncidentID);
                        cmd.Parameters.AddWithValue("@UserID", mainmodel.CurrentUserID);
                        cmd.Connection = conn;
                        conn.Open();
                        affect = cmd.ExecuteNonQuery();
                        conn.Close();


                    }
                }
                else if (mainmodel.DecisionTypeID == 6)
                {
                    using (SqlConnection conn = new SqlConnection(constring))
                    {
                        SqlCommand cmd = new SqlCommand();
                        List<ChemicalQTYXML> chemicalxml = new List<ChemicalQTYXML>();
                        if (mainmodel.ChemicalList.Count > 0)
                        {

                            foreach (var i in mainmodel.ChemicalList)
                            {
                                if (i.UserValue > 0)
                                {
                                    var xmllist = new ChemicalQTYXML
                                    {
                                        ChemicalID = i.ChemicalID,
                                        ChemicalValue = i.UserValue,
                                    };
                                    chemicalxml.Add(xmllist);
                                }
                            }
                        }
                        XmlSerializer xmlSerializer = new XmlSerializer(chemicalxml.GetType());

                        using (StringWriter textWriter = new StringWriter())
                        {
                            xmlSerializer.Serialize(textWriter, chemicalxml);
                            planDataString = textWriter.ToString();
                        }

                        cmd.CommandText = "[IncidentChemicalQTYInsert]";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ChemicalXml", planDataString);
                        cmd.Parameters.AddWithValue("@IncidentID", mainmodel.IncidentID);
                        cmd.Parameters.AddWithValue("@UserID", mainmodel.CurrentUserID);
                        cmd.Parameters.AddWithValue("@IncidentChemicalQTYType", mainmodel.IncidentChemicalQTYType);
                        cmd.Connection = conn;
                        conn.Open();
                        affect = cmd.ExecuteNonQuery();
                        conn.Close();


                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return affect;
        }

        public List<DecisionTypeDD> GetDecisionTypesDD(int IncidentID)
        {
            List<DecisionTypeDD> Decisionlist = new List<DecisionTypeDD>();
            try
            {
                using (SqlConnection conn = new SqlConnection(constring))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "Incident_GetDecisionTypeDD";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IncidentID", IncidentID);
                    cmd.Connection = conn;
                    conn.Open();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        DecisionTypeDD decisionType = new DecisionTypeDD();
                        decisionType.DecisionTypeID = int.Parse(reader["DecisionTypeID"].ToString());
                        decisionType.DecisionTypeName = reader["DecisionTypeName"].ToString();
                        decisionType.Status = int.Parse(reader["IdentityStatus"].ToString());
                        Decisionlist.Add(decisionType);
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return Decisionlist;
        }
        public List<API754Details> GetAPI754Details(int IncidentID)
        {
            List<API754Details> APIList = new List<API754Details>();
            try
            {
                using (SqlConnection conn = new SqlConnection(constring))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "GetIncidentAPI754Details";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IncidentID", IncidentID);
                    cmd.Connection = conn;
                    conn.Open();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        API754Details details = new API754Details();
                        details.QID = int.Parse(reader["QID"].ToString());
                        details.Description = reader["Description"].ToString();
                        details.RedirectionID = int.Parse(reader["RedirectionID"].ToString());
                        details.Result = reader["Result"].ToString();
                        details.UserValue = int.Parse(reader["UserValue"].ToString());
                        APIList.Add(details);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return APIList;
        }
        public int FishBoneInsert(FishBone fish)
        {
            int affect = 0;
            try
            {
                using(SqlConnection conn = new SqlConnection(constring))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "Incident_FishBoneInsert";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = conn;
                    cmd.Parameters.AddWithValue("@IncidentID", fish.IncidentID);
                    cmd.Parameters.AddWithValue("@Header1", fish.Header1);
                    cmd.Parameters.AddWithValue("@ManSub1", fish.ManSub1);
                    cmd.Parameters.AddWithValue("@ManSub2", fish.ManSub2);
                    cmd.Parameters.AddWithValue("@ManSub3", fish.ManSub3);
                    cmd.Parameters.AddWithValue("@ManSub4", fish.ManSub4);
                    cmd.Parameters.AddWithValue("@ManSub5", fish.ManSub5);
                    cmd.Parameters.AddWithValue("@Header2", fish.Header2);
                    cmd.Parameters.AddWithValue("@MachSub1", fish.MachiSub1);
                    cmd.Parameters.AddWithValue("@MachSub2", fish.MachiSub2);
                    cmd.Parameters.AddWithValue("@MachSub3", fish.MachiSub3);
                    cmd.Parameters.AddWithValue("@MachSub4", fish.MachiSub4);
                    cmd.Parameters.AddWithValue("@MachSub5", fish.MachiSub5);
                    cmd.Parameters.AddWithValue("@Header3", fish.Header3);
                    cmd.Parameters.AddWithValue("@MethodSub1", fish.MethodSub1);
                    cmd.Parameters.AddWithValue("@MethodSub2", fish.MethodSub2);
                    cmd.Parameters.AddWithValue("@MethodSub3", fish.MethodSub3);
                    cmd.Parameters.AddWithValue("@MethodSub4", fish.MethodSub4);
                    cmd.Parameters.AddWithValue("@MethodSub5", fish.MethodSub5);
                    cmd.Parameters.AddWithValue("@Header4", fish.Header4);
                    cmd.Parameters.AddWithValue("@MaterialSub1", fish.MaterialSub1);
                    cmd.Parameters.AddWithValue("@MaterialSub2", fish.MaterialSub2);
                    cmd.Parameters.AddWithValue("@MaterialSub3", fish.MaterialSub3);
                    cmd.Parameters.AddWithValue("@MaterialSub4", fish.MaterialSub4);
                    cmd.Parameters.AddWithValue("@MaterialSub5", fish.MaterialSub5);
                    cmd.Parameters.AddWithValue("@Header5", fish.Header5);
                    cmd.Parameters.AddWithValue("@MeasureSub1", fish.MeasureSub1);
                    cmd.Parameters.AddWithValue("@MeasureSub2", fish.MeasureSub2);
                    cmd.Parameters.AddWithValue("@MeasureSub3", fish.MeasureSub3);
                    cmd.Parameters.AddWithValue("@MeasureSub4", fish.MeasureSub4);
                    cmd.Parameters.AddWithValue("@MeasureSub5", fish.MeasureSub5);
                    cmd.Parameters.AddWithValue("@Header6", fish.Header6);
                    cmd.Parameters.AddWithValue("@EnviSub1", fish.EnviSub1);
                    cmd.Parameters.AddWithValue("@EnviSub2", fish.EnviSub2);
                    cmd.Parameters.AddWithValue("@EnviSub3", fish.EnviSub3);
                    cmd.Parameters.AddWithValue("@EnviSub4", fish.EnviSub4);
                    cmd.Parameters.AddWithValue("@EnviSub5", fish.EnviSub5);
                    cmd.Parameters.AddWithValue("@UserID", fish.CurrentUserID);
                    if (fish.FishImage != null)
                    {
                        cmd.Parameters.AddWithValue("@FishImg", fish.FishImage);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@FishImg", DBNull.Value);
                    }
                    conn.Open();
                    affect = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch(Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return affect;
        }
        public FishBone GetFishBoneDetails(int IncidentID)
        {
            FishBone fish = new FishBone();
            try
            {
                using(SqlConnection conn = new SqlConnection(constring))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "GetIncidentFishBoneDetails";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IncidentID", IncidentID);
                    cmd.Connection = conn;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        fish.Header1 = reader["Header1"].ToString();
                        fish.ManSub1 = reader["ManSub1"].ToString();
                        fish.ManSub2 = reader["ManSub2"].ToString();
                        fish.ManSub3 = reader["ManSub3"].ToString();
                        fish.ManSub4 = reader["ManSub4"].ToString(); 
                        fish.ManSub5 = reader["ManSub5"].ToString(); 
                        fish.Header2 = reader["Header2"].ToString();
                        fish.MachiSub1 = reader["MachineSub1"].ToString(); 
                        fish.MachiSub2 = reader["MachineSub2"].ToString();
                        fish.MachiSub3 = reader["MachineSub3"].ToString();
                        fish.MachiSub4 = reader["MachineSub4"].ToString();
                        fish.MachiSub5 = reader["MachineSub5"].ToString();
                        fish.Header3 = reader["Header3"].ToString();
                        fish.MethodSub1 = reader["MethodSub1"].ToString();
                        fish.MethodSub2 = reader["MethodSub2"].ToString();
                        fish.MethodSub3 = reader["MethodSub3"].ToString();
                        fish.MethodSub4 = reader["MethodSub4"].ToString();
                        fish.MethodSub5 = reader["MethodSub5"].ToString();
                        fish.Header4 = reader["Header4"].ToString();
                        fish.MaterialSub1 = reader["MaterialSub1"].ToString();
                        fish.MaterialSub2 = reader["MaterialSub2"].ToString();
                        fish.MaterialSub3 = reader["MaterialSub3"].ToString();
                        fish.MaterialSub4 = reader["MaterialSub4"].ToString();
                        fish.MaterialSub5 = reader["MaterialSub5"].ToString();
                        fish.Header5 = reader["Header5"].ToString();
                        fish.MeasureSub1 = reader["MeasureSub1"].ToString();
                        fish.MeasureSub2 = reader["MeasureSub2"].ToString();
                        fish.MeasureSub3 = reader["MeasureSub3"].ToString();
                        fish.MeasureSub4 = reader["MeasureSub4"].ToString();
                        fish.MeasureSub5 = reader["MeasureSub5"].ToString();
                        fish.Header6 = reader["Header6"].ToString();
                        fish.EnviSub1 = reader["EnviSub1"].ToString();
                        fish.EnviSub2 = reader["EnviSub2"].ToString();
                        fish.EnviSub3 = reader["EnviSub3"].ToString();
                        fish.EnviSub4 = reader["EnviSub4"].ToString();
                        fish.EnviSub5 = reader["EnviSub5"].ToString();
                        if (reader["FishImage"] != DBNull.Value)
                        {
                            fish.FishImage = reader["FishImage"].ToString();
                        }

                    }
                    conn.Close();

                }
            }
            catch(Exception ex )
            {

                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return fish;
        }

        public List<Status> GetIncidentStatus()
        {
            List<Status> StatusList = new List<Status>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetIncidentStats]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    if (reader.HasRows)
                    {
                        StatusList.Add(new Status { ID = 0, Name = "All" });
                    }

                    while (reader.Read())
                    {
                        StatusList.Add(new Status { ID = int.Parse(reader["ID"].ToString()), Name = reader["Name"].ToString(), Description = reader["Description"].ToString() });
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

        public List<IncidentClassfication> GetIncidentClassfication()
        {
            List<IncidentClassfication> IncidentClassficationList = new List<IncidentClassfication>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetIncidentClassfication]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    //if (reader.HasRows)
                    //{
                    //    IncidentClassficationList.Add(new IncidentClassfication { ID = "", Name = "--Select--" });
                    //}

                    while (reader.Read())
                    {
                        IncidentClassficationList.Add(new IncidentClassfication { ID = reader["ID"].ToString(), Name = reader["Name"].ToString(), Description = reader["Description"].ToString() });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return IncidentClassficationList;
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
                        PlantsList.Add(new Plants { 
                            ID = int.Parse(reader["ID"].ToString()),
                            Name = reader["Name"].ToString(),
                            Description = reader["Description"].ToString(),
                            AreaOwner = int.Parse(reader["FirstOwner"].ToString())
                        });
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

        public List<Priority> GetPriorities()
        {
            List<Priority> PriorityList = new List<Priority>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetPriority]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();
                    if (reader.HasRows)
                    {
                        PriorityList.Add(new Priority { ID = "", Name = "--Select--" });
                    }
                    while (reader.Read())
                    {
                        PriorityList.Add(new Priority { ID = reader["ID"].ToString(), Name = reader["Name"].ToString(), Description = reader["Description"].ToString() });
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
        #region GetDeptEmployees
        public List<Employee> GetDeptEmployees(int? deptID)
        {
            List<Employee> employeelist = new List<Employee>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "GetDeptEmployeeSelect";

                    if (deptID > 0)
                        objCom.Parameters.AddWithValue("@DeptID", deptID);
                    else
                        objCom.Parameters.AddWithValue("@DeptID", DBNull.Value);

                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();

                    SqlDataReader Results = objCom.ExecuteReader();

                    while (Results.Read())
                    {
                        Employee DeptEmp = new Employee();
                        DeptEmp.DeptEmpID = Results[0].ToString();
                        DeptEmp.FullName = Results[1].ToString();
                        employeelist.Add(DeptEmp);
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

            return employeelist;

        }
        #endregion
        public List<InjuryType> GetInjuryTypes()
        {
            List<InjuryType> InjuryTypeList = new List<InjuryType>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetInjuryTypes]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    if (reader.HasRows)
                    {
                        InjuryTypeList.Add(new InjuryType { ID = "", Name = "--Select--" });
                    }

                    while (reader.Read())
                    {
                        InjuryTypeList.Add(new InjuryType { ID = reader["ID"].ToString(), Name = reader["Name"].ToString(), Description = reader["Description"].ToString() });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return InjuryTypeList;
        }


        public List<ClassficationFactor> GetClassficationFactor()
        {
            List<ClassficationFactor> FactorsList = new List<ClassficationFactor>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetClassficationFactor]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    if (reader.HasRows)
                    {
                        FactorsList.Add(new ClassficationFactor { ID = "", Name = "--Select--" });
                    }

                    while (reader.Read())
                    {
                        FactorsList.Add(new ClassficationFactor { ID = reader["ID"].ToString(), Name = reader["Name"].ToString(), Description = reader["Description"].ToString() });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return FactorsList;
        }


        public NewIncidentViewModel GetSelectedRootCause(int incidentID)
        {
            NewIncidentViewModel RootCauseList = new NewIncidentViewModel();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[SelectedRootCause]";
                    objCom.Parameters.AddWithValue("@IncidentID", incidentID);
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    SqlDataReader reader = objCom.ExecuteReader();

                    var RootCauseMaster = new List<RootCauseMaster>();


                    while (reader.Read())
                    {
                        var MList = new RootCauseMaster();

                        MList.maincheck = int.Parse(reader["Checked"].ToString()) > 0 ? true : false;
                        MList.RootCauseID = int.Parse(reader["ID"].ToString());
                        MList.Name = reader["Name"].ToString();
                        MList.SubList = SelectedSubList(int.Parse(reader["ID"].ToString()), incidentID);
                        RootCauseMaster.Add(MList);

                    }
                    RootCauseList = new NewIncidentViewModel();
                    RootCauseList.RootCauseMasterList = RootCauseMaster;

                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return RootCauseList;
        }
        public List<RootCauseSubsection> SelectedSubList(int id, int incidentID)
        {
            List<RootCauseSubsection> selectedSubList = new List<RootCauseSubsection>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[SelectedSubList]";
                    objCom.Parameters.AddWithValue("@id", id);
                    objCom.Parameters.AddWithValue("@incidentID", incidentID);
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    objCom.Connection = objCon;
                    reader = objCom.ExecuteReader();


                    while (reader.Read())
                    {
                        selectedSubList.Add(new RootCauseSubsection
                        {
                            subcheck = int.Parse(reader["Checked"].ToString()) > 0 ? true : false,
                            SubsectionID = int.Parse(reader["SubsectionID"].ToString()),
                            RootCauseID = int.Parse(reader["RootCauseID"].ToString()),
                            Name = reader["Name"].ToString(),
                            RootCauseName = reader["RootCauseName"].ToString(),
                        });
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

            return selectedSubList;
        }
        public List<TenetsList> SaveTenets(NewIncidentViewModel incidentVM, List<TenetsList> Tenets)
        {

            string TenetString = string.Empty;
            using (SqlConnection objCon = new SqlConnection(constring))
            {
                SqlCommand objCom = new SqlCommand();
                int affectedRecordCount = 0;
                List<TenetsXML> tnetsxml = new List<TenetsXML>();

                foreach (var tnets in Tenets)
                {
                    if (tnets.YesNo == true)
                    {
                        var xml = new TenetsXML
                        {
                            TenetsID = tnets.ID,
                            TenetsName = tnets.Name,
                            Details = tnets.Details

                        };
                        tnetsxml.Add(xml);
                    }

                }
                XmlSerializer xmlSerializer = new XmlSerializer(tnetsxml.GetType());

                using (StringWriter textWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(textWriter, tnetsxml);

                    TenetString = textWriter.ToString();
                }

                objCom.CommandText = "Tenet";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Connection = objCon;
                objCon.Open();
                objCom.Parameters.AddWithValue("@IncidentID", incidentVM.IncidentID);
                objCom.Parameters.AddWithValue("@TenetSave", TenetString);
                affectedRecordCount = objCom.ExecuteNonQuery();
                objCom.Parameters.Clear();
                objCon.Close();

            }


            return Tenets;
        }


        public List<Tenets4> Save4Tenets(NewIncidentViewModel incidentVM, List<Tenets4> Tenets4)
        {

            string Tenet4String = string.Empty;
            using (SqlConnection objCon = new SqlConnection(constring))
            {
                SqlCommand objCom = new SqlCommand();
                int affectedRecordCount = 0;
                List<Tenets4XML> tnets4xml = new List<Tenets4XML>();

                foreach (var tnets4 in Tenets4)
                {
                    if (tnets4.YesNo == true)
                    {
                        var xml4 = new Tenets4XML
                        {
                            TemplateID = tnets4.TemplateID,
                            Tenets4ID = tnets4.ID,
                            Name = tnets4.Name

                        };
                        tnets4xml.Add(xml4);
                    }

                }
                XmlSerializer xmlSerializer = new XmlSerializer(tnets4xml.GetType());

                using (StringWriter textWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(textWriter, tnets4xml);

                    Tenet4String = textWriter.ToString();
                }

                objCom.CommandText = "Tenet4";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Connection = objCon;
                objCon.Open();
                objCom.Parameters.AddWithValue("@IncidentID", incidentVM.IncidentID);
                objCom.Parameters.AddWithValue("@Tenets4Save", Tenet4String);
                affectedRecordCount = objCom.ExecuteNonQuery();
                objCom.Parameters.Clear();
                objCon.Close();

            }


            return Tenets4;
        }

        public NewIncidentViewModel GetTenets(int incidentID)
        {
            List<TenetsList> tenets = new List<TenetsList>();
            List<Tenets4> tenets4 = new List<Tenets4>();
            NewIncidentViewModel newinc = new NewIncidentViewModel();
            using (SqlConnection objCon = new SqlConnection(constring))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "[TenetsGet]";
                objCom.Parameters.AddWithValue("@IncidentID", incidentID);
                objCom.CommandType = CommandType.StoredProcedure;
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();

                while (objReader.Read())
                {
                    TenetsList tenet = new TenetsList();
                    newinc.IncidentID = int.Parse(objReader["IncidentID"].ToString());
                    tenet.ID = int.Parse(objReader["TenetsID"].ToString());
                    tenet.YesNo = int.Parse(objReader["ChkBox"].ToString()) > 0 ? true : false;
                    tenet.Name = objReader["TenetName"].ToString();
                    tenet.Details = objReader["Details"].ToString();
                    tenets.Add(tenet);
                    newinc.Tenets = tenets;

                }

                if (objReader.NextResult())
                {
                    while (objReader.Read())
                    {
                        Tenets4 tenet4 = new Tenets4();
                        newinc.IncidentID = int.Parse(objReader["IncidentID"].ToString());
                        tenet4.ID = int.Parse(objReader["ID"].ToString());
                        tenet4.YesNo = int.Parse(objReader["ChkBox"].ToString()) > 0 ? true : false;
                        tenet4.Name = objReader["Name"].ToString();
                        tenet4.TemplateID = int.Parse(objReader["TemplateID"].ToString());
                        tenets4.Add(tenet4);
                        newinc.Tenets4 = tenets4;

                    }
                }
                objReader.Close();


                objCon.Close();

                return newinc;
            }

        }

        public List<Contractor> GetContractor()
        {
            List<Contractor> contractor = new List<Contractor>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[IncidentContractorDetails]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    if (reader.HasRows)
                    {
                        contractor.Add(new Contractor { ContractorID = "", CompanyName = "--Select--" });
                    }

                    while (reader.Read())
                    {
                        contractor.Add(new Contractor { ContractorID = reader["ContractorID"].ToString(), CompanyName = reader["CompanyName"].ToString(), FullName = reader["FullName"].ToString() });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return contractor;
        }
        public List<Gender> GetGender()
        {
            List<Gender> gender = new List<Gender>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetGender]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        gender.Add(new Gender { GenderID = reader["GenderID"].ToString(), Name = reader["Name"].ToString() });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return gender;
        }
        //Why tree delete
        public void DeleteWhyTreeImage(int IncidentID)
        {
            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "DeleteWhyTreeImage";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@incidentID", IncidentID);

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
        public List<ContractorEmp> GetContractorEmp()
        {
            List<ContractorEmp> cont = new List<ContractorEmp>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetContractorEmp]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    if (reader.HasRows)
                    {
                        cont.Add(new ContractorEmp { ID = "", Name = "--Select--" });
                    }

                    while (reader.Read())
                    {
                        cont.Add(new ContractorEmp { ID = reader["ID"].ToString(), Name = reader["Name"].ToString() });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return cont;
        }
        public int Savewhyform(NewIncidentViewModel form)
        {

            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "WhyFormInsert";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@IncidentID", form.IncidentID);
                    objCom.Parameters.AddWithValue("@Why", form.WhyForm.Why);
                    objCom.Parameters.AddWithValue("@FirstWhy", form.WhyForm.FirstWhy);
                    objCom.Parameters.AddWithValue("@SecondWhy", form.WhyForm.SecondWhy);
                    objCom.Parameters.AddWithValue("@ThirdWhy", form.WhyForm.ThirdWhy);
                    objCom.Parameters.AddWithValue("@FourthWhy", form.WhyForm.FourthWhy);
                    objCom.Parameters.AddWithValue("@FifthWhy", form.WhyForm.FifthWhy);

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
        public WhyForm GetWhyForm(int IncidentID)
        {
            WhyForm whyform = new WhyForm();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[WhyFormGet]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@IncidentID", IncidentID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    if (reader.Read())
                    {
                        whyform.Why = reader["Why"].ToString();
                        whyform.FirstWhy = reader["FirstWhy"].ToString();
                        whyform.SecondWhy = reader["SecondWhy"].ToString();
                        whyform.ThirdWhy = reader["ThirdWhy"].ToString();
                        whyform.FourthWhy = reader["FourthWhy"].ToString();
                        whyform.FifthWhy = reader["FifthWhy"].ToString();
                    }
                    objCon.Close();
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return whyform;
        }

        public int InsertInjuryPersonDetails(NewIncidentViewModel incidentReport)
        {

            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "InsertInjuredPerson";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@Name", incidentReport.Injuredpeoples.Name);
                    objCom.Parameters.AddWithValue("@Gender", incidentReport.Injuredpeoples.GenderID);
                    objCom.Parameters.AddWithValue("@Age", incidentReport.Injuredpeoples.Age);
                    objCom.Parameters.AddWithValue("@ContractorEmployee", incidentReport.Injuredpeoples.ContractorEmpID);
                    objCom.Parameters.AddWithValue("@FirstAid", incidentReport.Injuredpeoples.FirstAid);
                    objCom.Parameters.AddWithValue("@Hospitalized", incidentReport.Injuredpeoples.Hospitalized);
                    if (incidentReport.Injuredpeoples.CompanyName == 0)
                    {
                        objCom.Parameters.AddWithValue("@ContractorID", DBNull.Value);

                    }
                    else
                    {
                        objCom.Parameters.AddWithValue("@ContractorID", incidentReport.Injuredpeoples.CompanyName);
                    }

                    objCom.Parameters.AddWithValue("@IncidentID", incidentReport.IncidentID);


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
        public List<InjureList> GetAllInjuredPersonList(int incidentID)
        {
            List<InjureList> injure = new List<InjureList>();

            int SNo = 1;

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetAllInjuredPersonList]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@InciID", incidentID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        var injurelist = new InjureList();
                        injurelist.SNo = SNo++;
                        injurelist.IncidentID = int.Parse(reader["IncidentID"].ToString());
                        if (reader["InjuredPeopleID"] != DBNull.Value)
                        {
                            injurelist.InjuryPeopleID = int.Parse(reader["InjuredPeopleID"].ToString());
                        }
                        injurelist.Name = reader["Name"].ToString();
                        injurelist.Age = reader["Age"].ToString();
                        injurelist.GenderName = reader["GenderName"].ToString();
                        injurelist.CompanyName = reader["CompanyName"].ToString();
                        injurelist.ContractorEmp = reader["Contractemp"].ToString();
                        injurelist.FirstAid = reader["FirstAid"].ToString();
                        injurelist.Hospitalized = reader["Hospitalized"].ToString();

                        injure.Add(injurelist);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return injure;
        }


        public IncidentObserverViewModel GetIncidentLead(int IncidentID)
        {
            IncidentObserverViewModel AssignedLeadList = new IncidentObserverViewModel();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetLeadByIncidentId]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@IncidentID", IncidentID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        AssignedLeadList.AssignedLead = int.Parse(reader["Lead"].ToString());
                        AssignedLeadList.Manager = int.Parse(reader["DepartmentManager"].ToString());
                        AssignedLeadList.Investigator = int.Parse(reader["Investigator"].ToString());
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return AssignedLeadList;
        }
        public List<Employee> GetInvestigator()
        {
            List<Employee> investigator = new List<Employee>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetAllInvestigator]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        investigator.Add(new Employee
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

            return investigator;
        }
        public List<Employee> GetIncidentObservers(int IncidentID)
        {
            List<Employee> AssignedObserversList = new List<Employee>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetAssignedObserversByIncidentId]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@IncidentID", IncidentID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        AssignedObserversList.Add(new Employee
                        {
                            ID = int.Parse(reader["UserID"].ToString()),
                            FirstName = reader["FIRSTNAME"].ToString(),
                            LastName = reader["LASTNAME"].ToString(),
                            FullName = reader["FIRSTNAME"].ToString() + ' ' + reader["LASTNAME"].ToString(),
                            Designation = reader["DESIGNATION"].ToString()
                        });

                        //ObserversList.Add(new IncidentObserverViewModel { ID = int.Parse(reader["ID"].ToString()), ObserverD = int.Parse(reader["ObserverD"].ToString()), EmployeeName = reader["EmployeeName"].ToString(), IncidentTitle = reader["Title"].ToString() });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return AssignedObserversList;
        }

        public List<IncidentViewModel> GetOpenIncidents()
        {
            List<IncidentViewModel> IncidentsList = new List<IncidentViewModel>();
            try
            {
                int RecordCount = 1;
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetAllOpenIncidents]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {

                        IncidentsList.Add(
                            new IncidentViewModel
                            {
                                SNo = RecordCount++,
                                IncidentID = int.Parse(reader["ID"].ToString()),
                                IncidentNO = reader["IncidentNO"].ToString(),
                                Title = reader["Title"].ToString(),
                                Description = reader["Description"].ToString(),
                                IncidentTime = DateTime.Parse(reader["IncidentDate"].ToString()),
                                ObTitle = reader["ObTitle"].ToString(),
                                PlantArea = reader["PlantName"].ToString(),
                                IncidentType = reader["TypeName"].ToString(),
                                CurrentStatus = reader["StatusName"].ToString(),
                                FileName = reader["Attachments"].ToString(),
                                ActionTaken = reader["ActionTaken"].ToString(),
                                Incicreator = int.Parse(reader["Incicreator"].ToString()),
                                Inciclassification = reader["Classification"].ToString(),
                                CreatedBy = reader["CreatedByName"].ToString(),
                                ObCount= int.Parse(reader["ObCount"].ToString()),
                            });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return IncidentsList;
        }

        public List<ObserverTeamModel> GetAllObservations()
        {
            List<ObserverTeamModel> ObserverTeamList = new List<ObserverTeamModel>();
            try
            {
                int RecordCount = 1;
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetAllObservations]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        var ObserverTeamList1 = new ObserverTeamModel();

                        ObserverTeamList1.IncidentID = int.Parse(reader["ID"].ToString());
                        if (reader["ObserverLead"] != DBNull.Value)
                        {
                            ObserverTeamList1.ObserverTeamLead = int.Parse(reader["ObserverLead"].ToString());

                        }
                        if (reader["DepartmentManager"] != DBNull.Value)
                        {
                            ObserverTeamList1.DeptManager = int.Parse(reader["DepartmentManager"].ToString());
                        }
                        if (reader["ObserverD"] != DBNull.Value)
                        {
                            ObserverTeamList1.ObserverD = int.Parse(reader["ObserverD"].ToString());
                        }
                        if (reader["CompletedBy"] != DBNull.Value)
                        {
                            ObserverTeamList1.CompletedBy = int.Parse(reader["CompletedBy"].ToString());
                        }
                        ObserverTeamList.Add(ObserverTeamList1);

                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return ObserverTeamList;
        }
        public List<Employee> GetAllGeneralManager()
        {
            List<Employee> generalManager = new List<Employee>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetAllGeneralManager]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        generalManager.Add(new Employee
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

            return generalManager;
        }


        public List<Employee> GetAllManager()
        {
            List<Employee> DeptManager = new List<Employee>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetAllManager]";
                    objCom.CommandType = CommandType.StoredProcedure;
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
                            FullName = reader["FIRSTNAME"].ToString() + ' ' + reader["LASTNAME"].ToString(),
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

        public List<IncidentViewModel> SearchOpenIncidents(IncidentSearchViewModel incidentSearchViewModel)
        {
            List<IncidentViewModel> IncidentsList = new List<IncidentViewModel>();
            try
            {
                int RecordCount = 1;

                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[SearchOpenIncidents]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@Title", incidentSearchViewModel.IncidentTitle == null ? string.Empty : incidentSearchViewModel.IncidentTitle);
                    objCom.Parameters.AddWithValue("@IncidentStatus", incidentSearchViewModel.IncidentStatus);
                    objCom.Parameters.AddWithValue("@FromDate", incidentSearchViewModel.IncidentFromDate == null ? string.Empty : incidentSearchViewModel.IncidentFromDate);
                    objCom.Parameters.AddWithValue("@ToDate", incidentSearchViewModel.IncidentToDate == null ? string.Empty : incidentSearchViewModel.IncidentToDate);
                    objCom.Parameters.AddWithValue("@IncidentType", incidentSearchViewModel.IncidentType);
                    objCom.Parameters.AddWithValue("@IncidentPlant", incidentSearchViewModel.IncidentPlant);
                    objCom.Parameters.AddWithValue("@InciClass", incidentSearchViewModel.InciClass);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        IncidentsList.Add(
                            new IncidentViewModel
                            {
                                SNo = RecordCount++,
                                IncidentID = int.Parse(reader["ID"].ToString()),
                                IncidentNO = reader["IncidentNO"].ToString(),
                                Title = reader["Title"].ToString(),
                                Description = reader["Description"].ToString(),
                                IncidentTime = DateTime.Parse(reader["IncidentDate"].ToString()),
                                PlantArea = reader["PlantName"].ToString(),
                                IncidentType = reader["TypeName"].ToString(),
                                CurrentStatus = reader["StatusName"].ToString(),
                                FileName = reader["Attachments"].ToString(),
                                ActionTaken = reader["ActionTaken"].ToString(),
                                IncidentCloseTime = DateTime.Parse(reader["ClosedDate"].ToString()),
                                Inciclassification = reader["Classification"].ToString(),

                            });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return IncidentsList;
        }
        public List<IncidentViewModel> SearchClosedIncidents(IncidentSearchViewModel incidentSearchViewModel)
        {
            List<IncidentViewModel> IncidentsList = new List<IncidentViewModel>();
            try
            {
                int RecordCount = 1;

                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[SearchClosedIncidents]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@Title", incidentSearchViewModel.IncidentTitle == null ? string.Empty : incidentSearchViewModel.IncidentTitle);
                    objCom.Parameters.AddWithValue("@IncidentStatus", incidentSearchViewModel.IncidentStatus);
                    objCom.Parameters.AddWithValue("@FromDate", incidentSearchViewModel.IncidentFromDate == null ? string.Empty : incidentSearchViewModel.IncidentFromDate);
                    objCom.Parameters.AddWithValue("@ToDate", incidentSearchViewModel.IncidentToDate == null ? string.Empty : incidentSearchViewModel.IncidentToDate);
                    objCom.Parameters.AddWithValue("@IncidentType", incidentSearchViewModel.IncidentType);
                    objCom.Parameters.AddWithValue("@IncidentPlant", incidentSearchViewModel.IncidentPlant);
                    objCom.Parameters.AddWithValue("@InciClass", incidentSearchViewModel.InciClass);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        IncidentsList.Add(
                            new IncidentViewModel
                            {
                                SNo = RecordCount++,
                                IncidentID = int.Parse(reader["ID"].ToString()),
                                IncidentNO = reader["IncidentNO"].ToString(),
                                Title = reader["Title"].ToString(),
                                Description = reader["Description"].ToString(),
                                IncidentTime = DateTime.Parse(reader["IncidentDate"].ToString()),
                                PlantArea = reader["PlantName"].ToString(),
                                IncidentType = reader["TypeName"].ToString(),
                                CurrentStatus = reader["StatusName"].ToString(),
                                FileName = reader["Attachments"].ToString(),
                                ActionTaken = reader["ActionTaken"].ToString(),
                                IncidentCloseTime = DateTime.Parse(reader["ClosedDate"].ToString()),
                                Inciclassification = reader["Classification"].ToString(),

                            });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return IncidentsList;
        }
        public void IncidentCategoryOverallCalculation(int IncidentID)
        {
            try
            {
                using(SqlConnection conn = new SqlConnection(constring))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "IncidentCategoryOverallCalculation";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IncidentID", IncidentID);
                    cmd.Connection = conn;
                    conn.Open();
                    int affect = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch(Exception ex) {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
        }

        public List<IncidentViewModel> GetAllClosedIncidents()
        {
            List<IncidentViewModel> IncidentsList = new List<IncidentViewModel>();
            try
            {
                int RecordCount = 1;
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetAllClosedIncidents]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        IncidentsList.Add(
                            new IncidentViewModel
                            {
                                SNo = RecordCount++,
                                IncidentID = int.Parse(reader["ID"].ToString()),
                                IncidentNO = reader["IncidentNO"].ToString(),
                                ECNumber = reader["ECNumber"].ToString(),
                                Title = reader["Title"].ToString(),
                                Description = reader["Description"].ToString(),
                                IncidentTime = DateTime.Parse(reader["IncidentDate"].ToString()),
                                PlantArea = reader["PlantName"].ToString(),
                                IncidentType = reader["TypeName"].ToString(),
                                FileName = reader["Attachments"].ToString(),
                                CurrentStatus = reader["StatusName"].ToString(),
                                IncidentCloseTime = DateTime.Parse(reader["ClosedDate"].ToString()),
                                Inciclassification = reader["InciClass"].ToString(),
                            });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return IncidentsList;
        }

        public Incident GetIncident(int IncidentID)
        {
            Incident incident = new Incident();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetIncident]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@IncidentID", IncidentID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    if (reader.Read())
                    {
                        incident.IncidentID = int.Parse(reader["ID"].ToString());
                        incident.IncidentNO = reader["IncidentNO"].ToString();
                        incident.ECNumber = reader["ECNumber"].ToString();
                        incident.Title = reader["Title"].ToString();
                        incident.Description = reader["Description"].ToString();
                        incident.IncidentTime = reader["IncidentDateTime"].ToString();
                        if (reader["PlantID"] != DBNull.Value)
                        {
                            incident.PlantID = int.Parse(reader["PlantID"].ToString());
                        }
                        if (reader["IncidentClassID"] != DBNull.Value)
                        {
                            incident.IncidentClassficationID = int.Parse(reader["IncidentClassID"].ToString());
                        }
                        if (reader["IncidentTypeID"] != DBNull.Value)
                        {
                            incident.IncidentTypeID = int.Parse(reader["IncidentTypeID"].ToString());
                        }
                        if (reader["StatusID"] != DBNull.Value)
                        {
                            incident.StatusID = int.Parse(reader["StatusID"].ToString());
                        }
                        if (reader["PriorityID"] != DBNull.Value)
                        {
                            incident.PriorityID = int.Parse(reader["PriorityID"].ToString());
                        }
                        if (reader["InjuryTypeID"] != DBNull.Value)
                        {
                            incident.InjuryTypeID = int.Parse(reader["InjuryTypeID"].ToString());
                        }
                        incident.ReportedDate = reader["ReportedDateTime"].ToString();
                        incident.ReportedBy = reader["ReportedBy"].ToString();
                        incident.injuredOrNot = reader["InjuredOrNot"].ToString();
                        incident.CreatedByName = reader["CreatedByName"].ToString();
                        incident.PlantName = reader["Name"].ToString();
                        incident.IncidentType = reader["IncidentType"].ToString();
                        incident.PotentialLevel = reader["PotentialLevel"].ToString();
                        incident.IncidentClassName = reader["ClassID"].ToString();
                        if (reader["InjuredDescription"] != DBNull.Value)
                        {
                            incident.injuredDecription = reader["InjuredDescription"].ToString();
                        }
                        incident.LossOfMaterial = reader["LossOfMaterial"].ToString();
                        if (reader["LossQuantity"] != DBNull.Value)
                        {
                            incident.LossQuantity = reader["LossQuantity"].ToString();
                        }
                        incident.DamageEquipment = reader["DamageEquipment"].ToString();
                        if (reader["DamageDetails"] != DBNull.Value)
                        {
                            incident.DamageDetails = reader["DamageDetails"].ToString();
                        }
                        incident.PersonAvailable = reader["PersonAvailable"].ToString();
                        incident.ImmediateAction = reader["ImmediateAction"].ToString();
                        if (reader["ProbableCauses"] != DBNull.Value)
                        {
                            incident.ProbableCauses = reader["ProbableCauses"].ToString();
                        }
                        if (reader["Incidentchronology"] != DBNull.Value)
                        {
                            incident.Incidentchronology = reader["Incidentchronology"].ToString();
                        }
                        if (reader["Precautionarymeasures"] != DBNull.Value)
                        {
                            incident.Precautionarymeasures = reader["Precautionarymeasures"].ToString();
                        }
                        if (reader["Analysis"] != DBNull.Value)
                        {
                            incident.Analysis = reader["Analysis"].ToString();
                        }
                        if (reader["RootCauseID"] != DBNull.Value)
                        {
                            incident.RootCauseID = int.Parse(reader["RootCauseID"].ToString());
                        }
                        incident.FileName = reader["Attachments"].ToString();
                        incident.secondfile = reader["InvestigationAttachments"].ToString();
                        incident.Investigation = reader["Investigation"].ToString();
                        incident.WhyAnalysis = reader["WhyAnalysis"].ToString() == "Y" ? true : false;
                        incident.WhyTree = reader["WhyTree"].ToString() == "Y" ? true : false;
                        if (reader["ChemicalQTYType"] != DBNull.Value)
                        {
                            incident.IncidentChemicalQTY = int.Parse(reader["ChemicalQTYType"].ToString());
                        }

                       
                            if (reader.NextResult())
                            {
                                reader.Read();
                                if (reader["BeganTime"] != DBNull.Value)
                                {
                                    incident.InvestigationBegan = reader["BeganTime"].ToString();
                                }
                                if (reader["DidWork"] != DBNull.Value)
                                {
                                    incident.DidWork = reader["DidWork"].ToString();
                                }
                                if (reader["DidNotWork"] != DBNull.Value)
                                {
                                    incident.DidNotWork = reader["DidNotWork"].ToString();
                                }
                                if (reader["HaveHepled"] != DBNull.Value)
                                {
                                    incident.HaveHepled = reader["HaveHepled"].ToString();
                                }
                                if (reader["Findings"] != DBNull.Value)
                                {
                                    incident.Findings = reader["Findings"].ToString();
                                }
                                if (reader["Lessons"] != DBNull.Value)
                                {
                                    incident.Lessons = reader["Lessons"].ToString();
                                }
                            }
                        reader.NextResult();
                        if (reader.Read())
                        {
                            if (reader["RootCauseID"] != DBNull.Value)
                            {
                                incident.RootCauseCheck = int.Parse(reader["RootCauseID"].ToString());
                            }
                        }

                        objCon.Close();
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return incident;
        }

        public SessionDetails GetSession(int userid)
        {
            SessionDetails session = new SessionDetails();
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "GetSession";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@UserID", userid);
                    objCon.Open();
                    objCom.Connection = objCon;
                    var objReader = objCom.ExecuteReader();
                    if (objReader.Read())
                    {
                        session.SessionActive = objReader["SessionActive"].ToString();
                    }

                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return session;
        }

        public List<ObservationViewModel> GetMyInciObservation(int InciObservID)
        {
            List<ObservationViewModel> obList = new List<ObservationViewModel>();

            int SNo = 1;

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetMyInciObservations]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@InciObservID", InciObservID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        var observationList = new ObservationViewModel();

                        observationList.SNo = SNo++;
                        observationList.IncidentID = int.Parse(reader["IncidentID"].ToString());
                        observationList.OBID = int.Parse(reader["OBID"].ToString());
                        observationList.InciTitle = reader["InciTitle"].ToString();
                        observationList.InciDetails = reader["InciDetails"].ToString();
                        observationList.ObservationID = int.Parse(reader["ID"].ToString());
                        observationList.Observation = reader["Title"].ToString();
                        observationList.PriorityName = reader["Name"].ToString();
                        observationList.Description = reader["Description"].ToString();
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
        public List<IncidentEmail> GetObservationForMailing(int incidentID)
        {
            List<IncidentEmail> observersList = new List<IncidentEmail>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetObservationForMailing]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@IncidentID", incidentID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {

                        var incimail = new IncidentEmail();
                        {
                            incimail.AssignDate = reader["AssignDate"].ToString();
                            incimail.SubmitForApprovalDate = reader["SubmitForDate"].ToString();
                            incimail.IncidentDescription = reader["Description"].ToString();
                            if (reader["LeadEmail"] != DBNull.Value)
                            {
                                incimail.ObserverEmail = ',' + reader["LeadEmail"].ToString();
                            }
                            if (reader["InvestigatorEmail"] != DBNull.Value)
                            {
                                incimail.InvestigatorEmail = ',' + reader["InvestigatorEmail"].ToString();
                            }

                            incimail.OwnerEmail = reader["InciOwner"].ToString();

                            if (reader["ActionerEmail"] != DBNull.Value)
                            {
                                incimail.ActionerEmail = ',' + reader["ActionerEmail"].ToString();
                            }
                            if (reader["TeamList"] != DBNull.Value)
                            {
                                incimail.TeamListEmail = ',' + reader["TeamList"].ToString();
                            }
                            incimail.IncidentOwner = reader["OwnerName"].ToString();
                            if (reader["LeadName"] != DBNull.Value)
                            {
                                incimail.Teamlead = ',' + reader["LeadName"].ToString();
                            }
                            if (reader["TeamMembers"] != DBNull.Value)
                            {
                                incimail.TeamMembers = ',' + reader["TeamMembers"].ToString();
                            }
                            if (reader["Investigator"] != DBNull.Value)
                            {
                                incimail.Invesfacilitator = ',' + reader["Investigator"].ToString();
                            }
                            observersList.Add(incimail);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return observersList;
        }



        public List<ObservationViewModel> GetObservations(int incidentID, int ObservationID)
        {
            List<ObservationViewModel> obList = new List<ObservationViewModel>();

            int SNo = 1;

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetObservations1]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@IncidentID", incidentID);
                    objCom.Parameters.AddWithValue("@ObservationID", ObservationID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        var observationList = new ObservationViewModel();

                        observationList.SNo = SNo++;
                        observationList.IncidentID = int.Parse(reader["IncidentID"].ToString());
                        observationList.IncidentNO = reader["IncidentNO"].ToString();
                        observationList.OBID = int.Parse(reader["OBID"].ToString());
                        observationList.ObservationID = int.Parse(reader["ID"].ToString());
                        observationList.Observation = reader["Title"].ToString();
                        observationList.PriorityName = reader["Name"].ToString();
                        observationList.Description = reader["Description"].ToString();
                        observationList.TargetDate = reader["TargetDate"].ToString();
                        observationList.Recommendation = reader["Recommendation"].ToString();
                        observationList.ActionTaken = reader["ActionTaken"].ToString();
                        observationList.CompletedUser = reader["UserFullName"].ToString();
                        observationList.CompletedDate = reader["CompletedDate"].ToString();
                        if (reader["Approver"] != DBNull.Value)
                        {
                            observationList.Approver = int.Parse(reader["Approver"].ToString());
                        }
                        if (reader["CompletedBy"] != DBNull.Value)
                        {
                            observationList.ActionBy = int.Parse(reader["CompletedBy"].ToString());
                        }
                        observationList.Attachment = reader["Attachment"].ToString();
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

        public IncObservationViewModel GetObservation(int ObsID)
        {
            IncObservationViewModel incObservationVM = new IncObservationViewModel();
            IncidentObservation observation = new IncidentObservation();
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetObservation1]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@ObservationID", ObsID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        incObservationVM.IncidentTitle = reader["IncidentTitle"].ToString();
                        //incObservationVM. = reader["IncDescription"].ToString();
                        incObservationVM.PlantName = reader["PLANTNAME"].ToString();
                        observation.IncidentID = int.Parse(reader["IncidentID"].ToString());
                        observation.ObservationID = int.Parse(reader["ID"].ToString());
                        observation.Title = reader["Title"].ToString();
                        observation.PriorityID = int.Parse(reader["PriorityID"].ToString());
                        observation.Description = reader["Description"].ToString();
                        observation.TargetDate = reader["TargetDate"].ToString();
                        observation.Recommendation = reader["Recommendation"].ToString();
                        observation.ActionTaken = reader["ActionTaken"].ToString();
                        observation.CompletedBy = int.Parse(reader["CompletedBy"].ToString());
                        observation.CompletedDate = reader["CompletedDate"].ToString();
                        observation.CpStatusID = int.Parse(reader["StatusID"].ToString());
                        observation.DeptManager = int.Parse(reader["DeptManagerID"].ToString());
                        observation.InciAttachment = reader["Attachment"].ToString();
                    }
                    incObservationVM.incidentObservation = observation;
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return incObservationVM;
        }


        public List<MonthlyCount> GetMonthlyCount()
        {
            List<MonthlyCount> monthlyCounts = new List<MonthlyCount>();

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "GetIncidentCAPAOverallMonthlyCount";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        MonthlyCount monthlyCount = new MonthlyCount()
                        {
                            TotalCount = int.Parse(reader["IncidentCount"].ToString()),
                            MonthName = reader["MonthYear"].ToString(),
                            MonthlyCounts = int.Parse(reader["CAPACount"].ToString()),
                        };
                        monthlyCounts.Add(monthlyCount);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return monthlyCounts;
        }



        public List<ObservationStatusCount> GetRecommenStatusCount()
        {
            List<ObservationStatusCount> RecommenCounts = new List<ObservationStatusCount>();

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetInciActionsCount]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        ObservationStatusCount recommenCount = new ObservationStatusCount()
                        {
                            ActionName = reader["Actions"].ToString(),
                            TotalCount = int.Parse(reader["ActionsCount"].ToString())
                        };

                        RecommenCounts.Add(recommenCount);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return RecommenCounts;
        }
        public List<TypeCount> GetIncidentCategoryCount()
        {
            List<TypeCount> IncicategoryCounts = new List<TypeCount>();

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetTypeCount]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        TypeCount categoryCount = new TypeCount()
                        {
                            TypeName = reader["Name"].ToString(),
                            TotalCount = int.Parse(reader["Categorycount"].ToString())
                        };
                        if (categoryCount.TotalCount > 0)
                            IncicategoryCounts.Add(categoryCount);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return IncicategoryCounts;
        }
        public List<StatusCount> GetStatusCount()
        {
            List<StatusCount> statusCounts = new List<StatusCount>();

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetStatusCount]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        StatusCount statusCount = new StatusCount()
                        {
                            StatusName = reader["Name"].ToString(),
                            TotalCount = int.Parse(reader["StatusCount"].ToString())
                        };
                        if (statusCount.TotalCount > 0)
                            statusCounts.Add(statusCount);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return statusCounts;
        }

        public List<ActionsCount> GetMyActionStatusCount(int UserID)
        {
            List<ActionsCount> ActionCounts = new List<ActionsCount>();
            int SNo = 1;

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetMyActionStatusCount]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@UserID", UserID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        ActionsCount actionCount = new ActionsCount()
                        {
                            SNo = SNo++,
                            StatusName = reader["Actions"].ToString(),
                            IIR = int.Parse(reader["IIR"].ToString()),
                            CAPA = int.Parse(reader["CAPA"].ToString()),
                            PSSRID = int.Parse(reader["PSSR"].ToString()),
                            MOCID = int.Parse(reader["MOC"].ToString())

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

        public List<ClassificationCount> GetClassificationCount()
        {
            List<ClassificationCount> classificationcount = new List<ClassificationCount>();

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetClassificationCount]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        ClassificationCount classificationlist = new ClassificationCount()
                        {
                            ClassificationName = reader["Name"].ToString(),
                            TotalCount = int.Parse(reader["StatusCount"].ToString())
                        };
                        if (classificationlist.TotalCount > 0)
                            classificationcount.Add(classificationlist);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return classificationcount;
        }

        public List<RootCauseCount> GetRootCauseCount()
        {
            List<RootCauseCount> rootcausecount = new List<RootCauseCount>();

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetRootCauseCount]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        RootCauseCount rootcauselist = new RootCauseCount()
                        {
                            RootCauseName = reader["Name"].ToString(),
                            TotalCount = int.Parse(reader["StatusCount"].ToString())
                        };
                        if (rootcauselist.TotalCount > 0)
                            rootcausecount.Add(rootcauselist);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return rootcausecount;
        }

        #region "get Last monthly count Incident Summary chart"
        public List<MonthlyCount> GetLastMonthlyCount()
        {
            List<MonthlyCount> monthlyCounts = new List<MonthlyCount>();

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetLastMonthlyCount]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        MonthlyCount monthlyCount = new MonthlyCount()
                        {
                            MonthName = reader["IncidentMonth"].ToString(),
                            TotalCount = int.Parse(reader["MonthlyCount"].ToString())

                        };
                        monthlyCounts.Add(monthlyCount);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return monthlyCounts;
        }
        #endregion

        #region "get Last month Type Count chart"
        public List<TypeCount> GetLastMonthIncidentCategory()
        {
            List<TypeCount> IncicategoryCounts = new List<TypeCount>();

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetLastMonthlyTypeCount]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        TypeCount categoryCount = new TypeCount()
                        {
                            TypeName = reader["Name"].ToString(),
                            TotalCount = int.Parse(reader["Categorycount"].ToString())
                        };
                        if (categoryCount.TotalCount > 0)
                            IncicategoryCounts.Add(categoryCount);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return IncicategoryCounts;
        }
        #endregion

        #region "get Last month ClassificationCount chart"
        public List<ClassificationCount> GetLastMonthClassificationCount()
        {
            List<ClassificationCount> classificationcount = new List<ClassificationCount>();

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetLastMonthClassificationCount]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        ClassificationCount classificationlist = new ClassificationCount()
                        {
                            ClassificationName = reader["Name"].ToString(),
                            TotalCount = int.Parse(reader["StatusCount"].ToString())
                        };
                        if (classificationlist.TotalCount > 0)
                            classificationcount.Add(classificationlist);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return classificationcount;
        }
        #endregion

        #region "get Last month StatusCount chart"
        public List<StatusCount> GetLastMonthStatusCount()
        {
            List<StatusCount> statusCounts = new List<StatusCount>();

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetLastMonthStatusCount]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        StatusCount statusCount = new StatusCount()
                        {
                            StatusName = reader["Name"].ToString(),
                            TotalCount = int.Parse(reader["StatusCount"].ToString())
                        };
                        if (statusCount.TotalCount > 0)
                            statusCounts.Add(statusCount);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return statusCounts;
        }
        #endregion

        #region "get Last month RootCauseCount chart"
        public List<RootCauseCount> GetLastMonthRootCauseCount()
        {
            List<RootCauseCount> rootcausecount = new List<RootCauseCount>();

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetLastMonthRootCauseCount]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        RootCauseCount rootcauselist = new RootCauseCount()
                        {
                            RootCauseName = reader["Name"].ToString(),
                            TotalCount = int.Parse(reader["StatusCount"].ToString())
                        };
                        if (rootcauselist.TotalCount > 0)
                            rootcausecount.Add(rootcauselist);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return rootcausecount;
        }
        #endregion

        #region "get Last month RecommenStatusCount chart"
        public List<ObservationStatusCount> GetLastMonthRecommenStatusCount()
        {
            List<ObservationStatusCount> RecommenCounts = new List<ObservationStatusCount>();

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetLastMonthInciActionsCount]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        ObservationStatusCount recommenCount = new ObservationStatusCount()
                        {
                            ActionName = reader["Actions"].ToString(),
                            TotalCount = int.Parse(reader["ActionsCount"].ToString())
                        };

                        RecommenCounts.Add(recommenCount);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return RecommenCounts;
        }
        #endregion

        public DetailedIncidentViewModel GetIncidentDetailForView(int incidentID)
        {
            DetailedIncidentViewModel detailedIncidentViewModel = new DetailedIncidentViewModel();
            IncidentViewModel incidentViewModel = new IncidentViewModel();
            List<IncidentUser> incidentUsers = new List<IncidentUser>();
            List<ObservationViewModel> observationViewModels = new List<ObservationViewModel>();
            List<IncidentImage> incidentImages = new List<IncidentImage>();
            try
            {
                string ConnectionString = AppConfig.ConnectionString;

                using (SqlConnection objCon = new SqlConnection(ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "GetIncidentDetailForView";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@IncidentID", incidentID);
                    objCon.Open();
                    objCom.Connection = objCon;

                    SqlDataReader objReader = objCom.ExecuteReader();

                    if (objReader.Read())
                    {
                        incidentViewModel.Title = objReader["Title"].ToString();
                        incidentViewModel.Description = objReader["Description"].ToString();
                        incidentViewModel.CurrentStatus = objReader["CurrentStatus"].ToString();
                        incidentViewModel.PlantArea = objReader["PlantName"].ToString();
                        incidentViewModel.Comments = objReader["Comments"].ToString();
                        incidentViewModel.CreatedBy = objReader["CreatedBy"].ToString();
                        incidentViewModel.IncidentTime = DateTime.Parse(objReader["IncidentDate"].ToString());
                    }

                    objReader.NextResult();

                    while (objReader.Read())
                    {
                        IncidentUser incidentUser = new IncidentUser();
                        incidentUser.EmployeeName = objReader["EmployeeName"].ToString();
                        incidentUser.Designation = objReader["Designation"].ToString();
                        incidentUsers.Add(incidentUser);
                    }

                    objReader.NextResult();

                    while (objReader.Read())
                    {
                        ObservationViewModel observationViewModel = new ObservationViewModel();
                        observationViewModel.Observation = objReader["Title"].ToString();
                        observationViewModel.PriorityName = objReader["Name"].ToString();
                        observationViewModel.Recommendation = objReader["Recommendation"].ToString();
                        observationViewModel.ActionTaken = objReader["ActionTaken"].ToString();
                        observationViewModel.TargetDate = objReader["TargetDate"].ToString();
                        observationViewModel.CompletedDate = objReader["CompletedDate"].ToString();
                        observationViewModels.Add(observationViewModel);
                    }

                    objReader.NextResult();

                    while (objReader.Read())
                    {
                        IncidentImage IncImg = new IncidentImage();
                        IncImg.ImageName = objReader["ImageName"].ToString();
                        IncImg.ImageDescription = objReader["ImageDescription"].ToString();
                        IncImg.FileName = objReader["FileName"].ToString();
                        incidentImages.Add(IncImg);
                    }

                    detailedIncidentViewModel.incidentViewModel = incidentViewModel;
                    detailedIncidentViewModel.incidentUsers = incidentUsers;
                    detailedIncidentViewModel.ObservationDetail = observationViewModels;
                    detailedIncidentViewModel.incidentImages = incidentImages;
                }
            }
            catch (Exception ex)
            {

            }

            return detailedIncidentViewModel;

        }

        public List<IncidentImage> GetIncidentImages(int incidentID)
        {
            List<IncidentImage> incidentImages = new List<IncidentImage>();
            int SNo = 1;
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetIncidentImages]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@IncidentID", incidentID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        IncidentImage incidentImage = new IncidentImage()
                        {
                            SNo = SNo++,
                            IncidentImageId = Convert.ToInt32(reader["ID"].ToString()),
                            ImageName = reader["ImageName"].ToString(),
                            ImageDescription = reader["ImageDescription"].ToString(),
                            FileName = reader["FileName"].ToString()
                        };
                        incidentImages.Add(incidentImage);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return incidentImages;
        }

        #endregion

        #region "Save Methods"
        public int IncidentReportUpdate(NewIncidentViewModel incidentReport, string fileName, string fileName2, int currentuser)
        {
            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "IncidentReportUpdate";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@ECNumber", incidentReport.Incident.ECNumber);
                    objCom.Parameters.AddWithValue("@Title", incidentReport.Incident.Title);
                    objCom.Parameters.AddWithValue("@Description", incidentReport.Incident.Description);
                    objCom.Parameters.AddWithValue("@IncidentDate", DateTime.ParseExact(incidentReport.Incident.IncidentTime, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture));
                    if (!string.IsNullOrEmpty(incidentReport.Incident.ReportedDate))
                    {
                        objCom.Parameters.AddWithValue("@ReportedDate", DateTime.ParseExact(incidentReport.Incident.ReportedDate, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture));
                    }
                    objCom.Parameters.AddWithValue("@ReportedBy", incidentReport.Incident.ReportedBy);
                    objCom.Parameters.AddWithValue("@IncidentTypeID", incidentReport.Incident.IncidentTypeID);
                    objCom.Parameters.AddWithValue("@StatusID", incidentReport.Incident.StatusID);
                    objCom.Parameters.AddWithValue("@PriorityID", incidentReport.Incident.PriorityID);
                    objCom.Parameters.AddWithValue("@PlantID", incidentReport.Incident.PlantID);
                    objCom.Parameters.AddWithValue("@IncidentClassID", incidentReport.Incident.IncidentClassficationID);
                    if(incidentReport.Incident.Investigation == "No")
                    {

                        objCom.Parameters.AddWithValue("@InjuryTypeID", 6);
                    }
                    else
                    {

                        objCom.Parameters.AddWithValue("@InjuryTypeID", incidentReport.Incident.InjuryTypeID);
                    }
                    objCom.Parameters.AddWithValue("@Incidentchronology", incidentReport.Incident.Incidentchronology == null ? String.Empty : incidentReport.Incident.Incidentchronology);
                    objCom.Parameters.AddWithValue("@Precautionarymeasures", incidentReport.Incident.Precautionarymeasures == null ? String.Empty : incidentReport.Incident.Precautionarymeasures);
                    objCom.Parameters.AddWithValue("@Analysis", incidentReport.Incident.Analysis == null ? String.Empty : incidentReport.Incident.Analysis);
                    if (incidentReport.Incident.RootCauseID == 0)
                    {
                        objCom.Parameters.AddWithValue("@RootCauseID", DBNull.Value);

                    }
                    else
                    {
                        objCom.Parameters.AddWithValue("@RootCauseID", incidentReport.Incident.RootCauseID);
                    }
                    objCom.Parameters.AddWithValue("@injuredOrNot", incidentReport.Incident.injuredOrNot);
                    objCom.Parameters.AddWithValue("@injuredDecription", incidentReport.Incident.injuredDecription == null ? String.Empty : incidentReport.Incident.injuredDecription);
                    objCom.Parameters.AddWithValue("@LossOfMaterial", incidentReport.Incident.LossOfMaterial);
                    objCom.Parameters.AddWithValue("@LossQuantity", incidentReport.Incident.LossQuantity == null ? String.Empty : incidentReport.Incident.LossQuantity);
                    objCom.Parameters.AddWithValue("@DamageEquipment", incidentReport.Incident.DamageEquipment);
                    objCom.Parameters.AddWithValue("@DamageDetails", incidentReport.Incident.DamageDetails == null ? String.Empty : incidentReport.Incident.DamageDetails);
                    objCom.Parameters.AddWithValue("@PersonAvailable", incidentReport.Incident.PersonAvailable);
                    objCom.Parameters.AddWithValue("@ImmediateAction", incidentReport.Incident.ImmediateAction);
                    objCom.Parameters.AddWithValue("@ProbableCauses", incidentReport.Incident.ProbableCauses == null ? String.Empty : incidentReport.Incident.ProbableCauses);
                    objCom.Parameters.AddWithValue("@Investigation", incidentReport.Incident.Investigation == null ? String.Empty : incidentReport.Incident.Investigation);
                    objCom.Parameters.AddWithValue("@WhyAnalysis", incidentReport.Incident.WhyAnalysis == true ? "Y" : "N");
                    objCom.Parameters.AddWithValue("@WhyTree", incidentReport.Incident.WhyTree == true ? "Y" : "N");
                    objCom.Parameters.AddWithValue("@FileName", fileName);
                    objCom.Parameters.AddWithValue("@FileName2", fileName2);
                    objCom.Parameters.AddWithValue("@Comments", incidentReport.Incident.Comments);
                    objCom.Parameters.AddWithValue("@UserID", currentuser);
                    objCom.Parameters.AddWithValue("@IncidentID", incidentReport.Incident.IncidentID);
                    SqlParameter outPutParameter = new SqlParameter();
                    outPutParameter.ParameterName = "@NewIncidentID";
                    outPutParameter.SqlDbType = System.Data.SqlDbType.Int;

                    outPutParameter.Direction = System.Data.ParameterDirection.Output;
                    objCom.Parameters.Add(outPutParameter);

                    objCom.Connection = objCon;
                    objCon.Open();
                    affectedRecordCount = objCom.ExecuteNonQuery();
                    incidentReport.Incident.IncidentID = int.Parse(outPutParameter.Value.ToString());
                    objCom.Parameters.Clear();
                   
                        objCom.CommandText = "InvestigationInsert";
                        objCom.Parameters.AddWithValue("@IncidentID", incidentReport.Incident.IncidentID);
                        if (!string.IsNullOrEmpty(incidentReport.Incident.InvestigationBegan))
                        {
                            objCom.Parameters.AddWithValue("@InvestigationBegan", DateTime.ParseExact(incidentReport.Incident.InvestigationBegan, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                        }
                        objCom.Parameters.AddWithValue("@DidWork", incidentReport.Incident.DidWork);
                        objCom.Parameters.AddWithValue("@DidNotWork ", incidentReport.Incident.DidNotWork);
                        objCom.Parameters.AddWithValue("@HaveHepled", incidentReport.Incident.HaveHepled);
                        objCom.Parameters.AddWithValue("@Findings", incidentReport.Incident.Findings);
                        objCom.Parameters.AddWithValue("@Lessons", incidentReport.Incident.Lessons);
                        objCom.Parameters.AddWithValue("@UserId", currentuser);
                        objCom.ExecuteNonQuery();

                        objCon.Close();
                    
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

        public void UpdateIncidentStatus(int incidentID, int StatusID, string Comments, int userID)
        {
            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "UpdateIncidentStatus";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@IncidentID", incidentID);
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

        public void SaveObservers(int incidentID, string observerList, string lead, string deptManager, int userID, string investigator)
        {
            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "SaveObservers";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@IncidentID", incidentID);
                    objCom.Parameters.AddWithValue("@ObserverList", observerList);
                    objCom.Parameters.AddWithValue("@Lead", lead);
                    objCom.Parameters.AddWithValue("@UserID", userID);
                    objCom.Parameters.AddWithValue("@DeptManager", deptManager);
                    objCom.Parameters.AddWithValue("@Investigator", investigator);
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

        public void SaveObservervation(IncidentObservation insObservation, string fileName)
        {
            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "SaveObservation";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@IncidentID", insObservation.IncidentID);
                    objCom.Parameters.AddWithValue("@ObservationID", insObservation.ObservationID);
                    objCom.Parameters.AddWithValue("@Title", insObservation.Title == null ? String.Empty : insObservation.Title);
                    objCom.Parameters.AddWithValue("@Description", insObservation.Description == null ? String.Empty : insObservation.Description);
                    objCom.Parameters.AddWithValue("@PriorityID", insObservation.PriorityID);
                    objCom.Parameters.AddWithValue("@Recommendation", insObservation.Recommendation == null ? String.Empty : insObservation.Recommendation);
                    objCom.Parameters.AddWithValue("@ActionTaken", insObservation.ActionTaken == null ? String.Empty : insObservation.ActionTaken);
                    //objCom.Parameters.AddWithValue("@Comments", insObservation.Comments);
                    objCom.Parameters.AddWithValue("@TargetDate", DateTime.ParseExact(insObservation.TargetDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    objCom.Parameters.AddWithValue("@UserID", insObservation.CurrentUser);
                    objCom.Parameters.AddWithValue("@OBStatusID", insObservation.CpStatusID);
                    objCom.Parameters.AddWithValue("@DeptManager", insObservation.DeptManager);
                    objCom.Parameters.AddWithValue("@fileName", fileName);

                    if (insObservation.UserID > 0)
                    {

                        objCom.Parameters.AddWithValue("@CompletedBy", insObservation.UserID);
                    }
                    else
                    {
                        objCom.Parameters.AddWithValue("@CompletedBy", insObservation.CurrentUser);
                    }
                    if (!string.IsNullOrEmpty(insObservation.CompletedDate))
                    {
                        objCom.Parameters.AddWithValue("@CompletedDate", DateTime.ParseExact(insObservation.CompletedDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
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
        public int DeleteIncidentObservation(int OBID)
        {
            int AffectCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[DeleteIncidentObservation]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@RecomID", OBID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    AffectCount = objCom.ExecuteNonQuery();
                    objCon.Close();
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return AffectCount;
        }

        public void DeleteOBDOC(int obid)
        {
            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[DeleteIncidentOBDoc]";
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
        public List<ObservationViewModel> GetAllIncidentObservation()
        {
            List<ObservationViewModel> observationList = new List<ObservationViewModel>();

            int SNo = 1;

            try
            {
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[GetAllIncidentListObservation]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        var ObList = new ObservationViewModel();
                        ObList.SNo = SNo++;
                        ObList.IncidentID = int.Parse(reader["IncidentID"].ToString());
                        if (reader["ID"] != DBNull.Value)
                        {
                            ObList.ObservationID = int.Parse(reader["ID"].ToString());
                        }
                        ObList.IncidentNO = reader["IncidentNO"].ToString();
                        ObList.PlantName = reader["PlantName"].ToString();
                        ObList.ObservationID = int.Parse(reader["ID"].ToString());
                        ObList.InciStatusID = int.Parse(reader["StatusID"].ToString());
                        ObList.Observation = reader["Title"].ToString();
                        ObList.PriorityName = reader["Name"].ToString();
                        ObList.Description = reader["Description"].ToString();
                        ObList.TargetDate = reader["TargetDate"].ToString();
                        ObList.Recommendation = reader["Recommendation"].ToString();
                        ObList.ActionTaken = reader["ActionTaken"].ToString();
                        ObList.CompletedBy = reader["UserFullName"].ToString();
                        ObList.CompletedDate = reader["CompletedDate"].ToString();
                        ObList.Actionstatus = reader["Actionstatus"].ToString();
                        ObList.DeptManagerName = reader["FuncationalManager"].ToString();
                        ObList.Attachment = reader["Attachment"].ToString().Trim();
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

        public List<ObservationViewModel> SearchOpenIncidentsForObservation(IncidentSearchViewModel incidentsearchviewmodel)
        {

            List<ObservationViewModel> observationList = new List<ObservationViewModel>();
            int SNo = 1;
            try
            {

                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[SearchOpenIncidentForObservation]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@IncidentPlant", incidentsearchviewmodel.IncidentPlant);
                    objCom.Parameters.AddWithValue("@IncidentType", incidentsearchviewmodel.IncidentType);
                    objCom.Parameters.AddWithValue("@IncidentStatus", incidentsearchviewmodel.IncidentStatus);
                    objCom.Parameters.AddWithValue("@FromDate", incidentsearchviewmodel.IncidentFromDate == null ? string.Empty : incidentsearchviewmodel.IncidentFromDate);
                    objCom.Parameters.AddWithValue("@ToDate", incidentsearchviewmodel.IncidentToDate == null ? string.Empty : incidentsearchviewmodel.IncidentToDate);
                    objCom.Parameters.AddWithValue("@ActionBy", incidentsearchviewmodel.ActionerID);
                    objCom.Parameters.AddWithValue("@DeptID", incidentsearchviewmodel.DeptManger);

                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        var ObList = new ObservationViewModel();
                        ObList.SNo = SNo++;
                        ObList.IncidentID = int.Parse(reader["IncidentID"].ToString());
                        if (reader["ID"] != DBNull.Value)
                        {
                            ObList.ObservationID = int.Parse(reader["ID"].ToString());
                        }
                        ObList.IncidentNO = reader["IncidentNO"].ToString();
                        ObList.Observation = reader["Title"].ToString();
                        ObList.CompletedBy = reader["UserFullName"].ToString();
                        ObList.TargetDate = reader["TargetDate"].ToString();
                        ObList.Recommendation = reader["Recommendation"].ToString();
                        ObList.ActionTaken = reader["ActionTaken"].ToString();
                        ObList.CompletedDate = reader["CompletedDate"].ToString();
                        ObList.PlantName = reader["PlantName"].ToString();
                        ObList.DeptManagerName = reader["FuncationalManager"].ToString();
                        ObList.PriorityName = reader["Name"].ToString();
                        ObList.Actionstatus = reader["Actionstatus"].ToString();
                        ObList.Attachment = reader["Attachment"].ToString();
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


        public void DeleteObservervation(int ObservationID)
        {
            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "DeleteObservervation";
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

        public void SaveIncidentImages(IncidentImage incidentImage, string fileName, int currentUser)
        {
            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "SaveIncidentImages";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@IncidentID", incidentImage.IncidentId);
                    objCom.Parameters.AddWithValue("@ImageName", incidentImage.ImageName);
                    objCom.Parameters.AddWithValue("@ImageDescription", incidentImage.ImageDescription == null ? string.Empty : incidentImage.ImageDescription);
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

        public void DeleteImage(int ImageId)
        {
            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "DeleteImage";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@ImageID", ImageId);

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

        #endregion

        #region "Employee Info - This section should go to AccountDL Class"
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

            return EmployeeList;
        }
        #endregion
    }
}