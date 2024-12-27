using System;
using System.Collections.Generic;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Xml.Serialization;

namespace MonitPro.Models
{

    public class WorkPermitList : BaseEntity
    {
        public List<WorkPermit> WorkPermit { set; get; }
        public PagedList.IPagedList<WorkPermit> WorkPermits { get; set; }
        //[Required(ErrorMessage = "Please Put Your Comments")]
        public string GetComments { get; set; }
        public List<EquipmentEntity> PlantList { get; set; }
        public int PlantID { get; set; }
        //[Required(ErrorMessage = "Required")]
        public string AuditorRating { get; set; }
        public int EquipmentID { get; set; }
        public int DepartmentID { get; set; }
        public string EquipmentName { get; set; }
        public List<Equipment> EquipmentList { get; set; }
        public List<Department> DepartmentList { get; set; }
        public int ContractorID { get; set; }
        public List<ContractorMaster> ContractList { get; set; }
        public string FromDate { get; set; }
        public string Todate { get; set; }
        public string Attachment { get; set; }

        public int SNO { set; get; }
        public List<Role> RoleList { get; set; }
        public List<StatusCountwps> GetStatusCountwps { get; set; }
        public int WorkPermitID { get; set; }

        public string PermitNo { get; set; }
     


    }
    public class StatusCountwps
    {
        public string Name { get; set; }
        public int TotalCount { get; set; }
    }
    public class EquipmentCount
    {
        public string EquipmentName { get; set; }
        public int TotalCount { get; set; }
    }
    public class PermitInProgress
    {
        public string WorkType { get; set; }
        public int PermitCount { get; set; }
    }
    public class StackCount
    {
        public string PermitMonth { get; set; }
        public int MonthlyCount { get; set; }
        public int approved { get; set; }
        public int extend { get; set; }
        public int closed { get; set; }
        public int cancel { get; set; }

    }
    public class ContracChart
    {
        public string ContractorName { get; set; }
        public int CountOfCon { get; set; }

    }

    public class AssignTypeofWorkForApproverModel : BaseEntity
    {
        public List<WorkPermit> WorkPermit { set; get; }
        public List<UserProfile> ContractorApprover { get; set; }
        public List<User> Users { get; set; }
        public List<Equipment> Equipments { get; set; }
        public int SelectedUserID { get; set; }
        public int GetApproverList { get; set; }
        public int SaveUserEquipments { get; set; }

        public List<WorkTypeMaster> WorkType { get; set; }
       
    }
    public struct Datat

    {
        public Datat(string strValue, int intValue)
        {
            IntegerData = intValue;
            StringData = strValue;
        }

        public int IntegerData { get; private set; }
        public string StringData { get; private set; }
    }

