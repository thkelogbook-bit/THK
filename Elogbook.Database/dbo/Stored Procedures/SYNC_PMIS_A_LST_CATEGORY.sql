-- ==========================================================================================
-- Lấy dữ liệu từ database từ QLKT_INFO (PMIS) đưa vào Elogbook để chuẩn bị đồng bộ dữ liệu
-- ==========================================================================================
CREATE PROCEDURE [dbo].[SYNC_PMIS_A_LST_CATEGORY]
	@Data_Input A_LST_CATEGORY_TABLE_TYPE READONLY
AS
BEGIN
	SET NOCOUNT ON

	INSERT INTO dbo.PMIS_A_LST_CATEGORY(CATEGORYID, CATEGORYDESC, CATEGORYID_PARENT, ORGID, CATEGORYORD, ASSETIDPREFIX, USER_CR_ID, USER_CR_DTIME, USER_MDF_ID, 
		USER_MDF_DTIME, IsDeleted, CreatedUserID, InsertedAt, LastUpdatedUserID, UpdatedAt, JobId)
	SELECT  CATEGORYID, CATEGORYDESC, CATEGORYID_PARENT, ORGID, CATEGORYORD, ASSETIDPREFIX, USER_CR_ID, USER_CR_DTIME, USER_MDF_ID, 
		USER_MDF_DTIME, 0, NULL, InsertedAt, NULL, UpdatedAt, JobId
	FROM @Data_Input
END