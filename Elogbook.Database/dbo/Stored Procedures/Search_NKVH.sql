-- =============================================
-- Tìm kiếm danh sách ngày tạo NKVH
-- =============================================
CREATE PROCEDURE [dbo].[Search_NKVH]
	@PageNumber INT = 1,
	@PageSize INT = 5,
	@FullName NVARCHAR(255) = N'%',
	@FactoryID INT = -1,
	@OperationPositionID INT = -1,
	@OperationShiftID INT = -1,
	@OperationCrewID INT = -1,
	@FromDate DATETIME = NULL,
	@ToDate DATETIME = NULL
AS
BEGIN
	SET NOCOUNT ON

	IF (@FromDate IS NULL) SET @FromDate = CAST(-53690 AS DATETIME)
	IF (@ToDate IS NULL) SET @ToDate = GETDATE()

	DECLARE @TempData TABLE(GroupDate DATETIME)

	INSERT INTO @TempData
	SELECT DISTINCT CreatedDate AS GroupDate
	FROM dbo.OperationDiary od WITH(NOLOCK)
	LEFT JOIN dbo.Users u WITH(NOLOCK) ON od.CreatedUserID = u.ID
	WHERE od.IsDeleted = 0
		AND u.FullName LIKE @FullName
		AND (CAST(CreatedDate AS DATE) >= @FromDate AND CAST(CreatedDate AS DATE) <= @ToDate)
		AND (od.FactoryID = @FactoryID OR @FactoryID = -1)
		AND (od.OperationPositionID = @OperationPositionID OR @OperationPositionID = -1)
		AND (od.OperationShiftID = @OperationShiftID OR @OperationShiftID = -1)
		AND (od.OperationCrewID = @OperationCrewID OR  @OperationCrewID = -1)
	ORDER BY od.CreatedDate DESC

	DECLARE @TotalRecords INT = (SELECT COUNT(1) FROM @TempData)

	SELECT GroupDate,
		@TotalRecords AS TotalRecords
	FROM @TempData
	ORDER BY GroupDate DESC
	OFFSET (@PageNumber-1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY
END