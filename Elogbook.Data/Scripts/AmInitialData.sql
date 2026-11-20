USE [Elogbook_Phase2]
GO
------------AmDeviceCategory--------
INSERT INTO [dbo].[AmDeviceCategory]([FactoryID],[CategoryParent],[CategoryName],[CategoryCode],[Active],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt],[PMISId])VALUES(1,0,N'Phân loại thiết bị 01','PLTB01',1,0,508,'2024-03-30 22:14:47.277',508,'2024-03-30 22:14:47.277',NULL)
INSERT INTO [dbo].[AmDeviceCategory]([FactoryID],[CategoryParent],[CategoryName],[CategoryCode],[Active],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt],[PMISId])VALUES(1,0,N'Phân loại thiết bị 02','PLTB02',1,0,508,'2024-03-30 22:14:47.277',508,'2024-03-30 22:14:47.277',NULL)
INSERT INTO [dbo].[AmDeviceCategory]([FactoryID],[CategoryParent],[CategoryName],[CategoryCode],[Active],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt],[PMISId])VALUES(1,0,N'Phân loại thiết bị 03','PLTB03',1,0,508,'2024-03-30 22:14:47.277',508,'2024-03-30 22:14:47.277',NULL)

-----------AmCountryManufacture
INSERT INTO [dbo].[AmCountryManufacture]([CountryName],[CountryCode],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES('Country001','CT001',0,508,'2024-03-10 22:14:47.277',508,'2024-03-10 22:14:47.277')
INSERT INTO [dbo].[AmCountryManufacture]([CountryName],[CountryCode],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES('Country002','CT002',0,508,'2024-03-10 22:14:47.277',508,'2024-03-10 22:14:47.277')
INSERT INTO [dbo].[AmCountryManufacture]([CountryName],[CountryCode],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES('Country003','CT003',0,508,'2024-03-10 22:14:47.277',508,'2024-03-10 22:14:47.277')

-----------AmSupplier
INSERT INTO [dbo].[AmSupplier]([SupplierName],[SupplierCode],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES('Supplier001','SUP001',0,508,'2024-03-10 22:14:47.277',508,'2024-03-10 22:14:47.277')
INSERT INTO [dbo].[AmSupplier]([SupplierName],[SupplierCode],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES('Supplier002','SUP002',0,508,'2024-03-10 22:14:47.277',508,'2024-03-10 22:14:47.277')
INSERT INTO [dbo].[AmSupplier]([SupplierName],[SupplierCode],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES('Supplier003','SUP003',0,508,'2024-03-10 22:14:47.277',508,'2024-03-10 22:14:47.277')

-------AmDeviceAbnormalStatus
INSERT INTO [dbo].[AmDeviceAbnormalStatus]([Name],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES(N'Không có bất thường',0,508,'2024-03-02 17:58:00.023',508,'2024-03-02 17:58:00.023')
INSERT INTO [dbo].[AmDeviceAbnormalStatus]([Name],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES(N'Có bất thường - Chưa tạo phiếu yêu cầu',0,508,'2024-03-02 17:58:00.023',508,'2024-03-02 17:58:00.023')
INSERT INTO [dbo].[AmDeviceAbnormalStatus]([Name],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES(N'Có bất thường - Đã tạo phiếu yêu cầu',0,508,'2024-03-02 17:58:00.023',508,'2024-03-02 17:58:00.023')
INSERT INTO [dbo].[AmDeviceAbnormalStatus]([Name],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES(N'Có bất thường - Không duyệt',0,508,'2024-03-02 17:58:00.023',508,'2024-03-02 17:58:00.023')
INSERT INTO [dbo].[AmDeviceAbnormalStatus]([Name],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES(N'Có bất thường - Đã duyệt',0,508,'2024-03-02 17:58:00.023',508,'2024-03-02 17:58:00.023')
INSERT INTO [dbo].[AmDeviceAbnormalStatus]([Name],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES(N'Có bất thường - Đang thực hiện sửa chữa',0,508,'2024-03-02 17:58:00.023',508,'2024-03-02 17:58:00.023')
INSERT INTO [dbo].[AmDeviceAbnormalStatus]([Name],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES(N'Có bất thường - Hoàn thành sửa chữa',0,508,'2024-03-02 17:58:00.023',508,'2024-03-02 17:58:00.023')

-----------AmDeviceStatus
INSERT INTO [dbo].[AmDeviceStatus]([StatusDesc],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES(N'Tốt',0,508,'2024-03-10 22:14:47.277',508,'2024-03-10 22:14:47.277')
INSERT INTO [dbo].[AmDeviceStatus]([StatusDesc],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES(N'Xấu',0,508,'2024-03-10 22:14:47.277',508,'2024-03-10 22:14:47.277')
INSERT INTO [dbo].[AmDeviceStatus]([StatusDesc],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES(N'Kém',0,508,'2024-03-10 22:14:47.277',508,'2024-03-10 22:14:47.277')

------------AmStatus
INSERT INTO [dbo].[AmStatus]([StatusName],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES(N'Chưa thực hiện',0,508,'2024-03-02 17:58:00.023',508,'2024-03-02 17:58:00.023')
INSERT INTO [dbo].[AmStatus]([StatusName],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES(N'Không thực hiện',0,508,'2024-03-02 17:58:00.023',508,'2024-03-02 17:58:00.023')
INSERT INTO [dbo].[AmStatus]([StatusName],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES(N'Đang thực hiện',0,508,'2024-03-02 17:58:00.023',508,'2024-03-02 17:58:00.023')
INSERT INTO [dbo].[AmStatus]([StatusName],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES(N'Tạm dừng',0,508,'2024-03-02 17:58:00.023',508,'2024-03-02 17:58:00.023')
INSERT INTO [dbo].[AmStatus]([StatusName],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES(N'Dừng thực hiện',0,508,'2024-03-02 17:58:00.023',508,'2024-03-02 17:58:00.023')
INSERT INTO [dbo].[AmStatus]([StatusName],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES(N'Đã thực hiện',0,508,'2024-03-02 17:58:00.023',508,'2024-03-02 17:58:00.023')

-----------AmManufacturer
INSERT INTO [dbo].[AmManufacturer] ([ManufacturerName],[ManufacturerCode],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt]) VALUES('Manufacture001','MAFA001',0,508,'2024-03-10 22:14:47.277',508,'2024-03-10 22:14:47.277')
INSERT INTO [dbo].[AmManufacturer] ([ManufacturerName],[ManufacturerCode],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt]) VALUES('Manufacture002','MAFA002',0,508,'2024-03-10 22:14:47.277',508,'2024-03-10 22:14:47.277')
INSERT INTO [dbo].[AmManufacturer] ([ManufacturerName],[ManufacturerCode],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt]) VALUES('Manufacture003','MAFA003',0,508,'2024-03-10 22:14:47.277',508,'2024-03-10 22:14:47.277')

-----------AmReasonForPending
INSERT INTO [dbo].[AmReasonForPending]([FactoryID],[ReasonDesc],[Active],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES (1,N'Reason 01',2,0,508,'2024-03-09 19:39:33.960',508,'2024-03-09 19:39:33.960')
INSERT INTO [dbo].[AmReasonForPending]([FactoryID],[ReasonDesc],[Active],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES (1,N'Reason 02',2,0,508,'2024-03-09 19:39:33.960',508,'2024-03-09 19:39:33.960')
INSERT INTO [dbo].[AmReasonForPending]([FactoryID],[ReasonDesc],[Active],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES (1,N'Reason 03',2,0,508,'2024-03-09 19:39:33.960',508,'2024-03-09 19:39:33.960')

-----------AmReasonForStop
INSERT INTO [dbo].[AmReasonForStop]([FactoryID],[Active],[ReasonDesc],[ReasonType],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES(1,1,N'Reason 01',1,0,508,'2024-03-09 16:23:32.737',508,'2024-03-09 16:23:32.737')
INSERT INTO [dbo].[AmReasonForStop]([FactoryID],[Active],[ReasonDesc],[ReasonType],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES(1,1,N'Reason 02',1,0,508,'2024-03-09 16:23:32.737',508,'2024-03-09 16:23:32.737')
INSERT INTO [dbo].[AmReasonForStop]([FactoryID],[Active],[ReasonDesc],[ReasonType],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES(1,1,N'Reason 03',1,0,508,'2024-03-09 16:23:32.737',508,'2024-03-09 16:23:32.737')

-----------AmEmergency
INSERT INTO [dbo].[AmEmergency]([FactoryID],[Name],[Active],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES(1,N'Mức độ cảnh báo 01',1,0,508,'2024-03-02 17:58:00.023',508,'2024-03-02 17:58:00.023')
INSERT INTO [dbo].[AmEmergency]([FactoryID],[Name],[Active],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES(1,N'Mức độ cảnh báo 02',1,0,508,'2024-03-02 17:58:00.023',508,'2024-03-02 17:58:00.023')
INSERT INTO [dbo].[AmEmergency]([FactoryID],[Name],[Active],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES(1,N'Mức độ cảnh báo 03',1,0,508,'2024-03-02 17:58:00.023',508,'2024-03-02 17:58:00.023')

-----------AmLocation
INSERT INTO [dbo].[AmLocation]([FactoryID],[OrganizationID],[SiteParent],[SiteDesc],[SiteCode],[Active],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt],[PMISId])VALUES(1,1,1,0,N'Khu vực A','KV-A',1,0,508,'2024-03-02 17:58:00.023',508,'2024-03-02 17:58:00.023',NULL)
INSERT INTO [dbo].[AmLocation]([FactoryID],[OrganizationID],[SiteParent],[SiteDesc],[SiteCode],[Active],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt],[PMISId])VALUES(1,1,1,0,N'Khu vực B','KV-B',1,0,508,'2024-03-02 17:58:00.023',508,'2024-03-02 17:58:00.023',NULL)
INSERT INTO [dbo].[AmLocation]([FactoryID],[OrganizationID],[SiteParent],[SiteDesc],[SiteCode],[Active],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt],[PMISId])VALUES(1,1,1,0,N'Khu vực C','KV-C',1,0,508,'2024-03-02 17:58:00.023',508,'2024-03-02 17:58:00.023',NULL)

-----------AmDeviceType
INSERT INTO [dbo].[AmDeviceType]([FactoryID],[TypeCode],[TypeDesc],[Active],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt],[PMISType])VALUES(1,'LTB01',N'Loại thiết bị 01',1,0,508,'2024-03-02 17:58:00.023',508,'2024-03-02 17:58:00.023',NULL)
INSERT INTO [dbo].[AmDeviceType]([FactoryID],[TypeCode],[TypeDesc],[Active],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt],[PMISType])VALUES(1,'LTB02',N'Loại thiết bị 02',1,0,508,'2024-03-02 17:58:00.023',508,'2024-03-02 17:58:00.023',NULL)
INSERT INTO [dbo].[AmDeviceType]([FactoryID],[TypeCode],[TypeDesc],[Active],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt],[PMISType])VALUES(1,'LTB03',N'Loại thiết bị 03',1,0,508,'2024-03-02 17:58:00.023',508,'2024-03-02 17:58:00.023',NULL)

-----------AmDevice
INSERT INTO [dbo].[AmDevice]([FactoryID],[AmLocationID],[AssetParent],[AssetDesc],[AssetCode],[AmDeviceStatusID],[Active],[AmDeviceTypeID],[AmManufacturerID],[AmSupplierID],[AmCountryManufactureID],[OrganizationID],[SerialNum],[AssetType],[Meter],[Kss],[Khvh],[Note],[Description],[Drawing],[Images],[InstallDate],[OperatingDate],[AmDeviceCategoryID],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt],[PMISId])VALUES(1,1,0,N'Thiết bị 01','TB001',1,1,1,1,1,1,4,N'Số SN 0123',N'Chủng loại kiểu 0123',N'Thông số kỹ thuật 0123',N'Mã số KSS 0123',N'Ký hiệu vận hành 0123',N'Ghi chú 0123',N'Mô tả 0123',N'Bản vẽ liên quan 0123',NULL,'2024-03-20 00:00:00.000','2024-03-25 00:00:00.000',1,0,508,'2024-03-09 09:27:12.020',508,'2024-03-20 21:49:33.930',NULL)
INSERT INTO [dbo].[AmDevice]([FactoryID],[AmLocationID],[AssetParent],[AssetDesc],[AssetCode],[AmDeviceStatusID],[Active],[AmDeviceTypeID],[AmManufacturerID],[AmSupplierID],[AmCountryManufactureID],[OrganizationID],[SerialNum],[AssetType],[Meter],[Kss],[Khvh],[Note],[Description],[Drawing],[Images],[InstallDate],[OperatingDate],[AmDeviceCategoryID],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt],[PMISId])VALUES(1,1,0,N'Thiết bị 02','TB002',1,1,1,1,1,1,4,N'Số SN 0123',N'Chủng loại kiểu 0123',N'Thông số kỹ thuật 0123',N'Mã số KSS 0123',N'Ký hiệu vận hành 0123',N'Ghi chú 0123',N'Mô tả 0123',N'Bản vẽ liên quan 0123',NULL,'2024-03-20 00:00:00.000','2024-03-25 00:00:00.000',1,0,508,'2024-03-09 09:27:12.020',508,'2024-03-20 21:49:33.930',NULL)
INSERT INTO [dbo].[AmDevice]([FactoryID],[AmLocationID],[AssetParent],[AssetDesc],[AssetCode],[AmDeviceStatusID],[Active],[AmDeviceTypeID],[AmManufacturerID],[AmSupplierID],[AmCountryManufactureID],[OrganizationID],[SerialNum],[AssetType],[Meter],[Kss],[Khvh],[Note],[Description],[Drawing],[Images],[InstallDate],[OperatingDate],[AmDeviceCategoryID],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt],[PMISId])VALUES(1,1,0,N'Thiết bị 03','TB003',1,1,1,1,1,1,4,N'Số SN 0123',N'Chủng loại kiểu 0123',N'Thông số kỹ thuật 0123',N'Mã số KSS 0123',N'Ký hiệu vận hành 0123',N'Ghi chú 0123',N'Mô tả 0123',N'Bản vẽ liên quan 0123',NULL,'2024-03-20 00:00:00.000','2024-03-25 00:00:00.000',1,0,508,'2024-03-09 09:27:12.020',508,'2024-03-20 21:49:33.930',NULL)

-----------AmWorkType
INSERT INTO [dbo].[AmWorkType]([FactoryID],[TypeDesc],[Active],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES(1,N'Sửa chữa 01',1,0,508,'2024-03-05 20:17:53.513',508,'2024-03-05 20:48:02.743')
INSERT INTO [dbo].[AmWorkType]([FactoryID],[TypeDesc],[Active],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES(1,N'Sửa chữa 02',1,0,508,'2024-03-05 20:17:53.513',508,'2024-03-05 20:48:02.743')
INSERT INTO [dbo].[AmWorkType]([FactoryID],[TypeDesc],[Active],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES(1,N'Sửa chữa 03',1,0,508,'2024-03-05 20:17:53.513',508,'2024-03-05 20:48:02.743')

-----------AmWork
INSERT INTO [dbo].[AmWork]([FactoryID],[AmWorkTypeID],[AttachmentID],[Active],[WorkName],[WorkDesc],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES(1,1,NULL,1,N'Công việc 01',N'string',0,508,'2024-03-02 17:58:00.023',508,'2024-03-02 17:58:00.023')
INSERT INTO [dbo].[AmWork]([FactoryID],[AmWorkTypeID],[AttachmentID],[Active],[WorkName],[WorkDesc],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES(1,1,NULL,1,N'Công việc 02',N'string',0,508,'2024-03-02 17:58:00.023',508,'2024-03-02 17:58:00.023')
INSERT INTO [dbo].[AmWork]([FactoryID],[AmWorkTypeID],[AttachmentID],[Active],[WorkName],[WorkDesc],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])VALUES(1,1,NULL,1,N'Công việc 03',N'string',0,508,'2024-03-02 17:58:00.023',508,'2024-03-02 17:58:00.023')

GO


