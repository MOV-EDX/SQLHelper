using System.Collections.Generic;
using System.Threading.Tasks;

namespace SqlHelper
{
    public interface ISqlHelper
    {
        Task<IList<TEntity>> ExecuteQueryAsync<TEntity>(StoredProcedureProperties properties) where TEntity : class, new();
        Task<int> ExecuteNonQueryAsync(StoredProcedureProperties properties);
    }
}
