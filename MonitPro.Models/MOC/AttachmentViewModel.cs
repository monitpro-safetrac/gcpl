using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MonitPro.Models.Incident;

namespace MonitPro.Models.MOC
{
    public class AttachmentViewModel : MonitPro.Models.BaseEntity
    {
        public int MOCID { get; set; }
        public int StatusID { get; set; }
        public string MOCDescription { get; set; }
        public string PlantArea { get; set; }
    
        public MOCAttachment MocAttachments = new MOCAttachment();

        public List<MOCAttachment> mocattach = new List<MOCAttachment>();
    }
}
