CREATE VIEW [dbo].[view_Quick_Search_NKVH] 
AS
SELECT OperationDiaryID AS 'ID', Description AS 'Description', 1 AS 'DiaryType' FROM dbo.OperationSituation WITH(NOLOCK)
WHERE IsDeleted = 0 AND LEN(LTRIM(RTRIM(ISNULL(Description, '')))) > 0
UNION ALL
SELECT OperationDiaryID AS 'ID', Description AS 'Description', 2 AS 'DiaryType' FROM dbo.DeliveryShiftMethod WITH(NOLOCK)
WHERE IsDeleted = 0 AND LEN(LTRIM(RTRIM(ISNULL(Description, '')))) > 0
UNION ALL
SELECT OperationDiaryID AS 'ID', Description AS 'Description', 3 AS 'DiaryType' FROM dbo.AbnormalIncident WITH(NOLOCK)
WHERE IsDeleted = 0 AND LEN(LTRIM(RTRIM(ISNULL(Description, '')))) > 0
UNION ALL
SELECT OperationDiaryID AS 'ID', Content AS 'Description', 4 AS 'DiaryType' FROM dbo.ExistInShift WITH(NOLOCK)
WHERE IsDeleted = 0 AND LEN(LTRIM(RTRIM(ISNULL(Content, '')))) > 0
GO