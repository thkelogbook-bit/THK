--UPDATE Users
--	SET [PositionName] = (select top 1 [PositionName] from users where username = 'tanmv'),
--		[DateOfBirth] = (select top 1 [DateOfBirth] from users where username = 'tanmv'),
--		[NationalID] = (select top 1 [NationalID] from users where username = 'tanmv'),
--		[NationalAttachmentID] = (select top 1 [NationalAttachmentID] from users where username = 'tanmv'),
--		[ElectricRank] = (select top 1 [ElectricRank] from users where username = 'tanmv'),
--		[ElectricRankCard] = (select top 1 [ElectricRankCard] from users where username = 'tanmv'),
--		[ElectricRankAttachmentID] = (select top 1 [ElectricRankAttachmentID] from users where username = 'tanmv'),
--		[LaborCard] = (select top 1 [LaborCard] from users where username = 'tanmv'),
--		[LaborRank] = (select top 1 [LaborRank] from users where username = 'tanmv'),
--		[LaborCardAttachmentID] = (select top 1 [LaborCardAttachmentID] from users where username = 'tanmv'),
--		[Signature] = (select top 1 [Signature] from users where username = 'tanmv')

--Tạo khóa trên DB mới để mapping Nhà máy
ALTER TABLE [dbo].[Factory] ADD ID_NM NVARCHAR(20) NOT NULL
GO

