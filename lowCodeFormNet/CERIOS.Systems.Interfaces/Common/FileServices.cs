using CeriOS.LowCodeForm.Model.Helper;
using CERIOS.Common.Captcha.General;
using CERIOS.Common.Configuration;
using CERIOS.Common.Core.Manager.Files;
using CERIOS.Common.Enums;
using CERIOS.Common.Extension;
using CERIOS.Common.Manager;
using CERIOS.Common.Models;
using CERIOS.Common.Options;
using CERIOS.Common.Security;
using CERIOS.DataEncryption;
using CERIOS.DependencyInjection.Dependencies;
using CERIOS.DynamicApiController;
using CERIOS.Logging.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using CERIOS.RemoteRequest.Extensions;
using CeriOS.Core.Model.ViewModel;

namespace CERIOS.Systems.Interfaces.Common
{
    /// <summary>
    /// 业务实现：通用控制器
    /// </summary>
    public class FileService : IFileService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly AppOptions _appOptions;

        /// <summary>
        /// 验证码处理程序.
        /// </summary>
        //private readonly IGeneralCaptcha _captchaHandler;

        ///// <summary>
        ///// 用户管理.
        ///// </summary>
        //private readonly IUserManager _userManager;

        /// <summary>
        /// 文件服务.
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// 缓存服务.
        /// </summary>
        private readonly ICacheManager _cacheManager;

        /// <summary>
        /// 初始化一个<see cref="FileService"/>类型的新实例.
        /// </summary>
        public FileService(

            IHttpContextAccessor httpContextAccessor,
            //IUserManager userManager,
            //ICacheManager cacheManager,
            IOptions<AppOptions> appOptions,
            IFileManager fileManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _appOptions = appOptions.Value;
            //_captchaHandler = captchaHandler;
            //_userManager = userManager;
            _fileManager = fileManager;
        }

        #region GET

        /// <summary>
        /// 上传文件预览.
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> Preview(string fileName, string fileDownloadUrl)
        {
            string[]? typeList = new string[] { "doc", "docx", "xls", "xlsx", "ppt", "pptx", "pdf", "jpg", "jpeg", "gif", "png", "bmp" };
            string? type = fileName.Split('.').LastOrDefault();
            if (typeList.Contains(type))
            {
                if (fileName.IsNotEmptyOrNull())
                {
                    string previewUrl = string.Empty;
                    switch (_appOptions.PreviewType)
                    {
                        case PreviewType.kkfile:
                            previewUrl = KKFileUploaderPreview(fileName, fileDownloadUrl);
                            break;
                        case PreviewType.yozo:
                            previewUrl = await YoZoUploaderPreview(fileName, 5, 1);
                            break;
                    }

                    return previewUrl;
                }
                else
                {
                    throw new Exception("文件不存在");
                }
            }
            else
            {
                throw new Exception("预览失败，文件类型不支持");
            }
        }

        /// <summary>
        /// 生成图片链接.
        /// </summary>
        /// <param name="type">图片类型.</param>
        /// <param name="fileName">注意 后缀名前端故意把 .替换@ .</param>
        /// <returns></returns>
        public async Task<IActionResult> GetImg(string type, string fileName)
        {
            string? filePath = Path.Combine(GetPathByType(type), fileName.Replace("@", "."));
            return await _fileManager.DownloadFileByType(filePath, fileName);
        }

        /// <summary>
        /// 生成大屏图片链接.
        /// </summary>
        /// <param name="type">图片类型.</param>
        /// <param name="fileName">注意 后缀名前端故意把 .替换@ .</param>
        /// <returns></returns>
        public async Task<IActionResult> GetScreenImg(string type, string fileName)
        {
            string filePath = Path.Combine(GetPathByType(type), type, fileName.Replace("@", "."));
            return await _fileManager.DownloadFileByType(filePath, fileName);
        }

        /// <summary>
        /// 获取图形验证码.
        /// </summary>
        /// <param name="timestamp">时间戳.</param>
        /// <returns></returns>
        //public async Task<IActionResult> GetCode(string timestamp)
        //{
        //    return new FileContentResult(await _captchaHandler.CreateCaptchaImage(timestamp, 114, 32), "image/jpeg");
        //}

