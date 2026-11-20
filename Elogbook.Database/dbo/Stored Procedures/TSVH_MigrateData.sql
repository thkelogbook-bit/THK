CREATE PROCEDURE [dbo].[TSVH_MigrateData]
	@TSNumber INT,
	@TotalTHONGSO INT
AS
BEGIN
	DECLARE @Total INT = (SELECT COUNT(*) FROM dbo.GHI_THONGSO_Temp)
	IF(@TotalTHONGSO > @Total)
		SET @TotalTHONGSO = @Total
	
	DECLARE @NGAY_CKY DATETIME,
	@ID_CV INT,
	@ID_CKY INT,
	@GIO_GHI INT,
	@DefaultUserID INT,
	@DefaultFactoryID INT,
	@FactoryID INT,
	@UserID INT

	SELECT TOP 1 @DefaultUserID = ID, @DefaultFactoryID = FactoryID FROM dbo.Users WITH(NOLOCK) WHERE UserName = N'thuytt'

	DECLARE @op_temp TABLE([ParameterName] NVARCHAR(255) NOT NULL, [ParameterID] INT NOT NULL, [ParameterGroupID] INT NULL, 
				[OperationTimeLineID] INT NULL, [FactoryID] INT NOT NULL, [OperationPositionID] INT NULL, [CycleID] INT NULL, 
				[CurrentValue] NVARCHAR(MAX) NULL, [KKSCode] NVARCHAR(255) NULL, [UnitMeasure] NVARCHAR(255) NOT NULL, 
				[Setting] NVARCHAR(MAX) NULL, [FromRange] REAL NULL, [ToRange] REAL NULL, [IsNumberType] BIT NOT NULL, 
				[PreviousValue] NVARCHAR(MAX) NULL, [ErrorCode] INT NULL, [IsChecked]  BIT NOT NULL, [AlertDetails] NVARCHAR(MAX) NULL, 
				[IsDeleted] BIT NOT NULL, [CreatedUserID] INT NULL, [InsertedAt] DATETIME NOT NULL, [LastUpdatedUserID] INT NULL, [UpdatedAt] DATETIME NOT NULL)

	CREATE TABLE #TimeLineCycle_temp (TLNumber INT IDENTITY(1,1), [Hour] INT)

	--Duyệt qua các ngày migrate dữ liệu
	WHILE(@TSNumber <= @TotalTHONGSO)
	BEGIN
		select @TSNumber
		--Thông tin ngày, cương vị, chu kỳ ghi thông số vận hành
		SELECT @NGAY_CKY = NGAY_CKY, @ID_CV = ID_CV, @ID_CKY = ID_CKY FROM GHI_THONGSO_Temp WHERE TSNumber = @TSNumber

		IF NOT EXISTS(SELECT 1 FROM dbo.OperationTimeLine WITH(NOLOCK) WHERE InsertedAt = @NGAY_CKY AND OperationPositionID = @ID_CV AND CycleID = @ID_CKY)
		BEGIN
			--Danh sách các khung giờ ghi thông số vận hành trong mỗi chu kỳ
			DELETE #TimeLineCycle_temp
			IF(@TSNumber > 1)
				DBCC CHECKIDENT ('#TimeLineCycle_temp', RESEED, 0)
			INSERT INTO #TimeLineCycle_temp
			SELECT [Hour] FROM dbo.TimeLineCycle WITH(NOLOCK) WHERE CycleID = @ID_CKY ORDER BY [Hour] ASC

			DECLARE @TotalTL INT = (SELECT COUNT([Hour]) FROM #TimeLineCycle_temp),
			@TLNumber INT = 1,
			@Hour INT,
			@OperationTimeLineID INT,
			@TotalParameter INT

			--Duyệt qua từng khung giờ để ghi thông số
			WHILE(@TLNumber <= @TotalTL)
			BEGIN
				SELECT @Hour = [Hour] FROM #TimeLineCycle_temp WHERE TLNumber = @TLNumber

				DELETE FROM @op_temp
				INSERT INTO @op_temp
				SELECT 
					p.ParameterName,
					ts.ID_TS,
					p.ParameterGroupID,
					NULL AS OperationTimeLineID,
					(CASE WHEN u.FactoryID IS NULL THEN @DefaultFactoryID ELSE U.FactoryID END) AS FactoryID,
					ts.ID_CV,--@ID_CV,
					ts.ID_CKY,--@ID_CKY,
					(CASE WHEN p.IsNumberType = 1 THEN (CASE WHEN ts.GIA_TRI = 'null' THEN NULL ELSE ts.GIA_TRI END)
						ELSE (CASE WHEN ts.GIA_TRI_2 = 'F' OR ts.GIA_TRI_2 = '-' OR ts.GIA_TRI_2 = 'S' OR ts.GIA_TRI_2 = 'J' OR ts.GIA_TRI_2 = 'X' 
									THEN NULL ELSE ts.GIA_TRI_2 END) END) AS CurrentValue,
					p.KKSCode,
					p.UnitMeasure,
					p.Setting,
					p.FromRange,
					p.ToRange,
					p.IsNumberType,
					N'',
					(CASE WHEN ts.GIA_TRI_2 = 'F' THEN 1 --Thiết bị bị hỏng (F)
						WHEN ts.GIA_TRI_2 = '-' THEN 2 --Dừng thiết bị (-)
						WHEN ts.GIA_TRI_2 = 'S' THEN 3 --Sim thiết bị (S)
						WHEN ts.GIA_TRI_2 = 'J' THEN 4 --Jump thiết bị (J)
						WHEN ts.GIA_TRI_2 = 'X' THEN 5 --Bận công tác
						ELSE NULL END),
					1,
					N'' AS PreviousValue,
					0,
					(CASE WHEN u.ID IS NULL THEN @DefaultUserID ELSE U.ID END) AS CreatedUserID,
					NGAY_DB,
					(CASE WHEN u.ID IS NULL THEN @DefaultUserID ELSE U.ID END) AS LastUpdatedUserID,
					NGAY_DB
				FROM NKVH.dbo.GHI_THONGSO ts WITH(NOLOCK)
				INNER JOIN dbo.Parameters p WITH(NOLOCK) ON ts.ID_TS = p.ID
				LEFT JOIN dbo.Users u WITH(NOLOCK) ON ts.ID_NV = u.ID
				WHERE ts.NGAY_CKY = @NGAY_CKY AND ts.ID_CV = @ID_CV AND ts.ID_CKY = @ID_CKY AND ts.GIO_GHI = @Hour

				SET @TotalParameter = (SELECT COUNT(ParameterID) FROM @op_temp)
				SELECT TOP 1 @FactoryID = FactoryID, @UserID = @UserID FROM @op_temp
		
				IF(ISNULL(@TotalParameter, 0) > 0)
				BEGIN
					INSERT INTO dbo.OperationTimeLine([TimeLineSyncDataKey], [TimeLineName], [FactoryID], [OperationPositionID], [CycleID], [TotalParametersDefault], 
						[TotalParametersChecked], [TotalParametersSynchronized], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt], [SyncedDate])
					VALUES(N'', 
						@Hour, 
						@FactoryID, 
						@ID_CV, 
						@ID_CKY, 
						@TotalParameter, 
						@TotalParameter, 
						@TotalParameter, 
						0, 
						@UserID,
						@NGAY_CKY, 
						@UserID,
						@NGAY_CKY,
						@NGAY_CKY)

					SET @OperationTimeLineID = SCOPE_IDENTITY()

					INSERT INTO dbo.OperationParameters([ParameterName], [ParameterID], [ParameterGroupID], [OperationTimeLineID], [FactoryID], [OperationPositionID], [CycleID], 
						[CurrentValue], [KKSCode], [UnitMeasure], [Setting], [FromRange], [ToRange], [IsNumberType], [PreviousValue], [ErrorCode], [IsChecked], [AlertDetails], 
						[IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt], [CheckedDate], [SyncedDate])
					SELECT [ParameterName], [ParameterID], [ParameterGroupID], @OperationTimeLineID, [FactoryID], [OperationPositionID], [CycleID], 
						[CurrentValue], [KKSCode], [UnitMeasure], [Setting], [FromRange], [ToRange], [IsNumberType], [PreviousValue], [ErrorCode], [IsChecked], [AlertDetails], 
						[IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt], [InsertedAt], [InsertedAt] FROM @op_temp
				END

				SET @TLNumber += 1
				SET @OperationTimeLineID = NULL
				SET @FactoryID = NULL
				SET @UserID = NULL
				SET @TotalParameter = NULL
				SET @Hour = NULL
			END
		END
		SET @TSNumber += 1
		SET @NGAY_CKY = NULL
		SET @ID_CV = NULL
		SET @ID_CKY = NULL
		SET @GIO_GHI = NULL
	END
	DROP TABLE #TimeLineCycle_temp
END