BEGIN TRY
    BEGIN TRANSACTION

	DECLARE @UserID INT, @CurrentDate DATETIME = GETDATE(), @CompanyID INT, @PXVH1 INT, @PXVH2 INT

	------------------Danh mục Sơ đồ tổ chức------------------
	--Công ty nhiệt điện Phú Mỹ có 2 phân xưởng vận hành: Phân xưởng vận hành 1 & Phân xưởng vận hành 2

	SET IDENTITY_INSERT [dbo].[Organization] ON

	INSERT INTO [dbo].[Organization] ([ID], [ParentID], [Name], [ShortName], [Level], [CompanyType], [ProductionPlantType], [Active], [AttachmentID], [IsDeleted], 
										[CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES (1, NULL, N'Công ty nhiệt điện Phú Mỹ', N'PM', N'Công ty', N'', N'', 1, NULL, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)

	INSERT INTO [dbo].[Organization] ([ID], [ParentID], [Name], [ShortName], [Level], [CompanyType], [ProductionPlantType], [Active], [AttachmentID], [IsDeleted], 
										[CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES (2, 1, N'Phân xưởng vận hành 1', N'PXVH1', N'Phân xưởng', N'', N'Vận hành', 1, NULL, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)

	INSERT INTO [dbo].[Organization] ([ID], [ParentID], [Name], [ShortName], [Level], [CompanyType], [ProductionPlantType], [Active], [AttachmentID], [IsDeleted], 
										[CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES (3, 1, N'Phân xưởng vận hành 2', N'PXVH2', N'Phân xưởng', N'', N'Vận hành', 1, NULL, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)

	SET IDENTITY_INSERT [dbo].[Organization] OFF

	--Gán dữ liệu mã Công ty dùng cho những table khác
	SET @CompanyID = (SELECT TOP 1 ID FROM [dbo].[Organization] WITH(NOLOCK) WHERE [Level] = N'Công ty')
	SET @PXVH1 = (SELECT TOP 1 ID FROM [dbo].[Organization] WITH(NOLOCK) WHERE [ShortName] = N'PXVH1' AND [Level] = N'Phân xưởng')
	SET @PXVH2 = (SELECT TOP 1 ID FROM [dbo].[Organization] WITH(NOLOCK) WHERE [ShortName] = N'PXVH2' AND [Level] = N'Phân xưởng')

	------------------Danh mục Sơ đồ tổ chức------------------

	------------------Danh mục Nhà máy------------------

	DELETE [dbo].[Factory]
	DBCC CHECKIDENT ('Factory', RESEED, 1)
	INSERT INTO [dbo].[Factory] ([FactoryName], [CompanyID], [ProductionPlantID], [Status], [IsDeleted], 
									[CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt], ID_NM)
	SELECT 
		[TEN_NM],
		@CompanyID,
		(CASE WHEN [ID_NM] = 'PM1' THEN @PXVH1--Phân xưởng vận hành 1
			WHEN [ID_NM] = 'PM21' THEN @PXVH2--Phân xưởng vận hành 2
			WHEN [ID_NM] = 'PM4' THEN @PXVH1--Phân xưởng vận hành 1
			ELSE 2 END),--Chưa xác định phân xưởng, đang map Phân xưởng vận hành 1
		1,
		0,
		@UserID,
		@CurrentDate,
		@UserID,
		@CurrentDate,
		[ID_NM]
	FROM [NKVH].[dbo].[DM_NHAMAY] WITH(NOLOCK)

	------------------Danh mục Nhà máy------------------

	------------------Danh mục Vai trò------------------
	--Tạo vai trò mặc định để phân quyền cho người dùng

	SET IDENTITY_INSERT [dbo].[UserRoles] ON
	INSERT INTO [dbo].[UserRoles] ([ID], [RoleName], [Description], [Active], [DataPermission], [Permissions], [IsAdmin], [IsDeleted], 
									[CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES (1, N'Quản trị viên', N'Người dùng có tất cả các quyền trên ứng dụng', 1, 1, N'VIEW_TKNV;ADD_TKNV;EDIT_TKNV;DELETE_TKNV;VIEW_GU;ADD_GU;EDIT_GU;DELETE_GU;VIEW_VT;ADD_VT;EDIT_VT;DELETE_VT;VIEW_CVH;ADD_CVH;EDIT_CVH;DELETE_CVH;VIEW_KVH;ADD_KVH;EDIT_KVH;DELETE_KVH;VIEW_CK;ADD_CK;EDIT_CK;DELETE_CK;VIEW_CV;ADD_CV;EDIT_CV;DELETE_CV;VIEW_NTS;ADD_NTS;EDIT_NTS;DELETE_NTS;VIEW_TS;ADD_TS;EDIT_TS;DELETE_TS;VIEW_TM;ADD_TM;EDIT_TM;DELETE_TM;VIEW_SDTC;ADD_SDTC;EDIT_SDTC;DELETE_SDTC;VIEW_NM;ADD_NM;EDIT_NM;DELETE_NM;VIEWONLY_NKVH;VIEW_NKVH;ADD_NKVH;EDIT_NKVH;DELETE_NKVH;VIEWONLY_TSVH;VIEW_TSVH;EDIT_TSVH;VIEW_ĐKCTĐ;ADD_ĐKCTĐ;EDIT_ĐKCTĐ;DELETE_ĐKCTĐ;VIEW_CPCTĐ;ADD_CPCTĐ;EDIT_CPCTĐ;VIEW_PCTĐ;ADD_PCTĐ;CLOSE_PCTĐ;VIEW_ĐKCTTCNH;ADD_ĐKCTTCNH;EDIT_ĐKCTTCNH;DELETE_ĐKCTTCNH;VIEW_CPCTTCNH;ADD_CPCTTCNH;EDIT_CPCTTCNH;VIEW_PCTTCNH;ADD_PCTTCNH;CLOSE_PCTTCNH;VIEW_TTTC;EDIT_TTTC;DELETE_TTTC;VIEW_NKVH_MOBILE;ADD_NKVH_MOBILE;EDIT_NKVH_MOBILE;DELETE_NKVH_MOBILE;VIEW_TSVH_MOBILE;EDIT_TSVH_MOBILE;ALLOW_CTĐ_MOBILE;VIEW_CPCTĐ_MOBILE;EDIT_CPCTĐ_MOBILE;VIEW_PCTĐ_MOBILE;EDIT_PCTĐ_MOBILE;LOCK_PCTĐ_MOBILE;ALLOW_CTTCNH_MOBILE;VIEW_CPCTTCNH_MOBILE;EDIT_CPCTTCNH_MOBILE;VIEW_PCTTCNH_MOBILE;EDIT_PCTTCNH_MOBILE;LOCK_PCTTCNH_MOBILE;VIEW_TTTC_MOBILE;EDIT_TTTC_MOBILE;DELETE_TTTC_MOBILE', 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[UserRoles] ([ID], [RoleName], [Description], [Active], [DataPermission], [Permissions], [IsAdmin], [IsDeleted], 
									[CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES (2, N'Cập nhật (nội bộ)', N'', 1, 1, N'VIEWONLY_NKVH;VIEW_NKVH;ADD_NKVH;EDIT_NKVH;DELETE_NKVH;VIEWONLY_TSVH;VIEW_TSVH;EDIT_TSVH;VIEW_CPCTĐ;ADD_CPCTĐ;EDIT_CPCTĐ;VIEW_PCTĐ;ADD_PCTĐ;CLOSE_PCTĐ;VIEW_CPCTTCNH;ADD_CPCTTCNH;EDIT_CPCTTCNH;VIEW_PCTTCNH;ADD_PCTTCNH;CLOSE_PCTTCNH;VIEW_TTTC;EDIT_TTTC;DELETE_TTTC;VIEW_NKVH_MOBILE;ADD_NKVH_MOBILE;EDIT_NKVH_MOBILE;DELETE_NKVH_MOBILE;VIEW_TSVH_MOBILE;EDIT_TSVH_MOBILE;ALLOW_CTĐ_MOBILE;VIEW_CPCTĐ_MOBILE;EDIT_CPCTĐ_MOBILE;VIEW_PCTĐ_MOBILE;EDIT_PCTĐ_MOBILE;LOCK_PCTĐ_MOBILE;ALLOW_CTTCNH_MOBILE;VIEW_CPCTTCNH_MOBILE;EDIT_CPCTTCNH_MOBILE;VIEW_PCTTCNH_MOBILE;EDIT_PCTTCNH_MOBILE;LOCK_PCTTCNH_MOBILE;VIEW_TTTC_MOBILE;EDIT_TTTC_MOBILE;DELETE_TTTC_MOBILE', 0, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[UserRoles] ([ID], [RoleName], [Description], [Active], [DataPermission], [Permissions], [IsAdmin], [IsDeleted], 
									[CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES (3, N'Cập nhật (toàn hệ thống)', N'', 1, 2, N'VIEWONLY_NKVH;VIEW_NKVH;ADD_NKVH;EDIT_NKVH;DELETE_NKVH;VIEWONLY_TSVH;VIEW_TSVH;EDIT_TSVH;VIEW_CPCTĐ;ADD_CPCTĐ;EDIT_CPCTĐ;VIEW_PCTĐ;ADD_PCTĐ;CLOSE_PCTĐ;VIEW_CPCTTCNH;ADD_CPCTTCNH;EDIT_CPCTTCNH;VIEW_PCTTCNH;ADD_PCTTCNH;CLOSE_PCTTCNH;VIEW_TTTC;EDIT_TTTC;DELETE_TTTC;VIEW_NKVH_MOBILE;ADD_NKVH_MOBILE;EDIT_NKVH_MOBILE;DELETE_NKVH_MOBILE;VIEW_TSVH_MOBILE;EDIT_TSVH_MOBILE;ALLOW_CTĐ_MOBILE;VIEW_CPCTĐ_MOBILE;EDIT_CPCTĐ_MOBILE;VIEW_PCTĐ_MOBILE;EDIT_PCTĐ_MOBILE;LOCK_PCTĐ_MOBILE;ALLOW_CTTCNH_MOBILE;VIEW_CPCTTCNH_MOBILE;EDIT_CPCTTCNH_MOBILE;VIEW_PCTTCNH_MOBILE;EDIT_PCTTCNH_MOBILE;LOCK_PCTTCNH_MOBILE;VIEW_TTTC_MOBILE;EDIT_TTTC_MOBILE;DELETE_TTTC_MOBILE', 0, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[UserRoles] ([ID], [RoleName], [Description], [Active], [DataPermission], [Permissions], [IsAdmin], [IsDeleted], 
									[CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES (4, N'Xem (nội bộ)', N'', 1, 1, N'VIEWONLY_NKVH;VIEW_NKVH;VIEWONLY_TSVH;VIEW_TSVH;VIEW_CPCTĐ;VIEW_PCTĐ;VIEW_CPCTTCNH;VIEW_PCTTCNH;VIEW_NKVH_MOBILE;ADD_NKVH_MOBILE;EDIT_NKVH_MOBILE;DELETE_NKVH_MOBILE;VIEW_TSVH_MOBILE;EDIT_TSVH_MOBILE;ALLOW_CTĐ_MOBILE;VIEW_CPCTĐ_MOBILE;EDIT_CPCTĐ_MOBILE;VIEW_PCTĐ_MOBILE;EDIT_PCTĐ_MOBILE;LOCK_PCTĐ_MOBILE;ALLOW_CTTCNH_MOBILE;VIEW_CPCTTCNH_MOBILE;EDIT_CPCTTCNH_MOBILE;VIEW_PCTTCNH_MOBILE;EDIT_PCTTCNH_MOBILE;LOCK_PCTTCNH_MOBILE;VIEW_TTTC_MOBILE;EDIT_TTTC_MOBILE;DELETE_TTTC_MOBILE;VIEW_TTTC', 0, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[UserRoles] ([ID], [RoleName], [Description], [Active], [DataPermission], [Permissions], [IsAdmin], [IsDeleted], 
									[CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES (5, N'Xem (toàn hệ thống)', N'', 1, 2, N'VIEW_NKVH_MOBILE;ADD_NKVH_MOBILE;EDIT_NKVH_MOBILE;DELETE_NKVH_MOBILE;VIEW_TSVH_MOBILE;EDIT_TSVH_MOBILE;ALLOW_CTĐ_MOBILE;VIEW_CPCTĐ_MOBILE;EDIT_CPCTĐ_MOBILE;VIEW_PCTĐ_MOBILE;EDIT_PCTĐ_MOBILE;LOCK_PCTĐ_MOBILE;ALLOW_CTTCNH_MOBILE;VIEW_CPCTTCNH_MOBILE;EDIT_CPCTTCNH_MOBILE;VIEW_PCTTCNH_MOBILE;EDIT_PCTTCNH_MOBILE;LOCK_PCTTCNH_MOBILE;VIEW_TTTC_MOBILE;EDIT_TTTC_MOBILE;DELETE_TTTC_MOBILE;VIEWONLY_NKVH;VIEW_NKVH;VIEWONLY_TSVH;VIEW_TSVH;VIEW_CPCTĐ;VIEW_PCTĐ;VIEW_CPCTTCNH;VIEW_PCTTCNH;VIEW_TTTC', 0, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	SET IDENTITY_INSERT [dbo].[UserRoles] OFF

	------------------Danh mục Vai trò------------------

	------------------Danh mục Tài khoản nhân viên------------------

	SET IDENTITY_INSERT [dbo].[Users] ON
	INSERT INTO [dbo].[Users] ([ID], [FullName], [UserName], [Password], [PositionName], [DateOfBirth], [PhoneNumber], [Email], [NationalID], [NationalAttachmentID], 
								[ElectricRank], [ElectricRankCard], [ElectricRankAttachmentID], [LaborCard], [LaborRank], [LaborCardAttachmentID], [Signature], 
								[CompanyID], [ProductionPlantID], [TeamID], [GroupID], [FactoryID], [AccountType], [SourceType], [Status], [IsDeleted], 
								[CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
	SELECT 
		[ID_NV], --Mã nhân viên
		[TEN_DD], --Họ và tên
		[TEN_DN], --Tên đăng nhập
		--[MAT_KHAU],
		N'$2a$11$jkxe6LR7JIfm5EixRhZe7eG5zX7lc/5q7hUPPQyqwPXi7mZYJefke',--Mật khẩu:123456
		NULL,--Chức vụ
		NULL,--Ngày sinh
		NULL,--Số điện thoại
		NULL,--Email
		NULL,--Số CMND/CCCD
		0,--Hình ảnh số CMND/CCCD
		NULL,--Bậc an toàn điện
		NULL,--Thẻ an toàn điện
		0,--Hình ảnh thẻ an toàn điện
		NULL,--Thẻ an toàn lao động
		1,--Bậc thợ
		0,--Hình ảnh thẻ an toàn lao động
		NULL,--Chữ ký nhân viên
		@CompanyID,--Công ty
		(CASE WHEN [ID_NM] = 'PM1' THEN @PXVH1--Phân xưởng vận hành 1
			WHEN [ID_NM] = 'PM21' THEN @PXVH2--Phân xưởng vận hành 2
			WHEN [ID_NM] = 'PM4' THEN @PXVH1--Phân xưởng vận hành 1
			ELSE @PXVH1 END),--Chưa xác định phân xưởng, đang map Phân xưởng vận hành 1
		NULL,--Tổ
		NULL,--Đội/nhóm
		(CASE WHEN [ID_NM] = 'PM' THEN 1 ELSE (SELECT TOP 1 ID FROM dbo.Factory WITH(NOLOCK) WHERE [ID_NM] = [NKVH].[dbo].[DM_NHANVIEN].[ID_NM]) END),--Nhà máy
		1,--Loại tài khoản
		2,--Nguồn tài khoản
		1,--Trạng thái truy cập
		0,--Đánh dấu dữ liệu xóa hay chưa
		--[XOA], 
		[ID_NGUOI_TAO], 
		@CurrentDate, 
		[ID_NGUOI_TAO], 
		@CurrentDate
	FROM [NKVH].[dbo].[DM_NHANVIEN] WITH(NOLOCK)
	SET IDENTITY_INSERT [dbo].[Users] OFF

	--Khở tạo Vai trò cho người dùng
	--Danh sách id vai trò
	--1: Quản trị viên
	--2: Cập nhật (nội bộ)
	--3: Cập nhật (toàn hệ thống)
	--4: Xem (nội bộ)
	--5: Xem (toàn hệ thống)
	SET @UserID = (SELECT TOP 1 ID FROM dbo.Users WITH(NOLOCK) WHERE UserName = N'tanmv')
	INSERT INTO [dbo].[RoleInUsers] ([UserRoleID], [UserID], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	SELECT 
		(CASE WHEN N.ID_QUYEN = 1 THEN 1
			WHEN N.ID_QUYEN = 2 THEN (CASE WHEN CAST(ISNULL(N.XEM_PM1, 0) AS INT) + CAST(ISNULL(N.XEM_PM21, 0) AS INT) + CAST(ISNULL(N.XEM_PM4, 0) AS INT) > 1 THEN 3 ELSE 2 END)
			ELSE (CASE WHEN CAST(ISNULL(N.XEM_PM1, 0) AS INT) + CAST(ISNULL(N.XEM_PM21, 0) AS INT) + CAST(ISNULL(N.XEM_PM4, 0) AS INT) > 1 THEN 5 ELSE 4 END) END), --Mã vai trò
		ID, --Mã người dùng
		0, --Xác định dữ liệu xóa/chưa xóa
		@UserID, --Mã người tạo
		@CurrentDate, --Ngày tạo
		@UserID, --Mã người cập nhật
		@CurrentDate --Ngày cập nhật
	FROM [dbo].[Users] U WITH(NOLOCK)
	INNER JOIN [NKVH].[dbo].[DM_NHANVIEN] N WITH(NOLOCK) ON U.ID = N.ID_NV

	------------------Danh mục Tài khoản nhân viên------------------

	------------------Danh mục Cương vị------------------

	SET IDENTITY_INSERT [dbo].[OperationPosition] ON

	INSERT INTO [dbo].[OperationPosition] ([ID], [OperationPositionName], [FactoryID], [Active], [Quantity], [AuthenticationType], [IsUsingOnlyOneTablet], [AllowWriteDiaryPortal], 
											[AllowWriteDiaryMobile], [AllowWriteParameterPortal], [AllowWriteParameterMobile], [JobTitle], [TemplateDeliveryShiftMethod], [IsDeleted],
											[CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
	SELECT 
		[ID_CV], 
		[TEN_CV], 
		(SELECT TOP 1 ID FROM dbo.Factory WITH(NOLOCK) WHERE [ID_NM] = [NKVH].[dbo].[DM_CUONGVI].[ID_NM]),--Nhà máy
		1,
		1,
		--0: Không sử dụng, 1: RFID, 2: QR Code
		CAST([RFID] AS INT), 
		1,
		1,
		1,
		1,
		1,
		--1: Trưởng ca, 2: Trưởng kíp, 3: Vận hành viên
		(CASE WHEN NKVH.dbo.DM_CUONGVI.[TEN_CV] LIKE N'%Trưởng ca%' THEN 1 
			WHEN NKVH.dbo.DM_CUONGVI.[TEN_CV] LIKE N'%Trưởng kíp%' THEN 2
			ELSE 3 END),
		'',
		0,
		@UserID,
		@CurrentDate,
		@UserID,
		@CurrentDate
		--[STT], 
		--[ID_PX] 
	FROM NKVH.dbo.DM_CUONGVI WITH(NOLOCK)

	DECLARE @PositionTemp TABLE(IndexNumber INT IDENTITY(1,1), PositionID INT, FactoryID INT, IsLeadShift BIT)
	INSERT INTO @PositionTemp
	SELECT 
		ID,
		FactoryID,
		(CASE WHEN OperationPositionName LIKE N'%Trưởng ca%' THEN 1 
			  WHEN OperationPositionName LIKE N'%Trưởng kíp%' THEN 1
			  ELSE 0 END)
	FROM [dbo].[OperationPosition] WITH(NOLOCK)

	DECLARE @TotalPosition INT = (SELECT COUNT(1) FROM @PositionTemp),
		@RowIndex INT = 1,
		@PositionID INT,
		@IsLeadShift BIT,
		@FactoryID INT

	WHILE(@RowIndex <= @TotalPosition)
	BEGIN
		SELECT @PositionID = PositionID, @FactoryID = FactoryID, @IsLeadShift = IsLeadShift FROM @PositionTemp WHERE IndexNumber = @RowIndex
		IF(ISNULL(@PositionID,0) > 0)
		BEGIN
			--Cương vị trong ca
			IF(ISNULL(@IsLeadShift, 0) = 1)
			BEGIN
				INSERT INTO [dbo].[PositionInShift] ([ParentID], [OperationPositionID], [IsSystem], [Active], [Quantity], [Position], 
													[IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
				SELECT 
					@PositionID, 
					ID, 
					1,
					1,
					1,
					ROW_NUMBER() OVER(ORDER BY (SELECT 0)), --Thứ tự hiển thị
					0,
					@UserID,
					@CurrentDate,
					@UserID,
					@CurrentDate
				FROM [dbo].[OperationPosition] WITH(NOLOCK) 
				WHERE FactoryID = @FactoryID
				--ORDER BY pos ASC
			END
			ELSE
			BEGIN
				INSERT INTO [dbo].[PositionInShift] ([ParentID], [OperationPositionID], [IsSystem], [Active], [Quantity], [Position], 
													[IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
				SELECT 
					@PositionID, 
					ID, 
					1,
					1,
					1,
					ROW_NUMBER() OVER(ORDER BY (SELECT 0)), --Thứ tự hiển thị
					0,
					@UserID,
					@CurrentDate,
					@UserID,
					@CurrentDate
				FROM [dbo].[OperationPosition] WITH(NOLOCK)
				WHERE FactoryID = @FactoryID AND (ID = @PositionID OR OperationPositionName LIKE N'%Trưởng ca%' OR OperationPositionName LIKE N'%Trưởng kíp%')
				--ORDER BY STT ASC
			END
			--Cương vị trong ca
		END

		SET @RowIndex += 1
		SET @PositionID = NULL
		SET @FactoryID = NULL
		SET @IsLeadShift = NULL
	END

	SET IDENTITY_INSERT [dbo].[OperationPosition] OFF

	------------------Danh mục Cương vị------------------

	------------------Danh mục Ca vận hành------------------

	DECLARE @FactoryTemp TABLE(IndexNumber INT IDENTITY(1,1), FactoryID INT)
	INSERT INTO @FactoryTemp
	SELECT ID FROM dbo.Factory WITH(NOLOCK)
	DECLARE	@TotalFactory INT = (SELECT COUNT(FactoryID) FROM @FactoryTemp),
		@FactoryIDTemp INT
	SET @RowIndex = 1
	WHILE(@RowIndex <= @TotalFactory)
	BEGIN
		SELECT @FactoryIDTemp = FactoryID FROM @FactoryTemp WHERE IndexNumber = @RowIndex
		
		INSERT INTO [dbo].[OperationShift] ([OperationShiftName], [Position], [FactoryID], [StartTime], [EndTime], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
		SELECT 
			[CAVH], 
			--[STT], 
			ROW_NUMBER() OVER(ORDER BY (SELECT 0)), --Thứ tự hiển thị
			@FactoryIDTemp,
			(CASE WHEN ID_CAVH = N'Ca 1' THEN 7
				WHEN ID_CAVH = N'Ca 2' THEN 15
				ELSE 23 END), 
			(CASE WHEN ID_CAVH = N'Ca 1' THEN 15
				WHEN ID_CAVH = N'Ca 2' THEN 23
				ELSE 7 END), 
			1,
			0,
			@UserID,
			@CurrentDate,
			@UserID,
			@CurrentDate
		FROM NKVH.dbo.DM_CAVH WITH(NOLOCK)

		SET @RowIndex += 1
		SET @FactoryIDTemp = NULL
	END

	------------------Danh mục Ca vận hành------------------

	------------------Danh mục Kíp vận hành------------------
	SET @RowIndex = 1
	WHILE(@RowIndex <= @TotalFactory)
	BEGIN
		SELECT @FactoryIDTemp = FactoryID FROM @FactoryTemp WHERE IndexNumber = @RowIndex

		INSERT INTO [dbo].[OperationCrew] ([OperationCrewName], [FactoryID], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
		SELECT N'Ca A', @FactoryIDTemp, 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate
		UNION ALL
		SELECT N'Ca B', @FactoryIDTemp, 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate
		UNION ALL
		SELECT N'Ca C', @FactoryIDTemp, 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate
		UNION ALL
		SELECT N'Ca D', @FactoryIDTemp, 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate
		UNION ALL
		SELECT N'Ca E', @FactoryIDTemp, 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate

		SET @RowIndex += 1
		SET @FactoryIDTemp = NULL
	END

	------------------Danh mục Kíp vận hành------------------

	------------------Danh mục Chu kỳ------------------

	--Các chu kỳ
	SET IDENTITY_INSERT [dbo].[CycleManagement] ON
	INSERT INTO dbo.CycleManagement ([ID], [CycleName], [Position], [TimeLine], [OpenInputBefore], [BlockInputAfter], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
	SELECT 
		[ID_CKY], 
		[TEN_CKY], 
		--[STT], 
		ROW_NUMBER() OVER(ORDER BY (SELECT 0)), --Thứ tự hiển thị
		--[LOAI], 
		--[NGAY_BD],
		(SUBSTRING(( SELECT  ', ' + CAST(ck.[GIO] AS NVARCHAR(10))
		FROM    NKVH.dbo.DM_CHUKY_CHITIET ck WITH(NOLOCK)
		WHERE ID_CKY = DM_CHUKY.[ID_CKY] Order by ck.GIO ASC
		FOR
		XML PATH('')
		), 3, 999)),
		0,
		0,
		1,
		0,
		@UserID,
		@CurrentDate,
		@UserID,
		@CurrentDate
	FROM NKVH.dbo.[DM_CHUKY] WITH(NOLOCK)
	SET IDENTITY_INSERT [dbo].[CycleManagement] OFF

	--Khung giờ của Chu kỳ
	SET IDENTITY_INSERT [dbo].[TimeLineCycle] ON
	INSERT INTO [dbo].[TimeLineCycle] ([ID], [Hour], [CycleID], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
	SELECT 
		[ID_CHITIET], 
		[GIO], 
		[ID_CKY],
		1,
		0,
		@UserID,
		@CurrentDate,
		@UserID,
		@CurrentDate
	FROM NKVH.dbo.[DM_CHUKY_CHITIET] WITH(NOLOCK)
	SET IDENTITY_INSERT [dbo].[TimeLineCycle] OFF

	--Cập nhật nhà máy sử dụng chu kỳ
	SET @RowIndex = 1
	WHILE(@RowIndex <= @TotalFactory)
	BEGIN
		SELECT @FactoryIDTemp = FactoryID FROM @FactoryTemp WHERE IndexNumber = @RowIndex
		INSERT INTO [dbo].[CycleFactory] ([CycleID], [FactoryID], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
		SELECT 
			[ID_CKY],
			--Nhà máy
			@FactoryIDTemp,
			0,
			@UserID,
			@CurrentDate,
			@UserID,
			@CurrentDate
		FROM NKVH.dbo.[DM_CHUKY] WITH(NOLOCK)

		SET @RowIndex += 1
	END

	------------------Danh mục Chu kỳ------------------

	------------------Danh mục Nhóm thông số------------------

	DECLARE @ParameterGroupTemp TABLE(IndexNumber INT IDENTITY(1,1), NHOM_TS NVARCHAR(255))
	INSERT INTO @ParameterGroupTemp
	SELECT DISTINCT 
		NHOM_TS --Tên nhóm thông số
	FROM NKVH.dbo.[DM_THONGSO] WITH(NOLOCK) 
	WHERE LEN(ISNULL(NHOM_TS,'')) > 0
	ORDER BY NHOM_TS
	DECLARE @TotalParameterGroup INT = (SELECT COUNT(IndexNumber) FROM @ParameterGroupTemp),
		@NHOM_TS NVARCHAR(255)

	SET @RowIndex = 1
	WHILE(@RowIndex <= @TotalParameterGroup)
	BEGIN
		SELECT @NHOM_TS = NHOM_TS FROM @ParameterGroupTemp WHERE IndexNumber = @RowIndex

		INSERT INTO [dbo].[ParameterGroup] ([ParameterGroupName], [FactoryID], [OperationPositionID], [CycleID], [Position], [Status], [IsDeleted], 
											[CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
		SELECT TOP 1
			NHOM_TS, --Tên nhóm thông số
			(SELECT TOP 1 ID FROM dbo.Factory WITH(NOLOCK) WHERE [ID_NM] = N'PM4'), --Nhà máy,
			(SELECT TOP 1 ID_CV FROM NKVH.dbo.[DM_THONGSO] WITH(NOLOCK) WHERE NHOM_TS = @NHOM_TS AND ISNULL(ID_CV,0) > 0), 
			(SELECT TOP 1 ID_CKY FROM NKVH.dbo.[DM_THONGSO] WITH(NOLOCK) WHERE NHOM_TS = @NHOM_TS AND ISNULL(ID_CKY,0) > 0),
			(SELECT TOP 1 STT FROM NKVH.dbo.[DM_THONGSO] WITH(NOLOCK) WHERE NHOM_TS = @NHOM_TS),
			[Status] = 1, --Trạng thái sử dụng
			IsDeleted = 0, --Đánh dấu dữ liệu xóa/chưa xóa
			CreatedUserID = @UserID, --Người tạo
			InsertedAt = @CurrentDate, --Ngày tạo
			LastUpdatedUserID = @UserID, --Người cập nhật
			UpdatedAt = @CurrentDate --Ngày cập nhật
		FROM NKVH.dbo.[DM_THONGSO] WITH(NOLOCK) 
		WHERE LEN(ISNULL(NHOM_TS,'')) > 0 AND NHOM_TS = @NHOM_TS
		ORDER BY NHOM_TS

		SET @NHOM_TS = NULL
		SET @RowIndex += 1
	END

	------------------Danh mục Nhóm thông số------------------

	------------------Danh mục Thông số------------------

	SET IDENTITY_INSERT [dbo].[Parameters] ON
	INSERT INTO [dbo].[Parameters] ([ID], [IsGroup], [FactoryID], [OperationPositionID], [CycleID], [ParameterGroupID], [RFIDCode], [QRCode], [ParameterName], [KKSCode], 
									[UnitMeasure], [Setting], [FromRange], [ToRange], [IsNumberType], [QLKTCode], [Status], [Position], [IsDeleted], 
									[CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
	SELECT 
		[ID_TS], --Mã thông số
		(CASE WHEN ISNULL([NHOM_TS],'') = '' THEN 0 ELSE 1 END), --Nhóm thông số
		--Nhà máy
		(SELECT TOP 1 ID FROM dbo.Factory WITH(NOLOCK) WHERE [ID_NM] = cv.[ID_NM]),
		cv.ID_CV, --Cương vị
		ck.ID_CKY, --Chu kỳ
		(SELECT TOP 1 ID FROM [dbo].[ParameterGroup] WITH(NOLOCK) WHERE ParameterGroupName = [NHOM_TS]), --Mã nhóm thông số
		[MA_THE], --Mã RFID
		NULL, --Mã QRCode
		[TEN_TS], --Tên thông số
		[MA_KKS], --Mã KKS
		ISNULL([DVT], ''), --Đơn vị tính
		[SETTING], --Setting
		[RANGE1], --FromRange
		[RANGE2], --ToRange
		(CASE WHEN CAST(ISNULL([GIA_TRI_CHU],'') AS INT) = 1 THEN 0 ELSE 1 END), --Giá trị chữ/số
		[MA_QLKT], --QLKTCode
		1, --Trạng thái sử dụng
		ts.[STT], --Vị trí hiển thị
		CAST(ISNULL(ts.[XOA],'') AS BIT), --Xác định dữ liệu đã xóa
		@UserID, --Người tạo
		@CurrentDate, --Ngày tạo
		@UserID, --Người cập nhật
		@CurrentDate --Ngày cập nhật
	FROM [NKVH].[dbo].[DM_THONGSO] ts WITH(NOLOCK)
	INNER JOIN [NKVH].[dbo].[DM_CUONGVI] cv WITH(NOLOCK) ON ts.ID_CV = cv.ID_CV
	INNER JOIN [NKVH].[dbo].[DM_CHUKY] ck WITH(NOLOCK) ON ts.ID_CKY = ck.ID_CKY
	SET IDENTITY_INSERT [dbo].[Parameters] OFF

	------------------Danh mục Thông số------------------

	------------------Setting thông số------------------

	SET IDENTITY_INSERT [dbo].[AlertDetails] ON
	INSERT INTO [dbo].[AlertDetails] ([ID], [ParameterID], [Alert], [Operator], [Value], [Color], [IsDeleted], 
										[CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
	SELECT 
		[ID_SETTING], --Mã setting
		[ID_TS], --Mã thông số 
		[ALERT], --Alert
		[TOAN_TU], --Operator
		CAST(REPLACE(REPLACE(LTRIM(LTRIM(ISNULL([GIA_TRI], 0))), ',', '.'), ' ', '') AS REAL), --Value
		(CASE WHEN [MAU] = '#ffff33' THEN 'Yellow' ELSE 'Red' END), --Color
		CAST(ISNULL([XOA],'') AS BIT), --Xác định dữ liệu đã xóa
		@UserID, --Người tạo
		@CurrentDate, --Ngày tạo
		@UserID, --Người cập nhật
		@CurrentDate --Ngày cập nhật
	FROM [NKVH].[dbo].[DM_SETTING] WITH(NOLOCK)
	WHERE [ID_TS] IN (SELECT ID FROM [dbo].[Parameters] WITH(NOLOCK))
	SET IDENTITY_INSERT [dbo].[AlertDetails] OFF

	------------------Setting thông số------------------

	------------------Init data hệ thống------------------

	--Các loại phiếu thao tác
	DELETE [dbo].[WorkTicketFeatures]
	DBCC CHECKIDENT ('WorkTicketFeatures', RESEED, 1)
	INSERT INTO [dbo].[WorkTicketFeatures] ([FeatureName], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES (N'Thủy', 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[WorkTicketFeatures] ([FeatureName], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES (N'Cơ', 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[WorkTicketFeatures] ([FeatureName], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES (N'Nhiệt', 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[WorkTicketFeatures] ([FeatureName], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES (N'Hóa', 0, @UserID, @CurrentDate, @UserID, @CurrentDate)

	--Setting
	DELETE [dbo].[AlarmSettings]
	DBCC CHECKIDENT ('AlarmSettings', RESEED, 1)
	INSERT INTO [dbo].[AlarmSettings] ([SettingName], [SettingType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (N'Alarm', 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[AlarmSettings] ([SettingName], [SettingType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (N'Trip', 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[AlarmSettings] ([SettingName], [SettingType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (N'Min1', 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[AlarmSettings] ([SettingName], [SettingType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (N'Min2', 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[AlarmSettings] ([SettingName], [SettingType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (N'Max1', 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[AlarmSettings] ([SettingName], [SettingType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (N'Max2', 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[AlarmSettings] ([SettingName], [SettingType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (N'>', 2, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[AlarmSettings] ([SettingName], [SettingType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (N'>=', 2, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[AlarmSettings] ([SettingName], [SettingType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (N'<', 2, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[AlarmSettings] ([SettingName], [SettingType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (N'<=', 2, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[AlarmSettings] ([SettingName], [SettingType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (N'=', 2, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[AlarmSettings] ([SettingName], [SettingType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (N'Red', 3, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[AlarmSettings] ([SettingName], [SettingType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (N'Blue', 3, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[AlarmSettings] ([SettingName], [SettingType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (N'Yellow', 3, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)

	--Trạng thái Phiếu công tác
	DELETE [dbo].[TicketStatus]
	DBCC CHECKIDENT ('TicketStatus', RESEED, 1)
	INSERT INTO [dbo].[TicketStatus] ([StatusName], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (N'Chưa gửi phiếu đăng ký', 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[TicketStatus] ([StatusName], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (N'Chờ cấp phiếu công tác', 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[TicketStatus] ([StatusName], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (N'Ðã thu hồi', 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[TicketStatus] ([StatusName], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (N'Không được cấp phiếu', 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[TicketStatus] ([StatusName], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (N'Đã cấp phiếu', 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[TicketStatus] ([StatusName], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (N'Ðã cho phép', 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[TicketStatus] ([StatusName], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (N'Ðang thực hiện', 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[TicketStatus] ([StatusName], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (N'Chờ xác nhận hoàn thành', 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[TicketStatus] ([StatusName], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (N'Ðã hoàn thành', 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)

	---Trạng thái phiếu thao tác
	DELETE [dbo].[TicketManipulationStatus]
	DBCC CHECKIDENT ('TicketManipulationStatus', RESEED, 1)
	INSERT INTO [dbo].[TicketManipulationStatus] ([StatusName], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (N'Chưa gửi duyệt phiếu', 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[TicketManipulationStatus] ([StatusName], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (N'Chờ duyệt cấp phiếu', 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[TicketManipulationStatus] ([StatusName], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (N'Đã thu hồi', 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[TicketManipulationStatus] ([StatusName], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (N'Không được duyệt phiếu', 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[TicketManipulationStatus] ([StatusName], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (N'Đã duyệt phiếu', 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[TicketManipulationStatus] ([StatusName], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (N'Chưa thực hiện', 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[TicketManipulationStatus] ([StatusName], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (N'Chờ xác nhận hoàn thành', 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[TicketManipulationStatus] ([StatusName], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (N'Đã hoàn thành', 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[TicketManipulationStatus] ([StatusName], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (N'Bị huỷ', 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)

	TRUNCATE TABLE [dbo].[UserPermission]
	TRUNCATE TABLE [dbo].[Modules]

	INSERT INTO [dbo].[UserPermission]
			   ([PermissionKey]
			   ,[PermissionName]
			   ,[Active]
			   ,[ModuleValue]
			   ,[SystemType]
			   ,[IsDeleted]
			   ,[CreatedUserID]
			   ,[InsertedAt]
			   ,[LastUpdatedUserID]
			   ,[UpdatedAt])
			VALUES
				--Quản lý tài khoản nhân viên
				(N'VIEW_TKNV', N'Danh sách tài khoản nhân viên', 1, N'DM_TKNV', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'ADD_TKNV', N'Thêm mới tài khoản nhân viên', 1, N'DM_TKNV', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'EDIT_TKNV', N'Hiệu chỉnh tài khoản nhân viên', 1, N'DM_TKNV', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'DELETE_TKNV', N'Xóa tài khoản nhân viên', 1, N'DM_TKNV', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý người dùng ngoài hệ thống
				(N'VIEW_NDNHT', N'Danh sách người dùng ngoài hệ thống', 1, N'DM_NDNHT', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'ADD_NDNHT', N'Thêm mới người dùng ngoài hệ thống', 1, N'DM_NDNHT', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'EDIT_NDNHT', N'Hiệu chỉnh người dùng ngoài hệ thống', 1, N'DM_NDNHT', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'DELETE_NDNHT', N'Xóa người dùng ngoài hệ thống', 1, N'DM_NDNHT', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý vai trò
				(N'VIEW_VT', N'Danh sách vai trò', 1, N'DM_VT', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'ADD_VT', N'Thêm mới vai trò', 1, N'DM_VT', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'EDIT_VT', N'Hiệu chỉnh vai trò', 1, N'DM_VT', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'DELETE_VT', N'Xóa vai trò', 1, N'DM_VT', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý ca vận hành
				(N'VIEW_CVH', N'Danh sách ca vận hành', 1, N'DM_CVH', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'ADD_CVH', N'Thêm mới ca vận hành', 1, N'DM_CVH', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'EDIT_CVH', N'Hiệu chỉnh ca vận hành', 1, N'DM_CVH', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'DELETE_CVH', N'Xóa ca vận hành', 1, N'DM_CVH', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý kíp vận hành
				(N'VIEW_KVH', N'Danh sách kíp vận hành', 1, N'DM_KVH', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'ADD_KVH', N'Thêm mới kíp vận hành', 1, N'DM_KVH', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'EDIT_KVH', N'Hiệu chỉnh kíp vận hành', 1, N'DM_KVH', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'DELETE_KVH', N'Xóa kíp vận hành', 1, N'DM_KVH', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý chu kỳ
				(N'VIEW_CK', N'Danh sách chu kỳ', 1, N'DM_CK', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'ADD_CK', N'Thêm mới chu kỳ', 1, N'DM_CK', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'EDIT_CK', N'Hiệu chỉnh chu kỳ', 1, N'DM_CK', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'DELETE_CK', N'Xóa chu kỳ', 1, N'DM_CK', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý cương vị
				(N'VIEW_CV', N'Danh sách cương vị', 1, N'DM_CV', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'ADD_CV', N'Thêm mới cương vị', 1, N'DM_CV', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'EDIT_CV', N'Hiệu chỉnh cương vị', 1, N'DM_CV', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'DELETE_CV', N'Xóa cương vị', 1, N'DM_CV', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý nhóm thông số
				(N'VIEW_NTS', N'Danh sách nhóm thông số', 1, N'DM_NTS', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'ADD_NTS', N'Thêm mới nhóm thông số', 1, N'DM_NTS', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'EDIT_NTS', N'Hiệu chỉnh nhóm thông số', 1, N'DM_NTS', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'DELETE_NTS', N'Xóa nhóm thông số', 1, N'DM_NTS', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý thông số
				(N'VIEW_TS', N'Danh sách thông số', 1, N'DM_TS', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'ADD_TS', N'Thêm mới thông số', 1, N'DM_TS', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'EDIT_TS', N'Hiệu chỉnh thông số', 1, N'DM_TS', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'DELETE_TS', N'Xóa thông số', 1, N'DM_TS', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý tổ máy
				(N'VIEW_TM', N'Danh sách tổ máy', 1, N'DM_TM', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'ADD_TM', N'Thêm mới tổ máy', 1, N'DM_TM', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'EDIT_TM', N'Hiệu chỉnh tổ máy', 1, N'DM_TM', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'DELETE_TM', N'Xóa tổ máy', 1, N'DM_TM', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Sơ đồ tổ chức
				(N'VIEW_SDTC', N'Danh sách sơ đồ tổ chức', 1, N'DM_SDTC', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'ADD_SDTC', N'Thêm mới sơ đồ tổ chức', 1, N'DM_SDTC', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'EDIT_SDTC', N'Hiệu chỉnh sơ đồ tổ chức', 1, N'DM_SDTC', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'DELETE_SDTC', N'Xóa sơ đồ tổ chức', 1, N'DM_SDTC', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý nhà máy
				(N'VIEW_NM', N'Danh sách nhà máy', 1, N'DM_NM', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'ADD_NM', N'Thêm mới nhà máy', 1, N'DM_NM', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'EDIT_NM', N'Hiệu chỉnh nhà máy', 1, N'DM_NM', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'DELETE_NM', N'Xóa nhà máy', 1, N'DM_NM', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Theo dõi nhật ký vận hành
				(N'VIEWONLY_NKVH', N'Chi tiết nhật ký vận hành', 1, N'ONLY_NKVH', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý nhật ký vận hành
				(N'VIEW_NKVH', N'Danh sách nhật ký vận hành', 1, N'NKVH', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'ADD_NKVH', N'Thêm mới nhật ký vận hành', 1, N'NKVH', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'EDIT_NKVH', N'Hiệu chỉnh nhật ký vận hành', 1, N'NKVH', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'DELETE_NKVH', N'Xóa nhật ký vận hành', 1, N'NKVH', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Theo dõi thông số vận hành
				(N'VIEWONLY_TSVH', N'Chi tiết thông số vận hành', 1, N'ONLY_TSVH', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý thông số vận hành
				(N'VIEW_TSVH', N'Danh sách thông số vận hành', 1, N'TSVH', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'EDIT_TSVH', N'Hiệu chỉnh thông số vận hành', 1, N'TSVH', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý Đăng ký công tác Điện
				(N'VIEW_ĐKCTĐ', N'Danh sách đăng ký công tác điện', 1, N'ĐKCTĐ', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'ADD_ĐKCTĐ', N'Thêm mới đăng ký công tác điện', 1, N'ĐKCTĐ', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'EDIT_ĐKCTĐ', N'Hiệu chỉnh đăng ký công tác điện', 1, N'ĐKCTĐ', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'DELETE_ĐKCTĐ', N'Xóa đăng ký công tác điện', 1, N'ĐKCTĐ', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý Cấp phiếu công tác Điện
				(N'VIEW_CPCTĐ', N'Danh sách phiếu đăng ký công tác điện chờ cấp', 1, N'CPCTĐ', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'ADD_CPCTĐ', N'Cấp phiếu công tác điện mới', 1, N'CPCTĐ', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'EDIT_CPCTĐ', N'Xử lý phiếu đăng ký công tác điện', 1, N'CPCTĐ', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý Phiếu công tác Điện
				(N'VIEW_PCTĐ', N'Danh sách phiếu công tác điện', 1, N'PCTĐ', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'ADD_PCTĐ', N'Nhập liệu phiếu công tác điện', 1, N'PCTĐ', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'CLOSE_PCTĐ', N'Kiểm tra hoàn thành phiếu công tác điện', 1, N'PCTĐ', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý Đăng ký công tác Thuỷ-cơ-nhiệt-hoá
				(N'VIEW_ĐKCTTCNH', N'Danh sách đăng ký công tác Thuỷ-cơ-nhiệt-hoá', 1, N'ĐKCTTCNH', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'ADD_ĐKCTTCNH', N'Thêm mới đăng ký công tác Thuỷ-cơ-nhiệt-hoá', 1, N'ĐKCTTCNH', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'EDIT_ĐKCTTCNH', N'Hiệu chỉnh đăng ký công tácThuỷ-cơ-nhiệt-hoá', 1, N'ĐKCTTCNH', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'DELETE_ĐKCTTCNH', N'Xóa đăng ký công tác Thuỷ-cơ-nhiệt-hoá', 1, N'ĐKCTTCNH', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý Cấp phiếu công tác Thuỷ-cơ-nhiệt-hoá
				(N'VIEW_CPCTTCNH', N'Danh sách phiếu đăng ký công tác Thuỷ-cơ-nhiệt-hoá chờ cấp', 1, N'CPCTTCNH', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'ADD_CPCTTCNH', N'Cấp phiếu công tác Thuỷ-cơ-nhiệt-hoá mới', 1, N'CPCTTCNH', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'EDIT_CPCTTCNH', N'Xử lý phiếu đăng ký công tác Thuỷ-cơ-nhiệt-hoá', 1, N'CPCTTCNH', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý Phiếu công tác Thuỷ-cơ-nhiệt-hoá
				(N'VIEW_PCTTCNH', N'Danh sách phiếu công tác Thuỷ-cơ-nhiệt-hoá', 1, N'PCTTCNH', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'ADD_PCTTCNH', N'Nhập liệu phiếu công tác Thuỷ-cơ-nhiệt-hoá', 1, N'PCTTCNH', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý tồn tại trong ca
				(N'VIEW_TTTC', N'Danh sách tồn tại trong ca', 1, N'TTTC', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'EDIT_TTTC', N'Hiệu chỉnh tồn tại trong ca', 1, N'TTTC', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'DELETE_TTTC', N'Xóa tồn tại trong ca', 1, N'TTTC', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý Phiếu thao tác mẫu
				(N'VIEW_PTTM', N'Danh sách phiếu mẫu được tạo', 1, N'PTTM', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'ADD_PTTM', N'Tạo phiếu mẫu', 1, N'PTTM', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'EDIT_PTTM', N'Hiệu chỉnh phiếu mẫu', 1, N'PTTM', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'DELETE_PTTM', N'Xóa phiếu mẫu', 1, N'PTTM', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý Tạo phiếu thao tác
				(N'VIEW_TPTT', N'Danh sách phiếu thao tác được tạo', 1, N'TPTT', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'ADD_TPTT', N'Tạo phiếu thao tác', 1, N'TPTT', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'EDIT_TPTT', N'Hiệu chỉnh phiếu thao tác', 1, N'TPTT', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'DELETE_TPTT', N'Xóa phiếu thao tác', 1, N'TPTT', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý Cấp phiếu thao tác
				(N'VIEW_CPTT', N'Danh sách phiếu thao tác chờ duyệt', 1, N'CPTT', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'EDIT_CPTT', N'Xử lý phiếu thao tác chờ duyệt', 1, N'CPTT', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý Phiếu thao tác
				(N'VIEW_PTT', N'Danh sách phiếu thao tác', 1, N'PTT', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'CLOSE_PTT', N'Kiểm tra giao nhận sau thao tác', 1, N'PTT', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý nhật ký vận hành
				(N'VIEW_NKVH_MOBILE', N'Danh sách nhật ký vận hành', 1, N'NKVH_MOBILE', N'Mobile App', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'ADD_NKVH_MOBILE', N'Thêm mới nhật ký vận hành', 1, N'NKVH_MOBILE', N'Mobile App', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'EDIT_NKVH_MOBILE', N'Hiệu chỉnh nhật ký vận hành', 1, N'NKVH_MOBILE', N'Mobile App', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'DELETE_NKVH_MOBILE', N'Xóa nhật ký vận hành', 1, N'NKVH_MOBILE', N'Mobile App', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý thông số vận hành
				(N'VIEW_TSVH_MOBILE', N'Danh sách thông số vận hành', 1, N'TSVH_MOBILE', N'Mobile App', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'EDIT_TSVH_MOBILE', N'Hiệu chỉnh thông số vận hành', 1, N'TSVH_MOBILE', N'Mobile App', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quyền cho phép công tác điện
				(N'ALLOW_CTĐ_MOBILE', N'Quyền cho phép công tác điện', 1, N'CTĐ_MOBILE', N'Mobile App', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý Phiếu công tác Điện
				(N'VIEW_CPCTĐ_MOBILE', N'Danh sách phiếu công tác điện chờ cho phép', 1, N'PCTĐ_MOBILE', N'Mobile App', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'EDIT_CPCTĐ_MOBILE', N'Cho phép thực hiện công tác điện', 1, N'PCTĐ_MOBILE', N'Mobile App', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'VIEW_PCTĐ_MOBILE', N'Danh sách phiếu công tác điện đang có', 1, N'PCTĐ_MOBILE', N'Mobile App', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'EDIT_PCTĐ_MOBILE', N'Hiệu chỉnh phiếu công tác điện', 1, N'PCTĐ_MOBILE', N'Mobile App', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'LOCK_PCTĐ_MOBILE', N'Khoá phiếu công tác điện', 1, N'PCTĐ_MOBILE', N'Mobile App', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quyền cho phép công tác Thuỷ-cơ-nhiệt-hoá
				(N'ALLOW_CTTCNH_MOBILE', N'Quyền cho phép công tác Thuỷ-cơ-nhiệt-hoá', 1, N'CTTCNH_MOBILE', N'Mobile App', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý Phiếu công tác Thuỷ-cơ-nhiệt-hoá
				(N'VIEW_CPCTTCNH_MOBILE', N'Danh sách phiếu công tác Thuỷ-cơ-nhiệt-hoá chờ cho phép', 1, N'PCTTCNH_MOBILE', N'Mobile App', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'EDIT_CPCTTCNH_MOBILE', N'Cho phép thực hiện công tác Thuỷ-cơ-nhiệt-hoá', 1, N'PCTTCNH_MOBILE', N'Mobile App', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'VIEW_PCTTCNH_MOBILE', N'Danh sách phiếu công tác Thuỷ-cơ-nhiệt-hoá đang có', 1, N'PCTTCNH_MOBILE', N'Mobile App', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'EDIT_PCTTCNH_MOBILE', N'Hiệu chỉnh phiếu công tác Thuỷ-cơ-nhiệt-hoá', 1, N'PCTTCNH_MOBILE', N'Mobile App', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'LOCK_PCTTCNH_MOBILE', N'Khoá phiếu công tác Thuỷ-cơ-nhiệt-hoá', 1, N'PCTTCNH_MOBILE', N'Mobile App', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý tồn tại trong ca
				(N'VIEW_TTTC_MOBILE', N'Danh sách tồn tại trong ca', 1, N'TTTC_MOBILE', N'Mobile App', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'EDIT_TTTC_MOBILE', N'Hiệu chỉnh tồn tại trong ca', 1, N'TTTC_MOBILE', N'Mobile App', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'DELETE_TTTC_MOBILE', N'Xóa tồn tại trong ca', 1, N'TTTC_MOBILE', N'Mobile App', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý Phiếu thao tác
				(N'VIEW_PTT_MOBILE', N'Danh sách phiếu thao tác', 1, N'PTT_MOBILE', N'Mobile App', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				(N'EDIT_PTT_MOBILE', N'Hiệu chỉnh phiếu thao tác', 1, N'PTT_MOBILE', N'Mobile App', 0, @UserID, @CurrentDate, @UserID, @CurrentDate)

	INSERT INTO [dbo].[Modules]
			   ([ModuleValue]
			   ,[ModuleName]
			   ,[SystemType]
			   ,[IsDeleted]
			   ,[CreatedUserID]
			   ,[InsertedAt]
			   ,[LastUpdatedUserID]
			   ,[UpdatedAt])
			VALUES
				--Quản lý tài khoản nhân viên
				(N'DM_TKNV', N'Quản lý tài khoản nhân viên', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý Người dùng ngoài hệ thống
				(N'DM_NDNHT', N'Quản lý Người dùng ngoài hệ thống', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý vai trò
				(N'DM_VT', N'Quản lý vai trò', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý ca vận hành
				(N'DM_CVH', N'Quản lý ca vận hành', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý kíp vận hành
				(N'DM_KVH', N'Quản lý kíp vận hành', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý chu kỳ
				(N'DM_CK', N'Quản lý chu kỳ', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý cương vị
				(N'DM_CV', N'Quản lý cương vị', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý nhóm thông số
				(N'DM_NTS', N'Quản lý nhóm thông số', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý thông số
				(N'DM_TS', N'Quản lý thông số', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý tổ máy
				(N'DM_TM', N'Quản lý tổ máy', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Sơ đồ tổ chức
				(N'DM_SDTC', N'Sơ đồ tổ chức', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý nhà máy
				(N'DM_NM', N'Quản lý nhà máy', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Theo dõi nhật ký vận hành
				(N'ONLY_NKVH', N'Theo dõi nhật ký vận hành', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý nhật ký vận hành
				(N'NKVH', N'Quản lý nhật ký vận hành', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Theo dõi thông số vận hành
				(N'ONLY_TSVH', N'Theo dõi thông số vận hành', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý thông số vận hành
				(N'TSVH', N'Quản lý thông số vận hành', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý Đăng ký công tác Điện
				(N'ĐKCTĐ', N'Quản lý Đăng ký công tác Điện', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý Cấp phiếu công tác Điện
				(N'CPCTĐ', N'Quản lý Cấp phiếu công tác Điện', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý Phiếu công tác Điện
				(N'PCTĐ', N'Quản lý Phiếu công tác Điện', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý Đăng ký công tác Thuỷ-cơ-nhiệt-hoá
				(N'ĐKCTTCNH', N'Quản lý Đăng ký công tác Thuỷ-cơ-nhiệt-hoá', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý Cấp phiếu công tác Thuỷ-cơ-nhiệt-hoá
				(N'CPCTTCNH', N'Quản lý Cấp phiếu công tác Thuỷ-cơ-nhiệt-hoá', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý Phiếu công tác Thuỷ-cơ-nhiệt-hoá
				(N'PCTTCNH', N'Quản lý Phiếu công tác Thuỷ-cơ-nhiệt-hoá', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý tồn tại trong ca
				(N'TTTC', N'Quản lý tồn tại trong ca', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý Phiếu thao tác mẫu
				(N'PTTM', N'Quản lý Phiếu thao tác mẫu', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý Tạo phiếu thao tác
				(N'TPTT', N'Quản lý Tạo phiếu thao tác', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý Cấp phiếu thao tác
				(N'CPTT', N'Quản lý Cấp phiếu thao tác', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý Phiếu thao tác
				(N'PTT', N'Quản lý Phiếu thao tác', N'Admin Portal', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý nhật ký vận hành
				(N'NKVH_MOBILE', N'Quản lý nhật ký vận hành', N'Mobile App', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý thông số vận hành
				(N'TSVH_MOBILE', N'Quản lý thông số vận hành', N'Mobile App', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quyền cho phép công tác điện
				(N'CTĐ_MOBILE', N'Quyền cho phép công tác điện', N'Mobile App', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý Phiếu công tác Điện
				(N'PCTĐ_MOBILE', N'Quản lý Phiếu công tác Điện', N'Mobile App', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quyền cho phép công tác Thuỷ-cơ-nhiệt-hoá
				(N'CTTCNH_MOBILE', N'Quyền cho phép công tác Thuỷ-cơ-nhiệt-hoá', N'Mobile App', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý Phiếu công tác Thuỷ-cơ-nhiệt-hoá
				(N'PCTTCNH_MOBILE', N'Quản lý Phiếu công tác Thuỷ-cơ-nhiệt-hoá', N'Mobile App', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý tồn tại trong ca
				(N'TTTC_MOBILE', N'Quản lý tồn tại trong ca', N'Mobile App', 0, @UserID, @CurrentDate, @UserID, @CurrentDate),
				--Quản lý Phiếu thao tác
				(N'PTT_MOBILE', N'Quản lý Phiếu thao tác', N'Mobile App', 0, @UserID, @CurrentDate, @UserID, @CurrentDate)

	------------------Init data hệ thống------------------

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
    SELECT  ERROR_NUMBER() AS ErrorNumber ,
            ERROR_PROCEDURE() AS ErrorProcedure ,
            ERROR_MESSAGE() AS ErrorMessage

	SET IDENTITY_INSERT [dbo].[Organization] OFF
	SET IDENTITY_INSERT [dbo].[UserRoles] OFF
	SET IDENTITY_INSERT [dbo].[Users] OFF
	SET IDENTITY_INSERT [dbo].[OperationPosition] OFF
	SET IDENTITY_INSERT [dbo].[OperationShift] OFF
	SET IDENTITY_INSERT [dbo].[CycleManagement] OFF
	SET IDENTITY_INSERT [dbo].[TimeLineCycle] OFF
	SET IDENTITY_INSERT [dbo].[Parameters] OFF
	SET IDENTITY_INSERT [dbo].[AlertDetails] OFF

    ROLLBACK TRANSACTION
END CATCH

GO
ALTER TABLE [dbo].[Factory] DROP COLUMN ID_NM