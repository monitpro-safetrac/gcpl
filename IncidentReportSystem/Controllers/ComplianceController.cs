using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using IncidentReportSystem.Models;
using System.Configuration;
using MonitPro.Validations;
using MonitPro.Models;
using System.Linq;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Web;
using ClosedXML.Excel;
using System.IO;
using MonitPro.Common.Library;
using System.Web.UI;
using System.Web.UI.WebControls;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using PagedList;

namespace ValsparApp.Controllers
{
    [ValidateSession]
    public class ComplianceController : Controller
    {
        private static PdfPCell PhraseCell(Phrase phrase, int align)
        {
            PdfPCell cell = new PdfPCell(phrase);
            //  cell.BorderColor = Color.;
            //cell.VerticalAlignment = PdfCell.ALIGN_TOP;
            cell.HorizontalAlignment = align;
            cell.PaddingBottom = 2f;
            cell.PaddingTop = 0f;
            return cell;
        }

        // GET: Test
        public ActionResult PrintCompliance(int id)
        {
            try
            {
                using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[SavedCompliancePDF]";
                    objCom.Parameters.AddWithValue("@ComplianceID", id);
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    objCom.Connection = objCon;
                    SqlDataAdapter Adapter = new SqlDataAdapter();
                    Adapter.SelectCommand = objCom;
                    DataSet dataSet = new DataSet();
                    Adapter.Fill(dataSet);
                    objCon.Close();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition",
                        "attachment;filename=Form10.pdf");
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);

                    iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 20f, 20f, 30f, 10f);

                    HTMLWorker htmlparsers = new HTMLWorker(pdfDoc);
                    PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                    pdfDoc.Open();
                    PdfPCell cell = null;


