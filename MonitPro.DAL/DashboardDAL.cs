using MonitPro.Models.Dashboard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using MonitPro.Common.Library;
using MonitPro.Models.CAPAViewModel;
//using PdfSharp.Pdf.Content.Objects;
using System.Collections;

namespace MonitPro.DAL
{
    public class DashboardDAL
    {

        public List<IncidentSummaryModel> GetIncidentSummary(DateTime startDate, DateTime endDate)
        {
            List<IncidentSummaryModel> summaryList = new List<IncidentSummaryModel>();

            try
            {
                using (SqlConnection obj = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "GetIncidentSummary";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);
                    cmd.Connection = obj;
                    obj.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {

                        IncidentSummaryModel summary = new IncidentSummaryModel()
                        {
                            CategoryName = reader["IncidentMonth"].ToString(),
                            TotalCount = Convert.ToInt32(reader["MonthlyCount"]),
                        };
                        summaryList.Add(summary);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (log, throw, etc.)
                throw new Exception("Error retrieving IncidentSummary data.", ex);
            }

            return summaryList;
        }

        // Method to retrieve IncidentCategory data
        public List<IncidentCategoryModel> GetIncidentCategories(DateTime startDate, DateTime endDate)
        {
            List<IncidentCategoryModel> categories = new List<IncidentCategoryModel>();

            try
            {
                using (SqlConnection obj = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "GetIncidenCatedry";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);
                    cmd.Connection = obj;
                    obj.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        IncidentCategoryModel category = new IncidentCategoryModel()
                        {
                            CategoryName = reader["name"].ToString(),
                            TotalCount = Convert.ToInt32(reader["Categorycount"]),

                        };
                        categories.Add(category);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (log, throw, etc.)
                throw new Exception("Error retrieving IncidentCategory data.", ex);
            }

            return categories;
        }

        // Method to retrieve IncidentClassification data
        public List<IncidentClassificationModel> GetIncidentClassifications(DateTime startDate, DateTime endDate)
        {
            List<IncidentClassificationModel> classifications = new List<IncidentClassificationModel>();

            try
            {
                using (SqlConnection obj = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "GetIncidenClassification";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);
                    cmd.Connection = obj;
                    obj.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        IncidentClassificationModel classification = new IncidentClassificationModel()
                        {
                            CategoryName = reader["name"].ToString(),
                            TotalCount = Convert.ToInt32(reader["statuscount"]),
                        };
                        classifications.Add(classification);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (log, throw, etc.)
                throw new Exception("Error retrieving IncidentClassification data.", ex);
            }

            return classifications;
        }

        // Method to retrieve IncidentStatus data
        public List<IncidentStatusModel> GetIncidentStatuses(DateTime startDate, DateTime endDate)
        {
            List<IncidentStatusModel> statuses = new List<IncidentStatusModel>();

            try
            {
                using (SqlConnection obj = new SqlConnection(AppConfig.ConnectionString))
                {

                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "GetIncidenStatus";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);
                    cmd.Connection = obj;
                    obj.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        IncidentStatusModel status = new IncidentStatusModel()
                        {
                            CategoryName = reader["name"].ToString(),
                            TotalCount = Convert.ToInt32(reader["statuscount"]),
                        };
                        statuses.Add(status);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (log, throw, etc.)
                throw new Exception("Error retrieving IncidentStatus data.", ex);
            }

            return statuses;
        }

        // Method to retrieve RootCause data
        public List<RootCauseModel> GetRootCauses(DateTime startDate, DateTime endDate)
        {
            List<RootCauseModel> rootCauses = new List<RootCauseModel>();

            try
            {
                using (SqlConnection obj = new SqlConnection(AppConfig.ConnectionString))
                {

                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "GetIncidenRootCause";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);
                    cmd.Connection = obj;
                    obj.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        RootCauseModel rootCause = new RootCauseModel()
                        {
                            CategoryName = reader["Name"].ToString(),
                            TotalCount = Convert.ToInt32(reader["statuscount"]),
                        };
                        rootCauses.Add(rootCause);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (log, throw, etc.)
                throw new Exception("Error retrieving RootCause data.", ex);
            }

            return rootCauses;
        }

        // Method to retrieve Recommendation data
        public List<RecommendationModel> GetRecommendations(DateTime startDate, DateTime endDate)
        {
            List<RecommendationModel> recommendations = new List<RecommendationModel>();

            try
            {
                using (SqlConnection obj = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "Recommendation";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);
                    cmd.Connection = obj;
                    obj.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        RecommendationModel recommendation = new RecommendationModel()
                        {
                            CategoryName = reader["Actions"].ToString(),
                            TotalCount = Convert.ToInt32(reader["ActionsCount"]),
                        };
                        recommendations.Add(recommendation);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (log, throw, etc.)
                throw new Exception("Error retrieving Recommendation data.", ex);
            }

            return recommendations;
        }
       public CAPADashboard GetCAPADashboardDAL(DateTime startDate, DateTime endDate)
        {
            var dashboard = new CAPADashboard();

            try
            {
                using (SqlConnection obj = new SqlConnection(AppConfig.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("GetCAPADashboard", obj))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FromDate", startDate);
                        cmd.Parameters.AddWithValue("@ToDate", endDate);

                        obj.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Read OverallCAPAStatus
                            var overallCAPAStatus = new List<OverallCAPAStatus>();
                            while (reader.Read())
                            {
                                overallCAPAStatus.Add(new OverallCAPAStatus
                                {
                                    Actions = reader["Actions"].ToString(),
                                    ActionsCount = Convert.ToInt32(reader["ActionsCount"])
                                });
                            }
                            reader.NextResult();

                            // Read OverallCAPACategory
                            var overallCAPACategory = new List<OverallCAPACategory>();
                            while (reader.Read())
                            {
                                overallCAPACategory.Add(new OverallCAPACategory
                                {
                                    CategoryName = reader["CategoryName"].ToString(),
                                    TotalCount = Convert.ToInt32(reader["TotalCount"])
                                });
                            }
                            reader.NextResult();

                            // Read OverallCAPASource
                            var overallCAPASource = new List<OverallCAPASource>();
                            while (reader.Read())
                            {
                                overallCAPASource.Add(new OverallCAPASource
                                {
                                    CAPASourceName = reader["CapaSourceName"].ToString(),
                                    TotalCount = Convert.ToInt32(reader["TotalCount"])
                                });
                            }
                            reader.NextResult();

                            // Read OverallCAPAPriority
                            var overallCAPAPriority = new List<OverallCAPAPriority>();
                            while (reader.Read())
                            {
                                overallCAPAPriority.Add(new OverallCAPAPriority
                                {
                                    Name = reader["Name"].ToString(),
                                    TotalCount = Convert.ToInt32(reader["TotalCount"]),
                                    overdue = Convert.ToInt32(reader["overdue"]),
                                    closed = Convert.ToInt32(reader["closed"]),
                                    opened = Convert.ToInt32(reader["opened"]),
                                    New = Convert.ToInt32(reader["New"])
                                });
                            }
                            reader.NextResult();

                            // Read OverallCAPASummary
                            var overallCAPASummary = new List<OverallCAPASummary>();
                            while (reader.Read())
                            {
                                overallCAPASummary.Add(new OverallCAPASummary
                                {
                                    ObMonth = reader["ObMonth"].ToString(),
                                    ObDate = Convert.ToInt32(reader["ObDate"]),
                                    TotalCount = Convert.ToInt32(reader["TotalCount"])
                                });
                            }
                            reader.NextResult();

                            // Read OverallCAPARecommendation
                            var overallCAPARecommendation = new List<OverallCAPARecommendation>();
                            while (reader.Read())
                            {
                                overallCAPARecommendation.Add(new OverallCAPARecommendation
                                {
                                    FunctionalManager = reader["FunctionalManager"].ToString(),
                                    overdue = Convert.ToInt32(reader["overdue"]),
                                    closed = Convert.ToInt32(reader["closed"]),
                                    opened = Convert.ToInt32(reader["opened"]),

                                });
                            }

                            // Assign to CAPADashboard
                            dashboard.OverallCAPAStatus = overallCAPAStatus;
                            dashboard.OverallCAPACategory = overallCAPACategory;
                            dashboard.OverallCAPASource = overallCAPASource;
                            dashboard.OverallCAPAPriority = overallCAPAPriority;
                            dashboard.OverallCAPASummary = overallCAPASummary;
                            dashboard.OverallCAPARecommendation = overallCAPARecommendation;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (log, throw, etc.)
                throw new Exception("Error retrieving CAPA Dashboard data.", ex);
            }

            return dashboard;
        }
    }
}



