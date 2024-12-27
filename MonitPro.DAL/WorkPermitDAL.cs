using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using MonitPro.Models;
using System.Data.SqlClient;
using System.Data;

using MonitPro.Common.Library;
using System.Xml.Serialization;
using System.IO;

namespace MonitPro.DAL
{
    public class WorkPermitDAL
    {
        string constring = AppConfig.ConnectionString;
        SqlCommand objCom;
        SqlDataReader reader;

        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmployeeID { get; set; }
        public string MobileNumber { get; set; }
        public string ConCompanyName { get; set; }
        public string IsActiveSelect { get; set; }
        public string UserImage { get; set; }
        public string DisplayUserName { get; set; }
        public string TrainingTypeName { get; set; }
        public string SkillsName { get; set; }
        public string FrequencyName { get; set; }
        public string DeptName { get; set; }
        public string TraningDate { get; set; }
        public string NextTrainingDate { get; set; }
        public string Remarks { get; set; }
        public string IsInvestigateSelect { get; set; }
        public List<WorkTypeMaster> GetWorkType(int workPermitID = 0)
        {
            using (SqlConnection objCon = new SqlConnection(constring))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "WorkTypeSelect";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@workPermitID", workPermitID);
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();

                var WorkTypeList = new List<WorkTypeMaster>();
                while (objReader.Read())
                {
                    var workType = new WorkTypeMaster();
                    workType.WorkTypeID = int.Parse(objReader["WorkTypeID"].ToString());
                    workType.Ischecked = int.Parse(objReader["Checked"].ToString()) > 0 ? true : false;
                    workType.WorkTypeName = objReader["WorkType"].ToString();
                    WorkTypeList.Add(workType);
                }
                objCon.Close();

                return WorkTypeList;
            }
        }

