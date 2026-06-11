using CeriOS.Core.Common.DB;
using CeriOS.Core.Extensions.Middlewares;
using CeriOS.Core.Extensions.ServiceExtensions;
using CeriOS.Core.Extensions.ServiceFilter;
using CeriOS.LowCodeForm.Service.Service;
using CERIOS.Common.Cache;
using CERIOS.Common.Core.Manager.Files;
using CERIOS.Common.Manager;
using CERIOS.Common.Options;
using CERIOS.Systems.Interfaces.Common;
using CERIOS.ViewEngine;
using Mapster;
using OnceMi.AspNetCore.OSS;
using System.Reflection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// 手动加载 Configurations 文件夹下的配置文件
var configPath = Path.Combine(builder.Environment.ContentRootPath, "Configurations");
if (Directory.Exists(configPath))
{
    foreach (var file in Directory.GetFiles(configPath, "*.json"))
    {
        builder.Configuration.AddJsonFile(file, optional: true, reloadOnChange: true);
    }
}

// 使用反射初始化 CERIOS App 框架的内部配置
try
{
    var internalAppType = Type.GetType("CERIOS.App.Internal.InternalApp, CERIOS");
    if (internalAppType != null)
    {
        // 设置 Configuration
        var configField = internalAppType.GetField("Configuration", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        configField?.SetValue(null, builder.Configuration);

        // 设置 WebHostEnvironment
        var webHostEnvField = internalAppType.GetField("WebHostEnvironment", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        webHostEnvField?.SetValue(null, builder.Environment);

        // 设置 HostEnvironment
        var hostEnvField = internalAppType.GetField("HostEnvironment", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        hostEnvField?.SetValue(null, builder.Environment);
    }
}
catch (Exception ex)
{
    Console.WriteLine($"警告：无法初始化 CERIOS App 框架: {ex.Message}");
}

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();

// 使用标准 Options 模式注册配置（备用方案）
builder.Services.Configure<AppOptions>(builder.Configuration.GetSection("CERI_App"));
builder.Services.Configure<OssOptions>(builder.Configuration.GetSection("OSS"));

TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
//builder.Services.AddSignalR().AddNewtonsoftJsonProtocol();
builder.Services.AddMvc(options =>
{
    options.Filters.Add(typeof(ActionFilter));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddObjectMapper();
builder.AddSwaggerSetup();
builder.AddAuthorizationSetup();
builder.AddAutofacSetup();
builder.AddCorsSetup();
builder.AddLog4netSetup();
builder.AddRedisSetup();
InitTableAndData.InitTable();

builder.Services.AddScoped<IViewEngine, ViewEngine>();
// 注册 ICacheManager 及其实现
builder.Services.AddScoped<ICacheManager, CacheManager>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IFileManager, FileManager>();
builder.Services.AddScoped<IOSSServiceFactory, OSSServiceFactory>();
//builder.Services.AddScoped<ICache, MemoryCache>();
//builder.Services.AddScoped<ICache, RedisCache>();


var app = builder.Build();

// Configure the HTTP request pipeline.


app.UseSwaggerAuthorized();
app.UseSwaggerMildd();

app.UseHttpsRedirection();
app.UseRouting();

app.UseCorsMiddleware();

app.UseStaticFiles();


//app.UseAuthentication();
app.UseAuthorization();

//app.MapHub<ChatHub>("/api2/chatHub");

app.MapControllers();
app.Run();