                    var titleFont = FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD);
                    var titleFontnormal = FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL);
                    var titleFonteight = FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL);

                    String form = String.Format("  ".PadRight(99) + "FORM 10");

                    pdfDoc.Add(new iTextSharp.text.Paragraph(form, titleFont));

                    String form1 = String.Format("  ".PadRight(97) + "[See Rule 19 (1) ]".ToString().PadRight(200));

                    pdfDoc.Add(new iTextSharp.text.Paragraph(form1, titleFont));

                    PdfPTable tablemanifest = new PdfPTable(1);
                    tablemanifest.LockedWidth = true;
                    tablemanifest.SetWidths(new float[] { 2f });
                    tablemanifest.TotalWidth = 555f;
                    var phrase2 = new Phrase(new Chunk("MANIFEST FOR HAZARDOUS AND OTHER WASTE " + "\n", FontFactory.GetFont("Times New Roman", 12, iTextSharp.text.Font.BOLD)));
                    tablemanifest.AddCell(PhraseCell(phrase2, PdfPCell.ALIGN_CENTER));
                    pdfDoc.Add(tablemanifest);

                    PdfPTable tablesender = new PdfPTable(3);
                    tablesender.TotalWidth = 555f;
                    tablesender.LockedWidth = true;
                    tablesender.SetWidths(new float[] { 1f, 9.5f, 10.5f });

                    tablesender.AddCell(PhraseCell(new Phrase("1.", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("Sender's Name and Mailing Address:                                        (including Phone No. & e-mail)", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase( "Valspar (India) Coatings Corporation Pvt. Ltd.66B & 67, Bommasandra Industrial Area, Bangalore 560099.   Phone No. 080 - 3911 - 2860                                                      e-mail: Debmalya.Dasgupta@Valspar.com", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));



                    tablesender.AddCell(PhraseCell(new Phrase("2.", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("Sender’s Authorization No.", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("PCB/HWM/ SEO/ H.D. Reg. No.  53789/ 2013-14/ H1758", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));




                    tablesender.AddCell(PhraseCell(new Phrase("3.", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("Manifest Document No.", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][0].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                    tablesender.AddCell(PhraseCell(new Phrase("4.", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("Transporter’s Name & Address:                                       (including Phone No. & e - mail)", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][2].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                    tablesender.AddCell(PhraseCell(new Phrase("5.", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("Type of Vehicle", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][4].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));


                    tablesender.AddCell(PhraseCell(new Phrase("6.", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("Transporter’s Registration No.", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][3].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));


                    tablesender.AddCell(PhraseCell(new Phrase("7.", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("Vehicle Registration No. ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][5], FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));



                    tablesender.AddCell(PhraseCell(new Phrase("8.", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("Receiver’s Name & Address:                                                     (including Phone No. & e - mail) ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][6], FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));



                    tablesender.AddCell(PhraseCell(new Phrase("9.", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("Receiver’s Authorization No.", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][7], FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(tablesender);
                   
                    using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("[CompliancePDF]", sqlcon))
                        {
                            cmd.Parameters.AddWithValue("@ComplianceID", id);
                            cmd.CommandType = CommandType.StoredProcedure;

                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                DataTable dt = new DataTable();

                                da.Fill(dt);

                                if (dt != null)
                                {
                                    //Craete instance of the pdf table and set the number of column in that table
                                    PdfPTable PdfTable = new PdfPTable(dt.Columns.Count);
                                    PdfTable.TotalWidth = 555f;
                                    PdfTable.LockedWidth = true;
                                    PdfTable.SetWidths(new float[] { 0.95f, 9.05f, 5f, 5f });
                                    PdfPCell PdfPCell = null;
                                    var font8 = FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD);

                                    //Add Header of the pdf table
                                    PdfPCell = new PdfPCell(new Phrase(new Chunk("10.", font8)));
                                    PdfTable.AddCell(PdfPCell);

                                    PdfPCell = new PdfPCell(new Phrase(new Chunk("Waste Description", font8)));
                                    PdfTable.AddCell(PdfPCell);
                                    PdfPCell = new PdfPCell(new Phrase(new Chunk("UOM", font8)));
                                    PdfTable.AddCell(PdfPCell);
                                    PdfPCell = new PdfPCell(new Phrase(new Chunk("Quantity", font8)));
                                    PdfTable.AddCell(PdfPCell);
                                    //How add the data from datatable to pdf table
                                    for (int rows = 0; rows < dt.Rows.Count; rows++)
                                    {
                                        for (int column = 0; column < dt.Columns.Count; column++)
                                        {
                                            PdfPCell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), font8)));
                                            PdfTable.AddCell(PdfPCell);
                                        }
                                    }

                                    pdfDoc.Add(PdfTable); // add pdf table to the document

                                }
                            }
                        }

                    }


                    PdfPTable table1 = new PdfPTable(4);
                    table1.TotalWidth = 555f;
                    table1.LockedWidth = true;
                    table1.SetWidths(new float[] { 1f, 9.5f, 5.25f,5.25f });

                    table1.AddCell(PhraseCell(new Phrase("11.", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    table1.AddCell(PhraseCell(new Phrase("Total Quantity/No.of Containers", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    table1.AddCell(PhraseCell(new Phrase("Kg" , FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                    table1.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][9], FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(table1);

                    PdfPTable table2 = new PdfPTable(3);
                    table2.TotalWidth = 555f;
                    table2.LockedWidth = true;
                    table2.SetWidths(new float[] { 1f, 9.5f, 10.5f });

                    table2.AddCell(PhraseCell(new Phrase("12.", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    table2.AddCell(PhraseCell(new Phrase("Physical Form", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    table2.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][10], FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));



                    table2.AddCell(PhraseCell(new Phrase("13.", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    table2.AddCell(PhraseCell(new Phrase("Special Handling Instructions & Additional Information", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    table2.AddCell(PhraseCell(new Phrase("Use hand gloves, safety goggles, face mask, safety shoe etc. while handling", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    pdfDoc.Add(table2);


                    PdfPTable sendercerti = new PdfPTable(10);
                    sendercerti.TotalWidth = 555f;
                    sendercerti.LockedWidth = true;
                    sendercerti.SetWidths(new float[] { 1f, 9.5f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f, 1f, 1f, 1f });


                    PdfPCell Pcell1 = new PdfPCell(new Phrase("14.  ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    Pcell1.Rowspan = 3;
                    sendercerti.AddCell(Pcell1);


                    Pcell1 = new PdfPCell(new Phrase("Sender’s Certificate                                             \n\n\n\n\n".ToString() + dataSet.Tables[0].Rows[0][11], FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    sendercerti.AddCell(Pcell1);

                   

                    Pcell1 = new PdfPCell(new Phrase("I hereby declare that the contents of the consignment are fully and accurately described above by proper shipping name and categorized, packed, marked and labelled and are in all respects in proper condition for transport by road according to applicable national government regulations.", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    Pcell1.Colspan = 8;
                    sendercerti.AddCell(Pcell1);

                    Pcell1 = new PdfPCell(new Phrase("Name & Stamp                                  Signature                  ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    sendercerti.AddCell(Pcell1);

                    Pcell1 = new PdfPCell(new Phrase("	       Month", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    Pcell1.Colspan = 2;
                    sendercerti.AddCell(Pcell1);


                    Pcell1 = new PdfPCell(new Phrase("	       Day", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    Pcell1.Colspan = 2;
                    sendercerti.AddCell(Pcell1);

                    Pcell1 = new PdfPCell(new Phrase("	      Year", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    Pcell1.Colspan = 4;
                    sendercerti.AddCell(Pcell1);

                    Pcell1 = new PdfPCell(new Phrase("                      ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    sendercerti.AddCell(Pcell1);

                    Pcell1 = new PdfPCell(new Phrase("	       ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    sendercerti.AddCell(Pcell1);

                    Pcell1 = new PdfPCell(new Phrase("	       ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    sendercerti.AddCell(Pcell1);

                    Pcell1 = new PdfPCell(new Phrase("	      ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    sendercerti.AddCell(Pcell1);

                    Pcell1 = new PdfPCell(new Phrase("	      ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    sendercerti.AddCell(Pcell1);

                    Pcell1 = new PdfPCell(new Phrase("	       ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    sendercerti.AddCell(Pcell1);

                    Pcell1 = new PdfPCell(new Phrase("	       ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    sendercerti.AddCell(Pcell1);

                    Pcell1 = new PdfPCell(new Phrase("	       ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    sendercerti.AddCell(Pcell1);

                    Pcell1 = new PdfPCell(new Phrase("	       ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    sendercerti.AddCell(Pcell1);
                    pdfDoc.Add(sendercerti);


                    PdfPTable transporter = new PdfPTable(10);
                    transporter.TotalWidth = 555f;
                    transporter.LockedWidth = true;
                    transporter.SetWidths(new float[] { 1f, 9.5f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f, 1f, 1f, 1f });


                    PdfPCell Pcell2 = new PdfPCell(new Phrase("15.  ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    Pcell2.Rowspan = 3;
                    transporter.AddCell(Pcell2);


                    Pcell2 = new PdfPCell(new Phrase("	Transporter’s  acknowledgement of   receipt   if  wastes                                         ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    transporter.AddCell(Pcell2);


                    Pcell2 = new PdfPCell(new Phrase("	", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    Pcell2.Colspan = 8;
                    transporter.AddCell(Pcell2);


                    Pcell2 = new PdfPCell(new Phrase("Name & Stamp                                  Signature                  ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    transporter.AddCell(Pcell2);


                    Pcell2 = new PdfPCell(new Phrase("	       Month", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    Pcell2.Colspan = 2;
                    transporter.AddCell(Pcell2);


                    Pcell2 = new PdfPCell(new Phrase("	       Day", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    Pcell2.Colspan = 2;
                    transporter.AddCell(Pcell2);


                    Pcell2 = new PdfPCell(new Phrase("	      Year", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    Pcell2.Colspan = 4;
                    transporter.AddCell(Pcell2);


                    Pcell2 = new PdfPCell(new Phrase("                      ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    transporter.AddCell(Pcell2);

                    Pcell2 = new PdfPCell(new Phrase("	       ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    transporter.AddCell(Pcell2);

                    Pcell2 = new PdfPCell(new Phrase("	       ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    transporter.AddCell(Pcell2);


                    Pcell2 = new PdfPCell(new Phrase("	      ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    transporter.AddCell(Pcell2);


                    Pcell2 = new PdfPCell(new Phrase("	      ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    transporter.AddCell(Pcell2);


                    Pcell2 = new PdfPCell(new Phrase("	       ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    transporter.AddCell(Pcell2);


                    Pcell2 = new PdfPCell(new Phrase("	       ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    transporter.AddCell(Pcell2);

                    Pcell2 = new PdfPCell(new Phrase("	       ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    transporter.AddCell(Pcell2);

                    Pcell2 = new PdfPCell(new Phrase("	       ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));

                    transporter.AddCell(Pcell2);
                    pdfDoc.Add(transporter);



                    PdfPTable receipt = new PdfPTable(10);
                    receipt.TotalWidth = 555f;
                    receipt.LockedWidth = true;
                    receipt.SetWidths(new float[] { 1f, 9.5f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f, 1f, 1f, 1f });


                    PdfPCell Pcell3 = new PdfPCell(new Phrase("16.  ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    Pcell3.Rowspan = 3;
                    receipt.AddCell(Pcell3);



                    Pcell3 = new PdfPCell(new Phrase("Receiver’s Certificate for receipt to Hazardous  &    Other  waste", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    receipt.AddCell(Pcell3);


                    Pcell3 = new PdfPCell(new Phrase("	", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    Pcell3.Colspan = 8;
                    receipt.AddCell(Pcell3);


                    Pcell3 = new PdfPCell(new Phrase("Name & Stamp                                  Signature                  ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    receipt.AddCell(Pcell3);


                    Pcell3 = new PdfPCell(new Phrase("        Month", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    Pcell3.Colspan = 2;
                    receipt.AddCell(Pcell3);


                    Pcell3 = new PdfPCell(new Phrase("         Day", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    Pcell3.Colspan = 2;
                    receipt.AddCell(Pcell3);


                    Pcell3 = new PdfPCell(new Phrase("         Year", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    Pcell3.Colspan = 4;
                    receipt.AddCell(Pcell3);


                    Pcell3 = new PdfPCell(new Phrase("                      ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    receipt.AddCell(Pcell3);

                    Pcell3 = new PdfPCell(new Phrase("	       ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    receipt.AddCell(Pcell3);


                    Pcell3 = new PdfPCell(new Phrase("	       ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    receipt.AddCell(Pcell3);


                    Pcell3 = new PdfPCell(new Phrase("	      ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    receipt.AddCell(Pcell3);


                    Pcell3 = new PdfPCell(new Phrase("	      ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    receipt.AddCell(Pcell3);


                    Pcell3 = new PdfPCell(new Phrase("	       ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    receipt.AddCell(Pcell3);


                    Pcell3 = new PdfPCell(new Phrase("	       ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    receipt.AddCell(Pcell3);

                    Pcell3 = new PdfPCell(new Phrase("	       ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    receipt.AddCell(Pcell3);

                    Pcell3 = new PdfPCell(new Phrase("	       ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)));
                    receipt.AddCell(Pcell3);
                    pdfDoc.Add(receipt);

                    String cpy1 = String.Format("  ".PadRight(10) + "Copy1 (White): To be forwarded to the State Pollution Control Board after signing all the seven copies ");
                    pdfDoc.Add(new iTextSharp.text.Paragraph(cpy1, titleFontnormal));

                    String cpy2 = String.Format("  ".PadRight(10) + "Copy2 (Yellow): To be retained by the sender after taking signature on it from the transporter and the rest of the five signed copies to be carried by the  ".ToString().PadRight(166) + "transporter ");
                    pdfDoc.Add(new iTextSharp.text.Paragraph(cpy2, titleFontnormal));

                    String cpy3 = String.Format("  ".PadRight(10) + "Copy 3 (Pink): to be retained by the receiver (actual use or treatment storage and disposal facility operator) after receiving the waste and remaining ".ToString().PadRight(167) + "four copies are to be duly signed by the receiver. ");
                    pdfDoc.Add(new iTextSharp.text.Paragraph(cpy3, titleFontnormal));

                    String cpy4 = String.Format("  ".PadRight(10) + "Copy 4 (Orange): To be handed over to the transporter by the receiver after accepting waste ");
                    pdfDoc.Add(new iTextSharp.text.Paragraph(cpy4, titleFontnormal));

                    String cpy5 = String.Format("  ".PadRight(10) + "Copy 5 (Green): To be sent by the receiver to the State Pollution Control Board. ");
                    pdfDoc.Add(new iTextSharp.text.Paragraph(cpy5, titleFontnormal));

                    String cpy6 = String.Format("  ".PadRight(10) + "Copy 6 (Blue): To be sent by the receiver to the sender ");
                    pdfDoc.Add(new iTextSharp.text.Paragraph(cpy6, titleFontnormal));

                    String cpy7 = String.Format("  ".PadRight(10) + "Copy 7 (Grey): To be sent by the receiver to the State Pollution Control Board in case the sender is in another State. ");
                    pdfDoc.Add(new iTextSharp.text.Paragraph(cpy7, titleFontnormal));


                    pdfDoc.Close();
                    Response.Write(pdfDoc);
                    Response.End();

                }
            }
            catch (Exception objException)
            {
                // LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);
            }
            return View();
        }


        [HttpGet]
        [AllowAnonymous]

        public ActionResult EditTemplate()
        {
            ComplianceList comp = null;
            comp = ComplianceList();
            comp.UserId = CurrentUser.UserID;
            comp.Roles = CurrentUser.Roles;
            comp.UserFullName = CurrentUser.FullName;
            comp.ProfileImage = CurrentUser.ProfileImage;
            comp.IsRestrict = CurrentUser.IsRestrict;
            return View(comp);

        }
        public ComplianceList ComplianceList()
        {
            ComplianceList comp = new ComplianceList();
            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "WasteSelect";
                objCom.CommandType = CommandType.StoredProcedure;
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();

                var compliancelist = new List<WasteList>();

                while (objReader.Read())
                {
                    var waste = new WasteList();

                    waste.WasteId = int.Parse(objReader["WasteId"].ToString());

                    waste.WasteNo = int.Parse(objReader["WasteNo"].ToString());
                    waste.WasteTagId = objReader["WasteTagId"].ToString();
                    waste.WasteName = objReader["WasteName"].ToString();
                    waste.UoM = objReader["UoM"].ToString();
                    compliancelist.Add(waste);
                }
                objCon.Close();

                comp = new ComplianceList();
                comp.Waste = compliancelist;
                return comp;
            }

        }


        public int EditWasteList(WasteList waste)
        {
            int recordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "WasteUpdate";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@WasteId", waste.WasteId);
                    objCom.Parameters.AddWithValue("@WasteNo", waste.WasteNo);
                    objCom.Parameters.AddWithValue("@WasteTagId", waste.WasteTagId);
                    objCom.Parameters.AddWithValue("@WasteName", waste.WasteName);
                    objCom.Parameters.AddWithValue("@UoM", waste.UoM);

                    objCon.Open();
                    objCom.Connection = objCon;
                    objCom.ExecuteNonQuery();
                    objCon.Close();
                }
            }
            catch (Exception exception)
            {

                if (exception.Message == "WasteNo already exists.")
                    return -1;
                if (exception.Message == "WasteTagId already exists.")
                    return -2;
                if (exception.Message == "WasteName already exists.")
                    return -3;
                else
                    return 0;
            }
            return recordCount;
        }
        public int WasteAdd(WasteList waste)

        {
            int recordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                {

                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "WasteInsert";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@WasteNo", waste.WasteNo);

                    objCom.Parameters.AddWithValue("@WasteTagId", waste.WasteTagId);
                    objCom.Parameters.AddWithValue("@WasteName", waste.WasteName);
                    objCom.Parameters.AddWithValue("@UoM", waste.UoM);

                    objCon.Open();
                    objCom.Connection = objCon;
                    objCom.ExecuteNonQuery();
                    objCon.Close();
                }
            }
            catch (Exception exception)
            {

                if (exception.Message == "WasteNo already exists.")
                    return -1;
                if (exception.Message == "WasteTagId already exists.")
                    return -2;
                if (exception.Message == "WasteName already exists.")
                    return -3;
                else
                    return 0;
            }
            return recordCount;
        }

        [HttpGet]
        public ActionResult CreateComplianceForm()
        {
            var createcompliance = new CreateComplianceViewModel();
            createcompliance.ComplianceDate= DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            createcompliance.Contract = GetReceiver();
            createcompliance.Vechicle = GetVechicle();
            createcompliance.Form = GetPhyicalForm();
            createcompliance.Waste = GetWasteList();
            createcompliance.ApproverList = SenderApproverName();
            createcompliance.Roles = CurrentUser.Roles;
            createcompliance.UserFullName = CurrentUser.FullName;
            createcompliance.ProfileImage = CurrentUser.ProfileImage;
            createcompliance.IsRestrict = CurrentUser.IsRestrict;

            return View(createcompliance);
        }

        [HttpPost]
        public ActionResult CreateComplianceForm(CreateComplianceViewModel createcompliance, List<WasteList> Waste)
        {

            string WasteQuantityString = string.Empty;

            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                int affectedRecordCount = 0;
                SqlCommand objCom = new SqlCommand();
                objCom.CommandText = "ComplianceSave";
                objCom.CommandType = CommandType.StoredProcedure;

                objCom.Parameters.AddWithValue("@TransporterName", createcompliance.TransporterName);
                objCom.Parameters.AddWithValue("@TypeOfVechicle", createcompliance.VechicleID);
                objCom.Parameters.AddWithValue("@VechicleRegistrationNumber", createcompliance.VechicleRegistrationNumber);

                objCom.Parameters.AddWithValue("@ReceiverName", createcompliance.ReceiverName);
                objCom.Parameters.AddWithValue("@PhysicalForm", createcompliance.PhysicalFormId);
                objCom.Parameters.AddWithValue("@SenderCertificate", createcompliance.UserID);
                objCom.Parameters.AddWithValue("@ComplianceDate", DateTime.ParseExact(createcompliance.ComplianceDate, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture));

                var complianceID = new SqlParameter();
                complianceID.ParameterName = "@ComplianceID";
                complianceID.Direction = ParameterDirection.Output;
                complianceID.Size = int.MaxValue;
                objCom.Parameters.Add(complianceID);
                objCon.Open();
                objCom.Connection = objCon;
                objCom.ExecuteNonQuery();

                objCom.Parameters.Clear();

                List<WasteQuantity> wastequantity = new List<WasteQuantity>();

                foreach (var wasteq in Waste)
                {
                    if (wasteq.Quantity != null)
                    {
                        var waste1 = new WasteQuantity
                        {
                            WasteId = wasteq.WasteId,
                            WasteName = wasteq.WasteName,
                            Quantity = wasteq.Quantity

                        };
                        wastequantity.Add(waste1);
                    }

                }
                XmlSerializer xmlSerializer = new XmlSerializer(wastequantity.GetType());

                using (StringWriter textWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(textWriter, wastequantity);

                    WasteQuantityString = textWriter.ToString();
                }

                objCom.CommandText = "WasteQuantity";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@ComplianceFormID", complianceID.Value);
                objCom.Parameters.AddWithValue("@WasteMeasure", WasteQuantityString);
                objCom.Connection = objCon;
                affectedRecordCount = objCom.ExecuteNonQuery();
                objCom.Parameters.Clear();
                objCon.Close();

                return RedirectToAction("SavedComplianceForm");

            }

        }

        private List<Contract> GetReceiver()
        {
            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "ReceiverSelect";
                objCom.CommandType = CommandType.StoredProcedure;
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();
                var contractlist = new List<Contract>();

                while (objReader.Read())
                {
                    var ContractList = new Contract();
                    ContractList.ContractID = int.Parse(objReader["ContractorID"].ToString());
                    
                    ContractList.TransporterName = objReader["CompanyName"].ToString();
                    ContractList.ReceiverName= objReader["CompanyName"].ToString();
                    contractlist.Add(ContractList);
                }

                objCon.Close();
                return contractlist;
            }
        }

        private List<Vechicle> GetVechicle()
        {
            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "VechicleSelect";
                objCom.CommandType = CommandType.StoredProcedure;
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();
                var vechiclelist = new List<Vechicle>();

                while (objReader.Read())
                {
                    var VechicleList = new Vechicle();
                    VechicleList.VechicleId = int.Parse(objReader["VechicleId"].ToString());
                    VechicleList.VechicleType = objReader["VechicleType"].ToString();
                    vechiclelist.Add(VechicleList);
                }

                objCon.Close();

                return vechiclelist;
            }
        }

        private List<PhysicalForm> GetPhyicalForm()
        {
            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "physicalSelect";
                objCom.CommandType = CommandType.StoredProcedure;
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();
                var physicalform = new List<PhysicalForm>();

                while (objReader.Read())
                {
                    var Physicalform = new PhysicalForm();
                    Physicalform.PhysicalFormId = int.Parse(objReader["PhsyicalFormId"].ToString());
                    Physicalform.Type = objReader["Type"].ToString();
                    physicalform.Add(Physicalform);
                }

                objCon.Close();

                return physicalform;
            }
        }



        public List<WasteList> GetWasteList()
        {
            List<WasteList> wlist = new List<WasteList>();

            using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
            {
                SqlCommand objCom = new SqlCommand();
                objCom.CommandText = "GetWaste";
                objCom.CommandType = CommandType.StoredProcedure;

                objCon.Open();
                objCom.Connection = objCon;
                SqlDataReader objReader = objCom.ExecuteReader();

                int recordCount = 1;
                while (objReader.Read())
                {
                    WasteList wastelist = new WasteList();

                    wastelist.WasteId = int.Parse(objReader["WasteId"].ToString());
                    wastelist.WasteName = objReader["WasteName"].ToString();
                    wastelist.UoM = objReader["UoM"].ToString();
                    wlist.Add(wastelist);

                    recordCount++;
                }
            }
            return wlist;
        }

        public List<UserProfile> SenderApproverName()
        {
            using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
            {
                SqlCommand objCom = new SqlCommand();
                objCom.CommandText = "SenderApproverName";
                objCom.CommandType = CommandType.StoredProcedure;
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();
                var sender = new List<UserProfile>();
                while (objReader.Read())
                {
                    sender.Add(new UserProfile
                    {

                        UserID = int.Parse(objReader["UserID"].ToString()),

                        DisplayUserName = objReader["UserFullName"].ToString(),

                    });

                }
                objCon.Close();
                return sender;
            }
        }

        public ActionResult SavedComplianceForm()
        {
            Console.WriteLine("yesffr");
            SavedCompliance savedcompliance = null;

            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "SavedComplianceList";
                objCom.CommandType = CommandType.StoredProcedure;

                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();
                var savedlist = new List<CreateComplianceViewModel>();
                int recortCount = 1;
                while (objReader.Read())
                {
                    var createcompliance = new CreateComplianceViewModel();
                    createcompliance.SNO = recortCount++;
                    createcompliance.CreatedOn = objReader["ComplianceDate"].ToString();
                    createcompliance.ComplianceFormID = int.Parse(objReader["ComplianceFormID"].ToString());
                    createcompliance.ReceiverName = objReader["CompanyName"].ToString();
                    createcompliance.WasteName = objReader["WasteName"].ToString();
                    createcompliance.Quantity = objReader["Quantity"].ToString();
                    createcompliance.SenderName = objReader["SenderName"].ToString();

                    savedlist.Add(createcompliance);
                }
                objCon.Close();

                savedcompliance = new SavedCompliance();
                savedcompliance.createcompliance = savedlist;
            }
            savedcompliance.Roles = CurrentUser.Roles;
            savedcompliance.UserFullName = CurrentUser.FullName;
            savedcompliance.ProfileImage = CurrentUser.ProfileImage;
            savedcompliance.IsRestrict = CurrentUser.IsRestrict;
            return View(savedcompliance);

        }
        [HttpPost]
        public ActionResult SavedComplianceForm(SavedCompliance savedlist)
        {
            {
                string SNO = Request["Sno"];

                var createcompliance = savedlist.createcompliance.Find(x => x.SNO == int.Parse(SNO));

                var Attachment = Request.Files[int.Parse(SNO) - 1] as HttpPostedFileBase;

                if (Attachment != null && Attachment.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(Attachment.FileName);
                    savedlist.Attachment = Attachment.FileName;
                    var path = Path.Combine(Server.MapPath("~/SavedComplianceList/"), fileName);
                    Attachment.SaveAs(path);
                }

                else
                {
                    ViewBag.comments = "N";
                    savedlist.Roles = CurrentUser.Roles;
                    savedlist.UserFullName = CurrentUser.FullName;
                    savedlist.ProfileImage = CurrentUser.ProfileImage;
                    savedlist.IsRestrict = CurrentUser.IsRestrict;
                    return View(savedlist);
                }

                UpdateComplianceStatus(createcompliance);

            }
           

            return RedirectToAction("SavedComplianceForm");
        }
        private void UpdateComplianceStatus(CreateComplianceViewModel savedlist)
        {
            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                string SNO = Request["Sno"];



                var Attachment = Request.Files[int.Parse(SNO) - 1] as HttpPostedFileBase;
                var fileName = Path.GetFileName(Attachment.FileName);
                if (Attachment != null && Attachment.ContentLength > 0)
                {

                    savedlist.Attachment = Attachment.FileName;
                    var path = Path.Combine(Server.MapPath("~/SavedComplianceList/"), fileName);
                    Attachment.SaveAs(path);
                }

                var objCom = new SqlCommand();
                objCom.CommandText = "ComplianceStatusUpdate";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@ComplianceformID", savedlist.ComplianceFormID);

                objCom.Parameters.AddWithValue("@Status", savedlist.Status);



                if (!string.IsNullOrEmpty(fileName))
                    objCom.Parameters.AddWithValue("@Attachment", fileName);
                else
                    objCom.Parameters.AddWithValue("@Attachment", DBNull.Value);

                objCon.Open();
                objCom.Connection = objCon;
                objCom.ExecuteNonQuery();


                objCon.Close();

            }
        }

        public ActionResult UpdateComplianceForm(int id)
        {
            var createcompliance = GetCompliance(id);
            createcompliance.Contract = GetReceiver();
            createcompliance.Vechicle = GetVechicle();
            createcompliance.Form = GetPhyicalForm();
            createcompliance.Waste = GetWasteList();
            createcompliance.ApproverList = SenderApproverName();
            createcompliance.Roles = CurrentUser.Roles;
            createcompliance.UserFullName = CurrentUser.FullName;
            createcompliance.ProfileImage = CurrentUser.ProfileImage;
            createcompliance.IsRestrict = CurrentUser.IsRestrict;
            return View(createcompliance);
        }

        public CreateComplianceViewModel GetCompliance(int complianceFormID)
        {
            CreateComplianceViewModel createcompliance = null;
            List<WasteQuantity> wlist = new List<WasteQuantity>();
            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "[ComplianceGet]";
                objCom.Parameters.AddWithValue("@ComplianceFormID", complianceFormID);
                objCom.CommandType = CommandType.StoredProcedure;
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();

                createcompliance = new CreateComplianceViewModel();
                if (objReader.Read())
                {
                    createcompliance.ComplianceFormID = int.Parse(objReader["ComplianceFormID"].ToString());
                    createcompliance.TransporterName = objReader["TransporterName"].ToString();
                    createcompliance.VechicleID = int.Parse(objReader["TypeOfvechicle"].ToString());
                    createcompliance.VechicleRegistrationNumber = objReader["VechicleRegistrationNumber"].ToString();
                    createcompliance.ReceiverName = objReader["ReceiverName"].ToString();
                    createcompliance.PhysicalFormId = int.Parse(objReader["PhysicalForm"].ToString());
                    createcompliance.SenderName = objReader["SenderCertificate"].ToString();
                    createcompliance.ComplianceDate = objReader["ComplianceDate"].ToString(); 

                    int recordcount = 1;
                    if (objReader.NextResult())
                    {
                        while (objReader.Read())
                        {
                            WasteQuantity wastequantity = new WasteQuantity();
                            createcompliance.ComplianceFormID = int.Parse(objReader["ComplianceFormID"].ToString());
                            wastequantity.WasteId = int.Parse(objReader["WasteId"].ToString());
                            wastequantity.WasteName = objReader["WasteName"].ToString();
                            wastequantity.Quantity = objReader["Quantity"].ToString();
                            wlist.Add(wastequantity);
                            recordcount++;
                            createcompliance.wastequantity = wlist;

                        }
                        objReader.Close();
                    }
                    objCon.Close();
                }

                return createcompliance;
            }

        }
        [HttpPost]
        public ActionResult UpdateComplianceForm(CreateComplianceViewModel createcompliance, List<WasteQuantity> wastequantity)
        {
            string WasteQuantityString = string.Empty;

            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                int affectedRecordCount = 0;
                SqlCommand objCom = new SqlCommand();
                objCom.CommandText = "ComplianceUpdate";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@ComplianceFormID", createcompliance.ComplianceFormID);
                objCom.Parameters.AddWithValue("@TransporterName", createcompliance.TransporterName);
                objCom.Parameters.AddWithValue("@TypeOfVechicle", createcompliance.VechicleID);
                objCom.Parameters.AddWithValue("@VechicleRegistrationNumber", createcompliance.VechicleRegistrationNumber);

                objCom.Parameters.AddWithValue("@ReceiverName", createcompliance.ReceiverName);
                objCom.Parameters.AddWithValue("@PhysicalForm", createcompliance.PhysicalFormId);
                objCom.Parameters.AddWithValue("@SenderCertificate", createcompliance.SenderName);
                objCom.Parameters.AddWithValue("@Status", createcompliance.Status);
                objCom.Parameters.AddWithValue("@ComplianceDate", DateTime.ParseExact(createcompliance.ComplianceDate, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture));
                objCon.Open();
                objCom.Connection = objCon;
                objCom.ExecuteNonQuery();

                objCom.Parameters.Clear();




                XmlSerializer xmlSerializer = new XmlSerializer(wastequantity.GetType());

                using (StringWriter textWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(textWriter, wastequantity);

                    WasteQuantityString = textWriter.ToString();
                }

                objCom.CommandText = "WasteQuantityUpdate";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@ComplianceFormID", createcompliance.ComplianceFormID);
                objCom.Parameters.AddWithValue("@WasteMeasure", WasteQuantityString);
                objCom.Connection = objCon;
                affectedRecordCount = objCom.ExecuteNonQuery();
                objCom.Parameters.Clear();
                objCon.Close();


            }
           

            return RedirectToAction("SavedComplianceForm");
        }

        [HttpGet]
        public ActionResult SavedComplianceHistory(int? page)
        {

            SavedCompliance history = new SavedCompliance();
            List<CreateComplianceViewModel> createcompliance = new List<CreateComplianceViewModel>();
            List<DataRow> datarow = new List<DataRow>();
            List<DataColumn> datacolumn = new List<DataColumn>();
            history.Roles = CurrentUser.Roles;
            history.UserFullName = CurrentUser.FullName;
            history.ProfileImage = CurrentUser.ProfileImage;


            try
            {
                history.FromDate = DateTime.Now.ToString("dd/MM/yyyy 00:00");
                history.ToDate = DateTime.Now.ToString("dd/MM/yyyy 23:59");

                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[ComplianceHistory]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@fromdate", history.FromDate);
                    objCom.Parameters.AddWithValue("@Todate", history.ToDate);

                    objCon.Open();
                    objCom.Connection = objCon;
                    SqlDataAdapter Adapter = new SqlDataAdapter();
                    Adapter.SelectCommand = objCom;
                    DataSet dataSet = new DataSet();
                    Adapter.Fill(dataSet);

                    ViewBag.IsRecordFound = false;
                    var objReader = objCom.ExecuteReader();

                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.IsRecordFound = true;

                        foreach (DataColumn dc in dataSet.Tables[0].Columns)
                        {
                            datacolumn.Add(dc);
                        }
                        foreach (DataRow dr in dataSet.Tables[0].Rows)
                        {

                            datarow.Add(dr);

                        }
                    }


                    objCon.Close();
                    int pageSize = 10;
                    int PageNumber = 1;
                    if (page != null)
                    {
                        PageNumber = Convert.ToInt16(page);
                    }
                    

                    history.HistoryPages = createcompliance.ToPagedList(PageNumber, pageSize);
                  



                    history.Datacolumn = datacolumn;
                    history.Datarow = datarow;
                    history.createcompliance = createcompliance;

                    ViewBag.FromDate = history.FromDate;
                    ViewBag.EndDate = history.ToDate;

                }
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
            }
            return View(history);
        }
        [HttpPost]
        public ActionResult SavedComplianceHistory(SavedCompliance history)
        {
            List<DataRow> datarow = new List<DataRow>();
            List<DataColumn> datacolumn = new List<DataColumn>();
            List<CreateComplianceViewModel> createcompliance = new List<CreateComplianceViewModel>();


            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    ViewBag.FromDate = history.FromDate;
                    ViewBag.EndDate = history.ToDate;


                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "ComplianceHistory";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@fromdate", history.FromDate);
                    objCom.Parameters.AddWithValue("@Todate", history.ToDate);

                    objCon.Open();
                    objCom.Connection = objCon;
                    SqlDataAdapter Adapter = new SqlDataAdapter();
                    Adapter.SelectCommand = objCom;
                    DataSet dataSet = new DataSet();
                    Adapter.Fill(dataSet);

                    ViewBag.IsRecordFound = false;
                    var objReader = objCom.ExecuteReader();



                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.IsRecordFound = true;

                        foreach (DataColumn dc in dataSet.Tables[0].Columns)
                        {
                            datacolumn.Add(dc);
                        }
                        foreach (DataRow dr in dataSet.Tables[0].Rows)
                        {
                            datarow.Add(dr);
                      
                        }
                    }

                    objCon.Close();
                    int pageSize = 10;
                    int PageNumber = 1;
                   

                     history.HistoryPages = createcompliance.ToPagedList(PageNumber, pageSize);

                    history.Roles = CurrentUser.Roles;
                    history.UserFullName = CurrentUser.FullName;
                    history.ProfileImage = CurrentUser.ProfileImage;
                    history.IsRestrict = CurrentUser.IsRestrict;
                    history.Datacolumn = datacolumn;
                    history.Datarow = datarow;
                    history.createcompliance = createcompliance;
                }

            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
            }
            return View(history);
        }

        #region EXPORTTOEXCEL

        public ActionResult ComplianceSaveToExcel(string currentFromDate, string currentEndDate)
        {

            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "ComplianceHistory";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@Fromdate", currentFromDate);
                objCom.Parameters.AddWithValue("@Todate", currentEndDate);

                objCon.Open();
                objCom.Connection = objCon;
                SqlDataAdapter Adapter = new SqlDataAdapter();
                Adapter.SelectCommand = objCom;
                DataSet dataSet = new DataSet();
                Adapter.Fill(dataSet);
                objCon.Close();
                var wb = new XLWorkbook(Server.MapPath("~/Template/ClosedComplianceHistory.xlsx"));
                var worksheet = wb.Worksheet(1);
                worksheet.Cell("C5").Value = "Report Duration : " + currentFromDate + " to  " + currentEndDate;
                if (dataSet.Tables[0].Rows.Count > 0)
                {

                    var rangeWithDataSecond = worksheet.Cell(7, 1).InsertTable(dataSet.Tables[0].AsEnumerable());
                }
                else
                {
                    worksheet.Cell("C8").Value = "No Data Found";
                }

                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename= ClosedComplianceHistory.xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

            }


            return View();


        }
        #endregion
    }
}