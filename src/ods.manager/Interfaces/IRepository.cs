using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Theradex.ODS.Manager.Interfaces
{
    public interface ODSNpgsqlLogger<T>
    {
        void Add(T entity);
        Task AddAsync(T entity);

        Task AddMultipleAsync(List<T> records);
        void AddMultiple(List<T> records);
        T GetById(int id);
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        List<T> GetAll();
        void Update(T entity);
        void Delete(T entity);
        void Delete(Expression<Func<T, bool>> where);
    }
    public interface IBatchRunControlRepository<T> : ODSNpgsqlLogger<T>
    {
        void Started(T entity);
        Task StartedAsync(T entity);
        void Completed(T entity);
        Task CompletedAsync(T entity);
        void CompletedError(T entity, string error);
        Task CompletedErrorAsync(T entity, string error);
        T GetNextBatch(string tableName);
        Task<T> GetNextBatchAsync(string tableName);
        T GetByTableNameAndId(string tableName, int Id);
        Task<T> GetByTableNameAndIdAsync(string tableName, int Id);
        Task<List<T>> HasActiveJobsAsync(string tableName);
        List<T> HasActiveJobs(string tableName);
    }
}

