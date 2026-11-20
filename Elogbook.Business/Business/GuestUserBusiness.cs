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
    public class GuestUserBusiness : IGuestUserBusiness
    {
        private readonly IGuestUserService _guestUserService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService"></param>
        public GuestUserBusiness(IGuestUserService guestUserService)
        {
            _guestUserService = guestUserService;
        }

        public ResponseBaseModel GetGuestUsers(SearchGuestUserModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _guestUserService.GetGuestUsers(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = CommonConstant.GUESTUSER_COMMON_ERROR;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.GuestUserBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<GuestUserModel> GetGuestUserById(int id)
        {
            var responseModel = new ResponseModel<GuestUserModel>();

            try
            {
                responseModel.Data = _guestUserService.GetGuestUserById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(CommonConstant.GUESTUSER_COMMON_ERROR);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.GuestUserBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<GuestUserModel> AddGuestUser(AddEditGuestUserModel requestModel)
        {
            var responseModel = new ResponseModel<GuestUserModel>();
            _errorMessages = new List<string>();

            try
            {
                if (!IsValidData(requestModel, true))
                {
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    responseModel.Data = _guestUserService.AddGuestUser(requestModel);
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(CommonConstant.GUESTUSER_COMMON_ERROR);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.GuestUserBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> UpdateGuestUser(AddEditGuestUserModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            _errorMessages = new List<string>();

            try
            {
                if (!IsValidData(requestModel, false))
                {
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    responseModel.Data = _guestUserService.UpdateGuestUser(requestModel);
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(CommonConstant.GUESTUSER_COMMON_ERROR);
                responseModel.Data = false;
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.GuestUserBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> DeleteGuestUser(int id, int userID)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                if (!_guestUserService.CheckInUseGuestUser(id))
                {
                    responseModel.Data = _guestUserService.DeleteGuestUser(id, userID);
                }
                else
                {
                    responseModel.Data = false;
                    responseModel.ErrorMessages.Add(CommonConstant.COMMON_DELETE_INUSE_DATA);
                }
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(CommonConstant.GUESTUSER_COMMON_ERROR);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.GuestUserBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> CheckInUseGuestUser(int id)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _guestUserService.CheckInUseGuestUser(id);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(CommonConstant.GUESTUSER_COMMON_ERROR);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.GuestUserBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        private bool IsValidData(AddEditGuestUserModel requestModel, bool isAddMode)
        {
            if (string.IsNullOrWhiteSpace(requestModel.FullName))
            {
                _errorMessages.Add(CommonConstant.FULLNAME_REQUIRED);
            }

            if (!string.IsNullOrWhiteSpace(requestModel.NationalID) && _guestUserService.CheckDuplicateData(requestModel.ID, requestModel.NationalID, isAddMode, fieldType: 1))
            {
                _errorMessages.Add(CommonConstant.DUPLICATE_NATIONALID);
            }

            if (!string.IsNullOrWhiteSpace(requestModel.ElectricRankCard) && _guestUserService.CheckDuplicateData(requestModel.ID, requestModel.ElectricRankCard, isAddMode, fieldType: 2))
            {
                _errorMessages.Add(CommonConstant.DUPLICATE_ELECTRICRANKCARD);
            }

            if (!string.IsNullOrWhiteSpace(requestModel.LaborCard) && _guestUserService.CheckDuplicateData(requestModel.ID, requestModel.LaborCard, isAddMode, fieldType: 3))
            {
                _errorMessages.Add(CommonConstant.DUPLICATE_LABORCARD);
            }

            // Bậc an toàn điện (x/y: x <= y) ví dụ: 1/5 
            if (!string.IsNullOrWhiteSpace(requestModel.ElectricRank))
            {
                string[] arrValue = requestModel.ElectricRank.Split('/');
                int.TryParse(arrValue[0], out int x);
                int.TryParse(arrValue[1], out int y);

                if (x > y)
                {
                    _errorMessages.Add("Bậc an toàn điện không hợp lệ");
                }

            }
            // Bậc thợ (x/y: x <= y) ví dụ: 1/7 
            if (!string.IsNullOrWhiteSpace(requestModel.LaborRank))
            {
                string[] arrValue = requestModel.LaborRank.Split('/');
                int.TryParse(arrValue[0], out int x);
                int.TryParse(arrValue[1], out int y);

                if (x > y)
                {
                    _errorMessages.Add("Bậc thợ không hợp lệ");
                }
            }

            return _errorMessages.Count <= 0;
        }
    }
}
