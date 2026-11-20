-- =============================================
-- Tìm kiếm danh sách tồn tại trong ca
-- =============================================
--EXECUTE [dbo].[Search_TDTTC] 1, 100, -1, -1, -1, N'12/2022/PXHĐ-PM', NULL, NULL, N'', N'DESC'
CREATE PROCEDURE [dbo].[Search_TDTTC]
	@PageNumber INT = 1,
	@PageSize INT = 10,
	@FactoryID INT = -1,
	@OperationPositionID INT = -1,
	@StatusID INT = -1,
	@SearchText NVARCHAR(255) = N'%',
	@FromDate DATETIME = NULL,
	@ToDate DATETIME = NULL,
	@SortColumn NVARCHAR(100) = '',
	@SortOrder NVARCHAR(100) = ''
AS
BEGIN
	SET NOCOUNT ON
	SET @SortColumn = ISNULL(@SortColumn, '')
	SET @SortOrder = ISNULL(@SortOrder, '')
	IF (ISNULL(@SearchText, '') = '')
	BEGIN
		SET @SearchText = N'%'
	END
	ELSE
	BEGIN
		SET @SearchText = '%' + @SearchText + '%'
	END
	
	DECLARE @TempData TABLE(
		RowIndex INT IDENTITY(1, 1),
		ID INT,
		InsertedAt DATETIME,
		Content NVARCHAR(MAX),
		CreatedUserID INT,
		FullName NVARCHAR(255),
		StatusID INT,
		OperationDiaryID INT,
		RegistrationTicketID INT,
		RegistrationTicketName NVARCHAR(255),
		TeamLeader NVARCHAR(255),
		Area NVARCHAR(MAX),
		Attachments NVARCHAR(500),
		FactoryID INT,
		SyncDataKey NVARCHAR(100),
		AppID NVARCHAR(100)
	)

	IF (UPPER(@SortColumn) = 'INSERTEDAT')
	BEGIN
		IF (UPPER(@SortOrder) = 'DESC')
		BEGIN
			INSERT INTO @TempData(ID, InsertedAt, Content, CreatedUserID, FullName, StatusID, OperationDiaryID, RegistrationTicketID, RegistrationTicketName, TeamLeader,
				Area, Attachments, FactoryID, SyncDataKey, AppID)
			SELECT DISTINCT 
				eis.ID,
				eis.InsertedAt,
				eis.Content,
				u.ID AS CreatedUserID,
				u.FullName AS FullName,
				eis.[StatusID],
				eis.OperationDiaryID,
				eis.RegistrationTicketID,
				eis.RegistrationTicketName,
				eis.TeamLeader,
				eis.Area,
				eis.Attachments,
				od.FactoryID,
				eis.SyncDataKey,
				eis.AppID
			FROM dbo.ExistInShift eis WITH(NOLOCK)
			LEFT JOIN dbo.OperationDiary od WITH(NOLOCK) ON eis.OperationDiaryID = od.ID
			LEFT JOIN dbo.OperationPosition op WITH(NOLOCK) ON od.OperationPositionID = op.ID
			LEFT JOIN dbo.Users u WITH(NOLOCK) ON eis.CreatedUserID = u.ID
			LEFT JOIN dbo.IssueOfShiftDetail isd WITH(NOLOCK) ON eis.ID = isd.ExistInShiftID
			LEFT JOIN dbo.Users u1 WITH(NOLOCK) ON isd.CreatedUserID = u1.ID
			WHERE eis.IsDeleted = 0
				AND eis.IsDeleteIssueOfShift = 0
				AND (od.FactoryID = @FactoryID OR @FactoryID = -1)
				AND (od.OperationPositionID = @OperationPositionID OR @OperationPositionID = -1)
				AND (eis.StatusID = @StatusID OR @StatusID = -1)
				AND ((ISNULL(@FromDate, '') = '' OR CAST(od.TimeDelivery AS DATE) >= CAST(@FromDate AS DATE)) 
					AND (ISNULL(@ToDate, '') = '' OR CAST(od.TimeDelivery AS DATE) <= CAST(@ToDate AS DATE)))
				AND (u.FullName LIKE @SearchText OR eis.RegistrationTicketName LIKE @SearchText OR u1.FullName LIKE @SearchText)
			ORDER BY eis.InsertedAt DESC, eis.StatusID ASC
		END
		ELSE
		BEGIN
			INSERT INTO @TempData(ID, InsertedAt, Content, CreatedUserID, FullName, StatusID, OperationDiaryID, RegistrationTicketID, RegistrationTicketName, TeamLeader,
				Area, Attachments, FactoryID, SyncDataKey, AppID)
			SELECT DISTINCT 
				eis.ID,
				eis.InsertedAt,
				eis.Content,
				u.ID AS CreatedUserID,
				u.FullName AS FullName,
				eis.[StatusID],
				eis.OperationDiaryID,
				eis.RegistrationTicketID,
				eis.RegistrationTicketName,
				eis.TeamLeader,
				eis.Area,
				eis.Attachments,
				od.FactoryID,
				eis.SyncDataKey,
				eis.AppID
			FROM dbo.ExistInShift eis WITH(NOLOCK)
			LEFT JOIN dbo.OperationDiary od WITH(NOLOCK) ON eis.OperationDiaryID = od.ID
			LEFT JOIN dbo.OperationPosition op WITH(NOLOCK) ON od.OperationPositionID = op.ID
			LEFT JOIN dbo.Users u WITH(NOLOCK) ON eis.CreatedUserID = u.ID
			LEFT JOIN dbo.IssueOfShiftDetail isd WITH(NOLOCK) ON eis.ID = isd.ExistInShiftID
			LEFT JOIN dbo.Users u1 WITH(NOLOCK) ON isd.CreatedUserID = u1.ID
			WHERE eis.IsDeleted = 0
				AND eis.IsDeleteIssueOfShift = 0
				AND (od.FactoryID = @FactoryID OR @FactoryID = -1)
				AND (od.OperationPositionID = @OperationPositionID OR @OperationPositionID = -1)
				AND (eis.StatusID = @StatusID OR @StatusID = -1)
				AND ((ISNULL(@FromDate, '') = '' OR CAST(od.TimeDelivery AS DATE) >= CAST(@FromDate AS DATE)) 
					AND (ISNULL(@ToDate, '') = '' OR CAST(od.TimeDelivery AS DATE) <= CAST(@ToDate AS DATE)))
				AND (u.FullName LIKE @SearchText OR eis.RegistrationTicketName LIKE @SearchText OR u1.FullName LIKE @SearchText)
			ORDER BY eis.InsertedAt ASC, eis.StatusID ASC
		END
	END
	ELSE
	BEGIN
		INSERT INTO @TempData(ID, InsertedAt, Content, CreatedUserID, FullName, StatusID, OperationDiaryID, RegistrationTicketID, RegistrationTicketName, TeamLeader,
				Area, Attachments, FactoryID, SyncDataKey, AppID)
		SELECT DISTINCT 
			eis.ID,
			eis.InsertedAt,
			eis.Content,
			u.ID AS CreatedUserID,
			u.FullName AS FullName,
			eis.[StatusID],
			eis.OperationDiaryID,
			eis.RegistrationTicketID,
			eis.RegistrationTicketName,
			eis.TeamLeader,
			eis.Area,
			eis.Attachments,
			od.FactoryID,
			eis.SyncDataKey,
			eis.AppID
		FROM dbo.ExistInShift eis WITH(NOLOCK)
		LEFT JOIN dbo.OperationDiary od WITH(NOLOCK) ON eis.OperationDiaryID = od.ID
		LEFT JOIN dbo.OperationPosition op WITH(NOLOCK) ON od.OperationPositionID = op.ID
		LEFT JOIN dbo.Users u WITH(NOLOCK) ON eis.CreatedUserID = u.ID
		LEFT JOIN dbo.IssueOfShiftDetail isd WITH(NOLOCK) ON eis.ID = isd.ExistInShiftID
		LEFT JOIN dbo.Users u1 WITH(NOLOCK) ON isd.CreatedUserID = u1.ID
		WHERE eis.IsDeleted = 0
			AND eis.IsDeleteIssueOfShift = 0
			AND (od.FactoryID = @FactoryID OR @FactoryID = -1)
			AND (od.OperationPositionID = @OperationPositionID OR @OperationPositionID = -1)
			AND (eis.StatusID = @StatusID OR @StatusID = -1)
			AND ((ISNULL(@FromDate, '') = '' OR CAST(od.TimeDelivery AS DATE) >= CAST(@FromDate AS DATE)) 
				AND (ISNULL(@ToDate, '') = '' OR CAST(od.TimeDelivery AS DATE) <= CAST(@ToDate AS DATE)))
			AND (u.FullName LIKE @SearchText OR eis.RegistrationTicketName LIKE @SearchText OR u1.FullName LIKE @SearchText)
		ORDER BY eis.StatusID ASC, eis.InsertedAt DESC
	END

	DECLARE @TotalRecords INT = (SELECT COUNT(1) FROM @TempData)

	SELECT temp.*,
		@TotalRecords AS TotalRecords
	FROM @TempData temp
	ORDER BY RowIndex ASC
	OFFSET (@PageNumber-1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY
END