CREATE TABLE [dbo].[AmDevice] (
    [ID]                     INT             IDENTITY (1, 1) NOT NULL,
    [AmLocationID]           INT             NULL,
    [AssetParent]            NVARCHAR (100)  NULL,
    [AssetDesc]              NVARCHAR (500)  COLLATE SQL_Latin1_General_CP1_CI_AI NOT NULL,
    [AssetCode]              NVARCHAR (100)  NULL,
    [AmDeviceStatusID]       INT             NULL,
    [Active]                 INT             NULL,
    [AmDeviceTypeID]         INT             NULL,
    [AmManufacturerID]       INT             NULL,
    [AmSupplierID]           INT             NULL,
    [AmCountryManufactureID] INT             NULL,
    [OrganizationID]         INT             NULL,
    [SerialNum]              NVARCHAR (100)  NULL,
    [AssetType]              NVARCHAR (200)  NULL,
    [Meter]                  NVARCHAR (2000) NULL,
    [Kss]                    NVARCHAR (255)  NULL,
    [Khvh]                   NVARCHAR (100)  NULL,
    [Note]                   NVARCHAR (2000) NULL,
    [Description]            NVARCHAR (2000) NULL,
    [Drawing]                NTEXT           NULL,
    [AmDeviceCategoryID]     INT             NULL,
    [Images]                 NVARCHAR (100)  NULL,
    [InstallDate]            DATETIME        NULL,
    [OperatingDate]          DATETIME        NULL,
    [IsDeleted]              BIT             NULL,
    [CreatedUserID]          INT             NULL,
    [InsertedAt]             DATETIME        NOT NULL,
    [LastUpdatedUserID]      INT             NULL,
    [UpdatedAt]              DATETIME        NOT NULL,
    [PMISId]                 NVARCHAR (25)   NULL,
    CONSTRAINT [PK_dbo.AmDevice] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.AmDevice_dbo.AmCountryManufacture_AmCountryManufactureID] FOREIGN KEY ([AmCountryManufactureID]) REFERENCES [dbo].[AmCountryManufacture] ([ID]),
    CONSTRAINT [FK_dbo.AmDevice_dbo.AmDeviceCategory_AmDeviceCategoryID] FOREIGN KEY ([AmDeviceCategoryID]) REFERENCES [dbo].[AmDeviceCategory] ([ID]),
    CONSTRAINT [FK_dbo.AmDevice_dbo.AmDeviceStatus_AmDeviceStatusID] FOREIGN KEY ([AmDeviceStatusID]) REFERENCES [dbo].[AmDeviceStatus] ([ID]),
    CONSTRAINT [FK_dbo.AmDevice_dbo.AmDeviceType_AmDeviceTypeID] FOREIGN KEY ([AmDeviceTypeID]) REFERENCES [dbo].[AmDeviceType] ([ID]),
    CONSTRAINT [FK_dbo.AmDevice_dbo.AmLocation_AmLocationID] FOREIGN KEY ([AmLocationID]) REFERENCES [dbo].[AmLocation] ([ID]),
    CONSTRAINT [FK_dbo.AmDevice_dbo.AmManufacturer_AmManufacturerID] FOREIGN KEY ([AmManufacturerID]) REFERENCES [dbo].[AmManufacturer] ([ID]),
    CONSTRAINT [FK_dbo.AmDevice_dbo.AmSupplier_AmSupplierID] FOREIGN KEY ([AmSupplierID]) REFERENCES [dbo].[AmSupplier] ([ID]),
    CONSTRAINT [FK_dbo.AmDevice_dbo.Organization_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [dbo].[Organization] ([ID]),
    CONSTRAINT [FK_dbo.AmDevice_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.AmDevice_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);







GO


GO
CREATE NONCLUSTERED INDEX [IX_AmDeviceTypeID]
    ON [dbo].[AmDevice]([AmDeviceTypeID] ASC);

GO


GO
CREATE NONCLUSTERED INDEX [IX_AmLocationID]
    ON [dbo].[AmDevice]([AmLocationID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[AmDevice]([LastUpdatedUserID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[AmDevice]([CreatedUserID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_OrganizationID]
    ON [dbo].[AmDevice]([OrganizationID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AmSupplierID]
    ON [dbo].[AmDevice]([AmSupplierID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AmManufacturerID]
    ON [dbo].[AmDevice]([AmManufacturerID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AmDeviceStatusID]
    ON [dbo].[AmDevice]([AmDeviceStatusID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AmDeviceCategoryID]
    ON [dbo].[AmDevice]([AmDeviceCategoryID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AmCountryManufactureID]
    ON [dbo].[AmDevice]([AmCountryManufactureID] ASC);

