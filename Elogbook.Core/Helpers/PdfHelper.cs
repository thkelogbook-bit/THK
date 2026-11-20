using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using Image = iTextSharp.text.Image;

namespace Elogbook.Core.Helpers
{
    /// <summary>
    /// PdfHelper
    /// </summary>
    public static class PdfHelper
    {
        /// <summary>
        /// Create Cell Image Table
        /// </summary>
        /// <param name="ImageUrl"></param>
        /// <param name="colSpan"></param>
        /// <param name="rowSpan"></param>
        /// <param name="hAlignment"></param>
        /// <param name="vAlignment"></param>
        public static PdfPCell GetCellImageHeader(string ImageUrl, int colSpan, int rowSpan, int hAlignment, int vAlignment)
        {
            try
            {
                Image image = Image.GetInstance(AppSettings.FILE_STORAGE_SERVER_DIR + ImageUrl);
                image.ScalePercent(50);

                PdfPCell imageCell = new PdfPCell(image)
                {
                    Colspan = colSpan,
                    Rowspan = rowSpan,
                    BorderColor = BaseColor.GRAY,
                    HorizontalAlignment = hAlignment,
                    VerticalAlignment = vAlignment
                };

                return imageCell;
            }
            catch
            {
                //ignore error
                return null;
            }
        }

        /// <summary>
        /// Create Cell Image Logo
        /// </summary>
        /// <param name="ImageUrl"></param>
        /// <param name="colSpan"></param>
        /// <param name="rowSpan"></param>
        /// <param name="hAlignment"></param>
        /// <param name="vAlignment"></param>
        /// <param name="imgHeight"></param>
        /// <param name="isTSVH"></param>
        /// <returns></returns>
        public static PdfPCell GetCellImageLogo(string ImageUrl, int colSpan, int rowSpan, int hAlignment, int vAlignment, float imgHeight, bool isTSVH)
        {
            try
            {
                Image img = Image.GetInstance(AppSettings.FILE_STORAGE_SERVER_DIR + ImageUrl);

                float imgWidth = (img.Width * imgHeight) / img.Height;
                if(imgWidth > (imgHeight * 2))
                {
                    imgWidth = isTSVH ? (imgHeight * 2) + 20 : (imgHeight * 2) + 5;
                }
                img.ScaleAbsolute(imgWidth, imgHeight);

                PdfPCell imageCell = new PdfPCell(img)
                {
                    Colspan = colSpan,
                    Rowspan = rowSpan,
                    BorderColor = BaseColor.GRAY,
                    HorizontalAlignment = hAlignment,
                    VerticalAlignment = vAlignment
                };

                return imageCell;
            }
            catch
            {
                PdfPCell cell = new PdfPCell(new Phrase(" "))
                {
                    HorizontalAlignment = hAlignment,
                    VerticalAlignment = vAlignment,
                    Rowspan = rowSpan,
                    Colspan = colSpan,
                    FixedHeight = imgHeight
                };

                return cell;
            }
        }

        /// <summary>
        /// Create Image Signature
        /// </summary>
        /// <param name="base64"></param>
        /// <param name="colSpan"></param>
        /// <param name="rowSpan"></param>
        /// <param name="hAlignment"></param>
        /// <param name="vAlignment"></param>
        /// <param name="fixHeight"></param>
        /// <returns></returns>
        public static PdfPCell GetCellImageSignature(string base64, int colSpan, int rowSpan, int hAlignment, int vAlignment, int fixHeight = 21)
        {
            if (string.IsNullOrWhiteSpace(base64))
            {
                PdfPCell cell = new PdfPCell(new Phrase(" "))
                {
                    HorizontalAlignment = hAlignment,
                    VerticalAlignment = vAlignment,
                    Rowspan = rowSpan,
                    Colspan = colSpan,
                    FixedHeight = fixHeight,
                    BorderColor = BaseColor.GRAY
                };

                return cell;
            }

            base64 = base64.Remove(0, base64.IndexOf(',') + 1);
            byte[] imgBytes = Convert.FromBase64String(base64);

            Image jpg = Image.GetInstance(imgBytes);
            jpg.ScalePercent(70);

            PdfPCell imageCell = new PdfPCell(jpg)
            {
                Colspan = colSpan,
                Rowspan = rowSpan,
                BorderColor = BaseColor.GRAY,
                HorizontalAlignment = 1
            };

            return imageCell;
        }

