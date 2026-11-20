using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Extensions;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elogbook.Domain.Services
{
    public class FactoryService : IFactoryService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        public FactoryService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ResponseBaseModel GetFactorys(SearchFactoryModel requestModel)
        {
            // Dữ liệu đang sử dụng
            var entities = _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => !x.IsDeleted);

            // Tên nhà máy
            if (!string.IsNullOrWhiteSpace(requestModel.FactoryName))
            {
                entities = entities.Where(x => x.FactoryName.Contains(requestModel.FactoryName));
            }
            // Công ty
            if (requestModel.CompanyID > 0)
            {
                entities = entities.Where(x => x.CompanyID == requestModel.CompanyID);
            }
            // Phân xưởng
            if (requestModel.ProductionPlantID > 0)
            {
                entities = entities.Where(x => x.ProductionPlantID == requestModel.ProductionPlantID);
            }

            // Tìm kiếm và paging dữ liệu
            var page = entities.Select(
                e => new FactoryModel()
                {
                    ID = e.ID,
                    FactoryName = e.FactoryName,
                    Company = new ValueLabelModel { Value = e.Company.ID, Label = e.Company.Name },
                    ProductionPlant = new ValueLabelModel { Value = e.ProductionPlant.ID, Label = e.ProductionPlant.Name },
                    Status = e.Status,
                    IsUseOperationParameterGroup = e.IsUseOperationParameterGroup,
                    ShortName = e.ShortName,
                    IsUseShortNameToTicket = e.IsUseShortNameToTicket,
                    PTTFormat = e.PTTFormat,
                    PCTTCNHFormat = e.PCTTCNHFormat,
                    PCTDFormat = e.PCTDFormat,
                    Email = e.Email,
                }).OrderBy(x => x.FactoryName)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        public FactoryModel GetFactoryById(int id)
        {
            return _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
                e => new FactoryModel()
                {
                    ID = e.ID,
                    FactoryName = e.FactoryName,
                    Company = new ValueLabelModel { Value = e.Company.ID, Label = e.Company.Name },
                    ProductionPlant = new ValueLabelModel { Value = e.ProductionPlant.ID, Label = e.ProductionPlant.Name },
                    Status = e.Status,
                    IsUseOperationParameterGroup = e.IsUseOperationParameterGroup,
                    ShortName = e.ShortName,
                    IsUseShortNameToTicket = e.IsUseShortNameToTicket,
                    PTTFormat = e.PTTFormat,
                    PCTTCNHFormat = e.PCTTCNHFormat,
                    PCTDFormat = e.PCTDFormat,
                    Email = e.Email,
                }).FirstOrDefault();
        }

        public int AddFactory(AddEditFactoryModel requestModel)
        {
            var entity = new Factory
            {
                FactoryName = requestModel.FactoryName.Trim(),
                CompanyID = requestModel.CompanyID,
                ProductionPlantID = requestModel.ProductionPlantID,
                Status = requestModel.Status <= 0 ? (int)AccessStatus.InActive : requestModel.Status,
                IsUseOperationParameterGroup = requestModel.IsUseOperationParameterGroup,
                ShortName = requestModel.ShortName ?? string.Empty,
                IsUseShortNameToTicket = requestModel.IsUseShortNameToTicket,
                CreatedUserID = requestModel.CurrentUserID,
                LastUpdatedUserID = requestModel.CurrentUserID,
                PTTFormat = requestModel.PTTFormat,
                PCTTCNHFormat = requestModel.PCTTCNHFormat,
                PCTDFormat = requestModel.PCTDFormat,
                Email = requestModel.Email,
            };

            _unitOfWork.ElogbookRepository<Factory>().Add(entity);
            var saveSuccess = _unitOfWork.Complete() > 0;
            if (saveSuccess)
            {
                return entity.ID;
            }
            else
            {
                return 0;
            }
        }

        public bool UpdateFactory(AddEditFactoryModel requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                var ticketDescription = entity.TicketDescription;
                entity.TicketDescription = ticketDescription;
                entity.FactoryName = requestModel.FactoryName.Trim();
                entity.CompanyID = requestModel.CompanyID;
                entity.ProductionPlantID = requestModel.ProductionPlantID;
                entity.Status = requestModel.Status <= 0 ? (int)AccessStatus.InActive : requestModel.Status;
                entity.IsUseOperationParameterGroup = requestModel.IsUseOperationParameterGroup;
                entity.ShortName = requestModel.ShortName ?? string.Empty;
                entity.IsUseShortNameToTicket = requestModel.IsUseShortNameToTicket;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;
                entity.PTTFormat = requestModel.PTTFormat;
                entity.PCTTCNHFormat = requestModel.PCTTCNHFormat;
                entity.PCTDFormat = requestModel.PCTDFormat;
                entity.Email = requestModel.Email;
                _unitOfWork.ElogbookRepository<Factory>().Update(entity);

                return _unitOfWork.Complete() > 0;
            }

            return false;
        }

        public bool DeleteFactory(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => x.ID == id && !x.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                _unitOfWork.ElogbookRepository<Factory>().Delete(entity, userID);
                _unitOfWork.Complete();

                return true;
            }

            return false;
        }

        public bool CheckInUseFactory(int id)
        {
            var isInUse = false;
            // Nhà máy sử dụng trong ca vận hành
            isInUse = _unitOfWork.ElogbookRepository<OperationShift>().GetQuery(x => x.FactoryID == id && !x.IsDeleted).Any();

            return isInUse;
        }

        public bool CheckDuplicateData(int factoryID, string factoryName, int companyID, bool isAddMode)
        {
            // Trường hợp thêm mới thì check Tên phân xưởng và Nhà máy
            if (isAddMode)
            {
                return _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => x.CompanyID == companyID
                                                                                    && x.FactoryName.Equals(factoryName, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
            }
            // Trường hợp cập nhật thì check Tên phân xưởng, Nhà máy và những Phân xưởng khác Phân xưởng đang cập nhật
            else
            {
                return _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => x.CompanyID == companyID
                                                                                    && x.ID != factoryID
                                                                                    && x.FactoryName.Equals(factoryName, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
            }
        }

        public bool CheckExistData(int companyID, int productionPlantID)
        {
            // Nếu có công ty/phân xưởng thì kiểm tra công ty/phân xưởng có tồn tại trong hệ thống và phân xưởng có thuộc công ty không
            var entities = _unitOfWork.ElogbookRepository<Organization>().GetQuery(x => (x.ID == companyID || x.ParentID == companyID)
                                                                                    && !x.IsDeleted).ToList();

            return entities.Exists(x => x.ID == productionPlantID && "Phân xưởng".Equals(x.Level, System.StringComparison.OrdinalIgnoreCase));
        }

        public List<ValueLabelModel> ParseTicketFormat(string ticketFormat, string type)
        {
            List<ValueLabelModel> result = new List<ValueLabelModel>();
            if (string.IsNullOrWhiteSpace(ticketFormat))
            {
                return result;
            }

            var values = ticketFormat.Split(',');
            var listTicketFormats = GetTicketFormat(type);
            foreach (var item in values)
            {
                ValueLabelModel formatTicket = listTicketFormats.Find(x => x.Value == int.Parse(item));
                if (formatTicket != null)
                {
                    result.Add(new ValueLabelModel
                    {
                        Value = formatTicket.Value,
                        Label = formatTicket.Label
                    });
                }
            }

            return result;
        }

        public List<ValueLabelModel> GetTicketFormat(string type)
        {
            var results = new List<ValueLabelModel>();

            switch (type)
            {
                case CommonConstant.PTT:
                    results = Enum.GetValues(typeof(PTTFormat))
                                        .Cast<PTTFormat>()
                                        .Select(i => new ValueLabelModel { Label = EnumExtensions.GetEnumDescription(i), Value = (int)i })
                                        .ToList();
                    break;
                case CommonConstant.PCTTCNH:
                    results = Enum.GetValues(typeof(PCTTCNHFormat))
                                        .Cast<PCTTCNHFormat>()
                                        .Select(i => new ValueLabelModel { Label = EnumExtensions.GetEnumDescription(i), Value = (int)i })
                                        .ToList();
                    break;
                case CommonConstant.PCTD:
                    results = Enum.GetValues(typeof(PCTDFormat))
                                       .Cast<PCTDFormat>()
                                       .Select(i => new ValueLabelModel { Label = EnumExtensions.GetEnumDescription(i), Value = (int)i })
                                       .ToList();
                    break;
            }

            return results;
        }
    }
}
