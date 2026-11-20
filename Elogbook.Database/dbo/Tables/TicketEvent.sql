CREATE TABLE [dbo].[TicketEvent] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [FactoryID]         INT            NOT NULL,
    [DataType]          INT            NOT NULL,
    [EventTypeID]       INT            NOT NULL,
    [EventContent]      NVARCHAR (500) NULL,
    [Active]            INT            NOT NULL,
    [IsSystem]          BIT            NOT NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.TicketEvent] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.TicketEvent_dbo.Factory_FactoryID] FOREIGN KEY ([FactoryID]) REFERENCES [dbo].[Factory] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.TicketEvent_dbo.SystemData_EventTypeID] FOREIGN KEY ([EventTypeID]) REFERENCES [dbo].[SystemData] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.TicketEvent_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.TicketEvent_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);

