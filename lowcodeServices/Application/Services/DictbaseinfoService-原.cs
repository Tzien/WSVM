//using JNPF.Common.Core.Manager;
//using JNPF.Engine.Entity.Model;
//using JNPF.ClayObject;
//using JNPF.Common.Models.NPOI;
//using JNPF.Common.CodeGen.ExportImport;
//using JNPF.Common.Core.Manager.Files;
//using JNPF.Common.Dtos;
//using JNPF.Common.CodeGen.DataParsing;
//using JNPF.Common.Const;
//using JNPF.Common.Manager;
//using JNPF.Common.Enums;
//using JNPF.Common.Extension;
//using JNPF.Common.Filter;
//using JNPF.Common.Models;
//using JNPF.Common.Security;
//using JNPF.DependencyInjection;
//using JNPF.DynamicApiController;
//using JNPF.FriendlyException;
//using JNPF.Systems.Entitys.Permission;
//using JNPF.Systems.Entitys.System;
//using JNPF.Systems.Interfaces.System;
//using JNPF.Common.Dtos.Datainterface;
//using JNPF.示例.Entitys.Dto.Dictbaseinfo;
//using JNPF.示例.Entitys;
//using Mapster;
//using Microsoft.AspNetCore.Mvc;
//using SqlSugar;
//using JNPF.Common.Models.Authorize;
//using JNPF.DatabaseAccessor;
//using JNPF.Common.Dtos;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using Dm.util;
//using CeriOS.Core.Common.DB;
//using Domain.Entities;
//using Application.Interfaces;
//using System;

//namespace Application.Services;

///// <summary>
///// 业务实现：钢种管理.
///// </summary>
//[ApiDescriptionSettings(Tag = "示例", Name = "Dictbaseinfo", Order = 200)]
//[Route("api/[controller]")]
//[ApiController]
//public class DictbaseinfoService2 : ControllerBase, IDictbaseinfoService, IDynamicApiController, ITransient
//{
//    public Repository<DictbaseinfoEntity> _db { get; set; }

//    public DictbaseinfoService2(Repository<DictbaseinfoEntity> db)
//    {
//        _db = db;
//    }



//    /// <summary>
//    /// 获取钢种管理.
//    /// </summary>
//    /// <param name="id">主键值.</param>
//    /// <returns></returns>
//    [HttpGet("{id}")]
//    public async Task<dynamic> GetInfo(string id)
//    {
//        //var dbLink = await _repository.AsSugarClient().Queryable<DbLinkEntity>().FirstAsync(it => it.Id.Equals("1"));
//        //_sqlSugarClient = _dataBaseManager.ChangeDataBase(dbLink);

//        var data = (await _db
//            .GetFirstAsync(it => it.id.Equals(id))).Adapt<DictbaseinfoInfoOutput>(); 

//            return data;
//    }


//    /// <summary>
//    /// 获取钢种管理列表.
//    /// </summary>
//    /// <param name="input">请求参数.</param>
//    /// <returns></returns>
//    [HttpPost("List")]
//    public async Task<dynamic> GetList([FromBody] DictbaseinfoListQueryInput input)
//    {
//        //var dbLink = await _repository.AsSugarClient().Queryable<DbLinkEntity>().FirstAsync(it => it.Id.Equals("1"));
//        //_sqlSugarClient = _dataBaseManager.ChangeDataBase(dbLink);

//        //var entityInfo = _db.EntityMaintenance.GetEntityInfo<DictbaseinfoEntity>();
//        var entityInfo = _db.Context.EntityMaintenance.GetEntityInfo(typeof(DictbaseinfoEntity));

//        //var superQuery = SuperQueryHelper.GetSuperQueryInput(input.superQueryJson, string.Empty, entityInfo, 0);
//        //superQuery = await _controlParsing.ReplaceDynamicQuery<ConvertSuper>(superQuery);
//        //List<IConditionalModel> mainConditionalModel = SuperQueryHelper.GetSuperQueryJson(superQuery);
//        var selectIds = input.selectIds?.Split(",").ToList();
//        var data = await _db.Context.Queryable<DictbaseinfoEntity>()
//            .WhereIF(selectIds != null && selectIds.Any() && input.dataType.Equals(2), it => selectIds.Contains(it.id))
//            //.Where(mainConditionalModel)
//            //.Where(it => it.FlowId == null)
//            .Select(it => new DictbaseinfoListOutput
//            {
//                id = it.id,
//            }).MergeTable().OrderByIF(string.IsNullOrEmpty(input.sidx), it => it.id).OrderByIF(!string.IsNullOrEmpty(input.sidx), input.sidx + " " + input.sort).ToPagedListAsync(input.currentPage, input.pageSize);

//        var inlineEditorList = data.list.Adapt<List<DictbaseinfoListOutput>>();

//        return PageResult<DictbaseinfoListOutput>.SqlSugarPageResult(new SqlSugarPagedList<DictbaseinfoListOutput>
//        {
//            pagination = data.pagination,
//            list = inlineEditorList
//        });
//    }



//}
