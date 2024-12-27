using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MonitPro.Models
{


    public class Division
    {
        public string FactoryID { set; get; }
        public string DivisionID { set; get; }
        public string DivisionName { set; get; }


    }

    //public class AllData
    //{
    //    public List<Division> DivisionList;
    //    public List<Template> EquipmentList;
    //    public List<MeasureData> TemplateList;
    //    public List<DataList> MeasuredList;
    //    public List<Frequency> FrequencyList;
    //}


    public class TreeMapData
    {
        public List<Factory> FactoryList;
        public List<Division> DivisionList;
        public List<Equipment> EquipmentList;
        public List<MeasureData> TemplateList;
        public List<DataList> MeasuredList;
        public List<Frequency> FrequencyList;
        public List<User> UserList;
    }


    public class Template
    {
        public string Templateid { set; get; }
        public string TemplateName { set; get; }
        public string Divisionid { set; get; }

    }

    public class Frequency
    {
        public string FrequencyID { set; get; }
        public string FrequencyName { set; get; }
    }

    public class DataList
    {
        public string PlanID;
        public float MeasuredValue;
    }


    public class MeasureData
    {
        public string TemplateID { set; get; }
        public string PlanID { set; get; }
        public string Parameter { set; get; }
        public string UOM { set; get; }
        public string LTV { set; get; }
        public string HTV { set; get; }
        public string Status { get; set; }
        public string LHTPercentage { get; set; }
        public string LastUpdated { get; set; }
        public string EquipmentID { set; get; }
        public string ParameterDesc { set; get; }
        public string ReasonForLow { set; get; }
        public string ConsequenceForLow { set; get; }
        public string ActionForLow { set; get; }
        public string ReasonForHigh { set; get; }
        public string ReasonForMonitoring { set; get; }
        public string Notes { set; get; }
        public int CreatedBy { set; get; }
        public string ConsequenceForHigh { set; get; }
        public string ActionForHigh { set; get; }
        public char Priority { set; get; }
        public string TagID { set; get; }
        public string FrequencyID { set; get; }
        public string IsActive { set; get; }
        public string IsKPI { get; set; }
        public string CreatedOn { set; get; }
        public string Responsibility { get; set; }
        public string FrequencyId { get; set; }
        public IEnumerable<SelectListItem> Frequencies { get; set; }
        public List<User> UserList { get; set; }
        public string IsNumeric { set; get; }
        public char ParameterType { get; set; }
        public string Formula { get; set; }
        public int ParameterNo { get; set; }
    }

    public class Factory
    {
        public string FactoryID { set; get; }
        public string FactoryName { set; get; }
    }
    public class PlanData
    {

        public string PlanID { set; get; }
        public string EquipmentID { get; set; }
        public string Parameter { set; get; }
        public string UOM { set; get; }

        public string LTV { set; get; }
        public string HTV { set; get; }
        public string LHTPercentage { get; set; }
        public string ParameterDesc { set; get; }
        public string ReasonForLow { set; get; }
        public string ConsequenceForLow { set; get; }
        public string ActionForLow { set; get; }
        public string ReasonForHigh { set; get; }
        public string ReasonForMonitoring { set; get; }
        public string Notes { set; get; }

        public string ConsequenceForHigh { set; get; }
        public string ActionForHigh { set; get; }
        public char Priority { set; get; }
        public string TagID { set; get; }
        public string FrequencyID { set; get; }

        public string IsNumeric { set; get; }
        public string Formula { get; set; }
        public string ParameterType { get; set; }
        public string IsActive { set; get; }
        public string IsKPI { get; set; }
        public int Responsibility { set; get; }
        public int UpdatedBy { set; get; }
        public int LicenseCount { get; set; }
        public int ParameterNo { get; set;}
    }


    public class PlanEditModel : BaseEntity
    {

        [Required]
        [Display(Name = "Equipment Name")]
        public string EquipmentId { get; set; }
        public IEnumerable<SelectListItem> EquipmentName { get; set; }

        [Required]
        [Display(Name = "Parameter Name")]
        public string ParameterId { get; set; }
        public IEnumerable<SelectListItem> ParameterName { get; set; }


    }


    public class EnterDataModel : BaseEntity
    {
        [Required]
        [Display(Name = "Select Equipment")]
        public int EquipmentId { get; set; }
        public IEnumerable<SelectListItem> EquipmentName { get; set; }
        public string equipment { get; set; }
    }

    public class ParameterViewModel
    {
        public List<MeasureData> Measuredata { get; set; }

        public string equipment { get; set; }
    }

    public class Admin
    {
        public class PlanEditModel
        {

            [Required]
            [Display(Name = "Equipment Name")]
            public string EquipmentId { get; set; }
            public IEnumerable<SelectListItem> EquipmentName { get; set; }

            [Required]
            [Display(Name = "Parameter Name")]
            public string ParameterId { get; set; }
            public IEnumerable<SelectListItem> ParameterName { get; set; }


        }

    }
}
