using LinkDev.Talabat.Core.Domain.Common;
using LinkDev.Talabat.Core.Domain.Entities.Products;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.Infrastructure.Persistence.Data.Config.Base
{
    internal class BaseAuditableEntityConfigurations<TEntity, TKey> : BaseEntityConfigurations<TEntity, TKey>
        where TEntity : BaseAuditableEntity<TKey> where TKey : IEquatable<TKey>
    {
        public override void Configure(EntityTypeBuilder<TEntity> builder)
        {
            base.Configure(builder);

            builder.Property(E => E.CreatedBy)
                .IsRequired(false);

            builder.Property(E => E.CreatedOn)
                .IsRequired()
                /*.HasDefaultValueSql("GETUTCDate()")*/;


            builder.Property(E => E.LastModifiedBy)
                .IsRequired(false);

            builder.Property(E => E.LastModifiedOn)
                .IsRequired()
                /*.HasDefaultValueSql("GETUTCDate()")*/;

        }
    }
}
