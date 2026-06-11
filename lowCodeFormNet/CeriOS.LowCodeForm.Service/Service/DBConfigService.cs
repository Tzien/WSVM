using CeriOS.LowCodeForm.Model.Model;
using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.Service.IService;
using CeriOS.LowCodeForm.Repository.IRepository;
using CeriOS.LowCodeForm.Model.ViewModel;
using SqlSugar;
using CeriOS.Core.Model.Model;
using Microsoft.Extensions.Hosting;
using System.Net;
using Dm;
using ColumnInfo = CeriOS.LowCodeForm.Model.ViewModel.ColumnInfo;
using System.Data;
using DbType = SqlSugar.DbType;
using Newtonsoft.Json;
namespace CeriOS.LowCodeForm.Service.Service
{

    public class DBConfigService : IDBConfigService
    {
        public IDBConfigRepository _repository { get; set; }

        public DBConfigService(IDBConfigRepository repository)
        {
            _repository = repository;
        }


        public async Task<QueryResponseDto<List<DBConfig>>> GetDBConfigAsync(int pageIndex, int pageSize, string? name, string? dbType)
        {
            return await _repository.GetDBConfigAsync(pageIndex, pageSize, name, dbType);
        }

        public async Task<ResponseDto> InsertDBConfigAsync(DBConfig parameter)
        {
            parameter.DBConfigId = Guid.NewGuid().ToString("N");

            var result = await _repository.InsertDBConfigAsync(parameter);
            return new ResponseDto().OK(result, result ? "添加成功" : "添加失败");
        }

        public async Task<ResponseDto> UpdateDBConfigAsync(DBConfig parameter)
        {
            var result = await _repository.UpdateDBConfigAsync(parameter);
            return new ResponseDto().OK(result, result ? "修改成功" : "修改失败");
        }


        public async Task<ResponseDto> DeleteDBConfigAsync(string id)
        {
            var result = await _repository.DeleteDBConfigAsync(id);
            return new ResponseDto().OK(result, result ? "删除成功" : "删除失败");
        }

        // 根据ID查询
        public async Task<QueryByIdResponseDto<DBConfig>> GetDBConfigByIdAsync(string id)
        {
            var dto = new QueryByIdResponseDto<DBConfig>();
            dto.Data = await _repository.GetDBConfigByIdAsync(id);
            dto.OK(true, "查询成功");
            return dto;
        }

        public async Task<bool> TestDb(DBConfig config)
        {
            try
            {
                var connectionString = BuildConnectionString(config);
                var dbType = GetDbType(config.DbType);

                 
                using (var db = new SqlSugarScope(new ConnectionConfig()
                {
                    ConnectionString = connectionString,
                    DbType = dbType,
                    IsAutoCloseConnection = true
                }))
                {
                    db.Ado.Open();
                    return db.Ado.IsValidConnection();
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 构建连接字符串
        /// </summary>
        private static string BuildConnectionString(DBConfig config)
        {
            if (string.IsNullOrEmpty(config.DbType))
            {
                throw new ArgumentException("DbType 不能为空");
            }

            return config.DbType.ToLower() switch
            {
                "mysql" => $"server={config.Addr};Port={config.Port};Database={config.DBName};Uid={config.UName};Pwd={config.Pwd};AllowLoadLocalInfile=true",
                "sqlserver" => $"Server={config.Addr},{config.Port};Database={config.DBName};User Id={config.UName};Password={config.Pwd};",
                "oracle" => $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={config.Addr})(PORT={config.Port}))(CONNECT_DATA=(SERVICE_NAME={config.ServiceName})));Persist Security Info = True;User ID={config.UName};Password={config.Pwd};",
                "postgresql" => $"Host={config.Addr};Port={config.Port};Database={config.DBName};Username={config.UName};Password={config.Pwd};",
                var dbType when dbType == "dm8" || dbType == "dm" => string.Format("server={0};port={1};database={2};User Id={3};PWD={4}", config.Addr, config.Port, config.DBName, config.UName, config.Pwd),
                _ => throw new NotSupportedException($"不支持的数据库类型: {config.DbType}")
            };
        }

        /// <summary>
        /// 获取 DbType
        /// </summary>
        private static DbType GetDbType(string dbType)
        {
            if (string.IsNullOrEmpty(dbType))
            {
                throw new ArgumentException("DbType 不能为空");
            }

            return dbType.ToLower() switch
            {
                "mysql" => DbType.MySql,
                "sqlserver" => DbType.SqlServer,
                "oracle" => DbType.Oracle,
                "postgresql" => DbType.PostgreSQL,
                var type when type == "dm8" || type == "dm" => DbType.Dm,
                _ => throw new NotSupportedException($"不支持的数据库类型: {dbType}")
            };
        }

