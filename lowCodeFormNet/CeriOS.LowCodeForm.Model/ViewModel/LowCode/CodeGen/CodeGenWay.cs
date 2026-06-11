using CeriOS.LowCodeForm.Model.Helper;
using CeriOS.LowCodeForm.Model.ViewModel.HelperModel;
using CERIOS.Common.Const;
using CERIOS.Common.Extension;
using CERIOS.Common.Security;
using CERIOS.Engine.Entity.Model;
using CERIOS.Engine.Entity.Model.CodeGen;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using NPOI.Util;

namespace CeriOS.LowCodeForm.Model.ViewModel.LowCode.CodeGen
{
    public class CodeGenWay
    {
        /// <summary>
        /// 字段重命名.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="oldFieldModels"></param>
        /// <returns></returns>
        public static List<ColumnInfo> GetTableFieldModelReName(string table, List<ColumnInfo> oldFieldModels, List<VisualAliasEntity> alias, bool isMainTable = false)
        {
            foreach (var item in alias.Where(x => x.TableName.Equals(table) && x.FieldName.IsNotEmptyOrNull()))
            {
                var field = oldFieldModels.Find(x => x.Name != null && (x.Name.Equals(item.FieldName) || x.Name.Equals(item.FieldName.Split("_ceri_").Last())));
                if (field != null && item.AliasName.IsNotEmptyOrNull()) field.ReName = item.AliasName;
            }
            if (isMainTable)
            {
                var pField = oldFieldModels.Find(x => x.IsPrimaryKey && !x.Name.ToLower().Equals("f_tenant_id"));
                if (pField != null) pField.ReName = "id";
            }
            return oldFieldModels;
        }

        /// <summary>
        /// 获取关键词搜索条件 拼接.
        /// </summary>
        /// <param name="templateEntity">模板实体.</param>
        /// <param name="userOrigin">pc 、 app.</param>
        /// <returns></returns>
        public static string GetCodeGenKeywordSearchColumn(FormDbDto templateEntity, string userOrigin)
        {
            var tInfo = new TemplateParsingBase(templateEntity);
            var keywordSearchWhere = string.Empty;
            var columnWhere = string.Empty;
            var columnData = userOrigin.Equals("pc") ? tInfo.ColumnData : tInfo.AppColumnData;
            if (columnData != null && columnData.searchList != null && columnData.searchList.Any(x => x.isKeyword))
            {
                var whereList = new List<string>();
                columnData.searchList.Where(x => x.isKeyword).ToList().ForEach(item =>
                {
                    var fieldItem = tInfo.AllFieldsModel.Find(x => x.__vModel__.Equals(item.id));
                    var tName = fieldItem.__config__.tableName == null ? string.Empty : fieldItem.__config__.tableName;
                    var vModel = fieldItem.__vModel__;
                    if (vModel.Contains("_ceri_"))
                    {
                        vModel = tName.ParseToPascalCase() + "." + vModel.Split("_ceri_").Last().ToUpperCase();
                    }

                    if (fieldItem.__config__.parentVModel.IsNotEmptyOrNull() && vModel.Contains(fieldItem.__config__.parentVModel))
                    {
                        tName = fieldItem.__config__.relationTable == null ? string.Empty : fieldItem.__config__.relationTable;
                        vModel = vModel.Replace(fieldItem.__config__.parentVModel + "-", "");
                        if (fieldItem.__config__.ceriKey.Equals(CeriKeyConst.NUMINPUT)) columnWhere = string.Format("it.{0}List.Any(x=>SqlFunc.ToString(x.{1}).Contains(input.jnpfKeyword))", tName.ParseToPascalCase(), vModel.ToUpperCase());
                        else columnWhere = string.Format("it.{0}List.Any(x=>x.{1}.Contains(input.jnpfKeyword))", tName.ParseToPascalCase(), vModel.ToUpperCase());
                    }
                    else
                    {
                        if (fieldItem.__config__.ceriKey.Equals(CeriKeyConst.NUMINPUT)) columnWhere = string.Format("SqlFunc.ToString(it.{0}).Contains(input.jnpfKeyword)", vModel.ToUpperCase());
                        else columnWhere = string.Format("it.{0}.Contains(input.jnpfKeyword)", vModel.ToUpperCase());
                    }

                    whereList.Add(columnWhere);
                });

                keywordSearchWhere = string.Join(" || ", whereList);
            }

            return keywordSearchWhere;
        }


        /// <summary>
        /// 单表后端.
        /// </summary>
        /// <param name="tableName">表名称.</param>
        /// <param name="dbTableFields">表字段.</param>
        /// <param name="controls">控件列表.</param>
        /// <param name="templateEntity">模板实体.</param>
        /// <returns></returns>
        public static CodeGenConfigModel SingleTableBackEnd(string? tableName, List<ColumnInfo> dbTableFields, List<FieldsModel> controls, VisualDevEntity templateEntity)
        {
            // 表单数据
            ColumnDesignModel columnDesignModel = JsonConvert.DeserializeObject<ColumnDesignModel>(templateEntity.ColumnData);
            columnDesignModel ??= new ColumnDesignModel();
            columnDesignModel.searchList = GetMultiEndQueryMerging(templateEntity, controls);
            columnDesignModel.columnList = GetMultiTerminalListDisplayAndConsolidation(templateEntity);
            FormDataModel formDataModel = JsonConvert.DeserializeObject<FormDataModel>(templateEntity.FormData);
            var tableColumnList = new List<TableColumnConfigModel>();

            // 移除乐观锁
            dbTableFields.RemoveAll(it => it.Name.ReplaceRegex("^f_", string.Empty).ToLower().Equals("version"));

            // 移除真实流程ID
            dbTableFields.RemoveAll(it => it.Name.ReplaceRegex("^f_", string.Empty).ToLower().Equals("flow_task_id"));

            // 移除流程引擎ID
            dbTableFields.RemoveAll(it => it.Name.ReplaceRegex("^f_", string.Empty).ToLower().Equals("flow_id"));

            // 移除逻辑删除
            dbTableFields.RemoveAll(it => it.Name.ReplaceRegex("^f_", string.Empty).ToLower().Equals("delete_mark"));
            dbTableFields.RemoveAll(it => it.Name.ReplaceRegex("^f_", string.Empty).ToLower().Equals("delete_time"));
            dbTableFields.RemoveAll(it => it.Name.ReplaceRegex("^f_", string.Empty).ToLower().Equals("delete_user_id"));

            // 多租户隔离字段
            dbTableFields.RemoveAll(it => it.Name.ReplaceRegex("^f_", string.Empty).ToLower().Equals("tenant_id"));

            foreach (ColumnInfo? column in dbTableFields)
            {
                //var field = column.field.ReplaceRegex("^f_", string.Empty).ParseToPascalCase().ToLowerCase();
                var field = column.Name;
                switch (column.IsPrimaryKey)
                {
                    case true:
                        tableColumnList.Add(new TableColumnConfigModel()
                        {
                            LowerColumnName = column.ReName,
                            ColumnName = column.ReName.IsNullOrEmpty() ? field : column.ReName,
                            ReColumnName = column.ReName,
                            OriginalColumnName = column.Name,
                            ColumnComment = column.Description,
                            DataType = column.DataType,
                            NetType = formDataModel.primaryKeyPolicy == 1 ? "string?" : CodeGenHelper.ConvertDataType(column.DataType),
                            PrimaryKey = true,
                            IsConversion = false,
                            IsSystemControl = false,
                            IsUpdate = false,
                        });
                        break;
                    default:
                        // 存在表单内控件
                        switch (controls.Any(c => c.__vModel__ == field))
                        {
                            case true:
                                FieldsModel control = controls.Find(c => c.__vModel__ == field);
                                var isImportField = IsImportField(templateEntity.WebType, columnDesignModel, field, control.__config__.ceriKey);
                                switch (control.__config__.ceriKey)
                                {
                                    case CeriKeyConst.MODIFYUSER:
                                    case CeriKeyConst.CREATEUSER:
                                    case CeriKeyConst.CURRORGANIZE:
                                    case CeriKeyConst.CURRPOSITION:
                                        tableColumnList.Add(new TableColumnConfigModel()
                                        {
                                            ColumnName = field.ToUpperCase(),
                                            ReColumnName = column.ReName,
                                            OriginalColumnName = column.Name,
                                            ColumnComment = column.Description,
                                            DataType = column.DataType,
                                            NetType = CodeGenHelper.ConvertDataType(column.DataType),
                                            PrimaryKey = column.IsPrimaryKey.ParseToBool(),
                                            QueryWhether = control.isQueryField,
                                            QueryType = CodeGenFieldJudgeHelper.ColumnQueryType(searchList: columnDesignModel.searchList, field),
                                            QueryMultiple = CodeGenFieldJudgeHelper.ColumnQueryMultiple(searchList: columnDesignModel.searchList, field),
                                            IsShow = control.isIndexShow,
                                            IsUnique = control.__config__.unique,
                                            IsBusinessKey = formDataModel.businessKeyList != null && formDataModel.businessKeyList.Any(x => x.Equals(field)),
                                            IsMultiple = CodeGenFieldJudgeHelper.IsMultipleColumn(controls, field),
                                            ceriKey = control.__config__.ceriKey,
                                            Rule = control.__config__.rule,
                                            IsDateTime = CodeGenFieldJudgeHelper.IsDateTime(control),
                                            ActiveTxt = control.activeTxt,
                                            InactiveTxt = control.inactiveTxt,
                                            IsConversion = control.__config__.ceriKey.Equals(CeriKeyConst.CURRORGANIZE),
                                            IsDetailConversion = control.__config__.ceriKey.Equals(CeriKeyConst.CURRORGANIZE),
                                            IsSystemControl = true,
                                            IsUpdate = CodeGenControlsAttributeHelper.JudgeControlIsSystemControls(control.__config__.ceriKey),
                                            ControlLabel = control.__config__.label,
                                            IsImportField = isImportField.ParseToBool(),
                                            ImportConfig = CodeGenControlsAttributeHelper.GetImportConfig(columnDesignModel, control, column.Name, tableName, formDataModel),
                                            ShowLevel = control.showLevel,
                                            ShowAllLevels = control.showAllLevels,
                                            IsTreeParentField = field.Equals(columnDesignModel.parentField),
                                        });
                                        break;
                                    default:
                                        var dataType = control.__config__.dataType != null ? control.__config__.dataType : null;
                                        tableColumnList.Add(new TableColumnConfigModel()
                                        {
                                            ColumnName = field.ToUpperCase(),
                                            ReColumnName = column.ReName,
                                            OriginalColumnName = column.Name,
                                            ColumnComment = column.Description,
                                            DataType = column.DataType,
                                            NetType = CodeGenHelper.ConvertDataType(column.DataType),
                                            PrimaryKey = column.IsPrimaryKey.ParseToBool(),
                                            QueryWhether = control.isQueryField,
                                            QueryType = CodeGenFieldJudgeHelper.ColumnQueryType(searchList: columnDesignModel.searchList, field),
                                            QueryMultiple = CodeGenFieldJudgeHelper.ColumnQueryMultiple(searchList: columnDesignModel.searchList, field),
                                            IsShow = control.isIndexShow,
                                            IsMultiple = CodeGenFieldJudgeHelper.IsMultipleColumn(controls, field),
                                            IsUnique = control.__config__.unique,
                                            IsBusinessKey = formDataModel.businessKeyList != null && formDataModel.businessKeyList.Any(x => x.Equals(field)),
                                            ceriKey = control.__config__.ceriKey,
                                            Rule = control.__config__.rule,
                                            IsDateTime = CodeGenFieldJudgeHelper.IsDateTime(control),
                                            Format = control.format,
                                            ActiveTxt = control.activeTxt,
                                            InactiveTxt = control.inactiveTxt,
                                            ControlsDataType = dataType,
                                            StaticData = control.__config__.ceriKey.Equals(CeriKeyConst.CASCADER) || control.__config__.ceriKey.Equals(CeriKeyConst.TREESELECT) ? CodeGenControlsAttributeHelper.ConversionStaticData(JsonConvert.SerializeObject(control.options)) : CodeGenControlsAttributeHelper.ConversionStaticData(JsonConvert.SerializeObject(control.options)),
                                            propsUrl = CodeGenControlsAttributeHelper.GetControlsPropsUrl(control.__config__.ceriKey, dataType, control),
                                            Label = CodeGenControlsAttributeHelper.GetControlsLabel(control.__config__.ceriKey, dataType, control),
                                            Value = CodeGenControlsAttributeHelper.GetControlsValue(control.__config__.ceriKey, dataType, control),
                                            Children = CodeGenControlsAttributeHelper.GetControlsChildren(control.__config__.ceriKey, dataType, control),
                                            Separator = control.__config__.ceriKey.Equals(CeriKeyConst.CASCADER) ? "/" : control.separator,
                                            IsConversion = CodeGenControlsAttributeHelper.JudgeControlIsDataConversion(control.__config__.ceriKey, dataType, CodeGenFieldJudgeHelper.IsMultipleColumn(controls, field)),
                                            IsDetailConversion = CodeGenControlsAttributeHelper.JudgeControlIsDataConversion(control.__config__.ceriKey, dataType, CodeGenFieldJudgeHelper.IsMultipleColumn(controls, field)),
                                            IsSystemControl = false,
                                            IsUpdate = CodeGenControlsAttributeHelper.JudgeControlIsSystemControls(control.__config__.ceriKey),
                                            ControlLabel = control.__config__.label,
                                            IsImportField = isImportField.ParseToBool(),
                                            ImportConfig = CodeGenControlsAttributeHelper.GetImportConfig(columnDesignModel, control, column.Name, tableName, formDataModel),
                                            ShowField = control.relational,
                                            ShowAllLevels = control.showAllLevels,
                                            IsTreeParentField = field.Equals(columnDesignModel.parentField),
                                            isStorage = CodeGenControlsAttributeHelper.GetIsControlStoreType(control.__config__.ceriKey, control.isStorage),
                                            IsLinkage = CodeGenControlsAttributeHelper.IsControlLinkageConfiguration(control),
                                            LinkageConfig = CodeGenControlsAttributeHelper.ObtainTheCurrentControlLinkageConfiguration(control, 0),
                                        });
                                        break;
                                }

                                break;
                            case false:
                                //tableColumnList.Add(new TableColumnConfigModel()
                                //{
                                //    ColumnName = field.ToUpperCase(),
                                //    ReColumnName = column.reName,
                                //    OriginalColumnName = column.field,
                                //    ColumnComment = column.fieldName,
                                //    DataType = column.dataType,
                                //    NetType = CodeGenHelper.ConvertDataType(column.dataType),
                                //    PrimaryKey = false,
                                //    IsConversion = false,
                                //    IsSystemControl = false,
                                //    IsUpdate = false,
                                //});
                                break;
                        }

                        break;
                }
            }

            if (!tableColumnList.Any(t => t.PrimaryKey))
                throw new Exception("表缺失主键");

            return GetCodeGenConfigModel(formDataModel, columnDesignModel, tableColumnList, controls, tableName, templateEntity);
        }

