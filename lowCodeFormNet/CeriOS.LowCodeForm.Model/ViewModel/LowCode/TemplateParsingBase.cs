using CeriOS.LowCodeForm.Model.Model;
using CeriOS.LowCodeForm.Model.ViewModel.Engine;
using CERIOS.Common.Const;
using CERIOS.Common.Extension;
using CERIOS.Engine.Entity.Model;
using Newtonsoft.Json;

namespace CeriOS.LowCodeForm.Model.ViewModel.LowCode
{
    public class TemplateParsingBase
    {
        public TemplateParsingBase() { }

        /// <summary>
        /// 模板实体.
        /// </summary>
        public FormDbDto visualDevEntity { get; set; }


        /// <summary>
        /// 页面类型 （1、纯表单，2、表单加列表，3、表单列表工作流）.
        /// </summary>
        public int WebType { get; set; }

        /// <summary>
        /// 表单配置JSON模型.
        /// </summary>
        public FormDataModel? FormModel { get; set; }

        /// <summary>
        /// 列配置JSON模型.
        /// </summary>
        public ColumnDesignModel ColumnData { get; set; }

        /// <summary>
        /// App列配置JSON模型.
        /// </summary>
        public ColumnDesignModel AppColumnData { get; set; }

        /// <summary>
        /// 所有控件集合.
        /// </summary>
        public List<FieldsModel> AllFieldsModel { get; set; }

        /// <summary>
        /// 所有控件集合(已剔除布局控件).
        /// </summary>
        public List<FieldsModel> FieldsModelList { get; set; }

        /// <summary>
        /// 主表控件集合.
        /// </summary>
        public List<FieldsModel> MainTableFieldsModelList { get; set; }

        /// <summary>
        /// 副表控件集合.
        /// </summary>
        public List<FieldsModel> AuxiliaryTableFieldsModelList { get; set; }

        /// <summary>
        /// 子表控件集合.
        /// </summary>
        public List<FieldsModel> ChildTableFieldsModelList { get; set; }

        /// <summary>
        /// 主/副表控件集合(列表展示数据控件).
        /// </summary>
        public List<FieldsModel> SingleFormData { get; set; }

        /// <summary>
        /// 所有表.
        /// </summary>
        public List<FormDb> AllTable { get; set; }

        /// <summary>
        /// 主表
        /// </summary>
        public FormDb? MainTable { get; set; }

        /// <summary>
        /// 主表 表名.
        /// </summary>
        public string? MainTableName { get; set; }

        /// <summary>
        /// 主/副表 系统生成控件集合.
        /// </summary>
        public List<FieldsModel> GenerateFields { get; set; }

        /// <summary>
        /// 主表 vModel 字段 字典.
        /// Key : vModel , Value : 主表.vModel.
        /// </summary>
        public Dictionary<string, string> MainTableFields { get; set; }

        /// <summary>
        /// 副表 vModel 字段 字典.
        /// Key : vModel , Value : 副表.vModel.
        /// </summary>
        public Dictionary<string, string> AuxiliaryTableFields { get; set; }

        /// <summary>
        /// 子表 vModel 字段 字典.
        /// Key : 设计子表-vModel , Value : 子表.vModel.
        /// </summary>
        public Dictionary<string, string> ChildTableFields { get; set; }

        /// <summary>
        /// 所有表 vModel 字段 字典.
        /// Key : 设计子表-vModel , Value : 表.vModel.
        /// </summary>
        public Dictionary<string, string> AllTableFields { get; set; }

        /// <summary>
        /// 功能名称.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 主表主键名.
        /// </summary>
        public string MainPrimary { get; set; }

        /// <summary>
        /// 数据库连接.
        /// </summary>
        public DBConfig DbLink { get; set; }

        /// <summary>
        /// 导入模式.(1 仅新增，2 更新和新增数据).
        /// </summary>
        public string dataType { get; set; } = "1";

        /// <summary>
        /// 导入数据列表.
        /// </summary>
        public List<string> selectKey { get; set; }


