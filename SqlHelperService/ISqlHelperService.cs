using System.Collections.Generic;
using System.Threading.Tasks;

namespace SqlHelper.SqlHelperService
{
    public interface ISqlHelperService
    {
        Task<IList<TEntity>> ExecuteQueryAsync<TEntity>(StoredProcedureProperties properties) where TEntity : class, new();
        Task<int> ExecuteNonQueryAsync(StoredProcedureProperties properties);
    }
}