        /// <summary>
        /// 前端.
        /// </summary>
        /// <param name="logic">生成逻辑;4-pc,5-app.</param>
        /// <param name="formDataModel">表单Json包.</param>
        /// <param name="controls">移除布局控件后的控件列表.</param>
        /// <param name="tableColumns">表字段.</param>
        /// <param name="templateEntity">模板实体.</param>
        /// <returns></returns>
        public static FrontEndGenConfigModel SingleTableFrontEnd(int logic, FormDataModel formDataModel, List<FieldsModel> controls, List<TableColumnConfigModel> tableColumns, VisualDevEntity templateEntity)
        {
            ColumnDesignModel columnDesignModel = new ColumnDesignModel();
            var columnOptions = string.Empty; // 前端要原生模板的 ColumnOptions
            bool isInlineEditor = false;
            switch (logic)
            {
                case 4:
                    columnDesignModel = JsonConvert.DeserializeObject<ColumnDesignModel>(templateEntity.ColumnData);
                    if (columnDesignModel != null)
                    {
                        var opt = templateEntity.ColumnData.ToObject<Dictionary<string, object>>();
                        if (opt.ContainsKey("columnOptions")) columnOptions = opt["columnOptions"].ToJsonString();
                    }

                    columnDesignModel ??= new ColumnDesignModel();
                    isInlineEditor = columnDesignModel.type == 4 ? true : false;
                    break;
                case 5:
                    ColumnDesignModel pcColumnDesignModel = JsonConvert.DeserializeObject<ColumnDesignModel>(templateEntity.ColumnData);
                    if (pcColumnDesignModel != null)
                    {
                        var opt = JsonConvert.DeserializeObject<Dictionary<string, object>>(templateEntity.ColumnData);
                        if (opt.ContainsKey("columnOptions")) columnOptions = JsonConvert.SerializeObject(opt["columnOptions"]);
                    }
                    isInlineEditor = pcColumnDesignModel?.type == 4 ? true : false;
                    columnDesignModel = JsonConvert.DeserializeObject<ColumnDesignModel>(templateEntity.AppColumnData);
                    columnDesignModel ??= new ColumnDesignModel();

                    // 移动端的分页遵循PC端
                    columnDesignModel.hasPage = templateEntity.WebType == 1 ? false : pcColumnDesignModel.hasPage;
                    break;
            }

            switch (templateEntity.Type)
            {
                case 3:
                    break;
                default:
                    if (templateEntity.WebType != 1)
                        controls = CodeGenUnifiedHandlerHelper.UnifiedHandlerFormDataModel(controls, columnDesignModel);
                    break;
            }

            // 联动关系链判断
            controls = CodeGenUnifiedHandlerHelper.LinkageChainJudgment(controls, columnDesignModel);

            Dictionary<string, List<string>> listQueryControls = CodeGenQueryControlClassificationHelper.ListQueryControl(logic);

            /*
             *  PC 逻辑时： 行内编辑时 pc端需要循环子表日期控件
             *  APP 逻辑时：循环出除子表外全部开启千位符的数字输入控件字段
             */
            List<CodeGenSpecifyDateFormatSetModel> specifyDateFormatSet = new List<CodeGenSpecifyDateFormatSetModel>();
            var appThousandField = string.Empty;
            switch (logic)
            {
                case 4:
                    switch (columnDesignModel.type)
                    {
                        case 4:
                            foreach (var item in controls)
                            {
                                var config = item.__config__;
                                switch (config.ceriKey)
                                {
                                    case CeriKeyConst.TABLE:
                                        var model = CodeGenFormControlDesignHelper.CodeGenSpecifyDateFormatSetModel(item);
                                        if (model != null)
                                            specifyDateFormatSet.Add(model);
                                        break;
                                }
                            }
                            break;
                    }
                    break;
                case 5:
                    //if (formDataModel.labelPosition.Equals("right"))
                    //{
                    //    formDataModel.labelPosition = "left";
                    //}
                    appThousandField = JsonConvert.SerializeObject(controls.FindAll(it => it.__config__.ceriKey.Equals(CeriKeyConst.NUMINPUT) && it.thousands).Select(it => it.__vModel__).ToList());
                    appThousandField = appThousandField == "[]" ? null : appThousandField;
                    break;
            }

            // 表单脚本设计
            List<FormScriptDesignModel>? formScriptDesign = CodeGenFormControlDesignHelper.FormScriptDesign("SingleTable", controls, tableColumns, columnDesignModel?.columnList);

            // 整个表单控件
            List<FormControlDesignModel>? formControlList = CodeGenFormControlDesignHelper.FormControlDesign(formDataModel.fields, controls, formDataModel, columnDesignModel?.columnList, columnDesignModel.type, logic, 2, true);

            var formRealControl = CodeGenFormControlDesignHelper.FormRealControl(controls);

            // 列表控件Option
            var indnxControlOption = CodeGenFormControlDesignHelper.FormControlProps(formDataModel.fields, controls, columnDesignModel, logic, true);

            // 列表查询字段设计
            var indexSearchFieldDesign = new List<IndexSearchFieldDesignModel>();

            // 查询条件查询差异列表
            var queryCriteriaQueryVarianceList = new List<IndexSearchFieldModel>();

            // 列表顶部按钮
            var indexTopButton = new List<IndexButtonDesign>();

            // 列表行按钮
            var indexColumnButtonDesign = new List<IndexButtonDesign>();

            // 列表页列表
            var indexColumnDesign = new List<IndexColumnDesign>();

            var indexSortFieldDesign = new List<IndexSearchFieldDesignModel>();

            switch (templateEntity.Type)
            {
                case 3:
                    break;
                default:
                    switch (templateEntity.WebType)
                    {
                        case 4:
                        case 2:

                            var newSearchList = new List<IndexSearchFieldModel>();

                            switch (logic)
                            {
                                case 4:
                                    newSearchList = JsonConvert.DeserializeObject<ColumnDesignModel>(templateEntity.ColumnData).searchList;

                                    switch (columnDesignModel.type)
                                    {
                                        case 2:
                                            newSearchList = CodeGenUnifiedHandlerHelper.UnifiedHandlerListQueries(newSearchList, JsonConvert.DeserializeObject<ColumnDesignModel>(templateEntity.ColumnData), JsonConvert.DeserializeObject<ColumnDesignModel>(templateEntity.AppColumnData));
                                            break;
                                    }
                                    break;
                                case 5:
                                    newSearchList = JsonConvert.DeserializeObject<ColumnDesignModel>(templateEntity.AppColumnData).searchList;
                                    break;
                            }

                            // 本身查询列表内带有控件全属性 单表不需要匹配表字段
                            var newlist = newSearchList?.Where(x => !x.noShow && x.__config__.ceriKey != CeriKeyConst.CALCULATE).ToList() ?? new List<IndexSearchFieldModel>();
                            foreach (var item in newlist)
                            {
                                // 查询控件分类
                                var queryControls = listQueryControls.Where(q => q.Value.Contains(item.__config__.ceriKey)).FirstOrDefault();

                                var childTableLabel = string.Empty;
                                var childControl = item.__config__.parentVModel;

                                // 是否子表查询
                                bool isChildQuery = false;

                                // 表单真实控件
                                FieldsModel? column = new FieldsModel();
                                if (!string.IsNullOrEmpty(childControl))
                                {
                                    isChildQuery = true;
                                    column = controls.Find(it => it.__config__.ceriKey.Equals(CeriKeyConst.TABLE) && it.__vModel__.Equals(childControl) && it.__config__.children.Any(child => child.__vModel__.Equals(item.__vModel__)));
                                    childTableLabel = column.__config__.label;
                                    column = column.__config__.children.Find(it => it.__vModel__ == item.__vModel__);
                                }
                                else
                                {
                                    column = controls.Find(c => c.__vModel__ == item.__vModel__);
                                }

                                var placeholder = item.ceriKey.Equals(CeriKeyConst.COMINPUT) || item.ceriKey.Equals(CeriKeyConst.LOCATION) || item.ceriKey.Equals(CeriKeyConst.AUTOCOMPLETE) || item.ceriKey.Equals(CeriKeyConst.TEXTAREA) ? "$t('common.inputText')" : "$t('common.chooseText')";

                                if (templateEntity.WebType.Equals(4))
                                {
                                    item.__config__.labelI18nCode = item.labelI18nCode;
                                    item.__config__.label = item.label;
                                    var searchDefaultValue = item.value.IsNotEmptyOrNull() ? JsonConvert.SerializeObject(item.value) : "undefined";
                                    indexSearchFieldDesign.Add(new IndexSearchFieldDesignModel()
                                    {
                                        OriginalName = string.IsNullOrEmpty(item.__config__.parentVModel) ? item.__vModel__ : string.Format("{0}_{1}", item.__config__.parentVModel, item.__vModel__),
                                        Name = string.IsNullOrEmpty(item.__config__.parentVModel) ? item.__vModel__ : string.Format("{0}_{1}", item.__config__.parentVModel, item.__vModel__),
                                        LowerName = item.__vModel__,
                                        DefaultValues = searchDefaultValue.Equals("[]") ? "undefined" : searchDefaultValue,
                                        Tag = item.__config__.tag,
                                        Clearable = item.clearable ? "clearable " : string.Empty,
                                        Format = item.format,
                                        ValueFormat = item.valueformat.IsNullOrEmpty() ? item.format : item.valueformat,
                                        Placeholder = logic == 5 && item.__config__.labelI18nCode.IsNotEmptyOrNull() ? " :placeholder=\"" + placeholder + " + ' ' + $t('" + item.__config__.labelI18nCode + "', '" + item.__config__.label + "')\" " : (item.__config__.label.IsNullOrEmpty() ? string.Empty : " :placeholder=\"" + placeholder + " + ' " + item.__config__.label + "'\" "),
                                        Label = logic == 5 && item.__config__.labelI18nCode.IsNotEmptyOrNull() ? " :label=\"$t('" + item.__config__.labelI18nCode + "', '" + item.__config__.label + "')\" " : (item.__config__.label.IsNullOrEmpty() ? string.Empty : " label='" + item.__config__.label + "' "),
                                        IsChildQuery = isChildQuery,
                                        QueryControlsKey = queryControls.Key != null ? queryControls.Key : null,
                                        Props = item.props,
                                        Index = newSearchList.IndexOf(item),
                                        Type = item.type,
                                        ShowAllLevels = (item?.showAllLevels).ParseToBool() ? "true" : "false",
                                        Level = item.level,
                                        IsMultiple = item.searchMultiple,
                                        IsKeyword = item.isKeyword,
                                        ceriKey = item.__config__.ceriKey,
                                        SelectType = item.selectType != null ? item.selectType.Equals("custom") ? string.Format("selectType='{0}' ", item.selectType) : string.Format("selectType='all' ") : string.Empty,
                                        AbleIds = item.selectType != null && item.selectType == "custom" ? string.Format(":ableIds='{0}_AbleIds' ", !string.IsNullOrEmpty(childControl) ? string.Format("{0}_{1}", childControl, item.__vModel__) : item.__vModel__) : string.Empty,
                                        RelationField = item.relationField,
                                        InterfaceId = item.interfaceId,
                                        Total = item.total,
                                        NoShow = item.noShow,
                                    });
                                }
                                else
                                {
                                    item.__config__.labelI18nCode = item.labelI18nCode;
                                    item.__config__.label = item.label;
                                    var searchDefaultValue = item.value.IsNotEmptyOrNull() ? JsonConvert.SerializeObject(item.value) : "undefined";
                                    indexSearchFieldDesign.Add(new IndexSearchFieldDesignModel()
                                    {
                                        OriginalName = string.IsNullOrEmpty(column.__config__.parentVModel) ? column.__vModel__ : string.Format("{0}_{1}", column.__config__.parentVModel, column.__vModel__),
                                        Name = string.IsNullOrEmpty(column.__config__.parentVModel) ? column.__vModel__ : string.Format("{0}_{1}", column.__config__.parentVModel, column.__vModel__),
                                        LowerName = column.__vModel__,
                                        DefaultValues = searchDefaultValue.Equals("[]") ? "undefined" : searchDefaultValue,
                                        Tag = column.__config__.tag,
                                        Clearable = item.clearable ? "clearable " : string.Empty,
                                        Format = column.format,
                                        ValueFormat = column.valueformat.IsNullOrEmpty() ? column.format : column.valueformat,
                                        Placeholder = logic == 5 && item.__config__.labelI18nCode.IsNotEmptyOrNull() ? " :placeholder=\"" + placeholder + " + ' ' + $t('" + item.__config__.labelI18nCode + "', '" + item.__config__.label + "')\" " : (item.__config__.label.IsNullOrEmpty() ? string.Empty : " :placeholder=\"" + placeholder + " + ' " + item.__config__.label + "'\" "),
                                        Label = logic == 5 && item.__config__.labelI18nCode.IsNotEmptyOrNull() ? " :label=\"$t('" + item.__config__.labelI18nCode + "', '" + item.__config__.label + "')\" " : (item.__config__.label.IsNullOrEmpty() ? string.Empty : " label='" + item.__config__.label + "' "),
                                        IsChildQuery = isChildQuery,
                                        QueryControlsKey = queryControls.Key != null ? queryControls.Key : null,
                                        Props = column.props,
                                        Index = newSearchList.IndexOf(item),
                                        Type = column.type,
                                        ShowAllLevels = (column?.showAllLevels).ParseToBool() ? "true" : "false",
                                        Level = column.level,
                                        IsMultiple = item.searchMultiple,
                                        IsKeyword = item.isKeyword,
                                        ceriKey = column.__config__.ceriKey,
                                        SelectType = column.selectType != null ? column.selectType.Equals("custom") ? string.Format("selectType='{0}' ", column.selectType) : string.Format("selectType='all' ") : string.Empty,
                                        AbleIds = column.selectType != null && column.selectType == "custom" ? string.Format(":ableIds='{0}_AbleIds' ", !string.IsNullOrEmpty(childControl) ? string.Format("{0}_{1}", childControl, column.__vModel__) : item.__vModel__) : string.Empty,
                                        RelationField = column.relationField,
                                        InterfaceId = column.interfaceId,
                                        Total = column.total,
                                        NoShow = item.noShow,
                                    });
                                }
                            }

                            var multipleQueryFields = GetMultiEndQueryMerging(templateEntity);

                            // 控件查询多选数组
                            var controlQueryMultipleSelectionArray = new List<string>
                        {
                            CeriKeyConst.SELECT,
                            CeriKeyConst.DEPSELECT,
                            CeriKeyConst.ROLESELECT,
                            CeriKeyConst.USERSELECT,
                            CeriKeyConst.USERSSELECT,
                            CeriKeyConst.COMSELECT,
                            CeriKeyConst.POSSELECT,
                            CeriKeyConst.GROUPSELECT,
                        };

                            // 查询条件查询差异列表

                            queryCriteriaQueryVarianceList = columnDesignModel.searchList
    ?.FindAll(it => controlQueryMultipleSelectionArray.Contains(it.__config__.ceriKey))
    ?.ToList()
    ?.FindAll(it => !it.searchMultiple.Equals(multipleQueryFields
        .Find(x => x.__config__.ceriKey.Equals(it.__config__.ceriKey) && x.prop.Equals(it.prop))
        .searchMultiple))
    ?? new List<IndexSearchFieldModel>();
                            //queryCriteriaQueryVarianceList = columnDesignModel.searchList.FindAll(it => controlQueryMultipleSelectionArray.Contains(it.__config__.ceriKey)).ToList().FindAll(it => !it.searchMultiple.Equals(multipleQueryFields.Find(x => x.__config__.ceriKey.Equals(it.__config__.ceriKey) && x.prop.Equals(it.prop)).searchMultiple));

                            queryCriteriaQueryVarianceList?.ForEach(item =>
                            {
                                switch (item.__config__.isSubTable)
                                {
                                    case true:
                                        item.__vModel__ = string.Format("{0}_{1}", item.__config__.parentVModel, item.__vModel__);
                                        break;
                                }
                            });

                            // 生成头部按钮信息
                            foreach (var item in columnDesignModel?.btnsList ?? new List<ButtonConfigModel>())
                            {
                                indexTopButton.Add(new IndexButtonDesign()
                                {
                                    Type = columnDesignModel.btnsList.IndexOf(item) == 0 ? "primary" : "text",
                                    Icon = item.icon,
                                    Method = GetCodeGenIndexButtonHelper.IndexTopButton(item.value, templateEntity.EnableFlow == 1 ? true : false),
                                    Value = item.value,
                                    Show = item.show,
                                    Label = logic == 5 && item.labelI18nCode.IsNotEmptyOrNull() ? "this.$t('" + item.labelI18nCode + "','" + item.label + "')" : item.label,
                                });
                            }

                            if (!templateEntity.WebType.Equals(4))
                            {
                                // 生成行按钮信息
                                foreach (var item in columnDesignModel.columnBtnsList ?? new List<ButtonConfigModel>())
                                {
                                    indexColumnButtonDesign.Add(new IndexButtonDesign()
                                    {
                                        Type = item.value == "remove" ? "class='CERI-table-delBtn' " : string.Empty,
                                        Icon = item.icon,
                                        Method = GetCodeGenIndexButtonHelper.IndexColumnButton(item.value, tableColumns.Find(it => it.PrimaryKey.Equals(true))?.LowerColumnName, formDataModel.primaryKeyPolicy, templateEntity.EnableFlow == 1 ? true : false, columnDesignModel?.type == 4 ? true : false),
                                        Value = item.value,
                                        Label = logic == 5 && item.labelI18nCode.IsNotEmptyOrNull() ? "{{this.$t('" + item.labelI18nCode + "','" + item.label + "')}}" : "'" + item.label + "'",
                                        Show = item.show,
                                        Disabled = GetCodeGenIndexButtonHelper.WorkflowIndexColumnButton(item.value)
                                    });
                                }
                            }

                            List<string> ChildControlField = new List<string>();

                            // 生成列信息
                            foreach (var item in columnDesignModel.columnList ?? new List<IndexGridFieldModel>())
                            {
                                if (!ChildControlField.Any(it => it == item.id))
                                {
                                    var relationTable = item?.__config__?.relationTable;
                                    if (relationTable != null && !indexColumnDesign.Any(it => it.TableName == relationTable))
                                    {
                                        var childTableAll = columnDesignModel.columnList.FindAll(it => it.__config__.relationTable == relationTable);
                                        var childTable = controls.Find(it => it.__config__.tableName == relationTable);
                                        if (childTable.__config__.ceriKey.Equals(CeriKeyConst.TABLE))
                                        {
                                            var childTableColumnDesign = new List<IndexColumnDesign>();
                                            foreach (var child in childTableAll)
                                            {
                                                var columnControl = childTable.__config__.children.Find(it => it.__vModel__.Equals(child.id.Split('-')[1]));
                                                childTableColumnDesign.Add(new IndexColumnDesign()
                                                {
                                                    TableName = child.__config__.tableName,
                                                    Name = columnControl.__vModel__,
                                                    OptionsName = columnControl.__vModel__,
                                                    LowerName = columnControl.__vModel__,
                                                    ceriKey = child.__config__.ceriKey,
                                                    Label = columnControl.__config__.label,
                                                    Width = child.width.ToString() == "0" ? "0" : string.Format("{0}", child.width),
                                                    Align = child.align,
                                                    IsSort = child.sortable ? string.Format("sortable='custom' ") : string.Empty,
                                                    IsChildTable = true,
                                                    Format = child.format?.ToLower().Replace(":mm", ":MM"),
                                                    ModelId = child.modelId,
                                                    Thousands = child.thousands,
                                                    Precision = child.precision == null ? 0 : child.precision,
                                                    UseMask = child.useMask ? "useMask " : string.Empty,
                                                    UseScan = child.useScan ? "useScan " : string.Empty,
                                                    MaskConfig = child.maskConfig != null ? $":maskConfig='{JsonConvert.SerializeObject(child.maskConfig)}' " : string.Empty,
                                                });
                                                ChildControlField.Add(string.Format("{0}", child.id));
                                            }

                                            indexColumnDesign.Add(new IndexColumnDesign()
                                            {
                                                TableName = relationTable,
                                                Name = childTable.__vModel__,
                                                Label = childTable.__config__.label,
                                                ceriKey = CeriKeyConst.TABLE,
                                                IsChildTable = true,
                                                ChildTableDesigns = childTableColumnDesign,
                                                Fixed = string.Empty,
                                            });
                                        }
                                    }
                                    else
                                    {
                                        indexColumnDesign.Add(new IndexColumnDesign()
                                        {
                                            TableName = item?.__config__?.tableName,
                                            Name = item.prop,
                                            OptionsName = item.prop,
                                            LowerName = item.prop,
                                            ceriKey = item.__config__.ceriKey,
                                            Label = item.label,
                                            Width = item.width == null ? string.Empty : string.Format("width='{0}' ", item.width),
                                            Fixed = columnDesignModel.childTableStyle == 1 ? (item.@fixed == "none" || item.@fixed == null ? string.Empty : string.Format("fixed='{0}' ", item.@fixed)) : string.Empty,
                                            Align = item.align,
                                            IsSort = item.sortable ? string.Format("sortable='custom' ") : string.Empty,
                                            IsChildTable = false,
                                            ModelId = item.modelId,
                                            Thousands = item.thousands,
                                            Precision = item.precision == null ? 0 : item.precision,
                                            UseMask = item.useMask ? "useMask " : string.Empty,
                                            UseScan = item.useScan ? "useScan " : string.Empty,
                                            MaskConfig = item.maskConfig != null ? $":maskConfig='{JsonConvert.SerializeObject(item.maskConfig)}' " : string.Empty,
                                        });
                                    }

                                }
                            }

                            foreach (var item in columnDesignModel?.columnList.Where(x => x.sortable && x.__config__.ceriKey != CeriKeyConst.CALCULATE).ToList())
                            {
                                indexSortFieldDesign.Add(new IndexSearchFieldDesignModel
                                {
                                    Label = item.label.IsNullOrEmpty() ? item.__config__.label : item.label,
                                    labelI18nCode = item.labelI18nCode.IsNullOrEmpty() ? item.__config__.labelI18nCode : item.labelI18nCode,
                                    Name = item.__vModel__,
                                });
                            }

                            break;
                    }

                    break;
            }

            // 复杂表头 处理
            if (columnDesignModel.type.Equals(3) || columnDesignModel.type.Equals(5)) columnDesignModel.complexHeaderList.Clear();

            var indexComplexHeaderList = new List<IndexColumnDesign>();
            List<FormControlDesignModel> complexFormAllContols = null;
            if (logic.Equals(4) && templateEntity.WebType != 1)
            {
                if (columnDesignModel.complexHeaderList != null && columnDesignModel.complexHeaderList.Any())
                {
                    var newColumnList = new List<IndexColumnDesign>();
                    var tfVModelList = columnDesignModel.columnList.Where(x => !x.__config__.tableFixed.Equals("none")).Select(x => x.id).ToList();
                    foreach (var item in columnDesignModel.columnList)
                    {
                        if (item.__config__.tableFixed.Equals("none") && columnDesignModel.complexHeaderList.Any(x => x.childColumns.Any(xx => xx.Equals(item.id))))
                        {
                            var complexPItem = columnDesignModel.complexHeaderList.FirstOrDefault(x => x.childColumns.Any(xx => xx.Equals(item.id)));
                            if (!newColumnList.Any(x => x.Name.Equals(complexPItem.id)))
                            {
                                var addItem = new IndexColumnDesign();
                                addItem.Name = complexPItem.id;
                                addItem.Align = complexPItem.align;
                                addItem.Label = complexPItem.fullName;
                                addItem.Fixed = string.Empty;
                                addItem.ComplexColumns = new List<IndexColumnDesign>();
                                complexPItem.childColumns.Where(x => !tfVModelList.Contains(x)).ToList().ForEach(it =>
                                {
                                    var cItem = columnDesignModel.columnList.Find(x => x.id.Equals(it));
                                    addItem.ComplexColumns.Add(new IndexColumnDesign()
                                    {
                                        TableName = cItem?.__config__?.tableName,
                                        Name = cItem.prop,
                                        OptionsName = cItem.prop,
                                        LowerName = cItem.prop,
                                        ceriKey = cItem.__config__.ceriKey,
                                        Label = cItem.label,
                                        Width = cItem.width == null ? string.Empty : string.Format("width='{0}' ", cItem.width),
                                        Fixed = columnDesignModel.childTableStyle == 1 ? (cItem.@fixed == "none" || cItem.@fixed == null ? string.Empty : string.Format("fixed='{0}' ", cItem.@fixed)) : string.Empty,
                                        Align = cItem.align,
                                        IsSort = cItem.sortable ? string.Format("sortable='custom' ") : string.Empty,
                                        IsChildTable = false,
                                        ModelId = cItem.modelId,
                                        Thousands = cItem.thousands,
                                        Precision = cItem.precision == null ? 0 : cItem.precision,
                                        UseMask = cItem.useMask ? "useMask " : string.Empty,
                                        UseScan = cItem.useScan ? "useScan " : string.Empty,
                                        MaskConfig = cItem.maskConfig != null ? $":maskConfig='{JsonConvert.SerializeObject(cItem.maskConfig)}' " : string.Empty,
                                    });
                                });
                                newColumnList.Add(addItem);
                            }
                        }
                        else
                        {
                            if (!newColumnList.Any(x => x.Name.Equals(item.id) || x.Name.Equals(item.__config__.parentVModel)))
                                newColumnList.Add(indexColumnDesign.Find(x => x.Name.Equals(item.id) || x.Name.Equals(item.__config__.parentVModel)));
                        }
                    }

                    foreach (var item in newColumnList)
                    {
                        if (item.ComplexColumns != null && item.ComplexColumns.Any())
                        {

                            item.CurrentIndex = indexColumnDesign.IndexOf(indexColumnDesign.Find(x => x.Name.Equals(item.ComplexColumns.FirstOrDefault().Name)));
                        }
                        else
                        {
                            item.CurrentIndex = indexColumnDesign.IndexOf(item);
                        }
                    }

                    indexColumnDesign = newColumnList.OrderBy(x => x.CurrentIndex).ToList();

                    if (isInlineEditor)
                    {
                        var newFormControlList = new List<FormControlDesignModel>();
                        var flist = CodeGenFormControlDesignHelper.GetFormControlDesignByTree(formControlList);
                        var ctfVModelList = columnDesignModel.columnList.Where(x => !x.__config__.tableFixed.Equals("none")).Select(x => x.id).ToList();

                        foreach (var item in columnDesignModel.columnList)
                        {
                            if (item.__config__.tableFixed.Equals("none") && columnDesignModel.complexHeaderList.Any(x => x.childColumns.Any(xx => xx.Equals(item.id))))
                            {
                                var complexPItem = columnDesignModel.complexHeaderList.FirstOrDefault(x => x.childColumns.Any(xx => xx.Equals(item.id)));
                                if (!newFormControlList.Any(x => x.Name.Equals(complexPItem.id)))
                                {
                                    var addItem = new FormControlDesignModel();
                                    addItem.Name = complexPItem.id;
                                    addItem.OriginalName = complexPItem.id;
                                    addItem.Align = complexPItem.align;
                                    addItem.Label = complexPItem.fullName;
                                    addItem.ComplexColumns = new List<FormControlDesignModel>();
                                    complexPItem.childColumns.Where(x => !ctfVModelList.Contains(x)).ToList().ForEach(it =>
                                    {
                                        var cItem = flist.Find(x => x.Name.Equals(it) && x.ceriKey != CeriKeyConst.POPUPATTR && x.ceriKey != CeriKeyConst.RELATIONFORMATTR).Copy();
                                        if (formDataModel.labelSuffix.IsNotEmptyOrNull()) cItem.Label = cItem.Label.Replace(formDataModel.labelSuffix, "");
                                        addItem.ComplexColumns.Add(cItem);
                                    });
                                    newFormControlList.Add(addItem);
                                }
                            }
                            else
                            {
                                if (!newFormControlList.Any(x => x.Name.Equals(item.__vModel__) || x.Name.Equals(item.__config__.parentVModel)))
                                {
                                    if (flist.Any(x => x.Name.Equals(item.id)))
                                    {
                                        var addItem = flist.Find(x => x.Name.Equals(item.id)).Copy();
                                        if (formDataModel.labelSuffix.IsNotEmptyOrNull()) addItem.Label = addItem.Label.Replace(formDataModel.labelSuffix, "");
                                        newFormControlList.Add(addItem);
                                    }
                                    else
                                    {
                                        var addItem = flist.Find(x => x.Name.IsNotEmptyOrNull() && (x.Name.Equals(item.__vModel__) || x.Name.Equals(item.__config__.parentVModel))).Copy();
                                        if (formDataModel.labelSuffix.IsNotEmptyOrNull()) addItem.Label = addItem.Label.Replace(formDataModel.labelSuffix, "");
                                        if (addItem != null) newFormControlList.Add(addItem);
                                    }
                                }
                            }
                        }

                        foreach (var item in newFormControlList)
                        {
                            if (item.ComplexColumns != null && item.ComplexColumns.Any())
                                item.CurrentIndex = columnDesignModel.columnList.IndexOf(columnDesignModel.columnList.Find(x => x.id.Equals(item.ComplexColumns.FirstOrDefault().Name)));
                            else
                                item.CurrentIndex = columnDesignModel.columnList.IndexOf(columnDesignModel.columnList.Find(x => x.id.Equals(item.Name)));
                        }

                        complexFormAllContols = newFormControlList.Where(x => x.ceriKey != CeriKeyConst.TABLE).OrderBy(x => x.CurrentIndex).ToList();
                    }
                }
            }

            var propertyJson = CodeGenFormControlDesignHelper.GetPropertyJson(formScriptDesign);

            var printIds = columnDesignModel.printIds != null ? string.Join(",", columnDesignModel.printIds) : null;
            var isBatchRemoveDel = indexTopButton.Any(it => it.Value == "batchRemove" && it.Show);
            var isBatchPrint = indexTopButton.Any(it => it.Value == "batchPrint" && it.Show);
            var isUpload = indexTopButton.Any(it => it.Value == "upload" && it.Show);
            var isDownload = indexTopButton.Any(it => it.Value == "download" && it.Show);
            var isRemoveDel = indexColumnButtonDesign.Any(it => it.Value == "remove" && it.Show);
            var isEdit = indexColumnButtonDesign.Any(it => it.Value == "edit" && it.Show);
            var isDetail = indexColumnButtonDesign.Any(it => it.Value == "detail" && it.Show);
            var isAdd = indexTopButton.Any(it => it.Value == "add" && it.Show);
            var isSort = columnDesignModel?.columnList?.Any(it => it.sortable) ?? false;
            var isSummary = formScriptDesign.Any(it => it.ceriKey.Equals("table") && it.ShowSummary.Equals(true));
            var isTreeRelation = !string.IsNullOrEmpty(columnDesignModel?.treeRelation);
            var isRelationForm = formControlList.Any(it => it.IsRelationForm);
            var isTreeRelationMultiple = indexSearchFieldDesign.Any(it => it.Name.Equals(columnDesignModel?.treeRelation?.Replace("-", "_")) && it.IsMultiple);
            var isFixed = columnDesignModel.childTableStyle == 1 ? indexColumnDesign.Any(it => it.Fixed.Equals("fixed='left' ") && !it.Name.Equals(columnDesignModel.groupField)) : false;
            var isChildrenRegular = formScriptDesign.Any(it => it.ceriKey.Equals(CeriKeyConst.TABLE) && it.RegList != null && it.RegList.Count > 0);
            var treeRelationControlKey = indexSearchFieldDesign.Find(it => it.Name.Equals(columnDesignModel?.treeRelation?.Replace("-", "_")))?.ceriKey;

            string allThousandsField = JsonConvert.SerializeObject(columnDesignModel.summaryField?.Intersect(formScriptDesign.FindAll(it => it.Thousands && !it.ceriKey.Equals(CeriKeyConst.TABLE)).Select(it => it.Name).ToList()).ToList());
            bool isChildrenThousandsField = formScriptDesign.Any(it => it.ceriKey.Equals(CeriKeyConst.TABLE) && it.Thousands);

            // 是否开启特殊属性
            var isDateSpecialAttribute = CodeGenFormControlDesignHelper.DetermineWhetherTheControlHasEnabledSpecialAttributes(controls, CeriKeyConst.DATE);
            var isTimeSpecialAttribute = CodeGenFormControlDesignHelper.DetermineWhetherTheControlHasEnabledSpecialAttributes(controls, CeriKeyConst.TIME);

            // 表单默认值控件列表
            var defaultFormControlList = new DefaultFormControlModel();
            var defaultSearchList = new List<DefaultSearchFieldModel>();
            switch (logic)
            {
                case 4:
                    columnDesignModel = JsonConvert.DeserializeObject<ColumnDesignModel>(templateEntity.ColumnData);
                    columnDesignModel ??= new ColumnDesignModel();
                    defaultFormControlList = CodeGenFormControlDesignHelper.DefaultFormControlList(controls, columnDesignModel.searchList);
                    defaultSearchList = CodeGenFormControlDesignHelper.DefaultSearchFieldList(columnDesignModel.searchList);
                    break;
                case 5:
                    ColumnDesignModel pcColumnDesignModel = JsonConvert.DeserializeObject<ColumnDesignModel>(templateEntity.ColumnData);
                    columnDesignModel = JsonConvert.DeserializeObject<ColumnDesignModel>(templateEntity.AppColumnData);
                    columnDesignModel ??= new ColumnDesignModel();
                    defaultFormControlList = CodeGenFormControlDesignHelper.DefaultFormControlList(controls, columnDesignModel.searchList);
                    defaultSearchList = CodeGenFormControlDesignHelper.DefaultSearchFieldList(columnDesignModel.searchList);

                    // 移动端的分页遵循PC端
                    columnDesignModel.hasPage = templateEntity.WebType == 1 ? false : pcColumnDesignModel.hasPage;
                    break;
            }

            var isDefaultFormControl = defaultFormControlList.IsExistTime || defaultFormControlList.IsExistDate || defaultFormControlList.IsExistDepSelect || defaultFormControlList.IsExistComSelect || defaultFormControlList.IsSignField ||
                defaultFormControlList.IsExistUserSelect || defaultFormControlList.IsExistUsersSelect || defaultFormControlList.IsExistRoleSelect || defaultFormControlList.IsExistPosSelect || defaultFormControlList.IsExistGroupsSelect || defaultFormControlList.IsExistSubTable ? true : false;

            // 是否查询默认值
            var isDefaultSearchField = defaultSearchList.Count > 0 ? true : false;

            switch (columnDesignModel.type)
            {
                case 3:
                case 5:
                    columnDesignModel.hasPage = false;
                    break;
            }

            List<CodeGenFrontEndFormState> extraOptions = new List<CodeGenFrontEndFormState>();
            List<CodeGenFrontEndFormState> interfaceRes = new List<CodeGenFrontEndFormState>();
            foreach (var item in formControlList)
            {
                switch (item.ceriKey)
                {
                    case CeriKeyConst.RELATIONFORM:
                        extraOptions.Add(item.ExtraOption);
                        break;
                    case CeriKeyConst.POPUPSELECT:
                        extraOptions.Add(item.ExtraOption);
                        item.ModelId = item.InterfaceId.Split("='").Last().Replace("'", string.Empty);
                        interfaceRes.Add(item.InterfaceRes);
                        break;
                }
            }

            var result = new FrontEndGenConfigModel();
            switch (templateEntity.WebType)
            {
                case 1:
                    result = new FrontEndGenConfigModel()
                    {
                        NameSpace = formDataModel.areasName,
                        ClassName = formDataModel.className.FirstOrDefault(),
                        FormRef = formDataModel.formRef,
                        FormModel = formDataModel.formModel,
                        Size = formDataModel.size,
                        LabelPosition = formDataModel.labelPosition,
                        LabelWidth = formDataModel.labelWidth,
                        FormRules = formDataModel.formRules,
                        GeneralWidth = formDataModel.generalWidth,
                        FullScreenWidth = formDataModel.fullScreenWidth,
                        DrawerWidth = formDataModel.drawerWidth,
                        FormStyle = formDataModel.formStyle,
                        Type = columnDesignModel.type,
                        PrimaryKey = tableColumns?.Find(it => it.PrimaryKey.Equals(true))?.LowerColumnName,
                        FormList = formScriptDesign,
                        PopupType = formDataModel.popupType,
                        OptionsList = indnxControlOption,
                        IsRemoveDel = isRemoveDel,
                        IsDetail = isDetail,
                        IsEdit = isEdit,
                        IsAdd = isAdd,
                        IsSort = isSort,
                        HasPage = columnDesignModel.hasPage,
                        FormAllContols = formControlList,
                        ComplexFormAllContols = complexFormAllContols,
                        CancelButtonText = logic == 5 && formDataModel.cancelButtonTextI18nCode.IsNotEmptyOrNull() ? "{{$t('" + formDataModel.cancelButtonTextI18nCode + "','" + formDataModel.cancelButtonText + "')}}" : formDataModel.cancelButtonText,
                        ConfirmButtonText = logic == 5 && formDataModel.confirmButtonTextI18nCode.IsNotEmptyOrNull() ? "{{$t('" + formDataModel.confirmButtonTextI18nCode + "','" + formDataModel.confirmButtonText + "')}}" : formDataModel.confirmButtonText,
                        UseBtnPermission = columnDesignModel.useBtnPermission,
                        UseColumnPermission = false,
                        UseFormPermission = false,
                        IsSummary = isSummary,
                        PageSize = columnDesignModel.pageSize,
                        Sort = columnDesignModel.sort,
                        HasPrintBtn = formDataModel.hasPrintBtn,
                        PrintButtonText = logic == 5 && formDataModel.printButtonTextI18nCode.IsNotEmptyOrNull() ? "{{$t('" + formDataModel.printButtonTextI18nCode + "','" + formDataModel.printButtonText + "')}}" : formDataModel.printButtonText,
                        PrintId = formDataModel.printId != null ? string.Join(",", formDataModel.printId) : null,
                        IsChildDataTransfer = formScriptDesign.Any(it => it.IsDataTransfer.Equals(true)),
                        IsChildTableQuery = indexSearchFieldDesign.Any(it => it.IsChildQuery.Equals(true)),
                        IsChildTableShow = indexColumnDesign.Any(it => it.IsChildTable.Equals(true)),
                        ColumnList = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<Dictionary<string, object>>(templateEntity.ColumnData).GetValueOrDefault("columnList")),
                        IsInlineEditor = isInlineEditor,
                        GroupField = columnDesignModel.groupField,
                        GroupShowField = columnDesignModel?.columnList?.Where(x => x.__vModel__.ToLower() != columnDesignModel?.groupField?.ToLower()).FirstOrDefault()?.__vModel__,
                        PrimaryKeyPolicy = formDataModel.primaryKeyPolicy,
                        IsRelationForm = isRelationForm,
                        ChildTableStyle = columnDesignModel.childTableStyle,
                        IsChildrenRegular = isChildrenRegular,
                        DefaultFormControlList = defaultFormControlList,
                        IsDefaultFormControl = isDefaultFormControl,
                        PropertyJson = propertyJson,
                        FormRealControl = formRealControl,
                        IsDateSpecialAttribute = isDateSpecialAttribute,
                        IsTimeSpecialAttribute = isTimeSpecialAttribute,
                        IsChildrenThousandsField = isChildrenThousandsField,
                        HasConfirmAndAddBtn = formDataModel.hasConfirmAndAddBtn,
                        ConfirmAndAddText = formDataModel.confirmAndAddText,
                        ExtraOptions = extraOptions,
                        InterfaceRes = interfaceRes,
                    };
                    break;
                default:
                    var codeGenColumnData = new CodeGenColumnData
                    {
                        treeInterfaceId = columnDesignModel.treeSyncInterfaceId,
                        treeTemplateJson = columnDesignModel.treeSyncTemplateJson,
                        templateJson = columnDesignModel.treeTemplateJson,
                    };
                    result = new FrontEndGenConfigModel();

                    result.PrintIds = printIds;
                    result.NameSpace = formDataModel.areasName;
                    result.ClassName = formDataModel.className.FirstOrDefault();
                    result.FormRef = formDataModel.formRef;
                    result.FormModel = formDataModel.formModel;
                    result.Size = formDataModel.size;
                    result.LabelPosition = formDataModel.labelPosition;
                    result.LabelWidth = formDataModel.labelWidth;
                    result.FormRules = formDataModel.formRules;
                    result.GeneralWidth = formDataModel.generalWidth;
                    result.FullScreenWidth = formDataModel.fullScreenWidth;
                    result.DrawerWidth = formDataModel.drawerWidth;
                    result.FormStyle = formDataModel.formStyle;
                    result.Type = columnDesignModel.type;
                    result.TreeRelation = columnDesignModel?.treeRelation?.Replace("-", "_");
                    result.TreeSelectType = columnDesignModel?.columnList?.FirstOrDefault(x => x.prop.Equals(columnDesignModel?.treeRelation))?.selectType;
                    result.TreeAbleIds = JsonConvert.SerializeObject(columnDesignModel?.columnList?.FirstOrDefault(x => x.prop.Equals(columnDesignModel?.treeRelation))?.ableIds);
                    result.TreeCeriKey = columnDesignModel?.columnList?.FirstOrDefault(x => x.prop.Equals(columnDesignModel?.treeRelation))?.ceriKey;
                    result.TreeTitle = columnDesignModel?.treeTitle;
                    result.TreePropsValue = columnDesignModel?.treePropsValue;
                    result.TreeDataSource = columnDesignModel?.treeDataSource;
                    result.TreeDictionary = columnDesignModel?.treeDictionary;
                    result.TreePropsUrl = columnDesignModel?.treePropsUrl;
                    result.TreePropsChildren = columnDesignModel?.treePropsChildren;
                    result.TreePropsLabel = columnDesignModel?.treePropsLabel;
                    result.TreeRelationControlKey = treeRelationControlKey;
                    result.IsTreeRelationMultiple = isTreeRelationMultiple;
                    result.IsExistQuery = templateEntity.Type == 3 ? false : columnDesignModel?.searchList?.Any(it => it.prop.Equals(columnDesignModel?.treeRelation)) ?? false ;
                    result.PrimaryKey = tableColumns?.Find(it => it.PrimaryKey.Equals(true))?.LowerColumnName;
                    result.FormList = formScriptDesign;
                    result.PopupType = formDataModel.popupType;
                    result.SearchColumnDesign = indexSearchFieldDesign;
                    result.SortFieldDesign = indexSortFieldDesign;
                    result.TopButtonDesign = indexTopButton;
                    result.ColumnButtonDesign = indexColumnButtonDesign;
                    result.ColumnDesign = indexColumnDesign;
                    result.OptionsList = indnxControlOption;
                    result.IsBatchRemoveDel = isBatchRemoveDel;
                    result.IsBatchPrint = isBatchPrint;
                    result.IsDownload = isDownload;
                    result.IsRemoveDel = isRemoveDel;
                    result.IsDetail = isDetail;
                    result.IsEdit = isEdit;
                    result.IsAdd = isAdd;
                    result.IsUpload = isUpload;
                    result.IsSort = isSort;
                    result.HasPage = columnDesignModel.hasPage;
                    result.FormAllContols = formControlList;
                    result.ComplexFormAllContols = complexFormAllContols;
                    result.CancelButtonText = logic == 5 && formDataModel.cancelButtonTextI18nCode.IsNotEmptyOrNull() ? "{{$t('" + formDataModel.cancelButtonTextI18nCode + "','" + formDataModel.cancelButtonText + "')}}" : formDataModel.cancelButtonText;
                    result.ConfirmButtonText = logic == 5 && formDataModel.confirmButtonTextI18nCode.IsNotEmptyOrNull() ? "{{$t('" + formDataModel.confirmButtonTextI18nCode + "','" + formDataModel.confirmButtonText + "')}}" : formDataModel.confirmButtonText;
                    result.UseBtnPermission = columnDesignModel.useBtnPermission;
                    result.UseColumnPermission = columnDesignModel.useColumnPermission;
                    result.UseFormPermission = columnDesignModel.useFormPermission;
                    result.IsSummary = isSummary;
                    result.PageSize = columnDesignModel.pageSize;
                    result.Sort = columnDesignModel.sort;
                    result.HasPrintBtn = formDataModel.hasPrintBtn;
                    result.PrintButtonText = logic == 5 && formDataModel.printButtonTextI18nCode.IsNotEmptyOrNull() ? "{{$t('" + formDataModel.printButtonTextI18nCode + "','" + formDataModel.printButtonText + "')}}" : formDataModel.printButtonText;
                    result.PrintId = formDataModel.printId != null ? string.Join(",", formDataModel.printId) : null;
                    result.IsChildDataTransfer = formScriptDesign.Any(it => it.IsDataTransfer.Equals(true));
                    result.IsChildTableQuery = indexSearchFieldDesign.Any(it => it.IsChildQuery.Equals(true));
                    result.IsChildTableShow = indexColumnDesign.Any(it => it.IsChildTable.Equals(true));
                    result.ColumnList = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<Dictionary<string, object>>(templateEntity.ColumnData).GetValueOrDefault("columnList"));
                    result.HasSuperQuery = columnDesignModel.hasSuperQuery;
                    result.ColumnOptions = columnOptions;
                    result.IsInlineEditor = isInlineEditor;
                    result.GroupField = columnDesignModel.groupField;
                    result.GroupShowField = columnDesignModel?.columnList?.Where(x => x.__vModel__.ToLower() != columnDesignModel?.groupField?.ToLower()).FirstOrDefault()?.__vModel__;
                    result.PrimaryKeyPolicy = formDataModel.primaryKeyPolicy;
                    result.IsRelationForm = isRelationForm;
                    result.ChildTableStyle = columnDesignModel.childTableStyle;
                    result.IsFixed = isFixed;
                    result.IsChildrenRegular = isChildrenRegular;
                    result.TreeSynType = columnDesignModel.treeSyncType;
                    result.HasTreeQuery = columnDesignModel.hasTreeQuery;
                    result.ColumnData = codeGenColumnData;
                    result.SummaryField = columnDesignModel.summaryField;
                    result.ShowSummary = columnDesignModel.showSummary;
                    result.DefaultFormControlList = defaultFormControlList;
                    result.IsDefaultFormControl = isDefaultFormControl;
                    result.IsDefaultSearchField = isDefaultSearchField;
                    result.DefaultSearchList = defaultSearchList;
                    result.PropertyJson = propertyJson;
                    result.FormRealControl = formRealControl;
                    result.QueryCriteriaQueryVarianceList = queryCriteriaQueryVarianceList;
                    result.IsDateSpecialAttribute = isDateSpecialAttribute;
                    result.IsTimeSpecialAttribute = isTimeSpecialAttribute;
                    result.AllThousandsField = allThousandsField;
                    result.IsChildrenThousandsField = isChildrenThousandsField;
                    result.SpecifyDateFormatSet = specifyDateFormatSet;
                    result.AppThousandField = appThousandField;
                    result.HasConfirmAndAddBtn = formDataModel.hasConfirmAndAddBtn;
                    result.ConfirmAndAddText = formDataModel.confirmAndAddText;
                    result.ShowOverflow = columnDesignModel.showOverflow;
                    result.ExtraOptions = extraOptions;
                    result.InterfaceRes = interfaceRes;

