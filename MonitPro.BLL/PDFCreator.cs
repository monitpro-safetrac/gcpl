using System;
using System.IO;
using System.Configuration;
using System.Diagnostics;

using IronPdf;
using System.Text;
using MonitPro.Models.IncidentViewModels;
using MonitPro.DAL;
using MonitPro.Models.Incident;

namespace MonitPro.BLL
{
    public class PDFCreator
    {
        public int IncidentId;
        public DetailedIncidentViewModel IncidentDetailVM = new DetailedIncidentViewModel();
        public IncidentReportDAL InciDAL = new IncidentReportDAL();
        public string LogoPath = string.Empty;
        public string PathToUpload = string.Empty;
        
        public PDFCreator(int incidentId, DetailedIncidentViewModel detailedIncidentViewModel)
        {
            IncidentId = incidentId;
            IncidentDetailVM = detailedIncidentViewModel;
            LogoPath = ConfigurationManager.AppSettings["ImagesPath"];
            PathToUpload = ConfigurationManager.AppSettings["PathToUpload"];

            if (!Directory.Exists(PathToUpload))
            {
                Directory.CreateDirectory(PathToUpload);
            }
        }

        public void CreatePDFFile()
        {
            HtmlToPdf render = new HtmlToPdf();
            PathToUpload = PathToUpload + "\\" + IncidentId;
            //IncidentDetailVM = InciDAL.GetIncidentDetailForView(IncidentId);

            if (!Directory.Exists(PathToUpload))
            {
                Directory.CreateDirectory(PathToUpload);
            }

            StringBuilder pdfDocumentString = new StringBuilder();
            pdfDocumentString.Append("<html><body>");
            pdfDocumentString.Append("<table width='100%' align='center'>");
            pdfDocumentString.Append("<tr><td>" + AddHeaderSection() + "</td></tr>");
            pdfDocumentString.Append("<tr><td>&nbsp;</td></tr>");
            pdfDocumentString.Append("<tr><td>" + AddBodyIncidentSection() + "</td></tr>");
            pdfDocumentString.Append("<tr><td>&nbsp;</td></tr>");
            pdfDocumentString.Append("<tr><td>" + AddBodyObserverSection() + "</td></tr>");
            pdfDocumentString.Append("<tr><td>&nbsp;</td></tr>");
            pdfDocumentString.Append("<tr><td>" + AddBodyObservationSection() + "</td></tr>");
            pdfDocumentString.Append("<tr><td>&nbsp;</td></tr>");
            pdfDocumentString.Append("<tr><td>" + AddBodyActionTakenSection() + "</td></tr>");
            pdfDocumentString.Append("<tr><td>&nbsp;</td></tr>");
            pdfDocumentString.Append("<tr><td>" + AddBodyIMagesSection() + "</td></tr>");
            
            pdfDocumentString.Append("</table>");
            pdfDocumentString.Append("</body></html>");

            var pdf12 = render.RenderHtmlAsPdf(pdfDocumentString.ToString());

            string OutputPath = PathToUpload + "\\" + IncidentId +"_PdfView.pdf";

            if (File.Exists(OutputPath))
            {
                File.Delete(OutputPath);
            }
            pdf12.SaveAs(OutputPath);

            Process.Start(OutputPath);
        }

        private string AddHeaderSection()
        {
            StringBuilder headerSection = new StringBuilder();
            headerSection.Append("<table border='1'><tr><td align='left'><img src='" + LogoPath + "customer-logo.png' /> </td>");
            headerSection.Append("<td align='left'><img src='" + LogoPath + "monitpro-logo.png' width='500px' /></td></tr>");
            headerSection.Append("<tr><td align='center' colspan='2'>Incident Management System</td></tr></table>");

            return headerSection.ToString();

            //public PdfSharp.Pdf.PdfDocument pdfDocument = new PdfSharp.Pdf.PdfDocument();
            //pdfDocument.Save(filePath + "/" + pdfFilename);
            //Process.Start(pdfFilename);
            //var ImageStream = new FileStream("D:\\Navane Work\\IncidentManagementSystem\\Images\\customer-logo.png", FileMode.Open);

            //pdfDocument
            //PdfSharp.Pdf.PdfPage pdfPage = pdfDocument.AddPage();
            //XImage xImage = XImage.FromFile(@"D:\\Sugan\\Pdf Docs\\customer-logo.png");


            //XGraphics graph = XGraphics.FromPdfPage(pdfPage);
            //graph.DrawImage(xImage, new System.Drawing.Point());
            //graph.DrawImage(xImage, new System.Drawing.Point(300, 10));
            //XFont font = new XFont("Verdana", 14, XFontStyle.Bold);
            //graph.DrawString("Incident Report", font, XBrushes.Black,
            //    new XRect(0, 0, pdfPage.Width.Point, 150), XStringFormats.Center);
            ////graph.DrawString("Incident Report", font, XBrushes.Black,
            //    new XRect(0, 0, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.Center);


            //PdfDocument pdfdoc = new PdfDocument();

            //pdfdoc.AddPage(pdfPage);

        }

