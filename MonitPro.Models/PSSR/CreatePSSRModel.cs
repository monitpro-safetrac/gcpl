using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MonitPro.Models.Incident;
namespace MonitPro.Models.PSSR
{
    public class CreatePSSRModel : BaseEntity
    {
        public HttpPostedFileBase PSSRAttach { get; set; }
        public long RoleID { get; set; }
        public int SaveButton { get; set; }
        public string FileName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int PlantID { get; set; }
        public int PSSRID { get; set; }
        public int CategoryID { get; set; }
        public string SystemDesc { get; set; }
        public int MOCNo { get; set; }
        public int PSSRType { get; set; }
        public string AssessmentDatetime { get; set; }
        public string CreatedBy { get; set; }
        public int CreatedByID { get; set; }
        public int PSSRStatus { get; set; }
        public string PlantName { get; set; }
        public string CategoryName { get; set; }
        public string CreatedDateTime { get; set; }
 
        public string ChairPersonComments { get; set; }
        public string OperationHeadComments { get; set; }
        public string HSELeadComments { get; set; }
        public string EnggLeadComments { get; set; }
        public string PSSRLeadComments { get; set; }
        public string ChairPersonDateTime { get; set; }
        public string OperationHeadDateTime { get; set; }
        public string HSELeadDateTime { get; set; }
        public string EnggLeadDateTime { get; set; }
        public string PSSRLeadDateTime { get; set; }
        public string ClosureComments { get; set; }
        public List<PSSRCategoryModel> PSSRCategoryList { get; set; }
        public List<Plants> PSSRPlantList { get; set; }
        public List<PSSRListViewModel> PSSRListView { get; set; }
        public List<PSSRType> PSSRTypeList { get; set; }
        public List<CheckListDD> GetCheckLists { get; set; }
        public List<PSSRStatus> PSSRStatusList { get; set; }
        public List<PSSR_Observation> PSSRObservation { get; set; }
        public AssignTeamViewModel GetAssignTeams { get; set; }
        public List<AssignTeamViewModel> GetAllAssignTeams { get; set; }
        public List<AllPSSR_Observation> AllPSSRObservation { get; set; }
        public List<MOCNumberListModel> MOCList { get; set; }
        public List<CheckList> IdentityChecklist { get; set; }
        public SearchList searchList = new SearchList();
    }
    public class Plants
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class PSSRType
    {
        public int PTID { get; set; }
        public string PTName { get; set; }
    }
   
}
