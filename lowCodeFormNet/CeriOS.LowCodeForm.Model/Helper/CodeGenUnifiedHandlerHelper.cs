using CERIOS.Common.Const;
using CERIOS.Common.Extension;
using CERIOS.Engine.Entity.Model;

namespace CeriOS.LowCodeForm.Model.Helper
{
    public class CodeGenUnifiedHandlerHelper
    {
        public static List<FieldsModel> UnifiedHandlerFormDataModel(List<FieldsModel> formDataModel, ColumnDesignModel pcColumnDesignModel, ColumnDesignModel appColumnDesignModel, bool isMain = true, string tableControlsKey = "")
        {
            var template = new List<FieldsModel>();

            foreach (var item in formDataModel)
            {
                var config = item.__config__;
                switch (config.ceriKey)
                {
                    case CeriKeyConst.TABLE:
                        item.__config__.children = UnifiedHandlerFormDataModel(item.__config__.children, pcColumnDesignModel, appColumnDesignModel, false, item.__vModel__);
                        template.Add(item);
                        break;

                    default:
                        {
                            if (isMain)
                            {
                                // 是否为PC端查询字段与移动端查询字段
                                bool pcSearch = (pcColumnDesignModel?.searchList?.Any(it => it.id.Equals(item.__vModel__))).ParseToBool();
                                bool appSearch = (appColumnDesignModel?.searchList?.Any(it => it.id.Equals(item.__vModel__))).ParseToBool();
                                bool treeSearch = (pcColumnDesignModel.treeRelation?.Equals(item.__vModel__)).ParseToBool();
                                if (pcSearch || appSearch || treeSearch)
                                    item.isQueryField = true;
                                else
                                    item.isQueryField = false;

                                bool pcColumn = (pcColumnDesignModel?.columnList?.Any(it => it.id.Equals(item.__vModel__))).ParseToBool();
                                bool appColumn = (appColumnDesignModel?.columnList?.Any(it => it.id.Equals(item.__vModel__))).ParseToBool();
                                if (pcColumn || appColumn)
                                    item.isIndexShow = true;
                                else
                                    item.isIndexShow = false;
                            }
                            else
                            {
                                bool pcSearch = (pcColumnDesignModel?.searchList?.Any(it => it.id.Equals(string.Format("{0}-{1}", tableControlsKey, item.__vModel__)))).ParseToBool();
                                bool appSearch = (appColumnDesignModel?.searchList?.Any(it => it.id.Equals(string.Format("{0}-{1}", tableControlsKey, item.__vModel__)))).ParseToBool();
                                bool treeSearch = (pcColumnDesignModel.treeRelation?.Equals(string.Format("{0}-{1}", tableControlsKey, item.__vModel__))).ParseToBool();
                                if (pcSearch || appSearch || treeSearch)
                                    item.isQueryField = true;
                                else
                                    item.isQueryField = false;

                                bool pcColumn = (pcColumnDesignModel?.columnList?.Any(it => it.id.Equals(string.Format("{0}-{1}", tableControlsKey, item.__vModel__)))).ParseToBool();
                                bool appColumn = (appColumnDesignModel?.columnList?.Any(it => it.id.Equals(string.Format("{0}-{1}", tableControlsKey, item.__vModel__)))).ParseToBool();
                                if (pcColumn || appColumn)
                                    item.isIndexShow = true;
                                else
                                    item.isIndexShow = false;
                            }
                            template.Add(item);
                        }
                        break;
                }
            }
            return template;
        }

