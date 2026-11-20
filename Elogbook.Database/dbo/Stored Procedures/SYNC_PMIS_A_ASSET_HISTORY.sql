
-- ==========================================================================================
-- Lấy dữ liệu từ database từ QLKT_INFO (PMIS) đưa vào Elogbook để chuẩn bị đồng bộ dữ liệu
-- ==========================================================================================
CREATE PROCEDURE [dbo].[SYNC_PMIS_A_ASSET_HISTORY]
	@AssetHistories_Input A_ASSET_HISTORY_TABLE_TYPE READONLY
AS
BEGIN
	SET NOCOUNT ON

	--Asset Histories
	INSERT INTO dbo.PMIS_ASSET_MAINTENANCE(ASSETID, WORDERID, TASKID, TASKDESC, TPLSTARTDATE, TPLFINISHDATE, TACSTARTDATE, TACFINISHDATE, WTYPEID, 
		IsDeleted, CreatedUserID, InsertedAt, LastUpdatedUserID, UpdatedAt, JobId, status, IsSync)
	SELECT ASSETID, WORDERID, TASKID, TASKDESC, TPLSTARTDATE, TPLFINISHDATE, TACSTARTDATE, TACFINISHDATE, WTYPEID, 
		0, NULL, GETDATE(), NULL, GETDATE(), JobId, NULL, NULL
	FROM @AssetHistories_Input
END