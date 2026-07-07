using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CERIOS.Common.Models;

namespace CERIOS.Systems.Interfaces.Common
{
    /// <summary>
    /// 通用控制器.
    /// </summary>
    public interface IFileService
    {
        Task<dynamic> Preview(string fileName, string fileDownloadUrl, string originalFileName, string fileExtension);

        dynamic DownloadUrl(string type, string fileName);

        Task<dynamic> DownloadAll(string type, List<FileControlsModel> input);

        Task<dynamic> DownloadFile(string encryption, string name);

        //下载文件
        /// <summary>
        /// 根据类型获取文件存储路径.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string GetPathByType(string type);

        /// <summary>
        /// 根据存储类型上传文件.
        /// </summary>
        /// <param name="uploadFilePath">上传文件地址.</param>
        /// <param name="directoryPath">保存文件夹.</param>
        /// <param name="fileName">新文件名.</param>
        /// <returns></returns>
        Task UploadFileByType(string uploadFilePath, string directoryPath, string fileName);
    }
}