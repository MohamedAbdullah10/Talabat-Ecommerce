using LinkDev.Talabat.APIs.Controllers.Controllers.Base;
using LinkDev.Talabat.APIs.Controllers.Filters;
using LinkDev.Talabat.Core.Application.Abstraction.Common;
using LinkDev.Talabat.Core.Application.Abstraction.Models.Products;
using LinkDev.Talabat.Core.Application.Abstraction.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.APIs.Controllers.Controllers.Products
{
    public class ProductsController(IServiceManager serviceManager) : BaseApiController
    {
        [Cached(600)]
        [HttpGet]
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery]ProductSpecParams specParams)
        {

            var products=await serviceManager.productService.GetProductsAsync(specParams);


            return Ok(products);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var product = await serviceManager.productService.GetProductByIdAsync(id);
            if (product is null)
                return NotFound();
            return Ok(product);
        }
        [HttpGet("categories")]
        public async Task<ActionResult<CategoryDto>> GetCategories()
        {
            var categories = await serviceManager.productService.GetCategoriesAsync();
            return Ok(categories);
        }
        [HttpGet("brands")]
        public async Task<ActionResult<BrandDto>> GetBrands()
        {
            var brands = await serviceManager.productService.GetBrandsAsync();
            return Ok(brands);
        }

    }
}