        /// <summary>
        /// 模板解析帮助 构造.
        /// </summary>
        /// <param name="entity">功能实体.</param>
        public TemplateParsingBase(FormDbDto entity)
        {
            visualDevEntity = entity;
            WebType = entity.WebType.Value;

            // 数据视图
            if (entity.WebType.Equals(4))
            {
                FullName = entity.Name;
                //IsHasTable = false;
                InitColumnData(entity);
                AllFieldsModel = new List<FieldsModel>();
                ColumnData.columnList.ForEach(item =>
                {
                    AllFieldsModel.Add(new FieldsModel() { __vModel__ = item.__vModel__, __config__ = new ConfigModel() { label = item.label, ceriKey = item.__config__.ceriKey } });
                });
                AppColumnData.columnList.ForEach(item =>
                {
                    AllFieldsModel.Add(new FieldsModel() { __vModel__ = item.__vModel__, __config__ = new ConfigModel() { label = item.label, ceriKey = item.__config__.ceriKey } });
                });
                AllFieldsModel = AllFieldsModel.DistinctBy(x => x.__vModel__).ToList();
                FieldsModelList = AllFieldsModel;
                AuxiliaryTableFieldsModelList = AllFieldsModel;
                MainTableFieldsModelList = AllFieldsModel;
                SingleFormData = AllFieldsModel;
            }
            else
            {
                var tables = JsonConvert.SerializeObject(entity.FormDbs);
                //FormDataModel formModel = entity.FormData.ToObjectOld<FormDataModel>();
                FormDataModel formModel = JsonConvert.DeserializeObject<FormDataModel>(entity.FormJson);
                DataFormatReplace(formModel.fields);
                FormModel = formModel; // 表单Json模型
                //IsHasTable = !string.IsNullOrEmpty(entity.Tables) && !"[]".Equals(entity.Tables); // 是否有表
                AllFieldsModel = TemplateAnalysis.AnalysisTemplateData(formModel.fields); // 所有控件集合
                FieldsModelList = TemplateAnalysis.AnalysisTemplateData(formModel.fields); // 已剔除布局控件集合
                MainTable = JsonConvert.DeserializeObject<List<FormDb>>(tables).Find(m => m.TypeId == 1); // 主表
                //MainTable = entity.Tables.ToList<TableModel>().Find(m => m.typeId.Equals("1")); // 主表
                MainTableName = MainTable?.TableName; // 主表名称
                AddChlidTableFeildsModel();

                // 处理旧控件 部分没有 tableName
                FieldsModelList.Where(x => string.IsNullOrWhiteSpace(x.__config__.tableName)).ToList().ForEach(item =>
                {
                    if (item.__vModel__.Contains("_ceri_")) item.__config__.tableName = item.__vModel__.ReplaceRegex(@"_ceri_(\w+)", string.Empty).Replace("ceri_", string.Empty); // 副表
                    else item.__config__.tableName = MainTableName != null ? MainTableName : string.Empty; // 主表
                });
                //AllTable = entity.Tables.ToObject<List<TableModel>>(); // 所有表
                AllTable = JsonConvert.DeserializeObject<List<FormDb>>(tables); //所有表 

                AuxiliaryTableFieldsModelList = FieldsModelList.Where(x => x.__vModel__.Contains("_ceri_")).ToList(); // 单控件副表集合
                ChildTableFieldsModelList = FieldsModelList.Where(x => x.__config__.ceriKey == CeriKeyConst.TABLE).ToList(); // 子表集合
                MainTableFieldsModelList = FieldsModelList.Except(AuxiliaryTableFieldsModelList).Except(ChildTableFieldsModelList).ToList(); // 主表控件集合
                SingleFormData = FieldsModelList.Where(x => x.__config__.ceriKey != CeriKeyConst.TABLE).ToList(); // 非子表集合
                GenerateFields = GetGenerateFields(); // 系统生成控件

                MainTableFields = new Dictionary<string, string>();
                AuxiliaryTableFields = new Dictionary<string, string>();
                ChildTableFields = new Dictionary<string, string>();
                AllTableFields = new Dictionary<string, string>();
                MainTableFieldsModelList.Where(x => x.__vModel__.IsNotEmptyOrNull()).ToList().ForEach(x =>
                {
                    MainTableFields.Add(x.__vModel__, x.__config__.tableName + "." + x.__vModel__);
                    AllTableFields.Add(x.__vModel__, x.__config__.tableName + "." + x.__vModel__);
                });
                AuxiliaryTableFieldsModelList.Where(x => x.__vModel__.IsNotEmptyOrNull()).ToList().ForEach(x =>
                {
                    AuxiliaryTableFields.Add(x.__vModel__, x.__vModel__.Replace("_ceri_", ".").Replace("ceri_", string.Empty));
                    AllTableFields.Add(x.__vModel__, x.__vModel__.Replace("_ceri_", ".").Replace("ceri_", string.Empty));
                });
                ChildTableFieldsModelList.ForEach(item =>
                {
                    item.__config__.children.Where(x => x.__vModel__.IsNotEmptyOrNull()).ToList().ForEach(x =>
                    {
                        ChildTableFields.Add(item.__vModel__ + "-" + x.__vModel__, item.__config__.tableName + "." + x.__vModel__);
                        AllTableFields.Add(item.__vModel__ + "-" + x.__vModel__, item.__config__.tableName + "." + x.__vModel__);
                    });
                });
                InitColumnData(entity);
            }
        }