        /// <summary>
        /// Insert Signature
        /// </summary>
        /// <param name="base64"></param>
        /// <param name="colSpan"></param>
        /// <param name="rowSpan"></param>
        /// <param name="borderWidthLeft"></param>
        /// <param name="borderWidthTop"></param>
        /// <param name="borderWidthRight"></param>
        /// <param name="borderWidthBottom"></param>
        /// <param name="hAlignment"></param>
        /// <param name="vAlignment"></param>
        /// <param name="fixHeight"></param>
        /// <returns></returns>
        public static PdfPCell GetCellImageSignatureSetBorder(string base64, int colSpan, int rowSpan, int borderWidthLeft, int borderWidthTop,
            int borderWidthRight, int borderWidthBottom, int hAlignment, int vAlignment, int fixHeight = 21)
        {
            if (string.IsNullOrWhiteSpace(base64))
            {
                PdfPCell cell = new PdfPCell(new Phrase(" "))
                {
                    HorizontalAlignment = hAlignment,
                    VerticalAlignment = vAlignment,
                    Rowspan = rowSpan,
                    Colspan = colSpan,
                    FixedHeight = fixHeight,
                    BackgroundColor = BaseColor.WHITE,
                    BorderColor = BaseColor.GRAY,
                    BorderWidthLeft = borderWidthLeft,
                    BorderWidthTop = borderWidthTop,
                    BorderWidthRight = borderWidthRight,
                    BorderWidthBottom = borderWidthBottom
                };

                return cell;
            }

            base64 = base64.Remove(0, base64.IndexOf(',') + 1);
            byte[] imgBytes = Convert.FromBase64String(base64);

            Image jpg = Image.GetInstance(imgBytes);
            if (jpg.Height > 900)
            {
                jpg.ScalePercent(5);
            }
            else if (jpg.Height > 500)
            {
                jpg.ScalePercent(10);
            }
            else if (jpg.Height > 200)
            {
                jpg.ScalePercent(15);
            }
            else
            {
                jpg.ScalePercent(20);
            }

            PdfPCell imageCell = new PdfPCell(jpg)
            {
                Colspan = colSpan,
                Rowspan = rowSpan,
                BorderColor = BaseColor.GRAY,
                HorizontalAlignment = 1,
                BorderWidthLeft = borderWidthLeft,
                BorderWidthTop = borderWidthTop,
                BorderWidthRight = borderWidthRight,
                BorderWidthBottom = borderWidthBottom,
                BackgroundColor = BaseColor.WHITE,
                PaddingTop = 3
            };

            return imageCell;
        }

        /// <summary>
        /// Create Cell Table
        /// </summary>
        /// <param name="fBase"></param>
        /// <param name="fSize"></param>
        /// <param name="textContent"></param>
        /// <param name="colSpan"></param>
        /// <param name="rowSpan"></param>
        /// <param name="hAlignment"></param>
        /// <param name="vAlignment"></param>
        /// <param name="minHeight"></param>
        public static PdfPCell GetCell(BaseFont fBase, float fSize, string textContent, int colSpan, int rowSpan, int hAlignment = 1, int vAlignment = 1, int minHeight = 16)
        {
            iTextSharp.text.Font font = new iTextSharp.text.Font(fBase, fSize,-1,BaseColor.BLACK);
            PdfPCell cell = new PdfPCell(new Phrase(textContent, font))
            {
                HorizontalAlignment = hAlignment,
                VerticalAlignment = vAlignment,
                Rowspan = rowSpan,
                Colspan = colSpan,
                MinimumHeight = minHeight,
                BorderColor = BaseColor.GRAY
            };

            return cell;
        }

        /// <summary>
        /// Create Cell Table
        /// </summary>
        /// <param name="fBase"></param>
        /// <param name="fSize"></param>
        /// <param name="textContent"></param>
        /// <param name="colSpan"></param>
        /// <param name="rowSpan"></param>
        /// <param name="hAlignment"></param>
        /// <param name="vAlignment"></param>
        /// <param name="fixedHeight"></param>
        public static PdfPCell GetCellFixedHeight(BaseFont fBase, float fSize, string textContent, int colSpan, int rowSpan, int hAlignment = 1, int vAlignment = 1, int fixedHeight = 21)
        {
            iTextSharp.text.Font font = new iTextSharp.text.Font(fBase, fSize);
            PdfPCell cell = new PdfPCell(new Phrase(textContent, font))
            {
                HorizontalAlignment = hAlignment,
                VerticalAlignment = vAlignment,
                Rowspan = rowSpan,
                Colspan = colSpan,
                FixedHeight = fixedHeight,
                BorderColor = BaseColor.GRAY
            };

            return cell;
        }

