using Microsoft.AspNetCore.Mvc;
using CeriOS.LowCodeForm.Service.IService;
using CeriOS.LowCodeForm.Model.Model;
using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.Model.ViewModel;
using Microsoft.IdentityModel.Tokens;
using Mapster;
using CeriOS.LowCodeForm.Model.ViewModel.LowCode;
using CeriOS.LowCodeForm.Service.LowCodeHelper.Service;
using Newtonsoft.Json;
using SqlSugar;
using CeriOS.LowCodeForm.BasicApi.DataEncryption.Encryptions;
using CeriOS.LowCodeForm.Model.ViewModel.Engine;
using CeriOS.Core.Common.Helper;
using CeriOS.Core.Model.Model;
using CeriOS.LowCodeForm.Model.ViewModel.LowCode.CodeGen;
using System.Text;
using CeriOS.LowCodeForm.Model.Helper;
using CeriOS.LowCodeForm.Model.Enums;
using CERIOS.Common.Extension;
using CERIOS.ViewEngine;
using System.IO.Compression;
using CERIOS.App;
using CERIOS.Systems.Interfaces.Common;
using CERIOS.Common.Configuration;
using CERIOS.Engine.Entity.Model;
using CERIOS.Common.Const;
using CERIOS.Engine.Entity.Model.CodeGen;
using CERIOS.Common.Security;
namespace CeriOS.LowCodeForm.BasicApi.Controller
{

    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Policy = "PermissionPolicy")]
    public class FormDbController : ControllerBase
    {
        public IFormDbService _FormDbService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IDBConfigService _DBConfigService;
        public IVisualPersonalService _VisualPersonalService;
        public IVisualDictionaryDataService _visualDictionaryDataService;
        public IVisualDevReleaseEntityService _visualDevReleaseEntityService;

        /// <summary>
        /// 文件服务.
        /// </summary>
        private readonly IFileService _fileService;

        /// <summary>
        /// 文件服务.
        /// </summary>
        //private readonly ICacheManager _cacheManager;

        /// <summary>
        /// 视图引擎.
        /// </summary>
        private readonly IViewEngine _viewEngine;

        public FormDbController(IViewEngine viewEngine, IVisualDevReleaseEntityService visualDevReleaseEntityService, IVisualDictionaryDataService visualDictionaryDataService, IFormDbService FormDbService, IHttpContextAccessor httpContextAccessor, IDBConfigService dBConfigService, IVisualPersonalService visualPersonalService, IFileService fileService)
        {
            _viewEngine = viewEngine;
            _visualDevReleaseEntityService = visualDevReleaseEntityService;
            _FormDbService = FormDbService;
            _httpContextAccessor = httpContextAccessor;
            _DBConfigService = dBConfigService;
            _VisualPersonalService = visualPersonalService;
            _visualDictionaryDataService = visualDictionaryDataService;
            _fileService = fileService;
        }

        [HttpGet]
        [Route("GetFormDbAsync")]
        public async Task<QueryResponseDto<List<FormDbDto>>> GetFormDbAsync(int pageIndex, int pageSize)
        {
            return await _FormDbService.GetFormDbAsync(pageIndex, pageSize);
        }


        [HttpGet]
        [Route("GetFormDbAsync2")]
        public async Task<QueryResponseDto<List<FormDbDto>>> GetFormDbAsync2(int pageIndex, int pageSize)
        {
            return await _FormDbService.GetFormDbAsync2(pageIndex, pageSize);
        }

        [HttpPost]
        [Route("InsertFormDbAsync")]
        public async Task<QueryByIdResponseDto<string>> InsertFormDbAsync(FormDbDto parameter)
        {
            return await _FormDbService.InsertFormDbAsync(parameter);
        }
        [HttpPost]
        [Route("UpdateFormDbAsync")]
        public async Task<ResponseDto> UpdateFormDbAsync(FormDbDto parameter)
        {
            return await _FormDbService.UpdateFormDbAsync(parameter);
        }

        [HttpGet]
        [Route("DeleteFormDbAsync")]
        public async Task<ResponseDto> DeleteFormDbAsync(string id)
        {
            return await _FormDbService.DeleteFormDbAsync(id);
        }

        [HttpGet]
        [Route("GetFormDbByIdAsync")]
        public async Task<QueryByIdResponseDto<FormDb>> GetFormDbByIdAsync(string id)
        {
            return await _FormDbService.GetFormDbByIdAsync(id);
        }

        [HttpGet]
        [Route("GetFormDbFormData")]
        public async Task<QueryByIdResponseDto<ModelDataConfigOutput>> GetData(string id, string type)
        {
            if (type.IsNullOrEmpty()) type = "1";
            var data = await _FormDbService.GetFormDesignDtoByIdAsync(id);
            if (data.Data == null) throw new Exception("该表单已删除");
            if (data.Data.FormJson.IsNullOrWhiteSpace()) throw new Exception("该模板内表单内容为空，无法预览!");
            if (data.Data.ColumnDataStr.IsNullOrWhiteSpace()) throw new Exception("该模板内列表内容为空，无法预览!");
            return await GetVisualDevModelDataConfig(data.Data);
            //return null;
        }


        /// <summary>
        /// 处理模板默认值.
        /// 用户选择 , 部门选择 , 岗位选择 , 角色选择 , 分组选择 ， 用户组件.
        /// </summary>
        /// <param name="config">模板.</param>
        /// <returns></returns>
        private async Task<QueryByIdResponseDto<ModelDataConfigOutput>> GetVisualDevModelDataConfig(FormDbDto config)
        {
            //if (config.WebType.Equals(4)) return config.Adapt<ModelDataConfigOutput>();

            var _runService = new RunService();

            var tInfo = new TemplateParsingBase();
            //if (config.Type == 1)
            //{
            //    tInfo = new TemplateParsingBase(config); // 解析模板
            //}
            //else if (config.Type == 2)
            //{
            //    tInfo = new TemplateParsingBase(config.FormData, config.Tables); // 解析模板
            //}
            tInfo = new TemplateParsingBase(config);
            //tInfo = new TemplateParsingBase(config.FormJson, config.TableJson); // 解析模板

            if (tInfo.AllFieldsModel.Any(x => (x.__config__.defaultCurrent) && (x.__config__.ceriKey.Equals(CeriKeyConst.USERSELECT) || x.__config__.ceriKey.Equals(CeriKeyConst.DEPSELECT) || x.__config__.ceriKey.Equals(CeriKeyConst.POSSELECT) || x.__config__.ceriKey.Equals(CeriKeyConst.ROLESELECT) || x.__config__.ceriKey.Equals(CeriKeyConst.GROUPSELECT) || x.__config__.ceriKey.Equals(CeriKeyConst.USERSSELECT))))
            {
                //获取用户ID
                var userId = GetUser.GetUserIdByHttpContext(_httpContextAccessor);
                //var userId = _userManager.UserId;
                //var depId = _visualDevRepository.AsSugarClient().Queryable<UserEntity, OrganizeEntity>((a, b) => new JoinQueryInfos(JoinType.Left, b.Id == a.OrganizeId))
                //    .Where((a, b) => a.Id.Equals(_userManager.UserId) && b.Category.Equals("department")).Select((a, b) => a.OrganizeId).First();
                //var posIds = _visualDevRepository.AsSugarClient().Queryable<PositionEntity, UserRelationEntity>((a, b) => new JoinQueryInfos(JoinType.Left, a.Id == b.ObjectId && b.ObjectType.Equals("Position")))
                //.Where((a, b) => b.UserId.Equals(_userManager.UserId) && a.OrganizeId.Equals(_userManager.User.OrganizeId)).Select(a => a.Id).ToList();
                //var roleIds = _visualDevRepository.AsSugarClient().Queryable<UserRelationEntity>()
                //    .Where(it => it.UserId.Equals(_userManager.UserId) && it.ObjectType.Equals("Role")).Select(it => it.ObjectId).ToList();
                //var groupIds = _visualDevRepository.AsSugarClient().Queryable<UserRelationEntity>()
                //    .Where(it => it.UserId.Equals(_userManager.UserId) && it.ObjectType.Equals("Group")).Select(it => it.ObjectId).ToList();

                //var allUserRelationList = _visualDevRepository.AsSugarClient().Queryable<UserRelationEntity>().Select(x => new UserRelationEntity() { UserId = x.UserId, ObjectId = x.ObjectId }).ToList();

                //var configData = config.FormJson.ToObject<Dictionary<string, object>>();
                var configData = JsonConvert.DeserializeObject<Dictionary<string, object>>(config.FormJson);
                //var columnList = configData["fields"].ToObject<List<Dictionary<string, object>>>();
                var columnList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(configData["fields"].ToString());
                _runService.FieldBindDefaultValue(ref columnList, userId);
                configData["fields"] = columnList;
                config.FormJson = JsonConvert.SerializeObject(configData);

                configData = JsonConvert.DeserializeObject<Dictionary<string, object>>(config.ColumnDataStr);
                var searchList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(configData["searchList"].ToString());
                columnList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(configData["columnList"].ToString());
                _runService.FieldBindDefaultValue(ref searchList, userId);
                _runService.FieldBindDefaultValue(ref columnList, userId);
                configData["searchList"] = searchList;
                configData["columnList"] = columnList;
                config.ColumnData = JsonConvert.SerializeObject(configData);

                //configData = config.AppColumnData.ToObject<Dictionary<string, object>>();
                //searchList = configData["searchList"].ToObject<List<Dictionary<string, object>>>();
                //columnList = configData["columnList"].ToObject<List<Dictionary<string, object>>>();
                //_runService.FieldBindDefaultValue(ref searchList, userId, depId, posIds, roleIds, groupIds, allUserRelationList);
                //_runService.FieldBindDefaultValue(ref columnList, userId, depId, posIds, roleIds, groupIds, allUserRelationList);
                //configData["searchList"] = searchList;
                //configData["columnList"] = columnList;
                //config.AppColumnData = configData.ToJsonString();
            }

            //var output = config.Adapt<ModelDataConfigOutput>();
            var output = new ModelDataConfigOutput()
            {
                appColumnData = config.AppColumnDataStr,
                columnData = config.ColumnDataStr,
                enCode = "1",
                flowEnCode = null,
                appUrlAddress = null,
                flowId = null,
                flowTemplateJson = null,
                formData = config.FormJson,
                fullName = config.Name,
                id = config.FormDesignId,
                interfaceId = "",
                propsValueList = new List<PropsValueModel>(),
                type = 1,
                webType = config.WebType.ToString(),
                urlAddress = null
            };

            // 主表主键
            var primaryKey = JsonConvert.DeserializeObject<List<ColumnInfo>>(tInfo.MainTable?.Fields).Find(x => x.IsPrimaryKey == true).Name;
            if (primaryKey.IsNullOrEmpty())
            {
                var data = await _DBConfigService.GetAllTables(config.DbId, tInfo.MainTableName);
                primaryKey = data[0].ColumnInfos.Find(a => a.IsPrimaryKey == true).Name;
            }

            output.propsValueList.Add(new PropsValueModel() { field = primaryKey, fieldName = "表单主键" });

            // 主表字段
            if (!tInfo.MainTableFieldsModelList.IsNullOrEmpty())
            {
                foreach (var mainTableField in tInfo.MainTableFieldsModelList)
                {
                    if (mainTableField.__config__.ceriKey.Equals(CeriKeyConst.COMINPUT) || mainTableField.__config__.ceriKey.Equals(CeriKeyConst.BILLRULE))
                        output.propsValueList.Add(new PropsValueModel() { field = mainTableField.__vModel__ + "_ceriId", fieldName = mainTableField.__config__.label });
                }
            }

            // 副表字段
            if (!tInfo.AuxiliaryTableFieldsModelList.IsNullOrEmpty())
            {
                foreach (var auxiliaryTableField in tInfo.AuxiliaryTableFieldsModelList)
                {
                    if (auxiliaryTableField.__config__.ceriKey.Equals(CeriKeyConst.COMINPUT) || auxiliaryTableField.__config__.ceriKey.Equals(CeriKeyConst.BILLRULE))
                        output.propsValueList.Add(new PropsValueModel() { field = auxiliaryTableField.__vModel__ + "_ceriId", fieldName = auxiliaryTableField.__config__.label });
                }
            }

            return new QueryByIdResponseDto<ModelDataConfigOutput>() { Data = output };
        }

        /// <summary>
        /// 列表.
        /// </summary>
        /// <param name="input">请求参数.</param>
        /// <returns>返回列表.</returns>
        [HttpGet("GetViewList")]
        public async Task<QueryByIdResponseDto<List<VisualPersonalListOutput>>> GetViewList(string menuId, string userId)
        {
            var list = await _VisualPersonalService.GetListBySth(menuId, userId);
            var data = list.Select(it => new VisualPersonalListOutput
            {
                id = it.F_ID,
                fullName = it.FullName,
                type = it.Type,
                status = it.Status,
                searchList = it.SearchList,
                columnList = it.ColumnList
            }).ToList();

            var defaultData = new VisualPersonalEntity()
            {
                F_ID = "systemId",
                FullName = "系统视图",
                Type = 0,
                Status = data.Any(x => x.status.Equals(1)) ? 0 : 1
            };

            var newDefaultData = new VisualPersonalListOutput()
            {
                columnList = defaultData.ColumnList,
                fullName = defaultData.FullName,
                id = defaultData.F_ID,
                searchList = defaultData.SearchList,
                status = defaultData.Status,
                type = defaultData.Type,
            };

            data.Insert(0, newDefaultData);
            var result = new QueryByIdResponseDto<List<VisualPersonalListOutput>>();
            result.Data = data;
            return result;
        }


        [HttpPost("CreateView")]
        public async Task<ResponseDto> Create(VisualPersonalCrInput input)
        {
            var result = new ResponseDto()
            {
                Code = 200,
                Message = "保存成功",
                Success = true
            };
            var dataList = await _VisualPersonalService.GetListBySth(input.menuId, input.userId);
            if (dataList.Count >= 5)
            {
                result.Message = "视图最多新建5个";
                result.Code = 500;
                return result;
            };
            if (input.fullName == "系统视图" || dataList.Any(x => x.FullName == input.fullName))
            {
                result.Message = "名称不能重复";
                result.Code = 500;
                return result;
            };

            var entity = new VisualPersonalEntity()
            {
                ColumnList = input.columnList,
                FullName = input.fullName,
                MenuId = input.menuId,
                SearchList = input.searchList,
            };
            entity.Status = 0;
            entity.Type = 1;
            entity.F_ID = Guid.NewGuid().ToString("N");
            entity.CreateId = input.userId;
            var isOk = await _VisualPersonalService.AddView(entity);
            if (isOk < 1)
            {
                result.Message = "新增数据失败";
                result.Code = 500;
                return result;
            }
            return result;
        }


        [HttpPost("UpdateView")]
        public async Task<ResponseDto> Update(string id, VisualPersonalUpInput input)
        {
            var result = new ResponseDto()
            {
                Code = 200,
                Message = "保存成功",
                Success = true
            };
            var dataById = await _VisualPersonalService.GetByIdSth(id, input.fullName, input.menuId, input.userId);
            if (input.fullName == "系统视图" || dataById > 0)
            {
                result.Message = "名称不能重复";
                result.Code = 500;
                return result;
            };
            var entity = new VisualPersonalEntity()
            {
                ColumnList = input.columnList,
                FullName = input.fullName,
                MenuId = input.menuId,
                SearchList = input.searchList,
            };
            entity.Status = 0;
            entity.Type = 1;
            entity.F_ID = id;
            entity.ModifyId = input.userId;


            var isOk = await _VisualPersonalService.UpdateView(entity);
            if (!isOk)
            {
                result.Message = "修改数据失败";
                result.Code = 500;
                return result;
            }
            return result;
        }

        [HttpPost("setDefault")]
        public async Task<ResponseDto> setDefault(string id, string menuId, string userId)
        {
            var result = new ResponseDto()
            {
                Code = 200,
                Message = "已成功切换默认视图",
                Success = true
            };

            var isOk = await _VisualPersonalService.SetDefault(id, menuId, userId);
            if (!isOk)
            {
                result.Message = "设置默认视图失败";
                result.Code = 500;
                return result;
            }
            return result;
        }


        [HttpDelete("DelView")]
        public async Task<ResponseDto> DelView(string id, string menuId)
        {
            var result = new ResponseDto()
            {
                Code = 200,
                Message = "删除成功",
                Success = true
            };
            var isSuccess = await _VisualPersonalService.DelView(id, menuId);

            if (!isSuccess)
            {
                result.Code = 500;
                result.Success = false;
                result.Message = "删除数据异常，请重试";
            }
            return result;
        }

        [HttpGet("AllDict")]
        public async Task<QueryByIdResponseDto<List<DictionaryDataAllListOutput>>> GetListAll()
        {
            var dictionaryDetail = await _visualDictionaryDataService.GetAllDictDetail();
            var dictionaryType = await _visualDictionaryDataService.GetAllDictData();
            var dictList = new List<DictionaryDataAllListOutput>();

            foreach (var item in dictionaryType)
            {
                dictList.Add(new DictionaryDataAllListOutput()
                {
                    isTree = item.DictType == "2" ? 1 : 0,
                    enCode = item.DictCode,
                    fullName = item.DictName,
                    id = item.DictBaseInfoId
                });
            }
            foreach (var item in dictList)
            {
                if (item.isTree == 1)
                {
                    var listSource = dictionaryDetail.FindAll(d => d.DictBaseInfoId == item.id);
                    var list = new List<DictionaryDataTreeOutput>();

                    //var list = listSource.Adapt<List<DictionaryDataTreeOutput>>();
                    foreach (var item2 in listSource)
                    {
                        list.Add(new DictionaryDataTreeOutput()
                        {
                            fullName = item2.ItemName,
                            enCode = item2.Code.ToString(),
                            enabledMark = item2.IsActive ? 1 : 0,
                            sortCode = item2.ItemSort,
                            id = item2.DictDetailId
                        });
                    }
                    var treeList = list.ToTree();

                    foreach (var item3 in list.Where(it => it.parentId == item.id))
                    {
                        var ori = item3.Adapt<DictionaryDataTreeOutput>();
                        ori.children = null;
                        treeList.Add(ori);
                    }

                    item.dictionaryList = treeList;
                }
                else
                {
                    item.dictionaryList = dictionaryDetail.FindAll(d => d.DictBaseInfoId == item.id).Adapt<List<DictionaryDataListOutput>>();
                }
            }
            return new QueryByIdResponseDto<List<DictionaryDataAllListOutput>>()
            {
                Code = 200,
                Data = dictList,
                Message = "OK",
                Success = true
            };
        }

        /// <summary>
        /// 下载文件链接.
        /// </summary>
        [HttpGet("DownloadFile")]
        public async Task<dynamic> DownloadFile(string encryption, string name = null)
        {
            return await _fileService.DownloadFile(encryption, name);
        }

        [HttpPost("DownloadCode/{id}")]
        public async Task<QueryByIdResponseDto<dynamic>> DownloadCode(string id, [FromBody] DownloadCodeFormInput downloadCodeForm)
        {
            //var templateEntity = await _visualDevReleaseEntityService.GetFirstAsync(id);
            var templateEntityold = await _FormDbService.GetFormDesignDtoByIdAsync(id);
            var templateEntity = templateEntityold.Data;
            _ = templateEntity ?? throw new Exception("检测数据不存在!");
            if (templateEntity.WebType.Equals(4)) templateEntity = GetCodeGenDataViewEntity(templateEntity);
            _ = templateEntity.FormDbs ?? throw new Exception("只能生成有表模板!");
            if (templateEntity.FormJson.IsNullOrEmpty()) throw new Exception("无法下载空表单!");
            if (templateEntity.WebType.Equals(2) && templateEntity.ColumnDataStr.IsNullOrEmpty()) throw new Exception("无法下载空列表!");

            //templateEntity.EnableFlow = downloadCodeForm.enableFlow;
            var tableList = templateEntity.FormDbs;
            var mainTable = tableList.Find(x => x.TypeId.Equals(1));

            // 表和字段重命名
            var aliasList = await _visualDevReleaseEntityService.GetAliasList(templateEntity.DbId);

            if (aliasList.Any(x => x.TableName.Equals(mainTable.TableName) && x.FieldName.IsNullOrEmpty() && !string.IsNullOrEmpty(x.AliasName)))
                downloadCodeForm.className = aliasList.Find(x => x.TableName.Equals(mainTable.TableName) && x.FieldName.IsNullOrEmpty() && !string.IsNullOrEmpty(x.AliasName)).AliasName;
            else
                downloadCodeForm.className = mainTable.TableName;
            //var model = templateEntity.FormData.ToObjectOld<FormDataModel>();
            var model = JsonConvert.DeserializeObject<FormDataModel>(templateEntity.FormJson);

            var client = new HttpClient();
            var ipAddress = AppConfigurtaionServices.GetValue("PlatFormUrl");
            var userToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var request = new HttpRequestMessage(HttpMethod.Get, $"{ipAddress}/api/DataDict/GetDictItemInfoById?dictItemId={downloadCodeForm.module}");
            request.Headers.Add("Authorization", "Bearer " + userToken);
            var response = await client.SendAsync(request);
            var tesss = await response.Content.ReadAsStringAsync();
            var dictionaryData = JsonConvert.DeserializeObject<QueryResponseDto<DictDetail>>(tesss).Data;


            downloadCodeForm.modulePackageName = dictionaryData.ItemName;
            if (templateEntity.WebType == 3)
                downloadCodeForm.modulePackageName = "WorkFlow";
            model.className = new List<string>() { downloadCodeForm.className.ParseToPascalCase() };
            model.areasName = downloadCodeForm.modulePackageName;
            string fileName = string.Format("{0}_{1:yyyyMMddHHmmss}", templateEntity.Name, DateTime.Now);

            //foreach (var item in tableList)
            //{
            //    if (item.TypeId.Equals("1"))
            //    {
            //        item.TableName = downloadCodeForm.className.ParseToPascalCase();
            //        item.tableName = downloadCodeForm.description;
            //    }
            //    else
            //    {
            //        if (aliasList.Any(x => x.TableName.Equals(item.table) && x.FieldName.IsNullOrEmpty() && x.AliasName.IsNotEmptyOrNull()))
            //            item.className = aliasList.Find(x => x.TableName.Equals(item.table) && x.FieldName.IsNullOrEmpty() && x.AliasName.IsNotEmptyOrNull()).AliasName;
            //        else
            //            item.className = item.table.ParseToPascalCase();
            //    }
            //}

            templateEntity.FormJson = JsonConvert.SerializeObject(model);
            templateEntity.FormDbs = tableList;

            // 模板数据聚合
            var tempDevEntity = new VisualDevEntity()
            {
                AppColumnData = templateEntity.AppColumnDataStr,
                Category = templateEntity.FormCategoryId,
                ColumnData = templateEntity.ColumnDataStr,
                CreateId = templateEntity.CreateId,
                CreateName = templateEntity.CreateName,
                CreateTime = templateEntity.CreateTime,
                DbLinkId = templateEntity.DbId,
                AppUrlAddress = null,
                Description = null,
                EnableFlow = 0,
                EnCode = templateEntity.Code,
                FlowId = null,
                FormData = templateEntity.FormJson,
                FullName = templateEntity.Name,
                Id = templateEntity.FormDesignId,
                InterfaceId = "",
                InterfaceParam = "",
                InterfaceUrl = "",
                IsDeleted = templateEntity.IsDeleted,
                ModifyId = templateEntity.ModifyId,
                ModifyName = templateEntity.ModifyName,
                ModifyTime = templateEntity.ModifyTime,
                State = templateEntity.Status,
                Tables = JsonConvert.SerializeObject(templateEntity.FormDbs),
                Type = 1,
                WebType = templateEntity.WebType.Value
            };
            await TemplatesDataAggregation(fileName, tempDevEntity, templateEntity, aliasList);
            string randPath = Path.Combine(KeyVariable.SystemPath, "CodeGenerate", fileName);
            string downloadPath = randPath + ".zip";

            // 判断是否存在同名称文件
            if (System.IO.File.Exists(downloadPath))
                System.IO.File.Delete(downloadPath);

            ZipFile.CreateFromDirectory(randPath, downloadPath);
            if (!App.Configuration["OSS:Provider"].Equals("Invalid"))
                await _fileService.UploadFileByType(downloadPath, "CodeGenerate", string.Format("{0}.zip", fileName));
            var userId = GetUser.GetUserIdByHttpContext(_httpContextAccessor);
            var downloadFileName = string.Format("{0}|{1}.zip|codeGenerator", userId, fileName);
            //_cacheManager.Set(fileName + ".zip", string.Empty);
            //return new { name = fileName, url = "/api/File/Download?encryption=" + DESEncryption.Encrypt(downloadFileName, "CERI") };

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            return new QueryByIdResponseDto<dynamic>()
            {
                Code = 200,
                Data = new { name = fileName, url = $"{baseUrl}/api/FormDb/DownloadFile?encryption=" + DESEncryption.Encrypt(downloadFileName, "CERI") },
                Message = "OK",
                Success = true
            };
        }

        [HttpPost("CodePreview/{id}")]
        public async Task<QueryByIdResponseDto<dynamic>> CodePreview(string id, [FromBody] DownloadCodeFormInput downloadCodeForm)
        {
            //var tEntity = await _repository.GetFirstAsync(v => v.Id == id && v.DeleteMark == null);
            var tEntityold = await _FormDbService.GetFormDesignDtoByIdAsync(id);
            var tEntity = tEntityold.Data;
            var dataList = await GetCodePreview(tEntity, downloadCodeForm);
            //if (downloadCodeForm.contrast)
            //{
                //var oldDataList = new List<Dictionary<string, object>>();
                //var vEntity = await _repository.AsSugarClient().Queryable<VisualDevEntity>().FirstAsync(v => v.Id == id && v.DeleteMark == null);
                //if (vEntity.State.Equals(2))
                //{
                //    oldDataList = await GetCodePreview(vEntity.Adapt<VisualDevReleaseEntity>(), downloadCodeForm);
                //}
                //else if (vEntity.State.Equals(1))
                //{
                //    var oldEntity = tEntity.OldContent.IsNotEmptyOrNull() ? tEntity.OldContent.ToObject<VisualDevReleaseEntity>() : tEntity.ToObject<VisualDevReleaseEntity>();
                //    if (tEntity.OldContent.IsNotEmptyOrNull())
                //    {
                //        oldEntity.EnCode = tEntity.EnCode;
                //        oldEntity.FullName = tEntity.FullName;
                //        oldEntity.Category = tEntity.Category;
                //        oldEntity.DbLinkId = tEntity.DbLinkId;
                //        oldEntity.CreatorTime = tEntity.CreatorTime;
                //        oldEntity.CreatorUserId = tEntity.CreatorUserId;
                //        if (oldEntity.WebType == null) oldEntity.WebType = tEntity.WebType;
                //    }
                //    oldDataList = await GetCodePreview(oldEntity, downloadCodeForm);
                //}

                //foreach (var item in dataList)
                //{
                //    var old = oldDataList.Find(x => x["fileName"].Equals(item["fileName"]));
                //    if (old != null)
                //    {
                //        var itemChildren = item["children"].ToObject<List<Dictionary<string, object>>>();
                //        var oldChildren = old["children"].ToObject<List<Dictionary<string, object>>>();

                //        foreach (var it in itemChildren)
                //        {
                //            var oldIT = oldChildren.Find(x => x["fileName"].Equals(it["fileName"]) && x["fileType"].Equals(it["fileType"]));
                //            if (oldIT != null)
                //            {
                //                if (vEntity.State.Equals(1))
                //                {
                //                    it["oldFileContent"] = oldIT["fileContent"];
                //                }
                //                else
                //                {
                //                    it["oldFileContent"] = it["fileContent"];
                //                    it["fileContent"] = oldIT["fileContent"];
                //                }
                //            }
                //        }

                //        item["children"] = itemChildren;
                //    }
                //}
            //}

            return new QueryByIdResponseDto<dynamic>()
            {
                Code = 200,
                Data = dataList,
                Message = "查询预览数据成功.",
                Success = true
            };
        }

        private async Task<List<Dictionary<string, object>>> GetCodePreview(FormDbDto templateEntity, [FromBody] DownloadCodeFormInput downloadCodeForm)
        {
            _ = templateEntity ?? throw new Exception("检测数据不存在.");
            if (templateEntity.WebType.Equals(4)) templateEntity = GetCodeGenDataViewEntity(templateEntity);
            _ = templateEntity.FormDbs ?? throw new Exception("只能生成有表模板.");
            if (templateEntity.FormJson.IsNullOrEmpty()) throw new Exception("无法预览空表单.");
            if (templateEntity.WebType.Equals(2) && templateEntity.ColumnDataStr.IsNullOrEmpty()) throw new Exception("无法预览空列表.");

            //templateEntity.EnableFlow = downloadCodeForm.enableFlow;
            var tableList = templateEntity.FormDbs;
            var mainTable = tableList.Find(x => x.TypeId.Equals(1));

            // 表和字段重命名
            var aliasList = await _visualDevReleaseEntityService.GetAliasList(templateEntity.DbId);

            if (aliasList.Any(x => x.TableName.Equals(mainTable.TableName) && x.FieldName.IsNullOrEmpty() && !string.IsNullOrEmpty(x.AliasName)))
                downloadCodeForm.className = aliasList.Find(x => x.TableName.Equals(mainTable.TableName) && x.FieldName.IsNullOrEmpty() && !string.IsNullOrEmpty(x.AliasName)).AliasName;
            else
                downloadCodeForm.className = mainTable.TableName;
            var model = JsonConvert.DeserializeObject<FormDataModel>(templateEntity.FormJson);

            var client = new HttpClient();
            var ipAddress = AppConfigurtaionServices.GetValue("PlatFormUrl");
            var userToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var request = new HttpRequestMessage(HttpMethod.Get, $"{ipAddress}/api/DataDict/GetDictItemInfoById?dictItemId={downloadCodeForm.module}");
            request.Headers.Add("Authorization", "Bearer " + userToken);
            var response = await client.SendAsync(request);
            var tesss = await response.Content.ReadAsStringAsync();
            var dictionaryData = JsonConvert.DeserializeObject<QueryResponseDto<DictDetail>>(tesss).Data;

            downloadCodeForm.modulePackageName = dictionaryData.ItemName;
            if (templateEntity.WebType == 3)
                downloadCodeForm.modulePackageName = "WorkFlow";
            model.className = new List<string>() { downloadCodeForm.className.ParseToPascalCase() };
            model.areasName = downloadCodeForm.modulePackageName;
            string fileName = string.Format("{0}_{1:yyyyMMddHHmmss}", templateEntity.Name, DateTime.Now);

            //foreach (var item in tableList)
            //{
            //    if (item.typeId.Equals("1"))
            //    {
            //        item.className = downloadCodeForm.className.ParseToPascalCase();
            //        item.tableName = downloadCodeForm.description;
            //    }
            //    else
            //    {
            //        if (aliasList.Any(x => x.TableName.Equals(item.table) && x.FieldName.IsNullOrEmpty() && x.AliasName.IsNotEmptyOrNull()))
            //            item.className = aliasList.Find(x => x.TableName.Equals(item.table) && x.FieldName.IsNullOrEmpty() && x.AliasName.IsNotEmptyOrNull()).AliasName;
            //        else
            //            item.className = item.table.ParseToPascalCase();
            //    }
            //}

            templateEntity.FormJson = JsonConvert.SerializeObject(model);
            templateEntity.FormDbs = tableList;

            // 模板数据聚合
            var tempDevEntity = new VisualDevEntity()
            {
                AppColumnData = templateEntity.AppColumnDataStr,
                Category = templateEntity.FormCategoryId,
                ColumnData = templateEntity.ColumnDataStr,
                CreateId = templateEntity.CreateId,
                CreateName = templateEntity.CreateName,
                CreateTime = templateEntity.CreateTime,
                DbLinkId = templateEntity.DbId,
                AppUrlAddress = null,
                Description = null,
                EnableFlow = 0,
                EnCode = templateEntity.Code,
                FlowId = null,
                FormData = templateEntity.FormJson,
                FullName = templateEntity.Name,
                Id = templateEntity.FormDesignId,
                InterfaceId = "",
                InterfaceParam = "",
                InterfaceUrl = "",
                IsDeleted = templateEntity.IsDeleted,
                ModifyId = templateEntity.ModifyId,
                ModifyName = templateEntity.ModifyName,
                ModifyTime = templateEntity.ModifyTime,
                State = templateEntity.Status,
                Tables = JsonConvert.SerializeObject(templateEntity.FormDbs),
                Type = 1,
                WebType = templateEntity.WebType.Value
            };

            await TemplatesDataAggregation(fileName, tempDevEntity, templateEntity, aliasList);
            string randPath = Path.Combine(KeyVariable.SystemPath, "CodeGenerate", fileName);
            var dataList = this.PriviewCode(randPath);
            if (dataList == null && dataList.Count == 0)
                throw new Exception("功能待完善.");
            return dataList;
        }

        /// <summary>
        /// 预览代码.
        /// </summary>
        /// <param name="codePath"></param>
        /// <returns></returns>
        private List<Dictionary<string, object>> PriviewCode(string codePath)
        {
            var dataList = CERIOS.Common.Security.FileHelper.GetAllFiles(codePath);
            List<Dictionary<string, string>> datas = new List<Dictionary<string, string>>();
            List<Dictionary<string, object>> allDatas = new List<Dictionary<string, object>>();
            foreach (var item in dataList)
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                var content = CERIOS.Common.Security.FileHelper.FileToString(item.FullName);

                switch (item.Extension)
                {
                    case ".cs":
                        {
                            string fileName = item.FullName.ToLower();
                            if (fileName.Contains("listqueryinput") || fileName.Contains("crinput") || fileName.Contains("upinput") || fileName.Contains("upoutput") || fileName.Contains("listoutput") || fileName.Contains("infooutput") || fileName.Contains("detailoutput") || fileName.Contains("inlineeditoroutput"))
                            {
                                data.Add("folderName", "dto");
                            }
                            else if (fileName.Contains("mapper"))
                            {
                                data.Add("folderName", "mapper");
                            }
                            else if (fileName.Contains("entity"))
                            {
                                data.Add("folderName", "entity");
                            }
                            else
                            {
                                data.Add("folderName", "dotnet");
                            }

                            data.Add("fileName", item.Name);

                            data.Add("fileContent", content);
                            data.Add("fileType", item.Extension.Replace(".", string.Empty));
                            datas.Add(data);
                        }
                        break;
                    case ".fff":
                        {
                            data.Add("folderName", "fff");
                            data.Add("id", GuidHelper.BuildGuid());
                            data.Add("fileName", item.Name);

                            data.Add("fileContent", content);
                            data.Add("fileType", item.Extension.Replace(".", string.Empty));
                            datas.Add(data);
                        }
                        break;
                    case ".vue":
                    case ".js":
                    case ".ts":
                        {
                            if (item.FullName.ToLower().Contains("pc"))
                                data.Add("folderName", "web");
                            else if (item.FullName.ToLower().Contains("app"))
                                data.Add("folderName", "app");

                            data.Add("id", GuidHelper.BuildGuid());
                            data.Add("fileName", item.Name);

                            data.Add("fileContent", content);
                            data.Add("fileType", item.Extension.Replace(".", string.Empty));
                            datas.Add(data);
                        }
                        break;
                }
            }

            // datas 集合去重
            foreach (var item in datas.GroupBy(d => d["folderName"]).Select(d => d.First()).OrderBy(d => d["folderName"]).ToList())
            {
                Dictionary<string, object> dataMap = new Dictionary<string, object>();
                dataMap["fileName"] = item["folderName"];
                dataMap["id"] = GuidHelper.BuildGuid();
                dataMap["children"] = datas.FindAll(d => d["folderName"] == item["folderName"]);
                allDatas.Add(dataMap);
            }

            return allDatas;
        }

        private async Task TemplatesDataAggregation(string fileName, VisualDevEntity templateEntity, FormDbDto formDbDto, List<VisualAliasEntity> aliasList)
        {
            // 类型名称
            var client = new HttpClient();
            var ipAddress = AppConfigurtaionServices.GetValue("PlatFormUrl");
            var userToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var request = new HttpRequestMessage(HttpMethod.Get, $"{ipAddress}/api/DataDict/GetDictItemInfoById?dictItemId={templateEntity.Category}");
            request.Headers.Add("Authorization", "Bearer " + userToken);
            var response = await client.SendAsync(request);
            var categoryobj = JsonConvert.DeserializeObject<QueryResponseDto<DictDetail>>(await response.Content.ReadAsStringAsync()).Data;

            //var categoryName = (await _dictionaryDataService.GetInfo(templateEntity.Category)).EnCode;

            // 表关系
            List<DbTableRelationModel>? tableRelation = new List<DbTableRelationModel>();
            var formDbs = JsonConvert.DeserializeObject<List<FormDb>>(templateEntity.Tables);
            foreach (var item in formDbs)
            {
                var dbTR = new DbTableRelationModel()
                {
                    className = item.TableName,
                    relationField = item.PK,
                    relationTable = item.TypeId == 1 ? "" : formDbs.Find(a => a.TypeId == 1).TableName,
                    table = item.TableName,
                    tableField = item.FK,
                    tableKey = null,
                    tableName = item.TableName,
                    typeId = item.TypeId.ToString()
                };
                tableRelation.Add(dbTR);
            }

            // 表单数据
            var formDataModel = JsonConvert.DeserializeObject<FormDataModel>(templateEntity.FormData);

            // 列表属性
            ColumnDesignModel? pcColumnDesignModel = JsonConvert.DeserializeObject<ColumnDesignModel>(templateEntity.ColumnData);
            ColumnDesignModel? appColumnDesignModel = JsonConvert.DeserializeObject<ColumnDesignModel>(templateEntity.AppColumnData);
            pcColumnDesignModel ??= new ColumnDesignModel();
            appColumnDesignModel ??= new ColumnDesignModel();

            // 既是行内编辑又是纯表单 强制改成普通列表.
            if (templateEntity.WebType.Equals(1) && pcColumnDesignModel.type.Equals(4))
            {
                var columnData = JsonConvert.DeserializeObject<Dictionary<string, object>>(templateEntity.ColumnData);
                columnData["type"] = 1;
                templateEntity.ColumnData = JsonConvert.SerializeObject(columnData);
            }

            // 分组和树形表格去掉复杂表头
            if (pcColumnDesignModel.type.Equals(3) || pcColumnDesignModel.type.Equals(5))
            {
                var columnData = JsonConvert.DeserializeObject<Dictionary<string, object>>(templateEntity.ColumnData);
                columnData["complexHeaderList"] = new List<object>();
                templateEntity.ColumnData = JsonConvert.SerializeObject(columnData);
            }

            // 开启数据权限
            bool useDataPermission = false;

            if (pcColumnDesignModel.useDataPermission && appColumnDesignModel.useDataPermission)
            {
                useDataPermission = true;
            }
            else if (!pcColumnDesignModel.useDataPermission && appColumnDesignModel.useDataPermission)
            {
                useDataPermission = true;
            }
            else if (pcColumnDesignModel.useDataPermission && !appColumnDesignModel.useDataPermission)
            {
                useDataPermission = true;
            }
            else
            {
                useDataPermission = false;
            }

            switch (templateEntity.WebType)
            {
                case 1:
                    useDataPermission = false;
                    break;
            }

            // 剔除多余布局控件组
            var controls = TemplateAnalysis.AnalysisTemplateData(formDataModel.fields);
            var fieldsCopy = JsonConvert.DeserializeObject<List<FieldsModel>>(JsonConvert.SerializeObject(formDataModel.fields));
            TemplateAnalysis.DataFormatReplace(controls);

            switch (templateEntity.WebType)
            {
                case 1:
                    break;
                default:
                    // 统一处理下表单内控件
                    controls = CodeGenUnifiedHandlerHelper.UnifiedHandlerFormDataModel(controls, pcColumnDesignModel, appColumnDesignModel);
                    controls = CodeGenUnifiedHandlerHelper.UnifiedHandlerControlRelationship(controls);
                    break;
            }

            List<string> targetPathList = new List<string>();
            List<string> templatePathList = new List<string>();

            string tableName = string.Empty;
            CodeGenConfigModel codeGenConfigModel = new CodeGenConfigModel();

            // 主表代码生成配置模型
            CodeGenConfigModel codeGenMainTableConfigModel = new CodeGenConfigModel();

            // 子表表名和主键.
            var ctPrimaryKey = new Dictionary<string, string>();

            /*
         * 区分是纯主表、主带副、主带子、主带副与子
         * 1-纯主表、2-主带子、3-主带副、4-主带副与子
         * 生成模式
         * 因ORM原因 导航查询 一对多 列表查询
         * 不能使用ORM 自带函数 待作者开放.Select()
         * 导致一对多列表查询转换必须全使用子查询
         * 远端数据与静态数据无法列表转换所以全部ThenMapper内转换
         * 数据字典又分为两种值转换ID与EnCode
         */
            var modelType = JudgmentGenerationModel(tableRelation, controls);
            if (templateEntity.WebType.Equals(4)) modelType = GeneratePatterns.DataView;


            var defaultLink = await _DBConfigService.GetDBConfigByIdAsync(templateEntity.DbLinkId);
            //var defaultLink = _databaseManager.GetTenantDbLink(string.Empty, string.Empty);
            switch (modelType)
            {
                case GeneratePatterns.DataView:
                    {
                        var viewDataModel = JsonConvert.DeserializeObject<FormDataModel>(templateEntity.FormData);
                        codeGenConfigModel.NameSpace = viewDataModel.areasName;
                        codeGenConfigModel.BusName = templateEntity.FullName;
                        codeGenConfigModel.ClassName = viewDataModel.className.FirstOrDefault();
                        targetPathList = CodeGenTargetPathHelper.BackendDataViewTargetPathList(formDataModel.className.FirstOrDefault(), fileName);
                        templatePathList = CodeGenTargetPathHelper.BackendDataViewTemplatePathList("6-DataView");

                        var tContent = System.IO.File.ReadAllText(templatePathList.First());
                        //var tResult = _viewEngine.RunCompileFromCached(tContent, new
                        //{
                        //    Id = templateEntity.Id,
                        //    NameSpace = codeGenConfigModel.NameSpace,
                        //    BusName = codeGenConfigModel.BusName,
                        //    ClassName = codeGenConfigModel.ClassName,
                        //});
                        //var dirPath = new DirectoryInfo(targetPathList.First()).Parent.FullName;
                        //if (!Directory.Exists(dirPath))
                        //    Directory.CreateDirectory(dirPath);
                        //System.IO.File.WriteAllText(targetPathList.First(), tResult, Encoding.UTF8);
                    }

                    break;
                //case GeneratePatterns.MainBelt:
                //    {
                //        var link = await _repository.AsSugarClient().Queryable<DbLinkEntity>().FirstAsync(m => m.Id == templateEntity.DbLinkId && m.DeleteMark == null);
                //        var targetLink = link ?? _databaseManager.GetTenantDbLink(_userManager.TenantId, _userManager.TenantDbName);

                //        List<CodeGenTableRelationsModel> tableRelationsList = new List<CodeGenTableRelationsModel>();

                //        var tableNo = 0;

                //        // 生成子表
                //        foreach (DbTableRelationModel? item in tableRelation.FindAll(it => it.typeId == "0"))
                //        {
                //            tableNo++;
                //            var controlId = string.Empty;
                //            var children = controls.Find(it => it.__config__.ceriKey.Equals(CeriKeyConst.TABLE) && it.__config__.tableName.Equals(item.table));
                //            if (children == null) continue;
                //            controlId = children.__vModel__;
                //            if (children != null) controls = children.__config__.children;

                //            var fieldList = _databaseManager.GetFieldList(targetLink, item.table);
                //            KingbaseNetType(fieldList, targetLink);
                //            fieldList = CodeGenWay.GetTableFieldModelReName(item.table, fieldList, aliasList);
                //            ctPrimaryKey.Add(item.table, fieldList.Find(x => x.primaryKey).field);

                //            if (fieldList.Count == 0) throw Oops.Oh(ErrorCode.D2106);

                //            // 默认主表开启自增子表也需要开启自增
                //            if (formDataModel.primaryKeyPolicy == 2 && !fieldList.Any(it => it.primaryKey && it.identity))
                //                throw Oops.Oh(ErrorCode.D2109);

                //            // 后端生成
                //            codeGenConfigModel = CodeGenWay.ChildTableBackEnd(item.table, item.className, fieldList, controls, templateEntity, controlId, item.tableField);
                //            codeGenConfigModel.IsMapper = true;
                //            codeGenConfigModel.BusName = children.__config__.label;
                //            codeGenConfigModel.ClassName = item.className;

                //            targetPathList = CodeGenTargetPathHelper.BackendChildTableTargetPathList(codeGenConfigModel.ClassName, fileName, templateEntity.WebType, templateEntity.Type, codeGenConfigModel.IsMapper, codeGenConfigModel.IsShowSubTableField);
                //            templatePathList = CodeGenTargetPathHelper.BackendChildTableTemplatePathList("SubTable", templateEntity.WebType, templateEntity.Type, codeGenConfigModel.IsMapper, codeGenConfigModel.IsShowSubTableField);

                //            // 生成子表相关文件
                //            for (int i = 0; i < templatePathList.Count; i++)
                //            {
                //                var tContent = File.ReadAllText(templatePathList[i]);
                //                var tResult = _viewEngine.RunCompileFromCached(tContent, new
                //                {
                //                    Id = templateEntity.Id,
                //                    IsInlineEditor = pcColumnDesignModel.type.Equals(4),
                //                    IsMainTable = false,
                //                    BusName = codeGenConfigModel.BusName,
                //                    ClassName = codeGenConfigModel.ClassName,
                //                    NameSpace = formDataModel.areasName,
                //                    PrimaryKey = codeGenConfigModel.TableField.Find(it => it.PrimaryKey).ColumnName,
                //                    MainClassName = codeGenConfigModel.ClassName,
                //                    OriginalMainTableName = item.table,
                //                    TableField = codeGenConfigModel.TableField,
                //                    RelationsField = codeGenConfigModel.RelationsField,
                //                    IsUploading = codeGenConfigModel.IsUpload,
                //                    IsMapper = codeGenConfigModel.IsMapper,
                //                    WebType = templateEntity.WebType,
                //                    Type = templateEntity.Type,
                //                    PrimaryKeyPolicy = codeGenConfigModel.PrimaryKeyPolicy,
                //                    IsImportData = pcColumnDesignModel.btnsList.Any(x => x.value.Equals("upload") && x.show) && codeGenConfigModel.IsImportData,
                //                    EnableFlow = templateEntity.EnableFlow == 0 ? false : true,
                //                    IsLogicalDelete = codeGenConfigModel.IsLogicalDelete,
                //                    TableType = codeGenConfigModel.TableType,
                //                    IsTenantColumn = false,
                //                });
                //                var dirPath = new DirectoryInfo(targetPathList[i]).Parent.FullName;
                //                if (!Directory.Exists(dirPath))
                //                    Directory.CreateDirectory(dirPath);
                //                File.WriteAllText(targetPathList[i], tResult, Encoding.UTF8);
                //            }

                //            tableRelationsList.Add(new CodeGenTableRelationsModel
                //            {
                //                ClassName = item.className,
                //                OriginalTableName = item.table,
                //                RelationTable = item.relationTable,
                //                TableName = item.table.ParseToPascalCase(),
                //                PrimaryKey = codeGenConfigModel.TableField.Find(it => it.PrimaryKey).ColumnName,
                //                TableField = codeGenConfigModel.TableField.Find(it => it.ForeignKeyField).ColumnName,
                //                OriginalTableField = codeGenConfigModel.TableField.Find(it => it.ForeignKeyField).OriginalColumnName,
                //                RelationField = codeGenConfigModel.PrimaryKey.ToLower() == item.relationField.ToLower() ? "id" : item.relationField.ToUpperCase(),
                //                OriginalRelationField = item.relationField,
                //                ControlTableComment = codeGenConfigModel.BusName,
                //                TableComment = item.tableName,
                //                ChilderColumnConfigList = codeGenConfigModel.TableField,
                //                ChilderColumnConfigListCount = codeGenConfigModel.TableField.FindAll(it => !it.PrimaryKey && !it.ForeignKeyField && it.ceriKey != null).Count(),
                //                TableNo = tableNo,
                //                ControlModel = controlId,
                //                IsQueryWhether = codeGenConfigModel.TableField.Any(it => it.QueryWhether),
                //                IsShowField = codeGenConfigModel.TableField.Any(it => it.IsShow),
                //                IsUnique = codeGenConfigModel.TableField.Any(it => it.IsUnique),
                //                IsConversion = codeGenConfigModel.TableField.Any(it => it.IsConversion.Equals(true)),
                //                IsDetailConversion = codeGenConfigModel.TableField.Any(it => it.IsDetailConversion.Equals(true)),
                //                IsImportData = pcColumnDesignModel.btnsList.Any(x => x.value.Equals("upload") && x.show) && codeGenConfigModel.TableField.Any(it => it.IsImportField.Equals(true)),
                //                IsSearchMultiple = codeGenConfigModel.IsSearchMultiple,
                //                IsControlParsing = codeGenConfigModel.TableField.Any(it => it.IsControlParsing),
                //            });

                //            // 还原全部控件
                //            controls = TemplateAnalysis.AnalysisTemplateData(formDataModel.fields);
                //        }

                //        // 将子表第一个创建人、修改人、所属岗位 切到最后面.
                //        foreach (var item in tableRelationsList)
                //        {
                //            var tempColumnList = new List<TableColumnConfigModel>();
                //            foreach (var it in item.ChilderColumnConfigList.Where(x => x.ceriKey.IsNotEmptyOrNull()))
                //            {
                //                if (it.ceriKey != null && (it.ceriKey.Equals(CeriKeyConst.CREATEUSER) || it.ceriKey.Equals(CeriKeyConst.MODIFYUSER) || it.ceriKey.Equals(CeriKeyConst.CURRPOSITION)))
                //                {
                //                    tempColumnList.Add(it);
                //                }
                //                else
                //                {
                //                    break;
                //                }
                //            }

                //            item.ChilderColumnConfigList.RemoveAll(x => tempColumnList.Contains(x));
                //            item.ChilderColumnConfigList.AddRange(tempColumnList);
                //        }

                //        // 生成主表
                //        foreach (DbTableRelationModel? item in tableRelation.FindAll(it => it.typeId == "1"))
                //        {
                //            var fieldList = _databaseManager.GetFieldList(targetLink, item.table);
                //            KingbaseNetType(fieldList, targetLink);
                //            fieldList = CodeGenWay.GetTableFieldModelReName(item.table, fieldList, aliasList, true);
                //            var pField = fieldList.Find(x => x.primaryKey && !x.field.ToLower().Equals("f_tenant_id"));
                //            foreach (var subItem in tableRelationsList)
                //            {
                //                if (subItem.RelationField.ToLower().Equals(pField.field.ToLower()) && pField.reName.IsNotEmptyOrNull()) subItem.RelationField = pField.reName;
                //                var reItem = fieldList.Find(x => x.field.ToLower().Equals(subItem.RelationField.ToLower()) && x.reName.IsNotEmptyOrNull());
                //                if (reItem != null) subItem.RelationField = reItem.reName;
                //            }

                //            if (fieldList.Count == 0) throw Oops.Oh(ErrorCode.D2106);

                //            // 开启乐观锁
                //            if (formDataModel.concurrencyLock && !fieldList.Any(it => it.field.ToLower().Equals("f_version")))
                //                throw Oops.Oh(ErrorCode.D2107);

                //            if (formDataModel.primaryKeyPolicy == 2 && !fieldList.Any(it => it.primaryKey && it.identity))
                //                throw Oops.Oh(ErrorCode.D2109);

                //            if (templateEntity.EnableFlow == 1 && !fieldList.Any(it => it.field.ToLower().Equals("f_flow_id")))
                //                throw Oops.Oh(ErrorCode.D2105);

                //            // 列表带流程 或者 流程表单 自增ID
                //            if (templateEntity.EnableFlow == 1 && !fieldList.Any(it => it.field.ToLower().Equals("f_flow_task_id")))
                //                throw Oops.Oh(ErrorCode.D2108);

                //            if (formDataModel.logicalDelete && (!fieldList.Any(it => it.field.ToLower().Equals("f_delete_mark")) || !fieldList.Any(it => it.field.ToLower().Equals("f_delete_time")) || !fieldList.Any(it => it.field.ToLower().Equals("f_delete_user_id"))))
                //                throw Oops.Oh(ErrorCode.D2110);

                //            // 后端生成
                //            codeGenConfigModel = CodeGenWay.MainBeltBackEnd(item.table, fieldList, controls, templateEntity);
                //            codeGenConfigModel.IsMapper = true;
                //            codeGenConfigModel.BusName = item.tableName;
                //            codeGenConfigModel.TableRelations = tableRelationsList;
                //            codeGenConfigModel.IsChildConversion = tableRelationsList.Any(it => it.IsConversion);
                //            switch (templateEntity.WebType)
                //            {
                //                case 1:
                //                    switch (templateEntity.Type)
                //                    {
                //                        case 3:
                //                            targetPathList = CodeGenTargetPathHelper.BackendFlowTargetPathList(item.className, fileName, codeGenConfigModel.IsMapper);
                //                            templatePathList = CodeGenTargetPathHelper.BackendFlowTemplatePathList("2-MainBelt", codeGenConfigModel.IsMapper);
                //                            break;
                //                        default:
                //                            targetPathList = CodeGenTargetPathHelper.BackendTargetPathList(item.className, fileName, templateEntity.WebType, templateEntity.EnableFlow, codeGenConfigModel.TableType == 4, codeGenConfigModel.IsMapper);
                //                            templatePathList = CodeGenTargetPathHelper.BackendTemplatePathList("2-MainBelt", templateEntity.WebType, templateEntity.EnableFlow, codeGenConfigModel.IsMapper);
                //                            break;
                //                    }
                //                    break;
                //                case 2:
                //                    switch (codeGenConfigModel.TableType)
                //                    {
                //                        case 4:
                //                            switch (templateEntity.Type)
                //                            {
                //                                case 3:
                //                                    break;
                //                                default:
                //                                    targetPathList = CodeGenTargetPathHelper.BackendTargetPathList(item.className, fileName, templateEntity.WebType, templateEntity.EnableFlow, codeGenConfigModel.TableType == 4, codeGenConfigModel.IsMapper);
                //                                    templatePathList = CodeGenTargetPathHelper.BackendInlineEditorTemplatePathList("2-MainBelt", templateEntity.WebType, templateEntity.EnableFlow, codeGenConfigModel.IsMapper);
                //                                    break;
                //                            }

                //                            break;
                //                        default:
                //                            switch (templateEntity.Type)
                //                            {
                //                                case 3:
                //                                    targetPathList = CodeGenTargetPathHelper.BackendFlowTargetPathList(item.className, fileName, codeGenConfigModel.IsMapper);
                //                                    templatePathList = CodeGenTargetPathHelper.BackendFlowTemplatePathList("2-MainBelt", codeGenConfigModel.IsMapper);
                //                                    break;
                //                                default:
                //                                    targetPathList = CodeGenTargetPathHelper.BackendTargetPathList(item.className, fileName, templateEntity.WebType, templateEntity.EnableFlow, codeGenConfigModel.TableType == 4, codeGenConfigModel.IsMapper);
                //                                    templatePathList = CodeGenTargetPathHelper.BackendTemplatePathList("2-MainBelt", templateEntity.WebType, templateEntity.EnableFlow, codeGenConfigModel.IsMapper);
                //                                    break;
                //                            }

                //                            break;
                //                    }
                //                    break;
                //            }

                //            // 生成后端文件
                //            for (int i = 0; i < templatePathList.Count; i++)
                //            {
                //                string tContent = File.ReadAllText(templatePathList[i]);
                //                string tResult = _viewEngine.RunCompileFromCached(tContent, new
                //                {
                //                    Id = templateEntity.Id,
                //                    IsInlineEditor = pcColumnDesignModel.type.Equals(4),
                //                    NameSpace = codeGenConfigModel.NameSpace,
                //                    BusName = codeGenConfigModel.BusName,
                //                    ClassName = codeGenConfigModel.ClassName,
                //                    PrimaryKey = codeGenConfigModel.PrimaryKey,
                //                    LowerPrimaryKey = codeGenConfigModel.LowerPrimaryKey,
                //                    OriginalPrimaryKey = codeGenConfigModel.OriginalPrimaryKey,
                //                    MainTable = codeGenConfigModel.MainTable,
                //                    LowerMainTable = codeGenConfigModel.LowerMainTable,
                //                    OriginalMainTableName = codeGenConfigModel.OriginalMainTableName,
                //                    hasPage = codeGenConfigModel.hasPage && !codeGenConfigModel.TableType.Equals(3),
                //                    Function = codeGenConfigModel.Function,
                //                    TableField = codeGenConfigModel.TableField,
                //                    RelationsField = codeGenConfigModel.RelationsField,
                //                    TableFieldCount = codeGenConfigModel.TableField.FindAll(it => !it.PrimaryKey && it.ceriKey != null).Count(),
                //                    DefaultSidx = codeGenConfigModel.DefaultSidx,
                //                    IsExport = codeGenConfigModel.IsExport,
                //                    IsBatchRemove = codeGenConfigModel.IsBatchRemove,
                //                    IsUploading = codeGenConfigModel.IsUpload,
                //                    IsTableRelations = codeGenConfigModel.IsTableRelations,
                //                    IsMapper = codeGenConfigModel.IsMapper,
                //                    IsSystemControl = codeGenConfigModel.IsSystemControl,
                //                    IsUpdate = codeGenConfigModel.IsUpdate,
                //                    IsBillRule = codeGenConfigModel.IsBillRule,
                //                    DbLinkId = codeGenConfigModel.DbLinkId,
                //                    FormId = codeGenConfigModel.FormId,
                //                    WebType = codeGenConfigModel.WebType,
                //                    Type = codeGenConfigModel.Type,
                //                    EnableFlow = codeGenConfigModel.EnableFlow,
                //                    IsMainTable = codeGenConfigModel.IsMainTable,
                //                    EnCode = codeGenConfigModel.EnCode,
                //                    UseDataPermission = useDataPermission,
                //                    SearchControlNum = codeGenConfigModel.SearchControlNum,
                //                    IsAuxiliaryTable = codeGenConfigModel.IsAuxiliaryTable,
                //                    ExportField = codeGenConfigModel.ExportField,
                //                    TableRelations = codeGenConfigModel.TableRelations,
                //                    ConfigId = _userManager.TenantId,
                //                    DBName = _userManager.TenantDbName,
                //                    PcUseDataPermission = pcColumnDesignModel.useDataPermission ? "true" : "false",
                //                    AppUseDataPermission = appColumnDesignModel.useDataPermission ? "true" : "false",
                //                    FullName = codeGenConfigModel.FullName,
                //                    IsConversion = codeGenConfigModel.IsConversion,
                //                    IsDetailConversion = codeGenConfigModel.IsDetailConversion,
                //                    HasSuperQuery = codeGenConfigModel.HasSuperQuery,
                //                    PrimaryKeyPolicy = codeGenConfigModel.PrimaryKeyPolicy,
                //                    ConcurrencyLock = codeGenConfigModel.ConcurrencyLock,
                //                    IsUnique = codeGenConfigModel.IsUnique || codeGenConfigModel.TableRelations.Any(it => it.IsUnique),
                //                    BusinessKeyList = codeGenConfigModel.BusinessKeyList,
                //                    BusinessKeyTip = formDataModel.businessKeyTip,
                //                    IsChildConversion = codeGenConfigModel.IsChildConversion,
                //                    IsChildIndexShow = codeGenConfigModel.TableRelations.Any(it => it.IsShowField),
                //                    GroupField = codeGenConfigModel.GroupField,
                //                    GroupShowField = codeGenConfigModel.GroupShowField,
                //                    IsImportData = pcColumnDesignModel.btnsList.Any(x => x.value.Equals("upload") && x.show) && codeGenConfigModel.IsImportData,
                //                    ImportColumnField = CodeGenExportFieldHelper.ImportColumnField(templateEntity, codeGenConfigModel, targetLink),
                //                    DefaultDbName = CodeGenExportFieldHelper.GetDefaultDbNameByDbType(defaultLink),
                //                    ParsCeriKeyConstList = codeGenConfigModel.ParsCeriKeyConstList,
                //                    ParsCeriKeyConstListDetails = codeGenConfigModel.ParsCeriKeyConstListDetails,
                //                    ImportDataType = codeGenConfigModel.ImportDataType,
                //                    DataRuleJson = CodeGenControlsAttributeHelper.GetDataRuleList(templateEntity, codeGenConfigModel),
                //                    IsSearchMultiple = codeGenConfigModel.IsSearchMultiple,
                //                    IsTreeTable = codeGenConfigModel.IsTreeTable,
                //                    ParentField = codeGenConfigModel.ParentField,
                //                    TreeShowField = codeGenConfigModel.TreeShowField,
                //                    IsLogicalDelete = codeGenConfigModel.IsLogicalDelete,
                //                    TableType = codeGenConfigModel.TableType,
                //                    IsTenantColumn = _tenant.MultiTenancy && _tenant.MultiTenancyType.Equals("COLUMN"),
                //                    PcKeywordSearchColumn = CodeGenWay.GetCodeGenKeywordSearchColumn(templateEntity, "pc"),
                //                    AppKeywordSearchColumn = CodeGenWay.GetCodeGenKeywordSearchColumn(templateEntity, "app"),
                //                    PcDefaultSortConfig = pcColumnDesignModel.defaultSortConfig != null && pcColumnDesignModel.defaultSortConfig.Any(),
                //                    AppDefaultSortConfig = appColumnDesignModel.defaultSortConfig != null && appColumnDesignModel.defaultSortConfig.Any(),
                //                });
                //                var dirPath = new DirectoryInfo(targetPathList[i]).Parent.FullName;
                //                if (!Directory.Exists(dirPath))
                //                    Directory.CreateDirectory(dirPath);
                //                File.WriteAllText(targetPathList[i], tResult, Encoding.UTF8);
                //            }

                //            controls = TemplateAnalysis.AnalysisTemplateData(formDataModel.fields);

                //            codeGenMainTableConfigModel = codeGenConfigModel;
                //        }
                //    }

                //    break;
                //case GeneratePatterns.MainBeltVice:
                //    {
                //        var link = await _repository.AsSugarClient().Queryable<DbLinkEntity>().FirstAsync(m => m.Id == templateEntity.DbLinkId && m.DeleteMark == null);
                //        var targetLink = link ?? _databaseManager.GetTenantDbLink(_userManager.TenantId, _userManager.TenantDbName);

                //        List<CodeGenTableRelationsModel> tableRelationsList = new List<CodeGenTableRelationsModel>();

                //        // 副表表字段配置
                //        List<TableColumnConfigModel> auxiliaryTableColumnList = new List<TableColumnConfigModel>();

                //        var tableNo = 0;
                //        tableName = tableRelation.Find(it => it.typeId == "1").table;

                //        // 生成副表
                //        foreach (DbTableRelationModel? item in tableRelation.FindAll(it => it.typeId == "0"))
                //        {
                //            tableNo++;
                //            var auxiliaryControls = controls.FindAll(it => it.__config__.tableName == item.table);
                //            var fieldList = _databaseManager.GetFieldList(targetLink, item.table);
                //            KingbaseNetType(fieldList, targetLink);
                //            fieldList = CodeGenWay.GetTableFieldModelReName(item.table, fieldList, aliasList);

                //            // 默认主表开启自增副表也需要开启自增
                //            if (formDataModel.primaryKeyPolicy == 2 && !fieldList.Any(it => it.primaryKey && it.identity))
                //            {
                //                throw Oops.Oh(ErrorCode.D2109);
                //            }

                //            codeGenConfigModel = CodeGenWay.AuxiliaryTableBackEnd(item.table, fieldList, auxiliaryControls, templateEntity, tableNo, 0, item.tableField);
                //            codeGenConfigModel.IsMapper = true;
                //            codeGenConfigModel.BusName = item.tableName;
                //            codeGenConfigModel.ClassName = item.className;

                //            targetPathList = CodeGenTargetPathHelper.BackendAuxiliaryTargetPathList(codeGenConfigModel.ClassName, fileName, templateEntity.WebType, templateEntity.Type, templateEntity.EnableFlow);
                //            templatePathList = CodeGenTargetPathHelper.BackendAuxiliaryTemplatePathList("3-Auxiliary", templateEntity.WebType, templateEntity.Type, templateEntity.EnableFlow);

                //            codeGenConfigModel.TableField.ForEach(items =>
                //            {
                //                items.ClassName = item.className;
                //            });

                //            // 生成副表相关文件
                //            for (int i = 0; i < templatePathList.Count; i++)
                //            {
                //                var tContent = File.ReadAllText(templatePathList[i]);
                //                var tResult = _viewEngine.RunCompileFromCached(tContent, new
                //                {
                //                    Id = templateEntity.Id,
                //                    IsInlineEditor = pcColumnDesignModel.type.Equals(4),
                //                    IsMainTable = false,
                //                    BusName = codeGenConfigModel.BusName,
                //                    ClassName = codeGenConfigModel.ClassName,
                //                    NameSpace = formDataModel.areasName,
                //                    PrimaryKey = codeGenConfigModel.TableField.Find(it => it.PrimaryKey).ColumnName,
                //                    AuxiliaryTable = item.table.ParseToPascalCase(),
                //                    MainTable = tableName.ParseToPascalCase(),
                //                    MainClassName = codeGenConfigModel.ClassName,
                //                    OriginalMainTableName = codeGenConfigModel.OriginalMainTableName,
                //                    TableField = codeGenConfigModel.TableField,
                //                    RelationsField = codeGenConfigModel.RelationsField,
                //                    IsUploading = codeGenConfigModel.IsUpload,
                //                    IsMapper = true,
                //                    WebType = templateEntity.WebType,
                //                    Type = templateEntity.Type,
                //                    PrimaryKeyPolicy = codeGenConfigModel.PrimaryKeyPolicy,
                //                    IsImportData = pcColumnDesignModel.btnsList.Any(x => x.value.Equals("upload") && x.show) && codeGenConfigModel.IsImportData,
                //                    EnableFlow = codeGenConfigModel.EnableFlow,
                //                    IsLogicalDelete = codeGenConfigModel.IsLogicalDelete,
                //                    IsTenantColumn = false,
                //                });
                //                var dirPath = new DirectoryInfo(targetPathList[i]).Parent.FullName;
                //                if (!Directory.Exists(dirPath))
                //                    Directory.CreateDirectory(dirPath);
                //                File.WriteAllText(targetPathList[i], tResult, Encoding.UTF8);
                //            }

                //            var count = controls.Count(x => x.__vModel__.Contains(item.table));
                //            tableRelationsList.Add(new CodeGenTableRelationsModel
                //            {
                //                ClassName = codeGenConfigModel.ClassName,
                //                OriginalTableName = item.table,
                //                RelationTable = item.relationTable,
                //                TableName = item.table.ParseToPascalCase(),
                //                PrimaryKey = codeGenConfigModel.TableField.Find(it => it.PrimaryKey).ColumnName,
                //                TableField = codeGenConfigModel.TableField.Find(it => it.ForeignKeyField).ColumnName,
                //                ChilderColumnConfigList = codeGenConfigModel.TableField,
                //                OriginalTableField = codeGenConfigModel.TableField.Find(it => it.ForeignKeyField).OriginalColumnName,
                //                RelationField = codeGenConfigModel.PrimaryKey.ToLower() == item.relationField.ToLower() ? "id" : item.relationField.ToUpperCase(),
                //                OriginalRelationField = item.relationField,
                //                TableComment = item.tableName,
                //                TableNo = tableNo,
                //                IsConversion = codeGenConfigModel.TableField.Any(it => it.IsConversion.Equals(true)),
                //                IsDetailConversion = codeGenConfigModel.TableField.Any(it => it.IsDetailConversion.Equals(true)),
                //                IsImportData = pcColumnDesignModel.btnsList.Any(x => x.value.Equals("upload") && x.show) && codeGenConfigModel.TableField.Any(it => it.IsImportField.Equals(true)),
                //                IsSystemControl = codeGenConfigModel.TableField.Any(it => it.IsSystemControl),
                //                IsUpdate = codeGenConfigModel.TableField.Any(it => it.IsUpdate),
                //                IsSearchMultiple = codeGenConfigModel.IsSearchMultiple,
                //                IsControlParsing = codeGenConfigModel.TableField.Any(it => it.IsControlParsing),
                //                FieldCount = count,
                //                ForeignKeyFieldDataType = string.Format(".ToObject<{0}>()", codeGenConfigModel.TableField.Find(it => it.ForeignKeyField).NetType.Replace("?", "")),
                //            });

                //            auxiliaryTableColumnList.AddRange(codeGenConfigModel.TableField.FindAll(it => it.ceriKey != null));
                //        }

                //        // 生成主表
                //        foreach (DbTableRelationModel? item in tableRelation.FindAll(it => it.typeId == "1"))
                //        {
                //            var fieldList = _databaseManager.GetFieldList(targetLink, tableName);
                //            KingbaseNetType(fieldList, targetLink);
                //            fieldList = CodeGenWay.GetTableFieldModelReName(item.table, fieldList, aliasList, true);
                //            var pField = fieldList.Find(x => x.primaryKey && !x.field.ToLower().Equals("f_tenant_id"));
                //            foreach (var auxItem in tableRelationsList)
                //            {
                //                if (auxItem.RelationField.ToLower().Equals(pField.field.ToLower()) && pField.reName.IsNotEmptyOrNull()) auxItem.RelationField = pField.reName;
                //                var reItem = fieldList.Find(x => x.field.ToLower().Equals(auxItem.RelationField.ToLower()) && x.reName.IsNotEmptyOrNull());
                //                if (reItem != null) auxItem.RelationField = reItem.reName;
                //            }

                //            if (fieldList.Count == 0) throw Oops.Oh(ErrorCode.D2106);

                //            // 开启乐观锁
                //            if (formDataModel.concurrencyLock && !fieldList.Any(it => it.field.ToLower().Equals("f_version")))
                //                throw Oops.Oh(ErrorCode.D2107);

                //            if (formDataModel.primaryKeyPolicy == 2 && !fieldList.Any(it => it.primaryKey && it.identity))
                //                throw Oops.Oh(ErrorCode.D2109);

                //            if (templateEntity.EnableFlow == 1 && !fieldList.Any(it => it.field.ToLower().Equals("f_flow_id")))
                //                throw Oops.Oh(ErrorCode.D2105);

                //            // 列表带流程 或者 流程表单 自增ID
                //            if (templateEntity.EnableFlow == 1 && !fieldList.Any(it => it.field.ToLower().Equals("f_flow_task_id")))
                //                throw Oops.Oh(ErrorCode.D2108);

                //            if (formDataModel.logicalDelete && (!fieldList.Any(it => it.field.ToLower().Equals("f_delete_mark")) || !fieldList.Any(it => it.field.ToLower().Equals("f_delete_time")) || !fieldList.Any(it => it.field.ToLower().Equals("f_delete_user_id"))))
                //                throw Oops.Oh(ErrorCode.D2110);

                //            // 后端生成
                //            codeGenConfigModel = CodeGenWay.MainBeltViceBackEnd(item.table, fieldList, auxiliaryTableColumnList, controls, templateEntity);
                //            codeGenConfigModel.IsMapper = true;

                //            switch (templateEntity.WebType)
                //            {
                //                case 1:
                //                    switch (templateEntity.Type)
                //                    {
                //                        case 3:
                //                            targetPathList = CodeGenTargetPathHelper.BackendFlowTargetPathList(codeGenConfigModel.ClassName, fileName, codeGenConfigModel.IsMapper);
                //                            templatePathList = CodeGenTargetPathHelper.BackendFlowTemplatePathList("4-MainBeltVice", codeGenConfigModel.IsMapper);
                //                            break;
                //                        default:
                //                            targetPathList = CodeGenTargetPathHelper.BackendTargetPathList(codeGenConfigModel.ClassName, fileName, templateEntity.WebType, templateEntity.EnableFlow, codeGenConfigModel.TableType == 4, codeGenConfigModel.IsMapper);
                //                            templatePathList = CodeGenTargetPathHelper.BackendTemplatePathList("4-MainBeltVice", templateEntity.WebType, templateEntity.EnableFlow, codeGenConfigModel.IsMapper);
                //                            break;
                //                    }
                //                    break;
                //                case 2:
                //                    switch (codeGenConfigModel.TableType)
                //                    {
                //                        case 4:
                //                            switch (templateEntity.Type)
                //                            {
                //                                case 3:
                //                                    break;
                //                                default:
                //                                    targetPathList = CodeGenTargetPathHelper.BackendTargetPathList(codeGenConfigModel.ClassName, fileName, templateEntity.WebType, templateEntity.EnableFlow, codeGenConfigModel.TableType == 4, codeGenConfigModel.IsMapper);
                //                                    templatePathList = CodeGenTargetPathHelper.BackendInlineEditorTemplatePathList("4-MainBeltVice", templateEntity.WebType, templateEntity.EnableFlow, codeGenConfigModel.IsMapper);
                //                                    break;
                //                            }
                //                            break;
                //                        default:
                //                            switch (templateEntity.Type)
                //                            {
                //                                case 3:
                //                                    targetPathList = CodeGenTargetPathHelper.BackendFlowTargetPathList(codeGenConfigModel.ClassName, fileName, codeGenConfigModel.IsMapper);
                //                                    templatePathList = CodeGenTargetPathHelper.BackendFlowTemplatePathList("4-MainBeltVice", codeGenConfigModel.IsMapper);
                //                                    break;
                //                                default:
                //                                    targetPathList = CodeGenTargetPathHelper.BackendTargetPathList(codeGenConfigModel.ClassName, fileName, templateEntity.WebType, templateEntity.EnableFlow, codeGenConfigModel.TableType == 4, codeGenConfigModel.IsMapper);
                //                                    templatePathList = CodeGenTargetPathHelper.BackendTemplatePathList("4-MainBeltVice", templateEntity.WebType, templateEntity.EnableFlow, codeGenConfigModel.IsMapper);
                //                                    break;
                //                            }
                //                            break;
                //                    }
                //                    break;
                //            }

                //            for (var i = 0; i < templatePathList.Count; i++)
                //            {
                //                var tContent = File.ReadAllText(templatePathList[i]);
                //                var tResult = _viewEngine.RunCompileFromCached(tContent, new
                //                {
                //                    Id = templateEntity.Id,
                //                    IsInlineEditor = pcColumnDesignModel.type.Equals(4),
                //                    NameSpace = codeGenConfigModel.NameSpace,
                //                    BusName = codeGenConfigModel.BusName,
                //                    ClassName = codeGenConfigModel.ClassName,
                //                    PrimaryKey = codeGenConfigModel.PrimaryKey,
                //                    LowerPrimaryKey = codeGenConfigModel.LowerPrimaryKey,
                //                    OriginalPrimaryKey = codeGenConfigModel.OriginalPrimaryKey,
                //                    MainTable = codeGenConfigModel.MainTable,
                //                    LowerMainTable = codeGenConfigModel.LowerMainTable,
                //                    OriginalMainTableName = codeGenConfigModel.OriginalMainTableName,
                //                    hasPage = codeGenConfigModel.hasPage && !codeGenConfigModel.TableType.Equals(3),
                //                    Function = codeGenConfigModel.Function,
                //                    TableField = codeGenConfigModel.TableField,
                //                    RelationsField = codeGenConfigModel.RelationsField,
                //                    TableFieldCount = codeGenConfigModel.TableField.FindAll(it => !it.PrimaryKey && it.ceriKey != null).Count(),
                //                    DefaultSidx = codeGenConfigModel.DefaultSidx,
                //                    IsExport = codeGenConfigModel.IsExport,
                //                    IsBatchRemove = codeGenConfigModel.IsBatchRemove,
                //                    IsUploading = codeGenConfigModel.IsUpload,
                //                    IsTableRelations = codeGenConfigModel.IsTableRelations,
                //                    IsMapper = codeGenConfigModel.IsMapper,
                //                    IsBillRule = codeGenConfigModel.IsBillRule,
                //                    DbLinkId = codeGenConfigModel.DbLinkId,
                //                    FormId = codeGenConfigModel.FormId,
                //                    WebType = codeGenConfigModel.WebType,
                //                    Type = codeGenConfigModel.Type,
                //                    EnableFlow = codeGenConfigModel.EnableFlow,
                //                    IsMainTable = codeGenConfigModel.IsMainTable,
                //                    EnCode = codeGenConfigModel.EnCode,
                //                    UseDataPermission = useDataPermission,
                //                    SearchControlNum = codeGenConfigModel.SearchControlNum,
                //                    IsAuxiliaryTable = codeGenConfigModel.IsAuxiliaryTable,
                //                    ExportField = codeGenConfigModel.ExportField,
                //                    ConfigId = _userManager.TenantId,
                //                    DBName = _userManager.TenantDbName,
                //                    PcUseDataPermission = pcColumnDesignModel.useDataPermission ? "true" : "false",
                //                    AppUseDataPermission = appColumnDesignModel.useDataPermission ? "true" : "false",
                //                    AuxiliayTableRelations = tableRelationsList,
                //                    FullName = codeGenConfigModel.FullName,
                //                    IsConversion = codeGenConfigModel.IsConversion,
                //                    IsDetailConversion = codeGenConfigModel.IsDetailConversion,
                //                    IsMainConversion = codeGenConfigModel.TableField.Any(it => it.IsAuxiliary.Equals(false) && it.IsConversion.Equals(true)),
                //                    IsUpdate = codeGenConfigModel.TableField.Any(it => it.IsUpdate.Equals(true) && it.IsAuxiliary.Equals(false) && it.ceriKey != null),
                //                    HasSuperQuery = codeGenConfigModel.HasSuperQuery,
                //                    PrimaryKeyPolicy = codeGenConfigModel.PrimaryKeyPolicy,
                //                    ConcurrencyLock = codeGenConfigModel.ConcurrencyLock,
                //                    IsUnique = codeGenConfigModel.IsUnique,
                //                    BusinessKeyList = codeGenConfigModel.BusinessKeyList,
                //                    BusinessKeyTip = formDataModel.businessKeyTip,
                //                    GroupField = codeGenConfigModel.GroupField,
                //                    GroupShowField = codeGenConfigModel.GroupShowField,
                //                    IsImportData = pcColumnDesignModel.btnsList.Any(x => x.value.Equals("upload") && x.show) && codeGenConfigModel.IsImportData,
                //                    ImportColumnField = CodeGenExportFieldHelper.ImportColumnField(templateEntity, codeGenConfigModel, targetLink),
                //                    DefaultDbName = CodeGenExportFieldHelper.GetDefaultDbNameByDbType(defaultLink),
                //                    ParsCeriKeyConstList = codeGenConfigModel.ParsCeriKeyConstList,
                //                    ParsCeriKeyConstListDetails = codeGenConfigModel.ParsCeriKeyConstListDetails,
                //                    ImportDataType = codeGenConfigModel.ImportDataType,
                //                    DataRuleJson = CodeGenControlsAttributeHelper.GetDataRuleList(templateEntity, codeGenConfigModel),
                //                    IsSearchMultiple = codeGenConfigModel.IsSearchMultiple,
                //                    IsTreeTable = codeGenConfigModel.IsTreeTable,
                //                    ParentField = codeGenConfigModel.ParentField,
                //                    TreeShowField = codeGenConfigModel.TreeShowField,
                //                    IsLogicalDelete = codeGenConfigModel.IsLogicalDelete,
                //                    TableType = codeGenConfigModel.TableType,
                //                    IsTenantColumn = _tenant.MultiTenancy && _tenant.MultiTenancyType.Equals("COLUMN"),
                //                    PcKeywordSearchColumn = CodeGenWay.GetCodeGenKeywordSearchColumn(templateEntity, "pc"),
                //                    AppKeywordSearchColumn = CodeGenWay.GetCodeGenKeywordSearchColumn(templateEntity, "app"),
                //                    PcDefaultSortConfig = pcColumnDesignModel.defaultSortConfig != null && pcColumnDesignModel.defaultSortConfig.Any(),
                //                    AppDefaultSortConfig = appColumnDesignModel.defaultSortConfig != null && appColumnDesignModel.defaultSortConfig.Any(),
                //                });
                //                var dirPath = new DirectoryInfo(targetPathList[i]).Parent.FullName;
                //                if (!Directory.Exists(dirPath))
                //                    Directory.CreateDirectory(dirPath);
                //                File.WriteAllText(targetPathList[i], tResult, Encoding.UTF8);

                //                codeGenMainTableConfigModel = codeGenConfigModel;
                //            }
                //        }
                //    }

                //    break;
                //case GeneratePatterns.PrimarySecondary:
                //    {
                //        var link = await _repository.AsSugarClient().Queryable<DbLinkEntity>().FirstAsync(m => m.Id == templateEntity.DbLinkId && m.DeleteMark == null);
                //        var targetLink = link ?? _databaseManager.GetTenantDbLink(_userManager.TenantId, _userManager.TenantDbName);

                //        // 解析子表
                //        var subTable = new List<DbTableRelationModel>();
                //        var secondaryTable = new List<DbTableRelationModel>();
                //        foreach (DbTableRelationModel? item in tableRelation.FindAll(it => it.typeId == "0"))
                //        {
                //            // 解析子表比副表效率
                //            // 先获取出子表 其他的默认为副表
                //            switch (controls.Any(it => it.__config__.ceriKey.Equals(CeriKeyConst.TABLE) && it.__config__.tableName.Equals(item.table)))
                //            {
                //                case true:
                //                    subTable.Add(item);
                //                    break;
                //                default:
                //                    secondaryTable.Add(item);
                //                    break;
                //            }
                //        }

                //        List<CodeGenTableRelationsModel> subTableRelationsList = new List<CodeGenTableRelationsModel>();
                //        List<CodeGenTableRelationsModel> secondaryTableRelationsList = new List<CodeGenTableRelationsModel>();

                //        int tableNo = 1;

                //        // 已经区分子表与副表
                //        // 生成子表
                //        foreach (DbTableRelationModel? item in subTable)
                //        {
                //            var controlId = string.Empty;
                //            var children = controls.Find(it => it.__config__.ceriKey.Equals(CeriKeyConst.TABLE) && it.__config__.tableName.Equals(item.table));
                //            if (children == null) continue;
                //            controlId = children.__vModel__;
                //            if (children != null) controls = children.__config__.children;

                //            var fieldList = _databaseManager.GetFieldList(targetLink, item.table);
                //            KingbaseNetType(fieldList, targetLink);
                //            fieldList = CodeGenWay.GetTableFieldModelReName(item.table, fieldList, aliasList);
                //            ctPrimaryKey.Add(item.table, fieldList.Find(x => x.primaryKey).field);

                //            if (fieldList.Count == 0) throw Oops.Oh(ErrorCode.D2106);

                //            // 默认主表开启自增子表也需要开启自增
                //            if (formDataModel.primaryKeyPolicy == 2 && !fieldList.Any(it => it.primaryKey && it.identity))
                //                throw Oops.Oh(ErrorCode.D2109);

                //            // 后端生成
                //            codeGenConfigModel = CodeGenWay.ChildTableBackEnd(item.table, item.className, fieldList, controls, templateEntity, controlId, item.tableField);
                //            codeGenConfigModel.IsMapper = true;
                //            codeGenConfigModel.BusName = children.__config__.label;
                //            codeGenConfigModel.ClassName = item.className;

                //            targetPathList = CodeGenTargetPathHelper.BackendChildTableTargetPathList(codeGenConfigModel.ClassName, fileName, templateEntity.WebType, templateEntity.Type, codeGenConfigModel.IsMapper, codeGenConfigModel.IsShowSubTableField);
                //            templatePathList = CodeGenTargetPathHelper.BackendChildTableTemplatePathList("SubTable", templateEntity.WebType, templateEntity.Type, codeGenConfigModel.IsMapper, codeGenConfigModel.IsShowSubTableField);

                //            // 生成子表相关文件
                //            for (int i = 0; i < templatePathList.Count; i++)
                //            {
                //                var tContent = File.ReadAllText(templatePathList[i]);
                //                var tResult = _viewEngine.RunCompileFromCached(tContent, new
                //                {
                //                    Id = templateEntity.Id,
                //                    IsInlineEditor = pcColumnDesignModel.type.Equals(4),
                //                    IsMainTable = false,
                //                    BusName = codeGenConfigModel.BusName,
                //                    ClassName = codeGenConfigModel.ClassName,
                //                    NameSpace = formDataModel.areasName,
                //                    PrimaryKey = codeGenConfigModel.TableField.Find(it => it.PrimaryKey).ColumnName,
                //                    MainClassName = codeGenConfigModel.ClassName,
                //                    OriginalMainTableName = item.table,
                //                    TableField = codeGenConfigModel.TableField,
                //                    RelationsField = codeGenConfigModel.RelationsField,
                //                    IsUploading = codeGenConfigModel.IsUpload,
                //                    IsMapper = codeGenConfigModel.IsMapper,
                //                    WebType = templateEntity.WebType,
                //                    Type = templateEntity.Type,
                //                    PrimaryKeyPolicy = codeGenConfigModel.PrimaryKeyPolicy,
                //                    IsImportData = pcColumnDesignModel.btnsList.Any(x => x.value.Equals("upload") && x.show) && codeGenConfigModel.IsImportData,
                //                    EnableFlow = templateEntity.EnableFlow == 0 ? false : true,
                //                    IsLogicalDelete = codeGenConfigModel.IsLogicalDelete,
                //                    TableType = codeGenConfigModel.TableType,
                //                    IsTenantColumn = false,
                //                });
                //                var dirPath = new DirectoryInfo(targetPathList[i]).Parent.FullName;
                //                if (!Directory.Exists(dirPath))
                //                    Directory.CreateDirectory(dirPath);
                //                File.WriteAllText(targetPathList[i], tResult, Encoding.UTF8);
                //            }

                //            subTableRelationsList.Add(new CodeGenTableRelationsModel
                //            {
                //                ClassName = item.className,
                //                OriginalTableName = item.table,
                //                RelationTable = item.relationTable,
                //                TableName = item.table.ParseToPascalCase(),
                //                PrimaryKey = codeGenConfigModel.TableField.Find(it => it.PrimaryKey).ColumnName,
                //                TableField = codeGenConfigModel.TableField.Find(it => it.ForeignKeyField).ColumnName,
                //                OriginalTableField = codeGenConfigModel.TableField.Find(it => it.ForeignKeyField).OriginalColumnName,
                //                RelationField = codeGenConfigModel.PrimaryKey.ToLower() == item.relationField.ToLower() ? "id" : item.relationField.ToUpperCase(),
                //                OriginalRelationField = item.relationField,
                //                ControlTableComment = codeGenConfigModel.BusName,
                //                TableComment = item.tableName,
                //                ChilderColumnConfigList = codeGenConfigModel.TableField,
                //                ChilderColumnConfigListCount = codeGenConfigModel.TableField.FindAll(it => !it.PrimaryKey && !it.ForeignKeyField && it.ceriKey != null).Count(),
                //                TableNo = tableNo,
                //                ControlModel = controlId,
                //                IsQueryWhether = codeGenConfigModel.TableField.Any(it => it.QueryWhether),
                //                IsShowField = codeGenConfigModel.TableField.Any(it => it.IsShow),
                //                IsUnique = codeGenConfigModel.TableField.Any(it => it.IsUnique),
                //                IsConversion = codeGenConfigModel.TableField.Any(it => it.IsConversion.Equals(true)),
                //                IsDetailConversion = codeGenConfigModel.TableField.Any(it => it.IsDetailConversion.Equals(true)),
                //                IsImportData = pcColumnDesignModel.btnsList.Any(x => x.value.Equals("upload") && x.show) && codeGenConfigModel.TableField.Any(it => it.IsImportField.Equals(true)),
                //                IsSearchMultiple = codeGenConfigModel.IsSearchMultiple,
                //                IsControlParsing = codeGenConfigModel.TableField.Any(it => it.IsControlParsing),
                //            });
                //            tableNo++;

                //            // 还原全部控件
                //            controls = TemplateAnalysis.AnalysisTemplateData(formDataModel.fields);
                //        }

                //        // 副表表字段配置
                //        List<TableColumnConfigModel> auxiliaryTableColumnList = new List<TableColumnConfigModel>();

                //        // 生成副表
                //        foreach (DbTableRelationModel? item in secondaryTable)
                //        {
                //            var auxiliaryControls = controls.FindAll(it => it.__config__.tableName == item.table);
                //            var fieldList = _databaseManager.GetFieldList(targetLink, item.table);
                //            KingbaseNetType(fieldList, targetLink);
                //            fieldList = CodeGenWay.GetTableFieldModelReName(item.table, fieldList, aliasList);

                //            // 默认主表开启自增副表也需要开启自增
                //            if (formDataModel.primaryKeyPolicy == 2 && !fieldList.Any(it => it.primaryKey && it.identity))
                //                throw Oops.Oh(ErrorCode.D2109);

                //            codeGenConfigModel = CodeGenWay.AuxiliaryTableBackEnd(item.table, fieldList, auxiliaryControls, templateEntity, tableNo, 1, item.tableField);
                //            codeGenConfigModel.IsMapper = true;
                //            codeGenConfigModel.BusName = item.tableName;
                //            codeGenConfigModel.ClassName = item.className;

                //            targetPathList = CodeGenTargetPathHelper.BackendAuxiliaryTargetPathList(codeGenConfigModel.ClassName, fileName, templateEntity.WebType, templateEntity.Type, templateEntity.EnableFlow);
                //            templatePathList = CodeGenTargetPathHelper.BackendAuxiliaryTemplatePathList("3-Auxiliary", templateEntity.WebType, templateEntity.Type, templateEntity.EnableFlow);

                //            codeGenConfigModel.TableField.ForEach(items => items.ClassName = item.className);

                //            // 生成副表相关文件
                //            for (int i = 0; i < templatePathList.Count; i++)
                //            {
                //                var tContent = File.ReadAllText(templatePathList[i]);
                //                var tResult = _viewEngine.RunCompileFromCached(tContent, new
                //                {
                //                    Id = templateEntity.Id,
                //                    IsInlineEditor = pcColumnDesignModel.type.Equals(4),
                //                    IsMainTable = false,
                //                    BusName = codeGenConfigModel.BusName,
                //                    ClassName = codeGenConfigModel.ClassName,
                //                    NameSpace = formDataModel.areasName,
                //                    PrimaryKey = codeGenConfigModel.TableField.Find(it => it.PrimaryKey).ColumnName,
                //                    AuxiliaryTable = item.table.ParseToPascalCase(),
                //                    MainTable = tableName.ParseToPascalCase(),
                //                    MainClassName = codeGenConfigModel.ClassName,
                //                    OriginalMainTableName = codeGenConfigModel.OriginalMainTableName,
                //                    TableField = codeGenConfigModel.TableField,
                //                    RelationsField = codeGenConfigModel.RelationsField,
                //                    IsUploading = codeGenConfigModel.IsUpload,
                //                    IsMapper = true,
                //                    WebType = templateEntity.WebType,
                //                    Type = templateEntity.Type,
                //                    PrimaryKeyPolicy = codeGenConfigModel.PrimaryKeyPolicy,
                //                    IsImportData = pcColumnDesignModel.btnsList.Any(x => x.value.Equals("upload") && x.show) && codeGenConfigModel.IsImportData,
                //                    EnableFlow = codeGenConfigModel.EnableFlow,
                //                    IsLogicalDelete = codeGenConfigModel.IsLogicalDelete,
                //                    IsTenantColumn = false,
                //                });
                //                var dirPath = new DirectoryInfo(targetPathList[i]).Parent.FullName;
                //                if (!Directory.Exists(dirPath))
                //                    Directory.CreateDirectory(dirPath);
                //                File.WriteAllText(targetPathList[i], tResult, Encoding.UTF8);
                //            }

                //            var count = controls.Count(x => x.__vModel__.Contains(item.table));
                //            secondaryTableRelationsList.Add(new CodeGenTableRelationsModel
                //            {
                //                ClassName = codeGenConfigModel.ClassName,
                //                OriginalTableName = item.table,
                //                RelationTable = item.relationTable,
                //                TableName = item.table.ParseToPascalCase(),
                //                PrimaryKey = codeGenConfigModel.TableField.Find(it => it.PrimaryKey).ColumnName,
                //                TableField = codeGenConfigModel.TableField.Find(it => it.ForeignKeyField).ColumnName,
                //                ChilderColumnConfigList = codeGenConfigModel.TableField,
                //                OriginalTableField = codeGenConfigModel.TableField.Find(it => it.ForeignKeyField).OriginalColumnName,
                //                RelationField = codeGenConfigModel.PrimaryKey.ToLower() == item.relationField.ToLower() ? "id" : item.relationField.ToUpperCase(),
                //                OriginalRelationField = item.relationField,
                //                TableComment = item.tableName,
                //                TableNo = tableNo,
                //                IsConversion = codeGenConfigModel.TableField.Any(it => it.IsConversion.Equals(true)),
                //                IsDetailConversion = codeGenConfigModel.TableField.Any(it => it.IsDetailConversion.Equals(true)),
                //                IsImportData = pcColumnDesignModel.btnsList.Any(x => x.value.Equals("upload") && x.show) && codeGenConfigModel.TableField.Any(it => it.IsImportField.Equals(true)),
                //                IsSystemControl = codeGenConfigModel.TableField.Any(it => it.IsSystemControl),
                //                IsUpdate = codeGenConfigModel.TableField.Any(it => it.IsUpdate),
                //                IsSearchMultiple = codeGenConfigModel.IsSearchMultiple,
                //                IsControlParsing = codeGenConfigModel.TableField.Any(it => it.IsControlParsing),
                //                FieldCount = count,
                //                ForeignKeyFieldDataType = string.Format(".ToObject<{0}>()", codeGenConfigModel.TableField.Find(it => it.ForeignKeyField).NetType.Replace("?", "")),
                //            });

                //            auxiliaryTableColumnList.AddRange(codeGenConfigModel.TableField.FindAll(it => it.ceriKey != null));
                //        }

                //        // 将子表第一个创建人、修改人、所属岗位 切到最后面.
                //        foreach (var item in subTableRelationsList)
                //        {
                //            var tempColumnList = new List<TableColumnConfigModel>();
                //            foreach (var it in item.ChilderColumnConfigList.Where(x => x.ceriKey.IsNotEmptyOrNull()))
                //            {
                //                if (it.ceriKey != null && (it.ceriKey.Equals(CeriKeyConst.CREATEUSER) || it.ceriKey.Equals(CeriKeyConst.MODIFYUSER) || it.ceriKey.Equals(CeriKeyConst.CURRPOSITION)))
                //                {
                //                    tempColumnList.Add(it);
                //                }
                //                else
                //                {
                //                    break;
                //                }
                //            }

                //            item.ChilderColumnConfigList.RemoveAll(x => tempColumnList.Contains(x));
                //            item.ChilderColumnConfigList.AddRange(tempColumnList);
                //        }

                //        // 解析主表
                //        foreach (DbTableRelationModel? item in tableRelation.FindAll(it => it.typeId == "1"))
                //        {
                //            var fieldList = _databaseManager.GetFieldList(targetLink, item.table);
                //            KingbaseNetType(fieldList, targetLink);
                //            fieldList = CodeGenWay.GetTableFieldModelReName(item.table, fieldList, aliasList, true);
                //            var pField = fieldList.Find(x => x.primaryKey && !x.field.ToLower().Equals("f_tenant_id"));
                //            foreach (var subItem in subTableRelationsList)
                //            {
                //                if (subItem.RelationField.ToLower().Equals(pField.field.ToLower()) && pField.reName.IsNotEmptyOrNull()) subItem.RelationField = pField.reName;
                //                var reItem = fieldList.Find(x => x.field.ToLower().Equals(subItem.RelationField.ToLower()) && x.reName.IsNotEmptyOrNull());
                //                if (reItem != null) subItem.RelationField = reItem.reName;
                //            }

                //            foreach (var auxItem in secondaryTableRelationsList)
                //            {
                //                if (auxItem.RelationField.ToLower().Equals(pField.field.ToLower()) && pField.reName.IsNotEmptyOrNull()) auxItem.RelationField = pField.reName;
                //                var reItem = fieldList.Find(x => x.field.ToLower().Equals(auxItem.RelationField.ToLower()) && x.reName.IsNotEmptyOrNull());
                //                if (reItem != null) auxItem.RelationField = reItem.reName;
                //            }

                //            if (fieldList.Count == 0) throw Oops.Oh(ErrorCode.D2106);

                //            // 开启乐观锁
                //            if (formDataModel.concurrencyLock && !fieldList.Any(it => it.field.ToLower().Equals("f_version")))
                //                throw Oops.Oh(ErrorCode.D2107);

                //            if (formDataModel.primaryKeyPolicy == 2 && !fieldList.Any(it => it.primaryKey && it.identity))
                //                throw Oops.Oh(ErrorCode.D2109);

                //            if (templateEntity.EnableFlow == 1 && !fieldList.Any(it => it.field.ToLower().Equals("f_flow_id")))
                //                throw Oops.Oh(ErrorCode.D2105);

                //            // 列表带流程 或者 流程表单 自增ID
                //            if (templateEntity.EnableFlow == 1 && !fieldList.Any(it => it.field.ToLower().Equals("f_flow_task_id")))
                //                throw Oops.Oh(ErrorCode.D2108);

                //            if (formDataModel.logicalDelete && (!fieldList.Any(it => it.field.ToLower().Equals("f_delete_mark")) || !fieldList.Any(it => it.field.ToLower().Equals("f_delete_time")) || !fieldList.Any(it => it.field.ToLower().Equals("f_delete_user_id"))))
                //                throw Oops.Oh(ErrorCode.D2110);

                //            // 后端生成
                //            codeGenConfigModel = CodeGenWay.PrimarySecondaryBackEnd(item.table, fieldList, auxiliaryTableColumnList, controls, templateEntity);

                //            codeGenConfigModel.IsMapper = true;
                //            codeGenConfigModel.BusName = tableRelation.Find(it => it.typeId.Equals("1")).tableName;
                //            codeGenConfigModel.TableRelations = subTableRelationsList;
                //            codeGenConfigModel.IsChildConversion = subTableRelationsList.Any(it => it.IsConversion);

                //            switch (templateEntity.WebType)
                //            {
                //                case 1:
                //                    switch (templateEntity.Type)
                //                    {
                //                        case 3:
                //                            targetPathList = CodeGenTargetPathHelper.BackendFlowTargetPathList(item.className, fileName, codeGenConfigModel.IsMapper);
                //                            templatePathList = CodeGenTargetPathHelper.BackendFlowTemplatePathList("5-PrimarySecondary", codeGenConfigModel.IsMapper);
                //                            break;
                //                        default:
                //                            targetPathList = CodeGenTargetPathHelper.BackendTargetPathList(item.className, fileName, templateEntity.WebType, templateEntity.EnableFlow, codeGenConfigModel.TableType == 4, codeGenConfigModel.IsMapper);
                //                            templatePathList = CodeGenTargetPathHelper.BackendTemplatePathList("5-PrimarySecondary", templateEntity.WebType, templateEntity.EnableFlow, codeGenConfigModel.IsMapper);
                //                            break;
                //                    }
                //                    break;
                //                case 2:
                //                    switch (codeGenConfigModel.TableType)
                //                    {
                //                        case 4:
                //                            switch (templateEntity.Type)
                //                            {
                //                                case 3:
                //                                    break;
                //                                default:
                //                                    targetPathList = CodeGenTargetPathHelper.BackendTargetPathList(item.className, fileName, templateEntity.WebType, templateEntity.EnableFlow, codeGenConfigModel.TableType == 4, codeGenConfigModel.IsMapper);
                //                                    templatePathList = CodeGenTargetPathHelper.BackendInlineEditorTemplatePathList("5-PrimarySecondary", templateEntity.WebType, templateEntity.EnableFlow, codeGenConfigModel.IsMapper);
                //                                    break;
                //                            }

                //                            break;
                //                        default:
                //                            switch (templateEntity.Type)
                //                            {
                //                                case 3:
                //                                    targetPathList = CodeGenTargetPathHelper.BackendFlowTargetPathList(item.className, fileName, codeGenConfigModel.IsMapper);
                //                                    templatePathList = CodeGenTargetPathHelper.BackendFlowTemplatePathList("5-PrimarySecondary", codeGenConfigModel.IsMapper);
                //                                    break;
                //                                default:
                //                                    targetPathList = CodeGenTargetPathHelper.BackendTargetPathList(item.className, fileName, templateEntity.WebType, templateEntity.EnableFlow, codeGenConfigModel.TableType == 4, codeGenConfigModel.IsMapper);
                //                                    templatePathList = CodeGenTargetPathHelper.BackendTemplatePathList("5-PrimarySecondary", templateEntity.WebType, templateEntity.EnableFlow, codeGenConfigModel.IsMapper);
                //                                    break;
                //                            }

                //                            break;
                //                    }
                //                    break;
                //            }

                //            // 生成后端文件
                //            for (int i = 0; i < templatePathList.Count; i++)
                //            {
                //                string tContent = File.ReadAllText(templatePathList[i]);
                //                string tResult = _viewEngine.RunCompileFromCached(tContent, new
                //                {
                //                    Id = templateEntity.Id,
                //                    IsInlineEditor = pcColumnDesignModel.type.Equals(4),
                //                    NameSpace = codeGenConfigModel.NameSpace,
                //                    BusName = codeGenConfigModel.BusName,
                //                    ClassName = codeGenConfigModel.ClassName,
                //                    PrimaryKey = codeGenConfigModel.PrimaryKey,
                //                    LowerPrimaryKey = codeGenConfigModel.LowerPrimaryKey,
                //                    OriginalPrimaryKey = codeGenConfigModel.OriginalPrimaryKey,
                //                    MainTable = codeGenConfigModel.MainTable,
                //                    LowerMainTable = codeGenConfigModel.LowerMainTable,
                //                    OriginalMainTableName = codeGenConfigModel.OriginalMainTableName,
                //                    hasPage = codeGenConfigModel.hasPage && !codeGenConfigModel.TableType.Equals(3),
                //                    Function = codeGenConfigModel.Function,
                //                    TableField = codeGenConfigModel.TableField,
                //                    RelationsField = codeGenConfigModel.RelationsField,
                //                    TableFieldCount = codeGenConfigModel.TableField.FindAll(it => !it.PrimaryKey && it.ceriKey != null).Count(),
                //                    DefaultSidx = codeGenConfigModel.DefaultSidx,
                //                    IsExport = codeGenConfigModel.IsExport,
                //                    IsBatchRemove = codeGenConfigModel.IsBatchRemove,
                //                    IsUploading = codeGenConfigModel.IsUpload,
                //                    IsTableRelations = codeGenConfigModel.IsTableRelations,
                //                    IsMapper = codeGenConfigModel.IsMapper,
                //                    IsBillRule = codeGenConfigModel.IsBillRule,
                //                    DbLinkId = codeGenConfigModel.DbLinkId,
                //                    FormId = codeGenConfigModel.FormId,
                //                    WebType = codeGenConfigModel.WebType,
                //                    Type = codeGenConfigModel.Type,
                //                    EnableFlow = codeGenConfigModel.EnableFlow,
                //                    IsMainTable = codeGenConfigModel.IsMainTable,
                //                    EnCode = codeGenConfigModel.EnCode,
                //                    UseDataPermission = useDataPermission,
                //                    SearchControlNum = codeGenConfigModel.SearchControlNum,
                //                    IsAuxiliaryTable = codeGenConfigModel.IsAuxiliaryTable,
                //                    ExportField = codeGenConfigModel.ExportField,
                //                    TableRelations = codeGenConfigModel.TableRelations,
                //                    ConfigId = _userManager.TenantId,
                //                    DBName = _userManager.TenantDbName,
                //                    PcUseDataPermission = pcColumnDesignModel.useDataPermission ? "true" : "false",
                //                    AppUseDataPermission = appColumnDesignModel.useDataPermission ? "true" : "false",
                //                    AuxiliayTableRelations = secondaryTableRelationsList,
                //                    FullName = codeGenConfigModel.FullName,
                //                    IsConversion = codeGenConfigModel.IsConversion,
                //                    IsDetailConversion = codeGenConfigModel.IsDetailConversion,
                //                    HasSuperQuery = codeGenConfigModel.HasSuperQuery,
                //                    PrimaryKeyPolicy = codeGenConfigModel.PrimaryKeyPolicy,
                //                    ConcurrencyLock = codeGenConfigModel.ConcurrencyLock,
                //                    IsUpdate = codeGenConfigModel.TableField.Any(it => it.IsUpdate.Equals(true) && it.IsAuxiliary.Equals(false) && it.ceriKey != null),
                //                    IsUnique = codeGenConfigModel.IsUnique || codeGenConfigModel.TableRelations.Any(it => it.IsUnique),
                //                    BusinessKeyList = codeGenConfigModel.BusinessKeyList,
                //                    BusinessKeyTip = formDataModel.businessKeyTip,
                //                    IsChildConversion = codeGenConfigModel.IsChildConversion,
                //                    IsChildIndexShow = codeGenConfigModel.TableRelations.Any(it => it.IsShowField),
                //                    GroupField = codeGenConfigModel.GroupField,
                //                    GroupShowField = codeGenConfigModel.GroupShowField,
                //                    IsImportData = pcColumnDesignModel.btnsList.Any(x => x.value.Equals("upload") && x.show) && codeGenConfigModel.IsImportData,
                //                    ImportColumnField = CodeGenExportFieldHelper.ImportColumnField(templateEntity, codeGenConfigModel, targetLink),
                //                    DefaultDbName = CodeGenExportFieldHelper.GetDefaultDbNameByDbType(defaultLink),
                //                    ParsCeriKeyConstList = codeGenConfigModel.ParsCeriKeyConstList,
                //                    ParsCeriKeyConstListDetails = codeGenConfigModel.ParsCeriKeyConstListDetails,
                //                    ImportDataType = codeGenConfigModel.ImportDataType,
                //                    DataRuleJson = CodeGenControlsAttributeHelper.GetDataRuleList(templateEntity, codeGenConfigModel),
                //                    IsSearchMultiple = codeGenConfigModel.IsSearchMultiple,
                //                    IsTreeTable = codeGenConfigModel.IsTreeTable,
                //                    ParentField = codeGenConfigModel.ParentField,
                //                    TreeShowField = codeGenConfigModel.TreeShowField,
                //                    IsLogicalDelete = codeGenConfigModel.IsLogicalDelete,
                //                    TableType = codeGenConfigModel.TableType,
                //                    IsTenantColumn = _tenant.MultiTenancy && _tenant.MultiTenancyType.Equals("COLUMN"),
                //                    PcKeywordSearchColumn = CodeGenWay.GetCodeGenKeywordSearchColumn(templateEntity, "pc"),
                //                    AppKeywordSearchColumn = CodeGenWay.GetCodeGenKeywordSearchColumn(templateEntity, "app"),
                //                    PcDefaultSortConfig = pcColumnDesignModel.defaultSortConfig != null && pcColumnDesignModel.defaultSortConfig.Any(),
                //                    AppDefaultSortConfig = appColumnDesignModel.defaultSortConfig != null && appColumnDesignModel.defaultSortConfig.Any(),
                //                });
                //                var dirPath = new DirectoryInfo(targetPathList[i]).Parent.FullName;
                //                if (!Directory.Exists(dirPath))
                //                    Directory.CreateDirectory(dirPath);
                //                File.WriteAllText(targetPathList[i], tResult, Encoding.UTF8);

                //                codeGenMainTableConfigModel = codeGenConfigModel;
                //            }
                //        }
                //    }

                //    break;
                default:
                    {
                        tableName = tableRelation.FirstOrDefault().table;
                        //var link = await _repository.AsSugarClient().Queryable<DbLinkEntity>().FirstAsync(m => m.Id == templateEntity.DbLinkId && m.DeleteMark == null);
                        var targetLink = defaultLink;
                        // 获取表结构
                        var fieldsJson = await _FormDbService.GetFormDbByNameAsync(tableName);
                        var fieldList = JsonConvert.DeserializeObject<List<ColumnInfo>>(fieldsJson.Data.Fields);
                        KingbaseNetType(fieldList, targetLink.Data);
                        fieldList = CodeGenWay.GetTableFieldModelReName(tableName, fieldList, aliasList, true);

                        if (fieldList.Count == 0) throw new Exception("模板内表不存在");

                        // 开启乐观锁
                        if (formDataModel.concurrencyLock && !fieldList.Any(it => it.Name.ToLower().Equals("f_version")))
                            throw new Exception("表缺失乐观锁字段：f_version");

                        //if (formDataModel.primaryKeyPolicy == 2 && !fieldList.Any(it => it.IsPrimaryKey && it.identity))
                        //    throw Oops.Oh(ErrorCode.D2109);

                        if (templateEntity.EnableFlow == 1 && !fieldList.Any(it => it.Name.ToLower().Equals("f_flow_id")))
                            throw new Exception("表缺失流程Id字段：f_flow_id");

                        // 列表带流程 或者 流程表单 自增ID
                        if (templateEntity.EnableFlow == 1 && !fieldList.Any(it => it.Name.ToLower().Equals("f_flow_task_id")))
                            throw new Exception("表缺失流程真实ID字段：f_flow_task_id");

                        if (formDataModel.logicalDelete && (!fieldList.Any(it => it.Name.ToLower().Equals("f_delete_mark")) || !fieldList.Any(it => it.Name.ToLower().Equals("f_delete_time")) || !fieldList.Any(it => it.Name.ToLower().Equals("f_delete_user_id"))))
                            throw new Exception("表缺失逻辑删除字段：f_delete_mark, f_delete_time, f_delete_user_id");

                        // 后端生成
                        codeGenConfigModel = CodeGenWay.SingleTableBackEnd(tableName, fieldList, controls, templateEntity);
                        codeGenConfigModel.IsMapper = true;

                        switch (templateEntity.WebType)
                        {
                            case 1:
                                switch (templateEntity.Type)
                                {
                                    case 3:
                                        targetPathList = CodeGenTargetPathHelper.BackendFlowTargetPathList(codeGenConfigModel.ClassName, fileName, codeGenConfigModel.IsMapper);
                                        templatePathList = CodeGenTargetPathHelper.BackendFlowTemplatePathList("1-SingleTable", codeGenConfigModel.IsMapper);
                                        break;
                                    default:
                                        targetPathList = CodeGenTargetPathHelper.BackendTargetPathList(codeGenConfigModel.ClassName, fileName, templateEntity.WebType, templateEntity.EnableFlow, codeGenConfigModel.TableType == 4, codeGenConfigModel.IsMapper);
                                        templatePathList = CodeGenTargetPathHelper.BackendTemplatePathList("1-SingleTable", templateEntity.WebType, templateEntity.EnableFlow, codeGenConfigModel.IsMapper);
                                        break;
                                }

                                break;
                            case 2:
                                switch (codeGenConfigModel.TableType)
                                {
                                    case 4:
                                        switch (templateEntity.Type)
                                        {
                                            // 流程表单没有行内编辑
                                            case 3:
                                                break;
                                            default:
                                                targetPathList = CodeGenTargetPathHelper.BackendTargetPathList(codeGenConfigModel.ClassName, fileName, templateEntity.WebType, templateEntity.EnableFlow, codeGenConfigModel.TableType == 4, codeGenConfigModel.IsMapper);
                                                templatePathList = CodeGenTargetPathHelper.BackendInlineEditorTemplatePathList("1-SingleTable", templateEntity.WebType, templateEntity.EnableFlow, codeGenConfigModel.IsMapper);
                                                break;
                                        }
                                        break;
                                    default:
                                        switch (templateEntity.Type)
                                        {
                                            case 3:
                                                targetPathList = CodeGenTargetPathHelper.BackendFlowTargetPathList(codeGenConfigModel.ClassName, fileName, codeGenConfigModel.IsMapper);
                                                templatePathList = CodeGenTargetPathHelper.BackendFlowTemplatePathList("1-SingleTable", codeGenConfigModel.IsMapper);
                                                break;
                                            default:
                                                targetPathList = CodeGenTargetPathHelper.BackendTargetPathList(codeGenConfigModel.ClassName, fileName, templateEntity.WebType, templateEntity.EnableFlow, codeGenConfigModel.TableType == 4, codeGenConfigModel.IsMapper);
                                                templatePathList = CodeGenTargetPathHelper.BackendTemplatePathList("1-SingleTable", templateEntity.WebType, templateEntity.EnableFlow, codeGenConfigModel.IsMapper);
                                                break;
                                        }
                                        break;
                                }
                                break;
                        }
                        try
                        {
                            // 生成后端文件
                            for (var i = 0; i < templatePathList.Count; i++)
                            {
                                var tContent = System.IO.File.ReadAllText(templatePathList[i]);
                                var tResult = _viewEngine.RunCompileFromCached(tContent, new
                                {
                                    Id = templateEntity.Id,
                                    IsInlineEditor = pcColumnDesignModel.type.Equals(4),
                                    IsMainTable = true,
                                    NameSpace = codeGenConfigModel.NameSpace,
                                    BusName = codeGenConfigModel.BusName,
                                    ClassName = codeGenConfigModel.ClassName,
                                    PrimaryKey = codeGenConfigModel.PrimaryKey,
                                    LowerPrimaryKey = codeGenConfigModel.LowerPrimaryKey,
                                    OriginalPrimaryKey = codeGenConfigModel.OriginalPrimaryKey,
                                    MainTable = codeGenConfigModel.MainTable,
                                    LowerMainTable = codeGenConfigModel.LowerMainTable,
                                    OriginalMainTableName = codeGenConfigModel.OriginalMainTableName,
                                    hasPage = codeGenConfigModel.hasPage && !codeGenConfigModel.TableType.Equals(3),
                                    Function = codeGenConfigModel.Function,
                                    TableField = codeGenConfigModel.TableField,
                                    RelationsField = codeGenConfigModel.RelationsField,
                                    TableFieldCount = codeGenConfigModel.TableField.FindAll(it => !it.PrimaryKey && it.ceriKey != null).Count(),
                                    DefaultSidx = codeGenConfigModel.DefaultSidx,
                                    IsExport = codeGenConfigModel.IsExport,
                                    IsBatchRemove = codeGenConfigModel.IsBatchRemove,
                                    IsUploading = codeGenConfigModel.IsUpload,
                                    IsTableRelations = codeGenConfigModel.IsTableRelations,
                                    IsMapper = codeGenConfigModel.IsMapper,
                                    IsSystemControl = codeGenConfigModel.IsSystemControl,
                                    IsUpdate = codeGenConfigModel.IsUpdate,
                                    IsBillRule = codeGenConfigModel.IsBillRule,
                                    DbLinkId = codeGenConfigModel.DbLinkId,
                                    FormId = codeGenConfigModel.FormId,
                                    WebType = codeGenConfigModel.WebType,
                                    Type = codeGenConfigModel.Type,
                                    EnableFlow = codeGenConfigModel.EnableFlow,
                                    EnCode = codeGenConfigModel.EnCode,
                                    UseDataPermission = useDataPermission,
                                    SearchControlNum = codeGenConfigModel.SearchControlNum,
                                    ExportField = codeGenConfigModel.ExportField,
                                    //ConfigId = _userManager.TenantId,
                                    //DBName = _userManager.TenantDbName,
                                    PcUseDataPermission = pcColumnDesignModel.useDataPermission ? "true" : "false",
                                    AppUseDataPermission = appColumnDesignModel.useDataPermission ? "true" : "false",
                                    FullName = codeGenConfigModel.FullName,
                                    IsConversion = codeGenConfigModel.IsConversion,
                                    IsDetailConversion = codeGenConfigModel.IsDetailConversion,
                                    HasSuperQuery = codeGenConfigModel.HasSuperQuery,
                                    PrimaryKeyPolicy = codeGenConfigModel.PrimaryKeyPolicy,
                                    ConcurrencyLock = codeGenConfigModel.ConcurrencyLock,
                                    IsUnique = codeGenConfigModel.IsUnique,
                                    BusinessKeyList = codeGenConfigModel.BusinessKeyList,
                                    BusinessKeyTip = formDataModel.businessKeyTip,
                                    GroupField = codeGenConfigModel.GroupField,
                                    GroupShowField = codeGenConfigModel.GroupShowField,
                                    IsImportData = pcColumnDesignModel.btnsList.Any(x => x.value.Equals("upload") && x.show) && codeGenConfigModel.IsImportData,
                                    ImportColumnField = CodeGenExportFieldHelper.ImportColumnField(templateEntity, codeGenConfigModel, targetLink.Data),
                                    DefaultDbName = CodeGenExportFieldHelper.GetDefaultDbNameByDbType(defaultLink.Data),
                                    ParsCeriKeyConstList = codeGenConfigModel.ParsCeriKeyConstList,
                                    ParsCeriKeyConstListDetails = codeGenConfigModel.ParsCeriKeyConstListDetails,
                                    ImportDataType = codeGenConfigModel.ImportDataType,
                                    DataRuleJson = CodeGenControlsAttributeHelper.GetDataRuleList(formDbDto, codeGenConfigModel),
                                    IsSearchMultiple = codeGenConfigModel.IsSearchMultiple,
                                    IsTreeTable = codeGenConfigModel.IsTreeTable,
                                    ParentField = codeGenConfigModel.ParentField,
                                    TreeShowField = codeGenConfigModel.TreeShowField,
                                    IsLogicalDelete = codeGenConfigModel.IsLogicalDelete,
                                    TableType = codeGenConfigModel.TableType,
                                    //IsTenantColumn = _tenant.MultiTenancy && _tenant.MultiTenancyType.Equals("COLUMN"),
                                    PcKeywordSearchColumn = CodeGenWay.GetCodeGenKeywordSearchColumn(formDbDto, "pc"),
                                    AppKeywordSearchColumn = CodeGenWay.GetCodeGenKeywordSearchColumn(formDbDto, "app"),
                                    PcDefaultSortConfig = pcColumnDesignModel.defaultSortConfig != null && pcColumnDesignModel.defaultSortConfig.Any(),
                                    AppDefaultSortConfig = appColumnDesignModel.defaultSortConfig != null && appColumnDesignModel.defaultSortConfig.Any(),
                                });
                                var dirPath = new DirectoryInfo(targetPathList[i]).Parent.FullName;
                                if (!Directory.Exists(dirPath))
                                    Directory.CreateDirectory(dirPath);
                                System.IO.File.WriteAllText(targetPathList[i], tResult, Encoding.UTF8);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }

                        codeGenMainTableConfigModel = codeGenConfigModel;
                    }

                    break;
            }
            // 强行将文件夹名称定义成 下载代码中输出配置的功能类名
            tableName = formDataModel.className.FirstOrDefault();

            // 还原模板
            controls = TemplateAnalysis.AnalysisTemplateData(fieldsCopy);

            // 捞取子表主键
            if (modelType.Equals(GeneratePatterns.MainBelt) || modelType.Equals(GeneratePatterns.PrimarySecondary))
            {
                foreach (var item in controls)
                {
                    if (item.__config__.ceriKey.Equals(CeriKeyConst.TABLE))
                        item.TablePrimaryKey = ctPrimaryKey[item.__config__.tableName].ReplaceRegex("^f_", string.Empty).ParseToPascalCase().ToLowerCase();
                }
            }

            // 生成前端
            await GenFrondEnd(categoryobj.ItemName, tableName.ToLowerCase(), codeGenConfigModel.DefaultSidx, formDataModel, controls, codeGenMainTableConfigModel.TableField, templateEntity, fileName, aliasList);
        }

        /// <summary>
        /// 生成前端.
        /// </summary>
        /// <param name="tableName">表名称.</param>
        /// <param name="defaultSidx">默认排序.</param>
        /// <param name="formDataModel">表单JSON包.</param>
        /// <param name="controls">移除布局控件后的控件列表.</param>
        /// <param name="tableColumns">表字段.</param>
        /// <param name="templateEntity">模板实体.</param>
        /// <param name="fileName">文件夹名称.</param>
        private async Task GenFrondEnd(string categoryName, string tableName, string defaultSidx, FormDataModel formDataModel, List<FieldsModel> controls, List<TableColumnConfigModel> tableColumns, VisualDevEntity templateEntity, string fileName, List<VisualAliasEntity> aliasList)
        {
            //var categoryName = (await _dictionaryDataService.GetInfo(templateEntity.Category)).EnCode;
            List<string> targetPathList = new List<string>();
            List<string> templatePathList = new List<string>();

            FrontEndGenConfigModel frondEndGenConfig = new FrontEndGenConfigModel();
            var vueVersion = 3;
            var userId = GetUser.GetUserIdByHttpContext(_httpContextAccessor);

            switch (vueVersion)
            {
                case 3:
                    {
                        CodeGenFrontEndConfigModel frontEndGenConfig = new CodeGenFrontEndConfigModel();

                        // 前端生成 APP与PC合并 4-pc,5-app
                        foreach (int logic in new List<int> { 4 })
                        {
                            // 每次循环前重新定义表单数据
                            formDataModel = JsonConvert.DeserializeObject<FormDataModel>(templateEntity.FormData);

                            frontEndGenConfig = CodeGenWay.CodeGenFrontEndEngine(logic, formDataModel, controls, tableColumns, templateEntity);
                            frontEndGenConfig.BasicInfo.MianTable = tableName;
                            frontEndGenConfig.BasicInfo.CreatorUserId = userId;
                            frontEndGenConfig.BasicInfo.AliasListJson = JsonConvert.SerializeObject(aliasList);

                            if (templateEntity.WebType.Equals(4))
                            {
                                targetPathList = CodeGenTargetPathHelper.FrontEndTargetPathList(tableName, fileName, templateEntity.WebType, frontEndGenConfig.HasFlow, frontEndGenConfig.HasDetail, frontEndGenConfig.TableConfig.HasSuperQuery);
                                templatePathList = CodeGenTargetPathHelper.FrontEndTemplatePathList(templateEntity.WebType, frontEndGenConfig.HasFlow, frontEndGenConfig.HasDetail, frontEndGenConfig.TableConfig.HasSuperQuery);
                            }
                            else
                            {
                                switch (templateEntity.Type)
                                {
                                    case 3:
                                        {
                                            targetPathList = CodeGenTargetPathHelper.FlowFrontEndTargetPathList(logic, tableName, fileName);
                                            templatePathList = CodeGenTargetPathHelper.FlowFrontEndTemplatePathList(logic, vueVersion);
                                        }

                                        break;
                                    default:
                                        {
                                            switch (logic)
                                            {
                                                case 4:
                                                    var columnDesignModel = JsonConvert.DeserializeObject<ColumnDesignModel>(templateEntity.ColumnData);
                                                    switch (templateEntity.WebType)
                                                    {
                                                        case 1:
                                                            frontEndGenConfig.TableConfig.HasSuperQuery = false;
                                                            frontEndGenConfig.HasSearch = false;
                                                            frontEndGenConfig.Type = 1;
                                                            break;
                                                    }

                                                    switch (frontEndGenConfig.Type)
                                                    {
                                                        case 4:
                                                            targetPathList = CodeGenTargetPathHelper.FrontEndInlineEditorTargetPathList(tableName, fileName, frontEndGenConfig.HasFlow, frontEndGenConfig.HasDetail, frontEndGenConfig.TableConfig.HasSuperQuery);
                                                            templatePathList = CodeGenTargetPathHelper.FrontEndInlineEditorTemplatePathList(frontEndGenConfig.HasFlow, frontEndGenConfig.HasDetail, frontEndGenConfig.TableConfig.HasSuperQuery);
                                                            break;
                                                        default:
                                                            targetPathList = CodeGenTargetPathHelper.FrontEndTargetPathList(tableName, fileName, templateEntity.WebType, frontEndGenConfig.HasFlow, frontEndGenConfig.HasDetail, frontEndGenConfig.TableConfig.HasSuperQuery);
                                                            templatePathList = CodeGenTargetPathHelper.FrontEndTemplatePathList(templateEntity.WebType, frontEndGenConfig.HasFlow, frontEndGenConfig.HasDetail, frontEndGenConfig.TableConfig.HasSuperQuery);
                                                            break;
                                                    }
                                                    break;
                                            }
                                        }

                                        break;
                                }
                            }

                            for (int i = 0; i < templatePathList.Count; i++)
                            {
                                string tContent = System.IO.File.ReadAllText(templatePathList[i]);
                                if (templatePathList[i].Contains("columnList.ts"))
                                {
                                    var columnObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(templateEntity.ColumnData).GetValueOrDefault("columnList");
                                    var cList = new List<Dictionary<string, object>>();
                                    var newObj = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(JsonConvert.SerializeObject(columnObj));
                                    foreach (var it in newObj) if (!it["ceriKey"].Equals(CeriKeyConst.CALCULATE)) cList.Add(it);
                                    frontEndGenConfig.ColumnList = JsonConvert.SerializeObject(cList, Formatting.Indented);
                                }
                                if (templatePathList[i].Contains("searchList.ts"))
                                {
                                    var columnObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(templateEntity.ColumnData).GetValueOrDefault("searchList");
                                    var cList = new List<Dictionary<string, object>>();
                                    var newObj = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(JsonConvert.SerializeObject(columnObj));
                                    foreach (var it in newObj) if (!it["ceriKey"].Equals(CeriKeyConst.CALCULATE)) cList.Add(it);
                                    frontEndGenConfig.SearchList = JsonConvert.SerializeObject(cList, Formatting.Indented);
                                }

                                var tResult = _viewEngine.RunCompileFromCached(tContent, frontEndGenConfig, builderAction: builder =>
                                {
                                    builder.AddUsing("CERIOS.Engine.Entity.Model.CodeGen");
                                    builder.AddAssemblyReferenceByName("CERIOS.Engine.Entity");
                                });
                                var dirPath = new DirectoryInfo(targetPathList[i]).Parent.FullName;
                                if (!Directory.Exists(dirPath))
                                    Directory.CreateDirectory(dirPath);
                                System.IO.File.WriteAllText(targetPathList[i], tResult, Encoding.UTF8);
                            }
                        }

                        foreach (int logic in new List<int> { 5 })
                        {
                            // 每次循环前重新定义表单数据
                            formDataModel = JsonConvert.DeserializeObject<FormDataModel>(templateEntity.FormData);

                            frondEndGenConfig = CodeGenWay.SingleTableFrontEnd(logic, formDataModel, controls, tableColumns, templateEntity);
                            frontEndGenConfig = CodeGenWay.CodeGenFrontEndEngine(logic, formDataModel, controls, tableColumns, templateEntity);
                            frontEndGenConfig.BasicInfo.MianTable = tableName;
                            frontEndGenConfig.BasicInfo.CreatorUserId = userId;
                            frontEndGenConfig.BasicInfo.AliasListJson = JsonConvert.SerializeObject(aliasList);

                            if (templateEntity.WebType.Equals(4))
                            {
                                targetPathList = CodeGenTargetPathHelper.AppFrontEndTargetPathList(tableName, fileName, templateEntity.WebType, frondEndGenConfig.IsDetail);
                                templatePathList = CodeGenTargetPathHelper.AppFrontEndTemplatePathList(templateEntity.WebType, frondEndGenConfig.IsDetail, true);
                            }
                            else
                            {
                                switch (templateEntity.Type)
                                {
                                    case 3:
                                        {
                                            targetPathList = CodeGenTargetPathHelper.FlowFrontEndTargetPathList(logic, tableName, fileName);
                                            templatePathList = CodeGenTargetPathHelper.FlowFrontEndTemplatePathList(logic, vueVersion);
                                        }

                                        break;
                                    default:
                                        {
                                            switch (logic)
                                            {
                                                case 4:
                                                    var columnDesignModel = templateEntity.ColumnData?.ToObject<ColumnDesignModel>();
                                                    var hasSuperQuery = false;
                                                    switch (templateEntity.WebType)
                                                    {
                                                        case 1:
                                                            hasSuperQuery = false;
                                                            frondEndGenConfig.Type = 1;
                                                            break;
                                                        default:
                                                            hasSuperQuery = columnDesignModel.hasSuperQuery;
                                                            break;
                                                    }

                                                    switch (frondEndGenConfig.Type)
                                                    {
                                                        case 4:
                                                            targetPathList = CodeGenTargetPathHelper.FrontEndInlineEditorTargetPathList(tableName, fileName, templateEntity.EnableFlow, frondEndGenConfig.IsDetail, hasSuperQuery);
                                                            templatePathList = CodeGenTargetPathHelper.FrontEndInlineEditorTemplatePathList(templateEntity.EnableFlow, frondEndGenConfig.IsDetail, hasSuperQuery);
                                                            break;
                                                        default:
                                                            targetPathList = CodeGenTargetPathHelper.FrontEndTargetPathList(tableName, fileName, templateEntity.WebType, templateEntity.EnableFlow, frondEndGenConfig.IsDetail, hasSuperQuery);
                                                            templatePathList = CodeGenTargetPathHelper.FrontEndTemplatePathList(templateEntity.WebType, templateEntity.EnableFlow, frondEndGenConfig.IsDetail, hasSuperQuery);
                                                            break;
                                                    }
                                                    break;
                                                case 5:
                                                    switch (templateEntity.EnableFlow)
                                                    {
                                                        case 0:
                                                            targetPathList = CodeGenTargetPathHelper.AppFrontEndTargetPathList(tableName, fileName, templateEntity.WebType, frondEndGenConfig.IsDetail);
                                                            templatePathList = CodeGenTargetPathHelper.AppFrontEndTemplatePathList(templateEntity.WebType, frondEndGenConfig.IsDetail, true);
                                                            break;
                                                        case 1:
                                                            targetPathList = CodeGenTargetPathHelper.AppFrontEndWorkflowTargetPathList(tableName, fileName, templateEntity.WebType);
                                                            templatePathList = CodeGenTargetPathHelper.AppFrontEndWorkflowTemplatePathList(templateEntity.WebType, true);
                                                            break;
                                                    }
                                                    break;
                                            }
                                        }

                                        break;
                                }
                            }

                            for (int i = 0; i < templatePathList.Count; i++)
                            {
                                if (templatePathList[i].Contains("columnList.js"))
                                {

                                    var columnObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(templateEntity.AppColumnData).GetValueOrDefault("columnList");
                                    var cList = new List<Dictionary<string, object>>();
                                    var newObj = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(JsonConvert.SerializeObject(columnObj));
                                    foreach (var it in newObj) if (!it["ceriKey"].Equals(CeriKeyConst.CALCULATE)) cList.Add(it);
                                    frondEndGenConfig.ColumnList = JsonConvert.SerializeObject(cList, Formatting.Indented);
                                }
                                if (templatePathList[i].Contains("searchList.js"))
                                {
                                    var columnObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(templateEntity.AppColumnData).GetValueOrDefault("searchList");
                                    var cList = new List<Dictionary<string, object>>();
                                    var newObj = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(JsonConvert.SerializeObject(columnObj));
                                    foreach (var it in newObj) if (!it["ceriKey"].Equals(CeriKeyConst.CALCULATE)) cList.Add(it);
                                    frontEndGenConfig.SearchList = JsonConvert.SerializeObject(cList, Formatting.Indented);
                                }
                                string tContent = System.IO.File.ReadAllText(templatePathList[i]);
                                var tResult = _viewEngine.RunCompileFromCached(tContent, new
                                {
                                    NameSpace = frondEndGenConfig.NameSpace,
                                    ClassName = frondEndGenConfig.ClassName,
                                    FormRef = frondEndGenConfig.FormRef,
                                    FormModel = frondEndGenConfig.FormModel,
                                    Size = frondEndGenConfig.Size,
                                    LabelPosition = frondEndGenConfig.LabelPosition,
                                    LabelWidth = frondEndGenConfig.LabelWidth,
                                    FormRules = frondEndGenConfig.FormRules,
                                    GeneralWidth = frondEndGenConfig.GeneralWidth,
                                    FullScreenWidth = frondEndGenConfig.FullScreenWidth,
                                    DrawerWidth = frondEndGenConfig.DrawerWidth,
                                    FormStyle = frondEndGenConfig.FormStyle,
                                    Type = frondEndGenConfig.Type,
                                    TreeRelation = frondEndGenConfig.TreeRelation,
                                    TreeSelectType = frondEndGenConfig.TreeSelectType,
                                    TreeAbleIds = frondEndGenConfig.TreeAbleIds,
                                    TreeJnpfKey = frondEndGenConfig.TreeCeriKey,
                                    TreeTitle = frondEndGenConfig.TreeTitle,
                                    TreePropsValue = frondEndGenConfig.TreePropsValue,
                                    TreeDataSource = frondEndGenConfig.TreeDataSource,
                                    TreeDictionary = frondEndGenConfig.TreeDictionary,
                                    TreePropsUrl = frondEndGenConfig.TreePropsUrl,
                                    TreePropsLabel = frondEndGenConfig.TreePropsLabel,
                                    TreePropsChildren = frondEndGenConfig.TreePropsChildren,
                                    TreeRelationControlKey = frondEndGenConfig.TreeRelationControlKey,
                                    IsTreeRelationMultiple = frondEndGenConfig.IsTreeRelationMultiple,
                                    IsExistQuery = frondEndGenConfig.IsExistQuery,
                                    PrimaryKey = frondEndGenConfig.PrimaryKey,
                                    FormList = frondEndGenConfig.FormList,
                                    PopupType = frondEndGenConfig.PopupType,
                                    SearchColumnDesign = frondEndGenConfig.SearchColumnDesign,
                                    IsKeywordSearchColumn = frondEndGenConfig.SearchColumnDesign?.Any(x => x.IsKeyword),
                                    IsAnyDefaultSearch = frondEndGenConfig.SearchColumnDesign?.Any(x => x.DefaultValues != "undefined"),
                                    SortFieldDesign = frondEndGenConfig.SortFieldDesign,
                                    TopButtonDesign = frondEndGenConfig.TopButtonDesign,
                                    ColumnButtonDesign = frondEndGenConfig.ColumnButtonDesign,
                                    ColumnDesign = frondEndGenConfig.ColumnDesign,
                                    OptionsList = frondEndGenConfig.OptionsList,
                                    IsBatchRemoveDel = frondEndGenConfig.IsBatchRemoveDel,
                                    IsBatchPrint = frondEndGenConfig.IsBatchPrint,
                                    PrintIds = frondEndGenConfig.PrintIds,
                                    IsDownload = frondEndGenConfig.IsDownload,
                                    IsRemoveDel = frondEndGenConfig.IsRemoveDel,
                                    IsDetail = frondEndGenConfig.IsDetail,
                                    IsEdit = frondEndGenConfig.IsEdit,
                                    IsAdd = frondEndGenConfig.IsAdd,
                                    IsSort = frondEndGenConfig.IsSort,
                                    IsUpload = frondEndGenConfig.IsUpload,
                                    FormAllContols = frondEndGenConfig.FormAllContols,
                                    ComplexFormAllContols = frondEndGenConfig.ComplexFormAllContols,
                                    CancelButtonText = frondEndGenConfig.CancelButtonText,
                                    ConfirmButtonText = frondEndGenConfig.ConfirmButtonText,
                                    UseBtnPermission = frondEndGenConfig.UseBtnPermission,
                                    UseColumnPermission = frondEndGenConfig.UseColumnPermission,
                                    UseFormPermission = frondEndGenConfig.UseFormPermission,
                                    DefaultSidx = defaultSidx,
                                    WebType = templateEntity.Type == 3 ? templateEntity.Type : templateEntity.WebType,
                                    HasPage = frondEndGenConfig.HasPage,
                                    IsSummary = frondEndGenConfig.IsSummary,
                                    AddTitleName = frondEndGenConfig.TopButtonDesign?.Find(it => it.Value.Equals("add"))?.Label,
                                    EditTitleName = frondEndGenConfig.ColumnButtonDesign?.Find(it => it.Value.Equals("edit"))?.Label,
                                    DetailTitleName = frondEndGenConfig.ColumnButtonDesign?.Find(it => it.Value.Equals("detail"))?.Label,
                                    PageSize = frondEndGenConfig.PageSize,
                                    Sort = frondEndGenConfig.Sort,
                                    HasPrintBtn = frondEndGenConfig.HasPrintBtn,
                                    PrintButtonText = frondEndGenConfig.PrintButtonText,
                                    PrintId = frondEndGenConfig.PrintId,
                                    EnCode = templateEntity.EnCode,
                                    FormId = templateEntity.Id,
                                    FullName = templateEntity.FullName,
                                    Category = categoryName,
                                    Tables = JsonConvert.SerializeObject(templateEntity.Tables),
                                    DbLinkId = templateEntity.DbLinkId,
                                    MianTable = tableName,
                                    PropertyJson = JsonConvert.SerializeObject(frondEndGenConfig.PropertyJson),
                                    CreatorTime = DateTime.Now.ParseToUnixTime(),
                                    CreatorUserId = userId,
                                    IsChildDataTransfer = frondEndGenConfig.IsChildDataTransfer,
                                    IsChildTableQuery = frondEndGenConfig.IsChildTableQuery,
                                    IsChildTableShow = frondEndGenConfig.IsChildTableShow,
                                    ColumnList = frondEndGenConfig.ColumnList,
                                    HasSuperQuery = frondEndGenConfig.HasSuperQuery,
                                    ColumnOptions = frondEndGenConfig.ColumnOptions,
                                    IsInlineEditor = frondEndGenConfig.IsInlineEditor,
                                    IndexDataType = frondEndGenConfig.IndexDataType,
                                    GroupField = frondEndGenConfig.GroupField,
                                    GroupShowField = frondEndGenConfig.GroupShowField,
                                    PrimaryKeyPolicy = frondEndGenConfig.PrimaryKeyPolicy,
                                    IsRelationForm = frondEndGenConfig.IsRelationForm,
                                    ChildTableStyle = controls.Any(it => it.__config__.ceriKey.Equals(CeriKeyConst.TABLE)) ? frondEndGenConfig.ChildTableStyle : 1,
                                    IsFixed = frondEndGenConfig.IsFixed,
                                    IsChildrenRegular = frondEndGenConfig.IsChildrenRegular,
                                    TreeSynType = frondEndGenConfig.TreeSynType,
                                    HasTreeQuery = frondEndGenConfig.HasTreeQuery,
                                    ColumnData = JsonConvert.SerializeObject(frondEndGenConfig.ColumnData),
                                    SummaryField = JsonConvert.SerializeObject(frondEndGenConfig.SummaryField),
                                    ShowSummary = frondEndGenConfig.ShowSummary,
                                    DefaultFormControlList = frondEndGenConfig.DefaultFormControlList,
                                    IsDefaultFormControl = frondEndGenConfig.IsDefaultFormControl,
                                    FormRealControl = frondEndGenConfig.FormRealControl,
                                    QueryCriteriaQueryVarianceList = frondEndGenConfig.QueryCriteriaQueryVarianceList,
                                    IsDateSpecialAttribute = frondEndGenConfig.IsDateSpecialAttribute,
                                    IsTimeSpecialAttribute = frondEndGenConfig.IsTimeSpecialAttribute,
                                    AllThousandsField = frondEndGenConfig.AllThousandsField,
                                    IsChildrenThousandsField = frondEndGenConfig.IsChildrenThousandsField,
                                    SpecifyDateFormatSet = frondEndGenConfig.SpecifyDateFormatSet,
                                    AppThousandField = frondEndGenConfig.AppThousandField,
                                    HasConfirmAndAddBtn = frondEndGenConfig.HasConfirmAndAddBtn,
                                    ConfirmAndAddText = frondEndGenConfig.ConfirmAndAddText,
                                    IsDefaultSearchField = frondEndGenConfig.IsDefaultSearchField,
                                    DefaultSearchList = frondEndGenConfig.DefaultSearchList,
                                    ShowOverflow = frondEndGenConfig.ShowOverflow,
                                    TableConfig = frontEndGenConfig.TableConfig,
                                    VueVersion = vueVersion,
                                    IsTabConfig = frontEndGenConfig.IsTabConfig,
                                    TabRelationField = frontEndGenConfig.TabRelationField,
                                    TabConfigHasAllTab = frontEndGenConfig.TabConfigHasAllTab,
                                    TabConfigDataType = frontEndGenConfig.TabConfigDataType,
                                    TabDictionaryType = frontEndGenConfig.TabDictionaryType,
                                    TabDataSource = frontEndGenConfig.TabDataSource,
                                    ExtraOptions = frondEndGenConfig.ExtraOptions,
                                    InterfaceRes = frondEndGenConfig.InterfaceRes,
                                }, builderAction: builder =>
                                {
                                    builder.AddUsing("CERIOS.Engine.Entity.Model.CodeGen");
                                    builder.AddAssemblyReferenceByName("CERIOS.Engine.Entity");
                                });
                                var dirPath = new DirectoryInfo(targetPathList[i]).Parent.FullName;
                                if (!Directory.Exists(dirPath))
                                    Directory.CreateDirectory(dirPath);
                                System.IO.File.WriteAllText(targetPathList[i], tResult, Encoding.UTF8);
                            }
                        }
                    }
                    break;
                default:
                    {
                        // 前端生成 APP与PC合并 4-pc,5-app
                        foreach (int logic in new List<int> { 4, 5 })
                        {
                            // 每次循环前重新定义表单数据
                            formDataModel = templateEntity.FormData.ToObject<FormDataModel>();

                            frondEndGenConfig = CodeGenWay.SingleTableFrontEnd(logic, formDataModel, controls, tableColumns, templateEntity);
                            var defaultSortConfig = "[]";
                            switch (templateEntity.Type)
                            {
                                case 3:
                                    {
                                        targetPathList = CodeGenTargetPathHelper.FlowFrontEndTargetPathList(logic, tableName, fileName);
                                        templatePathList = CodeGenTargetPathHelper.FlowFrontEndTemplatePathList(logic, vueVersion);
                                    }

                                    break;
                                default:
                                    {
                                        switch (logic)
                                        {
                                            case 4:
                                                var columnDesignModel = templateEntity.ColumnData?.ToObject<ColumnDesignModel>();
                                                defaultSortConfig = columnDesignModel?.defaultSortConfig != null ? columnDesignModel.defaultSortConfig?.ToJsonString() : "[]";
                                                var hasSuperQuery = false;
                                                switch (templateEntity.WebType)
                                                {
                                                    case 1:
                                                        hasSuperQuery = false;
                                                        frondEndGenConfig.Type = 1;
                                                        break;
                                                    default:
                                                        hasSuperQuery = columnDesignModel.hasSuperQuery;
                                                        break;
                                                }

                                                switch (frondEndGenConfig.Type)
                                                {
                                                    case 4:
                                                        targetPathList = CodeGenTargetPathHelper.FrontEndInlineEditorTargetPathList(tableName, fileName, templateEntity.EnableFlow, frondEndGenConfig.IsDetail, hasSuperQuery);
                                                        templatePathList = CodeGenTargetPathHelper.FrontEndInlineEditorTemplatePathList(templateEntity.EnableFlow, frondEndGenConfig.IsDetail, hasSuperQuery);
                                                        break;
                                                    default:
                                                        targetPathList = CodeGenTargetPathHelper.FrontEndTargetPathList(tableName, fileName, templateEntity.WebType, templateEntity.EnableFlow, frondEndGenConfig.IsDetail, hasSuperQuery);
                                                        templatePathList = CodeGenTargetPathHelper.FrontEndTemplatePathList(templateEntity.WebType, templateEntity.EnableFlow, frondEndGenConfig.IsDetail, hasSuperQuery);
                                                        break;
                                                }
                                                break;
                                            case 5:
                                                columnDesignModel = templateEntity.AppColumnData?.ToObject<ColumnDesignModel>();
                                                defaultSortConfig = columnDesignModel?.defaultSortConfig != null ? columnDesignModel.defaultSortConfig?.ToJsonString() : "[]";
                                                switch (templateEntity.EnableFlow)
                                                {
                                                    case 0:
                                                        targetPathList = CodeGenTargetPathHelper.AppFrontEndTargetPathList(tableName, fileName, templateEntity.WebType, frondEndGenConfig.IsDetail);
                                                        templatePathList = CodeGenTargetPathHelper.AppFrontEndTemplatePathList(templateEntity.WebType, frondEndGenConfig.IsDetail);
                                                        break;
                                                    case 1:
                                                        targetPathList = CodeGenTargetPathHelper.AppFrontEndWorkflowTargetPathList(tableName, fileName, templateEntity.WebType);
                                                        templatePathList = CodeGenTargetPathHelper.AppFrontEndWorkflowTemplatePathList(templateEntity.WebType);
                                                        break;
                                                }
                                                break;
                                        }
                                    }

                                    break;
                            }

                            for (int i = 0; i < templatePathList.Count; i++)
                            {
                                string tContent = System.IO.File.ReadAllText(templatePathList[i]);
                                var tResult = _viewEngine.RunCompileFromCached(tContent, new
                                {
                                    NameSpace = frondEndGenConfig.NameSpace,
                                    ClassName = frondEndGenConfig.ClassName,
                                    FormRef = frondEndGenConfig.FormRef,
                                    FormModel = frondEndGenConfig.FormModel,
                                    Size = frondEndGenConfig.Size,
                                    LabelPosition = frondEndGenConfig.LabelPosition,
                                    LabelWidth = frondEndGenConfig.LabelWidth,
                                    FormRules = frondEndGenConfig.FormRules,
                                    GeneralWidth = frondEndGenConfig.GeneralWidth,
                                    FullScreenWidth = frondEndGenConfig.FullScreenWidth,
                                    DrawerWidth = frondEndGenConfig.DrawerWidth,
                                    FormStyle = frondEndGenConfig.FormStyle,
                                    Type = frondEndGenConfig.Type,
                                    TreeRelation = frondEndGenConfig.TreeRelation,
                                    TreeSelectType = frondEndGenConfig.TreeSelectType,
                                    TreeAbleIds = frondEndGenConfig.TreeAbleIds,
                                    TreeJnpfKey = frondEndGenConfig.TreeCeriKey,
                                    TreeTitle = frondEndGenConfig.TreeTitle,
                                    TreePropsValue = frondEndGenConfig.TreePropsValue,
                                    TreeDataSource = frondEndGenConfig.TreeDataSource,
                                    TreeDictionary = frondEndGenConfig.TreeDictionary,
                                    TreePropsUrl = frondEndGenConfig.TreePropsUrl,
                                    TreePropsLabel = frondEndGenConfig.TreePropsLabel,
                                    TreePropsChildren = frondEndGenConfig.TreePropsChildren,
                                    TreeRelationControlKey = frondEndGenConfig.TreeRelationControlKey,
                                    IsTreeRelationMultiple = frondEndGenConfig.IsTreeRelationMultiple,
                                    IsExistQuery = frondEndGenConfig.IsExistQuery,
                                    PrimaryKey = frondEndGenConfig.PrimaryKey,
                                    FormList = frondEndGenConfig.FormList,
                                    PopupType = frondEndGenConfig.PopupType,
                                    SearchColumnDesign = frondEndGenConfig.SearchColumnDesign,
                                    IsKeywordSearchColumn = frondEndGenConfig.SearchColumnDesign?.Any(x => x.IsKeyword),
                                    IsAnyDefaultSearch = frondEndGenConfig.SearchColumnDesign?.Any(x => x.DefaultValues != "undefined"),
                                    SortFieldDesign = frondEndGenConfig.SortFieldDesign,
                                    TopButtonDesign = frondEndGenConfig.TopButtonDesign,
                                    ColumnButtonDesign = frondEndGenConfig.ColumnButtonDesign,
                                    ColumnDesign = frondEndGenConfig.ColumnDesign,
                                    OptionsList = frondEndGenConfig.OptionsList,
                                    IsBatchRemoveDel = frondEndGenConfig.IsBatchRemoveDel,
                                    IsBatchPrint = frondEndGenConfig.IsBatchPrint,
                                    PrintIds = frondEndGenConfig.PrintIds,
                                    IsDownload = frondEndGenConfig.IsDownload,
                                    IsRemoveDel = frondEndGenConfig.IsRemoveDel,
                                    IsDetail = frondEndGenConfig.IsDetail,
                                    IsEdit = frondEndGenConfig.IsEdit,
                                    IsAdd = frondEndGenConfig.IsAdd,
                                    IsSort = frondEndGenConfig.IsSort,
                                    IsUpload = frondEndGenConfig.IsUpload,
                                    FormAllContols = frondEndGenConfig.FormAllContols,
                                    ComplexFormAllContols = frondEndGenConfig.ComplexFormAllContols,
                                    CancelButtonText = frondEndGenConfig.CancelButtonText,
                                    ConfirmButtonText = frondEndGenConfig.ConfirmButtonText,
                                    UseBtnPermission = frondEndGenConfig.UseBtnPermission,
                                    UseColumnPermission = frondEndGenConfig.UseColumnPermission,
                                    UseFormPermission = frondEndGenConfig.UseFormPermission,
                                    DefaultSidx = defaultSidx,
                                    WebType = templateEntity.Type == 3 ? templateEntity.Type : templateEntity.WebType,
                                    HasPage = frondEndGenConfig.HasPage,
                                    IsSummary = frondEndGenConfig.IsSummary,
                                    AddTitleName = frondEndGenConfig.TopButtonDesign?.Find(it => it.Value.Equals("add"))?.Label,
                                    EditTitleName = frondEndGenConfig.ColumnButtonDesign?.Find(it => it.Value.Equals("edit"))?.Label,
                                    DetailTitleName = frondEndGenConfig.ColumnButtonDesign?.Find(it => it.Value.Equals("detail"))?.Label,
                                    PageSize = frondEndGenConfig.PageSize,
                                    Sort = frondEndGenConfig.Sort,
                                    HasPrintBtn = frondEndGenConfig.HasPrintBtn,
                                    PrintButtonText = frondEndGenConfig.PrintButtonText,
                                    PrintId = frondEndGenConfig.PrintId,
                                    EnCode = templateEntity.EnCode,
                                    FormId = templateEntity.Id,
                                    FullName = templateEntity.FullName,
                                    Category = categoryName,
                                    Tables = templateEntity.Tables.ToJsonString(),
                                    DbLinkId = templateEntity.DbLinkId,
                                    MianTable = tableName,
                                    PropertyJson = frondEndGenConfig.PropertyJson.ToJsonString(),
                                    CreatorTime = DateTime.Now.ParseToUnixTime(),
                                    CreatorUserId = userId,
                                    IsChildDataTransfer = frondEndGenConfig.IsChildDataTransfer,
                                    IsChildTableQuery = frondEndGenConfig.IsChildTableQuery,
                                    IsChildTableShow = frondEndGenConfig.IsChildTableShow,
                                    ColumnList = frondEndGenConfig.ColumnList,
                                    HasSuperQuery = frondEndGenConfig.HasSuperQuery,
                                    ColumnOptions = frondEndGenConfig.ColumnOptions,
                                    IsInlineEditor = frondEndGenConfig.IsInlineEditor,
                                    IndexDataType = frondEndGenConfig.IndexDataType,
                                    GroupField = frondEndGenConfig.GroupField,
                                    GroupShowField = frondEndGenConfig.GroupShowField,
                                    PrimaryKeyPolicy = frondEndGenConfig.PrimaryKeyPolicy,
                                    IsRelationForm = frondEndGenConfig.IsRelationForm,
                                    ChildTableStyle = controls.Any(it => it.__config__.ceriKey.Equals(CeriKeyConst.TABLE)) ? frondEndGenConfig.ChildTableStyle : 1,
                                    IsFixed = frondEndGenConfig.IsFixed,
                                    IsChildrenRegular = frondEndGenConfig.IsChildrenRegular,
                                    TreeSynType = frondEndGenConfig.TreeSynType,
                                    HasTreeQuery = frondEndGenConfig.HasTreeQuery,
                                    ColumnData = frondEndGenConfig.ColumnData.ToJsonString(),
                                    SummaryField = frondEndGenConfig.SummaryField.ToJsonString(),
                                    ShowSummary = frondEndGenConfig.ShowSummary,
                                    DefaultFormControlList = frondEndGenConfig.DefaultFormControlList,
                                    IsDefaultFormControl = frondEndGenConfig.IsDefaultFormControl,
                                    FormRealControl = frondEndGenConfig.FormRealControl,
                                    QueryCriteriaQueryVarianceList = frondEndGenConfig.QueryCriteriaQueryVarianceList,
                                    IsDateSpecialAttribute = frondEndGenConfig.IsDateSpecialAttribute,
                                    IsTimeSpecialAttribute = frondEndGenConfig.IsTimeSpecialAttribute,
                                    AllThousandsField = frondEndGenConfig.AllThousandsField,
                                    IsChildrenThousandsField = frondEndGenConfig.IsChildrenThousandsField,
                                    SpecifyDateFormatSet = frondEndGenConfig.SpecifyDateFormatSet,
                                    AppThousandField = frondEndGenConfig.AppThousandField,
                                    HasConfirmAndAddBtn = frondEndGenConfig.HasConfirmAndAddBtn,
                                    ConfirmAndAddText = frondEndGenConfig.ConfirmAndAddText,
                                    IsDefaultSearchField = frondEndGenConfig.IsDefaultSearchField,
                                    DefaultSearchList = frondEndGenConfig.DefaultSearchList,
                                    ShowOverflow = frondEndGenConfig.ShowOverflow,
                                    DefaultSortConfig = defaultSortConfig,
                                    VueVersion = 2,
                                    IsTabConfig = frondEndGenConfig.IsTabConfig,
                                    TabRelationField = frondEndGenConfig.TabRelationField,
                                    TabConfigHasAllTab = frondEndGenConfig.TabConfigHasAllTab,
                                    TabConfigDataType = frondEndGenConfig.TabConfigDataType,
                                    TabDictionaryType = frondEndGenConfig.TabDictionaryType,
                                    TabDataSource = frondEndGenConfig.TabDataSource,
                                }, builderAction: builder =>
                                {
                                    builder.AddUsing("CERIOS.Engine.Entity.Model.CodeGen");
                                    builder.AddAssemblyReferenceByName("CERIOS.Engine.Entity");
                                });
                                var dirPath = new DirectoryInfo(targetPathList[i]).Parent.FullName;
                                if (!Directory.Exists(dirPath))
                                    Directory.CreateDirectory(dirPath);
                                System.IO.File.WriteAllText(targetPathList[i], tResult, Encoding.UTF8);
                            }
                        }
                    }
                    break;
            }

        }

        /// <summary>
        /// 判断生成模式.
        /// </summary>
        /// <returns>1-纯主表、2-主带子、3-主带副、4-主带副与子.</returns>
        private GeneratePatterns JudgmentGenerationModel(List<DbTableRelationModel> tableRelation, List<FieldsModel> controls)
        {
            // 默认纯主表
            var codeModel = GeneratePatterns.PrimaryTable;

            // 找副表控件
            if (tableRelation.Count > 1 && controls.Any(x => x.__vModel__.Contains("_ceri_")) && controls.Any(it => it.__config__.ceriKey.Equals(CeriKeyConst.TABLE)))
                codeModel = GeneratePatterns.PrimarySecondary;
            else if (tableRelation.Count > 1 && controls.Any(x => x.__vModel__.Contains("_ceri_")))
                codeModel = GeneratePatterns.MainBeltVice;
            else if (tableRelation.Count > 1 && controls.Any(it => it.__config__.ceriKey.Equals(CeriKeyConst.TABLE)))
                codeModel = GeneratePatterns.MainBelt;
            switch (codeModel)
            {
                case GeneratePatterns.MainBelt:
                    // 在子表模式下 设计子表控件数量对不上表扣除主表后数量 强制定义为主子副模式
                    if (controls.Count(it => it.__config__.ceriKey.Equals(CeriKeyConst.TABLE)) < tableRelation.Count - 1)
                    {
                        codeModel = GeneratePatterns.PrimarySecondary;
                    }
                    break;
            }

            return codeModel;
        }

        /// <summary>
        /// 数据视图.
        /// </summary>
        /// <param name="templateEntity"></param>
        /// <returns></returns>
        private FormDbDto GetCodeGenDataViewEntity(FormDbDto templateEntity)
        {
            if (templateEntity.ColumnData.IsNullOrEmpty()) throw new Exception("无法下载空列表!"); ;
            var tableName = string.Format("mt{0}", templateEntity.DbId);
            string fileName = string.Format("{0}_{1:yyyyMMddHHmmss}", templateEntity.Name, DateTime.Now);
            //templateEntity.EnableFlow = 0;

            // 列表属性
            //ColumnDesignModel? pcColumnDesignModel = templateEntity.ColumnData?.ToObject<ColumnDesignModel>();
            ColumnDesignModel? pcColumnDesignModel = JsonConvert.DeserializeObject<ColumnDesignModel>(templateEntity.ColumnData);

            //ColumnDesignModel? appColumnDesignModel = templateEntity.AppColumnData?.ToObject<ColumnDesignModel>();
            ColumnDesignModel? appColumnDesignModel = JsonConvert.DeserializeObject<ColumnDesignModel>(templateEntity.AppColumnData);

            // 分组和树形表格去掉复杂表头
            if (pcColumnDesignModel.type.Equals(3))
            {
                //var columnData = templateEntity.ColumnData.ToObject<Dictionary<string, object>>();
                var columnData = JsonConvert.DeserializeObject<Dictionary<string, object>>(templateEntity.ColumnData);

                columnData["complexHeaderList"] = new List<object>();
                //templateEntity.ColumnData = columnData.ToJsonString();
                templateEntity.ColumnData = JsonConvert.SerializeObject(columnData);
            }

            var tbs = new List<FormDb>() { new FormDb() { TableName = tableName, TypeId = 1 } };
            templateEntity.FormDbs = tbs;
            templateEntity.FormJson = JsonConvert.SerializeObject(new FormDataModel() { fields = new List<FieldsModel>() });

            return templateEntity;
        }

        private void KingbaseNetType(List<ColumnInfo> fields, DBConfig dblink)
        {
            if (dblink.DbType.ToLower().Contains("kingbase") || dblink.DbType.ToLower().Contains("postgre"))
            {
                foreach (var item in fields.Where(x => x.DataType == "timestamp")) item.DataType = "datetime";
            }
        }

    }
}