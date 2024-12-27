using MonitPro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MonitPro.BLL;
using MonitPro.Validations;
using System.Data.SqlClient;
using System.Data;
using System.Xml.Serialization;
using System.IO;
using MonitPro.Common.Library;

namespace WorkPermitSystem.Controllers
{
    public class AssignWorkTypeController : Controller
    {
        // GET: AssignWorkType
        //public ActionResult Index()
        //{
        //    return View();
        //}
#region AssignTypeofWork

        public ActionResult AssignTypeofWork()
        {

            AssignEquipments assignEquipments = new AssignEquipments();

            try
            {
                //AdminBLL adminBLL = new AdminBLL();
                assignEquipments = AssignEquipment(0);

            }
            catch (Exception objException)
            {
                ModelState.AddModelError("Error", objException.Message);
            }
            if (CurrentUser.UserID != 0)
            {

                assignEquipments.UserID = CurrentUser.UserID;
                assignEquipments.UserFullName = CurrentUser.FullName;
                assignEquipments.Roles = CurrentUser.Roles;
                assignEquipments.ProfileImage = CurrentUser.ProfileImage;
            }
            return View(assignEquipments);

        }
#endregion


        #region AssignTypeofWork
        [HttpPost]
        //[ValidateAntiForgeryToken]
        //[AuthorizedUsers("Supervisor")]
        public ActionResult AssignTypeofWork(AssignEquipments assignEquipments)
        {
            try
            {
                AdminBLL adminBLL = new AdminBLL();

                if (Request.Form["GetEquipmentList"] == "1")
                {
                    int userID = assignEquipments.SelectedUserID;
                    assignEquipments = AssignEquipment(userID);
                    assignEquipments.SelectedUserID = userID;
                }

                if (Request.Form["SaveUserEquipments"] == "1")
                {
                    List<int> equipmentList = assignEquipments.Equipments.Where(x => x.Assigned == true).Select(y => y.EquipmentID).ToList();

                    MeasureDataUsersEntity measureDataUsersEntity = new MeasureDataUsersEntity();
                    measureDataUsersEntity.AssignTo = assignEquipments.SelectedUserID;
                    measureDataUsersEntity.AssignedBy = CurrentUser.UserID;
                    measureDataUsersEntity.EquipmentList = equipmentList;

                    //foreach (var item in assignEquipments.Equipments)
                    //{
                    //    if (item.Assigned ==true)
                    //    {
                    //        equipmentList.Add(item.EquipmentID);
                    //        listEquipments.Add( 
                    //            new MeasureDataUsersEntity
                    //            {
                    //                assi =assignEquipments.SelectedUserID,
                    //                EquipmentID =item.EquipmentID,
                    //                AssignedBy =CurrentUser.UserID 
                    //             });
                    //        //int recordCount = adminBLL.MeasureDataUsersInsert(listEquipments);
                    //        //ViewBag.IsInsertSuccessful = true;
                    //     }
                    //}

                    int recordCount = MeasureDataUsersInsert(measureDataUsersEntity);
                    ViewBag.IsInsertSuccessful = true;

                    int userID = assignEquipments.SelectedUserID;
                    assignEquipments = AssignEquipment(userID);
                    assignEquipments.SelectedUserID = userID;
                }
            }
            catch (Exception objException)
            {
                ModelState.AddModelError("ServerSideError", objException.Message);
                ViewBag.IsServerSideError = true;
            }

            ModelState.Clear();
            assignEquipments.UserID = CurrentUser.UserID;
            assignEquipments.Roles = CurrentUser.Roles;
            assignEquipments.UserFullName = CurrentUser.FullName;
            assignEquipments.ProfileImage = CurrentUser.ProfileImage;
            return View(assignEquipments);
        }





        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet ds = new DataSet();


        #endregion



        //#region AssignWorkTypes

        //[HttpGet]
        ////[AuthorizedUsers("Administrator")]
        //public ActionResult AssignEquipment()
        //{

        //    AssignEquipments assignEquipments = new AssignEquipments();

        //    try
        //    {
        //        //AdminBLL adminBLL = new AdminBLL();
        //        assignEquipments = AssignEquipment(0);

        //    }
        //    catch (Exception objException)
        //    {
        //        ModelState.AddModelError("Error", objException.Message);
        //    }
        //    if (CurrentUser.UserID != 0)
        //    {

        //        assignEquipments.UserID = CurrentUser.UserID;
        //        assignEquipments.UserFullName = CurrentUser.FullName;
        //        assignEquipments.Roles = CurrentUser.Roles;
        //        assignEquipments.ProfileImage = CurrentUser.ProfileImage;
        //    }
        //    return View(assignEquipments);

        //}

        //#endregion



        //#region AssignWorkTypes
        //[HttpPost]
        ////[ValidateAntiForgeryToken]
        ////[AuthorizedUsers("Supervisor")]
        //public ActionResult AssignEquipment(AssignEquipments assignEquipments)
        //{
        //    try
        //    {
        //        AdminBLL adminBLL = new AdminBLL();

