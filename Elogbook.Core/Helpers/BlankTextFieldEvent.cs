using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;

namespace Elogbook.Core.Helpers
{
    /// <summary>
    /// Tao Text field de dien ten nguoi ky.
    /// Font Arial Unicode EMBEDDED duoc dung de hien thi dung tieng Viet
    /// ma khong phu thuoc vao font he thong cua viewer.
    /// </summary>
    public class BlankTextFieldEvent : IPdfPCellEvent
    {
        private readonly string _fieldName;
        private readonly BaseFont _font;
        private readonly float _fontSize;

        // Font Arial Unicode EMBEDDED - dung chung cho tat ca instance
        // Tranh tao lai BaseFont moi moi lan CellLayout duoc goi
        private static readonly string ArialFontPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Fonts", "arial.ttf");

        public BlankTextFieldEvent(string fieldName, BaseFont font, float fontSize)
        {
            _fieldName = fieldName;
            _font      = font;
            _fontSize  = fontSize;
        }

        public void CellLayout(PdfPCell cell, Rectangle position, PdfContentByte[] canvases)
        {
            PdfWriter writer = canvases[PdfPTable.BASECANVAS].PdfWriter;

            // Dung Arial EMBEDDED neu co the, fallback ve _font truyen vao
            BaseFont bf = _font;
            if (File.Exists(ArialFontPath))
            {
                try { bf = BaseFont.CreateFont(ArialFontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED); }
                catch { /* fallback */ }
            }

            TextField tf = new TextField(writer, position, _fieldName)
            {
                Font      = bf,
                FontSize  = _fontSize,
                Alignment = Element.ALIGN_CENTER,
                // Khong set READ_ONLY khi tao - SetNameField se set READ_ONLY sau khi dien ten
                // Neu set o day, SetField trong PdfStamper se bi block, field se trong
            };

            writer.AddAnnotation(tf.GetTextField());
        }
    }
}
