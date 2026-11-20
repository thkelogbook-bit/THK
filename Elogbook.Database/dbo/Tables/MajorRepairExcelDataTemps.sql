CREATE TABLE [dbo].[MajorRepairExcelDataTemps] (
    [ID]                 INT             IDENTITY (1, 1) NOT NULL,
    [MajorRepairID]      INT             NOT NULL,
    [RowIndex]           INT             NOT NULL,
    [WorkContent]        NVARCHAR (4000) NOT NULL,
    [WorkTicketLocation] NVARCHAR (500)  NOT NULL,
    [ScreenType]         INT             NOT NULL,
    [WorkTicketUnitArea] NVARCHAR (500)  NOT NULL,
    [IsDeleted]          BIT             NOT NULL,
    [CreatedUserID]      INT             NULL,
    [InsertedAt]         DATETIME        NOT NULL,
    [LastUpdatedUserID]  INT             NULL,
    [UpdatedAt]          DATETIME        NOT NULL,
    CONSTRAINT [PK_ExcelDataTemps] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.MajorRepairExcelDataTemps_dbo.MajorRepair_MajorRepairStatusID] FOREIGN KEY ([MajorRepairID]) REFERENCES [dbo].[MajorRepair] ([ID]),
    CONSTRAINT [FK_dbo.MajorRepairExcelDataTemps_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.MajorRepairExcelDataTemps_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);



