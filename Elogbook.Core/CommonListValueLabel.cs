using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Models;
using System.Collections.Generic;

namespace Elogbook.Core
{
    /// <summary>
    /// Định nghĩa các hàm để trả về dữ liệu cho các combobox
    /// </summary>
    public static class CommonListValueLabel
    {
        /// <summary>
        /// Danh sách trạng thái sử dụng dữ liệu
        /// </summary>
        /// <returns></returns>
        public static List<ValueLabelModel> GetAccessStatus()
        {
            var list = new List<ValueLabelModel>
            {
                new ValueLabelModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                new ValueLabelModel { Value = (int)AccessStatus.Active, Label = EnumExtensions.GetEnumDescription(AccessStatus.Active)},
                new ValueLabelModel { Value = (int)AccessStatus.InActive, Label = EnumExtensions.GetEnumDescription(AccessStatus.InActive) }
            };

            return list;
        }

        /// <summary>
        /// Danh sách trạng thái truy cập của người dùng
        /// </summary>
        /// <returns></returns>
        public static List<ValueLabelModel> GetUserStatus()
        {
            var list = new List<ValueLabelModel>
            {
                new ValueLabelModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                new ValueLabelModel { Value = (int)UserStatus.Active, Label = EnumExtensions.GetEnumDescription(UserStatus.Active) },
                new ValueLabelModel { Value = (int)UserStatus.InActive, Label = EnumExtensions.GetEnumDescription(UserStatus.InActive) }
            };

            return list;
        }

        /// <summary>
        /// Danh sách loại tài khoản người dùng
        /// </summary>
        /// <returns></returns>
        public static List<ValueLabelModel> GetUserTypes()
        {
            var list = new List<ValueLabelModel>
            {
                new ValueLabelModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                new ValueLabelModel { Value = (int)UserType.TwoSystem, Label = EnumExtensions.GetEnumDescription(UserType.TwoSystem) },
                new ValueLabelModel { Value = (int)UserType.AdminPortal, Label = EnumExtensions.GetEnumDescription(UserType.AdminPortal) },
                new ValueLabelModel { Value = (int)UserType.MobileApp, Label = EnumExtensions.GetEnumDescription(UserType.MobileApp) }
            };

            return list;
        }

        /// <summary>
        /// Danh sách nguồn của tài khoản người dùng
        /// </summary>
        /// <returns></returns>
        public static List<ValueLabelModel> GetUserSources()
        {
            var list = new List<ValueLabelModel>
            {
                new ValueLabelModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                new ValueLabelModel { Value = (int)UserSource.ADUser, Label = EnumExtensions.GetEnumDescription(UserSource.ADUser) },
                new ValueLabelModel { Value = (int)UserSource.AdminPortal, Label = EnumExtensions.GetEnumDescription(UserSource.AdminPortal) }
            };

            return list;
        }

        /// <summary>
        /// Danh sách quyền thao tác dữ liệu
        /// </summary>
        /// <returns></returns>
        public static List<ValueLabelModel> GetDataPermission()
        {
            var list = new List<ValueLabelModel>
            {
                new ValueLabelModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                new ValueLabelModel { Value = (int)DataPermission.Limited, Label = EnumExtensions.GetEnumDescription(DataPermission.Limited) },
                new ValueLabelModel { Value = (int)DataPermission.Unlimited, Label = EnumExtensions.GetEnumDescription(DataPermission.Unlimited) }
            };

            return list;
        }

        /// <summary>
        /// Danh sách trạng thái sử dụng RFID/QR Code
        /// </summary>
        /// <returns></returns>
        public static List<ValueLabelModel> GetRFIDQR()
        {
            var list = new List<ValueLabelModel>
            {
                new ValueLabelModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                new ValueLabelModel { Value = (int)AthenticationType.NotUse, Label = EnumExtensions.GetEnumDescription(AthenticationType.NotUse)},
                new ValueLabelModel { Value = (int)AthenticationType.RFID, Label = EnumExtensions.GetEnumDescription(AthenticationType.RFID)},
                new ValueLabelModel { Value = (int)AthenticationType.QrCode, Label = EnumExtensions.GetEnumDescription(AthenticationType.QrCode) }
            };

            return list;
        }

        /// <summary>
        /// Danh sách loại ngày cần chọn để tìm kiếm
        /// </summary>
        /// <returns></returns>
        public static List<ValueLabelModel> SearchDateType()
        {
            var list = new List<ValueLabelModel>
            {
                new ValueLabelModel { Value = (int)DateType.CreateDate, Label = EnumExtensions.GetEnumDescription(DateType.CreateDate)},
                new ValueLabelModel { Value = (int)DateType.UseDate, Label = EnumExtensions.GetEnumDescription(DateType.UseDate) }
            };

            return list;
        }

