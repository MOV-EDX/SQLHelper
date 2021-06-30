using SqlBuilder;
using SqlBuilder.Reflector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SqlHelper.SqlHelperService
{
    public class SqlHelperService : ISqlHelperService
    {
        public async Task<IList<TEntity>> ExecuteQueryAsync<TEntity>(StoredProcedureProperties properties) where TEntity : class, new()
        {
            IList<TEntity> result = new List<TEntity>();

            if (properties is null) throw new ArgumentNullException(nameof(properties));

            if (string.IsNullOrEmpty(properties.ConnectionString)) throw new ArgumentException("The connection string is null or empty");

            using (SqlConnection connection = new SqlConnection(properties.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(properties.StoredProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    foreach (KeyValuePair<string, object> parameter in properties.Parameters)
                    {
                        command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                    }

                    try
                    {
                        await connection.OpenAsync();
                        SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                //TODO: Map the result set to the specified entity using Reflection
                                TEntity entity = Reflector.ReflectType<TEntity>(reader);
                                result.Add(entity);
                            }
                        }
                    }
                    catch (SqlException e)
                    {
                        Console.WriteLine(e);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    finally
                    {
                        await connection.CloseAsync();
                        await command.DisposeAsync();
                    }
                }
            }

            return result;
        }

        public async Task<int> ExecuteNonQueryAsync(StoredProcedureProperties properties)
        {
            int result = 0;

            if (properties is null) throw new ArgumentNullException(nameof(properties));

            if (string.IsNullOrEmpty(properties.ConnectionString)) throw new ArgumentException("The connection string is null or empty");

            using (SqlConnection connection = new SqlConnection(properties.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(properties.StoredProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    foreach (KeyValuePair<string, object> parameter in properties.Parameters)
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

                    try
                    {
                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();

                        if (properties.OutputParameter is not OutputParameterTypes.NoOutput)
                        {
                            string parameter = Enum.GetName(typeof(OutputParameterTypes), properties.OutputParameter);
                            result = Convert.ToInt32(command.Parameters[$"{parameter}"].Value);
                        }
                    }
                    catch (SqlException e)
                    {
                        Console.WriteLine(e);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    finally
                    {
                        await connection.CloseAsync();
                        await command.DisposeAsync();
                    }
                }
            }

            return result;
        }
    }
}
