using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using MonitPro.Validations;
using MonitPro.Models;
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
using IncidentReportSystem.Models;
using PagedList;
using ClosedXML;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Web.UI;
using MonitPro.BLL;
using MonitPro.DAL;
using System.Xml.Serialization;
using Font = iTextSharp.text.Font;
using Color = iTextSharp.text.BaseColor;
using System.Drawing.Imaging;

namespace ValsparApp.Controllers
{
    [ValidateSession]
    public class WorkPermitController : Controller
    {
        AdminBLL adminBLL = new AdminBLL();
        WorkPermit departlist = new WorkPermit();
        AdminDA adminDA = new AdminDA();
        WorkPermitBLL workPermitBLL = new WorkPermitBLL();
        Contract contractlist = new Contract();
        SessionDetails sess = new SessionDetails();
        private List<Contract> contractList;

        public WorkPermitController()
        {
            AdminDA adminDA = new AdminDA();
            var userList = adminDA.SelectUserProfile();
            var ActiveUser = userList.Where(x => x.IsActiveSelect == "Yes").ToList();
            ActiveUser.Insert(0, new UserProfile { UserID = -1, DisplayUserName = "N/A" });
            departlist.MechancialDept = ActiveUser.Where(x => x.DepartID == 8 || x.UserID == -1).ToList();
            departlist.ElectricalDept = ActiveUser.Where(x => x.DepartID == 6 || x.UserID == -1).ToList();
            departlist.InstrumentationDept = ActiveUser.Where(x => x.DepartID == 2 || x.UserID == -1).ToList();
            departlist.SafetyDept = ActiveUser.Where(x => x.DepartID == 13 || x.UserID == -1).ToList();
            departlist.ProcessManagerDept = ActiveUser.Where(x => x.DepartID == 1 || x.UserID == -1).ToList();
            departlist.GMDept = ActiveUser.Where(x => x.Role == 11 || x.UserID == -1).ToList();
            contractlist.ContractApprover = ActiveUser.Where(x => x.Role == 11).ToList();
            departlist.Standbyperson = ActiveUser.Where(x => x.DepartID != 15).ToList();
            var deptlist = adminDA.GetDepartmentList();
            deptlist.Insert(0, new Department { DeptID = -1, DeptName = "N/A" });
            departlist.DepartmentList = deptlist;
            sess = workPermitBLL.GetSession(CurrentUser.UserID);
        }



        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetContractors(int? workTypeID)
        {


            var contractors = workPermitBLL.GetContratorsSelect(workTypeID).Select(m => new SelectListItem()
            {
                Value = m.PermitHolderIdName.ToString(),
                Text = m.ContractorName
            });
            return Json(contractors, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxMethod(string name)
        {
            Approverlist typeofwork = new Approverlist();

            name = name.TrimEnd(',');

            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "ApproverContractorList";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@WorkType", name);

                    objCom.Connection = objCon;

                    DataSet dsResult = new DataSet();
                    SqlDataAdapter objAdapter = new SqlDataAdapter();
                    objAdapter.SelectCommand = objCom;
                    objAdapter.Fill(dsResult);

                    typeofwork.GetApproverList = new List<UserProfile>();
                    foreach (DataRow item in dsResult.Tables[0].Rows)
                    {

                        typeofwork.GetApproverList.Add(
                            new UserProfile
                            {
                                UserID = int.Parse(item["UserID"].ToString()),
                                DisplayUserName = item["UserFullName"].ToString()
                            }
                            );
                    }
                    typeofwork.GetApproverList = typeofwork.GetApproverList.Where(y => y.UserID != CurrentUser.UserID).ToList();


                    typeofwork.GetContractorList = new List<UserProfile>();
                    foreach (DataRow item in dsResult.Tables[1].Rows)
                    {

                        typeofwork.GetContractorList.Add(
                            new UserProfile
                            {
                                UserID = int.Parse(item["ContractorID"].ToString()),
                                DisplayUserName = item["CompanyName"].ToString()
                            }
                            );

                    }

                    typeofwork.GetCheckMaster = new List<CheckListMaster>();
                    foreach (DataRow item in dsResult.Tables[2].Rows)
                    {

                        typeofwork.GetCheckMaster.Add(
                            new CheckListMaster
                            {
                                CheckListID = int.Parse(item["ID"].ToString()),
                                ISChecked = int.Parse(item["Checked"].ToString()) > 0 ? true : false,
                                CheckListName = item["Name"].ToString()
                            }
                            );

                    }

                }
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);
            }



            return Json(typeofwork, JsonRequestBehavior.AllowGet);
        }

