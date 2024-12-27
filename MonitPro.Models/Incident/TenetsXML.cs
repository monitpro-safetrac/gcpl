using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MonitPro.Models.Incident
{
    [XmlRoot("TenetSave")]
    public class TenetsXML
    {
        public int TenetsID { get; set; }

        public string TenetsName { get; set; }

        public string Details { get; set; }
    }
}