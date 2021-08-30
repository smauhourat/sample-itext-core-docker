using iText.Forms;
using iText.Forms.Fields;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.IO;

namespace sample2
{
    class Program
    {
        public const String src = "../../../resources/job_application.pdf";
        public const String dest = "../../../results/edited_job_application.pdf";
        
        public const String src2 = "../../../resources/ProductQuoteTemplate_Inquimex_es.pdf";
        public const String dest2 = "../../../results/edited_ProductQuoteTemplate_Inquimex_es.pdf";

        public const String src3 = "../../../resources/ufo.pdf";
        public const String dest3 = "../../../results/add_content.pdf";


        public const String src41 = "ProductQuoteTemplate_Inquimex_es.pdf";
        public const String src42 = "ufo.pdf";
        public const String dest4 = "merged_content.pdf";


        static void Main(string[] args)
        {
            Console.WriteLine("Entrando al Main....");

            //FileInfo file = new FileInfo(dest);
            //file.Directory.Create();
            //new C05E01_AddAnnotationsAndContent().ManipulatePdf(src, dest);

            //FileInfo file = new FileInfo(dest2);
            //file.Directory.Create();
            //new C05E01_AddAnnotationsAndContent().StampPdf(src2, dest2);

            //FileInfo file = new FileInfo(dest3);
            //file.Directory.Create();
            //new C05E01_AddAnnotationsAndContent().AddContent(src3, dest3);

            //new C05E01_AddAnnotationsAndContent().MergePdf(src41, src42, dest4);

            string currentDir = System.IO.Directory.GetCurrentDirectory();

            var obj = new C05E01_AddAnnotationsAndContent();
            Console.WriteLine("Archivos ANTES.....");
            obj.GetAllFiles(currentDir);
            Console.WriteLine("Arrancando la funcion MergePdf....");
            obj.MergePdf(src41, src42, dest4);
            Console.WriteLine("Archivos DESPUES.....");
            obj.GetAllFiles(currentDir);

            Console.WriteLine("Saliendo del Main....");
        }
    }

    internal class C05E01_AddAnnotationsAndContent
    {
        internal void GetAllFiles(string directoryPath)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(directoryPath);
            FileInfo[] files = dirInfo.GetFiles();
            foreach (FileInfo f in files)
            {
                Console.WriteLine(f.FullName);
            }
        }

        internal void ManipulatePdf(string src, string dest)
        {
            //Initialize PDF document
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(src), new PdfWriter(dest));

            //Add text annotation
            PdfAnnotation ann = new PdfTextAnnotation(new Rectangle(400, 795, 0, 0))
                .SetOpen(true)
                .SetTitle(new PdfString("iText"))
                .SetContents("Please, fill out the form.");

            pdfDoc.GetFirstPage().AddAnnotation(ann);
            PdfCanvas canvas = new PdfCanvas(pdfDoc.GetFirstPage());
            canvas.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 12).MoveText(265, 597).ShowText("I agree to the terms and conditions.").EndText();

            //Add form field
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfButtonFormField checkField = PdfFormField.CreateCheckBox(pdfDoc, new Rectangle(245, 594, 15, 15), "agreement", "Off", PdfFormField.TYPE_CHECK);
            checkField.SetRequired(true);
            form.AddField(checkField);

            //Update reset button
            form.GetField("reset").SetAction(PdfAction.CreateResetForm(new String[] { "name", "language", "experience1", "experience2", "experience3", "shift", "info", "agreement" }, 0));
            pdfDoc.Close();
        }

        internal void StampPdf(string src, string dest)
        {
            //Initialize PDF document
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(src), new PdfWriter(dest));

            PdfCanvas canvas = new PdfCanvas(pdfDoc.GetFirstPage());
            canvas.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 12).MoveText(265, 597).ShowText("HOLAAAAAAA").EndText();

            pdfDoc.Close();
        }

        internal void AddContent(string src, string dest)
        {
            //Initialize PDF document
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(src), new PdfWriter(dest));
            Document document = new Document(pdfDoc);
            Rectangle pageSize;
            PdfCanvas canvas;
            int n = pdfDoc.GetNumberOfPages();
            for (int i = 1; i <= n; i++)
            {
                PdfPage page = pdfDoc.GetPage(i);
                pageSize = page.GetPageSize();
                canvas = new PdfCanvas(page);
                
                //Draw header text
                canvas.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 7).MoveText(pageSize
                    .GetWidth() / 2 - 24, pageSize.GetHeight() - 10).ShowText("I want to believe").EndText();
                
                //Draw footer line
                canvas.SetStrokeColor(ColorConstants.BLACK).SetLineWidth(.2f).MoveTo(pageSize.GetWidth() / 2 - 30, 20).LineTo(pageSize
                    .GetWidth() / 2 + 30, 20).Stroke();
                
                //Draw page number
                canvas.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 7).MoveText(pageSize
                    .GetWidth() / 2 - 7, 10).ShowText(i.ToString()).ShowText(" of ").ShowText(n.ToString()).EndText();

                //Draw watermark
                Paragraph p = new Paragraph("CONFIDENTIAL").SetFontSize(60);
                canvas.SaveState();
                
                PdfExtGState gs1 = new PdfExtGState().SetFillOpacity(0.2f);
                canvas.SetExtGState(gs1);
                
                document.ShowTextAligned(p, pageSize.GetWidth() / 2, pageSize.GetHeight() / 2, pdfDoc.GetPageNumber(page),
                    TextAlignment.CENTER, VerticalAlignment.MIDDLE, 45);
                canvas.RestoreState();
            }
            pdfDoc.Close();
        }

        internal void MergePdf(string src1, string src2, string dest)
        {
            FileInfo file = new FileInfo(dest);
            file.Directory.Create();

            PdfDocument pdf = new PdfDocument(new PdfWriter(dest));
            PdfMerger merger = new PdfMerger(pdf);

            //Add pages from the first document
            PdfDocument firstSourcePdf = new PdfDocument(new PdfReader(src1));
            merger.Merge(firstSourcePdf, 1, firstSourcePdf.GetNumberOfPages());

            //Add pages from the second pdf document
            PdfDocument secondSourcePdf = new PdfDocument(new PdfReader(src2));
            merger.Merge(secondSourcePdf, 1, secondSourcePdf.GetNumberOfPages());

            firstSourcePdf.Close();
            secondSourcePdf.Close();
            pdf.Close();
        }

    }
}
