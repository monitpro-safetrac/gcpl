using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MonitPro.Validations;
using MonitPro.Models;
using MonitPro.BLL;
using IncidentReportSystem.Models;
using MonitPro.DAL;
using MonitPro.Models.Dashboard;
using MonitPro.Models.MOC;
using MonitPro.Models.PSSR;

namespace MonitPro.Controllers
{
    public class HomeController : Controller
    {
        IncidentReportBLL IncidentBLL = new IncidentReportBLL();
        PSSRBLL pssrBLL = new PSSRBLL();
        CAPABLL CapaBLL = new CAPABLL();
        DashboardBLL SBal = new DashboardBLL();
        MOCBLL MocBLL = new MOCBLL();
        List<Role> UserRoles = new List<Role>();




        public ActionResult Index()
        {
            if (!MonitPro.Security.Security.Instance.IsLicenseValid())
                Response.RedirectToRoute("Default", new { Controller = "Account", action = "InvalidLicense" });

            return View();
        }

        
        public ActionResult Chart()
        {
            IncidentListViewModel Incidents = new IncidentListViewModel();
            Incidents.Roles = CurrentUser.Roles;
            Incidents.UserFullName = CurrentUser.FullName;
            Incidents.UserID = CurrentUser.UserID;
            Incidents.ProfileImage = CurrentUser.ProfileImage;
            Incidents.IsRestrict = CurrentUser.IsRestrict;
            return View(Incidents);
        }