        /// <summary>
        /// 模板解析帮助 构造 (功能表单).
        /// </summary>
        /// <param name="formJson">表单Json.</param>
        /// <param name="tables">涉及表Json.</param>
        public TemplateParsingBase(string formJson, string tables)
        {
            InitByFormType(formJson, tables, 2);
        }

        /// <summary>
        /// 模板解析帮助 构造.
        /// </summary>
        /// <param name="formJson">表单Json.</param>
        /// <param name="tables">涉及表Json.</param>
        /// <param name="formType">表单类型（1：自定义表单 2：系统表单）.</param>
        public TemplateParsingBase(string formJson, string tables, int formType)
        {
            InitByFormType(formJson, tables, formType);
        }

        /// <summary>
        /// 根据表单类型初始化.
        /// </summary>
        /// <param name="formJson">表单Json.</param>
        /// <param name="tables">涉及表Json.</param>
        /// <param name="formType">表单类型（1：自定义表单 2：系统表单）.</param>
        private void InitByFormType(string formJson, string tables, int formType)
        {
            if (formType.Equals(2))
            {
                AllFieldsModel = new List<FieldsModel>();
                var fields = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(formJson) ?? default(List<Dictionary<string, object>>);
                fields.ForEach(it =>
                {
                    if (it.ContainsKey("filedId"))
                        AllFieldsModel.Add(new FieldsModel() { __vModel__ = it["filedId"].ToString(), __config__ = new ConfigModel() { label = it["filedName"].ToString(), ceriKey = CeriKeyConst.COMINPUT } });
                });
                FieldsModelList = AllFieldsModel;
            }
            else
            {
                FormDataModel formModel = JsonConvert.DeserializeObject<FormDataModel>(formJson);
                DataFormatReplace(formModel.fields);
                FormModel = formModel; // 表单Json模型
                //IsHasTable = !string.IsNullOrEmpty(tables) && !"[]".Equals(tables) && tables.IsNullOrEmpty(); // 是否有表
                AllFieldsModel = TemplateAnalysis.AnalysisTemplateData(formModel.fields); // 所有控件集合
                FieldsModelList = TemplateAnalysis.AnalysisTemplateData(formModel.fields); // 已剔除布局控件集合
                MainTable = JsonConvert.DeserializeObject<List<FormDb>>(tables).Find(m => m.TypeId.Equals("1")); // 主表
                MainTableName = MainTable?.TableName; // 主表名称
                AddChlidTableFeildsModel();

                // 处理旧控件 部分没有 tableName
                FieldsModelList.Where(x => string.IsNullOrWhiteSpace(x.__config__.tableName)).ToList().ForEach(item =>
                {
                    if (item.__vModel__.Contains("_ceri_")) item.__config__.tableName = item.__vModel__.ReplaceRegex(@"_ceri_(\w+)", string.Empty).Replace("ceri_", string.Empty); // 副表
                    else item.__config__.tableName = MainTableName != null ? MainTableName : string.Empty; // 主表
                });
                AllTable = JsonConvert.DeserializeObject<List<FormDb>>(tables); //所有表 
                AuxiliaryTableFieldsModelList = FieldsModelList.Where(x => x.__vModel__.Contains("_ceri_")).ToList(); // 单控件副表集合
                ChildTableFieldsModelList = FieldsModelList.Where(x => x.__config__.ceriKey == CeriKeyConst.TABLE).ToList(); // 子表集合
                MainTableFieldsModelList = FieldsModelList.Except(AuxiliaryTableFieldsModelList).Except(ChildTableFieldsModelList).ToList(); // 主表控件集合
                SingleFormData = FieldsModelList.Where(x => x.__config__.ceriKey != CeriKeyConst.TABLE).ToList(); // 非子表集合
                GenerateFields = GetGenerateFields(); // 系统生成控件

                MainTableFields = new Dictionary<string, string>();
                AuxiliaryTableFields = new Dictionary<string, string>();
                ChildTableFields = new Dictionary<string, string>();
                AllTableFields = new Dictionary<string, string>();
                MainTableFieldsModelList.Where(x => x.__vModel__.IsNotEmptyOrNull()).ToList().ForEach(x =>
                {
                    MainTableFields.Add(x.__vModel__, x.__config__.tableName + "." + x.__vModel__);
                    AllTableFields.Add(x.__vModel__, x.__config__.tableName + "." + x.__vModel__);
                });
                AuxiliaryTableFieldsModelList.Where(x => x.__vModel__.IsNotEmptyOrNull()).ToList().ForEach(x =>
                {
                    AuxiliaryTableFields.Add(x.__vModel__, x.__vModel__.Replace("_ceri_", ".").Replace("ceri_", string.Empty));
                    AllTableFields.Add(x.__vModel__, x.__vModel__.Replace("_ceri_", ".").Replace("ceri_", string.Empty));
                });
                ChildTableFieldsModelList.ForEach(item =>
                {
                    item.__config__.children.Where(x => x.__vModel__.IsNotEmptyOrNull()).ToList().ForEach(x =>
                    {
                        ChildTableFields.Add(item.__vModel__ + "-" + x.__vModel__, item.__config__.tableName + "." + x.__vModel__);
                        AllTableFields.Add(item.__vModel__ + "-" + x.__vModel__, item.__config__.tableName + "." + x.__vModel__);
                    });
                });

                ColumnData = new ColumnDesignModel();
                AppColumnData = new ColumnDesignModel();
            }
        }

