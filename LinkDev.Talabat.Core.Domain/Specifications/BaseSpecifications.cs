using LinkDev.Talabat.Core.Domain.Common;
using LinkDev.Talabat.Core.Domain.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.Core.Domain.Specifications
{
    public abstract class BaseSpecifications<TEntity, TKey> : ISpecifications<TEntity, TKey> where TEntity : BaseEntity<TKey> where TKey : IEquatable<TKey>
    {
        public Expression<Func<TEntity, bool>>? Criteria { get; set; } = null;
        public List<Expression<Func<TEntity, object>>> Includes { get; set; } = [];
        public Expression<Func<TEntity, object>>? OrderBy { get  ; set ; }
        public Expression<Func<TEntity, object>>? OrderByDesc { get ; set ; }
        public int Skip { get ; set ; }
        public int Take { get; set; }
        public bool IsPagingEnabled { get ; set; }

        public BaseSpecifications(Expression<Func<TEntity,bool>> filter)
        {
            Criteria = filter;

        }

        protected BaseSpecifications(TKey key)
        {
            Criteria=p=>p.Id.Equals(key);
        }

        protected BaseSpecifications()
        {
        }

        private protected virtual void AddIncludes() { }
        private protected virtual void OrderByAsc(Expression<Func<TEntity,object>> orderybyasc)=>OrderBy=orderybyasc;
        private protected virtual void OrderByDes(Expression<Func<TEntity, object>> orderbydesc) => OrderByDesc = orderbydesc;
        private protected  void ApplyPagination(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }
    }
}
