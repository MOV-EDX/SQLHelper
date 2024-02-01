using System.Collections.Generic;
using System.Threading.Tasks;

namespace SqlMapper.Reader
{
    public interface IResultSetReader
    {
        Task<IEnumerable<TEntity>> ReadAsync<TEntity>(int predictedRows) where TEntity : class;
    }
}
