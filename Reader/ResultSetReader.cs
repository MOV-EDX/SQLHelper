using SqlMapper.Reflection;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SqlMapper.Reader
{
    public class ResultSetReader : IDisposable, IResultSetReader
    {
        private readonly SqlDataReader _reader;
        private bool _disposedValue;

        public ResultSetReader(SqlDataReader reader)
        {
            _reader = reader;
        }

        public async Task<IEnumerable<TEntity>> ReadAsync<TEntity>(int predictedRows = 1) where TEntity : class
        {
            var rows = new List<TEntity>(predictedRows);

            if (_reader.HasRows)
            {
                while (await _reader.ReadAsync())
                {
                    var entity = Mapper.MapRecord<TEntity>(_reader);
                    rows.Add(entity);
                }
            }

            rows.TrimExcess();

            if (!await _reader.NextResultAsync())
            {
                await _reader.DisposeAsync();
            }

            return rows;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _reader.Close();
                    _reader.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
