using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.Common
{
    public class MonitProEmail
    {
        public string ToAddress { get; set; }    

        public string CC { get; set; }

        public string BCC { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
        public string Body1 { get; set; }
    }
}
