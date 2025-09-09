using LinkDev.Talabat.Core.Domain.Contract.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.Infrastructure.Persistence._Common
{
    internal abstract class DbInitializer(DbContext _dbContext) : IDbInitializer
    {
        public async Task InitializeAsync()
        {
            var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
                await _dbContext.Database.MigrateAsync(); // Update-Database
        }

        public abstract Task SeedAsync();
    }
}
