using CeriOS.LowCodeForm.Model.Model;
using CeriOS.LowCodeForm.Model.ViewModel.LowCode;
using CERIOS.Common.Const;
using CERIOS.Common.Extension;
using CERIOS.Engine.Entity.Model;
using CERIOS.Engine.Entity.Model.CodeGen;
using Newtonsoft.Json;

namespace CeriOS.LowCodeForm.Model.Helper
{
    public class CodeGenExportFieldHelper
    {
        /// <summary>
        /// 获取主表字段名.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="comlexList">复杂表头.</param>
        /// <returns></returns>
        public static string ExportColumnField(List<IndexGridFieldModel>? list, List<ComplexHeaderModel> comlexList)
        {
            var columnSb = new List<string>();
            if (list != null)
            {
                foreach (var item in list)
                {
                    if (comlexList.Any(x => x.childColumns.Any(xx => xx.Equals(item.prop))))
                    {
                        var columns = comlexList.FirstOrDefault(x => x.childColumns.Any(yy => yy.Equals(item.prop))).childColumns;
                        item.currentIndex = list.IndexOf(list.Find(x => x.id.Equals(columns.FirstOrDefault())));
                        if (columns.FirstOrDefault().Equals(item.id)) item.currentIndex--;
                    }
                    else
                    {
                        item.currentIndex = list.IndexOf(list.Find(x => x.id.Equals(item.id)));
                    }
                }

                list = list.OrderBy(x => x.currentIndex).ToList();

                foreach (var item in list)
                {
                    if (comlexList.Any(x => x.childColumns.Any(xx => xx.Equals(item.prop))))
                    {
                        var comlex = comlexList.FirstOrDefault(x => x.childColumns.Any(xx => xx.Equals(item.prop)));

                        // 复杂表头格式 label 调整
                        var comlexLabel = string.Format("{0}@@{1}@@{2}@@{3}", comlex.id, comlex.fullName, comlex.align, item.label);
                        columnSb.Add(string.Format("{{\\\"value\\\":\\\"{0}\\\",\\\"field\\\":\\\"{1}\\\"}}", comlexLabel, item.prop));
                    }
                    else
                    {
                        if (item.__config__.parentVModel.IsNotEmptyOrNull() && item.__config__.parentVModel.ToLower().Contains("tablefield"))
                        {
                            foreach (var it in list.Where(x => x.prop.Contains(item.__config__.parentVModel)))
                            {
                                var addItem = string.Format("{{\\\"value\\\":\\\"{0}\\\",\\\"field\\\":\\\"{1}\\\"}}", it.label, it.prop);
                                if (!columnSb.Any(x => x.Equals(addItem))) columnSb.Add(addItem);
                            }
                        }
                        else
                        {
                            var addItem = string.Format("{{\\\"value\\\":\\\"{0}\\\",\\\"field\\\":\\\"{1}\\\"}}", item.label, item.prop);
                            if (!columnSb.Any(x => x.Equals(addItem))) columnSb.Add(addItem);
                        }
                    }
                }
            }

            return string.Join(",", columnSb);
        }

