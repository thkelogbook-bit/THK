--USE Elogbook
--GO
--ALTER DATABASE [Elogbook] SET COMPATIBILITY_LEVEL = 130

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('TSVH_GetParameters'))
	DROP PROC [dbo].[TSVH_GetParameters]
GO
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
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('TSVH_GetParameterValuesPrint'))
	DROP PROC [dbo].[TSVH_GetParameterValuesPrint]

GO
-- ========================================================
-- Lấy giá trị ghi thông số của chu kỳ
-- ========================================================
CREATE PROCEDURE [dbo].[TSVH_GetParameterValuesPrint]
	@FactoryID INT,
	@OperationPositionID INT,
	@InsertedAt DATE
AS
BEGIN
	SET NOCOUNT ON

	SELECT 
		op.CycleID,
		op.ParameterID, 
		CAST(otl.TimeLineName AS VARCHAR(5)) AS OpeartionTimeLineName,
		CASE WHEN ISNULL(ErrorCode, 0) > 0 THEN 
			(CASE WHEN ErrorCode = 1 THEN 'F'
				WHEN ErrorCode = 2 THEN '-'
				WHEN ErrorCode = 3 THEN 'S'
				WHEN ErrorCode = 4 THEN 'J'
				WHEN ErrorCode = 5 THEN 'x' END)
			ELSE CAST(CurrentValue AS NVARCHAR(MAX)) END AS ParameterValue
	FROM dbo.OperationParameters op WITH(NOLOCK)
	INNER JOIN dbo.OperationTimeLine otl WITH(NOLOCK) ON op.OperationTimeLineID = otl.ID
	WHERE otl.FactoryID = @FactoryID 
	AND otl.OperationPositionID = @OperationPositionID
	AND CAST(otl.InsertedAt AS DATE) = @InsertedAt
	AND op.IsDeleted = 0
	AND otl.IsDeleted = 0
END

GO
--CR: #10
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProbationaryStaff]') AND type in (N'U'))
	DROP TABLE [dbo].[ProbationaryStaff]
GO

CREATE TABLE [dbo].[ProbationaryStaff](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[OperationDiaryID] [int] NOT NULL,
	[OperationPositionID] [int] NOT NULL,
	[SyncDataKey] [nvarchar](100) NULL,
	[SyncedDate] [datetime] NULL,
	[AppID] [nvarchar](100) NULL,
	[AssignedUserID] [int] NULL,
	[AssignedUserName] [nvarchar](255) NULL,
	[PositionName] [nvarchar](255) NULL,
	[Description] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedUserID] [int] NULL,
	[InsertedAt] [datetime] NOT NULL,
	[LastUpdatedUserID] [int] NULL,
	[UpdatedAt] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.ProbationaryStaff] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProbationaryStaff]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ProbationaryStaff_dbo.OperationDiary_OperationDiaryID] FOREIGN KEY([OperationDiaryID])
REFERENCES [dbo].[OperationDiary] ([ID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ProbationaryStaff] CHECK CONSTRAINT [FK_dbo.ProbationaryStaff_dbo.OperationDiary_OperationDiaryID]
GO

ALTER TABLE [dbo].[ProbationaryStaff]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ProbationaryStaff_dbo.PositionInShift_PositionInShiftID] FOREIGN KEY([OperationPositionID])
REFERENCES [dbo].[PositionInShift] ([ID])
GO

ALTER TABLE [dbo].[ProbationaryStaff] CHECK CONSTRAINT [FK_dbo.ProbationaryStaff_dbo.PositionInShift_PositionInShiftID]
GO

ALTER TABLE [dbo].[ProbationaryStaff]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ProbationaryStaff_dbo.Users_AssignedUserID] FOREIGN KEY([AssignedUserID])
REFERENCES [dbo].[Users] ([ID])
GO

ALTER TABLE [dbo].[ProbationaryStaff] CHECK CONSTRAINT [FK_dbo.ProbationaryStaff_dbo.Users_AssignedUserID]
GO

ALTER TABLE [dbo].[ProbationaryStaff]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ProbationaryStaff_dbo.Users_CreatedUserID] FOREIGN KEY([CreatedUserID])
REFERENCES [dbo].[Users] ([ID])
GO

