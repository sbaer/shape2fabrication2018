using System;
using Rhino;
using Rhino.Commands;

namespace NewDevDaySamples
{
    public class DevDaySamplePDF : Command
    {
        public override string EnglishName => "DevDaySample1_PDF";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            var pdf = Rhino.FileIO.FilePdf.Create();

            Xfinium.Pdf.PdfFixedDocument pdfDoc = pdf.PdfDocumentImplementation() as Xfinium.Pdf.PdfFixedDocument;
            var coverPage = pdfDoc.Pages.Add();
            using (var font = new System.Drawing.Font("Arial", 30))
            {
                var pdffont = new Xfinium.Pdf.Graphics.PdfUnicodeTrueTypeFont(font, false);
                pdffont.Underline = true;
                var color = new Xfinium.Pdf.Graphics.PdfRgbColor(0, 0, 0);
                var appearance = new Xfinium.Pdf.Graphics.PdfStringAppearanceOptions(pdffont, null, new Xfinium.Pdf.Graphics.PdfBrush(color));
                var layout = new Xfinium.Pdf.Graphics.PdfStringLayoutOptions();
                layout.HorizontalAlign = Xfinium.Pdf.Graphics.PdfStringHorizontalAlign.Center;
                layout.X = coverPage.Width / 2;
                layout.Y = coverPage.Height / 2;
                coverPage.Graphics.DrawString("New Development Day 2018", appearance, layout);
            }

            const int w = (int)(8.5 * 300);
            const int h = 11 * 300;
            var views = doc.Views.GetViewList(true, false);
            for (int i=0; i<views.Length; i++)
            {
                var view = views[i];
                var viewcapture = new Rhino.Display.ViewCaptureSettings(view, new System.Drawing.Size(w, h), 300);
                int pageIndex = pdf.AddPage(viewcapture) - 1;
                using (var font = new System.Drawing.Font("Arial", 80))
                {
                    var pdffont = new Xfinium.Pdf.Graphics.PdfUnicodeTrueTypeFont(font, false);
                    var color = new Xfinium.Pdf.Graphics.PdfRgbColor(40, 40, 40);
                    var pen = new Xfinium.Pdf.Graphics.PdfPen(color, 3);
                    var appearance = new Xfinium.Pdf.Graphics.PdfStringAppearanceOptions(pdffont, pen, null);

                    var layout = new Xfinium.Pdf.Graphics.PdfStringLayoutOptions();
                    layout.HorizontalAlign = Xfinium.Pdf.Graphics.PdfStringHorizontalAlign.Center;
                    layout.Rotation = 45;
                    layout.X = coverPage.Width / 2;
                    layout.Y = coverPage.Height / 2;
                    pdfDoc.Pages[pageIndex].Graphics.DrawString("Top Secret", appearance, layout);
                }
                using (var font = new System.Drawing.Font("Arial", 20))
                {
                    var pdffont = new Xfinium.Pdf.Graphics.PdfUnicodeTrueTypeFont(font, false);
                    var color = new Xfinium.Pdf.Graphics.PdfRgbColor(0, 0, 0);
                    var appearance = new Xfinium.Pdf.Graphics.PdfStringAppearanceOptions(pdffont, null, new Xfinium.Pdf.Graphics.PdfBrush(color));

                    var layout = new Xfinium.Pdf.Graphics.PdfStringLayoutOptions();
                    layout.HorizontalAlign = Xfinium.Pdf.Graphics.PdfStringHorizontalAlign.Right;
                    layout.X = coverPage.Width * 0.9;
                    layout.Y = coverPage.Height * 0.9;
                    pdfDoc.Pages[pageIndex].Graphics.DrawString($"Page {i+1} of {views.Length}", appearance, layout);
                }
            }
            string path = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            path = System.IO.Path.Combine(path, "DevDaySamplePDF.pdf");
            pdf.Write(path);
            return Result.Success;
        }
    }
}
