using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.Core.Application.Abstraction

{
    public class BasketItemDto
    {
        public int Id { get; set; }
        public required string ProductName { get; set; }
        public string? PictureUrl { get; set; }

        [Range(.1, double.MaxValue, ErrorMessage = "Price must be greater than zero!!")]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least ont item")]
        public int Quantity { get; set; }
        public string? Brand { get; set; }
        public string? Category { get; set; }
    }
}
