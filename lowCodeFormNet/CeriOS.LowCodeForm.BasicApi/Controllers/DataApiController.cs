using Azure;
using CeriOS.Core.Common.Helper;
using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.Model.Model;
using CeriOS.LowCodeForm.Model.ViewModel;
using CeriOS.LowCodeForm.Service.IService;
using CERIOS.Common.Const;
using CERIOS.Common.Security;
using Jint;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using System.Text;

namespace CeriOS.LowCodeForm.BasicApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Policy = "PermissionPolicy")]
    public class DataApiController : ControllerBase
    {
        public IDataApiService _dataApiService;
        private readonly IDBConfigService _dbConfigService;
        private readonly IFormCategoryService _formCategoryService;
        private readonly IFormDbService _formDbService;

        public DataApiController(IDataApiService dataApiService, IDBConfigService dBConfigService, IFormCategoryService formCategoryService, IFormDbService formDbService)
        {

            _dataApiService = dataApiService;
            _dbConfigService = dBConfigService;
            _formCategoryService = formCategoryService;
            _formDbService = formDbService;
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="dataApi"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Save")]
        public async Task<ResponseDto> Save(DataApi dataApi)
        {
            return await _dataApiService.Save(dataApi);
        }



        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Delete")]
        public async Task<ResponseDto> Delete(string id)
        {
            return await _dataApiService.Delete(id);
        }



        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetById")]
        public async Task<QueryByIdResponseDto<DataApi>> GetById(string id)
        {
            return await _dataApiService.GetById(id);
        }


        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="category">接口分类</param>
        /// <param name="name">名称</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetList")]
        public async Task<QueryResponseDto<List<DataApi>>> GetList(int pageIndex, int pageSize, string? category, string? name, string? type)
        {
            return await _dataApiService.GetList(pageIndex, pageSize, category, name, type);
        }

        /// <summary>
        /// 测试接口
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("TestApi/{id}")]
        public async Task<object> TestApi(string id, [FromBody] TestApiRequestDto? data)
        {
            try
            {
                if (data == null)
                    data = new TestApiRequestDto();
                var obj = await _dataApiService.GetById(id);

                Object response = new object();
                if (obj.Data.Type == "0")
                {
                    response = obj.Data.StaticData;
                }
                else if (obj.Data.Type == "1")
                {
                    string sql = obj.Data.SqlStr;
                    obj.Data.FieldList = JsonConvert.DeserializeObject<List<FieldList>>(obj.Data.FieldListJson);
                    if (data.SqlApiParams.Count > 0)
                    {
                        sql = ReplaceSqlParams(obj.Data.SqlStr, data.SqlApiParams);
                    }
                    var sqlJson = await _dbConfigService.ExecuteSql(obj.Data.DbConfigId, sql);
                    if (obj.Data.FieldList.Count == 0)
                    {
                        response = sqlJson;
                    }
                    else
                    {
                        // 反序列化 SQL JSON
                        var jArray = JArray.Parse(sqlJson);

                        var dynamicList = new List<ExpandoObject>();

                        foreach (JObject row in jArray)
                        {
                            dynamic expando = new ExpandoObject();
                            var dict = (IDictionary<string, object>)expando;

                            foreach (var field in obj.Data.FieldList)
                            {
                                // SQL结果里有这个列才映射
                                if (row.TryGetValue(field.MapName, StringComparison.OrdinalIgnoreCase, out JToken value))
                                {
                                    dict[field.Name] = value.Type == JTokenType.Null ? null : value.ToObject<object>();
                                }
                            }

                            dynamicList.Add(expando);
                        }
                        string resultJson = JsonConvert.SerializeObject(dynamicList, Formatting.Indented);
                        response = dynamicList;
                    }

                }
                else if (obj.Data.Type == "2")
                {
                    using (var http = new HttpClient())
                    {
                        http.Timeout = TimeSpan.FromSeconds(10);
                        // 1. 解析 headers
                        var headersList = string.IsNullOrWhiteSpace(obj.Data.HeaderData)
                            ? new List<ApiParam>()
                            : JsonConvert.DeserializeObject<List<ApiParam>>(obj.Data.HeaderData);

                        foreach (var h in headersList.Where(x => !string.IsNullOrEmpty(x.name)))
                        {
                            http.DefaultRequestHeaders.TryAddWithoutValidation(h.name, h.value);
                        }

                        //请求内部token
                        var innerToken = await GetInnerToken();
                        http.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", innerToken);

                        // 2. 解析 query 参数
                        var queryParams = data.ApiParams
                            .Where(x => !string.IsNullOrEmpty(x.name))
                            .ToDictionary(x => x.name, x => x.value);

                        var url = obj.Data.ApiUrl;

                        if (queryParams.Any())
                        {
                            var queryString = string.Join("&", queryParams.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"));
                            url = url.Contains("?") ? $"{url}&{queryString}" : $"{url}?{queryString}";
                        }
                        if (obj.Data.Method == "GET")
                        {
                            response = await http.GetStringAsync(url);
                        }
                        else if (obj.Data.Method == "POST")
                        {
                            HttpContent content = null;

                            if (obj.Data.BodyType == "none")
                            {
                                content = new StringContent(""); // 空 body
                            }
                            else if (obj.Data.BodyType == "form-data")
                            {
                                var formDataList = string.IsNullOrWhiteSpace(obj.Data.FormData)
                                    ? new List<ApiParam>()
                                    : JsonConvert.DeserializeObject<List<ApiParam>>(obj.Data.FormData);

                                var formContent = new MultipartFormDataContent();
                                foreach (var item in formDataList.Where(x => !string.IsNullOrEmpty(x.name)))
                                {
                                    // 这里只考虑 text 类型，文件上传可以再扩展
                                    formContent.Add(new StringContent(item.value ?? ""), item.name);
                                }
                                content = formContent;
                            }
                            else if (obj.Data.BodyType == "x-www-form-urlencoded")
                            {
                                var wwwFormList = string.IsNullOrWhiteSpace(obj.Data.WwwFormData)
                                    ? new List<ApiParam>()
                                    : JsonConvert.DeserializeObject<List<ApiParam>>(obj.Data.WwwFormData);

                                var dict = wwwFormList
                                    .Where(x => !string.IsNullOrEmpty(x.name))
                                    .ToDictionary(x => x.name, x => x.value);

                                content = new FormUrlEncodedContent(dict);
                            }
                            else if (obj.Data.BodyType == "json")
                            {
                                var jsonStr = obj.Data.BodyJsonData ?? "{}";
                                content = new StringContent(jsonStr, Encoding.UTF8, "application/json");
                            }
                            else if (obj.Data.BodyType == "xml")
                            {
                                var xmlStr = obj.Data.BodyXmlData ?? "";
                                content = new StringContent(xmlStr, Encoding.UTF8, "application/xml");
                            }

                            var httpResp = await http.PostAsync(url, content);
                            response = await httpResp.Content.ReadAsStringAsync();
                        }
                    }
                }
                else if (obj.Data.Type == "3")
                {
                    string sql = obj.Data.SqlStr;
                    obj.Data.FieldList = JsonConvert.DeserializeObject<List<FieldList>>(obj.Data.FieldListJson);
                    if (data.SqlApiParams.Count > 0)
                    {
                        sql = ReplaceSqlParams(obj.Data.SqlStr, data.SqlApiParams);
                    }
                    var sqlJson = await _dbConfigService.ExecuteSql(obj.Data.DbConfigId, sql);
                    if (obj.Data.FieldList.Count == 0)
                    {
                        response = sqlJson;
                    }
                    else
                    {
                        // 反序列化 SQL JSON
                        var jArray = JArray.Parse(sqlJson);

                        var dynamicList = new List<ExpandoObject>();

                        foreach (JObject row in jArray)
                        {
                            dynamic expando = new ExpandoObject();
                            var dict = (IDictionary<string, object>)expando;

                            foreach (var field in obj.Data.FieldList)
                            {
                                // SQL结果里有这个列才映射
                                if (row.TryGetValue(field.MapName, StringComparison.OrdinalIgnoreCase, out JToken value))
                                {
                                    dict[field.Name] = value.Type == JTokenType.Null ? null : value.ToObject<object>();
                                }
                            }

                            dynamicList.Add(expando);
                        }
                        string resultJson = JsonConvert.SerializeObject(dynamicList, Formatting.Indented);
                        response = dynamicList;
                    }

                }

                //处理脚本

                var ss = HandlerJs(obj.Data.DataHandler, response);
                if (obj.Data.Type != "3")
                {

                    return new QueryByIdResponseDto<object>() { Data = ss };
                }
                else
                {
                    return ss;
                }
            }
            catch (Exception ex)
            {

                return new QueryByIdResponseDto<object>() {Success=false, Data = ex.Message };
            }



        }

        public static string HandlerJs(string jsScript, object data)
        {
            var engine = new Engine();

            // ⭐ 如果是 JSON 字符串，先转对象
            if (data is string str)
            {
                data = JsonConvert.DeserializeObject(str);
            }

            engine.SetValue("data", data);

            engine.Execute($"var func = {jsScript};");

            var result = engine.Invoke("func", engine.GetValue("data"));

            return JsonConvert.SerializeObject(result.ToObject(), Formatting.Indented);
        }



        private static async Task<string> GetInnerToken()
        {
            var getTokenHost = AppConfigurtaionServices.GetValue("AuthorityAddress");
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, getTokenHost + "/connect/token");
            var collection = new List<KeyValuePair<string, string>>
                        {
                            new("client_id", "lowcode"),
                            new("client_secret", "secret"),
                            new("grant_type", "client_credentials")
                        };
            var content = new FormUrlEncodedContent(collection);
            request.Content = content;
            var tokenResponse = await client.SendAsync(request);
            tokenResponse.EnsureSuccessStatusCode();
            var tokenResult = await tokenResponse.Content.ReadAsStringAsync();
            var obj = JObject.Parse(tokenResult);
            return "Bearer " + obj["access_token"].ToString();
        }


        public static string ReplaceSqlParams(string sql, List<ApiDataParams> paramList)
        {
            foreach (var param in paramList)
            {
                string placeholder = "{" + param.paramName + "}";

                if (!sql.Contains(placeholder)) continue;

                string valueStr = param.defaultValue?.ToString() ?? "";

                switch (param.paramType.ToLower())
                {
                    case "string":
                        valueStr = $"{valueStr}";
                        break;
                    case "datetime":
                        if (param.defaultValue is DateTime dt)
                            valueStr = $"{dt.ToString("yyyy-MM-dd HH:mm:ss")}";
                        break;
                    case "bool":
                        valueStr = (Convert.ToBoolean(param.defaultValue) ? "1" : "0"); // SQL Server bool 也可以用 1/0
                        break;
                        // int、double 等其他类型直接使用 ToString
                }

                sql = sql.Replace(placeholder, valueStr);
            }

            return sql;
        }




        /// <summary>
        /// 获取接口数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetApiData/{id}")]
        public async Task<QueryResponseDto<string>> GetApiData(string id, [FromBody] TestApiRequestDto data)
        {
            try
            {
                var obj = await _dataApiService.GetById(id);

                Object response = new object();
                if (obj.Data.Type == "0")
                {
                    response = obj.Data.StaticData;
                }
                else if (obj.Data.Type == "1")
                {
                    string sql = obj.Data.SqlStr;
                    obj.Data.FieldList = JsonConvert.DeserializeObject<List<FieldList>>(obj.Data.FieldListJson);
                    if (data.SqlApiParams.Count > 0)
                    {
                        sql = ReplaceSqlParams(obj.Data.SqlStr, data.SqlApiParams);
                    }
                    var sqlJson = await _dbConfigService.ExecuteSql(obj.Data.DbConfigId, sql);
                    if (obj.Data.FieldList.Count == 0)
                    {
                        response = sqlJson;
                    }
                    else
                    {
                        // 反序列化 SQL JSON
                        var jArray = JArray.Parse(sqlJson);

                        var dynamicList = new List<ExpandoObject>();

                        foreach (JObject row in jArray)
                        {
                            dynamic expando = new ExpandoObject();
                            var dict = (IDictionary<string, object>)expando;

                            foreach (var field in obj.Data.FieldList)
                            {
                                // SQL结果里有这个列才映射
                                if (row.TryGetValue(field.MapName, StringComparison.OrdinalIgnoreCase, out JToken value))
                                {
                                    dict[field.Name] = value.Type == JTokenType.Null ? null : value.ToObject<object>();
                                }
                            }

                            dynamicList.Add(expando);
                        }
                        string resultJson = JsonConvert.SerializeObject(dynamicList, Formatting.Indented);
                        response = dynamicList;
                    }

                }
                else if (obj.Data.Type == "2")
                {
                    using (var http = new HttpClient())
                    {
                        http.Timeout = TimeSpan.FromSeconds(10);
                        // 1. 解析 headers
                        var headersList = string.IsNullOrWhiteSpace(obj.Data.HeaderData)
                            ? new List<ApiParam>()
                            : JsonConvert.DeserializeObject<List<ApiParam>>(obj.Data.HeaderData);

                        foreach (var h in headersList.Where(x => !string.IsNullOrEmpty(x.name)))
                        {
                            http.DefaultRequestHeaders.TryAddWithoutValidation(h.name, h.value);
                        }

                        //请求内部token
                        var innerToken = await GetInnerToken();
                        http.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", innerToken);

                        // 2. 解析 query 参数
                        var queryParams = data.ApiParams
                            .Where(x => !string.IsNullOrEmpty(x.name))
                            .ToDictionary(x => x.name, x => x.value);

                        var url = obj.Data.ApiUrl;

                        if (queryParams.Any())
                        {
                            var queryString = string.Join("&", queryParams.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"));
                            url = url.Contains("?") ? $"{url}&{queryString}" : $"{url}?{queryString}";
                        }
                        if (obj.Data.Method == "GET")
                        {
                            response = await http.GetStringAsync(url);
                        }
                        else if (obj.Data.Method == "POST")
                        {
                            HttpContent content = null;

                            if (obj.Data.BodyType == "none")
                            {
                                content = new StringContent(""); // 空 body
                            }
                            else if (obj.Data.BodyType == "form-data")
                            {
                                var formDataList = string.IsNullOrWhiteSpace(obj.Data.FormData)
                                    ? new List<ApiParam>()
                                    : JsonConvert.DeserializeObject<List<ApiParam>>(obj.Data.FormData);

                                var formContent = new MultipartFormDataContent();
                                foreach (var item in formDataList.Where(x => !string.IsNullOrEmpty(x.name)))
                                {
                                    // 这里只考虑 text 类型，文件上传可以再扩展
                                    formContent.Add(new StringContent(item.value ?? ""), item.name);
                                }
                                content = formContent;
                            }
                            else if (obj.Data.BodyType == "x-www-form-urlencoded")
                            {
                                var wwwFormList = string.IsNullOrWhiteSpace(obj.Data.WwwFormData)
                                    ? new List<ApiParam>()
                                    : JsonConvert.DeserializeObject<List<ApiParam>>(obj.Data.WwwFormData);

                                var dict = wwwFormList
                                    .Where(x => !string.IsNullOrEmpty(x.name))
                                    .ToDictionary(x => x.name, x => x.value);

                                content = new FormUrlEncodedContent(dict);
                            }
                            else if (obj.Data.BodyType == "json")
                            {
                                var jsonStr = obj.Data.BodyJsonData ?? "{}";
                                content = new StringContent(jsonStr, Encoding.UTF8, "application/json");
                            }
                            else if (obj.Data.BodyType == "xml")
                            {
                                var xmlStr = obj.Data.BodyXmlData ?? "";
                                content = new StringContent(xmlStr, Encoding.UTF8, "application/xml");
                            }

                            var httpResp = await http.PostAsync(url, content);
                            response = await httpResp.Content.ReadAsStringAsync();
                        }
                    }
                }

                var fieldList = JsonConvert.DeserializeObject<List<FieldMapping>>(obj.Data.FieldListJson);
                var response2 = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(response.ToString());

                var result2 = response2.Select(item =>
                {
                    var newItem = new Dictionary<string, object>();

                    foreach (var mapping in fieldList)
                    {
                        if (item.ContainsKey(mapping.MapName))
                        {
                            newItem[mapping.Name] = item[mapping.MapName];
                        }
                    }

                    return newItem;
                }).ToList();

                var resultJson2 = JsonConvert.SerializeObject(result2, Formatting.Indented);

                return new QueryResponseDto<string>() { Data = resultJson2 };
            }
            catch (Exception ex)
            {

                return new QueryResponseDto<string>() { Success = false, Message = ex.Message };
            }



        }

        [HttpGet]
        [Route("GetFormList")]
        public async Task<QueryResponseDto<List<FormCategoryDto>>> GetFormList()
        {
            List<FormCategoryDto> list = new List<FormCategoryDto>();
            var forms = await _formDbService.GetFormDbAsync(1, 999);
            var categoryIds = forms.Data.Select(x => x.FormCategoryId).ToList();
            var categories = await _formCategoryService.GetFormCategoriesByIds(categoryIds);
            foreach (var item in categories)
            {
                List<FormDto> formList = new List<FormDto>();
                var form = forms.Data.Where(x => x.FormCategoryId == item.FormCategoryId).ToList();
                foreach (var obj in form)
                {
                    JObject jsonObject = JObject.Parse(obj.FormJson);
                    JArray fields = (JArray)jsonObject["fields"];
                    var result = new List<(string label, string tag)>();
                    foreach (var field in fields)
                    {
                        var config = field["__config__"];
                        if (config != null)
                        {
                            string label = config["label"]?.ToString();
                            string tag = config["tag"]?.ToString();
                            result.Add((label, tag));
                        }
                    }
                    formList.Add(new FormDto()
                    {
                        FormName = obj.Name,
                        FormId = obj.FormDesignId,
                        SaveField = result.Where(x=>x.tag== "CeriInput").Select(x=>x.label).ToList(),
                        ViewField = result.Select(x=>x.label).ToList()
                    });
                }
                list.Add(new FormCategoryDto()
                {
                    CategoryName=item.Name,
                    FormList= formList
                });
            }
            return new QueryResponseDto<List<FormCategoryDto>>() { Data = list };
        }
    }

    public class GetApiDataDto
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public class FieldMapping
    {
        public string Name { get; set; }
        public string MapName { get; set; }
    }

    public class FormCategoryDto
    {
        public string CategoryName { get; set; }
        public List<FormDto> FormList { get; set; }
    }
    public class FormDto
    {
        public string FormId { get; set; }
        public string FormName { get; set; }
        public List<string> SaveField { get; set; }
        public List<string> ViewField { get; set; }

    }



}
