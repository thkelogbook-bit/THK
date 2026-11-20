--04072022
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='FK_dbo.OperationDiary_dbo.AbnormalIncident_AbnormalIncidentID')
BEGIN
	ALTER TABLE [dbo].[OperationDiary] DROP CONSTRAINT [FK_dbo.OperationDiary_dbo.AbnormalIncident_AbnormalIncidentID]
END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='FK_dbo.OperationDiary_dbo.DeliveryShiftMethod_DeliveryShiftMethodID')
BEGIN
	ALTER TABLE [dbo].[OperationDiary] DROP CONSTRAINT [FK_dbo.OperationDiary_dbo.DeliveryShiftMethod_DeliveryShiftMethodID]
END

IF EXISTS (SELECT * FROM sys.indexes  WHERE name='IX_AbnormalIncidentID' AND object_id = OBJECT_ID('dbo.OperationDiary'))
BEGIN
	DROP INDEX [IX_AbnormalIncidentID] ON [dbo].[OperationDiary]
END

IF EXISTS (SELECT * FROM sys.indexes  WHERE name='IX_DeliveryShiftMethodID' AND object_id = OBJECT_ID('dbo.OperationDiary'))
BEGIN
	DROP INDEX [IX_DeliveryShiftMethodID] ON [dbo].[OperationDiary]
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
	ALTER TABLE dbo.OperationDiary DROP COLUMN DeliveryShiftMethodID
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'CancelTicketInfo'
                 AND COLUMN_NAME = 'AppID')
BEGIN
	ALTER TABLE dbo.CancelTicketInfo ADD AppID NVARCHAR (100) NULL 
END

IF EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'WorkPlacement'
                 AND COLUMN_NAME = 'Place')
BEGIN
	ALTER TABLE WorkPlacement ALTER COLUMN Place NVARCHAR(MAX) NULL
END
--04072022