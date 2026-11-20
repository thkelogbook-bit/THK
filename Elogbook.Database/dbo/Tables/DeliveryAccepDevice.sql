CREATE TABLE [dbo].[DeliveryAccepDevice] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [TicketID]          INT            NOT NULL,
    [IsBeforeDelivery]  BIT            NOT NULL,
    [DeliveryTime]      DATETIME       NOT NULL,
    [Unit]              NVARCHAR (500) NULL,
    [FullName]          NVARCHAR (255) NULL,
    [Content]           NVARCHAR (MAX) NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    [SyncedDate]        DATETIME       NULL,
    CONSTRAINT [PK_dbo.DeliveryAccepDevice] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.DeliveryAccepDevice_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.DeliveryAccepDevice_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[DeliveryAccepDevice]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[DeliveryAccepDevice]([CreatedUserID] ASC);

