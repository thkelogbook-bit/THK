CREATE TABLE [dbo].[DeliveryShiftMethod] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [OperationDiaryID]  INT            NOT NULL,
    [Description]       NVARCHAR (MAX) NULL,
    [Attachments]       NVARCHAR (500) NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    [SyncDataKey]       NVARCHAR (100) NULL,
    [AppID]             NVARCHAR (100) NULL,
    [SyncedDate]        DATETIME       NULL,
    [FactoryID]         INT            NULL,
    [DeviceID]          NVARCHAR (100) NULL,
    CONSTRAINT [PK_dbo.DeliveryShiftMethod] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.DeliveryShiftMethod_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.DeliveryShiftMethod_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);




GO
CREATE NONCLUSTERED INDEX [IX_Performance_Search]
    ON [dbo].[DeliveryShiftMethod]([IsDeleted] ASC, [InsertedAt] ASC, [FactoryID] ASC)
    INCLUDE([OperationDiaryID], [Description]);