ALTER TABLE [dbo].[ProbationaryStaff] CHECK CONSTRAINT [FK_dbo.ProbationaryStaff_dbo.Users_CreatedUserID]
GO

ALTER TABLE [dbo].[ProbationaryStaff]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ProbationaryStaff_dbo.Users_LastUpdatedUserID] FOREIGN KEY([LastUpdatedUserID])
REFERENCES [dbo].[Users] ([ID])
GO

ALTER TABLE [dbo].[ProbationaryStaff] CHECK CONSTRAINT [FK_dbo.ProbationaryStaff_dbo.Users_LastUpdatedUserID]
GO
--CR: #10

--CR: #8
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('TSVH_UpdateOperationTimeLinePartial'))
	DROP PROC [dbo].[TSVH_UpdateOperationTimeLinePartial]
GO
CREATE PROCEDURE [dbo].[TSVH_UpdateOperationTimeLinePartial]
    @FactoryID INT,
    @OperationPositionID INT,
    @CycleID INT,
    @OperationTimeLineIDs NVARCHAR(MAX),   
    @StopParameterIDs NVARCHAR(MAX),
    @ErrorCode INT,
    @CurrentUserID INT
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @TotalParamOther INT = 0,
            @CurrentDate DATETIME = GETDATE(),
            @Result NVARCHAR(20) = 'ERROR',
            @TotalTimeLine INT = 0,
            @TimeLineIndex INT = 1,
            @OperationTimeLineID INT

    DECLARE @OperationTimeLineIDTemp TABLE
    (
        RowIndex INT IDENTITY(1,1), 
        OperationTimeLineID INT
    )
		 DECLARE @StopParameterIDsTemp TABLE
    (
        ParameterID INT
    )


    INSERT INTO @StopParameterIDsTemp (ParameterID)
    SELECT value
    FROM STRING_SPLIT(@StopParameterIDs, ',')

    -- Chỉnh sửa điều kiện WHERE và sử dụng INNER JOIN trực tiếp trong câu lệnh SELECT
    INSERT INTO @OperationTimeLineIDTemp
    SELECT otl.ID
    FROM STRING_SPLIT(@OperationTimeLineIDs, ',') temp
    INNER JOIN dbo.OperationTimeLine otl ON temp.[value] = otl.ID

    SET @TotalTimeLine = (SELECT COUNT(OperationTimeLineID) FROM @OperationTimeLineIDTemp)

    IF(ISNULL(@TotalTimeLine, 0) > 0)
    BEGIN
        BEGIN TRANSACTION

        WHILE(@TimeLineIndex <= @TotalTimeLine)
        BEGIN
            SELECT TOP 1 @OperationTimeLineID = OperationTimeLineID FROM @OperationTimeLineIDTemp WHERE RowIndex = @TimeLineIndex
            IF(ISNULL(@OperationTimeLineID, 0) > 0)
            BEGIN
                -- Count số dòng dữ liệu báo bận, báo dừng máy của mỗi khung giờ
                SELECT @TotalParamOther = COUNT(p.ID)
                FROM dbo.OperationParameters p WITH (NOLOCK)
                WHERE p.FactoryID = @FactoryID
                    AND p.OperationPositionID = @OperationPositionID
                    AND p.CycleID = @CycleID
                    AND p.OperationTimeLineID = @OperationTimeLineID
                    AND p.IsChecked = 0
                    AND p.IsDeleted = 0
                    AND ParameterID IN (SELECT ParameterID FROM @StopParameterIDsTemp)

                SET @TotalParamOther = ISNULL(@TotalParamOther, 0)

                -- Cập nhật dữ liệu ghi thông số
                UPDATE p
                SET IsChecked = 1,
                    ErrorCode = CASE WHEN @ErrorCode < 1 AND @ErrorCode > 5 THEN 0 ELSE @ErrorCode END,
                    LastUpdatedUserID = @CurrentUserID,
                    CheckedDate = @CurrentDate,
                    SyncedDate = @CurrentDate
                FROM dbo.OperationParameters p
                WHERE p.FactoryID = @FactoryID
                    AND p.OperationPositionID = @OperationPositionID
                    AND p.CycleID = @CycleID
                    AND p.OperationTimeLineID = @OperationTimeLineID
                    AND p.IsChecked = 0
                    AND p.IsDeleted = 0
                    AND ParameterID IN (SELECT ParameterID FROM @StopParameterIDsTemp)

                -- Cập nhật thông tin khung giờ ghi thông số
                UPDATE dbo.OperationTimeLine
                SET TotalParametersChecked = TotalParametersChecked + @TotalParamOther,
                    TotalParametersSynchronized = TotalParametersChecked + @TotalParamOther,
                    LastUpdatedUserID = @CurrentUserID,
                    SyncedDate = @CurrentDate
                WHERE ID = @OperationTimeLineID
            END

            SET @OperationTimeLineID = NULL
            SET @TimeLineIndex = @TimeLineIndex + 1
        END

        -- COMMIT TRANSACTION nếu mọi thứ diễn ra suôn sẻ
        COMMIT TRANSACTION
        SET @Result = 'SUCCESS'
    END
    ELSE
    BEGIN
        SET @Result = 'SUCCESS' -- Nếu không có khung giờ nào được chọn, coi đây là thành công
    END

    -- Kết quả tạo danh sách thông số
    SELECT @Result AS Message
