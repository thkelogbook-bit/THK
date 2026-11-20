using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace Elogbook.Domain.Models
{
    /// <summary>
    /// XSSFHelper
    /// </summary>
    public class XssfHelper
    {
        #region Excel Helper
        public void SetStyleTableHeader(ISheet sheet, CellRangeAddress cellRange, XSSFColor xSSFColor, BorderStyle borderStyle)
        {
            XSSFWorkbook xssfWorkbook = (XSSFWorkbook)sheet.Workbook;
            var cellStyle = (XSSFCellStyle)xssfWorkbook.CreateCellStyle();

            for (int row = cellRange.FirstRow; row <= cellRange.LastRow; row++)
            {
                IRow currentRow = sheet.GetRow(row);
                for (int col = cellRange.FirstColumn; col <= cellRange.LastColumn; col++)
                {
                    ICell currentCell = currentRow.GetCell(col, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                    cellStyle.SetFillForegroundColor(xSSFColor);
                    cellStyle.FillPattern = FillPattern.SolidForeground;

                    var font = (XSSFFont)xssfWorkbook.CreateFont();
                    font.IsBold = true;
                    font.FontName = "Times New Roman";
                    cellStyle.SetFont(font);

                    cellStyle.BorderTop = borderStyle;
                    cellStyle.BorderLeft = borderStyle;
                    cellStyle.BorderRight = borderStyle;
                    cellStyle.BorderBottom = borderStyle;

                    cellStyle.Alignment = HorizontalAlignment.Center;
                    cellStyle.VerticalAlignment = VerticalAlignment.Center;

                    currentCell.CellStyle = cellStyle;
                }
            }
        }

        public void SetStyleTableBody(ISheet sheet, CellRangeAddress cellRange, XSSFColor xSSFColor, BorderStyle borderStyle)
        {
            XSSFWorkbook xssfWorkbook = (XSSFWorkbook)sheet.Workbook;
            var cellStyle = (XSSFCellStyle)xssfWorkbook.CreateCellStyle();

            for (int row = cellRange.FirstRow; row <= cellRange.LastRow; row++)
            {
                IRow currentRow = sheet.GetRow(row);
                for (int col = cellRange.FirstColumn; col <= cellRange.LastColumn; col++)
                {
                    ICell currentCell = currentRow.GetCell(col, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                    cellStyle.SetFillForegroundColor(xSSFColor);
                    cellStyle.FillPattern = FillPattern.SolidForeground;

                    var font = (XSSFFont)xssfWorkbook.CreateFont();
                    font.FontName = "Times New Roman";
                    cellStyle.SetFont(font);

                    cellStyle.BorderTop = borderStyle;
                    cellStyle.BorderLeft = borderStyle;
                    cellStyle.BorderRight = borderStyle;
                    cellStyle.BorderBottom = borderStyle;

                    cellStyle.Alignment = HorizontalAlignment.Left;
                    cellStyle.VerticalAlignment = VerticalAlignment.Center;

                    currentCell.CellStyle = cellStyle;
                }
            }
        }

        public void SetAlignmentCennter(ISheet sheet, int row, int col)
        {
            XSSFWorkbook xssfWorkbook = (XSSFWorkbook)sheet.Workbook;
            var cellStyle = (XSSFCellStyle)xssfWorkbook.CreateCellStyle();

            IRow currentRow = sheet.GetRow(row);
            ICell currentCell = currentRow.GetCell(col, MissingCellPolicy.CREATE_NULL_AS_BLANK);

            cellStyle.Alignment = HorizontalAlignment.Center;
            currentCell.CellStyle = cellStyle;
        }

        public void CreateCellTypeStr(ISheet sheet, CellRangeAddress cellRangeAddress, IRow headerRow, string value)
        {
            headerRow.CreateCell(cellRangeAddress.FirstColumn).SetCellValue(value);
            MergeColumnCellsTypeStr(sheet, cellRangeAddress);
        }

        public void MergeColumnCellsTypeStr(ISheet sheet, CellRangeAddress cellRangeAddress)
        {
            ICell cell = sheet.GetRow(cellRangeAddress.FirstRow).GetCell(cellRangeAddress.FirstColumn, MissingCellPolicy.CREATE_NULL_AS_BLANK);

            if (cell != null)
            {
                cell.SetCellType(CellType.String);

                cell.SetCellValue(cell.StringCellValue);

                sheet.AddMergedRegion(cellRangeAddress);
            }
        }

        public void CreateCellTypeNum(ISheet sheet, CellRangeAddress cellRangeAddress, IRow headerRow, int value)
        {
            headerRow.CreateCell(cellRangeAddress.FirstColumn).SetCellValue(value);
            MergeColumnCellsTypeNum(sheet, cellRangeAddress);
        }

        public void MergeColumnCellsTypeNum(ISheet sheet, CellRangeAddress cellRangeAddress)
        {
            ICell cell = sheet.GetRow(cellRangeAddress.FirstRow).GetCell(cellRangeAddress.FirstColumn, MissingCellPolicy.CREATE_NULL_AS_BLANK);

            if (cell != null)
            {
                cell.SetCellType(CellType.Numeric);

                cell.SetCellValue(cell.NumericCellValue);

                sheet.AddMergedRegion(cellRangeAddress);
            }
        }

        public void SetBoldweight(ISheet sheet, int row, int col, bool isBold = false, bool isItalic = false, int fontSize = 11)
        {
            XSSFWorkbook xssfWorkbook = (XSSFWorkbook)sheet.Workbook;
            var cellStyle = (XSSFCellStyle)xssfWorkbook.CreateCellStyle();
            var font = (XSSFFont)xssfWorkbook.CreateFont();
            cellStyle.SetFillForegroundColor(new XSSFColor(new byte[] { 255, 255, 255 }));
            cellStyle.FillPattern = FillPattern.SolidForeground;
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            font.FontName = "Times New Roman";
            font.IsBold = isBold;
            font.IsItalic = isItalic;
            font.FontHeightInPoints = fontSize;
            cellStyle.SetFont(font);
            ICell currentCell = sheet.GetRow(row).GetCell(col);
            currentCell.CellStyle = cellStyle;
        }

        public void SetStyleFinish(ISheet sheet, CellRangeAddress cellRangeAddress, int endRow, int endCol)
        {
            XSSFWorkbook xssfWorkbook = (XSSFWorkbook)sheet.Workbook;
            var cellStyle = (XSSFCellStyle)xssfWorkbook.CreateCellStyle();
            for (int row = 0; row < endRow; row++)
            {
                IRow currentRow = sheet.GetRow(row);
                if (currentRow == null)
                {
                    currentRow = sheet.CreateRow(row);
                }
                for (int col = 0; col < endCol; col++)
                {
                    if (row >= cellRangeAddress.FirstRow && row <= cellRangeAddress.LastRow && col >= cellRangeAddress.FirstColumn && col <= cellRangeAddress.LastColumn)
                    {
                        continue;
                    }
                    ICell currentCell = currentRow.GetCell(col, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                    if (currentCell.StringCellValue == "")
                    {
                        var font = (XSSFFont)xssfWorkbook.CreateFont();
                        font.FontName = "Times New Roman";
                        cellStyle.SetFont(font);
                        cellStyle.SetFillForegroundColor(new XSSFColor(new byte[] { 255, 255, 255 }));
                        cellStyle.FillPattern = FillPattern.SolidForeground;
                        currentCell.CellStyle = cellStyle;
                    }
                }
            }
        }

        public void CreateDataValidation01(ISheet sheet, CellRangeAddress cellRangeAddress, string[] listOption)
        {
            CellRangeAddressList addressList = new CellRangeAddressList(cellRangeAddress.FirstRow, cellRangeAddress.LastRow, cellRangeAddress.FirstColumn, cellRangeAddress.LastColumn);
            IDataValidationHelper dvHelper = sheet.GetDataValidationHelper();
            IDataValidationConstraint constraint = dvHelper.CreateExplicitListConstraint(listOption);
            IDataValidation dataValidation = dvHelper.CreateValidation(constraint, addressList);
            dataValidation.CreateErrorBox("Invalid Input", "Please select a valid option from the dropdown list.");
            dataValidation.ShowErrorBox = true;
            sheet.AddValidationData(dataValidation);
            sheet.AddMergedRegion(cellRangeAddress);
        }

        public void CreateDataValidation02(ISheet sheet, CellRangeAddress cellRangeAddress, string formula)
        {
            CellRangeAddressList addressList = new CellRangeAddressList(cellRangeAddress.FirstRow, cellRangeAddress.LastRow, cellRangeAddress.FirstColumn, cellRangeAddress.LastColumn);
            IDataValidationHelper dvHelper = sheet.GetDataValidationHelper();
            IDataValidationConstraint constraint = dvHelper.CreateFormulaListConstraint(formula);
            IDataValidation dataValidation = dvHelper.CreateValidation(constraint, addressList);
            dataValidation.CreateErrorBox("Invalid Input", "Please select a valid option from the dropdown list.");
            dataValidation.ShowErrorBox = true;
            sheet.AddValidationData(dataValidation);
            sheet.AddMergedRegion(cellRangeAddress);
        }

        #endregion
    }
}