                    break;
            }

            // 标签面板
            var columnData = logic.Equals(4) ? JsonConvert.DeserializeObject<ColumnDesignModel>(templateEntity.ColumnData) : JsonConvert.DeserializeObject<ColumnDesignModel>(templateEntity.AppColumnData);
            if (columnData != null && columnData.tabConfig != null && columnData.tabConfig.on && columnData.tabConfig.relationField.IsNotEmptyOrNull() && (columnData.type.Equals(1) || columnData.type.Equals(4)))
            {
                var relationField = controls.Find(x => x.__vModel__.Equals(columnData.tabConfig.relationField));
                if (relationField != null)
                {
                    if (columnData.tabConfig.hasAllTab)
                        result.TabConfigHasAllTab = true;
                    result.TabConfigDataType = relationField.__config__.dataType;
                    if (relationField.__config__.dataType.Equals("static"))
                    {
                        var tabConfig = new List<Dictionary<string, object>>();
                        relationField.options.ForEach(it => tabConfig.Add(new Dictionary<string, object>() { { "id", it["id"] }, { "fullName", it["fullName"] } }));
                        result.TabDataSource = JsonConvert.SerializeObject(tabConfig);
                    }
                    else if (relationField.__config__.dataType.Equals("dictionary"))
                    {
                        result.TabDataSource = relationField.__config__.dictionaryType;
                        result.TabDictionaryType = relationField.props.value;
                    }

                    result.IsTabConfig = true;
                    result.TabRelationField = columnData.tabConfig.relationField;
                }
            }

