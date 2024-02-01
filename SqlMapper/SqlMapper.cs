using SqlMapper.Common;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using SqlMapper.Reader;

namespace SqlMapper
{
    public class SqlMapper : ISqlMapper
    {
        public async Task<IResultSetReader> ExecuteQueryAsync(string connectionString, string query)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException(nameof(connectionString));
            if (string.IsNullOrWhiteSpace(query)) throw new ArgumentException(nameof(query));

            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(query, connection);

            SqlDataReader reader = null;

            try
            {
                command.CommandType = CommandType.Text;

                await connection.OpenAsync();
                reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                return new ResultSetReader(reader);
            }
            catch
            {
                if (reader is not null)
                {
                    if (!reader.IsClosed)
                    {
                        try
                        {
                            command.Cancel();
                        }
                        catch { }
                    }

                    await reader.DisposeAsync();
                }

                throw;
            }
        }

        public async Task<IResultSetReader> ExecuteQueryAsync(StoredProcedureProperties properties)
        {
            if (properties is null) throw new ArgumentNullException(nameof(properties));

            using var connection = new SqlConnection(properties.ConnectionString);
            using var command = new SqlCommand(properties.StoredProcedureName, connection);

            SqlDataReader reader = null;

            try
            {
                command.CommandType = CommandType.StoredProcedure;

                foreach (var parameter in properties.Parameters)
                {
                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }

                await connection.OpenAsync();
                reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                return new ResultSetReader(reader);
            }
            catch
            {
                if (reader is not null)
                {
                    if (!reader.IsClosed)
                    {
                        try
                        {
                            command.Cancel();
                        }
                        catch { }
                    }

                    await reader.DisposeAsync();
                }

                throw;
            }
        }

        public async Task<int> ExecuteNonQueryAsync(StoredProcedureProperties properties)
        {
            if (properties is null) throw new ArgumentNullException(nameof(properties));

            var result = 0;

            using var connection = new SqlConnection(properties.ConnectionString);
            using var command = new SqlCommand(properties.StoredProcedureName, connection);

            try
            {
                command.CommandType = CommandType.StoredProcedure;

                foreach (var parameter in properties.Parameters)
                {
                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }

                //Add any output parameter if appropriate
                if (properties.OutputParameter is not OutputParameterTypes.NoOutput)
                {
                    string parameter = Enum.GetName(typeof(OutputParameterTypes), properties.OutputParameter);

                    switch (properties.OutputParameter)
                    {
                        case OutputParameterTypes.Output:
                        case OutputParameterTypes.Identifier:
                        case OutputParameterTypes.RowCount:
                            command.Parameters.Add($"@{parameter}", SqlDbType.Int).Direction = ParameterDirection.Output;
                            break;
                        case OutputParameterTypes.ReturnValue:
                            command.Parameters.Add($"@{parameter}", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                            break;
                        default:
                            break;
                    }
                }

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                if (properties.OutputParameter is not OutputParameterTypes.NoOutput)
                {
                    string parameter = Enum.GetName(typeof(OutputParameterTypes), properties.OutputParameter);
                    result = Convert.ToInt32(command.Parameters[$"{parameter}"].Value);
                }

                return result;
            }
            catch
            {
                throw;
            }
        }
    }
}
