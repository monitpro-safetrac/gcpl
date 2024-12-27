using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
namespace MonitPro.Models.MOC
{
    [XmlRoot("MOCClosureSave")]
    public class MOCClosureXML
    {
        public int MOCClosureId { get; set; }
        public int SaveStatus { get; set; }
        public string SaveRemarks { get; set; }
    }
}