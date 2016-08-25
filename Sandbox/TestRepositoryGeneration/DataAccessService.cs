using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using VersionedRepositoryGeneration.Interfaces;

namespace VersionedRepositoryGeneration
{
    public class DataAccessService : IDataAccessService
    {
        private readonly IDataAccessContextProvider _dataAccessContextProvider;

        public DataAccessService(IDataAccessContextProvider dataAccessContextProvider)
        {
            _dataAccessContextProvider = dataAccessContextProvider;
        }

        private IDbConnection GetConnection()
        {
            var connectionString = _dataAccessContextProvider.Connection;
            return new SqlConnection(connectionString);
        }

        public async Task<IEnumerable<T>> GetAsync<T>(string sql, object parameterValues) where T : new()
        {
            var statem = GenerateFullSqlStatement(sql, typeof(T));
            IEnumerable<T> result;
            try
            {
                result = await GetConnection().QueryAsync<T>(statem, parameterValues).ConfigureAwait(false);
            }
            catch (SqlException e)
            {
                e.Data.Add("sql-exception-details", GetSqlInfo(statem, parameterValues));
                throw;
            }
            //LogConnectionPull("Get<T> 2");
            return result;
        }


        public async Task<IEnumerable<TReturn>> GetMultipleAsync<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object parameterValues = null, string splitOn = "Id")
        {
            IEnumerable<TReturn> result;
            sql = GenerateFullSqlStatement(sql, typeof(TReturn));
            try
            {
                result = await GetConnection().QueryAsync(sql, map, parameterValues, splitOn: splitOn).ConfigureAwait(false);
            }
            catch (SqlException e)
            {
                e.Data.Add("sql-exception-details", GetSqlInfo(sql, parameterValues));
                throw;
            }
            //LogConnectionPull("Get<T> 2");
            return result;
        }

