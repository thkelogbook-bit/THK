-- ========================================================
-- Tạo dữ liệu mặc định khi mở ưng dụng cho một tổ chức mới
-- ========================================================
CREATE PROCEDURE [dbo].[InitialSystemData]
	@CompanyName NVARCHAR(255),
	--Cần thêm TenantID vào các lệnh insert và trong điều kiện where
	@TenantID NVARCHAR(100),
	@FullName NVARCHAR(255),
	@UserName NVARCHAR(255),
	@Password NVARCHAR(100),
	@Email NVARCHAR(255)
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @UserID INT = NULL, @CurrentDate DATETIME = GETDATE(), @CompanyID INT, @PXVH INT
	------------------Danh mục Sơ đồ tổ chức------------------
	
	INSERT [dbo].[Organization] ([ParentID], [Name], [ShortName], [Level], [CompanyType], [ProductionPlantType], [Active], [IsDeleted], [InsertedAt], [UpdatedAt]) 
								VALUES (NULL, @CompanyName, N'CT', N'Công ty', N'', N'', 1, 0, @CurrentDate, @CurrentDate)
	SET @CompanyID = SCOPE_IDENTITY()
	INSERT [dbo].[Organization] ([ParentID], [Name], [ShortName], [Level], [CompanyType], [ProductionPlantType], [Active], [IsDeleted], [InsertedAt], [UpdatedAt]) 
								VALUES (@CompanyID, N'Phân xưởng vận hành', N'PXVH', N'Phân xưởng', N'', N'Vận hành', 1, 0, @CurrentDate, @CurrentDate)
	
	INSERT [dbo].[Organization] ([ParentID], [Name], [ShortName], [Level], [CompanyType], [ProductionPlantType], [Active], [IsDeleted], [InsertedAt], [UpdatedAt])
								VALUES (NULL, N'Công ty EPS', N'EPS', N'Công ty', N'Công ty sửa chữa độc lập', N'', 1, 0, @CurrentDate, @CurrentDate)
	DECLARE @temCompanyID INT = SCOPE_IDENTITY()
	INSERT [dbo].[Organization] ([ParentID], [Name], [ShortName], [Level], [CompanyType], [ProductionPlantType], [Active], [IsDeleted], [InsertedAt], [UpdatedAt])
								VALUES (@temCompanyID, N'Phân xưởng sửa chữa', N'PXSC', N'Phân xưởng', N'', N'Sửa chữa', 1, 0, @CurrentDate, @CurrentDate)

	--Gán dữ liệu mã Công ty dùng cho những table khác
	SET @PXVH = (SELECT TOP 1 ID FROM [dbo].[Organization] WITH(NOLOCK) WHERE [ShortName] = N'PXVH' AND [Level] = N'Phân xưởng' AND @TenantID = @TenantID)

	------------------Danh mục Sơ đồ tổ chức------------------

	------------------Danh mục Nhà máy------------------------
	
	INSERT INTO [dbo].[Factory] ([FactoryName], [CompanyID], [ProductionPlantID], [Status], [IsDeleted], [InsertedAt], [UpdatedAt])
	VALUES( 
		@CompanyName,
		@CompanyID,
		@PXVH,--Phân xưởng vận hành
		1,
		0,
		@CurrentDate,
		@CurrentDate)
	DECLARE @FactoryID INT = SCOPE_IDENTITY()

	------------------Danh mục Nhà máy------------------------

	------------------Danh mục Vai trò------------------------
	--Tạo vai trò mặc định để phân quyền cho người dùng

	INSERT INTO [dbo].[UserRoles] ([RoleName], [Description], [Active], [DataPermission], [Permissions], [IsAdmin], [IsDeleted], 
									[CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES (N'Quản trị viên', N'Người dùng có tất cả các quyền trên ứng dụng', 1, 1, N'VIEW_TKNV;ADD_TKNV;EDIT_TKNV;DELETE_TKNV;VIEW_NDNHT;ADD_NDNHT;EDIT_NDNHT;DELETE_NDNHT;VIEW_VT;ADD_VT;EDIT_VT;DELETE_VT;VIEW_CVH;ADD_CVH;EDIT_CVH;DELETE_CVH;VIEW_KVH;ADD_KVH;EDIT_KVH;DELETE_KVH;VIEW_CK;ADD_CK;EDIT_CK;DELETE_CK;VIEW_CV;ADD_CV;EDIT_CV;DELETE_CV;VIEW_NTS;ADD_NTS;EDIT_NTS;DELETE_NTS;VIEW_TS;ADD_TS;EDIT_TS;DELETE_TS;VIEW_TM;ADD_TM;EDIT_TM;DELETE_TM;VIEW_SDTC;ADD_SDTC;EDIT_SDTC;DELETE_SDTC;VIEW_NM;ADD_NM;EDIT_NM;DELETE_NM;VIEWONLY_NKVH;VIEW_NKVH;ADD_NKVH;EDIT_NKVH;DELETE_NKVH;VIEWONLY_TSVH;VIEW_TSVH;EDIT_TSVH;VIEW_ĐKCTĐ;ADD_ĐKCTĐ;EDIT_ĐKCTĐ;DELETE_ĐKCTĐ;VIEW_CPCTĐ;ADD_CPCTĐ;EDIT_CPCTĐ;VIEW_PCTĐ;ADD_PCTĐ;CLOSE_PCTĐ;VIEW_ĐKCTTCNH;ADD_ĐKCTTCNH;EDIT_ĐKCTTCNH;DELETE_ĐKCTTCNH;VIEW_CPCTTCNH;ADD_CPCTTCNH;EDIT_CPCTTCNH;VIEW_PCTTCNH;ADD_PCTTCNH;VIEW_TTTC;EDIT_TTTC;DELETE_TTTC;VIEW_PTTM;ADD_PTTM;EDIT_PTTM;DELETE_PTTM;VIEW_TPTT;ADD_TPTT;EDIT_TPTT;DELETE_TPTT;VIEW_CPTT;EDIT_CPTT;VIEW_PTT;CLOSE_PTT;VIEW_CHHT;EDIT_CHHT;VIEW_PMIS;EDIT_PMIS;QUICK_SEARCH_NKVH;QUICK_SEARCH_TSVH;VIEW_NKVH_MOBILE;ADD_NKVH_MOBILE;EDIT_NKVH_MOBILE;DELETE_NKVH_MOBILE;VIEW_TSVH_MOBILE;EDIT_TSVH_MOBILE;ALLOW_CTĐ_MOBILE;VIEW_CPCTĐ_MOBILE;EDIT_CPCTĐ_MOBILE;VIEW_PCTĐ_MOBILE;EDIT_PCTĐ_MOBILE;LOCK_PCTĐ_MOBILE;ALLOW_CTTCNH_MOBILE;VIEW_CPCTTCNH_MOBILE;EDIT_CPCTTCNH_MOBILE;VIEW_PCTTCNH_MOBILE;EDIT_PCTTCNH_MOBILE;LOCK_PCTTCNH_MOBILE;VIEW_TTTC_MOBILE;EDIT_TTTC_MOBILE;DELETE_TTTC_MOBILE;VIEW_PTT_MOBILE;EDIT_PTT_MOBILE', 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	DECLARE @AdminRoleID INT = SCOPE_IDENTITY()
	INSERT INTO [dbo].[UserRoles] ([RoleName], [Description], [Active], [DataPermission], [Permissions], [IsAdmin], [IsDeleted], 
									[CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES (N'Cập nhật (nội bộ)', N'Nhân viên vận hành', 1, 1, N'VIEWONLY_NKVH;VIEW_NKVH;ADD_NKVH;EDIT_NKVH;DELETE_NKVH;VIEWONLY_TSVH;VIEW_TSVH;EDIT_TSVH;VIEW_ĐKCTĐ;ADD_ĐKCTĐ;EDIT_ĐKCTĐ;DELETE_ĐKCTĐ;VIEW_CPCTĐ;ADD_CPCTĐ;EDIT_CPCTĐ;VIEW_PCTĐ;ADD_PCTĐ;CLOSE_PCTĐ;VIEW_CPCTTCNH;ADD_CPCTTCNH;EDIT_CPCTTCNH;VIEW_PCTTCNH;ADD_PCTTCNH;VIEW_TTTC;EDIT_TTTC;DELETE_TTTC;VIEW_PTTM;ADD_PTTM;EDIT_PTTM;DELETE_PTTM;VIEW_TPTT;ADD_TPTT;EDIT_TPTT;DELETE_TPTT;VIEW_CPTT;EDIT_CPTT;VIEW_PTT;CLOSE_PTT;QUICK_SEARCH_NKVH;QUICK_SEARCH_TSVH;VIEW_NKVH_MOBILE;ADD_NKVH_MOBILE;EDIT_NKVH_MOBILE;DELETE_NKVH_MOBILE;VIEW_TSVH_MOBILE;EDIT_TSVH_MOBILE;ALLOW_CTĐ_MOBILE;VIEW_CPCTĐ_MOBILE;EDIT_CPCTĐ_MOBILE;VIEW_PCTĐ_MOBILE;EDIT_PCTĐ_MOBILE;LOCK_PCTĐ_MOBILE;ALLOW_CTTCNH_MOBILE;VIEW_CPCTTCNH_MOBILE;EDIT_CPCTTCNH_MOBILE;VIEW_PCTTCNH_MOBILE;EDIT_PCTTCNH_MOBILE;LOCK_PCTTCNH_MOBILE;VIEW_TTTC_MOBILE;EDIT_TTTC_MOBILE;DELETE_TTTC_MOBILE;VIEW_PTT_MOBILE;EDIT_PTT_MOBILE;VIEW_ĐKCTTCNH;ADD_ĐKCTTCNH;EDIT_ĐKCTTCNH;DELETE_ĐKCTTCNH;VIEW_PMIS;EDIT_PMIS', 0, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[UserRoles] ([RoleName], [Description], [Active], [DataPermission], [Permissions], [IsAdmin], [IsDeleted], 
									[CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES (N'Cập nhật (toàn hệ thống)', N'Nhân viên vận hành', 1, 2, N'VIEWONLY_NKVH;VIEW_NKVH;ADD_NKVH;EDIT_NKVH;DELETE_NKVH;VIEWONLY_TSVH;VIEW_TSVH;EDIT_TSVH;VIEW_ĐKCTĐ;ADD_ĐKCTĐ;EDIT_ĐKCTĐ;DELETE_ĐKCTĐ;VIEW_CPCTĐ;ADD_CPCTĐ;EDIT_CPCTĐ;VIEW_PCTĐ;ADD_PCTĐ;CLOSE_PCTĐ;VIEW_ĐKCTTCNH;ADD_ĐKCTTCNH;EDIT_ĐKCTTCNH;DELETE_ĐKCTTCNH;VIEW_CPCTTCNH;ADD_CPCTTCNH;EDIT_CPCTTCNH;VIEW_PCTTCNH;ADD_PCTTCNH;VIEW_TTTC;EDIT_TTTC;DELETE_TTTC;VIEW_PTTM;ADD_PTTM;EDIT_PTTM;DELETE_PTTM;VIEW_TPTT;ADD_TPTT;EDIT_TPTT;DELETE_TPTT;VIEW_CPTT;EDIT_CPTT;VIEW_PTT;CLOSE_PTT;VIEW_PMIS;EDIT_PMIS;QUICK_SEARCH_NKVH;QUICK_SEARCH_TSVH;VIEW_NKVH_MOBILE;ADD_NKVH_MOBILE;EDIT_NKVH_MOBILE;DELETE_NKVH_MOBILE;VIEW_TSVH_MOBILE;EDIT_TSVH_MOBILE;ALLOW_CTĐ_MOBILE;VIEW_CPCTĐ_MOBILE;EDIT_CPCTĐ_MOBILE;VIEW_PCTĐ_MOBILE;EDIT_PCTĐ_MOBILE;LOCK_PCTĐ_MOBILE;ALLOW_CTTCNH_MOBILE;VIEW_CPCTTCNH_MOBILE;EDIT_CPCTTCNH_MOBILE;VIEW_PCTTCNH_MOBILE;EDIT_PCTTCNH_MOBILE;LOCK_PCTTCNH_MOBILE;VIEW_TTTC_MOBILE;EDIT_TTTC_MOBILE;DELETE_TTTC_MOBILE;VIEW_PTT_MOBILE;EDIT_PTT_MOBILE', 0, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[UserRoles] ([RoleName], [Description], [Active], [DataPermission], [Permissions], [IsAdmin], [IsDeleted], 
									[CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES (N'Xem (nội bộ)', N'Nhân viên vận hành', 1, 1, N'VIEWONLY_NKVH;VIEW_NKVH;VIEW_TSVH;VIEW_CPCTĐ;VIEW_PCTĐ;VIEW_CPCTTCNH;VIEW_PCTTCNH;VIEW_TTTC;QUICK_SEARCH_NKVH;QUICK_SEARCH_TSVH;VIEW_NKVH_MOBILE;VIEW_TSVH_MOBILE;VIEW_TTTC_MOBILE;VIEWONLY_TSVH;VIEW_ĐKCTĐ;VIEW_ĐKCTTCNH;VIEW_PTTM;VIEW_TPTT;VIEW_CPTT;VIEW_PTT;VIEW_PMIS;VIEW_PTT_MOBILE;VIEW_CPCTTCNH_MOBILE;VIEW_CPCTĐ_MOBILE', 0, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[UserRoles] ([RoleName], [Description], [Active], [DataPermission], [Permissions], [IsAdmin], [IsDeleted], 
									[CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES (N'Xem (toàn hệ thống)', N'Nhân viên vận hành', 1, 2, N'VIEW_NKVH_MOBILE;ADD_NKVH_MOBILE;EDIT_NKVH_MOBILE;DELETE_NKVH_MOBILE;VIEW_TSVH_MOBILE;EDIT_TSVH_MOBILE;ALLOW_CTĐ_MOBILE;VIEW_CPCTĐ_MOBILE;EDIT_CPCTĐ_MOBILE;VIEW_PCTĐ_MOBILE;EDIT_PCTĐ_MOBILE;LOCK_PCTĐ_MOBILE;ALLOW_CTTCNH_MOBILE;VIEW_CPCTTCNH_MOBILE;EDIT_CPCTTCNH_MOBILE;VIEW_PCTTCNH_MOBILE;EDIT_PCTTCNH_MOBILE;LOCK_PCTTCNH_MOBILE;VIEW_TTTC_MOBILE;EDIT_TTTC_MOBILE;DELETE_TTTC_MOBILE;VIEWONLY_NKVH;VIEW_NKVH;VIEWONLY_TSVH;VIEW_TSVH;VIEW_CPCTĐ;VIEW_PCTĐ;VIEW_CPCTTCNH;VIEW_PCTTCNH;VIEW_TTTC;QUICK_SEARCH_NKVH;QUICK_SEARCH_TSVH;QUICK_SEARCH_NKVH;QUICK_SEARCH_TSVH;QUICK_SEARCH_NKVH;QUICK_SEARCH_TSVH', 0, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)
	INSERT INTO [dbo].[UserRoles] ([RoleName], [Description], [Active], [DataPermission], [Permissions], [IsAdmin], [IsDeleted], 
									[CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES (N'Nhân viên sửa chữa', N'Nhân viên của Phân xướng sửa chữa hoặc công ty sửa chữa độc lập (EPS)', 1, 2, N'VIEW_NDNHT;ADD_NDNHT;EDIT_NDNHT;DELETE_NDNHT;VIEW_ĐKCTĐ;ADD_ĐKCTĐ;EDIT_ĐKCTĐ;VIEW_ĐKCTTCNH;ADD_ĐKCTTCNH;EDIT_ĐKCTTCNH;VIEW_CPCTĐ_MOBILE;VIEW_PCTĐ_MOBILE;EDIT_PCTĐ_MOBILE;VIEW_CPCTTCNH_MOBILE;VIEW_PCTTCNH_MOBILE;EDIT_PCTTCNH_MOBILE;VIEW_PTT;CLOSE_PTT;VIEW_CPTT;VIEW_TPTT;ADD_TPTT;EDIT_TPTT;DELETE_TPTT;VIEW_PTTM;ADD_PTTM;EDIT_PTTM;DELETE_PTTM;VIEW_PCTTCNH;VIEW_CPCTTCNH;VIEW_PCTĐ;ADD_PCTĐ;CLOSE_PCTĐ;VIEW_CPCTĐ;VIEW_PTT_MOBILE;EDIT_PTT_MOBILE', 0, 0, @UserID, @CurrentDate, @UserID, @CurrentDate)

	------------------Danh mục Vai trò------------------------

	------------------Danh mục Tài khoản nhân viên------------

	INSERT INTO [dbo].[Users] ([FullName], [UserName], [Password], [PositionName], [DateOfBirth], [PhoneNumber], [Email], [NationalID], [NationalAttachmentID], 
								[ElectricRank], [ElectricRankCard], [ElectricRankAttachmentID], [LaborCard], [LaborRank], [LaborCardAttachmentID], [Signature], 
								[CompanyID], [ProductionPlantID], [TeamID], [GroupID], [FactoryID], [AccountType], [SourceType], [Status], [IsDeleted], 
								[CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
	SELECT 
		@FullName, --Họ và tên
		@UserName, --Tên đăng nhập
		@Password, --[MAT_KHAU],
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
		@PXVH,--Phân xưởng vận hành
		NULL,--Tổ
		NULL,--Đội/nhóm
		@FactoryID,--Nhà máy
		1,--Loại tài khoản
		2,--Nguồn tài khoản
		1,--Trạng thái truy cập
		0,--Đánh dấu dữ liệu xóa hay chưa
		NULL,--Người tạo
		@CurrentDate,--Ngày tạo
		NULL,--Người cập nhật
		@CurrentDate--Ngày cập nhật
	SET @UserID = SCOPE_IDENTITY()

	--Khởi tạo Vai trò "Quản trị viên" cho người dùng
	INSERT INTO [dbo].[RoleInUsers] ([UserRoleID], [UserID], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	SELECT 
		@AdminRoleID, --Mã vai trò
		@UserID, --Mã người dùng
		0, --Xác định dữ liệu xóa/chưa xóa
		@UserID, --Mã người tạo
		@CurrentDate, --Ngày tạo
		@UserID, --Mã người cập nhật
		@CurrentDate --Ngày cập nhật

	------------------Danh mục Tài khoản nhân viên------------

	------------------Danh mục Ca vận hành--------------------
	
	INSERT INTO [dbo].[OperationShift] ([OperationShiftName], [Position], [FactoryID], [StartTime], [EndTime], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
	SELECT N'Ca 1', 1, @FactoryID, 6, 14, 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate
	UNION ALL
	SELECT N'Ca 2', 2, @FactoryID, 14, 22, 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate
	UNION ALL
	SELECT N'Ca 3', 3, @FactoryID, 22, 6, 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate

	------------------Danh mục Ca vận hành--------------------

	------------------Danh mục Kíp vận hành-------------------
	
	INSERT INTO [dbo].[OperationCrew] ([OperationCrewName], [FactoryID], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
	SELECT N'Ca A', @FactoryID, 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate
	UNION ALL
	SELECT N'Ca B', @FactoryID, 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate
	UNION ALL
	SELECT N'Ca C', @FactoryID, 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate
	UNION ALL
	SELECT N'Ca D', @FactoryID, 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate
	UNION ALL
	SELECT N'Ca E', @FactoryID, 1, 0, @UserID, @CurrentDate, @UserID, @CurrentDate

	------------------Danh mục Kíp vận hành-------------------

	------------------Danh mục Chu kỳ-------------------------
	
	--Các chu kỳ
	INSERT INTO dbo.CycleManagement ([CycleName], [Position], [TimeLine], [OpenInputBefore], [BlockInputAfter], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
	SELECT 
		N'Chu kỳ 01 giờ (1-2-3-4)', 
		1, --Thứ tự hiển thị
		'1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24',
		0,
		0,
		1,
		0,
		@UserID,
		@CurrentDate,
		@UserID,
		@CurrentDate
	DECLARE @CycleManagementID INT = SCOPE_IDENTITY()

	--Khung giờ của Chu kỳ
	INSERT INTO [dbo].[TimeLineCycle] ([Hour], [CycleID], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
	SELECT 
		rtrim(ltrim(value)), 
		@CycleManagementID,
		1,
		0,
		@UserID,
		@CurrentDate,
		@UserID,
		@CurrentDate
	FROM string_split((SELECT TOP 1 TimeLine FROM dbo.CycleManagement WHERE ID = @CycleManagementID),',')

	--Cập nhật nhà máy sử dụng chu kỳ
	INSERT INTO [dbo].[CycleFactory] ([CycleID], [FactoryID], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
	SELECT 
		@CycleManagementID,
		--Nhà máy
		@FactoryID,
		0,
		@UserID,
		@CurrentDate,
		@UserID,
		@CurrentDate

	--Các chu kỳ
	INSERT INTO dbo.CycleManagement ([CycleName], [Position], [TimeLine], [OpenInputBefore], [BlockInputAfter], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
	SELECT 
		N'Chu kỳ 02 giờ (1-3-5-7)', 
		1, --Thứ tự hiển thị
		'1, 3, 5, 7, 9, 11, 13, 15, 17, 19, 21, 23',
		0,
		0,
		1,
		0,
		@UserID,
		@CurrentDate,
		@UserID,
		@CurrentDate
	SET @CycleManagementID = SCOPE_IDENTITY()

	--Khung giờ của Chu kỳ
	INSERT INTO [dbo].[TimeLineCycle] ([Hour], [CycleID], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
	SELECT 
		rtrim(ltrim(value)), 
		@CycleManagementID,
		1,
		0,
		@UserID,
		@CurrentDate,
		@UserID,
		@CurrentDate
	FROM string_split((SELECT TOP 1 TimeLine FROM dbo.CycleManagement WHERE ID = @CycleManagementID),',')

	--Cập nhật nhà máy sử dụng chu kỳ
	INSERT INTO [dbo].[CycleFactory] ([CycleID], [FactoryID], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
	SELECT 
		@CycleManagementID,
		--Nhà máy
		@FactoryID,
		0,
		@UserID,
		@CurrentDate,
		@UserID,
		@CurrentDate

	------------------Danh mục Chu kỳ-------------------------
	
END