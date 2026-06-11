using CERIOS.Common.Const;
using CERIOS.Common.Extension;
using CERIOS.Engine.Entity.Model;

namespace CeriOS.LowCodeForm.Model.Helper
{
    public class CodeGenFieldJudgeHelper
    {
        /// <summary>
        /// 列查询类型.
        /// </summary>
        /// <param name="searchList">模板内查询列表.</param>
        /// <param name="fieldName">字段名称.</param>
        /// <returns></returns>
        public static int ColumnQueryType(List<IndexSearchFieldModel>? searchList, string fieldName)
        {
            var column = searchList?.Find(s => s.id == fieldName);
            return column?.searchType ?? 0;
        }
        /// <summary>
        /// 列表查询多选.
        /// </summary>
        /// <param name="searchList">模板内查询列表.</param>
        /// <param name="fieldName">字段名称.</param>
        /// <returns></returns>
        public static bool ColumnQueryMultiple(List<IndexSearchFieldModel>? searchList, string fieldName)
        {
            var column = searchList?.Find(s => s.id == fieldName);
            return (column?.searchMultiple).ParseToBool();
        }

        /// <summary>
        /// 获取是否多选.
        /// </summary>
        /// <param name="columnList">模板内控件列表.</param>
        /// <param name="fieldName">字段名称.</param>
        /// <returns></returns>
        public static bool IsMultipleColumn(List<FieldsModel> columnList, string fieldName)
        {
            bool isMultiple = false;
            var column = columnList.Find(s => s.__vModel__ == fieldName);
            if (column != null)
            {
                switch (column?.__config__.ceriKey)
                {
                    default:
                        isMultiple = column.multiple;
                        break;
                }
            }

            return isMultiple;
        }

        /// <summary>
        /// 获取是否多选.
        /// </summary>
        /// <param name="column">模板内控件.</param>
        /// <param name="fieldName">字段名称.</param>
        /// <returns></returns>
        public static bool IsMultipleColumn(FieldsModel column, string fieldName)
        {
            bool isMultiple = false;
            if (column != null)
            {
                switch (column?.__config__.ceriKey)
                {
                    default:
                        isMultiple = column.multiple;
                        break;
                }
            }

            return isMultiple;
        }

        /// <summary>
        /// 是否datetime.
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static bool IsDateTime(FieldsModel? fields)
        {
            bool isDateTime = false;
            if (fields?.__config__.ceriKey == CeriKeyConst.DATE || fields?.__config__.ceriKey == CeriKeyConst.TIME)
                isDateTime = true;
            return isDateTime;
        }
    }
}
