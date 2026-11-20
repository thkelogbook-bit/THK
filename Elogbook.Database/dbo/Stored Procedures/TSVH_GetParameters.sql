-- =============================================
-- Lấy danh sách ghi thông số theo điều kiện
-- Fix issue performance
-- =============================================
--TEST: EXECUTE [dbo].[TSVH_GetParameters] 1, 2, 3, 2, 45672, 1, 10
CREATE PROCEDURE [dbo].[TSVH_GetParameters]
	@FactoryID INT = 0,
	@CycleID INT = 0,
	@ParameterGroupID INT = 0,
	@OperationPositionID INT = 0,
	@OperationTimeLineID INT = 0,
	@PageNumber INT = 1,
	@PageSize INT = 10
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @IsUseOperationParameterGroup BIT = (SELECT TOP 1 IsUseOperationParameterGroup FROM dbo.Factory WITH(NOLOCK) WHERE ID = @FactoryID)
	IF(@ParameterGroupID IS NULL) SET @ParameterGroupID = 0
	IF(@IsUseOperationParameterGroup IS NULL) SET @IsUseOperationParameterGroup = 0

	DECLARE @TempData TABLE(
		[RowIndex] INT IDENTITY(1,1),
		[ID] INT,
		[IsGroup] BIT,
		[ParameterID] INT,
		[ParameterName] NVARCHAR(255),
		[KKSCode] NVARCHAR(255),
		[UnitMeasure] NVARCHAR(255),
		[PreviousValue] NVARCHAR(MAX),
		[CurrentValue] NVARCHAR(MAX),
		[ErrorCode] INT,
		[FromRange] REAL,
		[ToRange] REAL,
		[IsNumberType] BIT,
		[ParameterGroupID] INT,
		[OprerationTimeLineID] INT,
		[IsChecked] BIT,
		[AlertDetails] NVARCHAR(MAX),
		[CheckedDate] DATETIME,
		[Setting] NVARCHAR(MAX)
	)

	INSERT INTO @TempData
	SELECT
		p.ID,
		0,
		p.ParameterID,
		p.ParameterName,
		p.KKSCode,
		p.UnitMeasure,
		p.PreviousValue,
		p.CurrentValue,
		p.ErrorCode,
		p.FromRange,
		p.ToRange,
		p.IsNumberType,
		p.ParameterGroupID,
		p.OperationTimeLineID,
		p.IsChecked,
		p.AlertDetails,
		p.CheckedDate,
		NULL
	FROM dbo.OperationParameters p WITH(NOLOCK)
	WHERE p.FactoryID = @FactoryID
	AND p.OperationPositionID = @OperationPositionID
	AND p.OperationTimeLineID = @OperationTimeLineID
	AND p.CycleID = @CycleID
	AND p.IsDeleted = 0
	AND ((@IsUseOperationParameterGroup = 1 AND (@ParameterGroupID > 0 AND p.ParameterGroupID = @ParameterGroupID)
		OR (@ParameterGroupID <= 0 AND p.ParameterGroupID IS NULL)) OR @IsUseOperationParameterGroup = 0)

	DECLARE @TotalRecords INT = (SELECT COUNT(ID) FROM @TempData)

	SELECT temp.ID,
		CAST(CASE WHEN @ParameterGroupID > 0 THEN 1 ELSE 0 END AS BIT) AS IsGroup,
		temp.ParameterID,
		temp.ParameterName,
		temp.KKSCode,
		temp.UnitMeasure,
		REVERSE(REPLACE(STUFF((
					SELECT ':' + (CASE WHEN ISNULL(pd.ErrorCode, 0) = 1 THEN 'F' --Thiết bị bị hỏng (F)
										WHEN ISNULL(pd.ErrorCode, 0) = 2 THEN '-' --Dừng thiết bị (-)
										WHEN ISNULL(pd.ErrorCode, 0) = 3 THEN 'S' --Sim thiết bị (S)
										WHEN ISNULL(pd.ErrorCode, 0) = 4 THEN 'J' --Jump thiết bị (J)
										WHEN ISNULL(pd.ErrorCode, 0) = 5 THEN 'x' --Bận công tác
									ELSE ISNULL(REVERSE(pd.CurrentValue), '') END)
					FROM dbo.OperationParameters pd WITH(NOLOCK)
					WHERE pd.FactoryID = @FactoryID
						AND pd.OperationPositionID = @OperationPositionID
						AND pd.OperationTimeLineID < temp.OprerationTimeLineID
						AND pd.CycleID = @CycleID
						AND pd.ParameterID = temp.ParameterID
						AND pd.IsDeleted = 0
					ORDER BY pd.OperationTimeLineID DESC
					OFFSET (1-1) * 5 ROWS
					FETCH NEXT 5 ROWS ONLY
					FOR XML PATH(''), TYPE).value('.', 'VARCHAR(MAX)'), 1, 1, ''), 
			':', ' : ')) AS PreviousValue,
		temp.CurrentValue,
		CASE WHEN temp.ErrorCode = 5 THEN NULL ELSE temp.ErrorCode END AS ErrorCode,
		temp.FromRange,
		temp.ToRange,
		temp.IsNumberType,
		temp.ParameterGroupID,
		temp.OprerationTimeLineID,
		temp.IsChecked,
		temp.AlertDetails,
		temp.CheckedDate,
		REPLACE(STUFF((
					SELECT ',' + ISNULL(Alert,'') + ' ' + ISNULL(Operator,'') + ' ' + ISNULL(CAST(Value AS NVARCHAR(100)),'') + ' : ' + ISNULL(Color,'') + ';'
					FROM dbo.AlertDetails WITH(NOLOCK)
					WHERE ParameterID = temp.ParameterID
					FOR XML PATH(''), TYPE).value('.', 'VARCHAR(MAX)'), 1, 1, ''), 
			';,', ';') AS Setting,
		@TotalRecords AS TotalRecords
	FROM @TempData temp
	ORDER BY temp.ID
	OFFSET (@PageNumber-1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY
END