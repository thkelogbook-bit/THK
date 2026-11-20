-- =============================================
-- Tìm kiếm nhanh thông tin NKVH
-- =============================================
CREATE PROCEDURE [dbo].[Quick_Search_NKVH]
	@PageNumber INT = 1,
	@PageSize INT = 10,
	@Year INT,
	@FactoryID INT = 0,
	@SearchText NVARCHAR(1000) = ''
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @TempData TABLE(OperationDiaryID INT,
						FactoryID INT,
						OperationPositionID INT,
						OperationShiftID INT,
						[Description] NVARCHAR(MAX),
						CreatedDate DATETIME,
						DiaryType INT)

	;WITH datasource (ID, Description, DiaryType)
	AS
	(
		SELECT OperationDiaryID AS 'ID', Description AS 'Description', 1 AS 'DiaryType' FROM dbo.OperationSituation WITH(NOLOCK)
		WHERE IsDeleted = 0 AND YEAR(InsertedAt) = @Year AND FactoryID = @FactoryID AND LEN(LTRIM(RTRIM(Description))) > 0 AND Description LIKE N'%'+@SearchText+'%'
		UNION ALL
		SELECT OperationDiaryID AS 'ID', Description AS 'Description', 2 AS 'DiaryType' FROM dbo.DeliveryShiftMethod WITH(NOLOCK)
		WHERE IsDeleted = 0 AND YEAR(InsertedAt) = @Year AND FactoryID = @FactoryID AND LEN(LTRIM(RTRIM(ISNULL(Description, '')))) > 0 AND Description LIKE N'%'+@SearchText+'%'
		UNION ALL
		SELECT OperationDiaryID AS 'ID', Description AS 'Description', 3 AS 'DiaryType' FROM dbo.AbnormalIncident WITH(NOLOCK)
		WHERE IsDeleted = 0 AND YEAR(InsertedAt) = @Year AND FactoryID = @FactoryID AND LEN(LTRIM(RTRIM(ISNULL(Description, '')))) > 0 AND Description LIKE N'%'+@SearchText+'%'
		UNION ALL
		SELECT OperationDiaryID AS 'ID', Content AS 'Description', 4 AS 'DiaryType' FROM dbo.ExistInShift WITH(NOLOCK)
		WHERE IsDeleted = 0 AND YEAR(InsertedAt) = @Year AND FactoryID = @FactoryID AND LEN(LTRIM(RTRIM(Content))) > 0 AND Content LIKE N'%'+@SearchText+'%'
	)
	
	INSERT INTO @TempData
	SELECT 
		od.ID,
		od.FactoryID,
		od.OperationPositionID,
		od.OperationShiftID,
		temp.[Description], 
		od.CreatedDate,
		temp.DiaryType
	FROM datasource temp WITH(NOLOCK)
	INNER JOIN dbo.[OperationDiary] od WITH(NOLOCK) ON temp.ID = od.ID
	WHERE YEAR(od.CreatedDate) = @Year AND od.FactoryID = @FactoryID
	
	DECLARE @TotalRecords INT = (SELECT COUNT(1) FROM @TempData)
	DECLARE @Results TABLE(OperationDiaryID INT,
							FactoryID INT,
							OperationPositionID INT,
							OperationShiftID INT,
							[Description] NVARCHAR(MAX),
							CreatedDate DATETIME,
							DiaryType INT)
	
	INSERT INTO @Results
	SELECT 
		OperationDiaryID,
		FactoryID,
		OperationPositionID,
		OperationShiftID,
		[Description],
		CreatedDate,
		DiaryType
	FROM @TempData
	ORDER BY CreatedDate DESC, FactoryID ASC, OperationPositionID ASC, OperationShiftID ASC, DiaryType ASC
	OFFSET (@PageNumber-1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY
	
	DECLARE @strHighlight NVARCHAR(1000) = ''
	IF(ISNULL(@SearchText, '') != '')
	BEGIN
		SET @strHighlight = '<span class="selected">' + @SearchText + '</span>'
	END

	SELECT 
		OperationShiftName + ' - ' + FORMAT(CreatedDate, 'd', 'en-gb') AS ShiftDescription,
		FactoryName AS FactoryName,
		(CASE WHEN DiaryType = 1 THEN N'<span class="headerContent">' + N'Tình hình vận hành' + N' - ' + OperationPositionName + N'</span>'
			WHEN DiaryType = 2 THEN N'<span class="headerContent">' + N'Phương thức giao ca' + N' - ' + OperationPositionName + N'</span>'
			WHEN DiaryType = 3 THEN N'<span class="headerContent">' + N'Bất thường sự cố' + N' - ' + OperationPositionName + N'</span>'
			ELSE N'<span class="headerContent">' + N'Tồn tại trong ca' + N' - ' + OperationPositionName + N'</span>' END) AS HeaderContent, 
		(CASE WHEN ISNULL(@strHighlight, '') != '' THEN REPLACE(Description, @SearchText , @strHighlight) ELSE [Description] END) AS [Description],
		ISNULL(@TotalRecords, 0) AS TotalRecords,
		CreatedDate,
		DiaryType
	FROM @Results temp
	INNER JOIN dbo.[OperationShift] os WITH(NOLOCK) ON temp.OperationShiftID = os.ID
	INNER JOIN dbo.[OperationPosition] op WITH(NOLOCK) ON temp.OperationPositionID = op.ID
	INNER JOIN dbo.[Factory] f WITH(NOLOCK) ON temp.FactoryID = f.ID
	ORDER BY CreatedDate DESC, temp.FactoryID ASC, temp.OperationPositionID ASC, temp.OperationShiftID ASC, DiaryType ASC
END