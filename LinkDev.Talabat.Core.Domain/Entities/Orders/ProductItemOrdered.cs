using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.Core.Domain.Entities.Orders
{
  
    public class ProductItemOrdered
    {
        public ProductItemOrdered()
        {

        }
        public ProductItemOrdered(int id, string name, string? pictureUrl)
        {
            ProductId = id;
            ProductName = name;
            PictureUrl = pictureUrl;
        }

      
    
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? PictureUrl { get; set; }
    }
}
