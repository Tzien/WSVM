using JNPF.Common.Core.Manager;
using JNPF.Engine.Entity.Model123;
using JNPF.Engine.Entity.Model;
using JNPF.ClayObject;
using JNPF.Common.Models.NPOI;
using JNPF.Common.CodeGen.ExportImport;
using JNPF.Common.Core.Manager.Files;
using JNPF.Common.Dtos;
using JNPF.Common.CodeGen.DataParsing;
using JNPF.Common.Const;
using JNPF.Common.Manager;
using JNPF.Common.Enums;
using JNPF.Common.Extension;
using JNPF.Common.Filter;
using JNPF.Common.Models;
using JNPF.Common.Security;
using JNPF.DependencyInjection;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.Systems.Entitys.Permission;
using JNPF.Systems.Entitys.System;
using JNPF.Systems.Interfaces.System;
using JNPF.Common.Dtos.Datainterface;
using JNPF.示例.Entitys.Dto.I18Ntag;
using JNPF.示例.Entitys;
using JNPF.示例.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using JNPF.Common.Models.Authorize;
using JNPF.DatabaseAccessor;
using JNPF.Common.Dtos;

namespace JNPF.示例;

/// <summary>
/// 业务实现：测试其他模板.
/// </summary>
[ApiDescriptionSettings(Tag = "示例", Name = "I18Ntag", Order = 200)]
[Route("api/示例/[controller]")]
public class I18NtagService : II18NtagService, IDynamicApiController, ITransient
{
    /// <summary>
    /// 服务基础仓储.
    /// </summary>
    private readonly ISqlSugarRepository<I18NtagEntity> _repository;


    /// <summary>
    /// 数据库管理.
    /// </summary>
    private readonly IDataBaseManager _dataBaseManager;

    /// <summary>
    /// 数据接口服务.
    /// </summary>
    private readonly IDataInterfaceService _dataInterfaceService;
    
    /// <summary>
    /// 缓存管理.
    /// </summary>
    private readonly ICacheManager _cacheManager;
    
    /// <summary>
    /// 通用数据解析.
    /// </summary>
    private readonly ControlParsing _controlParsing;

    /// <summary>
    /// 用户管理.
    /// </summary>
    private readonly IUserManager _userManager;
    
    /// <summary>
    /// 代码生成导出数据帮助类.
    /// </summary>
    private readonly ExportImportDataHelper _exportImportDataHelper;

    /// <summary>
    /// 文件服务.
    /// </summary>
    private readonly IFileManager _fileManager;


    /// <summary>
    /// 客户端.
    /// </summary>
    private static SqlSugarScope? _sqlSugarClient;

    /// <summary>
    /// 导出字段.
    /// </summary>
    private readonly List<ParamsModel> paramList = "[{\"value\":\"名称\",\"field\":\"TagName\"},{\"value\":\"Code\",\"field\":\"Code\"}]".ToList<ParamsModel>();

    /// <summary>
    /// 初始化一个<see cref="I18NtagService"/>类型的新实例.
    /// </summary>
    public I18NtagService(
        ISqlSugarRepository<I18NtagEntity> repository,
        IDataInterfaceService dataInterfaceService,
        IDataBaseManager dataBaseManager,
        ISqlSugarClient context,
        ExportImportDataHelper exportImportDataHelper,
        IFileManager fileManager,
        ICacheManager cacheManager,
        ControlParsing controlParsing,
        IUserManager userManager)
    {
        _repository = repository;
        _dataBaseManager = dataBaseManager;
        _sqlSugarClient = (SqlSugarScope)context;
        _exportImportDataHelper = exportImportDataHelper;
        _fileManager = fileManager;
        _dataInterfaceService = dataInterfaceService;
        _cacheManager = cacheManager;
        _controlParsing = controlParsing;
        _userManager = userManager;
    }

    /// <summary>
    /// 获取测试其他模板.
    /// </summary>
    /// <param name="id">主键值.</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<dynamic> GetInfo(string id)
    {
        var dbLink = await _repository.AsSugarClient().Queryable<DbLinkEntity>().FirstAsync(it => it.Id.Equals("13424066e0734d79bb37112cca9b7239"));
        _sqlSugarClient = _dataBaseManager.ChangeDataBase(dbLink);

        var data = (await _sqlSugarClient.Queryable<I18NtagEntity>()
            .FirstAsync(it => it.id.Equals(id))).Adapt<I18NtagInfoOutput>();

            return data;
    }