    public class WorkPermit : BaseEntity
    {
        public AssignTypeofWorkForApproverModel TypeofWorkforApprover { get; set; }
        public List<AssignTypeofWorkForApproverModel> assigntypeofworkforapprover { get; set; }
         public List<EquipmentEntity> PlantList { get; set; }
        public string MailApprover { get; set; }
        public string MailSafetyOfficier { get; set; }
        public string MailProcessManager { get; set; }
        public string MailElectrical { get; set; }
        public string Mailmechanical { get; set; }
        public string Mailinstrument { get; set; }
        public string Mailgmoperations { get; set; }
        public string Mailfirewatch { get; set; }
        public string PlantName { get; set; }
        public int SNO { set; get; }
        public int WorkPermitID { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public int PlantID { get; set; }
        [Required(ErrorMessage = "Required")]
        public string ValidityFrom { get; set; }

        [Required(ErrorMessage = "Required")]
        public string ValidityTo { get; set; }

        [Required(ErrorMessage = "Required")]
        public string Location { get; set; }
        [Required(ErrorMessage = "Required")]
        public int ContractorEmpID { get; set; }
        [Required(ErrorMessage = "Department is Required", AllowEmptyStrings = false)]
        public int DepartID { get; set; }
        public List<Department> DepartmentList { get; set; }
        public string DepartmentName { get; set; }
        [Required(ErrorMessage = "Required")]
        public int EquipmentID { get; set; }
        [Required(ErrorMessage = "Required")]
        public string EquipmentName { get; set; }
        public List<Equipment> EquipmentList { get; set; }
        [Required(ErrorMessage = "Required")]
        public int PersonID { get; set; }
        public string PersonName { get; set; }
        public List<FireWatchPerson> FireWatchList { get; set; }

        public string Status { get; set; }
        public string ContractorName { get; set; }
        public string PermitNumber { get; set; }

        [Required(ErrorMessage = "Required")]
        public int ContractorID { get; set; }

        [Required(ErrorMessage = "Required")]
        public string PermitHolderIdName { get; set; }

        [Required(ErrorMessage = "Required")]
        public string WorkDescription { get; set; }

        public List<WorkTypeMaster> WorkType { get; set; }

        public List<ContractorMaster> ContractorList { get; set; }
        public List<UserProfile> ApproversList { get; set; }
        public List<CheckListMaster> ChecklistMaster { get; set; }
        public List<GeneralCheckList> GeneralList { get; set; }
        public int checkvalidapprover { get; set; } //use closepermit valid approver or not

        public string WorkTypeName { get; set; }

     
        public string ApproverComment { get; set; }

        [Required(ErrorMessage = "Required")]
        public int WorkTypeID { get; set; }

        public int ApprovedBy { get; set; }
        public string ApprovedOn { get; set; }

       
        public string ClosureComment { get; set; }
      
        public string ContractorComment { get; set; }        
        public int ClosedBy { get; set; }
        public string ClosedOn { get; set; }

        [Required(ErrorMessage = "Required")]
        public string PermitHolderSignedOn { get; set; }

        [Required(ErrorMessage = "Required")]
        public string AcceptedAreaOwnerSignedOn { get; set; }


        public string ContractorRating { get; set; }

        [Required(ErrorMessage = "Required")]
        public int ClosurePermitHolder { get; set; }

        [Required(ErrorMessage = "Required")]
        public string ClosurePermitHolderName { get; set; }

        public int? ClosureAreaOwnerID { get; set; }
        [Required(ErrorMessage = "Required")]
        public string ClosurePermitHolderSignedOn { get; set; }
        [Required(ErrorMessage = "Required")]
        public string ClosureAreaOwnerSignedOn { get; set; }

        [Required(ErrorMessage = "Required")]
        public int PermitHolder { get; set; }

        [Required(ErrorMessage = "Required")]
        public int AcceptedAreaOwner { get; set; }

        [Required(ErrorMessage = "Required")]
        public int PermitIssuerID { get; set; }

        [Required(ErrorMessage = "Required")]
        public int AdjacentAreaOwenerID { get; set; }

        [Required(ErrorMessage = "Required")]
        public int? ApproverID { get; set; }

        //this is link approvername

        [Required(ErrorMessage = "Required")]
        public int GetApproverID { get; set; }
        public List<Approverlist> GetApprover { get; set; }
        //public IEnumerable<SelectListItems> GetApproverList { get; set; }

        //
        public List<UserProfile> ApproverList { get; set; }

        public int MechanicalIncharge { get; set; }
        public int ElectricalIncharge { get; set; }
        public int InstrumentalIncharge { get; set; }
        public int SafetyOfficer { get; set; }
        public int ProcessManager { get; set; }
        public int GMOperations { get; set; }
        [Required(ErrorMessage = "Required")]
        public string JobID { get; set; }
        public List<UserProfile> Standbyperson { get; set; }
        public List<UserProfile> MechancialDept { get; set; }
        public List<UserProfile> ElectricalDept { get; set; }
        public List<UserProfile> InstrumentationDept { get; set; }
        public List<UserProfile> SafetyDept { get; set; }
        public List<UserProfile> ProcessManagerDept { get; set; }
        public List<UserProfile> GMDept { get; set; }

        public List<UserProfile> ExtensionPermitApproverList { get; set; }

        public List<UserProfile> AdjacentAreaOwnerList { get; set; }

        public List<UserProfile> ExtensionAreaOwnerList { get; set; }
        public List<PermitContractor> ContractorEmp { get; set; }

        public List<UserProfile> UserList { get; set; }

        //public bool FireWatchRequired { get; set; }
        [Required(ErrorMessage = "Required")]
        public int Fire { get; set; }
        [Required(ErrorMessage = "Required")]
        public int Risk { get; set; }

        [Required(ErrorMessage = "Required")]
        public string PermitHolderName { get; set; }

        public string ApproverName { get; set; }
        public string PermitClosureName { get; set; }

        public string PermitIssuerName { get; set; }
        [Required(ErrorMessage = "Required")]
        public string NoOfPersonAtSite { get; set; }

        [Required(ErrorMessage = "Please Choose File")]
        public HttpPostedFileBase Acknowledgement32 { set; get; }

        public string Attachment { get; set; }
        public List<ExtensionDetailsList> ExtensionDetails { get; set; }

        public string WholeAttachment { get; set; }


        public string PPEOthers { get; set; }

        [Required(ErrorMessage = "Required")]
        public List<PersonalPerspectiveEquipment> PPE { get; set; }
        [Required(ErrorMessage = "Required")]
        public List<TypeofWorkforCreatepermit> TypeofWorkforCreatepermit { get; set; }


        public string ExtensionIssuerName { get; set; }
        public string ExtensionFrom { get; set; }
        public string ExtensionTo { get; set; }
        public string ExtensionPermitHolder { get; set; }
        public int? ExtensionPermitIssuerID { get; set; }
        public int? ExtensionAreaOwnerID { get; set; }
        public int? ExtensionApproverID { get; set; }
        public int? ExtensionPermitID { get; set; }
        public string ExtensionApprover { get; set; }


        [Required(ErrorMessage = "Required")]
        //PermitReviewerPage
        //[Required(ErrorMessage = "Required")]
        public string ActionComments { get; set; }
        public int GetIsRecord { get; set; }
        public string GetStatus { get; set; }

        //[Required(ErrorMessage = "Please Put Your Comments")]
        public string GetComments { get; set; }

        //[Required(ErrorMessage = "Required")]
        public string AuditorRating { get; set; }

        public string Description { get; set; }
        public string DueDate { get; set; }
        public string FrequencyName { get; set; }
        public int FrequencyID { get; set; }
        public int AttachId { get; set; }

        public string FromDate { get; set; }
        public string Todate { get; set; }
        //public List<HistoryReview> historyreview { get; set; }


        //model code for HistoryReview

        public List<User> User { get; set; }
        //public string FromDate { get; set; }
        public string EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageCount { get; set; }
        public string ShiftInchName { get; set; }
        public string MechInchRemarks { get; set; }
        public string MechInchName { get; set; }
        public string MechInchDatetime { get; set; }
        public string ElecInchRemarks { get; set; }
        public string ElecInchName { get; set; }
        public string ElecInchDatetime { get; set; }
        public string InstruInchRemarks { get; set; }
        public string InstruInchName { get; set; }
        public string InstruInchDatetime { get; set; }
        public string SafetyOffRemarks { get; set; }
        public string SafetyOffName { get; set; }
        public string SafetyOffDatetime { get; set; }

        public string ProMgrRemarks { get; set; }
        public string ProMgrName { get; set; }
        public string ProMgrDatetime { get; set; }
        public string GMOpRemarks { get; set; }
        public string GMOpName { get; set; }
        public string GMOpDatetime { get; set; }
        public int Identity { get; set; }

    }
    [XmlRoot("WorkTypeSave")]
    public class WorkTypeXML
    {
        public string WorkTypeName { get; set; }

