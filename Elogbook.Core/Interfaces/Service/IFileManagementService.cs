using Elogbook.Core.Models.Response;
using System.Collections.Generic;
using System.Net.Http;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Quản lý tệp tin
    /// </summary>
    public interface IFileManagementService
    {
        /// <summary>
        /// Download tệp
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        HttpResponseMessage DownloadFile(int id);

        /// <summary>
        /// Upload tệp
        /// </summary>
        /// <param name="localFileName"></param>
        /// <param name="filePath"></param>
        /// <param name="filePathThumbnail"></param>
        /// <param name="name"></param>
        /// <param name="screenName"></param>
        /// <param name="fileType"></param>
        /// <param name="fileExtension"></param>
        /// <param name="isLogoFactory"></param>
        /// <returns></returns>
        UploadFileModel UploadFile(string localFileName, string filePath, string filePathThumbnail, string name, string screenName, string fileType, string fileExtension, bool isLogoFactory, string moduleValue = null);

        /// <summary>
        /// Xóa tệp đính kèm
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool DeleteFile(int id, int userID);

        /// <summary>
        /// Zip file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileNames"></param>
        /// <param name="zipFileName"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        UploadFileModel ZipFiles(string filePath, List<string> fileNames, string zipFileName, int? userID);

    }
}
