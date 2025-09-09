using LinkDev.Talabat.Core.Domain.Contract.Persistence;
using LinkDev.Talabat.Core.Domain.Entities.Identity;
using LinkDev.Talabat.Infrastructure.Persistence._Common;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.Infrastructure.Persistence.Identity
{
    internal class StoreIdentityDbInitializer(StoreIdentityDbContext _dbContext, UserManager<ApplicationUser> _userManager)
      : DbInitializer(_dbContext), IStoreIdentityDbInitializer
    {
        public override async Task SeedAsync()
        {
            if (!_userManager.Users.Any())
            {
                var user = new ApplicationUser()
                {
                    DisplayName = "Shadow",
                    UserName = "MD10",
                    Email = "mo@mo.mo",
                    PhoneNumber = "01551413742"
                };

                await _userManager.CreateAsync(user, "P@ssw0rd");
            }

        }
    }
}