        public int WorkTypeID { get; set; }
    }
    [XmlRoot("AllCheckList")]
    public class CheckListWorkXML
    {
        public string CheckListName { get; set; }

        public int CheckID { get; set; }

    }
    [XmlRoot("PPESave")]
    public class PPEXML
    {
        public string Name { get; set; }

        public int PPEID { get; set; }
    }
    [XmlRoot("GeneralSave")]
    public class GenXML
    {
        public string GeneralName { get; set; }

        public int GeneralID { get; set; }
    }
    [XmlRoot("PermitApprover")]

    public class PermitApproverXML
    {
        public int UserID { get; set; }

        public int WorkTypeID { get; set; }

        public int CreatedBy { get; set; }
    }
    [XmlRoot("OccupationalHealthSafetySave")]

    public class OccupationalHealthSafetyXML
    {
        public int CheckListID { get; set; }
        public string CheckedValue { get; set; }
        public string Remarks { get; set; }
    }
    public class FireWatchPerson
    {
        public int PersonID { get; set; }
        public string PersonName { get; set; }

    }
    public class PermitContractor
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }
    public class SearchContractorEmployee
    {
        public int ContractID { get; set; }
        public string Name { get; set; }
        public int SkillsID { get; set; }
        public int DepartID { get; set; }
        public string TrainingFromDate { get; set; }
        public string TrainingToDate { get; set; }
        public int ConCompanyName { get; set; }
        public int SkillsName { get; set; }
        public int DeptName { get; set; }
        public string NextTrainingDate { get; set; }
        public int SNo { get; set; }
        public int UserID { get; set; }
        public String FirstName { get; set; }
        public string LastName { get; set; }
        public string EmployeeID { get; set; }
        public string MobileNumber { get; set; }
        public string IsActiveSelect { get; set; }
        public string UserImage { get; set; }
        public string DisplayUserName { get; set; }
        public string TrainingTypeName { get; set; }
        public string FrequencyName { get; set; }
        public string Remarks { get; set; }
        public string IsInvestigateSelect { get; set; }
        public List<ContractorMaster> ContractorList { get; set; }
        public List<Department> DepartmentList { get; set; }
        public List<ContractorTrainingType> trainingtype { get; set; }
        public List<ContractorSkills> contractorskills { get; set; }
    }
    public class SearchContractorList
    {
       public int ValsparManager { get; set; }
        public int ContactID { get; set; }
    }
    public class CheckListMaster
    {
        public int CheckListID { get; set; }
        public string CheckListName { get; set; }
        public bool ISChecked { get; set; }

    }
    public class GeneralCheckList
    {
        public int WorkPermitID { get; set; }
        public int GenID { get; set; }
        public bool Gencheck { get; set; }
        public string GenName { get; set; }
    }
    public class ExtensionDetailsList : BaseEntity
    {

