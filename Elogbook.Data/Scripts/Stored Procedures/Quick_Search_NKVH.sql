-- =============================================
-- Tìm kiếm nhanh thông tin NKVH
-- =============================================
CREATE PROCEDURE [dbo].[Quick_Search_NKVH]
	@PageNumber INT = 1,
	@PageSize INT = 10,
	@Year INT,
	@FactoryID INT = -1,
	@SearchText NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @TempData TABLE(OperationShiftID INT,
							OperationShiftName NVARCHAR(255), 
							FactoryID INT,
							FactoryName NVARCHAR(255),
							[Description] NVARCHAR(MAX), 
							CreatedDate DATETIME, 
							OperationPositionID INT,
							OperationPositionName NVARCHAR(255),
							DiaryType INT)
	INSERT INTO @TempData
	SELECT 
		os.ID,
		os.OperationShiftName, 
		f.ID,
		f.FactoryName, 
		temp.[Description], 
		od.CreatedDate,
		op.ID,
		op.OperationPositionName,
		temp.DiaryType
	FROM dbo.[view_Quick_Search_NKVH] temp WITH(NOLOCK)
	INNER JOIN dbo.[OperationDiary] od WITH(NOLOCK) ON temp.ID = od.ID
	INNER JOIN dbo.[OperationShift] os WITH(NOLOCK) ON od.OperationShiftID = os.ID
	INNER JOIN dbo.[OperationPosition] op WITH(NOLOCK) ON od.OperationPositionID = op.ID
	INNER JOIN dbo.[Factory] f WITH(NOLOCK) ON od.FactoryID = f.ID
	WHERE YEAR(od.CreatedDate) = @Year
		AND (od.FactoryID = @FactoryID OR @FactoryID = -1)
		AND temp.[Description] LIKE N'%'+@SearchText+'%'
	ORDER BY od.CreatedDate DESC, f.ID ASC, op.ID ASC, os.ID ASC

	DECLARE @TotalRecords INT = (SELECT COUNT(1) FROM @TempData) 

	SELECT 
		OperationShiftName + ' - ' + FORMAT(CreatedDate, 'd', 'en-gb') AS ShiftDescription,
		FactoryName AS FactoryName,
		(CASE WHEN DiaryType = 1 THEN N'<span class="headerContent">' + N'Tình hình vận hành' + N' - ' + OperationPositionName + N'</span>'
			WHEN DiaryType = 2 THEN N'<span class="headerContent">' + N'Phương thức giao ca' + N' - ' + OperationPositionName + N'</span>'
			WHEN DiaryType = 3 THEN N'<span class="headerContent">' + N'Bất thường sự cố' + N' - ' + OperationPositionName + N'</span>'
			ELSE N'<span class="headerContent">' + N'Tồn tại trong ca' + N' - ' + OperationPositionName + N'</span>' END) AS HeaderContent, 
		[Description] AS [Description],
		ISNULL(@TotalRecords, 0) AS TotalRecords,
		CreatedDate,
		DiaryType
	FROM @TempData
	ORDER BY CreatedDate DESC, FactoryID ASC, OperationPositionID ASC, OperationShiftID ASC, DiaryType ASC
	OFFSET (@PageNumber-1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY
END