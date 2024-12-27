using System;
using System.ComponentModel.DataAnnotations;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MonitPro.Models.Incident
{
    public class Incident
    {
        public int IncidentID { get; set; }

        public string ECNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string IncidentTime { get; set; }
        public string IncidentNO { get; set; }
        [Required]

        public string InvestigationBegan { get; set; }
        public string DidWork { get; set; }
        public string DidNotWork { get; set; }
        public string HaveHepled { get; set; }
        public string Findings { get; set; }
        public string Lessons { get; set; }

        public string ReportedDate { get; set; }

        public string ReportedBy { get; set; }
        [Required]
        public int IncidentTypeID { get; set; }

        public int StatusID { get; set; }


        public int? PlantID { get; set; }
        [Required]
        public int PriorityID { get; set; }
        [Required]
        public string Investigation { get; set; }
        public bool WhyAnalysis { get; set; }
        public bool WhyTree { get; set; }
        public int? IncidentClassficationID { get; set; }

        public int InjuryClassficationID { get; set; }

        public int ClassficationFactorID { get; set; }

        public string Comments { get; set; }

        public int CurrentUserID { get; set; }

        [Required]
        public int InjuryTypeID { get; set; }

        public string PlantName { get; set; }
        public string IncidentType { get; set; }
        public string CreatedByName { get; set; }
        public string PotentialLevel { get; set; }
        public int RootCauseCheck { get; set; }
        public string ImageName { get; set; }

        public string FileName { get; set; }

        public string secondfile { get; set; }

        public string ActionTaken { get; set; }

        public string Incidentchronology { get; set; }

        public string Precautionarymeasures { get; set; }

        public string Analysis { get; set; }

        public int RootCauseID { get; set; }

        public HttpPostedFileBase ImageFile { get; set; }
        public HttpPostedFileBase InvesAttachment { get; set; }
        public InjuredPeoples injuredpeoples { get; set; }

        public string injuredOrNot { get; set; }
        public string injuredDecription { get; set; }

        public string LossOfMaterial { get; set; }
        public string LossQuantity { get; set; }

        public string DamageEquipment { get; set; }
        public string DamageDetails { get; set; }

        public string PersonAvailable { get; set; }

        public string ImmediateAction { get; set; }

        public string ProbableCauses { get; set; }
        public string ApproverComments { get; set; }
        public string IncidentClassName { get; set; }
        public int IncidentChemicalQTY { get; set; }

    }
}

