using CeriOS.Core.Common.DB;
using JNPF.Common.Dtos;
using JNPF.Common.Filter;
using CeriOS.示例.Entitys.Dto.Pictable;
using CeriOS.示例.Entitys;
using CeriOS.示例.Interfaces;
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
/// 业务实现：校验图片上传功能.
/// </summary>
[ApiDescriptionSettings(Tag = "示例", Name = "Pictable", Order = 200)]
[Route("api/[controller]")]
public class PictableService : ControllerBase, IPictableService
{
    public Repository<PictableEntity> _db { get; set; }

    static PictableService()
    {
        var cfg = TypeAdapterConfig.GlobalSettings;
        new CeriOS.示例.Entitys.Mapper.Pictable.Mapper().Register(cfg);
    }

    /// <summary>
    /// 初始化一个<see cref="PictableService"/>类型的新实例.
    /// </summary>
    public PictableService(Repository<PictableEntity> db)
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

   private static T JsonToObject<T>(object input)
   {
       if (input == null) return default;
       if (input is JsonElement jsonElement) return JsonSerializer.Deserialize<T>(jsonElement.GetRawText());
       if (input is string json)
       {
           if (string.IsNullOrWhiteSpace(json)) return default;
           return JsonSerializer.Deserialize<T>(json);
       }

       return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(input));
   }

   private static string JsonToString(object input)
   {
       return JsonSerializer.Serialize(input);
   }


    /// <summary>
    /// 获取校验图片上传功能.
    /// </summary>
    /// <param name="id">主键值.</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<dynamic> GetInfo(string id)
    {
        var data = (await _db.GetFirstAsync(it => it.id.Equals(id))).Adapt<PictableInfoOutput>();
        return new
        {
            code = 200,
            msg = "获取成功",
            data
        };      
    }

    /// <summary>
    /// 获取校验图片上传功能列表.
    /// </summary>
    /// <param name="input">请求参数.</param>
    /// <returns></returns>
    [HttpPost("List")]
    public async Task<dynamic> GetList([FromBody] PictableListQueryInput input)
    {
        var entityInfo = _db.Context.EntityMaintenance.GetEntityInfo(typeof(PictableEntity));
        var selectIds = input.selectIds?.Split(",").ToList();
      var query = _db.Context.Queryable<PictableEntity>();
      var isDeletedColumn = entityInfo.Columns.FirstOrDefault(o =>
        string.Equals(o?.PropertyName, "IsDeleted", System.StringComparison.OrdinalIgnoreCase) ||
        string.Equals(o?.DbColumnName, "IsDeleted", System.StringComparison.OrdinalIgnoreCase));
      if (isDeletedColumn != null)
      {
          query = query.Where(new List<IConditionalModel>
          {
              new ConditionalModel { FieldName = isDeletedColumn.DbColumnName, ConditionalType = ConditionalType.Equal, FieldValue = "0" }
          });
      }
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
        var __orderFields = new List<string>();
        foreach (var __rawSortField in (input.sidx ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            var __sortField = __rawSortField.Trim();
            var __isDesc = __sortField.StartsWith("-");
            var __sortFieldName = __isDesc ? __sortField.Substring(1) : __sortField;
            var __sortColumn = entityInfo.Columns.FirstOrDefault(o =>
                string.Equals(o?.PropertyName, __sortFieldName, System.StringComparison.OrdinalIgnoreCase) ||
                string.Equals(o?.DbColumnName, __sortFieldName, System.StringComparison.OrdinalIgnoreCase));
            if (__sortColumn == null) continue;
            __orderFields.Add(__sortColumn.DbColumnName + (__isDesc ? " DESC" : " ASC"));
        }
        if (__orderFields.Count > 0)
        {
            query = query.OrderBy(string.Join(",", __orderFields));
        }
        var __currentPage = input.currentPage <= 0 ? 1 : input.currentPage;
        var __pageSize = input.pageSize <= 0 ? 20 : input.pageSize;
        var __totalCount = await query.CountAsync();
        var data = await query.ToPagedListAsync(__currentPage, __pageSize);
      var __mapConfig = new TypeAdapterConfig();
      __mapConfig.Default.NameMatchingStrategy(NameMatchingStrategy.IgnoreCase);
      var inlineEditorList = data.list.Adapt<List<PictableListOutput>>(__mapConfig);
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
    /// 新建校验图片上传功能.
    /// </summary>
    /// <param name="input">参数.</param>
    /// <returns></returns>
    [HttpPost("")]
    public async Task<dynamic> Create([FromBody] PictableCrInput input)
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
        var entity = input.Adapt<PictableEntity>();
        entity.PicURL = input.PicURL != null && input.PicURL.Count > 0 ? JsonToString(input.PicURL).Replace("\r\n", "").Replace(" ", "") : null;
        entity.FileURL = input.FileURL != null && input.FileURL.Count > 0 ? JsonToString(input.FileURL).Replace("\r\n", "").Replace(" ", "") : null;
        entity.id = Guid.NewGuid().ToString("N");
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
    /// 更新校验图片上传功能.
    /// </summary>
    /// <param name="id">主键值.</param>
    /// <param name="input">参数.</param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<dynamic> Update(string id, [FromBody] PictableUpInput input)
    {
        var entity = input.Adapt<PictableEntity>();
        entity.PicURL = input.PicURL != null && input.PicURL.Count > 0 ? JsonToString(input.PicURL).Replace("\r\n", "").Replace(" ", "") : null;
        entity.FileURL = input.FileURL != null && input.FileURL.Count > 0 ? JsonToString(input.FileURL).Replace("\r\n", "").Replace(" ", "") : null;
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
    /// 删除校验图片上传功能.
    /// </summary>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<dynamic> Delete(string id)
    {
        var isOk = await _db.Context.Deleteable<PictableEntity>().Where(it => it.id.Equals(id)).ExecuteCommandAsync();   
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
    /// 批量删除校验图片上传功能.
    /// </summary>
    /// <param name="input">主键数组.</param>
    /// <returns></returns>
    [HttpPost("batchRemove")]
    public async Task<dynamic> BatchRemove([FromBody] List<string>? delids)
    {
        var ids = delids ?? new List<string>();
        if (ids.Count > 0)
        {
            var entitys = await _db.Context.Queryable<PictableEntity>().In(it => it.id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                 // 批量删除校验图片上传功能
                await _db.Context.Deleteable<PictableEntity>().In(it => it.id, ids).ExecuteCommandAsync();
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
    /// 校验图片上传功能详情.
    /// </summary>
    /// <param name="id">主键值.</param>
    /// <returns></returns>
    [HttpGet("Detail/{id}")]
    public async Task<dynamic> GetDetails(string id)
    {
          var data = (await _db.GetFirstAsync(it => it.id.Equals(id))).Adapt<PictableDetailOutput>();
          return new
          {
             code = 200,
             msg = "获取成功",
             data
          };         
    }
}
