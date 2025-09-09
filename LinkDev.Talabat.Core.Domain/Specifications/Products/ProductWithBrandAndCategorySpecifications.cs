using LinkDev.Talabat.Core.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.Core.Domain.Specifications.Products
{
    public enum ProductSort
    {
        PriceAsc=1,
        PriceDesc,
        NameAsc,
        NameDesc
    }
    public class ProductWithBrandAndCategorySpecifications : BaseSpecifications<Product, int>
    {
        public ProductWithBrandAndCategorySpecifications(string?sort,int? brandId,int? categoryId,int pageSize,int pageIndex,string search) : base(
            
            p=>
            (string.IsNullOrEmpty(search)||p.NormalizedName.Contains(search))&&
            (!brandId.HasValue||p.BrandId==brandId.Value)
            && (!categoryId.HasValue || p.CategoryId == categoryId.Value)

            )
        {

            AddIncludes();

            switch (sort) { 
            
                case "priceAsc":
                    OrderByAsc(p => p.Price);
                    break;
                case "priceDesc":
                    OrderByDes(p => p.Price);
                    break;
                case "nameAsc":
                    OrderByAsc(p => p.Name);
                    break;
                case "nameDesc":
                    OrderByDes(p => p.Name);
                    break;
                default:
                    OrderByAsc(p => p.Name);
                    break;


            }
            ApplyPagination(pageSize *(pageIndex-1),pageSize);

        }
        public ProductWithBrandAndCategorySpecifications(int id) : base(id)
        {
            AddIncludes();
        }

        private protected override void AddIncludes()
        {
            base.AddIncludes();
            Includes.Add(p => p.Brand!);
            Includes.Add(p => p.Category!);
        }
    }
}