        public string WorkPermitID { get; set; }
        public string ExtensionFrom { get; set; }
        public string ExtensionTo { get; set; }
        public string ExtensionPermitIssuer { get; set; }
        public string WorkTypeName { get; set; }
        public string ExtensionApprover { get; set; }
        public string ExtensionAreaOwner { get; set; }

    }
    public class SelectListItems
    {
        //public SelectListItem();

        public bool Selected { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
    }
    //public class BaseEntity
    //{
    //    public int UserID { get; set; }
    //    public string UserFullName { get; set; }
    //    public List<Role> Roles { get; set; }
    //    public string ProfileImage { get; set; }
    //}
    public class HistoryReviewes : BaseEntity
    {

        public PagedList.IPagedList<Review> Reviews { get; set; }
        public List<User> User { get; set; }
        public string FromDate { get; set; }
        public string EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageCount { get; set; }
    }
    public class Review
    {
        public int ID { get; set; }
        public string Frequency { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
        public string CompletedBy { get; set; }
        public string ActionTakenDateTime { get; set; }
        public string Attachment { get; set; }
        public string AuditorRating { get; set; }


    }

    public class Approverlist : BaseEntity
    {
        //public AssignEquipments();
        public List<UserProfile> GetApproverList { get; set; }
        public List<UserProfile> GetContractorList { get; set; }
        public List<CheckListMaster> GetCheckMaster { get; set; }
        public List<User> Users { get; set; }
        public int hotwork { get; set; }
        public int csentry { get; set; }
        public int nonelec { get; set; }
        public int elec { get; set; }
        public int unloadingchemical { get; set; }
        public int mecwork { get; set; }
        public int linebreaking { get; set; }
        public int lototagout { get; set; }
        public int hvac { get; set; }
        public int execwork { get; set; }
        public int WHRF { get; set; }
        public int civil { get; set; }
        public int painting { get; set; }
        public int workingfirehydrant { get; set; }
        public int plaming { get; set; }
        public int shiftingmeterial { get; set; }
        public int others { get; set; }
        //public int GetEquipmentList { get; set; }
        //public int SaveUserEquipments { get; set; }
    }


    public class TypeofWorkforCreatepermit
    {
        public int WorkPermitID { get; set; }
        public int TypeofWorkID { get; set; }
        public bool Ischecked { get; set; }
        public string TypeWorkName { get; set; }

    }

    public class PersonalPerspectiveEquipment
    {
        public int WorkPermitID { get; set; }
        public int PPEID { get; set; }
        public bool PPEcheck { get; set; }
        public string PPEName { get; set; }
    }

    public class WorkTypeMaster
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public int WorkPermitID { get; set; }
        public int WorkTypeID { get; set; }
        public bool Ischecked { get; set; }

        public string WorkTypeName { get; set; }
        public List<NewWorkType> NewWorkTypes { get; set; }
    }
    public class NewWorkType
    {