        private string AddBodyIncidentSection()
        {
            StringBuilder BodySection = new StringBuilder();
            BodySection.Append("<br /><table class='table' border='1'><tr><td align='center' colspan='2'><bold>Incident Details </bold></td></tr>");
            BodySection.Append("<tr><td width='150px'>Incident Id</td><td width='450px'>" + IncidentDetailVM.incidentViewModel.IncidentID.ToString() + " </td></tr>");
            BodySection.Append("<tr><td>Incident Title</td><td>" + IncidentDetailVM.incidentViewModel.Title + "</td></tr>");
            BodySection.Append("<tr><td>Incident Description</td><td> " + IncidentDetailVM.incidentViewModel.Description + " </td></tr>");
            BodySection.Append("<tr><td>Created Date</td><td>" + IncidentDetailVM.incidentViewModel.IncidentTime.ToString() + "</td></tr>");
            BodySection.Append("<tr><td>Status</td><td> " + IncidentDetailVM.incidentViewModel.CurrentStatus + "</td></tr>");
            BodySection.Append("<tr><td>Incident Type</td><td> " + IncidentDetailVM.incidentViewModel.IncidentType + "</td></tr>");
            BodySection.Append("<tr><td>Plan / Area</td><td> " + IncidentDetailVM.incidentViewModel.PlantArea + "</td></tr>");
            BodySection.Append("<tr><td>Created By</td><td> " + IncidentDetailVM.incidentViewModel.CreatedBy + "</td></tr>");
            BodySection.Append("</table>");

            return BodySection.ToString();
        }

        private string AddBodyObserverSection()
        {
            StringBuilder BodySection = new StringBuilder();

            BodySection.Append("<br /><br /> <table class='table' border='1'><tr><td colspan='2'><bold>Incident Observers</bold></td></tr>");
            BodySection.Append("<tr><td>Observer Name </td><td>Designation </td></tr>");

            foreach(IncidentUser user in IncidentDetailVM.incidentUsers)
            {
                BodySection.Append("<tr><td>" + user.EmployeeName +  " </td><td>" + user.Designation + " </td></tr>");
            }

            BodySection.Append("</table>");

            return BodySection.ToString();
        }

        private string AddBodyObservationSection()
        {
            StringBuilder BodySection = new StringBuilder();

            BodySection.Append("<br /><table class='table' align='left' border='1'><tr><td colspan='5' align='center'><bold>Observation Details</bold></td></tr>");
            BodySection.Append("<tr><td>Observation</td><td>Description</td><td>Recommendation</td><td>Root Cause</td><td>Target Date</td></tr>");

            foreach (ObservationViewModel ObsVM in IncidentDetailVM.ObservationDetail)
            {
                BodySection.Append("<tr><td>" + ObsVM.Observation + " </td><td>" + ObsVM.Description + " </td><td>" + ObsVM.Recommendation + " </td><td>" + ObsVM.RootCause + " </td><td>" + ObsVM.TargetDate.ToString() + " </td></tr>");
            }

            BodySection.Append("</table>");

            return BodySection.ToString();
        }

        private string AddBodyActionTakenSection()
        {
            StringBuilder BodySection = new StringBuilder();

            BodySection.Append("<br /><table class='table' align='left' border='1'><tr><td colspan='3' align='center'><bold>Action Taken Details</bold></td></tr>");
            BodySection.Append("<tr><td>Recommendation</td><td>Action Taken</td><td>Completed Date</td></tr>");

            foreach (ObservationViewModel ObsVM in IncidentDetailVM.ObservationDetail)
            {
                BodySection.Append("<tr><td>" + ObsVM.Recommendation + " </td><td>" + ObsVM.ActionTaken + " </td><td>" + ObsVM.CompletedDate + " </td></tr>");
            }

            BodySection.Append("</table>");

            return BodySection.ToString();
        }

        private string AddBodyIMagesSection()
        {
            StringBuilder BodySection = new StringBuilder();

            if (IncidentDetailVM.incidentImages.Count > 0)
            {            
                BodySection.Append("<br /><table class='table' border='1'><tr><td colspan='3' align='center'><bold>Attachments</bold></td></tr>");

                foreach (IncidentImage ImgVM in IncidentDetailVM.incidentImages)
                {
                    BodySection.Append("<tr><td>" + ImgVM.ImageName + " </td></tr>");
                    BodySection.Append("<tr><td><img src='" + ImgVM.FileName + "' /> </td></tr>");
                }

                BodySection.Append("</table>");
            }

            return BodySection.ToString();
        }
    }
}
