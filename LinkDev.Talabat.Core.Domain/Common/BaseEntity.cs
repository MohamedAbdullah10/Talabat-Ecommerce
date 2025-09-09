using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.Core.Domain.Common
{
    public  class BaseEntity<TKey> where TKey :IEquatable<TKey>
    {
        public TKey? Id { get; set; } //req mean dont allow to inialize prop with null must give it value when  intialization
      


    }
}