        //        if (Request.Form["GetEquipmentList"] == "1")
        //        {
        //            int userID = assignEquipments.SelectedUserID;
        //            assignEquipments = AssignEquipment(userID);
        //            assignEquipments.SelectedUserID = userID;
        //        }

        //        if (Request.Form["SaveUserEquipments"] == "1")
        //        {
        //            List<int> equipmentList = assignEquipments.Equipments.Where(x => x.Assigned == true).Select(y => y.EquipmentID).ToList();

        //            MeasureDataUsersEntity measureDataUsersEntity = new MeasureDataUsersEntity();
        //            measureDataUsersEntity.AssignTo = assignEquipments.SelectedUserID;
        //            measureDataUsersEntity.AssignedBy = CurrentUser.UserID;
        //            measureDataUsersEntity.EquipmentList = equipmentList;

        //            //foreach (var item in assignEquipments.Equipments)
        //            //{
        //            //    if (item.Assigned ==true)
        //            //    {
        //            //        equipmentList.Add(item.EquipmentID);
        //            //        listEquipments.Add( 
        //            //            new MeasureDataUsersEntity
        //            //            {
        //            //                assi =assignEquipments.SelectedUserID,
        //            //                EquipmentID =item.EquipmentID,
        //            //                AssignedBy =CurrentUser.UserID 
        //            //             });
        //            //        //int recordCount = adminBLL.MeasureDataUsersInsert(listEquipments);
        //            //        //ViewBag.IsInsertSuccessful = true;
        //            //     }
        //            //}

        //            int recordCount = MeasureDataUsersInsert(measureDataUsersEntity);
        //            ViewBag.IsInsertSuccessful = true;

        //            int userID = assignEquipments.SelectedUserID;
        //            assignEquipments = AssignEquipment(userID);
        //            assignEquipments.SelectedUserID = userID;
        //        }
        //    }
        //    catch (Exception objException)
        //    {
        //        ModelState.AddModelError("ServerSideError", objException.Message);
        //        ViewBag.IsServerSideError = true;
        //    }

        //    ModelState.Clear();
        //    assignEquipments.UserID = CurrentUser.UserID;
        //    assignEquipments.Roles = CurrentUser.Roles;
        //    assignEquipments.UserFullName = CurrentUser.FullName;
        //    assignEquipments.ProfileImage = CurrentUser.ProfileImage;
        //    return View(assignEquipments);
        //}





        ////SqlDataAdapter sda = new SqlDataAdapter();
        ////DataSet ds = new DataSet();


        //#endregion


        #region AssignWorkType

        public AssignEquipments AssignEquipment(int userID)
        {
            AssignEquipments assignEqipments = new AssignEquipments();
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "AssignApprover";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@UserID", userID);
                    objCom.Connection = objCon;

                    DataSet dsResult = new DataSet();
                    SqlDataAdapter objAdapter = new SqlDataAdapter();
                    objAdapter.SelectCommand = objCom;
                    objAdapter.Fill(dsResult);

                    assignEqipments.Users = new List<User>();
                    foreach (DataRow item in dsResult.Tables[0].Rows)
                    {

                        assignEqipments.Users.Add(
                            new User
                            {
                                UserID = int.Parse(item["UserID"].ToString()),
                                FullName = item["UserFullName"].ToString()
                            }
                            );
                    }

                    assignEqipments.Equipments = new List<Equipment>();
                    foreach (DataRow item in dsResult.Tables[1].Rows)
                    {

                        assignEqipments.Equipments.Add(
                            new Equipment
                            {
                                EquipmentID = int.Parse(item["WorkTypeID"].ToString()),
                                EquipmentName = item["WorkType"].ToString(),
                                Assigned = int.Parse(item["Assigned"].ToString()) > 0 ? true : false,
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

            return assignEqipments;
        }
        #endregion


        public int MeasureDataUsersInsert(MeasureDataUsersEntity measureDataUsersEntity)
        {
            
            string equipmentList = string.Empty;
            XmlSerializer xmlSerializer = new XmlSerializer(measureDataUsersEntity.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, measureDataUsersEntity);
                equipmentList = textWriter.ToString();
            }

            return MeasureDataUsersInsert(equipmentList);

        }

        #region MeasureDataUsersInsert
        public int MeasureDataUsersInsert(string equipmentList)
        {
            int affectedRecordCount = 0;

            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "MeasureDataUsersInsert";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@EquipmentList", equipmentList);
                    objCom.Connection = objCon;
                    objCon.Open();
                    affectedRecordCount = objCom.ExecuteNonQuery();
                    objCon.Close();
                }
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);
            }

            return affectedRecordCount;
        }

        #endregion


        //Permit reviewer Page

      

       


    }
}