using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MonitPro.Models.MOC
{
    [XmlRoot("MOCChangeDetails")]
    public class MOCChangeXML
    {

        public int MOCChangeID { get; set; }

    }
    [XmlRoot("MOCSecondaryChangeDetails")]
    public class MOCSecondaryChangeXML
    {
        public int SecondaryChangeID { get; set; }
    }
}