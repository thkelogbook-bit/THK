IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND OBJECT_ID = OBJECT_ID('dbo.WorkScheduleDayBreak'))
BEGIN
    CREATE TABLE [dbo].[WorkScheduleDayBreak]
    (
        [ID] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
        [WorkScheduleDayID] INT NULL,
        [BreakName] NVARCHAR(255) NULL,
        [BreakStartTime] TIME(7) NULL,
        [BreakEndTime] TIME(7) NULL,
        [BreakDurationMinutes] INT NOT NULL DEFAULT(0),
        [IsDeleted] BIT NOT NULL DEFAULT(0),
        [CreatedUserID] INT NULL,
        [InsertedAt] DATETIME NOT NULL DEFAULT(GETDATE()),
        [LastUpdatedUserID] INT NULL,
        [UpdatedAt] DATETIME NOT NULL DEFAULT(GETDATE())
    );

    ALTER TABLE [dbo].[WorkScheduleDayBreak]
    ADD CONSTRAINT [FK_WorkScheduleDayBreak_WorkScheduleDay]
    FOREIGN KEY ([WorkScheduleDayID]) 
    REFERENCES [dbo].[WorkScheduleDay]([ID]);

    ALTER TABLE [dbo].[WorkScheduleDayBreak]
    ADD CONSTRAINT [FK_WorkScheduleDayBreak_Users_CreatedUserID]
    FOREIGN KEY ([CreatedUserID]) 
    REFERENCES [dbo].[Users]([ID]);

    ALTER TABLE [dbo].[WorkScheduleDayBreak]
    ADD CONSTRAINT [FK_WorkScheduleDayBreak_Users_LastUpdatedUserID]
    FOREIGN KEY ([LastUpdatedUserID]) 
    REFERENCES [dbo].[Users]([ID]);
END