using System.IO;
using System.Text;
using OxyPlot;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace LangLang.Services
{
    public abstract class ReportService
    {
        public abstract void GenerateReport();
        protected static void SaveToPdf(PlotModel plotModel, string filePath)
        {
            using var stream = File.Create(filePath);
            var pdfExporter = new PdfExporter { Width = 600, Height = 400 };
            pdfExporter.Export(plotModel, stream);
        }

        protected static void MergePdf(string outputFilePath, string[] inputFilePaths)
        {
            PdfDocument outputPdfDocument = new PdfDocument();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            foreach (string filePath in inputFilePaths)
            {
                PdfDocument inputPdfDocument = PdfReader.Open(filePath, PdfDocumentOpenMode.Import);
                outputPdfDocument.Version = inputPdfDocument.Version;
                foreach (PdfPage page in inputPdfDocument.Pages)
                {
                    outputPdfDocument.AddPage(page);
                }
            }

            outputPdfDocument.Save(outputFilePath);

            foreach (string filePath in inputFilePaths)
            {
                File.Delete(filePath);
            }
        }

    }
}
