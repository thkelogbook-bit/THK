CREATE PROCEDURE [dbo].[TSVH_GetUserInfoExport]
	@FactoryID INT = 0,
	@OperationPositionID INT = 0,
	@OperationShiftID INT = 0,
	@CreatedDate DATETIME = NULL
AS
BEGIN
	SET NOCOUNT ON
	
	--Ca ngày
	SELECT TOP 1 u.ID, u.FullName, u.Signature, CAST(0 AS BIT) AS ShiftType
	FROM dbo.OperationDiary od WITH(NOLOCK)
	INNER JOIN dbo.OperationDiaryPositionShift odps WITH(NOLOCK) ON od.ID = odps.OperationDiaryID
	INNER JOIN dbo.PositionInShift pis WITH(NOLOCK) ON odps.PositionInShiftID = pis.ID
	INNER JOIN dbo.OperationPosition op WITH(NOLOCK) ON pis.OperationPositionID = op.ID
	INNER JOIN dbo.Users u WITH(NOLOCK) ON odps.AssignedUserID = u.ID
	WHERE od.FactoryID = @FactoryID
		AND od.OperationPositionID = @OperationPositionID
		AND od.OperationShiftID = 1
		AND CAST(CreatedDate AS DATE) = @CreatedDate
		AND op.JobTitle = 1
	UNION ALL
	--Ca đêm
	SELECT TOP 1 u.ID, u.FullName, u.Signature, CAST(1 AS BIT) AS ShiftType
	FROM dbo.OperationDiary od WITH(NOLOCK)
	INNER JOIN dbo.OperationDiaryPositionShift odps WITH(NOLOCK) ON od.ID = odps.OperationDiaryID
	INNER JOIN dbo.PositionInShift pis WITH(NOLOCK) ON odps.PositionInShiftID = pis.ID
	INNER JOIN dbo.OperationPosition op WITH(NOLOCK) ON pis.OperationPositionID = op.ID
	INNER JOIN dbo.Users u WITH(NOLOCK) ON odps.AssignedUserID = u.ID
	WHERE od.FactoryID = @FactoryID
		AND od.OperationPositionID = @OperationPositionID
		AND od.OperationShiftID = 2
		AND CAST(CreatedDate AS DATE) = @CreatedDate
		AND op.JobTitle = 1
END