END
--CR: #8

GO
--CR: #24
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'WorkTicket'
                 AND COLUMN_NAME = 'TrackingStatusID')
BEGIN
	ALTER TABLE dbo.WorkTicket Add TrackingStatusID INT NULL
END

GO
IF OBJECT_ID (N'WorkTicketTracking', N'U') IS NOT NULL
BEGIN
	DROP TABLE dbo.[WorkTicketTracking]
END
CREATE TABLE [dbo].[WorkTicketTracking] (
	[ID]                   INT            IDENTITY (1, 1) NOT NULL,
	[WorkTicketID]         INT            NOT NULL,
	[RequesterUserID]      INT            NOT NULL,
	[CurrentAllowerUserID] INT            NOT NULL,
	[NewAllowerUserID]     INT            NULL,
	[Content]              NVARCHAR (4000) NULL,
	[Reason]               NVARCHAR (4000) NULL,
	[TrackingStatusID]     INT            NULL,
	[IsDeleted]            BIT            NOT NULL,
	[CreatedUserID]        INT            NULL,
	[InsertedAt]           DATETIME       NOT NULL,
	[LastUpdatedUserID]    INT            NULL,
	[UpdatedAt]            DATETIME       NOT NULL,
	CONSTRAINT [PK_dbo.WorkTicketTracking] PRIMARY KEY CLUSTERED ([ID] ASC),
	CONSTRAINT [FK_dbo.WorkTicketTracking_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
	CONSTRAINT [FK_dbo.WorkTicketTracking_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID]),
	CONSTRAINT [FK_dbo.WorkTicketTracking_dbo.Users_NewAllowerUserID] FOREIGN KEY ([NewAllowerUserID]) REFERENCES [dbo].[Users] ([ID]),
	CONSTRAINT [FK_dbo.WorkTicketTracking_dbo.Users_RequesterUserID] FOREIGN KEY ([RequesterUserID]) REFERENCES [dbo].[Users] ([ID]),
	CONSTRAINT [FK_dbo.WorkTicketTracking_dbo.WorkTicket_WorkTicketID] FOREIGN KEY ([WorkTicketID]) REFERENCES [dbo].[WorkTicket] ([ID])
);
GO

CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
	ON [dbo].[WorkTicketTracking]([CreatedUserID] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
	ON [dbo].[WorkTicketTracking]([LastUpdatedUserID] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_NewAllowerUserID]
	ON [dbo].[WorkTicketTracking]([NewAllowerUserID] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_RequesterUserID]
	ON [dbo].[WorkTicketTracking]([RequesterUserID] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_WorkTicketID]
	ON [dbo].[WorkTicketTracking]([WorkTicketID] ASC);

GO
IF OBJECT_ID (N'TrackingStatus', N'U') IS NOT NULL
BEGIN
	DROP TABLE dbo.[TrackingStatus]
END
CREATE TABLE [dbo].[TrackingStatus](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Status] [nvarchar](255) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedUserID] [int] NULL,
	[InsertedAt] [datetime] NOT NULL,
	[LastUpdatedUserID] [int] NULL,
	[UpdatedAt] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.TrackingStatus] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TrackingStatus]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TrackingStatus_dbo.Users_CreatedUserID] FOREIGN KEY([CreatedUserID])
REFERENCES [dbo].[Users] ([ID])
GO

