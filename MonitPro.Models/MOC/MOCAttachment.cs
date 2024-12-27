using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MonitPro.Models.MOC
{
    public class MOCAttachment
    {
        public int MOCAttachmentId { get; set; }
        public int SNo { get; set; }
        public int MOCId { get; set; }
        public string MOCNo { get; set; }
        [Required(ErrorMessage = "Required")]
        public string ImageName { get; set; }

        public string FileName { get; set; }

        [Required(ErrorMessage = "Required")]
        public HttpPostedFileBase ImageFile { get; set; }
        [Required(ErrorMessage = "Required")]
        public string ImageDescription { get; set; }
    }
}