            return result;
        }

        /// <summary>
        /// 代码生成前端引擎.
        /// </summary>
        /// <param name="logic">生成逻辑;4-pc,5-app.</param>
        /// <param name="formDataModel">表单Json包.</param>
        /// <param name="controls">移除布局控件后的控件列表.</param>
        /// <param name="tableColumns">表字段.</param>
        /// <param name="templateEntity">模板实体.</param>
        /// <returns></returns>
        public static CodeGenFrontEndConfigModel CodeGenFrontEndEngine(int logic, FormDataModel formDataModel, List<FieldsModel> controls, List<TableColumnConfigModel> tableColumns, VisualDevEntity templateEntity)
        {
            var result = new CodeGenFrontEndConfigModel();
            ColumnDesignModel columnDesignModel = new ColumnDesignModel();
            var columnOptions = string.Empty; // 前端要原生模板的 ColumnOptions

            // 是否开启流程
            var hasFlow = templateEntity.EnableFlow == 1 ? true : false;
            if (templateEntity.Type == 3)
                hasFlow = true;

            // 导入URL
            var importUrl = string.Format("{0}/{1}", formDataModel.areasName, formDataModel.className.FirstOrDefault());

            // 判断生成逻辑模版读取方案
            switch (logic)
            {
                case 4:
                    columnDesignModel = JsonConvert.DeserializeObject<ColumnDesignModel>(templateEntity.ColumnData);
                    if (columnDesignModel != null)
                    {
                        var opt = JsonConvert.DeserializeObject<Dictionary<string, object>>(templateEntity.ColumnData);
                        if (opt.ContainsKey("columnOptions")) columnOptions = JsonConvert.SerializeObject(opt["columnOptions"]);
                    }
                    columnDesignModel ??= new ColumnDesignModel();

                    switch (columnDesignModel.type)
                    {
                        case 4:
                            break;
                        case 3:
                        case 5:
                            // 分组表格、树形表格 没有分页
                            columnDesignModel.hasPage = false;
                            break;
                    }
                    break;
                case 5:
                    columnDesignModel = JsonConvert.DeserializeObject<ColumnDesignModel>(templateEntity.AppColumnData);
                    if (columnDesignModel != null)
                    {
                        var opt = JsonConvert.DeserializeObject<Dictionary<string, object>>(templateEntity.ColumnData);
                        if (opt.ContainsKey("columnOptions")) columnOptions = JsonConvert.SerializeObject(opt["columnOptions"]);
                    }
                    columnDesignModel ??= new ColumnDesignModel();
                    break;
            }

            if (columnOptions.IsNotEmptyOrNull())
            {
                var cList = new List<Dictionary<string, object>>();
                foreach (var it in JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(columnOptions))
                    if (!it["__config__"].ToString().Contains("\"ceriKey\": \"calculate\",")) cList.Add(it);
                columnOptions = JsonConvert.SerializeObject(cList);
            }

            // 发起表单 没有查询条件
            switch (templateEntity.Type)
            {
                case 3:
                    break;
                default:
                    if (templateEntity.WebType != 1)
                        controls = CodeGenUnifiedHandlerHelper.UnifiedHandlerFormDataModel(controls, columnDesignModel);
                    break;
            }

            // 联动关系链判断
            controls = CodeGenUnifiedHandlerHelper.LinkageChainJudgment(controls, columnDesignModel);

            /*
            *  PC 逻辑时： 行内编辑时 pc端需要循环子表日期控件
            *  APP 逻辑时：循环出除子表外全部开启千位符的数字输入控件字段
            */
            List<CodeGenSpecifyDateFormatSetModel> specifyDateFormatSet = new List<CodeGenSpecifyDateFormatSetModel>();
            var appThousandField = string.Empty;
            switch (logic)
            {
                case 4:
                    switch (columnDesignModel.type)
                    {
                        case 4:
                            foreach (var item in controls)
                            {
                                var config = item.__config__;
                                switch (config.ceriKey)
                                {
                                    case CeriKeyConst.TABLE:
                                        var model = CodeGenFormControlDesignHelper.CodeGenSpecifyDateFormatSetModel(item);
                                        if (model != null)
                                            specifyDateFormatSet.Add(model);
                                        break;
                                }
                            }
                            break;
                    }
                    break;
                case 5:
                    appThousandField = JsonConvert.SerializeObject(controls.FindAll(it => it.__config__.ceriKey.Equals(CeriKeyConst.NUMINPUT) && it.thousands).Select(it => it.__vModel__).ToList());
                    appThousandField = appThousandField == "[]" ? null : appThousandField;
                    break;
            }

            var FormScriptConfigModel = CodeGenFormControlDesignHelper.FormScriptDesign(formDataModel.fields, controls, string.Empty, formDataModel, columnDesignModel?.columnList, columnDesignModel.type, logic, hasFlow, true);

            // 列表顶部按钮
            var indexTopButton = new List<IndexButtonDesign>();

            // 列表行按钮
            var indexColumnButtonDesign = new List<IndexButtonDesign>();

            // 显示列字符串
            var columnList = string.Empty;

            // 查询条件字符串
            var searchList = string.Empty;

            // 高级查询字符串
            var superQueryJson = string.Empty;

            switch (templateEntity.Type)
            {
                case 3:
                    break;
                default:
                    switch (templateEntity.WebType)
                    {
                        case 1:
                            {
                                var pureFormBtnsList = columnDesignModel?.btnsList?.FindAll(it => it.value.Equals("add"));
                                if (pureFormBtnsList == null)
                                {
                                    pureFormBtnsList = new List<ButtonConfigModel>
                                {
                                    new ButtonConfigModel
                                    {
                                        icon = "icon-ym icon-ym-btn-add",
                                        label = "新增",
                                        value = "add"
                                    }
                                };
                                }

                                // 生成头部按钮信息
                                foreach (var item in pureFormBtnsList)
                                {
                                    indexTopButton.Add(new IndexButtonDesign()
                                    {
                                        Type = item.value.Equals("add") ? "primary" : "link",
                                        Icon = item.icon,
                                        Method = GetCodeGenIndexButtonHelper.IndexTopButton(item.value, importUrl, hasFlow, templateEntity.WebType),
                                        Value = item.value,
                                        Show = item.show,
                                        Label = item.labelI18nCode.IsNotEmptyOrNull() ? "{{t('" + item.labelI18nCode + "','" + item.label + "')}}" : item.label,
                                    });
                                }
                            }
                            break;
                        case 2:
                            columnList = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<Dictionary<string, object>>(templateEntity.ColumnData).GetValueOrDefault("columnList"));
                            searchList = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<Dictionary<string, object>>(templateEntity.ColumnData).GetValueOrDefault("searchList"));
                            superQueryJson = columnOptions;

                            // 生成头部按钮信息
                            foreach (var item in columnDesignModel?.btnsList ?? new List<ButtonConfigModel>())
                            {
                                indexTopButton.Add(new IndexButtonDesign()
                                {
                                    Type = item.value.Equals("add") ? "primary" : "link",
                                    Icon = item.icon,
                                    Method = GetCodeGenIndexButtonHelper.IndexTopButton(item.value, importUrl, hasFlow, templateEntity.WebType),
                                    Value = item.value,
                                    Show = item.show,
                                    Label = item.labelI18nCode.IsNotEmptyOrNull() ? "{{t('" + item.labelI18nCode + "','" + item.label + "')}}" : item.label,
                                });
                            }

                            // 生成行按钮信息
                            foreach (var item in columnDesignModel.columnBtnsList ?? new List<ButtonConfigModel>())
                            {
                                indexColumnButtonDesign.Add(new IndexButtonDesign()
                                {
                                    Type = item.value == "remove" ? "class='CERI-table-delBtn' " : string.Empty,
                                    Icon = item.icon,
                                    Method = GetCodeGenIndexButtonHelper.IndexColumnButton(item.value, tableColumns.Find(it => it.PrimaryKey.Equals(true))?.LowerColumnName, formDataModel.primaryKeyPolicy, templateEntity.EnableFlow == 1 ? true : false, columnDesignModel?.type == 4 ? true : false),
                                    Value = item.value,
                                    Label = item.labelI18nCode.IsNotEmptyOrNull() ? "t('" + item.labelI18nCode + "','" + item.label + "')" : string.Format("\"{0}\"", item.label),
                                    Show = item.show,
                                    Disabled = GetCodeGenIndexButtonHelper.WorkflowIndexColumnButton(item.value),
                                });
                            }

                            break;
                        case 4:
                            columnList = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<Dictionary<string, object>>(templateEntity.ColumnData).GetValueOrDefault("columnList"));
                            searchList = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<Dictionary<string, object>>(templateEntity.ColumnData).GetValueOrDefault("searchList"));
                            superQueryJson = columnOptions;
                            // 生成头部按钮信息
                            foreach (var item in columnDesignModel?.btnsList ?? new List<ButtonConfigModel>())
                            {
                                indexTopButton.Add(new IndexButtonDesign()
                                {
                                    Type = item.value.Equals("add") ? "primary" : "link",
                                    Icon = item.icon,
                                    Method = GetCodeGenIndexButtonHelper.IndexTopButton(item.value, importUrl, hasFlow, templateEntity.WebType, columnDesignModel.viewKey.IsNotEmptyOrNull()),
                                    Value = item.value,
                                    Show = item.show,
                                    Label = item.labelI18nCode.IsNotEmptyOrNull() ? "{{t('" + item.labelI18nCode + "','" + item.label + "')}}" : item.label,
                                });
                            }
                            break;
                    }

                    break;
            }

            var multipleQueryFields = GetMultiEndQueryMerging(templateEntity, controls);

            // 控件查询多选数组
            var controlQueryMultipleSelectionArray = new List<string>
        {
            CeriKeyConst.SELECT,
            CeriKeyConst.DEPSELECT,
            CeriKeyConst.ROLESELECT,
            CeriKeyConst.USERSELECT,
            CeriKeyConst.USERSSELECT,
            CeriKeyConst.COMSELECT,
            CeriKeyConst.POSSELECT,
            CeriKeyConst.GROUPSELECT,
        };

            // 查询条件查询差异列表
            var queryCriteriaQueryVarianceList = columnDesignModel?.searchList?.FindAll(it => controlQueryMultipleSelectionArray.Contains(it.__config__.ceriKey)).ToList().FindAll(it => !it.searchMultiple.Equals(multipleQueryFields.Find(x => x.__config__.ceriKey.Equals(it.__config__.ceriKey) && x.prop.Equals(it.prop)).searchMultiple));

            queryCriteriaQueryVarianceList?.ForEach(item =>
            {
                switch (item.__config__.isSubTable)
                {
                    case true:
                        item.__vModel__ = string.Format("{0}_{1}", item.__config__.parentVModel, item.__vModel__);
                        break;
                }
            });

            List<string> queryVarianceList = queryCriteriaQueryVarianceList?.Select(it => it.__vModel__).ToList();

            // 判断下左侧控件是否为组织选择控件 如果是的话从查询差异列表移除该字段
            switch (logic)
            {
                case 4:
                    if (controls.Any(it => it.__vModel__.Equals(columnDesignModel?.treeRelation) && it.__config__.ceriKey.Equals(CeriKeyConst.COMSELECT)))
                        queryVarianceList.Remove(columnDesignModel?.treeRelation);
                    break;
            }

            // 重置 ColumnData 参数
            switch (logic)
            {
                case 4:
                    columnDesignModel = JsonConvert.DeserializeObject<ColumnDesignModel>(templateEntity.ColumnData);
                    columnDesignModel ??= new ColumnDesignModel();
                    switch (columnDesignModel.type)
                    {
                        case 4:
                            break;
                        case 3:
                        case 5:
                            // 分组表格、树形表格 没有分页
                            columnDesignModel.hasPage = false;
                            break;
                    }
                    break;
                case 5:
                    columnDesignModel = JsonConvert.DeserializeObject<ColumnDesignModel>(templateEntity.AppColumnData); 
                    columnDesignModel ??= new ColumnDesignModel();
                    break;
            }

            // 主键字段
            var primaryKeyField = tableColumns?.Find(it => it.PrimaryKey.Equals(true))?.LowerColumnName;

            // 是否存在查询
            var hasSearch = columnDesignModel?.searchList?.Any();

            // 是否存在子表
            var hasChildTable = controls.Any(it => it.__config__.ceriKey.Equals(CeriKeyConst.TABLE));

            // 是否开启`新增`
            var hasAdd = indexTopButton.Any(it => it.Value.Equals("add") && it.Show);

            // 是否开启`导出`
            var hasDownload = indexTopButton.Any(it => it.Value.Equals("download") && it.Show);

            // 是否开启`导入`
            var hasUpload = indexTopButton.Any(it => it.Value.Equals("upload") && it.Show);

            // 是否开启`批量删除`
            var hasBatchRemove = indexTopButton.Any(it => it.Value.Equals("batchRemove") && it.Show);

            // 是否开启`批量打印`
            var hasBatchPrint = indexTopButton.Any(it => it.Value.Equals("batchPrint") && it.Show);

            // 批量打印字段列表
            var batchPrints = JsonConvert.SerializeObject(columnDesignModel?.printIds);

            // 是否开启`编辑`
            var hasEdit = indexColumnButtonDesign.Any(it => it.Value.Equals("edit") && it.Show);

            // 是否开启`删除`
            var hasRemove = indexColumnButtonDesign.Any(it => it.Value.Equals("remove") && it.Show);

            // 是否开启`详情`
            var hasDetail = indexColumnButtonDesign.Any(it => it.Value.Equals("detail") && it.Show);

            // 是否开启`关联表单详情`
            var hasRelationDetail = controls.Any(it => it.__config__.ceriKey.Equals(CeriKeyConst.RELATIONFORM));

            // 是否开启子表`关联表单详情`
            var hasSubTableRelationDetail = controls.Any(it => it.__config__.ceriKey.Equals(CeriKeyConst.TABLE) && it.__config__.children.Any(c => c.__config__.ceriKey.Equals(CeriKeyConst.RELATIONFORM)));

            // 是否开启数组输入千位符
            var hasThousands = controls.Any(it => it.__config__.ceriKey.Equals(CeriKeyConst.NUMINPUT) && it.thousands);

            // 是否开启子表数组输入千位符
            var hasSubTableThousands = controls.Any(it => it.__config__.ceriKey.Equals(CeriKeyConst.TABLE) && it.__config__.children.Any(c => c.__config__.ceriKey.Equals(CeriKeyConst.NUMINPUT) && c.thousands));

            // 左侧树控件
            IndexSearchFieldModel? treeControl = new IndexSearchFieldModel();

            // 左侧树配置
            CodeGenFrontEndLeftTreeFieldDesignModel leftTree = new CodeGenFrontEndLeftTreeFieldDesignModel();

            // PC端
            switch (logic)
            {
                case 4:
                    {
                        // 列表布局为：左侧树
                        switch (columnDesignModel.type)
                        {
                            case 2:
                                {
                                    treeControl = multipleQueryFields?.Find(it => it.id.Equals(columnDesignModel?.treeRelation));
                                    leftTree = new CodeGenFrontEndLeftTreeFieldDesignModel
                                    {
                                        ceriKey = treeControl.__config__.ceriKey,
                                        HasSearch = (columnDesignModel?.hasTreeQuery).ParseToBool(),
                                        TreeSyncType = columnDesignModel.treeSyncType,
                                        TreeRelationFieldSelectType = columnDesignModel.treeRelationFieldSelectType,
                                        Title = logic.Equals(4) && columnDesignModel != null && columnDesignModel.treeTitleI18nCode.IsNotEmptyOrNull() ? $"t('{columnDesignModel?.treeTitleI18nCode}', '{columnDesignModel?.treeTitle}')" : string.Format("'{0}'", columnDesignModel?.treeTitle),
                                        TreeDataSource = columnDesignModel?.treeDataSource,
                                        TreeDictionary = columnDesignModel?.treeDictionary,
                                        TreeInterfaceId = columnDesignModel?.treeSyncInterfaceId,
                                        TreePropsUrl = columnDesignModel?.treePropsUrl,
                                        Key = columnDesignModel?.treePropsValue,
                                        ShowField = columnDesignModel?.treePropsLabel,
                                        Children = columnDesignModel?.treePropsChildren,
                                        TreeRelation = columnDesignModel?.treeRelation,
                                        TreeTemplateJson = JsonConvert.SerializeObject(columnDesignModel?.treeSyncTemplateJson),
                                        TemplateJson = JsonConvert.SerializeObject(columnDesignModel?.treeTemplateJson),
                                        IsMultiple = (treeControl?.searchMultiple).ParseToBool()
                                    };
                                }

                                break;
                        }
                    }

                    break;
            }

            // 表格配置
            var tableConfig = new CodeGenFrontEndTableConfigModel()
            {
                HasSuperQuery = (columnDesignModel?.hasSuperQuery).ParseToBool(),
                HasPage = (columnDesignModel?.hasPage).ParseToBool(),
                PageSize = columnDesignModel.pageSize,
                DefaultSortConfig = columnDesignModel.defaultSortConfig != null ? JsonConvert.SerializeObject(columnDesignModel.defaultSortConfig) : "[]",
                ShowSummary = (columnDesignModel?.showSummary).ParseToBool(),
                ChildTableStyle = columnDesignModel.childTableStyle,
                Sort = columnDesignModel?.sort,
                Sidx = columnDesignModel?.defaultSidx,
                SummaryField = JsonConvert.SerializeObject(columnDesignModel?.summaryField),
                GroupField = columnDesignModel?.groupField,
                ShowOverflow = columnDesignModel.showOverflow,
            };

            // 表单属性
            var formAttribute = new CodeGenFrontEndFormAttributeModel()
            {
                Size = formDataModel.size,
                LabelPosition = formDataModel.labelPosition,
                LabelWidth = formDataModel.labelWidth,
                PopupType = templateEntity.WebType == 1 ? "fullScreen" : formDataModel.popupType,
                Gutter = formDataModel.gutter,
                FormStyle = formDataModel.formStyle,
                CancelButtonText = formDataModel.cancelButtonText,
                ConfirmButtonText = formDataModel.confirmButtonText,
                GeneralWidth = formDataModel.generalWidth,
                FullScreenWidth = formDataModel.fullScreenWidth,
                DrawerWidth = formDataModel.drawerWidth,
                PrimaryKeyPolicy = formDataModel.primaryKeyPolicy,
                HasPrintBtn = formDataModel.hasPrintBtn,
                PrintButtonText = formDataModel.printButtonText,
                PrintId = JsonConvert.SerializeObject(formDataModel.printId),
            };

            var con = CodeGenFormControlDesignHelper.GetPropertyJson(controls);
            var basicInfo = new CodeGenBasicInfoAttributeModel();


            basicInfo.Id = templateEntity.Id;
            basicInfo.FullName = templateEntity.FullName;
            basicInfo.EnCode = templateEntity.EnCode;
            basicInfo.Category = templateEntity.Category;
            basicInfo.PropertyJson = JsonConvert.SerializeObject(con);
            basicInfo.TableJson = JsonConvert.SerializeObject(templateEntity.Tables);
            basicInfo.CreatorTime = DateTime.Now.ParseToUnixTime();
            basicInfo.DbLinkId = templateEntity.DbLinkId;


            switch (templateEntity.WebType)
            {
                default:
                    result = new CodeGenFrontEndConfigModel()
                    {
                        NameSpace = formDataModel.areasName,
                        ClassName = formDataModel.className.FirstOrDefault(),
                        WebType = templateEntity.Type == 3 ? templateEntity.Type : templateEntity.WebType,
                        Type = columnDesignModel.type,
                        Title = templateEntity.FullName,
                        HasSearch = hasSearch.ParseToBool(),
                        HasChildTable = hasChildTable,
                        LeftTree = leftTree,
                        TableConfig = tableConfig,
                        TopButtonDesign = indexTopButton,
                        ColumnButtonDesign = indexColumnButtonDesign,
                        ColumnButtonDesignAny = indexColumnButtonDesign.Any(x => x.Show),
                        HasFlow = hasFlow,
                        HasAdd = hasAdd,
                        HasDownload = hasDownload,
                        HasUpload = hasUpload,
                        HasBatchRemove = hasBatchRemove,
                        HasBatchPrint = hasBatchPrint,
                        BatchPrints = batchPrints,
                        HasEdit = hasEdit,
                        HasRemove = hasRemove,
                        HasDetail = hasDetail,
                        HasRelationDetail = hasRelationDetail,
                        HasSubTableRelationDetail = hasSubTableRelationDetail,
                        HasThousands = hasThousands,
                        HasSubTableThousands = hasSubTableThousands,
                        UseBtnPermission = (columnDesignModel?.useBtnPermission).ParseToBool(),
                        UseColumnPermission = (columnDesignModel?.useColumnPermission).ParseToBool(),
                        UseFormPermission = (columnDesignModel?.useFormPermission).ParseToBool(),
                        PrimaryKeyField = primaryKeyField,
                        FormAttribute = formAttribute,
                        BasicInfo = basicInfo,
                        ColumnList = columnList,
                        SearchList = searchList,
                        SuperQueryJson = superQueryJson,
                        FormScript = FormScriptConfigModel,
                        QueryCriteriaQueryVarianceList = queryVarianceList,
                        ComplexColumns = columnDesignModel.complexHeaderList != null && columnDesignModel.complexHeaderList.Any() ? JsonConvert.SerializeObject(columnDesignModel.complexHeaderList) : "[]",
                        HasConfirmAndAddBtn = formDataModel.hasConfirmAndAddBtn,
                    };
                    break;
            }

            // 标签面板
            var columnData = logic.Equals(4) ? JsonConvert.DeserializeObject<ColumnDesignModel>(templateEntity.ColumnData) : JsonConvert.DeserializeObject<ColumnDesignModel>(templateEntity.AppColumnData);
            if (columnData != null && columnData.tabConfig != null && columnData.tabConfig.on && columnData.tabConfig.relationField.IsNotEmptyOrNull() && (columnData.type.Equals(1) || columnData.type.Equals(4)))
            {
                var relationField = controls.Find(x => x.__vModel__.Equals(columnData.tabConfig.relationField));
                if (relationField != null)
                {
                    if (columnData.tabConfig.hasAllTab)
                        result.TabConfigHasAllTab = true;
                    result.TabConfigDataType = relationField.__config__.dataType;
                    if (relationField.__config__.dataType.Equals("static"))
                    {
                        var tabConfig = new List<Dictionary<string, object>>();
                        relationField.options.ForEach(it => tabConfig.Add(new Dictionary<string, object>() { { "id", it["id"] }, { "fullName", it["fullName"] } }));
                        result.TabDataSource = JsonConvert.SerializeObject(tabConfig);
                    }
                    else if (relationField.__config__.dataType.Equals("dictionary"))
                    {
                        result.TabDataSource = relationField.__config__.dictionaryType;
                        result.TabDictionaryType = relationField.props.value;
                    }

                    result.IsTabConfig = true;
                    result.TabRelationField = columnData.tabConfig.relationField;
                }
            }

            return result;
        }


        /// <summary>
        /// 代码生成配置模型.
        /// </summary>
        /// <param name="formDataModel">表单Json包.</param>
        /// <param name="columnDesignModel">列设计模型.</param>
        /// <param name="tableColumnList">数据库表列.</param>
        /// <param name="controls">表单控件列表.</param>
        /// <param name="tableName">表名称.</param>
        /// <param name="templateEntity">模板实体.</param>
        /// <returns></returns>
        public static CodeGenConfigModel GetCodeGenConfigModel(FormDataModel formDataModel, ColumnDesignModel columnDesignModel, List<TableColumnConfigModel> tableColumnList, List<FieldsModel> controls, string tableName, VisualDevEntity templateEntity)
        {
            // 标签面板
            if (columnDesignModel.tabConfig != null && columnDesignModel.tabConfig.on)
            {
                var field = tableColumnList.Find(x => x.OriginalColumnName.Equals(columnDesignModel.tabConfig.relationField));
                if (field != null)
                {
                    field.QueryType = 1;
                    field.QueryWhether = true;
                }
            }

            //var appColumnDesignModel = JsonConvert.DeserializeObject<ColumnDesignModel>(templateEntity.AppColumnData);
            var appColumnDesignModel = new ColumnDesignModel();
            //if (appColumnDesignModel != null)
            if (appColumnDesignModel.printIds != null)

            {
                foreach (var btnItem in appColumnDesignModel.btnsList?.Where(x => x.show).ToList())
                {
                    columnDesignModel.btnsList.FirstOrDefault(xx => xx.value.Equals(btnItem.value)).show = true;
                }

                // app标签面板
                if (appColumnDesignModel.tabConfig != null && appColumnDesignModel.tabConfig.on)
                {
                    var field = tableColumnList.Find(x => x.OriginalColumnName.Equals(appColumnDesignModel.tabConfig.relationField));
                    if (field != null)
                    {
                        field.QueryType = 1;
                        field.QueryWhether = true;
                    }
                }
            }

            // 默认排序 没设置以ID排序.
            var defaultSidx = string.Empty;

            // 是否导出
            bool isExport = false;

            // 是否批量删除
            bool isBatchRemove = false;

            // 是否查询条件多选
            bool isSearchMultiple = false;

            // 是否树形表格
            bool isTreeTable = false;

            // 树形表格-父级字段
            string parentField = string.Empty;

            // 树形表格-显示字段
            string treeShowField = string.Empty;

            switch (templateEntity.WebType)
            {
                case 2:
                    // 默认排序 没设置以ID排序.
                    defaultSidx = columnDesignModel.defaultSidx ?? tableColumnList.Find(t => t.PrimaryKey).ColumnName;
                    isExport = columnDesignModel.btnsList.Any(it => it.value == "download");
                    isBatchRemove = columnDesignModel.btnsList.Any(it => it.value == "batchRemove");
                    isSearchMultiple = tableColumnList.Any(it => it.QueryMultiple && !it.IsAuxiliary);
                    break;
            }

            switch (columnDesignModel.type)
            {
                case 5:
                    isTreeTable = true;
                    parentField = string.Format("{0}_pid", columnDesignModel.parentField);
                    treeShowField = columnDesignModel.columnList.Find(it => it.__vModel__.ToLower() != columnDesignModel.parentField.ToLower()).__vModel__;
                    break;
                default:
                    break;
            }

            // 是否存在上传
            bool isUpload = tableColumnList.Any(it => it.ceriKey != null && (it.ceriKey.Equals(CeriKeyConst.UPLOADIMG) || it.ceriKey.Equals(CeriKeyConst.UPLOADFZ)));

            // 是否对象映射
            bool isMapper = tableColumnList.Any(it => it.ceriKey != null && (it.ceriKey.Equals(CeriKeyConst.CHECKBOX) || it.ceriKey.Equals(CeriKeyConst.CASCADER) || it.ceriKey.Equals(CeriKeyConst.ADDRESS) || it.ceriKey.Equals(CeriKeyConst.COMSELECT) || it.ceriKey.Equals(CeriKeyConst.UPLOADIMG) || it.ceriKey.Equals(CeriKeyConst.UPLOADFZ) || (it.ceriKey.Equals(CeriKeyConst.SELECT) && it.IsMultiple) || (it.ceriKey.Equals(CeriKeyConst.USERSELECT) && it.IsMultiple) || (it.ceriKey.Equals(CeriKeyConst.TREESELECT) && it.IsMultiple) || (it.ceriKey.Equals(CeriKeyConst.DEPSELECT) && it.IsMultiple) || (it.ceriKey.Equals(CeriKeyConst.POSSELECT) && it.IsMultiple)));

            // 是否存在单据规则控件
            bool isBillRule = controls.Any(it => it.__config__.ceriKey.Equals(CeriKeyConst.BILLRULE));
            if (!isBillRule && controls.Any(it => it.__config__.ceriKey.Equals(CeriKeyConst.TABLE)))
            {
                foreach (var item in controls.Where(x => x.__config__.ceriKey.Equals(CeriKeyConst.TABLE)).ToList())
                {
                    if (item.__config__.children.Any(it => it.__config__.ceriKey.Equals(CeriKeyConst.BILLRULE)))
                    {
                        isBillRule = true;
                        break;
                    }
                }
            }

            // 条形码关联字段
            var relationField = new List<TableColumnConfigModel>();
            foreach (var item in controls.FindAll(x => x.__config__.ceriKey.Equals(CeriKeyConst.BARCODE) && x.dataType.Equals("relation")))
            {
                if (item.relationField.Contains("_ceri_"))
                {
                    var rField = item.relationField.Split("jnpf_").ToList();
                    rField.RemoveAt(0);
                    var tt = rField.FirstOrDefault().ReplaceRegex("^f_", string.Empty).ParseToPascalCase();
                    var ff = rField.LastOrDefault().ReplaceRegex("^f_", string.Empty).ParseToPascalCase();
                    relationField.Add(new TableColumnConfigModel() { ColumnName = string.Format("{0}.{1}", tt, ff), RelationColumnName = item.relationField + "_id" });
                }
                else
                {
                    relationField.Add(new TableColumnConfigModel() { ColumnName = item.relationField.ParseToPascalCase(), RelationColumnName = item.relationField + "_id" });
                }
            }

            bool isSystemControl = tableColumnList.Any(it => it.IsSystemControl);

            bool isUpdate = tableColumnList.Any(it => it.IsUpdate);

            bool isLogicalDelete = formDataModel.logicalDelete;

            List<CodeGenFunctionModel> function = new List<CodeGenFunctionModel>();

            switch (templateEntity.Type)
            {
                case 3:
                    function = CodeGenFunctionHelper.GetPureFormWithProcessMethod();
                    break;
                default:
                    // 是否启用流程
                    switch (templateEntity.EnableFlow)
                    {
                        case 1:
                            switch (templateEntity.WebType)
                            {
                                case 1:
                                    function = CodeGenFunctionHelper.GetPureFormWithProcessMethod();
                                    break;
                                case 2:
                                    columnDesignModel.btnsList.RemoveAll(it => it.value.Equals("add"));
                                    columnDesignModel.btnsList.Add(new ButtonConfigModel()
                                    {
                                        value = "save",
                                    });
                                    columnDesignModel.columnBtnsList.RemoveAll(it => it.value.Equals("edit"));
                                    function = CodeGenFunctionHelper.GetGeneralListWithProcessMethod(columnDesignModel.hasPage, columnDesignModel.btnsList, columnDesignModel.columnBtnsList);
                                    break;
                            }

                            break;
                        default:
                            switch (templateEntity.WebType)
                            {
                                case 1:
                                    function = CodeGenFunctionHelper.GetPureFormMethod();
                                    break;
                                default:
                                    function = CodeGenFunctionHelper.GetGeneralListMethod(columnDesignModel.hasPage, columnDesignModel.btnsList, columnDesignModel.columnBtnsList);
                                    break;
                            }
                            break;
                    }
                    break;
            }

            // 树形表格不管有没有导出 强行开双列表(分页与无分页接口)
            switch (columnDesignModel.type)
            {
                case 5:
                    switch (function.Any(it => it.FullName.Equals("page") && it.FullName.Equals("noPage")))
                    {
                        case false:
                            switch (function.Any(it => it.FullName.Equals("page")))
                            {
                                case true:
                                    if (!function.Any(x => x.FullName.Equals("noPage")))
                                    {
                                        function.Add(new CodeGenFunctionModel()
                                        {
                                            FullName = "noPage",
                                            IsInterface = true,
                                            orderBy = 3,
                                        });
                                    }
                                    break;
                                default:
                                    if (!function.Any(x => x.FullName.Equals("page")))
                                    {
                                        function.Add(new CodeGenFunctionModel()
                                        {
                                            FullName = "page",
                                            IsInterface = true,
                                            orderBy = 3,
                                        });
                                    }
                                    break;
                            }
                            function.FindAll(it => it.FullName.Equals("page") || it.FullName.Equals("noPage")).ForEach(item =>
                            {
                                item.IsInterface = true;
                            });
                            break;
                        case true:
                            function.FindAll(it => it.FullName.Equals("page") || it.FullName.Equals("noPage")).ForEach(item =>
                            {
                                item.IsInterface = true;
                            });
                            break;
                    }
                    break;
            }

            if (columnDesignModel.type.Equals(3) || columnDesignModel.type.Equals(5)) columnDesignModel.complexHeaderList.Clear();
            var columnList = JsonConvert.DeserializeObject<List<IndexGridFieldModel>>(JsonConvert.SerializeObject(columnDesignModel.columnList));
            if (columnDesignModel.type.Equals(4)) columnList = columnList.Where(x => !x.id.ToLower().Contains("tablefield")).ToList();

            foreach (var columnItem in tableColumnList.Where(x => x.ReColumnName.IsNotEmptyOrNull()))
            {
                var lowerColumn = columnItem.LowerColumnName;
                columnItem.ColumnName = columnItem.ReColumnName;
                columnItem.LowerColumnName = lowerColumn;
            }

            // 业务主键 条件拼接
            var businessKeyList = new List<string>();
            foreach (var column in tableColumnList.Where(x => x.IsBusinessKey))
            {
                if (column.IsAuxiliary)
                {
                    businessKeyList.Add(string.Format("it.{0}.{1} == auxiliay{0}.{1}", column.ClassName, column.ColumnName));
                }
                else
                {
                    businessKeyList.Add(string.Format("it.{0} == entity.{0}", column.ColumnName));
                }
            }

            return new CodeGenConfigModel()
            {
                NameSpace = formDataModel.areasName,
                BusName = templateEntity.FullName,
                ClassName = formDataModel.className.FirstOrDefault(),
                PrimaryKey = tableColumnList.Find(t => t.PrimaryKey).ColumnName,
                OriginalPrimaryKey = tableColumnList.Find(t => t.PrimaryKey).OriginalColumnName,
                MainTable = tableName.ParseToPascalCase(),
                OriginalMainTableName = tableName,
                hasPage = columnDesignModel.hasPage,
                Function = function,
                TableField = tableColumnList,
                RelationsField = relationField,
                DefaultSidx = defaultSidx,
                IsExport = isExport,
                IsBatchRemove = isBatchRemove,
                IsUpload = isUpload,
                IsTableRelations = false,
                IsMapper = isMapper,
                IsBillRule = isBillRule,
                DbLinkId = templateEntity.DbLinkId,
                FormId = templateEntity.Id,
                WebType = templateEntity.WebType,
                Type = templateEntity.Type,
                EnableFlow = templateEntity.EnableFlow.ParseToBool(),
                IsMainTable = true,
                EnCode = templateEntity.EnCode,
                UseDataPermission = (bool)columnDesignModel?.useDataPermission,
                SearchControlNum = tableColumnList.FindAll(it => it.QueryType.Equals(1) || it.QueryType.Equals(2)).Count(),
                IsAuxiliaryTable = false,
                ExportField = templateEntity.Type == 3 || templateEntity.WebType == 1 ? null : CodeGenExportFieldHelper.ExportColumnField(columnList, columnDesignModel.complexHeaderList),
                FullName = templateEntity.FullName,
                IsConversion = tableColumnList.Any(it => it.IsConversion.Equals(true)),
                IsDetailConversion = tableColumnList.Any(it => it.IsDetailConversion.Equals(true)),
                PrimaryKeyPolicy = formDataModel.primaryKeyPolicy,
                ConcurrencyLock = formDataModel.concurrencyLock,
                HasSuperQuery = columnDesignModel.hasSuperQuery,
                IsUnique = tableColumnList.Any(it => it.IsUnique),
                BusinessKeyList = string.Join(" && ", businessKeyList),
                GroupField = columnDesignModel?.groupField,
                GroupShowField = columnDesignModel?.columnList?.Where(x => x.__vModel__.ToLower() != columnDesignModel?.groupField?.ToLower()).FirstOrDefault()?.__vModel__,
                IsImportData = tableColumnList.Any(it => it.IsImportField.Equals(true)),
                ParsCeriKeyConstList = CodeGenControlsAttributeHelper.GetParsCeriKeyConstList(controls, (bool)columnDesignModel?.type.Equals(4)),
                ParsCeriKeyConstListDetails = CodeGenControlsAttributeHelper.GetParsCeriKeyConstListDetails(controls),
                ImportDataType = columnDesignModel?.uploaderTemplateJson?.dataType,
                IsSystemControl = isSystemControl,
                IsUpdate = isUpdate,
                IsSearchMultiple = isSearchMultiple,
                IsTreeTable = isTreeTable,
                ParentField = parentField,
                TreeShowField = treeShowField,
                IsLogicalDelete = isLogicalDelete,
                TableType = columnDesignModel.type,
            };
        }

        private static bool IsImportField(int WebType, ColumnDesignModel columnDesignModel, string field, string ceriKey)
        {
            bool? res = false;

            if (!WebType.Equals(1))
            {
                res = columnDesignModel?.uploaderTemplateJson?.selectKey?.Any(it => it.Equals(field));

                // 系统自动生成控件
                if (ceriKey.Equals(CeriKeyConst.BILLRULE) || ceriKey.Equals(CeriKeyConst.MODIFYUSER) || ceriKey.Equals(CeriKeyConst.CREATEUSER) || ceriKey.Equals(CeriKeyConst.MODIFYTIME) ||
                    ceriKey.Equals(CeriKeyConst.CREATETIME) || ceriKey.Equals(CeriKeyConst.CURRPOSITION) || ceriKey.Equals(CeriKeyConst.CURRORGANIZE)) res = true;
            }

            return res.ParseToBool();
        }

        /// <summary>
        /// 多端列表展示合并.
        /// </summary>
        /// <param name="templateEntity">模板实体.</param>
        /// <returns></returns>
        public static List<IndexGridFieldModel> GetMultiTerminalListDisplayAndConsolidation(VisualDevEntity templateEntity)
        {
            ColumnDesignModel pcColumnDesignModel = JsonConvert.DeserializeObject<ColumnDesignModel>(templateEntity.ColumnData);
            ColumnDesignModel appColumnDesignModel = JsonConvert.DeserializeObject<ColumnDesignModel>(templateEntity.AppColumnData);
            return pcColumnDesignModel?.columnList?.Union(appColumnDesignModel?.columnList, EqualityHelper<IndexGridFieldModel>.CreateComparer(it => it.prop)).ToList();
        }

        /// <summary>
        /// 多端查询合并.
        /// </summary>
        /// <param name="templateEntity">模板实体.</param>
        /// <param name="controls">移除布局演示后的表单全控件.</param>
        /// <param name="logic">生成逻辑;4-pc,5-app.</param>
        /// <returns></returns>
        public static List<IndexSearchFieldModel> GetMultiEndQueryMerging(VisualDevEntity templateEntity, List<FieldsModel> controls = null, int logic = 4)
        {
            ColumnDesignModel pcColumnDesignModel = JsonConvert.DeserializeObject<ColumnDesignModel>(templateEntity.ColumnData);
            ColumnDesignModel appColumnDesignModel = JsonConvert.DeserializeObject<ColumnDesignModel>(templateEntity.AppColumnData);

            // 当查询列表内没有选中某个字段 该字段又被作为左侧树查询字段 读取表单内该字段的多选属性 查询多选全由查询列表配置 
            if (templateEntity.Type != 3 && controls != null && pcColumnDesignModel?.type == 2 && (pcColumnDesignModel.searchList.Count == 0 || !pcColumnDesignModel.searchList.Any(it => it.prop.Equals(pcColumnDesignModel.treeRelation))))
            {
                var search = new IndexGridFieldModel();

                // 读取app查询列表 再读取表单内字段
                switch (appColumnDesignModel.searchList.Any(it => it.prop.Equals(pcColumnDesignModel.treeRelation)))
                {
                    case true:
                        pcColumnDesignModel.searchList.Add(appColumnDesignModel.searchList.Find(it => it.prop.Equals(pcColumnDesignModel.treeRelation)));
                        break;
                    default:
                        search = pcColumnDesignModel.columnOptions.Find(it => it.id.Equals(pcColumnDesignModel.treeRelation));
                        var newobj = new IndexSearchFieldModel()
                        {
                            ableIds = search.ableIds,
                            ableRelationIds = search.ableRelationIds,
                            accept = search.accept,
                            accordion = search.accordion,
                            activecolor = search.activecolor,
                            activeTxt = search.activeTxt,
                            activeValue = search.activeValue,
                            addonAfter = search.addonAfter,
                            addonBefore = search.addonBefore,
                            addType = search.addType,
                            adjustmentScope = search.adjustmentScope,
                            align = search.align,
                            allowHalf = search.allowHalf,
                            autoLocation = search.autoLocation,
                            autoSize = search.autoSize,
                            background = search.background,
                            border = search.border,
                            borderColor = search.borderColor,
                            borderType = search.borderType,
                            borderWidth = search.borderWidth,
                            buttonStyle = search.buttonStyle,
                            buttonText = search.buttonText,
                            buttonTextI18nCode = search.buttonTextI18nCode,
                            clearable = search.clearable,
                            closable = search.closable,
                            closeText = search.closeText,
                            closeTextI18nCode = search.closeTextI18nCode,
                            color = search.color,
                            colorDark = search.colorDark,
                            colorFormat = search.colorFormat,
                            colorLight = search.colorLight,
                            columnBtnsList = search.columnBtnsList,
                            columnOptions = search.columnOptions,
                            content = search.content,
                            contentI18nCode = search.contentI18nCode,
                            contentPosition = search.contentPosition,
                            controls = search.controls,
                            controlsPosition = search.controlsPosition,
                            count = search.count,
                            dataType = search.dataType,
                            defaultExpandAll = search.defaultExpandAll,
                            description = search.description,
                            descriptionI18nCode = search.descriptionI18nCode,
                            direction = search.direction,
                            disaabled = search.disaabled,
                            disabled = search.disabled,
                            enableDesktopLocation = search.enableDesktopLocation,
                            enableLocationScope = search.enableLocationScope,
                            endplaceholder = search.endplaceholder,
                            extraOptions = search.extraOptions,
                            fileSize = search.fileSize,
                            filterable = search.filterable,
                            flowId = search.flowId,
                            folder = search.folder,
                            fontStyle = search.fontStyle,
                            footerBtnsList = search.footerBtnsList,
                            format = search.format,
                            hasPage = search.hasPage,
                            header = search.header,
                            headerI18nCode = search.headerI18nCode,
                            height = search.height,
                            helpMessage = search.helpMessage,
                            helpMessageI18nCode = search.helpMessageI18nCode,
                            href = search.href,
                            icon = search.icon,
                            id = search.id,
                            inactivecolor = search.inactivecolor,
                            inactiveTxt = search.inactiveTxt,
                            inactiveValue = search.inactiveValue,
                            interfaceId = search.interfaceId,
                            isAccount = search.isAccount,
                            isAmountChinese = search.isAmountChinese,
                            isIndexShow = search.isIndexShow,
                            isInvoke = search.isInvoke,
                            isKeyword = search.isKeyword,
                            IsLinkage = search.IsLinkage,
                            IsLinked = search.IsLinked,
                            isQueryField = search.isQueryField,
                            isrange = search.isrange,
                            isStorage = search.isStorage,
                            ceriKey = search.ceriKey,
                            label = search.label,
                            labelI18nCode = search.labelI18nCode,
                            layoutType = search.layoutType,
                            level = search.level,
                            limit = search.limit,
                            lineColor = search.lineColor,
                            locationScope = search.locationScope,
                            maskConfig = search.maskConfig,
                            max = search.max,
                            maxlength = search.maxlength,
                            min = search.min,
                            modelId = search.modelId,
                            multiple = search.multiple,
                            name = search.name,
                            options = search.options,
                            optionType = search.optionType,
                            pageSize = search.pageSize,
                            pathType = search.pathType,
                            pickeroptions = search.pickeroptions,
                            placeholder = search.placeholder,
                            placeholderI18nCode = search.placeholderI18nCode,
                            isIncludeSubordinate = false,
                            noShow = false,
                            popupTitle = search.popupTitle,
                            popupType = search.popupType,
                            popupWidth = search.popupWidth,
                            precision = search.precision,
                            prefixIcon = search.prefixIcon,
                            processStatus = search.processStatus,
                            prop = search.prop,
                            props = search.props,
                            propsValue = search.propsValue,
                            queryType = search.queryType,
                            range = search.range,
                            rangeseparator = search.rangeseparator,
                            @readonly = search.@readonly,
                            relational = search.relational,
                            relationField = search.relationField,
                            roundType = search.roundType,
                            searchMultiple = search.searchMultiple,
                            searchType = search.searchType,
                            selectType = search.selectType,
                            separator = search.separator,
                            shadow = search.shadow,
                            showAllLevels = search.showAllLevels,
                            showAlpha = search.showAlpha,
                            showChinese = search.showChinese,
                            showCount = search.showCount,
                            showField = search.showField,
                            showIcon = search.showIcon,
                            showLevel = search.showLevel,
                            showPassword = search.showPassword,
                            showScore = search.showScore,
                            showstops = search.showstops,
                            showSummary = search.showSummary,
                            showtext = search.showtext,
                            showTip = search.showTip,
                            showWordlimit = search.showWordlimit,
                            simple = search.simple,
                            size = search.size,
                            sizeUnit = search.sizeUnit,
                            sortRule = search.sortRule,
                            startplaceholder = search.startplaceholder,
                            staticText = search.staticText,
                            step = search.step,
                            stepstrictly = search.stepstrictly,
                            style = search.style,
                            suffixIcon = search.suffixIcon,
                            summaryField = search.summaryField,
                            superiorVModel = search.superiorVModel,
                            TablePrimaryKey = search.TablePrimaryKey,
                            tabPosition = search.tabPosition,
                            target = search.target,
                            templateJson = search.templateJson,
                            textStyle = search.textStyle,
                            thousands = search.thousands,
                            timeFormat = search.timeFormat,
                            tipLabel = search.tipLabel,
                            tipText = search.tipText,
                            title = search.title,
                            titleI18nCode = search.titleI18nCode,
                            total = search.total,
                            type = search.type,
                            useMask = search.useMask,
                            useScan = search.useScan,
                            value = null,
                            valueformat = search.valueformat,
                            __vModel__ = search.__vModel__,
                            __config__ = search.__config__,
                            __slot__ = search.__slot__,
                            width = search.width.Value
                        };
                        var treeRelation = newobj; //search?.Adapt<IndexSearchFieldModel>();
                        treeRelation.multiple = false;
                        treeRelation.label = search.__config__.label;
                        treeRelation.prop = search.__vModel__;
                        treeRelation.searchType = 1;
                        pcColumnDesignModel.searchList.Add(newobj);
                        break;
                }
            }

            List<IndexSearchFieldModel>? newSearchList = new List<IndexSearchFieldModel>();

            switch (logic)
            {
                case 4:
                    newSearchList = pcColumnDesignModel?.searchList;
                    //newSearchList = pcColumnDesignModel?.searchList?.Union(appColumnDesignModel?.searchList, EqualityHelper<IndexSearchFieldModel>.CreateComparer(it => it.id)).ToList();
                    break;
                case 5:
                    newSearchList = appColumnDesignModel?.searchList?.Union(pcColumnDesignModel?.searchList, EqualityHelper<IndexSearchFieldModel>.CreateComparer(it => it.id)).ToList();
                    break;
            }
            newSearchList?.ForEach(item =>
            {
                var config = item.__config__;
                switch (config.ceriKey)
                {
                    case CeriKeyConst.SELECT:
                    case CeriKeyConst.DEPSELECT:
                    case CeriKeyConst.ROLESELECT:
                    case CeriKeyConst.USERSELECT:
                    case CeriKeyConst.USERSSELECT:
                    case CeriKeyConst.COMSELECT:
                    case CeriKeyConst.POSSELECT:
                    case CeriKeyConst.GROUPSELECT:
                        var pc = (pcColumnDesignModel?.searchList.Find(it => it.id.Equals(item.id))?.searchMultiple).ParseToBool();
                        //var app = (appColumnDesignModel?.searchList.Find(it => it.prop.Equals(item.prop))?.searchMultiple).ParseToBool();
                        var app = false;
                        if (pc && app)
                            item.searchMultiple = true;
                        if (!pc && !app)
                            item.searchMultiple = false;
                        else if (pc || app)
                            item.searchMultiple = true;
                        else
                            item.searchMultiple = false;
                        break;
                }
            });
            return newSearchList;
        }
    }
}
