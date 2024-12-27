using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using MonitPro.Common.Library;
using System.Xml.Serialization;
using System.IO;

using MonitPro.MobileApp.Model;

namespace MonitPro.DAL
{
    public class MobileAppDA
    {
        IncidentReportDAL InciDal = new IncidentReportDAL();
        #region ValidateUser
        public LoginResponse ValidateUser(LoginCredential loginCredential)
        {
            LogManager.Instance.Info("Incident - Login Validation - Start");
            LoginResponse LoginResponse = new LoginResponse();
            try
            {
                string strToken = null;
                object objUserID = null;
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[MobileLogin]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@UserName", loginCredential.UserName);
                    objCom.Parameters.AddWithValue("@Password", loginCredential.Password);
                    objCon.Open();
                    objCom.Connection = objCon;

                    objUserID = objCom.ExecuteScalar();

                    SqlDataReader reader = objCom.ExecuteReader();
                    strToken = Guid.NewGuid().ToString("N").ToUpper();

                    if (reader.Read())
                    {
                        LoginResponse = new LoginResponse
                        {
                            FirstName = reader["UserName"].ToString(),
                            UserID = int.Parse(reader["UserID"].ToString()),
                            ErrorMessage = "",
                            Token = strToken

                        };

                    }
                    objCon.Close();
                }
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();

                    objCom.CommandText = "[MobileTokenInsert]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@Token", strToken);
                    objCom.Parameters.AddWithValue("@UserID", objUserID);
                    objCon.Open();
                    objCom.Connection = objCon;

                    objCom.ExecuteNonQuery();

                    //LoginResponse.ErrorMessage = "";
                    //LoginResponse.Token = strToken;

