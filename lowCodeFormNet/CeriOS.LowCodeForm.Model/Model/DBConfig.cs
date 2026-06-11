using SqlSugar;
using CeriOS.Core.Model;
using CeriOS.LowCodeForm.Model.ViewModel;
namespace CeriOS.LowCodeForm.Model.Model
{

    public class DBConfig : PublicProperty
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, Length = 36)]
        public string DBConfigId { get; set; }
        /// <summary>
        /// 连接驱动
        /// </summary>
        [SugarColumn(IsNullable = true,  Length = 100)]
        public string? DbType { get; set; }
        /// <summary>
        /// 连接名称
        /// </summary>
        [SugarColumn(IsNullable = true,  Length = 100)]
        public string? Name { get; set; }
        /// <summary>
        /// 主机地址
        /// </summary>
        [SugarColumn(IsNullable = true,  Length = 100)]
        public string? Addr { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 10)]
        public string? Port { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 100)]
        public string? UName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 100)]
        public string? Pwd { get; set; }
        /// <summary>
        /// 库名
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 100)]
        public string? DBName { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [SugarColumn()]
        public int? Sort { get; set; }

        /// <summary>
        /// 是否更多
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public bool IsMore { get; set; }
        /// <summary>
        /// 连接方式
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string? Connectmethod { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string? Role { get; set; }
        /// <summary>
        /// 服务名
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string? ServiceName { get; set; }







    }
}