ALTER TABLE [dbo].[TrackingStatus] CHECK CONSTRAINT [FK_dbo.TrackingStatus_dbo.Users_CreatedUserID]
GO

ALTER TABLE [dbo].[TrackingStatus]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TrackingStatus_dbo.Users_LastUpdatedUserID] FOREIGN KEY([LastUpdatedUserID])
REFERENCES [dbo].[Users] ([ID])
GO

ALTER TABLE [dbo].[TrackingStatus] CHECK CONSTRAINT [FK_dbo.TrackingStatus_dbo.Users_LastUpdatedUserID]
GO

DECLARE @CurrentDate DATETIME = GETDATE()
IF NOT EXISTS (SELECT TOP 1 Status FROM dbo.TrackingStatus WHERE Status = N'Chờ duyệt đổi người cho phép')
BEGIN
	INSERT INTO [dbo].[TrackingStatus](Status, IsDeleted, CreatedUserID, InsertedAt, LastUpdatedUserID, UpdatedAt)
     VALUES(N'Chờ duyệt đổi người cho phép', 0, NULL, @CurrentDate, NULL, @CurrentDate)
END
IF NOT EXISTS (SELECT TOP 1 Status FROM dbo.TrackingStatus WHERE Status = N'Đã duyệt đổi người cho phép')
BEGIN
	INSERT INTO [dbo].[TrackingStatus](Status, IsDeleted, CreatedUserID, InsertedAt, LastUpdatedUserID, UpdatedAt)
     VALUES(N'Đã duyệt đổi người cho phép', 0, NULL, @CurrentDate, NULL, @CurrentDate)
END
IF NOT EXISTS (SELECT TOP 1 Status FROM dbo.TrackingStatus WHERE Status = N'Không duyệt đổi người cho phép')
BEGIN
	INSERT INTO [dbo].[TrackingStatus](Status, IsDeleted, CreatedUserID, InsertedAt, LastUpdatedUserID, UpdatedAt)
     VALUES(N'Không duyệt đổi người cho phép', 0, NULL, @CurrentDate, NULL, @CurrentDate)
END
--CR: #24

--CR: #25
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'StaffInformation'
                 AND COLUMN_NAME = 'IsDirectCommander')
BEGIN
	ALTER TABLE dbo.StaffInformation Add IsDirectCommander BIT NOT NULL DEFAULT(0)
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'StaffInformation'
                 AND COLUMN_NAME = 'CommanderUserID')
BEGIN
	ALTER TABLE dbo.StaffInformation Add CommanderUserID INT NOT NULL DEFAULT(0)
END
--CR: #25

GO
--CR
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'WorkPlacement'
                 AND COLUMN_NAME = 'CommanderUserID')
BEGIN
	ALTER TABLE dbo.WorkPlacement Add CommanderUserID INT NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'WorkPlacement'
                 AND COLUMN_NAME = 'CommanderSignatureOut')
BEGIN
	ALTER TABLE dbo.WorkPlacement Add CommanderSignatureOut INT NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'WorkPlacement'
                 AND COLUMN_NAME = 'AcceptorUserID')
BEGIN
	ALTER TABLE dbo.WorkPlacement Add AcceptorUserID INT NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'WorkPlacement'
                 AND COLUMN_NAME = 'AcceptorStartTime')
BEGIN
	ALTER TABLE dbo.WorkPlacement Add AcceptorStartTime DATETIME NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'WorkPlacement'
                 AND COLUMN_NAME = 'AcceptorEndTime')
BEGIN
	ALTER TABLE dbo.WorkPlacement Add AcceptorEndTime DATETIME NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'WorkPlacement'
                 AND COLUMN_NAME = 'AcceptorSignatureOut')
BEGIN
	ALTER TABLE dbo.WorkPlacement Add AcceptorSignatureOut INT NULL
END
--CR

--CR: #58
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'OperationDiary'
                 AND COLUMN_NAME = 'IsExistsShift')
BEGIN
	ALTER TABLE dbo.OperationDiary Add IsExistsShift BIT NULL
END
--CR: #58

--CR: #55
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'SequenceOperation'
                 AND COLUMN_NAME = 'ParentStep')
BEGIN
	ALTER TABLE dbo.SequenceOperation Add ParentStep INT NULL
END
--CR: #55