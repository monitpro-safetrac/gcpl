using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;
using System.Xml.Serialization;
using System.IO;
using MonitPro.Common.Library;
using MonitPro.DAL;
using NCalc;
using MonitPro.MobileApp.Model;
using System.Configuration;

namespace MonitPro.BLL
{
    public class MobileAppBLL
    {
        MobileAppDA mobileAppDA = new MobileAppDA();
        public LoginResponse ValidateUser(LoginCredential loginCredential)
        {

            for (int i = 0; i < 3; i++)
            {
                loginCredential.Password = MonitPro.Security.Security.Instance.Encrypt(loginCredential.Password);
            }
            MobileAppDA mobileAppDA = new MobileAppDA();
            return mobileAppDA.ValidateUser(loginCredential);
        }

        public List<Plants> GetPlant()
        {
            return mobileAppDA.GetPlant();
        }

        public LoginResponse UpdateWorkPermitRating(UpdateWorkPermitRating rating)
        {
            return mobileAppDA.UpdateWorkPermitRating(rating);
        }


        public CAPAObservation GetAPAObservation(int CAPAID)
        {
            return mobileAppDA.GetAPAObservation(CAPAID);
        }
       
        public List<IncidentClassfication> GetIncidentClassfication()
        {
            return mobileAppDA.GetIncidentClassfication();
        }
        public List<AuditType> GetAuditType()
        {

            return mobileAppDA.GetAuditType();
        }
        public List<CAPASource> GetAuditCAPAsource(int? AuditID)
        {
            MobileAppDA mobileAppDA = new MobileAppDA();
            return mobileAppDA.GetAuditCAPAsource(AuditID);
        }
        public List<ContractorEmp> GetContractorEmp()
        {

            return mobileAppDA.GetContractorEmp();
        }
      
        public LoginResponse IncidentReportUpdate(NewIncidentViewModel incidentReport)
        {

            string strECNo = String.Format("EC{0}", DateTime.Now);
            incidentReport.ECNumber = incidentReport.ECNumber == null ? strECNo : incidentReport.ECNumber;
            string fileName = null;
            if (incidentReport.ImageFile != null)
            {
                fileName = Path.GetFileName(incidentReport.FileName);

                mobileAppDA.IncidentReportUpdate(incidentReport, fileName);
            }
            else
            {
                fileName = incidentReport.FileName == null ? String.Empty : incidentReport.FileName;

                mobileAppDA.IncidentReportUpdate(incidentReport, fileName);
            }
            return mobileAppDA.IncidentReportUpdate(incidentReport, fileName);
        }
        public LoginResponse CAPAObservation(SAVECAPAObservation scapa)
        {
            string fileName = null;
            if (scapa.CAPAImgeFile != null)
            {
                fileName = Path.GetFileName(scapa.FileName);

                mobileAppDA.CAPAObservation(scapa, fileName);
            }
            else
            {
                fileName = scapa.FileName == null ? String.Empty : scapa.FileName;

                mobileAppDA.CAPAObservation(scapa, fileName);
            }
            return mobileAppDA.CAPAObservation(scapa, fileName);
        }
        public LoginResponse CAPAInsertUpdate(CAPAinsert cAPAinsert)
        {
            return mobileAppDA.CAPAInsertUpdate(cAPAinsert);
        }
        public List<CAPAPlants> GetcapaPlants()
        {
            return mobileAppDA.GetcapaPlants();
        }
        public LoginResponse WorkPermitUpdate(PermitUpdate permit)
        {
            return mobileAppDA.WorkPermitUpdate(permit);
        }
        public LoginResponse WorkPermitAttach(WorkPermitAttach workPermitAttach)
        {
            return mobileAppDA.WorkPermitAttach(workPermitAttach);
        }
        public List<WorkPermit> GetWorkpermitApproverList()
        {

            return mobileAppDA.GetWorkpermitApproverList();
        }

        public List<IncidentViewModel> GetIncident()
        {

            return mobileAppDA.GetIncident();
        }
        public Attachment AddTempAttachment(string FullFileName, string folderToUpload, int sno)
        {
            Attachment attachment = new Attachment();
            string pathToUpload = ConfigurationManager.AppSettings["PathToUpload"];

            attachment.FileName = Path.GetFileName(FullFileName);
            attachment.FilePath = FullFileName;
            attachment.SNo = sno++;


            return attachment;
        }
        public LogoutResponse Logout(LogoutUser logout)
        {
            return mobileAppDA.Logout(logout);

        }
        public List<LoginCredential> GetLogin()
        {

            return mobileAppDA.GetLogin();
        }
        public List<EquipmentResponse> GetEquipment(string userName)
        {

            var equipment = mobileAppDA.GetEquipment(userName);
            var equipmentInfo = equipment.Select(x => new { x.EquipmentID, x.EquipmentTagID, x.EquipmentName, x.ScanOption }).Distinct().ToList();

            List<EquipmentResponse> equipmentList = new List<EquipmentResponse>();

            foreach (var equipmentTagName in equipmentInfo)
            {
                EquipmentResponse equipmentResponse = new EquipmentResponse();
                equipmentResponse.id = equipmentTagName.EquipmentID;
                equipmentResponse.tagId = equipmentTagName.EquipmentTagID;
                equipmentResponse.name = equipmentTagName.EquipmentName;
                equipmentResponse.scanOption = equipmentTagName.ScanOption;
                var equipmentParameters = equipment.Where(x => x.EquipmentTagID == equipmentTagName.EquipmentTagID);

                List<Parameter> parameterList = new List<Parameter>();
                foreach (var item in equipmentParameters)
                {
                    Parameter parameter = new Parameter();
                    parameter.id = item.ParameterID;
                    parameter.tagId = item.ParameterTagID;
                    parameter.name = item.ParameterName;
                    parameter.min = item.LTV;
                    parameter.max = item.HTV;
                    parameter.uom = item.UOM;
                    parameter.dataType = item.DataType;
                    parameterList.Add(parameter);
                }

                equipmentResponse.parameter = parameterList;
                equipmentList.Add(equipmentResponse);
            }

            return equipmentList;

        }

