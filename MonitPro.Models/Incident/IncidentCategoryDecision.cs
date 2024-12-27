using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.Incident
{
    public class IncidentCategoryDecision
    {
        public int QuestionID { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public int UserValue { get; set; }
        public int DescriptionIdentity { get; set; }

    }
    public class IncidentMaincategoryModel :BaseEntity
    {
        public int IncidentID { get; set; }
        public int CurrentUserID { get; set; }
        public int DecisionTypeID { get; set; }
        public int tempValue { get; set; }
        public string IncidentTitle { get; set; }
        public string IncidentNO { get; set; }
        public string PlantName { get; set; }
        public int IncidentChemicalQTYType { get; set; }
        public CategoryCalculation calculationResult { get; set; }
        public List<DecisionTypeDD> DecisionTypeList { get; set; }
        public List<IncidentCategoryDecision> decisionlist { get; set;}
        public List<API754Details> Api754List { get; set; }
        public List<ChemicalQTY> ChemicalList { get; set; }
    }
    public class DecisionTypeDD
    {
        public int DecisionTypeID { get; set; }
        public string DecisionTypeName { get; set; }
        public int Status { get; set; }
    }

    public class IncidentCategoryDecisionXML
    {
        public int QuestionID { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public int UserValue { get; set; }
        public int descriptionIdentity { get; set; }
    }
    public class CategoryCalculation
    {
        public int Total { get; set; }
        public string CategoryName { get; set; }
    }
    public class API754Details
    {
        public int QID { get; set; }
        public string Description { get; set; }
        public int UserValue { get; set; }
        public int RedirectionID { get; set; }
        public string Result { get; set; }
    }
    public class API754XML
    {
        public int QuestionID { get; set; }
        public string Description { get; set; }
        public int UserValue { get; set; }
    }
    public class ChemicalQTY
    {
        public int ChemicalID { get; set; }
        public string ChemicalName { get; set; }
        public int UserValue { get;set; }
        public int Tier1Indoor { get; set; }
        public int Tier1Outdoor { get; set; }
        public int Tier2Indoor { get;set; }
        public int Tier2Outdoor { get; set; }
    }
    public class ChemicalQTYXML
    {
        public int ChemicalID { get; set; }
        public int ChemicalValue { get; set; }
    }
}
