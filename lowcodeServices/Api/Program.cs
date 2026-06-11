using Api;
using Api.Extensions;
using Application.Services;
using CeriOS.Core.Common.DB;
using CeriOS.Core.Extensions.Middlewares;
using CeriOS.Core.Extensions.ServiceExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.AddCorsSetup();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped(typeof(Repository<>));

builder.AddAutofacSetup2();
builder.AddLog4netSetup();
builder.AddRedisSetup();
InitTableAndData.InitTable();

builder.Services.AddControllers();


// 添加 Swagger/OpenAPI 支持
builder.Services.AddEndpointsApiExplorer();
builder.AddSwaggerSetup();
builder.AddAuthorizationSetup();

var app = builder.Build();

// Configure the HTTP request pipeline.



//app.UseSwaggerAuthorized();
app.UseHttpsRedirection();
app.UseCorsMiddleware();

// 开启静态文件  将客户端代码写入wwwroot中  防止跨域
app.UseStaticFiles();
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CeriOS.Core.Service API v1");
});

app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.Run();
