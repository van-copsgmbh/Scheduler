using Quartz.Impl.AdoJobStore.Common;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Corima.Scheduler
{
    public class CustomSqlServerConnectionProvider : IDbProvider
    {
        public CustomSqlServerConnectionProvider()
        {
            this.Metadata = new DbMetadata
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
            this.Metadata.Init();
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
            return new SqlConnection(this.ConnectionString);
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