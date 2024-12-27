using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MonitPro.Models.PSSR
{
    public class PSSR_Observation : BaseEntity
    {
        public long RoleID { get; set; }
        public int PSSRID { get; set; }
        public int RecommID { set; get; }
        public string RecommText { set; get; }
        public string CategoryName { get; set; }
        public string PriorityName { get; set; }
        public string PlantName { get; set; }
        public string ActionByName { get; set; }
        public int PSSRCategory { set; get; }
        public string TargetDate { set; get; }
        public int PriorityID { set; get; }
        public string CompletedDate { set; get; }
        public int ActionBy { set; get; }
        public string ActionTaken { set; get; }
        public string Remarks { set; get; }
        public int RecommUserID { set; get; }
        public int RecommStatus { set; get; }
        public string RecommStatusName { get; set; }
        public string PSSRObAttachmentName { get; set; }
        public int RequestIdentity { get; set; }
        public HttpPostedFileBase PSSRObAttachment { get; set; }
        public CreatePSSRModel PSSRModel { set; get; }
        public List<PSSRCategoryModel> PSSRCategoryList { set; get; }
        public List<Priority> PriorityList { get; set; }
        public List<Employee> EmployeeList { get; set; }
        public List<Plants> PlantList { get; set; }
       public List<AllPSSR_Observation> OBlist { get; set; }
        public SearchPSSRObservation searchOB = new SearchPSSRObservation();
        public List<RecommStatusModel> RecommStatusList { get; set; }
    }

    public class Priority
    {
        public int PriorityID { get; set; }
        public string PriorityName { get; set; }
    }
    public class RecommStatusModel
    {
        public int RecommStatusID { get; set; }
        public string RecommStatusName { get; set; }
    }


    public class AllPSSR_Observation
    {
        public int SNO { get; set; }
        public int PSSRID { get; set; }
       public int RecommID { set; get; }
        public string RecommText { set; get; }
        public string CategoryName { get; set; }
        public string PriorityName { get; set; }
        public string PlantName { get; set; }
        public string ActionByName { get; set; }
        public string RecommStatusName { get; set; }
        public string CompletedDate { set; get; }
        public string ActionTaken { set; get; }
        public string TargetDate { set; get; }
        public string AttachmentName { get; set; }
        public int RequestIdentity { get; set; }
    }
    public class SearchPSSRObservation
    {
        public int Plant { get; set; }
        public int Category { get; set; }
        public int ActionBy { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int Priority { get; set; }
        public int RecommStatus { get; set; }
    }
}



