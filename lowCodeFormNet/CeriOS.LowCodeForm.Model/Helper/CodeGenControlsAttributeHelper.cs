
using CeriOS.LowCodeForm.Model.ViewModel.LowCode;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;
using SqlSugar;
using CeriOS.LowCodeForm.Model.Model.Authorize;
using CeriOS.LowCodeForm.Model.ViewModel;
using NPOI.Util;
using CERIOS.Common.Extension;
using CERIOS.Engine.Entity.Model;
using CERIOS.Common.Const;
using CERIOS.Common.Models;
using CERIOS.Common.Dtos.Datainterface;

namespace CeriOS.LowCodeForm.Model.Helper
{
    public class CodeGenControlsAttributeHelper
    {
        /// <summary>
        /// 获取需解析的字段集合.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="isInlineEditor"></param>
        /// <returns>ceriKey @@ vmodel集合以 , 号隔开.</returns>
        public static List<string[]> GetParsCeriKeyConstListDetails(List<FieldsModel> control)
        {
            var res = new Dictionary<string, List<string>>();

            control.ForEach(item =>
            {
                switch (item.__config__.ceriKey)
                {
                    case CeriKeyConst.USERSSELECT: // 用户选择组件(包含组织、角色、岗位、分组、用户 Id)
                        if (!res.ContainsKey(CeriKeyConst.USERSSELECT)) res.Add(CeriKeyConst.USERSSELECT, new List<string>());
                        res[CeriKeyConst.USERSSELECT].Add(item.__vModel__);
                        break;
                    case CeriKeyConst.POPUPSELECT: // 弹窗选择.
                        if (!res.ContainsKey(CeriKeyConst.POPUPSELECT)) res.Add(CeriKeyConst.POPUPSELECT, new List<string>());
                        res[CeriKeyConst.POPUPSELECT].Add(item.__vModel__);
                        break;
                    case CeriKeyConst.RELATIONFORM: // 关联表单.
                        if (!res.ContainsKey(CeriKeyConst.RELATIONFORM)) res.Add(CeriKeyConst.RELATIONFORM, new List<string>());
                        res[CeriKeyConst.RELATIONFORM].Add(item.__vModel__);
                        break;
                    case CeriKeyConst.TABLE: // 遍历 子表 控件
                        var ctRes = GetParsCeriKeyConstListDetails(item.__config__.children);
                        if (ctRes != null && ctRes.Any())
                        {
                            foreach (var ct in ctRes)
                            {
                                if (!res.ContainsKey(ct.FirstOrDefault())) res.Add(ct.FirstOrDefault(), new List<string>());
                                var lasts = ct.LastOrDefault().Split(",");
                                foreach (var strItem in lasts) res[ct.FirstOrDefault()].Add(item.__vModel__ + "-" + strItem);
                            }
                        }

                        break;
                }
            });

            var ret = new List<string[]>();
            foreach (var item in res)
            {
                ret.Add(new string[] { item.Key, string.Join(",", item.Value) });
            }

            return ret;
        }

        /// <summary>
        /// 获取需解析的字段集合.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="isInlineEditor"></param>
        /// <returns>ceriKey @@ vmodel集合以 , 号隔开.</returns>
        public static List<string[]> GetParsCeriKeyConstList(List<FieldsModel> control, bool isInlineEditor)
        {
            var res = new Dictionary<string, List<string>>();

            control.ForEach(item =>
            {
                switch (item.__config__.ceriKey)
                {
                    case CeriKeyConst.USERSSELECT: // 用户选择组件(包含组织、角色、岗位、分组、用户 Id)
                        if (!res.ContainsKey(CeriKeyConst.USERSSELECT)) res.Add(CeriKeyConst.USERSSELECT, new List<string>());
                        res[CeriKeyConst.USERSSELECT].Add(item.__vModel__);
                        break;
                    case CeriKeyConst.POPUPSELECT: // 弹窗选择
                        if (!res.ContainsKey(CeriKeyConst.POPUPSELECT)) res.Add(CeriKeyConst.POPUPSELECT, new List<string>());
                        res[CeriKeyConst.POPUPSELECT].Add(item.__vModel__);
                        break;
                    case CeriKeyConst.RELATIONFORM: // 关联表单
                        if (!res.ContainsKey(CeriKeyConst.RELATIONFORM)) res.Add(CeriKeyConst.RELATIONFORM, new List<string>());
                        res[CeriKeyConst.RELATIONFORM].Add(item.__vModel__);
                        break;
                    case CeriKeyConst.TABLE: // 遍历 子表 控件
                        var ctRes = GetParsCeriKeyConstList(item.__config__.children, false);
                        if (ctRes != null && ctRes.Any())
                        {
                            foreach (var ct in ctRes)
                            {
                                if (!res.ContainsKey(ct.FirstOrDefault())) res.Add(ct.FirstOrDefault(), new List<string>());
                                var lasts = ct.LastOrDefault().Split(",");
                                foreach (var strItem in lasts) res[ct.FirstOrDefault()].Add(item.__vModel__ + "-" + strItem);
                            }
                        }
                        break;
                }
            });

            var ret = new List<string[]>();
            foreach (var item in res)
            {
                // 如果是行内编辑
                if (isInlineEditor)
                {
                    var newValue = new List<string>();
                    foreach (var it in item.Value) newValue.Add(it + "_name");
                    res[item.Key] = newValue;
                }
            }
            foreach (var item in res)
            {
                ret.Add(new string[] { item.Key, string.Join(",", item.Value) });
            }
            return ret;
        }

