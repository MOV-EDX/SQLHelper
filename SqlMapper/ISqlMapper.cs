using SqlMapper.Common;
using SqlMapper.Reader;
using System.Threading.Tasks;

namespace SqlMapper
{
    public interface ISqlMapper
    {
        Task<IResultSetReader> ExecuteQueryAsync(string connectionString, string query);
        Task<IResultSetReader> ExecuteQueryAsync(StoredProcedureProperties properties);
        Task<int> ExecuteNonQueryAsync(StoredProcedureProperties properties);
    }
}