        public void GetAutoGeneratePermitID(WorkPermit workPermit)
        {

            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    objCon.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "AutoGeneratePermitID";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = objCon;
                    var permitID = new SqlParameter();
                    permitID.ParameterName = "@NO";
                    permitID.Direction = ParameterDirection.Output;
                    permitID.Size = int.MaxValue;
                    cmd.Parameters.Add(permitID);


                    cmd.ExecuteNonQuery();
                    workPermit.PermitNumber = (string)cmd.Parameters["@NO"].Value;

                    //cmd.Parameters.Add("@NO", SqlDbType.Char, 500);
                    //cmd.Parameters["@NO"].Direction = ParameterDirection.Output;
                    //cmd.ExecuteNonQuery();
                    //workPermit.PermitNumber = (string)cmd.Parameters["@NO"].Value;        
                    //Object value = Convert.ToString(cmd.ExecuteScalar());

                    //string a = value.ToString();
                    //workPermit.PermitNumber = a;

                    objCon.Close();
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }

        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetPlantEquipmentSelect(int? PlantID)
        {

            var cs = workPermitBLL.GetPlantEquipmentSelect(PlantID).Select(m => new SelectListItem()
            {
                Value = m.EquipmentID.ToString(),
                Text = m.EquipmentName
            });
            return Json(cs, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        // [AuthorizedUsers("Administrator")]
        public ActionResult CreatePermit()
        {

            var workPermit = new WorkPermit();
            workPermit.CurrentSessionID = CurrentUser.CurrentSessionID;
            workPermit.PrevoiusSessionID = sess.SessionActive;
            if (workPermit.CurrentSessionID == workPermit.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            workPermit.ValidityFrom = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            //workPermit.ValidityTo = DateTime.Today.AddHours(18).ToString("dd/MM/yyyy HH:mm:ss");
            workPermit.ApproverList = new List<UserProfile>();
            //var approverlist = new List<UserProfile>();
            //workPermit.ApproverList = approverlist.Where(x => x.UserID != CurrentUser.UserID).ToList();
            workPermit.PlantList = adminBLL.DivisionDD();

            workPermit.MechancialDept = departlist.MechancialDept;
            workPermit.ElectricalDept = departlist.ElectricalDept;
            workPermit.SafetyDept = departlist.SafetyDept;
            workPermit.ProcessManagerDept = departlist.ProcessManagerDept;
            workPermit.GMDept = departlist.GMDept;
            workPermit.InstrumentationDept = departlist.InstrumentationDept;
            workPermit.ContractorList = new List<ContractorMaster>();
            workPermit.ChecklistMaster = workPermitBLL.GetCheckList();
            workPermit.GeneralList = workPermitBLL.GetGeneralCheckList();
            workPermit.EquipmentList = GetequipmentList();
            workPermit.Standbyperson = departlist.Standbyperson;
            workPermit.DepartmentList = departlist.DepartmentList;
            workPermit.PPE = workPermitBLL.GetPPE();
            workPermit.WorkType = workPermitBLL.GetWorkType();
            workPermit.Roles = CurrentUser.Roles;

            workPermit.UserFullName = CurrentUser.FullName;
            workPermit.ProfileImage = CurrentUser.ProfileImage;
            workPermit.PermitIssuerName = CurrentUser.FullName;
            workPermit.PermitIssuerID = CurrentUser.UserID;
            workPermit.IsRestrict = CurrentUser.IsRestrict;
            return View(workPermit);
        }


        [HttpPost]
        public ActionResult CreatePermit(WorkPermit workPermit, int[] work, int[] AllCheckList)
        {
            workPermit.PlantList = adminBLL.DivisionDD();
            Approverlist app = new Approverlist();
            int affectedRecordCount;
            GetAutoGeneratePermitID(workPermit);
            if (Request.Form["GetEquipmentList"] == "1")
            {
                workPermit.WorkType = workPermitBLL.GetWorkType();
            }
            else
            {
                using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "WorkPermitInsert";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@ValidityFrom", DateTime.ParseExact(workPermit.ValidityFrom, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture));
                    objCom.Parameters.AddWithValue("@ValidityTo", DateTime.ParseExact(workPermit.ValidityTo, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture));
                    objCom.Parameters.AddWithValue("@Location", workPermit.Location);
                    objCom.Parameters.AddWithValue("@PlantID", workPermit.PlantID);
                    objCom.Parameters.AddWithValue("@EquipmentID", workPermit.EquipmentID);
                    objCom.Parameters.AddWithValue("@ContractorID", workPermit.ContractorID);
                    objCom.Parameters.AddWithValue("@WorkDesctiption", workPermit.WorkDescription);
                    //objCom.Parameters.AddWithValue("@WorkTypeID", workPermit.WorkTypeID);
                    //objCom.Parameters.AddWithValue("@FireWatchRequired", workPermit.FireWatchRequired == true ? 1 : 0);
                    objCom.Parameters.AddWithValue("@RiskAssessmentRequired", workPermit.Risk);
                    objCom.Parameters.AddWithValue("@FireWatchRequired", workPermit.PersonID);
                    objCom.Parameters.AddWithValue("@PermitHolderName", workPermit.PermitHolderName);
                    objCom.Parameters.AddWithValue("@NoOfPersonAtSite", workPermit.NoOfPersonAtSite);
                    objCom.Parameters.AddWithValue("@PermitIssuerID", workPermit.PermitIssuerID);
                    objCom.Parameters.AddWithValue("@ApproverID", workPermit.ApproverID);
                    objCom.Parameters.AddWithValue("@AdjacentAreaOwenerID", workPermit.AdjacentAreaOwenerID);
                    //  objCom.Parameters.AddWithValue("@Attachment", workPermit.Attachment);
                    objCom.Parameters.AddWithValue("@CreatedBy", CurrentUser.UserID);
                    objCom.Parameters.AddWithValue("@Status", workPermit.Status);
                    objCom.Parameters.AddWithValue("@PPEOthers", workPermit.PPEOthers);
                    objCom.Parameters.AddWithValue("@JobRequestID", workPermit.JobID);
                    objCom.Parameters.AddWithValue("@MechanicalIncharge", workPermit.MechanicalIncharge);
                    objCom.Parameters.AddWithValue("@ElectricalIncharge", workPermit.ElectricalIncharge);
                    objCom.Parameters.AddWithValue("@InstrumentalIncharge", workPermit.InstrumentalIncharge);
                    objCom.Parameters.AddWithValue("@SafetyOfficer", workPermit.SafetyOfficer);
                    objCom.Parameters.AddWithValue("@ProcessManager", workPermit.ProcessManager);
                    objCom.Parameters.AddWithValue("@GMOperations", workPermit.GMOperations);
                    objCom.Parameters.AddWithValue("@ContractorEmpID", workPermit.ContractorEmpID);
                    objCom.Parameters.AddWithValue("@DepartID", workPermit.DepartID);
                    objCom.Parameters.AddWithValue("@AutoPermitNO", workPermit.PermitNumber);

                    var permitID = new SqlParameter();
                    permitID.ParameterName = "@PermitNo";
                    permitID.Direction = ParameterDirection.Output;
                    permitID.Size = int.MaxValue;
                    objCom.Parameters.Add(permitID);
                    objCon.Open();
                    objCom.Connection = objCon;
                    affectedRecordCount = objCom.ExecuteNonQuery();


                    objCom.Parameters.Clear();
                    workPermit.WorkPermitID = Convert.ToInt32(permitID.Value);
                    var ppe = workPermitBLL.PPEInsert(workPermit);
                    var workype = workPermitBLL.WorkTypeInsert(workPermit, work);
                    var genr = workPermitBLL.GeneralChecklistInsert(workPermit);
                    var chklist = workPermitBLL.AllCheckListInsert(workPermit, AllCheckList);
                    objCon.Close();

                    if (affectedRecordCount > 0)
                    {
                        TempData["Message"] = string.Format("Work Permit ID _{0} is created successfully. Please review again and submit for approval", workPermit.PermitNumber);
                    }

                }


            }
            workPermit.Roles = CurrentUser.Roles;
            workPermit.UserFullName = CurrentUser.FullName;
            workPermit.ProfileImage = CurrentUser.ProfileImage;
            workPermit.PermitIssuerName = CurrentUser.FullName;
            workPermit.PermitIssuerID = CurrentUser.UserID;
            workPermit.IsRestrict = CurrentUser.IsRestrict;
            return RedirectToAction("NewPermitList");
        }


        //[AuthorizedUsers("Administrator", "Approver")]
        [HttpGet]
        public ActionResult NewPermitList()
        {

            Console.WriteLine("yesffr");
            WorkPermitList newWorkPermitList = null;

            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "NewWorkPermitList";
                objCom.CommandType = CommandType.StoredProcedure;
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();

                var workPermitList = new List<WorkPermit>();
                int recortCount = 1;
                while (objReader.Read())
                {

                    var workPermit = new WorkPermit();
                    workPermit.SNO = recortCount++;
                    workPermit.WorkPermitID = int.Parse(objReader["WorkPermitID"].ToString());
                    workPermit.ValidityFrom = objReader["ValidityFrom"].ToString();
                    workPermit.ValidityTo = objReader["ValidityTo"].ToString();
                    workPermit.PlantName = objReader["PlantName"].ToString();
                     workPermit.WorkDescription = objReader["WorkDesctiption"].ToString();
                    workPermit.DepartmentName = objReader["DepartmentName"].ToString();
                    workPermit.EquipmentName = objReader["EquipmentName"].ToString();
                    workPermit.WorkTypeName = objReader["WorkType"].ToString();
                    workPermit.Status = objReader["Status"].ToString();
                    workPermit.ApproverComment = objReader["ApproverComment"].ToString();
                    workPermit.ApproverName = objReader["Approver"].ToString();
                    workPermit.PermitIssuerID = int.Parse(objReader["PermitIssuerID"].ToString());
                    workPermit.PermitIssuerName = objReader["PermitIssuer"].ToString();
                    workPermit.AdjacentAreaOwenerID = int.Parse(objReader["AdjacentAreaOwenerID"].ToString());
                    workPermit.ApproverID = int.Parse(objReader["ApproverID"].ToString());
                    workPermit.Status = objReader["Status"].ToString();
                    workPermit.ContractorName = objReader["CompanyName"].ToString();
                    workPermit.PermitNumber = objReader["PermitNumber"].ToString();

                    workPermitList.Add(workPermit);
                }
                objCon.Close();

                newWorkPermitList = new WorkPermitList();
                newWorkPermitList.CurrentSessionID = CurrentUser.CurrentSessionID;
                newWorkPermitList.PrevoiusSessionID = sess.SessionActive;
                if (newWorkPermitList.CurrentSessionID == newWorkPermitList.PrevoiusSessionID)
                {

                }
                else
                {
                    ViewBag.SessMessage = "Session Already Exists";

                }
                newWorkPermitList.PlantList = adminBLL.DivisionDD();
                newWorkPermitList.ContractList = workPermitBLL.GetContractorCompany();
                newWorkPermitList.FromDate = DateTime.Now.ToString("dd/MM/yyyy 00:00");
                newWorkPermitList.Todate = DateTime.Now.ToString("dd/MM/yyyy 23:59");
                newWorkPermitList.WorkPermit = workPermitList;
                newWorkPermitList.Roles = CurrentUser.Roles;
                newWorkPermitList.UserID = CurrentUser.UserID;
                newWorkPermitList.UserFullName = CurrentUser.FullName;
                newWorkPermitList.ProfileImage = CurrentUser.ProfileImage;
                newWorkPermitList.IsRestrict = CurrentUser.IsRestrict;
            }


            return View(newWorkPermitList);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewPermitList(WorkPermitList NewPermitList)
        {
            WorkPermitList workPermitList = new WorkPermitList();
            try
            {
                using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                {
                    var objCom = new SqlCommand();
                    objCom.CommandText = "SearchNewPermitList";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@Fromdate", NewPermitList.FromDate);
                    objCom.Parameters.AddWithValue("@Todate", NewPermitList.Todate);
                    if (NewPermitList.ContractorID > 0)
                        objCom.Parameters.AddWithValue("@ContractorID", NewPermitList.ContractorID);
                    else
                        objCom.Parameters.AddWithValue("@ContractorID", DBNull.Value);
                    if (NewPermitList.PlantID > 0)
                        objCom.Parameters.AddWithValue("@PlantID", NewPermitList.PlantID);
                    else
                        objCom.Parameters.AddWithValue("@PlantID", DBNull.Value);
                    objCon.Open();
                    objCom.Connection = objCon;
                    var objReader = objCom.ExecuteReader();

                    var searchlist = new List<WorkPermit>();
                    int recortCount = 1;
                    while (objReader.Read())
                    {

                        var workPermit = new WorkPermit();
                        workPermit.SNO = recortCount++;
                        workPermit.WorkPermitID = int.Parse(objReader["WorkPermitID"].ToString());
                        workPermit.ValidityFrom = objReader["ValidityFrom"].ToString();
                        workPermit.ValidityTo = objReader["ValidityTo"].ToString();
                        workPermit.WorkDescription = objReader["WorkDesctiption"].ToString();
                        workPermit.PlantName = objReader["PlantName"].ToString();
                        workPermit.DepartmentName = objReader["DepartmentName"].ToString();
                        workPermit.EquipmentName = objReader["EquipmentName"].ToString();
                        workPermit.WorkTypeName = objReader["WorkType"].ToString();
                        workPermit.Status = objReader["Status"].ToString();
                        workPermit.ApproverComment = objReader["ApproverComment"].ToString();
                        workPermit.ApproverName = objReader["Approver"].ToString();
                        workPermit.PermitIssuerID = int.Parse(objReader["PermitIssuerID"].ToString());
                        workPermit.PermitIssuerName = objReader["PermitIssuer"].ToString();
                        workPermit.AdjacentAreaOwenerID = int.Parse(objReader["AdjacentAreaOwenerID"].ToString());
                        workPermit.ApproverID = int.Parse(objReader["ApproverID"].ToString());
                        workPermit.Status = objReader["Status"].ToString();
                        workPermit.ContractorName = objReader["CompanyName"].ToString();
                        workPermit.PermitNumber = objReader["PermitNumber"].ToString();
                        searchlist.Add(workPermit);
                        workPermitList.WorkPermit = searchlist;
                    }
                    objCon.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            workPermitList.FromDate = DateTime.Now.ToString("dd/MM/yyyy 00:00");
            workPermitList.Todate = DateTime.Now.ToString("dd/MM/yyyy 23:59");
            workPermitList.ContractList = workPermitBLL.GetContractorCompany();
            workPermitList.PlantList = adminBLL.DivisionDD();
            workPermitList.UserID = CurrentUser.UserID;
            workPermitList.Roles = CurrentUser.Roles;
            workPermitList.UserFullName = CurrentUser.FullName;
            workPermitList.ProfileImage = CurrentUser.ProfileImage;
            workPermitList.IsRestrict = CurrentUser.IsRestrict;

            return View(workPermitList);
        }

        #region AddEquipment

        // [AuthorizedUsers("Supervisor")]
        public ActionResult AddEquipment()
        {
            var adminBLL = new AdminBLL();
            var equipmentInfo = adminBLL.GetEquipmentInfo();
            equipmentInfo.CurrentSessionID = CurrentUser.CurrentSessionID;
            equipmentInfo.PrevoiusSessionID = sess.SessionActive;
            if (equipmentInfo.CurrentSessionID == equipmentInfo.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            equipmentInfo.EquipemntTypeList = adminBLL.GetEquipmentType();
            equipmentInfo.UserID = CurrentUser.UserID;
            equipmentInfo.UserFullName = CurrentUser.FullName;
            equipmentInfo.Roles = CurrentUser.Roles;
            equipmentInfo.ProfileImage = CurrentUser.ProfileImage;
            equipmentInfo.IsRestrict = CurrentUser.IsRestrict;
            return View(equipmentInfo);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[AuthorizedUsers("Supervisor")]
        public ActionResult AddEquipment(EquipmentEntity postedEquipmentEntity)
        {
            try
            {
                ViewBag.DivisionID = postedEquipmentEntity.DivisionID;
                var adminBLL = new AdminBLL();
                if (ModelState.IsValid)
                {
                    postedEquipmentEntity.UserID = CurrentUser.UserID;
                    postedEquipmentEntity.LicenseCount = MonitPro.Security.Security.Instance.LicensedParameterCount();
                    int recordCount = adminBLL.AddEquipment(postedEquipmentEntity);
                    if (recordCount > 0)
                    {
                        ViewBag.IsInsertSuccessful = true;
                        var equipementEntity = adminBLL.GetEquipmentInfo();
                        postedEquipmentEntity.FactoryList = equipementEntity.FactoryList;
                        postedEquipmentEntity.DivisionList = equipementEntity.DivisionList;
                        postedEquipmentEntity.EquipemntTypeList = adminBLL.GetEquipmentType();
                        postedEquipmentEntity.UserID = CurrentUser.UserID;
                        postedEquipmentEntity.UserFullName = CurrentUser.FullName;
                        postedEquipmentEntity.Roles = CurrentUser.Roles;
                        postedEquipmentEntity.ProfileImage = CurrentUser.ProfileImage;
                        postedEquipmentEntity.IsRestrict = CurrentUser.IsRestrict;
                    }
                }

            }
            catch (Exception objException)
            {
                ViewBag.IsValidationFailed = true;
                ModelState.AddModelError("ValidationError", objException.Message);

                var adminBLL = new AdminBLL();
                var equipmentInfo = adminBLL.GetEquipmentInfo();
                postedEquipmentEntity.FactoryList = equipmentInfo.FactoryList;
                postedEquipmentEntity.DivisionList = equipmentInfo.DivisionList;
                postedEquipmentEntity.EquipemntTypeList = adminBLL.GetEquipmentType();
                postedEquipmentEntity.UserID = CurrentUser.UserID;
                postedEquipmentEntity.UserFullName = CurrentUser.FullName;
                postedEquipmentEntity.Roles = CurrentUser.Roles;
                postedEquipmentEntity.ProfileImage = CurrentUser.ProfileImage;
                postedEquipmentEntity.IsRestrict = CurrentUser.IsRestrict;
            }
            return View(postedEquipmentEntity);
        }
        #endregion
        #region ExportData
        public ActionResult ExportEquipmentList()
        {

            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "ExportEquipmentList";
                objCom.CommandType = CommandType.StoredProcedure;

                objCon.Open();
                objCom.Connection = objCon;
                SqlDataAdapter Adapter = new SqlDataAdapter();
                Adapter.SelectCommand = objCom;
                DataSet dataSet = new DataSet();
                Adapter.Fill(dataSet);
                objCon.Close();
                var wb = new XLWorkbook(Server.MapPath("~/Template/EquipmentList.xlsx"));
                var worksheet = wb.Worksheet("EquipmentList");
                worksheet.Cell("C4").Value = "Report Generated by : " + CurrentUser.FullName;
                //worksheet.Cell("D4").Value = CurrentUser.FullName;

                //worksheet.Cell("D5").Value = currentFromDate + " to  " + currentEndDate;

                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    var rangeWithDataSecond = worksheet.Cell(7, 1).InsertTable(dataSet.Tables[0].AsEnumerable());
                }
                else
                {
                    worksheet.Cell("C8").Value = "No Data Found";
                }

                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename= EquipmentList.xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

            }


            return View();


        }
        #endregion ExportData

        #region EquipmentList

        [HttpGet]
        //[AuthorizedUsers("Supervisor")]
        public ActionResult EquipmentList()
        {

            EquipmentList equipmentList = new EquipmentList();
            equipmentList.CurrentSessionID = CurrentUser.CurrentSessionID;
            equipmentList.PrevoiusSessionID = sess.SessionActive;
            var Divisiondd = adminBLL.DivisionDD();
            var Equipmentdd = adminBLL.EquipmentDD();
            ViewBag.EquipmentList = new SelectList(Equipmentdd, "EquipmentID", "EquipmentName");
            ViewBag.DivisionList = new SelectList(Divisiondd, "DivisionID", "DivisionName");
            if (equipmentList.CurrentSessionID == equipmentList.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            equipmentList.EquipmentEntity = adminBLL.SelectEquipmentList();

            SearchEquipmentlist searchequipment = new SearchEquipmentlist();
            ViewBag.Divisions = searchequipment.DivisionID;
            ViewBag.EquipID = searchequipment.EquipID;
            equipmentList.UserFullName = CurrentUser.FullName;
            equipmentList.Roles = CurrentUser.Roles;
            equipmentList.ProfileImage = CurrentUser.ProfileImage;
            equipmentList.IsRestrict = CurrentUser.IsRestrict;
            return View(equipmentList);

        }
        [HttpPost]
        public ActionResult EquipmentList(SearchEquipmentlist searchequipment)
        {
            AdminBLL adminBLL = new AdminBLL();
            EquipmentList equipmentList = new EquipmentList();
            equipmentList.CurrentSessionID = CurrentUser.CurrentSessionID;
            equipmentList.PrevoiusSessionID = sess.SessionActive;
            var Divisiondd = adminBLL.DivisionDD();
            var Equipmentdd = adminBLL.EquipmentDD();
            ViewBag.EquipmentList = new SelectList(Equipmentdd, "EquipmentID", "EquipmentName");
            ViewBag.DivisionList = new SelectList(Divisiondd, "DivisionID", "DivisionName");

            if (equipmentList.CurrentSessionID == equipmentList.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            equipmentList.EquipmentEntity = adminBLL.SearchEquipmentlist(searchequipment);
            ViewBag.Divisions = searchequipment.DivisionID;
            ViewBag.TypeofEquipment = searchequipment.EquipID;
            equipmentList.UserFullName = CurrentUser.FullName;
            equipmentList.Roles = CurrentUser.Roles;
            equipmentList.ProfileImage = CurrentUser.ProfileImage;
            equipmentList.IsRestrict = CurrentUser.IsRestrict;
            return View(equipmentList);

        }
        [HttpGet]
        //[AuthorizedUsers("Supervisor")]
        public ActionResult UpdateEquipment(int id)
        {
            EquipmentEntity equipmentEntity = null;

            AdminBLL adminBLL = new AdminBLL();
            equipmentEntity = adminBLL.GetEquipment(id);
            equipmentEntity.EquipemntTypeList = adminBLL.GetEquipmentType();
            equipmentEntity.UserID = CurrentUser.UserID;
            equipmentEntity.UserFullName = CurrentUser.FullName;
            equipmentEntity.Roles = CurrentUser.Roles;
            equipmentEntity.ProfileImage = CurrentUser.ProfileImage;
            equipmentEntity.IsRestrict = CurrentUser.IsRestrict;
            equipmentEntity.CurrentSessionID = CurrentUser.CurrentSessionID;
            equipmentEntity.PrevoiusSessionID = sess.SessionActive;
            if (equipmentEntity.CurrentSessionID == equipmentEntity.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            return View(equipmentEntity);
        }
        [HttpPost]
        //[AuthorizedUsers("Supervisor")]
        public ActionResult UpdateEquipment(EquipmentEntity equipmentEntity)
        {
            try
            {
                AdminBLL adminBLL = new AdminBLL();
                adminBLL.UpdateEquipment(equipmentEntity);
                equipmentEntity.EquipemntTypeList = adminBLL.GetEquipmentType();
                return RedirectToAction("EquipmentList");
            }
            catch (Exception exception)
            {
                ViewBag.IsValidationFailed = true;
                ModelState.AddModelError("ValidationError", exception.Message);
                var adminBLL = new AdminBLL();
                var equipmentInfo = adminBLL.GetEquipmentInfo();
                equipmentEntity.FactoryList = equipmentInfo.FactoryList;
                equipmentEntity.DivisionList = equipmentInfo.DivisionList;
                equipmentEntity.EquipemntTypeList = adminBLL.GetEquipmentType();
                equipmentEntity.UserID = CurrentUser.UserID;
                equipmentEntity.UserFullName = CurrentUser.FullName;
                equipmentEntity.Roles = CurrentUser.Roles;
                equipmentEntity.ProfileImage = CurrentUser.ProfileImage;
                equipmentEntity.IsRestrict = CurrentUser.IsRestrict;

            }

            return View(equipmentEntity);
        }


        #endregion

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetDivisions(int? factoryID)
        {
            AdminBLL adminBLL = new AdminBLL();

            var divisions = adminBLL.GetDivisions(factoryID).Select(m => new SelectListItem()
            {
                Value = m.DivisionID.ToString(),
                Text = m.DivisionName
            });
            return Json(divisions, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]

        public int CheckValidApprover(int id)
        {
            WorkPermit workPermit = new WorkPermit();
            var approvermaster = new List<UserProfile>();

            using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
            {

                SqlCommand objCom = new SqlCommand();
                objCom.CommandText = "GetApproversSelect";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@WorkTypeID", id);

                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();




                while (objReader.Read())
                {
                    var approver = new UserProfile();
                    approver.UserID = int.Parse(objReader["UserID"].ToString());

                    approvermaster.Add(approver);


                }
                objCon.Close();
            }

            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "[WorkPermitGet]";
                objCom.Parameters.AddWithValue("@WorkPermitID", id);
                objCom.CommandType = CommandType.StoredProcedure;
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();


                if (objReader.Read())
                {

                    workPermit.PermitIssuerID = int.Parse(objReader["PermitIssuerID"].ToString());

                    //workPermit.Attachment = objReader["Attachment"].ToString();
                    workPermit.Attachment = objReader["Attachement"].ToString();


                }
                objCon.Close();
            }





            for (int i = 0; i < approvermaster.Count; i++)
            {

                if (approvermaster[i].UserID == CurrentUser.UserID && workPermit.PermitIssuerID != CurrentUser.UserID)
                {
                    return 1;
                }

            }

            return 0;

        }

        [HttpGet]
        public ActionResult UpdatePermit(int id)
        {

            var workPermit = workPermitBLL.GetWorkPermit(id);
            workPermit.CurrentSessionID = CurrentUser.CurrentSessionID;
            workPermit.PrevoiusSessionID = sess.SessionActive;
            if (workPermit.CurrentSessionID == workPermit.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            workPermit.ApproverList = workPermitBLL.GetApproversSelect(id);
            workPermit.WorkType = workPermitBLL.GetWorkType(id);
            workPermit.PPE = workPermitBLL.GetPPE(id);
            workPermit.MechancialDept = departlist.MechancialDept;
            workPermit.ElectricalDept = departlist.ElectricalDept;
            workPermit.SafetyDept = departlist.SafetyDept;
            workPermit.ProcessManagerDept = departlist.ProcessManagerDept;
            workPermit.GMDept = departlist.GMDept;
            workPermit.InstrumentationDept = departlist.InstrumentationDept;
            workPermit.GeneralList = workPermitBLL.GetGeneralCheckList(id);
            workPermit.ChecklistMaster = workPermitBLL.GetCheckList(id);
            workPermit.ContractorList = workPermitBLL.GetContratorsSelect(id);
            workPermit.Standbyperson = departlist.Standbyperson;
            workPermit.DepartmentList = departlist.DepartmentList;
            workPermit.checkvalidapprover = CheckValidApprover(id);
            workPermit.PlantList = adminBLL.DivisionDD();
            workPermit.EquipmentList = GetequipmentList();
            workPermit.Roles = CurrentUser.Roles;
            workPermit.UserFullName = CurrentUser.FullName;
            workPermit.ProfileImage = CurrentUser.ProfileImage;
            workPermit.IsRestrict = CurrentUser.IsRestrict;
            return View(workPermit);
        }

        private int Update(WorkPermit workPermit, int[] work, int[] AllCheckList)
        {
            int affectedcount = 0;
            try
            {

                using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                {
                    var objCom = new SqlCommand();
                    objCom.CommandText = "WorkPermitUpdate";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@WorkPermitID", workPermit.WorkPermitID);
                    objCom.Parameters.AddWithValue("@ValidityFrom", DateTime.ParseExact(workPermit.ValidityFrom, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture));
                    objCom.Parameters.AddWithValue("@ValidityTo", DateTime.ParseExact(workPermit.ValidityTo, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture));
                    objCom.Parameters.AddWithValue("@Location", workPermit.Location);
                    objCom.Parameters.AddWithValue("@PlantID", workPermit.PlantID);
                    objCom.Parameters.AddWithValue("@EquipmentID", workPermit.EquipmentID);
                    objCom.Parameters.AddWithValue("@ContractorID", workPermit.ContractorID);
                    objCom.Parameters.AddWithValue("@WorkDesctiption", workPermit.WorkDescription);
                    objCom.Parameters.AddWithValue("@RiskAssessmentRequired", workPermit.Risk);
                    objCom.Parameters.AddWithValue("@FireWatchRequired", workPermit.PersonID);
                    objCom.Parameters.AddWithValue("@PermitHolderName", workPermit.PermitHolderName);
                    objCom.Parameters.AddWithValue("@NoOfPersonAtSite", workPermit.NoOfPersonAtSite);
                    objCom.Parameters.AddWithValue("@PermitIssuerID", workPermit.PermitIssuerID);
                    objCom.Parameters.AddWithValue("@AdjacentAreaOwenerID", workPermit.AdjacentAreaOwenerID);
                    objCom.Parameters.AddWithValue("@ApproverID", workPermit.ApproverID);
                    objCom.Parameters.AddWithValue("@LastModifiedBy", CurrentUser.UserID);
                    objCom.Parameters.AddWithValue("@Status", workPermit.Status);
                    objCom.Parameters.AddWithValue("@PPEOthers", workPermit.PPEOthers);
                    objCom.Parameters.AddWithValue("@MechanicalIncharge", workPermit.MechanicalIncharge);
                    objCom.Parameters.AddWithValue("@ElectricalIncharge", workPermit.ElectricalIncharge);
                    objCom.Parameters.AddWithValue("@InstrumentalIncharge", workPermit.InstrumentalIncharge);
                    objCom.Parameters.AddWithValue("@SafetyOfficer", workPermit.SafetyOfficer);
                    objCom.Parameters.AddWithValue("@ProcessManager", workPermit.ProcessManager);
                    objCom.Parameters.AddWithValue("@GMOperations", workPermit.GMOperations);
                    objCom.Parameters.AddWithValue("@JobRequestID", workPermit.JobID);
                    objCom.Parameters.AddWithValue("@ContractorEmpID", workPermit.ContractorEmpID);
                    objCom.Parameters.AddWithValue("@DepartID", workPermit.DepartID);
                    if (workPermit.Status == "A")
                    {
                        objCom.Parameters.AddWithValue("@ApprovedBy", workPermit.ApproverID);
                        objCom.Parameters.AddWithValue("@ApproverComment", workPermit.ApproverComment);
                    }
                    objCom.Parameters.AddWithValue("@closedComment", workPermit.ClosureComment);
                    objCon.Open();
                    objCom.Connection = objCon;
                    affectedcount = objCom.ExecuteNonQuery();

                    objCom.Parameters.Clear();

                    objCon.Close();

                }
            }
            catch (Exception objException)
            {
                // throw new Exception(objException.Message);
                ViewBag.IsValidationFailed = true;
                ModelState.AddModelError("ValidationError", objException.Message);
            }
            return affectedcount;
        }

        [HttpPost]
        public ActionResult UpdatePermit(WorkPermit workPermit, int[] work, int[] AllCheckList)
        {
            try
            {

                int workpermitid = Update(workPermit, work, AllCheckList);

                var ppe = workPermitBLL.PPEInsert(workPermit);
                var worktype = workPermitBLL.WorkTypeInsert(workPermit, work);
                var chklist = workPermitBLL.AllCheckListInsert(workPermit, AllCheckList);
                var genr = workPermitBLL.GeneralChecklistInsert(workPermit);
                workPermit.MechancialDept = departlist.MechancialDept;
                workPermit.ElectricalDept = departlist.ElectricalDept;
                workPermit.SafetyDept = departlist.SafetyDept;
                workPermit.ProcessManagerDept = departlist.ProcessManagerDept;
                workPermit.GMDept = departlist.GMDept;
                workPermit.ContractorList = workPermitBLL.GetContratorsSelect(workPermit.WorkPermitID);
                workPermit.Standbyperson = departlist.Standbyperson;
                workPermit.DepartmentList = departlist.DepartmentList;
                workPermit.ChecklistMaster = workPermitBLL.GetCheckList(workPermit.WorkPermitID);
                workPermit.InstrumentationDept = departlist.InstrumentationDept;
                workPermit.GeneralList = workPermitBLL.GetGeneralCheckList(workPermit.WorkPermitID);
                workPermit.ApproverList = workPermitBLL.GetApproversSelect(workPermit.WorkPermitID);
                workPermit.WorkType = workPermitBLL.GetWorkType(workPermit.WorkPermitID);
                workPermit.PPE = workPermitBLL.GetPPE(workPermit.WorkPermitID);
                workPermit.checkvalidapprover = CheckValidApprover(workPermit.WorkPermitID);
                workPermit.PlantList = adminBLL.DivisionDD();
                workPermit.EquipmentList = GetequipmentList();
                var permit = workPermitBLL.GetWorkPermit(workPermit.WorkPermitID);


                if (workPermit.Status == "S")
                {

                    var userListt = new AdminDA().SelectUserProfile();
                    var list = userListt.Where(y => y.UserID == workPermit.PermitIssuerID).ToList();
                    workPermit.ApproverList = userListt.Where(y => y.UserID == workPermit.ApproverID).ToList();

                    var cc = userListt.Where(y => y.UserID == workPermit.PermitIssuerID).ToList();



                    string ToAddress;
                    string CCAddress;

                    ToAddress = workPermit.ApproverList[0].EmailAddress;
                    CCAddress = list[0].EmailAddress;


                    string Subject = "Work Permit#" + permit.PermitNumber + "-Submitted for approval".ToString(); //selectWorkTypelist[0].WorkType.ToString();
                    string Sender = CurrentUser.FullName;


                    string Statement = "Type of Work:-" + permit.WorkTypeName + "\n" +
                                        "Validity Date From:-" + permit.ValidityFrom + "& To:" + permit.ValidityFrom + "\n" +
                                        "Equipment Area:-" + permit.EquipmentName + "\n" +
                                        "Exact Location:-" + permit.Location + "\n" +
                                        "Work Description:-" + permit.WorkDescription + "\n\n\n" + "Work Permit#" + permit.PermitNumber + "-has been Submitted for Approval".ToString();



                    mail(ToAddress, CCAddress, Sender, Subject, Statement);

                }

                if (workpermitid > 0 && workPermit.Status == "D" && ViewBag.IsValidationFailed != true)
                {
                    TempData["Message"] = string.Format("Work Permit ID_ {0} is updated successfully", permit.PermitNumber);
                }
                else if (workpermitid > 0 && workPermit.Status == "T")
                {
                    TempData["Message"] = string.Format("Work Permit ID_ {0} is cancelled successfully", permit.PermitNumber);
                }
                else if (workpermitid > 0 && workPermit.Status == "S")
                {
                    TempData["Message"] = string.Format("Work Permit ID_ {0} is submitted successfully", permit.PermitNumber);
                }
                workPermit.PermitNumber = permit.PermitNumber;
                workPermit.PermitIssuerName = permit.PermitIssuerName;
                workPermit.Roles = CurrentUser.Roles;
                workPermit.UserFullName = CurrentUser.FullName;
                workPermit.ProfileImage = CurrentUser.ProfileImage;
                workPermit.IsRestrict = CurrentUser.IsRestrict;
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);

            }
            if (ViewBag.IsValidationFailed != true)
            {
                return RedirectToAction("NewPermitList");
            }
            else
            {
                workPermit.Status = "D";
                return View(workPermit);
            }


        }





        [HttpGet]
        public ActionResult ApprovePermit(int id)
        {
            var workPermit = workPermitBLL.GetWorkPermit(id);
            workPermit.CurrentSessionID = CurrentUser.CurrentSessionID;
            workPermit.PrevoiusSessionID = sess.SessionActive;
            if (workPermit.CurrentSessionID == workPermit.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            workPermit.ApproverList = workPermitBLL.GetApproversSelect(id);
            workPermit.MechancialDept = departlist.MechancialDept;
            workPermit.ElectricalDept = departlist.ElectricalDept;
            workPermit.SafetyDept = departlist.SafetyDept;
            workPermit.ProcessManagerDept = departlist.ProcessManagerDept;
            workPermit.GMDept = departlist.GMDept;
            workPermit.InstrumentationDept = departlist.InstrumentationDept;
            workPermit.ContractorList = workPermitBLL.GetContratorsSelect(id);
            workPermit.EquipmentList = GetequipmentList();
            workPermit.GeneralList = workPermitBLL.GetGeneralCheckList(id);
            workPermit.ChecklistMaster = workPermitBLL.GetCheckList(id);
            workPermit.checkvalidapprover = CheckValidApprover(id);
            workPermit.DepartmentList = departlist.DepartmentList;
            workPermit.Standbyperson = departlist.Standbyperson;
            workPermit.WorkType = workPermitBLL.GetWorkType(id);
            workPermit.PlantList = adminBLL.DivisionDD();
            workPermit.PPE = workPermitBLL.GetPPE(id);
            workPermit.Roles = CurrentUser.Roles;
            workPermit.UserFullName = CurrentUser.FullName;
            workPermit.ProfileImage = CurrentUser.ProfileImage;
            workPermit.UserID = CurrentUser.UserID;
            workPermit.IsRestrict = CurrentUser.IsRestrict;
            return View(workPermit);
        }


        [HttpPost]
        public ActionResult ApprovePermit(WorkPermit workPermit, int[] AllCheckList)
        {
            try
            {
                using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                {
                    int workpermitid = Update(workPermit, null, null);

                    var ppe = workPermitBLL.PPEInsert(workPermit);
                    var genr = workPermitBLL.GeneralChecklistInsert(workPermit);
                    var chklist = workPermitBLL.AllCheckListInsert(workPermit, AllCheckList);

                    objCon.Close();

                    var permit = workPermitBLL.GetWorkPermit(workPermit.WorkPermitID);
                    if (workPermit.Status == "A")
                    {

                        var approvers = workPermitBLL.GetAllApprovers(workPermit.WorkPermitID);
                        workPermit.WorkType = workPermitBLL.GetWorkType();
                        var selectWorkTypelist = workPermit.WorkType.Where(y => y.WorkTypeID == workPermit.WorkTypeID).ToList();
                        var userListt = new AdminDA().SelectUserProfile();
                        var list = userListt.Where(y => y.UserID == workPermit.PermitIssuerID).ToList();
                        var cc = userListt.Where(y => y.UserID == workPermit.PermitIssuerID).ToList();
                        string ToAddress;
                        string CCAddress;

                        ToAddress = approvers.MailApprover;
                        //ToAddress = approvers.MailApprover +  approvers.MailSafetyOfficier
                        //           + approvers.MailProcessManager +  approvers.MailElectrical
                        //           + approvers.Mailmechanical +  approvers.Mailinstrument +  approvers.Mailgmoperations;
                        CCAddress = list[0].EmailAddress;

                        string Subject = "Work Permit#" + permit.PermitNumber + "-Approved".ToString(); //selectWorkTypelist[0].WorkType.ToString();
                        string Sender = CurrentUser.FullName;

                        string Statement = "Type of Work:-" + permit.WorkTypeName + "\n" +
                                            "Validity Date From:-" + permit.ValidityFrom + "& To:" + permit.ValidityFrom + "\n" +
                                            "Equipmet Area:-" + permit.EquipmentName + "\n" +
                                            "Exact Location:-" + permit.Location + "\n" +
                                            "Work Description:-" + permit.WorkDescription + "\n\n\n" + "Work Permit#" + permit.PermitNumber + "-has been Approved".ToString();



                        mail(CCAddress, ToAddress, Sender, Subject, Statement);
                    }
                    else if (workPermit.Status == "R")
                    {
                        workPermit.WorkType = workPermitBLL.GetWorkType();
                        var approvers = workPermitBLL.GetAllApprovers(workPermit.WorkPermitID);
                        var selectWorkTypelist = workPermit.WorkType.Where(y => y.WorkTypeID == workPermit.WorkTypeID).ToList();
                        var userListt = new AdminDA().SelectUserProfile();
                        var list = userListt.Where(y => y.UserID == workPermit.PermitIssuerID).ToList();
                        string CCAddress;
                        string ToAddress;

                        ToAddress = approvers.MailApprover;
                        CCAddress = list[0].EmailAddress;


                        string Subject = "Work Permit#" + permit.PermitNumber + "-Rejected".ToString(); // selectWorkTypelist[0].WorkType.ToString();
                        string Sender = CurrentUser.FullName;
                        string Statement = "Rejected".ToString();
                        mail(CCAddress, ToAddress, Sender, Subject, Statement);
                    }



                    if (workpermitid > 0 && workPermit.Status == "A")
                    {
                        TempData["Message"] = string.Format("Work Permit ID_ {0} is approved successfully", permit.PermitNumber);
                    }
                    else if (workpermitid > 0 && workPermit.Status == "T")
                    {
                        TempData["Message"] = string.Format("Work Permit ID_ {0} is cancelled successfully", permit.PermitNumber);
                    }
                    else
                    {
                        TempData["Message"] = string.Format("Work Permit ID_ {0} is recycled successfully", permit.PermitNumber);
                    }
                }
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);

            }

            return RedirectToAction("NewPermitList");
        }

        [HttpGet]
        public ActionResult ApprovedPermitList()
        {
            WorkPermitList pendingApprovalList = null;
       

            pendingApprovalList = WorkPermitList();
            pendingApprovalList.CurrentSessionID = CurrentUser.CurrentSessionID;
            pendingApprovalList.PrevoiusSessionID = sess.SessionActive;
            if (pendingApprovalList.CurrentSessionID == pendingApprovalList.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            //pendingApprovalList.FromDate = DateTime.Now.ToString("dd/MM/yyyy 00:00");
            pendingApprovalList.Todate = DateTime.Now.ToString("dd/MM/yyyy 23:59");
            pendingApprovalList.ContractList = workPermitBLL.GetContractorCompany();
            pendingApprovalList.EquipmentList = GetequipmentList();
            pendingApprovalList.PlantList = adminBLL.DivisionDD();
            pendingApprovalList.Roles = CurrentUser.Roles;
            pendingApprovalList.UserFullName = CurrentUser.FullName;
            pendingApprovalList.ProfileImage = CurrentUser.ProfileImage;
            pendingApprovalList.IsRestrict = CurrentUser.IsRestrict;

            return View(pendingApprovalList);

        }
        public WorkPermitList WorkPermitList()
        {
            WorkPermitList pendingApprovalList = new WorkPermitList();
            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "ApprovedPermitList";
                objCom.CommandType = CommandType.StoredProcedure;
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();

                var workPermitList = new List<WorkPermit>();
                int recortCount = 1;
                while (objReader.Read())
                {
                    var workPermit = new WorkPermit();
                    workPermit.SNO = recortCount++;
                    workPermit.WorkPermitID = int.Parse(objReader["WorkPermitID"].ToString());
                    workPermit.GetStatus = objReader["GetStatus"].ToString();
                    workPermit.WorkDescription = objReader["WorkDesctiption"].ToString();
                    workPermit.PlantName = objReader["PlantName"].ToString();
                    workPermit.DepartmentName = objReader["DepartmentName"].ToString();
                    workPermit.EquipmentName = objReader["EquipmentName"].ToString();
                    workPermit.WorkTypeName = objReader["WorkType"].ToString();
                    workPermit.ValidityFrom = objReader["ValidityFrom"].ToString();
                    workPermit.ValidityTo = objReader["ValidityTo"].ToString();
                    workPermit.PermitHolderName = objReader["PermitHolderName"].ToString();
                    workPermit.ApproverName = objReader["ApproverName"].ToString();
                    workPermit.ApproverComment = objReader["ApproverComment"].ToString();
                    workPermit.PermitIssuerName = objReader["PermitIssuerName"].ToString();
                    workPermit.Attachment = objReader["Attachment"].ToString();
                    workPermit.ContractorName = objReader["CompanyName"].ToString();
                    workPermit.PermitNumber = objReader["PermitNumber"].ToString();
                    workPermit.ExtensionDetails = workPermitBLL.GetExtensionList(int.Parse(objReader["WorkPermitID"].ToString()));
                    workPermitList.Add(workPermit);
                }
                objCon.Close();

                pendingApprovalList = new WorkPermitList();
                pendingApprovalList.WorkPermit = workPermitList;
                return pendingApprovalList;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApprovedPermitList(WorkPermitList pendingApprovalList)
        {
            AdminBLL adminBLL = new AdminBLL();
            string SNO = Request["Sno"];
            if (SNO != "")
            {
                try
                {


                   var workPermit = pendingApprovalList.WorkPermit.Find(x => x.SNO == int.Parse(SNO));

                   var Attachment = Request.Files[int.Parse(SNO) - 1] as HttpPostedFileBase;

                    var fileName = Path.GetFileName(Attachment.FileName);
                    if (Attachment != null && Attachment.ContentLength > 0)
                    {

                        pendingApprovalList.Attachment = Attachment.FileName;
                        var path = Path.Combine(Server.MapPath("~/ClosedPermitAttachment/"), fileName);
                        Attachment.SaveAs(path);

                        using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                        {
                            SqlCommand objCom = new SqlCommand();
                            objCom.CommandText = "ApproveListAttachements";
                            objCom.CommandType = CommandType.StoredProcedure;
                            objCom.Parameters.AddWithValue("@WorkPermitId", (object)workPermit.WorkPermitID);
                            objCom.Parameters.AddWithValue("@Attachement ", fileName);
                            objCom.Parameters.AddWithValue("@UserId ", CurrentUser.UserID);
                            objCon.Open();
                            objCom.Connection = objCon;
                            objCom.ExecuteNonQuery();
                            objCon.Close();

                            ViewBag.issave = "Y";

                        }
                    }

                }

                catch (Exception objException)
                {
                    throw new Exception(objException.Message);

                }
                pendingApprovalList = WorkPermitList();
            }
            else
            {
                pendingApprovalList = workPermitBLL.SearchApprovedList(pendingApprovalList);
            }
            pendingApprovalList.PlantList = adminBLL.DivisionDD();
            pendingApprovalList.ContractList = workPermitBLL.GetContractorCompany();
            pendingApprovalList.EquipmentList = GetequipmentList();
            // pendingApprovalList.FromDate = DateTime.Now.ToString("dd/MM/yyyy 00:00");
            pendingApprovalList.Todate = DateTime.Now.ToString("dd/MM/yyyy 23:59");
            pendingApprovalList.RoleList = adminBLL.GetRollList();
            pendingApprovalList.UserID = CurrentUser.UserID;
            pendingApprovalList.Roles = CurrentUser.Roles;
            pendingApprovalList.UserFullName = CurrentUser.FullName;
            pendingApprovalList.ProfileImage = CurrentUser.ProfileImage;
            pendingApprovalList.IsRestrict = CurrentUser.IsRestrict;
            //return RedirectToAction("ApprovedPermitList");
            return View(pendingApprovalList);

        }


        [HttpGet]
        public ActionResult ExtensionSave(int id)
        {


            var workPermit = workPermitBLL.GetExtension(id);
            workPermit.CurrentSessionID = CurrentUser.CurrentSessionID;
            workPermit.PrevoiusSessionID = sess.SessionActive;
            if (workPermit.CurrentSessionID == workPermit.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }

            var userList = workPermitBLL.GetAreaOwner();
            //var approverList = userList.Where(x => x.RoleName == "Approver").ToList();
            var approverList = workPermitBLL.GetApproversSelect(id);
            workPermit.ExtensionFrom = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            var areaOwner = userList.Where(y => y.UserID != CurrentUser.UserID).ToList();
            areaOwner.Insert(0, new UserProfile { UserID = -1, DisplayUserName = "N/A" });
            userList.Insert(0, new UserProfile { UserID = -1, DisplayUserName = "N/A" });
            var approverListt = approverList.Where(y => y.UserID != CurrentUser.UserID).ToList();
            workPermit.ApproverList = workPermitBLL.GetApproversSelect(id);
            workPermit.ExtensionPermitApproverList = approverListt;
            workPermit.ExtensionAreaOwnerList = areaOwner;
            workPermit.AdjacentAreaOwnerList = userList;
            workPermit.MechancialDept = departlist.MechancialDept;
            workPermit.ElectricalDept = departlist.ElectricalDept;
            workPermit.SafetyDept = departlist.SafetyDept;
            workPermit.ProcessManagerDept = departlist.ProcessManagerDept;
            workPermit.GMDept = departlist.GMDept;
            workPermit.InstrumentationDept = departlist.InstrumentationDept;
            workPermit.ExtensionPermitIssuerID = CurrentUser.UserID;
            workPermit.ExtensionIssuerName = CurrentUser.FullName;
            workPermit.GeneralList = workPermitBLL.GetGeneralCheckList(id);
            workPermit.ChecklistMaster = workPermitBLL.GetCheckList(id);
            workPermit.WorkType = workPermitBLL.GetWorkType(id);
            workPermit.PPE = workPermitBLL.GetPPE(id);
            workPermit.UserList = userList;
            workPermit.ContractorList = workPermitBLL.GetContratorsSelect(id);
            workPermit.EquipmentList = GetequipmentList();
            workPermit.DepartmentList = departlist.DepartmentList;
            workPermit.Standbyperson = departlist.Standbyperson;
            workPermit.Roles = CurrentUser.Roles;
            workPermit.UserFullName = CurrentUser.FullName;
            workPermit.ProfileImage = CurrentUser.ProfileImage;
            workPermit.IsRestrict = CurrentUser.IsRestrict;
            workPermit.checkvalidapprover = CheckValidApprover(id);
            return View(workPermit);
        }
        [HttpPost]
        public ActionResult ExtensionSave(WorkPermit workPermit)
        {

            try
            {

                using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "ExtensionSave";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@WorkPermitID", workPermit.WorkPermitID);
                    objCom.Parameters.AddWithValue("@ExtensionFrom", DateTime.ParseExact(workPermit.ExtensionFrom, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture));
                    objCom.Parameters.AddWithValue("@ExtensionTo", DateTime.ParseExact(workPermit.ExtensionTo, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture));
                    objCom.Parameters.AddWithValue("@ExtensionPermitIssuerID", workPermit.ExtensionPermitIssuerID);
                    objCom.Parameters.AddWithValue("@ExtensionPermitHolder", workPermit.ExtensionPermitHolder);
                    objCom.Parameters.AddWithValue("@ExtensionAreaOwnerID", workPermit.ExtensionAreaOwnerID);
                    objCom.Parameters.AddWithValue("@ExtensionApproverID", workPermit.ExtensionApproverID);
                    objCom.Parameters.AddWithValue("@Status", workPermit.Status);
                    objCon.Open();
                    objCom.Connection = objCon;
                    objCom.ExecuteNonQuery();
                    objCon.Close();
                    if (workPermit.Status == "E")
                    {
                        //workPermit.WorkType = GetWorkType();
                        //   var selectWorkTypelist = workPermit.WorkType.Where(y => y.WorkTypeID == workPermit.WorkTypeID).ToList();
                        var userListt = new AdminDA().SelectUserProfile();
                        var list = userListt.Where(y => y.UserID == workPermit.ExtensionPermitIssuerID).ToList();
                        workPermit.ExtensionPermitApproverList = userListt.Where(y => y.UserID == workPermit.ExtensionApproverID).ToList();


                        string FromAddress = list[0].EmailAddress.ToString();
                        string ToAddress;
                        string CCAddress;


                        ToAddress = workPermit.ExtensionPermitApproverList[0].EmailAddress;
                        CCAddress = list[0].EmailAddress;


                        string Subject = "Work Permit#" + workPermit.PermitNumber + "-".ToString();
                        string Sender = CurrentUser.FullName;
                        string Statement = "Extended".ToString();
                        mail(CCAddress, ToAddress, Sender, Subject, Statement);


                    }



                }

            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);

            }
            return RedirectToAction("ApprovedPermitList");
        }


        [HttpGet]
        public ActionResult ClosePermit(int id)
        {
            var userlistt = workPermitBLL.GetAreaOwner();
            var workPermit = workPermitBLL.GetWorkPermit(id);

            workPermit.CurrentSessionID = CurrentUser.CurrentSessionID;
            workPermit.PrevoiusSessionID = sess.SessionActive;
            if (workPermit.CurrentSessionID == workPermit.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            workPermit.ClosurePermitHolderSignedOn = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            workPermit.ClosureAreaOwnerSignedOn = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            workPermit.ClosurePermitHolderName = workPermit.PermitHolderName;
            var userList = new AdminDA().SelectUserProfile();
            var approverListt = workPermitBLL.GetApproversSelect(id);

            userList.Insert(0, new UserProfile { UserID = -1, DisplayUserName = "N/A" });
            var areaOwner = userlistt.Where(y => y.UserID != CurrentUser.UserID).ToList();
            areaOwner.Insert(0, new UserProfile { UserID = -1, DisplayUserName = "N/A" });

            workPermit.MechancialDept = userList;
            workPermit.ElectricalDept = userList;
            workPermit.SafetyDept = userList;
            workPermit.ProcessManagerDept = userList;
            workPermit.GMDept = userList;
            workPermit.InstrumentationDept = userList;
            workPermit.AdjacentAreaOwnerList = userList;
            workPermit.ApproverList = approverListt;
            workPermit.WorkType = workPermitBLL.GetWorkType(id);
            workPermit.PPE = workPermitBLL.GetPPE(id);
            workPermit.GeneralList = workPermitBLL.GetGeneralCheckList(id);
            workPermit.ChecklistMaster = workPermitBLL.GetCheckList(id);
            workPermit.UserList = areaOwner;
            workPermit.ContractorList = workPermitBLL.GetContratorsSelect(id);
            workPermit.DepartmentList = departlist.DepartmentList;
            workPermit.Standbyperson = departlist.Standbyperson;
            workPermit.EquipmentList = GetequipmentList();
            workPermit.Roles = CurrentUser.Roles;
            workPermit.UserFullName = CurrentUser.FullName;
            workPermit.ProfileImage = CurrentUser.ProfileImage;
            workPermit.checkvalidapprover = CheckValidApprover(id);
            workPermit.ExtensionPermitApproverList = approverListt;
            workPermit.ExtensionAreaOwnerList = areaOwner;
            workPermit.ExtensionPermitIssuerID = CurrentUser.UserID;
            workPermit.ExtensionIssuerName = CurrentUser.FullName;
            workPermit.IsRestrict = CurrentUser.IsRestrict;
            return View(workPermit);
        }

        [HttpPost]
        public ActionResult ClosePermit(WorkPermit workPermit)
        {
            int affectedcount = 0;
            try
            {
                if (workPermit.Status != "E" && workPermit.Status != "A")
                {
                    using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                    {
                        SqlCommand objCom = new SqlCommand();
                        objCom.CommandText = "WorkPermitClose";
                        objCom.CommandType = CommandType.StoredProcedure;
                        objCom.Parameters.AddWithValue("@WorkPermitID", workPermit.WorkPermitID);

                        objCom.Parameters.AddWithValue("@ClosedBy", CurrentUser.UserID);
                        objCom.Parameters.AddWithValue("@ClosurePermitHolderName", workPermit.ClosurePermitHolderName);
                        if (workPermit.ClosurePermitHolderSignedOn != null)
                        {
                            objCom.Parameters.AddWithValue("@ClosurePermitHolderSignedOn", DateTime.ParseExact(workPermit.ClosurePermitHolderSignedOn, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture));
                        }
                        objCom.Parameters.AddWithValue("@ClosureAreaOwnerID", workPermit.ClosureAreaOwnerID);
                        objCom.Parameters.AddWithValue("@ClosureAreaOwnerSignedOn", DateTime.ParseExact(workPermit.ClosureAreaOwnerSignedOn, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture));
                        objCom.Parameters.AddWithValue("@ContractorRating", workPermit.ContractorRating);
                        //objCom.Parameters.AddWithValue("@WholeAttachment", workPermit.WholeAttachment);
                        objCom.Parameters.AddWithValue("@ClosureComment", workPermit.ClosureComment);
                        objCom.Parameters.AddWithValue("@ContractorComment", workPermit.ContractorComment);
                        objCom.Parameters.AddWithValue("@Status", workPermit.Status);
                        objCon.Open();
                        objCom.Connection = objCon;
                        affectedcount = objCom.ExecuteNonQuery();
                        if (affectedcount > 0 && workPermit.Status == "C")
                        {
                            TempData["Message"] = string.Format("Work Permit ID_ {0} is closed successfully", workPermit.PermitNumber);
                        }
                        else
                        {
                            TempData["Message"] = string.Format("Work Permit ID_ {0} is cancelled successfully", workPermit.PermitNumber);
                        }
                        objCon.Close();
                    }

                    var permit = workPermitBLL.GetWorkPermit(workPermit.WorkPermitID);
                    var approvers = workPermitBLL.GetAllApprovers(workPermit.WorkPermitID);
                    if (workPermit.Status == "C")
                    {
                        workPermit.WorkType = workPermitBLL.GetWorkType();

                        var selectWorkTypelist = workPermit.WorkType.Where(y => y.WorkTypeID == workPermit.WorkTypeID).ToList();
                        var userListt = new AdminDA().SelectUserProfile();
                        var list = userListt.Where(y => y.UserID == workPermit.PermitIssuerID).ToList();

                        string CCAddress;
                        string ToAddress;

                        ToAddress = approvers.MailApprover;
                        CCAddress = list[0].EmailAddress;
                        string Subject = "Work Permit#" + permit.PermitNumber + "-Closed".ToString();
                        string Sender = CurrentUser.FullName;

                        string Statement = "Type of Work:-" + permit.WorkTypeName + "\n" +
                                            "Validity Date From:-" + permit.ValidityFrom + "& To:" + permit.ValidityFrom + "\n" +
                                            "EquipmetArea:-" + permit.EquipmentName + "\n" +
                                            "Exact Location:-" + permit.Location + "\n" +
                                            "Work Description:-" + permit.WorkDescription + "\n\n\n" + "Work Permit#" + permit.PermitNumber + "-has been closed. ".ToString();


                        mail(CCAddress, ToAddress, Sender, Subject, Statement);
                    }



                    if (workPermit.Status == "T")
                    {
                        workPermit.WorkType = workPermitBLL.GetWorkType();
                        var selectWorkTypelist = workPermit.WorkType.Where(y => y.WorkTypeID == workPermit.WorkTypeID).ToList();
                        var userListt = new AdminDA().SelectUserProfile();
                        var list = userListt.Where(y => y.UserID == workPermit.PermitIssuerID).ToList();
                        string CCAddress;
                        string ToAddress;

                        ToAddress = approvers.MailApprover;
                        CCAddress = list[0].EmailAddress;
                        string Subject = "Work Permit#" + permit.PermitNumber + "-Cancelled".ToString();
                        string Sender = CurrentUser.FullName;

                        string Statement = "Type of Work:-" + permit.WorkTypeName + "\n" +
                                            "Validity Date From:-" + permit.ValidityFrom + "& To:" + permit.ValidityFrom + "\n" +
                                            "EquipmetArea:-" + permit.EquipmentName + "\n" +
                                            "Exact Location:-" + permit.Location + "\n" +
                                            "Work Description:-" + permit.WorkDescription + "\n\n\n" + "Work Permit#" + permit.PermitNumber + "-has been closed. ".ToString();


                        mail(CCAddress, ToAddress, Sender, Subject, Statement);
                    }

                    if (workPermit.ContractorRating == "R")
                    {
                        workPermit.WorkType = workPermitBLL.GetWorkType();
                        var selectWorkTypelist = workPermit.WorkType.Where(y => y.WorkTypeID == workPermit.WorkTypeID).FirstOrDefault();
                        var userList = new AdminDA().SelectUserProfile();
                        var contractorlist = workPermitBLL.GetContratorsSelect(0);
                        var contract = contractorlist.Where(y => y.ContractorID == workPermit.ContractorID).FirstOrDefault();
                        var list = userList.Where(y => y.UserID == workPermit.PermitIssuerID).ToList();

                        var closurelist = userList.Where(y => y.UserID == CurrentUser.UserID).FirstOrDefault();
                        string FromAddress = string.Empty;

                        if (closurelist != null)
                        {
                            FromAddress = closurelist.EmailAddress;
                        }

                        string ToAddress = string.Empty;
                        string Receiver = string.Empty;
                        if (contract != null)
                        {
                            ToAddress = contract.EmailAddress;
                            Receiver = contract.ContractorName;
                        }
                        string Subject = string.Empty;
                        //if (selectWorkTypelist != null)
                        //{
                        //    Subject = workPermit.WorkPermitID + "-" + selectWorkTypelist.WorkType;
                        //}
                        string Content = "Work Permit#" + permit.PermitNumber.ToString();
                        Subject = " Kothari Work Permit Rating ";
                        string Sender = CurrentUser.FullName;

                        string Statement = "Closed";
                        Contractmail(FromAddress, ToAddress, Sender, Receiver, Subject, Statement, Content);
                    }
                }
                else if(workPermit.Status == "E")
                {
                    using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                    {
                        SqlCommand objCom = new SqlCommand();
                        objCom.CommandText = "ExtensionSave";
                        objCom.CommandType = CommandType.StoredProcedure;
                        objCom.Parameters.AddWithValue("@WorkPermitID", workPermit.WorkPermitID);
                        objCom.Parameters.AddWithValue("@ExtensionFrom", DateTime.ParseExact(workPermit.ExtensionFrom, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture));
                        objCom.Parameters.AddWithValue("@ExtensionTo", DateTime.ParseExact(workPermit.ExtensionTo, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture));
                        objCom.Parameters.AddWithValue("@ExtensionPermitIssuerID", workPermit.ExtensionPermitIssuerID);
                        objCom.Parameters.AddWithValue("@ExtensionPermitHolder", workPermit.ExtensionPermitHolder);
                        objCom.Parameters.AddWithValue("@ExtensionAreaOwnerID", workPermit.ExtensionAreaOwnerID);
                        objCom.Parameters.AddWithValue("@ExtensionApproverID", workPermit.ExtensionApproverID);
                        objCom.Parameters.AddWithValue("@Status", workPermit.Status);
                        objCon.Open();
                        objCom.Connection = objCon;
                        affectedcount = objCom.ExecuteNonQuery();
                        if (affectedcount > 0 && workPermit.Status == "E")
                        {
                            TempData["Message"] = string.Format("Work Permit ID_ {0} has been extended", workPermit.PermitNumber);
                        }
                        objCon.Close();
                        if (workPermit.Status == "E")
                        {
                            //workPermit.WorkType = GetWorkType();
                            //   var selectWorkTypelist = workPermit.WorkType.Where(y => y.WorkTypeID == workPermit.WorkTypeID).ToList();
                            var userListt = new AdminDA().SelectUserProfile();
                            var list = userListt.Where(y => y.UserID == workPermit.ExtensionPermitIssuerID).ToList();
                            workPermit.ExtensionPermitApproverList = userListt.Where(y => y.UserID == workPermit.ExtensionApproverID).ToList();


                            string FromAddress = list[0].EmailAddress.ToString();
                            string ToAddress;
                            string CCAddress;


                            ToAddress = workPermit.ExtensionPermitApproverList[0].EmailAddress;
                            CCAddress = list[0].EmailAddress;


                            string Subject = "Work Permit#" + workPermit.PermitNumber + "-".ToString();
                            string Sender = CurrentUser.FullName;
                            string Statement = "Extended".ToString();
                            mail(CCAddress, ToAddress, Sender, Subject, Statement);


                        }
                    }
                }
                else
                {
                    using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                    {
                        SqlCommand objCom = new SqlCommand();
                        objCom.CommandText = "WorkpermitApprove";
                        objCom.CommandType = CommandType.StoredProcedure;
                        objCom.Parameters.AddWithValue("@PermitID", workPermit.WorkPermitID);
                        objCom.Parameters.AddWithValue("@Identity", workPermit.Identity);
                        objCom.Parameters.AddWithValue("@MechComments", workPermit.MechInchRemarks);
                        objCom.Parameters.AddWithValue("@ElecComments", workPermit.ElecInchRemarks);
                        objCom.Parameters.AddWithValue("@InstruComments", workPermit.InstruInchRemarks);
                        objCom.Parameters.AddWithValue("@SafeComments", workPermit.SafetyOffRemarks);
                        objCom.Parameters.AddWithValue("@ProMgrComments", workPermit.ProMgrRemarks);
                        objCom.Parameters.AddWithValue("@GMopComments", workPermit.GMOpRemarks);
                        objCon.Open();
                        objCom.Connection = objCon;
                        affectedcount = objCom.ExecuteNonQuery();
                        if (affectedcount > 0 && workPermit.Status == "A")
                        {
                            TempData["Message"] = string.Format("Work Permit ID_ {0} has been Approved", workPermit.PermitNumber);
                        }
                        objCon.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return RedirectToAction("ApprovedPermitList");
        }

        [HttpGet]
        public ActionResult ClosedPermitList(int? page, string currentFromDate, string currentEndDate, int? currentEquipmentID)
        {
            SqlDataAdapter sda = new SqlDataAdapter();
            DataSet ds = new DataSet();
            List<WorkPermit> workPermit = new List<WorkPermit>();
            WorkPermitList closedWorkPermitList = new WorkPermitList();
            List<Equipment> equipmentList = new List<Equipment>();
            closedWorkPermitList.CurrentSessionID = CurrentUser.CurrentSessionID;
            closedWorkPermitList.PrevoiusSessionID = sess.SessionActive;
            if (closedWorkPermitList.CurrentSessionID == closedWorkPermitList.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            closedWorkPermitList.PlantList = adminBLL.DivisionDD();
            currentFromDate = currentFromDate == null ? DateTime.Now.ToString("dd/MM/yyyy 00:00") : currentFromDate;
            currentEndDate = currentEndDate == null ? DateTime.Now.ToString("dd/MM/yyyy 23:59") : currentEndDate;
            if (!string.IsNullOrEmpty(currentFromDate) && !string.IsNullOrEmpty(currentEndDate))
            {
                ViewBag.fromdate = currentFromDate;
                ViewBag.Todate = currentEndDate;
                closedWorkPermitList.FromDate = currentFromDate;
                closedWorkPermitList.Todate = currentEndDate;
                string fromDate = currentFromDate;
                string enddate = currentEndDate;
                int EquipmentID = Convert.ToInt16(currentEquipmentID);

                ViewBag.EquipmentID = EquipmentID;

                try
                {
                    using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                    {
                        SqlCommand objCom = new SqlCommand();
                        objCom.CommandText = "ClosedPermitList";
                        objCom.CommandType = CommandType.StoredProcedure;
                        objCom.Parameters.AddWithValue("@Fromdate", closedWorkPermitList.FromDate);
                        objCom.Parameters.AddWithValue("@Todate", closedWorkPermitList.Todate);
                        if (currentEquipmentID > 0)
                            objCom.Parameters.AddWithValue("@EquipmentID", EquipmentID);
                        else
                            objCom.Parameters.AddWithValue("@EquipmentID", DBNull.Value);
                        if (closedWorkPermitList.PlantID > 0)
                            objCom.Parameters.AddWithValue("@PlantID", closedWorkPermitList.PlantID);
                        else
                            objCom.Parameters.AddWithValue("@PlantID", DBNull.Value);
                        objCon.Open();
                        objCom.Connection = objCon;
                        sda.SelectCommand = objCom;
                        ds.Clear();
                        sda.Fill(ds);
                        objCon.Close();

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow Item in ds.Tables[0].Rows)
                            {

                                workPermit.Add(new WorkPermit()
                                {
                                    WorkPermitID = int.Parse(Item["WorkPermitID"].ToString()),
                                    EquipmentName = Item["EquipmentName"].ToString(),
                                    WorkDescription = Item["WorkDesctiption"].ToString(),
                                    PlantName = Item["PlantName"].ToString(),
                                    ValidityFrom = Item["ValidityFrom"].ToString(),
                                    WorkTypeName = Item["WorkType"].ToString(),
                                    WholeAttachment = Item["Attachement"].ToString(),
                                    ClosureComment = Item["ClosureComment"].ToString(),
                                    PermitHolderName = Item["PermitHolderName"].ToString(),
                                    PermitIssuerName = Item["PermitIssuerName"].ToString(),
                                    ApproverName = Item["ApproverName"].ToString(),
                                    PermitClosureName = Item["PermitClosureName"].ToString(),
                                    ClosedOn = Item["ClosedOn"].ToString(),
                                    Status = Item["Status"].ToString(),
                                    DepartmentName = Item["DepartmentName"].ToString(),
                                    PermitNumber = Item["PermitNo"].ToString(),

                                }) ;
                            }
                        }
                        foreach (DataRow Item in ds.Tables[1].Rows)
                        {
                            equipmentList.Add(new Equipment
                            {
                                EquipmentID = int.Parse(Item["EquipmentID"].ToString()),
                                EquipmentName = Item["EquipmentName"].ToString()
                            });
                        }
                        int pageSize = 10;
                        int PageNumber = 1;
                        if (page != null)
                        {
                            PageNumber = Convert.ToInt16(page);
                        }
                        closedWorkPermitList.EquipmentID = EquipmentID;

                        closedWorkPermitList.WorkPermits = workPermit.ToPagedList(PageNumber, pageSize);
                        closedWorkPermitList.EquipmentList = equipmentList;

                    }
                }

                catch (Exception objException)
                {
                    LogManager.Instance.Error(objException);
                    throw new Exception(objException.Message);

                }
                closedWorkPermitList.Roles = CurrentUser.Roles;
                closedWorkPermitList.UserFullName = CurrentUser.FullName;
                closedWorkPermitList.ProfileImage = CurrentUser.ProfileImage;
                closedWorkPermitList.IsRestrict = CurrentUser.IsRestrict;

            }

            return View(closedWorkPermitList);

        }


        [HttpPost]
        public ActionResult ClosedPermitList(WorkPermitList model)
        {
            WorkPermitList workPermitList = null;
            try
            {
                SqlDataAdapter sda = new SqlDataAdapter();
                DataSet ds = new DataSet();
                ViewBag.fromdate = model.FromDate;
                ViewBag.Todate = model.Todate;
                ViewBag.EquipmentID = model.EquipmentID;
                workPermitList = new WorkPermitList();
                List<WorkPermit> workPermit = new List<WorkPermit>();
                WorkPermitList closedWorkPermitList = new WorkPermitList();
                List<Equipment> equipmentList = new List<Equipment>();
                string fromdate = model.FromDate;
                string enddate = model.Todate;
                int EquipmentID = model.EquipmentID;
                workPermitList.EquipmentID = EquipmentID;
                workPermitList.FromDate = model.FromDate;
                workPermitList.Todate = model.Todate;
                workPermitList.EquipmentID = model.EquipmentID;
                workPermitList.PlantList = adminBLL.DivisionDD();
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "ClosedPermitList";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@Fromdate", model.FromDate);
                    objCom.Parameters.AddWithValue("@Todate", model.Todate);
                    if (model.EquipmentID > 0)
                        objCom.Parameters.AddWithValue("@EquipmentID", model.EquipmentID);
                    else
                        objCom.Parameters.AddWithValue("@EquipmentID", DBNull.Value);
                    if (model.PlantID > 0)
                        objCom.Parameters.AddWithValue("@PlantID", model.PlantID);
                    else
                        objCom.Parameters.AddWithValue("@PlantID", DBNull.Value);
                    objCon.Open();
                    objCom.Connection = objCon;
                    sda.SelectCommand = objCom;
                    ds.Clear();
                    sda.Fill(ds);
                    objCon.Close();
                    ViewBag.IsRecordFound = false;
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.IsRecordFound = true;
                        foreach (DataRow Item in ds.Tables[0].Rows)
                        {

                            workPermit.Add(new WorkPermit()
                            {
                                WorkPermitID = int.Parse(Item["WorkPermitID"].ToString()),
                                EquipmentName = Item["EquipmentName"].ToString(),
                                WorkDescription = Item["WorkDesctiption"].ToString(),
                                PlantName = Item["PlantName"].ToString(),
                                ValidityFrom = Item["ValidityFrom"].ToString(),
                                WorkTypeName = Item["WorkType"].ToString(),
                                WholeAttachment = Item["Attachement"].ToString(),
                                ClosureComment = Item["ClosureComment"].ToString(),
                                PermitHolderName = Item["PermitHolderName"].ToString(),
                                PermitIssuerName = Item["PermitIssuerName"].ToString(),
                                ApproverName = Item["ApproverName"].ToString(),
                                PermitClosureName = Item["PermitClosureName"].ToString(),
                                ClosedOn = Item["ClosedOn"].ToString(),
                                Status = Item["Status"].ToString(),
                                DepartmentName = Item["DepartmentName"].ToString(),
                                PermitNumber = Item["PermitNo"].ToString(),


                            });
                        }
                    }
                    foreach (DataRow Item in ds.Tables[1].Rows)
                    {
                        equipmentList.Add(new Equipment
                        {
                            EquipmentID = int.Parse(Item["EquipmentID"].ToString()),
                            EquipmentName = Item["EquipmentName"].ToString()
                        });
                    }
                    int pageSize = 10;
                    int PageNumber = 1;
                    workPermitList.EquipmentID = EquipmentID;

                    workPermitList.WorkPermits = workPermit.ToPagedList(PageNumber, pageSize);
                    workPermitList.EquipmentList = equipmentList;

                }
            }

            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);

            }
            workPermitList.Roles = CurrentUser.Roles;
            workPermitList.UserFullName = CurrentUser.FullName;
            workPermitList.ProfileImage = CurrentUser.ProfileImage;
            workPermitList.IsRestrict = CurrentUser.IsRestrict;
            return View(workPermitList);
        }

        public ActionResult ExportClosedPermitList(string currentFromDate, string currentEndDate, int currentEquipmentID)
        {

            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "ExportClosedPermitList";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@Fromdate", currentFromDate);
                objCom.Parameters.AddWithValue("@Todate", currentEndDate);
                if (currentEquipmentID > 0)
                    objCom.Parameters.AddWithValue("@EquipmentID", currentEquipmentID);
                else
                    objCom.Parameters.AddWithValue("@EquipmentID", DBNull.Value);


                objCon.Open();
                objCom.Connection = objCon;
                SqlDataAdapter Adapter = new SqlDataAdapter();
                Adapter.SelectCommand = objCom;
                DataSet dataSet = new DataSet();
                Adapter.Fill(dataSet);
                objCon.Close();
                var wb = new XLWorkbook(Server.MapPath("~/Template/ClosedList.xlsx"));
                var worksheet = wb.Worksheet("ClosedList");
                worksheet.Cell("C4").Value = "Report Generated by : " + CurrentUser.FullName;
                //worksheet.Cell("D4").Value = CurrentUser.FullName;
                worksheet.Cell("C5").Value = "Report Duration : " + currentFromDate + " to  " + currentEndDate;
                //worksheet.Cell("D5").Value = currentFromDate + " to  " + currentEndDate;

                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    var rangeWithDataSecond = worksheet.Cell(7, 1).InsertTable(dataSet.Tables[0].AsEnumerable());
                }
                else
                {
                    worksheet.Cell("C8").Value = "No Data Found";
                }

                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename= ClosedWorkPermitHistoryRecord.xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

            }


            return View();


        }
        public ActionResult WorkPermitPDF(int id)
        {
            try
            {
                using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[PDFWorkPermit]";
                    objCom.Parameters.AddWithValue("@PermitID", id);
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    objCom.Connection = objCon;
                    SqlDataAdapter Adapter = new SqlDataAdapter();
                    Adapter.SelectCommand = objCom;
                    DataSet dataSet = new DataSet();
                    Adapter.Fill(dataSet);
                    objCon.Close();

                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition",
                        "filename=WorkPermit " + dataSet.Tables[0].Rows[0][0].ToString() + ".pdf");
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);

                    iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(iTextSharp.text.PageSize.LEGAL, 20f, 20f, 30f, 30f);
                    HTMLWorker htmlparsers = new HTMLWorker(pdfDoc);
                    PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                    pdfDoc.Open();
                    PdfPCell cell = null;

                    if (dataSet.Tables[0].Rows[0][24].ToString() == "D" || dataSet.Tables[0].Rows[0][24].ToString() == "R")
                    {
                        string imagePath = Server.MapPath("~/Images/watermark.png");
                        iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imagePath);

                        image.ScalePercent(200f);
                        image.RotationDegrees = 45f;
                        image.SetAbsolutePosition(0f, 200f);

                        pdfDoc.Add(image);
                    }
                    if (dataSet.Tables[0].Rows[0][24].ToString() == "C")
                    {
                        string imagePath = Server.MapPath("~/Images/closed.png");
                        iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imagePath);

                        image.ScalePercent(200f);
                        image.RotationDegrees = 45f;
                        image.SetAbsolutePosition(0f, 200f);

                        pdfDoc.Add(image);
                    }
                    if (dataSet.Tables[0].Rows[0][24].ToString() == "T")
                    {
                        string imagePath = Server.MapPath("~/Images/Canceled.png");
                        iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imagePath);

                        image.ScalePercent(200f);
                        image.RotationDegrees = 45f;
                        image.SetAbsolutePosition(0f, 200f);

                        pdfDoc.Add(image);
                    }

                    PdfPTable TitleTable = new PdfPTable(4);
                    TitleTable.LockedWidth = true;
                    TitleTable.SetWidths(new float[] { 10f, 18f, 6.8f, 5.2f });
                    TitleTable.SpacingBefore = 10f;
                    TitleTable.SpacingAfter = 1f;
                    TitleTable.TotalWidth = 555f;
                    PdfPCell Wpcell = new PdfPCell();
                    string imageURL = Server.MapPath("~/Images/SASALogo.png");
                    iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(imageURL);
                    gif.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                    gif.SpacingBefore = 10f;

                    gif.ScaleAbsolute(135f, 40f);
                    Wpcell = new PdfPCell(gif);

                    TitleTable.AddCell(Wpcell);

                    var phrase = new Phrase(new Chunk("\n SASA PTA PROJECT ", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    phrase.Add(new Chunk("\n ADANA,TURKEY \n", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    phrase.Add(new Chunk("\n\n", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    TitleTable.AddCell(PhraseCell(phrase, PdfPCell.ALIGN_CENTER));

                    Wpcell = new PdfPCell(new Phrase("\n WORK PERMIT NO ", FontFactory.GetFont("Times New Roman", 12, iTextSharp.text.Font.BOLD)));
                    Wpcell.HorizontalAlignment = Element.ALIGN_CENTER;

                    TitleTable.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase("\n " + dataSet.Tables[0].Rows[0][0].ToString(), FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    Wpcell.HorizontalAlignment = Element.ALIGN_LEFT;

                    TitleTable.AddCell(Wpcell);

                    // Font zapfdingbats = new Font(Font.FontFamily.ZAPFDINGBATS);
                    Font red = new Font(Font.FontFamily.ZAPFDINGBATS, 12, Font.NORMAL, BaseColor.GRAY);

                    String Tickmark = "\u0033";

                    Paragraph Tick = new Paragraph(Tickmark, red);

                    String FONT = "C:/Windows/Fonts/wingding.ttf";
                    String CheckedCheckboxText = "\u00fe";
                    String BlankCheckboxText = "o";
                    BaseFont bf = BaseFont.CreateFont(FONT, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    iTextSharp.text.Font f = new iTextSharp.text.Font(bf, 10);
                    Paragraph CheckedCheckbox = new Paragraph(CheckedCheckboxText, f);
                    Paragraph uncheckbox = new Paragraph(BlankCheckboxText, f);

                    String Blank = "";
                    Paragraph uncheckblank = new Paragraph(Blank, f);
                    pdfDoc.Add(TitleTable);

                    PdfPTable Typework = new PdfPTable(1);

                    Typework.TotalWidth = 555f;
                    Typework.LockedWidth = true;
                    Typework.SetWidths(new float[] { 40f });

                    Typework.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][9].ToString(), FontFactory.GetFont("Times New Roman", 14, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_CENTER));

                    pdfDoc.Add(Typework);

                    //String form = String.Format("  ".PadRight(60) + dataSet.Tables[0].Rows[0][9].ToString()+"\n\n");

                    //pdfDoc.Add(new iTextSharp.text.Paragraph(form, FontFactory.GetFont("Times New Roman", 14, iTextSharp.text.Font.BOLD)));


                    PdfPTable permitdetails = new PdfPTable(5);
                    permitdetails.TotalWidth = 555f;
                    permitdetails.LockedWidth = true;
                    permitdetails.SetWidths(new float[] { 2f, 8f, 9f, 9f, 12f });
                    permitdetails.AddCell(PhraseCell(new Phrase("1", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetails.AddCell(PhraseCell(new Phrase("VALIDITY FROM ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetails.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][1].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    permitdetails.AddCell(PhraseCell(new Phrase("VALIDITY TO ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetails.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][2].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    permitdetails.AddCell(PhraseCell(new Phrase("2", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetails.AddCell(PhraseCell(new Phrase("PERMIT ISSUER ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetails.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][3].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    permitdetails.AddCell(PhraseCell(new Phrase("PERMIT RECEIVER ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetails.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][4].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(permitdetails);
                    PdfPTable plant = new PdfPTable(5);
                    plant.TotalWidth = 555f;
                    plant.LockedWidth = true;
                    plant.SetWidths(new float[] { 2f, 8f, 9f, 9f, 12f });
                    plant.AddCell(PhraseCell(new Phrase("3", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    plant.AddCell(PhraseCell(new Phrase(" PLANT/AREA ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    plant.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][48].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    plant.AddCell(PhraseCell(new Phrase("JOB ID", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    plant.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][20].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(plant);

                    PdfPTable Equtable = new PdfPTable(5);
                    Equtable.TotalWidth = 555f;
                    Equtable.LockedWidth = true;
                    Equtable.SetWidths(new float[] { 2f, 8f, 9f, 9f, 12f });

                    Equtable.AddCell(PhraseCell(new Phrase("4", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Equtable.AddCell(PhraseCell(new Phrase("LOCATION OF THE WORK ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Equtable.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][6].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    Equtable.AddCell(PhraseCell(new Phrase("EQUIPMENT NAME", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Equtable.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][7].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(Equtable);
                    PdfPTable desc = new PdfPTable(3);
                    desc.TotalWidth = 555f;
                    desc.LockedWidth = true;
                    desc.SetWidths(new float[] { 2f, 8f, 30f });
                    desc.AddCell(PhraseCell(new Phrase("5", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    desc.AddCell(PhraseCell(new Phrase("DESCRIPTION OF WORK  ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    desc.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][5].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(desc);


                    PdfPTable loc = new PdfPTable(5);
                    loc.TotalWidth = 555f;
                    loc.LockedWidth = true;
                    loc.SetWidths(new float[] { 2f, 8f, 9f, 9f, 12f });


                    loc.AddCell(PhraseCell(new Phrase("6", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    loc.AddCell(PhraseCell(new Phrase("WORK DONE BY ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    loc.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][21].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    loc.AddCell(PhraseCell(new Phrase("DEPARTMENT", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    loc.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][22].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));


                    loc.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    loc.AddCell(PhraseCell(new Phrase("NAME OF CONTRACTOR", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    loc.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][8].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    loc.AddCell(PhraseCell(new Phrase("RISK ASSESSMENT REQUIRED?", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    loc.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][23].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));


                    pdfDoc.Add(loc);

                    PdfPTable area = new PdfPTable(2);
                    area.TotalWidth = 555f;
                    area.LockedWidth = true;
                    area.SetWidths(new float[] { 2f, 38f });
                    area.AddCell(PhraseCell(new Phrase("7", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    area.AddCell(PhraseCell(new Phrase("Following points should be checked & verified in Field before issuing the Work Permit as per the nature of job.", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    pdfDoc.Add(area);

                    PdfPTable gen = new PdfPTable(8);
                    var font8 = FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD);
                    var font9 = FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL);
                    var font7 = FontFactory.GetFont("Times New Roman", 11, iTextSharp.text.Font.BOLD);
                    gen.TotalWidth = 555f;
                    gen.LockedWidth = true;
                    gen.SetWidths(new float[] { 2f, 2.5f, 3.5f, 12f, 2f, 2.5f, 3.5f, 12f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    gen.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Done", font8)));
                    gen.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Not Required", font8)));
                    gen.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Check List", font8)));
                    gen.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    gen.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Done", font8)));
                    gen.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Not Required", font8)));
                    gen.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Check List", font8)));
                    gen.AddCell(Wpcell);


                    if (dataSet.Tables[1].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[1].Rows.Count; rows++)
                        {

                            gen.AddCell(PhraseCell(new Phrase(dataSet.Tables[1].Rows[rows][0].ToString(), font9), PdfPCell.ALIGN_CENTER));
                            gen.AddCell(dataSet.Tables[1].Rows[rows][1].ToString() == "1" ? Tick : uncheckblank);
                            gen.AddCell(dataSet.Tables[1].Rows[rows][1].ToString() == "0" ? Tick : uncheckblank);
                            Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[1].Rows[rows][2].ToString(), font9)));
                            gen.AddCell(Wpcell);

                        }
                    }
                    gen.AddCell("");
                    gen.AddCell("");
                    gen.AddCell("");
                    gen.AddCell("");
                    pdfDoc.Add(gen);

                    PdfPTable actualchklist = new PdfPTable(8);

                    actualchklist.TotalWidth = 555f;
                    actualchklist.LockedWidth = true;
                    actualchklist.SetWidths(new float[] { 2f, 2.5f, 3.5f, 12f, 2f, 2.5f, 3.5f, 12f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));

                    actualchklist.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actualchklist.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actualchklist.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Work Specific Check List", font8)));
                    actualchklist.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actualchklist.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actualchklist.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actualchklist.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Work Specific Check List", font8)));
                    actualchklist.AddCell(Wpcell);


                    if (dataSet.Tables[4].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[4].Rows.Count; rows++)
                        {
                            actualchklist.AddCell(PhraseCell(new Phrase(dataSet.Tables[4].Rows[rows][0].ToString(), font9), PdfPCell.ALIGN_CENTER));
                            actualchklist.AddCell(dataSet.Tables[4].Rows[rows][1].ToString() == "1" ? Tick : uncheckblank);
                            actualchklist.AddCell(dataSet.Tables[4].Rows[rows][1].ToString() == "0" ? Tick : uncheckblank);
                            Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[4].Rows[rows][2].ToString(), font9)));
                            actualchklist.AddCell(Wpcell);

                        }
                    }

                    pdfDoc.Add(actualchklist);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(300)));
                    PdfPTable ppe = new PdfPTable(2);
                    ppe.TotalWidth = 555f;
                    ppe.LockedWidth = true;
                    ppe.SetWidths(new float[] { 2f, 38f });
                    ppe.AddCell(PhraseCell(new Phrase("8", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    ppe.AddCell(PhraseCell(new Phrase("SPECIAL PROTECTION / PPE", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(ppe);


                    PdfPTable ppe1 = new PdfPTable(8);
                    ppe1.TotalWidth = 555f;
                    ppe1.LockedWidth = true;
                    ppe1.SetWidths(new float[] { 2f, 8f, 2f, 8f, 2f, 8f, 2f, 8f });

                    if (dataSet.Tables[2].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[2].Rows.Count; rows++)
                        {

                            ppe1.AddCell(dataSet.Tables[2].Rows[rows][1].ToString() == "1" ? CheckedCheckbox : uncheckbox);

                            Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[2].Rows[rows][2].ToString(), font9)));
                            ppe1.AddCell(Wpcell);

                        }
                    }

                    else
                    {
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Applicable", font9)));
                        Wpcell.Colspan = 8;
                        ppe1.AddCell(Wpcell);

                    }
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    ppe1.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));

                    ppe1.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));

                    ppe1.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));

                    ppe1.AddCell(Wpcell);


                    Wpcell = new PdfPCell(new Phrase(new Chunk("8.1 Others / L.C. No :".PadRight(5) + dataSet.Tables[0].Rows[0][10].ToString(), font8)));
                    Wpcell.Colspan = 8;
                    ppe1.AddCell(Wpcell);

                    pdfDoc.Add(ppe1);


                    if (dataSet.Tables[3].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[3].Rows.Count; rows++)
                        {
                            if ((dataSet.Tables[3].Rows[rows][1].ToString() == "1") || (dataSet.Tables[3].Rows[rows][1].ToString() == "3"))

                            {

                                PdfPTable gas = new PdfPTable(2);
                                gas.TotalWidth = 555f;
                                gas.LockedWidth = true;
                                gas.SetWidths(new float[] { 2f, 38f });
                                gas.AddCell(PhraseCell(new Phrase("8.2", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                                gas.AddCell(PhraseCell(new Phrase("GAS TEST RESULT: Suggested retesting frequency is every …………Hrs (use additional table, if required)", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                                pdfDoc.Add(gas);


                                PdfPTable gasdetails = new PdfPTable(9);
                                gasdetails.TotalWidth = 555f;
                                gasdetails.LockedWidth = true;
                                gasdetails.SetWidths(new float[] { 2f, 6f, 4f, 3f, 5f, 5f, 5f, 5f, 5f });
                                Wpcell = new PdfPCell(new Phrase(new Chunk("No.", font8)));
                                gasdetails.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("Values", font8)));
                                gasdetails.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("%O2", font8)));
                                gasdetails.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("%LEL", font8)));
                                gasdetails.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("Toxic Gas (PPM)", font8)));
                                gasdetails.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("Other Gases", font8)));
                                gasdetails.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("Name", font8)));
                                gasdetails.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("Signature", font8)));
                                gasdetails.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("Date Time", font8)));
                                gasdetails.AddCell(Wpcell);

                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                gasdetails.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("Acceptable Values", font9)));
                                gasdetails.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("19.5 -22.5%", font9)));
                                gasdetails.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("Zero", font9)));
                                gasdetails.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("Less than TLV", font9)));
                                gasdetails.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("Less than TLV", font9)));
                                gasdetails.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                gasdetails.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                gasdetails.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                gasdetails.AddCell(Wpcell);

                                Wpcell = new PdfPCell(new Phrase(new Chunk("1", font9)));
                                gasdetails.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                gasdetails.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                gasdetails.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                gasdetails.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                gasdetails.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                gasdetails.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                gasdetails.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                gasdetails.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                gasdetails.AddCell(Wpcell);

                                Wpcell = new PdfPCell(new Phrase(new Chunk("2", font9)));
                                gasdetails.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                gasdetails.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                gasdetails.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                gasdetails.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                gasdetails.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                gasdetails.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                gasdetails.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                gasdetails.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                gasdetails.AddCell(Wpcell);

                                pdfDoc.Add(gasdetails);
                            }
                            break;
                        }
                    }
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));


                    PdfPTable permithand = new PdfPTable(6);

                    permithand.TotalWidth = 555f;
                    permithand.LockedWidth = true;
                    permithand.SetWidths(new float[] { 2f, 10f, 8f, 9.2f, 5f, 5f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk("9", font8)));

                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("PERMIT APPROVALS", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Remarks ", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Signature", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Date Time", font8)));
                    permithand.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("I have checked the conditions in the field and the status is found as mentioned above. The job can be permitted.", font9)));

                    Wpcell.Colspan = 5;
                    permithand.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Permit Issuer", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][3].ToString(), font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Permit Approver(Shift In Charge)", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][11].ToString(), font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" " + dataSet.Tables[0].Rows[0][19].ToString(), font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][35].ToString(), font9)));
                    permithand.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Stand by Person", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][18].ToString(), font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Safety officer", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][12].ToString(), font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" " + dataSet.Tables[0].Rows[0][36].ToString(), font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" " + dataSet.Tables[0].Rows[0][37].ToString(), font9)));
                    permithand.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Process Manager", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][13].ToString(), font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" " + dataSet.Tables[0].Rows[0][38].ToString(), font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" " + dataSet.Tables[0].Rows[0][39].ToString(), font9)));
                    permithand.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Electrical In charge", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][14].ToString(), font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" " + dataSet.Tables[0].Rows[0][40].ToString(), font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" " + dataSet.Tables[0].Rows[0][41].ToString(), font9)));
                    permithand.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Mechanical In charge", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][15].ToString(), font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" " + dataSet.Tables[0].Rows[0][42].ToString(), font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" " + dataSet.Tables[0].Rows[0][43].ToString(), font9)));
                    permithand.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Instrument In charge", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][16].ToString(), font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" " + dataSet.Tables[0].Rows[0][44].ToString(), font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" " + dataSet.Tables[0].Rows[0][45].ToString(), font9)));
                    permithand.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Operations Head ", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][17].ToString(), font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" " + dataSet.Tables[0].Rows[0][46].ToString(), font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" " + dataSet.Tables[0].Rows[0][47].ToString(), font9)));
                    permithand.AddCell(Wpcell);


                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("I understood the above condition,arranged safety requirements, explained to the people concerned and received the permit for safe job execution.", font9)));
                    Wpcell.Colspan = 5;