    /// <summary>
    /// 获取测试其他模板列表.
    /// </summary>
    /// <param name="input">请求参数.</param>
    /// <returns></returns>
    [HttpPost("List")]
    public async Task<dynamic> GetList([FromBody] I18NtagListQueryInput input)
    {
        var dbLink = await _repository.AsSugarClient().Queryable<DbLinkEntity>().FirstAsync(it => it.Id.Equals("13424066e0734d79bb37112cca9b7239"));
        _sqlSugarClient = _dataBaseManager.ChangeDataBase(dbLink);

        var selectIds = input.selectIds?.Split(",").ToList();
        var data = await _sqlSugarClient.Queryable<I18NtagEntity>()
            .WhereIF(selectIds!=null && selectIds.Any() && input.dataType.Equals(2), it => selectIds.Contains(it.id))
            .WhereIF(!string.IsNullOrEmpty(input.TagName), it => it.TagName.Contains(input.TagName))
            .Where(it => it.FlowId==null)
            .Select(it => new I18NtagListOutput
            {
                id = it.id,
            }).MergeTable().OrderByIF(string.IsNullOrEmpty(input.sidx), it => it.id).OrderByIF(!string.IsNullOrEmpty(input.sidx), input.sidx + " " + input.sort).ToPagedListAsync(input.currentPage, input.pageSize);

        var inlineEditorList = data.list.Adapt<List<I18NtagInlineEditorOutput>>();

        return PageResult<I18NtagInlineEditorOutput>.SqlSugarPageResult(new SqlSugarPagedList<I18NtagInlineEditorOutput>
        {
            pagination = data.pagination,
            list = inlineEditorList
        });
    }

    /// <summary>
    /// 新建测试其他模板.
    /// </summary>
    /// <param name="input">参数.</param>
    /// <returns></returns>
    [HttpPost("")]
    public async Task Create([FromBody] I18NtagCrInput input)
    {
        input = CodeGenHelper.SetEmptyStringNull(input);
        var dbLink = await _repository.AsSugarClient().Queryable<DbLinkEntity>().FirstAsync(it => it.Id.Equals("13424066e0734d79bb37112cca9b7239"));
        _sqlSugarClient = _dataBaseManager.ChangeDataBase(dbLink);

        var entity = input.Adapt<I18NtagEntity>();
        entity.id = GuidHelper.BuildGuid();
    var entity = input.Adapt<I18NtagEntity>();
    await _repository.InsertAsync(entity);
        if (!(isOk > 0)) throw Oops.Oh(ErrorCode.COM1000);
    }

    /// <summary>
    /// 更新测试其他模板.
    /// </summary>
    /// <param name="id">主键值.</param>
    /// <param name="input">参数.</param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task Update(string id, [FromBody] I18NtagUpInput input)
    {
        input = CodeGenHelper.SetEmptyStringNull(input);
        var dbLink = await _repository.AsSugarClient().Queryable<DbLinkEntity>().FirstAsync(it => it.Id.Equals("13424066e0734d79bb37112cca9b7239"));
        _sqlSugarClient = _dataBaseManager.ChangeDataBase(dbLink);

        var entity = input.Adapt<I18NtagEntity>();
    var entity = input.Adapt<I18NtagEntity>();
    entity.id = id;
    await _repository.UpdateAsync(entity);
        if (!(isOk > 0)) throw Oops.Oh(ErrorCode.COM1001);
    }

    /// <summary>
    /// 删除测试其他模板.
    /// </summary>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task Delete(string id)
    {
        var dbLink = await _repository.AsSugarClient().Queryable<DbLinkEntity>().FirstAsync(it => it.Id.Equals("13424066e0734d79bb37112cca9b7239"));
        _sqlSugarClient = _dataBaseManager.ChangeDataBase(dbLink);

        var isOk = await _sqlSugarClient.Deleteable<I18NtagEntity>().Where(it => it.id.Equals(id)).ExecuteCommandAsync();    
        if (!(isOk > 0)) throw Oops.Oh(ErrorCode.COM1002);
    }

    /// <summary>
    /// 批量删除测试其他模板.
    /// </summary>
    /// <param name="input">主键数组.</param>
    /// <returns></returns>
    [HttpPost("batchRemove")]
    [UnitOfWork]
    public async Task BatchRemove([FromBody] BatchRemoveInput input)
    {
        var dbLink = await _repository.AsSugarClient().Queryable<DbLinkEntity>().FirstAsync(it => it.Id.Equals("13424066e0734d79bb37112cca9b7239"));
        _sqlSugarClient = _dataBaseManager.ChangeDataBase(dbLink);

        var entitys = await _sqlSugarClient.Queryable<I18NtagEntity>().In(it => it.id, input.ids).ToListAsync();
        if (entitys.Count > 0)
        {
            // 批量删除测试其他模板
            await _sqlSugarClient.Deleteable<I18NtagEntity>().In(it => it.id,input.ids).ExecuteCommandAsync();
        }
    }

    /// <summary>
    /// 测试其他模板详情.
    /// </summary>
    /// <param name="id">主键值.</param>
    /// <returns></returns>
    [HttpGet("Detail/{id}")]
    [UnifySerializerSetting("special")]
    public async Task<dynamic> GetDetails(string id)
    {
        var dbLink = await _repository.AsSugarClient().Queryable<DbLinkEntity>().FirstAsync(it => it.Id.Equals("13424066e0734d79bb37112cca9b7239"));
        _sqlSugarClient = _dataBaseManager.ChangeDataBase(dbLink);

        var data = await _sqlSugarClient.Queryable<I18NtagEntity>()
            .Select(it => new I18NtagDetailOutput
            {
                id = it.id,
            }).MergeTable().Where(it => it.id == id).ToListAsync();
        return data.FirstOrDefault();
    }
}