        public async Task<IEnumerable<TReturn>> GetMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object parameterValues = null, string splitOn = "Id")
        {
            IEnumerable<TReturn> result;
            sql = GenerateFullSqlStatement(sql, typeof(TReturn));
            try
            {
                result = await GetConnection().QueryAsync(sql, map, parameterValues, splitOn: splitOn).ConfigureAwait(false);
            }
            catch (SqlException e)
            {
                e.Data.Add("sql-exception-details", GetSqlInfo(sql, parameterValues));
                throw;
            }
            //LogConnectionPull("Get<T> 2");
            return result;
        }

        public async Task<IEnumerable<TReturn>> GetMultipleAsync<TReturn>(string sql, Type[] types, Func<object[], TReturn> map, object parameterValues, string splitOn)
        {
            IEnumerable<TReturn> result;
            sql = GenerateFullSqlStatement(sql, typeof(TReturn));
            try
            {
                result = await GetConnection().QueryAsync(sql, map: map, types: types, splitOn: splitOn, param: parameterValues).ConfigureAwait(false);
            }
            catch (SqlException e)
            {
                e.Data.Add("sql-exception-details", GetSqlInfo(sql, parameterValues));
                throw;
            }
            //LogConnectionPull("Get<T> 2");
            return result;
        }


        public IEnumerable<T> Get<T>(string sql, object parameterValues) where T : new()
        {
            var statem = GenerateFullSqlStatement(sql, typeof(T));
            IEnumerable<T> result;
            try
            {
                result = GetConnection().Query<T>(statem, parameterValues);
            }
            catch (SqlException e)
            {
                e.Data.Add("sql-exception-details", GetSqlInfo(sql, parameterValues));
                throw;
            }
            //LogConnectionPull("Get<T> 2");
            return result;
        }

        public bool IsTenantExist(Type t)
        {
            if (t.GetInterface(typeof(ITenantUnrelated).Name) != null)
                return false;

            return true;
        }

        public object ExecuteScalar<T>(string sql, object parameterValues)
        {
            sql = GenerateFullSqlStatement(sql, typeof(T));
            object result;
            try
            {
                result = GetConnection().ExecuteScalar(sql, parameterValues);
            }
            catch (SqlException e)
            {
                e.Data.Add("sql-exception-details", GetSqlInfo(sql, parameterValues));
                throw;
            }
            //LogConnectionPull("ExecuteScalar 1");
            return result;
        }

        public async Task<object> ExecuteScalarAsync<T>(string sql, object parameterValues)
        {
            sql = GenerateFullSqlStatement(sql, typeof(T));
            object result;
            try
            {
                result = await GetConnection().ExecuteScalarAsync(sql, parameterValues).ConfigureAwait(false);
            }
            catch (SqlException e)
            {
                e.Data.Add("sql-exception-details", GetSqlInfo(sql, parameterValues));
                throw;
            }
            //LogConnectionPull("ExecuteScalar 2");
            return result;
        }

        public object ExecuteScalar(string sql, object parameterValues)
        {
            object result;
            try
            {
                result = GetConnection().ExecuteScalar(sql, parameterValues);
            }
            catch (SqlException e)
            {
                e.Data.Add("sql-exception-details", GetSqlInfo(sql, parameterValues));
                throw;
            }
            //LogConnectionPull("ExecuteScalar 2");
            return result;
        }

        public async Task<object> InsertObjectAsync<T>(T obj, string statement)
        {
            if (PreprocessUpdate(obj))
            {
                if (obj == null)
                {
                    throw new ArgumentNullException("operation on empty object");
                }

                var columns = "";
                var values = "";

                if (statement.Contains("{columns}"))
                {
                    statement = statement.Replace("{values}", values);
                    statement = statement.Replace("{columns}", columns);
                }

                object result;
                try
                {
                    var type = typeof(T);
                        result = await GetConnection().ExecuteScalarAsync<object>(statement, obj).ConfigureAwait(false);

                }
                catch (SqlException e)
                {
                    e.Data.Add("sql-exception-details", GetSqlInfo(statement, obj));
                    throw;
                }
                //LogConnectionPull("InsertObject 1");
                return result;

            }
            return null;
        }

        public object InsertObject<T>(T obj, string statement)
        {
            if (PreprocessUpdate(obj))
            {
                if (obj == null)
                {
                    throw new ArgumentNullException("operation on empty object");
                }

                var columns = "";
                var values = "";

                if (statement.Contains("{columns}"))
                {
                    statement = statement.Replace("{values}", values);
                    statement = statement.Replace("{columns}", columns);
                }

                object result;
                try
                {
                    var type = typeof(T);
                        result = GetConnection().ExecuteScalar<object>(statement, obj);
                }
                catch (SqlException e)
                {
                    e.Data.Add("sql-exception-details", GetSqlInfo(statement, obj));
                    throw;
                }
                //LogConnectionPull("InsertObject 1");
                return result;

            }
            return null;
        }

        public async Task PersistObjectAsync<T>(T obj, string statement)
        {
            statement = GenerateFullSqlStatement(statement, typeof(T));
            if (PreprocessUpdate(obj))
            {
                if (obj == null)
                {
                    throw new ArgumentNullException("operation on empty object");
                }

                try
                {
                    await GetConnection().ExecuteAsync(statement, obj).ConfigureAwait(false);
                }
                catch (SqlException e)
                {
                    e.Data.Add("sql-exception-details", GetSqlInfo(statement, obj));
                    throw;
                }
                //LogConnectionPull("PersistObject<T> 1");
            }
        }

        public void PersistObject<T>(T obj, string statement)
        {
            statement = GenerateFullSqlStatement(statement, typeof(T));
            if (PreprocessUpdate(obj))
            {
                if (obj == null)
                {
                    throw new ArgumentNullException("operation on empty object");
                }

                try
                {
                    GetConnection().Execute(statement, obj);
                }
                catch (SqlException e)
                {
                    e.Data.Add("sql-exception-details", GetSqlInfo(statement, obj));
                    throw;
                }
                //LogConnectionPull("PersistObject<T> 1");
            }
        }

        public async Task PersistObjectAsync<T>(string statement, object parameterValues)
        {
            statement = GenerateFullSqlStatement(statement, typeof(T));
            try
            {
                await GetConnection().ExecuteAsync(statement, parameterValues).ConfigureAwait(false);
            }
            catch (SqlException e)
            {
                e.Data.Add("sql-exception-details", GetSqlInfo(statement, parameterValues));
                throw;
            }
            //LogConnectionPull("PersistObject<T> 1");
        }

        public void PersistObject<T>(string statement, object parameterValues)
        {
            statement = GenerateFullSqlStatement(statement, typeof(T));
            try
            {
                GetConnection().Execute(statement, parameterValues);
            }
            catch (SqlException e)
            {
                e.Data.Add("sql-exception-details", GetSqlInfo(statement, parameterValues));
                throw;
            }
            //LogConnectionPull("PersistObject<T> 1");
        }

        public async Task TruncateAsync(string tableName)
        {
            var sql = string.Format("TRUNCATE TABLE [dbo].[{0}]", tableName);
            try
            {
                await GetConnection().ExecuteAsync(sql).ConfigureAwait(false);
            }
            catch (SqlException e)
            {
                e.Data.Add("sql-exception-details", GetSqlInfo(sql, null));
                throw;
            }
            //LogConnectionPull("Truncate");
            // Debug.WriteLine("Truncated");
        }

        public void Truncate(string tableName)
        {
            var sql = string.Format("TRUNCATE TABLE [dbo].[{0}]", tableName);
            try
            {
                GetConnection().Execute(sql);
            }
            catch (SqlException e)
            {
                e.Data.Add("sql-exception-details", GetSqlInfo(sql, null));
                throw;
            }
            //LogConnectionPull("Truncate");
            // Debug.WriteLine("Truncated");
        }

        private bool PreprocessUpdate<T>(T obj)
        {
            return true;
        }

        private string GenerateWhere(string statem)
        {
            if (statem.Contains("{wheretenantid:}"))
                statem = statem.Replace("{wheretenantid:}", " WHERE [TenantId] = '");
            if (statem.Contains("{andtenantid:}"))
                statem = statem.Replace("{andtenantid:}", " AND [TenantId] = '");
            return statem;
        }

        private string GenerateWhereWithTableName(string statem)
        {
            if (statem.Contains("{wheretenantid:"))
            {
                var firstIndex = statem.IndexOf('{');
                var lastIndex = statem.IndexOf('}');
                var sequence = statem.Substring(firstIndex, lastIndex - firstIndex + 1);
                var index = sequence.IndexOf(':');
                var tableName = sequence.Substring(index + 1, sequence.Length - index - 2);
                statem = statem.Replace(sequence, " WHERE " + tableName + ".[TenantId] = '");
            }

            if (statem.Contains("{andtenantid:"))
            {
                var firstIndex = statem.IndexOf('{');
                var lastIndex = statem.IndexOf('}');
                var sequence = statem.Substring(firstIndex, lastIndex - firstIndex + 1);
                var index = sequence.IndexOf(':');
                var tableName = sequence.Substring(index + 1, sequence.Length - index - 2);
                statem = statem.Replace(sequence, " AND " + tableName + ".[TenantId] = '");
            }
            return statem;
        }

        public string GenerateFullSqlStatement(string statem, Type t)
        {
            var sql = statem.ToLower();

            return sql;
        }

        public async Task<object> ExecuteAsync(string sql)
        {
            object result;
            try
            {
                result = await GetConnection().ExecuteScalarAsync<object>(sql).ConfigureAwait(false);
            }
            catch (SqlException e)
            {
                e.Data.Add("sql-exception-details", GetSqlInfo(sql, null));
                throw;
            }
            //LogConnectionPull("Execute 2");
            return result;
        }

        public object Execute(string sql)
        {
            object result;
            try
            {
                result = GetConnection().ExecuteScalar<object>(sql);
            }
            catch (SqlException e)
            {
                e.Data.Add("sql-exception-details", GetSqlInfo(sql, null));
                throw;
            }
            //LogConnectionPull("Execute 2");
            return result;
        }

        public async Task ExecuteAsync(string sql, object parameters)
        {
            try
            {
                await GetConnection().ExecuteAsync(sql, parameters).ConfigureAwait(false);
            }
            catch (SqlException e)
            {
                e.Data.Add("sql-exception-details", GetSqlInfo(sql, parameters));
                throw;
            }
            //LogConnectionPull("Execute 2");
        }

        public void Execute(string sql, object parameters)
        {
            try
            {
                GetConnection().Execute(sql, parameters);
            }
            catch (SqlException e)
            {
                e.Data.Add("sql-exception-details", GetSqlInfo(sql, parameters));
                throw;
            }
            //LogConnectionPull("Execute 2");
        }

        public async Task<IEnumerable<TResult>> GetMultipleMappingAsync<TFirst, TSecond, TThird, TResult>(string sql, Func<TFirst, TSecond, TThird, TResult> map, object parameterValues, string splitOn)
        {
            sql = GenerateFullSqlStatement(sql, typeof(TResult));
            IEnumerable<TResult> result;
            try
            {
                result = await GetConnection().QueryAsync(sql, map, parameterValues, splitOn: splitOn).ConfigureAwait(false);
            }
            catch (SqlException e)
            {
                e.Data.Add("sql-exception-details", GetSqlInfo(sql, parameterValues));
                throw;
            }
            return result;
        }

        /// <summary> 
        ///     Get info about sql script in which occurred error. 
        ///     This info to be used in NLog extension.
        /// </summary>
        private static string GetSqlInfo(string sql, object[] parameterValues)
        {
            var parameters = parameterValues.Where(o => o != null).ToArray();
            return string.Format("Command: {0}\nParameter values: {1}", sql ?? "", string.Join(";", parameters));
        }

        private static string GetSqlInfo(string sql, object parameterValues)
        {
            var parameters = "";
            if (parameterValues != null)
            {
                var inst = parameterValues.GetType().GetProperties();
                parameters = string.Join(";", inst.ToDictionary(property => property.Name, property => property.GetValue(parameterValues, null)));
            };
            return string.Format("Command: {0}\nParameter values: {1}", sql ?? "", parameters);
        }

    }

}