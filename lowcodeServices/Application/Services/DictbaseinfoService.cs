//using Application.Interfaces;
//using CeriOS.Core.Common.DB;
//using CeriOS.示例.Entitys;
//using CeriOS.示例.Entitys.Dto.Dictbaseinfo;
//using CeriOS.示例.Interfaces;
//using JNPF.Common.Dtos;
//using JNPF.Common.Filter;
//using JNPF.Common.Security;
//using JNPF.示例.Entitys.Dto.Dictbaseinfo;
//using Mapster;
//using Microsoft.AspNetCore.Mvc;
//using SqlSugar;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text.Json;
//using System.Threading.Tasks;

using CeriOS.Core.Common.DB;
using CeriOS.示例.Entitys;
using CeriOS.示例.Entitys.Dto.Dictbaseinfo;
using CeriOS.示例.Interfaces;
using JNPF.Common.Dtos;
using JNPF.Common.Filter;
using JNPF.Common.Security;
using JNPF.示例.Entitys.Dto.Dictbaseinfo;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;


namespace Application.Services;

/// <summary>
/// 业务实现：2测试代码生成Demo2.
/// </summary>
[ApiDescriptionSettings(Tag = "示例", Name = "Dictbaseinfo", Order = 200)]
[Route("api/[controller]")]
public class DictbaseinfoService : ControllerBase, IDictbaseinfoService
{
    public Repository<DictbaseinfoEntity> _db { get; set; }

    /// <summary>
    /// 初始化一个<see cref="DictbaseinfoService"/>类型的新实例.
    /// </summary>
    public DictbaseinfoService(Repository<DictbaseinfoEntity> db)
    {
        _db = db;
    }

    private static IDictionary<string, JsonElement> GetExtraFilters(object input)
    {
        if (input == null) return null;
        var type = input.GetType();
        var prop = type.GetProperty("Extra");
        if (prop == null) prop = type.GetProperty("extra");
        if (prop == null) return null;
        return prop.GetValue(input) as IDictionary<string, JsonElement>;
    }


    /// <summary>
    /// 获取2测试代码生成Demo2.
    /// </summary>
    /// <param name="id">主键值.</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<dynamic> GetInfo(string id)
    {
        var data = (await _db.GetFirstAsync(it => it.id.Equals(id))).Adapt<DictbaseinfoInfoOutput>();
        return new
        {
            code = 200,
            msg = "获取成功",
            data
        };
    }

    /// <summary>
    /// 获取2测试代码生成Demo2列表.
    /// </summary>
    /// <param name="input">请求参数.</param>
    /// <returns></returns>
    [HttpPost("List")]
    public async Task<dynamic> GetList([FromBody] DictbaseinfoListQueryInput input)
    {
        var entityInfo = _db.Context.EntityMaintenance.GetEntityInfo(typeof(DictbaseinfoEntity));
        var selectIds = input.selectIds?.Split(",").ToList();
        var query = _db.Context.Queryable<DictbaseinfoEntity>();
        var extra = GetExtraFilters(input);
        if (extra != null && extra.Count > 0)
        {
            var columnMap = entityInfo.Columns
                  .Where(o => !string.IsNullOrWhiteSpace(o?.PropertyName) && !string.IsNullOrWhiteSpace(o?.DbColumnName))
                  .GroupBy(o => o.PropertyName, StringComparer.OrdinalIgnoreCase)
                  .ToDictionary(g => g.Key, g => g.First().DbColumnName, StringComparer.OrdinalIgnoreCase);
            foreach (var col in entityInfo.Columns)
            {
                if (string.IsNullOrWhiteSpace(col?.DbColumnName) || string.IsNullOrWhiteSpace(col?.PropertyName)) continue;
                if (!columnMap.ContainsKey(col.DbColumnName))
                {
                    columnMap[col.DbColumnName] = col.DbColumnName;
                }
            }
            var conditions = new List<IConditionalModel>();
            foreach (var kv in extra)
            {
                var rawField = kv.Key;
                if (string.IsNullOrWhiteSpace(rawField)) continue;
                if (!columnMap.TryGetValue(rawField, out var field) || string.IsNullOrWhiteSpace(field)) continue;
                var v = kv.Value;
                if (v.ValueKind == JsonValueKind.Null || v.ValueKind == JsonValueKind.Undefined) continue;
                if (v.ValueKind == JsonValueKind.String)
                {
                    var s = v.GetString();
                    if (string.IsNullOrWhiteSpace(s)) continue;
                    conditions.Add(new ConditionalModel { FieldName = field, ConditionalType = ConditionalType.Like, FieldValue = $"%{s}%" });
                    continue;
                }
                if (v.ValueKind == JsonValueKind.Number || v.ValueKind == JsonValueKind.True || v.ValueKind == JsonValueKind.False)
                {
                    conditions.Add(new ConditionalModel { FieldName = field, ConditionalType = ConditionalType.Equal, FieldValue = v.ToString() });
                    continue;
                }
                if (v.ValueKind == JsonValueKind.Array)
                {
                    var list = v.EnumerateArray()
                        .Select(o => o.ToString())
                        .Where(o => !string.IsNullOrWhiteSpace(o))
                        .ToList();
                    if (list.Count == 0) continue;
                    conditions.Add(new ConditionalModel { FieldName = field, ConditionalType = ConditionalType.In, FieldValue = string.Join(",", list) });
                    continue;
                }
            }
            if (conditions.Count > 0)
            {
                query = query.Where(conditions);
            }
        }
        var data = await query.ToPagedListAsync(input.currentPage, input.pageSize);
        var inlineEditorList = data.list.Adapt<List<DictbaseinfoListOutput>>();
        return PageResult<DictbaseinfoListOutput>.SqlSugarPageResult(new SqlSugarPagedList<DictbaseinfoListOutput>
        {
            pagination = data.pagination,
            list = inlineEditorList
        });
    }

