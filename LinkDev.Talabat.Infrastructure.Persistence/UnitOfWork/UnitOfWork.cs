using LinkDev.Talabat.Core.Domain.Common;
using LinkDev.Talabat.Core.Domain.Contract.Persistence;
using LinkDev.Talabat.Infrastructure.Persistence._Data;
using LinkDev.Talabat.Infrastructure.Persistence.Repositories;
using LinkDev.Talabat.Infrastructure.Persistence.Repositories.GenericRepository;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.Infrastructure.Persistence.UnitOfWork
{
    internal class UnitOfWork(StoreDbContext dbContext) : IUnitOfWork
    {
        private readonly ConcurrentDictionary<string, object> repositories = new();
        public async Task<int> CompleteAsync()=> await dbContext.SaveChangesAsync();

        public async ValueTask DisposeAsync()=> await dbContext.DisposeAsync();


        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>()
            where TEntity : BaseEntity<TKey>
            where TKey : IEquatable<TKey>
        {
            //var typename = typeof(TEntity).Name;
            //if (repositories.ContainsKey(typename))
            //    return (IGenericRepository<TEntity, TKey>)repositories[typename];
            //var repo=new GenericRepository<TEntity, TKey>(dbContext);
            //repositories.TryAdd(typename, repo);
            //return repo;

            return (IGenericRepository<TEntity, TKey>)repositories.GetOrAdd(typeof(TEntity).Name, _ => new GenericRepository<TEntity, TKey>(dbContext));

        }
    }
}
