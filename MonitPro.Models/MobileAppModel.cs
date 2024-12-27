using MonitPro.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

namespace MonitPro.MobileApp.Model
{
    public class LoginCredential
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class Plants
    {
        public int ID { get; set; }
        public string PlantName { get; set; }
        public string Description { get; set; }
    }
    public class CAPAPlants
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class WorkPermit
    {
        public string PermitNumber { get; set; }
        public string ValidityTo { get; set; }
        public string ApproverName { get; set; }
        public string WorkTypeName { get; set; }
        public string EquipmentName { get; set; }
        public string PlantName { get; set; }
        public int PermitID { get; set; }
        public int ApproverID { get; set; }
        public int SafetyOfficer { get; set; }
        public int ProcessManager { get; set; }
        public int Electrical { get; set; }
        public int Mechanical { get; set; }
        public int Instrument { get; set; }
        public int Gmoperation { get; set; }
        public string ApproverComments { get; set; }
        public string safetyRemarks { get; set; }
        public string proMgrRemarks { get; set; }
        public string ElecRemarks { get; set; }
        public string MechRemarks { get; set; }
        public string InstRemarks { get; set; }
        public string GmRemarks { get; set; }
        public string PermitIssuer { get; set; }
        public string Workdoneby { get; set; }
        public string PermitDescription { get; set; }

    }
    public class UpdateWorkPermitRating
    {
        public string PermitID { get; set; }
        public string ContractorComment { get; set; }
        public string ContractorRating { get; set; }
    }


    public class PermitUpdate
    {
        public string PermitID { get; set; }
        public int ApprovedBy { get; set; }
        public string ApproverComments { get; set; }
        public string Status { get; set; }
        public int Identity { get; set; }
    }
    public class IncidentClassfication
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class AuditType
    {
        public int AuditID { get; set; }
        public string AuditName { get; set; }
        public string AuditDescription { get; set; }
    }
    public class CAPASource
    {

        public string AuditCSID { get; set; }
        public string Name { get; set; }

    }
    public class ContractorEmp
    {
        public int EMPID { get; set; }
        public string EMPName { get; set; }
    }
    public class Incident
    {
        public int IncidentID { get; set; }
        public string IncidentNO { get; set; }


    }
    public class NewIncidentViewModel
    {

        public int CurrentUserID { get; set; }
        //[Required]
        //public HttpPostedFileBase ImageFile { get; set; }
        public int IncidentID { get; set; }

        public string ECNumber { get; set; }

        public string Description { get; set; }
        public string IncidentTime { get; set; }

        public string injuredOrNot { get; set; }
        public string injuredDecription { get; set; }
        // public int StatusID { get; set; }
        public int PlantID { get; set; }
        public int IncidentClassficationID { get; set; }
        public string ImmediateAction { get; set; }
        public string LossOfMaterial { get; set; }
        public string LossQuantity { get; set; }
        public string DamageEquipment { get; set; }
        public string DamageDetails { get; set; }
        public string PersonAvailable { get; set; }
        public string ImageFile { get; set; }
        public string FileName { get; set; }
        public string ReportedDate { get; set; }
    }
    public class EquipmentSchedulerList
    {
        public int SCID { get; set; }
        public string Frequency { get; set; }
        public string NextInspectionDate { get; set; }
        public string InspectEngineer { get; set; }
        public string Plant { get; set; }
        public string EquipmentName { get; set; }
    }
    public class IncidentViewModel
    {
        public string injuredOrNot;
        public string injuredDecription;
        public int IncidentID { get; set; }
        public string ECNumber { get; set; }
        public string Description { get; set; }
        public string IncidentTime { get; set; }
        public int CreatedBy { get; set; }
        public string FileName { get; set; }
        public string ReportedDate { get; set; }
        public string ReportedBy { get; set; }
        public int PlanID { get; set; }
        public string LossOfMaterial { get; set; }
        public string LossQuantity { get; set; }
        public string DamageEquipment { get; set; }
        public string DamageDetails { get; set; }
        public string PersonAvailable { get; set; }
        public string ImmediateAction { get; set; }
        public string ProbableCauses { get; set; }
        public int RootCauseID { get; set; }
        public int IncidentClassificationID { get; set; }
    }
    public class CAPAinsert
    {
        public int CAPAID { get; set; }
        public int plantID { get; set; }
        public int AuditType { get; set; }
        public int CAPASource { get; set; }
        public string ReportDate { get; set; }
        public int ReportBy { get; set; }
        public int CreatedBy { get; set; }
        public int StatusID { get; set; }

    }
    public class CAPAObservation
    {
        public string CAPANo { get; set; }
        public string PlantName { get; set; }
        public string CAPASourceName { get; set; }
        public string CreatedName { get; set; }
    }
    public class Attachment
    {
        public int SNo { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

    }
    public class LoginResponse
    {
        public string Token { get; set; }
        public string ErrorMessage { get; set; }
        public string IncidentID { get; set; }
        public int CAPAID { get; set; }
        public int UserID { get; set; }
        public string CAPANO { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PermitID { get; set; }
        public int RecomID { get; set; }
        public string ChecklistIdentity { get; set; }
    }
    public class ChecklistModel
    {
        public string PlantName { get; set; }
        public string Division { get; set; }
        public string EquipmentName { get; set; }
        public string ChecklistName { get; set; }

    }
    [XmlRoot("ChecklistDetailsSave")]
    public class ChecklistDataXML
    {
        public int CMID { get; set; }
        public int CID { get; set; }
        public string ChecklistDescription { get; set; }
        public string Target { get; set; }
        public string Value { get; set; }




    }

