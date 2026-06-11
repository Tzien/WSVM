using CeriOS.Core.Common.DB;
using JNPF.Common.Dtos;
using JNPF.Common.Filter;
using JNPF.Common.Security;
using CeriOS.示例.Entitys.Dto.I18Ntag;
using CeriOS.示例.Entitys;
using CeriOS.示例.Interfaces;
using CeriOS.示例.Entitys.Dto.Dictbaseinfo;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CeriOS.Core.Common.Helper;

namespace Application.Services;

/// <summary>
/// 业务实现：测试表单控件2.
/// </summary>
[ApiDescriptionSettings(Tag = "示例", Name = "I18Ntag", Order = 200)]
[Route("api/[controller]")]
public class I18NtagService : ControllerBase, II18NtagService
{
    public Repository<I18NtagEntity> _db { get; set; }

    /// <summary>
    /// 初始化一个<see cref="I18NtagService"/>类型的新实例.
    /// </summary>
    public I18NtagService(Repository<I18NtagEntity> db)
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
    /// 获取测试表单控件2.
    /// </summary>
    /// <param name="id">主键值.</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<dynamic> GetInfo(string id)
    {
        var data = (await _db.GetFirstAsync(it => it.id.Equals(id))).Adapt<I18NtagInfoOutput>();
        return new
        {
            code = 200,
            msg = "获取成功",
            data
        };      
    }

    /// <summary>
    /// 获取测试表单控件2列表.
    /// </summary>
    /// <param name="input">请求参数.</param>
    /// <returns></returns>
    [HttpPost("List")]
    public async Task<dynamic> GetList([FromBody] I18NtagListQueryInput input)
    {
        var entityInfo = _db.Context.EntityMaintenance.GetEntityInfo(typeof(I18NtagEntity));
        var selectIds = input.selectIds?.Split(",").ToList();
      var query = _db.Context.Queryable<I18NtagEntity>();
       var extra = GetExtraFilters(input);
       if (extra != null && extra.Count > 0)
        {
          Dictionary<string, int> searchTypes = null;
          if (extra.TryGetValue("__searchTypes", out var __stEl) && __stEl.ValueKind == JsonValueKind.Object)
          {
              try
              {
                  searchTypes = JsonSerializer.Deserialize<Dictionary<string, int>>(__stEl.GetRawText());
              }
              catch
              {
              }
          }
          var columnMap = entityInfo.Columns
                .Where(o => !string.IsNullOrWhiteSpace(o?.PropertyName) && !string.IsNullOrWhiteSpace(o?.DbColumnName))
                .GroupBy(o => o.PropertyName, System.StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First().DbColumnName, System.StringComparer.OrdinalIgnoreCase);
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
                if (rawField.Equals("__searchTypes", System.StringComparison.OrdinalIgnoreCase)) continue;
                if (!columnMap.TryGetValue(rawField, out var field) || string.IsNullOrWhiteSpace(field)) continue;
                var v = kv.Value;
                if (v.ValueKind == JsonValueKind.Null || v.ValueKind == JsonValueKind.Undefined) continue;
                if (v.ValueKind == JsonValueKind.String)
                {
                    var s = v.GetString();
                    if (string.IsNullOrWhiteSpace(s)) continue;
                    var __st = 0;
                    if (searchTypes != null && searchTypes.TryGetValue(rawField, out var __t)) __st = __t;
                    if (__st == 1)
                    {
                        conditions.Add(new ConditionalModel { FieldName = field, ConditionalType = ConditionalType.Equal, FieldValue = s });
                    }
                    else
                    {
                        conditions.Add(new ConditionalModel { FieldName = field, ConditionalType = ConditionalType.Like, FieldValue = $"%{s}%" });
                    }
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
        var __currentPage = input.currentPage <= 0 ? 1 : input.currentPage;
        var __pageSize = input.pageSize <= 0 ? 20 : input.pageSize;
        var __totalCount = await query.CountAsync();
        var data = await query.ToPagedListAsync(__currentPage, __pageSize);
      var inlineEditorList = data.list.Adapt<List<I18NtagListOutput>>();
        return new
        {
           list = inlineEditorList,
           pagination = new
           {
               currentPage = __currentPage,
               pageSize = __pageSize,
               total = __totalCount,
           }
        };
    }

    /// <summary>
    /// 新建测试表单控件2.
    /// </summary>
    /// <param name="input">参数.</param>
    /// <returns></returns>
    [HttpPost("")]
    public async Task<dynamic> Create([FromBody] I18NtagCrInput input)
    {
        if (input == null)
        {
            return new
            {
                code = 400,
                msg = "参数为空",
                data = (object)null
            };
        }
        var entity = input.Adapt<I18NtagEntity>();
        entity.id = GuidHelper.BuildGuid();
        var isOk = await _db.InsertAsync(entity);
        if (!isOk)
        {
            return new
            {
                code = 200,
                msg = "保存失败",
                data = (object)null
            };
        };
        return new
         {
             code = 200,
             msg = "保存成功",
             data = (object)null
         };
    }

    /// <summary>
    /// 更新测试表单控件2.
    /// </summary>
    /// <param name="id">主键值.</param>
    /// <param name="input">参数.</param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<dynamic> Update(string id, [FromBody] I18NtagUpInput input)
    {
        var entity = input.Adapt<I18NtagEntity>();
        var isOk = await _db.UpdateAsync(entity);
        if (!isOk)
        {
            return new
            {
                code = 200,
                 msg = "保存失败",
                data = (object)null
            };
        };
       return new
       {
           code = 200,
           msg = "保存成功",
           data = (object)null
       };
    }

    /// <summary>
    /// 删除测试表单控件2.
    /// </summary>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<dynamic> Delete(string id)
    {
        var isOk = await _db.Context.Deleteable<I18NtagEntity>().Where(it => it.id.Equals(id)).ExecuteCommandAsync();   
        if (isOk == 0)
        {
            return new
            {
                code = 200,
                msg = "删除失败",
                data = (object)null
            };
        };
         return new
         {
             code = 200,
             msg = "删除成功",
             data = (object)null
        };
    }

    /// <summary>
    /// 批量删除测试表单控件2.
    /// </summary>
    /// <param name="input">主键数组.</param>
    /// <returns></returns>
    [HttpPost("batchRemove")]
    public async Task<dynamic> BatchRemove([FromBody] List<string>? delids)
    {
        var ids = delids ?? new List<string>();
        if (ids.Count > 0)
        {
            var entitys = await _db.Context.Queryable<I18NtagEntity>().In(it => it.id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                 // 批量删除测试表单控件2
                await _db.Context.Deleteable<I18NtagEntity>().In(it => it.id, ids).ExecuteCommandAsync();
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
    /// 测试表单控件2详情.
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
