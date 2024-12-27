using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitPro.DAL;
using MonitPro.Models;
namespace MonitPro.BLL
{
    public class WorkPermitBLL
    {
        WorkPermitDAL workPermitDAL = new WorkPermitDAL();
        

        public WorkPermitBLL()
        {

        }

        public List<WorkTypeMaster> GetWorkType(int workPermitID = 0)
        {
            return workPermitDAL.GetWorkType(workPermitID);
        }
        public List<WorkTypeMaster> GetWorkTypeContractor(int workPermitID = 0)
        {
            return workPermitDAL.GetWorkTypeContractor(workPermitID);
        }
        public List<ExtensionDetailsList> GetExtensionList(int workPermitID)
        {
            return workPermitDAL.GetExtensionList(workPermitID);
        }
        public List<OccupationalHealthSafetyCheckList> GetContractorCheckList(int ContractorID = 0)
        {
            return workPermitDAL.GetContractorCheckList(ContractorID);
        }
        public List<ContractorMaster> GetContratorsSelect(int? workTypeID)
        {
            return workPermitDAL.GetContratorsSelect(workTypeID);
        }
        public int GeneralChecklistInsert(WorkPermit workPermit)
        {
            return workPermitDAL.GeneralChecklistInsert(workPermit);
        }
        public List<CheckListMaster> GetCheckList(int workPermitID = 0)
        {
            return workPermitDAL.GetCheckList(workPermitID);
        }
        public List<Equipment> GetPlantEquipmentSelect(int? PlantID)
        {
            return workPermitDAL.GetPlantEquipmentSelect(PlantID);
        }
        public List<GeneralCheckList> GetGeneralCheckList(int workpermitid = 0)
        {
            return workPermitDAL.GetGeneralCheckList(workpermitid);
        }
        public List<PersonalPerspectiveEquipment> GetPPE(int workpermitid = 0)
        {
            return workPermitDAL.GetPPE(workpermitid);
        }
        public int WorkTypeInsert(WorkPermit workPermit, int[] work)
        {
            return workPermitDAL.WorkTypeInsert(workPermit, work);
        }

        public int AllCheckListInsert(WorkPermit workPermit, int[] AllCheckList)
        {
            return workPermitDAL.AllCheckListInsert(workPermit, AllCheckList);
        }
        public int PPEInsert(WorkPermit workPermit)
        {
            return workPermitDAL.PPEInsert(workPermit);
        }
        public int WorkTypeContractInsert(Contract contract)
        {
            return workPermitDAL.WorkTypeContractInsert(contract);
        }
        public int ContractQuestionnaireInsert(Contract contract)
        {
            return workPermitDAL.ContractQuestionnaireInsert(contract);
        }
        public WorkPermit GetExtension(int workPermitID)
        {
            return workPermitDAL.GetExtension(workPermitID);
        }
        public List<UserProfile> GetAreaOwner()
        {
            return workPermitDAL.GetAreaOwner();
        }
        public WorkPermit GetWorkPermit(int workPermitID)
        {
            return workPermitDAL.GetWorkPermit(workPermitID);
        }
        public WorkPermit GetAllApprovers(int workPermitID)
        {
            return workPermitDAL.GetAllApprovers(workPermitID);
        }
        public List<UserProfile> GetApproversSelect(int? WorkPermitId)
        {
            return workPermitDAL.GetApproversSelect(WorkPermitId);
        }
        public WorkPermitList SearchApprovedList(WorkPermitList SearchApproval)
        {
            return workPermitDAL.SearchApprovedList(SearchApproval);
        }
        public int UpdateContatractEmp(EmpContractorprofile userprofile)
        {
            return workPermitDAL.UpdateContatractEmp(userprofile);
        }
        public EmpContractorprofile GetContractorEmpUserProfile(int UserID)
        {
            return workPermitDAL.GetContractorEmpUserProfile(UserID);
        }
        public List<EmpContractorprofile> SelectContractorList()
        {
            return workPermitDAL.SelectContractorList();
        }

        public List<EmpContractorprofile> SearchContractorEmployee(SearchContractorEmployee searchContractor)
        {
            return workPermitDAL.SearchContractorEmployee(searchContractor);
        }
        public List<Contract> SearchContractorList(SearchContractorList searchCon)
        {
            return workPermitDAL.SearchContractorList(searchCon);
        }
            public List<ContractorSkills> GetSkills()
        {
            return workPermitDAL.GetSkills();
        }
        public int InsertContractorEmp(NewContractor createcontractemp)
        {
            return workPermitDAL.InsertContractorEmp(createcontractemp);
        }
        public List<ContractorMaster> GetContractorCompany()
        {
            return workPermitDAL.GetContractorCompany();
        }
        public List<ContractorTrainingType> GetTrainingType()
        {
            return workPermitDAL.GetTrainingType();
        }
        public List<FrequencyOfEvaluation> GetfrequencyOfEvaluationList()
        {
            return workPermitDAL.GetfrequencyOfEvaluationList();
        }
        public AssignTypeofWorkForApproverModel AssignApprover()
        {
            return workPermitDAL.AssignApprover();
        }
        public Evaluation GetEvaluation(int ContractorID = 0)
        {
            return workPermitDAL.GetEvaluation(ContractorID);
        }
        public List<EvaluationCriteriaCheckList> GetEvaluationCheckList(int ContractorID = 0)
        {
            return workPermitDAL.GetEvaluationCheckList(ContractorID);
        }
        public List<OverallRating> GetRatingList()
        {
            return workPermitDAL.GetRatingList();
        }
        public int EvaluationCriteriaInsert(Evaluation evaluation)
        {
            return workPermitDAL.EvaluationCriteriaInsert(evaluation);
        }
        public Contract GetContract(int contractID)
        {
            return workPermitDAL.GetContract(contractID);
        }
        public SessionDetails GetSession(int userid)
        {
            IncidentReportDAL IncidentDAL = new IncidentReportDAL();
            return IncidentDAL.GetSession(userid);
        }
    }
}

