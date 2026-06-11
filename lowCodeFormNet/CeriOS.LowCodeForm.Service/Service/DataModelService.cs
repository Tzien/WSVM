using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.Model.Model;
using CeriOS.LowCodeForm.Model.ViewModel;
using CeriOS.LowCodeForm.Repository.IRepository;
using CeriOS.LowCodeForm.Service.IService;
using Mapster;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Caching.Distributed;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Service.Service
{
    public class DataModelService : IDataModelService
    {
        private readonly SqlSugarScope? _sqlSugarClient = new SqlSugarScope(new ConnectionConfig());

        private readonly IDataModelRepository _dataModelRepository;

        public DataModelService(IDataModelRepository dataModelRepository)
        {
            _dataModelRepository = dataModelRepository;
        }

        public async Task<DBConfig?> GetDBInfo(string linkId)
        {

            var db = await _dataModelRepository.GetDB(linkId);
            if (db == null)
            {
                return null;
            }
            if (!_sqlSugarClient.IsAnyConnection(linkId))
            {
                var connectionConfig = new ConnectionConfig
                {
                    ConfigId = linkId,
                    DbType = ToDbType(db.DbType),
                    ConnectionString = ToConnectionString(db),
                    IsAutoCloseConnection = true,
                };
                if (connectionConfig.DbType == SqlSugar.DbType.Oracle)
                {
                    connectionConfig.MoreSettings = new ConnMoreSettings()
                    {
                        MaxParameterNameLength = 30,
                    };
                }
                if (connectionConfig.DbType == SqlSugar.DbType.Kdbndp || connectionConfig.DbType == SqlSugar.DbType.Dm)
                {
                    connectionConfig.MoreSettings = new ConnMoreSettings()
                    {
                        IsAutoToUpper = false,
                    };
                }
                _sqlSugarClient.AddConnection(connectionConfig);
                var aa = _sqlSugarClient.IsAnyConnection(linkId);
            }
            return db;

        }

        public async Task<ResponseDto> AddTable(DatabaseTableInfoOutput databaseTableInfoOutput)
        {
            var db = await _dataModelRepository.GetDB(databaseTableInfoOutput.linkId);
            if (db == null)
            {
                return null;
            }
            if (!_sqlSugarClient.IsAnyConnection(databaseTableInfoOutput.linkId))
            {
                var connectionConfig = new ConnectionConfig
                {
                    ConfigId = databaseTableInfoOutput.linkId,
                    DbType = ToDbType(db.DbType),
                    ConnectionString = ToConnectionString(db),
                    IsAutoCloseConnection = true,
                };
                if (connectionConfig.DbType == SqlSugar.DbType.Oracle)
                {
                    connectionConfig.MoreSettings = new ConnMoreSettings()
                    {
                        MaxParameterNameLength = 30,
                    };
                }
                if (connectionConfig.DbType == SqlSugar.DbType.Kdbndp || connectionConfig.DbType == SqlSugar.DbType.Dm)
                {
                    connectionConfig.MoreSettings = new ConnMoreSettings()
                    {
                        IsAutoToUpper = false,
                    };
                }
                _sqlSugarClient.AddConnection(connectionConfig);
                var aa = _sqlSugarClient.IsAnyConnection(databaseTableInfoOutput.linkId);
            }
            if (db == null)
            {
                return new ResponseDto().OK(false, "error");
            }
            var bb = _sqlSugarClient.IsAnyConnection(databaseTableInfoOutput.linkId);
            var isAnyTable = _sqlSugarClient.GetConnection(databaseTableInfoOutput.linkId).DbMaintenance.IsAnyTable(databaseTableInfoOutput.tableInfo.newTable, false);
            if (isAnyTable)
            {
                return new ResponseDto().OK(false, "表已存在");
            }
            var tableInfo = databaseTableInfoOutput.tableInfo.Adapt<DbTableModel>();
            tableInfo.table = databaseTableInfoOutput.tableInfo.newTable;
            var tableFieldList = databaseTableInfoOutput.tableFieldList.Adapt<List<DbTableFieldModel>>();
            var str = await Create(db, tableInfo, tableFieldList);
            return new ResponseDto().OK(true, "操作成功");
        }

        public async Task<ResponseDto> deltable(string linkId, string table)
        {
            var db = await _dataModelRepository.GetDB(linkId);
            if (db == null)
            {
                return null;
            }
            if (!_sqlSugarClient.IsAnyConnection(linkId))
            {
                var connectionConfig = new ConnectionConfig
                {
                    ConfigId = linkId,
                    DbType = ToDbType(db.DbType),
                    ConnectionString = ToConnectionString(db),
                    IsAutoCloseConnection = true,
                };
                if (connectionConfig.DbType == SqlSugar.DbType.Oracle)
                {
                    connectionConfig.MoreSettings = new ConnMoreSettings()
                    {
                        MaxParameterNameLength = 30,
                    };
                }
                if (connectionConfig.DbType == SqlSugar.DbType.Kdbndp || connectionConfig.DbType == SqlSugar.DbType.Dm)
                {
                    connectionConfig.MoreSettings = new ConnMoreSettings()
                    {
                        IsAutoToUpper = false,
                    };
                }
                _sqlSugarClient.AddConnection(connectionConfig);
                var aa = _sqlSugarClient.IsAnyConnection(linkId);
            }
            if (db == null)
            {
                return new ResponseDto().OK(false, "error");
            }
            var bb = _sqlSugarClient.IsAnyConnection(linkId);

            var str = _sqlSugarClient.GetConnection(linkId).DbMaintenance.DropTable(table);
            return new ResponseDto().OK(str, str ? "删除成功" : "删除失败");
        }

        public async Task<ResponseDto> GetDBTables(string linkId, string? tableName, int pageIndex, int pageSize)
        {
            try
            {
                var db = await _dataModelRepository.GetDB(linkId);
                if (db == null)
                {
                    return new ResponseDto().OK(false, "error");
                }
                if (!_sqlSugarClient.IsAnyConnection(linkId))
                {
                    var connectionConfig = new ConnectionConfig
                    {
                        ConfigId = linkId,
                        DbType = ToDbType(db.DbType),
                        ConnectionString = ToConnectionString(db),
                        IsAutoCloseConnection = true,
                    };
                    if (connectionConfig.DbType == SqlSugar.DbType.Oracle)
                    {
                        connectionConfig.MoreSettings = new ConnMoreSettings()
                        {
                            MaxParameterNameLength = 30,
                        };
                    }
                    if (connectionConfig.DbType == SqlSugar.DbType.Kdbndp || connectionConfig.DbType == SqlSugar.DbType.Dm)
                    {
                        connectionConfig.MoreSettings = new ConnMoreSettings()
                        {
                            IsAutoToUpper = false,
                        };
                    }
                    _sqlSugarClient.AddConnection(connectionConfig);
                    var aa = _sqlSugarClient.IsAnyConnection(linkId);
                    //var data = new List<DatabaseTableListOutput>();
                    //var sql = DBTableSql(db.DbType);
                    //if ("postgresql".Equals(db.DbType.ToLower()))
                    //{
                    //    data = _sqlSugarClient.GetConnection(linkId).Ado.SqlQuery<dynamic>(sql).Select(x => new DatabaseTableListOutput { table = x.f_table, tableName = x.f_tablename, sum = x.f_sum }).ToList();
                    //}
                    //else
                    //{
                    //    var modelList = _sqlSugarClient.GetConnection(linkId).Ado.SqlQuery<DynamicDbTableModel>(sql).ToList();
                    //    data = modelList.Select(x => new DatabaseTableListOutput { table = x.F_TABLE, tableName = x.F_TABLENAME, sum = x.F_SUM.ParseToInt() }).ToList();
                    //}
                }
                var result = new QueryResponseDto<List<DbTableInfo>>();
                var data = GetTableInfos(db);
                result.Total = data.Count();
                if (!string.IsNullOrEmpty(tableName))
                    data = data.Where(p => p.Name.Contains(tableName)).ToList();
                result.Data = data.OrderBy(p => p.Name).Skip((pageIndex - 1) * pageSize).Take(pageSize).OrderBy(p => p.Name).ToList();
                result.OK(true, "查询成功");
                return result;
            }
            catch (Exception ex)
            {
                return new ResponseDto { Message = ex.Message + ex.InnerException?.ToString() + ex.StackTrace?.ToString() };
            }
        }

        public const string SYSTEMKEY = "PUBLIC,STRING,VOID,BASE,THIS,USING,CLASS,STRUCT,ABSTRACT,INTERFACE,IS,AS,IN,OUT,REF,OBJECT,INT,DECIMAL,DOUBLE,FLOAT,BOOL,PRIVATE,DELEGATE,DEFAULT,DO,IF,ELSE,SWITCH,CASE,FOR,FOREACH,FALSE,FINALLY,FIXED,INTERNAL,NAMESPACE,OVERRIDE,RETURN,NULL,TRUE,WHILE,CONST,BREAK,CONTINUE,VIRTUAL,TENANTID,ID,FOREIGNID,FLOWID,FLOWTASKID,DELETEUSERID,DELETETIME,DELETEMARK,VERSION,TENANT_ID,FOREIGN_ID,FLOW_ID,FLOW_TASK_ID,DELETE_USER_ID,DELETE_TIME,DELETE_MARK,F_TENANT_ID,F_ID,F_FOREIGN_ID,F_FLOW_ID,F_FLOW_TASK_ID,F_DELETE_USER_ ID,F_DELETE_TIME,F_DELETE_MARK,F_VERSION";

        public async Task<bool> Create(DBConfig link, DbTableModel tableModel, List<DbTableFieldModel> tableFieldList)
        {
            var systemKeyList = SYSTEMKEY.Split(",").ToList();
            var cloumnList = tableFieldList.Adapt<List<DbColumnInfo>>();
            var str = new List<DbColumnInfo>();
            for (int i = 0; i < tableFieldList.Count; i++)
            {
                str.Add(new DbColumnInfo
                {
                    DbColumnName = tableFieldList[i].field,
                    DataType = tableFieldList[i].dataType,
                    Length = int.Parse(tableFieldList[i].dataLength),
                    IsNullable = tableFieldList[i].primaryKey ? false : tableFieldList[i].allowNull == 1,
                    IsPrimarykey = tableFieldList[i].primaryKey,
                    ColumnDescription = tableFieldList[i].fieldName,
                    IsIdentity = tableFieldList[i].identity
                });
            }
            var flag = await DelDataLength(cloumnList);
            var isOk = _sqlSugarClient.GetConnection(link.DBConfigId).DbMaintenance.CreateTable(tableModel.table, str);
            _sqlSugarClient.GetConnection(link.DBConfigId).DbMaintenance.AddTableRemark(tableModel.table, tableModel.tableName);

            if (flag) AddTrigger(tableModel, tableFieldList);

            if (_sqlSugarClient.GetConnection(link.DBConfigId).CurrentConnectionConfig.DbType != SqlSugar.DbType.MySql)
            {
                foreach (var item in cloumnList)
                {
                    _sqlSugarClient.GetConnection(link.DBConfigId).DbMaintenance.AddColumnRemark(item.DbColumnName, tableModel.table, item.ColumnDescription);
                }
            }
            return true;
        }

        private void AddTrigger(DbTableModel tableModel, List<DbTableFieldModel> tableFieldList)
        {
            var tableName = tableModel.table;
            var primaryKey = tableFieldList.Find(it => it.field.ToLower() != "f_tenant_id" && it.primaryKey)?.field;

            // 序列
            var sequence = string.Format(
                "CREATE SEQUENCE {0}_seq\n" +
                "INCREMENT by 1\n" + // 每次增加1
                "START WITH 1\n" + // 从1开始计数
                "NOMAXVALUE\n" + // 无最大值
                "NOCYCLE\n" + // 一直累加，不循环
                "NOCACHE", tableName);

            // 触发器
            var trigger = string.Format(
                "CREATE OR REPLACE TRIGGER AUTO_{0}_tg\n" +
                "BEFORE INSERT ON {0}\n" +
                "FOR EACH ROW\n" +
                "BEGIN\n" +
                "\tSELECT {0}_seq.NEXTVAL INTO :new.{1} FROM dual;\n" +
                "END;", tableName, primaryKey);

            _sqlSugarClient.Ado.ExecuteCommand(sequence);
            _sqlSugarClient.Ado.ExecuteCommand(trigger);
        }

        /// <summary>
        /// 删除列长度(SqlSugar除了字符串其他不需要类型长度).
        /// </summary>
        /// <param name="dbColumnInfos"></param>
        /// <param name="dataTypeDic"></param>
        private async Task<bool> DelDataLength(List<DbColumnInfo> dbColumnInfos, Dictionary<string, string> dataTypeDic = null)
        {
            var flag = false;
            var error = new List<string>();
            foreach (var item in dbColumnInfos)
            {

                if (item.IsIdentity)
                {
                    if ("int".Equals(item.DataType.ToLower()) || "bigint".Equals(item.DataType.ToLower()))
                    {
                        if (_sqlSugarClient.CurrentConnectionConfig.DbType.Equals(SqlSugar.DbType.Oracle))
                            flag = true;
                    }

                }
                if (dataTypeDic == null)
                {
                    ColumnConversion(item, _sqlSugarClient.CurrentConnectionConfig.DbType);
                }
                else
                {
                    if (dataTypeDic.ContainsKey(item.DataType.ToLower()))
                    {
                        item.DataType = dataTypeDic[item.DataType.ToLower().Replace("(默认)", string.Empty)];
                    }
                }
            }


            return flag;
        }


        /// <summary>
        /// 字段转换.
        /// </summary>
        /// <param name="dbColumnInfo">字段.</param>
        /// <param name="databaseType">数据库类型</param>
        private void ColumnConversion(DbColumnInfo dbColumnInfo, SqlSugar.DbType databaseType)
        {
            switch (databaseType)
            {
                case SqlSugar.DbType.MySql:
                    switch (dbColumnInfo.DataType.ToLower())
                    {
                        case "varchar":
                            break;
                        case "int":
                            dbColumnInfo.Length = 0;
                            dbColumnInfo.DecimalDigits = 0;
                            break;
                        case "datetime":
                            dbColumnInfo.Length = 0;
                            dbColumnInfo.DecimalDigits = 0;
                            break;
                        case "decimal":
                            break;
                        case "bigint":
                            dbColumnInfo.Length = 0;
                            dbColumnInfo.DecimalDigits = 0;
                            break;
                        case "text":
                        case "longtext":
                            dbColumnInfo.Length = 0;
                            dbColumnInfo.DecimalDigits = 0;
                            break;
                    }
                    break;
                case SqlSugar.DbType.SqlServer:
                    switch (dbColumnInfo.DataType.ToLower())
                    {
                        case "varchar":
                            break;
                        case "int":
                            dbColumnInfo.Length = 0;
                            dbColumnInfo.DecimalDigits = 0;
                            break;
                        case "datetime":
                            dbColumnInfo.Length = 0;
                            dbColumnInfo.DecimalDigits = 0;
                            break;
                        case "decimal":
                            dbColumnInfo.Length = dbColumnInfo.Length > 38 ? 38 : dbColumnInfo.Length;
                            break;
                        case "bigint":
                            dbColumnInfo.Length = 0;
                            dbColumnInfo.DecimalDigits = 0;
                            break;
                        case "text":
                        case "longtext":
                            dbColumnInfo.DataType = "nvarchar(max)";
                            dbColumnInfo.Length = 0;
                            dbColumnInfo.DecimalDigits = 0;
                            break;
                        default:
                            break;
                    }
                    break;
                case SqlSugar.DbType.Oracle:
                    switch (dbColumnInfo.DataType.ToLower())
                    {
                        case "varchar":
                            dbColumnInfo.DataType = dbColumnInfo.DataType.ToUpper();
                            break;
                        case "int":
                            dbColumnInfo.DataType = dbColumnInfo.DataType.ToUpper();
                            dbColumnInfo.Length = 0;
                            dbColumnInfo.DecimalDigits = 0;
                            break;
                        case "datetime":
                            dbColumnInfo.DataType = "TIMESTAMP";
                            dbColumnInfo.Length = 0;
                            dbColumnInfo.DecimalDigits = 0;
                            break;
                        case "decimal":
                            dbColumnInfo.DataType = dbColumnInfo.DataType.ToUpper();
                            dbColumnInfo.Length = dbColumnInfo.Length > 38 ? 38 : dbColumnInfo.Length;
                            break;
                        case "bigint":
                            dbColumnInfo.DataType = "NUMBER";
                            dbColumnInfo.Length = 0;
                            dbColumnInfo.DecimalDigits = 0;
                            break;
                        case "text":
                        case "longtext":
                            dbColumnInfo.DataType = "CLOB";
                            dbColumnInfo.Length = 0;
                            dbColumnInfo.DecimalDigits = 0;
                            break;
                        default:
                            break;
                    }
                    break;
                case SqlSugar.DbType.PostgreSQL:
                    switch (dbColumnInfo.DataType.ToLower())
                    {
                        case "varchar":
                            break;
                        case "int":
                            dbColumnInfo.DataType = "int4";
                            dbColumnInfo.Length = 0;
                            dbColumnInfo.DecimalDigits = 0;
                            break;
                        case "datetime":
                            dbColumnInfo.DataType = "timestamp";
                            dbColumnInfo.Length = 0;
                            dbColumnInfo.DecimalDigits = 0;
                            break;
                        case "decimal":
                            break;
                        case "bigint":
                            dbColumnInfo.DataType = "int8";
                            dbColumnInfo.Length = 0;
                            dbColumnInfo.DecimalDigits = 0;
                            break;
                        case "text":
                        case "longtext":
                            dbColumnInfo.DataType = "text";
                            dbColumnInfo.Length = 0;
                            dbColumnInfo.DecimalDigits = 0;
                            break;
                        default:
                            break;
                    }
                    break;
                case SqlSugar.DbType.Dm:
                    switch (dbColumnInfo.DataType.ToLower())
                    {
                        case "varchar":
                            dbColumnInfo.DataType = dbColumnInfo.DataType.ToUpper();
                            break;
                        case "int":
                            dbColumnInfo.DataType = dbColumnInfo.DataType.ToUpper();
                            dbColumnInfo.Length = 0;
                            dbColumnInfo.DecimalDigits = 0;
                            break;
                        case "datetime":
                            dbColumnInfo.DataType = "TIMESTAMP";
                            dbColumnInfo.Length = 6;
                            dbColumnInfo.DecimalDigits = 0;
                            break;
                        case "decimal":
                            dbColumnInfo.DataType = dbColumnInfo.DataType.ToUpper();
                            dbColumnInfo.Length = dbColumnInfo.Length > 38 ? 38 : dbColumnInfo.Length;
                            break;
                        case "bigint":
                            dbColumnInfo.DataType = "NUMBER";
                            dbColumnInfo.Length = 0;
                            dbColumnInfo.DecimalDigits = 0;
                            break;
                        case "text":
                        case "longtext":
                            dbColumnInfo.DataType = "CLOB";
                            dbColumnInfo.Length = 0;
                            dbColumnInfo.DecimalDigits = 0;
                            break;
                        default:
                            break;
                    }
                    break;
                case SqlSugar.DbType.Kdbndp:
                    switch (dbColumnInfo.DataType.ToLower())
                    {
                        case "varchar":
                            break;
                        case "int":
                            dbColumnInfo.DataType = "int4";
                            dbColumnInfo.Length = 0;
                            dbColumnInfo.DecimalDigits = 0;
                            break;
                        case "datetime":
                            dbColumnInfo.DataType = "timestamp";
                            dbColumnInfo.Length = 0;
                            dbColumnInfo.DecimalDigits = 0;
                            break;
                        case "decimal":
                            break;
                        case "bigint":
                            dbColumnInfo.DataType = "int8";
                            dbColumnInfo.Length = 0;
                            dbColumnInfo.DecimalDigits = 0;
                            break;
                        case "text":
                        case "longtext":
                            dbColumnInfo.DataType = "text";
                            dbColumnInfo.Length = 0;
                            dbColumnInfo.DecimalDigits = 0;
                            break;
                        default:
                            break;
                    }
                    break;
            }
        }


        public List<DbTableInfo> GetTableInfos(DBConfig link, bool isView = false)
        {
            List<DbTableInfo> data = new List<DbTableInfo>();
            if (isView)
            {
                data = _sqlSugarClient.GetConnection(link.DBConfigId).DbMaintenance.GetViewInfoList(false);
            }
            else
            {
                data = _sqlSugarClient.GetConnection(link.DBConfigId).DbMaintenance.GetTableInfoList((dbtype, sql) =>
                {
                    //if (dbtype == SqlSugar.DbType.Kdbndp)
                    //{
                    //    sql = string.Format("select cast(relname as varchar) as Name,cast(obj_description ( c.relname::regclass ) as varchar) as Description from sys_class c left join sys_namespace n on c.relnamespace=n.oid where  relkind = 'r' and  c.oid > 16384 and c.relnamespace != 99 and n.nspname='{0}' order by relname", link.DbSchema);
                    //}

                    //if (dbtype == SqlSugar.DbType.Dm)
                    //{
                    //    sql = string.Format(" SELECT table_name name,(select TOP 1 COMMENTS from user_tab_comments where t.table_name=table_name )  as Description FROM DBA_TABLES t WHERE OWNER = '{0}' ORDER BY TABLE_NAME;", link.DbSchema);
                    //}

                    return sql;
                });
            }
            return data;
        }

        public SqlSugar.DbType ToDbType(string dbType)
        {
            switch (dbType.ToLower())
            {
                case "sqlserver":
                    return SqlSugar.DbType.SqlServer;
                case "mysql":
                    return SqlSugar.DbType.MySql;
                case "oracle":
                    return SqlSugar.DbType.Oracle;
                case "dm8":
                case "dm":
                    return SqlSugar.DbType.Dm;
                case "kdbndp":
                case "kingbasees":
                    return SqlSugar.DbType.Kdbndp;
                case "postgresql":
                    return SqlSugar.DbType.PostgreSQL;
                default:
                    return SqlSugar.DbType.MySql;
            }
        }

        public async Task<List<DBConfig>> GetDBXia()
        {
            return await _dataModelRepository.GetDBXia();
        }

        public string ToConnectionString(DBConfig dbLinkEntity)
        {
            switch (dbLinkEntity.DbType.ToLower())
            {
                case "sqlserver":
                    return string.Format("Data Source={0},{4};Initial Catalog={1};User ID={2};Password={3};MultipleActiveResultSets=true;Encrypt=True;TrustServerCertificate=True;", dbLinkEntity.Addr, dbLinkEntity.ServiceName, dbLinkEntity.UName, dbLinkEntity.Pwd, dbLinkEntity.Port);
                case "oracle":
                    if (dbLinkEntity.IsMore)
                    {
                        return string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVER = DEDICATED)(SERVICE_NAME={2})));User Id={3};Password={4}", dbLinkEntity.Addr, dbLinkEntity.Port.ToString(), dbLinkEntity.ServiceName, dbLinkEntity.UName, dbLinkEntity.Pwd);
                    }
                    else
                    {
                        return string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVER = DEDICATED)(SERVICE_NAME=ORCL)));User Id={2};Password={3}", dbLinkEntity.Addr, dbLinkEntity.Port.ToString(), dbLinkEntity.UName, dbLinkEntity.Pwd);
                    }

                case "mysql":
                    return string.Format("server={0};port={1};database={2};user={3};password={4};AllowLoadLocalInfile=true", dbLinkEntity.Addr, dbLinkEntity.Port.ToString(), dbLinkEntity.DBName, dbLinkEntity.UName, dbLinkEntity.Pwd);
                case "dm8":
                case "dm":
                    return string.Format("server={0};port={1};database={2};User Id={3};PWD={4}", dbLinkEntity.Addr, dbLinkEntity.Port.ToString(), dbLinkEntity.ServiceName, dbLinkEntity.UName, dbLinkEntity.Pwd);
                case "kdbndp":
                case "kingbasees":
                    return string.Format("server={0};port={1};database={2};UID={3};PWD={4}", dbLinkEntity.Addr, dbLinkEntity.Port.ToString(), dbLinkEntity.ServiceName, dbLinkEntity.UName, dbLinkEntity.Pwd);
                case "postgresql":
                    return string.Format("server={0};port={1};Database={2};User Id={3};Password={4};", dbLinkEntity.Addr, dbLinkEntity.Port.ToString(), dbLinkEntity.ServiceName, dbLinkEntity.UName, dbLinkEntity.Pwd);
                default:
                    return string.Format("server={0};port={1};database={2};user={3};password={4};AllowLoadLocalInfile=true", dbLinkEntity.Addr, dbLinkEntity.Port.ToString(), dbLinkEntity.ServiceName, dbLinkEntity.UName, dbLinkEntity.Pwd);
            }
        }

        public string DBTableSql(string dbType)
        {
            StringBuilder sb = new StringBuilder();
            switch (dbType.ToLower())
            {
                case "sqlserver":
                    sb.Append(@"SELECT s.Name F_TABLE, Convert(nvarchar(max), tbp.value) as F_TABLENAME, b.ROWS F_SUM FROM sysobjects s LEFT JOIN sys.extended_properties as tbp ON s.id = tbp.major_id and tbp.minor_id = 0 AND ( tbp.Name = 'MS_Description' OR tbp.Name is null ) LEFT JOIN sysindexes AS b ON s.id = b.id WHERE s.xtype IN('U') AND (b.indid IN (0, 1))");
                    break;
                case "oracle":
                    sb.Append(@"SELECT table_name F_TABLE , (select COMMENTS from user_tab_comments where t.table_name=table_name ) as F_TABLENAME, T.NUM_ROWS F_SUM from user_tables t where table_name!='HELP' AND table_name NOT LIKE '%$%' AND table_name NOT LIKE 'LOGMNRC_%' AND table_name!='LOGMNRP_CTAS_PART_MAP' AND table_name!='LOGMNR_LOGMNR_BUILDLOG' AND table_name!='SQLPLUS_PRODUCT_PROFILE'");
                    break;
                case "mysql":
                    sb.Append(@"select TABLE_NAME as F_TABLE,TABLE_ROWS as F_SUM ,TABLE_COMMENT as F_TABLENAME from information_schema.tables where TABLE_SCHEMA=(select database()) AND TABLE_TYPE='BASE TABLE'");
                    break;
                case "dm8":
                case "dm":
                    sb.Append(@"SELECT table_name F_TABLE , (select COMMENTS from user_tab_comments where t.table_name=table_name ) as F_TABLENAME, T.NUM_ROWS F_SUM from user_tables t where table_name!='HELP' AND table_name NOT LIKE '%$%' AND table_name NOT LIKE 'LOGMNRC_%' AND table_name!='LOGMNRP_CTAS_PART_MAP' AND table_name!='LOGMNR_LOGMNR_BUILDLOG' AND table_name!='SQLPLUS_PRODUCT_PROFILE'");
                    break;
                case "kdbndp":
                case "kingbasees":
                    sb.Append(@"select a.relname F_TABLE,a.n_live_tup F_SUM,b.description F_TABLENAME from sys_stat_user_tables a left outer join sys_description b on a.relid = b.objoid where a.schemaname='public' and b.objsubid='0'");
                    break;
                case "postgresql":
                    sb.Append(@"select cast(relname as varchar) as F_TABLE,cast(reltuples as int) as F_SUM, cast(obj_description(relfilenode,'pg_class') as varchar) as F_TABLENAME from pg_class c inner join pg_namespace n on n.oid = c.relnamespace and nspname='public' inner join pg_tables z on z.tablename=c.relname where relkind = 'r' and relname not like 'pg_%' and relname not like 'sql_%' and schemaname='public' order by relname");
                    break;
                default:
                    throw new Exception("不支持");
            }

            return sb.ToString();
        }


        public async Task<ResponseDto> UpdateTable(DatabaseTableInfoOutput databaseTableInfoOutput)
        {
            var db = await _dataModelRepository.GetDB(databaseTableInfoOutput.linkId);
            if (db == null)
            {
                return new ResponseDto().OK(false, "error");
            }
            if (!_sqlSugarClient.IsAnyConnection(databaseTableInfoOutput.linkId))
            {
                var connectionConfig = new ConnectionConfig
                {
                    ConfigId = databaseTableInfoOutput.linkId,
                    DbType = ToDbType(db.DbType),
                    ConnectionString = ToConnectionString(db),
                    IsAutoCloseConnection = true,
                };
                if (connectionConfig.DbType == SqlSugar.DbType.Oracle)
                {
                    connectionConfig.MoreSettings = new ConnMoreSettings()
                    {
                        MaxParameterNameLength = 30,
                    };
                }
                if (connectionConfig.DbType == SqlSugar.DbType.Kdbndp || connectionConfig.DbType == SqlSugar.DbType.Dm)
                {
                    connectionConfig.MoreSettings = new ConnMoreSettings()
                    {
                        IsAutoToUpper = false,
                    };
                }
                _sqlSugarClient.AddConnection(connectionConfig);
                var aa = _sqlSugarClient.IsAnyConnection(databaseTableInfoOutput.linkId);
            }



            var isAnyTable = _sqlSugarClient.GetConnection(databaseTableInfoOutput.linkId).DbMaintenance.IsAnyTable(databaseTableInfoOutput.tableInfo.table, false);
            bool delst = false;
            if (isAnyTable)
            {
                delst = _sqlSugarClient.GetConnection(databaseTableInfoOutput.linkId).DbMaintenance.DropTable(databaseTableInfoOutput.tableInfo.table);
            }
            if (delst)
            {
                var tableInfo = databaseTableInfoOutput.tableInfo.Adapt<DbTableModel>();
                tableInfo.table = databaseTableInfoOutput.tableInfo.newTable;
                var tableFieldList = databaseTableInfoOutput.tableFieldList.Adapt<List<DbTableFieldModel>>();
                var str = await Create(db, tableInfo, tableFieldList);
                return new ResponseDto().OK(true, "操作成功");
            }
            else
            {
                return new ResponseDto().OK(false, "操作失败");
            }

        }


        public async Task<ResponseDto> GetTableAsync(string linkId, string tableName)
        {
            var db = await _dataModelRepository.GetDB(linkId);
            if (db == null)
            {
                return new ResponseDto().OK(false, "error");
            }
            if (!_sqlSugarClient.IsAnyConnection(linkId))
            {
                var connectionConfig = new ConnectionConfig
                {
                    ConfigId = linkId,
                    DbType = ToDbType(db.DbType),
                    ConnectionString = ToConnectionString(db),
                    IsAutoCloseConnection = true,
                };
                if (connectionConfig.DbType == SqlSugar.DbType.Oracle)
                {
                    connectionConfig.MoreSettings = new ConnMoreSettings()
                    {
                        MaxParameterNameLength = 30,
                    };
                }
                if (connectionConfig.DbType == SqlSugar.DbType.Kdbndp || connectionConfig.DbType == SqlSugar.DbType.Dm)
                {
                    connectionConfig.MoreSettings = new ConnMoreSettings()
                    {
                        IsAutoToUpper = false,
                    };
                }
                _sqlSugarClient.AddConnection(connectionConfig);
                var aa = _sqlSugarClient.IsAnyConnection(linkId);
            }
            var str = new QueryByIdResponseDto<DatabaseTableInfoOutput>();
            var sqlCon = _sqlSugarClient.GetConnection(db.DBConfigId).DbMaintenance.GetTableInfoList(false).Find(m => m.Name == tableName);

            var sqlList = _sqlSugarClient.GetConnection(db.DBConfigId).DbMaintenance.GetColumnInfosByTableName(tableName, false);
            var tfl = new List<TableFieldOutput>();
            foreach (var item in sqlList)
            {
                tfl.Add(new TableFieldOutput
                {
                    primaryKey = item.IsPrimarykey ? 1 : 0,
                    allowNull = item.IsNullable ? 1 : 0,
                    dataLength = item.Length,
                    dataType = item.DataType,
                    field = item.DbColumnName,
                    fieldName = item.ColumnDescription,
                    identity = 0
                });
            }
            str.Data = new DatabaseTableInfoOutput()
            {
                tableInfo = new TableInfoOutput()
                {
                    table = sqlCon.Name,
                    newTable = sqlCon.Name,
                    tableName = sqlCon.Description
                },
                tableFieldList = tfl
            };
            str.OK(true, " 查询成功");
            str.Data.tableFieldList = ViewDataTypeConversion(str.Data.tableFieldList, _sqlSugarClient.CurrentConnectionConfig.DbType);
            return str;
        }

        public List<TableFieldOutput> ViewDataTypeConversion(List<TableFieldOutput> fields, SqlSugar.DbType databaseType)
        {
            foreach (var item in fields)
            {
                item.dataType = item.dataType.ToLower();
                switch (item.dataType)
                {
                    case "string":
                    case "guid":
                    case "byte[]":
                    case "nvarchar":
                    case "nvarchar2":
                    case "varchar2":
                        {
                            item.dataType = "varchar";
                            if (item.dataLength.ParseToInt() > 2000 || item.dataLength.ParseToInt() == -1)
                            {
                                item.dataType = "text";
                                item.dataLength = 50;
                            }
                        }
                        break;
                    case "single":
                        item.dataType = "decimal";
                        break;
                    case "int16":
                    case "int32":
                        item.dataType = "int";
                        break;
                    case "int64":
                        item.dataType = "bigint";
                        break;
                    case "timestamp":
                        item.dataType = "datetime";
                        break;
                }
            }

            return fields;
        }
    }



    public static class Extensions
    {
        public static int ParseToInt(this object str)
        {
            try
            {
                return Convert.ToInt32(str);
            }
            catch
            {
                return 0;
            }
        }
    }




}
