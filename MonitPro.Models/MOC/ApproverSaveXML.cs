using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MonitPro.Models.MOC
{
    [XmlRoot("ApprovalListInsert")]
    public class ApproverSaveXML
    {
        public int ApprovalStagesID { get; set; }
        public int UserID { get; set; }
        public string TargetDate { get; set; }
        public int IsTeamApproval { get; set; }
    }
}
