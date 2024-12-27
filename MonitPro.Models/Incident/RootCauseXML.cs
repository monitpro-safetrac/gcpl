using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
namespace MonitPro.Models.Incident
{
    [XmlRoot("RootCauseSave")]
    public class RootCauseXMLList : BaseEntity
    {
        public int SubsectionID { get; set; }

        public string Name { get; set; }

        public int RootCauseID { get; set; }
    }
}