        /// <summary>
        /// Danh sách các loại phiếu công tác
        /// </summary>
        /// <returns></returns>
        public static List<ValueLabelModel> GetTicketTypes()
        {
            var list = new List<ValueLabelModel>
            {
                new ValueLabelModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                new ValueLabelModel { Value = (int)TicketType.InsidePlan, Label = EnumExtensions.GetEnumDescription(TicketType.InsidePlan)},
                new ValueLabelModel { Value = (int)TicketType.OutsidePlan, Label = EnumExtensions.GetEnumDescription(TicketType.OutsidePlan) },
                new ValueLabelModel { Value = (int)TicketType.SpecialCase, Label = EnumExtensions.GetEnumDescription(TicketType.SpecialCase) },
            };

            return list;
        }

        /// <summary>
        /// Lấy danh sách bậc an toàn điện
        /// </summary>
        /// <returns></returns>
        public static List<ValueLabelModel> GetElectricRanks()
        {
            var list = new List<ValueLabelModel>
            {
                new ValueLabelModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                new ValueLabelModel { Value = (int)ElectricRank.Level1, Label = EnumExtensions.GetEnumDescription(ElectricRank.Level1)},
                new ValueLabelModel { Value = (int)ElectricRank.Level2, Label = EnumExtensions.GetEnumDescription(ElectricRank.Level2)},
                new ValueLabelModel { Value = (int)ElectricRank.Level3, Label = EnumExtensions.GetEnumDescription(ElectricRank.Level3)},
                new ValueLabelModel { Value = (int)ElectricRank.Level4, Label = EnumExtensions.GetEnumDescription(ElectricRank.Level4)},
                new ValueLabelModel { Value = (int)ElectricRank.Level5, Label = EnumExtensions.GetEnumDescription(ElectricRank.Level5)},
            };

            return list;
        }

        /// <summary>
        /// Lấy danh sách bậc thợ
        /// </summary>
        /// <returns></returns>
        public static List<ValueLabelModel> GetLaborRanks()
        {
            var list = new List<ValueLabelModel>
            {
                new ValueLabelModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                new ValueLabelModel { Value = (int)LaborRank.Level1, Label = EnumExtensions.GetEnumDescription(LaborRank.Level1)},
                new ValueLabelModel { Value = (int)LaborRank.Level2, Label = EnumExtensions.GetEnumDescription(LaborRank.Level2)},
                new ValueLabelModel { Value = (int)LaborRank.Level3, Label = EnumExtensions.GetEnumDescription(LaborRank.Level3)},
                new ValueLabelModel { Value = (int)LaborRank.Level4, Label = EnumExtensions.GetEnumDescription(LaborRank.Level4)},
                new ValueLabelModel { Value = (int)LaborRank.Level5, Label = EnumExtensions.GetEnumDescription(LaborRank.Level5)},
                new ValueLabelModel { Value = (int)LaborRank.Level6, Label = EnumExtensions.GetEnumDescription(LaborRank.Level6)},
                new ValueLabelModel { Value = (int)LaborRank.Level7, Label = EnumExtensions.GetEnumDescription(LaborRank.Level7)},
            };

            return list;
        }

        /// <summary>
        /// Danh sách lỗi thiết bị
        /// </summary>
        /// <returns></returns>
        public static List<ValueLabelModel> GetDeviceErrors()
        {
            var list = new List<ValueLabelModel>
            {
                new ValueLabelModel { Value = (int)DeviceErrorCode.Error, Label = EnumExtensions.GetEnumDescription(DeviceErrorCode.Error)},
                new ValueLabelModel { Value = (int)DeviceErrorCode.Stop, Label = EnumExtensions.GetEnumDescription(DeviceErrorCode.Stop)},
                new ValueLabelModel { Value = (int)DeviceErrorCode.Sim, Label = EnumExtensions.GetEnumDescription(DeviceErrorCode.Sim)},
                new ValueLabelModel { Value = (int)DeviceErrorCode.Jump, Label = EnumExtensions.GetEnumDescription(DeviceErrorCode.Jump)},
            };

            return list;
        }

        /// <summary>
        /// Danh sách tình trạng sử lý
        /// </summary>
        /// <returns></returns>
        public static List<ValueLabelModel> GetProcessingStatus()
        {
            var list = new List<ValueLabelModel>
            {
                new ValueLabelModel { Value = (int)ProcessingStatus.Waiting, Label = EnumExtensions.GetEnumDescription(ProcessingStatus.Waiting)},
                new ValueLabelModel { Value = (int)ProcessingStatus.InProgress, Label = EnumExtensions.GetEnumDescription(ProcessingStatus.InProgress)},
                new ValueLabelModel { Value = (int)ProcessingStatus.Completed, Label = EnumExtensions.GetEnumDescription(ProcessingStatus.Completed)},
            };

            return list;
        }
    }
}
