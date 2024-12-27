using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitPro.Models.PSSR;
using MonitPro.DAL;
using System.IO;

namespace MonitPro.BLL
{
    public class PSSRBLL
    {
        PSSRDAL pssrDAL = new PSSRDAL();
      
        public void UpdateTargetDateRequest(int Recomid, int identity, string Comments=null, string RevisedTargetDate = null)
        {
            pssrDAL.UpdateTargetDateRequest(Recomid,identity,Comments,RevisedTargetDate);
        }
        public List<ActionByRecomStatus> GetActionByRecomStatusCount(DateTime startDate, DateTime endDate)
        {
            return pssrDAL.GetActionByRecomStatusCount(startDate, endDate);
        }

        public List<CAPAPlantwiseCount> GetCAPAPlantCount(DateTime startDate, DateTime endDate)
        {
            return pssrDAL.GetCAPAPlantCountDAL(startDate, endDate);
        }
        public List<MonthwiseStatus> GetMonthwiseStatusCount(DateTime startDate, DateTime endDate)
        {
            return pssrDAL.GetMonthwiseStatusCount(startDate, endDate);
        }


        public List<PSSRCategoryModel> GetPSSRCatory()
        {
            return pssrDAL.GetPSSRCatory();
        }
        public List<Plants> GetPlants()
        {
            return pssrDAL.GetPlants();
        }
        public List<PSSRType> GetPSSRType()
        {
            return pssrDAL.GetPSSRType();
        }
        public List<Priority> GetPriority()
        {
            return pssrDAL.GetPriority();
        }
        public List<AssignTeamViewModel> GetAllPSSRAssign()
        {
            return pssrDAL.GetAllPSSRAssign();
        }
        public List<PSSRStatus> GetPSSRStatus()
        {
            return pssrDAL.GetPSSRStatus();
        }
        public List<Employee> GetAllEmployees()
        {
            return pssrDAL.GetAllEmployees();
        }
        public List<RecommStatusModel> RecommStatusListDD()
        {
            return pssrDAL.RecommStatusListDD();
        }
        public List<Employee> GetAssignTeamMembers(int PSSRID)
        {
            return pssrDAL.GetAssignTeamMembers(PSSRID);
        }
        public List<Department> GetDepartmentList()
        {
            return pssrDAL.GetDepartmentList();
        }
        public int PSSRInsertUpdate(CreatePSSRModel createmodel)
        {
            int PSSRID = 0;
            if (createmodel.PSSRAttach != null)
            {

                var fileName = Path.GetFileName(createmodel.PSSRAttach.FileName);


                PSSRID = pssrDAL.PSSRInsertUpdate(createmodel, fileName);
            }
            else
            {
                var fileName = "";

                PSSRID = pssrDAL.PSSRInsertUpdate(createmodel, fileName);
            }
            return PSSRID;
        }
        public List<PSSRListViewModel> GetAllPSSRList()
        {
            return pssrDAL.GetAllPSSRList();
        }
        public CreatePSSRModel GetPSSR(int PSSRID)
        {
            return pssrDAL.GetPSSR(PSSRID);
        }
        public AssignTeamViewModel GetPSSRAssign(int PSSRID)
        {
            return pssrDAL.GetPSSRAssign(PSSRID);
        }
        public void PSSRAssignTeamInsert(int PSSRID, int Coordinator, int ChairPerson, int OL, int HSELead, int EnggLead, int PSSRLead, string MemberList, int UserID)
        {
            if (MemberList.IndexOf(',') == 0)
            {
                MemberList = MemberList.Remove(0, 1);
            }
            pssrDAL.PSSRAssignTeamInsert(PSSRID, Coordinator, ChairPerson, OL, HSELead, EnggLead, PSSRLead, MemberList, UserID);
        }
        public List<AllPSSR_Observation> GetAllPSSRObservation()
        {
            return pssrDAL.GetAllPSSRObservation();
        }
        public void DeletePSSROBImage(int obid)
        {
            pssrDAL.DeletePSSROBImage(obid);
        }
            public int PSSRObservationInsertUpdate(PSSR_Observation createmodel)
        {
            return pssrDAL.PSSRObservationInsertUpdate(createmodel);
        }
        public List<AllPSSR_Observation> GetOBList(int PSSRID, int OBID)
        {
            return pssrDAL.GetOBList(PSSRID, OBID);
        }
        public PSSR_Observation EditObservation(int RecommID)
        {
            return pssrDAL.EditObservation(RecommID);
        }
        public List<CheckList> GetCheckList(int PCMID)
        {
            return pssrDAL.GetCheckList(PCMID);
        }
        public List<CheckList> PSSRSaveCheckList(int pcmID, int PSSRID)
        {
            return pssrDAL.PSSRSaveCheckList(pcmID, PSSRID);
        }
        public List<CheckListDD> GetCheckListDD()
        {
            return pssrDAL.GetCheckListDD();
        }
        public List<CheckListDD> GetAssignedChecklist(int PSSRID)
        {
            return pssrDAL.GetAssignedChecklist(PSSRID);
        }
        public int CheckListInsert(MainCheckListModel checklistModel)
        {
            return pssrDAL.CheckListInsert(checklistModel);
        }
        public List<CheckList> GetOverallCheckList(int PSSRID)
        {
            return pssrDAL.GetOverallCheckList(PSSRID);
        }
        public List<PSSRListViewModel> SearchPSSRList(SearchList searchModel)
        {
            return pssrDAL.SearchPSSRList(searchModel);
        }
        public List<AllPSSR_Observation> SearchPSSRObservation(SearchPSSRObservation searchModel)
        {
            return pssrDAL.SearchPSSRObservation(searchModel);
        }
        public void UpdatePSSRStatus(int PSSRID, int StatusID, string Comments, int userID)
        {
            pssrDAL.UpdatePSSRStatus(PSSRID, StatusID, Comments, userID);
        }
        public TargetDateApprove GetRequestedTargetDateDetails(int Recommid)
        {
            return pssrDAL.GetRequestedTargetDateDetails(Recommid);
        }
        public List<PSSRHistoryViewModel> GetAllPSSRHistoryList()
        {
            return pssrDAL.GetAllPSSRHistoryList();
        }
        public List<PSSRHistoryViewModel> SearchPSSRHistoryList(PSSRSearchHistory searchModel)
        {
            return pssrDAL.SearchPSSRHistoryList(searchModel);
        }
        public List<MOCNumberListModel> GetMocNumberList(int? PlantID)
        {
            return pssrDAL.GetMocNumberList(PlantID);
        }
    }
}