                    permithand.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("")));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name / Number of person(s) working at site", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][25].ToString(), font9)));
                    Wpcell.Colspan = 4;
                    permithand.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Permit Receiver(Engineer /\nTech.of recipient dept).", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][4].ToString(), font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);

                    pdfDoc.Add(permithand);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));



                    PdfPTable permitclosure = new PdfPTable(6);

                    permitclosure.TotalWidth = 555f;
                    permitclosure.LockedWidth = true;
                    permitclosure.KeepTogether = true;
                    permitclosure.SetWidths(new float[] { 2f, 10f, 8f, 9.2f, 5f, 5f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk("10", font8)));

                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("PERMIT CLOSURE", font8)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name", font8)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Remarks ", font8)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Signature", font8)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Date Time", font8)));
                    permitclosure.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Certified that the subject work has been completed/ stopped and area cleaned. ", font9)));
                    Wpcell.Colspan = 5;

                    permitclosure.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Job completed & Handed over by Receiver", font8)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][24].ToString() == "C" ? dataSet.Tables[0].Rows[0][31].ToString() : "", font9)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosure.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));

                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Job taken over & Permit closed by Issuer", font8)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][24].ToString() == "C" ? dataSet.Tables[0].Rows[0][33].ToString() : "", font9)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][24].ToString() == "C" ? dataSet.Tables[0].Rows[0][34].ToString() : "", font9)));
                    permitclosure.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Approver/Shift InCharge", font8)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][24].ToString() == "C" ? dataSet.Tables[0].Rows[0][32].ToString() : "", font9)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" " + dataSet.Tables[0].Rows[0][24].ToString() == "T" ? "" : dataSet.Tables[0].Rows[0][27].ToString(), font9)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosure.AddCell(Wpcell);


                    pdfDoc.Add(permitclosure);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable conr = new PdfPTable(6);
                    PdfPCell p = new PdfPCell();
                    conr.TotalWidth = 555f;
                    conr.LockedWidth = true;
                    conr.KeepTogether = true;
                    conr.SetWidths(new float[] { 1f, 3f, 1f, 3f, 1f, 22f });
                    cell = PhraseCell(new Phrase("11".PadRight(6) + "CONTRACTOR RATING  ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT);
                    cell.Colspan = 6;
                    conr.AddCell(cell);

                    conr.AddCell(dataSet.Tables[0].Rows[0][28].ToString() == "R" ? Tick : uncheckblank);
                    p = new PdfPCell(new Phrase(new Chunk("Red", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD))));
                    conr.AddCell(p);

                    conr.AddCell(dataSet.Tables[0].Rows[0][28].ToString() == "Y" ? Tick : uncheckblank);

                    p = new PdfPCell(new Phrase(new Chunk("Yellow", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD))));
                    conr.AddCell(p);

                    conr.AddCell(dataSet.Tables[0].Rows[0][28].ToString() == "G" ? Tick : uncheckblank);

                    p = new PdfPCell(new Phrase(new Chunk("Green", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD))));
                    conr.AddCell(p);

                    p = new PdfPCell(new Phrase(new Chunk(" Remarks (if Red and Yellow rating) :   " + dataSet.Tables[0].Rows[0][26].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL))));
                    p.Colspan = 8;
                    conr.AddCell(p);

                    pdfDoc.Add(conr);
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));
                    if (dataSet.Tables[3].Rows[0][1].ToString() == "4")
                    {
                        pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));
                    }

                    if (dataSet.Tables[3].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[3].Rows.Count; rows++)
                        {
                            if ((dataSet.Tables[3].Rows[rows][1].ToString() == "1") || (dataSet.Tables[3].Rows[rows][1].ToString() == "2") || (dataSet.Tables[3].Rows[rows][1].ToString() == "4") || (dataSet.Tables[3].Rows[rows][1].ToString() == "5") || (dataSet.Tables[3].Rows[rows][1].ToString() == "7"))

                            {

                                PdfPTable ren = new PdfPTable(2);
                                ren.TotalWidth = 555f;
                                ren.LockedWidth = true;
                                ren.SetWidths(new float[] { 2f, 38f });
                                ren.AddCell(PhraseCell(new Phrase("12", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                                ren.AddCell(PhraseCell(new Phrase("RENEWAL OF WORK PERMIT", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                                pdfDoc.Add(ren);


                                PdfPTable renewal = new PdfPTable(9);

                                renewal.TotalWidth = 555f;
                                renewal.LockedWidth = true;
                                renewal.SetWidths(new float[] { 7f, 5f, 4f, 4f, 4f, 4f, 4f, 4f, 4f });

                                Wpcell = new PdfPCell(new Phrase(new Chunk("Day/ Date/Time", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("1. LEL Level (ZERO)", font8)));

                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of Permit issuer ", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of shift In charge ", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of Stand by", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of safety In charge", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);

                                Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of Manager Process", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of Operations Head", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);

                                Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of  Receiver", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);

                                Wpcell = new PdfPCell(new Phrase(new Chunk("2. % O2(19.5 - 22.5 %)", font8)));

                                renewal.AddCell(Wpcell);

                                Wpcell = new PdfPCell(new Phrase(new Chunk("First Extension Date:................................", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);

                                Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell.Rowspan = 2;
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);

                                Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                                renewal.AddCell(Wpcell);

                                Wpcell = new PdfPCell(new Phrase(new Chunk("Second Extension Date:................................", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);

                                Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell.Rowspan = 2;
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);

                                Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                                renewal.AddCell(Wpcell);

                                Wpcell = new PdfPCell(new Phrase(new Chunk("Third Extension Date:................................", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);

                                Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell.Rowspan = 2;
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);

                                Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                                renewal.AddCell(Wpcell);

                                Wpcell = new PdfPCell(new Phrase(new Chunk("Fourth Extension Date:................................", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);

                                Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell.Rowspan = 2;
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);

                                Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                                renewal.AddCell(Wpcell);

                                Wpcell = new PdfPCell(new Phrase(new Chunk("Fifth Extension Date:................................", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);

                                Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell.Rowspan = 2;
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);

                                Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                                renewal.AddCell(Wpcell);

                                Wpcell = new PdfPCell(new Phrase(new Chunk("Sixth Extension Date:................................", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);

                                Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);
                                Wpcell.Rowspan = 2;
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                                Wpcell.Rowspan = 2;
                                renewal.AddCell(Wpcell);

                                Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                                renewal.AddCell(Wpcell);


                                pdfDoc.Add(renewal);
                                if ((dataSet.Tables[3].Rows[rows][1].ToString() != "1") && (dataSet.Tables[3].Rows[rows][1].ToString() != "3"))
                                {
                                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));
                                }
                            }
                            break;
                        }

                    }



                    if (dataSet.Tables[3].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[3].Rows.Count; rows++)
                        {
                            if (dataSet.Tables[3].Rows[rows][1].ToString() == "3")

                            {
                                pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));
                                PdfPTable cs = new PdfPTable(2);
                                cs.TotalWidth = 555f;
                                cs.LockedWidth = true;
                                cs.SetWidths(new float[] { 2f, 38f });
                                cs.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                                cs.AddCell(PhraseCell(new Phrase("CONFINED SPACE – MANENTRY/EXIT LOG SHEET (Use additional sheet, if required)", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                                pdfDoc.Add(cs);


                                PdfPTable man = new PdfPTable(8);

                                man.TotalWidth = 555f;
                                man.LockedWidth = true;
                                man.SetWidths(new float[] { 2f, 8f, 6f, 4f, 5f, 5f, 5f, 5f });

                                Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("Entrant Name", font8)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("In Time ", font8)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("Signature", font8)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("Sign of Stand by", font8)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("Out time", font8)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("Entrant Sign", font8)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("Sign of Stand by", font8)));
                                man.AddCell(Wpcell);

                                Wpcell = new PdfPCell(new Phrase(new Chunk("1", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);

                                Wpcell = new PdfPCell(new Phrase(new Chunk("2 ", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);

                                Wpcell = new PdfPCell(new Phrase(new Chunk("3 ", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);

                                Wpcell = new PdfPCell(new Phrase(new Chunk("4", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);

                                Wpcell = new PdfPCell(new Phrase(new Chunk("5 ", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);

                                Wpcell = new PdfPCell(new Phrase(new Chunk("6", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);

                                Wpcell = new PdfPCell(new Phrase(new Chunk("7", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                                man.AddCell(Wpcell);

                                pdfDoc.Add(man);

                                pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                            }


                        }

                    }

                    if (dataSet.Tables[3].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[3].Rows.Count; rows++)
                        {
                            if (dataSet.Tables[3].Rows[rows][1].ToString() == "1")

                            {
                                String form1 = String.Format("** Renewal of Hot work permit is allowed only if the work is carried out in Workshop ");

                                pdfDoc.Add(new iTextSharp.text.Paragraph(form1, font8));

                                pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));
                            }


                        }

                    }

                    PdfPTable genins = new PdfPTable(2);
                    genins.TotalWidth = 555f;
                    genins.LockedWidth = true;
                    genins.SetWidths(new float[] { 2f, 38f });
                    genins.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    genins.AddCell(PhraseCell(new Phrase("GENERAL INSTRUCTIONS : ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1.", font9)));
                    genins.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("In case of an Emergency or FIRE ALARM, all works must be stopped and permit stands cancelled. All personnel must leave work site and proceed to emergency Assembly points soon.", font9)));
                    genins.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2.", font9)));
                    genins.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("In case of any  LPG/ Gas /Liquid release (or) abnormality noticed, stop work immediately and move to safe location till further instruction,", font9)));
                    genins.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("3.", font9)));
                    genins.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("This permit must be available at work site all the time. On job closure (or) expiry, this permit must be returned to the permit issuer/Shift Incharge. ", font9)));
                    genins.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("4.", font9)));
                    genins.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Do not commence the work without a valid work permit.", font9)));
                    genins.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("5.", font9)));
                    genins.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Acquaint yourself with job and its hazards and ensure the tools used are safe.", font9)));
                    genins.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("6.", font9)));
                    genins.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Inform regarding unsafe condition/unsafe act to operator/Safety incharge/Shift incharge", font9)));
                    genins.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("7.", font9)));
                    genins.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("When in doubt, consult SASA employee of concerned department (or) SASA Operator ", font9)));
                    genins.AddCell(Wpcell);


                    Wpcell = new PdfPCell(new Phrase(new Chunk("8.", font9)));
                    genins.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Work should be started within one hour after issue of this permit", font9)));
                    genins.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("9.", font9)));
                    genins.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("All safety precautions should be meticulously followed. Keep access clear for emergency.", font9)));
                    genins.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("10.", font9)));
                    genins.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Incase of any emergency, permit should be surrendered to the issuing authority", font9)));
                    genins.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("11.", font9)));
                    genins.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Permit is valid only for the mentioned time. After that, further extension is needed before proceeding the job", font9)));
                    genins.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("12.", font9)));
                    genins.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Extention: To be extended only, if area and nature of work is same", font9)));
                    genins.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("13.", font9)));
                    genins.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Extention: Daily extension should be authorized per the concerned persons, after reviewing the site again.", font9)));
                    genins.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("14.", font9)));
                    genins.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Extention: If the job is discontinued, make a fresh permit", font9)));
                    genins.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("15.", font9)));
                    genins.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Extension: Extention can be done maximum one week duration. Make new permit every Monday.", font9)));
                    genins.AddCell(Wpcell);



                    pdfDoc.Add(genins);


                    PdfPTable spec = new PdfPTable(2);
                    spec.TotalWidth = 555f;
                    spec.LockedWidth = true;
                    spec.SetWidths(new float[] { 2f, 38f });

                    if (dataSet.Tables[3].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[3].Rows.Count; rows++)
                        {
                            if (dataSet.Tables[3].Rows[rows][1].ToString() == "1")

                            {
                                spec.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                                spec.AddCell(PhraseCell(new Phrase("PERMIT SPECIFIC INSTRUCTIONS - Hot Work ", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                                spec.AddCell(PhraseCell(new Phrase("1.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                                spec.AddCell(PhraseCell(new Phrase("Hot work includes spark, welding, gas cutting, hacksaw cutting, Hammering, burning, grinding, soldering, chipping, rivetting, shot blasting, drilling, camera flashing, power tools, IC Engine operations. ", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                                spec.AddCell(PhraseCell(new Phrase("2.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                                spec.AddCell(PhraseCell(new Phrase("Incase of gas/liquid leak, all HOT work must be stopped immediately ", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                                spec.AddCell(PhraseCell(new Phrase("3.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                                spec.AddCell(PhraseCell(new Phrase("For any hotwork inside the vessel, Confined Space permit should also be ensured", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                                pdfDoc.Add(spec);
                            }

                        }
                    }


                    PdfPTable cons = new PdfPTable(2);
                    cons.TotalWidth = 555f;
                    cons.LockedWidth = true;
                    cons.SetWidths(new float[] { 2f, 38f });
                    if (dataSet.Tables[3].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[3].Rows.Count; rows++)
                        {
                            if (dataSet.Tables[3].Rows[rows][1].ToString() == "3")

                            {
                                cons.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                                cons.AddCell(PhraseCell(new Phrase("PERMIT SPECIFIC INSTRUCTIONS - CONFINED SPACE ", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                                cons.AddCell(PhraseCell(new Phrase("1.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                                cons.AddCell(PhraseCell(new Phrase("Oxygen inside the vessel must be above 20.8%", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                                cons.AddCell(PhraseCell(new Phrase("2.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                                cons.AddCell(PhraseCell(new Phrase("Entry/exit log sheet oof personnel entering confined space to be maintained with name of person, ID number by the executing agency / contractor Supervisor.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                                cons.AddCell(PhraseCell(new Phrase("3.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                                cons.AddCell(PhraseCell(new Phrase("Job briefing/ Pep safety talk to be done at work site prior to entry in to confined space and working. ", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                                cons.AddCell(PhraseCell(new Phrase("4.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                                cons.AddCell(PhraseCell(new Phrase("Provision of standby person, emergency preparedness should be ensured at work site.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                                cons.AddCell(PhraseCell(new Phrase("5.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                                cons.AddCell(PhraseCell(new Phrase("For any hotwork inside the vessel, hot work permit should also be ensured", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                                cons.AddCell(PhraseCell(new Phrase("6.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                                cons.AddCell(PhraseCell(new Phrase("Do not allow gas cylinders inside the cofined space", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                                cons.AddCell(PhraseCell(new Phrase("7.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                                cons.AddCell(PhraseCell(new Phrase("Confined Space Entry permit can not be extended", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));


                                pdfDoc.Add(cons);

                            }

                        }
                    }


                    PdfPTable workh = new PdfPTable(2);
                    workh.TotalWidth = 555f;
                    workh.LockedWidth = true;
                    workh.SetWidths(new float[] { 2f, 38f });
                    if (dataSet.Tables[3].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[3].Rows.Count; rows++)
                        {
                            if (dataSet.Tables[3].Rows[rows][1].ToString() == "7")

                            {
                                workh.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                                workh.AddCell(PhraseCell(new Phrase("PERMIT SPECIFIC INSTRUCTIONS - WORK AT HEIGHT: ", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                                workh.AddCell(PhraseCell(new Phrase("1.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                                workh.AddCell(PhraseCell(new Phrase("Ensure Emergency communication & actions, in case of emergency. Maintain the list of personnel working at height and safety register.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                                workh.AddCell(PhraseCell(new Phrase("2.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                                workh.AddCell(PhraseCell(new Phrase(" Check scaffold stability. Do not overload the scaffolding with heavy material ", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                                workh.AddCell(PhraseCell(new Phrase("3.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                                workh.AddCell(PhraseCell(new Phrase("Do not use cracked material for scaffolds.  No portion of scaffold to be removed or modified without approval.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                                workh.AddCell(PhraseCell(new Phrase("4.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                                workh.AddCell(PhraseCell(new Phrase(" Wear safety belt postively and PPEs.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                                workh.AddCell(PhraseCell(new Phrase("5.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                                workh.AddCell(PhraseCell(new Phrase("No loose materials is to be kept on Jula top. They must be secured inside a box to aviod falling", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                                workh.AddCell(PhraseCell(new Phrase("6.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                                workh.AddCell(PhraseCell(new Phrase("Ensure spot communication to Crane operator, signalman (if applicable) during job execution", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                                workh.AddCell(PhraseCell(new Phrase("7.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                                workh.AddCell(PhraseCell(new Phrase("job allowed only when wind velocity is low.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                                workh.AddCell(PhraseCell(new Phrase("8.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                                workh.AddCell(PhraseCell(new Phrase("Personnel working at height must be experienced, medical fitness certificates to be obtained as required.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                                workh.AddCell(PhraseCell(new Phrase("9.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                                workh.AddCell(PhraseCell(new Phrase("Lifting equipment checked & inspected thoroughly for mechanical function & physical condition.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                                pdfDoc.Add(workh);

                            }

                        }
                    }


                    PdfPTable Lifting = new PdfPTable(2);
                    Lifting.TotalWidth = 555f;
                    Lifting.LockedWidth = true;
                    Lifting.SetWidths(new float[] { 2f, 38f });

                    if (dataSet.Tables[3].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[3].Rows.Count; rows++)
                        {
                            if (dataSet.Tables[3].Rows[rows][1].ToString() == "6")

                            {
                                Lifting.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                                Lifting.AddCell(PhraseCell(new Phrase("PERMIT SPECIFIC INSTRUCTIONS - Lifting", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                                Lifting.AddCell(PhraseCell(new Phrase("1.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                                Lifting.AddCell(PhraseCell(new Phrase("Ensure spot communication to crane operator, signalman during job execution", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                                Lifting.AddCell(PhraseCell(new Phrase("2.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                                Lifting.AddCell(PhraseCell(new Phrase("Job allowed when wind velocity is low and only upto 18:00 hours", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                                Lifting.AddCell(PhraseCell(new Phrase("3.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                                Lifting.AddCell(PhraseCell(new Phrase("Check the rope of the winch daily. Use proper wire ropes", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));


                                Lifting.AddCell(PhraseCell(new Phrase("4.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                                Lifting.AddCell(PhraseCell(new Phrase("Lifting work permit can not be extended. Make a fresh permit after time elapsed", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                                pdfDoc.Add(Lifting);

                            }

                        }
                    }


                    PdfPTable Excav = new PdfPTable(2);
                    Excav.TotalWidth = 555f;
                    Excav.LockedWidth = true;
                    Excav.SetWidths(new float[] { 2f, 38f });

                    if (dataSet.Tables[3].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[3].Rows.Count; rows++)
                        {
                            if (dataSet.Tables[3].Rows[rows][1].ToString() == "5")

                            {
                                Excav.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                                Excav.AddCell(PhraseCell(new Phrase("PERMIT SPECIFIC INSTRUCTIONS -Excavation ", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                                Excav.AddCell(PhraseCell(new Phrase("1.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                                Excav.AddCell(PhraseCell(new Phrase("Refer the below Sketch (or) attached sketch for excavation details", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));


                                pdfDoc.Add(Excav);

                            }

                        }
                    }

                    PdfPTable ExcavDraw = new PdfPTable(1);
                    ExcavDraw.TotalWidth = 555f;

                    ExcavDraw.LockedWidth = true;
                    ExcavDraw.SetWidths(new float[] { 38f });
                    if (dataSet.Tables[3].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[3].Rows.Count; rows++)
                        {
                            if (dataSet.Tables[3].Rows[rows][1].ToString() == "5")

                            {
                                Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));

                                Wpcell.FixedHeight = 130f;
                                ExcavDraw.AddCell(Wpcell);
                                pdfDoc.Add(ExcavDraw);
                            }
                        }
                    }


                    pdfDoc.Close();
                    Response.Write(pdfDoc);
                    Response.End();


                }
            }

            catch (Exception objException)
            {
                // LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);
            }

            return View();
        }
        //Contractor PDF

        public ActionResult ContractorPdf(int id)
        {
            try
            {
                using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[ContractorPdf]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@ContractID", id);
                    objCon.Open();
                    objCom.Connection = objCon;
                    SqlDataAdapter Adapter = new SqlDataAdapter();
                    Adapter.SelectCommand = objCom;
                    DataSet dataSet = new DataSet();
                    Adapter.Fill(dataSet);
                    objCon.Close();

                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition",
                        "filename=Contractor" + id + ".pdf");
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);

                    iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(iTextSharp.text.PageSize.LEGAL, 20f, 20f, 30f, 30f);
                    HTMLWorker htmlparsers = new HTMLWorker(pdfDoc);
                    PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                    pdfDoc.Open();
                    PdfPCell cell = null;
                    var dd = dataSet.Tables[0].Rows[0][20].ToString();

                    if (dataSet.Tables[0].Rows[0][20].ToString() == "D" || dataSet.Tables[0].Rows[0][20].ToString() == "S")
                    {
                        string imagePath = Server.MapPath("~/Images/watermark.png");
                        iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imagePath);

                        image.ScalePercent(200f);
                        image.RotationDegrees = 45f;
                        image.SetAbsolutePosition(0f, 200f);

                        pdfDoc.Add(image);
                    }
                    if (dataSet.Tables[0].Rows[0][20].ToString() == "A")
                    {
                        string imagePath = Server.MapPath("~/Images/approved.png");
                        iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imagePath);

                        image.ScalePercent(200f);
                        image.RotationDegrees = 45f;
                        image.SetAbsolutePosition(0f, 200f);

                        pdfDoc.Add(image);
                    }
                    //if (dataSet.Tables[0].Rows[0][20].ToString() == "R")
                    // {
                    //     string imagePath = Server.MapPath("~/Images/REJECTED.png");
                    //     iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imagePath);

                    //     image.ScalePercent(200f);
                    //     image.RotationDegrees = 45f;
                    //     image.SetAbsolutePosition(0f, 200f);

                    //     pdfDoc.Add(image);
                    // }
                    PdfPTable TitleTable = new PdfPTable(3);
                    TitleTable.LockedWidth = true;
                    TitleTable.SetWidths(new float[] { 10f, 18f, 8f });
                    TitleTable.SpacingBefore = 10f;
                    TitleTable.SpacingAfter = 1f;
                    TitleTable.TotalWidth = 555f;
                    PdfPCell Wpcell = new PdfPCell();
                    string imageURL = Server.MapPath("~/Images/SASALogo.png");
                    iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(imageURL);
                    gif.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                    gif.SpacingBefore = 10f;

                    gif.ScaleAbsolute(135f, 40f);
                    Wpcell = new PdfPCell(gif);

                    TitleTable.AddCell(Wpcell);

                    var phrase = new Phrase(new Chunk("\n SASA PTA PROJECT ", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    phrase.Add(new Chunk("ADANA,TURKEY", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    phrase.Add(new Chunk("\n\n", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    TitleTable.AddCell(PhraseCell(phrase, PdfPCell.ALIGN_CENTER));

                    Wpcell = new PdfPCell(new Phrase("\n Contractor Assessment Form", FontFactory.GetFont("Times New Roman", 12, iTextSharp.text.Font.BOLD)));
                    Wpcell.HorizontalAlignment = Element.ALIGN_CENTER;

                    TitleTable.AddCell(Wpcell);
                    Font zapfdingbats = new Font(Font.FontFamily.ZAPFDINGBATS);

                    String Tickmark = "\u0033";
                    Paragraph Tick = new Paragraph(Tickmark, zapfdingbats);

                    String FONT = "C:/Windows/Fonts/wingding.ttf";
                    String CheckedCheckboxText = "\u00fe";
                    String BlankCheckboxText = "o";
                    BaseFont bf = BaseFont.CreateFont(FONT, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    iTextSharp.text.Font f = new iTextSharp.text.Font(bf, 10);
                    Paragraph CheckedCheckbox = new Paragraph(CheckedCheckboxText, f);
                    Paragraph uncheckbox = new Paragraph(BlankCheckboxText, f);

                    String Blank = "";
                    Paragraph uncheckblank = new Paragraph(Blank, f);
                    pdfDoc.Add(TitleTable);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable Contractordetails = new PdfPTable(4);
                    Contractordetails.TotalWidth = 555f;
                    Contractordetails.LockedWidth = true;
                    Contractordetails.SetWidths(new float[] { 8f, 9f, 9f, 12f });

                    Contractordetails.AddCell(PhraseCell(new Phrase("Company's Name ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][1].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("Contract Manager ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][2].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("Supervisor First Name ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][3].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("Supervisor Last Name ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][4].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("Contractor Approver ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][21].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("Frequency Of Evaluation ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][5].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("Mobile No ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][12].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("Address ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][6].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("Street ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][7].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("City", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][8].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("State ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][9].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("Country", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][10].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("Post Code ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][11].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("Email Address  ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][13].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("Nature of business ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][14].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("Assessment Date ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][17].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("Is any sub-contractor involved ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][15].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("Have you received, read, understood and willing to follow Kothari safety code of conduct ? ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][16].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("Created By ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][18].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("Created Date ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Contractordetails.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][19].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    if (dataSet.Tables[0].Rows[0][20].ToString() == "A")
                    {
                        Contractordetails.AddCell(PhraseCell(new Phrase("Approved By ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                        Contractordetails.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][22].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                        Contractordetails.AddCell(PhraseCell(new Phrase("Approved Date ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                        Contractordetails.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][23].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    }

                    pdfDoc.Add(Contractordetails);
                    var font8 = FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD);
                    var font9 = FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL);
                    var font7 = FontFactory.GetFont("Times New Roman", 11, iTextSharp.text.Font.BOLD);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable tow = new PdfPTable(2);
                    tow.TotalWidth = 555f;
                    tow.LockedWidth = true;
                    tow.SetWidths(new float[] { 2f, 38f });
                    tow.AddCell(PhraseCell(new Phrase("2", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tow.AddCell(PhraseCell(new Phrase("Types of Work", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    pdfDoc.Add(tow);

                    PdfPTable tow1 = new PdfPTable(8);
                    tow1.TotalWidth = 555f;
                    tow1.LockedWidth = true;
                    tow1.SetWidths(new float[] { 2f, 8f, 2f, 8f, 2f, 8f, 2f, 8f });


                    if (dataSet.Tables[1].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[1].Rows.Count; rows++)
                        {

                            tow1.AddCell(dataSet.Tables[1].Rows[rows][1].ToString() == "1" ? CheckedCheckbox : uncheckbox);

                            Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[1].Rows[rows][2].ToString(), font9)));
                            tow1.AddCell(Wpcell);

                        }
                    }
                    else
                    {
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Applicable", font9)));
                        Wpcell.Colspan = 8;
                        tow1.AddCell(Wpcell);

                    }
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    tow1.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    tow1.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    tow1.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    tow1.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("")));
                    Wpcell.Colspan = 8;
                    tow1.AddCell(Wpcell);
                    pdfDoc.Add(tow1);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));
                    PdfPTable hsr = new PdfPTable(4);
                    hsr.TotalWidth = 555f;
                    hsr.LockedWidth = true;
                    hsr.SetWidths(new float[] { 2f, 18f, 4f, 14f });
                    hsr.AddCell(PhraseCell(new Phrase("S.No", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    hsr.AddCell(PhraseCell(new Phrase("Occupational Health & Safety Requirement", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    hsr.AddCell(PhraseCell(new Phrase("Yes/No/NA", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    hsr.AddCell(PhraseCell(new Phrase("Remarks", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    if (dataSet.Tables[2].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[2].Rows.Count; rows++)
                        {
                            //Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[2].Rows[rows][0].ToString(), font9)));
                            hsr.AddCell(PhraseCell(new Phrase(dataSet.Tables[2].Rows[rows][0].ToString(), font9), PdfPCell.ALIGN_CENTER));
                            hsr.AddCell(PhraseCell(new Phrase(dataSet.Tables[2].Rows[rows][2].ToString(), font9), PdfPCell.ALIGN_LEFT));
                            hsr.AddCell(PhraseCell(new Phrase(dataSet.Tables[2].Rows[rows][3].ToString(), font9), PdfPCell.ALIGN_CENTER));
                            hsr.AddCell(PhraseCell(new Phrase(dataSet.Tables[2].Rows[rows][4].ToString(), font9), PdfPCell.ALIGN_CENTER));
                            hsr.AddCell(Wpcell);

                            if (rows == 14)
                            {
                                break;
                            }
                        }
                    }
                    pdfDoc.Add(hsr);
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable emptrained = new PdfPTable(4);
                    emptrained.TotalWidth = 555f;
                    emptrained.LockedWidth = true;
                    emptrained.SetWidths(new float[] { 2f, 18f, 4f, 14f });
                    emptrained.AddCell(PhraseCell(new Phrase("S.No", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    emptrained.AddCell(PhraseCell(new Phrase("Whether your employees are trained on below topics", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    emptrained.AddCell(PhraseCell(new Phrase("Yes/No/NA", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    emptrained.AddCell(PhraseCell(new Phrase("Remarks", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                    if (dataSet.Tables[2].Rows.Count > 0)
                    {
                        for (int rows = 15; rows < dataSet.Tables[2].Rows.Count; rows++)
                        {

                            emptrained.AddCell(PhraseCell(new Phrase(dataSet.Tables[2].Rows[rows][0].ToString(), font9), PdfPCell.ALIGN_CENTER));
                            emptrained.AddCell(PhraseCell(new Phrase(dataSet.Tables[2].Rows[rows][2].ToString(), font9), PdfPCell.ALIGN_LEFT));
                            emptrained.AddCell(PhraseCell(new Phrase(dataSet.Tables[2].Rows[rows][3].ToString(), font9), PdfPCell.ALIGN_CENTER));
                            emptrained.AddCell(PhraseCell(new Phrase(dataSet.Tables[2].Rows[rows][4].ToString(), font9), PdfPCell.ALIGN_CENTER));
                            emptrained.AddCell(Wpcell);

                        }
                    }
                    pdfDoc.Add(emptrained);
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    //PdfPTable img = new PdfPTable(2);
                    //img.TotalWidth = 555f;
                    //img.LockedWidth = true;
                    //img.KeepTogether = true;
                    //img.SetWidths(new float[] { 2f, 28f });
                    //Wpcell = new PdfPCell(new Phrase(new Chunk("5", font8)));
                    //img.AddCell(Wpcell);
                    //Wpcell = new PdfPCell(new Phrase(new Chunk("Signed Attachment", font8)));
                    //img.AddCell(Wpcell);


                    //if (dataSet.Tables[3].Rows.Count > 0)
                    //{
                    //    for (int rows = 0; rows < dataSet.Tables[3].Rows.Count; rows++)
                    //    {

                    //        string imagePath1 = Server.MapPath("~/ContractAttachment/") + Path.GetFileName(dataSet.Tables[3].Rows[rows][0].ToString());

                    //        iTextSharp.text.Image image1 = iTextSharp.text.Image.GetInstance(imagePath1);
                    //        image1.ScaleAbsolute(500f, 650f);
                    //        Wpcell = new PdfPCell(image1);
                    //        Wpcell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    //        Wpcell.Colspan = 2;
                    //        img.AddCell(Wpcell);
                    //    }
                    //}
                    //else
                    //{
                    //    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    //    img.AddCell(Wpcell);
                    //    Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Applicable", font9)));
                    //    img.AddCell(Wpcell);

                    //}
                    //pdfDoc.Add(img);

                    pdfDoc.Close();
                    Response.Write(pdfDoc);
                    Response.End();
                }
            }
            catch (Exception objException)
            {
                // LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);
            }


            return View();
        }

        private static PdfPCell PhraseCell(Phrase phrase, int align)
        {
            PdfPCell cell = new PdfPCell(phrase);
            //  cell.BorderColor = Color.;
            //cell.VerticalAlignment = PdfCell.ALIGN_TOP;
            cell.HorizontalAlignment = align;
            cell.PaddingBottom = 2f;
            cell.PaddingTop = 0f;
            return cell;
        }



        private List<Equipment> GetequipmentList()
        {
            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "EquipmentSelect";
                objCom.CommandType = CommandType.StoredProcedure;
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();
                var equipmentList = new List<Equipment>();

                while (objReader.Read())
                {
                    var EquipmentList = new Equipment();
                    EquipmentList.EquipmentID = int.Parse(objReader["EquipmentID"].ToString());
                    EquipmentList.EquipmentName = objReader["EquipmentName"].ToString();
                    equipmentList.Add(EquipmentList);
                }

                objCon.Close();

                return equipmentList;
            }
        }


        private List<NatureOfBusinessMaster> GetNOBusinessMaster()
        {
            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "NOBusinessTypeSelect";
                objCom.CommandType = CommandType.StoredProcedure;
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();
                var natureOfBusinessMaster = new List<NatureOfBusinessMaster>();

                while (objReader.Read())
                {
                    var NatureOfBusines = new NatureOfBusinessMaster();
                    NatureOfBusines.NOBusinessTypeID = int.Parse(objReader["NOBusinessTypeID"].ToString());
                    NatureOfBusines.NOBusinessType = objReader["NOBusinessType"].ToString();
                    natureOfBusinessMaster.Add(NatureOfBusines);
                }

                objCon.Close();

                return natureOfBusinessMaster;
            }



        }


        [HttpGet]
        public ActionResult CreateContract()
        {

            Contract contract = new Contract();
            contract.CurrentSessionID = CurrentUser.CurrentSessionID;
            contract.PrevoiusSessionID = sess.SessionActive;
            if (contract.CurrentSessionID == contract.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            AssignTypeofWorkForApproverModel assignApprover = new AssignTypeofWorkForApproverModel();
            assignApprover = workPermitBLL.AssignApprover();
            contract.NatureOfBusiness = GetNOBusinessMaster();
            contract.FrequencyOfEvaluation = workPermitBLL.GetfrequencyOfEvaluationList();
            contract.ContractApprover = contractlist.ContractApprover;
            contract.WorkType = workPermitBLL.GetWorkTypeContractor();
            contract.ValsparContactList = assignApprover.ContractorApprover.ToList();
            contract.AssessmentDate = DateTime.Now.ToString("dd/MM/yyyy");
            contract.OccupationalHealthSafety = workPermitBLL.GetContractorCheckList();
            contract.ValsparManager = CurrentUser.UserID;
            contract.Roles = CurrentUser.Roles;
            contract.UserFullName = CurrentUser.FullName;
            contract.ProfileImage = CurrentUser.ProfileImage;
            contract.IsRestrict = CurrentUser.IsRestrict;
            return View(contract);
        }
        [HttpPost]
        public ActionResult CreateContract(Contract contract)
        {
            int affectedcount = 0;
            try
            {
                if (contract.Acknowledgement != null)
                {
                    var fileName = System.IO.Path.GetFileName(contract.Acknowledgement.FileName);
                    var path = System.IO.Path.Combine(Server.MapPath("~/ContractAttachment/"), fileName);
                    contract.Acknowledgement.SaveAs(path);
                    contract.Attachment = fileName;
                }

                using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "ContractInsert";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@CompanyName", contract.CompanyName);
                    objCom.Parameters.AddWithValue("@ValsparContact", contract.ContactID);
                    objCom.Parameters.AddWithValue("@SupervisorFirstName", contract.SupervisorFirstName);
                    objCom.Parameters.AddWithValue("@SupervisorLastName", contract.SupervisorLastName);
                    objCom.Parameters.AddWithValue("@FrequencyID", contract.FrequencyID);
                    objCom.Parameters.AddWithValue("@Address", contract.Address);
                    objCom.Parameters.AddWithValue("@Country", contract.Country);
                    objCom.Parameters.AddWithValue("@State", contract.State);
                    objCom.Parameters.AddWithValue("@City", contract.City);
                    objCom.Parameters.AddWithValue("@Street", contract.Street);
                    objCom.Parameters.AddWithValue("@EmailAddress", contract.EmailAddress);
                    objCom.Parameters.AddWithValue("@MobileNo", contract.MobileNo);
                    objCom.Parameters.AddWithValue("@PostCode", contract.PostCode);
                    objCom.Parameters.AddWithValue("@NOBusinessTypeID", contract.NOBusinessTypeID);
                    objCom.Parameters.AddWithValue("@SubcontractorInvolved", contract.SubcontractorInvolved ? 1 : 0);
                    objCom.Parameters.AddWithValue("@Have", contract.Have ? 1 : 0);
                    objCom.Parameters.AddWithValue("@Attachment", contract.Attachment);
                    objCom.Parameters.AddWithValue("@IsactiveSelect", contract.Isactiveselect ? 1 : 0);
                    objCom.Parameters.AddWithValue("@AssessmentDate", DateTime.ParseExact(contract.AssessmentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                    objCom.Parameters.AddWithValue("@CreatedBy", CurrentUser.UserID);
                    objCom.Parameters.AddWithValue("@ContractStatus", contract.ContractStatus);

                    objCom.Parameters.AddWithValue("@ValsparManager", contract.ValsparManager);

                    var contractID = new SqlParameter();
                    contractID.ParameterName = "@ContractID";
                    contractID.Direction = ParameterDirection.Output;
                    contractID.Size = int.MaxValue;
                    objCom.Parameters.Add(contractID);
                    objCon.Open();
                    objCom.Connection = objCon;
                    affectedcount = objCom.ExecuteNonQuery();

                    contract.ContractID = int.Parse(contractID.Value.ToString());
                    var workype = workPermitBLL.WorkTypeContractInsert(contract);
                    var checklist = workPermitBLL.ContractQuestionnaireInsert(contract);

                    if (affectedcount > 0)
                    {
                        TempData["Message"] = string.Format("Contractor ID_ {0} is created successfully", contract.ContractID);
                    }

                }
                return RedirectToAction("ContractList");
            }
            catch (Exception ex)
            {
                ViewBag.IsValidationFailed = true;
                ModelState.AddModelError("ValidationError", ex.Message);
               // throw new Exception(ex.Message);
            }
            AssignTypeofWorkForApproverModel assignApprover = new AssignTypeofWorkForApproverModel();
            assignApprover = workPermitBLL.AssignApprover();
            assignApprover = workPermitBLL.AssignApprover();
            contract.NatureOfBusiness = GetNOBusinessMaster();
            contract.FrequencyOfEvaluation = workPermitBLL.GetfrequencyOfEvaluationList();
            contract.ContractApprover = contractlist.ContractApprover;
            contract.WorkType = workPermitBLL.GetWorkTypeContractor();
            contract.ValsparContactList = assignApprover.ContractorApprover.ToList();
            contract.OccupationalHealthSafety = workPermitBLL.GetContractorCheckList();
            return View(contract);
        }


        [HttpGet]
        public ActionResult ContractList()
        {
            ContractList PendingContractList = null;
            AssignTypeofWorkForApproverModel assignApprover = new AssignTypeofWorkForApproverModel();
            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "PendingContractList";
                objCom.CommandType = CommandType.StoredProcedure;
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();

                var contractList = new List<Contract>();
                while (objReader.Read())
                {
                    var contract = new Contract();
                    contract.ContractID = int.Parse(objReader["ContractID"].ToString());
                    contract.CompanyName = objReader["CompanyName"].ToString();
                    contract.SupervisorFirstName = objReader["SupervisorFirstName"].ToString();

                    contract.EmailAddress = objReader["EmailAddress"].ToString();
                    contract.MobileNo = objReader["MobileNo"].ToString();
                    contract.FrequencyName = objReader["Frequency"].ToString();
                    contract.LastAssessmentDate = objReader["LastAssessmentDate"].ToString();
                    contract.NextAssessmentDate = objReader["NextAssessmentDate"].ToString();

                    contract.ContactPerson = objReader["ValsparContact"].ToString();

                    contract.Status = objReader["Status"].ToString();
                    contract.ContractStatus = objReader["ContractStatus"].ToString();
                    if (objReader["Approver"] != DBNull.Value)
                    {
                        contract.ContactID = int.Parse(objReader["Approver"].ToString());
                    }

                    contract.ContractorCreatedBy = objReader["CreatedBy"].ToString();
                    contract.ApproverComments = objReader["Comments"].ToString();
                    if (objReader["updatedby"] != DBNull.Value)
                    {
                        contract.updatedby = int.Parse(objReader["updatedby"].ToString());
                    }
                    contract.Attachment = objReader["Attachment"].ToString();
                    contractList.Add(contract);
                }
                objCon.Close();

                PendingContractList = new ContractList();
                PendingContractList.UserID = CurrentUser.UserID;
                PendingContractList.IsRestrict = CurrentUser.IsRestrict;
                PendingContractList.Contract = contractList;
            }
            PendingContractList.CurrentSessionID = CurrentUser.CurrentSessionID;
            PendingContractList.PrevoiusSessionID = sess.SessionActive;
            if (PendingContractList.CurrentSessionID == PendingContractList.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            PendingContractList.Roles = CurrentUser.Roles;
            PendingContractList.UserFullName = CurrentUser.FullName;
            PendingContractList.ProfileImage = CurrentUser.ProfileImage;
            PendingContractList.IsRestrict = CurrentUser.IsRestrict;
            assignApprover = workPermitBLL.AssignApprover();

            PendingContractList.ContractApprover = contractlist.ContractApprover;
            PendingContractList.ValsparContactList = assignApprover.ContractorApprover.ToList();
            return View(PendingContractList);

        }
        [HttpPost]
        public ActionResult ContractList(SearchContractorList contractorList)
        {
            var userListt = new AdminDA().SelectUserProfile();

            ContractList PendingContractList = null;
            AssignTypeofWorkForApproverModel assignApprover = new AssignTypeofWorkForApproverModel();
            PendingContractList = new ContractList();
            PendingContractList.UserID = CurrentUser.UserID;
            //PendingContractList.Contract = contractList;
            PendingContractList.CurrentSessionID = CurrentUser.CurrentSessionID;
            PendingContractList.PrevoiusSessionID = sess.SessionActive;
            if (PendingContractList.CurrentSessionID == PendingContractList.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            var searchcontractlist = workPermitBLL.SearchContractorList(contractorList);
            PendingContractList.Contract = searchcontractlist;
            PendingContractList.Roles = CurrentUser.Roles;
            PendingContractList.UserFullName = CurrentUser.FullName;
            PendingContractList.ProfileImage = CurrentUser.ProfileImage;
            PendingContractList.IsRestrict = CurrentUser.IsRestrict;
            assignApprover = workPermitBLL.AssignApprover();

            PendingContractList.ContractApprover = contractlist.ContractApprover;
            PendingContractList.ValsparContactList = assignApprover.ContractorApprover.ToList();

            //PendingContractList.ValsparContactList = userListt.Where(y => y.UserID == PendingContractList.searchCon.ContractManager).ToList();

            //PendingContractList.ContractApprover = userListt.Where(y => y.UserID == PendingContractList.searchCon.Approver).ToList();

            return View(PendingContractList);
        }
        public ActionResult ExportContractorList()
        {

            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "ExportContractorList";
                objCom.CommandType = CommandType.StoredProcedure;

                objCon.Open();
                objCom.Connection = objCon;
                SqlDataAdapter Adapter = new SqlDataAdapter();
                Adapter.SelectCommand = objCom;
                DataSet dataSet = new DataSet();
                Adapter.Fill(dataSet);
                objCon.Close();
                var wb = new XLWorkbook(Server.MapPath("~/Template/ContractorList.xlsx"));
                var worksheet = wb.Worksheet("ContratorList");
                worksheet.Cell("C4").Value = "Report Generated by : " + CurrentUser.FullName;
                //worksheet.Cell("D4").Value = CurrentUser.FullName;

                //worksheet.Cell("D5").Value = currentFromDate + " to  " + currentEndDate;

                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    var rangeWithDataSecond = worksheet.Cell(7, 1).InsertTable(dataSet.Tables[0].AsEnumerable());
                }
                else
                {
                    worksheet.Cell("C8").Value = "No Data Found";
                }

                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename= ContractorList.xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

            }


            return View();


        }
        [HttpPost]
        public ActionResult ContractorApproverStatus(int ContractID, string StatusID, string Comments)
        {
            string strMessage = String.Empty;
            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "ContractorApproverStatus";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@ContractorID", ContractID);
                    objCom.Parameters.AddWithValue("@StatusID", StatusID);
                    objCom.Parameters.AddWithValue("@Comments", Comments);
                    objCom.Parameters.AddWithValue("@UserID", CurrentUser.UserID);

                    objCom.Connection = objCon;
                    objCon.Open();
                    affectedRecordCount = objCom.ExecuteNonQuery();
                    objCon.Close();

                    if (StatusID == "A")
                    {
                        var contract = workPermitBLL.GetContract(ContractID);
                        var userListt = new AdminDA().SelectUserProfile();
                        contract.ContractApprover = userListt.Where(y => y.UserID == contract.ContactID).ToList();
                        contract.ContractCreated = userListt.Where(y => y.UserID == contract.ValsparManager).ToList();

                        string ToAddress;
                        string CCAddress;
                        if ((contract.ContractCreated[0].EmailAddress != "") && (contract.ContractApprover[0].EmailAddress != ""))
                        {
                            ToAddress = contract.ContractCreated[0].EmailAddress;
                            CCAddress = contract.ContractApprover[0].EmailAddress;
                            string Subject = "Contractor Assessment form approved".ToString();
                            string Sender = CurrentUser.FullName.ToString();
                            string Receiver = contract.ContractCreated[0].FirstName + " " + contract.ContractCreated[0].LastName.ToString();
                            string Statement = "Contractor Company:" + contract.CompanyName
                                + "\nThe contractor assessment form for the above company is approved.".ToString();
                            ContractorApprovermail(ToAddress, CCAddress, Sender, Receiver, Subject, Statement);
                        }

                    }
                    else if (StatusID == "R")
                    {
                        var contract = workPermitBLL.GetContract(ContractID);
                        var userListt = new AdminDA().SelectUserProfile();
                        contract.ContractApprover = userListt.Where(y => y.UserID == contract.ContactID).ToList();
                        contract.ContractCreated = userListt.Where(y => y.UserID == contract.ValsparManager).ToList();

                        string ToAddress;
                        string CCAddress;
                        if ((contract.ContractCreated[0].EmailAddress != "") && (contract.ContractApprover[0].EmailAddress != ""))
                        {
                            ToAddress = contract.ContractCreated[0].EmailAddress;
                            CCAddress = contract.ContractApprover[0].EmailAddress;
                            string Subject = "Contractor Assessment form Recycled".ToString();
                            string Sender = CurrentUser.FullName.ToString();
                            string Receiver = contract.ContractCreated[0].FirstName + " " + contract.ContractCreated[0].LastName.ToString();
                            string Statement = "Contractor Company:" + contract.CompanyName
                                + "\nThe contractor assessment form for the above company is suggested for recycle.".ToString();
                            ContractorApprovermail(ToAddress, CCAddress, Sender, Receiver, Subject, Statement);
                        }
                    }

                }
                strMessage = "Data Saved Successfully";
            }
            catch (Exception ex)
            {
                strMessage = ex.Message;
            }
            return Json(new { strMessage });
        }

        [HttpGet]
        public ActionResult UpdateContract(int Id)
        {
            var contract = workPermitBLL.GetContract(Id);
            contract.CurrentSessionID = CurrentUser.CurrentSessionID;
            contract.PrevoiusSessionID = sess.SessionActive;
            if (contract.CurrentSessionID == contract.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            AssignTypeofWorkForApproverModel assignApprover = new AssignTypeofWorkForApproverModel();
            assignApprover = workPermitBLL.AssignApprover();
            contract.NatureOfBusiness = GetNOBusinessMaster();
            contract.FrequencyOfEvaluation = workPermitBLL.GetfrequencyOfEvaluationList();
            contract.WorkType = workPermitBLL.GetWorkTypeContractor(Id);
            contract.OccupationalHealthSafety = workPermitBLL.GetContractorCheckList(Id);
            contract.ContractApprover = contractlist.ContractApprover;
            contract.ValsparContactList = assignApprover.ContractorApprover.ToList();
            contract.UserID = CurrentUser.UserID;
            contract.Roles = CurrentUser.Roles;
            contract.UserFullName = CurrentUser.FullName;
            contract.ProfileImage = CurrentUser.ProfileImage;
            contract.IsRestrict = CurrentUser.IsRestrict;

            return View(contract);

        }

        [HttpPost]
        public ActionResult UpdateContract(Contract contract)
        {
            int affectedcount = 0;

            try
            {
                if (contract.Acknowledgement != null)
                {
                    var fileName = System.IO.Path.GetFileName(contract.Acknowledgement.FileName);
                    var path = System.IO.Path.Combine(Server.MapPath("~/ContractAttachment/"), fileName);
                    contract.Acknowledgement.SaveAs(path);
                    contract.Attachment = fileName;
                }
                using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "ContractUpdate";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@ContractID", contract.ContractID);
                    objCom.Parameters.AddWithValue("@CompanyName", contract.CompanyName);
                    objCom.Parameters.AddWithValue("@ValsparContact", contract.ContactID);
                    objCom.Parameters.AddWithValue("@SupervisorFirstName", contract.SupervisorFirstName);
                    objCom.Parameters.AddWithValue("@SupervisorLastName", contract.SupervisorLastName);
                    objCom.Parameters.AddWithValue("@FrequencyID", contract.FrequencyID);
                    objCom.Parameters.AddWithValue("@Address", contract.Address);
                    objCom.Parameters.AddWithValue("@Country", contract.Country);
                    objCom.Parameters.AddWithValue("@State", contract.State);
                    objCom.Parameters.AddWithValue("@City", contract.City);
                    objCom.Parameters.AddWithValue("@Street", contract.Street);
                    objCom.Parameters.AddWithValue("@EmailAddress", contract.EmailAddress);
                    objCom.Parameters.AddWithValue("@MobileNo", contract.MobileNo);
                    objCom.Parameters.AddWithValue("@PostCode", contract.PostCode);
                    objCom.Parameters.AddWithValue("@NOBusinessTypeID", contract.NOBusinessTypeID);
                    objCom.Parameters.AddWithValue("@SubcontractorInvolved", contract.SubcontractorInvolved ? 1 : 0);
                    objCom.Parameters.AddWithValue("@Have", contract.Have ? 1 : 0);
                    objCom.Parameters.AddWithValue("@IsactiveSelect", contract.Isactiveselect ? 1 : 0);
                    objCom.Parameters.AddWithValue("@Attachment", contract.Attachment);
                    objCom.Parameters.AddWithValue("@AssessmentDate", DateTime.ParseExact(contract.AssessmentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                    objCom.Parameters.AddWithValue("@LastUpdatedBy", CurrentUser.UserID);
                    objCom.Parameters.AddWithValue("@contractStatus", contract.ContractStatus);
                    objCom.Parameters.AddWithValue("@ValsparManager", contract.ValsparManager);

                    objCon.Open();
                    objCom.Connection = objCon;

                    affectedcount = objCom.ExecuteNonQuery();

                    var workype = workPermitBLL.WorkTypeContractInsert(contract);
                    var checklist = workPermitBLL.ContractQuestionnaireInsert(contract);
                    objCon.Close();

                    if (contract.ContractStatus == "S")
                    {
                        //workPermit.WorkType = GetWorkType();
                        //   var selectWorkTypelist = workPermit.WorkType.Where(y => y.WorkTypeID == workPermit.WorkTypeID).ToList();
                        var userListt = new AdminDA().SelectUserProfile();

                        contract.ContractApprover = userListt.Where(y => y.UserID == contract.ContactID).ToList();
                        contract.ContractCreated = userListt.Where(y => y.UserID == contract.ValsparManager).ToList();
                        string ToAddress;
                        string CCAddress;
                        ToAddress = contract.ContractApprover[0].EmailAddress;
                        CCAddress = contract.ContractCreated[0].EmailAddress;

                        string Subject = "Contractor Assessment form submitted for approval".ToString();
                        string Sender = CurrentUser.FullName.ToString();
                        string Receiver = contract.ContractApprover[0].FirstName + " " + contract.ContractApprover[0].LastName.ToString();
                        string Statement = "Contractor Company:" + contract.CompanyName + "\n"
                           + "The contractor assessment form for the above company has been submitted for your review and approval.\n".ToString();

                        ContractorApprovermail(ToAddress, CCAddress, Sender, Receiver, Subject, Statement);
                    }


                    if (affectedcount > 0 && contract.ContractStatus == "D")
                    {
                        TempData["Message"] = string.Format("Contractor ID_ {0} is updated successfully", contract.ContractID);
                    }
                    else
                    {
                        TempData["Status"] = "S";
                        ViewBag.Submitted = string.Format("Contractor ID_ {0} is submitted for approval", contract.ContractID);
                    }
                }


            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            AssignTypeofWorkForApproverModel assignApprover = new AssignTypeofWorkForApproverModel();
            assignApprover = workPermitBLL.AssignApprover();
            contract.NatureOfBusiness = GetNOBusinessMaster();
            contract.FrequencyOfEvaluation = workPermitBLL.GetfrequencyOfEvaluationList();
            contract.ValsparContactList = assignApprover.ContractorApprover.ToList();
            contract.ContractApprover = contractlist.ContractApprover;
            contract.WorkType = workPermitBLL.GetWorkTypeContractor(contract.ContractID);
            contract.OccupationalHealthSafety = workPermitBLL.GetContractorCheckList(contract.ContractID);
            contract.Roles = CurrentUser.Roles;
            contract.UserFullName = CurrentUser.FullName;
            contract.ProfileImage = CurrentUser.ProfileImage;
            contract.IsRestrict = CurrentUser.IsRestrict;

            return View(contract);
        }

        [HttpGet]
        public ActionResult ContractAnnualRating()
        {
            ContractList ContractAnnualRatingList = new ContractList();
            ContractAnnualRatingList.CurrentSessionID = CurrentUser.CurrentSessionID;
            ContractAnnualRatingList.PrevoiusSessionID = sess.SessionActive;
            if (ContractAnnualRatingList.CurrentSessionID == ContractAnnualRatingList.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            // ContractAnnualRatingList.FromDate = DateTime.Now.ToString("dd/MM/yyyy 00:00:00");
            //ContractAnnualRatingList.Todate = DateTime.Now.ToString("dd/MM/yyyy 23:59:00");
            ViewBag.fromdate = ContractAnnualRatingList.FromDate;
            ViewBag.Todate = ContractAnnualRatingList.Todate;
            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "SelectContractAnnualRating";
                objCom.CommandType = CommandType.StoredProcedure;
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();

                var RatingList = new List<ContractAnnualRating>();
                while (objReader.Read())
                {
                    var contractAnnualRating = new ContractAnnualRating();


                    contractAnnualRating.ContractorName = objReader["ContractorName"].ToString();
                    contractAnnualRating.TotalPermits = int.Parse(objReader["TotalPermits"].ToString());
                    contractAnnualRating.GreenPermits = int.Parse(objReader["GreenPermits"].ToString());
                    contractAnnualRating.orangePermits = int.Parse(objReader["OrangePermits"].ToString());
                    contractAnnualRating.RedPermits = int.Parse(objReader["RedPermits"].ToString());
                    contractAnnualRating.TotalPercentage = int.Parse(objReader["TotalPercentage"].ToString());
                    contractAnnualRating.Rank = int.Parse(objReader["Rank"].ToString());
                    contractAnnualRating.AssesmentFrequency = objReader["AssessmentFrequency"].ToString();
                    contractAnnualRating.LastAssessmentDate = objReader["LastAssessmentDate"].ToString();
                    contractAnnualRating.NextAssessmentDate = objReader["NextAssessmentDate"].ToString();
                    contractAnnualRating.AttachmentName = objReader["Attachment"].ToString();
                    contractAnnualRating.RatingStatus = objReader["RatingStatus"].ToString();
                    RatingList.Add(contractAnnualRating);
                }
                objCon.Close();


                ContractAnnualRatingList.RatingList = RatingList;

            }
            ContractAnnualRatingList.Roles = CurrentUser.Roles;
            ContractAnnualRatingList.UserFullName = CurrentUser.FullName;
            ContractAnnualRatingList.ProfileImage = CurrentUser.ProfileImage;
            ContractAnnualRatingList.IsRestrict = CurrentUser.IsRestrict;
            return View(ContractAnnualRatingList);

        }

        [HttpPost]
        public ActionResult ContractAnnualRating(ContractList contractAnnualRatingList)
        {

            ViewBag.fromdate = contractAnnualRatingList.FromDate;
            ViewBag.Todate = contractAnnualRatingList.Todate;
            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "SearchContractAnnualRating";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@Fromdate", contractAnnualRatingList.FromDate);
                objCom.Parameters.AddWithValue("@Todate", contractAnnualRatingList.Todate);

                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();

                var RatingList = new List<ContractAnnualRating>();
                while (objReader.Read())
                {
                    var contractAnnualRating = new ContractAnnualRating();
                    contractAnnualRating.ContractorName = objReader["ContractorName"].ToString();
                    contractAnnualRating.TotalPermits = int.Parse(objReader["TotalPermits"].ToString());
                    contractAnnualRating.GreenPermits = int.Parse(objReader["GreenPermits"].ToString());
                    contractAnnualRating.orangePermits = int.Parse(objReader["OrangePermits"].ToString());
                    contractAnnualRating.RedPermits = int.Parse(objReader["RedPermits"].ToString());
                    contractAnnualRating.TotalPercentage = int.Parse(objReader["TotalPercentage"].ToString());
                    contractAnnualRating.Rank = int.Parse(objReader["Rank"].ToString());
                    contractAnnualRating.AssesmentFrequency = objReader["AssessmentFrequency"].ToString();
                    contractAnnualRating.LastAssessmentDate = objReader["LastAssessmentDate"].ToString();
                    contractAnnualRating.NextAssessmentDate = objReader["NextAssessmentDate"].ToString();
                    contractAnnualRating.AttachmentName = objReader["Attachment"].ToString();
                    contractAnnualRating.RatingStatus = objReader["RatingStatus"].ToString();
                    RatingList.Add(contractAnnualRating);
                }
                objCon.Close();


                contractAnnualRatingList.RatingList = RatingList;

            }

            contractAnnualRatingList.Roles = CurrentUser.Roles;
            contractAnnualRatingList.UserFullName = CurrentUser.FullName;
            contractAnnualRatingList.ProfileImage = CurrentUser.ProfileImage;
            contractAnnualRatingList.IsRestrict = CurrentUser.IsRestrict;
            return View(contractAnnualRatingList);


        }
        public ActionResult ExportContractRatingList(string currentFromDate, string currentTodate)
        {

            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "ExportContractRating";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@Fromdate", currentFromDate);
                objCom.Parameters.AddWithValue("@Todate", currentTodate);

                objCon.Open();
                objCom.Connection = objCon;
                SqlDataAdapter Adapter = new SqlDataAdapter();
                Adapter.SelectCommand = objCom;
                DataSet dataSet = new DataSet();
                Adapter.Fill(dataSet);
                objCon.Close();
                var wb = new XLWorkbook(Server.MapPath("~/Template/ContractRatingList.xlsx"));
                // var worksheet = wb.Worksheet("ContractRatingList");
                var worksheet = wb.Worksheet(1);
                worksheet.Cell("C4").Value = "Report Generated by : " + CurrentUser.FullName;

                worksheet.Cell("C5").Value = "Report Duration : ";
                worksheet.Cell("D5").Value = currentFromDate + " to  " + currentTodate;

                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    var rangeWithDataSecond = worksheet.Cell(7, 1).InsertTable(dataSet.Tables[0].AsEnumerable());
                }
                else
                {
                    worksheet.Cell("C8").Value = "No Data Found";
                }

                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename= ContractRatingList.xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

            }


            return View();


        }
        [HttpGet]
        public ActionResult ContractorEvaluation(int Id)
        {
            var evaluation = new Evaluation();
            evaluation.CurrentSessionID = CurrentUser.CurrentSessionID;
            evaluation.PrevoiusSessionID = sess.SessionActive;
            if (evaluation.CurrentSessionID == evaluation.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            evaluation = workPermitBLL.GetEvaluation(Id);
            evaluation.EvaluationCriteriaCheckList = workPermitBLL.GetEvaluationCheckList(Id);
            evaluation.Ratinglist = workPermitBLL.GetRatingList();
            evaluation.ContractorID = Id;
            // evaluation.EvaluationDate = DateTime.Now.ToString("dd/MM/yyyy");
            evaluation.EvaluatedBy = CurrentUser.FullName;
            evaluation.Roles = CurrentUser.Roles;
            evaluation.UserFullName = CurrentUser.FullName;
            evaluation.ProfileImage = CurrentUser.ProfileImage;
            evaluation.IsRestrict = CurrentUser.IsRestrict;
            return View(evaluation);
        }
        [HttpPost]
        public ActionResult ContractorEvaluation(Evaluation evaluation)
        {
            int affectedCount = 0;
            if (evaluation.Attachment != null)
            {
                var fileName = System.IO.Path.GetFileName(evaluation.Attachment.FileName);
                var path = System.IO.Path.Combine(Server.MapPath("~/EvaluationAttachment/"), fileName);
                evaluation.Attachment.SaveAs(path);
                evaluation.AttachmentName = fileName;
            }
            try
            {
                using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "EvaluationFormInsert";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@ContractorID", evaluation.ContractorID);
                    objCom.Parameters.AddWithValue("@EvaluationPeriod", evaluation.EvaluationPeriod);
                    objCom.Parameters.AddWithValue("@FrequencyEvaluation", evaluation.FrequencyofEvaluation);
                    objCom.Parameters.AddWithValue("@EvaluationDate", DateTime.ParseExact(evaluation.EvaluationDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    objCom.Parameters.AddWithValue("@SWContact", evaluation.SWContact);
                    objCom.Parameters.AddWithValue("@RatingID", evaluation.RatingID);
                    objCom.Parameters.AddWithValue("@UserID", CurrentUser.UserID);
                    objCom.Parameters.AddWithValue("@Attachment", evaluation.AttachmentName);
                    objCon.Open();
                    objCom.Connection = objCon;
                    affectedCount = objCom.ExecuteNonQuery();
                    int Questions = workPermitBLL.EvaluationCriteriaInsert(evaluation);
                    objCon.Close();
                    if (affectedCount > 0)
                    { ViewBag.Message = "Evaluation inserted successfully"; }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }

            evaluation = workPermitBLL.GetEvaluation(evaluation.ContractorID);
            evaluation.EvaluationCriteriaCheckList = workPermitBLL.GetEvaluationCheckList(evaluation.ContractorID);
            evaluation.Ratinglist = workPermitBLL.GetRatingList();
            evaluation.Roles = CurrentUser.Roles;
            evaluation.UserFullName = CurrentUser.FullName;
            evaluation.ProfileImage = CurrentUser.ProfileImage;
            evaluation.IsRestrict = CurrentUser.IsRestrict;
            return View(evaluation);
        }


        public ActionResult mail(string cc, string toAddress, string sender, string subject, string statement)
        {

            try
            {
                MailMessage mail = new MailMessage();
                String username = System.Configuration.ConfigurationManager.AppSettings["UserName"];  // Replace with your SMTP username.
                String password = System.Configuration.ConfigurationManager.AppSettings["Password"];   // Replace with your SMTP password.
                String host = System.Configuration.ConfigurationManager.AppSettings["smtp"];
                int port = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Port"]);
                mail.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["FromAddressWPS"]);
                mail.CC.Add(new MailAddress(cc));
                //string[] Multi = (toAddress).Split(',');
                //foreach (string multimailid in Multi)
                //{
                mail.To.Add(new MailAddress(toAddress));
                //}


                mail.Subject = subject;
                mail.Body = "Dear Sir, " + "\n\n"
               + statement + "Please log in to the SASA application and do the needful. \n\n"
                + System.Configuration.ConfigurationManager.AppSettings["Link"] + "\n"
                + "Thank You,\n"
                + sender + "\n\n Regards.\nNote: This is a system generated email.";
                using (var client = new System.Net.Mail.SmtpClient(host, port))
                {
                    client.Credentials = new System.Net.NetworkCredential(username, password);
                    client.EnableSsl = true;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.Send(mail);
                }
            }
            catch (Exception objException)
            {
                //throw objException;

                LogManager.Instance.Error(objException);

            }


            return View();
        }



        public ActionResult Contractmail(string fromAddress, string toAddress, string sender, string receiver, string subject, string statement, string Content)
        {

            try
            {
                MailMessage mail = new MailMessage();
                String username = System.Configuration.ConfigurationManager.AppSettings["UserName"];  // Replace with your SMTP username.
                String password = System.Configuration.ConfigurationManager.AppSettings["Password"];   // Replace with your SMTP password.
                String host = System.Configuration.ConfigurationManager.AppSettings["smtp"];
                int port = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Port"]);
                mail.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["FromAddressWPS"]);
                string[] Multi = (toAddress).Split(',');
                foreach (string multimailid in Multi)
                {
                    mail.To.Add(new MailAddress(multimailid));
                }


                mail.Subject = subject;

                mail.Body = "Dear Mr. " + receiver + ", \n\n"
                + "The rating for the  " + Content + " has been rated " + "Red." + " Please meet the under signed  for learnings and improvements in the future.   \n\n"

                + "Thank You,\n"
               + sender + "\n";
                using (var client = new System.Net.Mail.SmtpClient(host, port))
                {
                    client.Credentials = new System.Net.NetworkCredential(username, password);
                    client.EnableSsl = true;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.Send(mail);
                }
            }
            catch (Exception objException)
            {
                //throw objException;
                LogManager.Instance.Error(objException);

            }


            return View();
        }

        public ActionResult ContractorApprovermail(string toAddress, string ccAddress, string sender, string receiver, string subject, string statement)
        {

            try
            {
                MailMessage mail = new MailMessage();
                String username = System.Configuration.ConfigurationManager.AppSettings["UserName"];  // Replace with your SMTP username.
                String password = System.Configuration.ConfigurationManager.AppSettings["Password"];   // Replace with your SMTP password.
                String host = System.Configuration.ConfigurationManager.AppSettings["smtp"];
                string link = System.Configuration.ConfigurationManager.AppSettings["link"];
                int port = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Port"]);
                mail.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["FromAddressWPS"]);
                mail.CC.Add(new MailAddress(ccAddress));
                string[] Multi = (toAddress).Split(',');

                foreach (string multimailid in Multi)
                {
                    mail.To.Add(new MailAddress(multimailid));
                }


                mail.Subject = subject;
                mail.Body = "Dear Sir, \n\n"
                + statement + "\nPlease log in to SASA application and do the needful.\n\n"
                + link + " \n"
                + "Thank You,\n"
               + sender + "\n\n Regards.\nNote: This is a system generated email.";
                using (var client = new System.Net.Mail.SmtpClient(host, port))
                {
                    client.Credentials = new System.Net.NetworkCredential(username, password);
                    client.EnableSsl = true;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.Send(mail);
                }
            }
            catch (Exception objException)
            {
                //throw objException;

                LogManager.Instance.Error(objException);

            }


            return View();
        }


        [HttpGet]
        public ActionResult AssignTypeofWorkForApprover()
        {
            AssignTypeofWorkForApproverModel assignapprover = new AssignTypeofWorkForApproverModel();
            assignapprover.CurrentSessionID = CurrentUser.CurrentSessionID;
            assignapprover.PrevoiusSessionID = sess.SessionActive;
            if (assignapprover.CurrentSessionID == assignapprover.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            WorkPermit wp = new WorkPermit();
            try
            {
                //AdminBLL adminBLL = new AdminBLL();
                assignapprover = workPermitBLL.AssignApprover();
            }
            catch (Exception objException)
            {
                ModelState.AddModelError("Error", objException.Message);
            }
            if (CurrentUser.UserID != 0)
            {

                assignapprover.UserID = CurrentUser.UserID;
                assignapprover.UserFullName = CurrentUser.FullName;
                assignapprover.Roles = CurrentUser.Roles;
                assignapprover.ProfileImage = CurrentUser.ProfileImage;
                assignapprover.IsRestrict = CurrentUser.IsRestrict;
            }
            return View(assignapprover);




        }
        [HttpPost]
        public ActionResult AssignTypeofWorkForApprover(AssignTypeofWorkForApproverModel assignapprover)
        {
            int userID = assignapprover.SelectedUserID;
            var assignapprovers = new AssignTypeofWorkForApproverModel();
            try
            {
                AdminBLL adminBLL = new AdminBLL();


                //WorkPermit workPermit = null;
                if (Request.Form["GetApproverList"] == "1")
                {
                    assignapprover = workPermitBLL.AssignApprover();

                }
                string AssignString = string.Empty;
                int affectedcount = 0;

                List<PermitApproverXML> Approverlist = new List<PermitApproverXML>();

                foreach (var list in assignapprover.WorkType)
                {
                    foreach (var newlist in list.NewWorkTypes)
                    {
                        if (newlist.Ischecked == true)
                        {
                            var list1 = new PermitApproverXML
                            {
                                WorkTypeID = newlist.WorkTypeID,
                                UserID = list.UserID,
                                CreatedBy = CurrentUser.UserID,

                            };
                            Approverlist.Add(list1);
                        }
                    }
                }
                XmlSerializer xmlSerializer = new XmlSerializer(Approverlist.GetType());

                using (StringWriter textWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(textWriter, Approverlist);

                    AssignString = textWriter.ToString();
                }
                if (Request.Form["SaveUserEquipments"] == "1")
                {

                    using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                    {
                        SqlCommand objCom = new SqlCommand();
                        objCom.CommandText = "AssignTypeofWorkForApprover";
                        objCom.CommandType = CommandType.StoredProcedure;
                        objCom.Parameters.AddWithValue("@PermitApprover", AssignString);
                        objCon.Open();
                        objCom.Connection = objCon;

                        objCom.ExecuteNonQuery();
                        objCon.Close();
                        ViewBag.issave = "Y";

                        assignapprover = workPermitBLL.AssignApprover();
                    }
                }
            }
            catch (Exception objException)
            {
                ModelState.AddModelError("ServerSideError", objException.Message);
                ViewBag.IsServerSideError = true;
            }

            ModelState.Clear();
            //assignapprover.TypeofWorkforApprover = assignapprovers;
            assignapprover.SelectedUserID = userID;
            assignapprover.UserID = CurrentUser.UserID;
            assignapprover.Roles = CurrentUser.Roles;
            assignapprover.UserFullName = CurrentUser.FullName;
            assignapprover.ProfileImage = CurrentUser.ProfileImage;
            assignapprover.IsRestrict = CurrentUser.IsRestrict;
            return View(assignapprover);

        }

        [HttpGet]
        public ActionResult CreateContractorEmployee()
        {
            AdminBLL adminBLL = new AdminBLL();
            NewContractor createcontractemp = new NewContractor();
            createcontractemp.CurrentSessionID = CurrentUser.CurrentSessionID;
            createcontractemp.PrevoiusSessionID = sess.SessionActive;
            if (createcontractemp.CurrentSessionID == createcontractemp.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            createcontractemp.FrequencyOfEvaluation = workPermitBLL.GetfrequencyOfEvaluationList();
            createcontractemp.contractorskills = workPermitBLL.GetSkills();
            createcontractemp.trainingtype = workPermitBLL.GetTrainingType();
            GetAutoGenerateContractorEMPID(createcontractemp);
            createcontractemp.TraningDate = DateTime.Now.ToString("dd/MM/yyyy");
            createcontractemp.UserID = CurrentUser.UserID;
            createcontractemp.UserFullName = CurrentUser.FullName;
            createcontractemp.Roles = CurrentUser.Roles;
            createcontractemp.ProfileImage = CurrentUser.ProfileImage;
            createcontractemp.IsRestrict = CurrentUser.IsRestrict;
            createcontractemp.ContractorList = workPermitBLL.GetContractorCompany();
            createcontractemp.DepartmentList = adminBLL.GetDepartmentList();

            return View(createcontractemp);
        }
        [HttpPost]
        public ActionResult CreateContractorEmployee(NewContractor createcontractemp)
        {
            AdminBLL adminBLL = new AdminBLL();

            try
            {
                if (ModelState.IsValid)
                {

                    if (workPermitBLL.InsertContractorEmp(createcontractemp) > 0)
                    {
                        ViewBag.IsInsertSuccessful = true;
                    }
                }

            }
            catch (Exception objException)
            {
                ViewBag.IsValidationFailed = true;
                ModelState.AddModelError("ValidationError", objException.Message);
            }
            createcontractemp.FrequencyOfEvaluation = workPermitBLL.GetfrequencyOfEvaluationList();
            createcontractemp.contractorskills = workPermitBLL.GetSkills();
            createcontractemp.trainingtype = workPermitBLL.GetTrainingType();
            createcontractemp.ContractorList = workPermitBLL.GetContractorCompany();
            createcontractemp.DepartmentList = adminBLL.GetDepartmentList();
            createcontractemp.UserID = CurrentUser.UserID;
            createcontractemp.UserFullName = CurrentUser.FullName;
            createcontractemp.Roles = CurrentUser.Roles;
            createcontractemp.ProfileImage = CurrentUser.ProfileImage;
            createcontractemp.IsRestrict = CurrentUser.IsRestrict;
            return View(createcontractemp);
        }
        public void GetAutoGenerateContractorEMPID(NewContractor contractor)
        {

            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    objCon.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "AutoGenerateContractorEMPID";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = objCon;
                    Object value = Convert.ToString(cmd.ExecuteScalar());

                    string a = value.ToString();
                    contractor.EmployeeID = a;
                    objCon.Close();
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }

        }

        [HttpGet]
        public ActionResult ContractorEmployeeList()
        {
            AdminBLL adminBLL = new AdminBLL();

            EmpProfile newcon = new EmpProfile();
            newcon.CurrentSessionID = CurrentUser.CurrentSessionID;
            newcon.PrevoiusSessionID = sess.SessionActive;
            if (newcon.CurrentSessionID == newcon.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            var prf = workPermitBLL.SelectContractorList();
            newcon.empUserProfile = prf;
            newcon.UserID = CurrentUser.UserID;
            newcon.UserFullName = CurrentUser.FullName;
            newcon.Roles = CurrentUser.Roles;
            newcon.ProfileImage = CurrentUser.ProfileImage;
            newcon.IsRestrict = CurrentUser.IsRestrict;
            newcon.contractorskills = workPermitBLL.GetSkills();
            newcon.trainingtype = workPermitBLL.GetTrainingType();
            newcon.ContractorList = workPermitBLL.GetContractorCompany();
            newcon.DepartmentList = adminBLL.GetDepartmentList();
            return View(newcon);
        }


        [HttpPost]
        public ActionResult ContractorEmployeeList(SearchContractorEmployee searchContractor)
        {
            AdminBLL adminBLL = new AdminBLL();
            EmpProfile newcon = new EmpProfile();
            newcon.CurrentSessionID = CurrentUser.CurrentSessionID;
            newcon.PrevoiusSessionID = sess.SessionActive;
            if (newcon.CurrentSessionID == newcon.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            var prf = workPermitBLL.SearchContractorEmployee(searchContractor);
            newcon.empUserProfile = prf;
            newcon.UserID = CurrentUser.UserID;
            newcon.UserFullName = CurrentUser.FullName;
            newcon.Roles = CurrentUser.Roles;
            newcon.ProfileImage = CurrentUser.ProfileImage;
            newcon.IsRestrict = CurrentUser.IsRestrict;
            newcon.contractorskills = workPermitBLL.GetSkills();
            newcon.trainingtype = workPermitBLL.GetTrainingType();
            newcon.ContractorList = workPermitBLL.GetContractorCompany();
            newcon.DepartmentList = adminBLL.GetDepartmentList();
            return View(newcon);
        }




        [HttpGet]
        public ActionResult UpdateContractorEmployeeProfile(int id)
        {
            AdminBLL adminBLL = new AdminBLL();


            EmpContractorprofile userProfile = workPermitBLL.GetContractorEmpUserProfile(id);
            userProfile.CurrentSessionID = CurrentUser.CurrentSessionID;
            userProfile.PrevoiusSessionID = sess.SessionActive;
            if (userProfile.CurrentSessionID == userProfile.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            userProfile.UserFullName = CurrentUser.FullName;
            userProfile.Roles = CurrentUser.Roles;
            userProfile.ProfileImage = CurrentUser.ProfileImage;
            userProfile.IsRestrict = CurrentUser.IsRestrict;
            userProfile.FrequencyOfEvaluation = workPermitBLL.GetfrequencyOfEvaluationList();
            userProfile.contractorskills = workPermitBLL.GetSkills();
            userProfile.trainingtype = workPermitBLL.GetTrainingType();
            userProfile.ContractorList = workPermitBLL.GetContractorCompany();
            userProfile.DepartmentList = adminBLL.GetDepartmentList();
            return View(userProfile);
        }
        [HttpPost]
        public ActionResult UpdateContractorEmployeeProfile(EmpContractorprofile userProfile)
        {
            AdminBLL adminBLL = new AdminBLL();

            try
            {
                if (userProfile.ContractorProfile != null)
                {
                    if (userProfile.ContractorProfile.ContentLength > 100000)
                    {
                        throw new Exception("Profile image should be less than or equal to 100KB.");
                    }

                    var img = System.Drawing.Image.FromStream(userProfile.ContractorProfile.InputStream);

                    if (!img.RawFormat.Equals(ImageFormat.Jpeg))
                    {
                        throw new Exception("Profile image should be JPEG format.");
                    }

                    var fileName = Path.GetFileName(userProfile.ContractorProfile.FileName);
                    var path = Path.Combine(Server.MapPath("~/ContractorUserImage/"), fileName);
                    userProfile.ContractorProfile.SaveAs(path);

                }

                workPermitBLL.UpdateContatractEmp(userProfile);
                return RedirectToAction("ContractorEmployeeList");
            }
            catch (Exception exception)
            {

                ViewBag.IsValidationFailed = true;
                ModelState.AddModelError("ValidationError", exception.Message);
            }

            userProfile.UserFullName = CurrentUser.FullName;
            userProfile.Roles = CurrentUser.Roles;
            userProfile.ProfileImage = CurrentUser.ProfileImage;
            userProfile.IsRestrict = CurrentUser.IsRestrict;
            userProfile.FrequencyOfEvaluation = workPermitBLL.GetfrequencyOfEvaluationList();
            userProfile.contractorskills = workPermitBLL.GetSkills();
            userProfile.trainingtype = workPermitBLL.GetTrainingType();
            userProfile.ContractorList = workPermitBLL.GetContractorCompany();
            userProfile.DepartmentList = adminBLL.GetDepartmentList();
            return View(userProfile);
        }

        public ActionResult ContractorEmployeePDF(int id)
        {
            try
            {
                using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[ContractorEmployeePDF]";
                    objCom.Parameters.AddWithValue("@ConUserID", id);
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    objCom.Connection = objCon;
                    SqlDataAdapter Adapter = new SqlDataAdapter();
                    Adapter.SelectCommand = objCom;
                    DataSet dataSet = new DataSet();
                    Adapter.Fill(dataSet);
                    objCon.Close();

                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition",
                        "filename=ContractorEmployee " + id + ".pdf");
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);

                    iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 20f, 20f, 30f, 30f);
                    var font8 = FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD);
                    var font9 = FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL);
                    var font7 = FontFactory.GetFont("Times New Roman", 11, iTextSharp.text.Font.BOLD);
                    PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                    pdfDoc.Open();
                    PdfPCell cell = null;

                    String form = String.Format("".PadRight(65) + "CONTRACTOR SAFETY PASS" + "\n\n");

                    pdfDoc.Add(new iTextSharp.text.Paragraph(form, FontFactory.GetFont("Times New Roman", 11, iTextSharp.text.Font.BOLD)));


                    PdfPTable TitleTable = new PdfPTable(2);
                    TitleTable.LockedWidth = true;
                    TitleTable.SetWidths(new float[] { 8f, 15.5f});
                    //TitleTable.SpacingBefore = 10f;
                    //TitleTable.SpacingAfter = 1f;
                    TitleTable.TotalWidth = 450f;

                    PdfPCell Wpcell = new PdfPCell();
                    string imageURL = Server.MapPath("~/Images/SASALogo.png");
                    iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(imageURL);
                    gif.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                    gif.SpacingBefore = 10f;
                                                                                                                                                                                                    
                    gif.ScaleAbsolute(120, 25f);
                    Wpcell = new PdfPCell(gif);

                    TitleTable.AddCell(Wpcell);


                    var phrase = new Phrase("\n SASA PTA PROJECT ".PadRight(1000), FontFactory.GetFont("Times New Roman", 11, iTextSharp.text.Font.BOLD));
                    phrase.Add(new Chunk("\n ADANA,TURKEY \n", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    phrase.Add(new Chunk("\n", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    Wpcell.HorizontalAlignment = Element.ALIGN_CENTER;
                    TitleTable.AddCell(PhraseCell(phrase, PdfPCell.ALIGN_CENTER));

                    

                    String FONT = "C:/Windows/Fonts/wingding.ttf";
                    String CheckedCheckboxText = "\u00fe";
                    String BlankCheckboxText = "o";
                    BaseFont bf = BaseFont.CreateFont(FONT, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    iTextSharp.text.Font f = new iTextSharp.text.Font(bf, 10);
                    Paragraph CheckedCheckbox = new Paragraph(CheckedCheckboxText, f);
                    Paragraph uncheckbox = new Paragraph(BlankCheckboxText, f);

                    pdfDoc.Add(TitleTable);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable Details = new PdfPTable(3);
                    Details.TotalWidth = 450f;
                    Details.LockedWidth = true;
                    Details.SetWidths(new float[] { 10f, 9f, 11f });

                    if (dataSet.Tables[0].Rows[0][11].ToString() != "")
                    {

                        string imagePath = Server.MapPath("~/ContractorUserImage/") + Path.GetFileName(dataSet.Tables[0].Rows[0][11].ToString());

                        iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imagePath);
                        //image.ScaleAbsolute(400f, 400f);
                        image.ScaleToFit(130f, 120f);

                        Wpcell = new PdfPCell(image);
                        Wpcell.Rowspan = 10;
                        Wpcell.PaddingTop = 30f;
                        Wpcell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;

                        Details.AddCell(Wpcell);
                    }
                    else
                    {

                        string imageURL2 = Server.MapPath("~/ContractorUserImage/notfound.jpg");
                        iTextSharp.text.Image gif2 = iTextSharp.text.Image.GetInstance(imageURL2);
                        gif2.ScaleToFit(130f, 120f);

                        Wpcell = new PdfPCell(gif2);
                        Wpcell.Rowspan = 10;
                        Wpcell.PaddingTop = 20f;
                        Wpcell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;

                        Details.AddCell(Wpcell);

                    }


                    Wpcell = new PdfPCell(new Phrase("Name ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)));
                    Wpcell.FixedHeight = 15f;
                    Details.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase("" + dataSet.Tables[0].Rows[0][1].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)));
                    Wpcell.FixedHeight = 15f;
                    Details.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase("Contractor Employee ID ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)));
                    Wpcell.FixedHeight = 15f;
                    Details.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase("" + dataSet.Tables[0].Rows[0][2].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)));
                    Wpcell.FixedHeight = 15f;
                    Details.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase("Age ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)));
                    Wpcell.FixedHeight = 15f;
                    Details.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase("" + dataSet.Tables[0].Rows[0][3].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)));
                    Wpcell.FixedHeight = 15f;
                    Details.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase("Contractor Name ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)));
                    Wpcell.FixedHeight = 15f;
                    Details.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase("" + dataSet.Tables[0].Rows[0][4].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)));
                    Wpcell.FixedHeight = 15f;
                    Details.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase("Department ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)));
                    Wpcell.FixedHeight = 15f;
                    Details.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase("" + dataSet.Tables[0].Rows[0][5].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)));
                    Wpcell.FixedHeight = 15f;
                    Details.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase("Work Type", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)));
                    Wpcell.FixedHeight = 15f;
                    Details.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase("" + dataSet.Tables[0].Rows[0][6].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)));
                    Wpcell.FixedHeight = 15f;
                    Details.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase("Date of Induction ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)));
                    Wpcell.FixedHeight = 15f;
                    Details.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase("" + dataSet.Tables[0].Rows[0][7].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)));
                    Wpcell.FixedHeight = 15f;
                    Details.AddCell(Wpcell);


                    Wpcell = new PdfPCell(new Phrase("Contact Number", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)));
                    Wpcell.FixedHeight = 15f;
                    Details.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase("" + dataSet.Tables[0].Rows[0][8].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)));
                    Wpcell.FixedHeight = 15f;
                    Details.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase("Emergency Contact number", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)));
                    Wpcell.FixedHeight = 15f;
                    Details.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase("" + dataSet.Tables[0].Rows[0][9].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)));
                    Wpcell.FixedHeight = 15f;
                    Details.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase("Valid up to ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)));
                    Wpcell.FixedHeight = 15f;
                    Details.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase("" + dataSet.Tables[0].Rows[0][10].ToString(), FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)));
                    Wpcell.FixedHeight = 15f;
                    Details.AddCell(Wpcell);

                    pdfDoc.Add(Details);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));
                    String RiskAssessment1 = String.Format("".PadRight(20) + "HR/Admin Signature ".PadRight(97) + "Safety In-Charge Signature");
                    pdfDoc.Add(new iTextSharp.text.Paragraph(RiskAssessment1, font8));
                    pdfDoc.Close();
                    Response.Write(pdfDoc);
                    Response.End();

                }
            }



            catch (Exception objException)
            {
                // LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);
            }

            return View();
        }

        public ActionResult ExportContractorEmployeeList()
        {

            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "ExportContractorEmployeeList";
                objCom.CommandType = CommandType.StoredProcedure;

                objCon.Open();
                objCom.Connection = objCon;
                SqlDataAdapter Adapter = new SqlDataAdapter();
                Adapter.SelectCommand = objCom;
                DataSet dataSet = new DataSet();
                Adapter.Fill(dataSet);
                objCon.Close();
                var wb = new XLWorkbook(Server.MapPath("~/Template/ContractorEmployeeList.xlsx"));
                var worksheet = wb.Worksheet("ContractorEmployeeList");
                worksheet.Cell("C4").Value = "Report Generated by : " + CurrentUser.FullName;
                //worksheet.Cell("D4").Value = CurrentUser.FullName;

                //worksheet.Cell("D5").Value = currentFromDate + " to  " + currentEndDate;

                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    var rangeWithDataSecond = worksheet.Cell(7, 1).InsertTable(dataSet.Tables[0].AsEnumerable());
                }
                else
                {
                    worksheet.Cell("C8").Value = "No Data Found";
                }

                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename= ContractorEmployeeList.xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

            }


            return View();


        }
        public ActionResult ExportContractorTrainingDetails(int id)
        {

            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "ExportContractorTrainingDetails";
                objCom.Parameters.AddWithValue("@ConUserID", id);
                objCom.CommandType = CommandType.StoredProcedure;

                objCon.Open();
                objCom.Connection = objCon;
                SqlDataAdapter Adapter = new SqlDataAdapter();
                Adapter.SelectCommand = objCom;
                DataSet dataSet = new DataSet();
                Adapter.Fill(dataSet);
                objCon.Close();
                var wb = new XLWorkbook(Server.MapPath("~/Template/ContractorTrainingDetails.xlsx"));
                var worksheet = wb.Worksheet("ContractorTrainingDetails");
                worksheet.Cell("C4").Value = "Report Generated by : " + CurrentUser.FullName;
                //worksheet.Cell("D4").Value = CurrentUser.FullName;

                //worksheet.Cell("D5").Value = currentFromDate + " to  " + currentEndDate;

                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    var rangeWithDataSecond = worksheet.Cell(7, 1).InsertTable(dataSet.Tables[0].AsEnumerable());
                }
                else
                {
                    worksheet.Cell("C8").Value = "No Data Found";
                }

                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=TraningDetails.xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

            }


            return View();


        }
        public ActionResult WorkPermitBlankPDF()
        {
            try
            {
                using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[PDFBlankWorkPermit]";

                    objCom.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    objCom.Connection = objCon;
                    SqlDataAdapter Adapter = new SqlDataAdapter();
                    Adapter.SelectCommand = objCom;
                    DataSet dataSet = new DataSet();
                    Adapter.Fill(dataSet);
                    objCon.Close();

                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition",
                        "attachment;filename=WorkPermit.pdf");
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);

                    iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(iTextSharp.text.PageSize.LEGAL, 20f, 20f, 30f, 30f);
                    HTMLWorker htmlparsers = new HTMLWorker(pdfDoc);
                    PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                    pdfDoc.Open();
                    PdfPCell cell = null;


                    PdfPTable TitleTable = new PdfPTable(4);
                    TitleTable.LockedWidth = true;
                    TitleTable.SetWidths(new float[] { 10f, 18f, 6.8f, 5.2f });
                    TitleTable.SpacingBefore = 10f;
                    TitleTable.SpacingAfter = 1f;
                    TitleTable.TotalWidth = 555f;
                    PdfPCell Wpcell = new PdfPCell();
                    string imageURL = Server.MapPath("~/Images/SASALogo.png");
                    iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(imageURL);
                    gif.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                    gif.SpacingBefore = 10f;

                    gif.ScaleAbsolute(135f, 40f);
                    Wpcell = new PdfPCell(gif);

                    TitleTable.AddCell(Wpcell);

                    var phrase = new Phrase(new Chunk("\n SASA PTA PROJECT ", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    phrase.Add(new Chunk("\n ADANA,TURKEY \n", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    phrase.Add(new Chunk("\n\n", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    TitleTable.AddCell(PhraseCell(phrase, PdfPCell.ALIGN_CENTER));

                    Wpcell = new PdfPCell(new Phrase("\n WORK PERMIT NO:", FontFactory.GetFont("Times New Roman", 12, iTextSharp.text.Font.BOLD)));
                    Wpcell.HorizontalAlignment = Element.ALIGN_CENTER;

                    TitleTable.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase("\n ", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    Wpcell.HorizontalAlignment = Element.ALIGN_LEFT;

                    TitleTable.AddCell(Wpcell);

                    Font zapfdingbats = new Font(Font.FontFamily.ZAPFDINGBATS);
                    String Tickmark = "\u0033";
                    Paragraph Tick = new Paragraph(Tickmark, zapfdingbats);
                    String FONT = "C:/Windows/Fonts/wingding.ttf";
                    String CheckedCheckboxText = "\u00fe";
                    String BlankCheckboxText = "o";
                    BaseFont bf = BaseFont.CreateFont(FONT, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    iTextSharp.text.Font f = new iTextSharp.text.Font(bf, 10);
                    Paragraph CheckedCheckbox = new Paragraph(CheckedCheckboxText, f);
                    Paragraph uncheckbox = new Paragraph(BlankCheckboxText, f);

                    String Blank = "";
                    Paragraph uncheckblank = new Paragraph(Blank, f);
                    pdfDoc.Add(TitleTable);

                    PdfPTable Typework = new PdfPTable(1);

                    Typework.TotalWidth = 555f;
                    Typework.LockedWidth = true;
                    Typework.SetWidths(new float[] { 40f });

                    Typework.AddCell(PhraseCell(new Phrase("HOT WORK PERMIT ", FontFactory.GetFont("Times New Roman", 14, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_CENTER));

                    pdfDoc.Add(Typework);


                    PdfPTable permitdetails = new PdfPTable(5);
                    permitdetails.TotalWidth = 555f;
                    permitdetails.LockedWidth = true;
                    permitdetails.SetWidths(new float[] { 2f, 8f, 9f, 9f, 12f });
                    permitdetails.AddCell(PhraseCell(new Phrase("1", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetails.AddCell(PhraseCell(new Phrase("VALIDITY FROM ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetails.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    permitdetails.AddCell(PhraseCell(new Phrase("VALIDITY TO ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetails.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    permitdetails.AddCell(PhraseCell(new Phrase("2", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetails.AddCell(PhraseCell(new Phrase("PERMIT ISSUER", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetails.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    permitdetails.AddCell(PhraseCell(new Phrase("PERMIT RECEIVER", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetails.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(permitdetails);
                    PdfPTable plant = new PdfPTable(5);
                    plant.TotalWidth = 555f;
                    plant.LockedWidth = true;
                    plant.SetWidths(new float[] { 2f, 8f, 9f, 9f, 12f });
                    plant.AddCell(PhraseCell(new Phrase("3", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    plant.AddCell(PhraseCell(new Phrase("PLANT/AREA", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    plant.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    plant.AddCell(PhraseCell(new Phrase("JOB ID", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    plant.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(plant);

                    PdfPTable Equtable = new PdfPTable(5);
                    Equtable.TotalWidth = 555f;
                    Equtable.LockedWidth = true;
                    Equtable.SetWidths(new float[] { 2f, 8f, 9f, 9f, 12f });

                    Equtable.AddCell(PhraseCell(new Phrase("4", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Equtable.AddCell(PhraseCell(new Phrase("LOCATION OF THE WORK ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Equtable.AddCell(PhraseCell(new Phrase("" , FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    Equtable.AddCell(PhraseCell(new Phrase("EQUIPMENT NAME", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Equtable.AddCell(PhraseCell(new Phrase("" , FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(Equtable);
                    PdfPTable desc = new PdfPTable(3);
                    desc.TotalWidth = 555f;
                    desc.LockedWidth = true;
                    desc.SetWidths(new float[] { 2f, 8f, 30f });
                    desc.AddCell(PhraseCell(new Phrase("5", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    desc.AddCell(PhraseCell(new Phrase("DESCRIPTION OF WORK  ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    desc.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(desc);


                    PdfPTable loc = new PdfPTable(5);
                    loc.TotalWidth = 555f;
                    loc.LockedWidth = true;
                    loc.SetWidths(new float[] { 2f, 8f, 9f, 9f, 12f });

                   
                    loc.AddCell(PhraseCell(new Phrase("6", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    loc.AddCell(PhraseCell(new Phrase("WORK DONE BY ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    loc.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    loc.AddCell(PhraseCell(new Phrase("DEPARTMENT", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    loc.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));


                    loc.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    loc.AddCell(PhraseCell(new Phrase("NAME OF CONTRACTOR", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    loc.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    loc.AddCell(PhraseCell(new Phrase("RISK ASSESSMENT REQUIRED?", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    loc.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(loc);

                    PdfPTable area = new PdfPTable(2);
                    area.TotalWidth = 555f;
                    area.LockedWidth = true;
                    area.SetWidths(new float[] { 2f, 38f });
                    area.AddCell(PhraseCell(new Phrase("7", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    area.AddCell(PhraseCell(new Phrase(" Following points should be checked & verified in Field before issuing the Work Permit as per the nature of job.", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    pdfDoc.Add(area);

                    PdfPTable gen = new PdfPTable(8);
                    var font8 = FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD);
                    var font9 = FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL);
                    var font7 = FontFactory.GetFont("Times New Roman", 11, iTextSharp.text.Font.BOLD);
                    gen.TotalWidth = 555f;
                    gen.LockedWidth = true;
                    gen.SetWidths(new float[] { 2f, 2.5f, 3.5f, 12f, 2f, 2.5f, 3.5f, 12f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    gen.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Done", font8)));
                    gen.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Not Required", font8)));
                    gen.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Check List", font8)));
                    gen.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    gen.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Done", font8)));
                    gen.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Not Required", font8)));
                    gen.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Check List", font8)));
                    gen.AddCell(Wpcell);


                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[0].Rows.Count; rows++)
                        {

                            gen.AddCell(PhraseCell(new Phrase(dataSet.Tables[0].Rows[rows][0].ToString(), font9), PdfPCell.ALIGN_CENTER));
                            gen.AddCell("");
                            gen.AddCell("");
                            Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[0].Rows[rows][1].ToString(), font9)));
                            gen.AddCell(Wpcell);

                        }
                    }
                    gen.AddCell("");
                    gen.AddCell("");
                    gen.AddCell("");
                    gen.AddCell("");
                    pdfDoc.Add(gen);

                    PdfPTable actualchklist = new PdfPTable(8);

                    actualchklist.TotalWidth = 555f;
                    actualchklist.LockedWidth = true;
                    actualchklist.SetWidths(new float[] { 2f, 2.5f, 3.5f, 12f, 2f, 2.5f, 3.5f, 12f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));

                    actualchklist.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actualchklist.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actualchklist.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Work Specific Check List", font8)));
                    actualchklist.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actualchklist.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actualchklist.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actualchklist.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Work Specific Check List", font8)));
                    actualchklist.AddCell(Wpcell);


                    if (dataSet.Tables[2].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[2].Rows.Count; rows++)
                        {


                            actualchklist.AddCell(PhraseCell(new Phrase(dataSet.Tables[2].Rows[rows][0].ToString(), font9), PdfPCell.ALIGN_CENTER));
                            actualchklist.AddCell("");
                            actualchklist.AddCell("");
                            Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[2].Rows[rows][1].ToString(), font9)));
                            actualchklist.AddCell(Wpcell);

                        }
                    }

                    pdfDoc.Add(actualchklist);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));


                    PdfPTable ppe = new PdfPTable(2);
                    ppe.TotalWidth = 555f;
                    ppe.LockedWidth = true;
                    ppe.SetWidths(new float[] { 2f, 38f });
                    ppe.AddCell(PhraseCell(new Phrase("8", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    ppe.AddCell(PhraseCell(new Phrase("SPECIAL PROTECTION / PPE", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(ppe);


                    PdfPTable ppe1 = new PdfPTable(8);
                    ppe1.TotalWidth = 555f;
                    ppe1.LockedWidth = true;
                    ppe1.SetWidths(new float[] { 2f, 8f, 2f, 8f, 2f, 8f, 2f, 8f });

                    if (dataSet.Tables[1].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[1].Rows.Count; rows++)
                        {

                            ppe1.AddCell(uncheckbox);

                            Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[1].Rows[rows][1].ToString(), font9)));
                            ppe1.AddCell(Wpcell);

                        }
                    }

                    else
                    {
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Applicable", font9)));
                        Wpcell.Colspan = 8;
                        ppe1.AddCell(Wpcell);

                    }
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    ppe1.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));

                    ppe1.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));

                    ppe1.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));

                    ppe1.AddCell(Wpcell);


                    Wpcell = new PdfPCell(new Phrase(new Chunk("")));
                    Wpcell.Colspan = 8;
                    ppe1.AddCell(Wpcell);



                    Wpcell = new PdfPCell(new Phrase(new Chunk("8.1 Others / L.C. No :", font8)));
                    Wpcell.Colspan = 8;
                    ppe1.AddCell(Wpcell);
                    pdfDoc.Add(ppe1);


                    PdfPTable gas = new PdfPTable(2);
                    gas.TotalWidth = 555f;
                    gas.LockedWidth = true;
                    gas.SetWidths(new float[] { 2f, 38f });
                    gas.AddCell(PhraseCell(new Phrase("8.2", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    gas.AddCell(PhraseCell(new Phrase("GAS TEST RESULT: Suggested retesting frequency is every …………Hrs (use additional table, if required)", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(gas);


                    PdfPTable gasdetails = new PdfPTable(9);
                    gasdetails.TotalWidth = 555f;
                    gasdetails.LockedWidth = true;
                    gasdetails.SetWidths(new float[] { 2f, 6f, 4f, 3f, 5f, 5f, 5f, 5f, 5f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("No.", font8)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Values", font8)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("%O2", font8)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("%LEL", font8)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Toxic Gas (PPM)", font8)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Other Gases", font8)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name", font8)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Signature", font8)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Date Time", font8)));
                    gasdetails.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Acceptable Values", font9)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("19.5 -22.5%", font9)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Zero", font9)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Less than TLV", font9)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Less than TLV", font9)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    gasdetails.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1", font9)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    gasdetails.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2", font9)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    gasdetails.AddCell(Wpcell);

                    pdfDoc.Add(gasdetails);
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable permithand = new PdfPTable(6);

                    permithand.TotalWidth = 555f;
                    permithand.LockedWidth = true;
                    permithand.SetWidths(new float[] { 2f, 10f, 8f, 9.2f, 5f, 5f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk("9", font8)));

                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("PERMIT APPROVALS", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Remarks ", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Signature", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Date Time", font8)));
                    permithand.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("I have checked the conditions in the field and the status is found as mentioned above. The job can be permitted.", font9)));

                    Wpcell.Colspan = 5;
                    permithand.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Permit Issuer", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Permit Approver(Shift In Charge)", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Stand by Person", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Safety officer", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Process Manager", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Electrical In charge", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Mechanical In charge ", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Instrument In charge", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Operations Head ", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);


                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("I understood the above condition,arranged safety requirements, explained to the people concerned and received the permit for safe job execution.", font9)));
                    Wpcell.Colspan = 5;

                    permithand.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name / Number of person(s) working at site", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("")));
                    Wpcell.Colspan = 4;
                    permithand.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Permit Receiver(Engineer /\nTech.of recipient dept).", font8)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithand.AddCell(Wpcell);

                    pdfDoc.Add(permithand);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));



                    PdfPTable permitclosure = new PdfPTable(6);

                    permitclosure.TotalWidth = 555f;
                    permitclosure.LockedWidth = true;
                    permitclosure.KeepTogether = true;
                    permitclosure.SetWidths(new float[] { 2f, 10f, 8f, 9.2f, 5f, 5f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk("10", font8)));

                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("PERMIT CLOSURE", font8)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name", font8)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Remarks ", font8)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Signature", font8)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Date Time", font8)));
                    permitclosure.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Certified that the subject work has been completed/ stopped and area cleaned. ", font9)));
                    Wpcell.Colspan = 5;

                    permitclosure.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Job completed & Handed over by Receiver", font8)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosure.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));

                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Job taken over & Permit closed by Issuer", font8)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosure.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Approver/Shift InCharge", font8)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosure.AddCell(Wpcell);


                    pdfDoc.Add(permitclosure);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));



                    PdfPTable conr = new PdfPTable(6);
                    PdfPCell p = new PdfPCell();
                    conr.TotalWidth = 555f;
                    conr.LockedWidth = true;
                    conr.SetWidths(new float[] { 1f, 3f, 1f, 3f, 1f, 22f });
                    cell = PhraseCell(new Phrase("11".PadRight(6) + "CONTRACTOR RATING  ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT);
                    cell.Colspan = 6;
                    conr.AddCell(cell);
                    conr.AddCell(uncheckbox);

                    p = new PdfPCell(new Phrase(new Chunk("Red", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD))));
                    conr.AddCell(p);

                    conr.AddCell(uncheckbox);

                    p = new PdfPCell(new Phrase(new Chunk("Yellow", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD))));
                    conr.AddCell(p);

                    conr.AddCell(uncheckbox);

                    p = new PdfPCell(new Phrase(new Chunk("Green", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD))));
                    conr.AddCell(p);

                    p = new PdfPCell(new Phrase(new Chunk(" Remarks (if Red and Yellow rating) :                                                                  \n\n", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL))));
                    p.Colspan = 8;
                    conr.AddCell(p);

                    pdfDoc.Add(conr);
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable ren = new PdfPTable(2);
                    ren.TotalWidth = 555f;
                    ren.LockedWidth = true;
                    ren.SetWidths(new float[] { 2f, 38f });
                    ren.AddCell(PhraseCell(new Phrase("12", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    ren.AddCell(PhraseCell(new Phrase("RENEWAL OF WORK PERMIT", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(ren);


                    PdfPTable renewal = new PdfPTable(9);

                    renewal.TotalWidth = 555f;
                    renewal.LockedWidth = true;
                    renewal.SetWidths(new float[] { 7f, 5f, 4f, 4f, 4f, 4f, 4f, 4f, 4f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Day/ Date/Time", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. LEL Level (ZERO)", font8)));

                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of Permit issuer ", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of shift In charge ", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of Stand by", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of safety In charge", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of Manager Process", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of Operations Head", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of  Receiver", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. % O2(19.5 - 22.5 %)", font8)));

                    renewal.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("First Extension Date:................................", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell.Rowspan = 2;
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                    renewal.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Second Extension Date:................................", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell.Rowspan = 2;
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                    renewal.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Third Extension Date:................................", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell.Rowspan = 2;
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                    renewal.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Fourth Extension Date:................................", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell.Rowspan = 2;
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                    renewal.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Fifth Extension Date:................................", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell.Rowspan = 2;
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                    renewal.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Sixth Extension Date:................................", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);
                    Wpcell.Rowspan = 2;
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewal.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                    renewal.AddCell(Wpcell);


                    pdfDoc.Add(renewal);

                    String form1 = String.Format("** Renewal of Hot work permit is allowed only if the work is carried out in Workshop ");

                    pdfDoc.Add(new iTextSharp.text.Paragraph(form1, font8));

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable genins = new PdfPTable(2);
                    genins.TotalWidth = 555f;
                    genins.LockedWidth = true;
                    genins.SetWidths(new float[] { 2f, 38f });
                    genins.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    genins.AddCell(PhraseCell(new Phrase("GENERAL INSTRUCTIONS : ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1.", font9)));
                    genins.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("In case of an Emergency or FIRE ALARM, all works must be stopped and permit stands cancelled. All personnel must leave work site and proceed to emergency Assembly points soon.", font9)));
                    genins.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2.", font9)));
                    genins.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("In case of any  LPG/ Gas /Liquid release (or) abnormality noticed, stop work immediately and move to safe location till further instruction,", font9)));
                    genins.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("3.", font9)));
                    genins.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("This permit must be available at work site all the time. On job closure (or) expiry, this permit must be returned to the permit issuer/Shift Incharge. ", font9)));
                    genins.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("4.", font9)));
                    genins.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Do not commence the work without a valid work permit.", font9)));
                    genins.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("5.", font9)));
                    genins.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Acquaint yourself with job and its hazards and ensure the tools used are safe.", font9)));
                    genins.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("6.", font9)));
                    genins.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Inform regarding unsafe condition/unsafe act to operator/Safety incharge/Shift incharge", font9)));
                    genins.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("7.", font9)));
                    genins.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("When in doubt, consult SASA employee of concerned department (or) SASA Operator ", font9)));
                    genins.AddCell(Wpcell);


                    Wpcell = new PdfPCell(new Phrase(new Chunk("8.", font9)));
                    genins.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Work should be started within one hour after issue of this permit", font9)));
                    genins.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("9.", font9)));
                    genins.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("All safety precautions should be meticulously followed. Keep access clear for emergency.", font9)));
                    genins.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("10.", font9)));
                    genins.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Incase of any emergency, permit should be surrendered to the issuing authority", font9)));
                    genins.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("11.", font9)));
                    genins.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Permit is valid only for the mentioned time. After that, further extension is needed before proceeding the job", font9)));
                    genins.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("12.", font9)));
                    genins.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Extention: To be extended only, if area and nature of work is same", font9)));
                    genins.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("13.", font9)));
                    genins.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Extention: Daily extension should be authorized per the concerned persons, after reviewing the site again.", font9)));
                    genins.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("14.", font9)));
                    genins.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Extention: If the job is discontinued, make a fresh permit", font9)));
                    genins.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("15.", font9)));
                    genins.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Extension: Extention can be done maximum one week duration. Make new permit every Monday.", font9)));
                    genins.AddCell(Wpcell);

                    pdfDoc.Add(genins);


                    PdfPTable spec = new PdfPTable(2);
                    spec.TotalWidth = 555f;
                    spec.LockedWidth = true;
                    spec.SetWidths(new float[] { 2f, 38f });

                    spec.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    spec.AddCell(PhraseCell(new Phrase("PERMIT SPECIFIC INSTRUCTIONS - Hot Work ", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    spec.AddCell(PhraseCell(new Phrase("1.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    spec.AddCell(PhraseCell(new Phrase("Hot work includes spark, welding, gas cutting, hacksaw cutting, Hammering, burning, grinding, soldering, chipping, rivetting, shot blasting, drilling, camera flashing, power tools, IC Engine operations. ", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    spec.AddCell(PhraseCell(new Phrase("2.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    spec.AddCell(PhraseCell(new Phrase("Incase of gas/liquid leak, all HOT work must be stopped immediately ", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    spec.AddCell(PhraseCell(new Phrase("3.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    spec.AddCell(PhraseCell(new Phrase("For any hotwork inside the vessel, Confined Space permit should also be ensured", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(spec);
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(700)));

                    PdfPTable TitleTablecold = new PdfPTable(4);
                    TitleTablecold.LockedWidth = true;
                    TitleTablecold.SetWidths(new float[] { 10f, 18f, 6.8f, 5.2f });
                    TitleTablecold.SpacingBefore = 10f;
                    TitleTablecold.SpacingAfter = 1f;
                    TitleTablecold.TotalWidth = 555f;


                    Wpcell = new PdfPCell(gif);

                    TitleTablecold.AddCell(Wpcell);

                    var phrasecold = new Phrase(new Chunk("\n SASA PTA PROJECT ", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    phrasecold.Add(new Chunk("ADANA,TURKEY", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    phrasecold.Add(new Chunk("\n\n", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    TitleTablecold.AddCell(PhraseCell(phrasecold, PdfPCell.ALIGN_CENTER));

                    Wpcell = new PdfPCell(new Phrase("\n WORK PERMIT NO:", FontFactory.GetFont("Times New Roman", 12, iTextSharp.text.Font.BOLD)));
                    Wpcell.HorizontalAlignment = Element.ALIGN_CENTER;

                    TitleTablecold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase("\n ", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    Wpcell.HorizontalAlignment = Element.ALIGN_LEFT;

                    TitleTablecold.AddCell(Wpcell);
                    pdfDoc.Add(TitleTablecold);
                    PdfPTable Typeworkcold = new PdfPTable(1);

                    Typeworkcold.TotalWidth = 555f;
                    Typeworkcold.LockedWidth = true;
                    Typeworkcold.SetWidths(new float[] { 40f });

                    Typeworkcold.AddCell(PhraseCell(new Phrase("COLD WORK PERMIT ", FontFactory.GetFont("Times New Roman", 14, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_CENTER));

                    pdfDoc.Add(Typeworkcold);


                    PdfPTable permitdetailscold = new PdfPTable(5);
                    permitdetailscold.TotalWidth = 555f;
                    permitdetailscold.LockedWidth = true;
                    permitdetailscold.SetWidths(new float[] { 2f, 8f, 9f, 9f, 12f });
                    permitdetailscold.AddCell(PhraseCell(new Phrase("1", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetailscold.AddCell(PhraseCell(new Phrase("VALIDITY FROM ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetailscold.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    permitdetailscold.AddCell(PhraseCell(new Phrase("VALIDITY TO ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetailscold.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    permitdetailscold.AddCell(PhraseCell(new Phrase("2", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetailscold.AddCell(PhraseCell(new Phrase("PERMIT ISSUER: ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetailscold.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    permitdetailscold.AddCell(PhraseCell(new Phrase("PERMIT RECEIVER: ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetailscold.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(permitdetailscold);
                    PdfPTable plant1 = new PdfPTable(5);
                    plant1.TotalWidth = 555f;
                    plant1.LockedWidth = true;
                    plant1.SetWidths(new float[] { 2f, 8f, 9f, 9f, 12f });
                    plant1.AddCell(PhraseCell(new Phrase("3", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    plant1.AddCell(PhraseCell(new Phrase("PLANT/AREA", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    plant1.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    plant1.AddCell(PhraseCell(new Phrase("JOB ID", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    plant1.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(plant1);

                    PdfPTable Equtable1 = new PdfPTable(5);
                    Equtable1.TotalWidth = 555f;
                    Equtable1.LockedWidth = true;
                    Equtable1.SetWidths(new float[] { 2f, 8f, 9f, 9f, 12f });

                    Equtable1.AddCell(PhraseCell(new Phrase("4", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Equtable1.AddCell(PhraseCell(new Phrase("LOCATION OF THE WORK ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Equtable1.AddCell(PhraseCell(new Phrase("" , FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    Equtable1.AddCell(PhraseCell(new Phrase("EQUIPMENT NAME", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Equtable1.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(Equtable1);
                    PdfPTable desccold = new PdfPTable(3);
                    desccold.TotalWidth = 555f;
                    desccold.LockedWidth = true;
                    desccold.SetWidths(new float[] { 2f, 8f, 30f });
                    desccold.AddCell(PhraseCell(new Phrase("5", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    desccold.AddCell(PhraseCell(new Phrase("DESCRIPTION OF WORK  ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    desccold.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(desccold);


                    PdfPTable loccold = new PdfPTable(5);
                    loccold.TotalWidth = 555f;
                    loccold.LockedWidth = true;
                    loccold.SetWidths(new float[] { 2f, 8f, 9f, 9f, 12f });

                    loccold.AddCell(PhraseCell(new Phrase("6", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    loccold.AddCell(PhraseCell(new Phrase("WORK DONE BY ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    loccold.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    loccold.AddCell(PhraseCell(new Phrase("DEPARTMENT", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    loccold.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));


                    loccold.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    loccold.AddCell(PhraseCell(new Phrase("NAME OF CONTRACTOR", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    loccold.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    loccold.AddCell(PhraseCell(new Phrase("RISK ASSESSMENT REQUIRED?", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    loccold.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(loccold);

                    PdfPTable areacold = new PdfPTable(2);
                    areacold.TotalWidth = 555f;
                    areacold.LockedWidth = true;
                    areacold.SetWidths(new float[] { 2f, 38f });
                    areacold.AddCell(PhraseCell(new Phrase("7", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    areacold.AddCell(PhraseCell(new Phrase("Following points should be checked & verified in Field before issuing the Work Permit as per the nature of job.", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    pdfDoc.Add(areacold);

                    PdfPTable gencold = new PdfPTable(8);

                    gencold.TotalWidth = 555f;
                    gencold.LockedWidth = true;
                    gencold.SetWidths(new float[] { 2f, 2.5f, 3.5f, 12f, 2f, 2.5f, 3.5f, 12f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    gencold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Done", font8)));
                    gencold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Not Required", font8)));
                    gencold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Check List", font8)));
                    gencold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    gencold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Done", font8)));
                    gencold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Not Required", font8)));
                    gencold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Check List", font8)));
                    gencold.AddCell(Wpcell);


                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[0].Rows.Count; rows++)
                        {

                            gencold.AddCell(PhraseCell(new Phrase(dataSet.Tables[0].Rows[rows][0].ToString(), font9), PdfPCell.ALIGN_CENTER));
                            gencold.AddCell("");
                            gencold.AddCell("");
                            Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[0].Rows[rows][1].ToString(), font9)));
                            gencold.AddCell(Wpcell);

                        }
                    }
                    gencold.AddCell("");
                    gencold.AddCell("");
                    gencold.AddCell("");
                    gencold.AddCell("");
                    pdfDoc.Add(gencold);
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable ppecold = new PdfPTable(2);
                    ppecold.TotalWidth = 555f;
                    ppecold.LockedWidth = true;
                    ppecold.SetWidths(new float[] { 2f, 38f });
                    ppecold.AddCell(PhraseCell(new Phrase("8", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    ppecold.AddCell(PhraseCell(new Phrase("SPECIAL PROTECTION / PPE", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(ppecold);


                    PdfPTable ppe1cold = new PdfPTable(8);
                    ppe1cold.TotalWidth = 555f;
                    ppe1cold.LockedWidth = true;
                    ppe1cold.SetWidths(new float[] { 2f, 8f, 2f, 8f, 2f, 8f, 2f, 8f });

                    if (dataSet.Tables[1].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[1].Rows.Count; rows++)
                        {

                            ppe1cold.AddCell(uncheckbox);

                            Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[1].Rows[rows][1].ToString(), font9)));
                            ppe1cold.AddCell(Wpcell);

                        }
                    }

                    else
                    {
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Applicable", font9)));
                        Wpcell.Colspan = 8;
                        ppe1cold.AddCell(Wpcell);

                    }
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    ppe1cold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));

                    ppe1cold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));

                    ppe1cold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));

                    ppe1cold.AddCell(Wpcell);


                    Wpcell = new PdfPCell(new Phrase(new Chunk("")));
                    Wpcell.Colspan = 8;
                    ppe1cold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("8.1 Others / L.C. No :", font8)));
                    Wpcell.Colspan = 8;
                    ppe1cold.AddCell(Wpcell);

                    pdfDoc.Add(ppe1cold);
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable permithandcold = new PdfPTable(6);

                    permithandcold.TotalWidth = 555f;
                    permithandcold.LockedWidth = true;
                    permithandcold.SetWidths(new float[] { 2f, 10f, 8f, 9.2f, 5f, 5f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk("9", font8)));

                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("PERMIT APPROVALS", font8)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name", font8)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Remarks ", font8)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Signature", font8)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Date Time", font8)));
                    permithandcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("I have checked the conditions in the field and the status is found as mentioned above. The job can be permitted.", font9)));

                    Wpcell.Colspan = 5;
                    permithandcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Permit Issuer", font8)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Permit Approver(Shift In Charge)", font8)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Stand by Person", font8)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Safety officer", font8)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Process Manager", font8)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Electrical In charge", font8)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Mechanical In charge ", font8)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Instrumental In charge", font8)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Operations Head ", font8)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandcold.AddCell(Wpcell);


                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("I understood the above condition, arranged safety requirements, explained to the people concerned and received the permit for safe job execution.", font9)));
                    Wpcell.Colspan = 5;

                    permithandcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("")));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name / Number of person(s) working at site", font8)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("")));
                    Wpcell.Colspan = 4;
                    permithandcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Permit Receiver(Engineer /\nTech.of recipient dept).", font8)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandcold.AddCell(Wpcell);

                    pdfDoc.Add(permithandcold);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));


                    PdfPTable permitclosurecold = new PdfPTable(6);

                    permitclosurecold.TotalWidth = 555f;
                    permitclosurecold.LockedWidth = true;
                    permitclosurecold.SetWidths(new float[] { 2f, 10f, 8f, 9.2f, 5f, 5f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk("10", font8)));

                    permitclosurecold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("PERMIT CLOSURE", font8)));
                    permitclosurecold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name", font8)));
                    permitclosurecold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Remarks ", font8)));
                    permitclosurecold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Signature", font8)));
                    permitclosurecold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Date Time", font8)));
                    permitclosurecold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permitclosurecold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Certified that the subject work has been completed/ stopped and area cleaned. ", font9)));
                    Wpcell.Colspan = 5;

                    permitclosurecold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));

                    permitclosurecold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Job completed & Hand over by Receiver", font8)));
                    permitclosurecold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurecold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permitclosurecold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurecold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurecold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));

                    permitclosurecold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Job taken over & Permit closed by issuer", font8)));
                    permitclosurecold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurecold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permitclosurecold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurecold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurecold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permitclosurecold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Shift InCharge", font8)));
                    permitclosurecold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurecold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permitclosurecold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurecold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurecold.AddCell(Wpcell);


                    pdfDoc.Add(permitclosurecold);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable conrcold = new PdfPTable(6);
                    PdfPCell pcold = new PdfPCell();
                    conrcold.TotalWidth = 555f;
                    conrcold.LockedWidth = true;
                    conrcold.SetWidths(new float[] { 1f, 3f, 1f, 3f, 1f, 22f });
                    cell = PhraseCell(new Phrase("11".PadRight(6) + "CONTRACTOR RATING  ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT);
                    cell.Colspan = 6;
                    conrcold.AddCell(cell);
                    conrcold.AddCell(uncheckbox);

                    pcold = new PdfPCell(new Phrase(new Chunk("Red", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD))));
                    conrcold.AddCell(pcold);

                    conrcold.AddCell(uncheckbox);

                    pcold = new PdfPCell(new Phrase(new Chunk("Yellow", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD))));
                    conrcold.AddCell(pcold);

                    conrcold.AddCell(uncheckbox);

                    pcold = new PdfPCell(new Phrase(new Chunk("Green", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD))));
                    conrcold.AddCell(pcold);

                    pcold = new PdfPCell(new Phrase(new Chunk(" Remarks (if Red and Yellow rating) :                                                                  \n\n", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL))));
                    pcold.Colspan = 8;
                    conrcold.AddCell(pcold);

                    pdfDoc.Add(conrcold);
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable rencold = new PdfPTable(2);
                    rencold.TotalWidth = 555f;
                    rencold.LockedWidth = true;
                    rencold.SetWidths(new float[] { 2f, 38f });
                    rencold.AddCell(PhraseCell(new Phrase("12", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    rencold.AddCell(PhraseCell(new Phrase("RENEWAL OF WORK PERMIT", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(rencold);


                    PdfPTable renewalcold = new PdfPTable(9);

                    renewalcold.TotalWidth = 555f;
                    renewalcold.LockedWidth = true;
                    renewalcold.SetWidths(new float[] { 7f, 5f, 4f, 4f, 4f, 4f, 4f, 4f, 4f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Day/ Date/Time", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. LEL Level (ZERO)", font8)));

                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of Permit issuer ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of shift In charge ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of Stand by", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of safety In charge", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of Manager Process", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of Operations Head", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of  Receiver", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. % O2(19.5 - 22.5 %)", font8)));

                    renewalcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("First Extension Date:................................", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell.Rowspan = 2;
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                    renewalcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Second Extension Date:................................", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell.Rowspan = 2;
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                    renewalcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Third Extension Date:................................", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell.Rowspan = 2;
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                    renewalcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Fourth Extension Date:................................", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell.Rowspan = 2;
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                    renewalcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Fifth Extension Date:................................", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell.Rowspan = 2;
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                    renewalcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Sixth Extension Date:................................", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                    renewal.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);
                    Wpcell.Rowspan = 2;
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalcold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                    renewalcold.AddCell(Wpcell);

                    pdfDoc.Add(renewal);


                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable geninscold = new PdfPTable(2);
                    geninscold.TotalWidth = 555f;
                    geninscold.LockedWidth = true;
                    geninscold.SetWidths(new float[] { 2f, 38f });
                    geninscold.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    geninscold.AddCell(PhraseCell(new Phrase("GENERAL INSTRUCTIONS : ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1.", font9)));
                    geninscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("In case of an Emergency or FIRE ALARM, all works must be stopped and permit stands cancelled. All personnel must leave work site and proceed to emergency Assembly points soon.", font9)));
                    geninscold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2.", font9)));
                    geninscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("In case of any  LPG/ Gas /Liquid release (or) abnormality noticed, stop work immediately and move to safe location till further instruction,", font9)));
                    geninscold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("3.", font9)));
                    geninscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("This permit must be available at work site all the time. On job closure (or) expiry, this permit must be returned to the permit issuer/Shift Incharge. ", font9)));
                    geninscold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("4.", font9)));
                    geninscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Do not commence the work without a valid work permit.", font9)));
                    geninscold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("5.", font9)));
                    geninscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Acquaint yourself with job and its hazards and ensure the tools used are safe.", font9)));
                    geninscold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("6.", font9)));
                    geninscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Inform regarding unsafe condition/unsafe act to operator/Safety incharge/Shift incharge", font9)));
                    geninscold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("7.", font9)));
                    geninscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("When in doubt, consult SASA employee of concerned department (or) SASA Operator ", font9)));
                    geninscold.AddCell(Wpcell);


                    Wpcell = new PdfPCell(new Phrase(new Chunk("8.", font9)));
                    geninscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Work should be started within one hour after issue of this permit", font9)));
                    geninscold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("9.", font9)));
                    geninscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("All safety precautions should be meticulously followed. Keep access clear for emergency.", font9)));
                    geninscold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("10.", font9)));
                    geninscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Incase of any emergency, permit should be surrendered to the issuing authority", font9)));
                    geninscold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("11.", font9)));
                    geninscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Permit is valid only for the mentioned time. After that, further extension is needed before proceeding the job", font9)));
                    geninscold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("12.", font9)));
                    geninscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Extention: To be extended only, if area and nature of work is same", font9)));
                    geninscold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("13.", font9)));
                    geninscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Extention: Daily extension should be authorized per the concerned persons, after reviewing the site again.", font9)));
                    geninscold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("14.", font9)));
                    geninscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Extention: If the job is discontinued, make a fresh permit", font9)));
                    geninscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("15.", font9)));
                    geninscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Extension: Extention can be done maximum one week duration. Make new permit every Monday.", font9)));
                    geninscold.AddCell(Wpcell);

                    pdfDoc.Add(genins);
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(6000)));

                    PdfPTable TitleTableconfined = new PdfPTable(4);
                    TitleTableconfined.LockedWidth = true;
                    TitleTableconfined.SetWidths(new float[] { 10f, 18f, 6.8f, 5.2f });
                    TitleTableconfined.SpacingBefore = 10f;
                    TitleTableconfined.SpacingAfter = 1f;
                    TitleTableconfined.TotalWidth = 555f;


                    Wpcell = new PdfPCell(gif);

                    TitleTableconfined.AddCell(Wpcell);

                    var phraseconfined = new Phrase(new Chunk("\n SASA PTA PROJECT ", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    phraseconfined.Add(new Chunk("ADANA,TURKEY", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    phraseconfined.Add(new Chunk("\n\n", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    TitleTableconfined.AddCell(PhraseCell(phraseconfined, PdfPCell.ALIGN_CENTER));

                    Wpcell = new PdfPCell(new Phrase("\n WORK PERMIT NO:", FontFactory.GetFont("Times New Roman", 12, iTextSharp.text.Font.BOLD)));
                    Wpcell.HorizontalAlignment = Element.ALIGN_CENTER;

                    TitleTableconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase("\n ", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    Wpcell.HorizontalAlignment = Element.ALIGN_LEFT;

                    TitleTableconfined.AddCell(Wpcell);
                    pdfDoc.Add(TitleTableconfined);
                    PdfPTable Typeworkconfined = new PdfPTable(1);

                    Typeworkconfined.TotalWidth = 555f;
                    Typeworkconfined.LockedWidth = true;
                    Typeworkconfined.SetWidths(new float[] { 40f });

                    Typeworkconfined.AddCell(PhraseCell(new Phrase("CONFINED SPACE PERMIT ", FontFactory.GetFont("Times New Roman", 14, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_CENTER));

                    pdfDoc.Add(Typeworkconfined);


                    PdfPTable permitdetailsconfined = new PdfPTable(5);
                    permitdetailsconfined.TotalWidth = 555f;
                    permitdetailsconfined.LockedWidth = true;
                    permitdetailsconfined.SetWidths(new float[] { 2f, 8f, 9f, 9f, 12f });
                    permitdetailsconfined.AddCell(PhraseCell(new Phrase("1", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetailsconfined.AddCell(PhraseCell(new Phrase("VALIDITY FROM ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetailsconfined.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    permitdetailsconfined.AddCell(PhraseCell(new Phrase("VALIDITY TO ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetailsconfined.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    permitdetailsconfined.AddCell(PhraseCell(new Phrase("2", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetailsconfined.AddCell(PhraseCell(new Phrase("PERMIT ISSUER: ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetailsconfined.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    permitdetailsconfined.AddCell(PhraseCell(new Phrase("PERMIT RECEIVER: ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetailsconfined.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(permitdetailscold);
                    PdfPTable plant2 = new PdfPTable(5);
                    plant2.TotalWidth = 555f;
                    plant2.LockedWidth = true;
                    plant2.SetWidths(new float[] { 2f, 8f, 9f, 9f, 12f });
                    plant2.AddCell(PhraseCell(new Phrase("3", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    plant2.AddCell(PhraseCell(new Phrase("PLANT/AREA", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    plant2.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    plant2.AddCell(PhraseCell(new Phrase("JOB ID", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    plant2.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(plant2);

                    PdfPTable Equtable2 = new PdfPTable(5);
                    Equtable2.TotalWidth = 555f;
                    Equtable2.LockedWidth = true;
                    Equtable2.SetWidths(new float[] { 2f, 8f, 9f, 9f, 12f });

                    Equtable2.AddCell(PhraseCell(new Phrase("4", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Equtable2.AddCell(PhraseCell(new Phrase("LOCATION OF THE WORK ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Equtable2.AddCell(PhraseCell(new Phrase("" , FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    Equtable2.AddCell(PhraseCell(new Phrase("EQUIPMENT NAME", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Equtable2.AddCell(PhraseCell(new Phrase("" , FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(Equtable2);
                    PdfPTable descconfined = new PdfPTable(3);
                    descconfined.TotalWidth = 555f;
                    descconfined.LockedWidth = true;
                    descconfined.SetWidths(new float[] { 2f, 8f, 30f });
                    descconfined.AddCell(PhraseCell(new Phrase("5", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    descconfined.AddCell(PhraseCell(new Phrase("DESCRIPTION OF WORK  ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    descconfined.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(descconfined);


                    PdfPTable locconfined = new PdfPTable(5);
                    locconfined.TotalWidth = 555f;
                    locconfined.LockedWidth = true;
                    locconfined.SetWidths(new float[] { 2f, 8f, 9f, 9f, 12f });
                    locconfined.AddCell(PhraseCell(new Phrase("6", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    locconfined.AddCell(PhraseCell(new Phrase("WORK DONE BY ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    locconfined.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    locconfined.AddCell(PhraseCell(new Phrase("DEPARTMENT", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    locconfined.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));


                    locconfined.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    locconfined.AddCell(PhraseCell(new Phrase("NAME OF CONTRACTOR", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    locconfined.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    locconfined.AddCell(PhraseCell(new Phrase("RISK ASSESSMENT REQUIRED?", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    locconfined.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(locconfined);

                    PdfPTable areaconfined = new PdfPTable(2);
                    areaconfined.TotalWidth = 555f;
                    areaconfined.LockedWidth = true;
                    areaconfined.SetWidths(new float[] { 2f, 38f });
                    areaconfined.AddCell(PhraseCell(new Phrase("7", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    areaconfined.AddCell(PhraseCell(new Phrase(" Following points should be checked & verified in Field before issuing the Work Permit as per the nature of job.", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    pdfDoc.Add(areaconfined);

                    PdfPTable genconfined = new PdfPTable(8);

                    genconfined.TotalWidth = 555f;
                    genconfined.LockedWidth = true;
                    genconfined.SetWidths(new float[] { 2f, 2.5f, 3.5f, 12f, 2f, 2.5f, 3.5f, 12f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    genconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Done", font8)));
                    genconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Not Required", font8)));
                    genconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Check List", font8)));
                    genconfined.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    genconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Done", font8)));
                    genconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Not Required", font8)));
                    genconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Check List", font8)));
                    genconfined.AddCell(Wpcell);


                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[0].Rows.Count; rows++)
                        {

                            genconfined.AddCell(PhraseCell(new Phrase(dataSet.Tables[0].Rows[rows][0].ToString(), font9), PdfPCell.ALIGN_CENTER));
                            genconfined.AddCell("");
                            genconfined.AddCell("");
                            Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[0].Rows[rows][1].ToString(), font9)));
                            genconfined.AddCell(Wpcell);

                        }
                    }
                    genconfined.AddCell("");
                    genconfined.AddCell("");
                    genconfined.AddCell("");
                    genconfined.AddCell("");
                    pdfDoc.Add(genconfined);

                    PdfPTable actualchklistconfined = new PdfPTable(8);

                    actualchklistconfined.TotalWidth = 555f;
                    actualchklistconfined.LockedWidth = true;
                    actualchklistconfined.SetWidths(new float[] { 2f, 2.5f, 3.5f, 12f, 2f, 2.5f, 3.5f, 12f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));

                    actualchklistconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actualchklistconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actualchklistconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Work Specific Check List", font8)));
                    actualchklistconfined.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actualchklistconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actualchklistconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actualchklistconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Work Specific Check List", font8)));
                    actualchklistconfined.AddCell(Wpcell);


                    if (dataSet.Tables[3].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[3].Rows.Count; rows++)
                        {

                            actualchklistconfined.AddCell(PhraseCell(new Phrase(dataSet.Tables[3].Rows[rows][0].ToString(), font9), PdfPCell.ALIGN_CENTER));
                            actualchklistconfined.AddCell("");
                            actualchklistconfined.AddCell("");
                            Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[3].Rows[rows][1].ToString(), font9)));
                            actualchklistconfined.AddCell(Wpcell);

                        }
                    }

                    pdfDoc.Add(actualchklistconfined);


                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable ppeconfined = new PdfPTable(2);
                    ppeconfined.TotalWidth = 555f;
                    ppeconfined.LockedWidth = true;
                    ppeconfined.SetWidths(new float[] { 2f, 38f });
                    ppeconfined.AddCell(PhraseCell(new Phrase("8", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    ppeconfined.AddCell(PhraseCell(new Phrase("SPECIAL PROTECTION / PPE", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(ppeconfined);


                    PdfPTable ppe1confined1 = new PdfPTable(8);
                    ppe1confined1.TotalWidth = 555f;
                    ppe1confined1.LockedWidth = true;
                    ppe1confined1.SetWidths(new float[] { 2f, 8f, 2f, 8f, 2f, 8f, 2f, 8f });

                    if (dataSet.Tables[1].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[1].Rows.Count; rows++)
                        {

                            ppe1confined1.AddCell(uncheckbox);

                            Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[1].Rows[rows][1].ToString(), font9)));
                            ppe1confined1.AddCell(Wpcell);

                        }
                    }

                    else
                    {
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Applicable", font9)));
                        Wpcell.Colspan = 8;
                        ppe1confined1.AddCell(Wpcell);

                    }
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    ppe1confined1.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));

                    ppe1confined1.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));

                    ppe1confined1.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));

                    ppe1confined1.AddCell(Wpcell);


                    Wpcell = new PdfPCell(new Phrase(new Chunk("")));
                    Wpcell.Colspan = 8;
                    ppe1confined1.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("8.1 Others / L.C. No :", font8)));
                    Wpcell.Colspan = 8;
                    ppe1confined1.AddCell(Wpcell);

                    pdfDoc.Add(ppe1confined1);


                    PdfPTable gascold = new PdfPTable(2);
                    gascold.TotalWidth = 555f;
                    gascold.LockedWidth = true;
                    gascold.SetWidths(new float[] { 2f, 38f });
                    gascold.AddCell(PhraseCell(new Phrase("8.2", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    gascold.AddCell(PhraseCell(new Phrase("GAS TEST RESULT: Suggested retesting frequency is every …………Hrs (use additional table, if required)\n\n", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(gascold);


                    PdfPTable gasdetailscold = new PdfPTable(9);
                    gasdetailscold.TotalWidth = 555f;
                    gasdetailscold.LockedWidth = true;
                    gasdetailscold.SetWidths(new float[] { 2f, 6f, 4f, 3f, 5f, 5f, 5f, 5f, 5f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("No.", font8)));
                    gasdetailscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Values", font8)));
                    gasdetailscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("%O2", font8)));
                    gasdetailscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("%LEL", font8)));
                    gasdetailscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Toxic Gas (PPM)", font8)));
                    gasdetailscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Other Gases", font8)));
                    gasdetailscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name", font8)));
                    gasdetailscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Signature", font8)));
                    gasdetailscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Date Time", font8)));
                    gasdetailscold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    gasdetailscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Acceptable Values", font9)));
                    gasdetailscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("19.5 -22.5%", font9)));
                    gasdetailscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Zero", font9)));
                    gasdetailscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Less than TLV", font9)));
                    gasdetailscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Less than TLV", font9)));
                    gasdetailscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    gasdetails.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    gasdetailscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    gasdetailscold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1", font9)));
                    gasdetailscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    gasdetailscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    gasdetailscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    gasdetailscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    gasdetailscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    gasdetailscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    gasdetailscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    gasdetailscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    gasdetailscold.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2", font9)));
                    gasdetailscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    gasdetailscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    gasdetailscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    gasdetailscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    gasdetailscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    gasdetailscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    gasdetailscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    gasdetailscold.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    gasdetailscold.AddCell(Wpcell);

                    pdfDoc.Add(gasdetailscold);


                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable permithandconfined = new PdfPTable(6);

                    permithandconfined.TotalWidth = 555f;
                    permithandconfined.LockedWidth = true;
                    permithandconfined.SetWidths(new float[] { 2f, 10f, 8f, 9.2f, 5f, 5f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk("9", font8)));

                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("PERMIT APPROVALS", font8)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name", font8)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Remarks ", font8)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Signature", font8)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Date Time", font8)));
                    permithandconfined.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("I have checked the conditions in the field and the status is found as mentioned above. The job can be permitted.", font9)));

                    Wpcell.Colspan = 5;
                    permithandconfined.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Permit Issuer", font8)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandconfined.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Permit Approver(Shift In Charge)", font8)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandconfined.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Stand by Person", font8)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandconfined.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Safety officer", font8)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandconfined.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Process Manager", font8)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandconfined.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Electrical In charge", font8)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandconfined.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Mechanical In charge ", font8)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandconfined.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Instrumental In charge", font8)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandconfined.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Operations Head ", font8)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandconfined.AddCell(Wpcell);


                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("I understood the above condition, arranged safety requirements, explained to the people concerned and received the permit for safe job execution.", font9)));
                    Wpcell.Colspan = 5;

                    permithandconfined.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("")));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name / Number of person(s) working at site", font8)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("")));
                    Wpcell.Colspan = 4;
                    permithandconfined.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Permit Receiver(Engineer /\nTech.of recipient dept).", font8)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandconfined.AddCell(Wpcell);

                    pdfDoc.Add(permithandconfined);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));


                    PdfPTable permitclosureconfined = new PdfPTable(6);

                    permitclosureconfined.TotalWidth = 555f;
                    permitclosureconfined.LockedWidth = true;
                    permitclosureconfined.SetWidths(new float[] { 2f, 10f, 8f, 9.2f, 5f, 5f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk("10", font8)));

                    permitclosureconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("PERMIT CLOSURE", font8)));
                    permitclosureconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name", font8)));
                    permitclosureconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Remarks ", font8)));
                    permitclosureconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Signature", font8)));
                    permitclosureconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Date Time", font8)));
                    permitclosureconfined.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permitclosureconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Certified that the subject work has been completed/ stopped and area cleaned. ", font9)));
                    Wpcell.Colspan = 5;

                    permitclosureconfined.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));

                    permitclosureconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Job completed & Hand over by Receiver", font8)));
                    permitclosureconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosureconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permitclosureconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosureconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosureconfined.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));

                    permitclosureconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Job taken over & Permit closed by issuer", font8)));
                    permitclosureconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosureconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permitclosureconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosureconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosureconfined.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permitclosureconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Shift InCharge", font8)));
                    permitclosureconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosureconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permitclosureconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosureconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosureconfined.AddCell(Wpcell);


                    pdfDoc.Add(permitclosureconfined);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable conrconfined = new PdfPTable(6);
                    PdfPCell pconfined = new PdfPCell();
                    conrconfined.TotalWidth = 555f;
                    conrconfined.LockedWidth = true;
                    conrconfined.SetWidths(new float[] { 1f, 3f, 1f, 3f, 1f, 22f });
                    cell = PhraseCell(new Phrase("11".PadRight(6) + "CONTRACTOR RATING  ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT);
                    cell.Colspan = 6;
                    conrconfined.AddCell(cell);
                    conrconfined.AddCell(uncheckbox);

                    pconfined = new PdfPCell(new Phrase(new Chunk("Red", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD))));
                    conrconfined.AddCell(pconfined);

                    conrconfined.AddCell(uncheckbox);

                    pconfined = new PdfPCell(new Phrase(new Chunk("Yellow", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD))));
                    conrconfined.AddCell(pconfined);

                    conrconfined.AddCell(uncheckbox);

                    pconfined = new PdfPCell(new Phrase(new Chunk("Green", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD))));
                    conrconfined.AddCell(pconfined);

                    pconfined = new PdfPCell(new Phrase(new Chunk(" Remarks (if Red and Yellow rating) :                                                                  \n\n", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL))));
                    pconfined.Colspan = 8;
                    conrconfined.AddCell(pconfined);

                    pdfDoc.Add(conrconfined);
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));


                    PdfPTable cs = new PdfPTable(2);
                    cs.TotalWidth = 555f;
                    cs.LockedWidth = true;
                    cs.SetWidths(new float[] { 2f, 38f });
                    cs.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    cs.AddCell(PhraseCell(new Phrase("CONFINED SPACE – MANENTRY/EXIT LOG SHEET (Use additional sheet, if required)", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(cs);


                    PdfPTable man = new PdfPTable(8);

                    man.TotalWidth = 555f;
                    man.LockedWidth = true;
                    man.SetWidths(new float[] { 2f, 8f, 6f, 4f, 5f, 5f, 5f, 5f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Entrant Name", font8)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("In Time ", font8)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Signature", font8)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Sign of Stand by", font8)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Out time", font8)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Entrant Sign", font8)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Sign of Stand by", font8)));
                    man.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2 ", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("3 ", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("4", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("5 ", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("6", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("7", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    man.AddCell(Wpcell);

                    pdfDoc.Add(man);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable geninsconfined = new PdfPTable(2);
                    geninsconfined.TotalWidth = 555f;
                    geninsconfined.LockedWidth = true;
                    geninsconfined.SetWidths(new float[] { 2f, 38f });
                    geninsconfined.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    geninsconfined.AddCell(PhraseCell(new Phrase("GENERAL INSTRUCTIONS : ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1.", font9)));
                    geninsconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("In case of an Emergency or FIRE ALARM, all works must be stopped and permit stands cancelled. All personnel must leave work site and proceed to emergency Assembly points soon.", font9)));
                    geninsconfined.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2.", font9)));
                    geninsconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("In case of any  LPG/ Gas /Liquid release (or) abnormality noticed, stop work immediately and move to safe location till further instruction,", font9)));
                    geninsconfined.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("3.", font9)));
                    geninsconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("This permit must be available at work site all the time. On job closure (or) expiry, this permit must be returned to the permit issuer/Shift Incharge. ", font9)));
                    geninsconfined.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("4.", font9)));
                    geninsconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Do not commence the work without a valid work permit.", font9)));
                    geninsconfined.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("5.", font9)));
                    geninsconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Acquaint yourself with job and its hazards and ensure the tools used are safe.", font9)));
                    geninsconfined.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("6.", font9)));
                    geninsconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Inform regarding unsafe condition/unsafe act to operator/Safety incharge/Shift incharge", font9)));
                    geninsconfined.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("7.", font9)));
                    geninsconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("When in doubt, consult SASA employee of concerned department (or) SASA Operator ", font9)));
                    geninsconfined.AddCell(Wpcell);


                    Wpcell = new PdfPCell(new Phrase(new Chunk("8.", font9)));
                    geninsconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Work should be started within one hour after issue of this permit", font9)));
                    geninsconfined.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("9.", font9)));
                    geninsconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("All safety precautions should be meticulously followed. Keep access clear for emergency.", font9)));
                    geninsconfined.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("10.", font9)));
                    geninsconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Incase of any emergency, permit should be surrendered to the issuing authority", font9)));
                    geninsconfined.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("11.", font9)));
                    geninsconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Permit is valid only for the mentioned time. After that, further extension is needed before proceeding the job", font9)));
                    geninsconfined.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("12.", font9)));
                    geninsconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Extention: To be extended only, if area and nature of work is same", font9)));
                    geninsconfined.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("13.", font9)));
                    geninsconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Extention: Daily extension should be authorized per the concerned persons, after reviewing the site again.", font9)));
                    geninsconfined.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("14.", font9)));
                    geninsconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Extention: If the job is discontinued, make a fresh permit", font9)));
                    geninsconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("15.", font9)));
                    geninsconfined.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Extension: Extention can be done maximum one week duration. Make new permit every Monday.", font9)));
                    geninsconfined.AddCell(Wpcell);

                    pdfDoc.Add(geninsconfined);


                    PdfPTable consconfined = new PdfPTable(2);
                    consconfined.TotalWidth = 555f;
                    consconfined.LockedWidth = true;
                    consconfined.SetWidths(new float[] { 2f, 38f });
                    consconfined.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    consconfined.AddCell(PhraseCell(new Phrase("PERMIT SPECIFIC INSTRUCTIONS - CONFINED SPACE ", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                    consconfined.AddCell(PhraseCell(new Phrase("1.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    consconfined.AddCell(PhraseCell(new Phrase("Oxygen inside the vessel must be above 20.8%", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    consconfined.AddCell(PhraseCell(new Phrase("2.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    consconfined.AddCell(PhraseCell(new Phrase("Entry/exit log sheet oof personnel entering confined space to be maintained with name of person, ID number by the executing agency / contractor Supervisor.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    consconfined.AddCell(PhraseCell(new Phrase("3.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    consconfined.AddCell(PhraseCell(new Phrase("Job briefing/ Pep safety talk to be done at work site prior to entry in to confined space and working. ", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    consconfined.AddCell(PhraseCell(new Phrase("4.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    consconfined.AddCell(PhraseCell(new Phrase("Provision of standby person, emergency preparedness should be ensured at work site.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    consconfined.AddCell(PhraseCell(new Phrase("5.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    consconfined.AddCell(PhraseCell(new Phrase("For any hotwork inside the vessel, hot work permit should also be ensured", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    consconfined.AddCell(PhraseCell(new Phrase("6.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    consconfined.AddCell(PhraseCell(new Phrase("Do not allow gas cylinders inside the cofined space", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    consconfined.AddCell(PhraseCell(new Phrase("7.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    consconfined.AddCell(PhraseCell(new Phrase("Confined Space Entry permit can not be extended", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));


                    pdfDoc.Add(consconfined);
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(3000)));

                    PdfPTable TitleTableelectrical = new PdfPTable(4);
                    TitleTableelectrical.LockedWidth = true;
                    TitleTableelectrical.SetWidths(new float[] { 10f, 18f, 6.8f, 5.2f });
                    TitleTableelectrical.SpacingBefore = 10f;
                    TitleTableelectrical.SpacingAfter = 1f;
                    TitleTableelectrical.TotalWidth = 555f;


                    Wpcell = new PdfPCell(gif);

                    TitleTableelectrical.AddCell(Wpcell);

                    var phraseelectrical = new Phrase(new Chunk("\n SASA PTA PROJECT ", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    phraseelectrical.Add(new Chunk("ADANA,TURKEY", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    phraseelectrical.Add(new Chunk("\n\n", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    TitleTableelectrical.AddCell(PhraseCell(phraseelectrical, PdfPCell.ALIGN_CENTER));

                    Wpcell = new PdfPCell(new Phrase("\n WORK PERMIT NO:", FontFactory.GetFont("Times New Roman", 12, iTextSharp.text.Font.BOLD)));
                    Wpcell.HorizontalAlignment = Element.ALIGN_CENTER;

                    TitleTableelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase("\n ", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    Wpcell.HorizontalAlignment = Element.ALIGN_LEFT;

                    TitleTableelectrical.AddCell(Wpcell);
                    pdfDoc.Add(TitleTableelectrical);
                    PdfPTable Typeworkelectrical = new PdfPTable(1);

                    Typeworkelectrical.TotalWidth = 555f;
                    Typeworkelectrical.LockedWidth = true;
                    Typeworkelectrical.SetWidths(new float[] { 40f });

                    Typeworkelectrical.AddCell(PhraseCell(new Phrase("ELECTRICAL PERMIT ", FontFactory.GetFont("Times New Roman", 14, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_CENTER));

                    pdfDoc.Add(Typeworkelectrical);


                    PdfPTable permitdetailselectrical = new PdfPTable(5);
                    permitdetailselectrical.TotalWidth = 555f;
                    permitdetailselectrical.LockedWidth = true;
                    permitdetailselectrical.SetWidths(new float[] { 2f, 8f, 9f, 9f, 12f });
                    permitdetailselectrical.AddCell(PhraseCell(new Phrase("1", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetailselectrical.AddCell(PhraseCell(new Phrase("VALIDITY FROM ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetailselectrical.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    permitdetailselectrical.AddCell(PhraseCell(new Phrase("VALIDITY TO ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetailselectrical.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    permitdetailselectrical.AddCell(PhraseCell(new Phrase("2", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetailselectrical.AddCell(PhraseCell(new Phrase("PERMIT ISSUER: ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetailselectrical.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    permitdetailselectrical.AddCell(PhraseCell(new Phrase("PERMIT RECEIVER: ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetailselectrical.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(permitdetailselectrical);
                    PdfPTable plant3 = new PdfPTable(5);
                    plant3.TotalWidth = 555f;
                    plant3.LockedWidth = true;
                    plant3.SetWidths(new float[] { 2f, 8f, 9f, 9f, 12f });
                    plant3.AddCell(PhraseCell(new Phrase("3", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    plant3.AddCell(PhraseCell(new Phrase("PLANT/AREA", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    plant3.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    plant3.AddCell(PhraseCell(new Phrase("JOB ID", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    plant3.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(plant3);

                    PdfPTable Equtable3 = new PdfPTable(5);
                    Equtable3.TotalWidth = 555f;
                    Equtable3.LockedWidth = true;
                    Equtable3.SetWidths(new float[] { 2f, 8f, 9f, 9f, 12f });

                    Equtable3.AddCell(PhraseCell(new Phrase("4", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Equtable3.AddCell(PhraseCell(new Phrase("LOCATION OF THE WORK ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Equtable3.AddCell(PhraseCell(new Phrase("" , FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    Equtable3.AddCell(PhraseCell(new Phrase("EQUIPMENT NAME", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Equtable3.AddCell(PhraseCell(new Phrase("" , FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(Equtable3);
                    PdfPTable desccoldelectrical = new PdfPTable(3);
                    desccoldelectrical.TotalWidth = 555f;
                    desccoldelectrical.LockedWidth = true;
                    desccoldelectrical.SetWidths(new float[] { 2f, 8f, 30f });
                    desccoldelectrical.AddCell(PhraseCell(new Phrase("5", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    desccoldelectrical.AddCell(PhraseCell(new Phrase("DESCRIPTION OF WORK  ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    desccoldelectrical.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(desccoldelectrical);


                    PdfPTable loccoldelectrical = new PdfPTable(5);
                    loccoldelectrical.TotalWidth = 555f;
                    loccoldelectrical.LockedWidth = true;
                    loccoldelectrical.SetWidths(new float[] { 2f, 8f, 9f, 9f, 12f });


                    loccoldelectrical.AddCell(PhraseCell(new Phrase("6", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    loccoldelectrical.AddCell(PhraseCell(new Phrase("WORK DONE BY ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    loccoldelectrical.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    loccoldelectrical.AddCell(PhraseCell(new Phrase("DEPARTMENT", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    loccoldelectrical.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));


                    loccoldelectrical.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    loccoldelectrical.AddCell(PhraseCell(new Phrase("NAME OF CONTRACTOR", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    loccoldelectrical.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    loccoldelectrical.AddCell(PhraseCell(new Phrase("RISK ASSESSMENT REQUIRED?", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    loccoldelectrical.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(loccoldelectrical);

                    PdfPTable areaelectrical = new PdfPTable(2);
                    areaelectrical.TotalWidth = 555f;
                    areaelectrical.LockedWidth = true;
                    areaelectrical.SetWidths(new float[] { 2f, 38f });
                    areaelectrical.AddCell(PhraseCell(new Phrase("7", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    areaelectrical.AddCell(PhraseCell(new Phrase("Following points should be checked & verified in Field before issuing the Work Permit as per the nature of job.", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    pdfDoc.Add(areaelectrical);

                    PdfPTable genelectrical = new PdfPTable(8);

                    genelectrical.TotalWidth = 555f;
                    genelectrical.LockedWidth = true;
                    genelectrical.SetWidths(new float[] { 2f, 2.5f, 3.5f, 12f, 2f, 2.5f, 3.5f, 12f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    genelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Done", font8)));
                    genelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Not Required", font8)));
                    genelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Check List", font8)));
                    genelectrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    genelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Done", font8)));
                    genelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Not Required", font8)));
                    genelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Check List", font8)));
                    genelectrical.AddCell(Wpcell);


                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[0].Rows.Count; rows++)
                        {

                            genelectrical.AddCell(PhraseCell(new Phrase(dataSet.Tables[0].Rows[rows][0].ToString(), font9), PdfPCell.ALIGN_CENTER));
                            genelectrical.AddCell("");
                            genelectrical.AddCell("");
                            Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[0].Rows[rows][1].ToString(), font9)));
                            genelectrical.AddCell(Wpcell);

                        }
                    }
                    genelectrical.AddCell("");
                    genelectrical.AddCell("");
                    genelectrical.AddCell("");
                    genelectrical.AddCell("");
                    pdfDoc.Add(genelectrical);

                    PdfPTable actualchklistElectrical = new PdfPTable(8);

                    actualchklistElectrical.TotalWidth = 555f;
                    actualchklistElectrical.LockedWidth = true;
                    actualchklistElectrical.SetWidths(new float[] { 2f, 2.5f, 3.5f, 12f, 2f, 2.5f, 3.5f, 12f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));

                    actualchklistElectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actualchklistElectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actualchklistElectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Work Specific Check List", font8)));
                    actualchklistElectrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actualchklistElectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actualchklistElectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actualchklistElectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Work Specific Check List", font8)));
                    actualchklistElectrical.AddCell(Wpcell);


                    if (dataSet.Tables[4].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[4].Rows.Count; rows++)
                        {


                            actualchklistElectrical.AddCell(PhraseCell(new Phrase(dataSet.Tables[4].Rows[rows][0].ToString(), font9), PdfPCell.ALIGN_CENTER));
                            actualchklistElectrical.AddCell("");
                            actualchklistElectrical.AddCell("");
                            Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[4].Rows[rows][1].ToString(), font9)));
                            actualchklistElectrical.AddCell(Wpcell);

                        }
                    }

                    pdfDoc.Add(actualchklistElectrical);



                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable ppeelectrical = new PdfPTable(2);
                    ppeelectrical.TotalWidth = 555f;
                    ppeelectrical.LockedWidth = true;
                    ppeelectrical.SetWidths(new float[] { 2f, 38f });
                    ppeelectrical.AddCell(PhraseCell(new Phrase("8", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    ppeelectrical.AddCell(PhraseCell(new Phrase("SPECIAL PROTECTION / PPE", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(ppeelectrical);


                    PdfPTable ppe1eelctrical = new PdfPTable(8);
                    ppe1eelctrical.TotalWidth = 555f;
                    ppe1eelctrical.LockedWidth = true;
                    ppe1eelctrical.SetWidths(new float[] { 2f, 8f, 2f, 8f, 2f, 8f, 2f, 8f });

                    if (dataSet.Tables[1].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[1].Rows.Count; rows++)
                        {

                            ppe1eelctrical.AddCell(uncheckbox);

                            Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[1].Rows[rows][1].ToString(), font9)));
                            ppe1eelctrical.AddCell(Wpcell);

                        }
                    }

                    else
                    {
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Applicable", font9)));
                        Wpcell.Colspan = 8;
                        ppe1eelctrical.AddCell(Wpcell);

                    }
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    ppe1eelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));

                    ppe1eelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));

                    ppe1eelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));

                    ppe1eelctrical.AddCell(Wpcell);


                    Wpcell = new PdfPCell(new Phrase(new Chunk("")));
                    Wpcell.Colspan = 8;
                    ppe1eelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("8.1 Others / L.C. No :", font8)));
                    Wpcell.Colspan = 8;
                    ppe1eelctrical.AddCell(Wpcell);

                    pdfDoc.Add(ppe1eelctrical);
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));


                    PdfPTable permithandelectrical = new PdfPTable(6);

                    permithandelectrical.TotalWidth = 555f;
                    permithandelectrical.LockedWidth = true;
                    permithandelectrical.SetWidths(new float[] { 2f, 10f, 8f, 9.2f, 5f, 5f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk("9", font8)));

                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("PERMIT APPROVALS", font8)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name", font8)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Remarks ", font8)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Signature", font8)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Date Time", font8)));
                    permithandelectrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("I have checked the conditions in the field and the status is found as mentioned above. The job can be permitted.", font9)));

                    Wpcell.Colspan = 5;
                    permithandelectrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Permit Issuer", font8)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandelectrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Permit Approver(Shift In Charge)", font8)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandelectrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Stand by Person", font8)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandelectrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Safety officer", font8)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandelectrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Process Manager", font8)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandelectrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Electrical In charge", font8)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandelectrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Mechanical In charge  ", font8)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandelectrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Instrumental In charge", font8)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandelectrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Operations Head ", font8)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandelectrical.AddCell(Wpcell);


                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("I understood the above condition, arranged safety requirements, explained to the people concerned and received the permit for safe job execution.", font9)));
                    Wpcell.Colspan = 5;
                    ;
                    permithandelectrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("")));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name / Number of person(s) working at site", font8)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("")));
                    Wpcell.Colspan = 4;
                    permithandelectrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Permit Receiver(Engineer /\nTech.of recipient dept).", font8)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandelectrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandelectrical.AddCell(Wpcell);

                    pdfDoc.Add(permithandelectrical);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));


                    PdfPTable permitclosurelectricl = new PdfPTable(6);

                    permitclosurelectricl.TotalWidth = 555f;
                    permitclosurelectricl.LockedWidth = true;
                    permitclosurelectricl.SetWidths(new float[] { 2f, 10f, 8f, 9.2f, 5f, 5f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk("10", font8)));

                    permitclosurelectricl.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("PERMIT CLOSURE", font8)));
                    permitclosurelectricl.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name", font8)));
                    permitclosurelectricl.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Remarks ", font8)));
                    permitclosurelectricl.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Signature", font8)));
                    permitclosurelectricl.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Date Time", font8)));
                    permitclosurelectricl.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permitclosurelectricl.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Certified that the subject work has been completed/ stopped and area cleaned. ", font9)));
                    Wpcell.Colspan = 5;

                    permitclosurelectricl.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));

                    permitclosurelectricl.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Job completed & Hand over by Receiver", font8)));
                    permitclosurelectricl.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurelectricl.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permitclosurelectricl.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurelectricl.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurelectricl.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));

                    permitclosurelectricl.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Job taken over & Permit closed by issuer", font8)));
                    permitclosurelectricl.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurelectricl.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permitclosurelectricl.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurelectricl.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurelectricl.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permitclosurelectricl.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Shift InCharge", font8)));
                    permitclosurelectricl.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurelectricl.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permitclosurelectricl.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurelectricl.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurelectricl.AddCell(Wpcell);


                    pdfDoc.Add(permitclosurelectricl);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable conrelectrical = new PdfPTable(6);
                    PdfPCell pelectrical = new PdfPCell();
                    conrelectrical.TotalWidth = 555f;
                    conrelectrical.LockedWidth = true;
                    conrelectrical.SetWidths(new float[] { 1f, 3f, 1f, 3f, 1f, 22f });
                    cell = PhraseCell(new Phrase("11".PadRight(6) + "CONTRACTOR RATING  ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT);
                    cell.Colspan = 6;
                    conrelectrical.AddCell(cell);
                    conrelectrical.AddCell(uncheckbox);

                    pelectrical = new PdfPCell(new Phrase(new Chunk("Red", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD))));
                    conrelectrical.AddCell(pelectrical);

                    conrelectrical.AddCell(uncheckbox);

                    pelectrical = new PdfPCell(new Phrase(new Chunk("Yellow", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD))));
                    conrelectrical.AddCell(pelectrical);

                    conrelectrical.AddCell(uncheckbox);

                    pelectrical = new PdfPCell(new Phrase(new Chunk("Green", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD))));
                    conrelectrical.AddCell(pelectrical);

                    pelectrical = new PdfPCell(new Phrase(new Chunk(" Remarks (if Red and Yellow rating) :                                                                  \n\n", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL))));
                    pelectrical.Colspan = 8;
                    conrelectrical.AddCell(pelectrical);

                    pdfDoc.Add(conrelectrical);
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable renelectrical = new PdfPTable(2);
                    renelectrical.TotalWidth = 555f;
                    renelectrical.LockedWidth = true;
                    renelectrical.SetWidths(new float[] { 2f, 28f });
                    renelectrical.AddCell(PhraseCell(new Phrase("12", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    renelectrical.AddCell(PhraseCell(new Phrase("RENEWAL OF WORK PERMIT", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(renelectrical);


                    PdfPTable renewaleelctrical = new PdfPTable(9);

                    renewaleelctrical.TotalWidth = 555f;
                    renewaleelctrical.LockedWidth = true;
                    renewaleelctrical.SetWidths(new float[] { 7f, 5f, 4f, 4f, 4f, 4f, 4f, 4f, 4f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Day/ Date/Time", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. LEL Level (ZERO)", font8)));

                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of Permit issuer ", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of shift In charge ", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of Stand by", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of safety In charge", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of Manager Process", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of Operations Head", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of  Receiver", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. % O2(19.5 - 22.5 %)", font8)));

                    renewaleelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("First Extension Date:................................", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell.Rowspan = 2;
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                    renewaleelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Second Extension Date:................................", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell.Rowspan = 2;
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                    renewaleelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Third Extension Date:................................", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell.Rowspan = 2;
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                    renewaleelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Fourth Extension Date:................................", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell.Rowspan = 2;
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                    renewaleelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Fifth Extension Date:................................", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell.Rowspan = 2;
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                    renewaleelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Sixth Extension Date:................................", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);
                    Wpcell.Rowspan = 2;
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewaleelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                    renewaleelctrical.AddCell(Wpcell);


                    pdfDoc.Add(renewaleelctrical);


                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable geninseelctrical = new PdfPTable(2);
                    geninseelctrical.TotalWidth = 555f;
                    geninseelctrical.LockedWidth = true;
                    geninseelctrical.SetWidths(new float[] { 2f, 38f });
                    geninseelctrical.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    geninseelctrical.AddCell(PhraseCell(new Phrase("GENERAL INSTRUCTIONS : ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1.", font9)));
                    geninseelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("In case of an Emergency or FIRE ALARM, all works must be stopped and permit stands cancelled. All personnel must leave work site and proceed to emergency Assembly points soon.", font9)));
                    geninseelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2.", font9)));
                    geninseelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("In case of any  LPG/ Gas /Liquid release (or) abnormality noticed, stop work immediately and move to safe location till further instruction,", font9)));
                    geninseelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("3.", font9)));
                    geninseelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("This permit must be available at work site all the time. On job closure (or) expiry, this permit must be returned to the permit issuer/Shift Incharge. ", font9)));
                    geninseelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("4.", font9)));
                    geninseelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Do not commence the work without a valid work permit.", font9)));
                    geninseelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("5.", font9)));
                    geninseelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Acquaint yourself with job and its hazards and ensure the tools used are safe.", font9)));
                    geninseelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("6.", font9)));
                    geninseelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Inform regarding unsafe condition/unsafe act to operator/Safety incharge/Shift incharge", font9)));
                    geninseelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("7.", font9)));
                    geninseelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("When in doubt, consult SASA employee of concerned department (or) SASA Operator ", font9)));
                    geninseelctrical.AddCell(Wpcell);


                    Wpcell = new PdfPCell(new Phrase(new Chunk("8.", font9)));
                    geninseelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Work should be started within one hour after issue of this permit", font9)));
                    geninseelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("9.", font9)));
                    geninseelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("All safety precautions should be meticulously followed. Keep access clear for emergency.", font9)));
                    geninseelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("10.", font9)));
                    geninseelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Incase of any emergency, permit should be surrendered to the issuing authority", font9)));
                    geninseelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("11.", font9)));
                    geninseelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Permit is valid only for the mentioned time. After that, further extension is needed before proceeding the job", font9)));
                    geninseelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("12.", font9)));
                    geninseelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Extention: To be extended only, if area and nature of work is same", font9)));
                    geninseelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("13.", font9)));
                    geninseelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Extention: Daily extension should be authorized per the concerned persons, after reviewing the site again.", font9)));
                    geninseelctrical.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("14.", font9)));
                    geninseelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Extention: If the job is discontinued, make a fresh permit", font9)));
                    geninseelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("15.", font9)));
                    geninseelctrical.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Extension: Extention can be done maximum one week duration. Make new permit every Monday.", font9)));
                    geninseelctrical.AddCell(Wpcell);

                    pdfDoc.Add(geninseelctrical);
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(3000)));

                    PdfPTable TitleTableex = new PdfPTable(4);
                    TitleTableex.LockedWidth = true;
                    TitleTableex.SetWidths(new float[] { 10f, 18f, 6.8f, 5.2f });
                    TitleTableex.SpacingBefore = 10f;
                    TitleTableex.SpacingAfter = 1f;
                    TitleTableex.TotalWidth = 555f;


                    Wpcell = new PdfPCell(gif);

                    TitleTableex.AddCell(Wpcell);

                    var phraseex = new Phrase(new Chunk("\n SASA PTA PROJECT ", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    phraseex.Add(new Chunk("ADANA,TURKEY", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    phraseex.Add(new Chunk("\n\n", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    TitleTableex.AddCell(PhraseCell(phraseex, PdfPCell.ALIGN_CENTER));

                    Wpcell = new PdfPCell(new Phrase("\n WORK PERMIT NO:", FontFactory.GetFont("Times New Roman", 12, iTextSharp.text.Font.BOLD)));
                    Wpcell.HorizontalAlignment = Element.ALIGN_CENTER;

                    TitleTableex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase("\n ", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    Wpcell.HorizontalAlignment = Element.ALIGN_LEFT;

                    TitleTableex.AddCell(Wpcell);
                    pdfDoc.Add(TitleTableex);


                    PdfPTable Typeworkex = new PdfPTable(1);

                    Typeworkex.TotalWidth = 555f;
                    Typeworkex.LockedWidth = true;
                    Typeworkex.SetWidths(new float[] { 40f });

                    Typeworkex.AddCell(PhraseCell(new Phrase("EXCAVATION PERMIT", FontFactory.GetFont("Times New Roman", 14, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_CENTER));

                    pdfDoc.Add(Typeworkex);

                    PdfPTable permitdetailsec = new PdfPTable(5);
                    permitdetailsec.TotalWidth = 555f;
                    permitdetailsec.LockedWidth = true;
                    permitdetailsec.SetWidths(new float[] { 2f, 8f, 9f, 9f, 12f });
                    permitdetailsec.AddCell(PhraseCell(new Phrase("1", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetailsec.AddCell(PhraseCell(new Phrase("VALIDITY FROM ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetailsec.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    permitdetailsec.AddCell(PhraseCell(new Phrase("VALIDITY TO ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetailsec.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    permitdetailsec.AddCell(PhraseCell(new Phrase("2", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetailsec.AddCell(PhraseCell(new Phrase("PERMIT ISSUER: ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetailsec.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    permitdetailsec.AddCell(PhraseCell(new Phrase("PERMIT RECEIVER: ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetailsec.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(permitdetailsec);
                    PdfPTable plant4 = new PdfPTable(5);
                    plant4.TotalWidth = 555f;
                    plant4.LockedWidth = true;
                    plant4.SetWidths(new float[] { 2f, 8f, 9f, 9f, 12f });
                    plant4.AddCell(PhraseCell(new Phrase("3", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    plant4.AddCell(PhraseCell(new Phrase("PLANT/AREA", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    plant4.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    plant4.AddCell(PhraseCell(new Phrase("JOB ID", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    plant4.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(plant4);

                    PdfPTable Equtable4 = new PdfPTable(5);
                    Equtable4.TotalWidth = 555f;
                    Equtable4.LockedWidth = true;
                    Equtable4.SetWidths(new float[] { 2f, 8f, 9f, 9f, 12f });

                    Equtable4.AddCell(PhraseCell(new Phrase("4", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Equtable4.AddCell(PhraseCell(new Phrase("LOCATION OF THE WORK ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Equtable4.AddCell(PhraseCell(new Phrase("" , FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    Equtable4.AddCell(PhraseCell(new Phrase("EQUIPMENT NAME", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Equtable4.AddCell(PhraseCell(new Phrase("" , FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(Equtable4);
                    PdfPTable descexl = new PdfPTable(3);
                    descexl.TotalWidth = 555f;
                    descexl.LockedWidth = true;
                    descexl.SetWidths(new float[] { 2f, 8f, 30f });
                    descexl.AddCell(PhraseCell(new Phrase("5", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    descexl.AddCell(PhraseCell(new Phrase("DESCRIPTION OF WORK  ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    descexl.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(descexl);


                    PdfPTable locex = new PdfPTable(5);
                    locex.TotalWidth = 555f;
                    locex.LockedWidth = true;
                    locex.SetWidths(new float[] { 2f, 8f, 9f, 9f, 12f });

                    locex.AddCell(PhraseCell(new Phrase("6", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    locex.AddCell(PhraseCell(new Phrase("LOCATION OF THE WORK ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    locex.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    locex.AddCell(PhraseCell(new Phrase("EQUIPMENT NAME", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    locex.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));


                    locex.AddCell(PhraseCell(new Phrase("7", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    locex.AddCell(PhraseCell(new Phrase("WORK DONE BY ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    locex.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    locex.AddCell(PhraseCell(new Phrase("DEPARTMENT", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    locex.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));


                    locex.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    locex.AddCell(PhraseCell(new Phrase("NAME OF CONTRACTOR", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    locex.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    locex.AddCell(PhraseCell(new Phrase("RISK ASSESSMENT REQUIRED?", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    locex.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(locex);

                    PdfPTable areaex = new PdfPTable(2);
                    areaex.TotalWidth = 555f;
                    areaex.LockedWidth = true;
                    areaex.SetWidths(new float[] { 2f, 38f });
                    areaex.AddCell(PhraseCell(new Phrase("8", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    areaex.AddCell(PhraseCell(new Phrase(" Following points should be checked & verified in Field before issuing the Work Permit as per the nature of job.", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    pdfDoc.Add(areaex);

                    PdfPTable genex = new PdfPTable(8);

                    genex.TotalWidth = 555f;
                    genex.LockedWidth = true;
                    genex.SetWidths(new float[] { 2f, 2.5f, 3.5f, 12f, 2f, 2.5f, 3.5f, 12f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    genex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Done", font8)));
                    genex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Not Required", font8)));
                    genex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Check List", font8)));
                    genex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    genex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Done", font8)));
                    genex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Not Required", font8)));
                    genex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Check List", font8)));
                    genex.AddCell(Wpcell);


                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[0].Rows.Count; rows++)
                        {

                            genex.AddCell(PhraseCell(new Phrase(dataSet.Tables[0].Rows[rows][0].ToString(), font9), PdfPCell.ALIGN_CENTER));
                            genex.AddCell("");
                            genex.AddCell("");
                            Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[0].Rows[rows][1].ToString(), font9)));
                            genex.AddCell(Wpcell);

                        }
                    }
                    genex.AddCell("");
                    genex.AddCell("");
                    genex.AddCell("");
                    genex.AddCell("");
                    pdfDoc.Add(genex);

                    PdfPTable actualchkex = new PdfPTable(8);

                    actualchkex.TotalWidth = 555f;
                    actualchkex.LockedWidth = true;
                    actualchkex.SetWidths(new float[] { 2f, 2.5f, 3.5f, 12f, 2f, 2.5f, 3.5f, 12f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));

                    actualchkex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actualchkex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actualchkex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Work Specific Check List", font8)));
                    actualchkex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actualchkex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actualchkex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actualchkex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Work Specific Check List", font8)));
                    actualchkex.AddCell(Wpcell);


                    if (dataSet.Tables[5].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[5].Rows.Count; rows++)
                        {


                            actualchkex.AddCell(PhraseCell(new Phrase(dataSet.Tables[5].Rows[rows][0].ToString(), font9), PdfPCell.ALIGN_CENTER));
                            actualchkex.AddCell("");
                            actualchkex.AddCell("");
                            Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[5].Rows[rows][1].ToString(), font9)));
                            actualchkex.AddCell(Wpcell);

                        }
                    }

                    pdfDoc.Add(actualchkex);



                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable ppeex = new PdfPTable(2);
                    ppeex.TotalWidth = 555f;
                    ppeex.LockedWidth = true;
                    ppeex.SetWidths(new float[] { 2f, 38f });
                    ppeex.AddCell(PhraseCell(new Phrase("9", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    ppeex.AddCell(PhraseCell(new Phrase("SPECIAL PROTECTION / PPE", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(ppeex);


                    PdfPTable ppeex1 = new PdfPTable(8);
                    ppeex1.TotalWidth = 555f;
                    ppeex1.LockedWidth = true;
                    ppeex1.SetWidths(new float[] { 2f, 8f, 2f, 8f, 2f, 8f, 2f, 8f });

                    if (dataSet.Tables[1].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[1].Rows.Count; rows++)
                        {

                            ppeex1.AddCell(uncheckbox);

                            Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[1].Rows[rows][1].ToString(), font9)));
                            ppeex1.AddCell(Wpcell);

                        }
                    }

                    else
                    {
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Applicable", font9)));
                        Wpcell.Colspan = 8;
                        ppeex1.AddCell(Wpcell);

                    }
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    ppeex1.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));

                    ppeex1.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));

                    ppeex1.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));

                    ppeex1.AddCell(Wpcell);


                    Wpcell = new PdfPCell(new Phrase(new Chunk("")));
                    Wpcell.Colspan = 8;
                    ppeex1.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("9.1 Others / L.C. No :", font8)));
                    Wpcell.Colspan = 8;
                    ppeex1.AddCell(Wpcell);

                    pdfDoc.Add(ppeex1);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));


                    PdfPTable permithandEX = new PdfPTable(6);

                    permithandEX.TotalWidth = 555f;
                    permithandEX.LockedWidth = true;
                    permithandEX.SetWidths(new float[] { 2f, 10f, 8f, 9.2f, 5f, 5f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk("10", font8)));

                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("PERMIT APPROVALS", font8)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name", font8)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Remarks ", font8)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Signature", font8)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Date Time", font8)));
                    permithandEX.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("I have checked the conditions in the field and the status is found as mentioned above. The job can be permitted.", font9)));

                    Wpcell.Colspan = 5;
                    permithandEX.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Permit Issuer", font8)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandEX.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Permit Approver(Shift In Charge)", font8)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandEX.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Stand by Person", font8)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandEX.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Safety officer", font8)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandEX.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Process Manager", font8)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandEX.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Electrical In charge", font8)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandEX.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Mechanical In charge ", font8)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandEX.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Instrumental In charge", font8)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandEX.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Operations Head ", font8)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandEX.AddCell(Wpcell);


                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("I understood the above condition,arranged safety requirements, explained to the people concerned and received the permit for safe job execution.", font9)));
                    Wpcell.Colspan = 5;

                    permithandEX.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("")));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name / Number of person(s) working at site", font8)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("")));
                    Wpcell.Colspan = 4;
                    permithandEX.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Permit Receiver(Engineer /\nTech.of recipient dept).", font8)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandEX.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandEX.AddCell(Wpcell);

                    pdfDoc.Add(permithandEX);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));


                    PdfPTable permitclosureex = new PdfPTable(6);

                    permitclosureex.TotalWidth = 555f;
                    permitclosureex.LockedWidth = true;
                    permitclosureex.SetWidths(new float[] { 2f, 10f, 8f, 9.2f, 5f, 5f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk("11", font8)));

                    permitclosureex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("PERMIT CLOSURE", font8)));
                    permitclosureex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name", font8)));
                    permitclosureex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Remarks ", font8)));
                    permitclosureex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Signature", font8)));
                    permitclosureex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Date Time", font8)));
                    permitclosureex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permitclosureex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Certified that the subject work has been completed/ stopped and area cleaned. ", font9)));
                    Wpcell.Colspan = 5;

                    permitclosureex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permitclosureex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Job completed & Hand over by Receiver", font8)));
                    permitclosureex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosureex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permitclosureex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosureex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosureex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));

                    permitclosureex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Job taken over & Permit closed by issuer", font8)));
                    permitclosureex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosureex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permitclosureex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosureex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosureex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permitclosureex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Shift InCharge", font8)));
                    permitclosureex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosureex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permitclosureex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosureex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosureex.AddCell(Wpcell);


                    pdfDoc.Add(permitclosureex);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable conrex = new PdfPTable(6);
                    PdfPCell pex = new PdfPCell();
                    conrex.TotalWidth = 555f;
                    conrex.LockedWidth = true;
                    conrex.SetWidths(new float[] { 1f, 3f, 1f, 3f, 1f, 22f });
                    cell = PhraseCell(new Phrase("12".PadRight(6) + "CONTRACTOR RATING  ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT);
                    cell.Colspan = 6;
                    conrex.AddCell(cell);
                    conrex.AddCell(uncheckbox);

                    pex = new PdfPCell(new Phrase(new Chunk("Red", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD))));
                    conrex.AddCell(pex);

                    conrex.AddCell(uncheckbox);

                    pex = new PdfPCell(new Phrase(new Chunk("Yellow", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD))));
                    conrex.AddCell(pex);

                    conrex.AddCell(uncheckbox);

                    pex = new PdfPCell(new Phrase(new Chunk("Green", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD))));
                    conrex.AddCell(pex);

                    pex = new PdfPCell(new Phrase(new Chunk(" Remarks (if Red and Yellow rating) :                                                                  \n\n", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL))));
                    pex.Colspan = 8;
                    conrex.AddCell(pex);

                    pdfDoc.Add(conrex);
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));



                    PdfPTable renex = new PdfPTable(2);
                    renex.TotalWidth = 555f;
                    renex.LockedWidth = true;
                    renex.SetWidths(new float[] { 2f, 38f });
                    renex.AddCell(PhraseCell(new Phrase("13", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    renex.AddCell(PhraseCell(new Phrase("RENEWAL OF WORK PERMIT", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(renex);


                    PdfPTable renewalex = new PdfPTable(9);

                    renewalex.TotalWidth = 555f;
                    renewalex.LockedWidth = true;
                    renewalex.SetWidths(new float[] { 7f, 5f, 4f, 4f, 4f, 4f, 4f, 4f, 4f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Day/ Date/Time", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. LEL Level (ZERO)", font8)));

                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of Permit issuer ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of shift In charge ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of Stand by", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of safety In charge", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of Manager Process", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of Operations Head", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of  Receiver", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. % O2(19.5 - 22.5 %)", font8)));

                    renewalex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("First Extension Date:................................", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell.Rowspan = 2;
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                    renewalex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Second Extension Date:................................", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell.Rowspan = 2;
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                    renewalex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Third Extension Date:................................", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell.Rowspan = 2;
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                    renewalex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Fourth Extension Date:................................", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell.Rowspan = 2;
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                    renewalex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Fifth Extension Date:................................", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell.Rowspan = 2;
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                    renewalex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Sixth Extension Date:................................", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);
                    Wpcell.Rowspan = 2;
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                    renewalex.AddCell(Wpcell);


                    pdfDoc.Add(renewalex);


                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable geninsex = new PdfPTable(2);
                    geninsex.TotalWidth = 555f;
                    geninsex.LockedWidth = true;
                    geninsex.SetWidths(new float[] { 2f, 38f });
                    geninsex.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    geninsex.AddCell(PhraseCell(new Phrase("GENERAL INSTRUCTIONS : ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1.", font9)));
                    geninsex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("In case of an Emergency or FIRE ALARM, all works must be stopped and permit stands cancelled. All personnel must leave work site and proceed to emergency Assembly points soon.", font9)));
                    geninsex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2.", font9)));
                    geninsex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("In case of any  LPG/ Gas /Liquid release (or) abnormality noticed, stop work immediately and move to safe location till further instruction,", font9)));
                    geninsex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("3.", font9)));
                    geninsex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("This permit must be available at work site all the time. On job closure (or) expiry, this permit must be returned to the permit issuer/Shift Incharge. ", font9)));
                    geninsex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("4.", font9)));
                    geninsex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Do not commence the work without a valid work permit.", font9)));
                    geninsex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("5.", font9)));
                    geninsex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Acquaint yourself with job and its hazards and ensure the tools used are safe.", font9)));
                    geninsex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("6.", font9)));
                    geninsex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Inform regarding unsafe condition/unsafe act to operator/Safety incharge/Shift incharge", font9)));
                    geninsex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("7.", font9)));
                    geninsex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("When in doubt, consult SASA employee of concerned department (or) SASA Operator ", font9)));
                    geninsex.AddCell(Wpcell);


                    Wpcell = new PdfPCell(new Phrase(new Chunk("8.", font9)));
                    geninsex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Work should be started within one hour after issue of this permit", font9)));
                    geninsex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("9.", font9)));
                    geninsex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("All safety precautions should be meticulously followed. Keep access clear for emergency.", font9)));
                    geninsex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("10.", font9)));
                    geninsex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Incase of any emergency, permit should be surrendered to the issuing authority", font9)));
                    geninsex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("11.", font9)));
                    geninsex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Permit is valid only for the mentioned time. After that, further extension is needed before proceeding the job", font9)));
                    geninsex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("12.", font9)));
                    geninsex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Extention: To be extended only, if area and nature of work is same", font9)));
                    geninsex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("13.", font9)));
                    geninsex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Extention: Daily extension should be authorized per the concerned persons, after reviewing the site again.", font9)));
                    geninsex.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("14.", font9)));
                    geninsex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Extention: If the job is discontinued, make a fresh permit", font9)));
                    geninsex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("15.", font9)));
                    geninsex.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Extension: Extention can be done maximum one week duration. Make new permit every Monday.", font9)));
                    geninsex.AddCell(Wpcell);

                    pdfDoc.Add(genins);

                    PdfPTable Excav1 = new PdfPTable(2);
                    Excav1.TotalWidth = 555f;
                    Excav1.LockedWidth = true;
                    Excav1.SetWidths(new float[] { 2f, 38f });


                    Excav1.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Excav1.AddCell(PhraseCell(new Phrase("PERMIT SPECIFIC INSTRUCTIONS -Excavation ", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Excav1.AddCell(PhraseCell(new Phrase("1.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    Excav1.AddCell(PhraseCell(new Phrase("Refer the below Sketch (or) attached sketch for excavation details", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));


                    pdfDoc.Add(Excav1);

                    PdfPTable ExcavDraw1 = new PdfPTable(1);
                    ExcavDraw1.TotalWidth = 555f;

                    ExcavDraw1.LockedWidth = true;
                    ExcavDraw1.SetWidths(new float[] { 38f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));

                    Wpcell.FixedHeight = 130f;
                    ExcavDraw1.AddCell(Wpcell);
                    pdfDoc.Add(ExcavDraw1);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable TitleTableLif = new PdfPTable(4);
                    TitleTableLif.LockedWidth = true;
                    TitleTableLif.SetWidths(new float[] { 10f, 18f, 6.8f, 5.2f });
                    TitleTableLif.SpacingBefore = 10f;
                    TitleTableLif.SpacingAfter = 1f;
                    TitleTableLif.TotalWidth = 555f;


                    Wpcell = new PdfPCell(gif);

                    TitleTableLif.AddCell(Wpcell);

                    var phraselif = new Phrase(new Chunk("\n SASA PTA PROJECT ", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    phraselif.Add(new Chunk("ADANA,TURKEY", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    phraselif.Add(new Chunk("\n\n", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    TitleTableLif.AddCell(PhraseCell(phraselif, PdfPCell.ALIGN_CENTER));

                    Wpcell = new PdfPCell(new Phrase("\n WORK PERMIT NO:", FontFactory.GetFont("Times New Roman", 12, iTextSharp.text.Font.BOLD)));
                    Wpcell.HorizontalAlignment = Element.ALIGN_CENTER;

                    TitleTableLif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase("\n ", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    Wpcell.HorizontalAlignment = Element.ALIGN_LEFT;

                    TitleTableLif.AddCell(Wpcell);
                    pdfDoc.Add(TitleTableex);

                    PdfPTable Typeworklif = new PdfPTable(1);

                    Typeworklif.TotalWidth = 555f;
                    Typeworklif.LockedWidth = true;
                    Typeworklif.SetWidths(new float[] { 40f });

                    Typeworklif.AddCell(PhraseCell(new Phrase("LIFTING PERMIT", FontFactory.GetFont("Times New Roman", 14, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_CENTER));

                    pdfDoc.Add(Typeworklif);

                    PdfPTable permitdetaillif = new PdfPTable(5);
                    permitdetaillif.TotalWidth = 555f;
                    permitdetaillif.LockedWidth = true;
                    permitdetaillif.SetWidths(new float[] { 2f, 8f, 9f, 9f, 12f });
                    permitdetaillif.AddCell(PhraseCell(new Phrase("1", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetaillif.AddCell(PhraseCell(new Phrase("VALIDITY FROM ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetaillif.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    permitdetaillif.AddCell(PhraseCell(new Phrase("VALIDITY TO ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetaillif.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    permitdetaillif.AddCell(PhraseCell(new Phrase("2", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetaillif.AddCell(PhraseCell(new Phrase("PERMIT ISSUER: ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetaillif.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    permitdetaillif.AddCell(PhraseCell(new Phrase("PERMIT RECEIVER: ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetaillif.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(permitdetaillif);
                    PdfPTable plant5 = new PdfPTable(5);
                    plant5.TotalWidth = 555f;
                    plant5.LockedWidth = true;
                    plant5.SetWidths(new float[] { 2f, 8f, 9f, 9f, 12f });
                    plant5.AddCell(PhraseCell(new Phrase("3", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    plant5.AddCell(PhraseCell(new Phrase("PLANT/AREA", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    plant5.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    plant5.AddCell(PhraseCell(new Phrase("JOB ID", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    plant5.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(plant5);

                    PdfPTable Equtable5 = new PdfPTable(5);
                    Equtable5.TotalWidth = 555f;
                    Equtable5.LockedWidth = true;
                    Equtable5.SetWidths(new float[] { 2f, 8f, 9f, 9f, 12f });

                    Equtable5.AddCell(PhraseCell(new Phrase("4", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Equtable5.AddCell(PhraseCell(new Phrase("LOCATION OF THE WORK ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Equtable5.AddCell(PhraseCell(new Phrase("" , FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    Equtable5.AddCell(PhraseCell(new Phrase("EQUIPMENT NAME", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Equtable5.AddCell(PhraseCell(new Phrase("" , FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(Equtable5);

                    PdfPTable desclif = new PdfPTable(3);
                    desclif.TotalWidth = 555f;
                    desclif.LockedWidth = true;
                    desclif.SetWidths(new float[] { 2f, 8f, 30f });
                    desclif.AddCell(PhraseCell(new Phrase("5", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    desclif.AddCell(PhraseCell(new Phrase("DESCRIPTION OF WORK  ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    desclif.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(desclif);


                    PdfPTable loclif = new PdfPTable(5);
                    loclif.TotalWidth = 555f;
                    loclif.LockedWidth = true;
                    loclif.SetWidths(new float[] { 2f, 8f, 9f, 9f, 12f });

                    loclif.AddCell(PhraseCell(new Phrase("6", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    loclif.AddCell(PhraseCell(new Phrase("WORK DONE BY ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    loclif.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    loclif.AddCell(PhraseCell(new Phrase("DEPARTMENT", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    loclif.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));


                    loclif.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    loclif.AddCell(PhraseCell(new Phrase("NAME OF CONTRACTOR", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    loclif.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    loclif.AddCell(PhraseCell(new Phrase("RISK ASSESSMENT REQUIRED?", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    loclif.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(loclif);

                    PdfPTable arealif = new PdfPTable(2);
                    arealif.TotalWidth = 555f;
                    arealif.LockedWidth = true;
                    arealif.SetWidths(new float[] { 2f, 38f });
                    arealif.AddCell(PhraseCell(new Phrase("7", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    arealif.AddCell(PhraseCell(new Phrase("Following points should be checked & verified in Field before issuing the Work Permit as per the nature of job.", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    pdfDoc.Add(arealif);

                    PdfPTable genlif = new PdfPTable(8);

                    genlif.TotalWidth = 555f;
                    genlif.LockedWidth = true;
                    genlif.SetWidths(new float[] { 2f, 2.5f, 3.5f, 12f, 2f, 2.5f, 3.5f, 12f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    genlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Done", font8)));
                    genlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Not Required", font8)));
                    genlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Check List", font8)));
                    genlif.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    genlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Done", font8)));
                    genlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Not Required", font8)));
                    genlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Check List", font8)));
                    genlif.AddCell(Wpcell);


                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[0].Rows.Count; rows++)
                        {

                            genlif.AddCell(PhraseCell(new Phrase(dataSet.Tables[0].Rows[rows][0].ToString(), font9), PdfPCell.ALIGN_CENTER));
                            genlif.AddCell("");
                            genlif.AddCell("");
                            Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[0].Rows[rows][1].ToString(), font9)));
                            genlif.AddCell(Wpcell);

                        }
                    }
                    genex.AddCell("");
                    genex.AddCell("");
                    genex.AddCell("");
                    genex.AddCell("");
                    pdfDoc.Add(genex);

                    PdfPTable actuallif = new PdfPTable(8);

                    actuallif.TotalWidth = 555f;
                    actuallif.LockedWidth = true;
                    actuallif.SetWidths(new float[] { 2f, 2.5f, 3.5f, 12f, 2f, 2.5f, 3.5f, 12f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));

                    actuallif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actuallif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actuallif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Work Specific Check List", font8)));
                    actuallif.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actuallif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actuallif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actuallif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Work Specific Check List", font8)));
                    actuallif.AddCell(Wpcell);


                    if (dataSet.Tables[6].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[6].Rows.Count; rows++)
                        {


                            actuallif.AddCell(PhraseCell(new Phrase(dataSet.Tables[6].Rows[rows][0].ToString(), font9), PdfPCell.ALIGN_CENTER));
                            actuallif.AddCell("");
                            actuallif.AddCell("");
                            Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[6].Rows[rows][1].ToString(), font9)));
                            actuallif.AddCell(Wpcell);

                        }
                    }

                    pdfDoc.Add(actuallif);



                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable ppelif = new PdfPTable(2);
                    ppelif.TotalWidth = 555f;
                    ppelif.LockedWidth = true;
                    ppelif.SetWidths(new float[] { 2f, 38f });
                    ppelif.AddCell(PhraseCell(new Phrase("8", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    ppelif.AddCell(PhraseCell(new Phrase("SPECIAL PROTECTION / PPE", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(ppelif);


                    PdfPTable ppelif1 = new PdfPTable(8);
                    ppelif1.TotalWidth = 555f;
                    ppelif1.LockedWidth = true;
                    ppelif1.SetWidths(new float[] { 2f, 8f, 2f, 8f, 2f, 8f, 2f, 8f });

                    if (dataSet.Tables[1].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[1].Rows.Count; rows++)
                        {

                            ppelif1.AddCell(uncheckbox);

                            Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[1].Rows[rows][1].ToString(), font9)));
                            ppelif1.AddCell(Wpcell);

                        }
                    }

                    else
                    {
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Applicable", font9)));
                        Wpcell.Colspan = 8;
                        ppelif1.AddCell(Wpcell);

                    }
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    ppelif1.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));

                    ppelif1.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));

                    ppelif1.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));

                    ppelif1.AddCell(Wpcell);


                    Wpcell = new PdfPCell(new Phrase(new Chunk("")));
                    Wpcell.Colspan = 8;
                    ppelif1.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("8.1 Others / L.C. No :", font8)));
                    Wpcell.Colspan = 8;
                    ppelif1.AddCell(Wpcell);

                    pdfDoc.Add(ppelif1);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));


                    PdfPTable permithandlif = new PdfPTable(6);

                    permithandlif.TotalWidth = 555f;
                    permithandlif.LockedWidth = true;
                    permithandlif.SetWidths(new float[] { 2f, 10f, 8f, 9.2f, 5f, 5f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk("9", font8)));

                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("PERMIT APPROVALS", font8)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name", font8)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Remarks ", font8)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Signature", font8)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Date Time", font8)));
                    permithandlif.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("I have checked the conditions in the field and the status is found as mentioned above. The job can be permitted.", font9)));

                    Wpcell.Colspan = 5;
                    permithandlif.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Permit Issuer", font8)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandlif.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Permit Approver(Shift In Charge)", font8)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandlif.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Stand by Person", font8)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandlif.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Safety officer", font8)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandlif.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Process Manager", font8)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandlif.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Electrical In charge", font8)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandlif.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Mechanical In charge  ", font8)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandlif.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Instrumental In charge", font8)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandlif.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Operations Head ", font8)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandlif.AddCell(Wpcell);


                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("I understood the above condition,arranged safety requirements, explained to the people concerned and received the permit for safe job execution.", font9)));
                    Wpcell.Colspan = 5;

                    permithandlif.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("")));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name / Number of person(s) working at site", font8)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("")));
                    Wpcell.Colspan = 4;
                    permithandlif.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Permit Receiver(Engineer /\nTech.of recipient dept).", font8)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandlif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandlif.AddCell(Wpcell);

                    pdfDoc.Add(permithandlif);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));


                    PdfPTable permitclosurelif = new PdfPTable(6);

                    permitclosurelif.TotalWidth = 555f;
                    permitclosurelif.LockedWidth = true;
                    permitclosurelif.SetWidths(new float[] { 2f, 10f, 8f, 9.2f, 5f, 5f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk("10", font8)));

                    permitclosurelif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("PERMIT CLOSURE", font8)));
                    permitclosurelif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name", font8)));
                    permitclosurelif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Remarks ", font8)));
                    permitclosurelif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Signature", font8)));
                    permitclosurelif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Date Time", font8)));
                    permitclosurelif.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permitclosurelif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Certified that the subject work has been completed/ stopped and area cleaned. ", font9)));
                    Wpcell.Colspan = 5;

                    permitclosurelif.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permitclosurelif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Job completed & Hand over by Receiver", font8)));
                    permitclosurelif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurelif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permitclosurelif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurelif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurelif.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));

                    permitclosurelif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Job taken over & Permit closed by issuer", font8)));
                    permitclosurelif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurelif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permitclosurelif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurelif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurelif.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permitclosurelif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Shift InCharge", font8)));
                    permitclosurelif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurelif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permitclosurelif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurelif.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurelif.AddCell(Wpcell);


                    pdfDoc.Add(permitclosurelif);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable conrlif = new PdfPTable(6);
                    PdfPCell plif = new PdfPCell();
                    conrlif.TotalWidth = 555f;
                    conrlif.LockedWidth = true;
                    conrlif.SetWidths(new float[] { 1f, 3f, 1f, 3f, 1f, 22f });
                    cell = PhraseCell(new Phrase("11".PadRight(6) + "CONTRACTOR RATING  ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT);
                    cell.Colspan = 6;
                    conrlif.AddCell(cell);
                    conrlif.AddCell(uncheckbox);

                    plif = new PdfPCell(new Phrase(new Chunk("Red", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD))));
                    conrlif.AddCell(plif);

                    conrlif.AddCell(uncheckbox);

                    plif = new PdfPCell(new Phrase(new Chunk("Yellow", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD))));
                    conrlif.AddCell(plif);

                    conrlif.AddCell(uncheckbox);

                    plif = new PdfPCell(new Phrase(new Chunk("Green", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD))));
                    conrlif.AddCell(plif);

                    plif = new PdfPCell(new Phrase(new Chunk(" Remarks (if Red and Yellow rating) :                                                                  \n\n", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL))));
                    pex.Colspan = 8;
                    conrlif.AddCell(plif);

                    pdfDoc.Add(conrlif);
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));


                    PdfPTable geninli = new PdfPTable(2);
                    geninli.TotalWidth = 555f;
                    geninli.LockedWidth = true;
                    geninli.SetWidths(new float[] { 2f, 38f });
                    geninli.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    geninli.AddCell(PhraseCell(new Phrase("GENERAL INSTRUCTIONS : ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1.", font9)));
                    geninli.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("In case of an Emergency or FIRE ALARM, all works must be stopped and permit stands cancelled. All personnel must leave work site and proceed to emergency Assembly points soon.", font9)));
                    geninli.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2.", font9)));
                    geninli.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("In case of any  LPG/ Gas /Liquid release (or) abnormality noticed, stop work immediately and move to safe location till further instruction,", font9)));
                    geninli.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("3.", font9)));
                    geninli.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("This permit must be available at work site all the time. On job closure (or) expiry, this permit must be returned to the permit issuer/Shift Incharge. ", font9)));
                    geninli.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("4.", font9)));
                    geninli.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Do not commence the work without a valid work permit.", font9)));
                    geninli.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("5.", font9)));
                    geninli.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Acquaint yourself with job and its hazards and ensure the tools used are safe.", font9)));
                    geninli.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("6.", font9)));
                    geninli.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Inform regarding unsafe condition/unsafe act to operator/Safety incharge/Shift incharge", font9)));
                    geninli.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("7.", font9)));
                    geninli.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("When in doubt, consult SASA employee of concerned department (or) SASA Operator ", font9)));
                    geninli.AddCell(Wpcell);


                    Wpcell = new PdfPCell(new Phrase(new Chunk("8.", font9)));
                    geninli.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Work should be started within one hour after issue of this permit", font9)));
                    geninli.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("9.", font9)));
                    geninli.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("All safety precautions should be meticulously followed. Keep access clear for emergency.", font9)));
                    geninli.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("10.", font9)));
                    geninli.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Incase of any emergency, permit should be surrendered to the issuing authority", font9)));
                    geninli.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("11.", font9)));
                    geninli.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Permit is valid only for the mentioned time. After that, further extension is needed before proceeding the job", font9)));
                    geninli.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("12.", font9)));
                    geninli.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Extention: To be extended only, if area and nature of work is same", font9)));
                    geninli.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("13.", font9)));
                    geninli.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Extention: Daily extension should be authorized per the concerned persons, after reviewing the site again.", font9)));
                    geninli.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("14.", font9)));
                    geninli.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Extention: If the job is discontinued, make a fresh permit", font9)));
                    geninli.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("15.", font9)));
                    geninli.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Extension: Extention can be done maximum one week duration. Make new permit every Monday.", font9)));
                    geninli.AddCell(Wpcell);

                    pdfDoc.Add(geninli);

                    PdfPTable Lifting = new PdfPTable(2);
                    Lifting.TotalWidth = 555f;
                    Lifting.LockedWidth = true;
                    Lifting.SetWidths(new float[] { 2f, 38f });


                    Lifting.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Lifting.AddCell(PhraseCell(new Phrase("PERMIT SPECIFIC INSTRUCTIONS - Lifting", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Lifting.AddCell(PhraseCell(new Phrase("1.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    Lifting.AddCell(PhraseCell(new Phrase("Ensure spot communication to crane operator, signalman during job execution", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    Lifting.AddCell(PhraseCell(new Phrase("2.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    Lifting.AddCell(PhraseCell(new Phrase("Job allowed when wind velocity is low and only upto 18:00 hours", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    Lifting.AddCell(PhraseCell(new Phrase("3.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    Lifting.AddCell(PhraseCell(new Phrase("Check the rope of the winch daily. Use proper wire ropes", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));


                    Lifting.AddCell(PhraseCell(new Phrase("4.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    Lifting.AddCell(PhraseCell(new Phrase("Lifting work permit can not be extended. Make a fresh permit after time elapsed", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(Lifting);
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(6000)));

                    PdfPTable TitleTablewk = new PdfPTable(4);
                    TitleTablewk.LockedWidth = true;
                    TitleTablewk.SetWidths(new float[] { 10f, 18f, 6.8f, 5.2f });
                    TitleTablewk.SpacingBefore = 10f;
                    TitleTablewk.SpacingAfter = 1f;
                    TitleTablewk.TotalWidth = 555f;


                    Wpcell = new PdfPCell(gif);

                    TitleTablewk.AddCell(Wpcell);

                    var phrasewk = new Phrase(new Chunk("\n SASA PTA PROJECT ", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    phrasewk.Add(new Chunk("ADANA,TURKEY", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    phrasewk.Add(new Chunk("\n\n", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    TitleTablewk.AddCell(PhraseCell(phrasewk, PdfPCell.ALIGN_CENTER));

                    Wpcell = new PdfPCell(new Phrase("\n WORK PERMIT NO:", FontFactory.GetFont("Times New Roman", 12, iTextSharp.text.Font.BOLD)));
                    Wpcell.HorizontalAlignment = Element.ALIGN_CENTER;

                    TitleTablewk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase("\n ", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    Wpcell.HorizontalAlignment = Element.ALIGN_LEFT;

                    TitleTablewk.AddCell(Wpcell);
                    pdfDoc.Add(TitleTablewk);

                    PdfPTable Typeworkwk = new PdfPTable(1);

                    Typeworkwk.TotalWidth = 555f;
                    Typeworkwk.LockedWidth = true;
                    Typeworkwk.SetWidths(new float[] { 40f });

                    Typeworkwk.AddCell(PhraseCell(new Phrase("WORK AT HEIGHT PERMIT", FontFactory.GetFont("Times New Roman", 14, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_CENTER));

                    pdfDoc.Add(Typeworkwk);

                    PdfPTable permitdetailswk = new PdfPTable(5);
                    permitdetailswk.TotalWidth = 555f;
                    permitdetailswk.LockedWidth = true;
                    permitdetailswk.SetWidths(new float[] { 2f, 8f, 9f, 9f, 12f });
                    permitdetailswk.AddCell(PhraseCell(new Phrase("1", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetailswk.AddCell(PhraseCell(new Phrase("VALIDITY FROM ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetailswk.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    permitdetailswk.AddCell(PhraseCell(new Phrase("VALIDITY TO ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetailswk.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    permitdetailswk.AddCell(PhraseCell(new Phrase("2", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetailswk.AddCell(PhraseCell(new Phrase("PERMIT ISSUER: ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetailswk.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    permitdetailswk.AddCell(PhraseCell(new Phrase("PERMIT RECEIVER: ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    permitdetailswk.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(permitdetailswk);
                    PdfPTable plant6 = new PdfPTable(5);
                    plant6.TotalWidth = 555f;
                    plant6.LockedWidth = true;
                    plant6.SetWidths(new float[] { 2f, 8f, 9f, 9f, 12f });
                    plant6.AddCell(PhraseCell(new Phrase("3", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    plant6.AddCell(PhraseCell(new Phrase("PLANT/AREA", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    plant6.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    plant6.AddCell(PhraseCell(new Phrase("JOB ID", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    plant6.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(plant6);
                    PdfPTable Equtable6 = new PdfPTable(5);
                    Equtable6.TotalWidth = 555f;
                    Equtable6.LockedWidth = true;
                    Equtable6.SetWidths(new float[] { 2f, 8f, 9f, 9f, 12f });

                    Equtable6.AddCell(PhraseCell(new Phrase("4", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Equtable6.AddCell(PhraseCell(new Phrase("LOCATION OF THE WORK ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Equtable6.AddCell(PhraseCell(new Phrase("" , FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    Equtable6.AddCell(PhraseCell(new Phrase("EQUIPMENT NAME", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Equtable6.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(Equtable6);

                    PdfPTable descewk = new PdfPTable(3);
                    descewk.TotalWidth = 555f;
                    descewk.LockedWidth = true;
                    descewk.SetWidths(new float[] { 2f, 8f, 30f });
                    descewk.AddCell(PhraseCell(new Phrase("5", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    descewk.AddCell(PhraseCell(new Phrase("DESCRIPTION OF WORK  ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    descewk.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(descewk);


                    PdfPTable locwk = new PdfPTable(5);
                    locwk.TotalWidth = 555f;
                    locwk.LockedWidth = true;
                    locwk.SetWidths(new float[] { 2f, 8f, 9f, 9f, 12f });

                    locwk.AddCell(PhraseCell(new Phrase("6", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    locwk.AddCell(PhraseCell(new Phrase("LOCATION OF THE WORK ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    locwk.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    locwk.AddCell(PhraseCell(new Phrase("EQUIPMENT NAME", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    locwk.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));


                    locwk.AddCell(PhraseCell(new Phrase("7", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    locwk.AddCell(PhraseCell(new Phrase("WORK DONE BY ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    locwk.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    locwk.AddCell(PhraseCell(new Phrase("DEPARTMENT", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    locwk.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));


                    locwk.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    locwk.AddCell(PhraseCell(new Phrase("NAME OF CONTRACTOR", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    locwk.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    locwk.AddCell(PhraseCell(new Phrase("RISK ASSESSMENT REQUIRED?", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    locwk.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(locwk);

                    PdfPTable areawk = new PdfPTable(2);
                    areawk.TotalWidth = 555f;
                    areawk.LockedWidth = true;
                    areawk.SetWidths(new float[] { 2f, 38f });
                    areawk.AddCell(PhraseCell(new Phrase("8", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    areawk.AddCell(PhraseCell(new Phrase("Following points should be checked & verified in Field before issuing the Work Permit as per the nature of job.", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    pdfDoc.Add(areawk);

                    PdfPTable genwk = new PdfPTable(8);

                    genwk.TotalWidth = 555f;
                    genwk.LockedWidth = true;
                    genwk.SetWidths(new float[] { 2f, 2.5f, 3.5f, 12f, 2f, 2.5f, 3.5f, 12f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    genwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Done", font8)));
                    genwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Not Required", font8)));
                    genwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Check List", font8)));
                    genwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    genwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Done", font8)));
                    genwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Not Required", font8)));
                    genwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Check List", font8)));
                    genwk.AddCell(Wpcell);


                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[0].Rows.Count; rows++)
                        {

                            genwk.AddCell(PhraseCell(new Phrase(dataSet.Tables[0].Rows[rows][0].ToString(), font9), PdfPCell.ALIGN_CENTER));
                            genwk.AddCell("");
                            genwk.AddCell("");
                            Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[0].Rows[rows][1].ToString(), font9)));
                            genwk.AddCell(Wpcell);

                        }
                    }
                    genwk.AddCell("");
                    genwk.AddCell("");
                    genwk.AddCell("");
                    genwk.AddCell("");
                    pdfDoc.Add(genwk);

                    PdfPTable actualchwk = new PdfPTable(8);

                    actualchwk.TotalWidth = 555f;
                    actualchwk.LockedWidth = true;
                    actualchwk.SetWidths(new float[] { 2f, 2.5f, 3.5f, 12f, 2f, 2.5f, 3.5f, 12f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));

                    actualchwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actualchwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actualchwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Work Specific Check List", font8)));
                    actualchwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actualchwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actualchwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    actualchwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Work Specific Check List", font8)));
                    actualchwk.AddCell(Wpcell);


                    if (dataSet.Tables[6].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[6].Rows.Count; rows++)
                        {


                            actualchwk.AddCell(PhraseCell(new Phrase(dataSet.Tables[6].Rows[rows][0].ToString(), font9), PdfPCell.ALIGN_CENTER));
                            actualchwk.AddCell("");
                            actualchwk.AddCell("");
                            Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[6].Rows[rows][1].ToString(), font9)));
                            actualchwk.AddCell(Wpcell);

                        }
                    }

                    pdfDoc.Add(actualchwk);



                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable ppewk = new PdfPTable(2);
                    ppewk.TotalWidth = 555f;
                    ppewk.LockedWidth = true;
                    ppewk.SetWidths(new float[] { 2f, 38f });
                    ppewk.AddCell(PhraseCell(new Phrase("9", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    ppewk.AddCell(PhraseCell(new Phrase("SPECIAL PROTECTION / PPE", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(ppewk);


                    PdfPTable ppewk1 = new PdfPTable(8);
                    ppewk1.TotalWidth = 555f;
                    ppewk1.LockedWidth = true;
                    ppewk1.SetWidths(new float[] { 2f, 8f, 2f, 8f, 2f, 8f, 2f, 8f });

                    if (dataSet.Tables[1].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[1].Rows.Count; rows++)
                        {

                            ppewk1.AddCell(uncheckbox);

                            Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[1].Rows[rows][1].ToString(), font9)));
                            ppewk1.AddCell(Wpcell);

                        }
                    }

                    else
                    {
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Applicable", font9)));
                        Wpcell.Colspan = 8;
                        ppewk1.AddCell(Wpcell);

                    }
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    ppewk1.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));

                    ppewk1.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));

                    ppewk1.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));

                    ppewk1.AddCell(Wpcell);


                    Wpcell = new PdfPCell(new Phrase(new Chunk("")));
                    Wpcell.Colspan = 8;
                    ppewk1.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("9.1 Others / L.C. No :", font8)));
                    Wpcell.Colspan = 8;
                    ppewk1.AddCell(Wpcell);

                    pdfDoc.Add(ppewk1);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));


                    PdfPTable permithandwk = new PdfPTable(6);

                    permithandwk.TotalWidth = 555f;
                    permithandwk.LockedWidth = true;
                    permithandwk.SetWidths(new float[] { 2f, 10f, 8f, 9.2f, 5f, 5f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk("10", font8)));

                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("PERMIT APPROVALS", font8)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name", font8)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Remarks ", font8)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Signature", font8)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Date Time", font8)));
                    permithandwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("I have checked the conditions in the field and the status is found as mentioned above. The job can be permitted.", font9)));

                    Wpcell.Colspan = 5;
                    permithandwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Permit Issuer", font8)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Permit Approver(Shift In Charge)", font8)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Stand by Person", font8)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Safety officer", font8)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Process Manager", font8)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Electrical In charge", font8)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Mechanical In charge ", font8)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Instrumental In charge", font8)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Operations Head ", font8)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandwk.AddCell(Wpcell);


                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("I understood the above condition,arranged safety requirements, explained to the people concerned and received the permit for safe job execution.", font9)));
                    Wpcell.Colspan = 5;

                    permithandwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("")));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name / Number of person(s) working at site", font8)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("")));
                    Wpcell.Colspan = 4;
                    permithandwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Permit Receiver(Engineer /\nTech.of recipient dept).", font8)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permithandwk.AddCell(Wpcell);

                    pdfDoc.Add(permithandwk);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));


                    PdfPTable permitclosurewk = new PdfPTable(6);

                    permitclosurewk.TotalWidth = 555f;
                    permitclosurewk.LockedWidth = true;
                    permitclosurewk.SetWidths(new float[] { 2f, 10f, 8f, 9.2f, 5f, 5f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk("11", font8)));

                    permitclosurewk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("PERMIT CLOSURE", font8)));
                    permitclosurewk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name", font8)));
                    permitclosurewk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Remarks ", font8)));
                    permitclosurewk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Signature", font8)));
                    permitclosurewk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Date Time", font8)));
                    permitclosurewk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    permitclosurewk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Certified that the subject work has been completed/ stopped and area cleaned. ", font9)));
                    Wpcell.Colspan = 5;

                    permitclosurewk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permitclosurewk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Job completed & Hand over by Receiver", font8)));
                    permitclosurewk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurewk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permitclosurewk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurewk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurewk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));

                    permitclosurewk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Job taken over & Permit closed by issuer", font8)));
                    permitclosurewk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurewk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permitclosurewk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurewk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurewk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.FixedHeight = 25f;
                    permitclosurewk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Shift InCharge", font8)));
                    permitclosurewk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurewk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font9)));
                    permitclosurewk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurewk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    permitclosurewk.AddCell(Wpcell);


                    pdfDoc.Add(permitclosurewk);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable conrwk = new PdfPTable(6);
                    PdfPCell pwk = new PdfPCell();
                    conrwk.TotalWidth = 555f;
                    conrwk.LockedWidth = true;
                    conrwk.SetWidths(new float[] { 1f, 3f, 1f, 3f, 1f, 22f });
                    cell = PhraseCell(new Phrase("12".PadRight(6) + "CONTRACTOR RATING  ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT);
                    cell.Colspan = 6;
                    conrwk.AddCell(cell);
                    conrwk.AddCell(uncheckbox);

                    pwk = new PdfPCell(new Phrase(new Chunk("Red", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD))));
                    conrwk.AddCell(pwk);

                    conrwk.AddCell(uncheckbox);

                    pwk = new PdfPCell(new Phrase(new Chunk("Yellow", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD))));
                    conrwk.AddCell(pwk);

                    conrwk.AddCell(uncheckbox);

                    pwk = new PdfPCell(new Phrase(new Chunk("Green", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD))));
                    conrwk.AddCell(pwk);

                    pwk = new PdfPCell(new Phrase(new Chunk(" Remarks (if Red and Yellow rating) :                                                                  \n\n", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.NORMAL))));
                    pwk.Colspan = 8;
                    conrwk.AddCell(pwk);

                    pdfDoc.Add(conrwk);
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));



                    PdfPTable renwk = new PdfPTable(2);
                    renwk.TotalWidth = 555f;
                    renwk.LockedWidth = true;
                    renwk.SetWidths(new float[] { 2f, 38f });
                    renwk.AddCell(PhraseCell(new Phrase("13", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    renwk.AddCell(PhraseCell(new Phrase("RENEWAL OF WORK PERMIT", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(renwk);


                    PdfPTable renewalwk = new PdfPTable(9);

                    renewalwk.TotalWidth = 555f;
                    renewalwk.LockedWidth = true;
                    renewalwk.SetWidths(new float[] { 7f, 5f, 4f, 4f, 4f, 4f, 4f, 4f, 4f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Day/ Date/Time", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. LEL Level (ZERO)", font8)));

                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of Permit issuer ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of shift In charge ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of Stand by", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of safety In charge", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of Manager Process", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of Operations Head", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name & sign of  Receiver", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. % O2(19.5 - 22.5 %)", font8)));

                    renewalwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("First Extension Date:................................", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell.Rowspan = 2;
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                    renewalwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Second Extension Date:................................", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell.Rowspan = 2;
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                    renewalwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Third Extension Date:................................", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell.Rowspan = 2;
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                    renewalwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Fourth Extension Date:................................", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell.Rowspan = 2;
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                    renewalwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Fifth Extension Date:................................", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell.Rowspan = 2;
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                    renewalwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Sixth Extension Date:................................", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1. ", font8)));
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);
                    Wpcell.Rowspan = 2;
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Wpcell.Rowspan = 2;
                    renewalwk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2. ", font8)));
                    renewalwk.AddCell(Wpcell);


                    pdfDoc.Add(renewalwk);


                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable geninswk = new PdfPTable(2);
                    geninswk.TotalWidth = 555f;
                    geninswk.LockedWidth = true;
                    geninswk.SetWidths(new float[] { 2f, 38f });
                    geninswk.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    geninswk.AddCell(PhraseCell(new Phrase("GENERAL INSTRUCTIONS : ", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1.", font9)));
                    geninswk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("In case of an Emergency or FIRE ALARM, all works must be stopped and permit stands cancelled. All personnel must leave work site and proceed to emergency Assembly points soon.", font9)));
                    geninswk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2.", font9)));
                    geninswk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("In case of any  LPG/ Gas /Liquid release (or) abnormality noticed, stop work immediately and move to safe location till further instruction,", font9)));
                    geninswk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("3.", font9)));
                    geninswk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("This permit must be available at work site all the time. On job closure (or) expiry, this permit must be returned to the permit issuer/Shift Incharge. ", font9)));
                    geninswk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("4.", font9)));
                    geninswk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Do not commence the work without a valid work permit.", font9)));
                    geninswk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("5.", font9)));
                    geninswk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Acquaint yourself with job and its hazards and ensure the tools used are safe.", font9)));
                    geninswk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("6.", font9)));
                    geninswk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Inform regarding unsafe condition/unsafe act to operator/Safety incharge/Shift incharge", font9)));
                    geninswk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("7.", font9)));
                    geninswk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("When in doubt, consult SASA employee of concerned department (or) SASA Operator ", font9)));
                    geninswk.AddCell(Wpcell);


                    Wpcell = new PdfPCell(new Phrase(new Chunk("8.", font9)));
                    geninswk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Work should be started within one hour after issue of this permit", font9)));
                    geninswk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("9.", font9)));
                    geninswk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("All safety precautions should be meticulously followed. Keep access clear for emergency.", font9)));
                    geninswk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("10.", font9)));
                    geninswk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Incase of any emergency, permit should be surrendered to the issuing authority", font9)));
                    geninswk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("11.", font9)));
                    geninswk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Permit is valid only for the mentioned time. After that, further extension is needed before proceeding the job", font9)));
                    geninswk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("12.", font9)));
                    geninswk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Extention: To be extended only, if area and nature of work is same", font9)));
                    geninswk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("13.", font9)));
                    geninswk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Extention: Daily extension should be authorized per the concerned persons, after reviewing the site again.", font9)));
                    geninswk.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("14.", font9)));
                    geninswk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Extention: If the job is discontinued, make a fresh permit", font9)));
                    geninswk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("15.", font9)));
                    geninswk.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Extension: Extention can be done maximum one week duration. Make new permit every Monday.", font9)));
                    geninswk.AddCell(Wpcell);

                    pdfDoc.Add(geninswk);

                    PdfPTable workh = new PdfPTable(2);
                    workh.TotalWidth = 555f;
                    workh.LockedWidth = true;
                    workh.SetWidths(new float[] { 2f, 38f });
                    workh.AddCell(PhraseCell(new Phrase("", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    workh.AddCell(PhraseCell(new Phrase("PERMIT SPECIFIC INSTRUCTIONS - WORK AT HEIGHT: ", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                    workh.AddCell(PhraseCell(new Phrase("1.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    workh.AddCell(PhraseCell(new Phrase("Ensure Emergency communication & actions, in case of emergency. Maintain the list of personnel working at height and safety register.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    workh.AddCell(PhraseCell(new Phrase("2.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    workh.AddCell(PhraseCell(new Phrase(" Check scaffold stability. Do not overload the scaffolding with heavy material ", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    workh.AddCell(PhraseCell(new Phrase("3.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    workh.AddCell(PhraseCell(new Phrase("Do not use cracked material for scaffolds.  No portion of scaffold to be removed or modified without approval.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    workh.AddCell(PhraseCell(new Phrase("4.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    workh.AddCell(PhraseCell(new Phrase(" Wear safety belt postively and PPEs.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    workh.AddCell(PhraseCell(new Phrase("5.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    workh.AddCell(PhraseCell(new Phrase("No loose materials is to be kept on Jula top. They must be secured inside a box to aviod falling", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    workh.AddCell(PhraseCell(new Phrase("6.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    workh.AddCell(PhraseCell(new Phrase("Ensure spot communication to Crane operator, signalman (if applicable) during job execution", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    workh.AddCell(PhraseCell(new Phrase("7.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    workh.AddCell(PhraseCell(new Phrase("job allowed only when wind velocity is low.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    workh.AddCell(PhraseCell(new Phrase("8.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    workh.AddCell(PhraseCell(new Phrase("Personnel working at height must be experienced, medical fitness certificates to be obtained as required.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    workh.AddCell(PhraseCell(new Phrase("9.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    workh.AddCell(PhraseCell(new Phrase("Lifting equipment checked & inspected thoroughly for mechanical function & physical condition.", FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(workh);


                    pdfDoc.Close();
                    Response.Write(pdfDoc);
                    Response.End();


                }
            }

            catch (Exception objException)
            {
                // LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);
            }

            return View();
        }

    }
}