        /// <summary>
        /// Create cell
        /// </summary>
        /// <param name="fBase"></param>
        /// <param name="fSize"></param>
        /// <param name="textContent"></param>
        /// <param name="colSpan"></param>
        /// <param name="rowSpan"></param>
        /// <param name="borderWidthLeft"></param>
        /// <param name="borderWidthTop"></param>
        /// <param name="borderWidthRight"></param>
        /// <param name="borderWidthBottom"></param>
        /// <param name="hAlignment"></param>
        /// <param name="vAlignment"></param>
        /// <param name="fixedHeight"></param>
        /// <returns></returns>
        public static PdfPCell GetCellSetBorder(BaseFont fBase, float fSize, string textContent, int colSpan, int rowSpan, int borderWidthLeft, int borderWidthTop, int borderWidthRight, int borderWidthBottom
            , int hAlignment = 1, int vAlignment = 1, int fixedHeight = 21)
        {
            iTextSharp.text.Font font = new iTextSharp.text.Font(fBase, fSize);
            PdfPCell cell = new PdfPCell(new Phrase(textContent, font))
            {
                HorizontalAlignment = hAlignment,
                VerticalAlignment = vAlignment,
                Rowspan = rowSpan,
                Colspan = colSpan,
                FixedHeight = fixedHeight,
                BackgroundColor = BaseColor.WHITE,
                BorderColor = BaseColor.GRAY,
                BorderWidthLeft = borderWidthLeft,
                BorderWidthTop = borderWidthTop,
                BorderWidthRight = borderWidthRight,
                BorderWidthBottom = borderWidthBottom
            };

            return cell;
        }

        /// <summary>
        /// Tạo một ô (Cell) trống trong bảng PDF chứa một trường chữ ký điện tử (Signature Field) ẩn.
        /// Trường này sẽ được kích hoạt để người dùng ký số hoặc chèn ảnh chữ ký sau đó.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="borderWidthLeft"></param>
        /// <param name="borderWidthTop"></param>
        /// <param name="borderWidthRight"></param>
        /// <param name="borderWidthBottom"></param>
        /// <param name="fixHeight"></param>
        /// <returns></returns>
        public static PdfPCell GetBlankSignatureCell(string fieldName, int borderWidthLeft, int borderWidthTop,
            int borderWidthRight, int borderWidthBottom, int fixHeight = 55)
        {
            PdfPCell cell = new PdfPCell()
            {
                FixedHeight = fixHeight,
                BackgroundColor = BaseColor.WHITE,
                BorderColor = BaseColor.GRAY,
                BorderWidthLeft = borderWidthLeft,
                BorderWidthTop = borderWidthTop,
                BorderWidthRight = borderWidthRight,
                BorderWidthBottom = borderWidthBottom
            };

            // Gắn event để vẽ trường chữ ký ẩn bên trong Cell
            cell.CellEvent = new BlankSignatureFieldEvent(fieldName);
            return cell;
        }

        /// <summary>
        /// Tạo một ô (Cell) trống trong bảng PDF chứa một trường nhập liệu văn bản (Text Form Field) ẩn.
        /// Cho phép người dùng điền hoặc cập nhật thông tin text trực tiếp trên file PDF sau khi xuất.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="font"></param>
        /// <param name="fontSize"></param>
        /// <param name="borderWidthLeft"></param>
        /// <param name="borderWidthTop"></param>
        /// <param name="borderWidthRight"></param>
        /// <param name="borderWidthBottom"></param>
        /// <param name="fixHeight"></param>
        /// <returns></returns>
        public static PdfPCell GetBlankTextField(string fieldName, BaseFont font, float fontSize,
            int borderWidthLeft, int borderWidthTop, int borderWidthRight, int borderWidthBottom, int fixHeight = 22)
        {
            PdfPCell cell = new PdfPCell(new Phrase(" ")) // Để trống văn bản ban đầu
            {
                FixedHeight = fixHeight,
                BackgroundColor = BaseColor.WHITE,
                BorderColor = BaseColor.GRAY,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BorderWidthLeft = borderWidthLeft,
                BorderWidthTop = borderWidthTop,
                BorderWidthRight = borderWidthRight,
                BorderWidthBottom = borderWidthBottom
            };

            // Gắn event để vẽ trường nhập văn bản ẩn bên trong Cell
            cell.CellEvent = new BlankTextFieldEvent(fieldName, font, fontSize);
            return cell;
        }
    }
}
