USE Elogbook

IF NOT EXISTS (SELECT 1
            FROM sys.indexes I              
            WHERE I.Name = 'IX_Mapping_TimeLine'
             AND I.object_id = OBJECT_ID('dbo.OperationTimeLine'))
BEGIN
	PRINT N'Create IX_Mapping_TimeLine success'
    /****** Object:  Index [IX_Mapping_TimeLine]    Script Date: 5/10/2022 1:08:02 AM ******/
	CREATE NONCLUSTERED INDEX [IX_Mapping_TimeLine] ON [dbo].[OperationTimeLine]
	(
		[TimeLineSyncDataKey] ASC,
		[IsDeleted] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
END

IF NOT EXISTS (SELECT 1
            FROM sys.indexes I              
            WHERE I.Name = 'ID_Mapping_Parameter'
             AND I.object_id = OBJECT_ID('dbo.OperationParameters'))
BEGIN
	PRINT N'Create ID_Mapping_Parameter success'
	/****** Object:  Index [ID_Mapping_Parameter]    Script Date: 5/10/2022 1:44:30 AM ******/
	CREATE NONCLUSTERED INDEX [ID_Mapping_Parameter] ON [dbo].[OperationParameters]
	(
		[SyncDataKey] ASC,
		[ParameterID] ASC,
		[IsDeleted] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
END