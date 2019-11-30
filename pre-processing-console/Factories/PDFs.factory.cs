using System;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace pre_processing_console.factories
{
    public class PDFFactory
    {
        public PDFFactory()
        {

        }

        public string[] ListPDFsLocally(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new Exception(String.Concat("Ops, parece que esta pasta não existe...", path));
            }

            return Directory.GetFiles(path, "*.pdf", SearchOption.AllDirectories);
        }

        public string ExtractPDFullText(string pdfPath)
        {
            PdfReader reader = new PdfReader(pdfPath);
            StringWriter output = new StringWriter();
            output.WriteLine(PdfTextExtractor.GetTextFromPage(reader, 1, new SimpleTextExtractionStrategy()));

            return output.ToString();
        }
    }
}