        /// <summary>
        /// 下载.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="type"></param>
        public async Task FileDown(string fileName, [FromQuery] string type)
        {
            string? systemFilePath = Path.Combine(FileVariable.SystemFilePath, fileName);
            if (type.IsNotEmptyOrNull())
            {
                systemFilePath = Path.Combine(_fileManager.GetPathByType(type), fileName);
            }
            var fileStreamResult = await _fileManager.DownloadFileByType(systemFilePath, fileName);
            byte[] bytes = new byte[fileStreamResult.FileStream.Length];

            fileStreamResult.FileStream.Read(bytes, 0, bytes.Length);

            fileStreamResult.FileStream.Close();
            var httpContext = App.App.HttpContext;
            httpContext.Response.ContentType = "application/octet-stream";
            httpContext.Response.Headers.Add("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, Encoding.UTF8));
            httpContext.Response.Headers.Add("Content-Length", bytes.Length.ToString());
            await httpContext.Response.Body.WriteAsync(bytes);
            httpContext.Response.Body.Flush();
            httpContext.Response.Body.Close();
        }

        #region 下载附件

        /// <summary>
        /// 获取下载文件链接.
        /// </summary>
        /// <param name="type">图片类型.</param>
        /// <param name="fileName">文件名称.</param>
        /// <returns></returns>
        public dynamic DownloadUrl(string type, string fileName)
        {
            var userId = GetUser.GetUserIdByHttpContext(_httpContextAccessor);
            string? url = string.Format("{0}|{1}|{2}", userId, fileName, type);
            string? encryptStr = DESEncryption.Encrypt(url, "CERIOS");
            //_cacheManager.Set(fileName, string.Empty);
            return new { name = fileName, url = string.Format("/api/FormDb/DownloadFile?encryption={0}", encryptStr) };
        }

        /// <summary>
        /// 全部下载.
        /// </summary>
        /// <param name="type">图片类型.</param>
        /// <param name="fileName">文件名称.</param>
        /// <returns></returns>
        public async Task<dynamic> DownloadAll(string type, [FromBody] List<FileControlsModel> input)
        {
            var fileName = RandomExtensions.NextLetterAndNumberString(new Random(), 7);
            //临时目录
            string directoryPath = Path.Combine(App.App.GetConfig<AppOptions>("CERIOS_App", true).SystemPath, "TemporaryFile", fileName);
            Directory.CreateDirectory(directoryPath);
            foreach (var item in input)
            {
                string filePath = Path.Combine(GetPathByType(type), item.fileId.Replace("@", "."));
                var toFilePath = Path.Combine(directoryPath, item.fileName);
                if (FileHelper.Exists(toFilePath))
                {
                    var random = new Random().NextLetterAndNumberString(5).ToLower();
                    var fileExtension = Path.GetExtension(item.fileName);
                    var newFileName = string.Format("{0}副本{1}{2}", item.fileName.Replace(fileExtension, string.Empty), random, fileExtension);
                    toFilePath = Path.Combine(directoryPath, newFileName);
                }
                await _fileManager.CopyFile(filePath, toFilePath);
            }
            // 压缩文件
            string downloadPath = directoryPath + ".zip";

            // 判断是否存在同名称文件
            if (File.Exists(downloadPath))
                File.Delete(downloadPath);

            ZipFile.CreateFromDirectory(directoryPath, downloadPath);
            if (!App.App.Configuration["OSS:Provider"].Equals("Invalid"))
                await UploadFileByType(downloadPath, "SystemPath", string.Format("文件{0}.zip", fileName));
            var userId = GetUser.GetUserIdByHttpContext(_httpContextAccessor);
            var downloadFileName = string.Format("{0}|{1}.zip|TemporaryFile", userId, fileName);
            //_cacheManager.Set(fileName + ".zip", string.Empty);
            return new { downloadName = string.Format("文件{0}.zip", fileName), downloadVo = new { name = fileName, url = "/api/FormDb/DownloadFile?encryption=" + DESEncryption.Encrypt(downloadFileName, "CERIOS") } };
        }

        /// <summary>
        /// 下载文件链接.
        /// </summary>
        public async Task<dynamic> DownloadFile(string encryption, string name = null)
        {
            string decryptStr = DESEncryption.Decrypt(encryption, "CERI");
            List<string> paramsList = decryptStr.Split("|").ToList();
            if (paramsList.Count > 0)
            {
                string fileName = paramsList.Count > 1 ? paramsList[1] : string.Empty;
                //if (_cacheManager.Exists(fileName))
                //{
                //    _cacheManager.Del(fileName);
                //}
                //else
                //{
                //    throw new Exception("文件删除失败");
                //}
                string type = paramsList.Count > 2 ? paramsList[2] : string.Empty;
                string filePath = Path.Combine(GetPathByType(type), fileName.Replace("@", "."));
                string fileDownloadName = name.IsNullOrEmpty() ? fileName : name;
                return await _fileManager.DownloadFileByType(filePath, fileDownloadName);
            }
            else
            {
                throw new Exception("文件不存在");
            }
        }

        /// <summary>
        /// App启动信息.
        /// </summary>
        public async Task<dynamic> AppStartInfo(string appName)
        {
            return new { appVersion = KeyVariable.AppVersion, appUpdateContent = KeyVariable.AppUpdateContent };
        }

        #endregion

        /// <summary>
        /// 分片上传获取.
        /// </summary>
        /// <param name="input">请求参数.</param>
        /// <returns></returns>
        public async Task<QueryByIdResponseDto<dynamic>> CheckChunk([FromQuery] ChunkModel input)
        {
            try
            {
                if (!AllowFileType(input.extension, input.extension))
                    throw new Exception("上传失败，文件格式不允许上传");
                string path = GetPathByType(string.Empty);
                string filePath = Path.Combine(path, input.identifier);
                var chunkFiles = FileHelper.GetAllFiles(filePath);
                List<int> existsChunk = chunkFiles.FindAll(x => !FileHelper.GetFileType(x).Equals("tmp"))
                    .Select(x => x.FullName.Replace(input.identifier + "-", string.Empty).ParseToInt()).ToList();
                return new QueryByIdResponseDto<dynamic>() { Code = 200, Success = true, Data = new { chunkNumbers = existsChunk, merge = false } };
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #endregion

        #region POST

        /// <summary>
        /// 上传文件/图片.
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> Uploader(string type, [FromForm] ChunkModel input)
        {
            string? fileType = Path.GetExtension(input.file.FileName).Replace(".", string.Empty);
            if (!AllowFileType(fileType, type))
                throw new Exception("上传失败，文件格式不允许上传");
            string saveFileName = string.Format("{0}{1}{2}", DateTime.Now.ToString("yyyyMMdd"), RandomExtensions.NextLetterAndNumberString(new Random(), 5), Path.GetExtension(input.file.FileName));
            var stream = input.file.OpenReadStream();
            input.type = type;
            _fileManager.GetChunkModel(input, saveFileName);
            await _fileManager.UploadFileByType(stream, input.folder, saveFileName);
            if (AllowImageType(fileType) && type.Equals("annexpic"))
            {
                var slStram = await _fileManager.GetFileStream(Path.Combine(input.folder, saveFileName));
                await _fileManager.MakeThumbnail(slStram, saveFileName, input.folder);
                return new FileControlsModel { name = input.fileName, url = string.Format("/api/FormDb/Image/{0}/{1}", type, input.fileName), thumbUrl = string.Format("/api/FormDb/Image/{0}/{1}", type, input.slImgName), fileExtension = fileType, fileSize = input.file.Length, fileName = input.slImgName };
            }
            else
            {
                return new FileControlsModel { name = input.fileName, url = string.Format("/api/FormDb/Image/{0}/{1}", type, input.fileName), fileExtension = fileType, fileSize = input.file.Length, fileName = input.fileName };
            }
        }

        /// <summary>
        /// 上传文件/图片.
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> Uploader(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                byte[] fileBytes = memoryStream.ToArray();
                string base64String = Convert.ToBase64String(fileBytes);
                return string.Format("data:image/jpeg;base64,{0}", base64String);
            }
        }

        /// <summary>
        /// 上传图片.
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> UploadImage(IFormFile file)
        {
            string? ImgType = Path.GetExtension(file.FileName).Replace(".", string.Empty);
            if (!this.AllowImageType(ImgType))
                throw new Exception("上传失败，图片格式不允许上传");
            string? filePath = FileVariable.UserAvatarFilePath;
            string? fileName = string.Format("{0}{1}{2}", DateTime.Now.ToString("yyyyMMdd"), RandomExtensions.NextLetterAndNumberString(new Random(), 5), Path.GetExtension(file.FileName));
            var stream = file.OpenReadStream();
            await _fileManager.UploadFileByType(stream, filePath, fileName);
            return new FileControlsModel { name = fileName, url = string.Format("/api/FormDb/Image/userAvatar/{0}", fileName), fileSize = file.Length, fileExtension = ImgType };
        }

        /// <summary>
        /// 分片上传附件.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<QueryByIdResponseDto<dynamic>> UploadChunk([FromForm] ChunkModel input)
        {
            if (!AllowFileType(input.extension, input.extension))
                throw new Exception("上传失败，文件格式不允许上传");
            return await _fileManager.UploadChunk(input);
        }

        /// <summary>
        /// 分片组装.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<dynamic> Merge([FromForm] ChunkModel input)
        {
            return await _fileManager.Merge(input);
        }
        #endregion

        #region PublicMethod

        #region 多种存储文件

        /// <summary>
        /// 根据存储类型上传文件.
        /// </summary>
        /// <param name="uploadFilePath">上传文件地址.</param>
        /// <param name="directoryPath">保存文件夹.</param>
        /// <param name="fileName">新文件名.</param>
        /// <returns></returns>
        public async Task UploadFileByType(string uploadFilePath, string directoryPath, string fileName)
        {
            FileStream? file = new FileStream(uploadFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            await _fileManager.UploadFileByType(file, directoryPath, fileName);
        }
        #endregion

        /// <summary>
        /// 根据类型获取文件存储路径.
        /// </summary>
        /// <param name="type">文件类型.</param>
        /// <returns></returns>
        public string GetPathByType(string type)
        {
            return _fileManager.GetPathByType(type);
        }

        #region kkfile 文件预览

        /// <summary>
        /// KKFile 文件预览.
        /// </summary>
        /// <param name="fileName">文件名称.</param>
        /// <param name="fileDownloadUrl">文件地址.</param>
        /// <returns></returns>
        public string KKFileUploaderPreview(string fileName, string fileDownloadUrl)
        {
            var domain = _appOptions.Domain;
            var filePath = (domain + "/api/FormDb/Image/annex/" + fileName).ToBase64String();
            if (fileDownloadUrl.IsNotEmptyOrNull())
            {
                filePath = (fileDownloadUrl.StartsWith("http") ? fileDownloadUrl : domain + fileDownloadUrl).ToBase64String();
            }
            var kkFileDoMain = _appOptions.KKFileDomain;
            var kkurl = kkFileDoMain + "/onlinePreview?url=";

            return kkurl + filePath;
        }

        #endregion

        #region YoZo 生成 sign 方法

        /// <summary>
        /// 调用YoZo 文件预览.
        /// </summary>
        /// <param name="fileName">文件名.</param>
        /// <param name="maxNumber">最多请求次数.</param>
        /// <param name="number">当前请求次数.</param>
        /// <returns></returns>
        public async Task<string> YoZoUploaderPreview(string fileName, int maxNumber, int number)
        {
            string domain = _appOptions.YOZO.Domain;
            string uploadAPI = _appOptions.YOZO.UploadAPI;
            string downloadAPI = _appOptions.YOZO.DownloadAPI;
            string yozoAppId = _appOptions.YOZO.AppId;
            string yozoAppKey = _appOptions.YOZO.AppKey;
            string outputFilePath = string.Format("{0}/api/FormDb/Image/annex/{1}", domain, fileName);

            Dictionary<string, string[]> dic = new Dictionary<string, string[]>();
            dic.Add("fileUrl", new string[] { outputFilePath });
            dic.Add("appId", new string[] { yozoAppId });
            string? sign = generateSign(yozoAppKey, dic);
            uploadAPI = string.Format(uploadAPI, outputFilePath, yozoAppId, sign);
            string? resStr = await uploadAPI.PostAsStringAsync();
            if (resStr.IsNotEmptyOrNull())
            {
                Dictionary<string, object>? result = resStr.ToObject<Dictionary<string, object>>();
                if (result.ContainsKey("data"))
                {
                    Dictionary<string, object>? data = result["data"].ToObject<Dictionary<string, object>>();
                    if (data != null)
                    {
                        string? fileVersionId = data.ContainsKey("fileVersionId") ? data["fileVersionId"].ToString() : string.Empty;

                        #region 生成签名sign

                        dic = new Dictionary<string, string[]>();
                        dic.Add("fileVersionId", new string[] { fileVersionId });
                        dic.Add("appId", new string[] { yozoAppId });
                        sign = generateSign(yozoAppKey, dic);

                        #endregion

                        return string.Format(downloadAPI, fileVersionId, yozoAppId, sign);
                    }
                    else
                    {
                        return await YoZoUploaderPreview(fileName, maxNumber, number + 1);
                    }
                }
                else
                {
                    if (number >= maxNumber) return string.Empty;
                    else return await YoZoUploaderPreview(fileName, maxNumber, number + 1);
                }
            }
            else
            {
                if (number >= maxNumber) return string.Empty;
                else return await YoZoUploaderPreview(fileName, maxNumber, number + 1);
            }
        }

        #endregion

        #endregion

        #region PrivateMethod

        /// <summary>
        /// 生成令牌.
        /// </summary>
        /// <param name="secret">签名.</param>
        /// <param name="paramMap">参数集合.</param>
        /// <returns></returns>
        private string generateSign(string secret, Dictionary<string, string[]> paramMap)
        {
            string fullParamStr = uniqSortParams(paramMap);
            return HmacSHA256(fullParamStr, secret);
        }

        /// <summary>
        /// uniq类型参数.
        /// </summary>
        /// <param name="paramMap"></param>
        /// <returns></returns>
        private string uniqSortParams(Dictionary<string, string[]> paramMap)
        {
            paramMap.Remove("sign");
            paramMap = paramMap.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
            StringBuilder strB = new StringBuilder();
            foreach (KeyValuePair<string, string[]> kvp in paramMap)
            {
                string key = kvp.Key;
                string[] value = kvp.Value;
                if (value.Length > 0)
                {
                    Array.Sort(value);
                    foreach (string temp in value)
                    {
                        strB.Append(key).Append("=").Append(temp);
                    }
                }
                else
                {
                    strB.Append(key).Append("=");
                }

            }

            return strB.ToString();
        }

        /// <summary>
        /// 加密.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private string HmacSHA256(string data, string key)
        {
            string signRet = string.Empty;
            using (HMACSHA256 mac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hash = mac.ComputeHash(Encoding.UTF8.GetBytes(data));
                signRet = hash.ToHexString();
            }

            return signRet;
        }

        /// <summary>
        /// 允许文件类型.
        /// </summary>
        /// <param name="fileExtension">文件后缀名.</param>
        /// <param name="type">文件类型.</param>
        /// <returns></returns>
        private bool AllowFileType(string fileExtension, string type)
        {
            List<string> allowExtension = KeyVariable.AllowUploadFileType.Distinct().ToList();
            if (fileExtension.IsNullOrEmpty() || type.IsNullOrEmpty()) return false;
            if (type.Equals("weixin"))
                allowExtension = KeyVariable.WeChatUploadFileType;
            foreach (var item in Enum.GetValues(typeof(ExportFileType)))
            {
                allowExtension.Add(item.ToString());
            }
            return allowExtension.Any(a => a == fileExtension.ToLower());
        }

        /// <summary>
        /// 允许文件类型.
        /// </summary>
        /// <param name="fileExtension">文件后缀名.</param>
        /// <returns></returns>
        private bool AllowImageType(string fileExtension)
        {
            return KeyVariable.AllowImageType.Any(a => a == fileExtension.ToLower());
        }

        #endregion
    }
}