        /// <summary>
        /// 统一处理表单内控件.
        /// </summary>
        /// <returns></returns>
        public static List<FieldsModel> UnifiedHandlerFormDataModel(List<FieldsModel> formDataModel, ColumnDesignModel columnDesignModel, bool isMain = true, string tableControlsKey = "")
        {
            var template = new List<FieldsModel>();

            // 循环表单内控件
            formDataModel.ForEach(item =>
            {
                var config = item.__config__;
                switch (config.ceriKey)
                {
                    case CeriKeyConst.TABLE:
                        item.__config__.children = UnifiedHandlerFormDataModel(item.__config__.children, columnDesignModel, false, item.__vModel__);
                        template.Add(item);
                        break;
                    default:
                        {
                            if (isMain)
                            {
                                // 是否为PC端查询字段与移动端查询字段
                                bool search = columnDesignModel?.searchList?.Any(it => it.__vModel__.Equals(item.__vModel__)) ?? false;
                                if (search)
                                    item.isQueryField = true;
                                else
                                    item.isQueryField = false;

                                bool column = columnDesignModel?.columnList?.Any(it => it.__vModel__.Equals(item.__vModel__)) ?? false;
                                if (column)
                                    item.isIndexShow = true;
                                else
                                    item.isIndexShow = false;
                            }
                            else
                            {
                                bool search = columnDesignModel?.searchList?.Any(it => it.__vModel__.Equals(string.Format("{0}-{1}", tableControlsKey, item.__vModel__))) ?? false;
                                if (search)
                                {
                                    item.isQueryField = true;
                                    item.superiorVModel = tableControlsKey;
                                }
                                else
                                {
                                    item.isQueryField = false;
                                }

                                bool column = columnDesignModel?.columnList?.Any(it => it.__vModel__.Equals(string.Format("{0}-{1}", tableControlsKey, item.__vModel__))) ?? false;
                                if (column)
                                    item.isIndexShow = true;
                                else
                                    item.isIndexShow = false;
                            }

                            template.Add(item);
                        }

                        break;
                }
            });

            return template;
        }

