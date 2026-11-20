using Elogbook.Core.Models.Response;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;

namespace Elogbook.Core.Helpers
{
    /** Inner class to add a header and a footer. */
    public class HeaderFooterManipulation : PdfPageEventHelper
    {
        private string _ticketNumber = string.Empty;
        private string _company = string.Empty;
        private string _approvedUnit = string.Empty;
        readonly TicketDetailModel _ticketDetail;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ticketDetail"></param>
        /// <param name="companyName"></param>
        /// <param name="factoryName"></param>
        public HeaderFooterManipulation(TicketDetailModel ticketDetail, string companyName, string factoryName)
        {
            _ticketDetail = ticketDetail;
            if (!string.IsNullOrWhiteSpace(companyName))
            {
                _ticketDetail.Company.Label = companyName;
            }
            if (!string.IsNullOrWhiteSpace(factoryName))
            {
                _ticketDetail.ApprovedUnit.Label = factoryName;
            }
            SetDataHeader();
        }

        private void SetDataHeader()
        {
            _ticketNumber = "Số phiếu: " + (!string.IsNullOrEmpty(_ticketDetail.TicketNumber) ? _ticketDetail.TicketNumber : ".................");
            _company = !string.IsNullOrEmpty(_ticketDetail.Company.Label) ? _ticketDetail.Company.Label.ToUpper() : ".................";
            _approvedUnit = !string.IsNullOrEmpty(_ticketDetail.ApprovedUnit.Label) ? _ticketDetail.ApprovedUnit.Label.ToUpper() : ".................";
        }

        /** Current page number (will be reset for every chapter). */
        int pagenumber;
        private readonly BaseFont _fNomal = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\times.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        private readonly BaseFont _fBold = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\timesbd.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        PdfTemplate pageNumberTemplate;
        PdfContentByte canvas;

        /**
         * Initialize one of the headers.
         * @see com.itextpdf.text.pdf.PdfPageEventHelper#onOpenDocument(
         *      com.itextpdf.text.pdf.PdfWriter, com.itextpdf.text.Document)
         */
        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            canvas = writer.DirectContent;
            pageNumberTemplate = canvas.CreateTemplate(50, 50);
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
            base.OnEndPage(writer, document);
            String text = "Trang số: " + writer.PageNumber + "/ ";
            canvas.BeginText();

            if (writer.PageNumber == 1)
            {
                canvas.SetFontAndSize(_fNomal, 11);
                canvas.SetTextMatrix(document.PageSize.GetRight(528), document.PageSize.GetTop(30));
                canvas.ShowText(_company);

                canvas.SetTextMatrix(document.PageSize.GetRight(520), document.PageSize.GetTop(50));
                canvas.ShowText(_approvedUnit);

                //canvas.SetFontAndSize(_fBold, 16);
                //canvas.SetTextMatrix(document.PageSize.GetRight(350), document.PageSize.GetTop(35));
                //canvas.ShowText("PHIẾU THAO TÁC");
            }
            canvas.SetFontAndSize(_fNomal, 13);
            canvas.SetTextMatrix(document.PageSize.GetRight(200), document.PageSize.GetTop(30));
            canvas.ShowText(_ticketNumber);
            canvas.SetTextMatrix(document.PageSize.GetRight(160), document.PageSize.GetTop(50));
            canvas.ShowText(text);
            canvas.EndText();
            float len = _fNomal.GetWidthPoint(text, 13);
            canvas.AddTemplate(pageNumberTemplate, document.PageSize.GetRight(160) + len, document.PageSize.GetTop(50));
        }

        /// <summary>
        /// OnCloseDocument
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="document"></param>
        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);

            pageNumberTemplate.BeginText();
            pageNumberTemplate.SetFontAndSize(_fNomal, 13);
            pageNumberTemplate.SetTextMatrix(0, 0);
            pageNumberTemplate.ShowText((writer.PageNumber - 1).ToString());
            pageNumberTemplate.EndText();
        }
    }
    // end inner class
}
