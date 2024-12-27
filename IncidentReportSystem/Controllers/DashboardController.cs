using System;
using MonitPro.BLL;
using MonitPro.Models.Dashboard;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MonitPro.Models.Account;
using MonitPro.Models;
using Microsoft.Ajax.Utilities;
using IncidentReportSystem.Models;
using MonitPro.Models.CAPA;
using MonitPro.Models.CAPAViewModel;
using DocumentFormat.OpenXml.Drawing.Charts;
using MonitPro.DAL;
using MonitPro.Models.MOC;

namespace WorkPermitSystem.Controllers
{
    public class DashboardController : Controller
    {
        DashboardBLL SBal = new DashboardBLL();
        CAPABLL CapaBLL = new CAPABLL();
        List<CAPAObservationStatus> capaobservationstatus = new List<CAPAObservationStatus>();
        SessionDetails sess = new SessionDetails();
        MOCBLL MocBLL = new MOCBLL();

        public ActionResult InsidentDashboard()
        {

            IncidentDashboard DDModel = new IncidentDashboard();

            DDModel.Roles = CurrentUser.Roles;
            DDModel.UserFullName = CurrentUser.FullName;
            DDModel.ProfileImage = CurrentUser.ProfileImage;
            DDModel.DesigID = CurrentUser.Designation;
            DDModel.UserID = CurrentUser.UserID;
            string Name = CurrentUser.FullName;
            sess = CapaBLL.GetSession(CurrentUser.UserID);
            int UserID = CurrentUser.UserID;
            return View(DDModel);
        }

        // GET: Home/IncidentSummaryChart
        public ActionResult IncidentSummaryChart(DateTime startDate, DateTime endDate)
        {
            IncidentDashboard summaryData = new IncidentDashboard
            {
                IncidentSummaryModel = SBal.GetIncidentSummaryBLL(startDate, endDate)
            };
            return Json(summaryData, JsonRequestBehavior.AllowGet);
        }

        // GET: Home/IncidentCategoryChart
        public ActionResult IncidentCategoryChart(DateTime startDate, DateTime endDate)
        {
            IncidentDashboard categoryData = new IncidentDashboard
            {
                IncidentCategoryModel = SBal.GetIncidentCategoriesBLL(startDate, endDate)
            };
            return Json(categoryData, JsonRequestBehavior.AllowGet);
        }

        // GET: Home/IncidentClassificationChart
        public ActionResult IncidentClassificationChart(DateTime startDate, DateTime endDate)
        {
            IncidentDashboard classificationData = new IncidentDashboard
            {
                IncidentClassificationModel = SBal.GetIncidentClassificationsBLL(startDate, endDate)
            };
            return Json(classificationData, JsonRequestBehavior.AllowGet);
        }

        // GET: Home/IncidentStatusChart
        public ActionResult IncidentStatusChart(DateTime startDate, DateTime endDate)
        {
            IncidentDashboard statusData = new IncidentDashboard
            {
                IncidentStatusModel = SBal.GetIncidentStatusesBLL(startDate, endDate)
            };
            return Json(statusData, JsonRequestBehavior.AllowGet);
        }

        // GET: Home/RootCauseChart
        public ActionResult RootCauseChart(DateTime startDate, DateTime endDate)
        {
            IncidentDashboard rootCauseData = new IncidentDashboard
            {
                RootCauseModel = SBal.GetRootCausesBLL(startDate, endDate)
            };
            //List<RootCauseModel> rootCauseData = SBal.GetRootCausesBLL(startDate, endDate);
            return Json(rootCauseData, JsonRequestBehavior.AllowGet);
        }

        // GET: Home/RecommendationChart
        public ActionResult RecommendationChart(DateTime startDate, DateTime endDate)
        {
            IncidentDashboard recommendationData = new IncidentDashboard
            {
                RecommendationModel = SBal.GetRecommendationsBLL(startDate, endDate)
            };
            //List<RecommendationModel> recommendationData = SBal.GetRecommendationsBLL(startDate, endDate);
            return Json(recommendationData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index(DateTime? startDate, DateTime? endDate)
        {

            var dashboardData = SBal.GetCAPADashboardBLL(startDate.Value, endDate.Value);
            return Json(dashboardData, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult MOCClassMonthlyCount(DateTime startDate, DateTime endDate)
        {
            MOCDashboard Mocdash = new MOCDashboard()
            {
                ClassCount = MocBLL.GetMOCClassCount(startDate, endDate)
            };
            return Json(Mocdash.ClassCount.ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult MOCPlantCount(DateTime startDate, DateTime endDate)
        {
            MOCDashboard Mocdash = new MOCDashboard()
            {
                PlantCount = MocBLL.GetMOCPlantCount(startDate, endDate)
            };
            return Json(Mocdash.PlantCount.ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult MOCCategoryCount(DateTime startDate, DateTime endDate)
        {
            MOCDashboard Mocdash = new MOCDashboard()
            {
                CategoryCount = MocBLL.GetMOCCategoryCount(startDate, endDate)
            };
            return Json(Mocdash.CategoryCount.ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult MOCStatusCount(DateTime startDate, DateTime endDate)
        {
            MOCDashboard Mocdash = new MOCDashboard()
            {
                StatusCount = MocBLL.GetMOCStatusCount(startDate, endDate)
            };
            return Json(Mocdash.StatusCount.ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult MOCPriorityCount(DateTime startDate, DateTime endDate)
        {
            MOCDashboard Mocdash = new MOCDashboard()
            {
                PriorityCount = MocBLL.GetMOCPriorityCount(startDate, endDate)
            };
            return Json(Mocdash.PriorityCount.ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult MOCRecomStatusCount(DateTime startDate, DateTime endDate)
        {
            MOCDashboard Mocdash = new MOCDashboard()
            {
                RecomStatusCount = MocBLL.GetMOCRecomStatusCount(startDate, endDate)
            };
            return Json(Mocdash.RecomStatusCount.ToList(), JsonRequestBehavior.AllowGet);
        }
    }
}


