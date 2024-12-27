using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.CAPAViewModel
{
     public class CAPAViewModel
    {
        public string CAPANumber { get; set; }
        public int SNo { get; set; }

        public int CAPAID { get; set; } 

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime AuditTime { get; set; }
        
        public string PlantArea { get; set; }

        public string CAPASource { get; set; }

        public string AuditType { get; set; }

        public string CurrentStatus { get; set; }

        public string ClosedStatus { get; set; }

        public int CreatedBy { get; set; }

        public string CreatedByName { get; set; }

        public string Comments { get; set; }    

        public string FileName { get; set; }

        public string ActionTaken { get; set; }

        public int CompletedBy { get; set; }      

        public DateTime ClosedTime { get; set; }



    }
    public class CAPADetails
    {
        public string Actioner { get; set; }
        public string FunctionalManager { get; set; }
        public string CAPAAdvisorEmail { get; set; }
        public string FactoryName { get; set; }
        public string Status { get; set; }
        public string TargetDate { get; set; }
        public string Source { get; set; }
        public int RecomID { get; set; }
        public string Recommendation { get; set; }
        public string Priority { get; set; }

        public List<MyActionStatus> List { get; set; }
    }
    public class EmailModel1
    {
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string CC { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
    public class MyActionStatus
    {
        public string FactoryName { get; set; }
        public int IncidentID { get; set; }
        public int CAPAID { get; set; }
        public string Status { get; set; }
        public string Targetdate { get; set; }
        public string Source { get; set; }
        public int RecomID { get; set; }
        public string Recommendation { get; set; }
        public string Priority { get; set; }

    }
}