        public int SaveEquipment(ParametersSaveRequest reqEntity)
        {



            /******* Formula Calculation part *******/

            string ConnectionString = AppConfig.ConnectionString;
            string planDataString = string.Empty;
            string ListString = string.Empty;
            List<MobileFormula> mobilelist = new List<MobileFormula>();
            List<MobileFormula> measuredlist = new List<MobileFormula>();
            List<MobileFormula> calculatedlist = new List<MobileFormula>();
            MobileFormula mobileunit = new MobileFormula();
            List<TimeList> timelist = new List<TimeList>();
            List<ID> idlist = new List<ID>();
            ParametersSaveRequest reqEntity_temp = new ParametersSaveRequest();
            reqEntity_temp = reqEntity;

            foreach (EquipmentParameterValues entity in reqEntity.Equipments)
            {
                idlist.Add(new ID { equipmentid = entity.id.ToString() });
                timelist.Add(new TimeList { equipmentid = entity.id.ToString(), savetime = entity.saveTime.ToString() });

            }
            XmlSerializer xmlSerializer = new XmlSerializer(idlist.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, idlist);
                ListString = textWriter.ToString();
            }

            using (SqlConnection objCon = new SqlConnection(ConnectionString))
            {
                SqlCommand objCom = new SqlCommand();
                objCom.CommandText = "MobileCalculationParams";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("List", ListString);
                objCon.Open();
                objCom.Connection = objCon;
                SqlDataReader objReader = objCom.ExecuteReader();

                while (objReader.Read())
                {


                    foreach (TimeList t in timelist.Where(x => x.equipmentid == objReader["EquipmentID"].ToString()).ToList())
                    {
                        mobileunit = new MobileFormula();

                        mobileunit.equipmentid = objReader["EquipmentID"].ToString();
                        mobileunit.formula = objReader["Formula"].ToString();
                        mobileunit.planid = objReader["PlanID"].ToString();
                        mobileunit.ptype = objReader["ParameterType"].ToString();
                        mobileunit.tagid = objReader["TagID"].ToString();
                        mobileunit.value = "";
                        mobileunit.time = t.savetime.ToString();
                        mobilelist.Add(mobileunit);

                    }

                }
                measuredlist = mobilelist.Where(x => x.ptype == "M").ToList();
                calculatedlist = mobilelist.Where(x => x.ptype == "C").ToList();
                objCon.Close();
            }

            foreach (MobileFormula m in mobilelist.Where(x => x.ptype == "M").ToList())
            {
                MeasureData mdata = new MeasureData();
                string val = "";
                decimal number;
                mdata = reqEntity.Equipments.Where(x => x.id == m.equipmentid && x.saveTime == m.time).First().parameter.Where(x => x.id == m.planid).FirstOrDefault();
                if (mdata != null)
                    val = mdata.value;
                if (Decimal.TryParse(val, out number))
                {
                    m.value = Math.Round(Convert.ToDecimal(val), 3).ToString("G29");
                    mdata.value = m.value;
                }
                else
                {
                    m.value = val;

                }

            }

            foreach (MobileFormula m in mobilelist.Where(x => x.ptype == "C").ToList())
            {

                double data = 0.00;
                int expressionError = 0;
                object expressionResult = new object();

                string dataString = null;


                string[] output = m.formula.Split('[', ']');

                int z = 0;
                var expression = new Expression(m.formula);

                MobileFormula item = new MobileFormula();
                while (z < output.Length)
                {
                    if (z % 2 != 0)
                    {
                        item = mobilelist.Find(x => x.tagid == output[z] && x.time == m.time);
                        if (item != null)
                        {
                            if (String.IsNullOrEmpty(item.value))
                            {
                                expressionError = 1;
                                break;
                            }
                            expression.Parameters[item.tagid] = Convert.ToDouble(String.Format("{0:+0.00;-#.00}", item.value));
                        }
                        else
                            expressionError = 1;
                    }

                    z++;
                }


                if (!expression.HasErrors() && expressionError == 0)
                {
                    expressionResult = expression.Evaluate();
                    dataString = expressionResult.ToString();
                    if (dataString == "Infinity")
                        expressionResult = null;
                }
                else
                    expressionResult = null;

                if (expressionResult != null)
                {
                    data = Convert.ToDouble(String.Format("{0:+0.00;-#.00}", expressionResult.ToString()));
                    dataString = Math.Round(Convert.ToDecimal(data), 3).ToString("G29");
                }
                else
                    dataString = null;

                if (dataString != null)
                {
                    m.value = dataString;
                    reqEntity_temp.Equipments.Where(x => x.id == m.equipmentid && x.saveTime == m.time).First().parameter.Add(new MeasureData { id = m.planid, value = m.value });

                }

            }




            /*** End of Formula Calculation part ***/






            return mobileAppDA.SaveEquipment(reqEntity_temp);

        }

        public List<MeasuredData> GetRecentMeasuredValues(int equipmentId)
        {

            return mobileAppDA.GetRecentMeasuredValues(equipmentId);

        }
        public List<Notification> GetOutofRange(string userName, string appToken)
        {

            return mobileAppDA.GetOutofRange(userName, appToken);
        }

        public data Data(string tag, string value, string DateTime)
        {

            return mobileAppDA.Data(tag, value, DateTime);

        }

    }
}