        public int WorkTypeID { get; set; }
        public bool Ischecked { get; set; }
    }

    public class ContractorMaster
    {
        public int ContractorID { get; set; }
        public string ContractorName { get; set; }
        public string PermitHolderIdName { get; set; }
        public string EmailAddress { get; set; }
    }
    public class ApproverMaster
    {
        public int UserID { get; set; }
        public string DisplayUserName { get; set; }

    }

    public class UserList
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
    }

    public class HazardInformation
    {
        public int HazardInformationID { get; set; }
        public bool WorkPermitID { get; set; }
        public bool ChemicalExposures { get; set; }
        public bool FlammableVaporsGases { get; set; }
        public bool SolventsCleaners { get; set; }
        public bool Noise { get; set; }
        public bool SlipsTripsFalls { get; set; }
        public bool Engulfment { get; set; }
        public bool Electrical { get; set; }
        public bool Mechanical { get; set; }
        public bool Pnuematic { get; set; }
        public bool Hydraulic { get; set; }
        public bool HotWork { get; set; }
        public bool CompressedGases { get; set; }
        public string Others { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedOn { get; set; }
    }

    public class HazardControlMethod
    {
        public bool HazardControlMethodID { get; set; }
        public bool WorkPermitID { get; set; }
        public bool PreEntryRescueBriefing { get; set; }
        public bool CommunicationVerbal { get; set; }
        public bool CommunicationRadio { get; set; }
        public bool LockoutProceduresInitiated { get; set; }
        public bool LineBreakingProceduresInitiated { get; set; }
        public bool HotWorkProceduresInitiated { get; set; }
        public bool ExternalBarricades { get; set; }
        public bool VentilationAirChanges { get; set; }
        public bool GroundFaultCircuitInterrupter { get; set; }
        public bool NonSparkingTools { get; set; }
        public bool ExplosionProofLighting { get; set; }
        public bool MSDSChemicalInfoAvailable { get; set; }
        public bool FlushPurgeDrainClean { get; set; }
        public bool IsolatePiping { get; set; }
        public bool IsolatePipingDoubleBlock { get; set; }
        public bool IsolatePipingBlankBlindPipe { get; set; }
        public bool IsolatePipingDisconnect { get; set; }
        public bool FireExtinguisher { get; set; }
        public string Others { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedOn { get; set; }
    }
    //public class Equipment
    //{
    //    public int EquipmentID { get; set; }
    //    public string EquipmentName { get; set; }

    //}

