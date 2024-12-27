using System;

namespace MonitPro.Models.MOC
{
    public class MOCViewModel
    {
        public int SNo { get; set; }

        public int MOCID { get; set; }

        public int MOCCreatedBy { get; set; }
        public string MOCCOOrdinate { get; set; }
        public int FuncationalManagerID { get; set; }

        public string MOCNumber { get; set; }

        public string MOCType{ get; set; }

        public string PlantArea { get; set; }
        
        public string MOCCategory { get; set; }

        public string TargetDate { get; set; }

        public string MocStatus { get; set; }

       public string ActionTaken { get; set; }

       public string Approver { get; set; }

       public string FileName { get; set; }

       public string Description { get; set; }

       public string MOCCreated { get; set; }

       public string MOCClosedDate { get; set; }

       public string MOCTitle { get; set; }

       public string ClassName { get; set; }
        public string TempMOCStatus { get; set; }
      

    }
}