        /// <summary>
        /// 联动关系链判断.
        /// </summary>
        /// <param name="formDataModel"></param>
        /// <param name="columnDesignModel"></param>
        /// <param name="isMain"></param>
        /// <param name="tableControlsKey"></param>
        /// <returns></returns>
        public static List<FieldsModel> LinkageChainJudgment(List<FieldsModel> formDataModel, ColumnDesignModel columnDesignModel, bool isMain = true, string tableControlsKey = "")
        {
            var NewFormDataModel = formDataModel;
            var childrenFormModel = new List<FieldsModel>();
            if (!isMain)
            {
                formDataModel = NewFormDataModel.Find(it => it.__vModel__.Equals(tableControlsKey) && it.__config__.ceriKey.Equals(CeriKeyConst.TABLE)).__config__.children;
                childrenFormModel = formDataModel;
            }

            formDataModel.ForEach(item =>
            {
                var config = item.__config__;
                switch (config.ceriKey)
                {
                    case CeriKeyConst.TABLE:
                        {
                            NewFormDataModel = LinkageChainJudgment(NewFormDataModel, columnDesignModel, false, item.__vModel__);
                        }
                        break;
                    case CeriKeyConst.RADIO:
                    case CeriKeyConst.CHECKBOX:
                    case CeriKeyConst.SELECT:
                    case CeriKeyConst.CASCADER:
                    case CeriKeyConst.TREESELECT:
                        switch (isMain)
                        {
                            case true:
                                // dataType = dynamic && templateJson属性有长度，则代表有远端联动
                                if (config.dataType == "dynamic" && config.templateJson?.Count() > 0)
                                {
                                    var mainFieldModel = NewFormDataModel.Where(it => item.__vModel__.Equals(it.__vModel__) && it.__config__.ceriKey.Equals(config.ceriKey)).FirstOrDefault();
                                    config.templateJson.FindAll(it => it.relationField != null && it.relationField.Any()).ForEach(items =>
                                    {
                                        mainFieldModel.IsLinkage = true;

                                        // 被联动控件信息
                                        var fieldModel = NewFormDataModel.Where(it => it.__vModel__.Equals(items.relationField) && it.__config__.ceriKey.Equals(items.ceriKey)).FirstOrDefault();
                                        if (fieldModel != null)
                                        {
                                            fieldModel.IsLinked = true;
                                            List<LinkageConfig> linkageConfigs = new List<LinkageConfig>
                                        {
                                            new LinkageConfig()
                                            {
                                                field = item.__vModel__,
                                                fieldName = item.__vModel__.ToLowerCase(),
                                                ceriKey = config.ceriKey,
                                                IsMultiple = config.ceriKey.Equals(CeriKeyConst.CHECKBOX)? true : item.multiple,
                                            }
                                        };
                                            fieldModel.linkageReverseRelationship.AddRange(linkageConfigs);
                                        }
                                    });
                                }
                                break;
                            default:
                                if (config.dataType == "dynamic" && config.templateJson?.Count() > 0)
                                {
                                    var childrenFieldModel = childrenFormModel.Where(it => item.__vModel__.Equals(it.__vModel__) && it.__config__.ceriKey.Equals(config.ceriKey)).FirstOrDefault();
                                    config.templateJson.FindAll(it => it.relationField != null && it.relationField.Any()).ForEach(items =>
                                    {
                                        childrenFieldModel.IsLinkage = true;
                                        var isTrigger = false;
                                        var fieldModel = childrenFormModel.Where(it => items.relationField.Equals(string.Format("{0}-{1}", tableControlsKey, it.__vModel__)) && it.__config__.ceriKey.Equals(items.ceriKey)).FirstOrDefault();
                                        if (fieldModel == null)
                                        {
                                            isTrigger = true;
                                            fieldModel = NewFormDataModel.Where(it => it.__vModel__.Equals(items.relationField) && it.__config__.ceriKey.Equals(items.ceriKey)).FirstOrDefault();
                                        }

                                        if (fieldModel != null)
                                        {
                                            fieldModel.IsLinked = true;
                                            List<LinkageConfig> linkageConfigs = new List<LinkageConfig>
                                        {
                                            new LinkageConfig()
                                            {
                                                field = item.__vModel__,
                                                fieldName = tableControlsKey,
                                                ceriKey = config.ceriKey,
                                                isChildren = isTrigger,
                                                IsMultiple = item.multiple,
                                            }
                                        };
                                            fieldModel.linkageReverseRelationship.AddRange(linkageConfigs);
                                        }
                                    });
                                }
                                break;
                        }
                        break;
                    case CeriKeyConst.POPUPTABLESELECT:
                    case CeriKeyConst.POPUPSELECT:
                    case CeriKeyConst.AUTOCOMPLETE:
                    case CeriKeyConst.RELATIONFORM:
                        switch (isMain)
                        {
                            case true:
                                var mainFieldModel = NewFormDataModel.Where(it => item.__vModel__.Equals(it.__vModel__) && it.__config__.ceriKey.Equals(config.ceriKey)).FirstOrDefault();
                                item.templateJson?.FindAll(it => it.relationField != null && it.relationField.Any()).ForEach(items =>
                                {
                                    mainFieldModel.IsLinkage = true;
                                    var fieldModel = NewFormDataModel.Where(it => it.__vModel__.Equals(items.relationField) && it.__config__.ceriKey.Equals(items.ceriKey)).FirstOrDefault();
                                    if (fieldModel != null)
                                    {
                                        fieldModel.IsLinked = true;
                                        List<LinkageConfig> linkageConfigs = new List<LinkageConfig>
                                    {
                                        new LinkageConfig()
                                        {
                                            field = item.__vModel__,
                                            fieldName = item.__vModel__.ToLowerCase(),
                                            ceriKey = config.ceriKey,
                                            IsMultiple = config.ceriKey.Equals(CeriKeyConst.CHECKBOX) ? true : item.multiple,
                                        }
                                    };
                                        fieldModel.linkageReverseRelationship.AddRange(linkageConfigs);
                                    }
                                });
                                break;
                            default:
                                var childrenFieldModel = childrenFormModel.Where(it => item.__vModel__.Equals(it.__vModel__) && it.__config__.ceriKey.Equals(config.ceriKey)).FirstOrDefault();
                                item.templateJson?.FindAll(it => it.relationField != null && it.relationField.Any() && !it.relationField.Equals("@keyword")).ForEach(items =>
                                {
                                    childrenFieldModel.IsLinkage = true;
                                    var isTrigger = false;
                                    var fieldModel = childrenFormModel.Where(it => items.relationField.Equals(string.Format("{0}-{1}", tableControlsKey, it.__vModel__)) && it.__config__.ceriKey.Equals(items.ceriKey)).FirstOrDefault();
                                    if (fieldModel == null)
                                    {
                                        isTrigger = true;
                                        fieldModel = NewFormDataModel.Where(it => it.__vModel__.Equals(items.relationField) && it.__config__.ceriKey.Equals(items.ceriKey)).FirstOrDefault();
                                    }

                                    if (fieldModel != null)
                                    {
                                        fieldModel.IsLinked = true;
                                        List<LinkageConfig> linkageConfigs = new List<LinkageConfig>
                                    {
                                        new LinkageConfig()
                                        {
                                            field = item.__vModel__,
                                            fieldName = tableControlsKey,
                                            ceriKey = config.ceriKey,
                                            isChildren = isTrigger,
                                            IsMultiple = item.multiple,
                                        }
                                    };
                                        fieldModel.linkageReverseRelationship.AddRange(linkageConfigs);
                                    }
                                });
                                break;
                        }
                        break;
                }
            });

            if (!isMain)
            {
                NewFormDataModel.Find(it => it.__vModel__.Equals(tableControlsKey) && it.__config__.ceriKey.Equals(CeriKeyConst.TABLE)).__config__.children = childrenFormModel;
            }
            return NewFormDataModel;
        }

