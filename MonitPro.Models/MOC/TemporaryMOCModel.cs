using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitPro.Models.Incident;

namespace MonitPro.Models.MOC
{
   public  class TemporaryMOCModel:MonitPro.Models.BaseEntity
    {
        public string TempIdentity { get; set; }
        public List<TemporaryMOCList> TemporaryMOC = new List<TemporaryMOCList>();
      public MOCSearchViewModel MOCSearchVM = new MOCSearchViewModel();
      public int MOCID { get; set; }
      public string Comments { get; set; }
      public MOCa moca { get; set; }
      public int CurrentUser { get; set; }
      public int FactoryManager { get; set; }
      public string  CloseStatus { get; set; }
        public List<MOCTempStatus> TempStatusList { get; set; }
        public List<UserProfile> MocCoordinateUserList { get; set; }
    }

}
