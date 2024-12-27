using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Serialization;
namespace MonitPro.Models
{

    public class ExportData : BaseEntity
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public List<Equipment> Equipment { get; set; }
        public string EquipmentName { get; set; }
        public int EquipmentID { get; set; }
        public List<DataRow> Datarow { get; set; }
        public List<DataColumn> Datacolumn { get; set; }
    }

    public class DailyReportsmodel : BaseEntity
    {
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public List<DataRow> Datarow { get; set; }
        public List<DataColumn> Datacolumn { get; set; }

    }

   

    public class MeasureDataViewModel :BaseEntity
    {
        public List<EquipmentEntity> Equipments { get; set; }
        public List<PlanEntity> Plans { get; set; }
        public int EquipmentID { get; set; }
       
    }

    public class PlanEntity : BaseEntity
    {
        public int SNO { set; get; }
        public long PlanID { get; set; }
        public int EquipmentID { get; set; }
        public string TagID { get; set; }
        public string Parameter { get; set; }
        public string UOM { get; set; }
        public string LTV { get; set; }
        public string HTV { get; set; }
        public string CV { get; set; }
        public bool IsNumeric { get; set; }
        public string ReasonForMonitoring { get; set; }
        public string ParameterType { get; set; }
        public string Formula { get; set; }
       
    }

    [XmlRoot("PlanData")]
    public class PlanDataEntity : BaseEntity
    {
      public long PlanID{get;set;}
      public string MeasuredValue{get;set;}
      public int MeasuredBy{get;set;}
      public string TagID { get; set; }
    }


    public class WorkPlannerViewModel : BaseEntity
    {
        public int SNO { set; get; }
        public long PlanDataID { get; set; }
        public long PlanID { get; set; }
        public string MeasuredDateTime { get; set; }
        public string Frequency { get; set; }
        public char Status { set; get; }
        public string StatusIcon { get; set; }
        public string TagID { get; set; }
        public string EquipmentName { get; set; }
        public string Parameter { get; set; }
        public string LTV { get; set; }
        public string HTV { get; set; }
        public string CV { get; set; }
        public string UOM { get; set; }
        public string LHTpercentage { get; set; }
        public int Responsibility { get; set; }
        public string ActionNotes { set; get; }
        public int AssignTo { set; get; }
        public string AnalysisDocument { get; set; }
        public char CompletionStatus { set; get; }
        public string AssignedBy { get; set; }
        public int AssignedUserID { get; set; }
        public string UserImage { get; set; }
    }
    
    public class WorkPlanner:BaseEntity
    {
       public List<WorkPlannerViewModel> WorkPlans { get; set; }
       public List<SelectListItem> Users { get; set; }
    }

    public class Approval : BaseEntity
    {
        public List<ApprovalViewModel> approvalViewModel { get; set; }
        //public string FromDate { get; set; }
        //public string ToDate { get; set; }
        //public List<PlanEntity> Plans { get; set; }
        public List<Equipment> Equipment { get; set; }
        public int EquipmentID { get; set; }
        public Int64 PlanID { get; set; }

    }
    public class ApprovalViewModel
    {
        public int SNO { set; get; }
        public int ID { get; set; }
        public int EquipmentID { get; set; }
        public string EquipmentName { get; set; }
        public string TagID { get; set; }
        public string ChangedFunction { get; set; }
        public string oldvalue { get; set; }
        public string newvalue { get; set; }
        public string Action { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public char Status { get; set; }
        public int ApprovedOrRejectedBy { get; set; }
        public DateTime ApprovedOrRejectedOn { get; set; }
    }

    public class AuditHistory:BaseEntity
    {
        public List<Changedfunction> changedFunction { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public List<PlanEntity> Plans { get; set; }
        public List<Equipment> Equipment { get; set; }
        public int EquipmentID { get; set; }

    }
    public class Changedfunction
    {
        public int PlanID { get; set; }
        public int EquipmentID { get; set; }
        public string EquipmentName { get; set; }
        public string TagID { get; set; }
        public string ChangedFunction { get; set; }
        public string oldvalue { get; set; }
        public string newvalue { get; set; }
        public string Action { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }


    }
    public class EquipmentStatusList:BaseEntity
    {
        public List<EquipmentStatus> Elist { get; set; }
    }

    public class EquipmentStatus
    {
        public string EquipmentName { get; set; }
        public string FromDate { get; set; }
        public string Status { get; set; }
        public string Interval { get; set; }
        public string OfflineDays { get; set; }
        public string OnlineDays { get; set; }
        public string DueDate { get; set; }
        public float DueHours { get; set; }
        public string SetPoint { get; set; }
        public string Comments { get; set; }
        public string ID { get; set; }
    }

    public class User
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
    }


    public class DashboardViewModel:BaseEntity
    {
         
        public TrendViewModel Trend { get; set; }
        public WorkPlannerModel WorkPlanner { get; set; }
    }





    public class WorkPlannerData 
    {
        public int serialNumber { set; get; }
        public string riscFactor { set; get; }
        public string TagID { get; set; }
        public string Equip_Name { get; set; }
        public string Parameter_Name { get; set; }
        public string LTV { get; set; }
        public string HTV { get; set; }
        public string MV { get; set; }
        public string UOM { get; set; }
        public string Comments { set; get; }
        public string Assigned { set; get; }
        public int MeasureID { set; get; }
        public int TempID { set; get; }
        public int EquipID { set; get; }
        public String Address { set; get; }
     //   List<UserDetails> UserDataInfo { set; get; }
        public List<SelectListItem> UserData { set; get; }
        public String SelectedUser { set; get; }
        public String status { set;get;}
    }
    public class UserDetails
    {
        public int userID { set; get; }
        public string designation { set; get; }
        public string firstName { set; get; }
        public string lastName { set; get; }
    }
    public class WorkPlannerModel:BaseEntity
    {

        public List<String> SelectedUser { set; get; }
        public List<WorkPlannerData> WorkData { set; get; }
        public List<UserDetails> UserDataInfo { set; get; }
      
    }


#region WorkPlannerHistory
    public class WorkPlanHistory :BaseEntity
    {
        public int EquipmentID { get; set; }
        public string EquipmentName { get; set; }
        public int WorkPlanID { get; set; }
        public PagedList.IPagedList<Workplanner> Workplan { get; set; }
        public List<Equipment> Equipment { get; set; }
        public List<User> User { get; set; }
        public string FromDate { get; set; }
        public string EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageCount { get; set; }
    }
    public class Workplanner
    {
        public int ID { get; set; }
        public string Frequency { get; set; }
        public string Status { get; set; }
        public string TagID { get; set; }
        public string Equipment { get; set; }
        public string Parameter { get; set; }
        public string LTV { get; set; }
        public string HTV { get; set; }
        public string CV { get; set; }
        public string UOM { get; set; }
        public string Comments { get; set; }
        public string CompletedBy { get; set; }
        public string ActionTakenDateTime { get; set; }
        public string Attachment { get; set; }
        
        public int Equipments { get; set; }
    }
    public class Equipment
    {
        public int EquipmentID { get; set; }
        public string EquipmentName { get; set; }
        public bool Assigned { get; set; }
        public string DivisionID { get; set; }
    }
     
#endregion

}


