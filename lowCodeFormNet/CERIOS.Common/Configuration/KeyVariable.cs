using CERIOS.Common.Enums;
using CERIOS.Common.Options;

namespace CERIOS.Common.Configuration;

/// <summary>
/// Key常量.
/// </summary>
public class KeyVariable
{
    private static readonly AppOptions _ceri = App.App.GetConfig<AppOptions>("CERI_App", true);

    private static readonly OssOptions Oss = App.App.GetConfig<OssOptions>("OSS", true);

    /// <summary>
    /// 系统文件路径.
    /// </summary>
    public static string SystemPath
    {
        get
        {
            return Oss.Provider.Equals(OSSProviderType.Invalid) ? (string.IsNullOrEmpty(_ceri.SystemPath) ? Directory.GetCurrentDirectory() : _ceri.SystemPath) : string.Empty;
        }
    }

    /// <summary>
    /// 允许上传图片类型.
    /// </summary>
    public static List<string> AllowImageType
    {
        get
        {
            return string.IsNullOrEmpty(_ceri.AllowUploadImageType.ToString()) ? new List<string>() : _ceri.AllowUploadImageType;
        }
    }

    /// <summary>
    /// 允许上传文件类型.
    /// </summary>
    public static List<string> AllowUploadFileType
    {
        get
        {
            return string.IsNullOrEmpty(_ceri.AllowUploadFileType.ToString()) ? new List<string>() : _ceri.AllowUploadFileType;
        }
    }

    /// <summary>
    /// 微信允许上传文件类型.
    /// </summary>
    public static List<string> WeChatUploadFileType
    {
        get
        {
            return string.IsNullOrEmpty(_ceri.WeChatUploadFileType.ToString()) ? new List<string>() : _ceri.WeChatUploadFileType;
        }
    }

    /// <summary>
    /// 过滤上传文件名称特殊字符.
    /// </summary>
    public static List<string> SpecialString
    {
        get
        {
            return string.IsNullOrEmpty(_ceri.SpecialString.ToString()) ? new List<string>() : _ceri.SpecialString;
        }
    }

    /// <summary>
    /// 过滤内网ip.
    /// </summary>
    public static List<string> InternalNetwork
    {
        get
        {
            return string.IsNullOrEmpty(_ceri.InternalNetwork.ToString()) ? new List<string>() : _ceri.InternalNetwork;
        }
    }

    /// <summary>
    /// MinIO桶.
    /// </summary>
    public static string BucketName
    {
        get
        {
            return string.IsNullOrEmpty(Oss.BucketName) ? string.Empty : Oss.BucketName;
        }
    }

    /// <summary>
    /// 文件储存类型.
    /// </summary>
    public static OSSProviderType FileStoreType
    {
        get
        {
            return string.IsNullOrEmpty(Oss.Provider.ToString()) ? OSSProviderType.Invalid : Oss.Provider;
        }
    }

    /// <summary>
    /// App版本.
    /// </summary>
    public static string AppVersion
    {
        get
        {
            return string.IsNullOrEmpty(App.App.Configuration["CERIOS_APP:AppVersion"]) ? string.Empty : App.App.Configuration["CERIOS_APP:AppVersion"];
        }
    }

    /// <summary>
    /// 文件储存类型.
    /// </summary>
    public static string AppUpdateContent
    {
        get
        {
            return string.IsNullOrEmpty(App.App.Configuration["CERIOS_APP:AppUpdateContent"]) ? string.Empty : App.App.Configuration["CERIOS_APP:AppUpdateContent"];
        }
    }
}