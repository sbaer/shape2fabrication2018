using System;
using Rhino;
using Rhino.Commands;

namespace NewDevDaySamples
{
  public class DevDaySamplePDF : Command
  {
    public override string EnglishName => "DevDaySamplePDF";

    protected override Result RunCommand(RhinoDoc doc, RunMode mode)
    {
      var pdf = Rhino.FileIO.FilePdf.Create();

      Xfinium.Pdf.PdfFixedDocument pdfDoc = pdf.PdfDocumentImplementation() as Xfinium.Pdf.PdfFixedDocument;
      var pdfPage = pdfDoc.Pages.Add();
      using (var font = new System.Drawing.Font("Arial", 30))
      {
        var pdffont = new Xfinium.Pdf.Graphics.PdfUnicodeTrueTypeFont(font, false);
        var color = new Xfinium.Pdf.Graphics.PdfRgbColor(0, 0, 0);
        var appearance = new Xfinium.Pdf.Graphics.PdfStringAppearanceOptions(pdffont, null, new Xfinium.Pdf.Graphics.PdfBrush(color));
        var layout = new Xfinium.Pdf.Graphics.PdfStringLayoutOptions();
        layout.HorizontalAlign = Xfinium.Pdf.Graphics.PdfStringHorizontalAlign.Center;
        layout.Rotation = 45;
        layout.X = pdfPage.Width / 2;
        layout.Y = pdfPage.Height / 2;
        pdfPage.Graphics.DrawString("New Development Day 2018", appearance, layout);
      }

      const int w = (int)(8.5 * 300);
      const int h = 11 * 300;
      var viewcapture = new Rhino.Display.ViewCaptureSettings(doc.Views.ActiveView, new System.Drawing.Size(w,h), 300);
      pdf.AddPage(viewcapture);

      string path = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
      path = System.IO.Path.Combine(path, "DevDaySamplePDF.pdf");
      pdf.Write(path);
      return Result.Success;
    }
  }
}
