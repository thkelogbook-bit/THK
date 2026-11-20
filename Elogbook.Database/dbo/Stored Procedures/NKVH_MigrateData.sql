CREATE PROCEDURE [dbo].[NKVH_MigrateData]
	@NKVH_Index INT,
	@TotalNKVH INT
AS
BEGIN
	DECLARE @Total INT = (SELECT COUNT(*) FROM dbo.DM_NKVH_Temp)
	IF(@TotalNKVH > @Total)
	SET @TotalNKVH = @Total
	--=1: Tình hình thực hiện
	--=2: Phiếu công tác
	--=3: Tình hình bất thường/sự cố
	--=4: Tình hình giao ca
	--=5: Quản lý tồn tại
	DECLARE 
		@NGAY DATETIME,
		@ID_CV INT,
		@CA_VH NVARCHAR(100),
		@ID_NV INT,
		@FactoryID INT,
		@OperationDiaryID INT,
		@NGAY_TAO DATETIME, 
		@NGAY_DB DATETIME, 
		@VHV_01 NVARCHAR(100), 
		@VHV_02 NVARCHAR(100), 
		@VHV_03 NVARCHAR(100), 
		@VHV_04 NVARCHAR(100), 
		@VHV_05 NVARCHAR(100), 
		@VHV_06 NVARCHAR(100), 
		@VHV_07 NVARCHAR(100), 
		@VHV_08 NVARCHAR(100), 
		@VHV_09 NVARCHAR(100), 
		@VHV_10 NVARCHAR(100), 
		@VHV_11 NVARCHAR(100), 
		@VHV_12 NVARCHAR(100), 
		@XOA NVARCHAR(10), 
		@CA_ABC NVARCHAR(50),
		@ShiftID INT,
		@CrewID INT,
		@DefaultUserID INT,
		@DefaultFactoryID INT,
		@OperationDiaryPositionShiftID INT

	SELECT TOP 1 @DefaultUserID = ID, @DefaultFactoryID = FactoryID FROM dbo.Users WITH(NOLOCK) WHERE UserName = N'thuytt'

	WHILE(@NKVH_Index <= @TotalNKVH)
	BEGIN
		SELECT @NGAY = NGAY, @ID_CV = ID_CV, @CA_VH = CA_VH, @ID_NV = ID_NV FROM DM_NKVH_Temp WHERE NKVH_Index = @NKVH_Index
		SET @ShiftID = (SELECT TOP 1 ID FROM dbo.OperationShift WITH(NOLOCK) WHERE OperationShiftName = @CA_VH)
		--SELECT @ShiftID, @CA_VH

		IF NOT EXISTS(SELECT 1 FROM dbo.OperationDiary OD WITH(NOLOCK)
						WHERE CreatedDate = @NGAY AND OperationPositionID = @ID_CV AND OD.CreatedUserID = @ID_NV AND OD.OperationShiftID = @ShiftID)
		BEGIN
			SELECT TOP 1 
				@FactoryID = NV.FactoryID,
				@CrewID = (SELECT TOP 1 ID FROM dbo.OperationCrew WITH(NOLOCK) WHERE FactoryID = nv.FactoryID AND OperationCrewName = nk.CA_ABC),
				@NGAY_DB = nk.NGAY_DB,
				@NGAY_TAO = nk.NGAY_TAO,
				@VHV_01 = nk.VHV_01,
				@VHV_02 = nk.VHV_02,
				@VHV_03 = nk.VHV_03,
				@VHV_04 = nk.VHV_04,
				@VHV_05 = nk.VHV_05,
				@VHV_06 = nk.VHV_06,
				@VHV_07 = nk.VHV_07,
				@VHV_08 = nk.VHV_08,
				@VHV_09 = nk.VHV_09,
				@VHV_10 = nk.VHV_10,
				@VHV_11 = nk.VHV_11,
				@VHV_12 = nk.VHV_12
			FROM NKVH.dbo.DM_NKVH nk
			INNER JOIN dbo.Users nv WITH(NOLOCK) ON nv.ID = nk.ID_NV
			INNER JOIN NKVH.dbo.DM_CAVH ch WITH(NOLOCK) ON nk.CA_VH = ch.ID_CAVH
			INNER JOIN dbo.OperationShift os WITH(NOLOCK) ON ch.CAVH = os.OperationShiftName
			WHERE NGAY = @NGAY AND ID_CV = @ID_CV AND ch.CAVH = @CA_VH AND ID_NV = @ID_NV AND nk.XOA = '0'

			INSERT INTO dbo.OperationDiary([FactoryID], [CreatedDate], [OperationPositionID], [OperationShiftID], [OperationCrewID], [IsReceived], [IsDelivered], [TimeDelivery], 
										[CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt], IsCanEdit, IsDeleted, [SyncDataKey], [AppID], [SyncedDate])
			VALUES(@FactoryID, @NGAY, @ID_CV, @ShiftID, @CrewID, 0, 0, NULL, @ID_NV, 
				(CASE WHEN @NGAY_TAO IS NULL THEN GETDATE() ELSE @NGAY_TAO END), @ID_NV, 
				(CASE WHEN @NGAY_DB IS NULL THEN GETDATE() ELSE @NGAY_DB END), 0, 0, '', '', (CASE WHEN @NGAY_DB IS NULL THEN GETDATE() ELSE @NGAY_DB END))
			SET @OperationDiaryID = SCOPE_IDENTITY()

			--Cương vị trong ca
			--Trưởng Ca
			DECLARE @AssignedUserID INT, @PositionShiftID INT, @PositionName NVARCHAR(255)
			IF(ISNULL(@VHV_01, '') != '')
			BEGIN
				SET @AssignedUserID = (SELECT TOP 1 ID FROM dbo.Users WHERE FullName = @VHV_01)
				IF(ISNULL(@AssignedUserID, 0) = 0)
					SET @AssignedUserID = @DefaultUserID

				INSERT INTO dbo.OperationDiaryPositionShift([OperationDiaryID], [PositionInShiftID], [Label], [AssignedUserID], [IsDefault], [Position], [IsDeleted], 
					[CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt], [SyncDataKey], [AppID], [SyncedDate])
				SELECT 
					@OperationDiaryID,
					(SELECT ID FROM dbo.PositionInShift WHERE ParentID = @ID_CV AND OperationPositionID = c1.ID_CV),
					c1.TEN_CV,
					@AssignedUserID,
					0,
					1,
					0,
					@ID_NV,
					@NGAY,
					@ID_NV,
					@NGAY,
					'',
					'',
					GETDATE()
				FROM NKVH.dbo.DM_CUONGVI c1
				INNER JOIN dbo.OperationPosition c2 on c1.ID_CV = c2.id 
				WHERE FactoryID = @FactoryID AND c1.STT = 1
				ORDER BY c1.STT ASC
			END

			--Trưởng kíp
			IF(ISNULL(@VHV_02, '') != '')
			BEGIN
				SET @AssignedUserID = (SELECT TOP 1 ID FROM dbo.Users WHERE FullName = @VHV_02)
				IF(ISNULL(@AssignedUserID, 0) = 0)
					SET @AssignedUserID = @DefaultUserID

				INSERT INTO dbo.OperationDiaryPositionShift([OperationDiaryID], [PositionInShiftID], [Label], [AssignedUserID], [IsDefault], [Position], [IsDeleted], 
					[CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt], [SyncDataKey], [AppID], [SyncedDate])
				SELECT 
					@OperationDiaryID,
					(SELECT TOP 1 ID FROM dbo.PositionInShift WHERE ParentID = @ID_CV AND OperationPositionID = @ID_CV),
					c1.TEN_CV,
					@AssignedUserID,
					0,
					1,
					0,
					@ID_NV,
					@NGAY,
					@ID_NV,
					@NGAY,
					'',
					'',
					GETDATE()
				FROM NKVH.dbo.DM_CUONGVI c1
				INNER JOIN dbo.OperationPosition c2 on c1.ID_CV = c2.id 
				WHERE FactoryID = @FactoryID AND c1.ID_CV = @ID_CV
			END

			--Vận hành viên 01
			IF(ISNULL(@VHV_03, '') != '')
			BEGIN
				SET @AssignedUserID = (SELECT TOP 1 ID FROM dbo.Users WHERE FullName = @VHV_03)
				IF(ISNULL(@AssignedUserID, 0) = 0)
					SET @AssignedUserID = @DefaultUserID

				SELECT 
					@PositionShiftID = (SELECT TOP 1 ID FROM dbo.PositionInShift WHERE ParentID = @ID_CV AND OperationPositionID = c1.ID_CV),
					@PositionName = c1.TEN_CV
				FROM NKVH.dbo.DM_CUONGVI c1
				INNER JOIN dbo.OperationPosition c2 on c1.ID_CV = c2.id 
				WHERE FactoryID = @FactoryID AND c1.ID_CV = @ID_CV

				IF(ISNULL(@PositionShiftID, 0) <= 0)
					SELECT @PositionShiftID = p1.ID, @PositionName = p2.OperationPositionName 
					FROM PositionInShift p1 
					INNER JOIN OperationPosition p2 on p1.OperationPositionID = p2.ID
					WHERE ParentID = @ID_CV AND p1.OperationPositionID = @ID_CV AND p2.FactoryID = @FactoryID

				INSERT INTO dbo.OperationDiaryPositionShift([OperationDiaryID], [PositionInShiftID], [Label], [AssignedUserID], [IsDefault], [Position], [IsDeleted], 
					[CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt], [SyncDataKey], [AppID], [SyncedDate])
				VALUES(@OperationDiaryID, @PositionShiftID, @PositionName, @AssignedUserID, 0, 1, 0, @ID_NV, @NGAY, @ID_NV, @NGAY, '', '', GETDATE())
			END

			--Giao/Nhận ca
			SELECT DISTINCT TOP 1 @OperationDiaryPositionShiftID = odps.ID 
			FROM dbo.OperationDiaryPositionShift odps WITH(NOLOCK)
			INNER JOIN dbo.PositionInShift pis WITH(NOLOCK) ON odps.PositionInShiftID = pis.ID
			WHERE pis.OperationPositionID = @ID_CV and odps.OperationDiaryID = @OperationDiaryID

			IF(ISNULL(@OperationDiaryPositionShiftID, 0) > 0)
			BEGIN
				INSERT INTO dbo.DeliveryReceiveShift(OperationDiaryID, OperationDiaryPositionShiftID, SupervisorID, IsDeliveryShift, IsDeleted, CreatedUserID, InsertedAt, LastUpdatedUserID, UpdatedAt, SyncDataKey, AppID, SyncedDate)
				SELECT TOP 1
					@OperationDiaryID, 
					@OperationDiaryPositionShiftID, 
					t2.ID_NV, 
					0, 
					0, 
					@DefaultUserID, 
					NGAY_NHAN, 
					@DefaultUserID, 
					NGAY_NHAN, 
					'', 
					'', 
					NGAY_DB
				FROM NKVH.dbo.GIAO_NHAN_CA t1 WITH(NOLOCK)
				INNER JOIN NKVH.dbo.DM_NHANVIEN t2 WITH(NOLOCK) ON t1.NGUOI_NHAN = t2.TEN_DN
				WHERE CAST(NGAY_VH AS DATETIME) = @NGAY AND ID_CV = @ID_CV AND CA_VH = @CA_VH

				UPDATE od SET IsReceived = 1
				FROM dbo.OperationDiary od
				INNER JOIN dbo.DeliveryReceiveShift drs on od.ID = drs.OperationDiaryID
				WHERE od.ID = @OperationDiaryID

				INSERT INTO dbo.DeliveryReceiveShift(OperationDiaryID, OperationDiaryPositionShiftID, SupervisorID, IsDeliveryShift, IsDeleted, CreatedUserID, InsertedAt, LastUpdatedUserID, UpdatedAt, SyncDataKey, AppID, SyncedDate)
				SELECT TOP 1
					@OperationDiaryID, 
					@OperationDiaryPositionShiftID, 
					t2.ID_NV, 
					1, 
					0, 
					@DefaultUserID, 
					NGAY_GIAO, 
					@DefaultUserID, 
					NGAY_GIAO, 
					'', 
					'', 
					NGAY_DB
				FROM NKVH.dbo.GIAO_NHAN_CA t1 WITH(NOLOCK)
				INNER JOIN NKVH.dbo.DM_NHANVIEN t2 WITH(NOLOCK) ON t1.NGUOI_GIAO = t2.TEN_DN
				WHERE CAST(NGAY_VH AS DATETIME) = @NGAY AND ID_CV = @ID_CV AND CA_VH = @CA_VH

				UPDATE od SET IsDelivered = 1, TimeDelivery = drs.InsertedAt
				FROM dbo.OperationDiary od
				INNER JOIN dbo.DeliveryReceiveShift drs on od.ID = drs.OperationDiaryID
				WHERE od.ID = @OperationDiaryID
			END
			--Giao/Nhận ca

			--Tình hình vận hành
			INSERT INTO dbo.OperationSituation([OperationDiaryID], [Hour], [AssignedFromID], [AssignedFromName], [AssignedToID], [Description], [Attachments], [IsDeleted], 
			[CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt], [SyncDataKey], [AppID], [AssignedToName], [SyncedDate], [FactoryID])
			SELECT 
				@OperationDiaryID,
				CASE WHEN TRY_CAST(REPLACE(REPLACE(ct.GIO, ':', '.'), 'BS ', '') AS FLOAT) IS NULL THEN 0 ELSE TRY_CAST(REPLACE(REPLACE(ct.GIO, ':', '.'), 'BS ', '') AS FLOAT) END,
				(SELECT TOP 1 ID_NV FROM NKVH.dbo.DM_NHANVIEN WHERE TEN_DD = ct.PHIEU_CT),
				ct.PHIEU_CT,
				(SELECT TOP 1 ID_NV FROM NKVH.dbo.DM_NHANVIEN WHERE TEN_DD = ct.NHOM_TRUONG),
				(SELECT REPLACE(ct.NOI_DUNG, N'</br>', CHAR(10))),
				N'',
				TRY_CAST(ct.XOA AS BIT),
				ct.ID_NV,
				ct.NGAY_TAO,
				ct.ID_NV,
				ct.NGAY_DB,
				'',
				'',
				'',
				ct.NGAY_DB,
				@FactoryID
			FROM NKVH.dbo.DM_NKVH_CHITIET ct
			INNER JOIN NKVH.dbo.DM_NHANVIEN nv ON ct.ID_NV = nv.ID_NV
			INNER JOIN NKVH.dbo.DM_CAVH ch WITH(NOLOCK) ON ct.CA_VH = ch.ID_CAVH
			INNER JOIN dbo.OperationShift os WITH(NOLOCK) ON ch.CAVH = os.OperationShiftName
			WHERE LOAI = 1 AND ID_CV = @ID_CV AND NGAY = @NGAY AND ch.CAVH = @CA_VH

			--Bất thường sự cố
			INSERT INTO dbo.AbnormalIncident([OperationDiaryID], [Description], [Attachments], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt], 
											[SyncDataKey], [AppID], [SyncedDate], [FactoryID])
			SELECT TOP 1 
				@OperationDiaryID, 
				(SELECT REPLACE(ct.NOI_DUNG, N'</br>', CHAR(10))), 
				'', 
				TRY_CAST(ct.XOA AS BIT), 
				ct.ID_NV, 
				ct.NGAY_TAO, 
				ct.ID_NV, 
				ct.NGAY_DB,
				'',
				'',
				ct.NGAY_DB,
				@FactoryID
			FROM NKVH.dbo.DM_NKVH_CHITIET ct
			INNER JOIN NKVH.dbo.DM_NHANVIEN nv ON ct.ID_NV = nv.ID_NV
			INNER JOIN NKVH.dbo.DM_CAVH ch WITH(NOLOCK) ON ct.CA_VH = ch.ID_CAVH
			INNER JOIN dbo.OperationShift os WITH(NOLOCK) ON ch.CAVH = os.OperationShiftName
			WHERE LOAI = 3 AND ID_CV = @ID_CV AND NGAY = @NGAY AND ch.CAVH = @CA_VH
			ORDER BY ct.NGAY_DB DESC

			--Phương thức giao ca
			INSERT INTO dbo.DeliveryShiftMethod([OperationDiaryID], [Description], [Attachments], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], 
												[UpdatedAt], [SyncDataKey], [AppID], [SyncedDate], [FactoryID])
			SELECT TOP 1 
				@OperationDiaryID, 
				(SELECT REPLACE(ct.NOI_DUNG, N'</br>', CHAR(10))), 
				'', 
				TRY_CAST(ct.XOA AS BIT), 
				ct.ID_NV, 
				ct.NGAY_TAO, 
				ct.ID_NV, 
				ct.NGAY_DB,
				'',
				'',
				ct.NGAY_DB,
				@FactoryID
			FROM NKVH.dbo.DM_NKVH_CHITIET ct
			INNER JOIN NKVH.dbo.DM_NHANVIEN nv ON ct.ID_NV = nv.ID_NV
			INNER JOIN NKVH.dbo.DM_CAVH ch WITH(NOLOCK) ON ct.CA_VH = ch.ID_CAVH
			INNER JOIN dbo.OperationShift os WITH(NOLOCK) ON ch.CAVH = os.OperationShiftName
			WHERE LOAI = 4 AND ID_CV = @ID_CV AND NGAY = @NGAY AND ch.CAVH = @CA_VH
			ORDER BY ct.NGAY_DB DESC

			--Tồn tại trong ca
			INSERT INTO dbo.ExistInShift([OperationDiaryID], [RegistrationTicketID], [RegistrationTicketName], [TeamLeader], [Area], [Content], [Attachments], 
				[StatusID], [IsDeleteIssueOfShift], [CompletedDateTime], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt], 
				[SyncDataKey], [AppID], [SyncedDate], [FactoryID])
			SELECT 
				@OperationDiaryID, 
				NULL, 
				ct.PHIEU_CT, 
				ct.NHOM_TRUONG, 
				ct.GIO_KHOA_PHIEU, 
				(SELECT REPLACE(ct.NOI_DUNG, N'</br>', CHAR(10))), 
				'', 
				(CASE WHEN ct.KET_QUA = '0' THEN 0 ELSE 2 END), 
				0, 
				ct.NGAY_DB, 
				TRY_CAST(ct.XOA AS BIT), 
				ct.ID_NV, 
				ct.NGAY_TAO, 
				ct.ID_NV, 
				ct.NGAY_DB,
				'',
				'',
				ct.NGAY_DB,
				@FactoryID
			FROM NKVH.dbo.DM_NKVH_CHITIET ct
			INNER JOIN NKVH.dbo.DM_NHANVIEN nv ON ct.ID_NV = nv.ID_NV
			INNER JOIN NKVH.dbo.DM_CAVH ch WITH(NOLOCK) ON ct.CA_VH = ch.ID_CAVH
			INNER JOIN dbo.OperationShift os WITH(NOLOCK) ON ch.CAVH = os.OperationShiftName
			WHERE LOAI = 5 AND ID_CV = @ID_CV AND NGAY = @NGAY AND ch.CAVH = @CA_VH
		END

		SET @NKVH_Index += 1
		SET @NGAY = NULL
		SET @ID_CV = NULL
		SET @CA_VH = NULL
		SET @ID_NV = NULL
		SET @FactoryID = NULL
		SET @OperationDiaryID = NULL
		SET @OperationDiaryPositionShiftID = NULL
	END
END