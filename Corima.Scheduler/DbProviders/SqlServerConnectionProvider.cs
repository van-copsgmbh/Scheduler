using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Quartz.Impl.AdoJobStore.Common;

namespace Corima.Scheduler.DbProviders
{
    public class SqlServerConnectionProvider : IDbProvider
    {
        public SqlServerConnectionProvider()
        {
            Metadata = new DbMetadata
            {
                AssemblyName = typeof(SqlConnection).AssemblyQualifiedName,
                BindByName = true,
                CommandType = typeof(SqlCommand),
                ConnectionType = typeof(SqlConnection),
                DbBinaryTypeName = "VarBinary",
                ExceptionType = typeof(SqlException),
                ParameterDbType = typeof(SqlDbType),
                ParameterDbTypePropertyName = "SqlDbType",
                ParameterNamePrefix = "@",
                ParameterType = typeof(SqlParameter),
                UseParameterNamePrefixInParameterCollection = true
            };
            Metadata.Init();
        }

        public void Initialize()
        {
        }

        public DbCommand CreateCommand()
        {
            return new SqlCommand();
        }

        public DbConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public string ConnectionString
        {
            get => System.Configuration.ConfigurationManager.ConnectionStrings["MSSQL_ConnectionString"].ConnectionString;
            set => System.Configuration.ConfigurationManager.ConnectionStrings["MSSQL_ConnectionString"].ConnectionString = value;
        }

        public DbMetadata Metadata { get; }

        public void Shutdown()
        {
        }
    }
}