using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Data.Entity;
using System.Globalization;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Runtime.Serialization;
using Newtonsoft.Json;
namespace MonitPro.Models
{

    public class Section
    {
        public string id { get; set; }
        public string name { get; set; }
        public Data data { get; set; }
        public List<Children> children { get; set; }
    }

    public class Data
    {
        [DataMember(Name = "$color")]
        [JsonProperty(PropertyName = "$color")]
        public string color { get; set; }
        public string EqpName { get; set; }
        public string type { get; set; }
        public List<Children> dynamic { get; set; }
        [DataMember(Name = "$area")]
        [JsonProperty(PropertyName = "$area")]
        public int area { get; set; }
        public string[] idata { get; set; }
    }

    public class Children
    {
        public string id { get; set; }
        public string name { get; set; }
        public Data data { get; set; }
        public List<Children> children { get; set; }
    }

    public class AutoCompleteData
    {
        public string value { get; set; }
        public string label { get; set; }
        public string desc { get; set; }
    }

    public class LineChartData
    {
        public string TagID { get; set; }
        public string EquipmentName { get; set; }
        public DateTime DateAndTime { get; set; }
        public float MeasuredValue { get; set; }
        public string Parameter { get; set; }
        public string UOM { get; set; }
        public string LTV { get; set; }
        public string HTV { get; set; }
    }

    public class LineChartEntity
    {
        public String Series { set; get; }
        public List<LineChartData> ChartData { get; set; }
        public LineChartEntity()
        {
            ChartData = new List<LineChartData>();
        }
    }

    public class CheckBoxEntity
    {
        public int ID
        {
            get;
            set;
        }
        public string TagID
        {
            get;
            set;
        }

        public bool Checked
        {
            get;
            set;
        }
    }

    public class TrendViewModel :BaseEntity
        {

            [DisplayName("Tag ID")]
            public string TagID { get; set; }

            [DisplayName("From")]
            public string FromDate { get; set; }

            [DisplayName("To")]
            public string ToDate { get; set; }

            
            public List<CheckBoxEntity> TagList { get; set; }

             
            public string ChartImagePath { get; set; }

            public string YaxisMinValue { get; set; }

            public string YaxisMaxValue { get; set; }
        }

#region KPI
   
    public class KPI
    {
        [DataMember(Name = "id")]
         
        public string ID { set; get; }
        [DataMember(Name = "value")]
         public float CalculatedValue { get; set; }
        [DataMember(Name = "name")]
         public string Name { get; set; }
        [DataMember(Name = "red")]
         public int Danger { get; set; }
        [DataMember(Name = "yellow")]
         public float Warning { get; set; }
        [DataMember(Name = "uom")]
         public string UOM { get; set; }
        [DataMember(Name = "etarget")]
         public float EndTarget { get; set; }
        [DataMember(Name = "mtarget")]
         public float MovingTarget { get; set; }
        [DataMember(Name = "ptarget")]
         public float PredictedTarget { get; set; }
    }

    public class Admin1 /* Earlier this class name was AccountDA */
    {
        public List<Template> getKPIIDs(int j = 0)
        {
            List<Template> templateList = new List<Template>();

            try
            {
                string ConnectionString = "Data Source=GURU-PC\\SQLEXPRESS;Initial Catalog=monitpro;Integrated Security=True";

                using (SqlConnection objCon = new SqlConnection(ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    if (j == 0)
                        objCom.CommandText = "usp_GetKPI";
                    else
                        objCom.CommandText = "usp_GetEnterKPI";
                    objCom.CommandType = CommandType.StoredProcedure;


                    objCon.Open();
                    objCom.Connection = objCon;

                    SqlDataReader objReader = objCom.ExecuteReader();

                    /*if (objReader.Read())
                    {
                        string ID = objReader["factoryid"].ToString();
                        
                    }*/





                    while (objReader.Read())
                    {
                        Template template = new Template();
                        template.Templateid = objReader["KPIID"].ToString();
                        template.TemplateName = objReader["ParameterName"].ToString();

                        templateList.Add(template);
                    }



                }

            }
            catch (Exception objException)
            {

                throw objException;
            }
            return templateList;
        }
    }

    //public class Template
    //{
    //    [DataMember(Name = "id")]
    //    [JsonProperty(PropertyName = "id")]
    //    public string Templateid { set; get; }
    //    [DataMember(Name = "name")]
    //    [JsonProperty(PropertyName = "name")]
    //    public string TemplateName { set; get; } //properties
    //    public string Divisionid { set; get; }

    //}

    public class AddKPIModel
    {

        [Required]
        [Display(Name = "KPI Name")]
        public string TemplateId { get; set; }
        public IEnumerable<SelectListItem> Name { get; set; }


        [Required]
        [Display(Name = "End Target")]
        public float EndTarget { get; set; }



        [Required]
        [Display(Name = "Warning Value")]
        public float WarningValue { get; set; }

        [Required]
        [Display(Name = "Danger Value")]
        public int Danger { get; set; }




    }

    public class KPIModel :BaseEntity
    {
        [Required]
        [Display(Name = "Select View")]
        public int ID { get; set; }
        public IEnumerable<SelectListItem> ViewName { get; set; }




    }
#endregion

    public class OtherKPI:BaseEntity
    {


        public string ID { set; get; }
        public List<SelectListItem> Equipments { get; set; } 
        public float CalculatedValue { get; set; }
        public float EndTarget { get; set; }
        public float Min { get; set; }
        public int BandCheck { get; set; }
        public string Name { get; set; }
        public int Danger { get; set; }
        public float Warning { get; set; }
        public float RedBand { get; set; }
        public string UOM { get; set; }
        public string EquipmentID { get; set; }
        public string EquipmentName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
    public class RefreshKPI
    {

        public string ID { set; get; }
        public float CalculatedValue { get; set; }

    }

    }

 