        /// <summary>
        /// 获取导入字段.
        /// </summary>
        /// <param name="templateEntity"></param>
        /// <param name="configModel"></param>
        /// <param name="dbLink"></param>
        /// <returns></returns>
        public static string ImportColumnField(VisualDevEntity templateEntity, CodeGenConfigModel configModel, DBConfig dbLink)
        {
            var resDic = new Dictionary<string, string>();

            foreach (var item in configModel.TableField)
            {
                var lowerColumnName = dbLink.DbType.ToLower().Equals("oracle") ? item.OriginalColumnName : item.LowerColumnName;
                var columnName = templateEntity.EnableFlow.Equals(1) ? item.LowerColumnName : item.OriginalColumnName;
                if (item.IsAuxiliary) resDic.Add(string.Format("ceri_{0}_ceri_{1}", item.TableName, lowerColumnName), string.Format("ceri_{0}_ceri_{1}", item.TableName, columnName));
                else resDic.Add(lowerColumnName, columnName);
            }

            if (configModel.TableRelations != null && configModel.TableRelations.Any())
            {
                foreach (var table in configModel.TableRelations)
                {
                    foreach (var item in table.ChilderColumnConfigList)
                    {
                        var lowerColumnName = dbLink.DbType.ToLower().Equals("oracle") ? item.OriginalColumnName : item.LowerColumnName;
                        var columnName = templateEntity.EnableFlow.Equals(1) ? lowerColumnName : item.OriginalColumnName;
                        resDic.Add(string.Format("{0}-{1}", table.ControlModel, lowerColumnName), string.Format("{0}-{1}", table.ControlModel, columnName));
                    }
                }
            }

            var res = new List<string>();
            if (templateEntity.ColumnData.IsNotEmptyOrNull())
            {
                var columnDesignModel = JsonConvert.DeserializeObject<ColumnDesignModel>(templateEntity.ColumnData);
                if (columnDesignModel.type.Equals(3) || columnDesignModel.type.Equals(5)) columnDesignModel.complexHeaderList.Clear();

                if (columnDesignModel.uploaderTemplateJson != null && columnDesignModel.uploaderTemplateJson.selectKey != null)
                {
                    var excList = columnDesignModel.uploaderTemplateJson.selectKey.Except(columnDesignModel.defaultColumnList.Select(xx => xx.id));
                    foreach (var item in excList)
                    {
                        columnDesignModel.defaultColumnList.Insert(columnDesignModel.uploaderTemplateJson.selectKey.IndexOf(item), new IndexGridFieldModel()
                        {
                            id = item,
                            ceriKey = item,
                        });
                    }

                    foreach (var vModel in columnDesignModel.defaultColumnList)
                    {
                        if (vModel != null && (vModel.ceriKey.Equals(CeriKeyConst.CREATETIME) || vModel.ceriKey.Equals(CeriKeyConst.CREATEUSER) || vModel.ceriKey.Equals(CeriKeyConst.BILLRULE) ||
                            vModel.ceriKey.Equals(CeriKeyConst.MODIFYUSER) || vModel.ceriKey.Equals(CeriKeyConst.MODIFYTIME) || vModel.ceriKey.Equals(CeriKeyConst.CURRDEPT)
                             || vModel.ceriKey.Equals(CeriKeyConst.CURRORGANIZE) || vModel.ceriKey.Equals(CeriKeyConst.CURRPOSITION))) continue;
                        var item = columnDesignModel.uploaderTemplateJson.selectKey.FirstOrDefault(x => x.Equals(vModel.id));
                        if (item != null)
                        {
                            if (columnDesignModel.complexHeaderList.Any(x => x.childColumns.Any(xx => xx.Equals(item))))
                            {
                                var chItems = columnDesignModel.complexHeaderList.First(x => x.childColumns.Any(xx => xx.Equals(item))).childColumns;
                                chItems.ForEach(it =>
                                {
                                    if (columnDesignModel.uploaderTemplateJson.selectKey.Contains(it) && !res.Contains(resDic[it])) res.Add(resDic[it]);
                                });
                            }
                            else
                            {
                                if (!res.Contains(resDic[item]))
                                {
                                    if (resDic[item].ToLower().Contains("tablefield") && resDic[item].Contains("-"))
                                    {
                                        var ctModel = resDic[item].Split("-").First() + "-";

                                        foreach (var it in columnDesignModel.defaultColumnList.Where(x => x.id.Contains(ctModel)))
                                            if (columnDesignModel.uploaderTemplateJson.selectKey.Any(x => x.Equals(it.id))) res.Add(resDic[it.id]);
                                    }
                                    else
                                    {
                                        res.Add(resDic[item]);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return "{\"" + string.Join("\",\"", res) + "\"}";
        }

        /// <summary>
        /// 获取跨库.
        /// </summary>
        /// <param name="DefaultLink"></param>
        /// <returns></returns>
        public static string GetDefaultDbNameByDbType(DBConfig DefaultLink)
        {
            var res = string.Empty;
            switch (DefaultLink.DbType.ToLower())
            {
                case "sqlserver":
                    res = string.Format("{0}.dbo", DefaultLink.ServiceName);
                    break;
                case "mysql":
                    res = DefaultLink.ServiceName;
                    break;
            }

            return res;
        }
    }
}