        /// <summary>
        /// 处理日期格式.
        /// </summary>
        private void DataFormatReplace(List<FieldsModel> fList)
        {
            foreach (FieldsModel item in fList)
            {
                if (item.__config__.ceriKey.Equals(CeriKeyConst.DATE)) item.format = item.format.Replace("YYYY-MM-DD", "yyyy-MM-dd").Replace("YYYY", "yyyy");
                else if (item.__config__.children != null && item.__config__.children.Any()) DataFormatReplace(item.__config__.children);
            }
        }



        /// <summary>
        /// 处理子表内的控件 添加到所有控件.
        /// </summary>
        private void AddChlidTableFeildsModel()
        {
            var ctList = new List<FieldsModel>();
            AllFieldsModel.Where(x => x.__config__.ceriKey.Equals(CeriKeyConst.TABLE)).ToList().ForEach(item =>
            {
                item.__config__.children.Where(it => it.__vModel__.IsNotEmptyOrNull()).ToList().ForEach(it => it.__vModel__ = item.__vModel__ + "-" + it.__vModel__);
                ctList.AddRange(TemplateAnalysis.AnalysisTemplateData(item.__config__.children));
            });
            AllFieldsModel.AddRange(ctList);
        }

        /// <summary>
        /// 获取系统生成字段空格键.
        /// </summary>
        /// <returns></returns>
        public List<FieldsModel> GetGenerateFields()
        {
            // 系统生成字段 key
            var gfList = new List<string>() { CeriKeyConst.BILLRULE, CeriKeyConst.CREATEUSER, CeriKeyConst.CREATETIME, CeriKeyConst.MODIFYUSER, CeriKeyConst.MODIFYTIME, CeriKeyConst.CURRPOSITION, CeriKeyConst.CURRORGANIZE, CeriKeyConst.UPLOADFZ };

            return SingleFormData.Where(x => gfList.Contains(x.__config__.ceriKey)).ToList();
        }

        /// <summary>
        /// 初始化列配置模型.
        /// </summary>
        private void InitColumnData(FormDbDto entity)
        {
            if (!string.IsNullOrWhiteSpace(entity.ColumnDataStr)) ColumnData = JsonConvert.DeserializeObject<ColumnDesignModel>(entity.ColumnDataStr); // 列配置模型
            else ColumnData = new ColumnDesignModel();

            if (!string.IsNullOrWhiteSpace(entity.AppColumnData)) AppColumnData = JsonConvert.DeserializeObject<ColumnDesignModel>(entity.AppColumnDataStr);  // 列配置模型
            else AppColumnData = new ColumnDesignModel();

            FullName = entity.Name;

            if (ColumnData.uploaderTemplateJson != null && ColumnData.uploaderTemplateJson.selectKey != null)
            {
                dataType = ColumnData.uploaderTemplateJson.dataType;
                selectKey = new List<string>();

                // 列顺序
                AllFieldsModel.ForEach(item =>
                {
                    if (ColumnData.uploaderTemplateJson.selectKey.Any(x => x.Equals(item.__vModel__))) selectKey.Add(item.__vModel__);
                });
            }
        }

    }
}
