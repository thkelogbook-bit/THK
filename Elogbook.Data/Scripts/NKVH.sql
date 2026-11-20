--NKVH/Nhận xét thông số
select * from OperationDiary
--Cương vị trong ca
select * from OperationDiaryPositionShift
--1.Tình hình vận hành/files
select * from OperationSituation
--2.Bất thường sự cố/files
select * from AbnormalIncident
--3.Phương thức giao ca/files
select * from DeliveryShiftMethod
--4.Tồn tại trong ca/files
select * from ExistInShift
--6.Giao/nhận ca
select * from DeliveryReceiveShift

Delete OperationDiaryPositionShift
DBCC CHECKIDENT ('OperationDiaryPositionShift', RESEED, 0)
Delete DeliveryShiftMethod
DBCC CHECKIDENT ('DeliveryShiftMethod', RESEED, 0)
Delete AbnormalIncident
DBCC CHECKIDENT ('AbnormalIncident', RESEED, 0)
Delete DeliveryReceiveShift
DBCC CHECKIDENT ('DeliveryReceiveShift', RESEED, 0)
Delete ExistInShift
DBCC CHECKIDENT ('ExistInShift', RESEED, 0)
Delete OperationSituation
DBCC CHECKIDENT ('OperationSituation', RESEED, 0)
Delete OperationDiary
DBCC CHECKIDENT ('OperationDiary', RESEED, 0)