    /// <summary>
    /// 新建2测试代码生成Demo2.
    /// </summary>
    /// <param name="input">参数.</param>
    /// <returns></returns>
    [HttpPost("")]
    public async Task<dynamic> Create([FromBody] DictbaseinfoCrInput input)
    {
        var entity = input.Adapt<DictbaseinfoEntity>();
        entity.id = SnowflakeIdHelper.NextId();
        var isOk = await _db.InsertAsync(entity);
        if (!isOk)
        {
            return new
            {
                code = 200,
                msg = "保存失败",
                data = (object)null
            };
        }
        ;
        return new
        {
            code = 200,
            msg = "保存成功",
            data = (object)null
        };
    }

    /// <summary>
    /// 更新2测试代码生成Demo2.
    /// </summary>
    /// <param name="id">主键值.</param>
    /// <param name="input">参数.</param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<dynamic> Update(string id, [FromBody] DictbaseinfoUpInput input)
    {
        var entity = input.Adapt<DictbaseinfoEntity>();
        var isOk = await _db.UpdateAsync(entity);
        if (!isOk)
        {
            return new
            {
                code = 200,
                msg = "保存失败",
                data = (object)null
            };
        }
        ;
        return new
        {
            code = 200,
            msg = "保存成功",
            data = (object)null
        };
    }

    /// <summary>
    /// 删除2测试代码生成Demo2.
    /// </summary>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<dynamic> Delete(string id)
    {
        var isOk = await _db.Context.Deleteable<DictbaseinfoEntity>().Where(it => it.id.Equals(id)).ExecuteCommandAsync();
        if (isOk == 0)
        {
            return new
            {
                code = 200,
                msg = "删除失败",
                data = (object)null
            };
        }
        ;
        return new
        {
            code = 200,
            msg = "删除成功",
            data = (object)null
        };
    }

    /// <summary>
    /// 批量删除2测试代码生成Demo2.
    /// </summary>
    /// <param name="input">主键数组.</param>
    /// <returns></returns>
    [HttpPost("batchRemove")]
    public async Task<dynamic> BatchRemove([FromBody] DictbaseinfoBatchRemoveInput input)
    {
        var ids = input?.ids ?? new List<string>();
        if (ids.Count > 0)
        {
            var entitys = await _db.Context.Queryable<DictbaseinfoEntity>().In(it => it.id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                // 批量删除钢种管理
                await _db.Context.Deleteable<DictbaseinfoEntity>().In(it => it.id, ids).ExecuteCommandAsync();
            }
        }

        return new
        {
            code = 200,
            msg = "删除成功",
            data = (object)null
        };
    }

    /// <summary>
    /// 2测试代码生成Demo2详情.
    /// </summary>
    /// <param name="id">主键值.</param>
    /// <returns></returns>
    [HttpGet("Detail/{id}")]
    public async Task<dynamic> GetDetails(string id)
    {
        var data = (await _db.GetFirstAsync(it => it.id.Equals(id))).Adapt<DictbaseinfoDetailOutput>();
        return new
        {
            code = 200,
            msg = "获取成功",
            data
        };
    }
}

public class DictbaseinfoBatchRemoveInput
{
    public List<string> ids { get; set; }
}