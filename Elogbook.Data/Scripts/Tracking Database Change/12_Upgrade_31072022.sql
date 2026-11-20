--31072022
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'LeadershipUser'
                 AND COLUMN_NAME = 'LaborCard')
BEGIN
	ALTER TABLE LeadershipUser ADD LaborCard NVARCHAR (255) NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'LeadershipUser'
                 AND COLUMN_NAME = 'LaborRank')
BEGIN
	ALTER TABLE LeadershipUser ADD LaborRank INT NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'Stakeholders'
                 AND COLUMN_NAME = 'LaborCard')
BEGIN
	ALTER TABLE Stakeholders ADD LaborCard NVARCHAR (255) NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'Stakeholders'
                 AND COLUMN_NAME = 'LaborRank')
BEGIN
	ALTER TABLE Stakeholders ADD LaborRank INT NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'Signature'
                 AND COLUMN_NAME = 'SignatureContentByte')
BEGIN
	ALTER TABLE Signature ADD SignatureContentByte VARBINARY (MAX) NULL
END
--31072022