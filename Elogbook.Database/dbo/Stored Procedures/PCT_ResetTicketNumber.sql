--Clear ticket number of PCT not use every day
CREATE PROCEDURE [dbo].[PCT_ResetTicketNumber]
AS
BEGIN
	SET NOCOUNT ON

    DECLARE @TicketNumberTemp TABLE (TicketNumber INT)

	-- Đăng ký phiếu điện
	INSERT INTO @TicketNumberTemp
	SELECT SUBSTRING(TicketNumber,0, CHARINDEX('/', TicketNumber)) FROM dbo.RegistrationTicket WITH(NOLOCK) WHERE RegistrationTicketType = 1
	DELETE FROM dbo.ExclusiveData WHERE TicketNumber NOT IN(SELECT TicketNumber FROM @TicketNumberTemp) AND TicketType = 'DKPCTD'
	AND InsertedAt <= DATEADD(HOUR,-1,GETDATE())
	DELETE @TicketNumberTemp

	-- Đăng ký phiếu thủy cơ nhiệt hóa
	INSERT INTO @TicketNumberTemp
	SELECT SUBSTRING(TicketNumber,0, CHARINDEX('/', TicketNumber)) FROM dbo.RegistrationTicket WITH(NOLOCK) WHERE RegistrationTicketType = 2
	DELETE FROM dbo.ExclusiveData WHERE TicketNumber NOT IN(SELECT TicketNumber FROM @TicketNumberTemp) AND TicketType = 'DKPTCNH'
	AND InsertedAt <= DATEADD(HOUR,-1,GETDATE())
	DELETE @TicketNumberTemp

	-- Quản lý phiếu điện
	INSERT INTO @TicketNumberTemp
	SELECT SUBSTRING(WorkTicketNumber,0, CHARINDEX('/', WorkTicketNumber)) FROM dbo.WorkTicket WITH(NOLOCK) WHERE ScreenType = 1
	DELETE FROM dbo.ExclusiveData WHERE TicketNumber NOT IN(SELECT TicketNumber FROM @TicketNumberTemp) AND TicketType = 'QLPCTD'
	AND InsertedAt <= DATEADD(HOUR,-1,GETDATE())
	DELETE @TicketNumberTemp

	-- Quản lý phiếu thủy cơ nhiệt hóa
	INSERT INTO @TicketNumberTemp
	SELECT SUBSTRING(WorkTicketNumber,0, CHARINDEX('/', WorkTicketNumber)) FROM dbo.WorkTicket WITH(NOLOCK) WHERE ScreenType = 2
	DELETE FROM dbo.ExclusiveData WHERE TicketNumber NOT IN(SELECT TicketNumber FROM @TicketNumberTemp) AND TicketType = 'QLPTCNH'
	AND InsertedAt <= DATEADD(HOUR,-1,GETDATE())
	DELETE @TicketNumberTemp

	SELECT 'SUCCESS' AS Message
END