        public JsonResult IncidentCategoryChart()
        {
            IncidentTypeChart CategoryChart = new IncidentTypeChart()
            {
                Roles = UserRoles,
                CategoryCounts = IncidentBLL.GetIncidentCategoryCount()
            };
            return Json(CategoryChart.CategoryCounts.ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStatus()
        {
            StatusChartViewModel statusChartViewModel = new StatusChartViewModel()
            {
                Roles = UserRoles,
                statusCounts = IncidentBLL.GetStatusCount()
            };

            return Json(statusChartViewModel.statusCounts.ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult IncidentRecommenChart()
        {
            ObservationStatusChart ObStatus = new ObservationStatusChart()
            {
                Roles = UserRoles,
                ActionCounts = IncidentBLL.GetRecommenStatusCount()
            };

            return Json(ObStatus.ActionCounts.ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult RootCauseCount()
        {
            RootCauseChartViewModel rootcausecount = new RootCauseChartViewModel
            {
                Roles = UserRoles,
                rootCauseCounts = IncidentBLL.GetRootCauseCount()
            };

            return Json(rootcausecount.rootCauseCounts.ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult IncidentSummary()
        {
            ChartViewModel chartViewModel = new ChartViewModel
            {
                Roles = UserRoles,
                MonthlyCounts = IncidentBLL.GetMonthlyCount()
            };

            return Json(chartViewModel.MonthlyCounts.ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult IncidentClassification()
        {
            IncidentClassificationCount classificationcount = new IncidentClassificationCount
            {
                Roles = UserRoles,
                classificationCounts = IncidentBLL.GetClassificationCount()
            };

            return Json(classificationcount.classificationCounts.ToList(), JsonRequestBehavior.AllowGet);
        }
        //Get: Last Month Incident Report
        public JsonResult LastMonthlyReportChart()
        {
            ChartViewModel chartViewModel = new ChartViewModel
            {
                Roles = UserRoles,
                MonthlyCounts = IncidentBLL.GetLastMonthlyCount()
            };
            return Json(chartViewModel.MonthlyCounts.ToList(), JsonRequestBehavior.AllowGet);
        }

        //Get: Last Month Incident Category Chart Report
        public JsonResult LastMonthIncidentCategoryChart()
        {
            IncidentTypeChart CategoryChart = new IncidentTypeChart()
            {
                Roles = UserRoles,
                CategoryCounts = IncidentBLL.GetLastMonthIncidentCategory()
            };

            return Json(CategoryChart.CategoryCounts.ToList(), JsonRequestBehavior.AllowGet);
        }

        //Get: Last Month Incident ClassificationCount Chart Report
        public JsonResult LastMonthIncidentClassificationCount()
        {
            IncidentClassificationCount classificationcount = new IncidentClassificationCount
            {
                Roles = UserRoles,
                classificationCounts = IncidentBLL.GetLastMonthClassificationCount()
            };

            return Json(classificationcount.classificationCounts.ToList(), JsonRequestBehavior.AllowGet);
        }

        //Get: Last Month StatusChart Chart Report
        public JsonResult LastMonthStatusChart()
        {
            StatusChartViewModel statusChartViewModel = new StatusChartViewModel()
            {
                Roles = UserRoles,
                statusCounts = IncidentBLL.GetLastMonthStatusCount()
            };

            return Json(statusChartViewModel.statusCounts.ToList(), JsonRequestBehavior.AllowGet);
        }

        //Get: Last Month RootCauseCount Chart Report
        public JsonResult LastMonthRootCauseCount()
        {
            RootCauseChartViewModel rootcausecount = new RootCauseChartViewModel
            {
                Roles = UserRoles,
                rootCauseCounts = IncidentBLL.GetLastMonthRootCauseCount()
            };

            return Json(rootcausecount.rootCauseCounts.ToList(), JsonRequestBehavior.AllowGet);
        }

        //Get: Last Month IncidentRecommenChart Report
        public JsonResult LastMonthIncidentRecommenChart()
        {
            ObservationStatusChart ObStatus = new ObservationStatusChart()
            {
                Roles = UserRoles,
                ActionCounts = IncidentBLL.GetLastMonthRecommenStatusCount()
            };

            return Json(ObStatus.ActionCounts.ToList(), JsonRequestBehavior.AllowGet);
        }

        //capa chart dashboard
        public ActionResult Capachart()
        {
            IncidentListViewModel Incidents = new IncidentListViewModel();
            Incidents.Roles = CurrentUser.Roles;
            Incidents.UserFullName = CurrentUser.FullName;
            Incidents.UserID = CurrentUser.UserID;
            Incidents.ProfileImage = CurrentUser.ProfileImage;
            Incidents.IsRestrict = CurrentUser.IsRestrict;
            return View(Incidents);
        }

        //Get:   observationCount
        public ActionResult ObservationCount()
        {
            ActionsStatusChartViewModel observationcount = new ActionsStatusChartViewModel
            {
                Roles = UserRoles,
                observationcount = CapaBLL.GetObservationCount()
            };

            return Json(observationcount.observationcount.ToList(), JsonRequestBehavior.AllowGet);
        }

        //Get: CapaSourceCount
        public ActionResult CapaSourceCount()
        {
            ActionsStatusChartViewModel capasourcecount = new ActionsStatusChartViewModel
            {
                Roles = UserRoles,
                CapaSourceCount = CapaBLL.GetCapaSourceCount()
            };

            return Json(capasourcecount.CapaSourceCount.ToList(), JsonRequestBehavior.AllowGet);
        }

        //Get:  CategoryCount
        public ActionResult CategoryCount()
        {
            ActionsStatusChartViewModel categorycount = new ActionsStatusChartViewModel
            {
                Roles = UserRoles,
                categorycount = CapaBLL.GetCategoryCount()
            };


            return Json(categorycount.categorycount.ToList(), JsonRequestBehavior.AllowGet);
        }


        //Get: capa StatusChart
        public ActionResult ActionStatusChart()
        {
            ActionsStatusChartViewModel actionstatusChartViewModel = new ActionsStatusChartViewModel()
            {
                Roles = UserRoles,
                ActionCounts = CapaBLL.GetActionStatusCount()
            };


            return Json(actionstatusChartViewModel.ActionCounts.ToList(), JsonRequestBehavior.AllowGet);
        }

        //Get: last month CapaPriorityCount
        public ActionResult CapaPriorityCount()
        {
            ActionsStatusChartViewModel priority = new ActionsStatusChartViewModel
            {
                Roles = UserRoles,
                prioritycount = CapaBLL.GetCapaPriorityCount()
            };

            return Json(priority.prioritycount.ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CapaManagerCount()
        {
            ActionsStatusChartViewModel count = new ActionsStatusChartViewModel
            {
                Roles = UserRoles,
                prioritycount = CapaBLL.GetCapaFunctionalManagerCount()
            };

            return Json(count.prioritycount.ToList(), JsonRequestBehavior.AllowGet);
        }
        //last month capa
        public ActionResult LastMonthObservation()
        {
            ActionsStatusChartViewModel observationcount = new ActionsStatusChartViewModel
            {
                Roles = UserRoles,
                observationcount = CapaBLL.GetLastMonthObservationCount()
            };
            return Json(observationcount.observationcount.ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult LastMonthCapaSource()
        {
            ActionsStatusChartViewModel capasourcecount = new ActionsStatusChartViewModel
            {
                Roles = UserRoles,
                CapaSourceCount = CapaBLL.GetLastMonthCapaSourceCount()
            };

            return Json(capasourcecount.CapaSourceCount.ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult LastMonthCategoryCount()
        {
            ActionsStatusChartViewModel categorycount = new ActionsStatusChartViewModel
            {
                Roles = UserRoles,
                categorycount = CapaBLL.GetLastMonthCategoryCount()
            };

            return Json(categorycount.categorycount.ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult LastMonthActionStatus()
        {
            ActionsStatusChartViewModel actionstatusChartViewModel = new ActionsStatusChartViewModel()
            {
                Roles = UserRoles,
                ActionCounts = CapaBLL.GetLastMonthActionStatusCount()
            };

            return Json(actionstatusChartViewModel.ActionCounts.ToList(), JsonRequestBehavior.AllowGet);
        }
        //Get: last month CapaPriorityCount
        public ActionResult LastMonthCapaPriorityCount()
        {
            ActionsStatusChartViewModel priority = new ActionsStatusChartViewModel
            {
                Roles = UserRoles,
                prioritycount = CapaBLL.GetLastMonthCapaPriorityCount()
            };

            return Json(priority.prioritycount.ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult LastMonthCapaManagerCount()
        {
            ActionsStatusChartViewModel managercount = new ActionsStatusChartViewModel
            {
                Roles = UserRoles,
                prioritycount = CapaBLL.GetLastMonthCapaFunctionalManagerCount()
            };

            return Json(managercount.prioritycount.ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult MOCRecomStatusCount()
        {
            DateTime currentDate = DateTime.Now;
            DateTime startDate;
            DateTime endDate;

            if (currentDate.Month < 4)
            {
                startDate = new DateTime(currentDate.Year - 1, 4, 1); // April 1st of previous year
            }
            else
            {
                startDate = new DateTime(currentDate.Year, 4, 1); // April 1st of current year
            }

            // Determine the end date
            if (currentDate.Month < 4)
            {
                endDate = new DateTime(currentDate.Year, 3, 31); // March 31st of current year
            }
            else
            {
                endDate = new DateTime(currentDate.Year + 1, 3, 31); // March 31st of next year
            }


            MOCDashboard Mocdash = new MOCDashboard()
            {
                Roles = UserRoles,
                RecomStatusCount = CapaBLL.GetMOCRecomStatusCount(startDate, endDate)
            };

            return Json(Mocdash.RecomStatusCount.ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult RecommendationChart()
        {
            DateTime currentDate = DateTime.Now;
            DateTime startDate;
            DateTime endDate;

            if (currentDate.Month < 4)
            {
                startDate = new DateTime(currentDate.Year - 1, 4, 1); // April 1st of previous year
            }
            else
            {
                startDate = new DateTime(currentDate.Year, 4, 1); // April 1st of current year
            }

            // Determine the end date
            if (currentDate.Month < 4)
            {
                endDate = new DateTime(currentDate.Year, 3, 31); // March 31st of current year
            }
            else
            {
                endDate = new DateTime(currentDate.Year + 1, 3, 31); // March 31st of next year
            }
            //endDate = currentDate;

            IncidentDashboard recommendationData = new IncidentDashboard
            {
                RecommendationModel = SBal.GetRecommendationsBLL(startDate, endDate)
            };
            //List<RecommendationModel> recommendationData = SBal.GetRecommendationsBLL(startDate, endDate);
            return Json(recommendationData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CAPAPlantCount()
        {
            DateTime currentDate = DateTime.Now;
            DateTime startDate;
            DateTime endDate;

            if (currentDate.Month < 4)
            {
                startDate = new DateTime(currentDate.Year - 1, 4, 1); // April 1st of previous year
            }
            else
            {
                startDate = new DateTime(currentDate.Year, 4, 1); // April 1st of current year
            }

            // Determine the end date
            if (currentDate.Month < 4)
            {
                endDate = new DateTime(currentDate.Year, 3, 31); // March 31st of current year
            }
            else
            {
                endDate = new DateTime(currentDate.Year + 1, 3, 31); // March 31st of next year
            }
            PSSRDashboard dash = new PSSRDashboard
            {
                Roles = UserRoles,
                CAPACountList = pssrBLL.GetCAPAPlantCount(startDate, endDate)
            };
            return Json(dash.CAPACountList.ToList(), JsonRequestBehavior.AllowGet);
        }




    }
}