    public class WorkPermitAttach
    {
        public string PermitID { get; set; }
        public string FileName { get; set; }
        public string PermitImageFile { get; set; }
    }

    public class SAVECAPAObservation
    {
        public string Observation { get; set; }
        public string Recommendation { get; set; }
        public int CAPAID { get; set; }
        public int OBID { get; set; }
        public string FileName { get; set; }
        public int OBPlantID { get; set; }
        public int UserID { get; set; }
        public string CAPAImgeFile { get; set; }

    }
    public class ChecklistDescModel
    {
        public string ChecklistDesc { get; set; }
        public string Target { get; set; }
        public string Value { get; set; }
        public int CMID { get; set; }
        public int CID { get; set; }



    }
    public class LogoutUser
    {
        public string Token { get; set; }

    }

    public class UserRequest : BaseEntity
    {
        public string UserName { get; set; }
        public string Token { get; set; }
    }
    public class Notification
    {
        public string EquipmentID { set; get; }
        public string EquipmentName { set; get; }
        public string ParameterID { get; set; }
        public string ParameterName { set; get; }
        public string status { get; set; }
        public string Time { get; set; }
    }
    public class LogoutResponse
    {
        public string Response { get; set; }
    }

    public class EquipmentResponse
    {
        public string id { get; set; }
        public string name { get; set; }
        public string tagId { get; set; }
        public string scanOption { get; set; }
        public List<Parameter> parameter { get; set; }
    }

    [Serializable]
    public class EquipmentParameterValues
    {
        public string id { get; set; }
        public string saveTime { get; set; }
        public List<MeasureData> parameter { get; set; }
    }

    [Serializable]
    public class ParametersSaveRequest
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public List<EquipmentParameterValues> Equipments { get; set; }
    }



    [Serializable]
    public class MeasureData
    {
        public string id { get; set; }
        public string value { get; set; }
    }

    public class ID
    {
        public string equipmentid { get; set; }
    }
    public class TimeList
    {
        public string equipmentid { get; set; }
        public string savetime { get; set; }
    }
    public class MobileFormula
    {
        public string planid { get; set; }
        public string equipmentid { get; set; }
        public string tagid { get; set; }
        public string formula { get; set; }
        public string ptype { get; set; }
        public string value { get; set; }
        public string time { get; set; }
        public string measuredby { get; set; }
    }

    public class Parameter
    {
        public string id { set; get; }
        public string name { set; get; }
        public string min { set; get; }
        public string max { set; get; }
        public string uom { set; get; }
        public string value { set; get; }
        public string dataType { set; get; }
        public string tagId { set; get; }
    }

    public class Equipment
    {
        public string EquipmentID { set; get; }
        public string EquipmentTagID { set; get; }
        public string EquipmentName { set; get; }
        public string ParameterID { get; set; }
        public string ParameterTagID { set; get; }
        public string ParameterName { set; get; }
        public string ScanOption { set; get; }
        public string LTV { set; get; }
        public string HTV { set; get; }
        public string UOM { set; get; }
        public string Value { set; get; }
        public string DataType { set; get; }
    }

    public class MeasuredData
    {
        public string ParameterName { set; get; }
        public string UOM { set; get; }
        public List<Value> Value { get; set; }
    }

    public class Value
    {
        public string Time { get; set; }
        public string value { set; get; }
    }

    public class data
    {

        public string Tag { get; set; }
        public string Value { get; set; }
        public string Time { get; set; }
    }
}
