using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System;
using System.Collections.Generic;
using System.Net;

namespace Elogbook.Business
{
    public class PositionInShiftBusiness : IPositionInShiftBusiness
    {
        private readonly IPositionInShiftService _positionInShiftService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameterGroupService"></param>
        public PositionInShiftBusiness(IPositionInShiftService positionInShiftService)
        {
            _positionInShiftService = positionInShiftService;
        }

        /// <summary>
        /// Thêm mới Cương vị trong ca
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<int> AddPositionInShift(AddEditPositionInShiftModel requestModel)
        {
            var responseModel = new ResponseModel<int>();
            _errorMessages = new List<string>();

            try
            {
                if (!IsValidData(requestModel))
                {
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    //kiểm tra cương vị trong ca đã tồn tài
                    if (_positionInShiftService.CheckDuplicateData(requestModel.OperatinPositionID, requestModel.ParentID))
                    {
                        _errorMessages.Add("Cương vị trong ca đã tồn tại");
                        responseModel.ErrorMessages = _errorMessages;
                        responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                    else
                    {
                        responseModel.Data = _positionInShiftService.AddPositionInShift(requestModel);
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.PositionInShiftBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Kiểm tra Cương vị trong ca đã có ghi nhận nhật ký vận hành
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel<bool> CheckInUsePositionInShift(int id)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _positionInShiftService.CheckInUsePositionInShift(id);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.PositionInShiftBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Xóa Cương vị trong ca
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ResponseModel<bool> DeletePositionInShift(int id, int userID)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                if (!_positionInShiftService.CheckInUsePositionInShift(id))
                {
                    responseModel.Data = _positionInShiftService.DeletePositionInShift(id, userID);
                }
                else
                {
                    responseModel.Data = false;
                    responseModel.ErrorMessages.Add(CommonConstant.MSG_CANNOT_DELETE_POSITION_IN_SHIFT);
                }

            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.PositionInShiftBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Tìm kiếm một Cương vị trong ca theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel<PositionInShiftModel> GetPositionInShiftById(int id)
        {
            var responseModel = new ResponseModel<PositionInShiftModel>();

            try
            {
                responseModel.Data = _positionInShiftService.GetPositionInShiftById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.PositionInShiftBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Tìm kiếm tất cả Cương vị trong ca theo cương vị ID
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel GetPositionInShifts(SearchPositionInShiftModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _positionInShiftService.GetPositionInShifts(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.PositionInShiftBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        /// <summary>
        /// Cập nhật Cương vị trong ca
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<PositionInShiftModel> UpdatePositionInShift(AddEditPositionInShiftModel requestModel)
        {
            var responseModel = new ResponseModel<PositionInShiftModel>();
            _errorMessages = new List<string>();

            try
            {
                responseModel.Data = _positionInShiftService.UpdatePositionInShift(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.PositionInShiftBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Kiểm tra dữ liệu bắt buộc khi tạo mới/cập nhật cương vị trong ca
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private bool IsValidData(AddEditPositionInShiftModel requestModel)
        {
            // Số lượng là bắt buộc
            if (requestModel.Quantity < 1)
            {
                _errorMessages.Add(CommonConstant.QUANTITY_REQUIRED);
            }

            // Thứ tự sắp xếp là bắt buộc
            if (requestModel.Position < 1)
            {
                _errorMessages.Add(CommonConstant.POSITION_REQUIRED);
            }

            return _errorMessages.Count <= 0;
        }
    }
}
