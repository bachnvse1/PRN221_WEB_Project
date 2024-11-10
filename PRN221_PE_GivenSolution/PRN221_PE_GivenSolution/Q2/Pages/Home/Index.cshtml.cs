using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Q2.Hubs;
using Q2.Model;
using System.Linq;
using System.Text.Json;

namespace Q2.Pages.Home
{
    public class IndexModel : PageModel
    {
        private readonly DBContext _context;
        private readonly IHubContext<ProductsHub> hubContext;
        public IndexModel(IHubContext<ProductsHub> hubContext)
        {
            _context = new DBContext();
            this.hubContext = hubContext;
        }

        public string SearchQuery { get; set; }
        public string SelectedCategory { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
        public List<string> Categories { get; set; } = new List<string>();

        public void OnGet(string searchQuery, string selectedCategory, int pages = 1)
        {
            int pageSize = 8;
            int skip = (pages - 1) * pageSize;

            SearchQuery = searchQuery;
            SelectedCategory = selectedCategory;

            Categories = _context.Categories
                .Select(c => c.categoryName)
                .ToList();

            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(p => p.ProductName.Contains(searchQuery));
            }

            if (!string.IsNullOrEmpty(selectedCategory))
            {
                var category = _context.Categories.FirstOrDefault(c => c.categoryName == SelectedCategory);
                if (category != null)
                {
                    int categoryId = category.categoryId;
                    query = query.Where(p => p.CategoryId == categoryId);
                }
            }

            Products = query.Skip(skip).Take(pageSize).ToList();

            int totalProducts = query.Count();
            ViewData["TotalPages"] = (int)Math.Ceiling(totalProducts / (double)pageSize);
            ViewData["CurrentPage"] = pages;
        }

        public IActionResult OnGetProductDetails(int id)
        {
            var product = _context.Products.Where(x => x.ProductId == id).FirstOrDefault();
            if (product == null)
            {
                return NotFound();
            }
            return new JsonResult(product);
        }

        public IActionResult OnPostProductUpdate(int? productId, string productName, string productPrice, string productWeight, string productImage)
        {
            if (!productId.HasValue)
            {
                return BadRequest("Product ID is required");
            }

            var product = _context.Products.FirstOrDefault(x => x.ProductId == productId);
            if (product == null)
            {
                return NotFound();
            }

            decimal unitPrice = decimal.Parse(productPrice);
            decimal weight = decimal.Parse(productWeight);

            product.ProductName = productName;
            product.UnitPrice = unitPrice;
            product.Weight = weight;
            product.Image = productImage;

            _context.SaveChanges();
            return RedirectToPage("");
        }

        public IActionResult OnGetProductRemove(int? productId)
        {
            if (productId == null)
            {
                return new JsonResult(new { success = false, message = "Không tìm thấy sản phẩm." });
            }


            var orderDetails = _context.OrderDetails.Where(od => od.ProductId == productId).ToList();

            if (orderDetails.Any())
            {
                _context.OrderDetails.RemoveRange(orderDetails); 
                _context.SaveChanges(); 
            }

           
            var product = _context.Products.Where(x => x.ProductId == productId).FirstOrDefault();

            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges(); 
                hubContext.Clients.All.SendAsync("ProductDeleted", productId);
                return new JsonResult(new { success = true });
            }
            else
            {
                return new JsonResult(new { success = false, message = "Sản phẩm không tồn tại." });
            }
        }

        public IActionResult OnPostAddProduct(string productName, string productPrice, string productWeight, string productImage, string categoryName)
        {
            if (string.IsNullOrEmpty(productName) || string.IsNullOrEmpty(productPrice) || string.IsNullOrEmpty(productWeight))
            {
                return new JsonResult(new { success = false, message = "Tên sản phẩm, giá và cân nặng là bắt buộc." });
            }

            decimal unitPrice;
            decimal weight;
            if (!decimal.TryParse(productPrice, out unitPrice) || !decimal.TryParse(productWeight, out weight))
            {
                return new JsonResult(new { success = false, message = "Giá hoặc cân nặng không hợp lệ." });
            }

            var newProduct = new Product
            {
                ProductName = productName,
                UnitPrice = unitPrice,
                Weight = weight,
                Image = productImage,
                CategoryId = _context.Categories.FirstOrDefault(x => x.categoryName == categoryName).categoryId
            };

            _context.Products.Add(newProduct);
            _context.SaveChanges();

            return new JsonResult(new { success = true, message = "Sản phẩm đã được thêm thành công." });
        }

        public IActionResult OnGetAddToCart(int productId)
        {
            List<OrderDetail> orders = new List<OrderDetail>();
            if (HttpContext.Session.GetString("cart") != null)
            {
                string data = HttpContext.Session.GetString("cart");
                orders = JsonSerializer.Deserialize<List<OrderDetail>>(data);
            }
            else orders = new List<OrderDetail>();

            OrderDetail order = orders.FirstOrDefault(x => x.ProductId == productId);
            if (order != null)
            {
                order.Quantity++;
            }
            else
            {
                order = new OrderDetail();
                order.ProductId = productId;
                order.Quantity = 1;
                orders.Add(order);
            }

            HttpContext.Session.SetString("cart", JsonSerializer.Serialize(orders));

            return RedirectToPage("/Carts/CarList");
        }
    }
}
