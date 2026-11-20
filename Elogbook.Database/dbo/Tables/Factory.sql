CREATE TABLE [dbo].[Factory] (
    [ID]                           INT            IDENTITY (1, 1) NOT NULL,
    [FactoryName]                  NVARCHAR (255) NOT NULL,
    [CompanyID]                    INT            NOT NULL,
    [ProductionPlantID]            INT            NOT NULL,
    [Status]                       INT            NOT NULL,
    [IsDeleted]                    BIT            NOT NULL,
    [CreatedUserID]                INT            NULL,
    [InsertedAt]                   DATETIME       NOT NULL,
    [LastUpdatedUserID]            INT            NULL,
    [UpdatedAt]                    DATETIME       NOT NULL,
    [PMIS_USERID]                  NVARCHAR (25)  NULL,
    [IsFilterFactory]              BIT            CONSTRAINT [DF__Factory__IsFilte__0C1BC9F9] DEFAULT ((1)) NOT NULL,
    [IsUseOperationParameterGroup] BIT            CONSTRAINT [DF__Factory__IsUseOp__0D0FEE32] DEFAULT ((0)) NOT NULL,
    [ShortName]                    NVARCHAR (100) CONSTRAINT [DF_Factory_ShortName] DEFAULT ('') NOT NULL,
    [IsUseShortNameToTicket]       BIT            CONSTRAINT [DF_Factory_IsUseShortNameToTicket] DEFAULT ((0)) NOT NULL,
    [TicketDescription]            NVARCHAR (255) NULL,
    [PTTFormat]                    NVARCHAR (50)  NULL,
    [PCTDFormat]                   NVARCHAR (50)  NULL,
    [PCTTCNHFormat]                NVARCHAR (50)  NULL,
    [PMIS_ID]                      NVARCHAR (25)  NULL,
    [Email]                        NVARCHAR (255) NULL,
    CONSTRAINT [PK_dbo.Factory] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.Factory_dbo.Organization_CompanyID] FOREIGN KEY ([CompanyID]) REFERENCES [dbo].[Organization] ([ID]),
    CONSTRAINT [FK_dbo.Factory_dbo.Organization_ProductionPlantID] FOREIGN KEY ([ProductionPlantID]) REFERENCES [dbo].[Organization] ([ID]),
    CONSTRAINT [FK_dbo.Factory_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.Factory_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);
















GO
CREATE NONCLUSTERED INDEX [IX_ProductionPlantID]
    ON [dbo].[Factory]([ProductionPlantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[Factory]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[Factory]([CreatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CompanyID]
    ON [dbo].[Factory]([CompanyID] ASC);

