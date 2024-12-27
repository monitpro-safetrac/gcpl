using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using MonitPro.Models.Incident;
using MonitPro.Models.CAPA;
using MonitPro.Models.CAPAViewModel;
using MonitPro.Models.IncidentViewModels;



namespace IncidentReportSystem.Models
{
    public class CAPAListViewModel : MonitPro.Models.BaseEntity
    {
      public List<CAPAViewModel> CapaList = new List<CAPAViewModel>();

       public CAPASearchViewModel CAPASearch = new CAPASearchViewModel();

       public List<Status> statusList { get; set; }

        public List<ActionerModel> actionermodel = new List<ActionerModel>();

        public int CurrentUser { get; set; }

        public string FileName { get; set; }

    }
    public class PlantOwnerModel
    {
        public int PlantID { get; set; }
        public int DeptManagerID { get; set; }
    }
    
}