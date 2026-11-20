
-- ==========================================================================================
-- Đồng bộ dữ liệu từ PMIS sang Elogbook
-- ==========================================================================================
CREATE PROCEDURE [dbo].[SYNC_PMIS_AMDEVICE]
	@JobId INT
AS
BEGIN
	SET NOCOUNT ON

	--Sync data of Country Code
	MERGE INTO dbo.AmCountryManufacture AS target
	USING dbo.PMIS_G_ERP_COUNTRY AS source
	ON (target.PMISId = source.[status])
	--Xử lý cập nhật dữ liệu khi trùng lặp giữa PMIS và ELogbook
	WHEN MATCHED THEN
		UPDATE SET 
			target.CountryCode = source.COUNTRY_CODE,
			target.CountryName = source.COUNTRY_NAME,
			target.UpdatedAt = GETDATE(),
			target.IsDeleted = 0,
			target.PMISId = source.[status]
	--Xử lý thêm mới dữ liệu nếu không tìm thấy ở Elogbook
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (CountryCode, CountryName, IsDeleted, CreatedUserID, InsertedAt, LastUpdatedUserID, UpdatedAt, PMISId)
		VALUES (source.COUNTRY_CODE, source.COUNTRY_NAME, 0, NULL, GETDATE(), NULL, GETDATE(), source.[status]);
	--Xử lý xóa dữ liệu Elogbook nếu không tìm thấy ở PMIS
	--WHEN NOT MATCHED BY SOURCE THEN
	--    DELETE;

	--Sync data of Manufacturer
	MERGE INTO dbo.AmManufacturer AS target
	USING dbo.PMIS_S_COMPANY AS source
	ON (target.PMISId = source.COMPID AND source.TYPEIDLIST LIKE N'%MNF%')
	--Xử lý cập nhật dữ liệu khi trùng lặp giữa PMIS và ELogbook
	WHEN MATCHED THEN
		UPDATE SET 
			target.ManufacturerCode = source.COMPID,
			target.ManufacturerName = source.COMPDESC,
			target.UpdatedAt = GETDATE(),
			target.IsDeleted = 0,
			target.PMISId = source.COMPID
	--Xử lý thêm mới dữ liệu nếu không tìm thấy ở Elogbook
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (ManufacturerName, ManufacturerCode, IsDeleted, CreatedUserID, InsertedAt, LastUpdatedUserID, UpdatedAt, PMISId)
		VALUES (source.COMPDESC, source.COMPID, 0, NULL, GETDATE(), NULL, GETDATE(), source.COMPID);
	--Xử lý xóa dữ liệu Elogbook nếu không tìm thấy ở PMIS
	--WHEN NOT MATCHED BY SOURCE THEN
	--    DELETE;

	--Sync data of Supplier
	MERGE INTO dbo.AmSupplier AS target
	USING dbo.PMIS_S_COMPANY AS source
	ON (target.PMISId = source.COMPID AND source.TYPEIDLIST LIKE N'%VEN%')
	--Xử lý cập nhật dữ liệu khi trùng lặp giữa PMIS và ELogbook
	WHEN MATCHED THEN
		UPDATE SET 
			target.SupplierCode = source.COMPID,
			target.SupplierName = source.COMPDESC,
			target.UpdatedAt = GETDATE(),
			target.IsDeleted = 0,
			target.PMISId = source.COMPID
	--Xử lý thêm mới dữ liệu nếu không tìm thấy ở Elogbook
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (SupplierName, SupplierCode, IsDeleted, CreatedUserID, InsertedAt, LastUpdatedUserID, UpdatedAt, PMISId)
		VALUES (source.COMPDESC, source.COMPID, 0, NULL, GETDATE(), NULL, GETDATE(), source.COMPID);
	--Xử lý xóa dữ liệu Elogbook nếu không tìm thấy ở PMIS
	--WHEN NOT MATCHED BY SOURCE THEN
	--    DELETE;

	CREATE TABLE #PMIS_A_ASSET_Temp(
		ASSETID NVARCHAR(25) NOT NULL, 
		ASSETID_PARENT NVARCHAR(100) NULL, 
		SITEID NVARCHAR(25) NOT NULL, 
		AmLocationID INT NULL, 
		AmDeviceTypeID INT NULL, 
		AmDeviceStatusID INT NULL, 
		AmDeviceCategoryID INT NULL, 
		AmSupplierID INT NULL, 
		AmManufacturerID INT NULL, 
		AmCountryManufactureID INT NULL, 
		PMISId NVARCHAR(25) NULL, 
		AssetDesc NVARCHAR(500) NOT NULL,
		AssetCode NVARCHAR(100) NULL, 
		Khvh NVARCHAR(100) NULL, 
		Kss NVARCHAR(255) NULL,  
		ASSET_TYPE NVARCHAR(200) NULL, 
		Meter NVARCHAR(2000) NULL, 
		Drawing NTEXT NULL, 
		Note NVARCHAR(2000) NULL, 
		IsDeleted BIT NOT NULL, 
		InsertedAt DATETIME NOT NULL, 
		UpdatedAt DATETIME NOT NULL)

	INSERT INTO #PMIS_A_ASSET_Temp
	SELECT
		ad.ASSETID,
		ad.ASSETID_PARENT,
		ad.SITEID,
		al.ID AS AmLocationID, 
		adt.ID AS AmDeviceTypeID,
		--Trạng thái thiết bị
		1 AS AmDeviceStatusID,
		adc.ID AS AmDeviceCategoryID,
		asp.ID AS AmSupplierID,
		amf.ID AS AmManufacturerID,
		act.ID AS AmCountryManufactureID, 
		ad.ASSETID AS PMISId,
		ad.ASSETDESC AS AssetDesc,
		ad.ASSETID  AS AssetCode,
		ad.KHVH AS Khvh,
		ad.KKS AS Kss,
		ad.ASSET_TYPE AS AssetType,
		ad.METER AS Meter,
		ad.DRAWING AS Drawing,
		ad.NOTE AS Note,
		'False' AS IsDeleted,
		GETDATE() AS InsertedAt,
		GETDATE() AS UpdatedAt
	FROM dbo.PMIS_A_ASSET ad
	LEFT JOIN dbo.AmLocation al WITH(NOLOCK) ON ad.SITEID = al.SiteCode COLLATE SQL_Latin1_General_CP1_CI_AI
	LEFT JOIN dbo.AmDeviceType adt WITH(NOLOCK) ON ad.TYPEID = adt.PMISId
	LEFT JOIN dbo.AmDeviceCategory adc WITH(NOLOCK) ON adc.CategoryCode = ad.CATEGORYID
	LEFT JOIN dbo.AmSupplier asp WITH(NOLOCK) ON asp.PMISId  = ad.P_VENDORID
	LEFT JOIN dbo.AmManufacturer amf WITH(NOLOCK) ON amf.PMISId = ad.P_MANUFACTURERID
	LEFT JOIN dbo.AmCountryManufacture act WITH(NOLOCK) ON act.PMISId = ad.COUNTRY_ID
	WHERE ad.JobId = @JobId

	MERGE INTO dbo.AmDevice AS target
	USING #PMIS_A_ASSET_Temp AS source
	ON (target.PMISId = source.ASSETID)
	--Xử lý cập nhật dữ liệu khi trùng lặp giữa PMIS và ELogbook
	WHEN MATCHED THEN
		UPDATE SET 
			target.AssetParent = source.ASSETID_PARENT, 
			target.AmLocationID = source.AmLocationID, 
			target.AmDeviceTypeID = source.AmDeviceTypeID, 
			target.AmDeviceStatusID = source.AmDeviceStatusID,
			target.AmDeviceCategoryID = source.AmDeviceCategoryID, 
			target.AmSupplierID = source.AmSupplierID, 
			target.AmManufacturerID = source.AmManufacturerID, 
			target.AmCountryManufactureID = source.AmCountryManufactureID, 
			target.PMISId = source.ASSETID, 
			target.AssetDesc = source.ASSETDESC,
			target.AssetCode = source.ASSETID, 
			target.AssetType = source.ASSET_TYPE, 
			target.Meter = source.METER, 
			target.Drawing = source.DRAWING, 
			target.Note = source.NOTE, 
			target.UpdatedAt = GETDATE()
	--Xử lý thêm mới dữ liệu nếu không tìm thấy ở Elogbook
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (
			AssetParent, 
			AmLocationID, 
			AmDeviceTypeID, 
			AmDeviceStatusID, 
			AmDeviceCategoryID, 
			AmSupplierID, 
			AmManufacturerID, 
			AmCountryManufactureID, 
			PMISId, 
			AssetDesc,
			AssetCode, 
			Khvh, 
			Kss, 
			AssetType, 
			Meter, 
			Drawing, 
			Note, 
			Active, 
			IsDeleted, 
			InsertedAt, 
			UpdatedAt)
		VALUES (
			source.ASSETID_PARENT, 
			source.AmLocationID, 
			source.AmDeviceTypeID, 
			source.AmDeviceStatusID, 
			source.AmDeviceCategoryID, 
			source.AmSupplierID, 
			source.AmManufacturerID, 
			source.AmCountryManufactureID, 
			source.ASSETID, 
			source.ASSETDESC,
			source.ASSETID, 
			NULL, 
			NULL, 
			source.ASSET_TYPE, 
			source.METER, 
			source.DRAWING, 
			source.NOTE, 
			1, 
			0, 
			GETDATE(), 
			GETDATE()
		);
	--Xử lý xóa dữ liệu Elogbook nếu không tìm thấy ở PMIS
	--WHEN NOT MATCHED BY SOURCE THEN
	--    DELETE;

	DROP TABLE #PMIS_A_ASSET_Temp
	DELETE dbo.PMIS_A_ASSET WHERE JobId = @JobId
	DELETE dbo.PMIS_G_ERP_COUNTRY WHERE JobId = @JobId
	DELETE dbo.PMIS_S_COMPANY WHERE JobId = @JobId
END