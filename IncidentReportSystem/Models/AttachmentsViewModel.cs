using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using MonitPro.Models;
using MonitPro.Models.Incident;

namespace IncidentReportSystem.Models
{
    public class AttachmentsViewModel : BaseEntity
    {
        public List<Attachment> attachments = new List<Attachment>();

        public IncidentImage IncidentImage = new IncidentImage();
    }
}