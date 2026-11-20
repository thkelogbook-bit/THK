CREATE TABLE [dbo].[OperationPosition] (
    [ID]                          INT            IDENTITY (1, 1) NOT NULL,
    [OperationPositionName]       NVARCHAR (255) NOT NULL,
    [FactoryID]                   INT            NULL,
    [Active]                      INT            NOT NULL,
    [Quantity]                    INT            NOT NULL,
    [AuthenticationType]          INT            NOT NULL,
    [IsUsingOnlyOneTablet]        BIT            NOT NULL,
    [AllowWriteDiaryPortal]       BIT            NOT NULL,
    [AllowWriteDiaryMobile]       BIT            NOT NULL,
    [AllowWriteParameterPortal]   BIT            NOT NULL,
    [AllowWriteParameterMobile]   BIT            NOT NULL,
    [JobTitle]                    INT            NOT NULL,
    [TemplateDeliveryShiftMethod] NVARCHAR (MAX) NULL,
    [IsDeleted]                   BIT            NOT NULL,
    [CreatedUserID]               INT            NULL,
    [InsertedAt]                  DATETIME       NOT NULL,
    [LastUpdatedUserID]           INT            NULL,
    [UpdatedAt]                   DATETIME       NOT NULL,
    [Position]                    INT            CONSTRAINT [DF__Operation__Posit__2022C2A6] DEFAULT ((0)) NOT NULL,
    [IsConcurrentlyInShift]       BIT            CONSTRAINT [DF__Operation__IsCon__7132C993] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.OperationPosition] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.OperationPosition_dbo.Factory_FactoryID] FOREIGN KEY ([FactoryID]) REFERENCES [dbo].[Factory] ([ID]),
    CONSTRAINT [FK_dbo.OperationPosition_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.OperationPosition_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);






GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[OperationPosition]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FactoryID]
    ON [dbo].[OperationPosition]([FactoryID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[OperationPosition]([CreatedUserID] ASC);

