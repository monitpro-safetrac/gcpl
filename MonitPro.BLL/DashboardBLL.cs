using MonitPro.DAL;
using MonitPro.Models.CAPAViewModel;
using MonitPro.Models.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.BLL
{
    public class DashboardBLL
    {
        DashboardDAL data = new DashboardDAL();

        // Method to get incident summary
        public List<IncidentSummaryModel> GetIncidentSummaryBLL(DateTime startDate, DateTime endDate)
        {
            return data.GetIncidentSummary(startDate, endDate);
        }

        // Method to get incident categories
        public List<IncidentCategoryModel> GetIncidentCategoriesBLL(DateTime startDate, DateTime endDate)
        {
            return data.GetIncidentCategories(startDate, endDate);
        }

        // Method to get incident classifications
        public List<IncidentClassificationModel> GetIncidentClassificationsBLL(DateTime startDate, DateTime endDate)
        {
            return data.GetIncidentClassifications(startDate, endDate);
        }

        // Method to get incident statuses
        public List<IncidentStatusModel> GetIncidentStatusesBLL(DateTime startDate, DateTime endDate)
        {
            return data.GetIncidentStatuses(startDate, endDate);
        }

        // Method to get root causes
        public List<RootCauseModel> GetRootCausesBLL(DateTime startDate, DateTime endDate)
        {
            return data.GetRootCauses(startDate, endDate);
        }

        // Method to get recommendations
        public List<RecommendationModel> GetRecommendationsBLL(DateTime startDate, DateTime endDate)
        {
            return data.GetRecommendations(startDate, endDate);
        }

        /// ////////////////////////////CAPADashboard

        public CAPADashboard GetCAPADashboardBLL(DateTime startDate, DateTime endDate)
        {
            return data.GetCAPADashboardDAL(startDate, endDate);
        }
    }
}



