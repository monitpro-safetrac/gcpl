using System;

namespace MonitPro.Models.Incident
{
    public class Attachment
    {
        public int SNo { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public Guid CurrentSessionId { get; set; }
    }
}