        public async Task<QueryResponseDto<List<DbSelectDto>>> GetDbSelect()
        {
            var data = new List<DbSelectDto>();
            var all = await _repository.GetDBConfigAsync(1, 999, null, null);
            var types = all.Data.Select(x => x.DbType).Distinct().ToList();
            foreach (var type in types)
            {
                DbSelectDto dbSelectDto = new DbSelectDto();
                dbSelectDto.DbType = type;
                dbSelectDto.Items = new List<DbSelectItemDto>();
                var items = all.Data.Where(x => x.DbType == type);  
                foreach (var item in items)
                {
                    dbSelectDto.Items.Add(new DbSelectItemDto()
                    {
                        DbConfigId = item.DBConfigId,
                        Name = item.Name
                    });
                }
                data.Add(dbSelectDto);
            }
            return new QueryResponseDto<List<DbSelectDto>>() { Code = 200, Success = true, Data = data };
        }





        /// <summary>
        /// 获取所有表名和表注释
        /// </summary>
        public async Task<List<TableInfo>> GetAllTables(string dbConfigId,string? tableName)
        {
            var config = await _repository.GetDBConfigByIdAsync(dbConfigId);
            if (config == null)
                return new List<TableInfo>();
            var connectionString = BuildConnectionString(config);
            var dbType = GetDbType(config.DbType);


            using (var db = new SqlSugarScope(new ConnectionConfig()
            {
                ConnectionString = connectionString,
                DbType = dbType,
                IsAutoCloseConnection = true
            }))
            {
                db.Ado.Open();
                var tables = new List<TableInfo>();

                string sql = "";
                switch (dbType)
                {
                    case DbType.MySql:
                        sql = @"
                SELECT 
                    TABLE_NAME AS Name,
                    IFNULL(TABLE_COMMENT, '') AS Description
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_SCHEMA = DATABASE()";
                        break;

                    case DbType.SqlServer:
                        sql = @"
                SELECT 
                    t.name AS Name,
                    ISNULL(ep.value, '') AS Description
                FROM sys.tables t
                LEFT JOIN sys.extended_properties ep ON 
                    ep.major_id = t.object_id AND 
                    ep.minor_id = 0 AND 
                    ep.name = 'MS_Description'";
                        break;

                    case DbType.PostgreSQL:
                        sql = @"
                SELECT 
                    tablename AS Name,
                    COALESCE(obj_description(relid), '') AS Description
                FROM pg_catalog.pg_tables
                WHERE schemaname = 'public'";
                        break;

                    case DbType.Oracle:
                        sql = @"
                SELECT 
                    TABLE_NAME AS Name,
                    NVL(COMMENTS, '') AS Description
                FROM USER_TAB_COMMENTS";
                        break;

                    default:
                        throw new NotSupportedException($"不支持的数据库类型: {dbType}");
                }
                var tableData = db.Ado.GetDataTable(sql);

                foreach (System.Data.DataRow row in tableData.Rows)
                {
                    if (!string.IsNullOrEmpty(tableName))
                    {
                        if(tableName== row["Name"].ToString())
                        {

                            var tableInfo = new TableInfo();
                            tableInfo.Name = row["Name"].ToString();
                            tableInfo.Description = row["Description"].ToString();
                            tableInfo.ColumnInfos = GetTableColumns(db, tableInfo.Name, dbType);
                            tables.Add(tableInfo);
                        }
                    }
                    else
                    {
                        var tableInfo = new TableInfo();
                        tableInfo.Name = row["Name"].ToString();
                        tableInfo.Description = row["Description"].ToString();
                        tableInfo.ColumnInfos = GetTableColumns(db, tableInfo.Name, dbType);
                        tables.Add(tableInfo);
                    }
                }
                return tables;
            }


        }

