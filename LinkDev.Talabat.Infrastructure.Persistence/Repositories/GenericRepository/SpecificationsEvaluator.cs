using LinkDev.Talabat.Core.Domain.Common;
using LinkDev.Talabat.Core.Domain.Contract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.Infrastructure.Persistence.Repositories.GenericRepository
{
    public static class SpecificationsEvaluator<TEntity,TKey> where TEntity : BaseEntity<TKey> where TKey : IEquatable<TKey>
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecifications<TEntity, TKey> specifications)
        {
            var query = inputQuery;
            if (specifications.Criteria is not null)
                query = query.Where(specifications.Criteria);

            if(specifications.OrderBy is not null)
                query=query.OrderBy(specifications.OrderBy);
            if (specifications.OrderByDesc is not null)
                query=query.OrderByDescending(specifications.OrderByDesc);

            if (specifications.IsPagingEnabled)
            {
                query = query.Skip(specifications.Skip).Take(specifications.Take);
            }




            query =specifications.Includes.Aggregate(query, (current, include) => current.Include(include));



            return query;

        }
    }
}
