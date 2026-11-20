
-- ==========================================================================================
-- Đồng bộ dữ liệu từ PMIS sang Elogbook
-- ==========================================================================================
CREATE PROCEDURE [dbo].[SYNC_PMIS_AMDEVICECATEGORY]
	@JobId INT = NULL
AS
BEGIN
	SET NOCOUNT ON

	--Đồng bộ dữ liệu phân loại thiết bị
	MERGE INTO dbo.AmDeviceCategory AS target
	USING dbo.PMIS_A_LST_CATEGORY AS source
	ON (target.CategoryCode = source.[CATEGORYID])
	--Xử lý cập nhật dữ liệu khi trùng lặp giữa PMIS và ELogbook
	WHEN MATCHED THEN
		UPDATE SET 
			target.CategoryParent = NULL,
			target.CategoryName = source.CATEGORYDESC,
			target.CategoryCode = source.CATEGORYID,
			target.UpdatedAt = GETDATE(),
			target.IsDeleted = 0,
			target.Active = 1,
			target.PMISId = source.CATEGORYID_PARENT
	--Xử lý thêm mới dữ liệu nếu không tìm thấy ở Elogbook
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (CategoryName, CategoryCode, Active, IsDeleted, CreatedUserID, InsertedAt, LastUpdatedUserID, UpdatedAt, PMISId, OrgId)
		VALUES (source.CATEGORYDESC, source.CATEGORYID, 1, 0, NULL, GETDATE(), NULL, GETDATE(), source.CATEGORYID_PARENT, NULL);
	--Xử lý xóa dữ liệu Elogbook nếu không tìm thấy ở PMIS
	--WHEN NOT MATCHED BY SOURCE THEN
	--    DELETE;
	
	DECLARE @currentID INT,
		@maxID INT,
		@CATEGORYID_PARENT NVARCHAR(25),
		@ID INT,
		@RowIndex INT = 1,
		@TotalRow INT = 0;

	DECLARE @temp TABLE(
		RowIndex INT IDENTITY(1,1),
		ID INT,
		CATEGORYID_PARENT NVARCHAR(25),
		CategoryCode NVARCHAR(25)
	)
	INSERT INTO @temp
	SELECT ID, PMISId, CategoryCode FROM dbo.AmDeviceCategory WITH(NOLOCK)

	SET @TotalRow = (SELECT COUNT(ID) FROM @temp)

	WHILE @RowIndex <= @TotalRow
	BEGIN
		SELECT TOP 1 @CATEGORYID_PARENT = CATEGORYID_PARENT, @ID = ID FROM @temp WHERE RowIndex = @RowIndex

		IF((ISNULL(@ID, 0)) > 0)
		BEGIN
			UPDATE dbo.AmDeviceCategory 
			SET CategoryParent = (SELECT TOP 1 ID FROM @temp WHERE CategoryCode = @CATEGORYID_PARENT) 
			WHERE ID = @ID
		END

		SET @RowIndex = @RowIndex + 1;
		SET @CATEGORYID_PARENT = NULL;
		SET @ID = NULL;
	END

	DELETE dbo.PMIS_A_LST_CATEGORY WHERE JobId = @JobId;
END