--15072022
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'Factory'
                 AND COLUMN_NAME = 'IsUseOperationParameterGroup')
BEGIN
	ALTER TABLE dbo.Factory ADD IsUseOperationParameterGroup BIT NOT NULL DEFAULT (0)

	UPDATE dbo.Factory SET IsUseOperationParameterGroup = 1 
END
--15072022

--18072022
IF NOT EXISTS(SELECT ModuleValue FROM [dbo].[Modules] WHERE ModuleValue = N'PMIS' AND ModuleName = N'Liên kết Mã QLKT' AND SystemType = N'Admin Portal' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[Modules] ([ModuleValue], [ModuleName], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'PMIS', N'Liên kết Mã QLKT', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'VIEW_PMIS' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'VIEW_PMIS', N'Xem Mã QLKT', 1, N'PMIS', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'EDIT_PMIS' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'EDIT_PMIS', N'Cập nhật Mã QLKT', 1, N'PMIS', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END
--18072022

--22072022
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'OperationParameters'
                 AND COLUMN_NAME = 'CheckedDate')
BEGIN
	ALTER TABLE dbo.OperationParameters ADD CheckedDate DATETIME NULL
END

DeLETE T
FROM
(
SELECT DupRank = ROW_NUMBER() OVER (
              PARTITION BY DeviceID
              ORDER BY (SELECT NULL)
            ), *
FROM TrackingSyncMobileData
) AS T
WHERE DupRank > 1
--22072022

--25072022
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'Attachment'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.Attachment ADD SyncedDate DATETIME NULL
END
UPDATE Attachment SET SyncedDate = UpdatedAt

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'CancelTicketInfo'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.CancelTicketInfo ADD SyncedDate DATETIME NULL
END
UPDATE CancelTicketInfo SET SyncedDate = UpdatedAt

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'DeliveryAccepDevice'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.DeliveryAccepDevice ADD SyncedDate DATETIME NULL
END
UPDATE DeliveryAccepDevice SET SyncedDate = UpdatedAt

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'ManipulationTicket'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.ManipulationTicket ADD SyncedDate DATETIME NULL
END
UPDATE ManipulationTicket SET SyncedDate = UpdatedAt

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'OtherRequest'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.OtherRequest ADD SyncedDate DATETIME NULL
END
UPDATE OtherRequest SET SyncedDate = UpdatedAt

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'Participant'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.Participant ADD SyncedDate DATETIME NULL
END
UPDATE Participant SET SyncedDate = UpdatedAt

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'SequenceOperation'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.SequenceOperation ADD SyncedDate DATETIME NULL
END
UPDATE SequenceOperation SET SyncedDate = UpdatedAt

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'AbnormalIncident'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.AbnormalIncident ADD SyncedDate DATETIME NULL
END
UPDATE AbnormalIncident SET SyncedDate = UpdatedAt

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'DeliveryReceiveShift'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.DeliveryReceiveShift ADD SyncedDate DATETIME NULL
END
UPDATE DeliveryReceiveShift SET SyncedDate = UpdatedAt

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'DeliveryShiftMethod'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.DeliveryShiftMethod ADD SyncedDate DATETIME NULL
END
UPDATE DeliveryShiftMethod SET SyncedDate = UpdatedAt

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'ExistInShift'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.ExistInShift ADD SyncedDate DATETIME NULL
END
UPDATE ExistInShift SET SyncedDate = UpdatedAt

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'OperationDiary'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.OperationDiary ADD SyncedDate DATETIME NULL
END
UPDATE OperationDiary SET SyncedDate = UpdatedAt

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'OperationDiaryPositionShift'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.OperationDiaryPositionShift ADD SyncedDate DATETIME NULL
END
UPDATE OperationDiaryPositionShift SET SyncedDate = UpdatedAt

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'OperationSituation'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.OperationSituation ADD SyncedDate DATETIME NULL
END
UPDATE OperationSituation SET SyncedDate = UpdatedAt

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'OperationParameters'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.OperationParameters ADD SyncedDate DATETIME NULL
END
UPDATE OperationParameters SET SyncedDate = UpdatedAt

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'OperationTimeLine'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.OperationTimeLine ADD SyncedDate DATETIME NULL
END
UPDATE OperationTimeLine SET SyncedDate = UpdatedAt

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'AlertDetails'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.AlertDetails ADD SyncedDate DATETIME NULL
END
UPDATE AlertDetails SET SyncedDate = UpdatedAt

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'Parameters'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.Parameters ADD SyncedDate DATETIME NULL
END
UPDATE Parameters SET SyncedDate = UpdatedAt
--25072022