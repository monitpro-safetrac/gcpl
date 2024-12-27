using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MonitPro.Models.Incident
{
    [XmlRoot("Tenets4Save")]
    public class Tenets4XML
    {
        public int TemplateID { get; set; }

        public int Tenets4ID { get; set; }

        public string Name { get; set; }
    }
}