        /// <summary>
        /// 获取单个表的所有字段信息
        /// </summary>
        private List<ColumnInfo> GetTableColumns(SqlSugarScope _db, string tableName, DbType _dbType)
        {
            return _dbType switch
            {
                DbType.MySql => _db.Ado.SqlQuery<ColumnInfo>(@"
    SELECT 
        COLUMN_NAME AS Name,
        DATA_TYPE AS DataType,
        COLUMN_COMMENT AS Description,
        IS_NULLABLE = 'YES' AS IsNullable,
        COALESCE(CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION) AS Length,
        CASE WHEN COLUMN_KEY = 'PRI' THEN 1 ELSE 0 END AS IsPrimaryKey
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE 
        TABLE_SCHEMA = DATABASE() 
        AND TABLE_NAME = @TableName",
    new { TableName = tableName }),

                DbType.SqlServer => _db.Ado.SqlQuery<ColumnInfo>(@"
            SELECT 
                c.name AS Name,
                t.name AS DataType,
                ep.value AS Description,
                c.is_nullable AS IsNullable,
                c.max_length AS Length,
                CASE WHEN pk.column_id IS NOT NULL THEN 1 ELSE 0 END AS IsPrimaryKey
            FROM sys.columns c
            JOIN sys.types t ON c.user_type_id = t.user_type_id
            LEFT JOIN sys.extended_properties ep ON 
                ep.major_id = c.object_id AND 
                ep.minor_id = c.column_id AND 
                ep.name = 'MS_Description'
            LEFT JOIN (
                SELECT ic.column_id, ic.object_id 
                FROM sys.index_columns ic
                JOIN sys.indexes i ON ic.object_id = i.object_id AND ic.index_id = i.index_id
                WHERE i.is_primary_key = 1
            ) pk ON c.object_id = pk.object_id AND c.column_id = pk.column_id
            WHERE c.object_id = OBJECT_ID(@TableName)",
                    new { TableName = tableName }),

                DbType.PostgreSQL => _db.Ado.SqlQuery<ColumnInfo>(@"
            SELECT 
                c.column_name AS Name,
                c.data_type AS DataType,
                d.description AS Description,
                c.is_nullable = 'YES' AS IsNullable,
                c.character_maximum_length AS Length,
                (SELECT EXISTS (
                    SELECT 1
                    FROM information_schema.table_constraints tc
                    JOIN information_schema.key_column_usage kcu 
                        ON tc.constraint_name = kcu.constraint_name
                    WHERE tc.constraint_type = 'PRIMARY KEY'
                    AND tc.table_name = c.table_name
                    AND kcu.column_name = c.column_name
                )) AS IsPrimaryKey
            FROM information_schema.columns c
            LEFT JOIN pg_catalog.pg_description d 
                ON d.objoid = (quote_ident(c.table_schema) || '.' || quote_ident(c.table_name))::regclass::oid 
                AND d.objsubid = c.ordinal_position
            WHERE 
                c.table_schema = 'public' 
                AND c.table_name = @TableName",
                    new { TableName = tableName.ToLower() }),

                DbType.Oracle => _db.Ado.SqlQuery<ColumnInfo>(@"
            SELECT 
                c.COLUMN_NAME AS Name,
                c.DATA_TYPE AS DataType,
                cc.COMMENTS AS Description,
                CASE c.NULLABLE WHEN 'Y' THEN 1 ELSE 0 END AS IsNullable,
                c.DATA_LENGTH AS Length,
                CASE WHEN pk.COLUMN_NAME IS NOT NULL THEN 1 ELSE 0 END AS IsPrimaryKey
            FROM USER_TAB_COLUMNS c
            LEFT JOIN USER_COL_COMMENTS cc 
                ON cc.TABLE_NAME = c.TABLE_NAME AND cc.COLUMN_NAME = c.COLUMN_NAME
            LEFT JOIN (
                SELECT col.COLUMN_NAME 
                FROM USER_CONSTRAINTS con
                JOIN USER_CONS_COLUMNS col ON con.CONSTRAINT_NAME = col.CONSTRAINT_NAME
                WHERE con.CONSTRAINT_TYPE = 'P' AND col.TABLE_NAME = :TableName
            ) pk ON pk.COLUMN_NAME = c.COLUMN_NAME
            WHERE c.TABLE_NAME = :TableName",
                    new { TableName = tableName.ToUpper() }),

                _ => throw new NotSupportedException($"不支持的数据库类型: {_dbType}")
            };
        }

        public async Task<string> ExecuteSql(string id, string sql)
        {
            try
            {

                var config = await _repository.GetDBConfigByIdAsync(id);
                var connectionString = BuildConnectionString(config);
                var dbType = GetDbType(config.DbType);


                using (var db = new SqlSugarScope(new ConnectionConfig()
                {
                    ConnectionString = connectionString,
                    DbType = dbType,
                    IsAutoCloseConnection = true
                }))
                {
                    db.Ado.Open();


                    string sqlTrim = sql.Trim().ToLower();

                    if (sqlTrim.StartsWith("select"))
                    {
                        var dt = await db.Ado.GetDataTableAsync(sql);
                        return JsonConvert.SerializeObject(dt);
                    }
                    else
                    {
                        try
                        {
                            await db.BeginTranAsync();
                            var sqlList = sql.Split(";").ToList();
                            foreach (var sqlstr in sqlList)
                            {
                                if(!string.IsNullOrEmpty(sqlstr))
                                    await db.Ado.ExecuteCommandAsync(sqlstr);
                            }
                            await db.CommitTranAsync();
                            return "执行成功";
                        }
                        catch (Exception e)
                        {
                            await db.RollbackTranAsync();
                            return "执行失败："+e.Message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

}