                    objCon.Close();

                }

            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                LoginResponse = new LoginResponse
                {
                    Token = "",
                    ErrorMessage = objException.Message
                };
            }
            LogManager.Instance.Info("Incident - Login Validation - End");
            return LoginResponse;
        }

        #endregion
        public List<Plants> GetPlant()
        {

            try
            {
                using (SqlConnection objcon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objcom = new SqlCommand();
                    objcom.CommandText = "[GetPlants]";
                    objcom.CommandType = CommandType.StoredProcedure;
                    objcom.Connection = objcon;
                    objcon.Open();
                    var plant = new List<Plants>();
                    SqlDataReader reader = objcom.ExecuteReader();
                    if (reader.HasRows)
                    {
                        plant.Add(new Plants { ID = 0, PlantName = "--Select--" });

                    }
                    while (reader.Read())
                    {
                        plant.Add(new Plants
                        {
                            ID = reader["ID"].GetHashCode(),
                            PlantName = reader["Name"].ToString(),
                            Description = reader["Description"].ToString()

                        });

                    }

                    return plant;
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Equals(ex);
                throw new Exception(ex.Message);
            }

        }
        public List<CAPAPlants> GetcapaPlants()
        {
            List<CAPAPlants> obPlantsList = new List<CAPAPlants>();
            try
            {
                using (SqlConnection objcon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[GetCAPAPlants]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objcon;
                    objcon.Open();
                    SqlDataReader reader = objCom.ExecuteReader();

                    if (reader.HasRows)
                    {

                        obPlantsList.Add(new CAPAPlants { ID = 0, Name = "--Select--" });
                    }

                    while (reader.Read())
                    {
                        obPlantsList.Add(new CAPAPlants { ID = int.Parse(reader["ID"].ToString()), Name = reader["Name"].ToString(), Description = reader["Description"].ToString() });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return obPlantsList;
        }

        public List<IncidentClassfication> GetIncidentClassfication()
        {
            List<IncidentClassfication> IncidentClassficationList = new List<IncidentClassfication>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    var objCom = new SqlCommand();
                    objCom.CommandText = "[GetIncidentClassfication]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    var reader = objCom.ExecuteReader();

                    if (reader.HasRows)
                    {
                        IncidentClassficationList.Add(new IncidentClassfication { ID = 0, Name = "--Select--" });

                    }
                    while (reader.Read())
                    {
                        IncidentClassficationList.Add(new IncidentClassfication
                        {
                            ID = reader["ID"].GetHashCode(),
                            Name = reader["Name"].ToString(),
                            Description = reader["Description"].ToString()
                        });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return IncidentClassficationList;
        }
        public List<AuditType> GetAuditType()
        {
            List<AuditType> AuditTypeList = new List<AuditType>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    var objCom = new SqlCommand();
                    objCom.CommandText = "[GetAuditType]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    var reader = objCom.ExecuteReader();
                    if (reader.HasRows)
                    {
                        AuditTypeList.Add(new AuditType { AuditID = 0, AuditName = "--Select--" });

                    }
                    while (reader.Read())
                    {
                        AuditTypeList.Add(new AuditType
                        {
                            AuditID = int.Parse(reader["ID"].ToString()),
                            AuditName = reader["Name"].ToString(),
                            AuditDescription = reader["Description"].ToString()
                        });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return AuditTypeList;
        }

        public List<CAPASource> GetAuditCAPAsource(int? AuditID)
        {
            List<CAPASource> sourcelist = new List<CAPASource>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "GetCAPASourceSelect";

                    if (AuditID > 0)
                        objCom.Parameters.AddWithValue("@AuditID", AuditID);
                    else
                        objCom.Parameters.AddWithValue("@AuditID", DBNull.Value);

                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();

                    SqlDataReader Results = objCom.ExecuteReader();
                    if (Results.HasRows)
                    {
                        sourcelist.Add(new CAPASource { AuditCSID = "0", Name = "--Select--" });

                    }
                    while (Results.Read())
                    {
                        CAPASource cs = new CAPASource();
                        cs.AuditCSID = Results[0].ToString();
                        cs.Name = Results[1].ToString();
                        sourcelist.Add(cs);
                    }
                    objCon.Close();

                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return sourcelist;

        }

        public LoginResponse UpdateWorkPermitRating(UpdateWorkPermitRating rating)
        {
            LoginResponse loginResponse = null;
            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[UpdateWorkPermitRating]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@PermitID", rating.PermitID);
                    objCom.Parameters.AddWithValue("@ContractorComment", rating.ContractorComment);
                    objCom.Parameters.AddWithValue("@ContractorRating", rating.ContractorRating);
                    objCon.Open();
                    objCom.Connection = objCon;
                    affectedRecordCount = objCom.ExecuteNonQuery();

                    objCon.Close();
                    loginResponse = new LoginResponse
                    {
                        PermitID = rating.PermitID,
                        ErrorMessage = "Updated Successfully"
                    };
                }

            }
            catch (Exception ex)
            {
                LogManager.Instance.Equals(ex);
                throw new Exception(ex.Message);
            }
            return loginResponse;
        }



        public CAPAObservation GetAPAObservation(int CAPAID)
        {
            CAPAObservation co = new CAPAObservation();
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[GetMobileCAPADetails]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@capaID", CAPAID);

                    objCon.Open();
                    objCom.Connection = objCon;

                    SqlDataReader objReader = objCom.ExecuteReader();
                    while (objReader.Read())
                    {
                        co.CAPANo = objReader["CAPANumber"].ToString();
                        co.PlantName = objReader["Plant"].ToString();
                        co.CAPASourceName = objReader["CAPASource"].ToString();
                        co.CreatedName = objReader["CreateBy"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Equals(ex);
                throw new Exception(ex.Message);
            }
            return co;
        }

        public List<WorkPermit> GetWorkpermitApproverList()
        {
            List<WorkPermit> workPermits = new List<WorkPermit>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    var objCom = new SqlCommand();
                    objCom.CommandText = "MobileNewWorkPermitList";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    objCom.Connection = objCon;

                    var objReader = objCom.ExecuteReader();

                    var workPermitList = new List<WorkPermit>();

                    while (objReader.Read())
                    {

                        var workPermit = new WorkPermit();

                        workPermit.ValidityTo = objReader["ValidityTo"].ToString();
                        workPermit.PlantName = objReader["PlantName"].ToString();
                        workPermit.EquipmentName = objReader["EquipmentName"].ToString();
                        workPermit.WorkTypeName = objReader["WorkType"].ToString();
                        workPermit.ApproverName = objReader["Approver"].ToString();
                        workPermit.PermitNumber = objReader["PermitNumber"].ToString();
                        workPermit.PermitID = int.Parse(objReader["PermitID"].ToString());
                        workPermit.ApproverID = int.Parse(objReader["ApproverID"].ToString());
                        workPermit.SafetyOfficer = int.Parse(objReader["SafetyOfficier"].ToString());
                        workPermit.ProcessManager = int.Parse(objReader["ProcessManager"].ToString());
                        workPermit.Electrical = int.Parse(objReader["Electrical"].ToString());
                        workPermit.Mechanical = int.Parse(objReader["mechanical"].ToString());
                        workPermit.Instrument = int.Parse(objReader["instrument"].ToString());
                        workPermit.Gmoperation = int.Parse(objReader["gmoperations"].ToString());

                        if (objReader["WorkDesctiption"] != DBNull.Value)
                        {
                            workPermit.PermitDescription = objReader["WorkDesctiption"].ToString();
                        }
                        if (objReader["PermitIssuer"] != DBNull.Value)
                        {
                            workPermit.PermitIssuer = objReader["PermitIssuer"].ToString();
                        }
                        if (objReader["CompanyName"] != DBNull.Value)
                        {
                            workPermit.Workdoneby = objReader["CompanyName"].ToString();
                        }

                        if (objReader["ApproverComment"] != DBNull.Value)
                        {

                            workPermit.ApproverComments = objReader["ApproverComment"].ToString();
                        }
                        if (objReader["SafetyRemarks"] != DBNull.Value)
                        {
                            workPermit.safetyRemarks = objReader["SafetyRemarks"].ToString();
                        }
                        if (objReader["ProcessMgrRemarks"] != DBNull.Value)
                        {
                            workPermit.proMgrRemarks = objReader["ProcessMgrRemarks"].ToString();
                        }
                        if (objReader["ElecInchargeRemarks"] != DBNull.Value)
                        {
                            workPermit.ElecRemarks = objReader["ElecInchargeRemarks"].ToString();
                        }
                        if (objReader["MechInchRemarks"] != DBNull.Value)
                        {
                            workPermit.MechRemarks = objReader["MechInchRemarks"].ToString();
                        }
                        if (objReader["InstruInchargeRemarks"] != DBNull.Value)
                        {
                            workPermit.InstRemarks = objReader["InstruInchargeRemarks"].ToString();
                        }
                        if (objReader["GMOperRemarks"] != DBNull.Value)
                        {
                            workPermit.GmRemarks = objReader["GMOperRemarks"].ToString();
                        }

                        workPermits.Add(workPermit);

                    }
                    objCon.Close();

                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Equals(ex);
                throw new Exception(ex.Message);
            }
            return workPermits;
        }
        public List<ContractorEmp> GetContractorEmp()
        {
            List<ContractorEmp> cont = new List<ContractorEmp>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    var objCom = new SqlCommand();
                    objCom.CommandText = "[GetContractorEmp]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    var reader = objCom.ExecuteReader();
                    if (reader.HasRows)
                    {
                        cont.Add(new ContractorEmp { EMPID = 0, EMPName = "--Select--" });

                    }
                    while (reader.Read())
                    {
                        cont.Add(new ContractorEmp
                        {
                            EMPID = reader["ID"].GetHashCode(),
                            EMPName = reader["Name"].ToString()
                        });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return cont;
        }
        public LoginResponse WorkPermitUpdate(PermitUpdate permit)
        {
            LoginResponse loginResponse = null;
            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[MobileWorkPermitUpdate]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@WorkPermitID", permit.PermitID);
                    objCom.Parameters.AddWithValue("@ApprovedBy", permit.ApprovedBy);
                    objCom.Parameters.AddWithValue("@Approvercomment", permit.ApproverComments);
                    objCom.Parameters.AddWithValue("@Identity", permit.Identity);
                    objCom.Parameters.AddWithValue("@Status", permit.Status);
                    objCon.Open();
                    objCom.Connection = objCon;
                    affectedRecordCount = objCom.ExecuteNonQuery();

                    objCom.Parameters.Clear();

                    objCon.Close();
                    loginResponse = new LoginResponse
                    {
                        ErrorMessage = "Updated Successfully",
                        PermitID = permit.PermitID
                    };
                }

            }
            catch (Exception ex)
            {
                LogManager.Instance.Equals(ex);
                throw new Exception(ex.Message);
            }
            return loginResponse;
        }
        public LoginResponse WorkPermitAttach(WorkPermitAttach workPermitAttach)
        {
            LoginResponse loginResponse = null;
            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[WorkPermitAttachSave]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@Permit", workPermitAttach.PermitID);
                    objCom.Parameters.AddWithValue("@FileName", workPermitAttach.FileName);

                    objCon.Open();
                    objCom.Connection = objCon;
                    affectedRecordCount = objCom.ExecuteNonQuery();

                    objCom.Parameters.Clear();

                    objCon.Close();
                    loginResponse = new LoginResponse
                    {
                        ErrorMessage = "Attached Successfully",
                        PermitID = workPermitAttach.PermitID
                    };
                }

            }
            catch (Exception ex)
            {
                LogManager.Instance.Equals(ex);
                throw new Exception(ex.Message);
            }
            return loginResponse;
        }
        public LoginResponse CAPAInsertUpdate(CAPAinsert cAPAinsert)
        {
            LoginResponse loginResponse = null;
            CAPAObservation capaob = new CAPAObservation();
            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[CAPAInsert]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@PlantID", cAPAinsert.plantID);
                    objCom.Parameters.AddWithValue("@AuditTypeID", cAPAinsert.AuditType);
                    objCom.Parameters.AddWithValue("@CAPASourceID", cAPAinsert.CAPASource);
                    objCom.Parameters.AddWithValue("@ReportedBy", cAPAinsert.ReportBy);
                    objCom.Parameters.AddWithValue("@AuditDate", DateTime.ParseExact(cAPAinsert.ReportDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    objCom.Parameters.AddWithValue("@UserID", cAPAinsert.CreatedBy);
                    objCom.Parameters.AddWithValue("@CAPAID", cAPAinsert.CAPAID);
                    objCom.Parameters.AddWithValue("@StatusID", 1);
                    SqlParameter outPutParameter = new SqlParameter();
                    outPutParameter.ParameterName = "@NewcapaID";
                    outPutParameter.SqlDbType = System.Data.SqlDbType.Int;
                    outPutParameter.Direction = System.Data.ParameterDirection.Output;
                    objCom.Parameters.Add(outPutParameter);

                    objCom.Connection = objCon;
                    objCon.Open();
                    affectedRecordCount = objCom.ExecuteNonQuery();
                    cAPAinsert.CAPAID = int.Parse(outPutParameter.Value.ToString());
                    objCom.Parameters.Clear();
                    capaob = GetAPAObservation(cAPAinsert.CAPAID);
                    loginResponse = new LoginResponse
                    {
                        ErrorMessage = "Created Successfully",
                        CAPAID = cAPAinsert.CAPAID,
                        CAPANO = capaob.CAPANo
                    };
                    objCon.Close();
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Equals(ex);
                throw new Exception(ex.Message);
            }
            return loginResponse;
        }


        public LoginResponse CAPAObservation(SAVECAPAObservation scapa, string filename)
        {
            LoginResponse response = null;
            CAPAObservation capaob = new CAPAObservation();
            int affectedCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[SaveCAPAObservation]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@CAPAID", scapa.CAPAID);
                    objCom.Parameters.AddWithValue("@ObservationID", scapa.OBID);
                    objCom.Parameters.AddWithValue("@ObPlantID", scapa.OBPlantID);
                    objCom.Parameters.AddWithValue("@Title", scapa.Observation);
                    objCom.Parameters.AddWithValue("@Recommendation", scapa.Recommendation);
                    objCom.Parameters.AddWithValue("@FileName", filename);
                    objCom.Parameters.AddWithValue("@UserID", scapa.UserID);
                    SqlParameter outPutParameter = new SqlParameter();
                    outPutParameter.ParameterName = "@OBID";
                    outPutParameter.SqlDbType = System.Data.SqlDbType.Int;
                    outPutParameter.Direction = System.Data.ParameterDirection.Output;
                    objCom.Parameters.Add(outPutParameter);

                    objCom.Connection = objCon;
                    objCon.Open();
                    affectedCount = objCom.ExecuteNonQuery();
                    if (scapa.OBID == 0)
                    {
                        scapa.OBID = int.Parse(outPutParameter.Value.ToString());
                    }
                    objCom.Parameters.Clear();
                    capaob = GetAPAObservation(scapa.CAPAID);
                    response = new LoginResponse
                    {
                        CAPANO = capaob.CAPANo,
                        RecomID = scapa.OBID,
                        ErrorMessage = "CAPA Observation Saved Successfully!"
                    };
                    objCon.Close();
                }

            }
            catch (Exception ex)
            {
                LogManager.Instance.Equals(ex);
                throw new Exception(ex.Message);

            }
            return response;
        }
        public LoginResponse IncidentReportUpdate(NewIncidentViewModel incidentReport, string fileName)
        {
            int affectedRecordCount = 0;
            LoginResponse LoginResponse = null;
            Incident inci = new Incident();
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "MobileIncidentReportUpdate";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@ECNumber", incidentReport.ECNumber);
                    objCom.Parameters.AddWithValue("@Description", incidentReport.Description);
                    objCom.Parameters.AddWithValue("@IncidentDate", DateTime.ParseExact(incidentReport.IncidentTime, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture));
                    if (!string.IsNullOrEmpty(incidentReport.ReportedDate))
                    {
                        objCom.Parameters.AddWithValue("@ReportedDate", DateTime.ParseExact(incidentReport.ReportedDate, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture));
                    }
                    objCom.Parameters.AddWithValue("@StatusID", 1);
                    objCom.Parameters.AddWithValue("@PlantID", incidentReport.PlantID);
                    objCom.Parameters.AddWithValue("@IncidentClassID", incidentReport.IncidentClassficationID);

                    objCom.Parameters.AddWithValue("@injuredOrNot", incidentReport.injuredOrNot);
                    objCom.Parameters.AddWithValue("@injuredDecription", incidentReport.injuredDecription == null ? String.Empty : incidentReport.injuredDecription);
                    objCom.Parameters.AddWithValue("@LossOfMaterial", incidentReport.LossOfMaterial);
                    objCom.Parameters.AddWithValue("@LossQuantity", incidentReport.LossQuantity == null ? String.Empty : incidentReport.LossQuantity);
                    objCom.Parameters.AddWithValue("@DamageEquipment", incidentReport.DamageEquipment);
                    objCom.Parameters.AddWithValue("@DamageDetails", incidentReport.DamageDetails == null ? String.Empty : incidentReport.DamageDetails);
                    objCom.Parameters.AddWithValue("@PersonAvailable", incidentReport.PersonAvailable);
                    objCom.Parameters.AddWithValue("@ImmediateAction", incidentReport.ImmediateAction);
                    objCom.Parameters.AddWithValue("@FileName", fileName);

                    objCom.Parameters.AddWithValue("@UserID", incidentReport.CurrentUserID);
                    objCom.Parameters.AddWithValue("@IncidentID", incidentReport.IncidentID);
                    SqlParameter outPutParameter = new SqlParameter();
                    outPutParameter.ParameterName = "@NewIncidentID";
                    outPutParameter.SqlDbType = System.Data.SqlDbType.Int;
                    outPutParameter.Direction = System.Data.ParameterDirection.Output;
                    objCom.Parameters.Add(outPutParameter);

                    objCom.Connection = objCon;
                    objCon.Open();
                    affectedRecordCount = objCom.ExecuteNonQuery();
                    incidentReport.IncidentID = int.Parse(outPutParameter.Value.ToString());

                    objCom.Parameters.Clear();
                    inci = GetIncidentValue(incidentReport.IncidentID);

                    LoginResponse = new LoginResponse
                    {
                        ErrorMessage = "Created Successfully",
                        IncidentID = inci.IncidentNO
                    };
                    objCon.Close();
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return LoginResponse;
        }
        public Incident GetIncidentValue(int IncidentID)
        {
            Incident incident = new Incident();
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    var objCom = new SqlCommand();
                    objCom.CommandText = "[GetIncident]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@IncidentID", IncidentID);
                    objCom.Connection = objCon;
                    objCon.Open();
                    var reader = objCom.ExecuteReader();

                    if (reader.Read())
                    {
                        incident.IncidentID = int.Parse(reader["ID"].ToString());
                        incident.IncidentNO = reader["IncidentNO"].ToString();


                        objCon.Close();
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return incident;
        }
        public List<IncidentViewModel> GetIncident()
        {
            List<IncidentViewModel> incident = new List<IncidentViewModel>();
            try
            {
                using (SqlConnection objcon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objcom = new SqlCommand();
                    objcom.CommandText = "GetIncidentListForAndroid";
                    objcom.CommandType = CommandType.StoredProcedure;
                    objcom.Connection = objcon;
                    objcon.Open();
                    SqlDataReader reader = objcom.ExecuteReader();
                    while (reader.Read())
                    {
                        IncidentViewModel incidentView = new IncidentViewModel();

                        incidentView.IncidentID = int.Parse(reader["ID"].ToString());
                        incidentView.ECNumber = reader["ECNumber"].ToString();
                        incidentView.Description = reader["Description"].ToString();
                        incidentView.IncidentTime = reader["IncidentDate"].ToString();
                        incidentView.PlanID = int.Parse(reader["PlantID"].ToString());
                        incidentView.IncidentClassificationID = int.Parse(reader["IncidentClassID"].ToString());
                        incidentView.ReportedDate = reader["ReportedDate"].ToString();
                        incidentView.ReportedBy = reader["ReportedBy"].ToString();
                        incidentView.injuredOrNot = reader["InjuredOrNot"].ToString();
                        incidentView.CreatedBy = int.Parse(reader["CreatedBy"].ToString());

                        if (reader["InjuredDescription"] != DBNull.Value)
                        {
                            incidentView.injuredDecription = reader["InjuredDescription"].ToString();
                        }
                        incidentView.LossOfMaterial = reader["LossOfMaterial"].ToString();
                        if (reader["LossQuantity"] != DBNull.Value)
                        {
                            incidentView.LossQuantity = reader["LossQuantity"].ToString();
                        }
                        incidentView.DamageEquipment = reader["DamageEquipment"].ToString();
                        if (reader["DamageDetails"] != DBNull.Value)
                        {
                            incidentView.DamageDetails = reader["DamageDetails"].ToString();
                        }
                        incidentView.PersonAvailable = reader["PersonAvailable"].ToString();
                        incidentView.ImmediateAction = reader["ImmediateAction"].ToString();

                        if (reader["RootCauseID"] != DBNull.Value)
                        {
                            incidentView.RootCauseID = int.Parse(reader["RootCauseID"].ToString());
                        }
                        incidentView.FileName = reader["Attachments"].ToString();

                        incident.Add(incidentView);
                    }

                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Equals(ex);
                throw new Exception(ex.Message);
            }
            return incident;
        }
      
        public List<LoginCredential> GetLogin()
        {



            try
            {
                using (SqlConnection objcon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objcom = new SqlCommand();
                    objcom.CommandText = "GetLogin";
                    objcom.CommandType = CommandType.StoredProcedure;
                    objcom.Connection = objcon;
                    objcon.Open();
                    var lc = new List<LoginCredential>();
                    SqlDataReader reader = objcom.ExecuteReader();
                    while (reader.Read())
                    {
                        lc.Add(new LoginCredential
                        {
                            UserName = reader["UserName"].ToString(),
                            Password = reader["Password"].ToString()
                        });

                    }

                    return lc;
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Equals(ex);
                throw new Exception(ex.Message);
            }

        }
        #region Logout
        public LogoutResponse Logout(LogoutUser logout)
        {
            LogManager.Instance.Info("MobileApp - Logout - Start");
            string planDataString = string.Empty;
            int affectedRecordCount = 0;
            LogoutResponse logout1 = null;

            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[MobileLogout]";
                    objCom.CommandType = CommandType.StoredProcedure;

                    objCom.Parameters.AddWithValue("@Token", logout.Token);
                    objCom.Connection = objCon;
                    objCon.Open();
                    affectedRecordCount = objCom.ExecuteNonQuery();
                    objCon.Close();
                    if (affectedRecordCount > 0)
                    {
                        logout1 = new LogoutResponse();
                        logout1.Response = "Success";
                    }
                }
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
            }
            LogManager.Instance.Info("MobileApp - Logout - End");

            return logout1;
        }
        #endregion

        #region GetEquipment

        public List<Equipment> GetEquipment(string username)
        {
            LogManager.Instance.Info("MobileApp - GetEquipment - Start");
            List<Equipment> equipmentList = new List<Equipment>();

            try
            {
                string ConnectionString = AppConfig.ConnectionString;
                using (SqlConnection objCon = new SqlConnection(ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[MobileEquipmentListByUser]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@UserName", username);
                    // objCom.Parameters.AddWithValue("@Token", appToken);
                    objCon.Open();
                    objCom.Connection = objCon;

                    SqlDataReader objReader = objCom.ExecuteReader();

                    while (objReader.Read())
                    {
                        Equipment equipment = new Equipment();
                        equipment.EquipmentID = objReader["EquipmentID"].ToString();
                        equipment.ScanOption = objReader["ScanOption"].ToString();
                        equipment.EquipmentTagID = objReader["EquipmentTagID"].ToString();
                        equipment.EquipmentName = objReader["EquipmentName"].ToString();
                        equipment.ParameterID = objReader["ParameterID"].ToString();
                        equipment.ParameterTagID = objReader["HistorianTagID"].ToString();
                        equipment.ParameterName = objReader["ParameterName"].ToString();

                        equipment.LTV = objReader["LTV"].ToString();
                        equipment.HTV = objReader["HTV"].ToString();
                        equipment.UOM = objReader["UOM"].ToString();
                        equipment.DataType = objReader["IsNumeric"].ToString();
                        equipmentList.Add(equipment);
                    }
                }
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
            }
            LogManager.Instance.Info("MobileApp - GetEquipment - End");
            return equipmentList;

        }


        #endregion

        #region SaveEquipment
        public int SaveEquipment(ParametersSaveRequest reqEntity)
        {
            LogManager.Instance.Info("MobileApp - SaveEquipment - Start");

            string planDataString = string.Empty;
            int affectedRecordCount = 0;

            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(reqEntity.GetType());
                    using (StringWriter textWriter = new StringWriter())
                    {
                        xmlSerializer.Serialize(textWriter, reqEntity);
                        planDataString = textWriter.ToString();

                        LogManager.Instance.Info(planDataString);
                    }


                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[MobilePlanDataInsert]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@PlanData", planDataString);
                    objCom.Connection = objCon;
                    objCon.Open();
                    affectedRecordCount = objCom.ExecuteNonQuery();
                    objCon.Close();
                }
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
            }
            LogManager.Instance.Info("MobileApp - SaveEquipment - End");

            return affectedRecordCount;
        }
        #endregion

        #region GetRecentMeasuredValues

        public List<MeasuredData> GetRecentMeasuredValues(int equipmentId)
        {
            LogManager.Instance.Info("MobileApp - GetRecentMeasuredValues - Start");
            List<MeasuredData> measuredDataList = null;

            try
            {
                string ConnectionString = AppConfig.ConnectionString;
                using (SqlConnection objCon = new SqlConnection(ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[MobileRecentMeasuredValuesByEquipment]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@EquipmentId", equipmentId);
                    objCon.Open();
                    objCom.Connection = objCon;

                    SqlDataReader objReader = objCom.ExecuteReader();

                    measuredDataList = new List<MeasuredData>();
                    MeasuredData measuredData = null;
                    Value TimeValue = null;
                    string parameterName = string.Empty;
                    while (objReader.Read())
                    {
                        parameterName = objReader["ParameterName"].ToString();

                        if (measuredDataList.FindIndex(x => x.ParameterName == parameterName) == -1)
                        {
                            measuredData = new MeasuredData();
                            measuredData.Value = new List<Value>();
                            measuredData.ParameterName = objReader["ParameterName"].ToString();
                            measuredData.UOM = objReader["UOM"].ToString();
                        }

                        TimeValue = new Value();
                        TimeValue.Time = objReader["Time"].ToString();
                        TimeValue.value = objReader["CV"].ToString();

                        measuredData.Value.Add(TimeValue);

                        if (measuredDataList.FindIndex(x => x.ParameterName == parameterName) == -1)
                        {
                            measuredDataList.Add(measuredData);
                        }

                    }
                }
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
            }
            LogManager.Instance.Info("MobileApp - GetRecentMeasuredValues - End");
            return measuredDataList;

        }


        #endregion
        #region GetOutofRange

        public List<Notification> GetOutofRange(string userName, string appToken)
        {
            LogManager.Instance.Info("MobileApp - GetOutofRange - Start");
            List<Notification> notificationList = new List<Notification>();

            try
            {
                string ConnectionString = AppConfig.ConnectionString;
                using (SqlConnection objCon = new SqlConnection(ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[MobileGetOutOfRangeParameter]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@UserName", userName);
                    objCom.Parameters.AddWithValue("@Token", appToken);
                    objCon.Open();
                    objCom.Connection = objCon;

                    SqlDataReader objReader = objCom.ExecuteReader();

                    while (objReader.Read())
                    {
                        Notification notification = new Notification();

                        notification.EquipmentID = objReader["EquipmentID"].ToString();
                        notification.EquipmentName = objReader["EquipmentName"].ToString();
                        notification.ParameterID = objReader["ParameterID"].ToString();
                        notification.ParameterName = objReader["ParameterName"].ToString();
                        notification.status = objReader["Status"].ToString();
                        notification.Time = objReader["MeasuredTime"].ToString();
                        notificationList.Add(notification);

                    }
                }
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
            }
            LogManager.Instance.Info("MobileApp - GetEquipment - End");
            return notificationList;

        }



        #endregion
        public data Data(string tag, string value, string DateTime)
        {

            data dataobj = new data();
            using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
            {

                SqlCommand objCom = new SqlCommand();
                objCom.CommandText = "[DataInsert]";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@TAG", tag.ToString());
                objCom.Parameters.AddWithValue("@Value", value);
                objCom.Parameters.AddWithValue("@DateTime", DateTime);
                objCom.Connection = objCon;
                objCon.Open();
                objCom.ExecuteNonQuery();
                objCon.Close();
            }


            return dataobj;
        }
    }
}
