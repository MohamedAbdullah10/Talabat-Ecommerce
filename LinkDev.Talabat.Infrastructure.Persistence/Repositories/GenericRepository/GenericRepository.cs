using LinkDev.Talabat.Core.Domain.Common;
using LinkDev.Talabat.Core.Domain.Contract;
using LinkDev.Talabat.Core.Domain.Contract.Persistence;
using LinkDev.Talabat.Core.Domain.Entities.Products;
using LinkDev.Talabat.Infrastructure.Persistence._Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.Infrastructure.Persistence.Repositories.GenericRepository
{
    internal class GenericRepository<TEntity, TKey>(StoreDbContext _dbContext) : IGenericRepository<TEntity, TKey>
       where TEntity : BaseEntity<TKey>
       where TKey : IEquatable<TKey>
    {

        public async Task<IEnumerable<TEntity>> GetAllWithSpecAsync(ISpecifications<TEntity, TKey> spec, bool withTracking = false)
        {
            var query = ApplySpecificatinos(spec);
            if (!withTracking)
                query = query.AsNoTracking();
            return await query.ToListAsync();



        }



        public async Task<TEntity?> GetByIdWithSpecAsync(ISpecifications<TEntity, TKey> spec)
        {
            return await ApplySpecificatinos(spec)
                .FirstOrDefaultAsync();
        }
        public async Task AddAsync(TEntity entity)
        {
          await _dbContext.Set<TEntity>().AddAsync(entity);
        }

        public void Delete(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(bool withTracking = false) {
            //if (typeof(TEntity) == typeof(Product)) { 
            
            //    return withTracking? (IEnumerable<TEntity>) await _dbContext.Set<Product>().Include(s=>s.Brand).Include(s=>s.Category).ToListAsync():
            //       (IEnumerable<TEntity>) await _dbContext.Set<Product>().Include(s=>s.Brand).Include(s=>s.Category).AsNoTracking().ToListAsync();
            
            //}

            return withTracking? await _dbContext.Set<TEntity>().ToListAsync() : await _dbContext.Set<TEntity>().AsNoTracking().ToListAsync();
        }

       

        public async Task<TEntity?> GetByIdAsync(TKey id)
        {
            //if (typeof(TEntity)==typeof(Product)) {
                
            ////var products=  await _dbContext.Set<Product>().Include(s=>s.Brand).Include(s=>s.Category).FirstOrDefaultAsync(s =>s.Id!.Equals(id));
            ////    return (TEntity?)(object?)products;
            //return await _dbContext.Set<Product>().Where(s => s.Id!.Equals(id))
            //    .Include(s => s.Brand)
            //    .Include(s => s.Category)
            //    .FirstOrDefaultAsync() as TEntity;
            //}
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

    

        public void Update(TEntity entity)
        => _dbContext.Set<TEntity>().Update(entity);



        public async Task<int> GetCountAsync(ISpecifications<TEntity, TKey> spec)
        {
            return await ApplySpecificatinos(spec).CountAsync();
        }


        #region Helper
        private  IQueryable<TEntity> ApplySpecificatinos(ISpecifications<TEntity, TKey> specifications) {

            return   SpecificationsEvaluator<TEntity,TKey>.GetQuery(_dbContext.Set<TEntity>(), specifications);
        
        }

      
        #endregion




    }
}
