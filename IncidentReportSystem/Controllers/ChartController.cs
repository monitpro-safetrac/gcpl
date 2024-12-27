using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using MonitPro.DAL;
using MonitPro.Validations;
using MonitPro.Models;
using IncidentReportSystem.Models;
using System.Linq;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Web;
using ClosedXML.Excel;
using System.IO;
using System.Globalization;
using System.Net.Mail;
using MonitPro.Common.Library;
using PagedList;
using ClosedXML;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Web.UI;
using MonitPro.BLL;

namespace ValsparApp.Controllers
{
    [ValidateSession]
    public class ChartController : Controller
    {


        public List<StatusCountwps> GetStatusCount()
        {
            List<StatusCountwps> statusCounts = new List<StatusCountwps>();

            try
            {
                using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();

                    objCom.CommandText = "[GetStatusCountwps]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    SqlDataReader reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        StatusCountwps statusCount = new StatusCountwps()
                        {
                            Name = reader["Name"].ToString(),
                            TotalCount = int.Parse(reader["TotalCount"].ToString())
                        };
                        statusCounts.Add(statusCount);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return statusCounts;
        }
        public List<PermitInProgress> GetPermitCount()
        {
            List<PermitInProgress> PermitCounts = new List<PermitInProgress>();

            try
            {
                using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();

                    objCom.CommandText = "[PermitProgress]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    SqlDataReader reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        PermitInProgress permitCounts = new PermitInProgress()
                        {
                            WorkType = reader["WorkType"].ToString(),
                            PermitCount = int.Parse(reader["PermitCount"].ToString(
                                ))
                        };
                        PermitCounts.Add(permitCounts);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return PermitCounts;
        }

        public List<PermitInProgress> GetCummulativePermitCount(DateTime startDate, DateTime endDate)
        {
            List<PermitInProgress> PermitCounts = new List<PermitInProgress>();

            try
            {
                using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();

                    objCom.CommandText = "[CummualtiveTypeofPermitProgress]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@StartDate", startDate);
                    objCom.Parameters.AddWithValue("@EndDate", endDate);
                    objCom.Connection = objCon;
                    objCon.Open();
                    SqlDataReader reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        PermitInProgress permitCounts = new PermitInProgress()
                        {
                            WorkType = reader["WorkType"].ToString(),
                            PermitCount = int.Parse(reader["PermitCount"].ToString())
                        };
                        PermitCounts.Add(permitCounts);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return PermitCounts;
        }
        public List<StackCount> GetStackCount(DateTime startDate, DateTime endDate)
        {
            List<StackCount> StackCount = new List<StackCount>();

            try
            {
                using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();

                    objCom.CommandText = "[GetStackChart]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@StartDate", startDate);
                    objCom.Parameters.AddWithValue("@EndDate", endDate);
                    objCom.Connection = objCon;
                    objCon.Open();
                    SqlDataReader reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        StackCount stackCount = new StackCount()
                        {
                            PermitMonth = reader["PermitMonth"].ToString(),
                            //MonthlyCount = int.Parse(reader["MonthlyCount"].ToString()),
                            approved = int.Parse(reader["Approved"].ToString()),
                            extend = int.Parse(reader["Extend"].ToString()),
                            closed = int.Parse(reader["Closed"].ToString()),
                            cancel = int.Parse(reader["Cancelled"].ToString()),
                        };
                        StackCount.Add(stackCount);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return StackCount;
        }
        public List<ContracChart> GetContracCount()
        {
            List<ContracChart> ContracChart = new List<ContracChart>();

            try
            {
                using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();

                    objCom.CommandText = "[GetContChart]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    SqlDataReader reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        ContracChart contracChart = new ContracChart()
                        {
                            ContractorName = reader["ContractorName"].ToString(),
                            CountOfCon = int.Parse(reader["CountOfCon"].ToString()),

                        };
                        ContracChart.Add(contracChart);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return ContracChart;
        }

        [HttpGet]
        public ActionResult Newchartwps()
        {
            WorkPermit workPermit = new WorkPermit();
            workPermit.Roles = CurrentUser.Roles;
            workPermit.UserFullName = CurrentUser.FullName;
            workPermit.ProfileImage = CurrentUser.ProfileImage;
            workPermit.IsRestrict = CurrentUser.IsRestrict;
            return View(workPermit);
        }

        // Type of Permits in Progress drawChart2
        public JsonResult Typeofpermit()
        {
            var list1 = GetPermitCount().ToList();

            return Json(list1, JsonRequestBehavior.AllowGet);
        }

        // Contractors Vs Number of Permits(Today )
        public JsonResult ContractorPermits()
        {
            var list3 = GetContracCount().ToList();
            return Json(list3, JsonRequestBehavior.AllowGet);
        }

        // Cumulative Status of  Permits
        public JsonResult StatusPermit()
        {
            var list = GetStatusCount().ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //cumulative work permit progress chart 4
        public JsonResult PermitProgress()
        {
            var list = GetContracCount().ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CummulativeTypeofpermit(DateTime startDate, DateTime endDate)
        {
            var list1 = GetCummulativePermitCount(startDate, endDate).ToList();

            return Json(list1, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EquipmentPermitCount()
        {
            var list = EquipmentCount().ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetData2(DateTime startDate, DateTime endDate)
        {

            var list2 = GetStackCount(startDate, endDate).ToList();
            return Json(list2, JsonRequestBehavior.AllowGet);
        }
        public List<EquipmentCount> EquipmentCount()
        {
            List<EquipmentCount> equipmentCounts = new List<EquipmentCount>();

            try
            {
                using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();

                    objCom.CommandText = "[GetEquipmentPermitCount]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    SqlDataReader reader = objCom.ExecuteReader();

                    while (reader.Read())
                    {
                        EquipmentCount Count = new EquipmentCount()
                        {
                            EquipmentName = reader["Name"].ToString(),
                            TotalCount = int.Parse(reader["TotalCount"].ToString())
                        };
                        equipmentCounts.Add(Count);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return equipmentCounts;
        }


    }



}