    public class ContractList : BaseEntity
    {
        public string FromDate { get; set; }
        public string Todate { get; set; }
        public List<Contract> Contract = new List<Contract>();
        public List<ContractAnnualRating> RatingList;
        public SearchContractorList searchCon = new SearchContractorList();
        public List<UserProfile> ValsparContactList { get; set; }
        public List<UserProfile> ContractApprover { get; set; }
        public int ContactID { get; set; }
        public int ValsparManager { get; set; }
    }
    public class Contract : BaseEntity
    {
        public int ContractID { get; set; }
        [Required(ErrorMessage = "Required")]
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "Required")]
        public int ContactID { get; set; }
        public string ContactPerson { get; set; }
        public List<UserProfile> ValsparContactList { get; set; }
        [Required(ErrorMessage = "Required")]
        public string SupervisorFirstName { get; set; }
        [Required(ErrorMessage = "Required")]
        public string SupervisorLastName { get; set; }
        public string FrequencyName { get; set; }
        public int FrequencyID { get; set; }
        [Required(ErrorMessage = "Required")]
        public List<FrequencyOfEvaluation> FrequencyOfEvaluation { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Country { get; set; }
        [Required(ErrorMessage = "Required")]
        public string State { get; set; }
        [Required(ErrorMessage = "Required")]
        public string City { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Street { get; set; }
        [Required(ErrorMessage = "Required")]
        public string AssessmentDate { get; set; }

        [RegularExpression(@"^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,3})$", ErrorMessage = "Invalid e-mail address")]
        [Required(ErrorMessage = "Email Address is Required", AllowEmptyStrings = false)]
        public string EmailAddress { get; set; }
        [Required(ErrorMessage = "Required")]
        public string PostCode { get; set; }
        [Required(ErrorMessage = "Required")]
        public string MobileNo { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Status { get; set; }
        public bool Isactiveselect { get; set; }
        public bool Have { get; set; }
        public string NextAssessmentDate { get; set; }
        public string LastAssessmentDate { get; set; }
        public HttpPostedFileBase Acknowledgement { get; set; }
        [Required(ErrorMessage = "Required")]
        public List<NatureOfBusinessMaster> NatureOfBusiness { get; set; }
        public int NOBusinessTypeID { get; set; }
        public bool SubcontractorInvolved { get; set; }
        public string Attachment { get; set; }
        public List<OccupationalHealthSafetyCheckList> OccupationalHealthSafety { get; set; }
        public ContractAnnualRating ContractAnnualRating { get; set; }
        [Required(ErrorMessage = "Required")]

        public List<WorkTypeMaster> WorkType { get; set; }

        public string ApproverComments { get; set; }
        public string ReceiverName { get; set; }
        public string ContractorCreatedBy { get; set; }
        public int updatedby { get; set; }
        public int ValsparManager { get; set; }
        public string TransporterName { get; set; }

        public string ContractStatus { get; set; }
        public List<UserProfile> ContractApprover { get; set; }
        public List<UserProfile> ContractCreated { get; set; }
      
    }
    public class FrequencyOfEvaluation
    {
        public int FrequencyID { get; set; }
        public string FrequencyName { get; set; }
    }

    public class NatureOfBusinessMaster
    {
        public int NOBusinessTypeID { get; set; }
        public string NOBusinessType { get; set; }

    }

    public class OccupationalHealthSafetyCheckList
    {
        public int ContractID { get; set; }
        public int HealthSafetyPolicy { get; set; }
        public string CheckListName { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Ischecked { get; set; }
        public string Remarks { get; set; }
    }




    public class ContractAnnualRating
    {
        public string ContractorName { get; set; }
        public string AttachmentName { get; set; }
        public string RatingStatus { get; set; }
        public int TotalPermits { get; set; }
        public int GreenPermits { get; set; }
        public int orangePermits { get; set; }
        public int RedPermits { get; set; }
        public int TotalPercentage { get; set; }
        public int Rank { get; set; }
        public string AssesmentFrequency { get; set; }
        public string LastAssessmentDate { get; set; }
        public string NextAssessmentDate { get; set; }
    }
    public class ComplianceList : BaseEntity
    {
        public int VechicleId { get; set; }
        public string VechicleName { get; set; }
        public int ReceiverId { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverAddress { get; set; }
        public string VehicleRegistrationNo { get; set; }
        public string PhysicalName { get; set; }
        public string SenderApproverName { get; set; }
        public List<WasteList> Waste { get; set; }
        public int UserId { get; set; }


    }

    public class WasteList : BaseEntity
    {
        public int WasteId { get; set; }
        public int WasteNo { get; set; }
        public string WasteTagId { get; set; }
        public string WasteName { get; set; }
        public string UoM { get; set; }
        public string Quantity { get; set; }


    }
    [XmlRoot("WasteMeasure")]
    public class WasteQuantity : BaseEntity
    {
        public int WasteId { get; set; }

        public string WasteName { get; set; }

        public string Quantity { get; set; }
    }


    public class CreateComplianceViewModel : BaseEntity
    {
        public List<Role> RoleList { get; set; }
        public List<UserProfile> UserList { get; set; }

        public List<User> User { get; set; }

        [Required(ErrorMessage = "Required")]
        public int ContractID { get; set; }

        [Required(ErrorMessage = "Required")]
        public int VechicleID { get; set; }

        public string VechicleType { get; set; }

        [Required(ErrorMessage = "Required")]
        public string VechicleRegistrationNumber { get; set; }

        public string Quantity { get; set; }

        [Required(ErrorMessage = "Required")]
        public int PhysicalFormId { get; set; }
        public string Type { get; set; }
        public List<Contract> Contract;
        public List<WasteList> Waste { get; set; }
        public List<UserProfile> ApproverList { get; set; }
        public List<Vechicle> Vechicle { get; set; }
        public List<PhysicalForm> Form { get; set; }
        public string RoleID { get; set; }
        public string RoleName { get; set; }

        public int SNO { set; get; }
        public int ComplianceFormID { get; set; }
        public List<WasteQuantity> wastequantity { get; set; }
        public string CreatedOn { get; set; }

        [Required(ErrorMessage = "Required")]
        public string ReceiverName { get; set; }

        public string WasteName { get; set; }
        public string SenderName { get; set; }
        public string Attachment { get; set; }
        public string ComplianceDate { get; set; }
        [Required(ErrorMessage = "Required")]
        public string TransporterName { get; set; }

        public string Status { get; set; }
        public string CompanyName { get; set; }

    }

    public class SavedCompliance : BaseEntity
    {


        public List<Role> RoleList { get; set; }
        public List<UserProfile> UserList { get; set; }

        public List<User> User { get; set; }
        public int ComplianceFormID { get; set; }
        public int SNO { set; get; }
        public int ContractID { get; set; }

        public int VechicleID { get; set; }
        public string VechicleName { get; set; }
        public string VechicleRegistrationNumber { get; set; }
        public int Quantity { get; set; }
        public int PhysicalFormId { get; set; }
        public string Type { get; set; }
        public List<Contract> Contract;
        public List<WasteList> Waste { get; set; }
        public List<UserProfile> ApproverList { get; set; }
        public List<Vechicle> Vechicle { get; set; }
        public List<PhysicalForm> Form { get; set; }
        public string RoleID { get; set; }
        public string RoleName { get; set; }
        public string WasteName { get; set; }
        public string CreatedOn { get; set; }
        public string ReceiverName { get; set; }
        public int TotalQuatity { get; set; }
        public string SenderName { get; set; }
        public List<CreateComplianceViewModel> createcompliance { get; set; }
        public PagedList.IPagedList<CreateComplianceViewModel> HistoryPages { get; set; }
        public string Attachment { get; set; }

        public string Status { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }


        public List<DataRow> Datarow { get; set; }
        public List<DataColumn> Datacolumn { get; set; }

    }



    public class Vechicle : BaseEntity
    {
        public int VechicleId { set; get; }
        public string VechicleType { set; get; }
    }

    public class PhysicalForm : BaseEntity
    {
        public int PhysicalFormId { set; get; }
        public string Type { set; get; }
    }

    public class NewContractor : BaseEntity
    {
        [Required(ErrorMessage = "First Name is Required", AllowEmptyStrings = false)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is Required", AllowEmptyStrings = false)]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Mobile Number is Required", AllowEmptyStrings = false)]
        public string MobileNumber { get; set; }
        [Required(ErrorMessage = "Emergency Contact Number is Required", AllowEmptyStrings = false)]
        public string EmergencyContactNumber { get; set; }

        [Required(ErrorMessage = "Contractor Company is Required")]
        public int ContractID { get; set; }
        [Required]
        public int TrainingTypeID { get; set; }
        [Required]
        public int SkillsID { get; set; }

        public List<Role> RoleList { get; set; }
        [Required(ErrorMessage = "Department is Required", AllowEmptyStrings = false)]
        public int DepartID { get; set; }

        public string EmployeeID { get; set; }
        public List<Department> DepartmentList { get; set; }
        public List<ContractorMaster> ContractorList { get; set; }
        [Required(ErrorMessage = "Remarks is Required")]
        public string Remarks { get; set; }
        [Required(ErrorMessage = "Required")]
        public string TraningDate { get; set; }
        [Required(ErrorMessage = "Frequency is Required")]
        public int FrequencyID { get; set; }

        [Required(ErrorMessage = "Date OF Birth is Required")]
        public string DateOFBirth { get; set; }


        public int Age { get; set; }
        [Required(ErrorMessage = "Date Of Joining is Required")]
        public string DateOfJoining { get; set; }

        [Required(ErrorMessage = "Identity Detail is Required")]
        public string IDDetail { get; set; }

        public bool IsActive { get; set; }


        public string PFNumber { get; set; }
        [Required(ErrorMessage = "Address is Required")]
        public string Address { get; set; }


        public List<FrequencyOfEvaluation> FrequencyOfEvaluation { get; set; }
        public List<ContractorTrainingType> trainingtype { get; set; }
        public List<ContractorSkills> contractorskills { get; set; }

    }


    public class ContractorTrainingType
    {
        public int TypeID { get; set; }
        public string TypeName { get; set; }
    }
    public class ContractorSkills
    {
        public int SkillsID { get; set; }
        public string SkillName { get; set; }
    }

    public class EmpProfile : BaseEntity
    {
        public List<EmpContractorprofile> empUserProfile { get; set; }
        public string Search { get; set; }

        public SearchContractorEmployee searchContractor = new SearchContractorEmployee();

         public List<ContractorMaster> ContractorList { get; set; }
        public List<Department> DepartmentList { get; set; }
        public List<ContractorTrainingType> trainingtype { get; set; }
        public List<ContractorSkills> contractorskills { get; set; }
        public int SNo { get; set; }
    }

    public class EmpContractorprofile : BaseEntity
    {
        [Required(ErrorMessage = "First Name is Required", AllowEmptyStrings = false)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is Required", AllowEmptyStrings = false)]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Mobile Number is Required", AllowEmptyStrings = false)]
        public string MobileNumber { get; set; }
        [Required(ErrorMessage = "Emergency Contact Number is Required", AllowEmptyStrings = false)]
        public string EmergencyContactNumber { get; set; }

        public string PFNumber { get; set; }
        [Required(ErrorMessage = "Address is Required")]
        public string Address { get; set; }
        public string ProfilePictureName { get; set; }
        [Required(ErrorMessage = "Contractor Company is Required")]
        public int ContractID { get; set; }

        public string ConCompanyName { get; set; }

        [Required]
        public int TrainingTypeID { get; set; }

        public string TrainingTypeName { get; set; }
        [Required]
        public int SkillsID { get; set; }

        [ValidateFile(ErrorMessage = "Please select a PNG image smaller than 1MB")]
        public HttpPostedFileBase ContractorProfile { get; set; }
        public string SkillsName { get; set; }
        public bool IsActive { get; set; }
        public bool IsInvesatigate { get; set; }
        public string IsActiveSelect { get; set; }
        public string UserImage { get; set; }

        public string DisplayUserName { get; set; }

        public string IsInvestigateSelect { get; set; }

        public string DeptName { get; set; }

        [Required(ErrorMessage = "Department is Required", AllowEmptyStrings = false)]
        public int DepartID { get; set; }
        [Required(ErrorMessage = "Employee ID is Required", AllowEmptyStrings = false)]
        public string EmployeeID { get; set; }
        public List<Department> DepartmentList { get; set; }
        public List<ContractorMaster> ContractorList { get; set; }
        [Required(ErrorMessage = "Remarks is Required")]
        public string Remarks { get; set; }
        [Required(ErrorMessage = "Required")]
        public string TraningDate { get; set; }

        public string NextTrainingDate { get; set; }
        [Required(ErrorMessage = "Frequency is Required")]
        public int FrequencyID { get; set; }
        public string FrequencyName { get; set; }
        public int ModifiedBy { get; set; }
        [Required(ErrorMessage = "Date Of Birth is Required")]
        public string DateOFBirth { get; set; }


        public int Age { get; set; }
        [Required(ErrorMessage = "Date Of Joining is Required")]
        public string DateOfJoining { get; set; }

        [Required(ErrorMessage = "Identity Detail is Required")]
        public string IDDetail { get; set; }



        public List<FrequencyOfEvaluation> FrequencyOfEvaluation { get; set; }
        public List<ContractorTrainingType> trainingtype { get; set; }
        public List<ContractorSkills> contractorskills { get; set; }

    }

    public class Evaluation : BaseEntity
    {
        public int ContractorID { get; set; }
        public string ContractorName { get; set; }
        public string EvaluationPeriod { get; set; }
        public string FrequencyofEvaluation { get; set; }
        public string EvaluatedBy { get; set; }
        public string SWContact { get; set; }
        public string EvaluationDate { get; set; }
        public List<EvaluationCriteriaCheckList> EvaluationCriteriaCheckList { get; set; }
        public List<OverallRating> Ratinglist { get; set; }
        public int RatingID { set; get; }
        public HttpPostedFileBase Attachment { get; set; }
        public string AttachmentName { get; set; }

    }

    [XmlRoot("EvaluationCriteriaSave")]

    public class EvaluationCriteriaXML
    {
        public int CheckListID { get; set; }
        public string CheckedValue { get; set; }
        public string Remarks { get; set; }
    }


    public class EvaluationCriteriaCheckList
    {
        public int EvaluationID { get; set; }
        public int EvaluationQuestionsID { get; set; }
        public string EvaluationQuestions { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Ischecked { get; set; }
        public string Remarks { get; set; }
    }

    public class OverallRating
    {
        public int RatingID { get; set; }
        public string RatingName { get; set; }
        public string Description { get; set; }
    }




}
