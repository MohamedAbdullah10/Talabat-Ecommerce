using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.Core.Domain.Contract.Persistence
{
    public interface IDbInitializer
    {
        Task InitializeAsync();
        Task SeedAsync();
    }
}