        /// <summary>
        /// 系统控件不更新.
        /// </summary>
        /// <param name="ceriKey">控件Key.</param>
        /// <returns></returns>
        public static bool JudgeControlIsSystemControls(string ceriKey)
        {
            bool tag = true;
            switch (ceriKey)
            {
                case CeriKeyConst.CREATEUSER:
                case CeriKeyConst.CREATETIME:
                case CeriKeyConst.CURRPOSITION:
                case CeriKeyConst.CURRORGANIZE:
                case CeriKeyConst.BILLRULE:
                    tag = false;
                    break;
            }

            return tag;
        }

        /// <summary>
        /// 转换静态数据.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<StaticDataModel> ConversionStaticData(string data)
        {
            var list = new List<StaticDataModel>();
            if (!string.IsNullOrEmpty(data))
            {
                var conData = JsonConvert.DeserializeObject<List<StaticDataModel>>(data);
                if (conData != null)
                {
                    foreach (var item in conData)
                    {
                        list.Add(new StaticDataModel()
                        {
                            id = item.id,
                            fullName = item.fullName,
                        });
                        if (item.children != null)
                            list.AddRange(ConversionStaticData(JsonConvert.SerializeObject(item.children)));
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 获取控件数据来源ID.
        /// </summary>
        /// <param name="ceriKey">控件Key.</param>
        /// <param name="dataType">数据类型.</param>
        /// <param name="control">控件全属性.</param>
        /// <returns></returns>
        public static string GetControlsPropsUrl(string ceriKey, string dataType, FieldsModel control)
        {
            string propsUrl = string.Empty;
            switch (ceriKey)
            {
                case CeriKeyConst.POPUPTABLESELECT:
                    propsUrl = control.interfaceId;
                    break;
                default:
                    switch (dataType)
                    {
                        case "dictionary":
                            propsUrl = control.__config__.dictionaryType;
                            break;
                        default:
                            propsUrl = control.__config__.propsUrl;
                            break;
                    }

                    break;
            }

            return propsUrl;
        }

        /// <summary>
        /// 获取控件指定选项的值.
        /// </summary>
        /// <param name="ceriKey">控件Key.</param>
        /// <param name="dataType">数据类型.</param>
        /// <param name="control">控件全属性.</param>
        /// <returns></returns>
        public static string GetControlsLabel(string ceriKey, string dataType, FieldsModel control)
        {
            string label = string.Empty;
            switch (ceriKey)
            {
                case CeriKeyConst.POPUPTABLESELECT:
                    label = control.relationField;
                    break;
                case CeriKeyConst.CASCADER:
                case CeriKeyConst.TREESELECT:
                    label = control.props.label;
                    break;
                default:
                    label = control.props?.label;
                    break;
            }

            return label;
        }

        /// <summary>
        /// 获取控件指定选项标签.
        /// </summary>
        /// <param name="ceriKey">控件Key.</param>
        /// <param name="dataType">数据类型.</param>
        /// <param name="control">控件全属性.</param>
        /// <returns></returns>
        public static string GetControlsValue(string ceriKey, string dataType, FieldsModel control)
        {
            string value = string.Empty;
            switch (ceriKey)
            {
                case CeriKeyConst.POPUPTABLESELECT:
                    value = control.propsValue;
                    break;
                case CeriKeyConst.CASCADER:
                case CeriKeyConst.TREESELECT:
                    value = control.props.value;
                    break;
                default:
                    value = control.props?.value;
                    break;
            }

            return value;
        }

        /// <summary>
        /// 获取控件指定选项的子选项.
        /// </summary>
        /// <param name="ceriKey">控件Key.</param>
        /// <param name="dataType">数据类型.</param>
        /// <param name="control">控件全属性.</param>
        /// <returns></returns>
        public static string GetControlsChildren(string ceriKey, string dataType, FieldsModel control)
        {
            string children = string.Empty;
            switch (ceriKey)
            {
                case CeriKeyConst.CASCADER:
                case CeriKeyConst.TREESELECT:
                    children = control.props.children;
                    break;
                default:
                    children = control.props?.children;
                    break;
            }

            return children;
        }

        /// <summary>
        /// 获取导出配置
        /// </summary>
        /// <param name="columnDesignModel">控件全.</param>
        /// <param name="control">控件全属性.</param>
        /// <param name="model">数据库真实字段.</param>
        /// <param name="tableName">表名称.</param>
        /// <param name="formDataModel">表单模型.</param>
        /// <returns></returns>
        public static CodeGenFieldsModel GetImportConfig(ColumnDesignModel columnDesignModel, FieldsModel control, string model, string tableName, FormDataModel formDataModel)
        {
            // 处理发杂表头
            if (columnDesignModel != null && columnDesignModel.complexHeaderList != null && columnDesignModel.complexHeaderList.Any(x => x.childColumns.Any(xx => xx.Equals(control.__vModel__))))
            {
                var comlex = columnDesignModel.complexHeaderList.FirstOrDefault(x => x.childColumns.Any(xx => xx.Equals(control.__vModel__)));

                // 复杂表头格式 label 调整
                control.__config__.label = string.Format("{0}@@{1}@@{2}@@{3}", comlex.id, comlex.fullName, comlex.align, control.__config__.label);
            }

            var fieldModel = new CodeGenFieldsModel();
            var configModel = new CodeGenConfigModel();
            fieldModel.__vModel__ = model;
            fieldModel.level = control.level;
            fieldModel.min = control.min == null ? 95279527 : control.min;
            fieldModel.max = control.max == null ? 95279527 : control.max;
            fieldModel.allowHalf = control.allowHalf;
            fieldModel.count = control.count;
            fieldModel.maxlength = control.maxlength == null ? "9999999L" : control.maxlength + "L";
            fieldModel.activeTxt = control.activeTxt;
            fieldModel.inactiveTxt = control.inactiveTxt;
            fieldModel.format = control.format;
            fieldModel.multiple = CodeGenFieldJudgeHelper.IsMultipleColumn(control, model);
            fieldModel.separator = control.separator;
            fieldModel.props = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<CodeGenPropsBeanModel>(JsonConvert.SerializeObject(control.props)));
            fieldModel.options = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<List<object>>(JsonConvert.SerializeObject(control.options)));
            fieldModel.propsValue = control.propsValue;
            fieldModel.relationField = control.relationField;
            fieldModel.modelId = control.modelId;
            fieldModel.interfaceId = control.interfaceId;
            fieldModel.selectType = control.selectType;
            fieldModel.ableIds = JsonConvert.SerializeObject(JsonConvert.SerializeObject(control.ableIds));
            if (control.templateJson != null && control.templateJson.Any() && control.__config__.templateJson == null)
                control.__config__.templateJson = control.templateJson;
            configModel = JsonConvert.DeserializeObject<CodeGenConfigModel>(JsonConvert.SerializeObject(control.__config__));
            if (formDataModel.useBusinessKey)
            {
                configModel.IsBusinessKey = formDataModel.businessKeyList.Any(x => x.Equals(model));
                configModel.tag = formDataModel.businessKeyTip;
            }
            configModel.tableName = tableName;
            configModel.showLevel = control.showLevel;
            configModel.precision = control.precision;
            fieldModel.__config__ = JsonConvert.SerializeObject(JsonConvert.SerializeObject(configModel));
            return fieldModel;
        }

        /// <summary>
        /// 判断控件是否数据转换.
        /// </summary>
        /// <returns></returns>
        public static bool JudgeControlIsDataConversion(string ceriKey, string dataType, bool multiple)
        {
            bool tag = false;
            switch (ceriKey)
            {
                case CeriKeyConst.LOCATION:
                case CeriKeyConst.UPLOADFZ:
                case CeriKeyConst.UPLOADIMG:
                    tag = true;
                    break;
                case CeriKeyConst.SELECT:
                case CeriKeyConst.TREESELECT:
                    {
                        switch (dataType)
                        {
                            case "dictionary":
                                if (!multiple)
                                {
                                    tag = false;
                                }
                                else
                                {
                                    tag = true;
                                }

                                break;
                            default:
                                tag = true;
                                break;
                        }
                    }

                    break;
                case CeriKeyConst.RADIO:
                    {
                        switch (dataType)
                        {
                            case "dictionary":
                                tag = false;
                                break;
                            default:
                                tag = true;
                                break;
                        }
                    }

                    break;
                case CeriKeyConst.DEPSELECT:
                case CeriKeyConst.POSSELECT:
                case CeriKeyConst.USERSELECT:
                case CeriKeyConst.ROLESELECT:
                case CeriKeyConst.GROUPSELECT:
                    {
                        if (!multiple)
                            tag = false;
                        else
                            tag = true;
                    }

                    break;
                case CeriKeyConst.CHECKBOX:
                case CeriKeyConst.CASCADER:
                case CeriKeyConst.COMSELECT:
                case CeriKeyConst.POPUPTABLESELECT:
                case CeriKeyConst.ADDRESS:
                case CeriKeyConst.USERSSELECT:
                    tag = true;
                    break;
            }

            return tag;
        }

        /// <summary>
        /// 获取控件是否存储类型.
        /// </summary>
        /// <returns></returns>
        public static bool GetIsControlStoreType(string ceriKey, int isStorage)
        {
            bool flag = true;

            switch (ceriKey)
            {
                case CeriKeyConst.POPUPATTR:
                case CeriKeyConst.RELATIONFORMATTR:
                    switch (isStorage)
                    {
                        case 0:
                            flag = false;
                            break;
                    }
                    break;
            }
            return flag;
        }

        /// <summary>
        /// 当前是否控件联动配置.
        /// </summary>
        /// <param name="fieldsModel">联动配置.</param>
        /// <returns></returns>
        public static bool IsControlLinkageConfiguration(FieldsModel fieldsModel)
        {
            var linkageConfigs = fieldsModel.templateJson == null ? fieldsModel.__config__.templateJson : fieldsModel.templateJson;
            return (linkageConfigs == null || !linkageConfigs.Any()) ? false : true;
        }

        /// <summary>
        /// 获取当前控件联动配置.
        /// </summary>
        /// <param name="fieldsModel">联动配置.</param>
        /// <param name="tableRelationship">0-主表,1-副表,2-子表.</param>
        /// <param name="tableName">表名称.</param>
        /// <returns></returns>
        public static List<DataInterfaceParameter> ObtainTheCurrentControlLinkageConfiguration(FieldsModel fieldsModel, int tableRelationship, string tableName = null)
        {
            var list = new List<DataInterfaceParameter>();
            var linkageConfigs = fieldsModel.templateJson == null ? fieldsModel.__config__.templateJson : fieldsModel.templateJson;
            if (linkageConfigs?.Count > 0)
            {
                linkageConfigs.ForEach(item =>
                {
                    var relationField = string.Empty;
                    var primaryAndSecondaryField = false;
                    switch (tableRelationship)
                    {
                        case 2:
                            relationField = item.relationField.Replace(string.Format("{0}-", tableName), string.Empty);
                            primaryAndSecondaryField = !item.relationField.Contains(tableName) ? true : false;
                            break;
                        default:
                            relationField = item.relationField;
                            break;
                    }

                    if (relationField.IsNotEmptyOrNull())
                    {
                        switch (item.sourceType)
                        {
                            case 1:
                                relationField = relationField;
                                break;
                            case 2:
                                relationField = (item.dataType.Equals("int") ? relationField : "\"" + relationField + "\"");
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                    }
                    list.Add(new DataInterfaceParameter
                    {
                        field = item.field,
                        relationField = relationField.IsNullOrEmpty() ? "string.Empty" : relationField,
                        isSubTable = primaryAndSecondaryField,
                        dataType = item.dataType,
                        defaultValue = item.defaultValue?.Replace("\"", "\\\""),
                        fieldName = item.fieldName,
                        sourceType = item.sourceType
                    });
                });
            }
            return list;
        }

        /// <summary>
        /// 获取模板配置的数据过滤.
        /// </summary>
        /// <param name="templateEntity"></param>
        /// <param name="codeGenConfigModel"></param>
        /// <returns></returns>
        public static string GetDataRuleList(FormDbDto templateEntity, CERIOS.Engine.Entity.Model.CodeGen.CodeGenConfigModel codeGenConfigModel)
        {
            var ruleList = new List<CodeGenDataRuleModuleResourceModel>();
            var tInfo = new TemplateParsingBase(templateEntity);

            // 数据过滤
            if (tInfo.ColumnData.ruleList != null && tInfo.ColumnData.ruleList.conditionList != null && tInfo.ColumnData.ruleList.conditionList.Any())
            {
                tInfo.ColumnData.ruleList.conditionList.ForEach(item =>
                {
                    var addItems = GetItemRule(tInfo.ColumnData.ruleList.matchLogic, item, tInfo, "pc", codeGenConfigModel, ref ruleList);
                    addItems.ForEach(addItem => { if (addItem.conditionalModel.Any()) ruleList.Add(addItem); });
                });
            }
            if (tInfo.AppColumnData.ruleListApp != null && tInfo.AppColumnData.ruleListApp.conditionList != null && tInfo.AppColumnData.ruleListApp.conditionList.Any())
            {
                tInfo.AppColumnData.ruleListApp.conditionList.ForEach(item =>
                {
                    var addItems = GetItemRule(tInfo.AppColumnData.ruleListApp.matchLogic, item, tInfo, "app", codeGenConfigModel, ref ruleList);
                    addItems.ForEach(addItem => { if (addItem.conditionalModel.Any()) ruleList.Add(addItem); });
                });
            }

            var res = new List<CodeGenDataRuleModuleResourceModel>();
            foreach (var userOriginItem in new List<string>() { "pc", "app" })
            {
                ruleList.Where(x => x.UserOrigin.Equals(userOriginItem)).Select(x => x.TableName).Distinct().ToList().ForEach(tName =>
                {
                    var first = ruleList.FirstOrDefault(x => x.UserOrigin.Equals(userOriginItem) && x.TableName.Equals(tName));
                    var condList = ruleList.Where(x => x.UserOrigin.Equals(userOriginItem) && x.TableName.Equals(tName)).ToList();
                    var condTree = new ConditionalTree() { ConditionalList = new List<KeyValuePair<WhereType, IConditionalModel>>() };
                    var whereType = condList.Any() && condList.FirstOrDefault().WhereType.ToUpper().Equals("AND") ? WhereType.And : WhereType.Or;
                    condList.ForEach(cItems =>
                    {
                        var condTreeItem = new ConditionalTree() { ConditionalList = new List<KeyValuePair<WhereType, IConditionalModel>>() };
                        var cItemsWhereType = cItems.WhereType.ToUpper().Equals("AND") ? WhereType.And : WhereType.Or;
                        var isNewGroupItem = true;
                        cItems.conditionalModel.ForEach(cItem =>
                        {
                            condTreeItem.ConditionalList.Add(new KeyValuePair<WhereType, IConditionalModel>(isNewGroupItem ? whereType : cItemsWhereType, cItem));
                            if (isNewGroupItem) isNewGroupItem = false;
                        });
                        condTree.ConditionalList.Add(new KeyValuePair<WhereType, IConditionalModel>(whereType, condTreeItem));
                    });

                    res.Add(new CodeGenDataRuleModuleResourceModel()
                    {
                        FieldRule = first.TableName.Contains("@ChildFieldIsNull") ? -1 : first.FieldRule,
                        TableName = first.TableName.Replace("@ChildFieldIsNull", string.Empty),
                        UserOrigin = first.UserOrigin,
                        conditionalModelJson = JsonConvert.SerializeObject(new List<IConditionalModel>() { condTree })
                    });
                });
            }

            return JsonConvert.SerializeObject(res).Replace("\r\n", string.Empty).Replace("\\r\\n", string.Empty).Replace("\"", "\\\"").Replace("\\\\\"", "\\\\\\\"").Replace("\\\\\\\\\"", "\\\\\\\\\\\\\"");
        }


        private static List<CodeGenDataRuleModuleResourceModel> GetItemRule(string matchLogic, DataFilteringConditionListModel condModel, TemplateParsingBase tInfo, string userOrigin, CERIOS.Engine.Entity.Model.CodeGen.CodeGenConfigModel codeGenConfigModel, ref List<CodeGenDataRuleModuleResourceModel> ruleList)
        {
            var dynamicQueryKey = CommonConst.DYNAMICQUERYKEY.Split(',');
            var resultList = new List<CodeGenDataRuleModuleResourceModel>();
            foreach (var item in condModel.groups)
            {
                item.field = item.field.Replace("--And", "");
                var result = new CodeGenDataRuleModuleResourceModel() { WhereType = matchLogic, FieldRule = 0, TableName = tInfo.MainTableName.ToLower(), UserOrigin = userOrigin, conditionalModel = new List<IConditionalModel>() };
                var vModel = item.field;
                if (tInfo.AuxiliaryTableFields.ContainsKey(vModel))
                {
                    var tf = tInfo.AuxiliaryTableFields[vModel].Split('.');
                    result.FieldRule = 1;
                    result.TableName = tf.FirstOrDefault().ToLower();
                    item.field = tf.LastOrDefault();
                }
                else if (tInfo.ChildTableFields.ContainsKey(vModel))
                {
                    var tf = tInfo.ChildTableFields[vModel].Split('.');
                    result.FieldRule = 2;
                    result.TableName = tf.FirstOrDefault().ToLower();
                    item.field = tf.LastOrDefault();

                    if (item.symbol.Equals("null") && matchLogic.Equals("or"))
                    {
                        var mainTableRelationsQuery = result.Copy();
                        mainTableRelationsQuery.TableName = mainTableRelationsQuery.TableName + "@ChildFieldIsNull";
                        var ctPrimaryKey = codeGenConfigModel.TableRelations.Find(x => x.OriginalTableName.Equals(result.TableName)).ChilderColumnConfigList.Find(x => x.ColumnName.Equals(codeGenConfigModel.TableRelations.Find(x => x.OriginalTableName.Equals(result.TableName)).PrimaryKey)).OriginalColumnName;
                        var condTree = new ConditionalCollections()
                        {
                            ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                    {
                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, new ConditionalModel() { FieldName = ctPrimaryKey, ConditionalType = ConditionalType.NoEqual, FieldValue = "0", FieldValueConvertFunc = it => SqlSugar.UtilMethods.ChangeType2(it, typeof(string)) })
                    }
                        };

                        mainTableRelationsQuery.conditionalModel = new List<IConditionalModel>() { condTree };
                        ruleList.Add(mainTableRelationsQuery);
                    }
                }

                if (codeGenConfigModel.TableField.Any(x => x.LowerColumnName.Equals(item.field)))
                {
                    var fieldList = codeGenConfigModel.TableField.Where(x => x.LowerColumnName.Equals(item.field)).ToList();
                    if (fieldList.Any() && fieldList.Count.Equals(1)) item.field = fieldList.First().OriginalColumnName;
                    else if (fieldList.Any(x => x.TableName != null && x.TableName.Equals(item.__config__.tableName))) item.field = fieldList.Find(x => x.TableName != null && x.TableName.Equals(item.__config__.tableName))?.OriginalColumnName;
                    else if (codeGenConfigModel.TableRelations.Any(x => x.OriginalTableName.Equals(result.TableName)))
                    {
                        var tableRelations = codeGenConfigModel.TableRelations.Find(x => x.OriginalTableName.Equals(result.TableName) && x.ChilderColumnConfigList.Any(xx => xx.LowerColumnName.Equals(item.field)));
                        if (tableRelations != null) item.field = tableRelations.ChilderColumnConfigList.Find(x => x.LowerColumnName.Equals(item.field)).OriginalColumnName;
                    }
                }
                else if (codeGenConfigModel.TableRelations.Any(x => x.OriginalTableName.Equals(result.TableName)))
                {
                    var tableRelations = codeGenConfigModel.TableRelations.Find(x => x.OriginalTableName.Equals(result.TableName) && x.ChilderColumnConfigList.Any(xx => xx.LowerColumnName.Equals(item.field)));
                    if (tableRelations != null) item.field = tableRelations.ChilderColumnConfigList.Find(x => x.LowerColumnName.Equals(item.field)).OriginalColumnName;
                }

                var conditionalType = ConditionalType.Equal;
                var between = new List<string>();
                if (item.fieldValue.IsNotEmptyOrNull() && !item.fieldValue.Equals("@currentTime"))
                {
                    if (item.symbol.Equals("between")) between = JsonConvert.DeserializeObject<List<string>>(JsonConvert.SerializeObject(item.fieldValue));
                    switch (item.ceriKey)
                    {
                        case CeriKeyConst.CURRORGANIZE:
                            if (item.fieldValue.IsNotEmptyOrNull() && item.fieldValue.ToString().Replace("\r\n", "").Replace(" ", "").Contains("[[")) item.fieldValue = JsonConvert.DeserializeObject<List<List<string>>>(JsonConvert.SerializeObject(item.fieldValue)).Select(x => x.Last() + "\"]").ToList();
                            else if (item.fieldValue.IsNotEmptyOrNull() && JsonConvert.SerializeObject(item.fieldValue).Contains("[")) item.fieldValue = item.fieldValue.ToString().Replace("\r\n", "").Replace(" ", "");
                            break;
                        case CeriKeyConst.COMSELECT:
                            if (item.fieldValue.IsNotEmptyOrNull() && item.symbol.Equals("==") || item.symbol.Equals("<>"))
                            {
                                item.fieldValue = item.fieldValue.ToString().Replace("\r\n", "").Replace(" ", "");
                            }
                            else
                            {
                                if (item.fieldValue.ToString().Replace("\r\n", "").Replace(" ", "").Contains("[[")) item.fieldValue = JsonConvert.DeserializeObject<List<List<string>>>(JsonConvert.SerializeObject(item.fieldValue)).Select(x => x.Last() + "\"]").ToList();
                                else if (item.fieldValue.ToString().Replace("\r\n", "").Replace(" ", "").Contains("[") && JsonConvert.DeserializeObject<List<string>>(JsonConvert.SerializeObject(item.fieldValue)).Any(x => x.Contains("[")))
                                    item.fieldValue =JsonConvert.DeserializeObject<List<string>>(JsonConvert.SerializeObject(item.fieldValue)).Select(x => x + "\"]").ToList();
                            }
                            break;
                        case CeriKeyConst.CREATETIME:
                        case CeriKeyConst.MODIFYTIME:
                        case CeriKeyConst.DATE:
                            {
                                if (item.symbol.Equals("between"))
                                {
                                    var startTime = between.First().TimeStampToDateTime();
                                    var endTime = between.Last().TimeStampToDateTime();
                                    between[0] = startTime.ToString();
                                    between[1] = endTime.ToString();
                                }
                                else
                                {
                                    if (item.fieldValue is DateTime)
                                    {
                                        item.fieldValue = item.fieldValue.ToString();
                                    }
                                    else if (item.fieldValue is long)
                                    {
                                        item.fieldValue = item.fieldValue.ToString().TimeStampToDateTime();
                                    }
                                }
                            }
                            break;
                        case CeriKeyConst.TIME:
                            {
                                if (!item.symbol.Equals("between"))
                                {
                                    item.fieldValue = string.Format("{0:" + item.format + "}", Convert.ToDateTime(item.fieldValue));
                                }
                            }
                            break;
                        case CeriKeyConst.CASCADER:
                        case CeriKeyConst.ADDRESS:
                            if (item.fieldValue.IsNotEmptyOrNull() && (item.symbol.Equals("==") || item.symbol.Equals("<>")))
                                item.fieldValue = item.fieldValue.ToString().Replace("\r\n", "").Replace(" ", "");
                            break;

                    }
                }

                switch (item.symbol)
                {
                    case ">=":
                        conditionalType = ConditionalType.GreaterThanOrEqual;
                        break;
                    case ">":
                        conditionalType = ConditionalType.GreaterThan;
                        break;
                    case "==":
                        conditionalType = ConditionalType.Equal;
                        break;
                    case "<=":
                        conditionalType = ConditionalType.LessThanOrEqual;
                        break;
                    case "<":
                        conditionalType = ConditionalType.LessThan;
                        break;
                    case "<>":
                        conditionalType = ConditionalType.NoEqual;
                        break;
                    case "like":
                        if (item.fieldValue != null && item.fieldValue.ToString().Contains("[")) item.fieldValue = item.fieldValue.ToString().Replace("[", string.Empty).Replace("]", string.Empty);
                        conditionalType = ConditionalType.Like;
                        break;
                    case "notLike":
                        if (item.fieldValue != null && item.fieldValue.ToString().Contains("[")) item.fieldValue = item.fieldValue.ToString().Replace("[", string.Empty).Replace("]", string.Empty);
                        conditionalType = ConditionalType.NoLike;
                        break;
                    case "in":
                    case "notIn":
                        if (item.fieldValue != null && !dynamicQueryKey.Contains(item.fieldValue) && item.fieldValue.ToString().Contains("["))
                        {
                            var isListValue = false;
                            var itemField = tInfo.AllFieldsModel.Find(x => x.__vModel__.Equals(vModel));
                            if (itemField != null && (itemField.multiple || item.ceriKey.Equals(CeriKeyConst.CHECKBOX) || item.ceriKey.Equals(CeriKeyConst.CASCADER) || item.ceriKey.Equals(CeriKeyConst.ADDRESS)))
                                isListValue = true;
                            if (item.ceriKey.Equals(CeriKeyConst.COMSELECT)) isListValue = false;
                            var conditionalList = new ConditionalCollections() { ConditionalList = new List<KeyValuePair<WhereType, ConditionalModel>>() };
                            var ids = new List<string>();
                            if (item.fieldValue.ToString().Replace("\r\n", "").Replace(" ", "").Contains("[[")) ids = JsonConvert.DeserializeObject<List<List<string>>>(JsonConvert.SerializeObject(item.fieldValue)).Select(x => x.Last()).ToList();
                            else ids = JsonConvert.DeserializeObject<List<string>>(JsonConvert.SerializeObject(item.fieldValue));
                            for (var i = 0; i < ids.Count; i++)
                            {
                                var it = ids[i];
                                var whereType = WhereType.And;
                                if (item.symbol.Equals("in")) whereType = i.Equals(0) && condModel.logic.ToUpper().Equals("AND") ? WhereType.And : WhereType.Or;
                                else whereType = i.Equals(0) && condModel.logic.ToUpper().Equals("OR") ? WhereType.Or : WhereType.And;
                                conditionalList.ConditionalList.Add(new KeyValuePair<WhereType, ConditionalModel>(whereType, new ConditionalModel
                                {
                                    FieldName = item.field,
                                    ConditionalType = item.symbol.Equals("in") ? (item.ceriKey.Equals(CeriKeyConst.TREESELECT) ? ConditionalType.Equal : ConditionalType.Like) : (item.ceriKey.Equals(CeriKeyConst.TREESELECT) ? ConditionalType.NoEqual : ConditionalType.NoLike),
                                    FieldValue = isListValue ? JsonConvert.SerializeObject(it): it
                                }));
                            }

                            if (item.symbol.Equals("notIn"))
                            {
                                conditionalList.ConditionalList.Add(new KeyValuePair<WhereType, ConditionalModel>(WhereType.And, new ConditionalModel
                                {
                                    FieldName = item.field,
                                    ConditionalType = ConditionalType.IsNot,
                                    FieldValue = string.Empty
                                }));
                            }

                            result.conditionalModel.Add(conditionalList);
                            resultList.Add(result);
                            continue;
                        }
                        conditionalType = item.symbol.Equals("in") ? ConditionalType.In : ConditionalType.NotIn;
                        break;
                    case "null":
                        conditionalType = (item.ceriKey.Equals(CeriKeyConst.CALCULATE) || item.ceriKey.Equals(CeriKeyConst.NUMINPUT)) ? ConditionalType.EqualNull : ConditionalType.IsNullOrEmpty;
                        break;
                    case "notNull":
                        conditionalType = ConditionalType.IsNot;
                        if (item.fieldValue.IsNullOrEmpty())
                            item.fieldValue = null;
                        break;
                    case "between":
                        var condItem = new ConditionalCollections()
                        {
                            ConditionalList = new List<KeyValuePair<WhereType, ConditionalModel>>()
                    {
                        new KeyValuePair<WhereType, ConditionalModel>((condModel.logic.ToUpper().Equals("AND") ? WhereType.And : WhereType.Or), new ConditionalModel
                        {
                            FieldName = item.field,
                            ConditionalType = ConditionalType.GreaterThanOrEqual,
                            FieldValue = between.First(),
                            FieldValueConvertFunc = it => Convert.ToDateTime(it)
                        }),
                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.And, new ConditionalModel
                        {
                            FieldName = item.field,
                            ConditionalType = ConditionalType.LessThanOrEqual,
                            FieldValue = between.Last(),
                            FieldValueConvertFunc = it => Convert.ToDateTime(it)
                        })
                    }
                        };

                        result.conditionalModel.Add(condItem);
                        resultList.Add(result);
                        continue;
                }

                var resItem = new ConditionalCollections()
                {
                    ConditionalList = new List<KeyValuePair<WhereType, ConditionalModel>>()
            {
                new KeyValuePair<WhereType, ConditionalModel>((condModel.logic.ToUpper().Equals("AND") ? WhereType.And : WhereType.Or), new ConditionalModel
                {
                    FieldName = item.field,
                    ConditionalType = conditionalType,
                    FieldValue = item.fieldValue == null ? string.Empty : item.fieldValue.ToString()
                })
            }
                };
                result.conditionalModel.Add(resItem);
                resultList.Add(result);
            }
            return resultList;
        }
    }
}
