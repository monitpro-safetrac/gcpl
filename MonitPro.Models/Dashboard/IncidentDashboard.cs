using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.Dashboard
{
    public class IncidentDashboard : BaseEntity
    {
        public List<IncidentSummaryModel> IncidentSummaryModel { get; set; }
        public List<IncidentCategoryModel> IncidentCategoryModel { get; set; }
        public List<IncidentClassificationModel> IncidentClassificationModel { get; set; }
        public List<IncidentStatusModel> IncidentStatusModel { get; set; }
        public List<RootCauseModel> RootCauseModel { get; set; }
        public List<RecommendationModel> RecommendationModel { get; set; }
        public int UserID { get; set; }
        public string Name { get; set; }

    }
    public class IncidentSummaryModel
    {
        public string CategoryName { get; set; }
        public int TotalCount { get; set; }
    }
    public class IncidentCategoryModel
    {
        public string CategoryName { get; set; }
        public int TotalCount { get; set; }
    }
    public class IncidentClassificationModel
    {
        public string CategoryName { get; set; }
        public int TotalCount { get; set; }
    }
    public class IncidentStatusModel
    {
        public string CategoryName { get; set; }
        public int TotalCount { get; set; }
    }
    public class RootCauseModel
    {
        public string CategoryName { get; set; }
        public int TotalCount { get; set; }
    }
    public class RecommendationModel
    {
        public string CategoryName { get; set; }
        public int TotalCount { get; set; }
    }
   
    public class CAPADashboard : BaseEntity
    {
        public List<OverallCAPAStatus> OverallCAPAStatus { get; set; }
        public List<OverallCAPACategory> OverallCAPACategory { get; set; }
        public List<OverallCAPASource> OverallCAPASource { get; set; }
        public List<OverallCAPAPriority> OverallCAPAPriority { get; set; }
        public List<OverallCAPASummary> OverallCAPASummary { get; set; }
        public List<OverallCAPARecommendation> OverallCAPARecommendation { get; set; }
    }
    public class OverallCAPAStatus
    {
        public string Actions { get; set; }
        public int ActionsCount { get; set; }
    }
    public class OverallCAPACategory
    {
        public string CategoryName { get; set; }
        public int TotalCount { get; set; }
    }
    public class OverallCAPASource
    {
        public string CAPASourceName { get; set; }
        public int TotalCount { get; set; }
    }
    public class OverallCAPAPriority
    {
        public string Name { get; set; }
        public int TotalCount { get; set; }
        public int overdue { get; set; }
        public int closed { get; set; }
        public int opened { get; set; }
        public int New { get; set; }
    }
    public class OverallCAPASummary
    {
        public string ObMonth { get; set; }
        public int ObDate { get; set; }
        public int TotalCount { get; set; }
    }
    public class OverallCAPARecommendation
    {
        public string FunctionalManager { get; set; }
        public int TotalCount { get; set; }
        public int overdue { get; set; }
        public int closed { get; set; }
        public int opened { get; set; }
        public int New { get; set; }
    }
}


