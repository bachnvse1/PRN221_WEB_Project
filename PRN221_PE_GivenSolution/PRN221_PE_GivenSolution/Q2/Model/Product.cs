using System;
using System.Collections.Generic;

namespace Q2.Model
{
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int ProductId { get; set; }
        public int? CategoryId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal? Weight { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? UnitsInStock { get; set; }
        public string? Image { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual Category Category { get; set; } // Điều này định nghĩa mối quan hệ
    }
}
