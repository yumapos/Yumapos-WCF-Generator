using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestRepositoryGeneration.Infrastructure
{
    public interface IDataAccessService
    {
        Task<IEnumerable<T>> GetAsync<T>(string sql, object parameterValues) where T : new();
        Task<IEnumerable<TReturn>> GetMultipleAsync<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object parameterValues, string splitOn);
        Task<IEnumerable<TReturn>> GetMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object parameterValues, string splitOn);
        Task<IEnumerable<TReturn>> GetMultipleAsync<TReturn>(string sql, Type[] types, Func<object[], TReturn> map, object parameterValues, string splitOn);

        IEnumerable<T> Get<T>(string sql, object parameterValues) where T : new();
        bool IsTenantExist(Type t);
        Task<object> ExecuteScalarAsync<T>(string sql, object parameterValues);
        object ExecuteScalar<T>(string sql, object parameterValues);
        object ExecuteScalar(string sql, object parameterValues);
        Task<object> InsertObjectAsync<T>(T obj, string statement);
        object InsertObject<T>(T obj, string statement);
        Task PersistObjectAsync<T>(T obj, string statement);
        void PersistObject<T>(T obj, string statement);
        Task PersistObjectAsync<T>(string statement, object parameterValues);
        void PersistObject<T>(string statement, object parameterValues);
        Task TruncateAsync(string tableName);
        void Truncate(string tableName);
        string GenerateFullSqlStatement(string statem, Type t);
        Task<object> ExecuteAsync(string sql);
        object Execute(string sql);
        Task ExecuteAsync(string sql, object parameters);
        void Execute(string sql, object parameters);

        Task<IEnumerable<TResult>> GetMultipleMappingAsync<TFirst, TSecond, TThird, TResult>(string sql,
            Func<TFirst, TSecond, TThird, TResult> map, object parameterValues, string splitOn);
    }
}