        public List<WorkTypeMaster> GetWorkTypeContractor(int workPermitID = 0)
        {
            using (SqlConnection objCon = new SqlConnection(constring))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "WorkTypeContractorSelect";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@ContractorID", workPermitID);
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();

                var WorkTypeList = new List<WorkTypeMaster>();
                while (objReader.Read())
                {
                    var workType = new WorkTypeMaster();
                    workType.WorkTypeID = int.Parse(objReader["WorkTypeID"].ToString());
                    workType.Ischecked = int.Parse(objReader["Checked"].ToString()) > 0 ? true : false;
                    workType.WorkTypeName = objReader["WorkType"].ToString();
                    WorkTypeList.Add(workType);
                }
                objCon.Close();

                return WorkTypeList;
            }
        }
        public List<Equipment> GetPlantEquipmentSelect(int? PlantID)
        {
            List<Equipment> EquipmentList = new List<Equipment>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "GetWorkperkpermitEquipmentSelect";

                    if (PlantID > 0)
                        objCom.Parameters.AddWithValue("@PlantID", PlantID);
                    else
                        objCom.Parameters.AddWithValue("@PlantID", DBNull.Value);

                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();

                    SqlDataReader Results = objCom.ExecuteReader();

                    while (Results.Read())
                    {
                        Equipment eq = new Equipment();
                        eq.EquipmentID = int.Parse(Results[0].ToString());
                        eq.EquipmentName = Results[1].ToString();
                        EquipmentList.Add(eq);
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

            return EquipmentList;

        }
        public List<ExtensionDetailsList> GetExtensionList(int workPermitID)
        {
            using (SqlConnection objCon = new SqlConnection(constring))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "ExtensionDetails";
                objCom.Parameters.AddWithValue("@WorkPermitID", workPermitID);
                objCom.CommandType = CommandType.StoredProcedure;
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();

                var extensionList = new List<ExtensionDetailsList>();
                while (objReader.Read())
                {
                    var extension = new ExtensionDetailsList();

                    extension.WorkPermitID = objReader["WorkPermitNO"].ToString();
                    extension.ExtensionFrom = objReader["ExtensionFrom"].ToString();
                    extension.ExtensionTo = objReader["ExtensionTo"].ToString();
                    extension.WorkTypeName = objReader["WorkType"].ToString();
                    extension.ExtensionPermitIssuer = objReader["ExtensionPermitIssuer"].ToString();
                    extension.ExtensionApprover = objReader["ExtensionApprover"].ToString();
                    extension.ExtensionAreaOwner = objReader["ExtensionAreaOwner"].ToString();

                    extensionList.Add(extension);
                }
                objCon.Close();

                return extensionList;
            }
        }

        public List<OccupationalHealthSafetyCheckList> GetContractorCheckList(int ContractorID = 0)
        {
            using (SqlConnection objCon = new SqlConnection(constring))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "ContractorCheckListSelect";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@ContractorID", ContractorID);
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();

                var safetychecklist = new List<OccupationalHealthSafetyCheckList>();
                while (objReader.Read())
                {
                    var checklist = new OccupationalHealthSafetyCheckList();
                    checklist.HealthSafetyPolicy = int.Parse(objReader["ID"].ToString());
                    checklist.Ischecked = objReader["Checked"].ToString();
                    checklist.CheckListName = objReader["HealthSafetyName"].ToString();
                    checklist.Remarks = objReader["Remarks"].ToString();
                    safetychecklist.Add(checklist);
                }
                objCon.Close();



                return safetychecklist;
            }
        }
        public List<EmpContractorprofile> SearchContractorEmployee(SearchContractorEmployee searchContractor)
        {
            List<EmpContractorprofile> ContractorEmployeelist = new List<EmpContractorprofile>();
            try
            {
               
                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[SearchContractorEmployee]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@CompanyName ", searchContractor.ContractID);
                    objCom.Parameters.AddWithValue("@Skill", searchContractor.SkillsID);
                    objCom.Parameters.AddWithValue("@TrainingFromdate", searchContractor.TrainingFromDate == null ? string.Empty : searchContractor.TrainingFromDate);
                    objCom.Parameters.AddWithValue("@ToDate", searchContractor.TrainingToDate == null ? string.Empty : searchContractor.TrainingToDate);
                    objCom.Parameters.AddWithValue("@DeptId", searchContractor.DepartID);
                    
                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {

                        EmpContractorprofile emp = new EmpContractorprofile();

                        emp.UserID = int.Parse(reader["ContractorUserID"].ToString());
                        emp.FirstName = reader["FirstName"].ToString();
                        emp.LastName = reader["LastName"].ToString();
                        emp.EmployeeID = reader["ContractorEmpID"].ToString();
                        emp.MobileNumber = reader["MobileNumber"].ToString();
                        emp.ConCompanyName = reader["CompanyName"].ToString();
                        emp.IsActiveSelect = reader["IsActive"].ToString();
                        emp.UserImage = reader["UserImage"].ToString();
                        emp.DisplayUserName = reader["DisplayUserName"].ToString();
                        emp.TrainingTypeName = reader["TrainingType"].ToString();
                        emp.SkillsName = reader["SkillsID"].ToString();
                        emp.FrequencyName = reader["FrequencyName"].ToString();
                        emp.DeptName = reader["Department"].ToString();
                        emp.TraningDate = reader["TraniningDate"].ToString();
                        emp.NextTrainingDate = reader["NextTrainingDate"].ToString();
                        emp.Remarks = reader["Remarks"].ToString();
                        emp.IsInvestigateSelect = reader["Investigate"].ToString();

                         ContractorEmployeelist.Add(emp);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return ContractorEmployeelist;
        }
        public List<Contract> SearchContractorList(SearchContractorList searchCon)
        {
            List<Contract> Contractorlist = new List<Contract>();
            try
            {

                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    objCom = new SqlCommand();
                    objCom.CommandText = "[SearchContractorList]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@ContractManager ", searchCon.ValsparManager);
                    objCom.Parameters.AddWithValue("@Approver", searchCon.ContactID);
                    

                    objCom.Connection = objCon;
                    objCon.Open();
                    reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {

                        Contract cont = new Contract();

                        cont.ContractID = int.Parse(reader["ContractID"].ToString());
                        cont.CompanyName = reader["CompanyName"].ToString();
                        cont.SupervisorFirstName = reader["SupervisorFirstName"].ToString();

                        cont.EmailAddress = reader["EmailAddress"].ToString();
                        cont.MobileNo = reader["MobileNo"].ToString();
                        cont.FrequencyName = reader["Frequency"].ToString();
                        cont.LastAssessmentDate = reader["LastAssessmentDate"].ToString();
                        cont.NextAssessmentDate = reader["NextAssessmentDate"].ToString();

                        cont.ContactPerson = reader["ValsparContact"].ToString();

                        cont.Status = reader["Status"].ToString();
                        cont.ContractStatus = reader["ContractStatus"].ToString();
                        if (reader["Approver"] != DBNull.Value)
                        {
                            cont.ContactID = int.Parse(reader["Approver"].ToString());
                        }

                        cont.ContractorCreatedBy = reader["ContractManager"].ToString();
                        cont.ApproverComments = reader["Comments"].ToString();
                        if (reader["updatedby"] != DBNull.Value)
                        {
                            cont.updatedby = int.Parse(reader["updatedby"].ToString());
                        }
                        cont.Attachment = reader["Attachment"].ToString();
                        Contractorlist.Add(cont);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return Contractorlist;
        }

        public List<ContractorMaster> GetContratorsSelect(int? workTypeID)
        {

            using (SqlConnection objCon = new SqlConnection(constring))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "ContractorMasterSelect";
                if (workTypeID > 0)
                    objCom.Parameters.AddWithValue("@WorkTypeID", workTypeID);
                else
                    objCom.Parameters.AddWithValue("@WorkTypeID", DBNull.Value);

                objCom.CommandType = CommandType.StoredProcedure;
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();

                var contractorList = new List<ContractorMaster>();
                while (objReader.Read())
                {
                    var contractorMaster = new ContractorMaster();
                    contractorMaster.ContractorID = int.Parse(objReader["ContractorID"].ToString());
                    contractorMaster.ContractorName = objReader["CompanyName"].ToString();
                    contractorMaster.PermitHolderIdName = objReader["PermitHolderIdName"].ToString();
                    contractorMaster.EmailAddress = objReader["EmailAddress"].ToString();
                    contractorList.Add(contractorMaster);
                }
                objCon.Close();

                return contractorList;
            }
        }
        public int GeneralChecklistInsert(WorkPermit workPermit)
        {
            string genString = string.Empty;
            int affectedcount = 0;

            List<GenXML> genxml = new List<GenXML>();

            foreach (var list in workPermit.GeneralList)
            {
                if (list.Gencheck == true)
                {
                    var list1 = new GenXML
                    {
                        GeneralID = list.GenID,
                    };
                    genxml.Add(list1);
                }
            }
            XmlSerializer xmlSerializer = new XmlSerializer(genxml.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, genxml);

                genString = textWriter.ToString();
            }
            using (SqlConnection objCon = new SqlConnection(constring))
            {

                SqlCommand objCom = new SqlCommand();
                objCom.CommandText = "GeneralCheckListInsert";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@WorkPermitID", workPermit.WorkPermitID);
                objCom.Parameters.AddWithValue("@GeneralSave", genString);
                objCom.Connection = objCon;
                objCon.Open();
                objCom.Connection = objCon;
                affectedcount = objCom.ExecuteNonQuery();
                objCon.Close();
            }
            return affectedcount;
        }
        public List<CheckListMaster> GetCheckList(int workPermitID = 0)
        {
            using (SqlConnection objCon = new SqlConnection(constring))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "CheckListMasterSelect";
                objCom.Parameters.AddWithValue("@workPermitID", workPermitID);
                objCom.CommandType = CommandType.StoredProcedure;
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();

                var chklist = new List<CheckListMaster>();
                while (objReader.Read())
                {
                    var chklistmaster = new CheckListMaster();
                    chklistmaster.CheckListID = int.Parse(objReader["ID"].ToString());
                    chklistmaster.ISChecked = int.Parse(objReader["Checked"].ToString()) > 0 ? true : false;
                    chklistmaster.CheckListName = objReader["Name"].ToString();

                    chklist.Add(chklistmaster);
                }
                objCon.Close();
                //var contractorMaster1 = new ContractorMaster();
                //contractorMaster1.ContractorID = -1;
                //contractorMaster1.ContractorName = "N/A";

                //contractorList.Insert(1,contractorMaster1);
                return chklist;
            }
        }
        public List<GeneralCheckList> GetGeneralCheckList(int workpermitid = 0)
        {
            using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
            {
                SqlCommand objCom = new SqlCommand();
                objCom.CommandText = "GetGeneralCheckListSelect";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@workPermitID", workpermitid);

                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();


                var genlist = new List<GeneralCheckList>();
                while (objReader.Read())
                {
                    var general = new GeneralCheckList();
                    general.GenID = int.Parse(objReader["ID"].ToString());
                    general.Gencheck = int.Parse(objReader["Checked"].ToString()) > 0 ? true : false;
                    general.GenName = objReader["Name"].ToString();
                    genlist.Add(general);


                }

                objCon.Close();
                return genlist;
            }

        }
        public int AllCheckListInsert(WorkPermit workPermit, int[] AllCheckList)
        {
            string checklistString = string.Empty;
            int affectedcount = 0;

            List<CheckListWorkXML> CkList = new List<CheckListWorkXML>();

            if (AllCheckList != null)
            {
                foreach (var list3 in AllCheckList)
                {
                    var list4 = new CheckListWorkXML
                    {
                        CheckID = list3,

                    };
                    CkList.Add(list4);

                }
            }
            XmlSerializer xmlSerializer = new XmlSerializer(CkList.GetType());

            using (StringWriter textWriter1 = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter1, CkList);

                checklistString = textWriter1.ToString();
            }
            using (SqlConnection objCon = new SqlConnection(constring))
            {

                SqlCommand objCom = new SqlCommand();
                objCom.CommandText = "WorkTypeAllChecklistInsert";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@WorkPermitID", workPermit.WorkPermitID);
                objCom.Parameters.AddWithValue("@AllCheckListSave", checklistString);
                objCom.Connection = objCon;
                objCon.Open();
                objCom.Connection = objCon;
                affectedcount = objCom.ExecuteNonQuery();
                objCon.Close();
            }

            return affectedcount;
        }
        public int PPEInsert(WorkPermit workPermit)
        {
            int affectedcount = 0;
            try
            {
                string PPEString = string.Empty;


                List<PPEXML> ppeList = new List<PPEXML>();

                foreach (var list in workPermit.PPE)
                {
                    if (list.PPEcheck == true)
                    {
                        var list1 = new PPEXML
                        {
                            PPEID = list.PPEID,
                        };
                        ppeList.Add(list1);
                    }
                }
                XmlSerializer xmlSerializer = new XmlSerializer(ppeList.GetType());

                using (StringWriter textWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(textWriter, ppeList);

                    PPEString = textWriter.ToString();
                }
                using (SqlConnection objCon = new SqlConnection(constring))
                {

                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "PPEInsert";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@WorkPermitID", workPermit.WorkPermitID);
                    objCom.Parameters.AddWithValue("@PPESave", PPEString);
                    objCom.Connection = objCon;
                    objCon.Open();
                    objCom.Connection = objCon;
                    affectedcount = objCom.ExecuteNonQuery();
                    objCon.Close();
                }
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);

            }
            return affectedcount;
        }
        public int WorkTypeInsert(WorkPermit workPermit, int[] work)
        {
            string WorkTypeString = string.Empty;
            int affectedcount = 0;
            try
            {
                List<WorkTypeXML> WorkTypeList = new List<WorkTypeXML>();

                foreach (var list in work)
                {
                    var list1 = new WorkTypeXML
                    {
                        WorkTypeID = list,
                    };
                    WorkTypeList.Add(list1);

                }
                XmlSerializer xmlSerializer = new XmlSerializer(WorkTypeList.GetType());

                using (StringWriter textWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(textWriter, WorkTypeList);

                    WorkTypeString = textWriter.ToString();
                }
                using (SqlConnection objCon = new SqlConnection(constring))
                {

                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "TypeofWorkforCreatepermitInsert";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@WorkPermitID", workPermit.WorkPermitID);
                    objCom.Parameters.AddWithValue("@TypeofWorkforCreatePermit", WorkTypeString);
                    objCom.Connection = objCon;
                    objCon.Open();
                    objCom.Connection = objCon;
                    affectedcount = objCom.ExecuteNonQuery();
                    objCon.Close();
                }
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);

            }
            return affectedcount;
        }
        public int ContractQuestionnaireInsert(Contract contract)
        {
            string ChecklistString = string.Empty;
            int affectedcount = 0;

            List<OccupationalHealthSafetyXML> contractorchecklist = new List<OccupationalHealthSafetyXML>();

            foreach (var list in contract.OccupationalHealthSafety)
            {
                if (list.Ischecked != null)
                {
                    var list1 = new OccupationalHealthSafetyXML
                    {
                        CheckListID = list.HealthSafetyPolicy,
                        CheckedValue = list.Ischecked,
                        Remarks = list.Remarks,
                    };
                    contractorchecklist
                        .Add(list1);
                }
            }
            XmlSerializer xmlSerializer = new XmlSerializer(contractorchecklist.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, contractorchecklist);

                ChecklistString = textWriter.ToString();
            }
            using (SqlConnection objCon = new SqlConnection(constring))
            {

                SqlCommand objCom = new SqlCommand();
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.CommandText = "OccupationalHealthSafetyInsert";
                objCom.Parameters.AddWithValue("@ContractorID", contract.ContractID);
                objCom.Parameters.AddWithValue("@OccupationalHealthSafetySave", ChecklistString);
                objCom.Connection = objCon;
                objCon.Open();
                objCom.Connection = objCon;
                affectedcount = objCom.ExecuteNonQuery();
                objCon.Close();
            }
            return affectedcount;
        }
        public int WorkTypeContractInsert(Contract contract)
        {
            string WorkTypeString = string.Empty;
            int affectedcount = 0;

            List<WorkTypeXML> WorkTypeList = new List<WorkTypeXML>();

            foreach (var list in contract.WorkType)
            {
                if (list.Ischecked == true)
                {
                    var list1 = new WorkTypeXML
                    {
                        WorkTypeID = list.WorkTypeID,
                    };
                    WorkTypeList.Add(list1);
                }
            }
            XmlSerializer xmlSerializer = new XmlSerializer(WorkTypeList.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, WorkTypeList);

                WorkTypeString = textWriter.ToString();
            }
            using (SqlConnection objCon = new SqlConnection(constring))
            {

                SqlCommand objCom = new SqlCommand();
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.CommandText = "TypeofWorkInsert";
                objCom.Parameters.AddWithValue("@ContractorID", contract.ContractID);
                objCom.Parameters.AddWithValue("@TypeofWorkChecklist", WorkTypeString);
                objCom.Connection = objCon;
                objCon.Open();
                objCom.Connection = objCon;
                affectedcount = objCom.ExecuteNonQuery();
                objCon.Close();
            }
            return affectedcount;
        }
        public List<PersonalPerspectiveEquipment> GetPPE(int workpermitid = 0)
        {
            using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
            {
                SqlCommand objCom = new SqlCommand();
                objCom.CommandText = "PPESelect";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@workPermitID", workpermitid);

                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();


                var ppelist = new List<PersonalPerspectiveEquipment>();
                while (objReader.Read())
                {
                    var ppe = new PersonalPerspectiveEquipment();
                    ppe.PPEID = int.Parse(objReader["PPEID"].ToString());
                    ppe.PPEcheck = int.Parse(objReader["Checked"].ToString()) > 0 ? true : false;
                    ppe.PPEName = objReader["PPEName"].ToString();
                    ppelist.Add(ppe);


                }

                objCon.Close();
                return ppelist;
            }

        }
        public WorkPermit GetExtension(int workPermitID)
        {

            WorkPermit workPermitget = GetWorkPermit(workPermitID);
            WorkPermit workPermit = workPermitget;
            using (SqlConnection objCon = new SqlConnection(constring))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "[ExtensionGet]";
                objCom.Parameters.AddWithValue("@WorkPermitID", workPermitID);
                objCom.CommandType = CommandType.StoredProcedure;
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();

                //workPermit = new WorkPermit();
                if (objReader.Read())
                {

                    if (objReader["ExtensionFrom"] != DBNull.Value)
                    {
                        workPermit.ExtensionFrom = objReader["ExtensionFrom"].ToString();
                    }

                    if (objReader["ExtensionTo"] != DBNull.Value)
                    {
                        workPermit.ExtensionTo = objReader["ExtensionTo"].ToString();
                    }
                    if (objReader["ExtensionPermitIssuerID"] != DBNull.Value)
                    {
                        workPermit.ExtensionPermitIssuerID = int.Parse(objReader["ExtensionPermitIssuerID"].ToString());
                    }
                    if (objReader["ExtensionAreaOwnerID"] != DBNull.Value)
                    {
                        workPermit.ExtensionAreaOwnerID = int.Parse(objReader["ExtensionAreaOwnerID"].ToString());
                    }
                    if (objReader["ExtensionApproverID"] != DBNull.Value)
                    {
                        workPermit.ExtensionApproverID = int.Parse(objReader["ExtensionApproverID"].ToString());
                    }

                    if (objReader["ExtensionPermitHolder"] != DBNull.Value)
                    {
                        workPermit.ExtensionPermitHolder = objReader["ExtensionPermitHolder"].ToString();
                    }

                    workPermit.PermitHolderIdName = workPermit.ContractorID + "#" + workPermit.PermitHolderName;



                    var ppelist = new List<PersonalPerspectiveEquipment>();

                    objCon.Close();
                }

                return workPermit;
            }
        }
        public List<UserProfile> GetAreaOwner()
        {
            using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
            {
                SqlCommand objCom = new SqlCommand();
                objCom.CommandText = "AreaOwnerSelect";
                objCom.CommandType = CommandType.StoredProcedure;


                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();


                var areaOwner = new List<UserProfile>();
                while (objReader.Read())
                {
                    var AreaOwner = new UserProfile();
                    AreaOwner.UserID = int.Parse(objReader["UserID"].ToString());
                    AreaOwner.RoleName = objReader["RoleName"].ToString();
                    AreaOwner.DisplayUserName = objReader["UserFullName"].ToString();
                    areaOwner.Add(AreaOwner);


                }


                objCon.Close();
                return areaOwner;
            }
        }
        public WorkPermit GetAllApprovers(int workPermitID)
        {
            WorkPermit workPermit = null;
            using (SqlConnection objCon = new SqlConnection(constring))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "[MailApprover]";
                objCom.Parameters.AddWithValue("@PermitID", workPermitID);
                objCom.CommandType = CommandType.StoredProcedure;
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();

                workPermit = new WorkPermit();
                if (objReader.Read())
                {
                    workPermit.WorkPermitID = int.Parse(objReader["WorkPermitID"].ToString());

                    if (objReader["ApproverEmailAddress"] != DBNull.Value)
                    {
                        workPermit.MailApprover = objReader["ApproverEmailAddress"].ToString();
                    }
                    if (objReader["SafetyEmailAddress"] != DBNull.Value)
                    {
                        workPermit.MailSafetyOfficier = ',' + objReader["SafetyEmailAddress"].ToString();
                    }
                    if (objReader["ProcessEmailAddress"] != DBNull.Value)
                    {
                        workPermit.MailProcessManager = ',' + objReader["ProcessEmailAddress"].ToString();

                    }
                    if (objReader["ElectricalEmailAddress"] != DBNull.Value)
                    {
                        workPermit.MailElectrical = ',' + objReader["ElectricalEmailAddress"].ToString();
                    }
                    if (objReader["MechanicalEmailAddress"] != DBNull.Value)
                    {
                        workPermit.Mailmechanical = ',' + objReader["MechanicalEmailAddress"].ToString();
                    }
                    if (objReader["InstrumentEmailAddress"] != DBNull.Value)
                    {
                        workPermit.Mailinstrument = ',' + objReader["InstrumentEmailAddress"].ToString();
                    }
                    if (objReader["gmoperationsEmailAddress"] != DBNull.Value)
                    {
                        workPermit.Mailgmoperations = ',' + objReader["gmoperationsEmailAddress"].ToString();
                    }


                    objCon.Close();
                }

                return workPermit;
            }
        }
        public WorkPermit GetWorkPermit(int workPermitID)
        {
            WorkPermit workPermit = null;
            using (SqlConnection objCon = new SqlConnection(constring))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "[WorkPermitGet]";
                objCom.Parameters.AddWithValue("@WorkPermitID", workPermitID);
                objCom.CommandType = CommandType.StoredProcedure;
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();

                workPermit = new WorkPermit();
                if (objReader.Read())
                {
                    workPermit.WorkPermitID = int.Parse(objReader["WorkPermitID"].ToString());
                    workPermit.PermitNumber = objReader["WorkPermitNO"].ToString();
                    workPermit.ValidityFrom = objReader["ValidityFrom"].ToString();
                    workPermit.ValidityTo = objReader["ValidityTo"].ToString();
                    workPermit.Location = objReader["Location"].ToString();
                    workPermit.ContractorID = int.Parse(objReader["ContractorID"].ToString());
                    workPermit.PlantID = int.Parse(objReader["PlantID"].ToString());
                    workPermit.EquipmentID = int.Parse(objReader["EquipmentID"].ToString());
                    workPermit.WorkDescription = objReader["WorkDesctiption"].ToString();
                    workPermit.Risk = int.Parse(objReader["RiskAssessmentRequired"].ToString());
                    workPermit.PersonID = int.Parse(objReader["FireWatchRequired"].ToString());
                    workPermit.PermitHolderName = objReader["PermitHolderName"].ToString();
                    workPermit.NoOfPersonAtSite = objReader["NoOfPersonAtSite"].ToString();
                    workPermit.PermitIssuerID = int.Parse(objReader["PermitIssuerID"].ToString());
                    workPermit.PermitIssuerName = objReader["PermitIssuerName"].ToString();
                    workPermit.AdjacentAreaOwenerID = int.Parse(objReader["AdjacentAreaOwenerID"].ToString());
                    workPermit.ApproverID = int.Parse(objReader["ApproverID"].ToString());
                    workPermit.Status = objReader["Status"].ToString();
                    //workPermit.Attachment = objReader["Attachment"].ToString();
                    workPermit.Attachment = objReader["Attachement"].ToString();
                    workPermit.PPEOthers = objReader["PPEOthers"].ToString();
                    workPermit.MechanicalIncharge = int.Parse(objReader["MechanicalIncharge"].ToString());
                    workPermit.InstrumentalIncharge = int.Parse(objReader["InstrumentalIncharge"].ToString());
                    workPermit.SafetyOfficer = int.Parse(objReader["SafetyOfficer"].ToString());
                    workPermit.ElectricalIncharge = int.Parse(objReader["ElectricalIncharge"].ToString());
                    workPermit.GMOperations = int.Parse(objReader["GMOperations"].ToString());
                    workPermit.ProcessManager = int.Parse(objReader["ProcessManager"].ToString());
                    workPermit.JobID = objReader["JobRequestID"].ToString();
                    workPermit.EquipmentName = objReader["EquipmentName"].ToString();
                    workPermit.WorkTypeName = objReader["WorkType"].ToString();
                    workPermit.ContractorEmpID = int.Parse(objReader["ContractorEmpID"].ToString());
                    workPermit.DepartID = int.Parse(objReader["DeptID"].ToString());
                    workPermit.ExtensionPermitHolder = objReader["PermitHolderName"].ToString();

                    if (objReader["ExtensionPermitID"] != DBNull.Value)
                    {
                        workPermit.ExtensionPermitID = int.Parse(objReader["ExtensionPermitID"].ToString());
                    }

                    workPermit.PermitHolderIdName = workPermit.ContractorID + "#" + workPermit.PermitHolderName;

                    workPermit.ApproverComment = objReader["ApproverComment"].ToString();




                    if (objReader["WholeAttachment"] != DBNull.Value)
                    {
                        workPermit.WholeAttachment = objReader["WholeAttachment"].ToString();
                    }




                    if (objReader["ApprovedBy"] != DBNull.Value)
                    {
                        workPermit.ApprovedBy = int.Parse(objReader["ApprovedBy"].ToString());
                    }


                    if (objReader["ApprovedOn"] != DBNull.Value)
                    {
                        workPermit.ApprovedOn = objReader["ApprovedOn"].ToString();
                    }

                    if (objReader["ClosureComment"] != DBNull.Value)
                    {
                        workPermit.ClosureComment = objReader["ClosureComment"].ToString();
                    }

                    if (objReader["ClosedBy"] != DBNull.Value)
                    {
                        workPermit.ClosedBy = int.Parse(objReader["ClosedBy"].ToString());
                    }

                    if (objReader["ClosedOn"] != DBNull.Value)
                    {
                        workPermit.ClosedOn = objReader["ClosedOn"].ToString();
                    }
                    workPermit.ShiftInchName = objReader["ApproverName"].ToString();
                    workPermit.MechInchName = objReader["MechName"].ToString();
                    workPermit.MechInchRemarks = objReader["MechInchRemarks"].ToString();
                    if (objReader["MechDate"] != DBNull.Value)
                    {
                        workPermit.MechInchDatetime = objReader["MechDate"].ToString();
                    }
                    workPermit.ElecInchName = objReader["ElecName"].ToString();
                    workPermit.ElecInchRemarks = objReader["ElecInchargeRemarks"].ToString();
                    if (objReader["ElecDate"] != DBNull.Value)
                    {
                        workPermit.ElecInchDatetime = objReader["ElecDate"].ToString();
                    }
                    workPermit.InstruInchName = objReader["InstrName"].ToString();
                    workPermit.InstruInchRemarks = objReader["InstruInchargeRemarks"].ToString();
                    if (objReader["InstruDate"] != DBNull.Value)
                    {
                        workPermit.InstruInchDatetime = objReader["InstruDate"].ToString();
                    }
                    workPermit.SafetyOffName = objReader["SafetyName"].ToString();
                    workPermit.SafetyOffRemarks = objReader["SafetyRemarks"].ToString();
                    if (objReader["SafetyDate"] != DBNull.Value)
                    {
                        workPermit.SafetyOffDatetime = objReader["SafetyDate"].ToString();
                    }
                    workPermit.ProMgrName = objReader["ProMgrName"].ToString();
                    workPermit.ProMgrRemarks = objReader["ProcessMgrRemarks"].ToString();
                    if (objReader["ProMgrDate"] != DBNull.Value)
                    {
                        workPermit.ProMgrDatetime = objReader["ProMgrDate"].ToString();
                    }
                    workPermit.GMOpName = objReader["GMOpName"].ToString();
                    workPermit.GMOpRemarks = objReader["GMOperRemarks"].ToString();
                    if (objReader["GMOpDate"] != DBNull.Value)
                    {
                        workPermit.GMOpDatetime = objReader["GMOpDate"].ToString();
                    }
                }
                if (objReader.NextResult())
                {
                    if (objReader.Read())
                    {
                        if (objReader["ExtensionFrom"] != DBNull.Value)
                        {
                            workPermit.ExtensionFrom = objReader["ExtensionFrom"].ToString();
                        }

                        if (objReader["ExtensionTo"] != DBNull.Value)
                        {
                            workPermit.ExtensionTo = objReader["ExtensionTo"].ToString();
                        }
                        if (objReader["ExtensionPermitIssuerID"] != DBNull.Value)
                        {
                            workPermit.ExtensionPermitIssuerID = int.Parse(objReader["ExtensionPermitIssuerID"].ToString());
                        }
                        if (objReader["ExtensionAreaOwnerID"] != DBNull.Value)
                        {
                            workPermit.ExtensionAreaOwnerID = int.Parse(objReader["ExtensionAreaOwnerID"].ToString());
                        }
                        if (objReader["ExtensionApproverID"] != DBNull.Value)
                        {
                            workPermit.ExtensionApproverID = int.Parse(objReader["ExtensionApproverID"].ToString());
                        }

                        if (objReader["ExtensionPermitHolder"] != DBNull.Value)
                        {
                            workPermit.ExtensionPermitHolder = objReader["ExtensionPermitHolder"].ToString();
                        }
                        objReader.Close();
                    }

                    objCon.Close();
                }

                return workPermit;

            }
        }
        public List<UserProfile> GetApproversSelect(int? WorkPermitId)
        {

            using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
            {
                SqlCommand objCom = new SqlCommand();
                objCom.CommandText = "GetApproversSelect";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@WorkTypeID", WorkPermitId);

                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();

                var approvermaster = new List<UserProfile>();
                while (objReader.Read())
                {
                    var approver = new UserProfile();
                    approver.UserID = int.Parse(objReader["UserID"].ToString());

                    approver.DisplayUserName = objReader["UserFullName"].ToString();
                    approvermaster.Add(approver);


                }


                objCon.Close();
                return approvermaster;
            }

        }
        public WorkPermitList SearchApprovedList(WorkPermitList SearchApproval)
        {
            WorkPermitList pendingApprovalList = new WorkPermitList();
            using (SqlConnection objCon = new SqlConnection(constring))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "SearchApprovedPermitList";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@Fromdate", SearchApproval.FromDate);
                objCom.Parameters.AddWithValue("@Todate", SearchApproval.Todate);
                if (SearchApproval.ContractorID > 0)
                    objCom.Parameters.AddWithValue("@ContractorID", SearchApproval.ContractorID);
                else
                    objCom.Parameters.AddWithValue("@ContractorID", DBNull.Value);
                if (SearchApproval.EquipmentID > 0)
                    objCom.Parameters.AddWithValue("@EquipmentID", SearchApproval.EquipmentID);
                else
                    objCom.Parameters.AddWithValue("@EquipmentID", DBNull.Value);
                if (SearchApproval.PlantID > 0)
                    objCom.Parameters.AddWithValue("@PlantID", SearchApproval.PlantID);
                else
                    objCom.Parameters.AddWithValue("@PlantID", DBNull.Value);
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();

                var workPermitList = new List<WorkPermit>();
                int recortCount = 1;
                while (objReader.Read())
                {
                    var workPermit = new WorkPermit();
                    workPermit.SNO = recortCount++;
                    workPermit.WorkPermitID = int.Parse(objReader["WorkPermitID"].ToString());
                    workPermit.GetStatus = objReader["GetStatus"].ToString();
                    workPermit.WorkDescription = objReader["WorkDesctiption"].ToString();
                    workPermit.DepartmentName = objReader["DepartmentName"].ToString();
                    workPermit.WorkTypeName = objReader["WorkType"].ToString();
                    workPermit.ValidityFrom = objReader["ValidityFrom"].ToString();
                    workPermit.ValidityTo = objReader["ValidityTo"].ToString();
                    workPermit.PlantName = objReader["PlantName"].ToString();
                    workPermit.PermitHolderName = objReader["PermitHolderName"].ToString();
                    workPermit.ApproverName = objReader["ApproverName"].ToString();
                    workPermit.ApproverComment = objReader["ApproverComment"].ToString();
                    workPermit.PermitIssuerName = objReader["PermitIssuerName"].ToString();
                    workPermit.Attachment = objReader["Attachment"].ToString();
                    workPermit.ContractorName = objReader["CompanyName"].ToString();
                    workPermit.EquipmentName = objReader["EquipmentName"].ToString();
                    workPermit.PermitNumber = objReader["PermitNumber"].ToString();
                    workPermit.ExtensionDetails = GetExtensionList(int.Parse(objReader["WorkPermitID"].ToString()));
                    workPermitList.Add(workPermit);
                }
                objCon.Close();

                pendingApprovalList = new WorkPermitList();
                pendingApprovalList.WorkPermit = workPermitList;
                return pendingApprovalList;
            }
        }
        public int UpdateContatractEmp(EmpContractorprofile userprofile)
        {
            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "ContractorEmpUpdate";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@ConUserID", userprofile.UserID);
                    objCom.Parameters.AddWithValue("@FirstName", userprofile.FirstName);
                    objCom.Parameters.AddWithValue("@LastName", userprofile.LastName);
                    objCom.Parameters.AddWithValue("@DateOFBirth", DateTime.ParseExact(userprofile.DateOFBirth, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    objCom.Parameters.AddWithValue("@Age", userprofile.Age);
                    objCom.Parameters.AddWithValue("@IdentityCard", userprofile.IDDetail);
                    objCom.Parameters.AddWithValue("@ConCompanyID", userprofile.ContractID);
                    objCom.Parameters.AddWithValue("@ConEmployeeID", userprofile.EmployeeID);
                    objCom.Parameters.AddWithValue("@TrainingTypeID", userprofile.TrainingTypeID);
                    objCom.Parameters.AddWithValue("@TrainingDate", DateTime.ParseExact(userprofile.TraningDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    objCom.Parameters.AddWithValue("@FrequencyID", userprofile.FrequencyID);
                    objCom.Parameters.AddWithValue("@SkillsID", userprofile.SkillsID);
                    objCom.Parameters.AddWithValue("@DateOFJoining", DateTime.ParseExact(userprofile.DateOfJoining, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    if (!string.IsNullOrEmpty(userprofile.PFNumber))
                        objCom.Parameters.AddWithValue("@PFNumber", userprofile.PFNumber);
                    else
                        objCom.Parameters.AddWithValue("@PFNumber", DBNull.Value);


                    objCom.Parameters.AddWithValue("@EmergencyNumber", userprofile.EmergencyContactNumber);
                    objCom.Parameters.AddWithValue("@Remarks", userprofile.Remarks);
                    if (!string.IsNullOrEmpty(userprofile.MobileNumber))
                        objCom.Parameters.AddWithValue("@MobileNumber", userprofile.MobileNumber);
                    else
                        objCom.Parameters.AddWithValue("@MobileNumber", DBNull.Value);
                    objCom.Parameters.AddWithValue("@Address", userprofile.Address);
                    objCom.Parameters.AddWithValue("@IsActive", userprofile.IsActive == true ? "Y" : "N");
                    //objCom.Parameters.AddWithValue("@IsInvestigate", userprofile.IsInvesatigate == true ? "Y" : "N");
                    objCom.Parameters.AddWithValue("@Department", userprofile.DepartID);
                    objCom.Parameters.AddWithValue("@ModifiedBy", userprofile.UserID);

                    if (userprofile.ContractorProfile != null)
                    {
                        objCom.Parameters.AddWithValue("@UserImage", userprofile.ContractorProfile.FileName);
                    }
                    else if (!string.IsNullOrEmpty(userprofile.ProfilePictureName))
                    {
                        objCom.Parameters.AddWithValue("@UserImage", userprofile.ProfilePictureName);
                    }
                    else
                    {
                        objCom.Parameters.AddWithValue("@UserImage", DBNull.Value);
                    }

                    objCom.Connection = objCon;
                    objCon.Open();
                    affectedRecordCount = objCom.ExecuteNonQuery();
                    objCon.Close();
                }
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);
            }

            return affectedRecordCount;
        }
        public EmpContractorprofile GetContractorEmpUserProfile(int UserID)
        {
            EmpContractorprofile userProfile = null;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[ContractorEmpGet]";
                    objCom.Parameters.AddWithValue("@ConUserID", UserID);
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    objCom.Connection = objCon;
                    SqlDataReader objReader = objCom.ExecuteReader();
                    userProfile = new EmpContractorprofile();
                    if (objReader.Read())
                    {
                        userProfile.UserID = int.Parse(objReader["ContractorUserID"].ToString());
                        userProfile.FirstName = objReader["FirstName"].ToString();
                        userProfile.LastName = objReader["LastName"].ToString();
                        userProfile.DateOFBirth = objReader["DateOFBirth"].ToString();
                        userProfile.EmployeeID = objReader["ContractorEmpID"].ToString();
                        userProfile.MobileNumber = objReader["MobileNo"].ToString();
                        userProfile.EmergencyContactNumber = objReader["EmergencyNumber"].ToString();
                        userProfile.Age = int.Parse(objReader["Age"].ToString());
                        userProfile.ContractID = int.Parse(objReader["ContractorCompanyID"].ToString());
                        userProfile.IDDetail = objReader["IDCard"].ToString();
                        userProfile.IsActive = objReader["IsActive"].ToString() == "Y" ? true : false;
                        userProfile.IsInvesatigate = objReader["Investigate"].ToString() == "Y" ? true : false;
                        userProfile.DateOfJoining = objReader["DateOFJoining"].ToString();
                        userProfile.PFNumber = objReader["PFNumber"].ToString();
                        userProfile.Address = objReader["EmpAddress"].ToString();
                        userProfile.ProfilePictureName = objReader["UserImage"].ToString();
                        userProfile.DisplayUserName = objReader["DisplayUserName"].ToString();
                        userProfile.TrainingTypeID = int.Parse(objReader["TrainingTypeID"].ToString());
                        userProfile.SkillsID = int.Parse(objReader["SkillsID"].ToString());
                        userProfile.FrequencyID = int.Parse(objReader["FrequencyID"].ToString());
                        userProfile.DepartID = int.Parse(objReader["DepartmentID"].ToString());
                        userProfile.TraningDate = objReader["TraniningDate"].ToString();
                        userProfile.NextTrainingDate = objReader["NextTrainingDate"].ToString();
                        userProfile.Remarks = objReader["Remarks"].ToString();

                    }


                }
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);
            }
            return userProfile;
        }
        public List<EmpContractorprofile> SelectContractorList()
        {
            List<EmpContractorprofile> listContractoremp = null;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[ContractorEmpSelect]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    objCom.Connection = objCon;
                    SqlDataReader objReader = objCom.ExecuteReader();
                    listContractoremp = new List<EmpContractorprofile>();
                    while (objReader.Read())
                    {
                        listContractoremp.Add(new EmpContractorprofile
                        {
                            UserID = int.Parse(objReader["ContractorUserID"].ToString()),
                            FirstName = objReader["FirstName"].ToString(),
                            LastName = objReader["LastName"].ToString(),
                            EmployeeID = objReader["ContractorEmpID"].ToString(),
                            MobileNumber = objReader["MobileNumber"].ToString(),

                            ConCompanyName = objReader["CompanyName"].ToString(),
                            IsActiveSelect = objReader["IsActive"].ToString(),
                            UserImage = objReader["UserImage"].ToString(),
                            DisplayUserName = objReader["DisplayUserName"].ToString(),
                            TrainingTypeName = objReader["TrainingType"].ToString(),
                            SkillsName = objReader["SkillsID"].ToString(),
                            FrequencyName = objReader["FrequencyName"].ToString(),
                            DeptName = objReader["Department"].ToString(),
                            TraningDate = objReader["TraniningDate"].ToString(),
                            NextTrainingDate = objReader["NextTrainingDate"].ToString(),
                            Remarks = objReader["Remarks"].ToString(),
                            IsInvestigateSelect = objReader["Investigate"].ToString(),
                        });
                    }
                }
                LogManager.Instance.Info("MonitPro.DAL.AccountDA.SelectUserProfile Method - End");
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);
            }
            return listContractoremp;
        }
        public List<ContractorSkills> GetSkills()
        {
            using (SqlConnection objCon = new SqlConnection(constring))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "GetContractorSkills";
                objCom.CommandType = CommandType.StoredProcedure;
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();
                var skills = new List<ContractorSkills>();

                while (objReader.Read())
                {
                    var conskils = new ContractorSkills();
                    conskils.SkillsID = int.Parse(objReader["ID"].ToString());
                    conskils.SkillName = objReader["Name"].ToString();
                    skills.Add(conskils);
                }

                objCon.Close();

                return skills;
            }
        }

        public int InsertContractorEmp(NewContractor createcontractemp)
        {
            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "ContractorEmpInsert";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@FirstName", createcontractemp.FirstName);
                    objCom.Parameters.AddWithValue("@LastName", createcontractemp.LastName);
                    objCom.Parameters.AddWithValue("@DateOFBirth", DateTime.ParseExact(createcontractemp.DateOFBirth, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    objCom.Parameters.AddWithValue("@Age", createcontractemp.Age);
                    objCom.Parameters.AddWithValue("@ConCompanyID", createcontractemp.ContractID);
                    objCom.Parameters.AddWithValue("@ConEmployeeID", createcontractemp.EmployeeID);
                    objCom.Parameters.AddWithValue("@IdentityCard", createcontractemp.IDDetail);
                    objCom.Parameters.AddWithValue("@TrainingTypeID", createcontractemp.TrainingTypeID);
                    objCom.Parameters.AddWithValue("@TrainingDate", DateTime.ParseExact(createcontractemp.TraningDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    objCom.Parameters.AddWithValue("@FrequencyID", createcontractemp.FrequencyID);
                    objCom.Parameters.AddWithValue("@SkillsID", createcontractemp.SkillsID);
                    objCom.Parameters.AddWithValue("@EmergencyNumber", createcontractemp.EmergencyContactNumber);
                    objCom.Parameters.AddWithValue("@Remarks", createcontractemp.Remarks);
                    if (!string.IsNullOrEmpty(createcontractemp.MobileNumber))
                        objCom.Parameters.AddWithValue("@MobileNumber", createcontractemp.MobileNumber);
                    else
                        objCom.Parameters.AddWithValue("@MobileNumber", DBNull.Value);
                    objCom.Parameters.AddWithValue("@DateOFJoining", DateTime.ParseExact(createcontractemp.DateOfJoining, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    if (!string.IsNullOrEmpty(createcontractemp.PFNumber))
                        objCom.Parameters.AddWithValue("@PFNumber", createcontractemp.PFNumber);
                    else
                        objCom.Parameters.AddWithValue("@PFNumber", DBNull.Value);

                    objCom.Parameters.AddWithValue("@IsActive", createcontractemp.IsActive == true ? "Y" : "N");
                    objCom.Parameters.AddWithValue("@Address", createcontractemp.Address);
                    objCom.Parameters.AddWithValue("@Department", createcontractemp.DepartID);
                    objCom.Parameters.AddWithValue("@CreatedBy", createcontractemp.UserID);

                    objCom.Connection = objCon;
                    objCon.Open();
                    affectedRecordCount = objCom.ExecuteNonQuery();
                    objCon.Close();
                }
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);
            }

            return affectedRecordCount;
        }
        public List<ContractorMaster> GetContractorCompany()
        {
            using (SqlConnection objCon = new SqlConnection(constring))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "ContractorCompanySelect";

                objCom.CommandType = CommandType.StoredProcedure;
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();

                var contractorList = new List<ContractorMaster>();
                while (objReader.Read())
                {
                    var contractorMaster = new ContractorMaster();
                    contractorMaster.ContractorID = int.Parse(objReader["ContractorID"].ToString());
                    contractorMaster.ContractorName = objReader["ContractorName"].ToString();
                    contractorMaster.PermitHolderIdName = objReader["PermitHolderIdName"].ToString();

                    contractorList.Add(contractorMaster);
                }
                objCon.Close();

                return contractorList;
            }

        }
        public List<ContractorTrainingType> GetTrainingType()
        {
            using (SqlConnection objCon = new SqlConnection(constring))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "GetContractorTraningType";
                objCom.CommandType = CommandType.StoredProcedure;
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();
                var contraining = new List<ContractorTrainingType>();

                while (objReader.Read())
                {
                    var contrain = new ContractorTrainingType();
                    contrain.TypeID = int.Parse(objReader["ID"].ToString());
                    contrain.TypeName = objReader["Name"].ToString();
                    contraining.Add(contrain);
                }

                objCon.Close();

                return contraining;
            }
        }
        public List<FrequencyOfEvaluation> GetfrequencyOfEvaluationList()
        {
            using (SqlConnection objCon = new SqlConnection(constring))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "FrequencyForContractor";
                objCom.CommandType = CommandType.StoredProcedure;
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();
                var frequencyOfEvaluationList = new List<FrequencyOfEvaluation>();

                while (objReader.Read())
                {
                    var FrequencyofEvaluation = new FrequencyOfEvaluation();
                    FrequencyofEvaluation.FrequencyID = int.Parse(objReader["FrequencyID"].ToString());
                    FrequencyofEvaluation.FrequencyName = objReader["FrequencyName"].ToString();
                    frequencyOfEvaluationList.Add(FrequencyofEvaluation);
                }

                objCon.Close();

                return frequencyOfEvaluationList;
            }
        }
        public AssignTypeofWorkForApproverModel AssignApprover()
        {
            AssignTypeofWorkForApproverModel assignApprover = new AssignTypeofWorkForApproverModel();
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "AssignApprover";
                    objCom.CommandType = CommandType.StoredProcedure;

                    objCom.Connection = objCon;

                    DataSet dsResult = new DataSet();
                    SqlDataAdapter objAdapter = new SqlDataAdapter();
                    objAdapter.SelectCommand = objCom;
                    objAdapter.Fill(dsResult);

                    assignApprover.Users = new List<User>();
                    foreach (DataRow item in dsResult.Tables[0].Rows)
                    {

                        assignApprover.Users.Add(
                            new User
                            {
                                UserID = int.Parse(item["UserID"].ToString()),
                                FullName = item["UserFullName"].ToString()
                            }
                            );
                    }


                    assignApprover.ContractorApprover = new List<UserProfile>();
                    foreach (DataRow item in dsResult.Tables[1].Rows)
                    {

                        assignApprover.ContractorApprover.Add(
                            new UserProfile
                            {
                                UserID = int.Parse(item["UserID"].ToString()),
                                DisplayUserName = item["UserFullName"].ToString()
                            }
                            );
                    }
                }

                using (SqlConnection objCon = new SqlConnection(constring))
                {
                    var objCom = new SqlCommand();
                    objCom.CommandText = "GetAssignTypeofWorkforApprover";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    objCom.Connection = objCon;
                    SqlDataAdapter adapter = new SqlDataAdapter(objCom);
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet);
                    var WorkTypeList = new List<WorkTypeMaster>();
                    var newWorkTypeList = new List<NewWorkType>();
                    for (int rows = 0; rows < dataSet.Tables[0].Rows.Count; rows++)
                    {

                        var workType = new WorkTypeMaster();
                        workType.UserID = int.Parse(dataSet.Tables[0].Rows[rows][0].ToString());
                        workType.UserName = dataSet.Tables[0].Rows[rows][1].ToString();

                        int j = 1;
                        for (int i = 2; i <= 7; i++)
                        {

                            var newworktype = new NewWorkType();
                            newworktype.WorkTypeID = j;
                            newworktype.Ischecked = int.Parse(dataSet.Tables[0].Rows[rows][i].ToString()) > 0 ? true : false;
                            j++;

                            newWorkTypeList.Add(newworktype);
                        }

                        workType.NewWorkTypes = newWorkTypeList;
                        WorkTypeList.Add(workType);
                        assignApprover.WorkType = WorkTypeList;
                        newWorkTypeList = new List<NewWorkType>();
                    }

                }

            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);
            }

            return assignApprover;
        }
        public Evaluation GetEvaluation(int ContractorID = 0)
        {
            using (SqlConnection objCon = new SqlConnection(constring))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "EvaluationFormSelect";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@ContractorID", ContractorID);
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();

                var list = new Evaluation();
                while (objReader.Read())
                {
                    list.ContractorID = int.Parse(objReader["ContractorID"].ToString());
                    list.ContractorName = objReader["ContractorName"].ToString();
                    list.EvaluationPeriod = objReader["EvaluationPeriod"].ToString();
                    list.FrequencyofEvaluation = objReader["FrequencyEvaluation"].ToString();
                    list.EvaluatedBy = objReader["EvaluatedBy"].ToString();
                    list.SWContact = objReader["SWContact"].ToString();
                    list.EvaluationDate = objReader["EvaluatedDate"].ToString();
                    list.RatingID = int.Parse(objReader["OverallContractorRatingID"].ToString());
                    list.AttachmentName = objReader["Attachment"].ToString();
                }
                objCon.Close();



                return list;
            }
        }
        public List<EvaluationCriteriaCheckList> GetEvaluationCheckList(int ContractorID = 0)
        {
            using (SqlConnection objCon = new SqlConnection(constring))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "EvaluationCheckListSelect";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@ContractorID", ContractorID);
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();

                var Evaluationchecklist = new List<EvaluationCriteriaCheckList>();
                while (objReader.Read())
                {
                    var checklist = new EvaluationCriteriaCheckList();
                    checklist.EvaluationQuestionsID = int.Parse(objReader["EvaluationQuestionID"].ToString());
                    checklist.Ischecked = objReader["Checked"].ToString();
                    checklist.EvaluationQuestions = objReader["EvaluationQuestion"].ToString();
                    checklist.Remarks = objReader["Remarks"].ToString();
                    Evaluationchecklist.Add(checklist);
                }
                objCon.Close();



                return Evaluationchecklist;
            }
        }
        public List<OverallRating> GetRatingList()
        {
            using (SqlConnection objCon = new SqlConnection(constring))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "EvaluationRatingSelect";
                objCom.CommandType = CommandType.StoredProcedure; ;
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();

                var Ratinglist = new List<OverallRating>();
                while (objReader.Read())
                {
                    var list = new OverallRating();
                    list.RatingID = int.Parse(objReader["ID"].ToString());
                    list.RatingName = objReader["Name"].ToString();
                    list.Description = objReader["Description"].ToString();
                    Ratinglist.Add(list);
                }
                objCon.Close();



                return Ratinglist;
            }
        }
        public int EvaluationCriteriaInsert(Evaluation evaluation)
        {
            string ChecklistString = string.Empty;
            int affectedcount = 0;

            List<EvaluationCriteriaXML> evaluationchecklist = new List<EvaluationCriteriaXML>();

            foreach (var list in evaluation.EvaluationCriteriaCheckList)
            {
                if (list.Ischecked != null)
                {
                    var list1 = new EvaluationCriteriaXML
                    {
                        CheckListID = list.EvaluationQuestionsID,
                        CheckedValue = list.Ischecked,
                        Remarks = list.Remarks,
                    };
                    evaluationchecklist.Add(list1);
                }
            }
            XmlSerializer xmlSerializer = new XmlSerializer(evaluationchecklist.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, evaluationchecklist);

                ChecklistString = textWriter.ToString();
            }
            using (SqlConnection objCon = new SqlConnection(constring))
            {

                SqlCommand objCom = new SqlCommand();
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.CommandText = "EvaluationInsert";
                objCom.Parameters.AddWithValue("@ContractorID", evaluation.ContractorID);
                objCom.Parameters.AddWithValue("@EvalutionCriteriaSave", ChecklistString);
                objCom.Connection = objCon;
                objCon.Open();
                objCom.Connection = objCon;
                affectedcount = objCom.ExecuteNonQuery();
                objCon.Close();
            }
            return affectedcount;
        }
        public Contract GetContract(int contractID)
        {
            Contract contract = null;


            using (SqlConnection objCon = new SqlConnection(constring))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "[ContractGet]";
                objCom.Parameters.AddWithValue("@ContractID", contractID);
                objCom.CommandType = CommandType.StoredProcedure;
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();

                contract = new Contract();
                while (objReader.Read())
                {
                    contract.ContractID = int.Parse(objReader["ContractID"].ToString());
                    contract.CompanyName = objReader["CompanyName"].ToString();
                    if (objReader["ValsparContact"] != DBNull.Value)
                    {
                        contract.ContactID = int.Parse(objReader["ValsparContact"].ToString());
                    }

                    contract.SupervisorFirstName = objReader["SupervisorFirstName"].ToString();
                    contract.SupervisorLastName = objReader["SupervisorLastName"].ToString();
                    contract.FrequencyID = int.Parse(objReader["Frequency"].ToString());
                    contract.Address = objReader["Address"].ToString();
                    contract.Street = objReader["Street"].ToString();
                    contract.City = objReader["City"].ToString();
                    contract.State = objReader["State"].ToString();
                    contract.Country = objReader["Country"].ToString();
                    contract.PostCode = objReader["PostCode"].ToString();
                    contract.MobileNo = objReader["MobileNo"].ToString();
                    contract.EmailAddress = objReader["EmailAddress"].ToString();
                    contract.NOBusinessTypeID = int.Parse(objReader["NatureOfBusiness"].ToString());
                    contract.SubcontractorInvolved = int.Parse(objReader["SubcontractorInvolved"].ToString()) == 1 ? true : false;
                    contract.Have = int.Parse(objReader["Have"].ToString()) == 1 ? true : false;
                    contract.Attachment = objReader["Attachment"].ToString();
                    contract.AssessmentDate = objReader["AssessmentDate"].ToString();
                    contract.Isactiveselect = int.Parse(objReader["IsactiveSelect"].ToString()) == 1 ? true : false;
                    contract.ContractStatus = objReader["ContractStatus"].ToString();
                    contract.updatedby = int.Parse(objReader["LastUpdatedBy"].ToString());
                    if (objReader["ValsparManager"] != DBNull.Value)
                    {
                        contract.ValsparManager = int.Parse(objReader["ValsparManager"].ToString());
                    }
                }
                objCon.Close();
            }



            return contract;
        }

    }
}