        /// <summary>
        /// 统一处理控件关系.
        /// </summary>
        /// <param name="formDataModel">控件列表.</param>
        /// <returns></returns>
        public static List<FieldsModel> UnifiedHandlerControlRelationship(List<FieldsModel> formDataModel, bool isMain = true)
        {
            formDataModel.ForEach(item =>
            {
                switch (item.__config__.ceriKey)
                {
                    case CeriKeyConst.RELATIONFORM:
                        {
                            var list = formDataModel.FindAll(it => it.__config__.ceriKey.Equals(CeriKeyConst.RELATIONFORMATTR) && it.relationField.Equals(string.Format("{0}_ceriTable_{1}{2}", item.__vModel__, item.__config__.tableName, isMain ? 1 : 0)));
                            item.relational = string.Join(",", list.Select(it => it.showField).ToList());
                        }

                        break;
                    case CeriKeyConst.TABLE:
                        {
                            item.__config__.children = UnifiedHandlerControlRelationship(item.__config__.children, false);
                        }

                        break;
                    case CeriKeyConst.POPUPSELECT:
                        {
                            var list = formDataModel.FindAll(it => it.__config__.ceriKey.Equals(CeriKeyConst.POPUPATTR) && it.relationField.Equals(string.Format("{0}_ceriTable_{1}{2}", item.__vModel__, item.__config__.tableName, isMain ? 1 : 0)));
                            item.relational = string.Join(",", list.Select(it => it.showField).ToList());
                        }

                        break;
                }
            });

            return formDataModel;
        }

        /// <summary>
        /// 统一处理表单内控件.
        /// </summary>
        /// <returns></returns>
        public static List<IndexSearchFieldModel> UnifiedHandlerListQueries(List<IndexSearchFieldModel> formDataModel, ColumnDesignModel pcColumnDesignModel, ColumnDesignModel appColumnDesignModel)
        {
            switch (appColumnDesignModel.searchList.Any(it => it.id.Equals(pcColumnDesignModel?.treeRelation) && it.searchMultiple))
            {
                case true:
                    {
                        formDataModel.Add(appColumnDesignModel.searchList.Find(it => it.id.Equals(pcColumnDesignModel?.treeRelation) && it.searchMultiple));
                    }
                    break;
            }

            return formDataModel;
        }

    }
}
