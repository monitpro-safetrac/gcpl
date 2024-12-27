using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.Incident
{
    public class FishBone:BaseEntity
    {
        public int IncidentID { get; set; }
        public string IncidentNo { get; set; }
        public string PlantName { get; set; }
        public string Header1 { get; set; }
        public string ManSub1 { get; set; }
        public string ManSub2 { get; set; }
        public string ManSub3 { get; set; }
        public string ManSub4 { get; set; }
        public string ManSub5 { get; set; } 

        public string Header2 { get; set; }
        public string MachiSub1 { get; set; }
        public string MachiSub2 { get; set; }
        public string MachiSub3 { get; set; }
        public string MachiSub4 { get; set; }
        public string MachiSub5 { get; set; }

        public string Header3 { get; set; }
        public string MethodSub1 { get; set; }
        public string MethodSub2 { get; set; }
        public string MethodSub3 { get; set; }
        public string MethodSub4 { get; set; }
        public string MethodSub5 { get; set; }


        public string Header4 { get; set; }
        public string MaterialSub1 { get; set; }
        public string MaterialSub2 { get; set; }
        public string MaterialSub3 { get; set; }
        public string MaterialSub4 { get; set; }
        public string MaterialSub5 { get; set; }

        public string Header5 { get; set; }
        public string MeasureSub1 { get; set; }
        public string MeasureSub2 { get; set; }
        public string MeasureSub3 { get; set; }
        public string MeasureSub4 { get; set; }
        public string MeasureSub5 { get; set; }
        public string Header6 { get; set; }
        public string EnviSub1 { get; set; }
        public string EnviSub2 { get; set; }
        public string EnviSub3 { get; set; }
        public string EnviSub4 { get; set; }
        public string EnviSub5 { get; set; }
        public string Title { get; set; }
        public int CurrentUserID { get; set; }
        public int ButtonValue { get; set; }
        public string FishImage { get; set; }
    }
}
