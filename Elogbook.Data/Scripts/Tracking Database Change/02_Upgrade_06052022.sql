USE Elogbook

--06052022
IF NOT EXISTS (SELECT 1
            FROM sys.indexes I              
            WHERE I.Name = 'IDX_Get_Parameters_Performance'
             AND I.object_id = OBJECT_ID('dbo.OperationParameters'))
BEGIN
	/****** Object:  Index [IDX_Get_Parameters_Performance]    Script Date: 5/6/2022 11:00:59 AM ******/
	CREATE NONCLUSTERED INDEX [IDX_Get_Parameters_Performance] ON [dbo].[OperationParameters]
	(
		[OperationTimeLineID] ASC,
		[FactoryID] ASC,
		[OperationPositionID] ASC,
		[CycleID] ASC,
		[IsDeleted] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END

IF NOT EXISTS (SELECT 1
            FROM sys.indexes I              
            WHERE I.Name = 'IDX_Update_Parameters_Performance'
             AND I.object_id = OBJECT_ID('dbo.OperationParameters'))
BEGIN
	CREATE NONCLUSTERED INDEX [IDX_Update_Parameters_Performance] ON [dbo].[OperationParameters]
	(
		[FactoryID] ASC,
		[OperationPositionID] ASC,
		[CycleID] ASC,
		[OperationTimeLineID] ASC,
		[ParameterID] ASC,
		[IsDeleted] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END
--06052022