SET NOCOUNT ON 

DECLARE  @WorkTicketNumber nvarchar(50), @TicketNumber nvarchar(50) ,@Num varchar(20)
-- Đăng ký phiếu điện
DELETE FROM dbo.ExclusiveData where TicketType = 'DKPCTD'
DECLARE DKPCTD CURSOR FOR SELECT TicketNumber FROM dbo.RegistrationTicket  where RegistrationTicketType = 1 
OPEN DKPCTD FETCH NEXT FROM DKPCTD INTO @TicketNumber
	WHILE @@FETCH_STATUS = 0 
		BEGIN 
			SET @Num = SUBSTRING(@TicketNumber,0, CHARINDEX('/', @TicketNumber))
			INSERT INTO dbo.ExclusiveData([TicketNumber],[TicketType],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])
			VALUES (@Num ,'DKPCTD' ,0 ,NULL ,GETDATE() ,NULL ,GETDATE())

			FETCH NEXT
			FROM DKPCTD INTO @TicketNumber
		END	
CLOSE DKPCTD 
DEALLOCATE DKPCTD 

-- Đăng ký phiếu thủy cơ nhiệt hóa
DELETE FROM dbo.ExclusiveData where TicketType = 'DKPTCNH'
DECLARE DKPTCNH CURSOR FOR SELECT TicketNumber FROM dbo.RegistrationTicket  where RegistrationTicketType = 2 
OPEN DKPTCNH FETCH NEXT FROM DKPTCNH INTO @TicketNumber
	WHILE @@FETCH_STATUS = 0 
		BEGIN 
			SET @Num = SUBSTRING(@TicketNumber,0, CHARINDEX('/', @TicketNumber))

			INSERT INTO dbo.ExclusiveData([TicketNumber],[TicketType],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])
			VALUES (@Num ,'DKPTCNH' ,0 ,NULL ,GETDATE() ,NULL ,GETDATE())

			FETCH NEXT
			FROM DKPTCNH INTO @TicketNumber
		END	
CLOSE DKPTCNH 
DEALLOCATE DKPTCNH 

-- Quản lý phiếu điện
DELETE FROM dbo.ExclusiveData where TicketType = 'QLPCTD'
DECLARE QLPCTD CURSOR FOR SELECT WorkTicketNumber FROM dbo.WorkTicket  where ScreenType = 1
OPEN QLPCTD FETCH NEXT FROM QLPCTD INTO @WorkTicketNumber
	WHILE @@FETCH_STATUS = 0 
		BEGIN 
			SET @Num = SUBSTRING(@WorkTicketNumber,0, CHARINDEX('/', @WorkTicketNumber))

			INSERT INTO dbo.ExclusiveData([TicketNumber],[TicketType],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])
			VALUES (@Num ,'QLPCTD' ,0 ,NULL ,GETDATE() ,NULL ,GETDATE())

			FETCH NEXT
			FROM QLPCTD INTO @WorkTicketNumber
		END	
CLOSE QLPCTD 
DEALLOCATE QLPCTD 

-- Quản lý phiếu thủy cơ nhiệt hóa
DELETE FROM dbo.ExclusiveData where TicketType = 'QLPTCNH'
DECLARE QLPTCNH CURSOR FOR SELECT WorkTicketNumber FROM dbo.WorkTicket  where ScreenType = 2
OPEN QLPTCNH FETCH NEXT FROM QLPTCNH INTO @WorkTicketNumber
	WHILE @@FETCH_STATUS = 0 
		BEGIN 
			SET @Num = SUBSTRING(@WorkTicketNumber,0, CHARINDEX('/', @WorkTicketNumber))

			INSERT INTO dbo.ExclusiveData([TicketNumber],[TicketType],[IsDeleted],[CreatedUserID],[InsertedAt],[LastUpdatedUserID],[UpdatedAt])
			VALUES (@Num ,'QLPTCNH' ,0 ,NULL ,GETDATE() ,NULL ,GETDATE())

			FETCH NEXT
			FROM QLPTCNH INTO @WorkTicketNumber
		END	
CLOSE QLPTCNH 
DEALLOCATE QLPTCNH 

GO