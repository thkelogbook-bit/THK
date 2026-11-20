using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Elogbook.Core.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public class BlankSignatureFieldEvent : IPdfPCellEvent
    {
        private readonly string _fieldName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        public BlankSignatureFieldEvent(string fieldName) { _fieldName = fieldName; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="position"></param>
        /// <param name="canvases"></param>
        public void CellLayout(PdfPCell cell, Rectangle position, PdfContentByte[] canvases)
        {
            PdfWriter writer = canvases[PdfPTable.BASECANVAS].PdfWriter;
            PdfFormField field = PdfFormField.CreateSignature(writer);
            field.FieldName = _fieldName;
            field.SetWidget(position, PdfAnnotation.HIGHLIGHT_NONE);
            field.Flags = PdfAnnotation.FLAGS_PRINT;
            // Dat mau nen trang, bo mau tim mac dinh cua signature field chua ky
            field.MKBackgroundColor = BaseColor.WHITE;
            field.MKBorderColor = BaseColor.WHITE;
            writer.AddAnnotation(field);
        }
    }
}
