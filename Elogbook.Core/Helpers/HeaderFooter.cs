using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
namespace Elogbook.Core.Helpers
{
    /** Inner class to add a header and a footer. */
    public class HeaderFooter : PdfPageEventHelper
    {
        /** Current page number (will be reset for every chapter). */
        int pagenumber;
        private readonly BaseFont _fNomal = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\times.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

        /**
         * Initialize one of the headers.
         * @see com.itextpdf.text.pdf.PdfPageEventHelper#onOpenDocument(
         *      com.itextpdf.text.pdf.PdfWriter, com.itextpdf.text.Document)
         */
        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            // Method intentionally left empty.
        }

        /**
         * Initialize one of the headers, based on the chapter title;
         * reset the page number.
         * @see com.itextpdf.text.pdf.PdfPageEventHelper#onChapter(
         *      com.itextpdf.text.pdf.PdfWriter, com.itextpdf.text.Document, float,
         *      com.itextpdf.text.Paragraph)
         */
        public override void OnChapter(
          PdfWriter writer, Document document,
          float paragraphPosition, Paragraph title)
        {
            pagenumber = 1;
        }

        /**
         * Increase the page number.
         * @see com.itextpdf.text.pdf.PdfPageEventHelper#onStartPage(
         *      com.itextpdf.text.pdf.PdfWriter, com.itextpdf.text.Document)
         */
        public override void OnStartPage(PdfWriter writer, Document document)
        {
            pagenumber++;
        }

        /**
         * Adds the header and the footer.
         * @see com.itextpdf.text.pdf.PdfPageEventHelper#onEndPage(
         *      com.itextpdf.text.pdf.PdfWriter, com.itextpdf.text.Document)
         */
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            PdfContentByte canvas = writer.DirectContent;
            Font font = new Font(_fNomal, 10, -1, BaseColor.BLACK);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_RIGHT, new Phrase(String.Format("{0}", pagenumber), font), 545, 727, 0);
        }
    }
    // end inner class
}
