USE Elogbook

--AbnormalIncident
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'AbnormalIncident'
                 AND COLUMN_NAME = 'AppID')
BEGIN
	ALTER TABLE dbo.AbnormalIncident ADD AppID NVARCHAR (100) NULL 
END

--DeliveryReceiveShift
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'DeliveryReceiveShift'
                 AND COLUMN_NAME = 'AppID')
BEGIN
	ALTER TABLE dbo.DeliveryReceiveShift ADD AppID NVARCHAR (100) NULL 
END

--DeliveryShiftMethod

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'DeliveryShiftMethod'
                 AND COLUMN_NAME = 'AppID')
BEGIN
	ALTER TABLE dbo.DeliveryShiftMethod ADD AppID NVARCHAR (100) NULL 
END

--ExistInShift
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'ExistInShift'
                 AND COLUMN_NAME = 'AppID')
BEGIN
	ALTER TABLE dbo.ExistInShift ADD AppID NVARCHAR (100) NULL 
END

--OperationDiary
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'OperationDiary'
                 AND COLUMN_NAME = 'AppID')
BEGIN
	ALTER TABLE dbo.OperationDiary ADD AppID NVARCHAR (100) NULL 
END

IF EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'OperationDiary'
                 AND COLUMN_NAME = 'AbnormalIncidentID')
BEGIN
	ALTER TABLE dbo.OperationDiary DROP COLUMN AbnormalIncidentID
END

IF EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'OperationDiary'
                 AND COLUMN_NAME = 'DeliveryShiftMethodID')
BEGIN
	ALTER TABLE dbo.OperationDiary DROP COLUMN AbnormalIncidentID
END

--OperationDiaryPositionShift
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'OperationDiaryPositionShift'
                 AND COLUMN_NAME = 'AppID')
BEGIN
	ALTER TABLE dbo.OperationDiaryPositionShift ADD AppID NVARCHAR (100) NULL 
END

--OperationSituation
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'OperationSituation'
                 AND COLUMN_NAME = 'AppID')
BEGIN
	ALTER TABLE dbo.OperationSituation ADD AppID NVARCHAR (100) NULL 
END

--OperationParameters
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'OperationParameters'
                 AND COLUMN_NAME = 'AppID')
BEGIN
	ALTER TABLE dbo.OperationParameters ADD AppID NVARCHAR (100) NULL 
END

--OperationTimeLine
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'OperationTimeLine'
                 AND COLUMN_NAME = 'AppID')
BEGIN
	ALTER TABLE dbo.OperationTimeLine ADD AppID NVARCHAR (100) NULL 
END

--RegistrationTicket
IF EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'RegistrationTicket'
                 AND COLUMN_NAME = 'RegulationNumber')
BEGIN
	ALTER TABLE dbo.RegistrationTicket ALTER COLUMN RegulationNumber NVARCHAR (255) NULL 
END

--07052022
--ExistInShift
IF EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'ExistInShift'
                 AND COLUMN_NAME = 'CompletedDateTime')
BEGIN
	ALTER TABLE dbo.ExistInShift ALTER COLUMN CompletedDateTime DATETIME NULL 
END