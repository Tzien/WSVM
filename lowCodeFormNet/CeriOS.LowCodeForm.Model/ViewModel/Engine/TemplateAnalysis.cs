using CERIOS.Common.Const;
using CERIOS.Engine.Entity.Model;

namespace CeriOS.LowCodeForm.Model.ViewModel.Engine
{
    public static class TemplateAnalysis
    {
        /// <summary>
        /// 解析模板数据
        /// 移除模板内的布局类型控件.
        /// </summary>
        public static List<FieldsModel> AnalysisTemplateData(List<FieldsModel> fieldsModelList)
        {
            var template = new List<FieldsModel>();

            // 将模板内的无限children解析出来
            // 不包含子表children
            foreach (FieldsModel? item in fieldsModelList)
            {
                ConfigModel? config = item.__config__;
                switch (config.ceriKey)
                {
                    case CeriKeyConst.TABLE: // 设计子表
                        item.__config__.defaultCurrent = item.__config__.children.Any(it => it.__config__.defaultCurrent);
                        template.Add(item);
                        break;
                    case CeriKeyConst.ROW: // 栅格布局
                    case CeriKeyConst.CARD: // 卡片容器
                    case CeriKeyConst.TABITEM: // 标签面板Item
                    case CeriKeyConst.TABLEGRIDTR: // 表格容器Tr
                    case CeriKeyConst.TABLEGRIDTD: // 表格容器Td
                        template.AddRange(AnalysisTemplateData(config.children));
                        break;
                    case CeriKeyConst.COLLAPSE: // 折叠面板
                    case CeriKeyConst.TAB: // 标签面板
                    case CeriKeyConst.TABLEGRID: // 表格容器
                    case CeriKeyConst.STEPS: // 步骤条
                        config.children.ForEach(item => template.AddRange(AnalysisTemplateData(item.__config__.children)));
                        break;
                    case CeriKeyConst.CERITEXT: // 文本
                    case CeriKeyConst.DIVIDER: // 分割线
                    case CeriKeyConst.GROUPTITLE: // 分组标题
                    case CeriKeyConst.BUTTON: // 按钮
                    case CeriKeyConst.ALERT: // 提示
                    case CeriKeyConst.LINK: // 链接
                    case CeriKeyConst.IFRAME: // iframe
                    case CeriKeyConst.QRCODE: // 二维码
                    case CeriKeyConst.BARCODE: // 条形码
                        break;
                    default:
                        template.Add(item);
                        break;
                }
            }

            return template;
        }
        /// <summary>
        /// 处理日期格式.
        /// </summary>
        public static void DataFormatReplace(List<FieldsModel> fList)
        {
            foreach (FieldsModel item in fList)
            {
                if (item.__config__.ceriKey.Equals(CeriKeyConst.DATE)) item.format = item.format.Replace("YYYY-MM-DD", "yyyy-MM-dd").Replace("YYYY", "yyyy");
                else if (item.__config__.children != null && item.__config__.children.Any()) DataFormatReplace(item.__config__.children);
            }
        }
    }
}
